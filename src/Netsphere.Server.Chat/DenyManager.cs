using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlubLib.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Netsphere.Common;
using Netsphere.Database;
using Netsphere.Database.Game;
using Netsphere.Database.Helpers;
using Z.EntityFramework.Plus;

namespace Netsphere.Server.Chat
{
    public class DenyManager : ISaveable, IReadOnlyCollection<Deny>
    {
        private readonly IdGeneratorService _idGeneratorService;
        private readonly DatabaseService _databaseService;
        private readonly ConcurrentDictionary<ulong, Deny> _denies;
        private readonly ConcurrentStack<Deny> _deniesToRemove;

        public Player Player { get; private set; }
        public int Count => _denies.Count;
        public Deny this[ulong accountId] => CollectionExtensions.GetValueOrDefault(_denies, accountId);

        public DenyManager(IdGeneratorService idGeneratorService, DatabaseService databaseService)
        {
            _idGeneratorService = idGeneratorService;
            _databaseService = databaseService;
            _deniesToRemove = new ConcurrentStack<Deny>();
            _denies = new ConcurrentDictionary<ulong, Deny>();
        }

        public async Task Initialize(Player player, PlayerEntity entity)
        {
            Player = player;

            using (var db = _databaseService.Open<AuthContext>())
            {
                var ids = entity.Ignores.Select(x => x.DenyPlayerId).ToArray();
                var accounts = await db.Accounts.Where(x => ids.Contains(x.Id)).ToArrayAsync();

                foreach (var denyEntity in entity.Ignores)
                {
                    var deny = new Deny(denyEntity.Id, (ulong)denyEntity.DenyPlayerId,
                        accounts.First(x => x.Id == denyEntity.DenyPlayerId).Nickname);
                    deny.SetExistsState(true);
                    _denies[deny.DenyId] = deny;
                }
            }
        }

        public Deny Add(ulong accountId, string nickname)
        {
            if (_denies.ContainsKey(accountId))
                return null;

            var deny = new Deny(_idGeneratorService.GetNextId(IdKind.Deny), accountId, nickname);
            _denies.TryAdd(deny.DenyId, deny);
            return deny;
        }

        public bool Remove(Deny deny)
        {
            return Remove(deny.DenyId);
        }

        public bool Remove(ulong accountId)
        {
            var deny = this[accountId];
            if (deny == null)
                return false;

            _denies.Remove(accountId);
            if (deny.Exists)
                _deniesToRemove.Push(deny);

            return true;
        }

        public async Task Save(GameContext db)
        {
            if (!_deniesToRemove.IsEmpty)
            {
                var idsToRemove = new List<long>();
                while (_deniesToRemove.TryPop(out var denyToRemove))
                    idsToRemove.Add(denyToRemove.Id);

                await db.PlayerIgnores.Where(x => idsToRemove.Contains(x.Id)).DeleteAsync();
            }

            foreach (var deny in _denies.Values.Where(deny => !deny.Exists))
            {
                db.PlayerIgnores.Add(new PlayerDenyEntity
                {
                    Id = deny.Id,
                    PlayerId = (int)Player.Account.Id,
                    DenyPlayerId = (int)deny.DenyId
                });
                deny.SetExistsState(true);
            }
        }

        public bool Contains(ulong accountId)
        {
            return _denies.ContainsKey(accountId);
        }

        public IEnumerator<Deny> GetEnumerator()
        {
            return _denies.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Deny : DatabaseObject
    {
        public long Id { get; }
        public ulong DenyId { get; }
        public string Nickname { get; internal set; }

        public Deny(long id, ulong accountId, string nickname)
        {
            Id = id;
            DenyId = accountId;
            Nickname = nickname;
        }
    }
}
