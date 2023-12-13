using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Netsphere.Server.Game.Data;
using Netsphere.Tools.RandomShopEditor.Services;
using RandomShopEditor.Models;
using RandomShopEditor.Services;

using DColor = System.Drawing.Color;

namespace RandomShopEditor.Forms
{
  public partial class ShowItemUI : Form
  {
    private Point startPoint;
    public int Start { get; set; }
    public int Stop { get; set; }
    public List<Lineup> Lineups { get; set; }
    public List<Period> Periods { get; set; }
    public List<Models.Effect> Effects { get; set; }
    public List<Models.Color> Colors { get; set; }
    public Item[] Items { get; set; }
    public List<ShopPrice> ShopPrices { get; }
    public List<ShopPriceGroup> ShopPriceGroups { get; }
    public ShowItemUI(List<Lineup> lineups)
    {
      InitializeComponent();
      Lineups = lineups;
      Items = ResourceService.Instance.Items;
      Effects = EffectService.Instance.Effects;
      Colors = ColorService.Instance.Colors;
      Periods = PeriodService.Instance.Periods;
      ShopPrices = ShopPriceService.Instance.ShopPrices;
      ShopPriceGroups = ShopPriceGroupService.Instance.ShopPriceGroups;
      Start = 0;
      Stop = 15;
      populatePage();
    }

    private void buttonClose_Click(object sender, EventArgs e) => Close();

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
    private void buttonNextPage_Click(object sender, EventArgs e)
    {
      if (Start >= Lineups.Count)
        return;

      Controls.Clear();
      populatePage();
    }

    private void buttonPrevPage_Click(object sender, EventArgs e)
    {
      if (Start < 15)
        return;

      Controls.Clear();
      populatePageInverse();
    }

    private async void buttonRemove_Click(object sender, EventArgs e)
    {
      var panel = (Panel)((Button)sender).Parent;
      var id = int.Parse(panel.Controls.Find("labelId", true)[0].Text);

      var task = LineupService.Instance.RemoveLineup(id);
      var result = await task;
      if (!result)
      {
        MessageBox.Show("Incorrect data...");
        return;
      }

      MessageBox.Show("Successfully removed");
      Controls.Remove(panel);
    }
    #endregion

