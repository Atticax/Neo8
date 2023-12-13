using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Netsphere.Common;
using Netsphere.Database;
using Netsphere.Database.Auth;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Services;
using Netsphere.Server.Game.Data;

namespace Netsphere.Server.Game.Commands
{
    public class GameMasterCommands : ICommandHandler
    {
        private readonly PlayerManager _playerManager;
        private readonly DatabaseService _databaseService;
        private readonly GameDataService _gameDataService;

        public GameMasterCommands(
            PlayerManager playerManager,
            DatabaseService databaseService,
            GameDataService gameDataService)
        {
            _playerManager = playerManager;
            _databaseService = databaseService;
            _gameDataService = gameDataService;
        }

        [Command(
            CommandUsage.Player,
            SecurityLevel.GameMaster,
            "Usage: gm"
        )]
        public async Task<bool> GM(Player plr, string[] args)
        {
            plr.IsInGMMode = !plr.IsInGMMode;
            this.Reply(plr, plr.IsInGMMode ? "You are now in GM mode" : "You are no longer in GM mode");
            return true;
        }

        [Command(
            CommandUsage.Player,
            SecurityLevel.GameMaster,
            "Usage: announce"
        )]
        public async Task<bool> Announce(Player plr, string[] args)
        {
            if (args.Length == 0)
                return false;

            foreach (var p in _playerManager)
                p.SendNotice(string.Join(" ", args));

            return true;
        }

        [Command(
            CommandUsage.Player | CommandUsage.Console,
            SecurityLevel.GameMaster,
            "Usage: kick <accountId or nickname>"
        )]
        public async Task<bool> Kick(Player plr, string[] args)
        {
            if (args.Length != 2)
                return false;

            var playerToKick = ulong.TryParse(args[1], out var id)
                ? _playerManager[id]
                : _playerManager.GetByNickname(args[1]);

            if (playerToKick == null)
            {
                this.ReplyError(plr, "Player not found");
                return true;
            }

            playerToKick.Disconnect();
            this.Reply(plr, $"Kicked player {playerToKick.Account.Nickname}(Id:{playerToKick.Account.Id})");
            return true;
        }

        [Command(
            CommandUsage.Player | CommandUsage.Console,
            SecurityLevel.GameMaster,
            "Usage: ban <accountId or nickname> <duration> <reason>"
        )]
        public async Task<bool> Ban(Player plr, string[] args)
        {
            if (args.Length < 4)
                return false;

            var bannedBy = plr == null ? "SYSTEM" : plr.Account.Nickname;
            var reason = $"{bannedBy}: {string.Join(" ", args.Skip(2))}";

            if (!TimeSpan.TryParse(args[2], out var duration))
            {
                this.ReplyError(plr, "Invalid duration. Format is days:hours:minutes:seconds");
                return true;
            }

            AccountEntity account = null;
            if (ulong.TryParse(args[1], out var id))
            {
                var accountId = (long)id;
                using (var db = _databaseService.Open<AuthContext>())
                    account = await db.Accounts.Include(x => x.Bans).FirstOrDefaultAsync(x => x.Id == accountId);
            }

            if (account == null)
            {
                var nickname = args[1];
                using (var db = _databaseService.Open<AuthContext>())
                    account = await db.Accounts.Include(x => x.Bans).FirstOrDefaultAsync(x => x.Nickname == nickname);
            }

            if (account == null)
            {
                this.ReplyError(plr, "Player not found");
                return true;
            }

            // Check ban status
            var now = DateTimeOffset.Now.ToUnixTimeSeconds();
            var ban = account.Bans.FirstOrDefault(x => x.Duration == null || x.Date + x.Duration > now);
            if (ban != null)
            {
                this.ReplyError(plr, "Player is already banned");
                return true;
            }

            using (var db = _databaseService.Open<AuthContext>())
            {
                db.Bans.Add(new BanEntity
                {
                    AccountId = account.Id,
                    Date = now,
                    Duration = (long?)duration.TotalSeconds,
                    Reason = reason
                });

                await db.SaveChangesAsync();
            }

            // Kick player if online
            _playerManager[(ulong)account.Id]?.Disconnect();

            this.Reply(plr, $"Banned player {account.Nickname}(Id:{account.Id}) for {duration.ToHumanReadable()}");
            return true;
        }

