using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Netsphere;
using Netsphere.Server.Game.Data;
using RandomShopEditor.Models;
using RandomShopEditor.Services;

namespace RandomShopEditor.Forms
{
  public partial class ShowPriceUI : Form
  {
    private Point startPoint;
    public List<ShopPrice> ShopPrices { get; }
    public List<ShopPriceGroup> ShopPriceGroups { get; }
    public ShowPriceUI()
    {
      InitializeComponent();
      ShopPrices = ShopPriceService.Instance.ShopPrices;
      ShopPriceGroups = ShopPriceGroupService.Instance.ShopPriceGroups;

      ShopPriceGroups.ForEach(x => comboBoxPriceType.Items.Add(x.Name));
    }

    #region MouseMove
    private void MainMenu_MouseDown(object sender, MouseEventArgs e) => startPoint = new Point(-e.X, -e.Y);
    private void MainMenu_MouseMove(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        Point mouseMove = Control.MousePosition;
        mouseMove.Offset(startPoint.X, startPoint.Y);
        Location = mouseMove;
      }
    }
    #endregion

    #region ButtonClick
    private void buttonClose_Click(object sender, EventArgs e) => Close();

    private async void buttonAdd_Click(object sender, EventArgs e)
    {
      var periodTypeS = comboBoxName.Text.Equals("PERMANENT") ? "None" : comboBoxName.Text.Split(' ')[1];
      var value = int.Parse(comboBoxName.Text.Equals("PERMANENT") ? "0" : comboBoxName.Text.Split(' ')[0]);
      var name = (periodTypeS.Equals("PERMANENT")) ? "PERMANENT" : (value + " " + periodTypeS).ToUpper();
      var shopPriceId = ShopPrices
        .FirstOrDefault(x => x.PeriodType.ToString().ToUpper().Equals(periodTypeS) && x.Period == value);

      if (shopPriceId == null)
      {
        MessageBox.Show("No Price Id found");
        return;
      }

      var task = PeriodService.Instance.NewPeriod(shopPriceId.Id, name);
      var result = await task;
      if (result)
      {
        MessageBox.Show("Succesfully added");
        return;
      }

      MessageBox.Show("Something gone wrong");
    }
    #endregion

    private void comboBoxPriceType_SelectedValueChanged(object sender, EventArgs e)
    {
      comboBoxName.Items.Clear();
      comboBoxName.Text = "";
      var shopPriceGroupText = ((ComboBox)sender).Text;
      var shopPriceGroup = ShopPriceGroups.First(x => x.Name == shopPriceGroupText);

      ShopPrices.Where(x => x.PriceGroupId == shopPriceGroup.Id)
        .Select(x => (x.PeriodType == ItemPeriodType.None) ? "PERMANENT" : (x.Period + " " + x.PeriodType).ToUpper())
        .ToList().ForEach(x => comboBoxName.Items.Add(x));
    }
  }
}
