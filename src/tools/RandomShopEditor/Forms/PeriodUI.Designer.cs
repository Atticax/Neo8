
namespace RandomShopEditor.Forms
{
  partial class PeriodUI
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
      this.buttonAdd = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // buttonAdd
      // 
      this.buttonAdd.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.buttonAdd.FlatAppearance.BorderSize = 0;
      this.buttonAdd.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.buttonAdd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.buttonAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.buttonAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.buttonAdd.ForeColor = System.Drawing.Color.White;
      this.buttonAdd.Location = new System.Drawing.Point(0, 281);
      this.buttonAdd.Name = "buttonAdd";
      this.buttonAdd.Size = new System.Drawing.Size(467, 36);
      this.buttonAdd.TabIndex = 14;
      this.buttonAdd.Text = "ADD";
      this.buttonAdd.UseVisualStyleBackColor = true;
      this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
      // 
      // PeriodUI
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoScroll = true;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.ClientSize = new System.Drawing.Size(467, 317);
      this.Controls.Add(this.buttonAdd);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Name = "PeriodUI";
      this.Text = "PeriodUI";
      this.Load += new System.EventHandler(this.PeriodUI_Load);
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.Button buttonAdd;
  }
}