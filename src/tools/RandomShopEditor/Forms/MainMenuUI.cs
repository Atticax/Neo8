using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using MySql.Data.MySqlClient;
using Netsphere.Tools.RandomShopEditor.Services;
using RandomShopEditor.Forms;
using RandomShopEditor.Services;

namespace RandomShopEditor
{
  public partial class MainMenuUI : Form
  {
    private Point start;
    private Form activeForm;
    private string resourcePath;

    private readonly Dictionary<string, Form> _childForms = new Dictionary<string, Form>
    {
      { "ColorUI",   null },
      { "EffectUI",  null },
      { "PeriodUI",  null },
      { "LineupUI",  null },
      { "PackageUI", null }
    };

    public MainMenuUI()
    {
      InitializeComponent();
      panelChildren.DragEnter += (s, e) =>
      {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
          e.Effect = DragDropEffects.Copy;
      };

      panelChildren.DragDrop += (s, e) =>
      {
        var files = (string[])e.Data.GetData(DataFormats.FileDrop);
        if (files.Length != 1)
        {
          e.Effect = DragDropEffects.None;
          return;
        }

        resourcePath = files[0];
        if (!Path.GetExtension(resourcePath).Equals(".s4hd"))
        {
          e.Effect = DragDropEffects.None;
          return;
        }

        labelResourcePath.Text = resourcePath;
        label2.Visible = false;
      };
    }

    #region MoveEvent
    private void MainMenu_MouseDown(object sender, MouseEventArgs e)
      => start = new Point(-e.X, -e.Y);

    private void MainMenu_MouseMove(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        var mouseMove = Control.MousePosition;
        mouseMove.Offset(start.X, start.Y);
        Location = mouseMove;
      }
    }
    #endregion

    #region ButtonClick
    private async void btnConnect_Click(object sender, System.EventArgs e)
    {
      if (string.IsNullOrEmpty(resourcePath))
      {
        MessageBox.Show("Please, drop resource.s4hd");
        return;
      }

      buttonConnect.Text = "Connecting..";
      try
      {
        var connectionStringBuilder = new MySqlConnectionStringBuilder
        {
          Server = textBoxDatabase.Text,
          Port = 3306,
          Database = textBoxTable.Text,
          UserID = textBoxUsername.Text,
          Password = textBoxPassword.Text,
          SslMode = MySqlSslMode.None
        };

        progressBar1.Visible = true;
        RSService.Instance.LoadDatabase(connectionStringBuilder);
        RSService.Instance.LoadZip(resourcePath);
        ResourceService.Instance.ProgressChanged += (snd, @event) =>
        {
          progressBar1.Maximum = @event.TotalItems;
          progressBar1.Value = @event.CurrentItems;

          Application.DoEvents();
        };
        await ShopItemInfoService.Instance.LoadFromDatabase();
        ResourceService.Instance.Load();
        await PackageService.Instance.LoadFromDatabase();
        await LineupService.Instance.LoadFromDatabase();
        await EffectService.Instance.LoadFromDatabase();
        await ShopPriceService.Instance.LoadFromDatabase();
        await ShopPriceGroupService.Instance.LoadFromDatabase();
        await PeriodService.Instance.LoadFromDatabase();
        await ColorService.Instance.LoadFromDatabase();
        buttonConnect.Text = "Connected";
        buttonConnect.Enabled = false;
        btnDatabase.Enabled = false;

        btnPackages.Enabled = true;
        btnLineups.Enabled = true;
        btnPeriods.Enabled = true;
        btnEffects.Enabled = true;
        btnColors.Enabled = true;
        //_connection.Close();
      }
      catch (Exception es)
      {
        MessageBox.Show(es.Message);
      }
    }

    private async void btnPackages_Click(object sender, EventArgs e)
    {
      await PackageService.Instance.LoadFromDatabase();
      openChildForm<PackageUI>();
    }

    private async void btnLineups_Click(object sender, EventArgs e)
    {
      await LineupService.Instance.LoadFromDatabase();
      openChildForm<LineupUI>();
    }

    private void btnDatabase_Click(object sender, EventArgs e)
      => panelChildren.Visible = true;

    private void btnPeriods_Click(object sender, EventArgs e)
      => openChildForm<PeriodUI>();

    private void btnEffects_Click(object sender, EventArgs e)
      => openChildForm<EffectUI>();

    private void btnColors_Click(object sender, EventArgs e)
      => openChildForm<ColorUI>();

    #endregion

    private void openChildForm<T>()
      where T : Form
    {
      var name = typeof(T).Name;

      if (activeForm != null)
        activeForm.Hide();

      var childForm = _childForms[name] ?? Activator.CreateInstance<T>();
      if (_childForms[name] == null)
        _childForms[name] = childForm;

      panelChildren.AllowDrop = false;
      labelResourcePath.Visible = false;
      labelRPath.Visible = false;
      activeForm = childForm;
      childForm.TopLevel = false;
      childForm.FormBorderStyle = FormBorderStyle.None;
      panelChildren.Controls.Add(childForm);
      panelChildren.Tag = childForm.Tag;
      childForm.BringToFront();
      childForm.Show();
    }
  }
}
