using System;
using System.Collections.Generic;
using System.Linq;

namespace Netsphere.Server.Game.Data
{
    public class BtcOptions
    {
        public OptionBtcClear OptionBtc { get; set; }

        public List<BtcOptionComponent> Components { get; set; }

        public BtcOptions()
        {
            Components = new List<BtcOptionComponent>();
        }

        public BtcOptionComponent GetComponent(byte step) => this.Components.FirstOrDefault(x => x.Step.Equals(step));

    }

    public class BtcOptionComponent
    {
        public byte Step { get; set; }

        public List<BtcOptionRequitial> Requitials { get; set; }

        public BtcOptionComponent()
        {
            Requitials = new List<BtcOptionRequitial>();
        }
    }

    public class BtcOptionRequitial
    {
        public ItemNumber ItemNumber { get; set; }

        public ItemPriceType PriceType { get; set; }

        public ItemPeriodType PeriodType { get; set; }

        public ushort Period { get; set; }

        public byte Color { get; set; }

        public uint EffectId { get; set; }
    }

}
