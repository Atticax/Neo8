using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Netsphere.Database;
using RandomShopEditor.Models;

namespace RandomShopEditor.Services
{
  public class ShopItemInfoService
  {
    private static ShopItemInfoService _instance;
    public static ShopItemInfoService Instance
    {
      get
      {
        if (_instance == null)
          _instance = new ShopItemInfoService();

        return _instance;
      }
    }

    private readonly DatabaseService _databaseService;

    public List<ShopItemInfo> ShopItemInfos { get; set; }

    private ShopItemInfoService()
    {
      _databaseService = RSService.Instance.DatabaseService;
      ShopItemInfos = new List<ShopItemInfo>();
    }

    public async Task LoadFromDatabase()
    {
      ShopItemInfos.Clear();
      using (var db = _databaseService.Open<GameContext>())
      {
        var shopItems = await db.ItemInfos.ToListAsync();
        shopItems.ForEach(x => ShopItemInfos.Add(new ShopItemInfo(x)));
      }
    }
  }
}
