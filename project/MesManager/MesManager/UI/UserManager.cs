using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using MesManager.Control;

namespace MesManager.UI
{
    public partial class UserManager : RadForm
    {
        private MesService.MesServiceClient serviceClient;
        private DataTable dataSource;
        private const string USER_ID = "序号";
        private const string USER_NAME = "用户名";
        private const string USER_ROLE = "角色";
        //private const string USER_STATUS = "状态";
        private const string UPDATE_DATE = "更新日期";
        public UserManager()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
        }

        private void InitDataTable()
        {
            if (dataSource == null)
            {
                dataSource = new DataTable();
                dataSource.Columns.Add(USER_ID);
                dataSource.Columns.Add(USER_NAME);
                dataSource.Columns.Add(USER_ROLE);
                dataSource.Columns.Add(UPDATE_DATE);
            }
        }

        private void Menu_refresh_Click(object sender, EventArgs e)
        {
            SelectAllUser();
        }

        async private void SelectAllUser()
        {
            this.dataSource.Clear();

            var dt = (await serviceClient.GetAllUserInfoAsync()).Tables[0];
            if (dt.Rows.Count < 1)
                return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var username = dt.Rows[i][0].ToString();
                var userrole = dt.Rows[i][1].ToString();
                var updateDate = dt.Rows[i][3].ToString();
                if (userrole == "0")
                    userrole = "管理员";
                else if (userrole == "1")
                    userrole = "班组长";
                else if (userrole == "2")
                    userrole = "操作员";
                else if (userrole == "3")
                    userrole = "工人";
                DataRow dr = dataSource.NewRow();
                dr[USER_ID] = i + 1;
                dr[USER_NAME] = username;
                dr[USER_ROLE] = userrole;
                dr[UPDATE_DATE] = updateDate;
                this.dataSource.Rows.Add(dr);
            }

            this.radGridView1.DataSource = this.dataSource;
        }

        private void UserManager_Load(object sender, EventArgs e)
        {
            serviceClient = new MesService.MesServiceClient();
            InitDataTable();
            DataGridViewCommon.SetRadGridViewProperty(this.radGridView1,false);
            this.radGridView1.ReadOnly = true;
            SelectAllUser();
        }

        private void Menu_add_Click(object sender, EventArgs e)
        {
            Register register = new Register("添加新用户");
            register.ShowDialog();
        }

        private void Menu_del_Click(object sender, EventArgs e)
        {
            var username = this.radGridView1.CurrentRow.Cells[1].Value;
            if (username == null)
            {
                MessageBox.Show("请选择要删除的用户！","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            DeleteUser(username.ToString());
        }

        async private void DeleteUser(string username)
        {
            var dt = serviceClient.GetUserInfo(username).Tables[0];
            if (dt.Rows.Count < 1)
                return;
            var roleID = dt.Rows[0][0].ToString();
            if (roleID == "0")//admin
                return;
            if (MessageBox.Show($"确认要删除用户【{username}】?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
            {
                return;
            }
            int delV =  await serviceClient.DeleteUserAsync(username);
            if (delV == 1)
            {
                SelectAllUser();
                MessageBox.Show("删除成功！","提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
                return;
            }
            MessageBox.Show("删除失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Menu_commit_Click(object sender, EventArgs e)
        {
            var username = this.radGridView1.CurrentRow.Cells[1].Value;
            if (username == null)
            {
                MessageBox.Show("请选择要修改的用户！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ModifyPwd modifyPwd = new ModifyPwd(username.ToString());
            modifyPwd.ShowDialog();
        }
    }
}
