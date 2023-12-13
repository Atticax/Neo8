using ExpressMapper.Extensions;
using Foundatio.Messaging;
using Logging;
using Microsoft.Extensions.Options;
using Netsphere.Common.Configuration;
using Netsphere.Common.Messaging;
using Netsphere.Network;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Data;
using Netsphere.Server.Game.Rules;
using Netsphere.Server.Game.Services;
using ProudNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Netsphere.Server.Game.Handlers
{
    internal class GameOptionHandler :
        IHandle<NoteGiftItemReqMessage>,
        IHandle<NoteGiftItemGainReqMessage>,
        IHandle<CardGambleReqMessage>,
        IHandle<ItemEnchanReqMessage>,
        IHandle<EsperEnchantReqMessage>,
        IHandle<ItemUseEsperChipReqMessage>,
        IHandle<ItemUseRecordResetReqMessage>,
        IHandle<ItemUseJewelItemReqMessage>,
        IHandle<EsperEnchantPercentReqMessage>,
        IHandle<AlchemyCombinationReqMessage>,
        IHandle<AlchemyDecompositionReqMessage>,
        IHandle<UseInstantItemReqMessage>,
        IHandle<BtcClearReqMessage>,
        IHandle<PromotionNewYearCardUseReqMessage>
    {
        private readonly ILogger _logger;
        private readonly IMessageBus _messageBus;
        private readonly PlayerManager _playerManager;
        private readonly GameDataService _gameDataService;
        private readonly SystemsOptions _systemsOptions;

        public GameOptionHandler(
          ILogger<GameOptionHandler> logger,
          IMessageBus messageBus,
          PlayerManager playerManager,
          GameDataService gameDataService,
          IOptions<SystemsOptions> systemOptions)
        {
            _logger = (ILogger)logger;
            _messageBus = messageBus;
            _playerManager = playerManager;
            _gameDataService = gameDataService;
            _systemsOptions = systemOptions.Value;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, NoteGiftItemReqMessage message)
        {
            var session = context.GetSession<Session>();
            var plr = session.Player;
            var log = plr.AddContextToLogger(_logger).ForContext("Message", message, true);
            var target = _playerManager.GetByNickname(message.ToNickname);
            if (target == null)
            {
                log.Warning("Player={name} not found", message.ToNickname);
                session.Send(new ServerResultAckMessage(ServerResult.PlayerNotFound));
                return true;
            }
            var itemInfo = _gameDataService.GetItemInfo(message.ShopItem.ItemNumber);
            if (itemInfo == null)
            {
                log.Warning("Not foun item={Number}", message.ShopItem.ItemNumber.Id);
                session.Send(new NoteGiftItemAckMessage(NoteGiftResult.Error));
                return true;
            }
            if (message.Subject.Length > 100)
            {
                log.Warning("Title is too big({Length})", message.Subject.Length);
                session.Send(new NoteGiftItemAckMessage(NoteGiftResult.Error));
                return true;
            }
            string messsage = message.Message + "{N}" + itemInfo.Name;
            if (messsage.Length > 100)
            {
                log.Warning("Message is too big({Length})", message.Message.Length);
                session.Send(new NoteGiftItemAckMessage(NoteGiftResult.Error));
                return true;
            }
            var shopItemInfo = _gameDataService.GetShopItemInfo(message.ShopItem.ItemNumber, message.ShopItem.PriceType);
            if (shopItemInfo == null)
            {
                log.Warning("Item={Id} not exists on database", message.ShopItem.ItemNumber);
                session.Send(new ServerResultAckMessage(ServerResult.DBError));
                return true;
            }
            var shopPrice = shopItemInfo.PriceGroup.GetPrice(message.ShopItem.PeriodType, message.ShopItem.Period);
            if (shopPrice == null)
            {
                log.Warning("Item={Id} not exists on shopPrice", message.ShopItem.ItemNumber);
                session.Send(new ServerResultAckMessage(ServerResult.DBError));
                return true;
            }
            if ((long)shopPrice.Price > (long)plr.AP)
            {
                log.Warning("Not enogth AP");
                session.Send(new NoteGiftItemAckMessage(NoteGiftResult.NotEnogthAP));
                return true;
            }
            var shopGroupEffect = _gameDataService.GetEffectGruopByPreviewEffect(message.ShopItem.Effect);
            if (shopGroupEffect == null)
            {
                log.Warning("Not found effect={Id}", message.ShopItem.Effect);
                return true;
            }
            if ((await Common.MessageBusExtensions.PublishRequestAsync<PlayerMailboxRequest, PlayerMailboxResponse>(_messageBus, new PlayerMailboxRequest(plr.Account.Nickname, target.Account.Nickname, message.Subject, messsage, MailType.Gift))).MailId == 0)
            {
                log.Warning("Failed to send mail");
                session.Send(new NoteGiftItemAckMessage(NoteGiftResult.Error2));
                return true;
            }
            target.Inventory.Create(shopItemInfo, shopPrice, message.ShopItem.Color, shopGroupEffect.GetEffects());
            plr.AP -= (uint)shopPrice.Price;
            plr.SendMoneyUpdate();
            session.Send(new NoteGiftItemAckMessage(NoteGiftResult.Success));
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, NoteGiftItemGainReqMessage message)
        {
            var session = context.GetSession<Session>();
            session.Player.AddContextToLogger(_logger).ForContext("Message", message, true).Debug("OnHandle: {Message}");
            session.Send(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, CardGambleReqMessage message)
        {
            var session = context.GetSession<Session>();
            var player = session.Player;
            var cardGamble = _gameDataService.CardGamble;
            foreach (var card in cardGamble.Cards)
            {
                var playerItem1 = player.Inventory.GetItem(card.ItemNumber);
                if (playerItem1 == null)
                {
                    session.Send(new CardGambleAckMessage(CardGambleResult.NotEnoughCard));
                    return true;
                }

                playerItem1.Durability--;
                if (playerItem1.Durability <= 0)
                    player.Inventory.Remove(playerItem1);
                else
                    player.Inventory.Update(playerItem1);
            }

            var playerItem2 = player.Inventory.GetItem(cardGamble.Reward.ItemNumber);
            if (playerItem2 != null)
            {
                playerItem2.Durability += (int)cardGamble.Reward.Period;
                player.Inventory.Update(playerItem2);
            }
            else
            {
                player.Inventory.Create(cardGamble.Reward.ItemNumber, cardGamble.Reward.PriceType, cardGamble.Reward.PeriodType, cardGamble.Reward.Period, cardGamble.Reward.Color, new uint[0]);
            }

            session.Send(new CardGambleAckMessage(CardGambleResult.Success, new ShopItemDto()
            {
                ItemNumber = cardGamble.Reward.ItemNumber,
                PriceType = cardGamble.Reward.PriceType,
                PeriodType = cardGamble.Reward.PeriodType,
                Period = (ushort)cardGamble.Reward.Period,
                Color = cardGamble.Reward.Color,
                Effect = cardGamble.Reward.EffectId
            }));
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, ItemEnchanReqMessage message)
        {
            var session = context.GetSession<Session>();
            var player = session.Player;
            player.AddContextToLogger(_logger).ForContext("Message", message, true).Debug("OnHandle: {Message}");
            var playerItem1 = player.Inventory[message.ItemId];
            if (playerItem1 == null)
            {
                session.Send(new EnchantEnchantItemAckMessage(EnchantResult.ItemEnchantError));
                return true;
            }
            var playerItem2 = player.Inventory[message.BonusItemId];
            var enchantPriceNeed = _gameDataService.GetEnchantPriceNeed(playerItem1.EnchantLevel + 1, playerItem1.ItemNumber.Category);
            if (enchantPriceNeed == null)
            {
                session.Send(new EnchantEnchantItemAckMessage(EnchantResult.Error));
                return true;
            }
            if (enchantPriceNeed.Durability > player.PEN)
            {
                session.Send(new EnchantEnchantItemAckMessage(EnchantResult.NotEnoughMoney));
                return true;
            }
            var enchantSystem = _gameDataService.GetEnchantSystem(playerItem1.EnchantLevel + 1, playerItem1.ItemNumber);
            if (enchantSystem == null)
            {
                session.Send(new EnchantEnchantItemAckMessage(EnchantResult.Error));
                return true;
            }
            var aleatoryEffect = enchantSystem.GetAleatoryEffect();
            if (aleatoryEffect == null)
            {
                session.Send(new EnchantEnchantItemAckMessage(EnchantResult.NotEnoughEffect));
                return true;
            }
            var result = EnchantResult.Success;
            bool flag1 = true;
            bool flag2 = false;
            lock (playerItem1.Effects)
            {
                for (byte index = 1; index < (byte)5; ++index)
                {
                    if (playerItem1.Effects.Contains<uint>(aleatoryEffect.Id - (uint)index))
                    {
                        playerItem1.Effects.Remove(aleatoryEffect.Id - (uint)index);
                        flag2 = true;
                        break;
                    }
                }
                if (!flag2)
                {
                    for (byte index = 0; index < (byte)5; ++index)
                    {
                        if (playerItem1.Effects.Contains<uint>(aleatoryEffect.Id + (uint)index))
                        {
                            flag1 = false;
                            break;
                        }
                    }
                }
            }
            if (flag1)
            {
                playerItem1.Effects.Add(aleatoryEffect.Id);
                ++playerItem1.EnchantLevel;
            }
            else
            {
                if (playerItem2 == null || !playerItem2.ItemNumber.Id.Equals(4080003))
                {
                    playerItem1.Effects.ResetEnchant();
                    playerItem1.EnchantLevel = 0;
                }
                result = EnchantResult.Reset;
            }
            if (playerItem2 != null)
            {
                --playerItem2.Durability;
                if (playerItem2.Durability < 1)
                    player.Inventory.Remove(playerItem2);
                else
                    player.Inventory.Update(playerItem2);
                result = EnchantResult.SuccessJackpot;
            }
            playerItem1.EnchantMP = 0;
            player.PEN -= enchantPriceNeed.Durability;
            session.Send(new EnchantEnchantItemAckMessage(result, message.ItemId, aleatoryEffect.Id));
            player.Inventory.Update(playerItem1);
            player.SendMoneyUpdate();
            return true;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, EsperEnchantReqMessage message)
        {
            var session = context.GetSession<Session>();
            session.Player.AddContextToLogger(_logger).ForContext("Message", message, true).Debug("OnHandle: {Message}");
            session.Send(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
            return true;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, ItemUseEsperChipReqMessage message)
        {
            var session = context.GetSession<Session>();
            session.Player.AddContextToLogger(_logger).ForContext("Message", message, true).Debug("OnHandle: {Message}");
            session.Send(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
            return true;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, ItemUseJewelItemReqMessage message)
        {
            var session = context.GetSession<Session>();
            session.Player.AddContextToLogger(_logger).ForContext("Message", message, true).Debug("OnHandle: {Message}");
            session.Send(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
            return true;
        }

        [Inline]
        public async Task<bool> OnHandle(
          MessageContext context,
          ItemUseRecordResetReqMessage message)
        {
            var session = context.GetSession<Session>();
            session.Player.AddContextToLogger(_logger).ForContext("Message", message, true).Debug("OnHandle: {Message}");
            session.Send(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
            return true;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, EsperEnchantPercentReqMessage message)
        {
            var session = context.GetSession<Session>();
            session.Player.AddContextToLogger(_logger).ForContext("Message", message, true).Debug("OnHandle: {Message}");
            session.Send(new ServerResultAckMessage(ServerResult.FailedToRequestTask));
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, AlchemyCombinationReqMessage message)
        {
            var session = context.GetSession<Session>();
            session.Player.AddContextToLogger(_logger).ForContext("Message", message, true).Debug("OnHandle: {Message}");
            var player = session.Player;
            var combination = _gameDataService.GetCombination((uint)message.Key);
            var inventory = player.Inventory;

            var requitial = message.Requitials;
            var listItem = new List<ItemNumber>();
            var combinationRequitial = new CombinationRequitialInfo();
            ulong ItemTipo = 0;

            if(player.PEN < 300)
            {
                session.Send(new AlchemyCombinationAckMessage(CombinationResult.NotEnoughPEN));
                return true;
            }

            player.PEN -= 300;
            player.SendMoneyUpdate();

            foreach (var req in requitial)
            {
                var playerItem1 = player.Inventory.GetItem(req.ItemNumber);
                playerItem1.Durability -= req.UseValue;
                if (playerItem1.Durability <= 0)
                    player.Inventory.Remove(playerItem1);
                else
                    player.Inventory.Update(playerItem1);

                if (req.ItemNumber != null)
                {
                    listItem.Add(req.ItemNumber);
                }
            }

            if (listItem.Contains(6010101))
            {
                ItemTipo = 6010101;
                var item2 = combination.GetRequitial(300);
                combinationRequitial = item2;
            }
            else if (listItem.Contains(6010102))
            {
                ItemTipo = 6010102;
                var item1 = combination.GetRequitial(100);
                combinationRequitial = item1;
            }
            else
            {
                if (message.Key < 5)
                {
                    var item3 = combination.GetRequitial(10000);
                    combinationRequitial = item3;
                }
                else if (message.Key >= 5)
                {
                    var item4 = combination.GetAleatoryOrFirst();
                    combinationRequitial = item4;
                }
            }
            var shopItemInfo = _gameDataService.GetShopItemInfo(combinationRequitial.ItemNumber, combinationRequitial.PriceType);
            var shopItem = _gameDataService.GetShopItem(combinationRequitial.ItemNumber);
            if (shopItem == null)
            {
                _logger.Warning("Trying to buy non-existant shopItem");
                return true;
            }
            else if ((int)combinationRequitial.Color > shopItem.ColorGroup)
            {
                _logger.Warning("Trying to buy item with invalid color");
                return true;
            }
            else
            {
                var itemInfo = shopItem.GetItemInfo(combinationRequitial.PriceType);
                if (itemInfo == null)
                {
                    _logger.Warning("Trying to buy non-existant item");
                    return true;
                }
                uint[] effects = Array.Empty<uint>();
                if (itemInfo.EffectGroup.Effects.Count > 0)
                    effects = itemInfo.EffectGroup.Effects.Select(x => x.Effect).Where(x => x > 0).ToArray();

                PlayerItem stackitem = null;
                var stacked = false;
                switch (combinationRequitial.PeriodType)
                {
                    case ItemPeriodType.None:
                        break;
                    case ItemPeriodType.Days:
                        stackitem = player.Inventory.GetItemDay((uint)shopItemInfo.Id);
                        if (stackitem != null)
                        {
                            stackitem.Durability += combinationRequitial.Period;
                            stacked = true;
                        }
                        break;
                    case ItemPeriodType.Units:
                        stackitem = player.Inventory.GetItem((uint)shopItemInfo.Id);
                        if (stackitem != null)
                        {
                            stackitem.Durability += combinationRequitial.Period;
                            stacked = true;
                        }
                        break;
                    default:
                        _logger.Warning("Unknown PriceType {priceType}", combinationRequitial.PeriodType);
                        break;
                }

                var plrItem = stackitem;
                if (!stacked)
                {
                    plrItem = inventory.Create(combinationRequitial.ItemNumber, combinationRequitial.PriceType, combinationRequitial.PeriodType, combinationRequitial.Period, combinationRequitial.Color, effects);
                }
                else
                {
                    player.Inventory.Update(plrItem);
                }

            }

            session.Send(new AlchemyCombinationAckMessage(CombinationResult.Success, new RequitalLevelDto()
            {
                ItemNumber = ItemTipo,
                ItemNumber2 = combinationRequitial.ItemNumber,
                PriceType = combinationRequitial.PriceType,
                PeriodType = combinationRequitial.PeriodType,
                Period = combinationRequitial.Period,
                Color = combinationRequitial.Color,
                EffectId = combinationRequitial.EffectId
            }));

            return true;
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, AlchemyDecompositionReqMessage message)
        {
            var session = context.GetSession<Session>();
            var player = session.Player;
            var logger = player.AddContextToLogger(_logger).ForContext("Message", message, true);
            logger.Debug("OnHandle: {Message}");
            var item = player.Inventory.GetIId((ulong)message.ItemId);
            if (item == null)
            {
                session.Send(new AlchemyDecompositionAckMessage(DecompositionResult.ErrorDB));
                return true;
            }
            var decomposition = _gameDataService.Decomposition;
            if (decomposition.ProhibitionInfos.Any(x => x.ItemNumber.Equals(item.ItemNumber)))
            {
                logger.Information("Invalid decomposition for prohibition={value}", item.ItemNumber);
                session.Send(new AlchemyDecompositionAckMessage(DecompositionResult.CannotDecomposition));
                return true;
            }
            var methodInfo = decomposition.GetMethodInfo(item.PeriodType);
            if (methodInfo == null)
            {
                logger.Information("Error methodInfo PeriodType: {0}", item.PeriodType);
                session.Send(new AlchemyDecompositionAckMessage(DecompositionResult.Error));
                return true;
            }
            var bonusInfo = decomposition.GetBonusInfo(item.ItemNumber);
            int days = (DateTimeOffset.Now - item.ExpireDate).Days;
            var componentInfo = methodInfo.GetComponentInfo(days);
            if (componentInfo == null)
            {
                logger.Information("Invalid decomposition for condition={value}", days);
                session.Send(new AlchemyDecompositionAckMessage(DecompositionResult.NotDecomposition));
                return true;
            }
            if (bonusInfo != null && !methodInfo.Bonus)
                bonusInfo.Period = 1;
            var playerItem1 = player.Inventory.GetItem(componentInfo.ItemNumber);
            if (playerItem1 != null && playerItem1.PeriodType.Equals(ItemPeriodType.Units))
            {
                var playerItem2 = playerItem1;
                int durability = playerItem2.Durability;
                int period1 = (int)componentInfo.Period;
                int? period2 = bonusInfo?.Period;
                int num = (period2.HasValue ? new int?(period1 * period2.GetValueOrDefault()) : new int?()) ?? 1;
                playerItem2.Durability = durability + num;
                player.Inventory.Update(playerItem1);
            }
            else
            {
                var gruopByPreviewEffect = _gameDataService.GetEffectGruopByPreviewEffect(componentInfo.EffectId);
                var inventory = player.Inventory;
                ItemNumber itemNumber = componentInfo.ItemNumber;
                int priceType = (int)componentInfo.PriceType;
                int periodType = (int)componentInfo.PeriodType;
                int period1 = (int)componentInfo.Period;
                int? period2 = bonusInfo?.Period;
                int num = (int)(ushort)((period2.HasValue ? new int?(period1 * period2.GetValueOrDefault()) : new int?()) ?? 1);
                int color = (int)componentInfo.Color;
                uint[] effects = gruopByPreviewEffect.GetEffects();
                inventory.Create(itemNumber, (ItemPriceType)priceType, (ItemPeriodType)periodType, (ushort)num, (byte)color, effects);
            }
            player.Inventory.Remove(item);
            session.Send(new AlchemyDecompositionAckMessage(DecompositionResult.Success, new RequitalLevelDto[1]
            {new RequitalLevelDto(){
                ItemNumber = (ulong) componentInfo.ItemNumber.Id,
                ItemNumber2 = componentInfo.ItemNumber,
                PriceType = componentInfo.PriceType,
                PeriodType = componentInfo.PeriodType,
                Period = bonusInfo != null ? (ushort) bonusInfo.Period : (ushort) 1,
                Color = componentInfo.Color,
                EffectId = componentInfo.EffectId
            }
            }));
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, BtcClearReqMessage message)
        {
            var session = context.GetSession<Session>();
            var player = session.Player;
            player.AddContextToLogger(_logger).ForContext("Message", message, true).Debug("OnHandle: {Message}");
            var tutorialRequitials = _gameDataService.GetTutorialRequitials(message.Option, (byte)message.Option2);
            if (player.Tutorials.Add(message.Option, (byte)message.Option2))
            {
                foreach (var btcOptionRequitial in tutorialRequitials)
                {
                    var gruopByPreviewEffect = _gameDataService.GetEffectGruopByPreviewEffect(btcOptionRequitial.EffectId);
                    player.Inventory.Create(btcOptionRequitial.ItemNumber, btcOptionRequitial.PriceType, btcOptionRequitial.PeriodType, btcOptionRequitial.Period, btcOptionRequitial.Color, gruopByPreviewEffect.GetEffects());
                }
                var array = tutorialRequitials.Select(x => new RequitalGiveItemResultDto(x.ItemNumber, 0)).ToArray();
                session.Send(new BtcClearAckMessage(message.Option, array));
            }
            player.Tutorials.SendNoticeTutorial();
            return true;
        }

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
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

        [Firewall(typeof(MustBeLoggedIn), new object[] { })]
        [Inline]
        public async Task<bool> OnHandle(MessageContext context, PromotionNewYearCardUseReqMessage message)
        {
            var session = context.GetSession<Session>();
            Console.WriteLine("SummerEvent position: {0}", message.position);
            var player = session.Player;

            var CapsuleTokens = new[]
            {
                new { _itemNumber = 6000038 },
                new { _itemNumber = 4020100 },
                new { _itemNumber = 4020101 },
                new { _itemNumber = 4020102 },
                new { _itemNumber = 4020103 },
                new { _itemNumber = 4020104 },
                new { _itemNumber = 4020105 }
            }.ToList();

            var prev = player.Inventory.FirstOrDefault(p => p.ItemNumber == CapsuleTokens[0]._itemNumber);

            switch (message.position)
            {
                case 1:
                    prev.Count -= 10;
                    break;
                case 2:
                    prev.Count -= 30;
                    break;
                case 3:
                    prev.Count -= 60;
                    break;
                case 4:
                    prev.Count -= 100;
                    break;
                case 5:
                    prev.Count -= 230;
                    break;
                case 6:
                    prev.Count -= 250;
                    break;
                default:
                    break;
            }
            if (prev.Count <= 0)
            {
                player.Inventory.Remove(prev.Id);
            }

            var premio = player.Inventory.FirstOrDefault(p => p.ItemNumber == CapsuleTokens[message.position]._itemNumber);
            if (premio == null)
            {
                player.Inventory.Create((ItemNumber)CapsuleTokens[message.position]._itemNumber, ItemPriceType.AP, ItemPeriodType.Units, 1, 0, new uint[0], true);
            }
            else
            {
                premio.Count++;
                session.Send(new ItemUpdateInventoryAckMessage(InventoryAction.Update, premio.Map<PlayerItem, ItemDto>()));
            }

            session.Send(new ItemUpdateInventoryAckMessage(InventoryAction.Update, prev.Map<PlayerItem, ItemDto>()));
            session.Send(new ItemRefundItemAckMessage { Result = ItemRefundResult.OK });
            return true;
        }

    }
}
