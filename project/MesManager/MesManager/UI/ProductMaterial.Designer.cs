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
            Telerik.WinControls.UI.TableViewDefinition tableViewDefinition1 = new Telerik.WinControls.UI.TableViewDefinition();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn1 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewComboBoxColumn gridViewComboBoxColumn1 = new Telerik.WinControls.UI.GridViewComboBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn2 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn3 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn4 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn5 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn6 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.TableViewDefinition tableViewDefinition2 = new Telerik.WinControls.UI.TableViewDefinition();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProductMaterial));
            this.radDock1 = new Telerik.WinControls.UI.Docking.RadDock();
            this.dw_stockManager = new Telerik.WinControls.UI.Docking.DocumentWindow();
            this.radGridViewStock = new Telerik.WinControls.UI.RadGridView();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.tool_stockManager_add = new System.Windows.Forms.ToolStripButton();
            this.tool_stockManager_deleteSignal = new System.Windows.Forms.ToolStripButton();
            this.tool_stockManager_update = new System.Windows.Forms.ToolStripButton();
            this.tool_stockManager_ClearGrid = new System.Windows.Forms.ToolStripButton();
            this.tool_stockManager_ClearDB = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.tool_stock_queryCondition = new System.Windows.Forms.ToolStripComboBox();
            this.tool_stockManager_query = new System.Windows.Forms.ToolStripButton();
            this.documentContainer1 = new Telerik.WinControls.UI.Docking.DocumentContainer();
            this.documentTabStrip1 = new Telerik.WinControls.UI.Docking.DocumentTabStrip();
            this.dw_materialBind = new Telerik.WinControls.UI.Docking.DocumentWindow();
            this.radGridViewBind = new Telerik.WinControls.UI.RadGridView();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.tool_bind_add = new System.Windows.Forms.ToolStripButton();
            this.tool_bind_delete = new System.Windows.Forms.ToolStripButton();
            this.tool_bind_update = new System.Windows.Forms.ToolStripButton();
            this.tool_bind_cleargrid = new System.Windows.Forms.ToolStripButton();
            this.tool_bind_cleardb = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.tool_bind_queryCondition = new System.Windows.Forms.ToolStripComboBox();
            this.tool_bind_query = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.radDock1)).BeginInit();
            this.radDock1.SuspendLayout();
            this.dw_stockManager.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridViewStock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridViewStock.MasterTemplate)).BeginInit();
            this.toolStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.documentContainer1)).BeginInit();
            this.documentContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.documentTabStrip1)).BeginInit();
            this.documentTabStrip1.SuspendLayout();
            this.dw_materialBind.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridViewBind)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridViewBind.MasterTemplate)).BeginInit();
            this.toolStrip3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radDock1
            // 
            this.radDock1.ActiveWindow = this.dw_stockManager;
            this.radDock1.Controls.Add(this.documentContainer1);
            this.radDock1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radDock1.IsCleanUpTarget = true;
            this.radDock1.Location = new System.Drawing.Point(0, 0);
            this.radDock1.MainDocumentContainer = this.documentContainer1;
            this.radDock1.Name = "radDock1";
            this.radDock1.Padding = new System.Windows.Forms.Padding(0);
            // 
            // 
            // 
            this.radDock1.RootElement.MinSize = new System.Drawing.Size(25, 25);
            this.radDock1.Size = new System.Drawing.Size(1272, 722);
            this.radDock1.TabIndex = 17;
            this.radDock1.TabStop = false;
            this.radDock1.ThemeName = "Breeze";
            // 
            // dw_stockManager
            // 
            this.dw_stockManager.Controls.Add(this.radGridViewStock);
            this.dw_stockManager.Controls.Add(this.toolStrip2);
            this.dw_stockManager.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dw_stockManager.Location = new System.Drawing.Point(5, 27);
            this.dw_stockManager.Name = "dw_stockManager";
            this.dw_stockManager.PreviousDockState = Telerik.WinControls.UI.Docking.DockState.TabbedDocument;
            this.dw_stockManager.Size = new System.Drawing.Size(1262, 690);
            this.dw_stockManager.Text = "库存管理";
            // 
            // radGridViewStock
            // 
            this.radGridViewStock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radGridViewStock.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radGridViewStock.Location = new System.Drawing.Point(0, 29);
            // 
            // 
            // 
            this.radGridViewStock.MasterTemplate.ViewDefinition = tableViewDefinition1;
            this.radGridViewStock.Name = "radGridViewStock";
            this.radGridViewStock.Size = new System.Drawing.Size(1262, 661);
            this.radGridViewStock.TabIndex = 19;
            this.radGridViewStock.ThemeName = "Breeze";
            // 
            // toolStrip2
            // 
            this.toolStrip2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tool_stockManager_add,
            this.tool_stockManager_deleteSignal,
            this.tool_stockManager_update,
            this.tool_stockManager_ClearGrid,
            this.tool_stockManager_ClearDB,
            this.toolStripLabel2,
            this.tool_stock_queryCondition,
            this.tool_stockManager_query});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(1262, 29);
            this.toolStrip2.TabIndex = 17;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // tool_stockManager_add
            // 
            this.tool_stockManager_add.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tool_stockManager_add.Image = global::MesManager.Properties.Resources.add;
            this.tool_stockManager_add.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_stockManager_add.Name = "tool_stockManager_add";
            this.tool_stockManager_add.Size = new System.Drawing.Size(62, 26);
            this.tool_stockManager_add.Text = "新增";
            // 
            // tool_stockManager_deleteSignal
            // 
            this.tool_stockManager_deleteSignal.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tool_stockManager_deleteSignal.Image = global::MesManager.Properties.Resources.delete;
            this.tool_stockManager_deleteSignal.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_stockManager_deleteSignal.Name = "tool_stockManager_deleteSignal";
            this.tool_stockManager_deleteSignal.Size = new System.Drawing.Size(62, 26);
            this.tool_stockManager_deleteSignal.Text = "删除";
            // 
            // tool_stockManager_update
            // 
            this.tool_stockManager_update.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tool_stockManager_update.Image = global::MesManager.Properties.Resources.Refresh_16x16;
            this.tool_stockManager_update.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_stockManager_update.Name = "tool_stockManager_update";
            this.tool_stockManager_update.Size = new System.Drawing.Size(62, 26);
            this.tool_stockManager_update.Text = "更新";
            // 
            // tool_stockManager_ClearGrid
            // 
            this.tool_stockManager_ClearGrid.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tool_stockManager_ClearGrid.Image = global::MesManager.Properties.Resources.ClearGrid;
            this.tool_stockManager_ClearGrid.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_stockManager_ClearGrid.Name = "tool_stockManager_ClearGrid";
            this.tool_stockManager_ClearGrid.Size = new System.Drawing.Size(94, 26);
            this.tool_stockManager_ClearGrid.Text = "清空显示";
            // 
            // tool_stockManager_ClearDB
            // 
            this.tool_stockManager_ClearDB.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tool_stockManager_ClearDB.Image = global::MesManager.Properties.Resources.DeleteDataSource_16x16;
            this.tool_stockManager_ClearDB.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_stockManager_ClearDB.Name = "tool_stockManager_ClearDB";
            this.tool_stockManager_ClearDB.Size = new System.Drawing.Size(94, 26);
            this.tool_stockManager_ClearDB.Text = "清空数据";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(115, 26);
            this.toolStripLabel2.Text = "物料编码(RID):";
            // 
            // tool_stock_queryCondition
            // 
            this.tool_stock_queryCondition.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tool_stock_queryCondition.Name = "tool_stock_queryCondition";
            this.tool_stock_queryCondition.Size = new System.Drawing.Size(150, 29);
            // 
            // tool_stockManager_query
            // 
            this.tool_stockManager_query.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tool_stockManager_query.Image = global::MesManager.Properties.Resources.Search_16x16;
            this.tool_stockManager_query.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_stockManager_query.Name = "tool_stockManager_query";
            this.tool_stockManager_query.Size = new System.Drawing.Size(62, 26);
            this.tool_stockManager_query.Text = "查询";
            // 
            // documentContainer1
            // 
            this.documentContainer1.Controls.Add(this.documentTabStrip1);
            this.documentContainer1.Name = "documentContainer1";
            // 
            // 
            // 
            this.documentContainer1.RootElement.MinSize = new System.Drawing.Size(25, 25);
            this.documentContainer1.SizeInfo.SizeMode = Telerik.WinControls.UI.Docking.SplitPanelSizeMode.Fill;
            this.documentContainer1.ThemeName = "Breeze";
            // 
            // documentTabStrip1
            // 
            this.documentTabStrip1.CanUpdateChildIndex = true;
            this.documentTabStrip1.Controls.Add(this.dw_stockManager);
            this.documentTabStrip1.Controls.Add(this.dw_materialBind);
            this.documentTabStrip1.Location = new System.Drawing.Point(0, 0);
            this.documentTabStrip1.Name = "documentTabStrip1";
            // 
            // 
            // 
            this.documentTabStrip1.RootElement.MinSize = new System.Drawing.Size(25, 25);
            this.documentTabStrip1.SelectedIndex = 0;
            this.documentTabStrip1.Size = new System.Drawing.Size(1272, 722);
            this.documentTabStrip1.TabIndex = 0;
            this.documentTabStrip1.TabStop = false;
            this.documentTabStrip1.ThemeName = "Breeze";
            // 
            // dw_materialBind
            // 
            this.dw_materialBind.Controls.Add(this.radGridViewBind);
            this.dw_materialBind.Controls.Add(this.toolStrip3);
            this.dw_materialBind.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dw_materialBind.Location = new System.Drawing.Point(5, 27);
            this.dw_materialBind.Name = "dw_materialBind";
            this.dw_materialBind.PreviousDockState = Telerik.WinControls.UI.Docking.DockState.TabbedDocument;
            this.dw_materialBind.Size = new System.Drawing.Size(1262, 690);
            this.dw_materialBind.Text = "物料绑定";
            // 
            // radGridViewBind
            // 
            this.radGridViewBind.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(191)))), ((int)(((byte)(255)))));
            this.radGridViewBind.Cursor = System.Windows.Forms.Cursors.Default;
            this.radGridViewBind.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radGridViewBind.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.radGridViewBind.ForeColor = System.Drawing.Color.Black;
            this.radGridViewBind.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radGridViewBind.Location = new System.Drawing.Point(0, 29);
            // 
            // 
            // 
            gridViewTextBoxColumn1.EnableExpressionEditor = false;
            gridViewTextBoxColumn1.HeaderText = "序号";
            gridViewTextBoxColumn1.Name = "rdvc_order";
            gridViewComboBoxColumn1.EnableExpressionEditor = false;
            gridViewComboBoxColumn1.HeaderText = "产品型号";
            gridViewComboBoxColumn1.Name = "rdvc_typeNo";
            gridViewTextBoxColumn2.EnableExpressionEditor = false;
            gridViewTextBoxColumn2.HeaderText = "物料号";
            gridViewTextBoxColumn2.Name = "rdvc_materialPN";
            gridViewTextBoxColumn3.EnableExpressionEditor = false;
            gridViewTextBoxColumn3.HeaderText = "物料名称";
            gridViewTextBoxColumn3.Name = "dbv_material_name";
            gridViewTextBoxColumn4.EnableExpressionEditor = false;
            gridViewTextBoxColumn4.HeaderText = "描述";
            gridViewTextBoxColumn4.Name = "rdvc_describle";
            gridViewTextBoxColumn5.EnableExpressionEditor = false;
            gridViewTextBoxColumn5.HeaderText = "操作用户";
            gridViewTextBoxColumn5.Name = "rdbc_user";
            gridViewTextBoxColumn6.EnableExpressionEditor = false;
            gridViewTextBoxColumn6.HeaderText = "更新日期";
            gridViewTextBoxColumn6.Name = "rdbc_date";
            this.radGridViewBind.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewTextBoxColumn1,
            gridViewComboBoxColumn1,
            gridViewTextBoxColumn2,
            gridViewTextBoxColumn3,
            gridViewTextBoxColumn4,
            gridViewTextBoxColumn5,
            gridViewTextBoxColumn6});
            this.radGridViewBind.MasterTemplate.ViewDefinition = tableViewDefinition2;
            this.radGridViewBind.Name = "radGridViewBind";
            this.radGridViewBind.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.radGridViewBind.Size = new System.Drawing.Size(1262, 661);
            this.radGridViewBind.TabIndex = 19;
            this.radGridViewBind.ThemeName = "Breeze";
            // 
            // toolStrip3
            // 
            this.toolStrip3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tool_bind_add,
            this.tool_bind_delete,
            this.tool_bind_update,
            this.tool_bind_cleargrid,
            this.tool_bind_cleardb,
            this.toolStripLabel3,
            this.tool_bind_queryCondition,
            this.tool_bind_query});
            this.toolStrip3.Location = new System.Drawing.Point(0, 0);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(1262, 29);
            this.toolStrip3.TabIndex = 18;
            this.toolStrip3.Text = "toolStrip3";
            // 
            // tool_bind_add
            // 
            this.tool_bind_add.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tool_bind_add.Image = global::MesManager.Properties.Resources.add;
            this.tool_bind_add.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_bind_add.Name = "tool_bind_add";
            this.tool_bind_add.Size = new System.Drawing.Size(62, 26);
            this.tool_bind_add.Text = "新增";
            // 
            // tool_bind_delete
            // 
            this.tool_bind_delete.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tool_bind_delete.Image = global::MesManager.Properties.Resources.delete;
            this.tool_bind_delete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_bind_delete.Name = "tool_bind_delete";
            this.tool_bind_delete.Size = new System.Drawing.Size(62, 26);
            this.tool_bind_delete.Text = "删除";
            // 
            // tool_bind_update
            // 
            this.tool_bind_update.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tool_bind_update.Image = global::MesManager.Properties.Resources.Refresh_16x16;
            this.tool_bind_update.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_bind_update.Name = "tool_bind_update";
            this.tool_bind_update.Size = new System.Drawing.Size(62, 26);
            this.tool_bind_update.Text = "更新";
            // 
            // tool_bind_cleargrid
            // 
            this.tool_bind_cleargrid.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tool_bind_cleargrid.Image = global::MesManager.Properties.Resources.ClearGrid;
            this.tool_bind_cleargrid.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_bind_cleargrid.Name = "tool_bind_cleargrid";
            this.tool_bind_cleargrid.Size = new System.Drawing.Size(94, 26);
            this.tool_bind_cleargrid.Text = "清空显示";
            // 
            // tool_bind_cleardb
            // 
            this.tool_bind_cleardb.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tool_bind_cleardb.Image = global::MesManager.Properties.Resources.DeleteDataSource_16x16;
            this.tool_bind_cleardb.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_bind_cleardb.Name = "tool_bind_cleardb";
            this.tool_bind_cleardb.Size = new System.Drawing.Size(94, 26);
            this.tool_bind_cleardb.Text = "清空数据";
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(115, 26);
            this.toolStripLabel3.Text = "物料编码(RID):";
            // 
            // tool_bind_queryCondition
            // 
            this.tool_bind_queryCondition.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tool_bind_queryCondition.Name = "tool_bind_queryCondition";
            this.tool_bind_queryCondition.Size = new System.Drawing.Size(150, 29);
            // 
            // tool_bind_query
            // 
            this.tool_bind_query.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tool_bind_query.Image = global::MesManager.Properties.Resources.Search_16x16;
            this.tool_bind_query.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_bind_query.Name = "tool_bind_query";
            this.tool_bind_query.Size = new System.Drawing.Size(62, 26);
            this.tool_bind_query.Text = "查询";
            // 
            // ProductMaterial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(1272, 722);
            this.Controls.Add(this.radDock1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProductMaterial";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "物料管理";
            this.Load += new System.EventHandler(this.ProductMaterial_Load);
            ((System.ComponentModel.ISupportInitialize)(this.radDock1)).EndInit();
            this.radDock1.ResumeLayout(false);
            this.dw_stockManager.ResumeLayout(false);
            this.dw_stockManager.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridViewStock.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridViewStock)).EndInit();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.documentContainer1)).EndInit();
            this.documentContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.documentTabStrip1)).EndInit();
            this.documentTabStrip1.ResumeLayout(false);
            this.dw_materialBind.ResumeLayout(false);
            this.dw_materialBind.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridViewBind.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridViewBind)).EndInit();
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Telerik.WinControls.UI.Docking.RadDock radDock1;
        private Telerik.WinControls.UI.Docking.DocumentWindow dw_materialBind;
        private Telerik.WinControls.UI.Docking.DocumentContainer documentContainer1;
        private Telerik.WinControls.UI.Docking.DocumentTabStrip documentTabStrip1;
        private Telerik.WinControls.UI.Docking.DocumentWindow dw_stockManager;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox tool_stock_queryCondition;
        private System.Windows.Forms.ToolStripButton tool_stockManager_query;
        private System.Windows.Forms.ToolStripButton tool_stockManager_add;
        private System.Windows.Forms.ToolStripButton tool_stockManager_deleteSignal;
        private System.Windows.Forms.ToolStripButton tool_stockManager_update;
        private System.Windows.Forms.ToolStripButton tool_stockManager_ClearGrid;
        private System.Windows.Forms.ToolStripButton tool_stockManager_ClearDB;
        private Telerik.WinControls.UI.RadGridView radGridViewStock;
        private Telerik.WinControls.UI.RadGridView radGridViewBind;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripButton tool_bind_add;
        private System.Windows.Forms.ToolStripButton tool_bind_delete;
        private System.Windows.Forms.ToolStripButton tool_bind_update;
        private System.Windows.Forms.ToolStripButton tool_bind_cleargrid;
        private System.Windows.Forms.ToolStripButton tool_bind_cleardb;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripComboBox tool_bind_queryCondition;
        private System.Windows.Forms.ToolStripButton tool_bind_query;
    }
}
