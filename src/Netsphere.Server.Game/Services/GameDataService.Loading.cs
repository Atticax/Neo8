using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Netsphere.Database;
using Netsphere.Database.Game;
using Netsphere.Resource.xml;
using Netsphere.Resource.Xml;
using Netsphere.Server.Game.Data;

namespace Netsphere.Server.Game.Services
{
    public partial class GameDataService
    {
        public void LoadLevels()
        {
            _logger.Information("Loading levels...");
            var dto = Deserialize<ExperienceDto>("xml/experience.x7");
            Levels = Transform().ToImmutableDictionary(x => x.Level, x => x);
            _logger.Information("Loaded {Count} levels", Levels.Count);

            IEnumerable<LevelInfo> Transform()
            {
                for (var i = 0; i < dto.exp.Length; ++i)
                {
                    yield return new LevelInfo
                    {
                        Level = i,
                        ExperienceToNextLevel = dto.exp[i].require,
                        TotalExperience = dto.exp[i].accumulate
                    };
                }
            }
        }

        public void LoadMaps()
        {
            _logger.Information("Loading maps...");
            var dto = Deserialize<MapListDto>("xml/map.x7");
            var stringTable = Deserialize<StringTableDto>("language/xml/gameinfo_string_table.x7");
            Maps = Transform().ToImmutableArray();
            _logger.Information("Loaded {Count} maps", Maps.Length);

            IEnumerable<MapInfo> Transform()
            {
                foreach (var mapDto in dto.map)
                {
                    var map = new MapInfo
                    {
                        Id = (byte)mapDto.id,
                        GameRule = (GameRule)mapDto.@base.mode_number,
                        PlayerLimit = mapDto.@base.limit_player
                    };

                    if (!string.IsNullOrWhiteSpace(mapDto.@switch.eu) &&
                        mapDto.@switch.eu != "off" &&
                        mapDto.@switch.eu != "dev")
                    {
                        map.IsEnabled = true;
                    }

                    var name = stringTable.@string.FirstOrDefault(s => s.key.Equals(
                        mapDto.@base.map_name_key,
                        StringComparison.InvariantCultureIgnoreCase
                    ));
                    if (string.IsNullOrWhiteSpace(name?.eng))
                    {
                        _logger.Warning("Missing english translation for {MapKey}", mapDto.@base.map_name_key);
                        map.Name = mapDto.@base.map_name_key;
                    }
                    else
                    {
                        map.Name = name.eng;
                    }

                    yield return map;
                }
            }
        }

        public void LoadEffects()
        {
            _logger.Information("Loading effects...");
            var dto = Deserialize<EffectListDto>("xml/effect_list.x7");
            var stringTable = Deserialize<StringTableDto>("language/xml/item_effect_string_table.x7");
            Effects = Transform().ToImmutableDictionary(x => x.Id, x => x);
            _logger.Information("Loaded {Count} effects", Effects.Count);

            IEnumerable<ItemEffect> Transform()
            {
                foreach (var itemEffectDto in dto.item_effect.Where(itemEffect => itemEffect.effect_id != 0))
                {
                    var itemEffect = new ItemEffect
                    {
                        Id = itemEffectDto.effect_id
                    };

                    var nameDto = stringTable.@string.FirstOrDefault(x =>
                        x.key.Equals(itemEffectDto.name_key, StringComparison.InvariantCultureIgnoreCase)
                    );
                    var name = nameDto?.eng;
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        _logger.Warning("Missing english translation for item effect {Key}", itemEffectDto.name_key);
                        name = itemEffectDto.name_key;
                    }

                    itemEffect.Name = name;

                    if (Enum.IsDefined(typeof(EffectType), itemEffectDto.effect_type))
                    {
                        itemEffect.EffectType = (EffectType)itemEffectDto.effect_type;
                        itemEffect.Value = itemEffectDto.value_min;
                        itemEffect.Rate = itemEffectDto.rate_min / 10f;
                    }
                    else
                    {
                        _logger.Warning("Unknown effect type {EffectType} for Effect {EffectName}",
                            itemEffectDto.effect_type, itemEffect.Name);
                    }

                    yield return itemEffect;
                }
            }
        }

