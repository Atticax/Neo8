using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Foundatio.Messaging;
using Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Netsphere.Common.Configuration;
using Netsphere.Common.Messaging;
using Netsphere.Database;
using Netsphere.Database.Game;
using Netsphere.Network.Message.Club;
using Netsphere.Network.Message.Game;
using Netsphere.Network.Message.GameRule;
using Netsphere.Server.Game.Services;
using Z.EntityFramework.Plus;

namespace Netsphere.Server.Game
{
    public class ClanManager : IHostedService, IReadOnlyCollection<Clan>
    {
        private readonly ILogger _logger;
        private readonly DatabaseService _databaseService;
        private readonly IOptionsMonitor<ClanOptions> _clanOptions;
        private readonly IServiceProvider _serviceProvider;
        private Dictionary<uint, Clan> _clans;

        public Clan this[uint id] => GetClan(id);
        public Clan this[string name] => GetClan(name);

        public ClanManager(ILogger<ClanManager> logger, DatabaseService databaseService,
            IOptionsMonitor<ClanOptions> clanOptions, PlayerManager playerManager, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _databaseService = databaseService;
            _clanOptions = clanOptions;
            _serviceProvider = serviceProvider;

            playerManager.PlayerConnected += OnPlayerConnected;
            playerManager.PlayerDisconnected += OnPlayerDisconnected;
        }

        public Clan GetClan(uint id)
        {
            return _clans.GetValueOrDefault(id);
        }

        public Clan GetClan(string name)
        {
            return _clans.Values.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public ClubNameCheckResult CheckClanName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return ClubNameCheckResult.CannotBeUsed;

            var clanNameRestrictions = _clanOptions.CurrentValue;
            if (name.Length < clanNameRestrictions.NameMinLength)
                return ClubNameCheckResult.TooShort;

            if (name.Length > clanNameRestrictions.NameMaxLength)
                return ClubNameCheckResult.TooLong;

            if (GetClan(name) != null)
                return ClubNameCheckResult.NotAvailable;

            return ClubNameCheckResult.Available;
        }

        public async Task<(Clan, ClanCreateError)> CreateClan(Player plr, string name, string description,
            ClubArea area, ClubActivity activity,
            string question1, string question2, string question3, string question4, string question5)
        {
            if (plr.Clan != null)
                return (null, ClanCreateError.AlreadyInClan);

            if (CheckClanName(name) != ClubNameCheckResult.Available)
                return (null, ClanCreateError.NameAlreadyExists);

            Clan clan;
            using (var db = _databaseService.Open<GameContext>())
            {
                var clanEntity = new ClanEntity
                {
                    OwnerId = (int)plr.Account.Id,
                    CreationDate = DateTimeOffset.Now.ToUnixTimeSeconds(),
                    Icon = _clanOptions.CurrentValue.DefaultIcon,
                    Name = name,
                    Description = description,
                    Area = (byte)area,
                    Activity = (byte)activity,
                    Question1 = question1,
                    Question2 = question2,
                    Question3 = question3,
                    Question4 = question4,
                    Question5 = question5
                };
                var clanMemberEntity = new ClanMemberEntity
                {
                    ClanId = clanEntity.Id,
                    PlayerId = (int)plr.Account.Id,
                    JoinDate = DateTimeOffset.Now.ToUnixTimeSeconds(),
                    State = (byte)ClubMemberState.Joined,
                    Role = (byte)ClubRole.Master,
                    LastLoginDate = DateTimeOffset.Now.ToUnixTimeSeconds()
                };

                db.Clans.Add(clanEntity);
                clanEntity.Members.Add(clanMemberEntity);
                await db.SaveChangesAsync();

                clan = _serviceProvider.GetRequiredService<Clan>();
                clan.Initialize(this, clanEntity, new[]
                {
                    clanMemberEntity
                });
                _clans.Add(clan.Id, clan);
            }

            plr.Clan = clan;
            plr.ClanMember.Player = plr;
            plr.SendClubInfo();
            return (clan, ClanCreateError.None);
        }

