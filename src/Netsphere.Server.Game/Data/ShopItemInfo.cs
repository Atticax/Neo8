using Netsphere.Database.Game;
using Netsphere.Server.Game.Services;

namespace Netsphere.Server.Game.Data
{
    public class ShopItemInfo
    {
        public int Id { get; set; }
        public ShopPriceGroup PriceGroup { get; set; }
        public ShopEffectGroup EffectGroup { get; set; }
        public ShopEnabledType IsEnabled { get; set; }
        public int Discount { get; set; }

        public ShopItem ShopItem { get; }

        public ShopItemInfo(ShopItem shopItem, ShopItemInfoEntity entity, GameDataService gameDataService)
        {
            Id = entity.Id;
            PriceGroup = gameDataService.ShopPrices[entity.PriceGroupId];
            EffectGroup = gameDataService.ShopEffects[entity.EffectGroupId];
            IsEnabled = (ShopEnabledType)entity.IsEnabled;
            Discount = entity.DiscountPercentage;

            ShopItem = shopItem;
        }
    }
}