        public void LoadItems()
        {
            _logger.Information("Loading items...");
            var costumesDto = Deserialize<ItemListDto>("xml/item.x7");
            var weaponsDto = Deserialize<WeaponListDto>("xml/_eu_weapon.x7");
            var actionsDto = Deserialize<ActionListDto>("xml/action.x7");
            var stringTable = Deserialize<StringTableDto>("language/xml/iteminfo_string_table.x7");

            
            var items = Transform().ToArray();
            var dict = new Dictionary<ItemNumber, ItemInfo>();
            foreach (var item in items)
                dict[item.ItemNumber] = item;

            Items = dict.ToImmutableDictionary();
            _logger.Information("Loaded {Count} items", Items.Count);

            IEnumerable<ItemInfo> Transform()
            {
                foreach (var itemDto in costumesDto.item)
                {
                    var id = new ItemNumber(itemDto.item_key);
                    ItemInfo item = null;

                    switch (id.Category)
                    {
                        case ItemCategory.Weapon:
                            item = LoadWeapon(id);
                            break;

                        case ItemCategory.Skill:
                            item = LoadAction(id);
                            break;

                        default:
                            item = new ItemInfo();
                            break;
                    }

                    item.ItemNumber = id;
                    item.Gender = ParseGender(itemDto.@base.sex);
                    item.Image = itemDto.graphic.icon_image;

                    var name = stringTable.@string.FirstOrDefault(s =>
                        s.key.Equals(itemDto.@base.name_key, StringComparison.InvariantCultureIgnoreCase)
                    );
                    if (string.IsNullOrWhiteSpace(name?.eng))
                    {
                        _logger.Warning(
                            "Missing english translation for item {id}",
                            name != null ? itemDto.@base.name_key : id.ToString()
                        );
                        item.Name = name != null ? name.key : itemDto.@base.name;
                    }
                    else
                    {
                        item.Name = name.eng;
                    }

                    yield return item;
                }
            }

            Gender ParseGender(string gender)
            {
                bool Equals(string str)
                {
                    return gender.Equals(str, StringComparison.InvariantCultureIgnoreCase);
                }

                if (string.IsNullOrWhiteSpace(gender))
                    return Gender.None;

                if (Equals("unisex"))
                    return Gender.None;

                if (Equals("woman"))
                    return Gender.Female;

                if (Equals("man"))
                    return Gender.Male;

                throw new Exception("Invalid gender " + gender);
            }

            ItemInfo LoadAction(ItemNumber id)
            {
                var actionDto = actionsDto.Action.FirstOrDefault(x => x.name == id);
                if (actionDto == null)
                {
                    _logger.Warning("Missing action for item {id}", id);
                    return new ItemInfoAction();
                }

                var item = new ItemInfoAction
                {
                    RequiredMP = float.Parse(actionDto.ability.required_mp, CultureInfo.InvariantCulture),
                    DecrementMP = float.Parse(actionDto.ability.decrement_mp, CultureInfo.InvariantCulture),
                    DecrementMPDelay = float.Parse(actionDto.ability.decrement_mp_delay, CultureInfo.InvariantCulture)
                };

                if (actionDto.Float != null)
                {
                    item.ValuesF = new List<float>();

                    var props = actionDto.Float.GetType().GetProperties();
                    foreach (var prop in props)
                    {
                        var value = (string)prop.GetValue(actionDto.Float);
                        if (!string.IsNullOrWhiteSpace(value))
                            item.ValuesF.Add(float.Parse(value, CultureInfo.InvariantCulture));
                    }
                }

                if (actionDto.Integer != null)
                {
                    item.Values = new List<int>();

                    var props = actionDto.Integer.GetType().GetProperties();
                    foreach (var prop in props)
                    {
                        var value = (int)prop.GetValue(actionDto.Integer);
                        item.Values.Add(value);
                    }
                }

                return item;
            }

            ItemInfo LoadWeapon(ItemNumber id)
            {
                var weaponDto = weaponsDto.weapon.FirstOrDefault(x => x.item_key == id);
                if (weaponDto == null)
                {
                    _logger.Warning("Missing weapon for item {id}", id);
                    return new ItemInfoWeapon();
                }

                var ability = weaponDto.ability;
                var item = new ItemInfoWeapon
                {
                    Type = (byte)ability.type,
                    RateOfFire = float.Parse(ability.rate_of_fire, CultureInfo.InvariantCulture),
                    Power = float.Parse(ability.power, CultureInfo.InvariantCulture),
                    MoveSpeedRate = float.Parse(ability.move_speed_rate, CultureInfo.InvariantCulture),
                    AttackMoveSpeedRate = float.Parse(ability.attack_move_speed_rate, CultureInfo.InvariantCulture),
                    MagazineCapacity = ability.magazine_capacity,
                    CrackedMagazineCapacity = ability.cracked_magazine_capacity,
                    MaxAmmo = ability.max_ammo,
                    Accuracy = float.Parse(ability.accuracy, CultureInfo.InvariantCulture),
                    Range = string.IsNullOrWhiteSpace(ability.range)
                        ? 0
                        : float.Parse(ability.range, CultureInfo.InvariantCulture),
                    SupportSniperMode = ability.support_sniper_mode > 0,
                    SniperModeFov = ability.sniper_mode_fov > 0,
                    AutoTargetDistance = string.IsNullOrWhiteSpace(ability.auto_target_distance)
                        ? 0
                        : float.Parse(ability.auto_target_distance, CultureInfo.InvariantCulture)
                };

                return item;
            }
        }

