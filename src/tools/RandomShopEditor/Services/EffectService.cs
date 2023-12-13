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
  public class EffectService
  {
    private static EffectService _instance;
    public static EffectService Instance
    {
      get
      {
        if (_instance == null)
          _instance = new EffectService();

        return _instance;
      }
    }

    private readonly DatabaseService _databaseService;

    public List<Effect> Effects { get; set; }

    private EffectService()
    {
      _databaseService = RSService.Instance.DatabaseService;
      Effects = new List<Effect>();
    }

    public async Task LoadFromDatabase()
    {
      Effects.Clear();
      using (var db = _databaseService.Open<GameContext>())
      {
        var effects = await db.RandomShopEffects.ToListAsync();
        effects.ForEach(x => Effects.Add(new Effect(x)));
      }
    }

    public async Task Delete(Effect effect)
    {
      using (var db = _databaseService.Open<GameContext>())
        await db.RandomShopEffects.Where(x => x.Id == effect.Id).DeleteAsync();

      Effects.Remove(effect);
    }

    public async Task<RandomShopEffectEntity> NewLineup(Effect effect)
    {
      using (var db = _databaseService.Open<GameContext>())
      {
        var effectEntity = new RandomShopEffectEntity()
        {
          Id = effect.Id,
          Name = effect.Name,
          Probability = 100,
          Grade = 100
        };
        try
        {
          db.RandomShopEffects.Add(effectEntity);
          await db.SaveChangesAsync();
          Effects.Add(effect);
          return effectEntity;
        }
        catch (Exception e)
        {
          return null;
        }
      }
    }
  }
}
