using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BlubLib.IO;
using BlubLib.Serialization;
using Netsphere.Network;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Services;
using ProudNet;

namespace Netsphere.Server.Game.Handlers
{
    internal class ResourceHandler : IHandle<NewShopUpdateCheckReqMessage>, IHandle<RandomShopUpdateCheckReqMessage>
    {
        private readonly GameDataService _gameDataService;
        private readonly BlubSerializer _serializer;
        private readonly IDictionary<ShopResourceType, Func<GameDataService, object>> _shopResourceSelector;

        public ResourceHandler(GameDataService gameDataService, BlubSerializer serializer)
        {
            _gameDataService = gameDataService;
            _serializer = serializer;
            _shopResourceSelector = new Dictionary<ShopResourceType, Func<GameDataService, object>>
            {
                [ShopResourceType.Price] = x => x.ShopPrices,
                [ShopResourceType.Effect] = x => x.ShopEffects,
                [ShopResourceType.Item] = x => x.ShopItems,
                [ShopResourceType.RandomShop] = x => x.RandomShopPackages
            };
        }

        [Inline]
        public async Task<bool> OnHandle(MessageContext context, NewShopUpdateCheckReqMessage message)
        {
            var session = context.Session;

            var version = _gameDataService.ShopVersion;
            var flags = ShopResourceType.None;

            if (message.PriceVersion != version)
                flags |= ShopResourceType.Price;

            if (message.EffectVersion != version)
                flags |= ShopResourceType.Effect;

            if (message.ItemVersion != version)
                flags |= ShopResourceType.Item;


            session.Send(new NewShopUpdateCheckAckMessage
            {
                PriceVersion = version,
                EffectVersion = version,
                ItemVersion = version,
                UniqueItemVersion = message.UniqueItemVersion,
                Flags = flags
            });

            if (flags == ShopResourceType.None)
                return true;

            foreach (var pair in _shopResourceSelector)
            {
                if (!flags.HasFlag(pair.Key))
                    continue;

                using (var w = new BinaryWriter(new MemoryStream()))
                {
                    _serializer.Serialize(w, pair.Value(_gameDataService));
                    var data = w.ToArray();
                    var decompressedLength = data.Length;
                    var compressed = data.CompressLZO();
                    session.Send(new NewShopUpdateInfoAckMessage(pair.Key,
                        compressed, (uint)compressed.Length, (uint)decompressedLength, version));
                }
            }

            // TODO unique item (What is this even?)
            // using (var w = new BinaryWriter(new MemoryStream()))
            // {
            //     w.Write(0);
            //
            //     session.Send(new SNewShopUpdateInfoAckMessage
            //     {
            //         Type = ShopResourceType.UniqueItem,
            //         Data = w.ToArray(),
            //         Version = version
            //     });
            // }

            return true;
        }

        public Task<bool> OnHandle(MessageContext context, RandomShopUpdateCheckReqMessage message)
        {
            Session session = context.GetSession<Session>();
            session.Send(new RandomShopUpdateCheckAckMessage()
            {
                Unk = ""
            });
            foreach (var keyValuePair in _shopResourceSelector)
            {
                if (keyValuePair.Key == ShopResourceType.RandomShop)
                {
                    using (var binaryWriter = new BinaryWriter(new MemoryStream()))
                    {
                        _serializer.Serialize(binaryWriter, keyValuePair.Value(_gameDataService));
                        byte[] array = binaryWriter.ToArray();
                        int length = array.Length;
                        byte[] numArray = array.CompressLZO();
                        session.Send(new RandomShopUpdateInfoAckMessage()
                        {
                            Unk1 = (byte)31,
                            Unk2 = numArray,
                            Unk3 = numArray.Length,
                            Unk4 = length,
                            Unk5 = ""
                        });
                    }
                }
            }
            return Task.FromResult(true);
        }
    }
}
