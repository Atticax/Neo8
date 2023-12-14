using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Netsphere.Database;
using Netsphere.Database.Game;
using Netsphere.Resource.Xml;
using Netsphere.Server.Game.Data;

namespace Netsphere.Server.Game.Services
{
    public partial class GameDataService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly DatabaseService _databaseService;
        private readonly string _resourcePath;

        public ImmutableDictionary<int, LevelInfo> Levels { get; private set; }
        public ImmutableArray<MapInfo> Maps { get; private set; }
        public ImmutableDictionary<uint, ItemEffect> Effects { get; private set; }
        public ImmutableDictionary<ItemNumber, ItemInfo> Items { get; private set; }
        public ImmutableArray<DefaultItem> DefaultItems { get; private set; }
        public ImmutableDictionary<string, GameTempo> GameTempos { get; private set; }
        public ImmutableDictionary<int, EquipLimitInfo> EquipLimits { get; private set; }

        // Shop data
        public ImmutableDictionary<ItemNumber, ShopItem> ShopItems { get; private set; }
        public ImmutableDictionary<int, ShopEffectGroup> ShopEffects { get; private set; }
        public ImmutableDictionary<int, ShopPriceGroup> ShopPrices { get; private set; }
        public ImmutableDictionary<int, LevelReward> LevelRewards { get; private set; }
        public string ShopVersion { get; private set; }
        public ImmutableDictionary<ItemNumber, OneTimeCharge> OneTimeCharges { get; private set; }
        public CardSystem CardGamble { get; private set; }
        public EnchantData EnchantData { get; private set; }
        public ImmutableDictionary<uint, EnchantSystem> Enchant { get; private set; }
        public ImmutableDictionary<uint, CombinationInfo> Combination { get; private set; }
        public DecompositionInfo Decomposition { get; private set; }
        public ImmutableDictionary<OptionBtcClear, BtcOptions> Tutorials { get; private set; }
        public ImmutableDictionary<int, RandomShopPackage> RandomShopPackages { get; private set; }
        public ImmutableDictionary<ItemNumber, CapsuleReward> CapsuleRewards { get; private set; }

        public GameDataService(ILogger<GameDataService> logger, DatabaseService databaseService)
        {
            _logger = logger;
            _databaseService = databaseService;
            _resourcePath = Path.Combine(Program.BaseDirectory, "data");
        }

        public ItemInfo GetItemInfo(ItemNumber number)
        {
            ItemInfo itemInfo;
            Items.TryGetValue(number, out itemInfo);
            return itemInfo;
        }

        public DefaultItem GetDefaultItem(CharacterGender gender, CostumeSlot slot, byte variation)
        {
            return DefaultItems.FirstOrDefault(item =>
                item.Gender == gender && item.Variation == variation &&
                item.ItemNumber.SubCategory == (byte)slot);
        }

        public ShopItem GetShopItem(ItemNumber itemNumber)
        {
            ShopItems.TryGetValue(itemNumber, out var item);
            return item;
        }

        public ShopItemInfo GetShopItemInfo(ItemNumber itemNumber, ItemPriceType priceType)
        {
            var item = GetShopItem(itemNumber);
            return item?.GetItemInfo(priceType);
        }

        public LevelInfo GetLevelFromExperience(uint experience)
        {
            return Levels.Values
                       .OrderBy(levelInfo => levelInfo.Level)
                       .FirstOrDefault(levelInfo => experience >= levelInfo.TotalExperience &&
                                                    experience < levelInfo.TotalExperience + levelInfo.ExperienceToNextLevel)
                   ?? Levels.Last().Value;
        }

        public ShopEffectGroup GetEffectGruopByPreviewEffect(uint previewEffect)
        {
            return ShopEffects.Values.FirstOrDefault(x => (int)x.PreviewEffect == (int)previewEffect);
        }

        public OneTimeCharge GetOneTimeCharge(ItemNumber itemNumber)
        {
            OneTimeCharge oneTimeCharge;
            OneTimeCharges.TryGetValue(itemNumber, out oneTimeCharge);
            return oneTimeCharge;
        }

        public EnchantMasteryNeed GetEnchantMasteryNeed(uint level, ItemCategory itemCategory)
        {
            return EnchantData.MasteryNeed.FirstOrDefault(x => x.Level.Equals(level) && x.ItemCategory.Equals(itemCategory));
        }

        public EnchantMasteryNeed GetEnchantPriceNeed(uint level, ItemCategory itemCategory)
        {
            return EnchantData.PriceNeed.FirstOrDefault(x => x.Level.Equals(level) && x.ItemCategory.Equals(itemCategory));
        }