        public void LoadDefaultItems()
        {
            _logger.Information("Loading default items...");
            var dto = Deserialize<DefaultItemDto>("xml/default_item.x7");
            DefaultItems = Transform().ToImmutableArray();
            _logger.Information("Loaded {Count} default items", DefaultItems.Length);

            IEnumerable<DefaultItem> Transform()
            {
                foreach (var itemDto in dto.male.item)
                {
                    var item = new DefaultItem
                    {
                        ItemNumber = new ItemNumber(itemDto.category, itemDto.sub_category, itemDto.number),
                        Gender = CharacterGender.Male,
                        Variation = itemDto.variation
                    };
                    yield return item;
                }

                foreach (var itemDto in dto.female.item)
                {
                    var item = new DefaultItem
                    {
                        ItemNumber = new ItemNumber(itemDto.category, itemDto.sub_category, itemDto.number),
                        Gender = CharacterGender.Female,
                        Variation = itemDto.variation
                    };
                    yield return item;
                }
            }
        }

        public async Task LoadShop()
        {
            using (var db = _databaseService.Open<GameContext>())
            {
                _logger.Information("Loading effect groups...");
                var effects = await db.EffectGroups.Include(x => x.ShopEffects).ToArrayAsync();
                ShopEffects = effects.ToImmutableDictionary(x => x.Id, x => new ShopEffectGroup(x));
                _logger.Information("Loaded {Count} effect groups", ShopEffects.Count);

                _logger.Information("Loading price groups...");
                var prices = await db.PriceGroups.Include(x => x.ShopPrices).ToArrayAsync();
                ShopPrices = prices.ToImmutableDictionary(x => x.Id, x => new ShopPriceGroup(x));
                _logger.Information("Loaded {Count} price groups", ShopPrices.Count);

                _logger.Information("Loading shop items...");
                var items = await db.Items.Include(x => x.ItemInfos).ToArrayAsync();
                ShopItems = items.ToImmutableDictionary(x => (ItemNumber)x.Id, x => new ShopItem(x, this));
                _logger.Information("Loaded {Count} shop items", ShopItems.Count);

                var version = await db.ShopVersion.FirstOrDefaultAsync();
                if (version == null)
                {
                    _logger.Warning("No shop version found in database! Using current timestamp");
                    version = new ShopVersionEntity { Version = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() };
                }

                ShopVersion = version.Version;
                _logger.Information("Loaded shop version {Version}", ShopVersion);
            }
        }

