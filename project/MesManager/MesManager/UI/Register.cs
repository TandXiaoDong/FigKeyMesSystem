using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using System.Text.RegularExpressions;
using CommonUtils.CalculateAndString;

namespace MesManager.UI
{
    public partial class Register : Telerik.WinControls.UI.RadForm
    {
        private MesService.MesServiceClient serviceClient;
        public Register(string formText)
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = formText;
        }

        private void Register_Load(object sender, EventArgs e)
        {
            serviceClient = new MesService.MesServiceClient();
            tb_pwd.PasswordChar = '*';
            tb_repwd.PasswordChar = '*';
        }

        private void Btn_register_Click(object sender, EventArgs e)
        {
            RegisterUser();
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        async private void RegisterUser()
        {
            var username = this.tb_username.Text.Trim();
            var userpwd = this.tb_pwd.Text.Trim();
            var userRpwd = this.tb_repwd.Text.Trim();
            var userType = this.cb_userType.Text.Trim();

            if (string.IsNullOrEmpty(userType))
            {
                MessageBox.Show("用户类型不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //if (this.cb_userType.SelectedIndex == 0)//班组长
            //    userType = "1";
            //else if (this.cb_userType.SelectedIndex == 1)//操作员
            //    userType = "2";
            //else if (this.cb_userType.SelectedIndex == 2)
            //    userType = "3";
            if (this.cb_userType.SelectedIndex == 0)//操作员
                userType = "2";

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("用户名不能为空！","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(userpwd))
            {
                MessageBox.Show("密码不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(userRpwd))
            {
                MessageBox.Show("确认密码不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            DataSet dataSet = serviceClient.GetUserInfo(username);
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                MessageBox.Show("用户已存在！","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                tb_username.ForeColor = Color.Red;
                return;
            }
            tb_username.ForeColor = Color.Black;
            if (userpwd != userRpwd)
            {
                MessageBox.Show("两次密码不一致？","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            //校验密码复杂度
            if (!RegexHelper.IsMatchPassword(userpwd))
            {
                //密码复杂度不满足
                MessageBox.Show("密码必须包含数字、字母", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MesService.RegisterResult registerResult = await serviceClient.RegisterAsync(username,userpwd,"","",int.Parse(userType));
            if (registerResult == MesService.RegisterResult.REGISTER_SUCCESS)
            {
                //注册成功
                MessageBox.Show("注册成功！","提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }
    }
}
