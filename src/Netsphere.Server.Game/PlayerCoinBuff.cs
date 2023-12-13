using System;
using System.Collections.Generic;
using System.Text;
using Logging;
using Netsphere.Network.Message.Game;
using Netsphere.Network.Message.GameRule;

namespace Netsphere.Server.Game
{
   
    internal class PlayerCoinBuff

    {
        private static readonly ILogger _logger;
        private static readonly int _time = 550; //60 seconds

        public Player Player { get; internal set; }

        public CoinBuff[] CoinBuffs { get; set; }

        public PlayerCoinBuff(Player player)
        {
            this.Player = player;

            this.CoinBuffs = new CoinBuff[]
            {
        new CoinBuff(BuffType.PEN),
        new CoinBuff(BuffType.EXP),
        new CoinBuff(BuffType.Tracking),
        new CoinBuff(BuffType.HP),
        new CoinBuff(BuffType.SP)
            };
        }

        internal void StartBuffSystem(BuffType type)
        {
            const byte time = 60;

            var price = 0;
            var value = 0;

            switch (type)
            {
                case BuffType.PEN:
                    price = 50;
                    value = 30;
                    break;
                case BuffType.Respawn:
                    price = 150;
                    break;
                case BuffType.Tracking:
                    price = 50;
                    break;
                case BuffType.EXP:
                    price = 50;
                    value = 30;
                    break;
                case BuffType.HP:
                    price = 100;
                    value = 5;
                    break;
                case BuffType.SP:
                    price = 100;
                    value = 5;
                    break;
            }

            CoinBuff buff = this.FindBuff(type);
            if (buff == null)
            { return; }

            if (buff.IsEnabled == true)
            {
                this.Player.Session.Send(new MoneyUseCoinAckMessage { Message = UseCoinMessage.UnableToUse });
                return;
            }

            if (this.Player.PEN < price)
            {
                this.Player.Session.Send(new MoneyUseCoinAckMessage { Message = UseCoinMessage.InsufficientCoin });
                return;
            }

            buff.IsEnabled = true;

            this.Player.PEN -= (uint)price;

            //guessed
            Player.Session.Send(new MoneyRefreshCashInfoAckMessage { PEN = this.Player.PEN, AP = this.Player.AP });
            
            Player.Session.Send(new MoneyUseCoinAckMessage
            {
                Message = UseCoinMessage.Ok,
                BuffType = type,
                Time = time,
                Data = value,
                Unk5 = 0
            });
        }
        public CoinBuff FindBuff(BuffType type)
        {
            for (int i = 0; i < this.CoinBuffs.Length; i++)
            {
                if (this.CoinBuffs[i].Type == type)
                { return this.CoinBuffs[i]; }
            }

            return null;
        }

        public void Update(int value)
        {
            for (var i = 0; i < this.CoinBuffs.Length; i++)
            {
                if (this.CoinBuffs[i].IsEnabled)
                {
                    this.CoinBuffs[i].CurrentTime += value;
                }

                if (this.CoinBuffs[i].CurrentTime >= _time)
                {
                    this.CoinBuffs[i].CurrentTime = 0;
                    this.CoinBuffs[i].IsEnabled = false;
                }
            }
        }

        public void Reset()
        {
            for (var i = 0; i < this.CoinBuffs.Length; i++)
            {
                this.CoinBuffs[i].CurrentTime = 0;
                this.CoinBuffs[i].IsEnabled = false;
            }
        }
    }

    internal class CoinBuff
    {
        public BuffType Type { get; set; }

        public long CurrentTime { get; set; } //Millis

        public bool IsEnabled { get; set; }

        public CoinBuff()
        { }

        public CoinBuff(BuffType type)
        {
            this.Type = type;
            this.CurrentTime = 0;
            this.IsEnabled = false;
        }
    }
}
