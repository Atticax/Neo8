
namespace RandomShopEditor.Forms
{
  partial class PackageUI
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
      this.button1 = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.button1.Location = new System.Drawing.Point(0, 255);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(451, 23);
      this.button1.TabIndex = 0;
      this.button1.Text = "ADD PACKAGES";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.addPackage_Click);
      // 
      // PackageUI
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoScroll = true;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
      this.ClientSize = new System.Drawing.Size(451, 278);
      this.Controls.Add(this.button1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Name = "PackageUI";
      this.Text = "PackageUI";
      this.Load += new System.EventHandler(this.PackageUI_Load);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button button1;
  }
}