using System;
using System.Collections.Generic;

namespace Netsphere.Server.Game.Data
{
    public class EnchantSystem
    {
        public uint Level { get; set; }

        public List<EnchantSystemDataItem> EnchantSystemDataItems { get; set; }

        public EnchantSystem() => EnchantSystemDataItems = new List<EnchantSystemDataItem>();
    }

    public class EnchantSystemDataItem
    {
        private readonly Random _random;

        public ItemCategory ItemCategory { get; set; }

        public byte ItemSubCategory { get; set; }

        public List<EnchantDataEffect> EnchantDataEffects { get; set; }

        public EnchantSystemDataItem()
        {
            _random = new Random();
            EnchantDataEffects = new List<EnchantDataEffect>();
        }

        public EnchantDataEffect GetAleatoryEffect()
        {
            int prob = _random.Next(0, 100);
            return EnchantDataEffects.Find(x => x.Prob.Equals(prob)) ?? EnchantDataEffects[_random.Next(0, EnchantDataEffects.Count)];
        }
    }

    public class EnchantDataEffect
    {
        public uint Id { get; set; }

        public int Prob { get; set; }
    }

}