    #region PageHandler
    private Panel createBox(int id, string name, Bitmap bitmap, string color, string period, string effect, string prob, string grade)
    {
      var labelId = new Label();
      labelId.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
      labelId.ForeColor = DColor.White;
      labelId.Location = new Point(110, 0);
      labelId.Name = "labelId";
      labelId.Size = new Size(271, 20);
      labelId.TabIndex = 1;
      labelId.TextAlign = ContentAlignment.MiddleCenter;
      labelId.Text = id.ToString();
      labelId.Visible = false;

      var pictureBox1 = new PictureBox();
      pictureBox1.Name = "pictureBox1";
      pictureBox1.Location = new Point(0, 0);
      pictureBox1.Size = new Size(100, 100);
      pictureBox1.Dock = DockStyle.Left;
      pictureBox1.BackgroundImageLayout = ImageLayout.Center;
      pictureBox1.TabIndex = 0;
      pictureBox1.TabStop = false;
      pictureBox1.Image = bitmap;

      var labelProb = new Label();
      labelProb.AutoSize = true;
      labelProb.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
      labelProb.ForeColor = DColor.White;
      labelProb.Location = new Point(232, 57);
      labelProb.Name = "labelProb";
      labelProb.Size = new Size(45, 13);
      labelProb.TabIndex = 35;
      labelProb.Text = $"PROB: {prob}";

      var labelGrade = new Label();
      labelGrade.AutoSize = true;
      labelGrade.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
      labelGrade.ForeColor = DColor.White;
      labelGrade.Location = new Point(311, 57);
      labelGrade.Name = "labelGrade";
      labelGrade.Size = new Size(54, 13);
      labelGrade.TabIndex = 36;
      labelGrade.Text = $"GRADE:{grade}";

      var labelName = new Label();
      labelName.AutoSize = true;
      labelName.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
      labelName.ForeColor = DColor.White;
      labelName.Location = new Point(102, 0);
      labelName.Name = "labelName";
      labelName.Size = new Size(42, 13);
      labelName.TabIndex = 1;
      labelName.Text = name;

      var labelPeriod = new Label();
      labelPeriod.AutoSize = true;
      labelPeriod.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
      labelPeriod.ForeColor = DColor.White;
      labelPeriod.Location = new Point(102, 29);
      labelPeriod.Name = "labelPeriod";
      labelPeriod.Size = new Size(58, 13);
      labelPeriod.TabIndex = 2;
      labelPeriod.Text = $"PERIOD: {period}";

      var labelColor = new Label();
      labelColor.AutoSize = true;
      labelColor.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
      labelColor.ForeColor = DColor.White;
      labelColor.Location = new Point(102, 83);
      labelColor.Name = "labelColor";
      labelColor.Size = new Size(53, 13);
      labelColor.TabIndex = 4;
      labelColor.Text = $"COLOR: {color}";

      var labelEffect = new Label();
      labelEffect.AutoSize = true;
      labelEffect.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
      labelEffect.ForeColor = DColor.White;
      labelEffect.Location = new Point(102, 57);
      labelEffect.Name = "labelEffect";
      labelEffect.Size = new Size(57, 13);
      labelEffect.TabIndex = 3;
      labelEffect.Text = $"EFFECT: {effect}";

      var buttonRemove = new Button();
      buttonRemove.FlatAppearance.BorderSize = 0;
      buttonRemove.FlatAppearance.MouseDownBackColor = DColor.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      buttonRemove.FlatAppearance.MouseOverBackColor = DColor.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      buttonRemove.FlatStyle = FlatStyle.Flat;
      buttonRemove.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
      buttonRemove.ForeColor = DColor.White;
      buttonRemove.Location = new Point(275, 74);
      buttonRemove.Margin = new Padding(3, 2, 3, 2);
      buttonRemove.Name = "buttonRemove";
      buttonRemove.Size = new Size(115, 24);
      buttonRemove.TabIndex = 34;
      buttonRemove.Text = "REMOVE";
      buttonRemove.UseVisualStyleBackColor = true;
      buttonRemove.Click += buttonRemove_Click;

      var panelX = new Panel();
      panelX.BorderStyle = BorderStyle.FixedSingle;
      panelX.Dock = DockStyle.Top;
      panelX.Location = new Point(0, 265);
      panelX.Name = "panelX";
      panelX.Size = new Size(395, 102);
      panelX.TabIndex = 4;

      panelX.Controls.Add(pictureBox1);
      panelX.Controls.Add(labelName);
      panelX.Controls.Add(labelId);
      panelX.Controls.Add(buttonRemove);
      panelX.Controls.Add(labelColor);
      panelX.Controls.Add(labelEffect);
      panelX.Controls.Add(labelPeriod);
      panelX.Controls.Add(labelProb);
      panelX.Controls.Add(labelGrade);
      return panelX;
    }

