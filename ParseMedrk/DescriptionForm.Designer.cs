namespace ParseMedrk
{
  partial class DescriptionForm
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
      this.tbDescription = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // tbDescription
      // 
      this.tbDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tbDescription.Location = new System.Drawing.Point(12, 12);
      this.tbDescription.Multiline = true;
      this.tbDescription.Name = "tbDescription";
      this.tbDescription.ReadOnly = true;
      this.tbDescription.Size = new System.Drawing.Size(776, 426);
      this.tbDescription.TabIndex = 0;
      // 
      // DescriptionForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.tbDescription);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "DescriptionForm";
      this.Text = "Описание";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox tbDescription;
  }
}