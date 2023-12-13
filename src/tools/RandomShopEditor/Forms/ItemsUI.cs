using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Netsphere.Tools.RandomShopEditor.Services;

namespace RandomShopEditor.Forms
{
  public partial class ItemsUI : Form
  {
    private Point startPoint;
    public int Start { get; set; }
    public int Stop { get; set; }
    public int PackageId { get; set; }
    public Item[] Items { get; set; }

    public ItemsUI(int packageId)
    {
      InitializeComponent();
      Items = ResourceService.Instance.Items;
      Start = 0;
      Stop = Items.Length;
      PackageId = packageId;
      populatePage(Items);
    }

    #region PageHandler
    private Panel createBox(string name, Bitmap bitmap, string itemId)
    {
      var labelName = new Label();
      labelName.Font = new Font("Microsoft Sans Serif", 10.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
      labelName.ForeColor = Color.White;
      labelName.Name = "labelName";
      labelName.Location = new Point(106, 13);
      labelName.Size = new Size(139, 47);
      labelName.TextAlign = ContentAlignment.MiddleCenter;
      labelName.AutoSize = false;
      labelName.Text = name;

      var labelItemId = new Label();
      labelItemId.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
      labelItemId.ForeColor = Color.White;
      labelItemId.Location = new Point(123, 30);
      labelItemId.Name = "labelItemId";
      labelItemId.Size = new Size(248, 30);
      labelItemId.TabIndex = 1;
      labelItemId.TextAlign = ContentAlignment.MiddleCenter;
      labelItemId.Text = itemId;
      labelItemId.TextAlign = ContentAlignment.MiddleCenter;
      labelItemId.Dock = DockStyle.Top;
      labelItemId.Visible = false;

      var pictureBox1 = new PictureBox();
      pictureBox1.BackgroundImageLayout = ImageLayout.None;
      pictureBox1.Name = "pictureBox1";
      pictureBox1.Location = new Point(0, 0);
      pictureBox1.Size = new Size(100, 84);
      pictureBox1.Dock = DockStyle.Left;
      pictureBox1.BackgroundImageLayout = ImageLayout.Center;
      pictureBox1.TabIndex = 0;
      pictureBox1.TabStop = false;
      pictureBox1.Image = new Bitmap(bitmap, new Size(60, 60));

      var buttonAdd = new Button();
      buttonAdd.Dock = DockStyle.Bottom;
      buttonAdd.FlatAppearance.BorderSize = 0;
      buttonAdd.FlatAppearance.MouseDownBackColor = Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      buttonAdd.FlatAppearance.MouseOverBackColor = Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      buttonAdd.FlatStyle = FlatStyle.Flat;
      buttonAdd.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0)));
      buttonAdd.ForeColor = Color.White;
      buttonAdd.Location = new Point(94, 45);
      buttonAdd.Name = "buttonAdd";
      buttonAdd.Size = new Size(154, 23);
      buttonAdd.TabIndex = 2;
      buttonAdd.Text = "ADD";
      buttonAdd.UseVisualStyleBackColor = true;
      buttonAdd.MouseDown += buttonAdd_Click;

      var panelX = new Panel();
      panelX.Dock = DockStyle.Bottom;
      panelX.Location = new Point(0, 204);
      panelX.Name = "panelX";
      panelX.Size = new Size(248, 84);
      panelX.TabIndex = 0;
      panelX.BorderStyle = BorderStyle.FixedSingle;