        public void LoadGameTempos()
        {
            _logger.Information("Loading game tempos...");
            var dto = Deserialize<ConstantInfoDto>("xml/constant_info.x7");
            GameTempos = Transform().ToImmutableDictionary(x => x.Name, x => x);
            _logger.Information("Loaded {Count} game tempos", GameTempos.Count);

            IEnumerable<GameTempo> Transform()
            {
                foreach (var gameTempoDto in dto.GAMEINFOLIST)
                {
                    var gameTempo = new GameTempo
                    {
                        Name = gameTempoDto.TEMPVALUE.value
                    };

                    var values = gameTempoDto.GAMETEPMO_COMMON_TOTAL_VALUE;
                    gameTempo.ActorDefaultHPMax =
                        float.Parse(values.GAMETEMPO_actor_default_hp_max, CultureInfo.InvariantCulture);
                    gameTempo.ActorDefaultMPMax =
                        float.Parse(values.GAMETEMPO_actor_default_mp_max, CultureInfo.InvariantCulture);
                    gameTempo.ActorDefaultMoveSpeed = values.GAMETEMPO_fastrun_required_mp;

                    yield return gameTempo;
                }
            }
        }

        public void LoadEquipLimits()
        {
            _logger.Information("Loading equip limits...");
            var dto = Deserialize<EquipLimitDto>("xml/equip_limit.x7");
            EquipLimits = Transform().ToImmutableDictionary(x => x.Id, x => x);
            _logger.Information("Loaded {Count} equip limits", EquipLimits.Count);

            IEnumerable<EquipLimitInfo> Transform()
            {
                foreach (var limitDto in dto.preset)
                {
                    yield return new EquipLimitInfo
                    {
                        Id = limitDto.id,
                        Blacklist = limitDto.require_Item?.Select(x => new ItemNumber(x.Item_Id)).ToArray()
                                    ?? Array.Empty<ItemNumber>()
                    };
                }
            }
        }

        public async Task LoadLevelRewards()
        {
            using (var db = _databaseService.Open<GameContext>())
            {
                _logger.Information("Loading level rewards...");
                var rewards = await db.LevelRewards.ToArrayAsync();
                LevelRewards = rewards.ToImmutableDictionary(x => x.Level, x => new LevelReward(x));
                _logger.Information("Loaded {Count} level rewards", LevelRewards.Count);
            }
        }

        //public void LoadOneTimeCharges()
        //{
        //    _logger.Information("Loading onetimecharges...");
        //    var dto = Deserialize<OneTimeChargeListDto>("xml/_eu_onetimecharge.x7");
        //    OneTimeCharges = ImmutableDictionary.ToImmutableDictionary(Transform(), (x => x.ItemKey), (x => x));
        //    _logger.Information("Loaded {Count} onetimecharges", OneTimeCharges.Count);

