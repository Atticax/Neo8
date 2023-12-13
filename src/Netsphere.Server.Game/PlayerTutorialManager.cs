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
using System.Threading.Tasks;

namespace Netsphere.Server.Game
{
    public class PlayerTutorialManager :
    ISaveable,
    IReadOnlyCollection<Tutorial>
    {
        private ILogger _logger;
        private readonly GameDataService _gameDataService;
        private readonly IdGeneratorService _idGeneratorService;
        private readonly ConcurrentDictionary<long, Tutorial> _tutorials;

        public Player Player { get; private set; }

        public int Count => _tutorials.Count;

        public Tutorial this[OptionBtcClear optionBtc, byte option] => GetTutorial(optionBtc, option);

        public PlayerTutorialManager(
          ILogger<PlayerTutorialManager> logger,
          GameDataService gameDataService,
          IdGeneratorService idGeneratorService)
        {
            _logger = logger;
            _gameDataService = gameDataService;
            _idGeneratorService = idGeneratorService;
            _tutorials = new ConcurrentDictionary<long, Tutorial>();
        }

        public void Initialize(Player plr, PlayerEntity entity)
        {
            Player = plr;
            foreach (var tutorialWeaponEntity in entity.TutorialWeapon)
                _tutorials.TryAdd(tutorialWeaponEntity.Id, new Tutorial(tutorialWeaponEntity.Id, (OptionBtcClear)tutorialWeaponEntity.OptionBtc, (byte)tutorialWeaponEntity.Option));
            foreach (var tutorialSkillEntity in entity.TutorialSkill)
                _tutorials.TryAdd(tutorialSkillEntity.Id, new Tutorial(tutorialSkillEntity.Id, (OptionBtcClear)tutorialSkillEntity.OptionBtc, (byte)tutorialSkillEntity.Option));
            foreach (var tutorialRealEntity in entity.TutorialReal)
                _tutorials.TryAdd(tutorialRealEntity.Id, new Tutorial(tutorialRealEntity.Id, (OptionBtcClear)tutorialRealEntity.OptionBtc, (byte)tutorialRealEntity.Option));
        }

        public List<Tutorial> GetTutorial(OptionBtcClear optionBtc) => Count < 1 ? new List<Tutorial>() : _tutorials.Values.Where(x => x.OptionBtc.Equals(optionBtc)).ToList();

        public Tutorial GetTutorial(OptionBtcClear optionBtc, byte option) => _tutorials.Values.FirstOrDefault(x => x.OptionBtc.Equals(optionBtc) && x.Option.Equals(option));

        public bool Add(OptionBtcClear optionBtc, byte step)
        {
            if (Contains(optionBtc, step))
                return false;
            if (optionBtc.Equals(OptionBtcClear.Tutorial))
            {
                if (Player.TutorialState.Equals(1))
                    return false;
                Player.TutorialState = 1;
            }
            long nextId = _idGeneratorService.GetNextId(IdKind.Tutorial);
            _tutorials.TryAdd(nextId, new Tutorial(nextId, optionBtc, step, false));
            return true;
        }

        public void SendNoticeTutorial() => Player.Session.Send(new BtcSyncNoticeMessage()
        {
            OptionTutorialClear = (int)Player.TutorialState,
            OptionWeaponClear = OptionClear((byte)GetTutorial(OptionBtcClear.Weapons).Count),
            OptionSkillClear = OptionClear((byte)GetTutorial(OptionBtcClear.Skills).Count),
            OptionBattleClear = 0
        });

        public int OptionClear(byte step)
        {
            switch (step)
            {
                case 2:
                    return 3;
                case 3:
                    return 7;
                case 4:
                    return 15;
                case 5:
                    return 31;
                case 6:
                    return 63;
                default:
                    return step > (byte)6 ? (int)sbyte.MaxValue : (int)step;
            }
        }

        public async Task Save(GameContext db)
        {
            foreach (var tutorial in _tutorials.Values)
            {
                if (!tutorial.Exists)
                {
                    switch (tutorial.OptionBtc)
                    {
                        case OptionBtcClear.Weapons:
                            db.PlayerTutorialWeapons.Add(new PlayerTutorialWeaponEntity()
                            {
                                Id = tutorial.Id,
                                PlayerId = (int)Player.Account.Id,
                                OptionBtc = (byte)tutorial.OptionBtc,
                                Option = tutorial.Option
                            });
                            tutorial.SetExistsState(true);
                            continue;
                        case OptionBtcClear.Skills:
                            db.PlayerTutorialSkills.Add(new PlayerTutorialSkillEntity()
                            {
                                Id = tutorial.Id,
                                PlayerId = (int)Player.Account.Id,
                                OptionBtc = (byte)tutorial.OptionBtc,
                                Option = tutorial.Option
                            });
                            tutorial.SetExistsState(true);
                            continue;
                        case OptionBtcClear.Battle:
                            db.PlayerTutorialReals.Add(new PlayerTutorialRealEntity()
                            {
                                Id = tutorial.Id,
                                PlayerId = (int)Player.Account.Id,
                                OptionBtc = (byte)tutorial.OptionBtc,
                                Option = tutorial.Option
                            });
                            tutorial.SetExistsState(true);
                            continue;
                        default:
                            continue;
                    }
                }
            }
            await Task.CompletedTask;
        }

        public bool Contains(OptionBtcClear optionBtc, byte option) => GetTutorial(optionBtc, option) != null;

        public IEnumerator<Tutorial> GetEnumerator() => _tutorials.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class Tutorial : DatabaseObject
    {
        public long Id { get; }

        public OptionBtcClear OptionBtc { get; }

        public byte Option { get; }

        public Tutorial(long id, OptionBtcClear optionBtc, byte option, bool exists = true)
        {
            Id = id;
            OptionBtc = optionBtc;
            Option = option;
            SetExistsState(exists);
        }
    }
}
