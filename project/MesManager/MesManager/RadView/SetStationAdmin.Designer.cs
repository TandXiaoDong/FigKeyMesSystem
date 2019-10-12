namespace MesManager
{
    partial class SetStationAdmin
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
            this.rdb_sn = new Telerik.WinControls.UI.RadRadioButton();
            this.rdb_type_no = new Telerik.WinControls.UI.RadRadioButton();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.cb_type_no = new System.Windows.Forms.ComboBox();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.btn_apply = new Telerik.WinControls.UI.RadButton();
            this.btn_cancel = new Telerik.WinControls.UI.RadButton();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.cb_sn_type_num = new System.Windows.Forms.ComboBox();
            this.cb_sn_station = new System.Windows.Forms.ComboBox();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel5 = new Telerik.WinControls.UI.RadLabel();
            this.tb_sn_sn = new System.Windows.Forms.TextBox();
            this.radGroupBox_type = new Telerik.WinControls.UI.RadGroupBox();
            this.listView_select_station = new System.Windows.Forms.ListView();
            this.radGroupBox_sn = new Telerik.WinControls.UI.RadGroupBox();
            this.radGroupBox1 = new Telerik.WinControls.UI.RadGroupBox();
            this.lbx_explain_sn = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.rdb_sn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdb_type_no)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_apply)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_cancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox_type)).BeginInit();
            this.radGroupBox_type.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox_sn)).BeginInit();
            this.radGroupBox_sn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox1)).BeginInit();
            this.radGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lbx_explain_sn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // rdb_sn
            // 
            this.rdb_sn.Location = new System.Drawing.Point(79, 23);
            this.rdb_sn.Name = "rdb_sn";
            this.rdb_sn.Size = new System.Drawing.Size(68, 18);
            this.rdb_sn.TabIndex = 0;
            this.rdb_sn.Text = "设追溯码";
            // 
            // rdb_type_no
            // 
            this.rdb_type_no.Location = new System.Drawing.Point(182, 23);
            this.rdb_type_no.Name = "rdb_type_no";
            this.rdb_type_no.Size = new System.Drawing.Size(68, 18);
            this.rdb_type_no.TabIndex = 1;
            this.rdb_type_no.Text = "设零件号";
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(5, 38);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(42, 18);
            this.radLabel1.TabIndex = 2;
            this.radLabel1.Text = "零件号";
            // 
            // cb_type_no
            // 
            this.cb_type_no.FormattingEnabled = true;
            this.cb_type_no.Location = new System.Drawing.Point(65, 36);
            this.cb_type_no.Name = "cb_type_no";
            this.cb_type_no.Size = new System.Drawing.Size(217, 20);
            this.cb_type_no.TabIndex = 3;
            // 
            // radLabel2
            // 
            this.radLabel2.Location = new System.Drawing.Point(5, 78);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(54, 18);
            this.radLabel2.TabIndex = 5;
            this.radLabel2.Text = "选择站位";
            // 
            // btn_apply
            // 
            this.btn_apply.Location = new System.Drawing.Point(145, 475);
            this.btn_apply.Name = "btn_apply";
            this.btn_apply.Size = new System.Drawing.Size(73, 24);
            this.btn_apply.TabIndex = 6;
            this.btn_apply.Text = "应用";
            this.btn_apply.Click += new System.EventHandler(this.Btn_apply_Click_1);
            // 
            // btn_cancel
            // 
            this.btn_cancel.Location = new System.Drawing.Point(259, 475);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(73, 24);
            this.btn_cancel.TabIndex = 7;
            this.btn_cancel.Text = "取消";
            // 
            // radLabel3
            // 
            this.radLabel3.Location = new System.Drawing.Point(16, 31);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(42, 18);
            this.radLabel3.TabIndex = 0;
            this.radLabel3.Text = "零件号";
            // 
            // cb_sn_type_num
            // 
            this.cb_sn_type_num.FormattingEnabled = true;
            this.cb_sn_type_num.Location = new System.Drawing.Point(93, 29);
            this.cb_sn_type_num.Name = "cb_sn_type_num";
            this.cb_sn_type_num.Size = new System.Drawing.Size(160, 20);
            this.cb_sn_type_num.TabIndex = 1;
            // 
            // cb_sn_station
            // 
            this.cb_sn_station.FormattingEnabled = true;
            this.cb_sn_station.Location = new System.Drawing.Point(93, 125);
            this.cb_sn_station.Name = "cb_sn_station";
            this.cb_sn_station.Size = new System.Drawing.Size(160, 20);
            this.cb_sn_station.TabIndex = 3;
            // 
            // radLabel4
            // 
            this.radLabel4.Location = new System.Drawing.Point(16, 127);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(54, 18);
            this.radLabel4.TabIndex = 2;
            this.radLabel4.Text = "目标站位";
            // 
            // radLabel5
            // 
            this.radLabel5.Location = new System.Drawing.Point(16, 77);
            this.radLabel5.Name = "radLabel5";
            this.radLabel5.Size = new System.Drawing.Size(42, 18);
            this.radLabel5.TabIndex = 4;
            this.radLabel5.Text = "追溯码";
            // 
            // tb_sn_sn
            // 
            this.tb_sn_sn.Location = new System.Drawing.Point(93, 77);
            this.tb_sn_sn.Name = "tb_sn_sn";
            this.tb_sn_sn.Size = new System.Drawing.Size(160, 21);
            this.tb_sn_sn.TabIndex = 5;
            // 
            // radGroupBox_type
            // 
            this.radGroupBox_type.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.radGroupBox_type.Controls.Add(this.listView_select_station);
            this.radGroupBox_type.Controls.Add(this.radLabel1);
            this.radGroupBox_type.Controls.Add(this.cb_type_no);
            this.radGroupBox_type.Controls.Add(this.radLabel2);
            this.radGroupBox_type.HeaderText = "按零件号配置站位";
            this.radGroupBox_type.Location = new System.Drawing.Point(12, 65);
            this.radGroupBox_type.Name = "radGroupBox_type";
            this.radGroupBox_type.Size = new System.Drawing.Size(329, 398);
            this.radGroupBox_type.TabIndex = 6;
            this.radGroupBox_type.Text = "按零件号配置站位";
            // 
            // listView_select_station
            // 
            this.listView_select_station.Location = new System.Drawing.Point(65, 102);
            this.listView_select_station.Name = "listView_select_station";
            this.listView_select_station.Size = new System.Drawing.Size(217, 291);
            this.listView_select_station.TabIndex = 6;
            this.listView_select_station.UseCompatibleStateImageBehavior = false;
            // 
            // radGroupBox_sn
            // 
            this.radGroupBox_sn.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.radGroupBox_sn.Controls.Add(this.radGroupBox1);
            this.radGroupBox_sn.Controls.Add(this.tb_sn_sn);
            this.radGroupBox_sn.Controls.Add(this.radLabel3);
            this.radGroupBox_sn.Controls.Add(this.radLabel5);
            this.radGroupBox_sn.Controls.Add(this.cb_sn_type_num);
            this.radGroupBox_sn.Controls.Add(this.cb_sn_station);
            this.radGroupBox_sn.Controls.Add(this.radLabel4);
            this.radGroupBox_sn.HeaderText = "按追溯码设置";
            this.radGroupBox_sn.Location = new System.Drawing.Point(356, 65);
            this.radGroupBox_sn.Name = "radGroupBox_sn";
            this.radGroupBox_sn.Size = new System.Drawing.Size(329, 406);
            this.radGroupBox_sn.TabIndex = 10;
            this.radGroupBox_sn.Text = "按追溯码设置";
            // 
            // radGroupBox1
            // 
            this.radGroupBox1.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.radGroupBox1.Controls.Add(this.lbx_explain_sn);
            this.radGroupBox1.HeaderText = "说明";
            this.radGroupBox1.Location = new System.Drawing.Point(9, 239);
            this.radGroupBox1.Name = "radGroupBox1";
            this.radGroupBox1.Size = new System.Drawing.Size(313, 159);
            this.radGroupBox1.TabIndex = 11;
            this.radGroupBox1.Text = "说明";
            // 
            // lbx_explain_sn
            // 
            this.lbx_explain_sn.Location = new System.Drawing.Point(7, 39);
            this.lbx_explain_sn.Name = "lbx_explain_sn";
            this.lbx_explain_sn.Size = new System.Drawing.Size(77, 18);
            this.lbx_explain_sn.TabIndex = 6;
            this.lbx_explain_sn.Text = "lbx_explain_sn";
            // 
            // SetStationAdmin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(707, 508);
            this.Controls.Add(this.radGroupBox_sn);
            this.Controls.Add(this.radGroupBox_type);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_apply);
            this.Controls.Add(this.rdb_type_no);
            this.Controls.Add(this.rdb_sn);
            this.Name = "SetStationAdmin";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "设站";
            this.Load += new System.EventHandler(this.SetStationAdmin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.rdb_sn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdb_type_no)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_apply)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_cancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox_type)).EndInit();
            this.radGroupBox_type.ResumeLayout(false);
            this.radGroupBox_type.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox_sn)).EndInit();
            this.radGroupBox_sn.ResumeLayout(false);
            this.radGroupBox_sn.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox1)).EndInit();
            this.radGroupBox1.ResumeLayout(false);
            this.radGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lbx_explain_sn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadRadioButton rdb_sn;
        private Telerik.WinControls.UI.RadRadioButton rdb_type_no;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private System.Windows.Forms.ComboBox cb_type_no;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadButton btn_apply;
        private Telerik.WinControls.UI.RadButton btn_cancel;
        private System.Windows.Forms.TextBox tb_sn_sn;
        private Telerik.WinControls.UI.RadLabel radLabel5;
        private System.Windows.Forms.ComboBox cb_sn_station;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private System.Windows.Forms.ComboBox cb_sn_type_num;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.UI.RadGroupBox radGroupBox_type;
        private Telerik.WinControls.UI.RadGroupBox radGroupBox_sn;
        private Telerik.WinControls.UI.RadGroupBox radGroupBox1;
        private Telerik.WinControls.UI.RadLabel lbx_explain_sn;
        private System.Windows.Forms.ListView listView_select_station;
    }
}
