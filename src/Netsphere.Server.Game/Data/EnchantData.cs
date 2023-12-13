using System.Collections.Generic;

namespace Netsphere.Server.Game.Data
{
    public class EnchantData
    {
        public List<EnchantDataData> DataDatas { get; }

        public List<EnchantMasteryNeed> MasteryNeed { get; set; }

        public List<EnchantMasteryNeed> PriceNeed { get; set; }

        public EnchantData()
        {
            DataDatas = new List<EnchantDataData>();
            MasteryNeed = new List<EnchantMasteryNeed>();
            PriceNeed = new List<EnchantMasteryNeed>();
        }
    }

    public class EnchantDataData
    {
        public int MasteryPeerMin { get; set; }

        public uint BonusProb { get; set; }

        public uint Prob { get; set; }

        public uint Level { get; set; }
    }

    public class EnchantMasteryNeed
    {
        public ItemCategory ItemCategory { get; set; }

        public uint Level { get; set; }

        public uint Durability { get; set; }
    }

}
