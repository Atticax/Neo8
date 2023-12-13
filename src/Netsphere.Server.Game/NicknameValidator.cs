using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Netsphere.Common.Configuration;
using Netsphere.Database;
using Netsphere.Database.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Netsphere.Server.Game
{
    public class NicknameValidator
    {
        private readonly GameOptions _gameOptions;
        private readonly DatabaseService _databaseService;
        private readonly string[] _inAvailableNick;

        public NicknameValidator(IOptions<GameOptions> gameOptions, DatabaseService databaseService)
        {
            _gameOptions = gameOptions.Value;
            _databaseService = databaseService;
            _inAvailableNick = new string[4]
            {
                "admin",
                "adm",
                "gm",
                "gs"
            };
        }

        public async Task<bool> IsAvailableAsync(string nickname)
        {
            int minLength = _gameOptions.NickRestrictions.MinLength;
            int maxLength = _gameOptions.NickRestrictions.MaxLength;
            bool whitespaceAllowed = _gameOptions.NickRestrictions.WhitespaceAllowed;
            bool asciiOnly = _gameOptions.NickRestrictions.AsciiOnly;
            if (string.IsNullOrWhiteSpace(nickname) || !whitespaceAllowed && nickname.Contains(" ") || (((IEnumerable<string>)_inAvailableNick).Any(x => nickname.Contains(x, StringComparison.OrdinalIgnoreCase)) || nickname.Length < minLength || nickname.Length > maxLength) || asciiOnly && Encoding.UTF8.GetByteCount(nickname) != nickname.Length)
                return false;
            int maxRepeat = _gameOptions.NickRestrictions.MaxRepeat;
            if (maxRepeat > 0)
            {
                int num = 1;
                char ch = nickname[0];
                for (int index = 1; index < nickname.Length; index++)
                {
                    if ((int)ch != (int)nickname[index])
                    {
                        if (num > maxRepeat)
                            return false;
                        num = 0;
                        ch = nickname[index];
                    }
                    num++;
                }
                if (num > maxRepeat)
                    return false;
            }
            long now = DateTimeOffset.Now.ToUnixTimeSeconds();
            using (var db = _databaseService.Open<AuthContext>())
            {
                bool nickExists = await EntityFrameworkQueryableExtensions.AnyAsync(db.Accounts, (x => x.Nickname == nickname), new CancellationToken());
                bool flag = await EntityFrameworkQueryableExtensions.AnyAsync(db.Nicknames, (x => x.Nickname == nickname && (x.ExpireDate == -1 || x.ExpireDate > now)), new CancellationToken());
                return !nickExists && !flag;
            }
        }

    }
}
