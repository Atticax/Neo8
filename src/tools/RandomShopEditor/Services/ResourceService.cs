using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using ImageMagick;
using Netsphere.Resource;
using Netsphere.Resource.xml;
using Netsphere.Resource.Xml;
using ProgressBarSample;
using RandomShopEditor.Models;
using RandomShopEditor.Services;
using RandomShopEditor.XML;

using MyProgressEventArgs = RandomShopEditor.ProgressEventArgs;

namespace Netsphere.Tools.RandomShopEditor.Services
{
  public class ResourceService
  {
    private static ResourceService _instance;
    public static ResourceService Instance
    {
      get
      {
        if (_instance == null)
          _instance = new ResourceService();

        return _instance;
      }
    }

    private readonly S4Zip _zip;

    public event EventHandler<MyProgressEventArgs> ProgressChanged;

    public Effect[] Effects { get; private set; }
    public Effect[] EffectMatches { get; private set; }
    public Item[] Items { get; private set; }
    public RandomShopInfoStringTableDto RandomShopInfoStringTable { get; set; }
    public RandomShopPackageDTO RandomShopPackage { get; set; }

    public List<ShopItemInfo> ShopItemsInfo { get; set; }

    private ResourceService()
    {
      _zip = RSService.Instance.S4Zip;
      ShopItemsInfo = ShopItemInfoService.Instance.ShopItemInfos;
    }

    private void onProgressChanged(int current, int total)
    {
      ProgressChanged?.Invoke(this, new MyProgressEventArgs(current, total));
    }

    public void Load()
    {
      //var itemEffectDto = Deserialize<EffectListDto>("xml/effect_list.x7");
      //var effectMatchDto = Deserialize<EffectMatchListDto>("xml/effect_match_list.x7");
      //var stringTableDto = Deserialize<StringTableDto>("language/xml/item_effect_string_table.x7");
      //Effects = itemEffectDto.item_effect.Select(effectDto =>
      //{
      //  var id = effectDto.effect_id;
      //  var name = stringTableDto.@string.FirstOrDefault(x => x.key == effectDto.name_key)?.eng ?? effectDto.name_key;
      //  return new Effect(id, name);
      //}).ToArray();
      //
      //EffectMatches = effectMatchDto.match_key.Select(matchDto =>
      //{
      //  var id = matchDto.id;
      //  var name = stringTableDto.@string.FirstOrDefault(x => x.key == matchDto.name_key)?.eng ?? matchDto.name_key;
      //  return new Effect(id, name);
      //}).ToArray();
      RandomShopInfoStringTable = Deserialize<RandomShopInfoStringTableDto>("language/xml/randomshopinfo_string_table.x7");
      RandomShopPackage = Deserialize<RandomShopPackageDTO>("xml/randomshop_package.x7");
      var itemInfoDto = Deserialize<ItemListDto>("xml/item.x7");
      var stringTableDto = Deserialize<StringTableDto>("language/xml/iteminfo_string_table.x7");
      var items = new List<Item>();

      // Filter out entries we definitely dont need to make individual image loading faster
      var imageEntries = _zip.Values
          .Where(x => x.FullName.StartsWith("resources/image/", StringComparison.OrdinalIgnoreCase))
          .ToArray();

      var currentItemCount = 0;
      foreach (var itemDto in itemInfoDto.item)
      {
        var itemNumber = new ItemNumber(itemDto.item_key);
        var name = stringTableDto.@string.FirstOrDefault(x =>
                       x.key == itemDto.@base.name_key)?.eng ?? itemDto.@base.name;

        var imageName = itemDto.graphic?.icon_image ?? "noimage.png";
        imageName = Path.GetFileNameWithoutExtension(imageName);
        if (ShopItemsInfo.Any(x => x.ShopItemId == itemNumber.Id))
          items.Add(new Item(itemNumber, name, imageName, imageEntries));

        currentItemCount += 1;
        onProgressChanged(currentItemCount, itemInfoDto.item.Length);
      }

      Items = items.ToArray();
    }

    private TDto Deserialize<TDto>(string path)
    {
      var serializer = new XmlSerializer(typeof(TDto));
      using (var ms = new MemoryStream(_zip[path].GetData()))
        return (TDto)serializer.Deserialize(ms);
    }
  }

  public class Effect
  {
    public uint Id { get; }
    public string Name { get; }

    public Effect(uint id, string name)
    {
      Id = id;
      Name = name;
    }
  }

  public class Item
  {
    private readonly S4ZipEntry[] _imageEntries;
    private readonly string _imageName;

    public ItemNumber ItemNumber { get; set; }
    public string Name { get; }

    public Bitmap Image { get => LoadImage(); }

    public Item(ItemNumber itemNumber, string name, string imageName, S4ZipEntry[] imageEntries)
    {
      ItemNumber = itemNumber;
      Name = name;
      _imageEntries = imageEntries;
      _imageName = imageName;
    }

    private Bitmap LoadImage()
    {
       // The resource files not always contain the right extension so just test for both dds and tga
      var ddsImage = $"{_imageName}.dds";
      var tgaImage = $"{_imageName}.tga";
      var imageEntry = _imageEntries.FirstOrDefault(x =>
          x.FullName.EndsWith(ddsImage, StringComparison.OrdinalIgnoreCase) ||
          x.FullName.EndsWith(tgaImage, StringComparison.OrdinalIgnoreCase));
      if (imageEntry != null)
      {
        var readSettings = new MagickReadSettings();

        // Magick.Net seems to have problems detecting tga by itself so manually set the format to tga
        if (imageEntry.Name.EndsWith(".tga", StringComparison.OrdinalIgnoreCase))
          readSettings.Format = MagickFormat.Tga;

        using (var pngStream = new MemoryStream())
        using (var magickImage = new MagickImage(imageEntry.GetData(), readSettings))
        {
          magickImage.Write(pngStream, MagickFormat.Png32);
          pngStream.Position = 0;
          var imageLoaded = new Bitmap(pngStream);
          var resized = new Bitmap(imageLoaded, new System.Drawing.Size(80, 80));
          return resized;
        }
      }
      return new Bitmap(Path.Combine(Environment.CurrentDirectory, "resources", "noimage.png"));
    }

    private Bitmap loadImageByName(string name)
    {
      var ddsImage = $"{name}.dds";
      var tgaImage = $"{name}.tga";
      var imageEntry = _imageEntries.FirstOrDefault(x =>
          x.FullName.EndsWith(ddsImage, StringComparison.OrdinalIgnoreCase) ||
          x.FullName.EndsWith(tgaImage, StringComparison.OrdinalIgnoreCase));
      if (imageEntry == null)
        return new Bitmap(Path.Combine(Environment.CurrentDirectory, "resources", "noimage.png"));
      var readSettings = new MagickReadSettings();

      // Magick.Net seems to have problems detecting tga by itself so manually set the format to tga
      if (imageEntry.Name.EndsWith(".tga", StringComparison.OrdinalIgnoreCase))
        readSettings.Format = MagickFormat.Tga;

      using (var pngStream = new MemoryStream())
      using (var magickImage = new MagickImage(imageEntry.GetData(), readSettings))
      {
        magickImage.Write(pngStream, MagickFormat.Png32);
        pngStream.Position = 0;
        var imageLoaded = new Bitmap(pngStream);
        var resized = new Bitmap(imageLoaded, new Size(80, 80));
        return resized;
      }
    }
  }
}
