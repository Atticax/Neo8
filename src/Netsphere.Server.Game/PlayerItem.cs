using System;
using System.Collections.Immutable;
using System.Linq;
using Logging;
using Netsphere.Database.Game;
using Netsphere.Database.Helpers;
using Netsphere.Server.Game.Data;
using Netsphere.Server.Game.Services;
using Newtonsoft.Json;

namespace Netsphere.Server.Game
{
    public class PlayerItem : DatabaseObject
    {
        private readonly GameDataService _gameDataService;
        private int _durability;
        private uint _enchantMP;
        private uint _enchantLevel;

        public PlayerInventory Inventory { get; }

        public ulong Id { get; }
        public ItemNumber ItemNumber { get; }
        public ItemPriceType PriceType { get; }
        public ItemPeriodType PeriodType { get; }
        public ushort Period { get; }
        public byte Color { get; }
        public PlayerItemEffectCollection Effects { get; }
        public DateTimeOffset PurchaseDate { get; }

        private uint _count;

        public int Durability
        {
            get => _durability;
            set => SetIfChanged(ref _durability, value);
        }
        public DateTimeOffset ExpireDate =>
            PeriodType != ItemPeriodType.Days? DateTimeOffset.MinValue : PurchaseDate.AddDays(Durability);

        public DateTimeOffset CalculateExpireTime() => PeriodType == ItemPeriodType.Days ? PurchaseDate.AddDays(Durability) : DateTimeOffset.MinValue;

        public uint EnchantMP
        {
            get => _enchantMP;
            set => SetIfChanged(ref _enchantMP, value);
        }

        public uint EnchantLevel
        {
            get => _enchantLevel;
            set => SetIfChanged(ref _enchantLevel, value);
        }

        public uint Count
        {
            get => _count;
            set => SetIfChanged(ref _count, value);
        }

        public CharacterInventory CharacterInventory { get; internal set; }

        public bool IsBoost { get; internal set; }

        internal PlayerItem(ILogger logger, GameDataService gameDataService, PlayerInventory inventory, PlayerItemEntity entity)
        {
            _gameDataService = gameDataService;
            Inventory = inventory;
            Id = (ulong)entity.Id;

            var itemInfo = _gameDataService.ShopItems.Values.First(group => group.GetItemInfo(entity.ShopItemInfoId) != null);
            if (itemInfo == null)
                logger.Warning("Unable to load itemInfo from item={ItemId}", entity.Id);

            ItemNumber = itemInfo.ItemNumber;

            var priceGroup = _gameDataService.ShopPrices.Values.First(group => group.GetPrice(entity.ShopPriceId) != null);
            if (priceGroup == null)
                logger.Warning("Unable to load priceGroup from item={ItemId}", entity.Id);

            var price = priceGroup.GetPrice(entity.ShopPriceId);
            if (price == null)
                logger.Warning("Unable to load price from item={ItemId}", entity.Id);

            PriceType = priceGroup.PriceType;
            PeriodType = price.PeriodType;
            Period = price.Period;
            Color = entity.Color;

            var effects = Array.Empty<uint>();
            if (!string.IsNullOrWhiteSpace(entity.Effects))
            {
                try
                {
                    effects = JsonConvert.DeserializeObject<uint[]>(entity.Effects);
                }
                catch (Exception ex)
                {
                    logger.Warning(
                        ex,
                        "Unable to load effects from item={ItemId} effects={Effects}",
                        entity.Id,
                        entity.Effects
                    );
                }
            }

            Effects = new PlayerItemEffectCollection(this, effects);
            PurchaseDate = DateTimeOffset.FromUnixTimeSeconds(entity.PurchaseDate);
            _durability = entity.Durability;
            _enchantMP = (uint)entity.MP;
            _enchantLevel = (uint)entity.MPLevel;

            SetExistsState(true);
        }

