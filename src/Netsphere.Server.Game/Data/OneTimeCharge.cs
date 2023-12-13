using System;
using System.Collections.Generic;
using System.Linq;

namespace Netsphere.Server.Game.Data
{
    public class OneTimeCharge
    {
        public ItemNumber ItemKey { get; set; }

        public List<OneTimeChargeCategory> OneTimeChargeCategories { get; set; }
    }

    public class OneTimeChargeCategory
    {
        public CapsuleRewardType Type { get; set; }

        public List<OneTimeChargeSubCategory> OneTimeChargeSubCategories { get; set; }
    }

    public class OneTimeChargeSubCategory
    {
        public bool Aleatory { get; set; }

        public bool UniqueBoost { get; set; }

        public List<OneTimeChargeItem> OneTimeChargeItems { get; set; }

        public List<OneTimeChargeItem> GetItems(ItemNumber[] itemNumbers)
        {
            var oneTimeChargeItemList = new List<OneTimeChargeItem>();
            int num = 0;
            var random = new Random();
            if (UniqueBoost && itemNumbers.Length != 0)
            {
                if (itemNumbers.Any(x => x.Id.Equals(5040006)))
                {
                    oneTimeChargeItemList.AddRange(OneTimeChargeItems.Where(x => x.BoostKey.Equals(5040006)));
                    return oneTimeChargeItemList;
                }
                if (itemNumbers.Any(x => x.Id.Equals(5040005)))
                    num = 5;
                if (itemNumbers.Any(x => x.Id.Equals(5040004)))
                    num = 4;
                if (itemNumbers.Any(x => x.Id.Equals(5040003)))
                    num = 3;
                if (itemNumbers.Any(x => x.Id.Equals(5040002)))
                    num = 2;
                if (itemNumbers.Any(x => x.Id.Equals(5040001)))
                    num = 1;
            }
            if (num >= OneTimeChargeItems.Count)
                num = 0;
            if (Aleatory)
                oneTimeChargeItemList.Add(OneTimeChargeItems[random.Next(0, OneTimeChargeItems.Count - num)]);
            else
                oneTimeChargeItemList = OneTimeChargeItems.Select(x => x).ToList();
            return oneTimeChargeItemList;
        }
    }

    public class OneTimeChargeItem
    {
        public ItemNumber Key { get; set; }

        public uint PreviewEffect { get; set; }

        public ItemPriceType PriceType { get; set; }

        public ItemPeriodType PeriodType { get; set; }

        public ushort Period { get; set; }

        public int Color { get; set; }

        public uint Amount { get; set; }

        public uint BoostKey { get; set; }

        //public uint Rate { get; set; }
    }
}
