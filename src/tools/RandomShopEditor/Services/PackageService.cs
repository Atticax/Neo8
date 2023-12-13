using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Netsphere.Database;
using Netsphere.Database.Game;
using RandomShopEditor.Models;
using RandomShopEditor.XML;
using Z.EntityFramework.Plus;

namespace RandomShopEditor.Services
{
  public class PackageService
  {
    private static PackageService _instance;
    public static PackageService Instance
    {
      get
      {
        if (_instance == null)
          _instance = new PackageService();

        return _instance;
      }
    }

    private readonly DatabaseService _databaseService;

    public List<Package> Packages { get; set; }

    private PackageService() 
    {
      _databaseService = RSService.Instance.DatabaseService;
      Packages = new List<Package>();
    }

    public async Task LoadFromDatabase()
    {
      Packages.Clear();
      using (var db = _databaseService.Open<GameContext>())
      {
        var packagesEntity = await db.RandomShopPackages.ToListAsync();
        packagesEntity.ForEach(x => Packages.Add(new Package(x)));
      }
    }

    public async Task Delete(Package package)
    {
      using (var db = _databaseService.Open<GameContext>())
        await db.RandomShopPackages.Where(x => x.Id == package.Id).DeleteAsync();

      Packages.Remove(package);
    }

    public async Task<RandomShopPackageEntity> NewPackage(RSPackage rSPackage)
    {
      using (var db = _databaseService.Open<GameContext>())
      {
        var packageEntity = new RandomShopPackageEntity
        {
          Id = rSPackage.Id,
          Price = 1000,
          PriceType = 1,
          RequiredGender = 0,
          DescKey = rSPackage.DescKey,
          NameKey = rSPackage.NameKey,
          IsEnabled = true
        };
        try
        {
          db.RandomShopPackages.Add(packageEntity);
          await db.SaveChangesAsync();
          Packages.Add(new Package(packageEntity));
          return packageEntity;
        }
        catch (Exception e)
        {
          return null;
        }
      }
    }

    public async Task<bool> UpdatePackage(string id, string priceType, decimal price, string gender, string enabled)
    {
      using (var db = _databaseService.Open<GameContext>()) {
        var packageEntity = db.RandomShopPackages.FirstOrDefault(x => x.Id.ToString().Equals(id));
        if (packageEntity == null)
          return false;
        switch (priceType)
        {
          case "PEN":
            packageEntity.PriceType = 1;
            break;
          case "AP":
            packageEntity.PriceType = 2;
            break;
          case "PREMIUM":
            packageEntity.PriceType = 3;
            break;
          case "COUPON":
            packageEntity.PriceType = 5;
            break;
        }
        packageEntity.Price = (int)price;
        switch (gender) 
        {
          case "ALL":
            packageEntity.RequiredGender = 0;
            break;
          case "MALE":
            packageEntity.RequiredGender = 1;
            break;
          case "FEMALE":
            packageEntity.RequiredGender = 2;
            break;
        }
        packageEntity.IsEnabled = enabled.Equals("YES");
        db.RandomShopPackages.Update(packageEntity);
        await db.SaveChangesAsync();
        var toRemove = Packages.FirstOrDefault(x => x.Id.ToString().Equals(id));
        Packages.Remove(toRemove);
        Packages.Add(new Package(packageEntity));
        return true;
      }
    }

    public async Task<bool> RemovePackage(int id)
    {
      using (var db = _databaseService.Open<GameContext>())
      {
        var packageEntity = db.RandomShopPackages.Where(x => x.Id == id).First();
        if (packageEntity == null)
          return false;
        db.RandomShopPackages.Remove(packageEntity);
        await db.SaveChangesAsync();
        var toRemove = Packages.FirstOrDefault(x => x.Id.ToString().Equals(id));
        Packages.Remove(toRemove);
        return true;
      }
    }
  }
}