        internal PlayerItem(GameDataService gameDataService, PlayerInventory inventory, long id,
            ShopItemInfo itemInfo, ShopPrice price,
            byte color, uint[] effects, DateTimeOffset purchaseDate)
        {
            _gameDataService = gameDataService;
            Inventory = inventory;
            Id = (ulong)id;
            ItemNumber = itemInfo.ShopItem.ItemNumber;
            PriceType = itemInfo.PriceGroup.PriceType;
            PeriodType = price.PeriodType;
            Period = price.Period;
            Color = color;
            Effects = new PlayerItemEffectCollection(this, effects);
            PurchaseDate = purchaseDate;
            _durability = price.PeriodType == ItemPeriodType.Units || price.PeriodType == ItemPeriodType.Days ? (int)price.Period : GetShopPrice().Durability;
            _enchantMP = 0;
            _enchantLevel = 0;
        }

        public ItemEffect[] GetItemEffects()
        {
            if (Effects.Count == 0)
                return null;

            return Effects.Select(x => _gameDataService.Effects.GetValueOrDefault(x)).ToArray();
        }

        public ShopItem GetShopItem()
        {
            return _gameDataService.GetShopItem(ItemNumber);
        }

        public ShopItemInfo GetShopItemInfo()
        {
            return _gameDataService.GetShopItemInfo(ItemNumber, PriceType);
        }

        public ShopPrice GetShopPrice(ushort period)
        {
            return GetShopItemInfo().PriceGroup.GetPrice(PeriodType, period);
        }

        public ShopPrice GetShopPrice()
        {
            return GetShopItemInfo().PriceGroup.GetPrice(PeriodType, Period);
        }

        public int LoseDurability(int loss)
        {
            if (loss < 0)
                throw new ArgumentOutOfRangeException(nameof(loss));

            if (Inventory.Player.Room == null)
                throw new InvalidOperationException("Player is not inside a room");

            if (Durability == -1 || loss == 0)
                return 0;

            loss = Math.Min(Durability, loss);
            Durability -= loss;
            return loss;
        }

        public int LoseDurability(int loss, bool istime)
        {
            Durability -= loss;
            return -1;
        }

        public uint CalculateRefund(ShopPrice price)
        {
            TimeSpan timeSpan = ExpireDate - DateTimeOffset.Now;
            switch (PeriodType)
            {
                case ItemPeriodType.None:
                    return (uint)((price != null ? price.Price : 4) / 4);
                case ItemPeriodType.Hours:
                    ShopPrice shopPrice1 = GetShopPrice((ushort)(Durability / 3600));
                    return shopPrice1 != null ? (uint)(shopPrice1.Price / 5) : (uint)(Durability / 200);
                case ItemPeriodType.Days:
                    switch (ItemNumber.Category)
                    {
                        case ItemCategory.Costume:
                            ShopPrice shopPrice2 = GetShopPrice((ushort)timeSpan.TotalDays);
                            if (shopPrice2 != null)
                                return (uint)(shopPrice2.Price / 5.011);
                            return PriceType == ItemPriceType.Premium ? (uint)((timeSpan.TotalDays < 1 ? -35 : timeSpan.TotalDays) + 35) : (uint)(timeSpan.TotalSeconds / 393.52);
                        case ItemCategory.Weapon:
                            ShopPrice shopPrice3 = GetShopPrice((ushort)timeSpan.TotalDays);
                            if (shopPrice3 != null && Effects.Contains(1299600001U))
                                return (uint)(shopPrice3.Price / 5);
                            return PriceType == ItemPriceType.Premium ? (uint)((timeSpan.TotalDays < 1 ? 7 : timeSpan.TotalDays) - 7) : (uint)(timeSpan.TotalSeconds / 81.076);
                        case ItemCategory.Skill:
                            ShopPrice shopPrice4 = GetShopPrice((ushort)timeSpan.TotalDays);
                            if (shopPrice4 != null)
                                return (uint)(shopPrice4.Price / 5);
                            return PriceType == ItemPriceType.Premium ? (uint)((timeSpan.TotalDays < 1 ? -9 : timeSpan.TotalDays) + 9) : (uint)(timeSpan.TotalSeconds / 30.662);
                        default:
                            return 1;
                    }
                default:
                    return 1;
            }
        }

        public uint CalculateRepair()
        {
            return PeriodType != ItemPeriodType.None ? 0 : (uint)(GetShopPrice().Durability - Durability);
        }
 
    }
}
