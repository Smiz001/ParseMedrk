namespace ParseMedrk
{
  partial class MainForm
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
      this.btChooseSaveFile = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // btChooseSaveFile
      // 
      this.btChooseSaveFile.Location = new System.Drawing.Point(12, 51);
      this.btChooseSaveFile.Name = "btChooseSaveFile";
      this.btChooseSaveFile.Size = new System.Drawing.Size(121, 23);
      this.btChooseSaveFile.TabIndex = 0;
      this.btChooseSaveFile.Text = "Выберите путь";
      this.btChooseSaveFile.UseVisualStyleBackColor = true;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.btChooseSaveFile);
      this.Name = "MainForm";
      this.Text = "Парсер Медремкомплект";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btChooseSaveFile;
  }
}

