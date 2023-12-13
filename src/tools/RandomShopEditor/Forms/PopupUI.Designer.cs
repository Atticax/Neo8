
namespace RandomShopEditor.Forms
{
  partial class PopupUI
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.panelTopBar = new System.Windows.Forms.Panel();
      this.buttonClose = new System.Windows.Forms.Button();
      this.panelItem = new System.Windows.Forms.Panel();
      this.labelShopPriceId = new System.Windows.Forms.Label();
      this.buttonAdd = new System.Windows.Forms.Button();
      this.labelGrade = new System.Windows.Forms.Label();
      this.labelProb = new System.Windows.Forms.Label();
      this.labelPeriod = new System.Windows.Forms.Label();
      this.labelColor = new System.Windows.Forms.Label();
      this.labelEffect = new System.Windows.Forms.Label();
      this.numericGrade = new System.Windows.Forms.NumericUpDown();
      this.numericProbability = new System.Windows.Forms.NumericUpDown();
      this.comboBoxPeriod = new System.Windows.Forms.ComboBox();
      this.comboBoxColor = new System.Windows.Forms.ComboBox();
      this.comboBoxEffect = new System.Windows.Forms.ComboBox();
      this.panelTopBar.SuspendLayout();
      this.panelItem.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericGrade)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericProbability)).BeginInit();
      this.SuspendLayout();
      // 
      // panelTopBar
      // 
      this.panelTopBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.panelTopBar.Controls.Add(this.buttonClose);
      this.panelTopBar.Dock = System.Windows.Forms.DockStyle.Top;
      this.panelTopBar.Location = new System.Drawing.Point(0, 0);
      this.panelTopBar.Name = "panelTopBar";
      this.panelTopBar.Size = new System.Drawing.Size(367, 19);
      this.panelTopBar.TabIndex = 11;
      this.panelTopBar.Tag = "NR";
      this.panelTopBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainMenu_MouseDown);
      this.panelTopBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainMenu_MouseMove);
      // 
      // buttonClose
      // 
      this.buttonClose.Dock = System.Windows.Forms.DockStyle.Right;
      this.buttonClose.FlatAppearance.BorderSize = 0;
      this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.buttonClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.buttonClose.Location = new System.Drawing.Point(346, 0);
      this.buttonClose.Name = "buttonClose";
      this.buttonClose.Size = new System.Drawing.Size(21, 19);
      this.buttonClose.TabIndex = 0;
      this.buttonClose.Text = "X";
      this.buttonClose.UseVisualStyleBackColor = true;
      this.buttonClose.MouseClick += new System.Windows.Forms.MouseEventHandler(this.buttonClose_MouseClick);
      // 
      // panelItem
      // 
      this.panelItem.Controls.Add(this.labelShopPriceId);
      this.panelItem.Controls.Add(this.buttonAdd);
      this.panelItem.Controls.Add(this.labelGrade);
      this.panelItem.Controls.Add(this.labelProb);
      this.panelItem.Controls.Add(this.labelPeriod);
      this.panelItem.Controls.Add(this.labelColor);
      this.panelItem.Controls.Add(this.labelEffect);
      this.panelItem.Controls.Add(this.numericGrade);
      this.panelItem.Controls.Add(this.numericProbability);
      this.panelItem.Controls.Add(this.comboBoxPeriod);
      this.panelItem.Controls.Add(this.comboBoxColor);
      this.panelItem.Controls.Add(this.comboBoxEffect);
      this.panelItem.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panelItem.Location = new System.Drawing.Point(0, 19);
      this.panelItem.Name = "panelItem";
      this.panelItem.Size = new System.Drawing.Size(367, 109);
      this.panelItem.TabIndex = 12;
      // 
      // labelShopPriceId
      // 
      this.labelShopPriceId.AutoSize = true;
      this.labelShopPriceId.Location = new System.Drawing.Point(244, 14);
      this.labelShopPriceId.Name = "labelShopPriceId";
      this.labelShopPriceId.Size = new System.Drawing.Size(0, 13);
      this.labelShopPriceId.TabIndex = 34;
      this.labelShopPriceId.Visible = false;
      // 
      // buttonAdd
      // 
      this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonAdd.FlatAppearance.BorderSize = 0;
      this.buttonAdd.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.buttonAdd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.buttonAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.buttonAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.buttonAdd.ForeColor = System.Drawing.Color.White;
      this.buttonAdd.Location = new System.Drawing.Point(225, 72);
      this.buttonAdd.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.buttonAdd.Name = "buttonAdd";
      this.buttonAdd.Size = new System.Drawing.Size(130, 24);
      this.buttonAdd.TabIndex = 33;
      this.buttonAdd.Text = "ADD";
      this.buttonAdd.UseVisualStyleBackColor = true;
      this.buttonAdd.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonAdd_MouseDown);
      // 
      // labelGrade
      // 
      this.labelGrade.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.labelGrade.AutoSize = true;
      this.labelGrade.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelGrade.ForeColor = System.Drawing.Color.White;
      this.labelGrade.Location = new System.Drawing.Point(132, 60);
      this.labelGrade.Name = "labelGrade";
      this.labelGrade.Size = new System.Drawing.Size(53, 13);
      this.labelGrade.TabIndex = 32;
      this.labelGrade.Text = "RARITY";
      // 
      // labelProb
      // 
      this.labelProb.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.labelProb.AutoSize = true;
      this.labelProb.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelProb.ForeColor = System.Drawing.Color.White;
      this.labelProb.Location = new System.Drawing.Point(31, 60);
      this.labelProb.Name = "labelProb";
      this.labelProb.Size = new System.Drawing.Size(41, 13);
      this.labelProb.TabIndex = 31;
      this.labelProb.Text = "PROB";
      // 
      // labelPeriod
      // 
      this.labelPeriod.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.labelPeriod.AutoSize = true;
      this.labelPeriod.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelPeriod.ForeColor = System.Drawing.Color.White;
      this.labelPeriod.Location = new System.Drawing.Point(285, 14);
      this.labelPeriod.Name = "labelPeriod";
      this.labelPeriod.Size = new System.Drawing.Size(54, 13);
      this.labelPeriod.TabIndex = 30;
      this.labelPeriod.Text = "PERIOD";
      // 
      // labelColor
      // 
      this.labelColor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.labelColor.AutoSize = true;
      this.labelColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelColor.ForeColor = System.Drawing.Color.White;
      this.labelColor.Location = new System.Drawing.Point(159, 15);
      this.labelColor.Name = "labelColor";
      this.labelColor.Size = new System.Drawing.Size(49, 13);
      this.labelColor.TabIndex = 29;
      this.labelColor.Text = "COLOR";
      // 
      // labelEffect
      // 
      this.labelEffect.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.labelEffect.AutoSize = true;
      this.labelEffect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelEffect.ForeColor = System.Drawing.Color.White;
      this.labelEffect.Location = new System.Drawing.Point(31, 15);
      this.labelEffect.Name = "labelEffect";
      this.labelEffect.Size = new System.Drawing.Size(53, 13);
      this.labelEffect.TabIndex = 28;
      this.labelEffect.Text = "EFFECT";
      // 
      // numericGrade
      // 
      this.numericGrade.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.numericGrade.Location = new System.Drawing.Point(135, 76);
      this.numericGrade.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
      this.numericGrade.Name = "numericGrade";
      this.numericGrade.Size = new System.Drawing.Size(47, 20);
      this.numericGrade.TabIndex = 27;
      this.numericGrade.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
      // 
      // numericProbability
      // 
      this.numericProbability.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.numericProbability.Location = new System.Drawing.Point(31, 76);
      this.numericProbability.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
      this.numericProbability.Name = "numericProbability";
      this.numericProbability.Size = new System.Drawing.Size(47, 20);
      this.numericProbability.TabIndex = 26;
      this.numericProbability.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
      // 
      // comboBoxPeriod
      // 
      this.comboBoxPeriod.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.comboBoxPeriod.FormattingEnabled = true;
      this.comboBoxPeriod.Location = new System.Drawing.Point(263, 30);
      this.comboBoxPeriod.Name = "comboBoxPeriod";
      this.comboBoxPeriod.Size = new System.Drawing.Size(92, 21);
      this.comboBoxPeriod.TabIndex = 25;
      // 
      // comboBoxColor
      // 
      this.comboBoxColor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.comboBoxColor.FormattingEnabled = true;
      this.comboBoxColor.Location = new System.Drawing.Point(142, 30);
      this.comboBoxColor.Name = "comboBoxColor";
      this.comboBoxColor.Size = new System.Drawing.Size(92, 21);
      this.comboBoxColor.TabIndex = 24;
      // 
      // comboBoxEffect
      // 
      this.comboBoxEffect.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.comboBoxEffect.FormattingEnabled = true;
      this.comboBoxEffect.Location = new System.Drawing.Point(12, 30);
      this.comboBoxEffect.Name = "comboBoxEffect";
      this.comboBoxEffect.Size = new System.Drawing.Size(92, 21);
      this.comboBoxEffect.TabIndex = 23;
      // 
      // PopupUI
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoScroll = true;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.ClientSize = new System.Drawing.Size(367, 128);
      this.Controls.Add(this.panelItem);
      this.Controls.Add(this.panelTopBar);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.Name = "PopupUI";
      this.Text = "LineupUI";
      this.panelTopBar.ResumeLayout(false);
      this.panelItem.ResumeLayout(false);
      this.panelItem.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericGrade)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericProbability)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.Panel panelTopBar;
    private System.Windows.Forms.Button buttonClose;
    private System.Windows.Forms.Panel panelItem;
    private System.Windows.Forms.Button buttonAdd;
    private System.Windows.Forms.Label labelGrade;
    private System.Windows.Forms.Label labelProb;
    private System.Windows.Forms.Label labelPeriod;
    private System.Windows.Forms.Label labelColor;
    private System.Windows.Forms.Label labelEffect;
    private System.Windows.Forms.NumericUpDown numericGrade;
    private System.Windows.Forms.NumericUpDown numericProbability;
    private System.Windows.Forms.ComboBox comboBoxPeriod;
    private System.Windows.Forms.ComboBox comboBoxColor;
    private System.Windows.Forms.ComboBox comboBoxEffect;
    private System.Windows.Forms.Label labelShopPriceId;
  }
}