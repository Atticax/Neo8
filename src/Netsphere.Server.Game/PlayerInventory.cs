using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlubLib.Collections.Concurrent;
using ExpressMapper.Extensions;
using Logging;
using Netsphere.Common;
using Netsphere.Database;
using Netsphere.Database.Game;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Data;
using Netsphere.Server.Game.Services;
using Newtonsoft.Json;
using Z.EntityFramework.Plus;

namespace Netsphere.Server.Game
{
    public class PlayerInventory : IReadOnlyCollection<PlayerItem>
    {
        private readonly GameDataService _gameDataService;
        private readonly DatabaseService _databaseService;
        private readonly IdGeneratorService _idGeneratorService;
        private readonly ConcurrentDictionary<ulong, PlayerItem> _items;
        private readonly ConcurrentStack<PlayerItem> _itemsToRemove;
        private ILogger _logger;

        public Player Player { get; private set; }
        public int Count => _items.Count;

        /// <summary>
        /// Returns the item with the given id or null if not found
        /// </summary>
        public PlayerItem this[ulong id] => GetItem(id);

        public PlayerInventory(ILogger<PlayerInventory> logger, GameDataService gameDataService, DatabaseService databaseService,
            IdGeneratorService idGeneratorService)
        {
            _logger = logger;
            _gameDataService = gameDataService;
            _databaseService = databaseService;
            _idGeneratorService = idGeneratorService;
            _items = new ConcurrentDictionary<ulong, PlayerItem>();
            _itemsToRemove = new ConcurrentStack<PlayerItem>();
        }

        internal void Initialize(Player plr, PlayerEntity entity)
        {
            Player = plr;
            _logger = plr.AddContextToLogger(_logger);

            foreach (var item in entity.Items.Select(x => new PlayerItem(_logger, _gameDataService, this, x)))
                _items.TryAdd(item.Id, item);
        }

        /// <summary>
        /// Returns the item with the given id or null if not found
        /// </summary>
        public PlayerItem GetItem(ulong id)
        {
            _items.TryGetValue(id, out var item);
            return item;
        }

        public PlayerItem GetItem(ItemNumber itemNumber) => _items.Values.FirstOrDefault(x => x.ItemNumber.Equals(itemNumber));

        public PlayerItem GetIId(ulong id)
        {
            PlayerItem item;
            _items.TryGetValue(id, out item);
            return item;
        }

