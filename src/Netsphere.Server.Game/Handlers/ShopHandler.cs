using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExpressMapper.Extensions;
using Logging;
using Netsphere.Common;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Data;
using Netsphere.Server.Game.Services;
using ProudNet;

namespace Netsphere.Server.Game.Handlers
{
    internal class ShopHandler :
        IHandle<ItemBuyItemReqMessage>,
        IHandle<ShoppingBasketActionReqMessage>,
        IHandle<ShoppingBasketDeleteReqMessage>
    {
        private readonly GameDataService _gameDataService;
        private readonly ILogger _logger;

        public ShopHandler(GameDataService gameDataService, ILogger<ShopHandler> logger)
        {
            _gameDataService = gameDataService;
            _logger = logger;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, ItemBuyItemReqMessage message)
        {
            var session = context.GetSession<Session>();
            var player = session.Player;
            var logger1 = player.AddContextToLogger(_logger);
            var groupings = ((IEnumerable<ShopItemDto>)message.Items).GroupBy(x => x);
            List<PlayerItem> source1 = new List<PlayerItem>();
            foreach (var source2 in groupings)
            {
                var logger2 = logger1.ForContext("@ItemToBuy", source2.Key, true);
                var key = source2.Key;
                int num1 = source2.Count();
                logger2.Debug("Trying to buy item");
                var shopItem = _gameDataService.GetShopItem(key.ItemNumber);
                if (shopItem == null)
                {
                    logger2.Warning("Trying to buy non-existant shopItem");
                    session.Send(new ItemBuyItemAckMessage(key, ItemBuyResult.UnkownItem));
                }
                else if (shopItem.MinLevel > player.Level || shopItem.MaxLevel != 0 && shopItem.MaxLevel < player.Level)
                {
                    logger2.Warning("Trying to buy item without meeting level requirements MinLevel={Minlevel} MaxLevel={MaxLevel} PlayerLevel={PlayerLevel}", shopItem.MinLevel, shopItem.MaxLevel, player.Level);
                    session.Send(new ItemBuyItemAckMessage(key, ItemBuyResult.UnkownItem));
                }
                else if ((int)key.Color > shopItem.ColorGroup)
                {
                    logger2.Warning("Trying to buy item with invalid color");
                    session.Send(new ItemBuyItemAckMessage(key, ItemBuyResult.UnkownItem));
                }
                else
                {
                    var itemInfo = shopItem.GetItemInfo(key.PriceType);
                    if (itemInfo == null)
                    {
                        logger2.Warning("Trying to buy non-existant item");
                        session.Send(new ItemBuyItemAckMessage(key, ItemBuyResult.UnkownItem));
                    }
                    else if (itemInfo.IsEnabled == ShopEnabledType.Off)
                    {
                        logger2.Warning("Trying to buy disabled item IsEnabled: {0} Off: {1}", itemInfo.IsEnabled, ShopEnabledType.Off);
                        session.Send(new ItemBuyItemAckMessage(key, ItemBuyResult.UnkownItem));
                    }
                    else
                    {
                        var price = itemInfo.PriceGroup.GetPrice(key.PeriodType, key.Period);
                        if (price == null)
                        {
                            logger2.Warning("Trying to buy item with invalid price info");
                            session.Send(new ItemBuyItemAckMessage(key, ItemBuyResult.UnkownItem));
                        }
                        else if (!price.IsEnabled)
                        {
                            logger2.Warning("Trying to buy item with disabled price info");
                            session.Send(new ItemBuyItemAckMessage(key, ItemBuyResult.UnkownItem));
                        }
                        else
                        {
                            var num2 = (uint)((price.Price + itemInfo.Discount) * num1);
                            switch (key.PriceType)
                            {
                                case ItemPriceType.PEN:
                                    if (player.PEN < num2)
                                    {
                                        logger2.Warning<uint>("Trying to buy item without enough PEN currentPEN={CurrentPEN}", player.PEN);
                                        session.Send(new ItemBuyItemAckMessage(key, ItemBuyResult.NotEnoughMoney));
                                        return true;
                                    }
                                    player.PEN -= num2;
                                    break;
                                case ItemPriceType.AP:
                                case ItemPriceType.Premium:
                                    if (player.AP < num2)
                                    {
                                        logger2.Warning("Trying to buy item without enough AP currentAP={CurrentAP}", player.AP);
                                        session.Send(new ItemBuyItemAckMessage(key, ItemBuyResult.NotEnoughMoney));
                                        return true;
                                    }
                                    player.AP -= num2;
                                    break;
                                default:
                                    logger2.Warning("Trying to buy item with invalid price type");
                                    session.Send(new ItemBuyItemAckMessage(key, ItemBuyResult.DBError));
                                    return true;
                            }
                            try
                            {
                                uint[] effects = Array.Empty<uint>();
                                if (itemInfo.EffectGroup.Effects.Count > 0)
                                    effects = itemInfo.EffectGroup.Effects.Select(x => x.Effect).Where(x => x > 0).ToArray();
                                for (int index = 0; index < num1; ++index)
                                {
                                    var playerItem = player.Inventory.GetItem(shopItem.ItemNumber.Id);
                                    if (playerItem != null && playerItem.PeriodType.Equals(ItemPeriodType.Units))
                                    {
                                        playerItem.Durability += key.Period;
                                        player.Inventory.Update(playerItem);
                                    }
                                    else
                                        playerItem = player.Inventory.Create(itemInfo, price, key.Color, effects);
                                    source1.Add(playerItem);
                                }
                            }
                            catch (Exception ex)
                            {
                                logger2.Error(ex, "Unable to create item");
                            }
                            ulong[] array = source1.Select(x => x.Id).ToArray();
                            session.Send(new ItemBuyItemAckMessage(array, key));
                            source1.Clear();
                            player.SendMoneyUpdate();
                        }
                    }
                }
            }
            return true;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, ShoppingBasketActionReqMessage message)
        {
            context.GetSession<Session>().Player.ShoppingBaskets.AddBasket(message.ItemNumber, message.PriceType, message.PeriodType, message.Period, message.Color, message.EffectId);
            return true;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, ShoppingBasketDeleteReqMessage message)
        {
            var player = context.GetSession<Session>().Player;
            foreach (ulong id in message.Items)
                player.ShoppingBaskets.Remove(id);
            return true;
        }

        public Task<bool> OnHandle(MessageContext context, ItemUseCapsuleReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var item = plr.Inventory[message.ItemId];
            var capsuleRewards = _gameDataService.CapsuleRewards;

            var path = Path.Combine(Environment.CurrentDirectory, "test", "removeCapsule.txt");

            item.Durability--;

            if (item.Durability <= 0)
                plr.Inventory.Remove(item);
            else
                session.Send(new ItemUpdateInventoryAckMessage(InventoryAction.Update, item.Map<PlayerItem, ItemDto>()));

            if (!capsuleRewards.ContainsKey(item.ItemNumber.Id))
            {
                if (!File.Exists(path))
                {
                    using (var sw = File.CreateText(path))
                        sw.WriteLine($"{item.ItemNumber.Id}");
                }
                else
                {
                    using (var sw = File.AppendText(path))
                        sw.WriteLine($"{item.ItemNumber.Id}");
                }
                _logger.Error("Unable to find capsule, invalid id {id}", item.ItemNumber.Id);
                session.Send(new ItemUseCapsuleAckMessage(7));

                return Task.FromResult(true);
            }

            var capsule = capsuleRewards[item.ItemNumber];
            var bags = capsule.Bags;
            var itemsExtracted = new List<CapsuleRewardDto>();
            foreach (var b in bags)
            {
                var extracted = NumberExtractorService.ExtractIndex(b.ItemRewards.Select(x => (int)x.Rate).ToArray());
                var extractedItem = b.ItemRewards[extracted];

                if (extractedItem != null)
                {
                    var capsuleReward = new CapsuleRewardDto
                    {
                        ItemNumber = extractedItem.Item.Id,
                        PeriodType = extractedItem.PeriodType,
                        PriceType = extractedItem.PriceType,
                        RewardType = extractedItem.Type,
                        Effect = extractedItem.Effects.First(),
                        Period = extractedItem.Value,
                        Unk1 = extractedItem.Color
                    };

                    if (capsuleReward.RewardType.Equals(CapsuleRewardType.PEN))
                    {
                        capsuleReward.PEN = extractedItem.Value;
                        plr.PEN += extractedItem.Value;
                    }

                    else
                    {

                        if (capsuleReward.ItemNumber.Id == 6000001 && plr.Inventory.Any(x => x.ItemNumber == capsuleReward.ItemNumber))
                        {
                            plr.Inventory.First(x => x.ItemNumber == capsuleReward.ItemNumber).Durability++;
                        }
                        else
                        {
                            ShopItemInfo itemInfo = null;
                            ShopPrice shopPrice = null;
                            foreach (var group in _gameDataService.ShopPrices.Values)
                            {
                                foreach (var price in group.Prices)
                                {
                                    if (price.Period == capsuleReward.Period && price.PeriodType == capsuleReward.PeriodType)
                                    {
                                        shopPrice = price;
                                        foreach (var info in _gameDataService.ShopItems.Values.First(x => x.ItemNumber == extractedItem.Item).ItemInfos)
                                        {
                                            foreach (var _price in info.PriceGroup.Prices)
                                            {
                                                if (_price == price)
                                                {
                                                    itemInfo = info;
                                                    break;
                                                }
                                            }
                                        }

                                        break;
                                    }
                                }
                            }

                            var _item = plr.Inventory.Create(itemInfo, shopPrice, extractedItem.Color, extractedItem.Effects, true);

                            if (_item == null)
                            {
                                _logger.Error($"Unable to create item {capsuleReward.ItemNumber.Id}");
                                session.Send(new ItemUseCapsuleAckMessage(7));

                                return Task.FromResult(true);
                            }
                        }
                    }

                    itemsExtracted.Add(capsuleReward);
                }
            }

            plr.SendMoneyUpdate();
            _logger.Information(plr.Account.Nickname + $" tried to open capsule with id: {item.ItemNumber.Id} \n  ");
            session.Send(new ItemUseCapsuleAckMessage(itemsExtracted.ToArray(), 3));
            return Task.FromResult(true);
        }
    }
}

