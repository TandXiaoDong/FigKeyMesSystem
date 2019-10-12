namespace MesManager.RadView
{
    partial class Material
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
            Telerik.WinControls.UI.TableViewDefinition tableViewDefinition7 = new Telerik.WinControls.UI.TableViewDefinition();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Material));
            this.radGridView1 = new Telerik.WinControls.UI.RadGridView();
            this.btn_apply = new Telerik.WinControls.UI.RadButton();
            this.rlbx_explain = new Telerik.WinControls.UI.RadLabel();
            this.btn_select = new Telerik.WinControls.UI.RadButton();
            this.btn_clear_dgv = new Telerik.WinControls.UI.RadButton();
            this.btn_clear_server_data = new Telerik.WinControls.UI.RadButton();
            this.btn_cancel = new Telerik.WinControls.UI.RadButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_apply)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rlbx_explain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_select)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_clear_dgv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_clear_server_data)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_cancel)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radGridView1
            // 
            this.radGridView1.Location = new System.Drawing.Point(16, 51);
            // 
            // 
            // 
            this.radGridView1.MasterTemplate.ViewDefinition = tableViewDefinition7;
            this.radGridView1.Name = "radGridView1";
            this.radGridView1.Size = new System.Drawing.Size(542, 478);
            this.radGridView1.TabIndex = 0;
            // 
            // btn_apply
            // 
            this.btn_apply.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btn_apply.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_apply.ForeColor = System.Drawing.Color.Black;
            this.btn_apply.Location = new System.Drawing.Point(336, 602);
            this.btn_apply.Name = "btn_apply";
            // 
            // 
            // 
            this.btn_apply.RootElement.ApplyShapeToControl = true;
            this.btn_apply.RootElement.BorderHighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(119)))), ((int)(((byte)(52)))));
            this.btn_apply.Size = new System.Drawing.Size(110, 29);
            this.btn_apply.TabIndex = 8;
            this.btn_apply.Text = "提   交";
            this.btn_apply.Click += new System.EventHandler(this.Btn_apply_Click);
            // 
            // rlbx_explain
            // 
            this.rlbx_explain.Location = new System.Drawing.Point(16, 19);
            this.rlbx_explain.Name = "rlbx_explain";
            this.rlbx_explain.Size = new System.Drawing.Size(65, 18);
            this.rlbx_explain.TabIndex = 3;
            this.rlbx_explain.Text = "rlbx_explain";
            // 
            // btn_select
            // 
            this.btn_select.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btn_select.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_select.ForeColor = System.Drawing.Color.Black;
            this.btn_select.Location = new System.Drawing.Point(16, 21);
            this.btn_select.Name = "btn_select";
            this.btn_select.Size = new System.Drawing.Size(79, 24);
            this.btn_select.TabIndex = 5;
            this.btn_select.Text = "刷   新";
            this.btn_select.Click += new System.EventHandler(this.Btn_select_Click);
            // 
            // btn_clear_dgv
            // 
            this.btn_clear_dgv.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btn_clear_dgv.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_clear_dgv.ForeColor = System.Drawing.Color.Black;
            this.btn_clear_dgv.Location = new System.Drawing.Point(380, 21);
            this.btn_clear_dgv.Name = "btn_clear_dgv";
            this.btn_clear_dgv.Size = new System.Drawing.Size(79, 24);
            this.btn_clear_dgv.TabIndex = 7;
            this.btn_clear_dgv.Text = "清空显示";
            this.btn_clear_dgv.Click += new System.EventHandler(this.Btn_clear_dgv_Click);
            // 
            // btn_clear_server_data
            // 
            this.btn_clear_server_data.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btn_clear_server_data.ForeColor = System.Drawing.Color.Black;
            this.btn_clear_server_data.Location = new System.Drawing.Point(475, 21);
            this.btn_clear_server_data.Name = "btn_clear_server_data";
            this.btn_clear_server_data.Size = new System.Drawing.Size(79, 24);
            this.btn_clear_server_data.TabIndex = 6;
            this.btn_clear_server_data.Text = "清空数据";
            this.btn_clear_server_data.Click += new System.EventHandler(this.Btn_clear_server_data_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.btn_cancel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_cancel.ForeColor = System.Drawing.Color.Black;
            this.btn_cancel.Location = new System.Drawing.Point(466, 602);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(110, 29);
            this.btn_cancel.TabIndex = 11;
            this.btn_cancel.Text = "取   消";
            this.btn_cancel.Click += new System.EventHandler(this.Btn_cancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_clear_dgv);
            this.groupBox1.Controls.Add(this.btn_select);
            this.groupBox1.Controls.Add(this.btn_clear_server_data);
            this.groupBox1.Controls.Add(this.radGridView1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(564, 534);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "产品物料";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rlbx_explain);
            this.groupBox2.Location = new System.Drawing.Point(12, 552);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(564, 43);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "groupBox2";
            // 
            // Material
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(596, 643);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_apply);
            this.Controls.Add(this.btn_cancel);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Material";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "物料";
            this.ThemeName = "ControlDefault";
            this.Load += new System.EventHandler(this.Material_Load);
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_apply)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rlbx_explain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_select)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_clear_dgv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_clear_server_data)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_cancel)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadGridView radGridView1;
        private Telerik.WinControls.UI.RadButton btn_apply;
        private Telerik.WinControls.UI.RadLabel rlbx_explain;
        private Telerik.WinControls.UI.RadButton btn_select;
        private Telerik.WinControls.UI.RadButton btn_clear_dgv;
        private Telerik.WinControls.UI.RadButton btn_clear_server_data;
        private Telerik.WinControls.UI.RadButton btn_cancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}