        public Task CloseClan(uint clanId)
        {
            var clan = GetClan(clanId);
            return clan == null ? Task.CompletedTask : CloseClan(clan);
        }

        public async Task CloseClan(Clan clan)
        {
            if (clan.Members.Count() > 1)
                return;

            using (var db = _databaseService.Open<GameContext>())
            {
                db.Clans.Remove(new ClanEntity
                {
                    Id = (int)clan.Id
                });
                await db.SaveChangesAsync();
            }

            _clans.Remove(clan.Id);

            if (clan.Owner.Player != null)
            {
                clan.Owner.Player.Clan = null;
                clan.Owner.Player.SendClubInfo();
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Loading clans...");

            using (var db = _databaseService.Open<GameContext>())
            {
                var clanEntities = await db.Clans
                    .Include(x => x.Members)
                    .Include(x => x.Bans)
                    .Include(x => x.Events)
                    .ToArrayAsync();

                var clans = new List<Clan>();
                foreach (var clanEntity in clanEntities)
                {
                    var clan = _serviceProvider.GetRequiredService<Clan>();
                    clan.Initialize(
                        this,
                        clanEntity,
                        clanEntity.Members
                    );
                    clans.Add(clan);
                }

                _clans = clans.ToDictionary(x => x.Id, x => x);
            }

            _logger.Information("Loaded {Count} clans", _clans.Count);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void OnPlayerConnected(object sender, PlayerEventArgs e)
        {
            var plr = e.Player;
            var member = plr.ClanMember;
            if (member != null)
            {
                member.Player = plr;
                member.LastLogin = DateTimeOffset.Now;
                using (var db = _databaseService.Open<GameContext>())
                {
                    db.ClanMembers.Where(x => x.Id == member.Id).Update(x => new ClanMemberEntity
                    {
                        LastLoginDate = member.LastLogin.ToUnixTimeSeconds()
                    });
                }

                plr.Clan.OnMemberConnected(member);
            }
        }

        private static void OnPlayerDisconnected(object sender, PlayerEventArgs e)
        {
            var plr = e.Player;
            var member = plr.ClanMember;
            if (member != null)
            {
                member.Player = null;
                plr.Clan.OnMemberDisconnected(member);
            }
        }

        #region IReadOnlyCollection
        public int Count => _clans.Count;

        public IEnumerator<Clan> GetEnumerator()
        {
            return _clans.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }

    public class Clan : IReadOnlyCollection<ClanMember>
    {
        private readonly DatabaseService _databaseService;
        private readonly NicknameLookupService _nicknameLookupService;
        private readonly IMessageBus _messageBus;
        private readonly Dictionary<ulong, ClanMember> _members;
        private readonly HashSet<ulong> _bans;
        private readonly List<ClanEventEntry> _events;
        private ulong _ownerId;

        public ClanMember this[ulong id] => GetMember(id);
        public IEnumerable<ClanMember> Members => _members.Values;
        public IReadOnlyList<ClanEventEntry> Events => _events;
        public IReadOnlyCollection<ulong> Bans => _bans;

        public event EventHandler<ClanMemberEventArgs> MemberConnected;
        public event EventHandler<ClanMemberEventArgs> MemberDisconnected;
        public event EventHandler<ClanMemberEventArgs> MemberJoined;
        public event EventHandler<ClanMemberEventArgs> MemberLeft;

        internal void OnMemberConnected(ClanMember member)
        {
            _messageBus.PublishAsync(new ClanMemberUpdateMessage(
                Id,
                member.AccountId,
                ClubMemberPresenceState.Online,
                true
            ));
            MemberConnected?.Invoke(this, new ClanMemberEventArgs(member));
        }

        internal void OnMemberDisconnected(ClanMember member)
        {
            _messageBus.PublishAsync(new ClanMemberUpdateMessage(
                Id,
                member.AccountId,
                ClubMemberPresenceState.Offline
            ));
            MemberDisconnected?.Invoke(this, new ClanMemberEventArgs(member));
        }

        private void OnMemberJoined(ClanMember member, bool isLoggedIn = false)
        {
            MessagePublisherExtensions.PublishAsync(_messageBus, new ClanMemberUpdateMessage(Id, member.AccountId, ClubMemberPresenceState.Online, isLoggedIn), new TimeSpan?());
            var memberJoined = MemberJoined;
            if (memberJoined == null)
                return;
            memberJoined(this, new ClanMemberEventArgs(member));
        }

        //private void OnMemberJoined(ClanMember member)
        //{
        //    _messageBus.PublishAsync(new ClanMemberUpdateMessage(
        //        Id,
        //        member.AccountId,
        //        ClubMemberPresenceState.Online
        //    ));
        //    MemberJoined?.Invoke(this, new ClanMemberEventArgs(member));
        //}

        private void OnMemberLeft(ClanMember member)
        {
            MemberLeft?.Invoke(this, new ClanMemberEventArgs(member));
        }

        public ClanManager ClanManager { get; private set; }
        public uint Id { get; private set; }
        public DateTimeOffset CreationDate { get; private set; }
        public string Name { get; private set; }
        public string Icon { get; private set; }
        public string Description { get; private set; }
        public ClubArea Area { get; private set; }
        public ClubActivity Activity { get; private set; }
        public ClubClass Class { get; private set; }
        public bool IsPublic { get; internal set; }
        public byte RequiredLevel { get; internal set; }
        public string Question1 { get; internal set; }
        public string Question2 { get; internal set; }
        public string Question3 { get; internal set; }
        public string Question4 { get; internal set; }
        public string Question5 { get; internal set; }
        public string Announcement { get; internal set; }
        public ClanMember Owner => GetMember(_ownerId);

        public Clan(DatabaseService databaseService, NicknameLookupService nicknameLookupService, IMessageBus messageBus)
        {
            _databaseService = databaseService;
            _nicknameLookupService = nicknameLookupService;
            _messageBus = messageBus;
            _members = new Dictionary<ulong, ClanMember>();
            _bans = new HashSet<ulong>();
            _events = new List<ClanEventEntry>();
        }

        internal void Initialize(ClanManager clanManager, ClanEntity entity,
            IEnumerable<ClanMemberEntity> members)
        {
            ClanManager = clanManager;
            Id = (uint)entity.Id;
            CreationDate = DateTimeOffset.FromUnixTimeSeconds(entity.CreationDate);
            Name = entity.Name;
            Icon = entity.Icon;
            Description = entity.Description;
            Area = (ClubArea)entity.Area;
            Activity = (ClubActivity)entity.Activity;
            Class = (ClubClass)entity.Class;
            IsPublic = entity.IsPublic;
            RequiredLevel = entity.RequiredLevel;
            Question1 = entity.Question1;
            Question2 = entity.Question2;
            Question3 = entity.Question3;
            Question4 = entity.Question4;
            Question5 = entity.Question5;
            Announcement = entity.Announcement;
            _ownerId = (ulong)entity.OwnerId;

            foreach (var ban in entity.Bans.Select(x => (ulong)x.PlayerId))
                _bans.Add(ban);

            foreach (var eventEntity in entity.Events)
            {
                _events.Add(new ClanEventEntry(
                    (ulong)eventEntity.PlayerId,
                    (ClanEvent)eventEntity.Type,
                    DateTimeOffset.FromUnixTimeSeconds(eventEntity.Date),
                    eventEntity.Value1
                ));
            }

            foreach (var memberEntity in members)
                _members[(ulong)memberEntity.PlayerId] = new ClanMember(this, memberEntity, _nicknameLookupService);
        }

        public async Task<Network.Message.Club.ClubInfoAckMessage> GetClubInfo()
        {
            return new Network.Message.Club.ClubInfoAckMessage
            {
                ClanId = Id,
                ClanIcon = Icon,
                ClanName = Name,
                MemberCount = this.Count(x => x.State == ClubMemberState.Joined),
                OwnerName = await _nicknameLookupService.GetNicknameAsync(Owner.AccountId),
                CreationDate = CreationDate,
                Area = Area,
                Activity = Activity,
                Class = Class,
                Description = Description,
                Announcement = Announcement
            };
        }

        public async Task<ClubClubInfoAck2Message> GetClubInfo2()
        {
            var clubInfoAck2Message1 = new ClubClubInfoAck2Message();
            clubInfoAck2Message1.ClanId = (int)Id;
            clubInfoAck2Message1.ClanIcon = Icon;
            clubInfoAck2Message1.ClanName = Name;
            clubInfoAck2Message1.CreationDate = CreationDate;
            clubInfoAck2Message1.PlayersCount = this.Count(x => x.State == ClubMemberState.Joined);
            var clubInfoAck2Message2 = clubInfoAck2Message1;
            clubInfoAck2Message2.OwnerName = await _nicknameLookupService.GetNicknameAsync(Owner.AccountId);
            return clubInfoAck2Message1;
        }

        public ClanMember GetMember(ulong id)
        {
            return _members.GetValueOrDefault(id);
        }

        public ClanMember GetMember(Player plr)
        {
            return GetMember(plr.Account.Id);
        }

        public async Task<ClubJoinResult> JoinInvite(Player plr)
        {
            var clan = this;
            if (plr.Clan != null)
                return ClubJoinResult.AlreadyRegistered;
            if ((int)clan.RequiredLevel > (int)plr.Level)
                return ClubJoinResult.LevelRequirementNotMet;
            if (clan._bans.Contains(plr.Account.Id))
                return ClubJoinResult.CantRegister;
            if (!(await Common.MessageBusExtensions.PublishRequestAsync<ClanAdminJoinRequest, ClanAdminJoinResponse>(clan._messageBus, new ClanAdminJoinRequest(plr.Account.Id, "<Note Key=\"3\" Cnt=\"1\" Param1=\"" + clan.Name + "\"/>"))).Result)
                return ClubJoinResult.Failed;
            var clanMemberEntity = new ClanMemberEntity();
            clanMemberEntity.ClanId = (int)clan.Id;
            clanMemberEntity.PlayerId = (int)plr.Account.Id;
            var now = DateTimeOffset.Now;
            clanMemberEntity.JoinDate = now.ToUnixTimeSeconds();
            clanMemberEntity.State = clan.IsPublic ? (byte)2 : (byte)1;
            clanMemberEntity.Role = 5;
            now = DateTimeOffset.Now;
            clanMemberEntity.LastLoginDate = now.ToUnixTimeSeconds();
            var memberEntity = clanMemberEntity;
            using (var db = clan._databaseService.Open<GameContext>())
            {
                db.ClanMembers.Add(memberEntity);
                clan.AddEvent(db, clan.IsPublic ? ClanEvent.Join : ClanEvent.Register, plr.Account.Id);
                int num = await db.SaveChangesAsync(new CancellationToken());
            }
            clan._members.Add(plr.Account.Id, new ClanMember(clan, memberEntity, clan._nicknameLookupService));
            plr.Clan = clan;
            plr.ClanMember.Player = plr;
            plr.SendClubInfo();
            if (!clan.IsPublic)
                return ClubJoinResult.Registered;
            clan.OnMemberJoined(plr.ClanMember, true);
            return ClubJoinResult.Joined;
        }

        public async Task<ClubJoinResult> Join(Player plr,
            string answer1, string answer2, string answer3, string answer4, string answer5)
        {
            if (plr.Clan != null)
                return ClubJoinResult.AlreadyRegistered;

            if (RequiredLevel > plr.Level)
                return ClubJoinResult.LevelRequirementNotMet;

            if (_bans.Contains(plr.Account.Id))
                return ClubJoinResult.CantRegister;

            var memberEntity = new ClanMemberEntity
            {
                ClanId = (int)Id,
                PlayerId = (int)plr.Account.Id,
                JoinDate = DateTimeOffset.Now.ToUnixTimeSeconds(),
                State = (byte)(IsPublic ? ClubMemberState.Joined : ClubMemberState.JoinRequested),
                Role = (byte)ClubRole.Normal,
                LastLoginDate = DateTimeOffset.Now.ToUnixTimeSeconds(),
                Answer1 = answer1,
                Answer2 = answer2,
                Answer3 = answer3,
                Answer4 = answer4,
                Answer5 = answer5
            };

            using (var db = _databaseService.Open<GameContext>())
            {
                db.ClanMembers.Add(memberEntity);
                AddEvent(db, IsPublic ? ClanEvent.Join : ClanEvent.Register, plr.Account.Id);
                await db.SaveChangesAsync();
            }

            _members.Add(plr.Account.Id, new ClanMember(this ,memberEntity, _nicknameLookupService));
            plr.Clan = this;
            plr.ClanMember.Player = plr;
            plr.SendClubInfo();

            if (!IsPublic)
                return ClubJoinResult.Registered;

            OnMemberJoined(plr.ClanMember);
            return ClubJoinResult.Joined;
        }

        public async Task<bool> Leave(Player plr)
        {
            if (Count == 1)
                return false;

            var member = GetMember(plr);
            if (member == null)
                return false;

            using (var db = _databaseService.Open<GameContext>())
            {
                db.ClanMembers.Remove(new ClanMemberEntity
                {
                    Id = member.Id
                });
                AddEvent(db, ClanEvent.Leave, plr.Account.Id);
                await db.SaveChangesAsync();
            }

            _members.Remove(member.AccountId);
            plr.Clan = null;
            OnMemberLeft(member);
            return true;
        }

        public async Task<ClubAdminInviteResult> Invite(Player moderator, Player player)
        {
            return player.Clan != null || GetMember(player.Account.Id) != null ? ClubAdminInviteResult.NoMatchFound : ((await Common.MessageBusExtensions.PublishRequestAsync<PlayerMailboxRequest, PlayerMailboxResponse>(_messageBus, new PlayerMailboxRequest(moderator.Account.Nickname, player.Account.Nickname, "<Note Key=\"3\" Cnt=\"1\" Param1=\"" + Name + "\"/>", string.Format("<Note Key=\"4\" Srl=\"{0}\" Cnt=\"2\" Param1=\"{1}\" Param2=\"{2}\"/>", Id, Name, moderator.Account.Nickname), MailType.Club))).MailId != 0 ? ClubAdminInviteResult.Success : ClubAdminInviteResult.NoMatchFound);
        }

        public async Task<ClubCommandResult> Approve(Player moderator, ulong accountId)
        {
            var member = GetMember(accountId);
            if (member == null)
                return ClubCommandResult.MemberNotFound;

            if (member.State != ClubMemberState.JoinRequested)
                return ClubCommandResult.MemberNotFound;

            member.State = ClubMemberState.Joined;
            using (var db = _databaseService.Open<GameContext>())
            {
                await db.ClanMembers.Where(x => x.Id == member.Id).UpdateAsync(x => new ClanMemberEntity
                {
                    State = (byte)member.State
                });
                AddEvent(db, ClanEvent.Approve, moderator.Account.Id, (long)accountId);
                await db.SaveChangesAsync();
            }

            member.Player?.SendClubInfo();
            OnMemberJoined(member);
            return ClubCommandResult.Success;
        }

        public async Task<ClubCommandResult> Decline(Player moderator, ulong accountId)
        {
            var member = GetMember(accountId);
            if (member == null)
                return ClubCommandResult.MemberNotFound;

            if (member.State != ClubMemberState.JoinRequested)
                return ClubCommandResult.MemberNotFound;

            using (var db = _databaseService.Open<GameContext>())
            {
                db.ClanMembers.Remove(new ClanMemberEntity
                {
                    Id = member.Id
                });
                AddEvent(db, ClanEvent.Decline, moderator.Account.Id, (long)accountId);
                await db.SaveChangesAsync();
            }

            _members.Remove(member.AccountId);
            if (member.Player != null)
            {
                member.Player.Clan = null;
                member.Player.SendClubInfo();
            }

            return ClubCommandResult.Success;
        }

        public async Task<ClubCommandResult> Kick(Player moderator, ulong accountId)
        {
            var member = GetMember(accountId);
            if (member == null)
                return ClubCommandResult.MemberNotFound;

            if (member.State != ClubMemberState.Joined)
                return ClubCommandResult.MemberNotFound;

            using (var db = _databaseService.Open<GameContext>())
            {
                db.ClanMembers.Remove(new ClanMemberEntity
                {
                    Id = member.Id
                });
                AddEvent(db, ClanEvent.Kick, moderator.Account.Id, (long)accountId);
                await db.SaveChangesAsync();
            }

            _members.Remove(member.AccountId);
            if (member.Player != null)
            {
                member.Player.Clan = null;
                member.Player.SendClubInfo();
            }

            return ClubCommandResult.Success;
        }

        public async Task<ClubCommandResult> Ban(Player moderator, ulong accountId)
        {
            var member = GetMember(accountId);
            if (member == null)
                return ClubCommandResult.MemberNotFound;

            if (member.State != ClubMemberState.Joined)
                return ClubCommandResult.MemberNotFound;

            using (var db = _databaseService.Open<GameContext>())
            {
                db.ClanMembers.Remove(new ClanMemberEntity
                {
                    Id = member.Id
                });
                db.ClanBans.Add(new ClanBanEntity
                {
                    ClanId = (int)Id, PlayerId = (int)accountId, Date = DateTimeOffset.Now.ToUnixTimeSeconds()
                });
                AddEvent(db, ClanEvent.Ban, moderator.Account.Id, (long)accountId);
                await db.SaveChangesAsync();
            }

            _bans.Add(accountId);
            _members.Remove(member.AccountId);
            if (member.Player != null)
            {
                member.Player.Clan = null;
                member.Player.SendClubInfo();
            }

            return ClubCommandResult.Success;
        }

        public async Task<ClubCommandResult> Unban(Player moderator, ulong accountId)
        {
            if (!_bans.Contains(accountId))
                return ClubCommandResult.MemberNotFound;

            using (var db = _databaseService.Open<GameContext>())
            {
                var clanId = (int)Id;
                var id = (int)accountId;
                await db.ClanBans.Where(x => x.ClanId == clanId && x.PlayerId == id).DeleteAsync();
                AddEvent(db, ClanEvent.Unban, moderator.Account.Id, (long)accountId);
                await db.SaveChangesAsync();
            }

            _bans.Remove(accountId);
            return ClubCommandResult.Success;
        }

        public async Task ChangeAnnouncement(string announcement)
        {
            using (var db = _databaseService.Open<GameContext>())
            {
                var id = (int)Id;
                await db.Clans.Where(x => x.Id == id).UpdateAsync(x => new ClanEntity
                {
                    Announcement = announcement
                });
                Announcement = announcement;
            }
        }

        public async Task ChangeInfo(ClubArea area, ClubActivity activity, string description)
        {
            using (var db = _databaseService.Open<GameContext>())
            {
                var id = (int)Id;
                await db.Clans.Where(x => x.Id == id).UpdateAsync(x => new ClanEntity
                {
                    Area = (byte)area, Activity = (byte)activity, Description = description
                });
                Area = area;
                Activity = activity;
                Description = description;
            }
        }

        public async Task ChangeJoinCondition(bool isPublic, byte requiredLevel,
            string question1, string question2, string question3, string question4, string question5)
        {
            using (var db = _databaseService.Open<GameContext>())
            {
                var id = (int)Id;
                await db.Clans.Where(x => x.Id == id).UpdateAsync(x => new ClanEntity
                {
                    IsPublic = isPublic,
                    RequiredLevel = requiredLevel,
                    Question1 = question1,
                    Question2 = question2,
                    Question3 = question3,
                    Question4 = question4,
                    Question5 = question5,
                });
                IsPublic = isPublic;
                RequiredLevel = requiredLevel;
                Question1 = question1;
                Question2 = question2;
                Question3 = question3;
                Question4 = question4;
                Question5 = question5;
            }
        }

        public async Task ChangeRole(ClanMember member, ClubRole role)
        {
            using (var db = _databaseService.Open<GameContext>())
            {
                await db.ClanMembers.Where(x => x.Id == member.Id).UpdateAsync(x => new ClanMemberEntity
                {
                    Role = (byte)role
                });
                member.Role = role;
            }
        }

        public Task Close()
        {
            return ClanManager.CloseClan(this);
        }

        public async Task Broadcast(IGameMessage message)
        {
            foreach (var session in Members.Where(x => x.Player != null).Select(x => x.Player.Session))
                session.Send(message);
        }

        public async Task Broadcast(IGameRuleMessage message)
        {
            foreach (var session in Members.Where(x => x.Player != null).Select(x => x.Player.Session))
                session.Send(message);
        }

        public async Task Broadcast(IClubMessage message)
        {
            foreach (var session in Members.Where(x => x.Player != null).Select(x => x.Player.Session))
                session.Send(message);
        }

        private void AddEvent(GameContext db, ClanEvent clanEvent, ulong accountId, long value1 = 0)
        {
            var date = DateTimeOffset.Now;
            _events.Add(new ClanEventEntry(accountId, clanEvent, date, value1));
            db.ClanEvents.Add(new ClanEventEntity
            {
                ClanId = (int)Id,
                PlayerId = (int)accountId,
                Type = (byte)clanEvent,
                Value1 = value1,
                Date = date.ToUnixTimeSeconds()
            });
        }

        #region IReadOnlyCollection
        public int Count => _members.Count;

        public IEnumerator<ClanMember> GetEnumerator()
        {
            return _members.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }

    public class ClanMember
    {
        private readonly NicknameLookupService _nicknameLookupService;

        public Clan Clan { get; }
        public int Id { get; }
        public DateTimeOffset JoinDate { get; }
        public ClubMemberState State { get; internal set; }
        public ClubRole Role { get; internal set; }
        public ulong AccountId { get; }
        public string Name => _nicknameLookupService.GetNickname(AccountId);
        public Player Player { get; internal set; }
        public DateTimeOffset LastLogin { get; internal set; }
        public string Answer1 { get; }
        public string Answer2 { get; }
        public string Answer3 { get; }
        public string Answer4 { get; }
        public string Answer5 { get; }

        public ClanMember(Clan clan, ClanMemberEntity entity, NicknameLookupService nicknameLookupService)
        {
            Clan = clan;
            _nicknameLookupService = nicknameLookupService;
            Id = entity.Id;
            JoinDate = DateTimeOffset.FromUnixTimeSeconds(entity.JoinDate);
            State = (ClubMemberState)entity.State;
            Role = (ClubRole)entity.Role;
            AccountId = (ulong)entity.PlayerId;
            LastLogin = DateTimeOffset.FromUnixTimeSeconds(entity.LastLoginDate);
            Answer1 = entity.Answer1;
            Answer2 = entity.Answer2;
            Answer3 = entity.Answer3;
            Answer4 = entity.Answer4;
            Answer5 = entity.Answer5;
        }

        public async Task ChangeRole(ClubRole role)
        {
            await Clan.ChangeRole(this, role);
        }
    }

    public class ClanEventEntry
    {
        public ulong AccountId { get; }
        public ClanEvent Event { get; }
        public long Value1 { get; }
        public DateTimeOffset Date { get; }

        public ClanEventEntry(ulong accountId, ClanEvent @event, DateTimeOffset date, long value1)
        {
            AccountId = accountId;
            Event = @event;
            Date = date;
            Value1 = value1;
        }
    }
}