        //    IEnumerable<OneTimeCharge> Transform()
        //    {
        //        var oneTimeChargeDtoArray = dto.OneTimeChargeDtos;
        //        for (int index = 0; index < oneTimeChargeDtoArray.Length; index++)
        //        {
        //            var oneTimeChargeDto = oneTimeChargeDtoArray[index];
        //            yield return new OneTimeCharge()
        //            {
        //                ItemKey = (ItemNumber)oneTimeChargeDto.item_key,
        //                OneTimeChargeCategories = ((IEnumerable<OneTimeChargeCategoryDto>)oneTimeChargeDto.OneTimeChargeCategory).Select(x => new OneTimeChargeCategory()
        //                {
        //                    Type = (CapsuleRewardType)x.type,
        //                    OneTimeChargeSubCategories = ((IEnumerable<OneTimeChargeSubCategoryDto>)x.OneTimeChargeSubCategory).Select(y => new OneTimeChargeSubCategory()
        //                    {
        //                        Aleatory = y != null && y.aleatory,
        //                        UniqueBoost = y != null && y.unique_boost,
        //                        OneTimeChargeItems = ((IEnumerable<OneTimeChargeItemDto>)y.OneTimeChargeItem).Select(z => new OneTimeChargeItem()
        //                        {
        //                            Key = z != null ? z.key : 0,
        //                            PreviewEffect = z != null ? z.previewEffect : 0,
        //                            PriceType = z.priceType >= 1 ? (ItemPriceType)z.priceType : ItemPriceType.Premium,
        //                            PeriodType = z.periodType >= 1 ? (ItemPeriodType)z.periodType : ItemPeriodType.None,
        //                            Period = z != null ? z.period : (ushort)0,
        //                            Color = z != null ? z.color : 0,
        //                            Amount = z != null ? z.amount : 0,
        //                            BoostKey = z != null ? z.boostKey : 0
        //                            //Rate = z.rate
        //                        }).ToList()
        //                    }).ToList()
        //                }).ToList()
        //            };
        //        }
        //        oneTimeChargeDtoArray = null;
        //    }
        //}

        public void LoadCardGumble()
        {
            _logger.Information("Loading cardgumble...");
            var dto = Deserialize<CardSystemInfoDto>("xml/_eu_card_system_info.x7");
            StringTableDto stringTable = Deserialize<StringTableDto>("language/xml/iteminfo_string_table.x7");
            CardGamble = Transform();
            _logger.Information("Loaded {Count} cards", CardGamble.Cards.Count);

            CardSystem Transform()
            {
                var cardSystem = new CardSystem()
                {
                    Active = dto.active
                };
                foreach (var cardDto in dto.season.CardDtos)
                {
                    var card = cardDto;
                    List<CardSystemData> cards = cardSystem.Cards;
                    CardSystemData cardSystemData = new CardSystemData();
                    StringTableStringDto[] stringTableStringDtoArray = stringTable.@string;
                    cardSystemData.Name = (stringTableStringDtoArray != null ? ((IEnumerable<StringTableStringDto>)stringTableStringDtoArray).FirstOrDefault(x => x.key.Equals(string.Format("N{0}", (object)card.item_id), StringComparison.InvariantCultureIgnoreCase)).eng : (string)null) ?? "Unk";
                    cardSystemData.ItemNumber = new ItemNumber(card.item_id);
                    cardSystemData.PriceType = (ItemPriceType)((int)card.shop_id - 1);
                    cardSystemData.PeriodType = (ItemPeriodType)Enum.Parse<ItemPeriodType>(card.period_type, true);
                    cardSystemData.Period = card.period_value;
                    cardSystemData.Color = card.color;
                    cardSystemData.EffectId = card.effect_id;
                    cardSystemData.PlayProb = card.play_prob;
                    cards.Add(cardSystemData);
                }
                cardSystem.Reward = new CardSystemDataItem()
                {
                    ItemNumber = new ItemNumber(dto.season.reward.item_id),
                    PriceType = (ItemPriceType)dto.season.reward.shop_id,
                    PeriodType = (ItemPeriodType)Enum.Parse<ItemPeriodType>(dto.season.reward.period_type, true),
                    Period = dto.season.reward.period_value,
                    Color = dto.season.reward.color,
                    EffectId = dto.season.reward.effect_id
                };
                cardSystem.Formula = new CardSystemFormula()
                {
                    PlayLimitTime = TimeSpan.FromSeconds((double)dto.formula.play_limit_time),
                    PlayLimitCount = dto.formula.play_limit_min_count
                };
                return cardSystem;
            }
        }

