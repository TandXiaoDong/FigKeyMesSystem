namespace MesManager.UI
{
    partial class ProductPackageDetail
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.tb_package = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel6 = new System.Windows.Forms.ToolStripLabel();
            this.tool_package_exportFilter = new System.Windows.Forms.ToolStripComboBox();
            this.tool_package_export = new System.Windows.Forms.ToolStripButton();
            this.btn_selectOfPackage = new System.Windows.Forms.ToolStripButton();
            this.radGridViewPackage = new Telerik.WinControls.UI.RadGridView();
            this.panel1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridViewPackage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridViewPackage.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.toolStrip2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1033, 42);
            this.panel1.TabIndex = 34;
            // 
            // toolStrip2
            // 
            this.toolStrip2.BackColor = System.Drawing.Color.SkyBlue;
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip2.Font = new System.Drawing.Font("Microsoft YaHei UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel2,
            this.tb_package,
            this.toolStripLabel6,
            this.tool_package_exportFilter,
            this.tool_package_export,
            this.btn_selectOfPackage});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(1033, 42);
            this.toolStrip2.TabIndex = 32;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.ForeColor = System.Drawing.Color.White;
            this.toolStripLabel2.LinkColor = System.Drawing.Color.White;
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(61, 39);
            this.toolStripLabel2.Text = "追溯码：";
            // 
            // tb_package
            // 
            this.tb_package.ForeColor = System.Drawing.Color.Black;
            this.tb_package.Name = "tb_package";
            this.tb_package.Size = new System.Drawing.Size(121, 42);
            // 
            // toolStripLabel6
            // 
            this.toolStripLabel6.ForeColor = System.Drawing.Color.White;
            this.toolStripLabel6.Name = "toolStripLabel6";
            this.toolStripLabel6.Size = new System.Drawing.Size(74, 39);
            this.toolStripLabel6.Text = "导出格式：";
            // 
            // tool_package_exportFilter
            // 
            this.tool_package_exportFilter.ForeColor = System.Drawing.Color.Black;
            this.tool_package_exportFilter.Name = "tool_package_exportFilter";
            this.tool_package_exportFilter.Size = new System.Drawing.Size(121, 42);
            // 
            // tool_package_export
            // 
            this.tool_package_export.ForeColor = System.Drawing.Color.White;
            this.tool_package_export.Image = global::MesManager.Properties.Resources.Export_16x16;
            this.tool_package_export.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_package_export.Name = "tool_package_export";
            this.tool_package_export.Size = new System.Drawing.Size(55, 39);
            this.tool_package_export.Text = "导出";
            this.tool_package_export.ToolTipText = "导出";
            this.tool_package_export.Click += new System.EventHandler(this.Tool_package_export_Click);
            // 
            // btn_selectOfPackage
            // 
            this.btn_selectOfPackage.ForeColor = System.Drawing.Color.White;
            this.btn_selectOfPackage.Image = global::MesManager.Properties.Resources.Refresh_16x16;
            this.btn_selectOfPackage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_selectOfPackage.Name = "btn_selectOfPackage";
            this.btn_selectOfPackage.Size = new System.Drawing.Size(55, 39);
            this.btn_selectOfPackage.Text = "查询";
            this.btn_selectOfPackage.Click += new System.EventHandler(this.Btn_selectOfPackage_Click);
            // 
            // radGridViewPackage
            // 
            this.radGridViewPackage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radGridViewPackage.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radGridViewPackage.Location = new System.Drawing.Point(0, 42);
            // 
            // 
            // 
            this.radGridViewPackage.MasterTemplate.ViewDefinition = tableViewDefinition1;
            this.radGridViewPackage.Name = "radGridViewPackage";
            this.radGridViewPackage.Size = new System.Drawing.Size(1033, 454);
            this.radGridViewPackage.TabIndex = 39;
            this.radGridViewPackage.ThemeName = "Breeze";
            // 
            // ProductPackageDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(1033, 496);
            this.Controls.Add(this.radGridViewPackage);
            this.Controls.Add(this.panel1);
            this.Name = "ProductPackageDetail";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "包装箱产品";
            this.Load += new System.EventHandler(this.ProductPackageDetail_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridViewPackage.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridViewPackage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox tb_package;
        private System.Windows.Forms.ToolStripLabel toolStripLabel6;
        private System.Windows.Forms.ToolStripComboBox tool_package_exportFilter;
        private System.Windows.Forms.ToolStripButton tool_package_export;
        private System.Windows.Forms.ToolStripButton btn_selectOfPackage;
        private Telerik.WinControls.UI.RadGridView radGridViewPackage;
    }
}
