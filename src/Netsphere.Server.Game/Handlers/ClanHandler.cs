using System;
using System.Linq;
using System.Threading.Tasks;
using ExpressMapper.Extensions;
using Logging;
using Netsphere.Network;
using Netsphere.Network.Data.Club;
using Netsphere.Network.Message.Club;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Rules;
using Netsphere.Server.Game.Services;
using ProudNet;

namespace Netsphere.Server.Game.Handlers
{
    internal class ClanHandler:
        IHandle<ClubAdminBoardModifyReqMessage>, IHandle<ClubAdminInviteReqMessage>, IHandle<ClubAdminMasterChangeReqMessage>,
        IHandle<ClubAdminSubMasterCancelReqMessage>, IHandle<ClubAdminSubMasterReqMessage>, IHandle<ClubSearchReqMessage>,
        IHandle<Network.Message.Club.ClubInfoReqMessage>, IHandle<ClubNameCheckReqMessage>, IHandle<ClubCreateReqMessage>,
        IHandle<ClubCloseReqMessage>, IHandle<ClubJoinConditionInfoReqMessage>, IHandle<Network.Message.Club.ClubJoinReqMessage>,
        IHandle<ClubUnjoinReqMessage>, IHandle<ClubJoinWaiterInfoReqMessage>, IHandle<ClubAdminJoinCommandReqMessage>,
        IHandle<ClubNewJoinMemberInfoReqMessage>, IHandle<ClubUnjoinerListReqMessage>, IHandle<ClubAdminNoticeChangeReqMessage>,
        IHandle<ClubAdminInfoModifyReqMessage>, IHandle<ClubAdminJoinConditionModifyReqMessage>, IHandle<ClubAdminGradeChangeReqMessage>,
        IHandle<ClubBoardDeleteAllReqMessage>, IHandle<ClubBoardDeleteReqMessage>, IHandle<ClubBoardModifyReqMessage>,
        IHandle<ClubBoardReadMineReqMessage>, IHandle<ClubBoardReadOtherClubReqMessage>, IHandle<ClubBoardReadReqMessage>,
        IHandle<ClubBoardSearchNickReqMessage>, IHandle<ClubBoardWriteReqMessage>, IHandle<ClubCloseReq2Message>,
        IHandle<ClubClubInfoReq2Message>, IHandle<ClubCreateReq2Message>, IHandle<ClubEditIntroduceReqMessage>,
        IHandle<ClubEditURLReqMessage>, IHandle<ClubGradeCountReqMessage>, IHandle<ClubJoinReq2Message>,
        IHandle<ClubNewsInfoReqMessage>, IHandle<ClubRankListReqMessage>, IHandle<ClubRestoreReqMessage>,
        IHandle<ClubRestoreReq2Message>, IHandle<ClubSearchReq2Message>, IHandle<ClubStuffListReqMessage>,
        IHandle<ClubStuffListReq2Message>, IHandle<ClubUnjoinReq2Message>, IHandle<ClubUnjoinSettingMemberListReqMessage>
    {
        private readonly ILogger _logger;
        private readonly PlayerManager _playerManager;
        private readonly ClanManager _clanManager;
        private readonly NicknameLookupService _nicknameLookupService;

        public ClanHandler(ILogger<ClanHandler> logger, ClanManager clanManager, NicknameLookupService nicknameLookupService)
        {
            _logger = logger;
            _clanManager = clanManager;
            _nicknameLookupService = nicknameLookupService;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubUnjoinReq2Message message)
        {
            var session = context.GetSession<Session>();
            var player = session.Player;
            session.Send(new ClubUnjoinAck2Message(await player.Clan.Leave(player) ? ClubLeaveResult.Success : ClubLeaveResult.Failed));
            bool flag = true;
            session = null;
            return flag;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(
          MessageContext context,
          ClubUnjoinSettingMemberListReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ClubUnjoinSettingMemberListAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubStuffListReq2Message message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ClubStuffListAck2Message());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubStuffListReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ClubStuffListAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubSearchReq2Message message)
        {
            var session = context.GetSession<Session>();
            var array = _clanManager.Where(x => x.Name.Contains(message.ClanName, StringComparison.OrdinalIgnoreCase)).Select(x => new ClubInfo2Dto()
            {
                ClanId = (int)x.Id,
                ClanIcon = x.Icon,
                ClanName = x.Name,
                CreationDate = x.CreationDate,
                OwnerName = x.Owner.Name,
                PlayersCount = x.Count(i => i.State == ClubMemberState.Joined)
            }).ToArray();
            var searchAck2Message = new ClubSearchAck2Message(array.Length, array);
            session.Send(searchAck2Message);
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubRankListReqMessage message)
        {
            var session = context.GetSession<Session>();
            var array = _clanManager.Select(x => new ClubInfo2Dto()
            {
                ClanId = (int)x.Id,
                ClanIcon = x.Icon,
                ClanName = x.Name,
                CreationDate = x.CreationDate,
                OwnerName = x.Owner.Name,
                PlayersCount = x.Count(i => i.State == ClubMemberState.Joined)
            }).ToArray();
            var rankListAckMessage = new ClubRankListAckMessage(array.Length, array);
            session.Send(rankListAckMessage);
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubRestoreReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ClubRestoreAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubRestoreReq2Message message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ClubRestoreAck2Message());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubNewsInfoReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ClubNewsInfoAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { }, Invert = true)]
        public async Task<bool> OnHandle(MessageContext context, ClubJoinReq2Message message)
        {
            var session = context.GetSession<Session>();
            var player = session.Player;
            var clan = _clanManager[message.ClubId];
            if (clan == null)
            {
                session.Send(new ClubJoinAck2Message(ClubJoinResult.Failed));
                return true;
            }
            session.Send(new ClubJoinAck2Message(await clan.JoinInvite(player)));
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubCloseReq2Message message)
        {
            var session = context.GetSession<Session>();
            var player = session.Player;
            if (player.ClanMember.Role != ClubRole.Master)
            {
                session.Send(new ClubCloseAck2Message(ClubCloseResult.MasterRequired));
                return true;
            }
            if (player.Clan.Members.Count() > 1)
            {
                session.Send(new ClubCloseAck2Message(ClubCloseResult.ClanNotEmpty));
                return true;
            }
            await player.Clan.Close();
            session.Send(new ClubCloseAck2Message(ClubCloseResult.Success));
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, ClubClubInfoReq2Message message)
        {
            var session1 = context.GetSession<Session>();
            var player = session1.Player;
            var clan = _clanManager[message.ClubId];
            var session = session1;
            session.Send(await clan.GetClubInfo2());
            session = null;
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { }, Invert = true)]
        public async Task<bool> OnHandle(MessageContext context, ClubCreateReq2Message message)
        {
            var session = context.GetSession<Session>();
            var player = session.Player;
            if (_clanManager.CheckClanName(message.Name) != ClubNameCheckResult.Available)
            {
                session.Send(new ClubCreateAck2Message(ClubCreateResult.Failed));
                return true;
            }
            session.Send((await _clanManager.CreateClan(player, message.Name, message.Unk2, ClubArea.LatinAmerica, ClubActivity.ClanBattle, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)).Item2 == ClanCreateError.None ? new ClubCreateAck2Message(ClubCreateResult.Success) : new ClubCreateAck2Message(ClubCreateResult.Failed));
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubEditIntroduceReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ClubEditIntroduceAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubEditURLReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ClubEditURLAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubGradeCountReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ClubGradeCountAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubBoardReadReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            context.GetSession<Session>().Send(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
            //session.Send(new ClubBoardReadAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubBoardSearchNickReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            context.GetSession<Session>().Send(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
            //session.Send(new ClubBoardReadAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubBoardWriteReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ClubBoardWriteAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubBoardReadOtherClubReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            context.GetSession<Session>().Send(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
            //session.Send(new ClubBoardReadAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubBoardReadMineReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ClubBoardReadAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubBoardModifyReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ClubBoardModifyAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubBoardDeleteReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ClubBoardDeleteAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubBoardDeleteAllReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ClubBoardDeleteAllAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubAdminBoardModifyReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ClubAdminBoardModifyAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubAdminInviteReqMessage message)
        {
            var session = context.GetSession<Session>();
            var player1 = session.Player;
            var clan = player1.Clan;
            if (player1.ClanMember.Role != ClubRole.Master)
            {
                session.Send(new ClubAdminInviteAckMessage(ClubAdminInviteResult.NoMatchFound));
                return true;
            }
            Player player2 = _playerManager.Get(message.AccountId);
            if (player2 == null)
            {
                session.Send(new ServerResultAckMessage(ServerResult.PlayerNotFound));
                return true;
            }
            session.Send(new ClubAdminInviteAckMessage(await clan.Invite(player1, player2)));
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubAdminMasterChangeReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ClubAdminMasterChangeAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubAdminSubMasterCancelReqMessage message)
        {
            var session = context.GetSession<Session>();
            _logger.Debug("OnHandle() => {@MessageName}...", message);
            var cancelAckMessage = new ClubAdminSubMasterCancelAckMessage();
            session.Send(cancelAckMessage);
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Firewall(typeof(MustBeInClan), new object[] { })]
        public async Task<bool> OnHandle(MessageContext context, ClubAdminSubMasterReqMessage message)
        {
            var logger = _logger;
            var session = context.GetSession<Session>();
            var propertyValue = message;
            logger.Debug("OnHandle() => {@MessageName}...", propertyValue);
            session.Send(new ClubAdminSubMasterAckMessage());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        public async Task<bool> OnHandle(MessageContext context, Network.Message.Club.ClubInfoReqMessage message)
        {
            var session = context.GetSession<Session>();
            var clan = _clanManager[message.ClubId];
            session.Send(await clan.GetClubInfo());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        public async Task<bool> OnHandle(MessageContext context, ClubSearchReqMessage message)
        {
            var session = context.GetSession<Session>();

            // TODO Better queries, pages and sorting

            var result = _clanManager
                .Where(x => x.Name.Contains(message.Query, StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Map<Clan, ClubSearchResultDto>())
                .ToArray();

            session.Send(new ClubSearchAckMessage(result));
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Firewall(typeof(MustBeInClan), Invert = true)]
        public async Task<bool> OnHandle(MessageContext context, ClubNameCheckReqMessage message)
        {
            context.Session.Send(new ClubNameCheckAckMessage(
                _clanManager.CheckClanName(message.Name)
            ));
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Firewall(typeof(MustBeInClan), Invert = true)]
        public async Task<bool> OnHandle(MessageContext context, ClubCreateReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            if (_clanManager.CheckClanName(message.Name) != ClubNameCheckResult.Available)
                session.Send(new ClubCreateAckMessage(ClubCreateResult.Failed));

            var (_, result) = await _clanManager.CreateClan(
                plr,
                message.Name, message.Description,
                message.Area, message.Activity,
                message.Question1, message.Question2, message.Question3, message.Question4, message.Question5
            );

            session.Send(result == ClanCreateError.None
                ? new ClubCreateAckMessage(ClubCreateResult.Success)
                : new ClubCreateAckMessage(ClubCreateResult.Failed));

            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Firewall(typeof(MustBeInClan))]
        public async Task<bool> OnHandle(MessageContext context, ClubCloseReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            if (plr.ClanMember.Role != ClubRole.Master)
            {
                session.Send(new ClubCloseAckMessage(ClubCloseResult.MasterRequired));
                return true;
            }

            if (plr.Clan.Members.Count() > 1)
            {
                session.Send(new ClubCloseAckMessage(ClubCloseResult.ClanNotEmpty));
                return true;
            }

            await plr.Clan.Close();
            session.Send(new ClubCloseAckMessage(ClubCloseResult.Success));
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        public async Task<bool> OnHandle(MessageContext context, ClubJoinConditionInfoReqMessage message)
        {
            var session = context.GetSession<Session>();
            var clan = _clanManager[message.ClubId];

            if (clan == null)
            {
                session.Send(new Network.Message.Game.ServerResultAckMessage(ServerResult.FailedToRequestTask));
                return true;
            }

            session.Send(new ClubJoinConditionInfoAckMessage
            {
                JoinType = clan.IsPublic ? 2 : 1,
                RequiredLevel = clan.RequiredLevel,
                Question1 = clan.Question1,
                Question2 = clan.Question2,
                Question3 = clan.Question3,
                Question4 = clan.Question4,
                Question5 = clan.Question5
            });
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        public async Task<bool> OnHandle(MessageContext context, Network.Message.Club.ClubJoinReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var clan = _clanManager[message.ClubId];

            if (clan == null)
            {
                session.Send(new Network.Message.Club.ClubJoinAckMessage(ClubJoinResult.Failed));
                return true;
            }

            var result = await clan.Join(
                plr,
                message.Answer1,
                message.Answer2,
                message.Answer3,
                message.Answer4,
                message.Answer5
            );
            session.Send(new Network.Message.Club.ClubJoinAckMessage(result));
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Firewall(typeof(MustBeInClan))]
        public async Task<bool> OnHandle(MessageContext context, ClubUnjoinReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            var result = await plr.Clan.Leave(plr);
            session.Send(new ClubUnjoinAckMessage(result ? ClubLeaveResult.Success : ClubLeaveResult.Failed));
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Firewall(typeof(MustBeInClan))]
        public async Task<bool> OnHandle(MessageContext context, ClubJoinWaiterInfoReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var clan = _clanManager[message.ClubId];

            if (plr.ClanMember.Role > ClubRole.Staff)
            {
                session.Send(new Network.Message.Game.ServerResultAckMessage(ServerResult.FailedToRequestTask));
                return true;
            }

            session.Send(new ClubJoinWaiterInfoAckMessage
            {
                Waiters = clan
                    .Where(x => x.State == ClubMemberState.JoinRequested)
                    .Select(x => x.Map<ClanMember, JoinWaiterInfoDto>())
                    .ToArray()
            });
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Firewall(typeof(MustBeInClan))]
        public async Task<bool> OnHandle(MessageContext context, ClubAdminJoinCommandReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var clan = plr.Clan;

            if (plr.ClanMember.Role > ClubRole.Staff)
            {
                session.Send(new ClubAdminJoinCommandAckMessage(ClubCommandResult.PermissionDenied));
                return true;
            }

            if (message.AccountIds.Length > 1)
            {
                session.Send(new ClubAdminJoinCommandAckMessage(ClubCommandResult.MemberNotFound));
                return true;
            }

            ClubCommandResult result;
            switch (message.Command)
            {
                case ClubCommand.Accept:
                    result = await clan.Approve(plr, message.AccountIds[0]);
                    plr.SendClanJoinEvents();
                    break;

                case ClubCommand.Decline:
                    result = await clan.Decline(plr, message.AccountIds[0]);
                    break;

                case ClubCommand.Kick:
                {
                    var targetMember = clan.GetMember(message.AccountIds[0]);
                    if (targetMember.Role <= plr.ClanMember.Role)
                        result = ClubCommandResult.PermissionDenied;
                    else
                        result = await clan.Kick(plr, message.AccountIds[0]);

                    plr.SendClanLeaveEvents();
                    break;
                }

                case ClubCommand.Ban:
                {
                    var targetMember = clan.GetMember(message.AccountIds[0]);
                    if (targetMember.Role <= plr.ClanMember.Role)
                        result = ClubCommandResult.PermissionDenied;
                    else
                        result = await clan.Ban(plr, message.AccountIds[0]);

                    plr.SendClanLeaveEvents();
                    break;
                }

                case ClubCommand.Unban:
                    result = await clan.Unban(plr, message.AccountIds[0]);
                    plr.SendClanLeaveEvents();
                    break;

                default:
                    plr.AddContextToLogger(_logger).Warning("Unknown join command={command}", message.Command);
                    result = ClubCommandResult.MemberNotFound;
                    break;
            }

            session.Send(new ClubAdminJoinCommandAckMessage(result));
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Firewall(typeof(MustBeInClan))]
        public async Task<bool> OnHandle(MessageContext context, ClubNewJoinMemberInfoReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var clan = plr.Clan;

            if (plr.ClanMember.Role > ClubRole.Staff)
            {
                session.Send(new Network.Message.Game.ServerResultAckMessage(ServerResult.FailedToRequestTask));
                return true;
            }

            plr.SendClanJoinEvents();
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Firewall(typeof(MustBeInClan))]
        public async Task<bool> OnHandle(MessageContext context, ClubUnjoinerListReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;

            if (plr.ClanMember.Role > ClubRole.Staff)
            {
                session.Send(new Network.Message.Game.ServerResultAckMessage(ServerResult.FailedToRequestTask));
                return true;
            }

            plr.SendClanLeaveEvents();
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Firewall(typeof(MustBeInClan))]
        public async Task<bool> OnHandle(MessageContext context, ClubAdminNoticeChangeReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var clan = plr.Clan;

            if (plr.ClanMember.Role != ClubRole.Master)
            {
                session.Send(new ClubAdminNoticeChangeAckMessage(ClubNoticeChangeResult.NoMatchFound));
                return true;
            }

            await clan.ChangeAnnouncement(message.Notice);
            session.Send(new ClubAdminNoticeChangeAckMessage(ClubNoticeChangeResult.Success));
            await clan.Broadcast(await clan.GetClubInfo());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Firewall(typeof(MustBeInClan))]
        public async Task<bool> OnHandle(MessageContext context, ClubAdminInfoModifyReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var clan = plr.Clan;

            if (plr.ClanMember.Role != ClubRole.Master)
            {
                session.Send(new ClubAdminInfoModifyAckMessage(ClubAdminInfoModifyResult.NoMatchFound));
                return true;
            }

            await clan.ChangeInfo(message.Area, message.Activity, message.Description);
            session.Send(new ClubAdminInfoModifyAckMessage(ClubAdminInfoModifyResult.Success));
            await clan.Broadcast(await clan.GetClubInfo());
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Firewall(typeof(MustBeInClan))]
        public async Task<bool> OnHandle(MessageContext context, ClubAdminJoinConditionModifyReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var clan = plr.Clan;

            if (plr.ClanMember.Role != ClubRole.Master)
            {
                session.Send(new ClubAdminJoinConditionModifyAckMessage(ClubAdminJoinConditionModifyResult.NoMatchFound));
                return true;
            }

            await clan.ChangeJoinCondition(
                message.JoinType == 2,
                (byte)message.RequiredLevel,
                message.Question1,
                message.Question2,
                message.Question3,
                message.Question4,
                message.Question5
            );
            session.Send(new ClubAdminJoinConditionModifyAckMessage(ClubAdminJoinConditionModifyResult.Success));
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn))]
        [Firewall(typeof(MustBeInClan))]
        public async Task<bool> OnHandle(MessageContext context, ClubAdminGradeChangeReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var clan = plr.Clan;

            if (plr.ClanMember.Role != ClubRole.Master)
            {
                session.Send(new ClubAdminGradeChangeAckMessage(ClubAdminChangeRoleResult.PermissionDenied));
                return true;
            }

            foreach (var roleChange in message.Grades)
            {
                var member = clan.GetMember(roleChange.AccountId);
                if (member == null)
                {
                    session.Send(new ClubAdminGradeChangeAckMessage(ClubAdminChangeRoleResult.MemberNotFound));
                    return true;
                }

                if (member == plr.ClanMember ||
                    member.Role == ClubRole.Master ||
                    roleChange.Role == ClubRole.Master ||
                    roleChange.Role < ClubRole.Master || roleChange.Role > ClubRole.BadManner)
                {
                    session.Send(new ClubAdminGradeChangeAckMessage(ClubAdminChangeRoleResult.CantChangeRank));
                    return true;
                }

                if (member.Role <= plr.ClanMember.Role)
                {
                    session.Send(new ClubAdminGradeChangeAckMessage(ClubAdminChangeRoleResult.PermissionDenied));
                    return true;
                }

                await member.ChangeRole(roleChange.Role);
            }

            session.Send(new ClubAdminGradeChangeAckMessage(
                ClubAdminChangeRoleResult.Success,
                message.Grades.Select(x => x.AccountId).ToArray()
            ));
            return true;
        }
    }
}
