using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using CommonUtils.Logger;

namespace MesManager.UI
{
    public partial class MESMainForm : RadForm
    {
        private RadTitleBarElement titleBar;
        private bool isFormMoving = false;
        private static bool IsLogin;
        public static string currentUser;
        public static int currentUsetType;
        private MesService.MesServiceClient serviceClient;
        private bool IsFirstState = true;
        public MESMainForm()
        {
            InitializeComponent();
            //int left = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            //int top = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            //int right = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            //this.toolsGroup.Margin = new System.Windows.Forms.Padding(left / 3, top / 4, right / 4, 0);
            //this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
        }
        private void MESMainForm_Load(object sender, EventArgs e)
        {
            //this.pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            serviceClient = new MesService.MesServiceClient();
            TestCommunication();
            IsFirstState = false;
        }

        public void InitMain()
        {
            PrepareTitleBar();
            EventHandlers();
            System.Threading.Thread.Sleep(1000);
        }

        private void TestCommunication()
        {
            try
            {
                var inputStr = "TEST";
                var tRes = serviceClient.TestCommunication(inputStr);
                if (tRes != inputStr)
                {
                    //异常
                    this.ledControl1.LEDCircleColor = Color.Gray;
                    this.ledControl1.LEDCenterColor = Color.Gray;
                    this.ledControl1.LEDSurroundColor = Color.Gray;
                    //this.ledControl1.LEDSwitch = false;
                    this.ledControl1.Invalidate();
                    LogHelper.Log.Error(tRes);
                    MessageBox.Show("与服务器通讯失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                this.ledControl1.LEDCircleColor = Color.LimeGreen;
                this.ledControl1.LEDCenterColor = Color.LimeGreen;
                this.ledControl1.LEDSurroundColor = Color.LimeGreen;
                //this.ledControl1.LEDSwitch = true;
                this.ledControl1.Invalidate();
                if (IsFirstState)
                    return;
                MessageBox.Show("与服务器通讯正常！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error(ex.Message);
                MessageBox.Show(ex.Message, "ERR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EventHandlers()
        {
            this.mainBasicInfo.Click += MainBasicInfo_Click;
            //this.mainCheckStation.Click += MainCheckStation_Click;
            //this.mainGraphView.Click += MainGraphView_Click;
            this.mainMaterialManager.Click += MainMaterialManager_Click;
            //this.mainProduceManager.Click += MainProduceManager_Click;
            //this.mainProductCheck.Click += MainProductCheck_Click;
            this.mainProcess.Click += MainProcess_Click;
            this.mainQuanlityAnomaly.Click += MainQuanlityAnomaly_Click;
            //this.mainRepairCenter.Click += MainRepairCenter_Click;
            this.mainReportData.Click += MainReportData_Click;
            this.mainTestStandData.Click += MainTestStandData_Click;
            //this.mainStatisticalAnalysis.Click += MainStatisticalAnalysis_Click;
            this.btn_user_manger.Click += Btn_user_manger_Click;
            this.btn_user_login.Click += Btn_user_login_Click;
            this.ledControl1.Click += LedControl1_Click;
        }

        private void LedControl1_Click(object sender, EventArgs e)
        {
            TestCommunication();
        }

        private void PrepareTitleBar()
        {
            titleBar = new RadTitleBarElement();
            titleBar.Text = "万通智控产线追溯MES系统";
            titleBar.ForeColor = Color.White;
            titleBar.Font = new Font("Segoe UI Light", 21.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
            titleBar.FillPrimitive.Visibility = ElementVisibility.Hidden;
            titleBar.MaxSize = new Size(0, 50);
            titleBar.Children[1].Visibility = ElementVisibility.Hidden;

            titleBar.CloseButton.Parent.PositionOffset = new SizeF(0, 10);//与top距离
            titleBar.CloseButton.MinSize = new Size(50, 20);
            titleBar.CloseButton.ButtonFillElement.Visibility = ElementVisibility.Collapsed;

            titleBar.MinimizeButton.MinSize = new Size(50, 50);
            titleBar.MinimizeButton.ButtonFillElement.Visibility = ElementVisibility.Collapsed;

            titleBar.MaximizeButton.MinSize = new Size(50, 50);
            titleBar.MaximizeButton.ButtonFillElement.Visibility = ElementVisibility.Collapsed;

            titleBar.CloseButton.SetValue(RadFormElement.IsFormActiveProperty, true);
            titleBar.MinimizeButton.SetValue(RadFormElement.IsFormActiveProperty, true);
            titleBar.MaximizeButton.SetValue(RadFormElement.IsFormActiveProperty, true);

            titleBar.Close += new TitleBarSystemEventHandler(titleBar_Close);
            titleBar.Minimize += new TitleBarSystemEventHandler(titleBar_Minimize);
            titleBar.MaximizeRestore += new TitleBarSystemEventHandler(titleBar_MaximizeRestore);
            this.radPanorama1.PanoramaElement.PanGesture += new PanGestureEventHandler(radTilePanel1_PanGesture);
            this.radPanorama1.PanoramaElement.Children.Add(titleBar);
        }

        void radTilePanel1_PanGesture(object sender, PanGestureEventArgs e)
        {
            if (e.IsBegin && this.titleBar.ControlBoundingRectangle.Contains(e.Location))
            {
                isFormMoving = true;
            }

            if (isFormMoving)
            {
                this.Location = new Point(this.Location.X + e.Offset.Width, this.Location.Y + e.Offset.Height);
            }
            else
            {
                e.Handled = false;
            }

            if (e.IsEnd)
            {
                isFormMoving = false;
            }
        }

        #region Event Handlers
        private void Btn_user_login_Click(object sender, EventArgs e)
        {
            OpenLogin();
        }

        private bool OpenLogin()
        {
            Login login = new Login();
            login.ShowDialog();
            if (Login.loginResult == Login.LoginResult.STATUS_OK)
            {
                IsLogin = true;
                currentUser = Login.GetUserName;
                currentUsetType = Login.CurrentUserType;
                return true;
            }
            else if (Login.loginResult == Login.LoginResult.ERROR_PASSWORD)
            {
                IsLogin = false;
                return false;
            }
            else if (Login.loginResult == Login.LoginResult.ERROR_USER_NAME)
            {
                IsLogin = false;
                return false;
            }
            IsLogin = false;
            return false;
        }

        private void Btn_user_manger_Click(object sender, EventArgs e)
        {
            //用户管理
            if (!IsLoginAuthon())
                return;
            if (currentUsetType != 0)
            {
                MessageBox.Show("您没有此操作权限！","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            UserManager userManager = new UserManager();
            userManager.ShowDialog();
        }

        private void MainStatisticalAnalysis_Click(object sender, EventArgs e)
        {
            if (!IsLoginAuthon())
                return;
            StatisticalAnalysis statisticalAnalysis = new StatisticalAnalysis();
            statisticalAnalysis.ShowDialog();
        }

        private void MainTestStandData_Click(object sender, EventArgs e)
        {
            if (!IsLoginAuthon())
                return;
            TestStand testStand = new TestStand();
            testStand.ShowDialog();
        }
        private void MainReportData_Click(object sender, EventArgs e)
        {
            //报表中心/追溯中心
            if (!IsLoginAuthon())
                return;
            SNCenter sNCenter = new SNCenter();
            sNCenter.ShowDialog();
        }

        private void MainRepairCenter_Click(object sender, EventArgs e)
        {
            //维修中心
            if (!IsLoginAuthon())
                return;
            RepairCenter repairCenter = new RepairCenter();
            repairCenter.ShowDialog();
        }

        private void MainQuanlityAnomaly_Click(object sender, EventArgs e)
        {
            //品质异常管理
            if (!IsLoginAuthon())
                return;
            QuanlityAnomaly quanlityAnomaly = new QuanlityAnomaly();
            quanlityAnomaly.ShowDialog();
        }

        private void MainProcess_Click(object sender, EventArgs e)
        {
            //工艺流程
            if(!IsLoginAuthon())
                return;
            TProcess process = new TProcess();
            process.ShowDialog();
        }

        private void MainProductCheck_Click(object sender, EventArgs e)
        {
            //成品抽检
            
        }

        private void MainProduceManager_Click(object sender, EventArgs e)
        {
            //生产管理
            //建立工艺生产线
        }

        private void MainMaterialManager_Click(object sender, EventArgs e)
        {
            //物料绑定
            if (!IsLoginAuthon())
                return;
            ProductMaterial productMaterial = new ProductMaterial();
            productMaterial.ShowDialog();
        }

        private void MainGraphView_Click(object sender, EventArgs e)
        {
            //看板中心
            //图形显示
            if (!IsLoginAuthon())
                return;
            GraphView graphView = new GraphView();
            graphView.ShowDialog();
        }

        private void MainCheckStation_Click(object sender, EventArgs e)
        {
            //检验过站
            
        }

        private void MainBasicInfo_Click(object sender, EventArgs e)
        {
            //基础信息/基础配置
            //配置所有基本的信息界面
            //如配置型号/配置产线/配置物料信息等
            if(!IsLoginAuthon())
                return;
            BasicConfig basicConfig = new BasicConfig();
            basicConfig.ShowDialog();
        }

        private bool IsLoginAuthon()
        {
            if (!IsLogin)
            {
                if (MessageBox.Show("请先登录！立即打开登录界面？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    //打开登录
                    if(OpenLogin())
                        return true;
                    return false;
                }
            }
            return true;
        }

        void titleBar_MaximizeRestore(object sender, EventArgs args)
        {
            int left = Screen.PrimaryScreen.Bounds.Width;
            int top = Screen.PrimaryScreen.Bounds.Height;
            int right = Screen.PrimaryScreen.Bounds.Width;
            if (this.WindowState != FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Maximized;
                this.toolsGroup.Margin = new System.Windows.Forms.Padding(left / 3, top / 4, right / 4, 0);
                this.StartPosition = FormStartPosition.CenterScreen;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                //this.toolsGroup.Margin = new System.Windows.Forms.Padding(200, 130, 200, 0);
                this.toolsGroup.Margin = new System.Windows.Forms.Padding(400, 200, 200, 0);
                this.StartPosition = FormStartPosition.CenterScreen;
            }
        }

        void titleBar_Minimize(object sender, EventArgs args)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        void titleBar_Close(object sender, EventArgs args)
        {
            Application.Exit();
        }

        #endregion
    }
}