        //public void LoadEnchantData()
        //{
        //    _logger.Information("Loading enchantdata...");
        //    var dto = Deserialize<EnchantDataDto>("xml/enchant_data.x7");
        //    EnchantData = Transform();
        //    _logger.Information("Loaded {Count} mastery", EnchantData.MasteryNeed.Count);
        //    _logger.Information("Loaded {Count} price", EnchantData.PriceNeed.Count);

        //    EnchantData Transform()
        //    {
        //        EnchantData enchantData = new EnchantData();
        //        foreach (var data in dto.enchant_config.Datas)
        //        {
        //            enchantData.DataDatas.Add(new EnchantDataData()
        //            {
        //                Level = data.notice_enchant_cnt,
        //                MasteryPeerMin = data.mastery_per_min,
        //                BonusProb = data.bonus_prob,
        //                Prob = data.prob_unit
        //            });
        //        }

        //        foreach (var masteryNeedDto in dto.mastery_need_table.MasteryNeedDtos)
        //        {
        //            enchantData.MasteryNeed.Add(new EnchantMasteryNeed()
        //            {
        //                Level = masteryNeedDto.enchant_cnt,
        //                ItemCategory = (ItemCategory)Enum.Parse<ItemCategory>(masteryNeedDto.item_type, true),
        //                Durability = masteryNeedDto.durability
        //            });
        //        }

        //        foreach (var enchantPriceDto in dto.enchant_price_table.EnchantPriceDtos)
        //        {
        //            enchantData.PriceNeed.Add(new EnchantMasteryNeed()
        //            {
        //                Level = enchantPriceDto.enchant_cnt,
        //                ItemCategory = (ItemCategory)Enum.Parse<ItemCategory>(enchantPriceDto.item_type, true),
        //                Durability = enchantPriceDto.enchant_price
        //            });
        //        }

        //        return enchantData;
        //    }
        //}

        //public void LoadEnchantSystem()
        //{
        //    _logger.Information("Loading enchantSystem...");
        //    var dto = Deserialize<EnchantSystemDto>("xml/_eu_enchant_system.x7");
        //    Enchant = ImmutableDictionary.ToImmutableDictionary(Transform(), (x => x.Level), (x => x));
        //    _logger.Information<int>("Loaded {Count} levels", Enchant.Count);

        //    IEnumerable<EnchantSystem> Transform()
        //    {
        //        EnchantDtoDto[] enchantDtoDtoArray = dto.EnchantDtoDtos;
        //        for (int index = 0; index < enchantDtoDtoArray.Length; ++index)
        //        {
        //            EnchantDtoDto e = enchantDtoDtoArray[index];
        //            foreach (uint num in ((IEnumerable<string>)e.level.Split('|', StringSplitOptions.None)).Select(i => uint.Parse(i)))
        //            {
        //                yield return new EnchantSystem()
        //                {
        //                    Level = num,
        //                    EnchantSystemDataItems = ((IEnumerable<EnchantDataItemDto>)e.EnchantDataItemDtos).Select(i => new EnchantSystemDataItem()
        //                    {
        //                        ItemCategory = (ItemCategory)i.item_category,
        //                        ItemSubCategory = i.item_subcategory,
        //                        EnchantDataEffects = ((IEnumerable<EnchantDataItemEffectDto>)i.EnchantDataItemEffectDtos).Select(eff => new EnchantDataEffect()
        //                        {
        //                            Id = eff.id,
        //                            Prob = eff.prob
        //                        }).ToList()
        //                    }).ToList()
        //                };
        //            }

        //            e = (EnchantDtoDto)null;
        //        }

        //        enchantDtoDtoArray = (EnchantDtoDto[])null;
        //    }
        //}

