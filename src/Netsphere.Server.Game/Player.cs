using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpressMapper.Extensions;
using Logging;
using Microsoft.Extensions.Options;
using Netsphere.Common;
using Netsphere.Common.Configuration;
using Netsphere.Database;
using Netsphere.Database.Game;
using Netsphere.Database.Helpers;
using Netsphere.Network;
using Netsphere.Network.Data.Club;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Club;
using Netsphere.Network.Message.Game;
using Netsphere.Network.Message.GameRule;
using Netsphere.Server.Game.Services;

namespace Netsphere.Server.Game
{
    public class Player : DatabaseObject, ISaveable
    {
        private ILogger _logger;
        private int _totallosses;
        private readonly GameOptions _gameOptions;
        private readonly GameDataService _gameDataService;
        private readonly ClanManager _clanManager;
        public PlayerShoppingBasket ShoppingBaskets { get; }
        private readonly NicknameLookupService _nicknameLookupService;
        private byte _tutorialState;
        private uint _totalExperience;
        private uint _pen;
        private uint _ap;
        private uint _coins1;
        private uint _coins2;
        private uint _nametag;
        private PlayerState _state;
        private string _playtime;
        private DateTimeOffset _lastOnTime;

        public Session Session { get; private set; }
        public Account Account { get; private set; }
        public PlayerBoosterInventory BoosterInventory { get; }
        public CharacterManager CharacterManager { get; }
        public PlayerInventory Inventory { get; }
        public Clan Clan { get; internal set; }
        public ClanMember ClanMember => Clan?.GetMember(Account.Id);
        public PlayerTutorialManager Tutorials { get; }

        public byte TutorialState
        {
            get => _tutorialState;
            set => SetIfChanged(ref _tutorialState, value);
        }
        public uint TotalExperience
        {
            get => _totalExperience;
            set => SetIfChanged(ref _totalExperience, value);
        }
        public uint PEN
        {
            get => _pen;
            set => SetIfChanged(ref _pen, value);
        }
        public uint AP
        {
            get => _ap;
            set => SetIfChanged(ref _ap, value);
        }
        public uint Coins1
        {
            get => _coins1;
            set => SetIfChanged(ref _coins1, value);
        }
        public uint Coins2
        {
            get => _coins2;
            set => SetIfChanged(ref _coins2, value);
        }
        public uint Nametag
        {
            get => _nametag;
            set => SetIfChanged(ref _nametag, value);
        }
        public Channel Channel { get; internal set; }
        public int Level => _gameDataService.GetLevelFromExperience(_totalExperience).Level;

        public Room Room { get; internal set; }
        public byte Slot { get; internal set; }
        public PlayerState State
        {
            get => _state;
            internal set
            {
                if (_state == value)
                    return;

                _state = value;
                OnStateChanged();
            }
        }
        public PlayerGameMode Mode { get; internal set; }

        public uint TotalMatches { get;  set; }
        public bool IsConnectingToRoom { get; internal set; }
        public bool IsReady { get; internal set; }
        public Team Team { get; internal set; }
        public PlayerScore Score { get; internal set; }
        public LongPeerId PeerId { get; internal set; }
        public DateTimeOffset StartPlayTime { get; internal set; }
        public DateTimeOffset[] CharacterStartPlayTime { get; internal set; }
        public bool IsInGMMode { get; set; }
        public bool IsLoading { get; internal set; }

        internal PlayerCoinBuff PlayerCoinBuff { get; set; }
       

        public event EventHandler<PlayerEventArgs> Disconnected;
        public event EventHandler<PlayerEventArgs> StateChanged;
        public event EventHandler<NicknameEventArgs> NicknameCreated;
        public event EventHandler<ChannelEventArgs> ChannelJoined;
        public event EventHandler<ChannelEventArgs> ChannelLeft;
        public event EventHandler<RoomPlayerEventArgs> RoomJoined;
        public event EventHandler<RoomPlayerEventArgs> RoomLeft;

        internal void OnDisconnected()
        {
            Room?.Leave(this);
            Channel?.Leave(this);

            Disconnected?.Invoke(this, new PlayerEventArgs(this));
        }

        protected virtual void OnStateChanged()
        {
            StateChanged?.Invoke(this, new PlayerEventArgs(this));
        }

        protected internal virtual void OnNicknameCreated(string nickname)
        {
            NicknameCreated?.Invoke(this, new NicknameEventArgs(this, nickname));
        }

        protected internal virtual void OnChannelJoined(Channel channel)
        {
            ChannelJoined?.Invoke(this, new ChannelEventArgs(channel, this));
        }

