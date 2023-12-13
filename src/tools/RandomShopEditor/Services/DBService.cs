using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Netsphere.Database;
using Netsphere.Resource;

namespace RandomShopEditor.Services
{
  public class RSService
  {
    private static RSService _instance;
    public static RSService Instance
    {
      get
      {
        if (_instance == null)
          _instance = new RSService();

        return _instance;
      }
    }

    public DatabaseService DatabaseService { get; set;  }
    public S4Zip S4Zip { get; set;  }

    public void LoadDatabase(MySqlConnectionStringBuilder connectionStringBuilder)
    {
      try
      {
        var serviceProvider = new ServiceCollection()
          .AddSingleton<DatabaseService>()
          .AddDbContext<GameContext>(x => x.UseMySql(connectionStringBuilder.ConnectionString))
          .BuildServiceProvider();
        DatabaseService = serviceProvider.GetRequiredService<DatabaseService>();
      }
      catch (Exception e)
      {
        throw new Exception(e.Message);
      }
    }

    public void LoadZip(string path)
    {
      S4Zip = S4Zip.OpenZip(path);
    }
  }
}
