namespace MesManager
{
    partial class SetStation
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
            this.tb_remark = new System.Windows.Forms.TextBox();
            this.radLabel6 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            this.cb_testRes = new System.Windows.Forms.ComboBox();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.cb_station = new System.Windows.Forms.ComboBox();
            this.tb_sn = new System.Windows.Forms.TextBox();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.cb_typeNo = new System.Windows.Forms.ComboBox();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.btn_apply = new Telerik.WinControls.UI.RadButton();
            this.btn_cancel = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_apply)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_cancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // tb_remark
            // 
            this.tb_remark.Location = new System.Drawing.Point(121, 197);
            this.tb_remark.Name = "tb_remark";
            this.tb_remark.Size = new System.Drawing.Size(200, 21);
            this.tb_remark.TabIndex = 24;
            // 
            // radLabel6
            // 
            this.radLabel6.Location = new System.Drawing.Point(43, 197);
            this.radLabel6.Name = "radLabel6";
            this.radLabel6.Size = new System.Drawing.Size(30, 18);
            this.radLabel6.TabIndex = 23;
            this.radLabel6.Text = "标签";
            // 
            // radLabel4
            // 
            this.radLabel4.Location = new System.Drawing.Point(43, 156);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(54, 18);
            this.radLabel4.TabIndex = 20;
            this.radLabel4.Text = "测试结果";
            // 
            // cb_testRes
            // 
            this.cb_testRes.FormattingEnabled = true;
            this.cb_testRes.Location = new System.Drawing.Point(121, 156);
            this.cb_testRes.Name = "cb_testRes";
            this.cb_testRes.Size = new System.Drawing.Size(200, 20);
            this.cb_testRes.TabIndex = 19;
            // 
            // radLabel3
            // 
            this.radLabel3.Location = new System.Drawing.Point(43, 115);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(42, 18);
            this.radLabel3.TabIndex = 18;
            this.radLabel3.Text = "站位名";
            // 
            // cb_station
            // 
            this.cb_station.FormattingEnabled = true;
            this.cb_station.Location = new System.Drawing.Point(121, 115);
            this.cb_station.Name = "cb_station";
            this.cb_station.Size = new System.Drawing.Size(200, 20);
            this.cb_station.TabIndex = 17;
            // 
            // tb_sn
            // 
            this.tb_sn.Location = new System.Drawing.Point(121, 74);
            this.tb_sn.Name = "tb_sn";
            this.tb_sn.Size = new System.Drawing.Size(200, 21);
            this.tb_sn.TabIndex = 16;
            // 
            // radLabel2
            // 
            this.radLabel2.Location = new System.Drawing.Point(43, 77);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(42, 18);
            this.radLabel2.TabIndex = 15;
            this.radLabel2.Text = "追溯码";
            // 
            // cb_typeNo
            // 
            this.cb_typeNo.FormattingEnabled = true;
            this.cb_typeNo.Location = new System.Drawing.Point(121, 32);
            this.cb_typeNo.Name = "cb_typeNo";
            this.cb_typeNo.Size = new System.Drawing.Size(200, 20);
            this.cb_typeNo.TabIndex = 14;
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(43, 34);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(42, 18);
            this.radLabel1.TabIndex = 13;
            this.radLabel1.Text = "零件号";
            // 
            // btn_apply
            // 
            this.btn_apply.Location = new System.Drawing.Point(161, 248);
            this.btn_apply.Name = "btn_apply";
            this.btn_apply.Size = new System.Drawing.Size(60, 24);
            this.btn_apply.TabIndex = 25;
            this.btn_apply.Text = "应用";
            this.btn_apply.Click += new System.EventHandler(this.Btn_apply_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.Location = new System.Drawing.Point(257, 248);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(64, 24);
            this.btn_cancel.TabIndex = 26;
            this.btn_cancel.Text = "取消";
            this.btn_cancel.Click += new System.EventHandler(this.Btn_cancel_Click);
            // 
            // SetStation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 297);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_apply);
            this.Controls.Add(this.tb_remark);
            this.Controls.Add(this.radLabel6);
            this.Controls.Add(this.radLabel4);
            this.Controls.Add(this.cb_testRes);
            this.Controls.Add(this.radLabel3);
            this.Controls.Add(this.cb_station);
            this.Controls.Add(this.tb_sn);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.cb_typeNo);
            this.Controls.Add(this.radLabel1);
            this.Name = "SetStation";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "设站";
            this.Load += new System.EventHandler(this.SetStation_Load);
            ((System.ComponentModel.ISupportInitialize)(this.radLabel6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_apply)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_cancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_remark;
        private Telerik.WinControls.UI.RadLabel radLabel6;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private System.Windows.Forms.ComboBox cb_testRes;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private System.Windows.Forms.ComboBox cb_station;
        private System.Windows.Forms.TextBox tb_sn;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private System.Windows.Forms.ComboBox cb_typeNo;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadButton btn_apply;
        private Telerik.WinControls.UI.RadButton btn_cancel;
    }
}