        public EnchantSystemDataItem GetEnchantSystem(uint level, ItemNumber itemNumber)
        {
            EnchantSystem enchantSystem;
            return !Enchant.TryGetValue(level, out enchantSystem) ? null : enchantSystem.EnchantSystemDataItems.FirstOrDefault(x => x.ItemCategory.Equals(itemNumber.Category) & x.ItemSubCategory.Equals(itemNumber.SubCategory));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            LoadLevels();
            LoadMaps();
            LoadEffects();
            LoadItems();
            LoadDefaultItems();
            LoadGameTempos();
            LoadEquipLimits();
            LoadOneTimeCharges();
            LoadCardGumble();
            LoadEnchantData();
            LoadEnchantSystem();
            LoadCombinationInfo();
            LoadDecompositionInfo();
            LoadTutorials();
            LoadCapsules();
            await LoadShop();
            await LoadRandomShop();
            await LoadLevelRewards();

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private T Deserialize<T>(string fileName)
        {
            var serializer = new XmlSerializer(typeof(T));

            var path = Path.Combine(_resourcePath, fileName.Replace('/', Path.DirectorySeparatorChar));
            using (var r = new StreamReader(path))
                return (T)serializer.Deserialize(r);
        }

        private byte[] GetBytes(string fileName)
        {
            var path = Path.Combine(_resourcePath, fileName.Replace('/', Path.DirectorySeparatorChar));
            return File.Exists(path) ? File.ReadAllBytes(path) : null;
        }

        public CombinationInfo GetCombination(uint key)
        {
            CombinationInfo combinationInfo;
            if (!Combination.TryGetValue(key, out combinationInfo))
                combinationInfo = null;
            return combinationInfo;
        }

        public List<BtcOptionRequitial> GetTutorialRequitials(OptionBtcClear optionBtc, byte step)
        {
            BtcOptions btcOptions;
            Tutorials.TryGetValue(optionBtc, out btcOptions);
            return btcOptions?.GetComponent(step)?.Requitials ?? new List<BtcOptionRequitial>();
        }

        public void LoadTutorials()
        {
            _logger.Information("Loading tutorials...");
            var dto = Deserialize<BtcOptionsDto>("xml/_eu_reward_btc_options.x7");
            Tutorials = ImmutableDictionary.ToImmutableDictionary(Transform(), (x => x.OptionBtc), (x => x));
            _logger.Information("Loaded {Count} tutorials", Tutorials.Count);

            IEnumerable<BtcOptions> Transform() => ((IEnumerable<BtcOptionTutorialDto>)dto.TutorialDtos).Select(x => new BtcOptions()
            {
                OptionBtc = (OptionBtcClear)Enum.Parse<OptionBtcClear>(x.option, true),
                Components = ((IEnumerable<BtcOptionTutorialComponentDto>)x.ComponentDtos).Select(y => new BtcOptionComponent()
                {
                    Step = y.step,
                    Requitials = ((IEnumerable<BtcOptionTutorialRequitialDto>)y.RequitialDtos).Select(z => new BtcOptionRequitial()
                    {
                        ItemNumber = new ItemNumber(z.item_key),
                        PriceType = (ItemPriceType)z.shop_id,
                        PeriodType = (ItemPeriodType)Enum.Parse<ItemPeriodType>(z.period_type, true),
                        Period = z.period,
                        Color = z.color,
                        EffectId = z.effect_id
                    }).ToList()
                }).ToList()
            });
        }

        public async Task LoadRandomShop(bool force = false)
        {
            var randomShopPackages = new List<RandomShopPackage>();
            _logger.Information("Loading random shop packages...");
            using (var db = _databaseService.Open<GameContext>())
            {
                var packages = await db.RandomShopPackages.ToArrayAsync();
                var lineups = await db.RandomShopLineups.ToArrayAsync();
                var periods = await db.RandomShopPeriods.ToListAsync();
                var effects = await db.RandomShopEffects.ToListAsync();
                var colors = await db.RandomShopColors.ToListAsync();
                if (!RandomShopPackage.IsInitialized | force)
                {
                    var periodList = periods.Select(x => new RandomShopPeriod(x, this)).ToList();
                    var effectList = effects.Select(x => new RandomShopEffect(x)).ToList();
                    var colorList = colors.Select(x => new RandomShopColor(x)).ToList();
                    RandomShopPackage.Initialize(colorList, effectList, periodList);
                    periodList = null;
                    effectList = null;
                    colorList = null;
                }
                var shopPackageEntityArray = packages;
                for (int index = 0; index < shopPackageEntityArray.Length; index++)
                {
                    var package = shopPackageEntityArray[index];
                    var lineupList = new List<RandomShopLineup>();
                    foreach (var shopLineupEntity in ((IEnumerable<RandomShopLineupEntity>)lineups).Where(x => x.RandomShopPackageId == package.Id))
                    {
                        var currentLineup = shopLineupEntity;
                        var period = new RandomShopPeriod(currentLineup.RandomShopPeriod, this);
                        var effect = new RandomShopEffect(currentLineup.RandomShopEffect);
                        var color = new RandomShopColor(currentLineup.RandomShopColor);
                        var lineup = new RandomShopLineup((uint)package.Id, new RandomShopItem(currentLineup, color, effect, period));
                        lineupList.Add(lineup);
                        period = null;
                        effect = null;
                        color = null;
                        lineup = null;
                        currentLineup = null;
                    }
                    randomShopPackages.Add(new RandomShopPackage(package, lineupList));
                    lineupList = null;
                }
                shopPackageEntityArray = null;
                _logger.Information("Loaded {Count} random shop packages", randomShopPackages.Count);
                RandomShopPackages = randomShopPackages.ToImmutableDictionary((x => (int)x.Id), (x => x));
                packages = null;
                lineups = null;
                periods = null;
                effects = null;
                colors = null;
            }

            randomShopPackages = null;
        }

        public void LoadCapsules()
        {
            var dto = Deserialize<ItemRewardDto>("xml/ItemBag.xml");
            CapsuleRewards = Transform().ToImmutableDictionary(x => x.Item, x => x);

            IEnumerable<CapsuleReward> Transform()
            {
                foreach (var item in dto.Items)
                    yield return new CapsuleReward(item);
            }
        }
    }
}
