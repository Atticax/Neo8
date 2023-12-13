using Logging;
using Netsphere.Network.Message.Game;
using Netsphere.Network.Message.GameRule;
using Netsphere.Server.Game.Rules;
using ProudNet;
using System;
using System.Threading.Tasks;

namespace Netsphere.Server.Game.Handlers
{
    internal class GameRuleHandler :
        IHandle<MoneyUseCoinReqMessage>,
        IHandle<UseBurningBuffReqMessage>
    {
        private readonly ILogger<GameRuleHandler> _logger;
        private readonly PlayerManager _playerManager;

        public GameRuleHandler(ILogger<GameRuleHandler> logger, PlayerManager playerManager)
        {
            _logger = logger;
            _playerManager = playerManager;
        }

        [Firewall(typeof(MustBeInRoom), new object[] { })]
        [Firewall(typeof(MustBeGameState), new object[] { GameState.Playing })]
        public async Task<bool> OnHandle(MessageContext context, MoneyUseCoinReqMessage message)
        {
            var session = context.GetSession<Session>();
            var player = session.Player;
            var numArray = new uint[6]
            {
                50,
                50,
                150,
                50,
                100,
                100
            };
            int time = new int[6] { 120, 120, 300, 60, 120, 120 }[(int)message.BuffType];
            int data = 0;
            if (numArray[(int)message.BuffType] > player.PEN)
            {
                session.Send(new MoneyUseCoinAckMessage((UseCoinMessage)1));
                return true;
            }
            player.PEN -= numArray[(int)message.BuffType];
            switch (message.BuffType)
            {
                case BuffType.PEN:
                    player.Score.LuckyContextPEN = new LuckyContext(player, 1, DateTimeOffset.Now.AddSeconds(time));
                    break;
                case BuffType.EXP:
                    player.Score.LuckyContextEXP = new LuckyContext(player, 2, DateTimeOffset.Now.AddSeconds(time));
                    break;
                case BuffType.HP:
                case BuffType.SP:
                    data = 5;
                    break;
            }
            session.Send(new MoneyUseCoinAckMessage(0, message.BuffType, time, data));
            session.Send(new MoneyRefreshPenInfoAckMessage(player.PEN));
            return true;
        }

        [Firewall(typeof(MustBeInRoom), new object[] { })]
        [Firewall(typeof(MustBeGameState), new object[] { GameState.Playing })]
        public async Task<bool> OnHandle(MessageContext context, UseBurningBuffReqMessage message)
        {
            var session = context.GetSession<Session>();
            var player = session.Player;
            if ((int)player.Room.Options.Settings <= 1 && player.Room.Options.GameRule != GameRule.Deathmatch)
            {
                session.Send(new UseBurningBuffAckMessage(0, (byte)0));
                return true;
            }
            if (player.PEN < 300)
            {
                session.Send(new MoneyUseCoinAckMessage((UseCoinMessage)1));
                return true;
            }
            player.PEN -= 300;
            session.Send(new UseBurningBuffAckMessage(1, (byte)80));
            session.Send(new MoneyRefreshPenInfoAckMessage(player.PEN));
            return true;
        }
    }
}
