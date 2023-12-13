using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Netsphere.Database;
using Netsphere.Database.Game;
using RandomShopEditor.Models;
using Z.EntityFramework.Plus;

namespace RandomShopEditor.Services
{
  public class ColorService
  {
    private static ColorService _instance;
    public static ColorService Instance
    {
      get
      {
        if (_instance == null)
          _instance = new ColorService();

        return _instance;
      }
    }

    private readonly DatabaseService _databaseService;

    public List<Color> Colors { get; set; }

    private ColorService()
    {
      _databaseService = RSService.Instance.DatabaseService;
      Colors = new List<Color>();
    }

    public async Task LoadFromDatabase()
    {
      Colors.Clear();
      using (var db = _databaseService.Open<GameContext>())
      {
        var colors = await db.RandomShopColors.ToListAsync();
        colors.ForEach(x => Colors.Add(new Color(x)));
      }
    }

    public async Task Delete(Color color)
    {
      using (var db = _databaseService.Open<GameContext>())
        await db.RandomShopColors.Where(x => x.Id == color.Id).DeleteAsync();

      Colors.Remove(color);
    }

    public async Task<RandomShopColorEntity> NewLineup(Color color)
    {
      using (var db = _databaseService.Open<GameContext>())
      {
        var colorEntity = new RandomShopColorEntity()
        {
          Id = color.Id,
          Name = color.Name,
          Color = color.Color_,
          Probability = 100,
          Grade = 100
        };
        try
        {
          db.RandomShopColors.Add(colorEntity);
          await db.SaveChangesAsync();
          Colors.Add(color);
          return colorEntity;
        }
        catch (Exception e)
        {
          return null;
        }
      }
    }
  }
}