        public PlayerItem GetItem(uint shopItemId)
        {
            try
            {
                var item = _items.Values.Where(x => x.GetShopItemInfo().Id == shopItemId);
                return item.LastOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public PlayerItem GetItemDay(uint shopItemId)
        {
            try
            {
                var item = _items.Values.Where(x => x.GetShopItemInfo().Id == shopItemId && x.PeriodType == ItemPeriodType.Days);
                return item.LastOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public PlayerItemEntity GetId()
        {
            PlayerItemEntity playerItem;
            using (var db = _databaseService.Open<GameContext>())
            {
                playerItem = db.PlayerItems.OrderByDescending(x => x.Id).FirstOrDefault();
            }
            return playerItem;
        }



        /// <summary>
        /// Creates a new item
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public PlayerItem Create(ItemNumber itemNumber, ItemPriceType priceType, ItemPeriodType periodType, ushort period,
            byte color, uint[] effects, bool sendUpdate = true)
        {
            // TODO Remove exceptions and instead return a error code

            var shopItemInfo = _gameDataService.GetShopItemInfo(itemNumber, priceType);
            if (shopItemInfo == null)
                throw new ArgumentException("Item not found");

            var price = shopItemInfo.PriceGroup.GetPrice(periodType, period);
            if (price == null)
                throw new ArgumentException("Price not found");

            return Create(shopItemInfo, price, color, effects, sendUpdate);
        }

        /// <summary>
        /// Creates a new item
        /// </summary>
        public PlayerItem Create(ShopItemInfo shopItemInfo, ShopPrice price, byte color, uint[] effects, bool sendUpdate = true)
        {
            var item = new PlayerItem(_gameDataService, this, _idGeneratorService.GetNextId(IdKind.Item), shopItemInfo, price, color, effects, DateTimeOffset.Now);
            _items.TryAdd(item.Id, item);
            if (sendUpdate)
                Player.Session.Send(new ItemUpdateInventoryAckMessage(InventoryAction.Add, item.Map<PlayerItem, ItemDto>()));
            return item;

            //var itemId = _idGeneratorService.GetNextId(IdKind.Item);
            //var item = new PlayerItem(_gameDataService, this,
            //    itemId, shopItemInfo, price, color, effects, DateTimeOffset.Now);
            //_items.TryAdd(item.Id, item);

            //if (sendUpdate)
            //    Player.Session.Send(new ItemUpdateInventoryAckMessage(InventoryAction.Add, item.Map<PlayerItem, ItemDto>()));

            //return item;
        }

        public void Update(PlayerItem item) {
            Player.Session.Send(new ItemUpdateInventoryAckMessage(InventoryAction.Update, item.Map<PlayerItem, ItemDto>()));
        }

        /// <summary>
        /// Removes the item from the inventory
        /// </summary>
        public bool Remove(PlayerItem item)
        {
            return Remove(item.Id);
        }

        /// <summary>
        /// Removes the item from the inventory
        /// </summary>
        public bool Remove(ulong id)
        {
            var item = GetItem(id);
            if (item == null)
                return false;

            _items.Remove(item.Id);
            if (item.Exists)
                _itemsToRemove.Push(item);

            Player.Session.Send(new ItemInventroyDeleteAckMessage(item.Id));
            return true;
        }

        public bool RemoveOfCharacterIfNeed(PlayerItem item)
        {
            return RemoveOfCharacterIfNeed(item.Id);
        }

        public bool RemoveOfCharacterIfNeed(ulong id)
        {
            var playerItem = GetItem(id);
            if (playerItem == null)
                return false;
            foreach (var character in Player.CharacterManager)
            {
                switch (playerItem.ItemNumber.Category)
                {
                    case ItemCategory.Costume:
                        int itemSlot1 = character.Costumes.GetItemSlot(playerItem);
                        if (itemSlot1 >= 0)
                        {
                            int num = (int)character.Costumes.Remove((byte)itemSlot1);
                            continue;
                        }
                        continue;
                    case ItemCategory.Weapon:
                        int itemSlot2 = character.Weapons.GetItemSlot(playerItem);
                        if (itemSlot2 >= 0)
                        {
                            int num = (int)character.Weapons.Remove((byte)itemSlot2);
                            continue;
                        }
                        continue;
                    case ItemCategory.Skill:
                        int itemSlot3 = character.Skills.GetItemSlot(playerItem);
                        if (itemSlot3 >= 0)
                        {
                            int num = (int)character.Skills.Remove((byte)itemSlot3);
                            continue;
                        }
                        continue;
                    case ItemCategory.Boost:
                        Player.BoosterInventory.Remove(playerItem);
                        continue;
                    default:
                        continue;
                }
            }
            _items.Remove(playerItem.Id);
            if (playerItem.Exists)
                _itemsToRemove.Push(playerItem);
            Player.Session.Send(new ItemInventroyDeleteAckMessage(playerItem.Id));
            return true;
        }

        public async Task Save(GameContext db)
        {
            foreach (var playerItem in _items.Values.Where(x => x.PeriodType == ItemPeriodType.Hours || x.PeriodType == ItemPeriodType.Days))
            {
                if (Player.Room != null)
                {
                    if (Player.State != PlayerState.Lobby)
                        break;
                }
                switch (playerItem.PeriodType)
                {
                    case ItemPeriodType.Hours:
                        if (playerItem.Durability <= 0)
                        {
                            RemoveOfCharacterIfNeed(playerItem);
                            continue;
                        }
                        continue;
                    case ItemPeriodType.Days:
                        if (DateTimeOffset.Now >= playerItem.ExpireDate)
                        {
                            RemoveOfCharacterIfNeed(playerItem);
                            continue;
                        }
                        continue;
                    default:
                        continue;
                }
            }

            if (!_itemsToRemove.IsEmpty)
            {
                var idsToRemove = new List<long>();
                PlayerItem result;
                while (_itemsToRemove.TryPop(out result))
                    idsToRemove.Add((long)result.Id);
                int num = await db.PlayerItems.Where(x => idsToRemove.Contains(x.Id)).DeleteAsync();
            }
            foreach (var playerItem in _items.Values)
            {
                try
                {
                    if (!playerItem.Exists)
                    {
                        db.PlayerItems.Add(new PlayerItemEntity()
                        {
                            Id = (long)playerItem.Id,
                            PlayerId = (int)Player.Account.Id,
                            ShopItemInfoId = playerItem.GetShopItemInfo().Id,
                            ShopPriceId = playerItem.GetShopPrice().Id,
                            Effects = JsonConvert.SerializeObject(playerItem.Effects.ToArray()),
                            Color = playerItem.Color,
                            PurchaseDate = playerItem.PurchaseDate.ToUnixTimeSeconds(),
                            Durability = playerItem.Durability,
                            MP = (int)playerItem.EnchantMP,
                            MPLevel = (int)playerItem.EnchantLevel
                        });
                        playerItem.SetExistsState(true);
                    }
                    else if (playerItem.IsDirty)
                    {
                        db.Update(new PlayerItemEntity()
                        {
                            Id = (long)playerItem.Id,
                            PlayerId = (int)Player.Account.Id,
                            ShopItemInfoId = playerItem.GetShopItemInfo().Id,
                            ShopPriceId = playerItem.GetShopPrice().Id,
                            Effects = JsonConvert.SerializeObject(playerItem.Effects.ToArray()),
                            Color = playerItem.Color,
                            PurchaseDate = playerItem.PurchaseDate.ToUnixTimeSeconds(),
                            Durability = playerItem.Durability,
                            MP = (int)playerItem.EnchantMP,
                            MPLevel = (int)playerItem.EnchantLevel
                        });
                        playerItem.SetDirtyState(false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warning("Invalid save itemId={Id}", playerItem != null ? playerItem.Id : 0);
                }
            }
        }

        public bool Contains(ulong id)
        {
            return _items.ContainsKey(id);
        }

        public IEnumerator<PlayerItem> GetEnumerator()
        {
            return _items.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
