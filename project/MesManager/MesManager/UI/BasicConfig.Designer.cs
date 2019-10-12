namespace MesManager.UI
{
    partial class BasicConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BasicConfig));
            Telerik.WinControls.UI.TableViewDefinition tableViewDefinition2 = new Telerik.WinControls.UI.TableViewDefinition();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.circleShape1 = new Telerik.WinControls.CircleShape();
            this.chamferedRectShape1 = new Telerik.WinControls.ChamferedRectShape();
            this.donutShape1 = new Telerik.WinControls.Tests.DonutShape();
            this.customShape1 = new Telerik.WinControls.OldShapeEditor.CustomShape();
            this.radMenu1 = new Telerik.WinControls.UI.RadMenu();
            this.menu_add = new Telerik.WinControls.UI.RadMenuItem();
            this.menu_del = new Telerik.WinControls.UI.RadMenuItem();
            this.menu_commit = new Telerik.WinControls.UI.RadMenuItem();
            this.menu_refresh = new Telerik.WinControls.UI.RadMenuItem();
            this.menu_grid = new Telerik.WinControls.UI.RadMenuItem();
            this.menu_clear_db = new Telerik.WinControls.UI.RadMenuItem();
            this.menu_export = new Telerik.WinControls.UI.RadMenuItem();
            this.radGridView1 = new Telerik.WinControls.UI.RadGridView();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radMenu1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.Transparent;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 715);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1272, 22);
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.ForeColor = System.Drawing.Color.White;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(68, 17);
            this.toolStripStatusLabel1.Text = "当前用户：";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.ForeColor = System.Drawing.Color.White;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(44, 17);
            this.toolStripStatusLabel2.Text = "admin";
            // 
            // circleShape1
            // 
            this.circleShape1.IsRightToLeft = false;
            // 
            // chamferedRectShape1
            // 
            this.chamferedRectShape1.IsRightToLeft = false;
            // 
            // donutShape1
            // 
            this.donutShape1.IsRightToLeft = false;
            // 
            // customShape1
            // 
            this.customShape1.Dimension = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.customShape1.IsRightToLeft = false;
            // 
            // radMenu1
            // 
            this.radMenu1.BackColor = System.Drawing.Color.Transparent;
            this.radMenu1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radMenu1.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.menu_add,
            this.menu_del,
            this.menu_commit,
            this.menu_refresh,
            this.menu_grid,
            this.menu_clear_db,
            this.menu_export});
            this.radMenu1.Location = new System.Drawing.Point(0, 0);
            this.radMenu1.Name = "radMenu1";
            this.radMenu1.Size = new System.Drawing.Size(1272, 36);
            this.radMenu1.TabIndex = 9;
            // 
            // menu_add
            // 
            this.menu_add.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.menu_add.Image = global::MesManager.Properties.Resources.bullet_add;
            this.menu_add.Name = "menu_add";
            this.menu_add.Text = "新增";
            // 
            // menu_del
            // 
            this.menu_del.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.menu_del.Image = ((System.Drawing.Image)(resources.GetObject("menu_del.Image")));
            this.menu_del.Name = "menu_del";
            this.menu_del.Shape = null;
            this.menu_del.Text = "删除";
            // 
            // menu_commit
            // 
            this.menu_commit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.menu_commit.Image = global::MesManager.Properties.Resources.upload_for_cloud;
            this.menu_commit.Name = "menu_commit";
            this.menu_commit.Text = "修改";
            this.menu_commit.Click += new System.EventHandler(this.Menu_commit_Click);
            // 
            // menu_refresh
            // 
            this.menu_refresh.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.menu_refresh.Image = global::MesManager.Properties.Resources.update;
            this.menu_refresh.Name = "menu_refresh";
            this.menu_refresh.Text = "刷新";
            // 
            // menu_grid
            // 
            this.menu_grid.ForeColor = System.Drawing.Color.White;
            this.menu_grid.Image = global::MesManager.Properties.Resources.ClearGrid;
            this.menu_grid.Name = "menu_grid";
            this.menu_grid.Text = "清空显示";
            // 
            // menu_clear_db
            // 
            this.menu_clear_db.ForeColor = System.Drawing.Color.White;
            this.menu_clear_db.Image = global::MesManager.Properties.Resources.DeleteDataSource_16x16;
            this.menu_clear_db.Name = "menu_clear_db";
            this.menu_clear_db.Text = "清空数据";
            // 
            // menu_export
            // 
            this.menu_export.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.menu_export.Image = global::MesManager.Properties.Resources.Export_16x16;
            this.menu_export.Name = "menu_export";
            this.menu_export.Text = "导出";
            // 
            // radGridView1
            // 
            this.radGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radGridView1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radGridView1.Location = new System.Drawing.Point(0, 36);
            // 
            // 
            // 
            this.radGridView1.MasterTemplate.ViewDefinition = tableViewDefinition2;
            this.radGridView1.Name = "radGridView1";
            this.radGridView1.Size = new System.Drawing.Size(1272, 679);
            this.radGridView1.TabIndex = 14;
            this.radGridView1.ThemeName = "Breeze";
            // 
            // BasicConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(1272, 737);
            this.Controls.Add(this.radGridView1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.radMenu1);
            this.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BasicConfig";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "产品管理";
            this.Load += new System.EventHandler(this.BasicConfig_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radMenu1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Telerik.WinControls.UI.RadMenuItem menu_del;
        private Telerik.WinControls.UI.RadMenuItem menu_commit;
        private Telerik.WinControls.UI.RadMenuItem menu_refresh;
        private Telerik.WinControls.UI.RadMenuItem menu_grid;
        private Telerik.WinControls.UI.RadMenuItem menu_clear_db;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private Telerik.WinControls.CircleShape circleShape1;
        private Telerik.WinControls.ChamferedRectShape chamferedRectShape1;
        private Telerik.WinControls.Tests.DonutShape donutShape1;
        private Telerik.WinControls.OldShapeEditor.CustomShape customShape1;
        private Telerik.WinControls.UI.RadMenu radMenu1;
        private Telerik.WinControls.UI.RadMenuItem menu_add;
        private Telerik.WinControls.UI.RadGridView radGridView1;
        private Telerik.WinControls.UI.RadMenuItem menu_export;
    }
}
