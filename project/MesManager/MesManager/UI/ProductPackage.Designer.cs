namespace MesManager.UI
{
    partial class ProductPackage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProductPackage));
            Telerik.WinControls.UI.GridViewImageColumn gridViewImageColumn1 = new Telerik.WinControls.UI.GridViewImageColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn1 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn2 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn3 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn4 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.TableViewDefinition tableViewDefinition1 = new Telerik.WinControls.UI.TableViewDefinition();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_sn = new System.Windows.Forms.TextBox();
            this.tb_caseCode = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.menu_delete = new Telerik.WinControls.UI.RadMenuItem();
            this.menu_update = new Telerik.WinControls.UI.RadMenuItem();
            this.menu_grid = new Telerik.WinControls.UI.RadMenuItem();
            this.menu_clear_db = new Telerik.WinControls.UI.RadMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tool_materialCode = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel5 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tool_curNumber = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radGridView1 = new Telerik.WinControls.UI.RadGridView();
            this.radMenu1 = new Telerik.WinControls.UI.RadMenu();
            this.menu_add = new Telerik.WinControls.UI.RadMenuItem();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radMenu1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "产品SN:";
            // 
            // tb_sn
            // 
            this.tb_sn.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_sn.Location = new System.Drawing.Point(80, 10);
            this.tb_sn.Name = "tb_sn";
            this.tb_sn.Size = new System.Drawing.Size(366, 25);
            this.tb_sn.TabIndex = 2;
            // 
            // tb_caseCode
            // 
            this.tb_caseCode.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_caseCode.Location = new System.Drawing.Point(544, 10);
            this.tb_caseCode.Name = "tb_caseCode";
            this.tb_caseCode.Size = new System.Drawing.Size(366, 25);
            this.tb_caseCode.TabIndex = 12;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(466, 16);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(72, 13);
            this.label10.TabIndex = 11;
            this.label10.Text = "箱子编码：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(77, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(515, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "说明：输入SN，添加查询记录，最后将打印的箱子编码录入，点结束包装，本次装箱结束";
            // 
            // menu_delete
            // 
            this.menu_delete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.menu_delete.Image = ((System.Drawing.Image)(resources.GetObject("menu_delete.Image")));
            this.menu_delete.Name = "menu_delete";
            this.menu_delete.Shape = null;
            this.menu_delete.Text = "删除";
            this.menu_delete.UseCompatibleTextRendering = false;
            // 
            // menu_update
            // 
            this.menu_update.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.menu_update.Image = global::MesManager.Properties.Resources.update;
            this.menu_update.Name = "menu_update";
            this.menu_update.Text = "更新包装";
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
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.Transparent;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.tool_materialCode,
            this.toolStripStatusLabel5,
            this.tool_curNumber});
            this.statusStrip1.Location = new System.Drawing.Point(0, 707);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(936, 22);
            this.statusStrip1.TabIndex = 15;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(68, 17);
            this.toolStripStatusLabel1.Text = "箱子编码：";
            // 
            // tool_materialCode
            // 
            this.tool_materialCode.Name = "tool_materialCode";
            this.tool_materialCode.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel5
            // 
            this.toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            this.toolStripStatusLabel5.Size = new System.Drawing.Size(68, 17);
            this.toolStripStatusLabel5.Text = "已装数量：";
            // 
            // tool_curNumber
            // 
            this.tool_curNumber.Name = "tool_curNumber";
            this.tool_curNumber.Size = new System.Drawing.Size(0, 17);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.tb_sn);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.tb_caseCode);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 36);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(936, 85);
            this.panel1.TabIndex = 16;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radGridView1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 121);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(936, 586);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "装箱记录";
            // 
            // radGridView1
            // 
            this.radGridView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.radGridView1.Cursor = System.Windows.Forms.Cursors.Default;
            this.radGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radGridView1.Font = new System.Drawing.Font("Segoe UI", 11.25F);
            this.radGridView1.ForeColor = System.Drawing.Color.Black;
            this.radGridView1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radGridView1.Location = new System.Drawing.Point(3, 18);
            // 
            // 
            // 
            gridViewImageColumn1.EnableExpressionEditor = false;
            gridViewImageColumn1.HeaderText = "删除";
            gridViewImageColumn1.Name = "dgv_delete";
            gridViewTextBoxColumn1.EnableExpressionEditor = false;
            gridViewTextBoxColumn1.HeaderText = "序号";
            gridViewTextBoxColumn1.Name = "dgv_order";
            gridViewTextBoxColumn2.EnableExpressionEditor = false;
            gridViewTextBoxColumn2.HeaderText = "产品SN";
            gridViewTextBoxColumn2.Name = "dgv_sn";
            gridViewTextBoxColumn3.EnableExpressionEditor = false;
            gridViewTextBoxColumn3.HeaderText = "绑定状态";
            gridViewTextBoxColumn3.Name = "dgv_binding_state";
            gridViewTextBoxColumn4.EnableExpressionEditor = false;
            gridViewTextBoxColumn4.HeaderText = "备注";
            gridViewTextBoxColumn4.Name = "dgv_remark";
            this.radGridView1.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewImageColumn1,
            gridViewTextBoxColumn1,
            gridViewTextBoxColumn2,
            gridViewTextBoxColumn3,
            gridViewTextBoxColumn4});
            this.radGridView1.MasterTemplate.ViewDefinition = tableViewDefinition1;
            this.radGridView1.Name = "radGridView1";
            this.radGridView1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.radGridView1.Size = new System.Drawing.Size(930, 565);
            this.radGridView1.TabIndex = 0;
            this.radGridView1.ThemeName = "Breeze";
            // 
            // radMenu1
            // 
            this.radMenu1.BackColor = System.Drawing.Color.Transparent;
            this.radMenu1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radMenu1.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.menu_add,
            this.menu_delete,
            this.menu_update,
            this.menu_grid,
            this.menu_clear_db});
            this.radMenu1.Location = new System.Drawing.Point(0, 0);
            this.radMenu1.Name = "radMenu1";
            this.radMenu1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.radMenu1.Size = new System.Drawing.Size(936, 36);
            this.radMenu1.TabIndex = 14;
            // 
            // menu_add
            // 
            this.menu_add.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.menu_add.Image = global::MesManager.Properties.Resources.bullet_add;
            this.menu_add.Name = "menu_add";
            this.menu_add.Text = "添加";
            // 
            // ProductPackage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(936, 729);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.radMenu1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProductPackage";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "成品装箱";
            this.Load += new System.EventHandler(this.ProductPackage_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radMenu1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_sn;
        private System.Windows.Forms.TextBox tb_caseCode;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label2;
        private Telerik.WinControls.UI.RadMenuItem menu_delete;
        private Telerik.WinControls.UI.RadMenuItem menu_update;
        private Telerik.WinControls.UI.RadMenuItem menu_grid;
        private Telerik.WinControls.UI.RadMenuItem menu_clear_db;
        private Telerik.WinControls.UI.RadMenu radMenu1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel tool_materialCode;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel5;
        private System.Windows.Forms.ToolStripStatusLabel tool_curNumber;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox2;
        private Telerik.WinControls.UI.RadGridView radGridView1;
        private Telerik.WinControls.UI.RadMenuItem menu_add;
    }
}
