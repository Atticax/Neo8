using System;
using System.Threading.Tasks;
using System.Windows.Forms;

using Netsphere.Tools.RandomShopEditor.Services;
using RandomShopEditor.Services;

namespace RandomShopEditor.Forms
{
  public partial class PackageUI : Form
  {
    private delegate void populate_delegate();

    public PackageService PackageService { get; set; }
    public PackageUI()
    {
      InitializeComponent();
    }

    private void PackageUI_Load(object sender, EventArgs e)
    {
      var packages = PackageService.Instance;
      if (packages.Packages.Count > 0)
        Task.Run(populate);
    }

    private void populate()
    {
      if (!InvokeRequired)
      {
        var packages = PackageService.Instance;
        foreach (var package in packages.Packages)
        {
          var panel = createPanel();
          insertButtons(panel);
          insertLabels(panel, package.NameKey, package.DescKey, package.Id);
          insertLabel(panel, package.Price);
          insertComboBox(panel, package.PriceType, package.RequiredGender, package.Enabled);
          Controls.Add(panel);

          Application.DoEvents();
        }
      }
      else
      {
        var del = new populate_delegate(populate);
        Invoke(del);
      }
    }

    private Panel createPanel()
    {
      var panelPackage = new Panel();
      panelPackage.Dock = System.Windows.Forms.DockStyle.Top;
      panelPackage.Location = new System.Drawing.Point(0, 0);
      panelPackage.Name = "panel1";
      panelPackage.Size = new System.Drawing.Size(451, 107);

      var panelBar = new Panel();
      panelBar.Dock = System.Windows.Forms.DockStyle.Top;
      panelBar.Name = "panelBar";
      panelBar.Size = new System.Drawing.Size(451, 1);
      panelBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      panelBar.BorderStyle = BorderStyle.FixedSingle;
      panelPackage.Controls.Add(panelBar);

      return panelPackage;
    }

