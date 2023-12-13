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
  public class LineupService
  {
    private static LineupService _instance;
    public static LineupService Instance
    {
      get
      {
        if (_instance == null)
          _instance = new LineupService();

        return _instance;
      }
    }

    private readonly DatabaseService _databaseService;

    public List<Lineup> Lineups { get; set; }

    private LineupService()
    {
      _databaseService = RSService.Instance.DatabaseService;
      Lineups = new List<Lineup>();
    }

    public async Task LoadFromDatabase()
    {
      Lineups.Clear();
      using (var db = _databaseService.Open<GameContext>())
      {
        var lineups = await db.RandomShopLineups.ToListAsync();
        lineups.ForEach(x => Lineups.Add(new Lineup(x)));
      }
    }

    public async Task Delete(Lineup lineup)
    {
      using (var db = _databaseService.Open<GameContext>())
        await db.RandomShopLineups.Where(x => x.Id == lineup.Id).DeleteAsync();

      Lineups.Remove(lineup);
    }

    public async Task<bool> NewLineup(Lineup lineup)
    {
      using (var db = _databaseService.Open<GameContext>())
      {
        var lineupEntity = new RandomShopLineupEntity
        {
          Id = lineup.Id,
          RandomShopPackageId = lineup.PackageId,
          ItemCategoryId = lineup.ItemCategoryId,
          RewardValue = lineup.RewardValue,
          ShopItemId = lineup.ShopItemId,
          RandomShopColorId = lineup.ColorId,
          RandomShopEffectId = lineup.EffectId,
          RandomShopPeriodId = lineup.PeriodId,
          DefaultColor = lineup.DefaultColor,
          Probability = lineup.Probability,
          Grade = lineup.Grade
        };
        try
        {
          db.RandomShopLineups.Add(lineupEntity);
          await db.SaveChangesAsync();
          Lineups.Add(lineup);
          return true;
        }
        catch (Exception e)
        {
          return false;
        }
      }
    }

    public async Task<bool> UpdateLineup(string id, int effect, int color, int period, decimal prob, decimal grade)
    {
      using (var db = _databaseService.Open<GameContext>())
      {
        var lineupEntity = db.RandomShopLineups.Where(x => x.Id.ToString().Equals(id)).First();
        if (lineupEntity == null)
          return false;

        lineupEntity.RandomShopColorId = color;
        lineupEntity.RandomShopEffectId = effect;
        lineupEntity.RandomShopPeriodId = period;
        lineupEntity.Probability = (int)prob;
        lineupEntity.Grade = (byte)grade;
        db.RandomShopLineups.Update(lineupEntity);
        await db.SaveChangesAsync();
        var toRemove = Lineups.FirstOrDefault(x => x.Id.ToString().Equals(id));
        Lineups.Remove(toRemove);
        Lineups.Add(new Lineup(lineupEntity));
        return true;
      }
    }

    public async Task<bool> RemoveLineup(int id)
    {
      using (var db = _databaseService.Open<GameContext>())
      {
        var lineupEntity = db.RandomShopLineups.Where(x => x.Id == id).First();
        if (lineupEntity == null)
          return false;
        db.RandomShopLineups.Remove(lineupEntity);
        await db.SaveChangesAsync();
        var toRemove = Lineups.FirstOrDefault(x => x.Id.ToString().Equals(id));
        Lineups.Remove(toRemove);
        return true;
      }
    }

  }
}
