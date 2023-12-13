using Logging;
using Netsphere.Common;
using Netsphere.Database;
using Netsphere.Database.Game;
using Netsphere.Database.Helpers;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Services;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Netsphere.Server.Game
{
    public class PlayerBoosterInventory :
    IReadOnlyCollection<PlayerBoost>,
    IEnumerable<PlayerBoost>,
    IEnumerable
    {
        private ILogger _logger;
        private readonly GameDataService _gameDataService;
        private readonly IdGeneratorService _idGeneratorService;
        private readonly ConcurrentDictionary<byte, PlayerBoost> _items;
        private readonly ConcurrentStack<PlayerBoost> _itemsToRemove;

        public Player Player { get; private set; }

        public int Count => _items.Count;

        public PlayerBoost this[byte slot] => GetBoost(slot);

        public PlayerBoost this[uint shopItemId] => GetBoost(shopItemId);

        public PlayerBoosterInventory(
          ILogger<PlayerBoosterInventory> logger,
          GameDataService gameDataService,
          IdGeneratorService idGeneratorService)
        {
            _logger = (ILogger)logger;
            _gameDataService = gameDataService;
            _idGeneratorService = idGeneratorService;
            _items = new ConcurrentDictionary<byte, PlayerBoost>();
            _itemsToRemove = new ConcurrentStack<PlayerBoost>();
        }

        internal void Initialize(Player plr, PlayerInventory inventory, PlayerEntity entity)
        {
            Player = plr;
            _logger = plr.AddContextToLogger(_logger);
            foreach (var playerBoosterEntity in entity.Boosters.OrderBy(x => x.Slot))
            {
                if (playerBoosterEntity.BoostId.HasValue)
                {
                    var playerItem = inventory.GetItem(playerBoosterEntity.BoostId.Value);
                    if (playerItem != null)
                    {
                        playerItem.IsBoost = true;
                        _items.TryAdd(playerBoosterEntity.Slot, new PlayerBoost(playerBoosterEntity.Id, playerBoosterEntity.Slot, playerItem, true));
                    }
                }
            }
        }

        public CharacterInventoryError CharacterAdd(PlayerItem item)
        {
            var boostSlot = GetBoostSlot(item.ItemNumber);
            if (!IsSlotValid(boostSlot))
                return CharacterInventoryError.InvalidSlot;
            if (_items.ContainsKey(boostSlot))
                return CharacterInventoryError.SlotAlreadyInUse;
            if (!item.ItemNumber.Category.Equals(ItemCategory.Boost))
                return CharacterInventoryError.ItemNotAllowed;
            if (GetBoost(item.ItemNumber.Id) != null)
                return CharacterInventoryError.ItemAlreadyInUse;
            var nextId = _idGeneratorService.GetNextId(IdKind.Boost);
            item.IsBoost = true;
            _items.TryAdd(boostSlot, new PlayerBoost(nextId, boostSlot, item, false));
            Player.Session.Send(new ItemUseItemAckMessage(0, boostSlot, item.Id, UseItemAction.Equip));
            return CharacterInventoryError.OK;
        }

        public CharacterInventoryError CharacterRemove(PlayerItem item)
        {
            var boostSlot = GetBoostSlot(item.ItemNumber);
            if (!IsSlotValid(boostSlot))
                return CharacterInventoryError.InvalidSlot;
            PlayerBoost playerBoost;
            if (!_items.TryRemove(boostSlot, out playerBoost))
                return CharacterInventoryError.ItemNotAllowed;
            playerBoost.Item.IsBoost = false;
            _itemsToRemove.Push(playerBoost);
            Player.Session.Send(new ItemUseItemAckMessage(0, playerBoost.Slot, playerBoost.Item.Id, UseItemAction.UnEquip));
            return CharacterInventoryError.OK;
        }

        public bool Remove(PlayerItem item) => Remove(item.ItemNumber.Id);

        public bool Remove(uint shopItemId)
        {
            var boost = GetBoost(shopItemId);
            if (boost == null)
                return false;
            int num = (int)CharacterRemove(boost.Item);
            return true;
        }

        public (PlayerItem, CharacterInventoryError) GetItem(byte slot) => !IsSlotValid(slot) ? ((PlayerItem)null, CharacterInventoryError.InvalidSlot) : (_items[slot].Item, CharacterInventoryError.OK);

        public PlayerBoost GetBoost(byte slot)
        {
            PlayerBoost playerBoost;
            if (!_items.TryGetValue(slot, out playerBoost))
                playerBoost = null;
            return playerBoost;
        }

        public PlayerBoost GetBoost(uint shopItemId) => _items.Values.FirstOrDefault(x => x.Item.ItemNumber.Equals(shopItemId));

        public (BonusEtc, uint) GetBoostBonusEtc(uint penGain, uint bonusGain, bool isFriendly)
        {
            if (isFriendly)
                return (BonusEtc.None, bonusGain);
            var itemNumbers = GetItemNumbers();
            var bonusEtc = BonusEtc.None;
            double num;
            if (Player.Level >= 30)
            {
                bonusEtc |= BonusEtc.Thirty;
                num = 0.3;
            }
            else if (Player.Level >= 20)
            {
                bonusEtc |= BonusEtc.TwentyFive;
                num = 0.25;
            }
            else if (Player.Level >= 10)
            {
                bonusEtc |= BonusEtc.Twenty;
                num = 0.2;
            }
            else
                num = 0;
            if (itemNumbers.Length != 0)
            {
                if (((IEnumerable<ItemNumber>)itemNumbers).Any(x => x.SubCategory.Equals(0)))
                    bonusEtc |= BonusEtc.PENPlus;
                if (((IEnumerable<ItemNumber>)itemNumbers).Any(x => x.SubCategory.Equals(2)))
                    bonusEtc |= BonusEtc.EXPPlus;
            }
            return (bonusEtc, (uint)(penGain * num) + bonusGain);
        }

        public uint GetBonusMP(uint masteryGain)
        {
            var itemNumbers = GetItemNumbers();
            if (itemNumbers.Length != 0 && ((IEnumerable<ItemNumber>)itemNumbers).Any(x => x.SubCategory.Equals(3)))
            {
                if (((IEnumerable<ItemNumber>)itemNumbers).Any(x => x.Id.Equals(5030002)))
                    return masteryGain * 2U;
                if (((IEnumerable<ItemNumber>)itemNumbers).Any(x => x.Id.Equals(5030001)))
                    return (uint)(masteryGain * 0.2);
            }
            return masteryGain;
        }

        public uint GetBonusEXP(uint expGain, uint bonusGain, bool isFriendly)
        {
            if (isFriendly)
                return bonusGain;
            var itemNumbers = GetItemNumbers();
            if (itemNumbers.Length != 0 && ((IEnumerable<ItemNumber>)itemNumbers).Any(x => x.SubCategory.Equals(2)))
            {
                if (((IEnumerable<ItemNumber>)itemNumbers).Any(x => x.Id.Equals(5020001)))
                    return (uint)(expGain * 0.3) + bonusGain;
                if (((IEnumerable<ItemNumber>)itemNumbers).Any(x => x.Id.Equals(5020002)))
                    return (uint)(expGain * 0.2) + bonusGain;
                if (((IEnumerable<ItemNumber>)itemNumbers).Any(x => x.Id.Equals(5020003)))
                    return (uint)(expGain * 0.1) + bonusGain;
            }
            return bonusGain;
        }

        public uint GetBonusPEN(uint penGain, uint bonusGain, bool isFriendly)
        {
            if (isFriendly)
                return bonusGain;
            var itemNumbers = GetItemNumbers();
            if (itemNumbers.Length != 0 && ((IEnumerable<ItemNumber>)itemNumbers).Any(x => x.SubCategory.Equals(0)))
            {
                if (((IEnumerable<ItemNumber>)itemNumbers).Any(x => x.Id.Equals(5000002)))
                    return penGain + bonusGain;
                if (((IEnumerable<ItemNumber>)itemNumbers).Any(x => x.Id.Equals(5000010)))
                    return (uint)(penGain * 0.9) + bonusGain;
                if (((IEnumerable<ItemNumber>)itemNumbers).Any(x => x.Id.Equals(5000009)))
                    return (uint)(penGain * 0.8) + bonusGain;
                if (((IEnumerable<ItemNumber>)itemNumbers).Any(x => x.Id.Equals(5000008)))
                    return (uint)(penGain * 0.7) + bonusGain;
                if (((IEnumerable<ItemNumber>)itemNumbers).Any(x => x.Id.Equals(5000007)))
                    return (uint)(penGain * 0.6) + bonusGain;
                if (((IEnumerable<ItemNumber>)itemNumbers).Any(x => x.Id.Equals(5000001)))
                    return (uint)(penGain * 0.5) + bonusGain;
                if (((IEnumerable<ItemNumber>)itemNumbers).Any(x => x.Id.Equals(5000006)))
                    return (uint)(penGain * 0.4) + bonusGain;
                if (((IEnumerable<ItemNumber>)itemNumbers).Any(x => x.Id.Equals(5000005)))
                    return (uint)(penGain * 0.3) + bonusGain;
                if (((IEnumerable<ItemNumber>)itemNumbers).Any(x => x.Id.Equals(5000004)))
                    return (uint)(penGain * 0.2) + bonusGain;
                if (((IEnumerable<ItemNumber>)itemNumbers).Any(x => x.Id.Equals(5000003)))
                    return (uint)(penGain * 0.1) + bonusGain;
            }
            return bonusGain;
        }

        public PlayerItem[] GetItems() => _items.Values.Select(x => x.Item).ToArray();

        public ItemNumber[] GetItemNumbers() => _items.Values.Select(x => x.Item.ItemNumber).ToArray();

        public int GetItemSlot(PlayerItem item)
        {
            var items = GetItems();
            for (int index = 0; index < items.Length; index++)
            {
                var playerItem = items[index];
                if (playerItem != null && playerItem.ItemNumber == item.ItemNumber && (playerItem.PriceType == item.PriceType && playerItem.PeriodType == item.PeriodType))
                    return index;
            }
            return -1;
        }

        public async Task Save(GameContext db)
        {
            if (!_itemsToRemove.IsEmpty)
            {
                var idsToRemove = new List<long>();
                PlayerBoost result;
                while (_itemsToRemove.TryPop(out result))
                    idsToRemove.Add(result.Id);
                int num = await BatchDeleteExtensions.DeleteAsync(db.PlayerBoosters.Where(x => idsToRemove.Contains(x.Id)), new CancellationToken());
            }
            foreach (var playerBoost in _items.Values)
            {
                if (!playerBoost.Exists)
                {
                    db.PlayerBoosters.Add(new PlayerBoosterEntity()
                    {
                        Id = playerBoost.Id,
                        PlayerId = (int)Player.Account.Id,
                        Slot = playerBoost.Slot,
                        BoostId = new long?((long)playerBoost.Item.Id)
                    });
                    playerBoost.SetExistsState(true);
                }
                else if (playerBoost.IsDirty)
                {
                    db.PlayerBoosters.Update(new PlayerBoosterEntity()
                    {
                        Id = playerBoost.Id,
                        PlayerId = (int)Player.Account.Id,
                        Slot = playerBoost.Slot,
                        BoostId = new long?((long)playerBoost.Item.Id)
                    });
                    playerBoost.SetDirtyState(false);
                }
            }
        }

        public byte GetBoostSlot(ItemNumber itemNumber) => itemNumber.SubCategory.Equals(0) ? (byte)0 : (byte)(itemNumber.SubCategory - 1);

        public bool Contains(byte slot) => _items.ContainsKey(slot);

        public void Clear() => _items.Clear();

        public IEnumerator<PlayerBoost> GetEnumerator() => _items.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsSlotValid(byte slot) => slot < 4;
    }

    public class PlayerBoost : DatabaseObject
    {
        public long Id { get; }

        public byte Slot { get; }

        public PlayerItem Item { get; }

        public PlayerBoost(long id, byte slot, PlayerItem playerItem, bool existState)
        {
            Id = id;
            Slot = slot;
            Item = playerItem;
            SetExistsState(existState);
        }
    }
}
