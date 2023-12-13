using System;
using System.Threading.Tasks;
using ConsoleAppFramework;
using Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Netsphere.Common;
using Netsphere.Common.Configuration;
using PatchManager.Config;
using PatchManager.Controller;
using PatchManager.Model;
using PatchManager.Service;

namespace PatchManager
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("Hello World!");

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var configuration = Netsphere.Common.Startup.Initialize(baseDirectory, "config.hjson",
                x => x.GetSection(nameof(AppOptions.Logging)).Get<LoggerOptions>());

            await CreateHostBuilder(configuration.Get<AppOptions>()).RunConsoleAppFrameworkAsync<ConsoleCommandController>(args);
        }
        public static IHostBuilder CreateHostBuilder(AppOptions configuration)
        {
            return new HostBuilder().ConfigureServices(services =>
            {
                services
                    .AddSingleton<IFilesystemService, FilesystemService>()
                    .AddSingleton(configuration)
                    .AddDbContext<PatchDbContext>(options =>
                    {
                        options.UseMySql(configuration.Database.ConnectionString);
                    })
                    .AddLogging(builder => builder.AddSimpleConsole());
            });
        }
    }
}
