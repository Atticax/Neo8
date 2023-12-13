using Logging;
using Netsphere.Common;
using Netsphere.Network;
using Netsphere.Network.Data.Game;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Data;
using Netsphere.Server.Game.Services;
using ProudNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Netsphere.Server.Game.Handlers
{
    public class RandomShopHandler : IHandle<RandomShopRollingStartReqMessage>
    {
        private readonly GameDataService _gameDataService;
        private readonly ILogger<RandomShopHandler> _logger;
        private readonly NumberExtractorService _numberExtractorService;
        private readonly int _doubleChanceProb = 5;
        private readonly Random _random = new Random();

        public RandomShopHandler(
            ILogger<RandomShopHandler> logger,
            NumberExtractorService numberExtractorService,
            GameDataService gameDataService)
        {
            _logger = logger;
            _gameDataService = gameDataService;
            _numberExtractorService = numberExtractorService;
        }

        public Task<bool> OnHandle(MessageContext context, RandomShopRollingStartReqMessage message)
        {
            Session session = context.GetSession<Session>();
            Player player = session.Player;
            if (!_gameDataService.RandomShopPackages.ContainsKey(message.PackageId))
            {
                _logger.Error("Unable to find package, invalid id {0}", message.PackageId);
                session.Send(new RandomShopRollingStartAckMessage()
                {
                    Result = RandomShopRollingResult.Failed
                });
                return Task.FromResult(true);
            }
            RandomShopPackage randomShopPackage = _gameDataService.RandomShopPackages[message.PackageId];
            switch (randomShopPackage.PriceType)
            {
                case ItemPriceType.PEN:
                    if (player.PEN < randomShopPackage.Price)
                    {
                        session.Send(new RandomShopRollingStartAckMessage()
                        {
                            Result = RandomShopRollingResult.Failed
                        });
                        return Task.FromResult(true);
                    }
                    break;
                case ItemPriceType.CP:
                    if (player.Coins2 < randomShopPackage.Price)
                    {
                        session.Send(new RandomShopRollingStartAckMessage()
                        {
                            Result = RandomShopRollingResult.Failed
                        });
                        return Task.FromResult(true);
                    }
                    break;
            }
            List<RandomShopRollingDto> randomShopRollingDtoList = new List<RandomShopRollingDto>();
            int num1 = _random.Next(0, 100) <= _doubleChanceProb ? 2 : 1;
            for (int index1 = 0; index1 < num1; ++index1)
            {
                if (randomShopPackage.Count <= 0)
                {
                    _logger.Error("Selected package {0} is empty", randomShopPackage.Name);
                    session.Send(new RandomShopRollingStartAckMessage()
                    {
                        Result = RandomShopRollingResult.Failed
                    });
                    return Task.FromResult(true);
                }
                int index2 = _numberExtractorService.ExtractIndex(randomShopPackage.Select(x => (int)x.Item.Probability).ToArray());
                RandomShopLineup extractedItem = randomShopPackage[index2];
                if (extractedItem == null)
                {
                    _logger.Error("Unable to find item inside package {0} with index {1}", randomShopPackage.Name, index2);
                    session.Send(new RandomShopRollingStartAckMessage()
                    {
                        Result = RandomShopRollingResult.Failed
                    });
                    return Task.FromResult(true);
                }
                RandomShopColor[] array1 = RandomShopPackage.Colors.Where(x => x.Group.Equals(extractedItem.Item.Color.Group)).ToArray();
                RandomShopPeriod[] array2 = RandomShopPackage.Periods.Where(x => x.Group.Equals(extractedItem.Item.Period.Group)).ToArray();
                RandomShopEffect[] array3 = RandomShopPackage.Effects.Where(x => x.Group.Equals(extractedItem.Item.Effect.Group)).ToArray();
                RandomShopColor randomShopColor = array1[_numberExtractorService.ExtractIndex(((IEnumerable<RandomShopColor>)array1).Select(x => (int)x.Probability).ToArray())];
                RandomShopPeriod randomShopPeriod = array2[_numberExtractorService.ExtractIndex(((IEnumerable<RandomShopPeriod>)array2).Select(x => (int)x.Probability).ToArray())];
                RandomShopEffect extractedEffect = array3[_numberExtractorService.ExtractIndex(((IEnumerable<RandomShopEffect>)array3).Select(x => (int)x.Probability).ToArray())];
                byte num2 = (byte)((uint)(extractedItem.Item.Grade + (byte)randomShopColor.Grade + (byte)randomShopPeriod.Grade + (byte)extractedEffect.Grade) / 4U);
                if (randomShopColor == null || randomShopPeriod == null || extractedEffect == null)
                {
                    _logger.Error("Invalid item extracted {0}", extractedItem.Item.Item.Id);
                    session.Send(new RandomShopRollingStartAckMessage()
                    {
                        Result = RandomShopRollingResult.Failed
                    });
                    return Task.FromResult(true);
                }
                ShopEffectGroup shopEffectGroup = _gameDataService.ShopEffects.Values.FirstOrDefault(x => x.Id == extractedEffect.Effect);
                if (player.Inventory.Create(extractedItem.Item.Item, randomShopPeriod.PriceType, randomShopPeriod.Type, (ushort)randomShopPeriod.Value, randomShopColor.Color, shopEffectGroup.Effects.Select(x => x.Effect).ToArray(), true) == null)
                {
                    _logger.Error("Unable to create item {0}", extractedItem.Item.Item.Id);
                    session.Send(new RandomShopRollingStartAckMessage()
                    {
                        Result = RandomShopRollingResult.Failed
                    });
                    return Task.FromResult(true);
                }
                switch (randomShopPackage.PriceType)
                {
                    case ItemPriceType.PEN:
                        player.PEN -= randomShopPackage.Price;
                        break;
                    case ItemPriceType.CP:
                        player.Coins2 -= randomShopPackage.Price;
                        break;
                }
                randomShopRollingDtoList.Add(new RandomShopRollingDto()
                {
                    ItemId = extractedItem.Item.Item,
                    ShopItemId = (uint)extractedItem.Item.Item,
                    EffectGroupId = shopEffectGroup.PreviewEffect,
                    PeriodType = randomShopPeriod.Type,
                    Period = (int)randomShopPeriod.Value,
                    Color = randomShopColor.Color,
                    Grade = (byte)extractedItem.Item.Grade
                });
            }
            session.Send(new RandomShopRollingStartAckMessage()
            {
                Items = randomShopRollingDtoList.ToArray(),
                Result = RandomShopRollingResult.Ok
            });
            session.Send(new MoneyRefreshCashInfoAckMessage(player.PEN, player.AP));
            return Task.FromResult(true);
        }

        public int ExtractIndexs(params int[] probabilities)
        {
            int num = 100;
            return num;
        }

    }
}
