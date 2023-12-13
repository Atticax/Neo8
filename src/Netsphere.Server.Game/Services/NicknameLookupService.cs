using System;
using System.Linq;
using System.Threading.Tasks;
using BlubLib.Caching;
using Microsoft.EntityFrameworkCore;
using Netsphere.Common;
using Netsphere.Database;

namespace Netsphere.Server.Game.Services
{
    public class NicknameLookupService : IService
    {
        private static readonly TimeSpan s_cacheLifetime = TimeSpan.FromHours(1);

        private readonly DatabaseService _databaseService;
        private readonly PlayerManager _playerManager;
        private readonly ICache _cache;

        public NicknameLookupService(DatabaseService databaseService, PlayerManager playerManager)
        {
            _databaseService = databaseService;
            _playerManager = playerManager;
            _cache = new MemoryCache();

            _playerManager.PlayerConnected += OnPlayerConnected;
        }

        public string GetNickname(long accountId)
        {
            return GetNickname((ulong)accountId);
        }

        public string GetNickname(ulong accountId)
        {
            var cacheEntry = (string)_cache.Get(accountId.ToString());
            if (!string.IsNullOrWhiteSpace(cacheEntry))
                return cacheEntry;

            // Nickname is not cached so we need to look it up

            // Fast path - Check if the player is online
            var plr = _playerManager[accountId];
            if (plr != null)
                return CacheAndReturn(plr.Account.Nickname);

            // Slow path - Look it up from the database
            using (var db = _databaseService.Open<AuthContext>())
            {
                var id = (int)accountId;
                var accountEntity = db.Accounts.FirstOrDefault(x => x.Id == id);
                if (accountEntity == null || string.IsNullOrWhiteSpace(accountEntity.Nickname))
                    return null;

                return CacheAndReturn(accountEntity.Nickname);
            }

            string CacheAndReturn(string nickname)
            {
                CacheName(accountId, nickname);
                return nickname;
            }
        }

        public Task<string> GetNicknameAsync(long accountId)
        {
            return GetNicknameAsync((ulong)accountId);
        }

        public async Task<string> GetNicknameAsync(ulong accountId)
        {
            var cacheEntry = (string)_cache.Get(accountId.ToString());
            if (!string.IsNullOrWhiteSpace(cacheEntry))
                return cacheEntry;

            // Nickname is not cached so we need to look it up

            // Fast path - Check if the player is online
            var plr = _playerManager[accountId];
            if (plr != null)
                return CacheAndReturn(plr.Account.Nickname);

            // Slow path - Look it up from the database
            using (var db = _databaseService.Open<AuthContext>())
            {
                var id = (int)accountId;
                var accountEntity = await db.Accounts.FirstOrDefaultAsync(x => x.Id == id);
                if (accountEntity == null || string.IsNullOrWhiteSpace(accountEntity.Nickname))
                    return null;

                return CacheAndReturn(accountEntity.Nickname);
            }

            string CacheAndReturn(string nickname)
            {
                CacheName(accountId, nickname);
                return nickname;
            }
        }

        private void OnPlayerConnected(object sender, PlayerEventArgs e)
        {
            CacheName(e.Player.Account.Id, e.Player.Account.Nickname);
        }

        private void CacheName(ulong accountId, string nickname)
        {
            _cache.Set(accountId.ToString(), nickname, s_cacheLifetime);
        }
    }
}
