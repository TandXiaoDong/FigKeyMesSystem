using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.Docking;
using CommonUtils.Logger;
using System.Web;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using MesManager.UI;
using MesManager.Control;
using Sunisoft.IrisSkin;
using Telerik.WinControls.Themes;
using System.Threading.Tasks;

namespace MesManager
{
    public partial class MainForm : RadForm
    {
        private MesService.MesServiceClient serviceClient;
        public MainForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            Telerik.WinControls.ThemeResolutionService.ApplicationThemeName = this.ThemeName;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Login login = new Login();
            login.ShowDialog();
            if (login.DialogResult != DialogResult.OK)
            {
                //登录失败
                this.Close();
            }
        }

        async private void InitServiceInstance()
        {
            try
            {
                serviceClient = new MesService.MesServiceClient();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void InitMainForm()
        {
            InitServiceInstance();
            InitListView();
            LoadTreeView();
            InitControl();
            this.radDock1.RemoveAllDocumentWindows();
            this.radDock1.AddDocument(documentWindow_testRes);
            //this.radDock1.AddDocument(documentWindow_material_select);
            //this.radDock1.AddDocument(documentWindow_packageProduct);
            //this.radDock1.AddDocument(documentWindow_passRes);

            //if (Login.GetUserType == Login.UserType.USER_ADMIN)
            //{
            //    tool_status_user.Text = "管理员";

            //}
            //else if (Login.GetUserType == Login.UserType.USER_ORDINARY)
            //{
            //    tool_status_user.Text = "普通用户";
            //}
            tool_status_user.Text = Login.GetUserName;
            ControlEvent();
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            MessageBox.Show("task Exception "+e.Exception);
        }

        private void ControlEvent()
        {
            //menu
            menu_set_station.Click += Menu_set_station_Click;
            menu_produce_config.Click += Menu_produce_config_Click;
            menu_productType.Click += Menu_productType_Click;
            menu_select_testRes.Click += Menu_typeno_Click;
            menu_material_msg.Click += Menu_material_msg_Click;
            menu_product_binding.Click += Menu_product_binding_Click;
            menu_product_material.Click += Menu_product_material_Click;
            menu_select_testRes.Click += Menu_select_testRes_Click;
            menu_select_material.Click += Menu_select_material_Click;
            menu_select_packageProduct.Click += Menu_select_packageProduct_Click;
            menu_select_passRate.Click += Menu_select_passRate_Click;

            //button
            btn_search_record.Click += Btn_search_record_Click;
            btn_search_lastTestRes.Click += Btn_search_LastTestRes_Click;
            btn_selectMaterial.Click += Btn_selectMaterial_Click;
            btn_select_package.Click += Btn_select_package_Click;

            //style
            menu_vs2012dark.Click += Menu_vs2012dark_Click;
            menu_vs2012Light.Click += Menu_vs2012Light_Click;
            menu_material.Click += Menu_material_Click;
            menu_materialBlue.Click += Menu_materialBlue_Click;
            menu_materialPink.Click += Menu_materialPink_Click;//保留，控件变化大
            menu_materialTeal.Click += Menu_materialTeal_Click;//保留，需要做一些优化
            menu_telerikMetro.Click += Menu_telerikMetro_Click;
            menu_telerikMetroBlue.Click += Menu_telerikMetroBlue_Click;
            menu_telerikMetroTouch.Click += Menu_telerikMetroTouch_Click;
            menu_office2007Black.Click += Menu_office2007Black_Click;
            menu_office2007Silver.Click += Menu_office2007Silver_Click;
            menu_office2010Black.Click += Menu_office2010Black_Click;
            menu_office2010Blue.Click += Menu_office2010Blue_Click;
            menu_office2010Silver.Click += Menu_office2010Silver_Click;
            menu_office2013Dark.Click += Menu_office2013Dark_Click;
            menu_Office2013Light.Click += Menu_Office2013Light_Click;
            menu_windows7.Click += Menu_windows7_Click;
            menu_windows8.Click += Menu_windows8_Click;
        }

        async private void Btn_select_package_Click(object sender, EventArgs e)
        {
            //MesService.PackageProduct packageProduct = new MesService.PackageProduct();
            if (rb_package_caseCode.CheckState == CheckState.Checked)
            {
                //packageProduct.CaseCode = tb_input_packageMsg.Text.Trim();
            }
            else if (rb_package_sn.CheckState == CheckState.Checked)
            {
                //packageProduct.SnOutter = tb_input_packageMsg.Text.Trim();
            }
            //this.radGridViewPackage.DataSource = (await serviceClient.SelectPackageProductAsync(packageProduct)).Tables[0];
        }

        async private void Btn_selectMaterial_Click(object sender, EventArgs e)
        {
            //根据产品型号查询物料信息
            var typeNo = cb_material_typeNo.Text.Trim();
            MesService.ProductMaterial productMaterial = new MesService.ProductMaterial();
            productMaterial.TypeNo = typeNo;
            //DataTable dt = (await serviceClient.SelectProductMaterialAsync(productMaterial)).Tables[0];
            //this.radGridViewMaterial.DataSource = dt;
        }

        #region UI style
        private void Menu_windows8_Click(object sender, EventArgs e)
        {
            Windows8Theme windows8Theme = new Windows8Theme();
            ThemeResolutionService.ApplicationThemeName = windows8Theme.ThemeName;
        }

        private void Menu_windows7_Click(object sender, EventArgs e)
        {
            Windows7Theme windows7Theme = new Windows7Theme();
            ThemeResolutionService.ApplicationThemeName = windows7Theme.ThemeName;
        }

        private void Menu_Office2013Light_Click(object sender, EventArgs e)
        {
            Office2013LightTheme office2013LightTheme = new Office2013LightTheme();
            ThemeResolutionService.ApplicationThemeName = office2013LightTheme.ThemeName;
        }

        private void Menu_office2013Dark_Click(object sender, EventArgs e)
        {
            Office2013DarkTheme office2013DarkTheme = new Office2013DarkTheme();
            ThemeResolutionService.ApplicationThemeName = office2013DarkTheme.ThemeName;
        }

        private void Menu_office2010Silver_Click(object sender, EventArgs e)
        {
            Office2010SilverTheme office2010SilverTheme = new Office2010SilverTheme();
            ThemeResolutionService.ApplicationThemeName = office2010SilverTheme.ThemeName;
        }

        private void Menu_office2010Blue_Click(object sender, EventArgs e)
        {
            Office2010BlueTheme office2010BlueTheme = new Office2010BlueTheme();
            ThemeResolutionService.ApplicationThemeName = office2010BlueTheme.ThemeName;
        }

        private void Menu_office2010Black_Click(object sender, EventArgs e)
        {
            Office2010BlackTheme office2010BlackTheme = new Office2010BlackTheme();
            ThemeResolutionService.ApplicationThemeName = office2010BlackTheme.ThemeName;
        }

        private void Menu_office2007Silver_Click(object sender, EventArgs e)
        {
            Office2007SilverTheme office2007SilverTheme = new Office2007SilverTheme();
            ThemeResolutionService.ApplicationThemeName = office2007SilverTheme.ThemeName;
        }

        private void Menu_office2007Black_Click(object sender, EventArgs e)
        {
            Office2007BlackTheme office2007BlackTheme = new Office2007BlackTheme();
            ThemeResolutionService.ApplicationThemeName = office2007BlackTheme.ThemeName;
        }

        private void Menu_telerikMetroTouch_Click(object sender, EventArgs e)
        {
            TelerikMetroTouchTheme telerikMetroTouchTheme = new TelerikMetroTouchTheme();
            ThemeResolutionService.ApplicationThemeName = telerikMetroTouchTheme.ThemeName;
        }

        private void Menu_telerikMetroBlue_Click(object sender, EventArgs e)
        {
            TelerikMetroBlueTheme telerikMetroBlueTheme = new TelerikMetroBlueTheme();
            ThemeResolutionService.ApplicationThemeName = telerikMetroBlueTheme.ThemeName;
        }

        private void Menu_telerikMetro_Click(object sender, EventArgs e)
        {
            TelerikMetroBlueTheme telerikMetroBlueTheme = new TelerikMetroBlueTheme();
            ThemeResolutionService.ApplicationThemeName = telerikMetroBlueTheme.ThemeName;
        }

        private void Menu_materialTeal_Click(object sender, EventArgs e)
        {
            MaterialTealTheme materialTealTheme = new MaterialTealTheme();
            ThemeResolutionService.ApplicationThemeName = materialTealTheme.ThemeName;
        }

        private void Menu_materialPink_Click(object sender, EventArgs e)
        {
            MaterialPinkTheme materialPinkTheme = new MaterialPinkTheme();
            ThemeResolutionService.ApplicationThemeName = materialPinkTheme.ThemeName;
        }

        private void Menu_materialBlue_Click(object sender, EventArgs e)
        {
            MaterialBlueGreyTheme materialBlueGreyTheme = new MaterialBlueGreyTheme();
            ThemeResolutionService.ApplicationThemeName = materialBlueGreyTheme.ThemeName;
        }

        private void Menu_material_Click(object sender, EventArgs e)
        {
            MaterialTheme materialTheme = new MaterialTheme();
            ThemeResolutionService.ApplicationThemeName = materialTheme.ThemeName;
        }

        private void Menu_vs2012Light_Click(object sender, EventArgs e)
        {
            VisualStudio2012LightTheme visualStudio2012LightTheme = new VisualStudio2012LightTheme();
            ThemeResolutionService.ApplicationThemeName = visualStudio2012LightTheme.ThemeName;
        }

        private void Menu_vs2012dark_Click(object sender, EventArgs e)
        {
            VisualStudio2012DarkTheme visualStudio2012DarkTheme = new VisualStudio2012DarkTheme();
            ThemeResolutionService.ApplicationThemeName = visualStudio2012DarkTheme.ThemeName;
        }
        #endregion
        private void InitStyle()
        {
            SkinEngine skinEngine = new SkinEngine();
            string path = AppDomain.CurrentDomain.BaseDirectory+ "Skins\\Silver.ssk";
            skinEngine.SkinFile = path;
            LogHelper.Log.Info(path);
            skinEngine.Active = true;
            skinEngine.SkinAllForm = true;
        }

        private void Menu_select_passRate_Click(object sender, EventArgs e)
        {
            //合格率
            this.radDock1.AddDocument(this.documentWindow_passRes);
        }

        private void Menu_select_packageProduct_Click(object sender, EventArgs e)
        {
            //产品打包
            this.radDock1.AddDocument(this.documentWindow_packageProduct);
        }

        private void Menu_select_material_Click(object sender, EventArgs e)
        {
            //物料统计
            this.radDock1.AddDocument(this.documentWindow_material_select);
        }

        private void Menu_select_testRes_Click(object sender, EventArgs e)
        {
            //测试结果
            this.radDock1.AddDocument(documentWindow_testRes);
        }

        private void Menu_product_material_Click(object sender, EventArgs e)
        {
            ProductMaterial productMaterial = new ProductMaterial();
            productMaterial.ShowDialog();
        }

        private void Menu_product_binding_Click(object sender, EventArgs e)
        {
            //PackageProduct packageProduct = new PackageProduct();
            //packageProduct.ShowDialog();
        }

        private void Menu_material_msg_Click(object sender, EventArgs e)
        {
            //Material material = new Material();
            //material.ShowDialog();
        }

        async private void Btn_search_LastTestRes_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(tb_sn.Text))
                {
                    MessageBox.Show("追溯号不能为空！","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(cb_typeNo.Text))
                {
                    MessageBox.Show("零件号不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if(string.IsNullOrEmpty(cb_station.Text))
                {
                    MessageBox.Show("站位名不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                //由追溯码+型号+当前站位
                DataTable dt = (await serviceClient.SelectLastTestResultUpperAsync(tb_sn.Text.Trim(), cb_typeNo.Text.Trim(),cb_station.Text.Trim())).Tables[0];
                listView_TestRes.DataSource = dt;
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error(ex.Message+"\r\n"+ex.StackTrace);
            }
        }

        async private void Btn_search_record_Click(object sender, EventArgs e)
        {
            //由追溯码/零件号/站位名 查询历史记录
            string sn = tb_sn.Text.Trim();
            string typeNo = cb_typeNo.Text.Trim();
            string station = cb_station.Text.Trim();
            DataTable dt = null;
            dt = (await serviceClient.SelectTestResultUpperAsync(sn, typeNo, station, false)).Tables[0];

            listView_TestRes.DataSource = dt;
        }

        private void Menu_typeno_Click(object sender, EventArgs e)
        {
            this.radDock1.AddDocument(documentWindow_testRes);
        }

        private void Menu_productType_Click(object sender, EventArgs e)
        {
            ProductType productType = new ProductType();
            productType.StartPosition = FormStartPosition.CenterParent;
            productType.ShowDialog();
        }

        private void Menu_produce_config_Click(object sender, EventArgs e)
        {
            Station setProduce = new Station();
            setProduce.StartPosition = FormStartPosition.CenterParent;
            setProduce.ShowDialog();
        }

        private void Menu_set_station_Click(object sender, EventArgs e)
        {
            Login.UserType userType = Login.UserType.USER_ADMIN;//Login.GetUserType;
            if (userType == Login.UserType.USER_ADMIN)
            {
                //管理员
                SetStationAdmin setStationAdmin = new SetStationAdmin();
                setStationAdmin.StartPosition = FormStartPosition.CenterParent;
                setStationAdmin.ShowDialog();
                
            }
            else if (userType == Login.UserType.USER_ORIDNARY)
            {
                //普通用户
                SetStation setStation = new SetStation();
                setStation.StartPosition = FormStartPosition.CenterParent;
                setStation.ShowDialog();
            }
        }

        private void InitListView()
        {
            listView_TestRes.ViewType = ListViewType.DetailsView;
            listView_TestRes.ShowGridLines = true;
        }

        public void LoadTreeView()
        {
            TreeViewControl treeView = new TreeViewControl(radTreeView1);
            treeView.LoadTreeView();
            radTreeView1.NodeMouseClick += RadTreeView1_NodeMouseClick;
        }

        private void RadTreeView1_NodeMouseClick(object sender, RadTreeViewEventArgs e)
        {
            RadTreeNode treeNode = e.Node;
            switch (treeNode.Text)
            {
                case "产品A":
                    
                    break;
            }
        }

        private void Menu_manager_Click(object sender, EventArgs e)
        {
            this.toolWindow_left.Show();
        }

        async private void InitControl()
        {
            //type no 
            cb_typeNo.Items.Clear();
            cb_material_typeNo.Items.Clear();
            cb_station.Items.Clear();
            try
            {
                DataTable dt = (await serviceClient.SelectProductContinairCapacityAsync("")).Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cb_typeNo.Items.Add(dt.Rows[i][0]);
                    cb_material_typeNo.Items.Add(dt.Rows[i][0]);
                }
                cb_typeNo.Items.Add("");
                //station
                dt = null;//(await serviceClient.SelectStationAsync("", "")).Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cb_station.Items.Add(dt.Rows[i][1]);
                }
                cb_station.Items.Add("");
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error(ex.Message+"\r\n"+ex.StackTrace);
            }
            DataGridViewCommon.SetRadGridViewProperty(this.radGridViewMaterial,false);
            DataGridViewCommon.SetRadGridViewProperty(this.radGridViewPackage,false);
        }

        private void Menu_glass_Click(object sender, EventArgs e)
        {
            InitStyle();
        }

        private void RadMenuItem18_Click(object sender, EventArgs e)
        {

        }
    }
}
