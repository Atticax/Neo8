using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Netsphere.Server.Game.Data;
using RandomShopEditor.Models;
using RandomShopEditor.Services;

namespace RandomShopEditor.Forms
{
  public partial class PopupUI : Form
  {
    private Point startPoint;
    public int ItemId { get; set; }
    public int PId { get; set; }
    public List<Models.Color> Colors { get; }
    public List<Models.Effect> Effects { get; }
    public List<Period> Periods { get; }
    public List<ShopPrice> ShopPrices { get; }
    public List<ShopItemInfo> ShopItems { get; }
    public List<ShopPriceGroup> ShopPriceGroups { get; }

    public PopupUI(int itemId, int packageId)
    {
      InitializeComponent();
      ItemId = itemId;
      PId = packageId;
      Colors = ColorService.Instance.Colors;
      Effects = EffectService.Instance.Effects;
      Periods = PeriodService.Instance.Periods;
      ShopItems = ShopItemInfoService.Instance.ShopItemInfos;
      ShopPrices = ShopPriceService.Instance.ShopPrices;
      ShopPriceGroups = ShopPriceGroupService.Instance.ShopPriceGroups;

      Colors.ForEach(x => comboBoxColor.Items.Add(x.Name));
      Effects.ForEach(x => comboBoxEffect.Items.Add(x.Name));

      var shopPrices = new List<ShopPrice>();
      Periods.ForEach(x => comboBoxPeriod.Items.Add(x.Name));
    }

    #region MouseMove
    private void MainMenu_MouseDown(object sender, MouseEventArgs e) => startPoint = new System.Drawing.Point(-e.X, -e.Y);
    private void MainMenu_MouseMove(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        System.Drawing.Point mouseMove = Control.MousePosition;
        mouseMove.Offset(startPoint.X, startPoint.Y);
        Location = mouseMove;
      }
    }
    #endregion

    public async void buttonAdd_MouseDown(object sender, EventArgs e)
    {
      var effect = comboBoxEffect.Text;
      var color = comboBoxColor.Text;
      var period = comboBoxPeriod.Text;
      var probability = numericProbability.Value;
      var grade = numericGrade.Value;

      var effectId = Effects.First(x => x.Name.Equals(effect)).Id;
      var colorId = Colors.First(x => x.Name.Equals(color)).Id;
      var periodId = Periods.First(x => x.Name.Equals(period));
      ShopPrices.First(x => x.Id == periodId.ShopPriceId);


      var lineup = new Lineup
      {
        Id = (LineupService.Instance.Lineups.Count > 0) ? LineupService.Instance.Lineups.Last().Id + 1 : 1,
        PackageId = PId,
        ItemCategoryId = 1,
        RewardValue = 0,
        ShopItemId = ItemId,
        ColorId = colorId,
        EffectId = effectId,
        PeriodId = periodId.Id,
        DefaultColor = 0,
        Probability = (int)probability,
        Grade = (byte)grade
      };

      var task = LineupService.Instance.NewLineup(lineup);
      var result = await task;
      if (!result)
      {
        MessageBox.Show("Incorrect data...");
        return;
      }

      MessageBox.Show("Successfully added");
      Close();
    }

    private void buttonClose_MouseClick(object sender, EventArgs e) => Close();
  }
}