    private Panel createBottomPanel()
    {
      var buttonNextPage = new Button();
      buttonNextPage.Dock = DockStyle.Right;
      buttonNextPage.FlatAppearance.BorderSize = 0;
      buttonNextPage.FlatAppearance.MouseDownBackColor = DColor.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(166)))), ((int)(((byte)(36)))));
      buttonNextPage.FlatAppearance.MouseOverBackColor = DColor.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(136)))), ((int)(((byte)(36)))));
      buttonNextPage.FlatStyle = FlatStyle.Flat;
      buttonNextPage.ForeColor = DColor.White;
      buttonNextPage.Location = new Point(315, 0);
      buttonNextPage.Margin = new Padding(3, 2, 3, 2);
      buttonNextPage.Name = "buttonNextPage";
      buttonNextPage.Size = new Size(80, 24);
      buttonNextPage.TabIndex = 0;
      buttonNextPage.Text = "Next Page";
      buttonNextPage.UseVisualStyleBackColor = true;
      buttonNextPage.Click += buttonNextPage_Click;

      var buttonPrevPage = new Button();
      buttonPrevPage.Dock = DockStyle.Left;
      buttonPrevPage.FlatAppearance.BorderSize = 0;
      buttonPrevPage.FlatAppearance.MouseDownBackColor = DColor.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      buttonPrevPage.FlatAppearance.MouseOverBackColor = DColor.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      buttonPrevPage.FlatStyle = FlatStyle.Flat;
      buttonPrevPage.ForeColor = DColor.White;
      buttonPrevPage.Location = new Point(0, 0);
      buttonPrevPage.Margin = new Padding(3, 2, 3, 2);
      buttonPrevPage.Name = "buttonPrevPage";
      buttonPrevPage.Size = new Size(80, 24);
      buttonPrevPage.TabIndex = 1;
      buttonPrevPage.Text = "Prev Page";
      buttonPrevPage.UseVisualStyleBackColor = true;
      buttonPrevPage.Click += buttonPrevPage_Click;

      var panelBottomBar = new Panel();
      panelBottomBar.Controls.Add(buttonPrevPage);
      panelBottomBar.Controls.Add(buttonNextPage);
      panelBottomBar.Dock = DockStyle.Bottom;
      panelBottomBar.Location = new Point(0, 367);
      panelBottomBar.Margin = new Padding(3, 2, 3, 2);
      panelBottomBar.Name = "panelBottomBar";
      panelBottomBar.Size = new Size(395, 24);
      panelBottomBar.TabIndex = 2;
      panelBottomBar.Tag = "NR";

      return panelBottomBar;
    }

    public Panel createTopPanel()
    {
      var buttonClose = new Button();
      buttonClose.Dock = DockStyle.Right;
      buttonClose.FlatAppearance.BorderSize = 0;
      buttonClose.FlatStyle = FlatStyle.Flat;
      buttonClose.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
      buttonClose.Location = new Point(374, 0);
      buttonClose.Name = "buttonClose";
      buttonClose.Size = new Size(21, 20);
      buttonClose.TabIndex = 0;
      buttonClose.Text = "X";
      buttonClose.UseVisualStyleBackColor = true;
      buttonClose.Click += this.buttonClose_Click;

      var panelTopBar = new Panel();
      panelTopBar.BackColor = DColor.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      panelTopBar.Controls.Add(buttonClose);
      panelTopBar.Dock = DockStyle.Top;
      panelTopBar.Location = new Point(0, 0);
      panelTopBar.Name = "panelTopBar";
      panelTopBar.Size = new Size(395, 20);
      panelTopBar.TabIndex = 3;
      panelTopBar.Tag = "NR";
      panelTopBar.MouseDown += MainMenu_MouseDown;
      panelTopBar.MouseMove += MainMenu_MouseMove;

      return panelTopBar;
    }

    public void populatePage()
    {
      var i = 0;
      for (i = Start; i < Stop; i++)
      {
        if (i >= Lineups.Count)
          break;

        var effect = Effects.Find(x => x.Id == Lineups[i].EffectId).Name;
        var color = Colors.Find(x => x.Id == Lineups[i].ColorId).Name;
        var period = Periods.Find(x => x.Id == Lineups[i].PeriodId).Name;
        var item = Items.First(x => x.ItemNumber.Id == Lineups[i].ShopItemId);
        var prob = Lineups[i].Probability.ToString();
        var grade = Lineups[i].Grade.ToString();

        var shopPrices = new Dictionary<int, string>();
        ShopPrices.ForEach(x => shopPrices.Add(x.Id, x.Period + " " + x.PeriodType));

        Controls.Add(createBox(Lineups[i].Id, item.Name, item.Image, color, period, effect, prob, grade));
      }
      Controls.Add(createBottomPanel());
      Controls.Add(createTopPanel());
      Start = i;
      Stop += 15;
    }

    public void populatePageInverse()
    {
      var i = 0;
      for (i = Stop; i >= Start; i--)
      {
        if (i == 0)
          break;

        var effect = Effects.Find(x => x.Id == Lineups[i].EffectId).Name;
        var color = Colors.Find(x => x.Id == Lineups[i].ColorId).Name;
        var period = Periods.Find(x => x.Id == Lineups[i].PeriodId).Name;
        var item = Items.First(x => x.ItemNumber.Id == Lineups[i].ShopItemId);
        var prob = Lineups[i].Probability.ToString();
        var grade = Lineups[i].Grade.ToString();

        Controls.Add(createBox(Lineups[i].Id, item.Name, item.Image, color, period, effect, period, grade));
      }

      Controls.Add(createBottomPanel());
      Controls.Add(createTopPanel());
      Start = i;
      Stop -= 15;
    }
    #endregion
  }
}