        [Command(
            CommandUsage.Player | CommandUsage.Console,
            SecurityLevel.GameMaster,
            "Usage: unban <accountId or nickname>"
        )]
        public async Task<bool> Unban(Player plr, string[] args)
        {
            if (args.Length < 2)
                return false;

            AccountEntity account = null;
            if (ulong.TryParse(args[1], out var id))
            {
                var accountId = (long)id;
                using (var db = _databaseService.Open<AuthContext>())
                    account = await db.Accounts.Include(x => x.Bans).FirstOrDefaultAsync(x => x.Id == accountId);
            }

            if (account == null)
            {
                var nickname = args[1];
                using (var db = _databaseService.Open<AuthContext>())
                    account = await db.Accounts.Include(x => x.Bans).FirstOrDefaultAsync(x => x.Nickname == nickname);
            }

            if (account == null)
            {
                this.ReplyError(plr, "Player not found");
                return true;
            }

            // Check ban status
            var now = DateTimeOffset.Now.ToUnixTimeSeconds();
            var ban = account.Bans.FirstOrDefault(x => x.Duration == null || x.Date + x.Duration > now);
            if (ban != null)
            {
                using (var db = _databaseService.Open<AuthContext>())
                {
                    db.Bans.Remove(ban);
                    await db.SaveChangesAsync();
                }

                this.Reply(plr, $"Unbanned player {account.Nickname}({account.Id})");
                return true;
            }

            this.ReplyError(plr, "Player is not banned");
            return true;
        }

        [Command(
            CommandUsage.Player,
            SecurityLevel.GameMaster,
            "Usage: roominfo <roomId>"
        )]
        public async Task<bool> RoomInfo(Player plr, string[] args)
        {
            if (args.Length < 2)
                return false;
            if (plr.Channel == null)
            {
                this.ReplyError(plr, "No te encuentras en un canal");
                return true;
            }
            uint result;
            var room = uint.TryParse(args[1], out result) ? plr.Channel.RoomManager.Get(result) : null;
            if (room == null)
            {
                this.ReplyError(plr, "Room not found");
                return true;
            }
            this.Reply(plr, string.Format("Map: {0}", room.Map));
            this.Reply(plr, "Name: " + room.Options.Name);
            this.Reply(plr, "Password: " + room.Options.Password);
            this.Reply(plr, string.Format("Settings: {0}", room.Options.Settings));
            this.Reply(plr, string.Format("PlayersCount: {0}", room.Players.Count));
            this.Reply(plr, string.Format("SpectatorCount: {0}", room.TeamManager.Spectators.Count()));
            this.Reply(plr, "DateCreated: " + room.TimeCreated.ToString("dd/MM/yy HH:mm:ss"));
            return true;
        }

        [Command(
            CommandUsage.Player,
            SecurityLevel.GameMaster,
            "Usage: playerinfo <accountId or nickname>"
        )]
        public async Task<bool> PlayerInfo(Player plr, string[] args)
        {
            if (args.Length < 2)
                return false;
            ulong result;
            var player = ulong.TryParse(args[1], out result) ? _playerManager[result] : _playerManager.GetByNickname(args[1]);
            if (player == null)
            {
                this.ReplyError(plr, "Player not found");
                return true;
            }
            this.Reply(plr, string.Format("AccountId: {0}", player.Account.Id));
            //this.Reply(plr, string.Format("GameTime: {0}", player.GameTime));
            this.Reply(plr, string.Format("PEN: {0}", player.PEN));
            this.Reply(plr, string.Format("AP: {0}", player.AP));
            //this.Reply(plr, string.Format("TotalWins: {0}", player.TotalWins));
            //this.Reply(plr, string.Format("TotalLosses: {0}", player.TotalLosses));
            //this.Reply(plr, string.Format("TotalMatches: {0}", player.TotalMatches));
            this.Reply(plr, "Clan: " + (player?.Clan?.Name ?? "null"));
            this.Reply(plr, "Channel: " + (player?.Channel?.Name ?? "null"));
            this.Reply(plr, "RoomId: " + (player?.Room?.Id.ToString() ?? "null"));
            this.Reply(plr, string.Format("RoomSlot: {0}", player.Slot));
            this.Reply(plr, string.Format("State: {0}", player.State));
            //this.Reply(plr, string.Format("Kills: {0}, KillAssists: {1}, Deaths: {2}", player?.Score?.Kills.GetValueOrDefault(), (object)player?.Score?.KillAssists.GetValueOrDefault(), (object)player?.Score?.Deaths.GetValueOrDefault()));
            this.Reply(plr, string.Format("CharacterCount: {0}", player.CharacterManager.Count));
            this.Reply(plr, string.Format("BoosterCount: {0}", player.BoosterInventory.Count));
            this.Reply(plr, string.Format("ItemsCount: {0}", player.Inventory.Count));
            this.Reply(plr, string.Format("IPAddress: {0}", player.Session.RemoteEndPoint));
            return true;
        }

