namespace MesManager
{
    partial class Station
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
            this.btn_apply = new Telerik.WinControls.UI.RadButton();
            this.rlbx_explain = new Telerik.WinControls.UI.RadLabel();
            this.radGroupBox1 = new Telerik.WinControls.UI.RadGroupBox();
            this.btn_select = new Telerik.WinControls.UI.RadButton();
            this.radGroupBox2 = new Telerik.WinControls.UI.RadGroupBox();
            this.btn_clear_dgv = new Telerik.WinControls.UI.RadButton();
            this.btn_clear_server_data = new Telerik.WinControls.UI.RadButton();
            this.btn_cancel = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_apply)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rlbx_explain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox1)).BeginInit();
            this.radGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btn_select)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox2)).BeginInit();
            this.radGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btn_clear_dgv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_clear_server_data)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_cancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radGridView1
            // 
            this.radGridView1.Location = new System.Drawing.Point(14, 51);
            // 
            // 
            // 
            this.radGridView1.MasterTemplate.ViewDefinition = tableViewDefinition1;
            this.radGridView1.Name = "radGridView1";
            this.radGridView1.Size = new System.Drawing.Size(413, 478);
            this.radGridView1.TabIndex = 0;
            // 
            // btn_apply
            // 
            this.btn_apply.Location = new System.Drawing.Point(213, 617);
            this.btn_apply.Name = "btn_apply";
            this.btn_apply.Size = new System.Drawing.Size(110, 29);
            this.btn_apply.TabIndex = 2;
            this.btn_apply.Text = "提交";
            // 
            // rlbx_explain
            // 
            this.rlbx_explain.Location = new System.Drawing.Point(14, 21);
            this.rlbx_explain.Name = "rlbx_explain";
            this.rlbx_explain.Size = new System.Drawing.Size(65, 18);
            this.rlbx_explain.TabIndex = 3;
            this.rlbx_explain.Text = "rlbx_explain";
            // 
            // radGroupBox1
            // 
            this.radGroupBox1.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.radGroupBox1.Controls.Add(this.rlbx_explain);
            this.radGroupBox1.HeaderText = "说明";
            this.radGroupBox1.Location = new System.Drawing.Point(12, 565);
            this.radGroupBox1.Name = "radGroupBox1";
            this.radGroupBox1.Size = new System.Drawing.Size(441, 46);
            this.radGroupBox1.TabIndex = 4;
            this.radGroupBox1.Text = "说明";
            // 
            // btn_select
            // 
            this.btn_select.Location = new System.Drawing.Point(14, 21);
            this.btn_select.Name = "btn_select";
            this.btn_select.Size = new System.Drawing.Size(79, 24);
            this.btn_select.TabIndex = 5;
            this.btn_select.Text = "刷新";
            // 
            // radGroupBox2
            // 
            this.radGroupBox2.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.radGroupBox2.Controls.Add(this.btn_clear_dgv);
            this.radGroupBox2.Controls.Add(this.btn_clear_server_data);
            this.radGroupBox2.Controls.Add(this.radGridView1);
            this.radGroupBox2.Controls.Add(this.btn_select);
            this.radGroupBox2.HeaderText = "生产线站位";
            this.radGroupBox2.Location = new System.Drawing.Point(12, 25);
            this.radGroupBox2.Name = "radGroupBox2";
            this.radGroupBox2.Size = new System.Drawing.Size(441, 534);
            this.radGroupBox2.TabIndex = 6;
            this.radGroupBox2.Text = "生产线站位";
            // 
            // btn_clear_dgv
            // 
            this.btn_clear_dgv.Location = new System.Drawing.Point(250, 21);
            this.btn_clear_dgv.Name = "btn_clear_dgv";
            this.btn_clear_dgv.Size = new System.Drawing.Size(79, 24);
            this.btn_clear_dgv.TabIndex = 7;
            this.btn_clear_dgv.Text = "清空显示";
            // 
            // btn_clear_server_data
            // 
            this.btn_clear_server_data.Location = new System.Drawing.Point(345, 21);
            this.btn_clear_server_data.Name = "btn_clear_server_data";
            this.btn_clear_server_data.Size = new System.Drawing.Size(79, 24);
            this.btn_clear_server_data.TabIndex = 6;
            this.btn_clear_server_data.Text = "清空数据";
            // 
            // btn_cancel
            // 
            this.btn_cancel.Location = new System.Drawing.Point(343, 617);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(110, 29);
            this.btn_cancel.TabIndex = 7;
            this.btn_cancel.Text = "取消";
            // 
            // Station
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 656);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.radGroupBox2);
            this.Controls.Add(this.radGroupBox1);
            this.Controls.Add(this.btn_apply);
            this.Name = "Station";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "生产线";
            this.Load += new System.EventHandler(this.SetProduce_Load);
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_apply)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rlbx_explain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox1)).EndInit();
            this.radGroupBox1.ResumeLayout(false);
            this.radGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btn_select)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox2)).EndInit();
            this.radGroupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btn_clear_dgv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_clear_server_data)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_cancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadGridView radGridView1;
        private Telerik.WinControls.UI.RadButton btn_apply;
        private Telerik.WinControls.UI.RadLabel rlbx_explain;
        private Telerik.WinControls.UI.RadGroupBox radGroupBox1;
        private Telerik.WinControls.UI.RadButton btn_select;
        private Telerik.WinControls.UI.RadGroupBox radGroupBox2;
        private Telerik.WinControls.UI.RadButton btn_cancel;
        private Telerik.WinControls.UI.RadButton btn_clear_dgv;
        private Telerik.WinControls.UI.RadButton btn_clear_server_data;
    }
}
