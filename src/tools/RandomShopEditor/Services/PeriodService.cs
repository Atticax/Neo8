using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Netsphere.Database;
using Netsphere.Database.Game;
using RandomShopEditor.Models;

namespace RandomShopEditor.Services
{
  public class PeriodService
  {
    private static PeriodService _instance;
    public static PeriodService Instance
    {
      get
      {
        if (_instance == null)
          _instance = new PeriodService();

        return _instance;
      }
    }

    private readonly DatabaseService _databaseService;

    public List<Period> Periods { get; set; }

    private PeriodService()
    {
      _databaseService = RSService.Instance.DatabaseService;
      Periods = new List<Period>();
    }

    public async Task LoadFromDatabase()
    {
      Periods.Clear();
      using (var db = _databaseService.Open<GameContext>())
      {
        var periods = await db.RandomShopPeriods.ToListAsync();
        periods.ForEach(x => Periods.Add(new Period(x)));
      }
    }

    public async Task<bool> Delete(Period period)
    {
      using (var db = _databaseService.Open<GameContext>())
      {
        var periodEntity = db.RandomShopPeriods.FirstOrDefault(x => x.Id == period.Id);
        if (periodEntity == null)
          return false;
        try
        {
          db.RandomShopPeriods.Remove(periodEntity);
          await db.SaveChangesAsync();
          Periods.Remove(period);
          return true;
        }
        catch (Exception e)
        {
          return false;
        }
      }
    }

    public async Task<bool> NewPeriod(int id, string name)
    {
      using (var db = _databaseService.Open<GameContext>())
      {
        var periodEntity = new RandomShopPeriodEntity
        {
          Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
          ShopPriceId = id,
          Name = name,
          Probability = 100,
          Grade = 100
        };
        try
        {
          db.RandomShopPeriods.Add(periodEntity);
          await db.SaveChangesAsync();
          Periods.Add(new Period(periodEntity));
          return true;
        }
        catch (Exception e)
        {
          return false;
        }
      }
    }
  }
}
