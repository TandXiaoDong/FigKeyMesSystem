using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using CommonUtils.Logger;
using CommonUtils.FileHelper;
using System.Web;
using System.Net;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using MesManager.RadView;
using Telerik.WinControls.UI;
using MesManager.UI;

namespace MesManager
{
    public partial class Login : RadForm
    {
        private const string USER_ADMIN = "管理员";
        private const string USER_ORDINARY = "普通用户";
        private const string INI_CONFIG_NAME = "userConfig.ini";
        private const string INI_CONFIG_SECTION = "usercfg";
        private const string INI_CONFIG_USER = "username";
        private const string INI_CONFIG_PWD = "password";
        private const string INI_CONFIG_REMBER = "remberpwd";
        private string configPath;
        private MesService.MesServiceClient mesService;
        private bool isFormMoving = false;
        public static int CurrentUserType;
        public static string GetUserName;
        public static LoginResult loginResult;
        public static bool IsCloseFormState;

        [DllImport("user32.dll", EntryPoint = "HideCaret")]
        private static extern bool HideCaret(IntPtr hWnd);
        public Login()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        public enum LoginResult
        {
            STATUS_OK,
            ERROR_USER_NAME,
            ERROR_PASSWORD,
            STATUS_CANCEL_LOGIN,
            STATUS_CLOSE_FORM,
            ERROR_EXCEPT
        }

        private void DelegateAction(Action action)
        {
            if (InvokeRequired)
            {
                Invoke(action);
            }
            else
            {
                action();
            }
        }

        private void Timer()
        {
            var timer = new System.Timers.Timer(1000);
            timer.Elapsed += (s, x) =>
            {
                DelegateAction(() =>
                {
                    //
                });
            };
            timer.Enabled = true;
            timer.Start();
        }

        public enum UserType
        {
            /// <summary>
            /// 管理员
            /// </summary>
            USER_ADMIN,
            /// <summary>
            /// 班组长
            /// </summary>
            USER_TEAM_LEADER,
            /// <summary>
            /// 操作员
            /// </summary>
            USER_OPERATOR,
            /// <summary>
            /// 普通工人
            /// </summary>
            USER_ORIDNARY
        }

        private void Login_Load(object sender, EventArgs e)
        {
            Init();
            InitServiceInstance();
            tbx_username.KeyDown += Tbx_username_KeyDown;
            this.btn_cancel.Click += Btn_cancel_Click;
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            loginResult = LoginResult.STATUS_CLOSE_FORM;
            this.Close();
        }

