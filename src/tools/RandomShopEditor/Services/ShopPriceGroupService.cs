using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Netsphere.Database;
using RandomShopEditor.Models;

namespace RandomShopEditor.Services
{
  public class ShopPriceGroupService
  {
    private static ShopPriceGroupService _instance;
    public static ShopPriceGroupService Instance
    {
      get
      {
        if (_instance == null)
          _instance = new ShopPriceGroupService();

        return _instance;
      }
    }

    private readonly DatabaseService _databaseService;

    public List<ShopPriceGroup> ShopPriceGroups { get; set; }

    private ShopPriceGroupService()
    {
      _databaseService = RSService.Instance.DatabaseService;
      ShopPriceGroups = new List<ShopPriceGroup>();
    }

    public async Task LoadFromDatabase()
    {
      ShopPriceGroups.Clear();
      using (var db = _databaseService.Open<GameContext>())
      {
        var shopPriceGroups = await db.PriceGroups.ToListAsync();
        shopPriceGroups.ForEach(x => ShopPriceGroups.Add(new ShopPriceGroup(x)));
      }
    }
  }
}
