using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using Netsphere.Server.Game.Data;
using RandomShopEditor.Models;
using RandomShopEditor.Services;

namespace RandomShopEditor.Forms
{
  public partial class PeriodUI : Form
  {
    private delegate void populate_delegate();

    public List<Period> Periods { get; set; }
    public List<ShopPrice> ShopPrices { get; set; }
    public List<ShopPriceGroup> ShopPriceGroups { get; set; }

    public PeriodUI()
    {
      InitializeComponent();
      Periods = PeriodService.Instance.Periods;
      ShopPrices = ShopPriceService.Instance.ShopPrices;
      ShopPriceGroups = ShopPriceGroupService.Instance.ShopPriceGroups;
    }

    private void PeriodUI_Load(object sender, EventArgs e)
      => Task.Run(populate);

    private void populate()
    {
      if (!InvokeRequired)
      {
        Periods.ForEach(x => Controls.Add(CreatePanel(x)));
        Application.DoEvents();
      }
      else
      {
        var del = new populate_delegate(populate);
        Invoke(del);
      }
    }

    #region PageHandler
    public Panel CreatePanel(Period p)
    {
      var labelPeriodId = new Label();
      labelPeriodId.AutoSize = true;
      labelPeriodId.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      labelPeriodId.ForeColor = System.Drawing.Color.White;
      labelPeriodId.Location = new System.Drawing.Point(11, 27);
      labelPeriodId.Name = "labelPeriodId";
      labelPeriodId.Size = new System.Drawing.Size(79, 18);
      labelPeriodId.TabIndex = 1;
      labelPeriodId.Text = p.Id.ToString();
      labelPeriodId.Visible = false;

      var labelName = new Label();
      labelName.AutoSize = true;
      labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      labelName.ForeColor = System.Drawing.Color.White;
      labelName.Location = new System.Drawing.Point(12, 23);
      labelName.Name = "labelName";
      labelName.Size = new System.Drawing.Size(55, 16);
      labelName.TabIndex = 0;
      labelName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      var shopPrice = ShopPrices.First(x => x.Id == p.ShopPriceId);
      var name = (shopPrice.Period + " " + shopPrice.PeriodType).ToUpper();
      var realName = name.Equals("0 NONE") ? "PERMANENT" : name;
      labelName.Text = $"NAME: {realName}";

      var labelPriceGroupName = new Label();
      labelPriceGroupName.AutoSize = true;
      labelPriceGroupName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      labelPriceGroupName.ForeColor = System.Drawing.Color.White;
      labelPriceGroupName.Location = new System.Drawing.Point(200, 23);
      labelPriceGroupName.Name = "labelPriceGroupName";
      labelPriceGroupName.Size = new System.Drawing.Size(55, 16);
      labelPriceGroupName.TabIndex = 0;
      labelPriceGroupName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      var shopPriceGroupId = ShopPrices.First(x => x.Id == p.ShopPriceId).PriceGroupId;
      var shopPriceGroupName = ShopPriceGroups.First(x => x.Id == shopPriceGroupId).Name;
      labelPriceGroupName.Text = $"PRICE GROUP: {shopPriceGroupName}";

      var buttonRemove = new Button();
      buttonRemove.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
      buttonRemove.FlatAppearance.BorderSize = 0;
      buttonRemove.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      buttonRemove.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      buttonRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      buttonRemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      buttonRemove.ForeColor = System.Drawing.Color.White;
      buttonRemove.Location = new System.Drawing.Point(356, 20);
      buttonRemove.Name = "buttonRemove";
      buttonRemove.Size = new System.Drawing.Size(87, 23);
      buttonRemove.TabIndex = 1;
      buttonRemove.Text = "REMOVE";
      buttonRemove.UseVisualStyleBackColor = false;
      buttonRemove.Click += buttonRemove_Click;

      var panelPeriod = new Panel();
      panelPeriod.Controls.Add(buttonRemove);
      panelPeriod.Controls.Add(labelName);
      panelPeriod.Controls.Add(labelPriceGroupName);
      panelPeriod.Controls.Add(labelPeriodId);
      panelPeriod.Dock = System.Windows.Forms.DockStyle.Top;
      panelPeriod.Location = new System.Drawing.Point(0, 0);
      panelPeriod.Name = "panelPeriod";
      panelPeriod.Size = new System.Drawing.Size(467, 59);
      panelPeriod.BorderStyle = BorderStyle.FixedSingle;

      return panelPeriod;
    }
    #endregion

    private void buttonAdd_Click(object sender, EventArgs e)
    {
      var showPriceForm = new ShowPriceUI();
      showPriceForm.ShowDialog();
    }

    private async void buttonRemove_Click(object sender, EventArgs e)
    {
      var periodId = ((Button)sender).Parent.Controls.Find("labelPeriodId", true)[0].Text;
      var period = Periods.First(x => x.Id == int.Parse(periodId));
      var task = PeriodService.Instance.Delete(period);
      var result = await task;
      if (task == null) 
      {
        MessageBox.Show("Something gone wrong");
        return;
      }

      MessageBox.Show("Successfully removed");
    }
  }
}
