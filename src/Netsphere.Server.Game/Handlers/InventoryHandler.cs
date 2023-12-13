using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpressMapper.Extensions;
using Logging;
using Microsoft.Extensions.Options;
using Netsphere.Common.Configuration;
using Netsphere.Network;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Services;
using ProudNet;

namespace Netsphere.Server.Game.Handlers
{
    internal class InventoryHandler:
        IHandle<ItemUseItemReqMessage>,
        IHandle<ItemRepairItemReqMessage>,
        IHandle<ItemRefundItemReqMessage>,
        IHandle<ItemDiscardItemReqMessage>,
        IHandle<ItemUseCapsuleReqMessage>,
        IHandle<UseInstantItemReqMessage>,
        IHandle<UseInstantItemRemoveEffectReqMessage>
    {
        private readonly ILogger _logger;
        private readonly GameDataService _gameDataService;
        private readonly GameOptions _gameOptions;

        public InventoryHandler(
            ILogger<InventoryHandler> logger,
            GameDataService gameDataService,
            IOptions<GameOptions> gameOptions)
        {
            _logger = logger;
            _gameDataService = gameDataService;
            _gameOptions = gameOptions.Value;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, ItemUseItemReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var character = plr.CharacterManager[message.CharacterSlot];
            var item = plr.Inventory[message.ItemId];

            // This is a weird thing since newer seasons
            // The client sends a request with itemid 0 on login
            // and requires an answer to it for equipment to work properly
            if (message.Action == UseItemAction.UnEquip && message.ItemId == 0)
            {
                session.Send(new ItemUseItemAckMessage(
                    message.CharacterSlot,
                    message.EquipSlot,
                    message.ItemId,
                    message.Action
                ));
                return true;
            }

            if (character == null || item == null || plr.Room != null && plr.State != PlayerState.Lobby)
            {
                session.Send(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
                return true;
            }

            switch (message.Action)
            {
                case UseItemAction.Equip:
                    character.Equip(item, message.EquipSlot);
                    break;

                case UseItemAction.UnEquip:
                    character.UnEquip(item, message.EquipSlot);
                    break;
            }

            return true;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, ItemRepairItemReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var logger = plr.AddContextToLogger(_logger);

            foreach (var id in message.Items)
            {
                var item = session.Player.Inventory[id];
                if (item == null)
                {
                    logger.Warning("Item={ItemId} not found", id);
                    session.Send(new ItemRepairItemAckMessage(ItemRepairResult.Error0, 0));
                    return true;
                }

                if (item.Durability == -1)
                {
                    logger.Warning("Item={ItemId} can not be repaired", id);
                    session.Send(new ItemRepairItemAckMessage(ItemRepairResult.Error1, 0));
                    return true;
                }

                var cost = item.CalculateRepair();
                if (plr.PEN < cost)
                {
                    session.Send(new ItemRepairItemAckMessage(ItemRepairResult.NotEnoughMoney, 0));
                    return true;
                }

                var price = item.GetShopPrice();
                if (price == null)
                {
                    logger.Warning("No shop entry found item={ItemId}", id);
                    session.Send(new ItemRepairItemAckMessage(ItemRepairResult.Error2, 0));
                    return true;
                }

                if (item.Durability >= price.Durability)
                {
                    session.Send(new ItemRepairItemAckMessage(ItemRepairResult.OK, item.Id));
                    continue;
                }

                item.Durability = price.Durability;
                plr.PEN -= cost;

                session.Send(new ItemRepairItemAckMessage(ItemRepairResult.OK, item.Id));
                plr.SendMoneyUpdate();
            }

            return true;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, ItemRefundItemReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var item = plr.Inventory[message.ItemId];
            var logger = plr.AddContextToLogger(_logger);

            if (item == null)
            {
                logger.Warning("Item={ItemId} not found", message.ItemId);
                session.Send(new ItemRefundItemAckMessage(ItemRefundResult.Failed, 0));
                return true;
            }

            var price = item.GetShopPrice();
            if (price == null)
            {
                logger.Warning("No shop entry found item={ItemId}", message.ItemId);
                session.Send(new ItemRefundItemAckMessage(ItemRefundResult.Failed, 0));
                return true;
            }

            if (!price.CanRefund)
            {
                logger.Warning("Cannot refund item={ItemId}", message.ItemId);
                session.Send(new ItemRefundItemAckMessage(ItemRefundResult.Failed, 0));
                return true;
            }

            plr.PEN += item.CalculateRefund(price);
            plr.Inventory.Remove(item);

            session.Send(new ItemRefundItemAckMessage(ItemRefundResult.OK, item.Id));
            plr.SendMoneyUpdate();

            return true;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, ItemDiscardItemReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var item = plr.Inventory[message.ItemId];
            var logger = plr.AddContextToLogger(_logger);

            if (item == null)
            {
                logger.Warning("Item={ItemId} not found", message.ItemId);
                session.Send(new ItemDiscardItemAckMessage(2, 0));
                return true;
            }

            var shopItem = item.GetShopItem();
            if (shopItem == null)
            {
                logger.Warning("No shop entry found item={ItemId}", message.ItemId);
                session.Send(new ItemDiscardItemAckMessage(2, 0));
                return true;
            }

            if (!shopItem.IsDestroyable)
            {
                logger.Warning("Cannot discard item={ItemId}", message.ItemId);
                session.Send(new ItemDiscardItemAckMessage(2, 0));
                return true;
            }

            plr.Inventory.Remove(item);
            session.Send(new ItemDiscardItemAckMessage(0, item.Id));

            return true;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, UseInstantItemReqMessage message)
        {
            var session = context.GetSession<Session>();
            var player = session.Player;
            player.AddContextToLogger(_logger).ForContext("Message", message, true).Debug("OnHandle: {Message}");
            var playerItem1 = player.Inventory[message.InstantItemId];
            var playerItem2 = player.Inventory[message.ItemId];
            if (playerItem2 == null | playerItem1 == null)
            {
                session.Send(new UseInstantItemAckMessage(1));
                return true;
            }
            var enchantMasteryNeed = _gameDataService.GetEnchantMasteryNeed(playerItem2.EnchantLevel + 1, playerItem2.ItemNumber.Category);
            if (enchantMasteryNeed == null)
            {
                session.Send(new UseInstantItemAckMessage(1));
                return true;
            }
            if (playerItem2.EnchantMP >= enchantMasteryNeed.Durability)
            {
                session.Send(new UseInstantItemAckMessage(1));
                return true;
            }
            playerItem1.Durability--;
            if (playerItem1.Durability <= 0)
                player.Inventory.Remove(playerItem1);
            else
                player.Inventory.Update(playerItem1);
            playerItem2.EnchantMP += 168;
            if (playerItem2.EnchantMP > enchantMasteryNeed.Durability)
                playerItem2.EnchantMP = enchantMasteryNeed.Durability;
            session.Send(new UseInstantItemAckMessage(0));
            player.Inventory.Update(playerItem2);
            return true;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, ItemUseCapsuleReqMessage message)
        {
            var session = context.GetSession<Session>();
            var ascii = _gameOptions.Systems.EnabledCapsules;
            if (!ascii)
            {
                session.Send(new ItemUseCapsuleAckMessage(1));
                return true;
            }
            var plr = session.Player;
            var source = plr.Inventory[message.ItemId];
            var logger = plr.AddContextToLogger(_logger);

            if (source == null)
            {
                logger.Warning("Trying to use a one time charge with a non-existent item");
                session.Send(new ItemUseCapsuleAckMessage(1));
                return true;
            }

            var oneTimeCharge = _gameDataService.GetOneTimeCharge(source.ItemNumber);
            if (oneTimeCharge == null)
            {
                logger.Warning("Trying to use a one time charge with a non-existent item");
                session.Send(new ItemUseCapsuleAckMessage(1));
                return true;
            }

            if (source.Durability <= 1)
            {
                plr.Inventory.Remove(source);
            }
            else
            {
                source.Durability--;
                session.Send(new ItemUpdateInventoryAckMessage(InventoryAction.Update, source.Map<PlayerItem, ItemDto>()));
            }

            var rewards = new List<CapsuleRewardDto>();
            bool isBoostUnique = false;
            oneTimeCharge.OneTimeChargeCategories.ForEach(x => x.OneTimeChargeSubCategories.ForEach(y => y.GetItems(plr.BoosterInventory.GetItemNumbers()).ForEach(z =>
            {
                isBoostUnique |= z.BoostKey.Equals(5040006);
                rewards.Add(new CapsuleRewardDto()
                {
                    RewardType = x.Type,
                    PEN = z.Amount,
                    ItemNumber = z.Key,
                    PriceType = z.PriceType,
                    PeriodType = z.PeriodType,
                    Period = (uint)z.Period,
                    Color = (byte)z.Color,
                    Effect = z.PreviewEffect
                });
            })));

            var playerBoost = plr.BoosterInventory[5040006];
            if (playerBoost != null & isBoostUnique)
            {
                if (playerBoost.Item.Durability <= 1)
                {
                    plr.BoosterInventory.Remove(playerBoost.Item);
                    plr.Inventory.Remove(playerBoost.Item);
                }
                else
                {
                    playerBoost.Item.Durability--;
                    plr.Inventory.Update(playerBoost.Item);
                }
            }

            foreach (var capsuleRewardDto in rewards)
            {
                var i = capsuleRewardDto;
                if (i.RewardType == CapsuleRewardType.PEN)
                {
                    plr.PEN += i.PEN;
                }
                else
                {
                    var shopItemInfo = _gameDataService.GetShopItemInfo(i.ItemNumber, i.PriceType);
                    if (shopItemInfo == null)
                    {
                        logger.Warning("Trying to get non-existent item: ItemNumber {0} PriceType {1}", i.ItemNumber, i.PriceType);
                        session.Send(new ItemUseCapsuleAckMessage(1));
                        return true;
                    }
                    var price = shopItemInfo.PriceGroup.GetPrice(i.PeriodType, (ushort)i.Period);
                    if (price == null)
                    {
                        logger.Warning("Trying to get item with invalid price info");
                        session.Send(new ItemUseCapsuleAckMessage(1));
                        return true;
                    }
                    var gruopByPreviewEffect = _gameDataService.GetEffectGruopByPreviewEffect(i.Effect);
                    if (gruopByPreviewEffect == null)
                    {
                        logger.Warning("Trying to found effect with invalid effect Group");
                        session.Send(new ItemUseCapsuleAckMessage(1));
                        return true;
                    }
                    uint[] effects = gruopByPreviewEffect.Effects.Select(e => e.Effect).ToArray();
                    PlayerItem playerItem;
                    if ((playerItem = plr.Inventory.FirstOrDefault(x => x.ItemNumber == i.ItemNumber && x.PriceType == i.PriceType && (x.PeriodType == i.PeriodType && (int)x.Color == (int)i.Color) && x.Effects.Count == effects.Length)) != null)
                    {
                        switch (playerItem.PeriodType)
                        {
                            case ItemPeriodType.None:
                                plr.Inventory.Create(shopItemInfo, price, i.Color, effects);
                                continue;
                            case ItemPeriodType.Hours:
                                playerItem.Durability += 3600 * (int)i.Period;
                                plr.Inventory.Update(playerItem);
                                continue;
                            case ItemPeriodType.Days:
                            case ItemPeriodType.Units:
                                playerItem.Durability += (int)i.Period;
                                plr.Inventory.Update(playerItem);
                                continue;
                            default:
                                continue;
                        }
                    }
                    else
                        plr.Inventory.Create(shopItemInfo, price, i.Color, effects);
                }
            }
            session.Send(new ItemUseCapsuleAckMessage(rewards.ToArray(), (byte)3));
            plr.SendMoneyUpdate();
            return true;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, UseInstantItemRemoveEffectReqMessage message)
        {
            var session = context.GetSession<Session>();
            var player = session.Player;
            player.AddContextToLogger(_logger).ForContext("Message", message, true).Debug("OnHandle: {Message}");
            var playerItem1 = player.Inventory.GetItem(message.ItemRemoveEffectId);
            var playerItem2 = player.Inventory.GetItem(message.ItemId);
            if (playerItem2 == null | playerItem1 == null)
            {
                session.Send(new UseInstantItemRemoveEffectAckMessage(1));
                return true;
            }
            if (_gameDataService.GetEnchantMasteryNeed(playerItem2.EnchantLevel, playerItem2.ItemNumber.Category) == null)
            {
                session.Send(new UseInstantItemRemoveEffectAckMessage(1));
                return true;
            }
            playerItem1.Durability--;
            if (playerItem1.Durability < 1)
                player.Inventory.Remove(playerItem1);
            else
                player.Inventory.Update(playerItem1);
            playerItem2.Effects.Remove(message.EffectId);
            playerItem2.EnchantLevel--;
            playerItem2.EnchantMP = 0;
            player.Inventory.Update(playerItem2);
            session.Send(new UseInstantItemRemoveEffectAckMessage(playerItem2.Id, message.EffectId, (int)playerItem2.EnchantLevel, 0));
            return true;
        }

    }
}
