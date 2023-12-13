using System;
using System.Collections.Generic;
using System.Text;

namespace Netsphere.Server.Game.Data
{
    public class RandomShopLineup
    {
        public uint PackageId { get; set; }

        public RandomShopItem Item { get; set; }

        public RandomShopLineup(uint packageId, RandomShopItem item)
        {
            PackageId = packageId;
            Item = item;
        }
    }
}
