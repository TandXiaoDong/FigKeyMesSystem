namespace MesManager.UI
{
    partial class ModifyPwd
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModifyPwd));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tb_user = new System.Windows.Forms.TextBox();
            this.tb_oldPwd = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tb_newPwd = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tb_confirmPwd = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btn_modify = new Telerik.WinControls.UI.RadButton();
            this.btn_cancel = new Telerik.WinControls.UI.RadButton();
            this.breezeTheme1 = new Telerik.WinControls.Themes.BreezeTheme();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_modify)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_cancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(419, 64);
            this.panel1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(273, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "请输入用户旧密码与新密码，然后按\'修改\'按钮";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "【修改密码】";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = global::MesManager.Properties.Resources.ffpic1306125140893;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.pictureBox1.Location = new System.Drawing.Point(354, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(65, 64);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(81, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "用户名：";
            // 
            // tb_user
            // 
            this.tb_user.Location = new System.Drawing.Point(165, 105);
            this.tb_user.Name = "tb_user";
            this.tb_user.Size = new System.Drawing.Size(194, 21);
            this.tb_user.TabIndex = 2;
            // 
            // tb_oldPwd
            // 
            this.tb_oldPwd.Location = new System.Drawing.Point(165, 145);
            this.tb_oldPwd.Name = "tb_oldPwd";
            this.tb_oldPwd.Size = new System.Drawing.Size(194, 21);
            this.tb_oldPwd.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(81, 148);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "旧密码：";
            // 
            // tb_newPwd
            // 
            this.tb_newPwd.Location = new System.Drawing.Point(165, 185);
            this.tb_newPwd.Name = "tb_newPwd";
            this.tb_newPwd.Size = new System.Drawing.Size(194, 21);
            this.tb_newPwd.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(81, 188);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "新密码：";
            // 
            // tb_confirmPwd
            // 
            this.tb_confirmPwd.Location = new System.Drawing.Point(165, 222);
            this.tb_confirmPwd.Name = "tb_confirmPwd";
            this.tb_confirmPwd.Size = new System.Drawing.Size(194, 21);
            this.tb_confirmPwd.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(68, 225);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "确认密码：";
            // 
            // btn_modify
            // 
            this.btn_modify.Image = global::MesManager.Properties.Resources.Apply_16x16;
            this.btn_modify.Location = new System.Drawing.Point(167, 292);
            this.btn_modify.Name = "btn_modify";
            this.btn_modify.Size = new System.Drawing.Size(74, 24);
            this.btn_modify.TabIndex = 11;
            this.btn_modify.Text = "修改";
            this.btn_modify.ThemeName = "Breeze";
            this.btn_modify.Click += new System.EventHandler(this.Btn_modify_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.Image = global::MesManager.Properties.Resources.Cancel_16x16;
            this.btn_cancel.Location = new System.Drawing.Point(270, 292);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(74, 24);
            this.btn_cancel.TabIndex = 12;
            this.btn_cancel.Text = "取消";
            this.btn_cancel.ThemeName = "Breeze";
            this.btn_cancel.Click += new System.EventHandler(this.Btn_cancel_Click);
            // 
            // ModifyPwd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(419, 338);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_modify);
            this.Controls.Add(this.tb_confirmPwd);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tb_newPwd);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tb_oldPwd);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tb_user);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ModifyPwd";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "修改密码";
            this.Load += new System.EventHandler(this.ModifyPwd_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_modify)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_cancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tb_user;
        private System.Windows.Forms.TextBox tb_oldPwd;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tb_newPwd;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tb_confirmPwd;
        private System.Windows.Forms.Label label6;
        private Telerik.WinControls.UI.RadButton btn_modify;
        private Telerik.WinControls.UI.RadButton btn_cancel;
        private Telerik.WinControls.Themes.BreezeTheme breezeTheme1;
    }
}