    private void insertButtons(Panel panel)
    {
      var buttonRemove = new Button();
      buttonRemove.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
      buttonRemove.FlatAppearance.BorderSize = 0;
      buttonRemove.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      buttonRemove.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      buttonRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      buttonRemove.Location = new System.Drawing.Point(348, 78);
      buttonRemove.Size = new System.Drawing.Size(75, 20);
      buttonRemove.Name = "button2";
      buttonRemove.Text = "REMOVE";
      buttonRemove.UseVisualStyleBackColor = false;
      buttonRemove.Click += removePackage_Click;

      var buttonUpdate = new Button();
      buttonUpdate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
      buttonUpdate.FlatAppearance.BorderSize = 0;
      buttonUpdate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(166)))), ((int)(((byte)(36)))));
      buttonUpdate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(136)))), ((int)(((byte)(36)))));
      buttonUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      buttonUpdate.Location = new System.Drawing.Point(348, 40);
      buttonUpdate.Size = new System.Drawing.Size(75, 20);
      buttonUpdate.Name = "button1";
      buttonUpdate.Text = "UPDATE";
      buttonUpdate.UseVisualStyleBackColor = false;
      buttonUpdate.Click += updatePackage_Click;

      panel.Controls.Add(buttonUpdate);
      panel.Controls.Add(buttonRemove);

      void MyButtonClick(object sender, EventArgs e)
      {
        var btn = (Button)sender;
        var selectedPanel = (Panel)btn.Parent;

        var id = selectedPanel.Controls.Find("textBoxId", true)[0].Text;
        var nameKey = selectedPanel.Controls.Find("textBoxName", true)[0].Text;
        var descKey = selectedPanel.Controls.Find("textBoxDesc", true)[0].Text;
        var priceType = selectedPanel.Controls.Find("comboBoxPriceType", true)[0].Text;
        var gender = selectedPanel.Controls.Find("comboBoxGender", true)[0].Text;
        var isEnabled = selectedPanel.Controls.Find("comboBoxEnabled", true)[0].Text;
      }
    }

    private void insertLabels(Panel panel, string name, string desc, int id)
    {
      var labelEnabled = new Label();
      labelEnabled.AutoSize = true;
      labelEnabled.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      labelEnabled.ForeColor = System.Drawing.Color.White;
      labelEnabled.Location = new System.Drawing.Point(165, 80);
      labelEnabled.Name = "label5";
      labelEnabled.Size = new System.Drawing.Size(64, 13);
      labelEnabled.Text = "ENABLED";

      var labelGender = new Label();
      labelGender.AutoSize = true;
      labelGender.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      labelGender.ForeColor = System.Drawing.Color.White;
      labelGender.Location = new System.Drawing.Point(3, 80);
      labelGender.Name = "label4";
      labelGender.Size = new System.Drawing.Size(59, 13);
      labelGender.Text = "GENDER";

      var labelPrice = new Label();
      labelPrice.AutoSize = true;
      labelPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      labelPrice.ForeColor = System.Drawing.Color.White;
      labelPrice.Location = new System.Drawing.Point(150, 45);
      labelPrice.Name = "label_3";
      labelPrice.Size = new System.Drawing.Size(80, 13);
      labelPrice.Text = "PRICE";

      var labelPriceType = new Label();
      labelPriceType.AutoSize = true;
      labelPriceType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      labelPriceType.ForeColor = System.Drawing.Color.White;
      labelPriceType.Location = new System.Drawing.Point(3, 45);
      labelPriceType.Name = "label3";
      labelPriceType.Size = new System.Drawing.Size(80, 13);
      labelPriceType.Text = "TYPE";

      var labelDesc = new Label();
      labelDesc.AutoSize = true;
      labelDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      labelDesc.ForeColor = System.Drawing.Color.White;
      labelDesc.Location = new System.Drawing.Point(306, 10);
      labelDesc.Name = "label2";
      labelDesc.Size = new System.Drawing.Size(40, 13);
      var miniDesc = desc.Substring(0, 8) + "...";
      labelDesc.Text = $"DESC: {miniDesc}";

      var descTT = new System.Windows.Forms.ToolTip();
      descTT.SetToolTip(labelDesc, desc);

      var labelName = new Label();
      labelName.AutoSize = true;
      labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      labelName.ForeColor = System.Drawing.Color.White;
      labelName.Location = new System.Drawing.Point(110, 10);
      labelName.Name = "label1";
      labelName.Size = new System.Drawing.Size(42, 13);
      labelName.Text = $"NAME: {name}";

      var labelId = new Label();
      labelId.AutoSize = true;
      labelId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      labelId.ForeColor = System.Drawing.Color.White;
      labelId.Location = new System.Drawing.Point(3, 10);
      labelId.Name = "labelId";
      labelId.Text = $"ID: {id}";

      panel.Controls.Add(labelId);
      panel.Controls.Add(labelName);
      panel.Controls.Add(labelDesc);
      panel.Controls.Add(labelPriceType);
      panel.Controls.Add(labelGender);
      panel.Controls.Add(labelEnabled);
      panel.Controls.Add(labelPrice);
    }

    private void insertLabel(Panel panel, int price)
    {
      var numericPrice = new NumericUpDown();
      numericPrice.Location = new System.Drawing.Point(200, 43);
      numericPrice.Size = new System.Drawing.Size(100, 20);
      numericPrice.BorderStyle = System.Windows.Forms.BorderStyle.None;
      numericPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      numericPrice.Name = "numericPrice";
      numericPrice.Maximum = 9999999;
      numericPrice.Value = price;

      panel.Controls.Add(numericPrice);
    }

    private void insertComboBox(Panel panel, int priceType, byte gender, bool enabled)
    {
      var comboBoxPriceType = new ComboBox();
      comboBoxPriceType.FormattingEnabled = true;
      comboBoxPriceType.Location = new System.Drawing.Point(50, 40);
      comboBoxPriceType.Size = new System.Drawing.Size(90, 21);
      comboBoxPriceType.Name = "comboBoxPriceType";
      comboBoxPriceType.Items.AddRange(new string[] { "PEN", "AP", "COUPON", "PREMIUM" });
      switch (priceType)
      {
        case 1:
          comboBoxPriceType.Text = "PEN";
          break;
        case 2:
          comboBoxPriceType.Text = "AP";
          break;
        case 3:
          comboBoxPriceType.Text = "PREMIUM";
          break;
        case 4:
          comboBoxPriceType.Text = "???";
          break;
        case 5:
          comboBoxPriceType.Text = "COUPON";
          break;
      }

      var comboBoxGender = new ComboBox();
      comboBoxGender.FormattingEnabled = true;
      comboBoxGender.Location = new System.Drawing.Point(65, 75);
      comboBoxGender.Size = new System.Drawing.Size(90, 21);
      comboBoxGender.Name = "comboBoxGender";
      comboBoxGender.Items.AddRange(new string[] { "ALL", "MALE", "FEMALE" });
      switch (gender)
      {
        case 0:
          comboBoxGender.Text = "ALL";
          break;
        case 1:
          comboBoxGender.Text = "MALE";
          break;
        case 2:
          comboBoxGender.Text = "FEMALE";
          break;
      }

      var comboBoxEnabled = new ComboBox();
      comboBoxEnabled.FormattingEnabled = true;
      comboBoxEnabled.Location = new System.Drawing.Point(230, 75);
      comboBoxEnabled.Size = new System.Drawing.Size(45, 21);
      comboBoxEnabled.Name = "comboBoxEnabled";
      comboBoxEnabled.Items.AddRange(new string[] { "YES", "NO" });
      comboBoxEnabled.Text = (enabled) ? "YES" : "NO";

      panel.Controls.Add(comboBoxEnabled);
      panel.Controls.Add(comboBoxGender);
      panel.Controls.Add(comboBoxPriceType);
    }

    private async void addPackage_Click(object sender, EventArgs e)
    {
      var rsPackages = ResourceService.Instance.RandomShopPackage;
      var rsInfo = ResourceService.Instance.RandomShopInfoStringTable;
      foreach (var rsPackage in rsPackages.RSPackage)
      {
        var task = PackageService.Instance.NewPackage(rsPackage);
        var packageEntity = await task;
        if (packageEntity == null)
        {
          MessageBox.Show("Something gone wrong..");
          return;
        }
      }

      MessageBox.Show($"Added {rsPackages.RSPackage.Length} packages");
      await Task.Run(populate);
     }

    private async void updatePackage_Click(object sender, EventArgs e)
    {
      var panel = (Panel)((Button)sender).Parent;
      var id = panel.Controls.Find("labelId", true)[0].Text.Split(':')[1].Trim();
      var priceType = panel.Controls.Find("comboBoxPriceType", true)[0].Text;
      var price = ((NumericUpDown)panel.Controls.Find("numericPrice", true)[0]).Value;
      var gender = panel.Controls.Find("comboBoxGender", true)[0].Text;
      var enabled = panel.Controls.Find("comboBoxEnabled", true)[0].Text;

      var task = PackageService.Instance.UpdatePackage(id,priceType, price, gender, enabled);
      var result = await task;
      if (!result)
      {
        MessageBox.Show("Incorrect data...");
        return;
      }

      MessageBox.Show("Successfully updated");
    }

    private async void removePackage_Click(object sender, EventArgs e)
    {
      var panel = (Panel)((Button)sender).Parent;
      var id = (int)((NumericUpDown)panel.Controls.Find("numericId", true)[0]).Value;

      var task = PackageService.Instance.RemovePackage(id);
      var result = await task;
      if (!result)
      {
        MessageBox.Show("Incorrect data...");
        return;
      }

      MessageBox.Show("Successfully removed");
      Controls.Remove(panel);
    }
  }
}
