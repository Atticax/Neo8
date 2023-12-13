using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlubLib.Collections.Generic;
using BlubLib.Threading.Tasks;
using Logging;
using Microsoft.Extensions.Hosting;
using Netsphere.Common;
using Netsphere.Network.Message.Game;
using Netsphere.Server.Game.Services;

namespace Netsphere.Server.Game.Commands
{
    public class AdminCommands : ICommandHandler
    {
        private readonly ILogger<AdminCommands> _logger;
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly GameDataService _gameDataService;
        private readonly PlayerManager _playerManager;
        private readonly object _shutdownMutex;
        private bool _isInShutdown;
        public string Name { get; }
        public bool AllowConsole { get; }
        public SecurityLevel Permission { get; }
        

        public AdminCommands(ILogger<AdminCommands> logger, IApplicationLifetime applicationLifetime,
            GameDataService gameDataService, PlayerManager playerManager)
        {
            _logger = logger;
            _applicationLifetime = applicationLifetime;
            _gameDataService = gameDataService;
            _playerManager = playerManager;
            _shutdownMutex = new object();
            
        }


        [Command(
            CommandUsage.Player | CommandUsage.Console,
            SecurityLevel.Developer,
            "Usage:shutdown [minutes]"
        )]
        public async Task<bool>Shutdown(Player plr, string[] args)
        {
            lock (_shutdownMutex)
            {
                if (_isInShutdown)
                {
                    this.Reply(plr, "Server is already in shutdown mode");
                    return true;
                }

                if (args.Length == 1)
                {
                    _isInShutdown = true;
                    Shutdown();
                }
                else
                {
                    if (!uint.TryParse(args[1], out var minutes) || minutes <= 0)
                        return false;

                    _isInShutdown = true;
                    var timer = TimeSpan.FromMinutes(minutes);

                    var _ = Task.Run(async () =>
                    {
                        while (timer.TotalSeconds > 0)
                        {
                            // ReSharper disable CompareOfFloatsByEqualityOperator
                            if (timer.TotalMinutes >= 1 && timer.TotalSeconds % 60 == 0)
                                Announce();
                            else if (timer.TotalSeconds == 30)
                                Announce();
                            else if (timer.TotalSeconds <= 15 && timer.TotalSeconds % 5 == 0)
                                Announce();
                            // ReSharper restore CompareOfFloatsByEqualityOperator

                            await Task.Delay(TimeSpan.FromSeconds(1)).AnyContext();
                            timer -= TimeSpan.FromSeconds(1);
                        }

                        Shutdown();

                        void Announce()
                        {
                            _playerManager.ForEach(x => x.SendNotice($"Server shutdown in {timer.ToHumanReadable()}"));
                            _logger.Information("Server shutdown in {Time}", timer);
                        }
                    });
                }
            }

            return true;
        }

        [Command(
            CommandUsage.Player | CommandUsage.Console,
            SecurityLevel.Developer,
            "Usage: reloadshop"
        )]
        public async Task<bool> ReloadShop(Player plr, string[] args)
        {
            this.Reply(plr, "Reloading shop...");
            _playerManager.ForEach(x => x.SendNotice("Reloading shop - Game might lag for a minute"));
            await _gameDataService.LoadShop();
            _playerManager.ForEach(x => x.Session.Send(new NewShopUpdateEndAckMessage()));
            this.Reply(plr, "Reloaded shop!");
            return true;
        }

        //[Command(
        //    CommandUsage.Console,
        //    SecurityLevel.Developer,
        //    "Usage: reloadonetimec"
        //)]
        //public async Task<bool> ReloadOneTimeC(Player plr, string[] args)
        //{
        //    this.Reply(plr, "Reloading onetimecharge...");
        //    _gameDataService.LoadOneTimeCharges();
        //    this.Reply(plr, "Reloaded onetimecharge!");
        //    return true;
        //}

        [Command(
            CommandUsage.Console,
            SecurityLevel.Developer,
            "Usage: reloadcombination"
        )]
        public async Task<bool> ReloadCombination(Player plr, string[] args)
        {
            this.Reply(plr, "Reloading combination...");
            _gameDataService.LoadCombinationInfo();
            this.Reply(plr, "Reloaded combination!");
            return true;
        }

        [Command(
            CommandUsage.Console,
            SecurityLevel.Developer,
            "Usage: reloaddecomposition"
        )]
        public async Task<bool> ReloadDecomposition(Player plr, string[] args)
        {
            this.Reply(plr, "Reloading decomposition...");
            _gameDataService.LoadDecompositionInfo();
            this.Reply(plr, "Reloaded decomposition!");
            return true;
        }

        [Command(
            CommandUsage.Console,
            SecurityLevel.Developer,
            "Usage: reloadcards"
        )]
        public async Task<bool> ReloadCards(Player plr, string[] args)
        {
            this.Reply(plr, "Reloading cards...");
            _gameDataService.LoadCardGumble();
            this.Reply(plr, "Reloaded cards!");
            return true;
        }

        //[Command(
        //    CommandUsage.Console |
        //    CommandUsage.Player,
        //    SecurityLevel.Developer,
        //    "Usage: reloadenchants"
        //)]
        //public async Task<bool> ReloadEnchants(Player plr, string[] args)
        //{
        //    this.Reply(plr, "Reloading enchants...");
        //    _gameDataService.LoadEnchantSystem();
        //    this.Reply(plr, "Reloaded enchants!");
        //    return true;
        //}

        //[Command(
        //    CommandUsage.Console,
        //    SecurityLevel.Developer,
        //    "Usage: reloadenchantdata"
        //)]
        //public async Task<bool> ReloadEnchantData(Player plr, string[] args)
        //{
        //    this.Reply(plr, "Reloading enchantdata...");
        //    _gameDataService.LoadEnchantData();
        //    this.Reply(plr, "Reloaded enchantdata!");
        //    return true;
        //}

        private void Shutdown()
        {
            _playerManager.ForEach(x => x.SendNotice("Server is shutting down..."));
            _applicationLifetime.StopApplication();
        }
    }
}