        protected internal virtual void OnChannelLeft(Channel channel)
        {
            ChannelLeft?.Invoke(this, new ChannelEventArgs(channel, this));
        }

        protected internal virtual void OnRoomJoined(Room room)
        {
            RoomJoined?.Invoke(this, new RoomPlayerEventArgs(room, this));
        }

        protected internal virtual void OnRoomLeft(Room room)
        {
            RoomLeft?.Invoke(this, new RoomPlayerEventArgs(room, this));
        }

        public Player(
            ILogger<Player> logger,
            IOptions<GameOptions> gameOptions,
            GameDataService gameDataService,
            CharacterManager characterManager,
            PlayerBoosterInventory boostInventory,
            PlayerTutorialManager tutorials,
            PlayerInventory inventory,
            PlayerShoppingBasket shoppingbasket,
            ClanManager clanManager,
            NicknameLookupService nicknameLookupService)
        {
            _logger = logger;
            
            _gameOptions = gameOptions.Value;
            _gameDataService = gameDataService;
            _clanManager = clanManager;
            _nicknameLookupService = nicknameLookupService;
            CharacterManager = characterManager;
            BoosterInventory = boostInventory;
            ShoppingBaskets = shoppingbasket;
            Tutorials = tutorials;
            Inventory = inventory;
            CharacterStartPlayTime = new DateTimeOffset[3];
            PlayerCoinBuff = new PlayerCoinBuff(this);
        }

        internal void Initialize(Session session, Account account, PlayerEntity entity)
        {
            Session = session;
            Account = account;
            _logger = AddContextToLogger(_logger);
            
            //_totallosses = entity.TotalLosses;
            _tutorialState = entity.TutorialState;
            _totalExperience = (uint)entity.TotalExperience;
            _pen = (uint)entity.PEN;
            _ap = (uint)entity.AP;
            _coins1 = (uint)entity.Coins1;
            _coins2 = (uint)entity.Coins2;
            short? tagId = entity.Nametag?.TagId;
            _nametag = (tagId.HasValue ? new uint?((uint)tagId.GetValueOrDefault()) : new uint?()).GetValueOrDefault();


            if (entity.ClanMember != null)
                Clan = _clanManager[(uint)entity.ClanMember.ClanId];

            Inventory.Initialize(this, entity);
            BoosterInventory.Initialize(this, Inventory, entity);
            CharacterManager.Initialize(this, entity);
            ShoppingBaskets.Initialize(this, entity);
            Tutorials.Initialize(this, entity);
        }

        public void Disconnect()
        {
            _ = DisconnectAsync();
        }

        public Task DisconnectAsync()
        {
            return Session.CloseAsync();
        }

        /// <summary>
        /// Gains experiences and levels up if the player earned enough experience
        /// </summary>
        /// <param name="amount">Amount of experience to earn</param>
        /// <returns>true if the player leveled up</returns>
        public bool GainExperience(uint amount)
        {
            _logger.Debug("Gained {Amount} experience", amount);

            var levels = _gameDataService.Levels;
            var levelInfo = levels.GetValueOrDefault(Level);
            if (levelInfo == null)
            {
                _logger.Warning("Level={Level} not found", Level);
                return false;
            }

            // We cant earn experience when we reached max level
            if (levelInfo.ExperienceToNextLevel == 0 || Level >= _gameOptions.MaxLevel)
                return false;

            var leveledUp = false;
            TotalExperience += amount;

            // Did we level up?
            // Using a loop for multiple level ups
            while (levelInfo.ExperienceToNextLevel != 0 &&
                   levelInfo.ExperienceToNextLevel <= (int)(TotalExperience - levelInfo.TotalExperience) &&
                   levelInfo.Level < _gameOptions.MaxLevel)
            {
                var newLevel = Level + 1;
                levelInfo = levels.GetValueOrDefault(newLevel);

                if (levelInfo == null)
                {
                    _logger.Warning("Can't level up because level={Level} not found", newLevel);
                    break;
                }

                _logger.Debug("Leveled up to {Level}", newLevel);

                var reward = _gameDataService.LevelRewards.GetValueOrDefault(newLevel);
                if (reward != null)
                {
                    _logger.Debug("Level reward type={MoneyType} value={Value}", reward.Type, reward.Money);
                    switch (reward.Type)
                    {
                        case MoneyType.PEN:
                            PEN += (uint)reward.Money;
                            break;

                        case MoneyType.AP:
                            AP += (uint)reward.Money;
                            break;

                        default:
                            _logger.Warning("Unknown moneyType={MoneyType}", reward.Type);
                            break;
                    }

                    SendMoneyUpdate();
                }

                leveledUp = true;
            }

            if (!leveledUp)
                return false;
            return true;
        }

