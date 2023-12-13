using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlubLib.Collections.Concurrent;
using ExpressMapper.Extensions;
using Microsoft.EntityFrameworkCore;
using Netsphere.Common;
using Netsphere.Database;
using Netsphere.Database.Game;
using Netsphere.Database.Helpers;
using Netsphere.Network.Data.Chat;
using Netsphere.Network.Message.Chat;
using Z.EntityFramework.Plus;

namespace Netsphere.Server.Chat
{
    public class FriendManager : ISaveable, IReadOnlyCollection<Friend>
    {
        private readonly IdGeneratorService _idGeneratorService;
        private readonly DatabaseService _databaseService;
        private readonly ConcurrentDictionary<ulong, Friend> _friends;
        private readonly ConcurrentStack<Friend> _friendsToRemove;

        public Player Player { get; private set; }
        public int Count => _friends.Count;
        public Friend this[ulong accountId] => CollectionExtensions.GetValueOrDefault(_friends, accountId);

        public FriendManager(IdGeneratorService idGeneratorService, DatabaseService databaseService)
        {
            _idGeneratorService = idGeneratorService;
            _databaseService = databaseService;
            _friendsToRemove = new ConcurrentStack<Friend>();
            _friends = new ConcurrentDictionary<ulong, Friend>();
        }

        public async Task Initialize(Player player, PlayerEntity entity)
        {
            Player = player;

            using (var db = _databaseService.Open<AuthContext>())
            {
                var ids = entity.Friends.Select(x => x.FriendPlayerId).ToArray();
                var accounts = await db.Accounts.Where(x => ids.Contains(x.Id)).ToArrayAsync();

                foreach (var friendEntity in entity.Friends)
                {
                    var friend = new Friend(
                        friendEntity.Id,
                        (ulong)friendEntity.FriendPlayerId,
                        accounts.First(x => x.Id == friendEntity.FriendPlayerId).Nickname,
                        (FriendState)friendEntity.State
                    );
                    friend.SetExistsState(true);
                    _friends[friend.FriendId] = friend;
                }
            }
        }

        public Friend Add(ulong accountId, string nickname, FriendState state)
        {
            if (_friends.ContainsKey(accountId))
                return null;

            var friend = new Friend(_idGeneratorService.GetNextId(IdKind.Friend), accountId, nickname, state);
            _friends.TryAdd(friend.FriendId, friend);
            return friend;
        }

        public bool Remove(ulong accountId)
        {
            return Remove(_friends.GetValueOrDefault(accountId));
        }

        public bool Remove(Friend friend)
        {
            if (friend == null)
                return false;

            _friends.Remove(friend.FriendId);
            if (friend.Exists)
                _friendsToRemove.Push(friend);

            friend.State = FriendState.Removed;
            Player.Session.Send(new FriendActionAckMessage(
                FriendActionResult.Success,
                FriendAction.Remove,
                friend.Map<Friend, FriendDto>()
            ));

            return true;
        }

        public async Task Save(GameContext db)
        {
            if (!_friendsToRemove.IsEmpty)
            {
                var idsToRemove = new List<long>();
                while (_friendsToRemove.TryPop(out var friendToRemove))
                    idsToRemove.Add(friendToRemove.Id);

                await db.PlayerFriends.Where(x => idsToRemove.Contains(x.Id)).DeleteAsync();
            }

            foreach (var friend in _friends.Values.Where(x => !x.Exists))
            {
                db.PlayerFriends.Add(new PlayerFriendEntity
                {
                    Id = friend.Id,
                    PlayerId = (int)Player.Account.Id,
                    FriendPlayerId = (int)friend.FriendId,
                    State = (byte)friend.State
                });
                friend.SetExistsState(true);
                friend.SetDirtyState(false);
            }

            foreach (var friend in _friends.Values.Where(x => x.IsDirty))
            {
                db.PlayerFriends.Update(new PlayerFriendEntity
                {
                    Id = friend.Id,
                    PlayerId = (int)Player.Account.Id,
                    FriendPlayerId = (int)friend.FriendId,
                    State = (byte)friend.State
                });
                friend.SetDirtyState(false);
            }
        }

        public bool Contains(ulong accountId)
        {
            return _friends.ContainsKey(accountId);
        }

        public IEnumerator<Friend> GetEnumerator()
        {
            return _friends.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Friend : DatabaseObject
    {
        private FriendState _state;

        public long Id { get; }
        public ulong FriendId { get; }
        public string Nickname { get; internal set; }
        public FriendState State
        {
            get => _state;
            internal set => SetIfChanged(ref _state, value);
        }

        public Friend(long id, ulong accountId, string nickname, FriendState state)
        {
            Id = id;
            FriendId = accountId;
            Nickname = nickname;
            _state = state;
        }
    }
}
