namespace MesManager
{
    partial class ProductType
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
            this.radGridView1 = new Telerik.WinControls.UI.RadGridView();
            this.radGroupBox1 = new Telerik.WinControls.UI.RadGroupBox();
            this.btn_commit = new Telerik.WinControls.UI.RadButton();
            this.btn_select = new Telerik.WinControls.UI.RadButton();
            this.tbx_select_filter = new Telerik.WinControls.UI.RadTextBox();
            this.btn_clear_server = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox1)).BeginInit();
            this.radGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btn_commit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_select)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbx_select_filter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_clear_server)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radGridView1
            // 
            this.radGridView1.Location = new System.Drawing.Point(16, 58);
            // 
            // 
            // 
            this.radGridView1.MasterTemplate.ViewDefinition = tableViewDefinition1;
            this.radGridView1.Name = "radGridView1";
            this.radGridView1.Size = new System.Drawing.Size(331, 440);
            this.radGridView1.TabIndex = 0;
            // 
            // radGroupBox1
            // 
            this.radGroupBox1.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.radGroupBox1.Controls.Add(this.btn_clear_server);
            this.radGroupBox1.Controls.Add(this.tbx_select_filter);
            this.radGroupBox1.Controls.Add(this.btn_select);
            this.radGroupBox1.Controls.Add(this.btn_commit);
            this.radGroupBox1.Controls.Add(this.radGridView1);
            this.radGroupBox1.HeaderText = "型号配置";
            this.radGroupBox1.Location = new System.Drawing.Point(4, 12);
            this.radGroupBox1.Name = "radGroupBox1";
            this.radGroupBox1.Size = new System.Drawing.Size(352, 533);
            this.radGroupBox1.TabIndex = 1;
            this.radGroupBox1.Text = "型号配置";
            // 
            // btn_commit
            // 
            this.btn_commit.Location = new System.Drawing.Point(237, 504);
            this.btn_commit.Name = "btn_commit";
            this.btn_commit.Size = new System.Drawing.Size(110, 24);
            this.btn_commit.TabIndex = 1;
            this.btn_commit.Text = "提交";
            // 
            // btn_select
            // 
            this.btn_select.Location = new System.Drawing.Point(237, 21);
            this.btn_select.Name = "btn_select";
            this.btn_select.Size = new System.Drawing.Size(110, 24);
            this.btn_select.TabIndex = 2;
            this.btn_select.Text = "查询";
            // 
            // tbx_select_filter
            // 
            this.tbx_select_filter.Location = new System.Drawing.Point(16, 23);
            this.tbx_select_filter.Name = "tbx_select_filter";
            this.tbx_select_filter.Size = new System.Drawing.Size(215, 20);
            this.tbx_select_filter.TabIndex = 3;
            // 
            // btn_clear_server
            // 
            this.btn_clear_server.Location = new System.Drawing.Point(16, 504);
            this.btn_clear_server.Name = "btn_clear_server";
            this.btn_clear_server.Size = new System.Drawing.Size(110, 24);
            this.btn_clear_server.TabIndex = 4;
            this.btn_clear_server.Text = "清空数据";
            // 
            // ProductType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 557);
            this.Controls.Add(this.radGroupBox1);
            this.Name = "ProductType";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "产品型号";
            this.Load += new System.EventHandler(this.ProductType_Load);
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox1)).EndInit();
            this.radGroupBox1.ResumeLayout(false);
            this.radGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btn_commit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_select)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbx_select_filter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_clear_server)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadGridView radGridView1;
        private Telerik.WinControls.UI.RadGroupBox radGroupBox1;
        private Telerik.WinControls.UI.RadButton btn_commit;
        private Telerik.WinControls.UI.RadTextBox tbx_select_filter;
        private Telerik.WinControls.UI.RadButton btn_select;
        private Telerik.WinControls.UI.RadButton btn_clear_server;
    }
}
