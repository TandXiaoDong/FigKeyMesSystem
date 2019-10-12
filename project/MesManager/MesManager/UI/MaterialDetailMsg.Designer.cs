namespace MesManager.UI
{
    partial class MaterialDetailMsg
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
            Telerik.WinControls.UI.TableViewDefinition tableViewDefinition1 = new Telerik.WinControls.UI.TableViewDefinition();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MaterialDetailMsg));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.tool_exportFilter = new System.Windows.Forms.ToolStripComboBox();
            this.tool_sn_export = new System.Windows.Forms.ToolStripButton();
            this.radGridView1 = new Telerik.WinControls.UI.RadGridView();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.SkyBlue;
            this.toolStrip1.Font = new System.Drawing.Font("Microsoft YaHei UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel5,
            this.tool_exportFilter,
            this.tool_sn_export});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1272, 27);
            this.toolStrip1.TabIndex = 32;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.ForeColor = System.Drawing.Color.White;
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(84, 24);
            this.toolStripLabel5.Text = "导出格式：";
            // 
            // tool_exportFilter
            // 
            this.tool_exportFilter.ForeColor = System.Drawing.Color.Black;
            this.tool_exportFilter.Name = "tool_exportFilter";
            this.tool_exportFilter.Size = new System.Drawing.Size(121, 27);
            // 
            // tool_sn_export
            // 
            this.tool_sn_export.ForeColor = System.Drawing.Color.White;
            this.tool_sn_export.Image = global::MesManager.Properties.Resources.Export_16x16;
            this.tool_sn_export.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_sn_export.Name = "tool_sn_export";
            this.tool_sn_export.Size = new System.Drawing.Size(59, 24);
            this.tool_sn_export.Text = "导出";
            this.tool_sn_export.ToolTipText = "导出";
            this.tool_sn_export.Click += new System.EventHandler(this.Tool_sn_export_Click);
            // 
            // radGridView1
            // 
            this.radGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radGridView1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radGridView1.Location = new System.Drawing.Point(0, 27);
            // 
            // 
            // 
            this.radGridView1.MasterTemplate.ViewDefinition = tableViewDefinition1;
            this.radGridView1.Name = "radGridView1";
            this.radGridView1.Size = new System.Drawing.Size(1272, 478);
            this.radGridView1.TabIndex = 33;
            this.radGridView1.ThemeName = "Breeze";
            // 
            // MaterialDetailMsg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(1272, 505);
            this.Controls.Add(this.radGridView1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MaterialDetailMsg";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "物料使用详情";
            this.Load += new System.EventHandler(this.MaterialDetailMsg_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel5;
        private System.Windows.Forms.ToolStripComboBox tool_exportFilter;
        private System.Windows.Forms.ToolStripButton tool_sn_export;
        private Telerik.WinControls.UI.RadGridView radGridView1;
    }
}
