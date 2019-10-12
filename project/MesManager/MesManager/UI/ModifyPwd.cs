using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;

namespace MesManager.UI
{
    public partial class ModifyPwd : Telerik.WinControls.UI.RadForm
    {
        private MesService.MesServiceClient serviceClient;
        public ModifyPwd(string username)
        {
            InitializeComponent();
            this.tb_user.Text = username;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
        }

        private void Btn_modify_Click(object sender, EventArgs e)
        {
            CommitUserModify();
        }

        async private void CommitUserModify()
        {
            var username = this.tb_user.Text.Trim();
            var oldPwd = this.tb_oldPwd.Text.Trim();
            var newPwd = this.tb_newPwd.Text.Trim();
            var confirmPwd = this.tb_confirmPwd.Text.Trim();
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("用户名称不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(oldPwd))
            {
                MessageBox.Show("旧密码不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(newPwd))
            {
                MessageBox.Show("新密码不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(confirmPwd))
            {
                MessageBox.Show("确认密码不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var dt = (await serviceClient.GetUserInfoAsync(username)).Tables[0];
            if (dt.Rows.Count < 1)
            {
                MessageBox.Show("用户名不存在！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //验证旧密码
            MesService.LoginResult loginResult = await serviceClient.LoginAsync(username,oldPwd);
            if (loginResult != MesService.LoginResult.SUCCESS)
            {
                MessageBox.Show("旧密码错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (newPwd != confirmPwd)
            {
                MessageBox.Show("新密码与确认密码不一致！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var res = await serviceClient.ModifyUserPasswordAsync(username, confirmPwd);
            if (res == 1)
            {
                MessageBox.Show("修改成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("修改失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ModifyPwd_Load(object sender, EventArgs e)
        {
            serviceClient = new MesService.MesServiceClient();
            this.tb_newPwd.PasswordChar = '*';
            this.tb_confirmPwd.PasswordChar = '*';
        }
    }
}
