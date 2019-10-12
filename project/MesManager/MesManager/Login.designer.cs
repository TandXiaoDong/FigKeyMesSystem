using System.Windows.Forms;

namespace MesManager
{
    partial class Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.lbx_username = new Telerik.WinControls.UI.RadLabel();
            this.tbx_username = new System.Windows.Forms.ComboBox();
            this.lbx_pwd = new Telerik.WinControls.UI.RadLabel();
            this.tbx_pwd = new System.Windows.Forms.TextBox();
            this.cb_memberpwd = new Telerik.WinControls.UI.RadCheckBox();
            this.lbx_ToFindPwd = new System.Windows.Forms.LinkLabel();
            this.btn_login = new Telerik.WinControls.UI.RadButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lbx_register = new System.Windows.Forms.Label();
            this.btn_cancel = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.lbx_username)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbx_pwd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cb_memberpwd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_login)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_cancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // lbx_username
            // 
            this.lbx_username.BackColor = System.Drawing.Color.Transparent;
            this.lbx_username.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbx_username.ForeColor = System.Drawing.Color.White;
            this.lbx_username.Location = new System.Drawing.Point(39, 155);
            this.lbx_username.Name = "lbx_username";
            this.lbx_username.Size = new System.Drawing.Size(56, 24);
            this.lbx_username.TabIndex = 14;
            this.lbx_username.Text = "用户名";
            // 
            // tbx_username
            // 
            this.tbx_username.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbx_username.FormattingEnabled = true;
            this.tbx_username.Location = new System.Drawing.Point(110, 153);
            this.tbx_username.Name = "tbx_username";
            this.tbx_username.Size = new System.Drawing.Size(259, 29);
            this.tbx_username.TabIndex = 21;
            // 
            // lbx_pwd
            // 
            this.lbx_pwd.BackColor = System.Drawing.Color.Transparent;
            this.lbx_pwd.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbx_pwd.ForeColor = System.Drawing.Color.White;
            this.lbx_pwd.Location = new System.Drawing.Point(39, 206);
            this.lbx_pwd.Name = "lbx_pwd";
            this.lbx_pwd.Size = new System.Drawing.Size(53, 24);
            this.lbx_pwd.TabIndex = 15;
            this.lbx_pwd.Text = "密   码";
            // 
            // tbx_pwd
            // 
            this.tbx_pwd.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbx_pwd.Location = new System.Drawing.Point(110, 206);
            this.tbx_pwd.Name = "tbx_pwd";
            this.tbx_pwd.PasswordChar = '*';
            this.tbx_pwd.Size = new System.Drawing.Size(259, 29);
            this.tbx_pwd.TabIndex = 20;
            // 
            // cb_memberpwd
            // 
            this.cb_memberpwd.BackColor = System.Drawing.Color.Transparent;
            this.cb_memberpwd.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_memberpwd.ForeColor = System.Drawing.Color.White;
            this.cb_memberpwd.Location = new System.Drawing.Point(110, 248);
            this.cb_memberpwd.Name = "cb_memberpwd";
            this.cb_memberpwd.Size = new System.Drawing.Size(77, 21);
            this.cb_memberpwd.TabIndex = 16;
            this.cb_memberpwd.Text = "记住密码";
            // 
            // lbx_ToFindPwd
            // 
            this.lbx_ToFindPwd.AutoSize = true;
            this.lbx_ToFindPwd.BackColor = System.Drawing.Color.Transparent;
            this.lbx_ToFindPwd.ForeColor = System.Drawing.Color.GhostWhite;
            this.lbx_ToFindPwd.LinkColor = System.Drawing.Color.Cyan;
            this.lbx_ToFindPwd.Location = new System.Drawing.Point(284, 246);
            this.lbx_ToFindPwd.Name = "lbx_ToFindPwd";
            this.lbx_ToFindPwd.Size = new System.Drawing.Size(85, 21);
            this.lbx_ToFindPwd.TabIndex = 17;
            this.lbx_ToFindPwd.TabStop = true;
            this.lbx_ToFindPwd.Text = "忘记密码?";
            this.lbx_ToFindPwd.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Lbx_ToFindPwd_LinkClicked);
            // 
            // btn_login
            // 
            this.btn_login.BackColor = System.Drawing.Color.MediumSlateBlue;
            this.btn_login.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_login.ForeColor = System.Drawing.Color.White;
            this.btn_login.Location = new System.Drawing.Point(110, 313);
            this.btn_login.Name = "btn_login";
            // 
            // 
            // 
            this.btn_login.RootElement.ApplyShapeToControl = true;
            this.btn_login.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren;
            this.btn_login.RootElement.CanFocus = true;
            this.btn_login.RootElement.CustomFont = "TelerikWebUI";
            this.btn_login.Size = new System.Drawing.Size(103, 38);
            this.btn_login.TabIndex = 18;
            this.btn_login.Text = "登  录";
            this.btn_login.Click += new System.EventHandler(this.Btn_login_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBox1.Image = global::MesManager.Properties.Resources.万通公司LOG426x90;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(427, 90);
            this.pictureBox1.TabIndex = 23;
            this.pictureBox1.TabStop = false;
            // 
            // lbx_register
            // 
            this.lbx_register.AutoSize = true;
            this.lbx_register.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbx_register.ForeColor = System.Drawing.Color.White;
            this.lbx_register.Location = new System.Drawing.Point(12, 365);
            this.lbx_register.Name = "lbx_register";
            this.lbx_register.Size = new System.Drawing.Size(37, 19);
            this.lbx_register.TabIndex = 24;
            this.lbx_register.Text = "注册";
            this.lbx_register.Click += new System.EventHandler(this.Lbx_register_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.BackColor = System.Drawing.Color.MediumSlateBlue;
            this.btn_cancel.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_cancel.ForeColor = System.Drawing.Color.White;
            this.btn_cancel.Location = new System.Drawing.Point(266, 313);
            this.btn_cancel.Name = "btn_cancel";
            // 
            // 
            // 
            this.btn_cancel.RootElement.ApplyShapeToControl = true;
            this.btn_cancel.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren;
            this.btn_cancel.RootElement.CanFocus = true;
            this.btn_cancel.RootElement.CustomFont = "TelerikWebUI";
            this.btn_cancel.Size = new System.Drawing.Size(103, 38);
            this.btn_cancel.TabIndex = 25;
            this.btn_cancel.Text = "取  消";
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(427, 407);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.lbx_register);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lbx_username);
            this.Controls.Add(this.tbx_username);
            this.Controls.Add(this.lbx_pwd);
            this.Controls.Add(this.tbx_pwd);
            this.Controls.Add(this.cb_memberpwd);
            this.Controls.Add(this.lbx_ToFindPwd);
            this.Controls.Add(this.btn_login);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Login";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "登录";
            this.Load += new System.EventHandler(this.Login_Load);
            ((System.ComponentModel.ISupportInitialize)(this.lbx_username)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lbx_pwd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cb_memberpwd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_login)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_cancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadLabel lbx_username;
        private ComboBox tbx_username;
        private Telerik.WinControls.UI.RadLabel lbx_pwd;
        private TextBox tbx_pwd;
        private Telerik.WinControls.UI.RadCheckBox cb_memberpwd;
        private LinkLabel lbx_ToFindPwd;
        private Telerik.WinControls.UI.RadButton btn_login;
        private PictureBox pictureBox1;
        private Label lbx_register;
        private Telerik.WinControls.UI.RadButton btn_cancel;
    }
}
