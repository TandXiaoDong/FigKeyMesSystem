namespace MesManager.UI
{
    partial class GraphView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphView));
            this.menu_sn_result = new Telerik.WinControls.UI.RadMenuItem();
            this.menu_package = new Telerik.WinControls.UI.RadMenuItem();
            this.menu_material = new Telerik.WinControls.UI.RadMenuItem();
            this.menu_productCheck = new Telerik.WinControls.UI.RadMenuItem();
            this.menu_quanlity = new Telerik.WinControls.UI.RadMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            this.panel2 = new System.Windows.Forms.Panel();
            this.radMenu1 = new Telerik.WinControls.UI.RadMenu();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radMenu1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // menu_sn_result
            // 
            this.menu_sn_result.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.menu_sn_result.Image = global::MesManager.Properties.Resources.bullet_add;
            this.menu_sn_result.Name = "menu_sn_result";
            this.menu_sn_result.Text = "SN过站记录";
            this.menu_sn_result.UseCompatibleTextRendering = false;
            // 
            // menu_package
            // 
            this.menu_package.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.menu_package.Image = ((System.Drawing.Image)(resources.GetObject("menu_package.Image")));
            this.menu_package.Name = "menu_package";
            this.menu_package.Shape = null;
            this.menu_package.Text = "包装信息";
            this.menu_package.UseCompatibleTextRendering = false;
            // 
            // menu_material
            // 
            this.menu_material.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.menu_material.Image = global::MesManager.Properties.Resources.update;
            this.menu_material.Name = "menu_material";
            this.menu_material.Text = "物料信息";
            this.menu_material.UseCompatibleTextRendering = false;
            // 
            // menu_productCheck
            // 
            this.menu_productCheck.ForeColor = System.Drawing.Color.White;
            this.menu_productCheck.Image = global::MesManager.Properties.Resources.ClearGrid;
            this.menu_productCheck.Name = "menu_productCheck";
            this.menu_productCheck.Text = "成品抽检";
            this.menu_productCheck.UseCompatibleTextRendering = false;
            // 
            // menu_quanlity
            // 
            this.menu_quanlity.ForeColor = System.Drawing.Color.White;
            this.menu_quanlity.Image = global::MesManager.Properties.Resources.DeleteDataSource_16x16;
            this.menu_quanlity.Name = "menu_quanlity";
            this.menu_quanlity.Text = "品质异常";
            this.menu_quanlity.UseCompatibleTextRendering = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.zedGraphControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 36);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1164, 494);
            this.panel1.TabIndex = 15;
            // 
            // zedGraphControl1
            // 
            this.zedGraphControl1.Location = new System.Drawing.Point(24, 28);
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.ScrollGrace = 0D;
            this.zedGraphControl1.ScrollMaxX = 0D;
            this.zedGraphControl1.ScrollMaxY = 0D;
            this.zedGraphControl1.ScrollMaxY2 = 0D;
            this.zedGraphControl1.ScrollMinX = 0D;
            this.zedGraphControl1.ScrollMinY = 0D;
            this.zedGraphControl1.ScrollMinY2 = 0D;
            this.zedGraphControl1.Size = new System.Drawing.Size(1099, 448);
            this.zedGraphControl1.TabIndex = 0;
            this.zedGraphControl1.UseExtendedPrintDialog = true;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 530);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1164, 130);
            this.panel2.TabIndex = 16;
            // 
            // radMenu1
            // 
            this.radMenu1.BackColor = System.Drawing.Color.Transparent;
            this.radMenu1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radMenu1.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.menu_sn_result,
            this.menu_package,
            this.menu_material,
            this.menu_productCheck,
            this.menu_quanlity});
            this.radMenu1.Location = new System.Drawing.Point(0, 0);
            this.radMenu1.Name = "radMenu1";
            this.radMenu1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.radMenu1.Size = new System.Drawing.Size(1164, 36);
            this.radMenu1.TabIndex = 14;
            // 
            // GraphView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.MediumBlue;
            this.ClientSize = new System.Drawing.Size(1164, 660);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.radMenu1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GraphView";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "报表查询";
            this.Load += new System.EventHandler(this.GraphView_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radMenu1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Telerik.WinControls.UI.RadMenuItem menu_sn_result;
        private Telerik.WinControls.UI.RadMenuItem menu_package;
        private Telerik.WinControls.UI.RadMenuItem menu_material;
        private Telerik.WinControls.UI.RadMenuItem menu_productCheck;
        private Telerik.WinControls.UI.RadMenuItem menu_quanlity;
        private Telerik.WinControls.UI.RadMenu radMenu1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private ZedGraph.ZedGraphControl zedGraphControl1;
    }
}