        public TimeSpan GetCurrentPlayTime()
        {
            return DateTimeOffset.Now - StartPlayTime;
        }

        public TimeSpan GetCharacterPlayTime(byte slot)
        {
            if (slot >= CharacterStartPlayTime.Length)
                return default;

            return DateTimeOffset.Now - CharacterStartPlayTime[slot];
        }

        /// <summary>
        /// Gets the maximum hp for the current character
        /// </summary>
        public float GetMaxHP()
        {
            return _gameDataService.GameTempos["GAMETEMPO_FREE"].ActorDefaultHPMax +
                   GetAttributeValue(EffectType.HP);
        }

        /// <summary>
        /// Gets the total attribute value for the current character
        /// </summary>
        /// <param name="attribute">The attribute to retrieve</param>
        /// <returns></returns>
        public float GetAttributeValue(EffectType attribute)
        {
            if (CharacterManager.CurrentCharacter == null)
                return 0;

            var character = CharacterManager.CurrentCharacter;
            var value = GetAttributeValueFromItems(attribute, character.Weapons.GetItems());
            value += GetAttributeValueFromItems(attribute, character.Skills.GetItems());
            value += GetAttributeValueFromItems(attribute, character.Costumes.GetItems());

            return value;
        }

        /// <summary>
        /// Gets the total attribute rate for the current character
        /// </summary>
        /// <param name="attribute">The attribute to retrieve</param>
        /// <returns></returns>
        public float GetAttributeRate(EffectType attribute)
        {
            if (CharacterManager.CurrentCharacter == null)
                return 0;

            var character = CharacterManager.CurrentCharacter;
            var value = GetAttributeRateFromItems(attribute, character.Weapons.GetItems());
            value += GetAttributeRateFromItems(attribute, character.Skills.GetItems());
            value += GetAttributeRateFromItems(attribute, character.Costumes.GetItems());

            return value;
        }

        public async Task SendAccountInformation()
        {

            Session.Send(new ItemInventoryInfoAckMessage
            {
                Items = Inventory.Select(x => x.Map<PlayerItem, ItemDto>()).ToArray()
            });

            Session.Send(new CharacterCurrentSlotInfoAckMessage
            {
                ActiveCharacter = CharacterManager.CurrentSlot, CharacterCount = (byte)CharacterManager.Count, MaxSlots = 3
            });

            foreach (var character in CharacterManager)
            {
                Session.Send(new CharacterCurrentInfoAckMessage
                {
                    Slot = character.Slot,
                    Style = new CharacterStyle(character.Gender, character.Slot,
                        character.Hair.Variation, character.Face.Variation,
                        character.Shirt.Variation, character.Pants.Variation)
                });

                var message = new CharacterCurrentItemInfoAckMessage
                {
                    Slot = character.Slot,
                    Weapons = character.Weapons.GetItems().Select(x => x?.Id ?? 0).ToArray(),
                    Skills = new[]
                    {
                        character.Skills.GetItem(0).Item1?.Id ?? 0
                    },
                    Clothes = character.Costumes.GetItems().Select(x => x?.Id ?? 0).ToArray()
                };

                Session.Send(message);
            }

            SendMoneyUpdate();
            Session.Send(new ServerResultAckMessage(ServerResult.WelcomeToS4World));
            Session.Send(new PlayerAccountInfoAckMessage(new PlayerAccountInfoDto
            {
                Level = (byte)Level,
                TotalExperience = TotalExperience,
                AP = AP,
                PEN = PEN,
                TutorialState = (uint)(_gameOptions.EnableTutorial ? TutorialState : 2),
                Nickname = Account.Nickname,
                IsGM = Account.SecurityLevel > SecurityLevel.User
            }));

            SendClubInfo();
            ShoppingBaskets.SendBasketList();
            Session.Send(new ItemEquipBoostItemInfoAckMessage((BoosterInventory.GetItems()).Select(x => x == null ? 0 : x.Id).ToArray()));
            Tutorials.SendNoticeTutorial();
        }

        public TimeSpan OnTimeDelta
        {
            get
            {
                var ontime = DateTimeOffset.Now - _lastOnTime;
                _lastOnTime = DateTimeOffset.Now;
                return ontime;
            }
        }

        public string PlayTime
        {
            get
            {
                if (_playtime == "")
                    _playtime = TimeSpan.FromSeconds(0).ToString();

                _playtime = (TimeSpan.Parse(_playtime) + OnTimeDelta).ToString();
                
                return _playtime;
            }
        }