        private void Tbx_username_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                tbx_pwd.Focus();
            }
        }

        private void InitServiceInstance()
        {
            try
            {
                mesService = new MesService.MesServiceClient();
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接服务异常", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LogHelper.Log.Error("获取服务异常！" + ex.Message);
            }
        }

        #region 本地验证
        /// <summary>
        /// 本地验证用户角色 用户名和密码
        /// </summary>
        /// <returns></returns>
        private bool LocalValidate()
        {
            //if (string.IsNullOrEmpty(cob_userType.Text))
            //{
            //    cob_userType.ForeColor = Color.Red;
            //    cob_userType.Focus();
            //    return false;
            //}
            //cob_userType.ForeColor = Color.Black;
            //if (!cob_userType.Text.Equals(USER_ADMIN) && !cob_userType.Text.Trim().Equals(USER_ORDINARY))
            //{
            //    cob_userType.ForeColor = Color.Red;
            //    cob_userType.Focus();
            //    return false;
            //}
            //cob_userType.ForeColor = Color.Black;
            if (string.IsNullOrEmpty(tbx_username.Text))
            {
                tbx_username.ForeColor = Color.Red;
                tbx_username.Focus();
                return false;
            }
            tbx_username.ForeColor = Color.Black;
            if (string.IsNullOrEmpty(tbx_pwd.Text))
            {
                tbx_pwd.ForeColor = Color.Red;
                tbx_pwd.Focus();
                return false;
            }
            return true;
        }
        #endregion

        #region 接口验证用户名 密码
        /// <summary>
        /// 调用接口验证用户名和密码
        /// </summary>
        /// <param name="loginUser"></param>
        async private void RemoteValidate()
        {
            try
            {
                MesService.LoginResult loginRes = await mesService.LoginAsync(tbx_username.Text, tbx_pwd.Text);
                //验证用户密码
                switch (loginRes)
                {
                    case MesService.LoginResult.SUCCESS:
                        LogHelper.Log.Info("登录验证成功！");
                        //连接云服务
                        if (!ConnectCloudService())
                        {
                            MessageBox.Show("连接服务失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        LogHelper.Log.Info("连接服务成功！");
                        //启动主界面
                        this.DialogResult = DialogResult.OK;
                        loginResult = LoginResult.STATUS_OK;
                        this.Close();
                        break;
                    case MesService.LoginResult.FAIL_EXCEP:
                        LogHelper.Log.Info("登录失败!");
                        loginResult = LoginResult.ERROR_EXCEPT;
                        break;
                    case MesService.LoginResult.USER_NAME_ERR:
                        //该用户不存在
                        tbx_username.ForeColor = Color.Red;
                        tbx_username.Focus();
                        loginResult = LoginResult.ERROR_USER_NAME;
                        break;
                    case MesService.LoginResult.USER_PWD_ERR:
                        tbx_pwd.ForeColor = Color.Red;
                        tbx_pwd.Focus();
                        loginResult = LoginResult.ERROR_PASSWORD;
                        return;
                    default:
                        break;
                }
                tbx_pwd.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error(ex.Message);
                MessageBox.Show(ex.Message,"Err");
            }
        }
        #endregion

        private bool ConnectCloudService()
        {
            //SuperEasyClient.ConnectServer();
            //SuperEasyClient.btnLogin("this is client");
            //btnDecrypt

            return true;
        }

        private bool TestCommunication()
        {
            //检测通讯
            var testValue = "test";
            var rebackValue = mesService.TestCommunication(testValue);
            if (rebackValue != testValue)
            {
                //通讯异常
                MessageBox.Show(rebackValue, "ERR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        async private void Init()
        {
            mesService = new MesService.MesServiceClient();
            if (!TestCommunication())
                return;
            //设置单行
            //tbx_username.Multiline = false;
            tbx_pwd.Multiline = false;
            tbx_username.Items.Clear();
            DataSet ds = await mesService.GetAllUserInfoAsync();
            if (ds == null)
            {
                MessageBox.Show("连接数据库服务异常！","ERR",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            var dt = ds.Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                tbx_username.Items.Add(dt.Rows[i][0].ToString());
            }
            tbx_username.Text = "Admin";
            configPath = AppDomain.CurrentDomain.BaseDirectory+INI_CONFIG_NAME;
            ReadLastCfg();
        }
        private void ReadLastCfg()
        {
            try
            {
                var checkState = INIFile.GetValue(INI_CONFIG_SECTION,INI_CONFIG_REMBER,configPath);
                if (!string.IsNullOrEmpty(checkState))
                {
                    var curCbxState = (CheckState)Enum.Parse(typeof(CheckState),checkState);
                    cb_memberpwd.CheckState = curCbxState;
                    if (curCbxState == CheckState.Checked)
                    {
                        tbx_username.Text = INIFile.GetValue(INI_CONFIG_SECTION, INI_CONFIG_USER, configPath);
                        tbx_pwd.Text = INIFile.GetValue(INI_CONFIG_SECTION, INI_CONFIG_PWD, configPath);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error(ex.Message);
            }
        }

        private void UpdateUserCfg()
        {
            try
            {
                INIFile.SetValue(INI_CONFIG_SECTION,INI_CONFIG_REMBER,cb_memberpwd.CheckState+"",configPath);
                INIFile.SetValue(INI_CONFIG_SECTION,INI_CONFIG_USER,tbx_username.Text,configPath);
                INIFile.SetValue(INI_CONFIG_SECTION,INI_CONFIG_PWD,tbx_pwd.Text,configPath);
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error(ex.Message);
            }
        }

        private void Lbx_regist_Click(object sender, EventArgs e)
        {
            Register register = new Register("注册");
            register.ShowDialog();
        }

        private void Btn_login_Click(object sender, EventArgs e)
        {
            //登录验证:用户角色+用户名+用户密码
            //登录设置(保存本地配置)：记住密码+自动登录
            //连接远程服务

            //if (cob_userType.SelectedIndex == (int)UserType.USER_ADMIN)
            //{
            //    //管理员登录
            //    if (!LocalValidate())
            //        return;
            //    RemoteValidate(MesService.LoginUser.ADMIN_USER);
            //    GetUserType = UserType.USER_ADMIN;
            //}
            //else if (cob_userType.SelectedIndex == (int)UserType.USER_ORDINARY)
            //{
            //    //普通用户登录
            //    if (!LocalValidate())
            //        return;
            //    RemoteValidate(MesService.LoginUser.ORDINARY_USER);
            //    GetUserType = UserType.USER_ORDINARY;
            //}
            if (!TestCommunication())
                return;
            if (!LocalValidate())
                return;
            RemoteValidate();
            GetUserName = tbx_username.Text;
            SelectUserType();

            UpdateUserCfg();
            if (this.DialogResult == DialogResult.OK)
            {
                
                this.Close();
            }
        }
        private void SelectUserType()
        {
            var dt = mesService.GetUserInfo(tbx_username.Text).Tables[0];
            if (dt.Rows.Count < 1)
                return;
            CurrentUserType = int.Parse(dt.Rows[0][0].ToString()); 
        }

        private void Lbx_ToFindPwd_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            GetBackPwd getBackPwd = new GetBackPwd(this.tbx_username.Text);
            getBackPwd.ShowDialog();
        }

        private void Lbx_register_Click(object sender, EventArgs e)
        {
            Register register = new Register("注册");
            register.ShowDialog();
        }
    }
}