      panelX.Controls.Add(pictureBox1);
      panelX.Controls.Add(labelItemId);
      panelX.Controls.Add(labelName);
      panelX.Controls.Add(buttonAdd);
      return panelX;
    }

    private Panel createBottomPanel()
    {
      var buttonNextPage = new Button();
      buttonNextPage.Dock = DockStyle.Right;
      buttonNextPage.FlatAppearance.BorderSize = 0;
      buttonNextPage.FlatAppearance.MouseDownBackColor = Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(166)))), ((int)(((byte)(36)))));
      buttonNextPage.FlatAppearance.MouseOverBackColor = Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(136)))), ((int)(((byte)(36)))));
      buttonNextPage.FlatStyle = FlatStyle.Flat;
      buttonNextPage.ForeColor = Color.White;
      buttonNextPage.Location = new Point(168, 0);
      buttonNextPage.Margin = new Padding(3, 2, 3, 2);
      buttonNextPage.Name = "button1";
      buttonNextPage.Size = new Size(80, 26);
      buttonNextPage.TabIndex = 0;
      buttonNextPage.Text = "Next Page";
      buttonNextPage.UseVisualStyleBackColor = true;
      buttonNextPage.MouseDown += buttonNextPage_Click;

      var buttonPrevPage = new Button();
      buttonPrevPage.Dock = DockStyle.Left;
      buttonPrevPage.FlatAppearance.BorderSize = 0;
      buttonPrevPage.FlatAppearance.MouseDownBackColor = Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      buttonPrevPage.FlatAppearance.MouseOverBackColor = Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      buttonPrevPage.FlatStyle = FlatStyle.Flat;
      buttonPrevPage.ForeColor = Color.White;
      buttonPrevPage.Location = new Point(0, 0);
      buttonPrevPage.Margin = new Padding(3, 2, 3, 2);
      buttonPrevPage.Name = "button2";
      buttonPrevPage.Size = new Size(80, 26);
      buttonPrevPage.TabIndex = 1;
      buttonPrevPage.Text = "Prev Page";
      buttonPrevPage.UseVisualStyleBackColor = true;
      buttonPrevPage.Click += buttonPrevPage_Click;

      var panelBottomBar = new Panel();
      panelBottomBar.Controls.Add(buttonPrevPage);
      panelBottomBar.Controls.Add(buttonNextPage);
      panelBottomBar.Dock = DockStyle.Bottom;
      panelBottomBar.Location = new Point(0, 340);
      panelBottomBar.Margin = new Padding(3, 2, 3, 2);
      panelBottomBar.Name = "panelBottomBar";
      panelBottomBar.Size = new Size(248, 26);
      panelBottomBar.TabIndex = 2;

      return panelBottomBar;
    }

    private void populatePageInverse(Item[] items)
    {
      var inStart = Start;
      while (Start == 0)
      {
        if (inStart - 30 > Start)
          break;
        Controls.Add(createBox(items[Start].Name, items[Start].Image, items[Start].ItemNumber.Id.ToString()));
        Start--;
      }

      Controls.Add(createBottomPanel());
    }

    private void populatePage(Item[] items)
    {
      var inStart = Start;
      while (Start < items.Length)
      {
        if (Start > inStart + 30)
          break;

        Controls.Add(createBox(items[Start].Name, items[Start].Image, items[Start].ItemNumber.Id.ToString()));
        Start++;
      }

      Controls.Add(createBottomPanel());
    }
    #endregion

    #region MouseMove
    private void MainMenu_MouseDown(object sender, MouseEventArgs e) => startPoint = new Point(-e.X, -e.Y);
    private void MainMenu_MouseMove(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        var mouseMove = Control.MousePosition;
        mouseMove.Offset(startPoint.X, startPoint.Y);
        Location = mouseMove;
      }
    }
    #endregion

    #region ButtonClick
    private void buttonClose_Click(object sender, EventArgs e) => Close();

    private void textBoxSearch_MouseClick(object sender, EventArgs e) => textBoxSearch.Text = "";

    private void buttonSearch_Click(object sender, EventArgs e)
    {
      Start = 0;
      var searched = ((Button)sender).Parent.Controls.Find("textBoxSearch", true)[0].Text;
      if (string.IsNullOrEmpty(searched))
      {
        populatePage(Items);
        return;
      }

      var items = Items.Where(x => x.Name.IndexOf(searched, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();

      if (items.Length > 0)
      {
        var panels = Controls.OfType<Panel>().Where(x => x.Tag == null).ToList();
        panels.ForEach(x => Controls.Remove(x));
        populatePage(items);
      }
    }

    private void buttonAdd_Click(object sender, EventArgs e)
    {
      var itemId = ((Button)sender).Parent.Controls.Find("labelItemId", true)[0].Text;
      var showPopup = new PopupUI(int.Parse(itemId), PackageId);
      showPopup.ShowDialog();
    }

    private void buttonNextPage_Click(object sender, EventArgs e)
    {
      if (Start >= Items.Length)
        return;
      var panels = Controls.OfType<Panel>().Where(x => x.Tag == null).ToList();
      panels.ForEach(x => Controls.Remove(x));
      populatePage(Items);
    }

    private void buttonPrevPage_Click(object sender, EventArgs e)
    {
      if (Start < 30)
        return;
      var panels = Controls.OfType<Panel>().Where(x => !x.Tag.Equals("NR")).ToList();
      panels.ForEach(x => Controls.Remove(x));
      populatePageInverse(Items);
    }
    #endregion

  }
}