        public void SendClubInfo()
        {
            Session.Send(new ClubMyInfoAckMessage
            {
                ClanId = Clan?.Id ?? 0,
                ClanIcon = Clan?.Icon,
                ClanName = Clan?.Name,
                State = ClanMember?.State ?? ClubMemberState.None,
                Role = ClanMember?.Role ?? ClubRole.Normal
            });
        }

        public void SendMoneyUpdate()
        {
            Session.Send(new MoneyRefreshCashInfoAckMessage(PEN, AP));
            Session.Send(new MoenyRefreshCoinInfoAckMessage(Coins1, Coins2));
        }

        public void SendClanLeaveEvents()
        {
            if (Clan == null)
                return;

            var entries = Clan.Events
                .Where(x => x.Event == ClanEvent.Leave && Clan.GetMember(x.AccountId) == null ||
                            x.Event == ClanEvent.Kick && Clan.GetMember((ulong)x.Value1) == null ||
                            x.Event == ClanEvent.Ban && Clan.Bans.Contains((ulong)x.Value1))
                .GroupBy(x => x.Event == ClanEvent.Leave ? x.AccountId : (ulong)x.Value1)
                .Select(x =>
                {
                    var eventEntry = x.OrderByDescending(_ => _.Date).First();
                    var dto = new MemberLeftDto
                    {
                        AccountId = eventEntry.Event == ClanEvent.Leave ? (uint)eventEntry.AccountId : (uint)eventEntry.Value1,
                        Date = eventEntry.Date
                    };
                    dto.Name = _nicknameLookupService.GetNickname(dto.AccountId);
                    switch (eventEntry.Event)
                    {
                        case ClanEvent.Leave:
                            dto.Reason = ClubLeaveReason.Leave;
                            break;

                        case ClanEvent.Kick:
                            dto.Reason = ClubLeaveReason.Kick;
                            break;

                        case ClanEvent.Ban:
                            dto.Reason = ClubLeaveReason.Ban;
                            break;
                    }

                    return dto;
                })
                .ToArray();
            Session.Send(new ClubUnjoinerListAckMessage(entries));
        }

        public void SendClanJoinEvents()
        {
            if (Clan == null)
                return;

            var newMembers = Clan
                .Where(x => x.State == ClubMemberState.Joined)
                .OrderByDescending(x => x.JoinDate)
                .Select(x => x.Map<ClanMember, NewMemberInfoDto>())
                .ToArray();
            Session.Send(new ClubNewJoinMemberInfoAckMessage(newMembers));
        }

        /// <summary>
        /// Sends a message to the game master console
        /// </summary>
        /// <param name="message">The message to send</param>
        public void SendConsoleMessage(string message)
        {
            Session.Send(new AdminActionAckMessage(0, message));
        }

        /// <summary>
        /// Sends a notice message
        /// </summary>
        /// <param name="message">The message to send</param>
        public void SendNotice(string message)
        {
            Session.Send(new NoticeAdminMessageAckMessage(message));
        }

        public void SendBriefing()
        {
            if (Room == null)
                return;

            var briefing = Room.GetBriefing();
            Session.Send(new GameBriefingInfoAckMessage(false, false, briefing.GetData()));
        }


        public async Task Save(GameContext db)
        {
            if (IsDirty)
            {
                db.Players.Update(new PlayerEntity
                {
                    Id = (int)Account.Id,
                    TutorialState = TutorialState,
                    TotalExperience = (int)TotalExperience,
                    PEN = (int)PEN,
                    AP = (int)AP,
                    Coins1 = (int)Coins1,
                    Coins2 = (int)Coins2,
                    CurrentCharacterSlot = CharacterManager.CurrentSlot
                });

                SetDirtyState(false);
            }

            await Inventory.Save(db);
            await CharacterManager.Save(db);
            await BoosterInventory.Save(db);
            await ShoppingBaskets.Save(db);
            await Tutorials.Save(db);
        }

        public ILogger AddContextToLogger(ILogger logger)
        {
            return logger.ForContext(
                ("AccountId", Account.Id),
                ("HostId", Session.HostId),
                ("EndPoint", Session.RemoteEndPoint.ToString())
            );
        }

        private static float GetAttributeValueFromItems(EffectType attribute, IEnumerable<PlayerItem> items)
        {
            return items.Where(item => item != null)
                .SelectMany(item => item.GetItemEffects())
                .Where(effect => effect != null && effect.EffectType == attribute)
                .Sum(attrib => attrib.Value);
        }

        private static float GetAttributeRateFromItems(EffectType attribute, IEnumerable<PlayerItem> items)
        {
            return items.Where(item => item != null)
                .SelectMany(item => item.GetItemEffects())
                .Where(effect => effect != null && effect.EffectType == attribute)
                .Sum(attrib => attrib.Rate);
        }
    }
}