        public void LoadCombinationInfo()
        {
            _logger.Information("Loading combinationInfo...");
            var dto = Deserialize<CombinationInfoDto>("xml/_eu_combination_info.x7");
            Combination = ImmutableDictionary.ToImmutableDictionary(Transform(), (x => x.Key), (x => x));
            _logger.Information("Loaded {Count} combination", Combination.Count);

            IEnumerable<CombinationInfo> Transform()
            {
                var combinationMethodDtoArray = dto.MethodDtos;
                for (int index = 0; index < combinationMethodDtoArray.Length; index++)
                {
                    var method = combinationMethodDtoArray[index];
                    yield return new CombinationInfo()
                    {
                        Key = method.key,
                        Use = method.use.Equals("on"),
                        PricePEN = dto.pen_price,
                        UniqueProbItem = new ItemNumber(dto.unique_prob_up_item_key),
                        UniqueProbCount = dto.unique_prob_up_using_max_count,
                        ComponentInfo = new CombinationComponentInfo()
                        {
                            DataInfos = ((IEnumerable<CombinationComponentDataDto>)Array.Find(dto.ComponentDtos, (x => x.key.Equals(method.component_key))).ComponentDataDtos).Select(x => new CombinationComponentDataInfo()
                            {
                                ItemNumber = new ItemNumber(x.item_key),
                                Period = x.min_use_cnt
                            }).ToList()
                        },
                        Requitials = ((IEnumerable<CombinationMethodRequitalDto>)method.MethodRequitalDtos).Select(x => new CombinationRequitialInfo()
                        {
                            ItemNumber = new ItemNumber(x.item_key),
                            PriceType = (ItemPriceType)x.shop_id,
                            PeriodType = (ItemPeriodType)Enum.Parse<ItemPeriodType>(x.period_type, true),
                            Period = x.period,
                            Color = x.color,
                            EffectId = x.effect_id,
                            Probability = x.probability
                        }).ToList()
                    };
                }
                combinationMethodDtoArray = null;
            }
        }

        public void LoadDecompositionInfo()
        {
            _logger.Information("Loading decompositionInfo...");
            var dto = Deserialize<DecompositionInfoDto>("xml/_eu_decomposition_info.x7");
            Decomposition = Transform();
            _logger.Information("Loaded {Count} decomposition", Decomposition.MethodInfos.Count);

            DecompositionInfo Transform() => new DecompositionInfo()
            {
                PricePEN = dto.pen_price,
                MinHours = dto.min_hours,
                MinDays = dto.min_days,
                MethodInfos = ((IEnumerable<DecompositionMethodDto>)dto.Methods).Select(x => new DecompositionMethodInfo()
                {
                    PeriodType = (ItemPeriodType)Enum.Parse<ItemPeriodType>(x.period_type, true),
                    EffectMinCount = x.effect_min_cnt,
                    EffectMaxCount = x.effect_max_cnt,
                    Use = x.use.Equals("on"),
                    Bonus = x.bonus.Equals("on"),
                    ComponentInfos = ((IEnumerable<DecompositionMethodComponentDto>)x.Components).Select(i => new DecompositionComponentInfo()
                    {
                        ItemNumber = new ItemNumber(i.item_key),
                        PriceType = (ItemPriceType)i.shop_id,
                        PeriodType = (ItemPeriodType)Enum.Parse<ItemPeriodType>(i.period_type, true),
                        Period = i.period,
                        Color = i.color,
                        EffectId = i.effect_id,
                        Condition = i.condition
                    }).ToList()
                }).ToList(),
                BonusInfos = ((IEnumerable<DecompositionBonusDataDataDto>)dto.bonus_data.Datas).Select(x => new DecompositionBonusInfo()
                {
                    ItemCategory = (ItemCategory)Enum.Parse<ItemCategory>(x.item_main_type, true),
                    Period = x.period_multiple_value
                }).ToList(),
                ProhibitionInfos = ((IEnumerable<DecompositionProhibitionDataDto>)dto.prohibition.Datas).Select(x => new DecompositionProhibitionInfo()
                {
                    ItemNumber = new ItemNumber(x.item_key)
                }).ToList()
            };
        }

    }
}
