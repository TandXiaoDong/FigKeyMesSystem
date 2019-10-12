namespace MesManager.UI
{
    partial class ProductMaterial
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProductMaterial));
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn7 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewComboBoxColumn gridViewComboBoxColumn2 = new Telerik.WinControls.UI.GridViewComboBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn8 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn9 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn10 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn11 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn12 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.TableViewDefinition tableViewDefinition3 = new Telerik.WinControls.UI.TableViewDefinition();
            Telerik.WinControls.UI.TableViewDefinition tableViewDefinition4 = new Telerik.WinControls.UI.TableViewDefinition();
            this.radMenu1 = new Telerik.WinControls.UI.RadMenu();
            this.menu_add_row = new Telerik.WinControls.UI.RadMenuItem();
            this.menu_delete = new Telerik.WinControls.UI.RadMenuItem();
            this.menu_update = new Telerik.WinControls.UI.RadMenuItem();
            this.menu_grid = new Telerik.WinControls.UI.RadMenuItem();
            this.menu_clear_db = new Telerik.WinControls.UI.RadMenuItem();
            this.menu_refresh = new Telerik.WinControls.UI.RadMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tool_material_binding = new System.Windows.Forms.ToolStripButton();
            this.tool_material_stock = new System.Windows.Forms.ToolStripButton();
            this.radGridView1 = new Telerik.WinControls.UI.RadGridView();
            this.radGridView2 = new Telerik.WinControls.UI.RadGridView();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tool_queryFilter = new System.Windows.Forms.ToolStripComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.radMenu1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView2.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radMenu1
            // 
            this.radMenu1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.radMenu1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radMenu1.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.menu_add_row,
            this.menu_delete,
            this.menu_update,
            this.menu_grid,
            this.menu_clear_db,
            this.menu_refresh});
            this.radMenu1.Location = new System.Drawing.Point(0, 0);
            this.radMenu1.Name = "radMenu1";
            this.radMenu1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.radMenu1.Size = new System.Drawing.Size(1272, 36);
            this.radMenu1.TabIndex = 12;
            // 
            // menu_add_row
            // 
            this.menu_add_row.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.menu_add_row.Image = global::MesManager.Properties.Resources.bullet_add;
            this.menu_add_row.Name = "menu_add_row";
            this.menu_add_row.Text = "新增";
            // 
            // menu_delete
            // 
            this.menu_delete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.menu_delete.Image = ((System.Drawing.Image)(resources.GetObject("menu_delete.Image")));
            this.menu_delete.Name = "menu_delete";
            this.menu_delete.Shape = null;
            this.menu_delete.Text = "解绑";
            this.menu_delete.UseCompatibleTextRendering = false;
            // 
            // menu_update
            // 
            this.menu_update.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.menu_update.Image = global::MesManager.Properties.Resources.update;
            this.menu_update.Name = "menu_update";
            this.menu_update.Text = "更新";
            this.menu_update.UseCompatibleTextRendering = false;
            // 
            // menu_grid
            // 
            this.menu_grid.ForeColor = System.Drawing.Color.White;
            this.menu_grid.Image = global::MesManager.Properties.Resources.ClearGrid;
            this.menu_grid.Name = "menu_grid";
            this.menu_grid.Text = "清空显示";
            this.menu_grid.UseCompatibleTextRendering = false;
            // 
            // menu_clear_db
            // 
            this.menu_clear_db.ForeColor = System.Drawing.Color.White;
            this.menu_clear_db.Image = global::MesManager.Properties.Resources.DeleteDataSource_16x16;
            this.menu_clear_db.Name = "menu_clear_db";
            this.menu_clear_db.Text = "清空数据";
            this.menu_clear_db.UseCompatibleTextRendering = false;
            // 
            // menu_refresh
            // 
            this.menu_refresh.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.menu_refresh.Image = global::MesManager.Properties.Resources.update;
            this.menu_refresh.Name = "menu_refresh";
            this.menu_refresh.Text = "刷新";
            this.menu_refresh.UseCompatibleTextRendering = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.tool_queryFilter,
            this.tool_material_stock,
            this.tool_material_binding});
            this.toolStrip1.Location = new System.Drawing.Point(0, 36);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1272, 28);
            this.toolStrip1.TabIndex = 14;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tool_material_binding
            // 
            this.tool_material_binding.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tool_material_binding.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tool_material_binding.ForeColor = System.Drawing.Color.White;
            this.tool_material_binding.Image = ((System.Drawing.Image)(resources.GetObject("tool_material_binding.Image")));
            this.tool_material_binding.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_material_binding.Name = "tool_material_binding";
            this.tool_material_binding.Size = new System.Drawing.Size(78, 25);
            this.tool_material_binding.Text = "物料绑定";
            // 
            // tool_material_stock
            // 
            this.tool_material_stock.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tool_material_stock.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tool_material_stock.ForeColor = System.Drawing.Color.White;
            this.tool_material_stock.Image = ((System.Drawing.Image)(resources.GetObject("tool_material_stock.Image")));
            this.tool_material_stock.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_material_stock.Name = "tool_material_stock";
            this.tool_material_stock.Size = new System.Drawing.Size(78, 25);
            this.tool_material_stock.Text = "库存管理";
            // 
            // radGridView1
            // 
            this.radGridView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(191)))), ((int)(((byte)(255)))));
            this.radGridView1.Cursor = System.Windows.Forms.Cursors.Default;
            this.radGridView1.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.radGridView1.ForeColor = System.Drawing.Color.Black;
            this.radGridView1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radGridView1.Location = new System.Drawing.Point(0, 117);
            // 
            // 
            // 
            gridViewTextBoxColumn7.EnableExpressionEditor = false;
            gridViewTextBoxColumn7.HeaderText = "序号";
            gridViewTextBoxColumn7.Name = "rdvc_order";
            gridViewComboBoxColumn2.EnableExpressionEditor = false;
            gridViewComboBoxColumn2.HeaderText = "产品型号";
            gridViewComboBoxColumn2.Name = "rdvc_typeNo";
            gridViewTextBoxColumn8.EnableExpressionEditor = false;
            gridViewTextBoxColumn8.HeaderText = "物料号";
            gridViewTextBoxColumn8.Name = "rdvc_materialPN";
            gridViewTextBoxColumn9.EnableExpressionEditor = false;
            gridViewTextBoxColumn9.HeaderText = "物料名称";
            gridViewTextBoxColumn9.Name = "dbv_material_name";
            gridViewTextBoxColumn10.EnableExpressionEditor = false;
            gridViewTextBoxColumn10.HeaderText = "描述";
            gridViewTextBoxColumn10.Name = "rdvc_describle";
            gridViewTextBoxColumn11.EnableExpressionEditor = false;
            gridViewTextBoxColumn11.HeaderText = "操作用户";
            gridViewTextBoxColumn11.Name = "rdbc_user";
            gridViewTextBoxColumn12.EnableExpressionEditor = false;
            gridViewTextBoxColumn12.HeaderText = "更新日期";
            gridViewTextBoxColumn12.Name = "rdbc_date";
            this.radGridView1.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewTextBoxColumn7,
            gridViewComboBoxColumn2,
            gridViewTextBoxColumn8,
            gridViewTextBoxColumn9,
            gridViewTextBoxColumn10,
            gridViewTextBoxColumn11,
            gridViewTextBoxColumn12});
            this.radGridView1.MasterTemplate.ViewDefinition = tableViewDefinition3;
            this.radGridView1.Name = "radGridView1";
            this.radGridView1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.radGridView1.Size = new System.Drawing.Size(639, 572);
            this.radGridView1.TabIndex = 15;
            this.radGridView1.ThemeName = "Breeze";
            // 
            // radGridView2
            // 
            this.radGridView2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radGridView2.Location = new System.Drawing.Point(657, 117);
            // 
            // 
            // 
            this.radGridView2.MasterTemplate.ViewDefinition = tableViewDefinition4;
            this.radGridView2.Name = "radGridView2";
            this.radGridView2.Size = new System.Drawing.Size(587, 487);
            this.radGridView2.TabIndex = 16;
            this.radGridView2.ThemeName = "Breeze";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStripLabel1.ForeColor = System.Drawing.Color.White;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(99, 25);
            this.toolStripLabel1.Text = "物料编码(RID)";
            // 
            // tool_queryFilter
            // 
            this.tool_queryFilter.Name = "tool_queryFilter";
            this.tool_queryFilter.Size = new System.Drawing.Size(121, 28);
            // 
            // ProductMaterial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(1272, 722);
            this.Controls.Add(this.radGridView2);
            this.Controls.Add(this.radGridView1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.radMenu1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProductMaterial";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "物料管理";
            this.Load += new System.EventHandler(this.ProductMaterial_Load);
            ((System.ComponentModel.ISupportInitialize)(this.radMenu1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView2.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Telerik.WinControls.UI.RadMenu radMenu1;
        private Telerik.WinControls.UI.RadMenuItem menu_delete;
        private Telerik.WinControls.UI.RadMenuItem menu_update;
        private Telerik.WinControls.UI.RadMenuItem menu_refresh;
        private Telerik.WinControls.UI.RadMenuItem menu_grid;
        private Telerik.WinControls.UI.RadMenuItem menu_clear_db;
        private Telerik.WinControls.UI.RadMenuItem menu_add_row;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tool_material_binding;
        private System.Windows.Forms.ToolStripButton tool_material_stock;
        private Telerik.WinControls.UI.RadGridView radGridView1;
        private Telerik.WinControls.UI.RadGridView radGridView2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox tool_queryFilter;
    }
}
