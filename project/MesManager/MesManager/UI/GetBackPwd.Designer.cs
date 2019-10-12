namespace MesManager.UI
{
    partial class GetBackPwd
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GetBackPwd));
            this.tb_repwd = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tb_pwd = new Telerik.WinControls.UI.RadTextBox();
            this.tb_username = new Telerik.WinControls.UI.RadTextBox();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.btn_cancel = new Telerik.WinControls.UI.RadButton();
            this.btn_register = new Telerik.WinControls.UI.RadButton();
            this.breezeTheme1 = new Telerik.WinControls.Themes.BreezeTheme();
            ((System.ComponentModel.ISupportInitialize)(this.tb_pwd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_username)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_cancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_register)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // tb_repwd
            // 
            this.tb_repwd.Location = new System.Drawing.Point(98, 126);
            this.tb_repwd.Name = "tb_repwd";
            this.tb_repwd.Size = new System.Drawing.Size(163, 21);
            this.tb_repwd.TabIndex = 17;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(16, 129);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "确认密码：";
            // 
            // tb_pwd
            // 
            this.tb_pwd.Location = new System.Drawing.Point(99, 82);
            this.tb_pwd.Margin = new System.Windows.Forms.Padding(3, 2, 3, 3);
            this.tb_pwd.Name = "tb_pwd";
            this.tb_pwd.Size = new System.Drawing.Size(162, 20);
            this.tb_pwd.TabIndex = 14;
            // 
            // tb_username
            // 
            this.tb_username.Location = new System.Drawing.Point(98, 33);
            this.tb_username.Name = "tb_username";
            this.tb_username.Size = new System.Drawing.Size(162, 20);
            this.tb_username.TabIndex = 13;
            // 
            // radLabel2
            // 
            this.radLabel2.ForeColor = System.Drawing.Color.White;
            this.radLabel2.Location = new System.Drawing.Point(34, 82);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(54, 18);
            this.radLabel2.TabIndex = 11;
            this.radLabel2.Text = "新密码：";
            // 
            // radLabel1
            // 
            this.radLabel1.ForeColor = System.Drawing.Color.White;
            this.radLabel1.Location = new System.Drawing.Point(23, 33);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(65, 18);
            this.radLabel1.TabIndex = 10;
            this.radLabel1.Text = "用户名称：";
            // 
            // btn_cancel
            // 
            this.btn_cancel.BackColor = System.Drawing.Color.White;
            this.btn_cancel.ForeColor = System.Drawing.Color.SteelBlue;
            this.btn_cancel.Image = global::MesManager.Properties.Resources.ClearGrid;
            this.btn_cancel.Location = new System.Drawing.Point(190, 181);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(70, 24);
            this.btn_cancel.TabIndex = 15;
            this.btn_cancel.Text = "取消";
            this.btn_cancel.ThemeName = "Breeze";
            this.btn_cancel.Click += new System.EventHandler(this.Btn_cancel_Click);
            // 
            // btn_register
            // 
            this.btn_register.BackColor = System.Drawing.Color.White;
            this.btn_register.ForeColor = System.Drawing.Color.SteelBlue;
            this.btn_register.Image = global::MesManager.Properties.Resources.Apply_16x16;
            this.btn_register.Location = new System.Drawing.Point(99, 181);
            this.btn_register.Name = "btn_register";
            this.btn_register.Size = new System.Drawing.Size(71, 24);
            this.btn_register.TabIndex = 12;
            this.btn_register.Text = "确定";
            this.btn_register.ThemeName = "Breeze";
            this.btn_register.Click += new System.EventHandler(this.Btn_register_Click);
            // 
            // GetBackPwd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(305, 244);
            this.Controls.Add(this.tb_repwd);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.tb_pwd);
            this.Controls.Add(this.tb_username);
            this.Controls.Add(this.btn_register);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.radLabel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GetBackPwd";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "找回密码";
            this.Load += new System.EventHandler(this.GetBackPwd_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tb_pwd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_username)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_cancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_register)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_repwd;
        private System.Windows.Forms.Label label4;
        private Telerik.WinControls.UI.RadButton btn_cancel;
        private Telerik.WinControls.UI.RadTextBox tb_pwd;
        private Telerik.WinControls.UI.RadTextBox tb_username;
        private Telerik.WinControls.UI.RadButton btn_register;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.Themes.BreezeTheme breezeTheme1;
    }
}
