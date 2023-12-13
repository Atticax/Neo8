using ProgressBarSample;

namespace RandomShopEditor
{
  partial class MainMenuUI
  {
    /// <summary>
    /// Variabile di progettazione necessaria.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Pulire le risorse in uso.
    /// </summary>
    /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Codice generato da Progettazione Windows Form

    /// <summary>
    /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
    /// il contenuto del metodo con l'editor di codice.
    /// </summary>
    private void InitializeComponent()
    {
      this.panelButtons = new System.Windows.Forms.Panel();
      this.btnColors = new System.Windows.Forms.Button();
      this.btnEffects = new System.Windows.Forms.Button();
      this.btnPeriods = new System.Windows.Forms.Button();
      this.btnLineups = new System.Windows.Forms.Button();
      this.btnPackages = new System.Windows.Forms.Button();
      this.btnDatabase = new System.Windows.Forms.Button();
      this.panelNavBar = new System.Windows.Forms.Panel();
      this.panelName = new System.Windows.Forms.Panel();
      this.labelName = new System.Windows.Forms.Label();
      this.panelChildren = new System.Windows.Forms.Panel();
      this.progressBar1 = new ProgressBarSample.TextProgressBar();
      this.labelResourcePath = new System.Windows.Forms.Label();
      this.labelRPath = new System.Windows.Forms.Label();
      this.buttonConnect = new System.Windows.Forms.Button();
      this.textBoxPassword = new System.Windows.Forms.TextBox();
      this.textBoxUsername = new System.Windows.Forms.TextBox();
      this.textBoxTable = new System.Windows.Forms.TextBox();
      this.textBoxDatabase = new System.Windows.Forms.TextBox();
      this.labelPassword = new System.Windows.Forms.Label();
      this.labelUsername = new System.Windows.Forms.Label();
      this.labelTable = new System.Windows.Forms.Label();
      this.labelDatabase = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.panelButtons.SuspendLayout();
      this.panelName.SuspendLayout();
      this.panelChildren.SuspendLayout();
      this.SuspendLayout();
      // 
      // panelButtons
      // 
      this.panelButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.panelButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
      this.panelButtons.Controls.Add(this.btnColors);
      this.panelButtons.Controls.Add(this.btnEffects);
      this.panelButtons.Controls.Add(this.btnPeriods);
      this.panelButtons.Controls.Add(this.btnLineups);
      this.panelButtons.Controls.Add(this.btnPackages);
      this.panelButtons.Controls.Add(this.btnDatabase);
      this.panelButtons.Location = new System.Drawing.Point(0, 29);
      this.panelButtons.Name = "panelButtons";
      this.panelButtons.Size = new System.Drawing.Size(218, 386);
      this.panelButtons.TabIndex = 0;
      // 
      // btnColors
      // 
      this.btnColors.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
      this.btnColors.Enabled = false;
      this.btnColors.FlatAppearance.BorderSize = 0;
      this.btnColors.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.btnColors.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.btnColors.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.btnColors.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnColors.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnColors.ForeColor = System.Drawing.Color.White;
      this.btnColors.Location = new System.Drawing.Point(0, 304);
      this.btnColors.Margin = new System.Windows.Forms.Padding(0);
      this.btnColors.Name = "btnColors";
      this.btnColors.Size = new System.Drawing.Size(218, 45);
      this.btnColors.TabIndex = 6;
      this.btnColors.Text = "COLORS";
      this.btnColors.UseVisualStyleBackColor = false;
      this.btnColors.Click += new System.EventHandler(this.btnColors_Click);
      // 
      // btnEffects
      // 
      this.btnEffects.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
      this.btnEffects.Enabled = false;
      this.btnEffects.FlatAppearance.BorderSize = 0;
      this.btnEffects.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.btnEffects.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.btnEffects.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.btnEffects.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnEffects.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnEffects.ForeColor = System.Drawing.Color.White;
      this.btnEffects.Location = new System.Drawing.Point(0, 259);
      this.btnEffects.Margin = new System.Windows.Forms.Padding(0);
      this.btnEffects.Name = "btnEffects";
      this.btnEffects.Size = new System.Drawing.Size(218, 45);
      this.btnEffects.TabIndex = 5;
      this.btnEffects.Text = "EFFECTS";
      this.btnEffects.UseVisualStyleBackColor = false;
      this.btnEffects.Click += new System.EventHandler(this.btnEffects_Click);
      // 
      // btnPeriods
      // 
      this.btnPeriods.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
      this.btnPeriods.Enabled = false;
      this.btnPeriods.FlatAppearance.BorderSize = 0;
      this.btnPeriods.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.btnPeriods.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.btnPeriods.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.btnPeriods.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnPeriods.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnPeriods.ForeColor = System.Drawing.Color.White;
      this.btnPeriods.Location = new System.Drawing.Point(0, 214);
      this.btnPeriods.Margin = new System.Windows.Forms.Padding(0);
      this.btnPeriods.Name = "btnPeriods";
      this.btnPeriods.Size = new System.Drawing.Size(218, 45);
      this.btnPeriods.TabIndex = 4;
      this.btnPeriods.Text = "PERIODS";
      this.btnPeriods.UseVisualStyleBackColor = false;
      this.btnPeriods.Click += new System.EventHandler(this.btnPeriods_Click);
      // 
      // btnLineups
      // 
      this.btnLineups.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
      this.btnLineups.Enabled = false;
      this.btnLineups.FlatAppearance.BorderSize = 0;
      this.btnLineups.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.btnLineups.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.btnLineups.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.btnLineups.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnLineups.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnLineups.ForeColor = System.Drawing.Color.White;
      this.btnLineups.Location = new System.Drawing.Point(0, 169);
      this.btnLineups.Margin = new System.Windows.Forms.Padding(0);
      this.btnLineups.Name = "btnLineups";
      this.btnLineups.Size = new System.Drawing.Size(218, 45);
      this.btnLineups.TabIndex = 3;
      this.btnLineups.Text = "LINEUPS";
      this.btnLineups.UseVisualStyleBackColor = false;
      this.btnLineups.Click += new System.EventHandler(this.btnLineups_Click);
      // 
      // btnPackages
      // 
      this.btnPackages.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
      this.btnPackages.Enabled = false;
      this.btnPackages.FlatAppearance.BorderSize = 0;
      this.btnPackages.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.btnPackages.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.btnPackages.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.btnPackages.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnPackages.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnPackages.ForeColor = System.Drawing.Color.White;
      this.btnPackages.Location = new System.Drawing.Point(0, 124);
      this.btnPackages.Margin = new System.Windows.Forms.Padding(0);
      this.btnPackages.Name = "btnPackages";
      this.btnPackages.Size = new System.Drawing.Size(218, 45);
      this.btnPackages.TabIndex = 2;
      this.btnPackages.Text = "PACKAGES";
      this.btnPackages.UseVisualStyleBackColor = false;
      this.btnPackages.Click += new System.EventHandler(this.btnPackages_Click);
      // 
      // btnDatabase
      // 
      this.btnDatabase.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
      this.btnDatabase.FlatAppearance.BorderSize = 0;
      this.btnDatabase.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.btnDatabase.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.btnDatabase.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.btnDatabase.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btnDatabase.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnDatabase.ForeColor = System.Drawing.Color.White;
      this.btnDatabase.Location = new System.Drawing.Point(0, 79);
      this.btnDatabase.Margin = new System.Windows.Forms.Padding(0);
      this.btnDatabase.Name = "btnDatabase";
      this.btnDatabase.Size = new System.Drawing.Size(218, 45);
      this.btnDatabase.TabIndex = 1;
      this.btnDatabase.Text = "DATABASE";
      this.btnDatabase.UseVisualStyleBackColor = false;
      this.btnDatabase.Click += new System.EventHandler(this.btnDatabase_Click);
      // 
      // panelNavBar
      // 
      this.panelNavBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.panelNavBar.Dock = System.Windows.Forms.DockStyle.Top;
      this.panelNavBar.Location = new System.Drawing.Point(0, 0);
      this.panelNavBar.Name = "panelNavBar";
      this.panelNavBar.Size = new System.Drawing.Size(694, 30);
      this.panelNavBar.TabIndex = 7;
      this.panelNavBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainMenu_MouseDown);
      this.panelNavBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainMenu_MouseMove);
      // 
      // panelName
      // 
      this.panelName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
      this.panelName.Controls.Add(this.labelName);
      this.panelName.Location = new System.Drawing.Point(0, 29);
      this.panelName.Name = "panelName";
      this.panelName.Size = new System.Drawing.Size(691, 65);
      this.panelName.TabIndex = 8;
      // 
      // labelName
      // 
      this.labelName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.labelName.AutoSize = true;
      this.labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelName.ForeColor = System.Drawing.Color.White;
      this.labelName.Location = new System.Drawing.Point(34, 9);
      this.labelName.Name = "labelName";
      this.labelName.Size = new System.Drawing.Size(166, 50);
      this.labelName.TabIndex = 0;
      this.labelName.Text = "Random Shop \r\nEditor";
      this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // panelChildren
      // 
      this.panelChildren.AllowDrop = true;
      this.panelChildren.Controls.Add(this.progressBar1);
      this.panelChildren.Controls.Add(this.labelResourcePath);
      this.panelChildren.Controls.Add(this.labelRPath);
      this.panelChildren.Controls.Add(this.buttonConnect);
      this.panelChildren.Controls.Add(this.textBoxPassword);
      this.panelChildren.Controls.Add(this.textBoxUsername);
      this.panelChildren.Controls.Add(this.textBoxTable);
      this.panelChildren.Controls.Add(this.textBoxDatabase);
      this.panelChildren.Controls.Add(this.labelPassword);
      this.panelChildren.Controls.Add(this.labelUsername);
      this.panelChildren.Controls.Add(this.labelTable);
      this.panelChildren.Controls.Add(this.labelDatabase);
      this.panelChildren.Controls.Add(this.label2);
      this.panelChildren.Location = new System.Drawing.Point(224, 98);
      this.panelChildren.Name = "panelChildren";
      this.panelChildren.Size = new System.Drawing.Size(467, 317);
      this.panelChildren.TabIndex = 9;
      this.panelChildren.Visible = false;
      // 
      // progressBar1
      // 
      this.progressBar1.CustomText = "Loading Resources: ";
      this.progressBar1.Location = new System.Drawing.Point(83, 230);
      this.progressBar1.Name = "progressBar1";
      this.progressBar1.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.progressBar1.Size = new System.Drawing.Size(316, 23);
      this.progressBar1.TabIndex = 0;
      this.progressBar1.TextColor = System.Drawing.Color.Black;
      this.progressBar1.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.progressBar1.Visible = false;
      this.progressBar1.VisualMode = ProgressBarSample.ProgressBarDisplayMode.CurrProgress;
      // 
      // labelResourcePath
      // 
      this.labelResourcePath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(76)))));
      this.labelResourcePath.Location = new System.Drawing.Point(172, 280);
      this.labelResourcePath.Name = "labelResourcePath";
      this.labelResourcePath.Size = new System.Drawing.Size(250, 32);
      this.labelResourcePath.TabIndex = 19;
      this.labelResourcePath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // labelRPath
      // 
      this.labelRPath.AutoSize = true;
      this.labelRPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelRPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(76)))));
      this.labelRPath.Location = new System.Drawing.Point(54, 293);
      this.labelRPath.Name = "labelRPath";
      this.labelRPath.Size = new System.Drawing.Size(112, 15);
      this.labelRPath.TabIndex = 18;
      this.labelRPath.Text = "Resource location: ";
      // 
      // buttonConnect
      // 
      this.buttonConnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.buttonConnect.FlatAppearance.BorderSize = 0;
      this.buttonConnect.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.buttonConnect.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.buttonConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.buttonConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.buttonConnect.ForeColor = System.Drawing.Color.White;
      this.buttonConnect.Location = new System.Drawing.Point(159, 174);
      this.buttonConnect.Name = "buttonConnect";
      this.buttonConnect.Size = new System.Drawing.Size(132, 41);
      this.buttonConnect.TabIndex = 17;
      this.buttonConnect.Text = "Connect";
      this.buttonConnect.UseVisualStyleBackColor = false;
      this.buttonConnect.Click += new System.EventHandler(this.btnConnect_Click);
      // 
      // textBoxPassword
      // 
      this.textBoxPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textBoxPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
      this.textBoxPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.textBoxPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxPassword.ForeColor = System.Drawing.Color.White;
      this.textBoxPassword.Location = new System.Drawing.Point(204, 141);
      this.textBoxPassword.Multiline = true;
      this.textBoxPassword.Name = "textBoxPassword";
      this.textBoxPassword.PasswordChar = '*';
      this.textBoxPassword.Size = new System.Drawing.Size(168, 20);
      this.textBoxPassword.TabIndex = 16;
      // 
      // textBoxUsername
      // 
      this.textBoxUsername.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textBoxUsername.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
      this.textBoxUsername.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.textBoxUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxUsername.ForeColor = System.Drawing.Color.White;
      this.textBoxUsername.Location = new System.Drawing.Point(204, 103);
      this.textBoxUsername.Multiline = true;
      this.textBoxUsername.Name = "textBoxUsername";
      this.textBoxUsername.Size = new System.Drawing.Size(168, 20);
      this.textBoxUsername.TabIndex = 15;
      this.textBoxUsername.Text = "root";
      // 
      // textBoxTable
      // 
      this.textBoxTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textBoxTable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
      this.textBoxTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.textBoxTable.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxTable.ForeColor = System.Drawing.Color.White;
      this.textBoxTable.Location = new System.Drawing.Point(204, 64);
      this.textBoxTable.Multiline = true;
      this.textBoxTable.Name = "textBoxTable";
      this.textBoxTable.Size = new System.Drawing.Size(168, 20);
      this.textBoxTable.TabIndex = 14;
      this.textBoxTable.Text = "game";
      // 
      // textBoxDatabase
      // 
      this.textBoxDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textBoxDatabase.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
      this.textBoxDatabase.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.textBoxDatabase.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxDatabase.ForeColor = System.Drawing.Color.White;
      this.textBoxDatabase.Location = new System.Drawing.Point(204, 26);
      this.textBoxDatabase.Multiline = true;
      this.textBoxDatabase.Name = "textBoxDatabase";
      this.textBoxDatabase.Size = new System.Drawing.Size(168, 20);
      this.textBoxDatabase.TabIndex = 13;
      this.textBoxDatabase.Text = "127.0.0.1";
      // 
      // labelPassword
      // 
      this.labelPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.labelPassword.AutoSize = true;
      this.labelPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelPassword.ForeColor = System.Drawing.Color.White;
      this.labelPassword.Location = new System.Drawing.Point(96, 136);
      this.labelPassword.Name = "labelPassword";
      this.labelPassword.Size = new System.Drawing.Size(91, 24);
      this.labelPassword.TabIndex = 12;
      this.labelPassword.Text = "password";
      // 
      // labelUsername
      // 
      this.labelUsername.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.labelUsername.AutoSize = true;
      this.labelUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelUsername.ForeColor = System.Drawing.Color.White;
      this.labelUsername.Location = new System.Drawing.Point(94, 98);
      this.labelUsername.Name = "labelUsername";
      this.labelUsername.Size = new System.Drawing.Size(95, 24);
      this.labelUsername.TabIndex = 11;
      this.labelUsername.Text = "username";
      // 
      // labelTable
      // 
      this.labelTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.labelTable.AutoSize = true;
      this.labelTable.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelTable.ForeColor = System.Drawing.Color.White;
      this.labelTable.Location = new System.Drawing.Point(116, 60);
      this.labelTable.Name = "labelTable";
      this.labelTable.Size = new System.Drawing.Size(50, 24);
      this.labelTable.TabIndex = 10;
      this.labelTable.Text = "table";
      // 
      // labelDatabase
      // 
      this.labelDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.labelDatabase.AutoSize = true;
      this.labelDatabase.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelDatabase.ForeColor = System.Drawing.Color.White;
      this.labelDatabase.Location = new System.Drawing.Point(98, 22);
      this.labelDatabase.Name = "labelDatabase";
      this.labelDatabase.Size = new System.Drawing.Size(86, 24);
      this.labelDatabase.TabIndex = 9;
      this.labelDatabase.Text = "database";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(76)))), ((int)(((byte)(76)))));
      this.label2.Location = new System.Drawing.Point(79, 256);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(320, 24);
      this.label2.TabIndex = 0;
      this.label2.Text = "Drop resources resource.s4hd here..";
      // 
      // MainMenuUI
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.ClientSize = new System.Drawing.Size(694, 415);
      this.Controls.Add(this.panelNavBar);
      this.Controls.Add(this.panelChildren);
      this.Controls.Add(this.panelName);
      this.Controls.Add(this.panelButtons);
      this.Cursor = System.Windows.Forms.Cursors.Default;
      this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Name = "MainMenuUI";
      this.Text = "RandomShopEditor";
      this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainMenu_MouseDown);
      this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainMenu_MouseMove);
      this.panelButtons.ResumeLayout(false);
      this.panelName.ResumeLayout(false);
      this.panelName.PerformLayout();
      this.panelChildren.ResumeLayout(false);
      this.panelChildren.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel panelButtons;
    private System.Windows.Forms.Button btnDatabase;
    private System.Windows.Forms.Button btnColors;
    private System.Windows.Forms.Button btnEffects;
    private System.Windows.Forms.Button btnPeriods;
    private System.Windows.Forms.Button btnLineups;
    private System.Windows.Forms.Label labelName;
    private System.Windows.Forms.Panel panelNavBar;
    private System.Windows.Forms.Panel panelName;
    private System.Windows.Forms.Panel panelChildren;
    private System.Windows.Forms.Label label2;
    public System.Windows.Forms.Button btnPackages;
    private System.Windows.Forms.Button buttonConnect;
    private System.Windows.Forms.TextBox textBoxPassword;
    private System.Windows.Forms.TextBox textBoxUsername;
    private System.Windows.Forms.TextBox textBoxTable;
    private System.Windows.Forms.TextBox textBoxDatabase;
    private System.Windows.Forms.Label labelPassword;
    private System.Windows.Forms.Label labelUsername;
    private System.Windows.Forms.Label labelTable;
    private System.Windows.Forms.Label labelDatabase;
    private System.Windows.Forms.Label labelRPath;
    private System.Windows.Forms.Label labelResourcePath;
    private TextProgressBar progressBar1;
  }
}

