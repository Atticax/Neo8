using BlubLib.Collections.Concurrent;
using ExpressMapper.Extensions;
using Logging;
using Netsphere.Common;
using Netsphere.Database;
using Netsphere.Database.Game;
using Netsphere.Database.Helpers;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Game;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Netsphere.Server.Game
{
    public class PlayerShoppingBasket :
        IReadOnlyCollection<ShoppingBasket>,
        IEnumerable<ShoppingBasket>,
        IEnumerable
    {
        private readonly IdGeneratorService _idGeneratorService;
        private readonly ConcurrentDictionary<ulong, ShoppingBasket> _items;
        private readonly ConcurrentStack<ShoppingBasket> _itemsToRemove;
        private ILogger _logger;

        public Player Player { get; private set; }

        public int Count => _items.Count;

        public PlayerShoppingBasket(
          ILogger<PlayerShoppingBasket> logger,
          IdGeneratorService idGeneratorService)
        {
            _logger = logger;
            _idGeneratorService = idGeneratorService;
            _items = new ConcurrentDictionary<ulong, ShoppingBasket>();
            _itemsToRemove = new ConcurrentStack<ShoppingBasket>();
        }

        internal void Initialize(Player plr, PlayerEntity entity)
        {
            Player = plr;
            _logger = plr.AddContextToLogger(_logger);
            foreach (var shoppingBasket in entity.ShoppingBaskets.Select(x => new ShoppingBasket(x)))
                _items.TryAdd(shoppingBasket.Id, shoppingBasket);
            _logger.Information("basket loaded {Count}", _items.Count);
        }

        public ShoppingBasket GetItem(ulong id)
        {
            ShoppingBasket shoppingBasket;
            _items.TryGetValue(id, out shoppingBasket);
            return shoppingBasket;
        }

        public ShoppingBasket AddBasket(int itemNumber, int priceType, int periodType, short period, byte color, uint effectId)
        {
            var nextId = _idGeneratorService.GetNextId(IdKind.ShoppingBasket);
            var source = new ShoppingBasket(nextId, itemNumber, priceType, periodType, period, color, effectId);
            _items.TryAdd(source.Id, source);
            _logger.Information("Añadió a su lista de deseos itemId={ItemId}", nextId);
            Player.Session.Send(new ShoppingBasketActionAckMessage(1, 1, source.Map<ShoppingBasket, ShoppingBasketDto>()));
            return source;
        }

        public bool Remove(ShoppingBasket basket) => Remove(basket.Id);

        public bool Remove(ulong id)
        {
            var source = GetItem(id);
            if (source == null)
                return false;
            _items.Remove(source.Id);
            if (source.Exists)
                _itemsToRemove.Push(source);
            _logger.Information("Removió de su lista de deseos itemId={ItemId}", source.Id);
            Player.Session.Send(new ShoppingBasketActionAckMessage(3, 3, source.Map<ShoppingBasket, ShoppingBasketDto>()));
            return true;
        }

        public void SendBasketList() => Player.Session.Send(new ShoppingBasketListInfoAckMessage(this.Select(x => x.Map<ShoppingBasket, ShoppingBasketDto>()).ToArray()));

        public async Task Save(GameContext db)
        {
            if (!_itemsToRemove.IsEmpty)
            {
                var idsToRemove = new List<long>();
                ShoppingBasket result;
                while (_itemsToRemove.TryPop(out result))
                    idsToRemove.Add((long)result.Id);
                int num = await BatchDeleteExtensions.DeleteAsync(db.PlayerShoppingBaskets.Where(x => idsToRemove.Contains(x.Id)), new CancellationToken());
            }
            foreach (var shoppingBasket in _items.Values)
            {
                if (!shoppingBasket.Exists)
                {
                    db.PlayerShoppingBaskets.Add(new PlayerShoppingBasketEntity()
                    {
                        Id = (long)shoppingBasket.Id,
                        PlayerId = (int)Player.Account.Id,
                        ShopItemId = (long)shoppingBasket.ItemNumber.Id,
                        ShopPriceType = (int)shoppingBasket.PriceType,
                        ShopPeriodType = (int)shoppingBasket.PeriodType,
                        ShopPeriod = (int)shoppingBasket.Period,
                        ShopColor = (byte)shoppingBasket.Color,
                        ShopEffectId = (int)shoppingBasket.EffectId
                    });
                    shoppingBasket.SetExistsState(true);
                }
            }
        }

        public bool Contains(ulong id) => _items.ContainsKey(id);

        public IEnumerator<ShoppingBasket> GetEnumerator() => _items.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ShoppingBasket : DatabaseObject
    {
        public ulong Id { get; }

        public ItemNumber ItemNumber { get; }

        public ItemPriceType PriceType { get; }

        public ItemPeriodType PeriodType { get; }

        public ushort Period { get; }

        public byte Color { get; }

        public uint EffectId { get; }

        public ShopItemDto GetShopItemDto() => new ShopItemDto()
        {
            ItemNumber = ItemNumber,
            PriceType = PriceType,
            PeriodType = PeriodType,
            Period = Period,
            Color = Color,
            Effect = EffectId
        };

        internal ShoppingBasket(PlayerShoppingBasketEntity entity)
        {
            Id = (ulong)entity.Id;
            ItemNumber = new ItemNumber(entity.ShopItemId);
            PriceType = (ItemPriceType)entity.ShopPriceType;
            PeriodType = (ItemPeriodType)entity.ShopPeriodType;
            Period = (ushort)entity.ShopPeriod;
            Color = entity.ShopColor;
            EffectId = (uint)entity.ShopEffectId;
            SetExistsState(true);
        }

        internal ShoppingBasket(
          long id,
          int itemNumber,
          int priceType,
          int periodType,
          short period,
          byte color,
          uint effectId)
        {
            Id = (ulong)id;
            ItemNumber = new ItemNumber((long)itemNumber);
            PriceType = (ItemPriceType)priceType;
            PeriodType = (ItemPeriodType)periodType;
            Period = (ushort)period;
            Color = color;
            EffectId = effectId;
            SetExistsState(false);
        }
    }
}
