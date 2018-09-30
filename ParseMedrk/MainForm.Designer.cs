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
      this.components = new System.ComponentModel.Container();
      this.btDownloadData = new System.Windows.Forms.Button();
      this.dgvMainInfo = new System.Windows.Forms.DataGridView();
      this.cmsForDg = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.btExport = new System.Windows.Forms.Button();
      this.sfdExport = new System.Windows.Forms.SaveFileDialog();
      this.показатьОписаниеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      ((System.ComponentModel.ISupportInitialize)(this.dgvMainInfo)).BeginInit();
      this.cmsForDg.SuspendLayout();
      this.SuspendLayout();
      // 
      // btDownloadData
      // 
      this.btDownloadData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btDownloadData.Location = new System.Drawing.Point(851, 12);
      this.btDownloadData.Name = "btDownloadData";
      this.btDownloadData.Size = new System.Drawing.Size(121, 23);
      this.btDownloadData.TabIndex = 0;
      this.btDownloadData.Text = "Загрузить данные";
      this.btDownloadData.UseVisualStyleBackColor = true;
      this.btDownloadData.Click += new System.EventHandler(this.btDownloadData_Click);
      // 
      // dgvMainInfo
      // 
      this.dgvMainInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.dgvMainInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgvMainInfo.ContextMenuStrip = this.cmsForDg;
      this.dgvMainInfo.Location = new System.Drawing.Point(12, 12);
      this.dgvMainInfo.Name = "dgvMainInfo";
      this.dgvMainInfo.ReadOnly = true;
      this.dgvMainInfo.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
      this.dgvMainInfo.Size = new System.Drawing.Size(833, 437);
      this.dgvMainInfo.TabIndex = 1;
      this.dgvMainInfo.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMainInfo_CellClick);
      this.dgvMainInfo.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvMainInfo_CellMouseDown);
      // 
      // cmsForDg
      // 
      this.cmsForDg.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.показатьОписаниеToolStripMenuItem});
      this.cmsForDg.Name = "contextMenuStrip1";
      this.cmsForDg.Size = new System.Drawing.Size(181, 26);
      // 
      // btExport
      // 
      this.btExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btExport.Location = new System.Drawing.Point(851, 41);
      this.btExport.Name = "btExport";
      this.btExport.Size = new System.Drawing.Size(121, 23);
      this.btExport.TabIndex = 2;
      this.btExport.Text = "Экспортировать";
      this.btExport.UseVisualStyleBackColor = true;
      this.btExport.Click += new System.EventHandler(this.btExport_Click);
      // 
      // показатьОписаниеToolStripMenuItem
      // 
      this.показатьОписаниеToolStripMenuItem.Name = "показатьОписаниеToolStripMenuItem";
      this.показатьОписаниеToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
      this.показатьОписаниеToolStripMenuItem.Text = "Показать описание";
      this.показатьОписаниеToolStripMenuItem.Click += new System.EventHandler(this.показатьОписаниеToolStripMenuItem_Click);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(984, 461);
      this.Controls.Add(this.btExport);
      this.Controls.Add(this.dgvMainInfo);
      this.Controls.Add(this.btDownloadData);
      this.Name = "MainForm";
      this.Text = "Парсер Медремкомплект";
      this.Load += new System.EventHandler(this.MainForm_Load);
      ((System.ComponentModel.ISupportInitialize)(this.dgvMainInfo)).EndInit();
      this.cmsForDg.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btDownloadData;
    private System.Windows.Forms.DataGridView dgvMainInfo;
    private System.Windows.Forms.ContextMenuStrip cmsForDg;
    private System.Windows.Forms.Button btExport;
    private System.Windows.Forms.SaveFileDialog sfdExport;
    private System.Windows.Forms.ToolStripMenuItem показатьОписаниеToolStripMenuItem;
  }
}

