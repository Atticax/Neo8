
namespace RandomShopEditor.Forms
{
  partial class ShowPriceUI
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
      this.panelPeriod = new System.Windows.Forms.Panel();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.comboBoxPriceType = new System.Windows.Forms.ComboBox();
      this.comboBoxName = new System.Windows.Forms.ComboBox();
      this.buttonAdd = new System.Windows.Forms.Button();
      this.panelTopBar.SuspendLayout();
      this.panelPeriod.SuspendLayout();
      this.SuspendLayout();
      // 
      // panelTopBar
      // 
      this.panelTopBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.panelTopBar.Controls.Add(this.buttonClose);
      this.panelTopBar.Dock = System.Windows.Forms.DockStyle.Top;
      this.panelTopBar.Location = new System.Drawing.Point(0, 0);
      this.panelTopBar.Name = "panelTopBar";
      this.panelTopBar.Size = new System.Drawing.Size(456, 20);
      this.panelTopBar.TabIndex = 3;
      this.panelTopBar.Tag = "NR";
      this.panelTopBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainMenu_MouseDown);
      this.panelTopBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainMenu_MouseMove);
      // 
      // buttonClose
      // 
      this.buttonClose.Dock = System.Windows.Forms.DockStyle.Right;
      this.buttonClose.FlatAppearance.BorderSize = 0;
      this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.buttonClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.buttonClose.Location = new System.Drawing.Point(435, 0);
      this.buttonClose.Name = "buttonClose";
      this.buttonClose.Size = new System.Drawing.Size(21, 20);
      this.buttonClose.TabIndex = 0;
      this.buttonClose.Text = "X";
      this.buttonClose.UseVisualStyleBackColor = true;
      this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
      // 
      // panelPeriod
      // 
      this.panelPeriod.Controls.Add(this.buttonAdd);
      this.panelPeriod.Controls.Add(this.comboBoxName);
      this.panelPeriod.Controls.Add(this.comboBoxPriceType);
      this.panelPeriod.Controls.Add(this.label2);
      this.panelPeriod.Controls.Add(this.label1);
      this.panelPeriod.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panelPeriod.Location = new System.Drawing.Point(0, 20);
      this.panelPeriod.Name = "panelPeriod";
      this.panelPeriod.Size = new System.Drawing.Size(456, 133);
      this.panelPeriod.TabIndex = 4;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.ForeColor = System.Drawing.Color.White;
      this.label1.Location = new System.Drawing.Point(44, 20);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(97, 16);
      this.label1.TabIndex = 2;
      this.label1.Text = "PRICE TYPE";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.ForeColor = System.Drawing.Color.White;
      this.label2.Location = new System.Drawing.Point(340, 20);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(57, 16);
      this.label2.TabIndex = 3;
      this.label2.Text = "VALUE";
      // 
      // comboBoxPriceType
      // 
      this.comboBoxPriceType.FormattingEnabled = true;
      this.comboBoxPriceType.Location = new System.Drawing.Point(24, 39);
      this.comboBoxPriceType.Name = "comboBoxPriceType";
      this.comboBoxPriceType.Size = new System.Drawing.Size(129, 21);
      this.comboBoxPriceType.TabIndex = 4;
      this.comboBoxPriceType.SelectedValueChanged += new System.EventHandler(this.comboBoxPriceType_SelectedValueChanged);
      // 
      // comboBoxName
      // 
      this.comboBoxName.FormattingEnabled = true;
      this.comboBoxName.Location = new System.Drawing.Point(305, 39);
      this.comboBoxName.Name = "comboBoxName";
      this.comboBoxName.Size = new System.Drawing.Size(121, 21);
      this.comboBoxName.TabIndex = 5;
      // 
      // buttonAdd
      // 
      this.buttonAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
      this.buttonAdd.FlatAppearance.BorderSize = 0;
      this.buttonAdd.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.buttonAdd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.buttonAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.buttonAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.buttonAdd.ForeColor = System.Drawing.Color.White;
      this.buttonAdd.Location = new System.Drawing.Point(156, 86);
      this.buttonAdd.Name = "buttonAdd";
      this.buttonAdd.Size = new System.Drawing.Size(144, 35);
      this.buttonAdd.TabIndex = 6;
      this.buttonAdd.Text = "ADD";
      this.buttonAdd.UseVisualStyleBackColor = false;
      this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
      // 
      // ShowPriceUI
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoScroll = true;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.ClientSize = new System.Drawing.Size(456, 153);
      this.Controls.Add(this.panelPeriod);
      this.Controls.Add(this.panelTopBar);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
      this.Name = "ShowPriceUI";
      this.Text = "LineupUI";
      this.panelTopBar.ResumeLayout(false);
      this.panelPeriod.ResumeLayout(false);
      this.panelPeriod.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.Panel panelTopBar;
    private System.Windows.Forms.Button buttonClose;
    private System.Windows.Forms.Panel panelPeriod;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox comboBoxPriceType;
    private System.Windows.Forms.ComboBox comboBoxName;
    private System.Windows.Forms.Button buttonAdd;
  }
}