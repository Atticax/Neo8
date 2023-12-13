using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using Netsphere.Tools.RandomShopEditor.Services;
using RandomShopEditor.Models;
using RandomShopEditor.Services;

namespace RandomShopEditor.Forms
{
  public partial class LineupUI : Form
  {
    private delegate void populate_delegate();

    public List<Package> Packages { get; set; }
    public List<Lineup> Lineups { get; set; }
    public Item[] Items { get; set; }

    public LineupUI()
    {
      InitializeComponent();

      Packages = PackageService.Instance.Packages;
      Lineups = LineupService.Instance.Lineups;
      Items = ResourceService.Instance.Items;
    }

    private void LineupUI_Load(object sender, EventArgs e)
      => Task.Run(populate);

    void populate()
    {
      if (!InvokeRequired)
      {
        foreach (var package in Packages)
        {
          var count = Lineups.Where(x => x.PackageId == package.Id).ToList().Count;
          createPackagesPanel(package.Id.ToString(), package.NameKey, count);

          Application.DoEvents();
        }
      }
      else
      {
        var del = new populate_delegate(populate);
        Invoke(del);
      }
    }

    private void createPackagesPanel(string id, string name, int count)
    {
      var labelPackageId = new Label();
      labelPackageId.AutoSize = true;
      labelPackageId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      labelPackageId.ForeColor = System.Drawing.Color.White;
      labelPackageId.Location = new System.Drawing.Point(13, 13);
      labelPackageId.Name = "labelPackageId";
      labelPackageId.Size = new System.Drawing.Size(85, 13);
      labelPackageId.TabIndex = 0;
      labelPackageId.Text = $"PACKAGE ID: {id}";

      var labelPackageName = new Label();
      labelPackageName.AutoSize = true;
      labelPackageName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      labelPackageName.ForeColor = System.Drawing.Color.White;
      labelPackageName.Location = new System.Drawing.Point(12, 43);
      labelPackageName.Name = "labelPackageName";
      labelPackageName.Size = new System.Drawing.Size(107, 13);
      labelPackageName.TabIndex = 1;
      labelPackageName.Text = $"PACKAGE NAME: {name}";

      var buttonAdd = new Button();
      buttonAdd.FlatAppearance.BorderSize = 0;
      buttonAdd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
      buttonAdd.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(166)))), ((int)(((byte)(36)))));
      buttonAdd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(136)))), ((int)(((byte)(36)))));
      buttonAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      buttonAdd.Location = new System.Drawing.Point(238, 38);
      buttonAdd.Name = "buttonAdd";
      buttonAdd.Size = new System.Drawing.Size(52, 23);
      buttonAdd.TabIndex = 2;
      buttonAdd.Text = "ADD";
      buttonAdd.UseVisualStyleBackColor = true;
      buttonAdd.Click += buttonAdd_Click;

      var labelItemsCount = new Label();
      labelItemsCount.AutoSize = true;
      labelItemsCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      labelItemsCount.ForeColor = System.Drawing.Color.White;
      labelItemsCount.Location = new System.Drawing.Point(241, 13);
      labelItemsCount.Name = "labelItemsCount";
      labelItemsCount.Size = new System.Drawing.Size(49, 13);
      labelItemsCount.TabIndex = 4;
      labelItemsCount.Text = $"ITEMS: {count}";

      var buttonRemove = new Button();
      buttonRemove.FlatAppearance.BorderSize = 0;
      buttonRemove.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      buttonRemove.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      buttonRemove.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
      buttonRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      buttonRemove.Location = new System.Drawing.Point(309, 38);
      buttonRemove.Name = "buttonRemove";
      buttonRemove.Size = new System.Drawing.Size(68, 23);
      buttonRemove.TabIndex = 5;
      buttonRemove.Text = "REMOVE";
      buttonRemove.UseVisualStyleBackColor = true;

      var buttonShow = new Button();
      buttonShow.FlatAppearance.BorderSize = 0;
      buttonShow.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(136)))), ((int)(((byte)(236)))));
      buttonShow.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(136)))), ((int)(((byte)(206)))));
      buttonShow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20))))); ;
      buttonShow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      buttonShow.Location = new System.Drawing.Point(392, 38);
      buttonShow.Name = "buttonShow";
      buttonShow.Size = new System.Drawing.Size(56, 23);
      buttonShow.TabIndex = 6;
      buttonShow.Text = "SHOW";
      buttonShow.UseVisualStyleBackColor = true;
      buttonShow.Click += buttonShow_Click;

      var panelPackage = new Panel();
      panelPackage.Controls.Add(buttonShow);
      panelPackage.Controls.Add(buttonRemove);
      panelPackage.Controls.Add(labelItemsCount);
      panelPackage.Controls.Add(buttonAdd);
      panelPackage.Controls.Add(labelPackageName);
      panelPackage.Controls.Add(labelPackageId);
      panelPackage.Dock = System.Windows.Forms.DockStyle.Top;
      panelPackage.Location = new System.Drawing.Point(0, 0);
      panelPackage.Name = "panelPackage";
      panelPackage.Size = new System.Drawing.Size(451, 75);
      panelPackage.TabIndex = 0;

      Controls.Add(panelPackage);
    }

    private async void buttonShow_Click(object sender, EventArgs e)
    {
      await LineupService.Instance.LoadFromDatabase();
      Lineups = LineupService.Instance.Lineups;
      var panel = (Panel)((Button)sender).Parent;
      var package = panel.Controls.Find("labelPackageId", true).First().Text.Split(':')[1].Trim();
      var lineups = Lineups.Where(x => x.PackageId == int.Parse(package)).ToList();
      var items = new Item[lineups.Count];
      var i = 0;
      foreach (var lineup in lineups)
        items[i++] = Items.First(x => x.ItemNumber.Id == lineup.ShopItemId);

      var showItem = new ShowItemUI(lineups);
      showItem.ShowDialog();
    }

    private void buttonAdd_Click(object sender, EventArgs e) 
    {
      var packageId = ((Button)sender).Parent.Controls.Find("labelPackageId", true)[0].Text.Split(':')[1].Trim();
      var itemUI = new ItemsUI(int.Parse(packageId));
      itemUI.ShowDialog();
    }
  }
}