        [Command(
            CommandUsage.Player,
            SecurityLevel.GameMaster,
            "Usage: roomkick <accountId or nickname>"
        )]
        public async Task<bool> RoomKick(Player plr, string[] args)
        {
            if (args.Length < 2)
                return false;
            ulong result;
            var plr1 = ulong.TryParse(args[1], out result) ? _playerManager[result] : _playerManager.GetByNickname(args[1]);
            if (plr1 == null)
            {
                this.ReplyError(plr, "Player not found");
                return true;
            }
            if (plr1.Room == null)
            {
                this.ReplyError(plr, plr1.Account.Nickname + " is not in a room");
                return true;
            }
            plr1.Room.Leave(plr1, RoomLeaveReason.Kicked);
            this.Reply(plr, string.Format("Room kick to {0}(Id:{1})", plr1.Account.Nickname, plr1.Account.Id));
            return true;
        }

        [Command(
            CommandUsage.Console,
            SecurityLevel.GameMaster,
            "Usage: nick <result>"
        )]
        public async Task<bool> Nick(Player plr, string[] args)
        {
            if (args.Length < 2)
                return false;
            uint result;
            if (!uint.TryParse(args[1], out result))
                result = 0;
            plr.Session.Send(new ItemUseChangeNickAckMessage(result));
            return true;
        }

        [Command(
            CommandUsage.Console,
            SecurityLevel.GameMaster,
            "Usage: sendi <accountId or nickname> <itemNumber> <itemPriceType> <itemPeriodType> <itemPeriod> <itemColor> <itemPreviewEffect>"
        )]
        public async Task<bool> SendI(Player plr, string[] args)
        {
            if (args.Length < 8)
                return false;
            ulong result1;
            var player = ulong.TryParse(args[1], out result1) ? _playerManager[result1] : _playerManager.GetByNickname(args[1]);
            if (player == null)
            {
                this.ReplyError(plr, "Player not found");
                return true;
            }
            foreach (var s in ((IEnumerable<string>)args).Skip(1))
            {
                if (!uint.TryParse(s, out uint _))
                {
                    this.ReplyError(plr, s + " not parse");
                    return false;
                }
            }
            uint result2;
            uint.TryParse(args[2], out result2);
            uint result3;
            uint.TryParse(args[3], out result3);
            uint result4;
            uint.TryParse(args[4], out result4);
            ushort result5;
            ushort.TryParse(args[5], out result5);
            byte result6;
            byte.TryParse(args[6], out result6);
            uint result7;
            uint.TryParse(args[7], out result7);
            if (plr.AP < 689)
            {
                this.ReplyError(plr, "Su AP debe ser mayor a 689 para enviar un item.");
                return true;
            }
            var shopItemInfo = _gameDataService.GetShopItemInfo((ItemNumber)result2, (ItemPriceType)result3);
            if (shopItemInfo == null)
            {
                this.ReplyError(plr, "Trying to get non-existent item");
                return true;
            }
            var price = shopItemInfo.PriceGroup.GetPrice((ItemPeriodType)result4, result5);
            if (price == null)
            {
                this.ReplyError(plr, "Trying to get item with invalid price info");
                return true;
            }
            if (_gameDataService.GetEffectGruopByPreviewEffect(result7) == null)
            {
                this.ReplyError(plr, "Trying to found effect with invalid effect Group");
                return true;
            }
            var effects = Array.Empty<uint>();
            if (shopItemInfo.EffectGroup.Effects.Count > 0)
                effects = shopItemInfo.EffectGroup.Effects.Select(x => x.Effect).Where(x => x > 0).ToArray();
            player.Inventory.Create(shopItemInfo, price, result6, effects);
            if (plr.Account.SecurityLevel.Equals(SecurityLevel.GameMaster))
            {
                plr.AP -= 689;
                plr.SendMoneyUpdate();
            }
            this.Reply(plr, string.Format("Sent item={0} to {1}(Id:{2})", result2, player.Account.Nickname, player.Account.Id));
            return true;
        }

    }
}
