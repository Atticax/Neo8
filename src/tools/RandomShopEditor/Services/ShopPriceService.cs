using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Netsphere.Database;
using Netsphere.Server.Game.Data;

namespace RandomShopEditor.Services
{
  public class ShopPriceService
  {
    private static ShopPriceService _instance;
    public static ShopPriceService Instance
    {
      get
      {
        if (_instance == null)
          _instance = new ShopPriceService();

        return _instance;
      }
    }

    private readonly DatabaseService _databaseService;

    public List<ShopPrice> ShopPrices { get; set; }

    private ShopPriceService()
    {
      _databaseService = RSService.Instance.DatabaseService;
      ShopPrices = new List<ShopPrice>();
    }

    public async Task LoadFromDatabase()
    {
      ShopPrices.Clear();
      using (var db = _databaseService.Open<GameContext>())
      {
        var shopPrices = await db.Prices.ToListAsync();
        shopPrices.ForEach(x => ShopPrices.Add(new ShopPrice(x)));
      }
    }
  }
}
