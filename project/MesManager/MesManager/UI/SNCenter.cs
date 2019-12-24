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
using System.IO;
using Telerik.WinControls.UI.Export;
using CommonUtils.Logger;
using CommonUtils.FileHelper;
using WindowsFormTelerik.GridViewExportData;
using System.Threading;
using Telerik.WinControls.Themes;
using Sunisoft.IrisSkin;
using MesManager.Common;

namespace MesManager.UI
{
    public partial class SNCenter : RadForm
    {
        private MesService.MesServiceClient serviceClient;
        private DataTable dataSourceMaterialBasic;
        private const string DATA_ORDER = "序号";
        private string currentQueryCondition;
        /// <summary>
        /// 当前页
        /// </summary>
        private int currentPage = 1;
        /// <summary>
        /// 每页的大小
        /// </summary>
        private int pageSize = 100;
        /// <summary>
        /// 总页数
        /// </summary>
        private int pageCount;

        private DataTable bindRowSource;

        #region 物料统计字段
        private const string MATERIAL_PN = "物料号";
        private const string MATERIAL_LOT = "批次号";
        private const string MATERIAL_RID = "料盘号";
        private const string MATERIAL_DC = "收料日期";
        private const string MATERIAL_QTY = "入库库存";
        private const string MATERIAL_NAME = "物料名称";
        private const string PRODUCT_TYPENO = "产品型号";
        private const string SN_PCBA = "PCBA";
        private const string SN_OUTTER = "外壳";
        private const string STATION_NAME = "工站名称";
        private const string USE_AMOUNTED = "当前使用数量";
        private const string RESIDUE_STOCK = "入库剩余库存";
        private const string CURRENT_RESIDUE_STOCK = "当前剩余库存";
        private const string TEAM_LEADER = "班组长";
        private const string ADMIN = "管理员";
        private const string UPDATE_DATE = "更新日期";

        private const string EXCEPT_TYPE = "异常类型";
        private const string EXCEPT_STOCK = "异常数量";
        private const string ACTUAL_STOCK = "实际库存";
        private const string MATERIAL_STATE = "物料状态";
        private const string SHUT_REASON = "结单原因";
        private const string USER_NAME = "结单用户";
        private const string STATEMENT_DATE = "结单日期";
        #endregion

        public SNCenter()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void SNCenter_Load(object sender, EventArgs e)
        {
            Init();
            EventHandlers();
            //InitStyle();
        }

        private void InitStyle()
        {
            SkinEngine skinEngine = new SkinEngine();
            string path = AppDomain.CurrentDomain.BaseDirectory + "Skins\\EighteenColor2.ssk";
            skinEngine.SkinFile = path;
            LogHelper.Log.Info(path);
            skinEngine.Active = true;
            skinEngine.SkinAllForm = true;
        }

        private void EventHandlers()
        {
            this.btn_materialSelect.Click += Btn_materialSelect_Click;
            this.btn_selectOfSn.Click += Btn_selectOfSn_Click;
            this.btn_selectOfPackage.Click += Btn_selectOfPackage_Click;
            this.btn_productCheck.Click += Btn_productCheck_Click;
            this.btn_quanlity.Click += Btn_quanlity_Click;

            this.tool_sn_export.Click += Tool_sn_export_Click;
            this.tool_material_export.Click += Tool_material_export_Click;
            this.tool_package_export.Click += Tool_package_export_Click;
            this.tool_productCheck_export.Click += Tool_productCheck_export_Click;
            this.tool_quanlity_export.Click += Tool_quanlity_export_Click;

            this.radGridViewMaterial.CellDoubleClick += RadGridViewMaterial_CellDoubleClick;
            this.radGridViewPackage.CellDoubleClick += RadGridViewPackage_CellDoubleClick;

            this.tool_SNClearDB.Click += Tool_SNClearDB_Click;
            this.tool_quanlityClearDB.Click += Tool_quanlityClearDB_Click;
            this.tool_packageClearDB.Click += Tool_packageClearDB_Click;
            this.tool_materialClearDB.Click += Tool_materialClearDB_Click;
            this.tool_productCheckClearDB.Click += Tool_productCheckClearDB_Click;

            this.bindingNavigator1.ItemClicked += BindingNavigator1_ItemClicked;
            this.bindingNavigatorCountItem.TextChanged += BindingNavigatorCountItem_TextChanged;
            this.bindingNavigatorPositionItem.TextChanged += BindingNavigatorPositionItem_TextChanged;

            this.radDock1.ActiveWindowChanged += RadDock1_ActiveWindowChanged;
        }

        private void RadDock1_ActiveWindowChanged(object sender, Telerik.WinControls.UI.Docking.DockWindowEventArgs e)
        {
            this.currentPage = 1;
            this.bindingNavigatorPositionItem.Text = this.currentPage.ToString();
        }

        private void Tool_productCheckClearDB_Click(object sender, EventArgs e)
        {
            if (this.radGridViewCheck.RowCount < 1)
            {
                MessageBox.Show("没有可以清除的数据!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("确认要清除当前数据？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                return;
            var delRow = serviceClient.DeleteProductPackage(this.currentQueryCondition,0);
            if (delRow > 0)
            {
                MessageBox.Show("删除数据完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("删除数据未完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            SelectOfPackageCheck("0");
        }

        private void Tool_materialClearDB_Click(object sender, EventArgs e)
        {
            if (this.radGridViewMaterial.Rows.Count < 1)
            {
                MessageBox.Show("没有可以清除的数据!","提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("确认要清除当前数据？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                return;
            var delRow = serviceClient.DeleteMaterialBasicMsg(this.currentQueryCondition);
            if (delRow > 0)
            {
                MessageBox.Show("删除数据完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("删除数据未完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            SelectOfMaterial();
        }

        private void Tool_packageClearDB_Click(object sender, EventArgs e)
        {
            if (this.radGridViewPackage.RowCount < 1)
            {
                MessageBox.Show("没有可以清除的数据!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("确认要清除当前数据？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                return;
            var delRow = serviceClient.DeleteProductPackage(this.currentQueryCondition,1);
            if (delRow > 0)
            {
                MessageBox.Show("删除数据完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("删除数据未完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            SelectOfPackage("1");
        }

        private void Tool_quanlityClearDB_Click(object sender, EventArgs e)
        {
            if (this.radGridViewQuanlity.RowCount < 1)
            {
                MessageBox.Show("没有可以清除的数据!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("确认要清除当前数据？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                return;
            var delRow = serviceClient.DeleteQuanlityMsg(this.currentQueryCondition);
            if (delRow > 0)
            {
                MessageBox.Show("删除数据完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("删除数据未完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            SelectOfMaterialQuanlity();
        }

        private void Tool_SNClearDB_Click(object sender, EventArgs e)
        {
            
        }

        #region UI style
        private void Menu_windows8_Click()
        {
            Windows8Theme windows8Theme = new Windows8Theme();
            ThemeResolutionService.ApplicationThemeName = windows8Theme.ThemeName;
        }

        private void Menu_windows7_Click()
        {
            Windows7Theme windows7Theme = new Windows7Theme();
            ThemeResolutionService.ApplicationThemeName = windows7Theme.ThemeName;
        }

        private void Menu_Office2013Light_Click()
        {
            Office2013LightTheme office2013LightTheme = new Office2013LightTheme();
            ThemeResolutionService.ApplicationThemeName = office2013LightTheme.ThemeName;
        }

        private void Menu_office2013Dark_Click()
        {
            Office2013DarkTheme office2013DarkTheme = new Office2013DarkTheme();
            ThemeResolutionService.ApplicationThemeName = office2013DarkTheme.ThemeName;
        }

        private void Menu_office2010Silver_Click()
        {
            Office2010SilverTheme office2010SilverTheme = new Office2010SilverTheme();
            ThemeResolutionService.ApplicationThemeName = office2010SilverTheme.ThemeName;
        }

        private void Menu_office2010Blue_Click()
        {
            Office2010BlueTheme office2010BlueTheme = new Office2010BlueTheme();
            ThemeResolutionService.ApplicationThemeName = office2010BlueTheme.ThemeName;
        }

        private void Menu_office2010Black_Click()
        {
            Office2010BlackTheme office2010BlackTheme = new Office2010BlackTheme();
            ThemeResolutionService.ApplicationThemeName = office2010BlackTheme.ThemeName;
        }

        private void Menu_office2007Silver_Click()
        {
            Office2007SilverTheme office2007SilverTheme = new Office2007SilverTheme();
            ThemeResolutionService.ApplicationThemeName = office2007SilverTheme.ThemeName;
        }

        private void Menu_office2007Black_Click()
        {
            Office2007BlackTheme office2007BlackTheme = new Office2007BlackTheme();
            ThemeResolutionService.ApplicationThemeName = office2007BlackTheme.ThemeName;
        }

        private void Menu_telerikMetroTouch_Click()
        {
            TelerikMetroTouchTheme telerikMetroTouchTheme = new TelerikMetroTouchTheme();
            ThemeResolutionService.ApplicationThemeName = telerikMetroTouchTheme.ThemeName;
        }

        private void Menu_telerikMetroBlue_Click()
        {
            TelerikMetroBlueTheme telerikMetroBlueTheme = new TelerikMetroBlueTheme();
            ThemeResolutionService.ApplicationThemeName = telerikMetroBlueTheme.ThemeName;
        }

        private void Menu_telerikMetro_Click()
        {
            TelerikMetroBlueTheme telerikMetroBlueTheme = new TelerikMetroBlueTheme();
            ThemeResolutionService.ApplicationThemeName = telerikMetroBlueTheme.ThemeName;
        }

        private void Menu_materialTeal_Click()
        {
            MaterialTealTheme materialTealTheme = new MaterialTealTheme();
            ThemeResolutionService.ApplicationThemeName = materialTealTheme.ThemeName;
        }

        private void Menu_materialPink_Click()
        {
            MaterialPinkTheme materialPinkTheme = new MaterialPinkTheme();
            ThemeResolutionService.ApplicationThemeName = materialPinkTheme.ThemeName;
        }

        private void Menu_materialBlue_Click()
        {
            MaterialBlueGreyTheme materialBlueGreyTheme = new MaterialBlueGreyTheme();
            ThemeResolutionService.ApplicationThemeName = materialBlueGreyTheme.ThemeName;
        }

        private void Menu_material_Click()
        {
            MaterialTheme materialTheme = new MaterialTheme();
            ThemeResolutionService.ApplicationThemeName = materialTheme.ThemeName;
        }

        private void Menu_vs2012Light_Click()
        {
            VisualStudio2012LightTheme visualStudio2012LightTheme = new VisualStudio2012LightTheme();
            ThemeResolutionService.ApplicationThemeName = visualStudio2012LightTheme.ThemeName;
        }

        private void Menu_vs2012dark_Click()
        {
            VisualStudio2012DarkTheme visualStudio2012DarkTheme = new VisualStudio2012DarkTheme();
            ThemeResolutionService.ApplicationThemeName = visualStudio2012DarkTheme.ThemeName;
        }
        #endregion

        private void Btn_quanlity_Click(object sender, EventArgs e)
        {
            SelectOfMaterialQuanlity();
        }

        private void Btn_productCheck_Click(object sender, EventArgs e)
        {
            SelectOfPackageCheck("0");
        }

        private void RadGridViewMaterial_CellDoubleClick(object sender, GridViewCellEventArgs e)
        {
            var ridCode = this.radGridViewMaterial.CurrentRow.Cells[3].Value.ToString();
            var materialCode = serviceClient.GetMaterialCode(ridCode);
            MaterialDetailMsg materialDetailMsg = new MaterialDetailMsg(materialCode);
            materialDetailMsg.ShowDialog();
        }

        private void RadGridViewPackage_CellDoubleClick(object sender, GridViewCellEventArgs e)
        {
            var outCaseCode = this.radGridViewPackage.CurrentRow.Cells[1].Value.ToString();
            ProductPackageDetail productPackageDetail = new ProductPackageDetail(outCaseCode);
            productPackageDetail.ShowDialog();
        }

        private void Tool_package_export_Click(object sender, EventArgs e)
        {
            ExportGridViewData(this.tool_package_exportFilter.Text, this.radGridViewPackage);
        }

        private void Tool_quanlity_export_Click(object sender, EventArgs e)
        {
            ExportGridViewData(this.tool_quanlity_exportFilter.Text, this.radGridViewQuanlity);
        }

        private void Tool_productCheck_export_Click(object sender, EventArgs e)
        {
            ExportGridViewData(this.tool_productCheck_exportFilter.Text, this.radGridViewCheck);
        }

        private void Tool_material_export_Click(object sender, EventArgs e)
        {
            ExportGridViewData(this.tool_material_exportFilter.Text, this.radGridViewMaterial);
        }

        private void Tool_sn_export_Click(object sender, EventArgs e)
        {
            ExportGridViewData(this.tool_sn_exportFilter.Text,this.radGridViewSn);
        }

        private void ExportGridViewData(string exportCondition,RadGridView radGridView)
        {
            GridViewExport.ExportFormat exportFormat = GridViewExport.ExportFormat.EXCEL;
            Enum.TryParse(exportCondition,out exportFormat);
            GridViewExport.ExportGridViewData(exportFormat, radGridView);
        }

        private void Btn_materialSelect_Click(object sender, EventArgs e)
        {
            SelectOfMaterial();
        }

        private void Init()
        {
            serviceClient = new MesService.MesServiceClient();
            DataGridViewCommon.SetRadGridViewProperty(this.radGridViewSn, false);
            DataGridViewCommon.SetRadGridViewProperty(this.radGridViewPackage, false);
            DataGridViewCommon.SetRadGridViewProperty(this.radGridViewMaterial, false);
            DataGridViewCommon.SetRadGridViewProperty(this.radGridViewCheck,false);
            DataGridViewCommon.SetRadGridViewProperty(this.radGridViewQuanlity, false);
            this.radGridViewSn.ReadOnly = true;
            this.radGridViewPackage.ReadOnly = true;
            this.radGridViewMaterial.ReadOnly = true;
            this.radGridViewCheck.ReadOnly = true;
            this.radGridViewQuanlity.ReadOnly = true;
            InitDataTable();
            this.panel_sn.Visible = true;
            this.panel_sn.Dock = DockStyle.Fill;
            this.tool_sn_exportFilter.Items.Clear();
            this.tool_sn_exportFilter.Items.Add(GridViewExport.ExportFormat.EXCEL);
            this.tool_sn_exportFilter.Items.Add(GridViewExport.ExportFormat.HTML);
            this.tool_sn_exportFilter.Items.Add(GridViewExport.ExportFormat.PDF);
            this.tool_sn_exportFilter.Items.Add(GridViewExport.ExportFormat.CSV);
            this.tool_sn_exportFilter.SelectedIndex = 0;
            this.tool_package_exportFilter.Items.Clear();
            this.tool_package_exportFilter.Items.Add(GridViewExport.ExportFormat.EXCEL);
            this.tool_package_exportFilter.Items.Add(GridViewExport.ExportFormat.HTML);
            this.tool_package_exportFilter.Items.Add(GridViewExport.ExportFormat.PDF);
            this.tool_package_exportFilter.Items.Add(GridViewExport.ExportFormat.CSV);
            this.tool_package_exportFilter.SelectedIndex = 0;
            this.tool_material_exportFilter.Items.Clear();
            this.tool_material_exportFilter.Items.Add(GridViewExport.ExportFormat.EXCEL);
            this.tool_material_exportFilter.Items.Add(GridViewExport.ExportFormat.HTML);
            this.tool_material_exportFilter.Items.Add(GridViewExport.ExportFormat.PDF);
            this.tool_material_exportFilter.Items.Add(GridViewExport.ExportFormat.CSV);
            this.tool_material_exportFilter.SelectedIndex = 0;
            this.tool_productCheck_exportFilter.Items.Clear();
            this.tool_productCheck_exportFilter.Items.Add(GridViewExport.ExportFormat.EXCEL);
            this.tool_productCheck_exportFilter.Items.Add(GridViewExport.ExportFormat.HTML);
            this.tool_productCheck_exportFilter.Items.Add(GridViewExport.ExportFormat.PDF);
            this.tool_productCheck_exportFilter.Items.Add(GridViewExport.ExportFormat.CSV);
            this.tool_productCheck_exportFilter.SelectedIndex = 0;
            this.tool_quanlity_exportFilter.Items.Clear();
            this.tool_quanlity_exportFilter.Items.Add(GridViewExport.ExportFormat.EXCEL);
            this.tool_quanlity_exportFilter.Items.Add(GridViewExport.ExportFormat.HTML);
            this.tool_quanlity_exportFilter.Items.Add(GridViewExport.ExportFormat.PDF);
            this.tool_quanlity_exportFilter.Items.Add(GridViewExport.ExportFormat.CSV);
            this.tool_quanlity_exportFilter.SelectedIndex = 0;
            if (MESMainForm.currentUsetType != 0)
            {
                this.tool_materialClearDB.Enabled = false;
                this.tool_packageClearDB.Enabled = false;
                this.tool_productCheckClearDB.Enabled = false;
                this.tool_quanlityClearDB.Enabled = false;
                this.tool_SNClearDB.Enabled = false;

            }
            this.tool_SNClearDB.Visible = false;
            this.radDock1.ActiveWindow = this.dw_snHistory;
        }

        async private void SelectOfSn()
        {
            //page
            var filter = tb_sn.Text;
            if (filter != "")
            {
                this.currentPage = 1;//根据条件查询
                this.bindingNavigatorPositionItem.Text = currentPage.ToString();
            }
            this.radGridViewSn.DataSource = null;
            this.radGridViewSn.Update();
            var pcbaList = serviceClient.SelectUseAllPcbaSN().Length;
            if (pcbaList % pageSize > 0)
            {
                pageCount = pcbaList / pageSize + 1;
            }
            else
            {
                pageCount = pcbaList / pageSize;
            }
            var dtSource = InitBindRowSource();
            //DataSet ds = (await serviceClient.SelectTestResultUpperAsync(filter, filter, filter, true));
            DataSet ds = await serviceClient.SelectTestResultDetailAsync(filter,currentPage,pageSize,true);
            this.radGridViewSn.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.None;
            this.radGridViewSn.BeginEdit();
            DataTable dt = ds.Tables[0];
            this.radGridViewSn.DataSource = dt;
            bindingSource1.DataSource = dtSource;
            this.bindingNavigator1.BindingSource = bindingSource1;
            this.radGridViewSn.EndEdit();
            this.radGridViewSn.BestFitColumns();
        }

        
        async private void SelectOfPackage(string state)
        {
            this.radGridViewPackage.DataSource = null;
            this.radGridViewPackage.Update();
            this.currentQueryCondition = tb_package.Text;
            if (this.currentQueryCondition != "")
            {
                this.currentPage = 1;//根据条件查询
                this.bindingNavigatorPositionItem.Text = currentPage.ToString();
            }
            //箱子编码/追溯码/型号
            var packageHistory = (await serviceClient.SelectPackageStorageAsync(currentQueryCondition,currentPage,pageSize));

            if (packageHistory.PackageCaseNumber % pageSize > 0)
            {
                pageCount = packageHistory.PackageCaseNumber / pageSize + 1;
            }
            else
            {
                pageCount = packageHistory.PackageCaseNumber / pageSize;
            }
            var dtSource = InitBindRowSource();
            this.radGridViewPackage.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
            this.radGridViewPackage.BeginEdit();
            var dt = packageHistory.PackageCaseData.Tables[0];
            this.radGridViewPackage.DataSource = dt;
            this.bindingSource1.DataSource = dtSource;
            this.bindingNavigator1.BindingSource = this.bindingSource1;
            this.radGridViewPackage.EndEdit();
            this.radGridViewPackage.BestFitColumns();
        }

        async private void SelectOfPackageCheck(string state)
        {
            this.radGridViewPackage.DataSource = null;
            this.radGridViewPackage.Update();
            this.currentQueryCondition = tb_package.Text;
            currentQueryCondition = tb_productCheck.Text;
            if (this.currentQueryCondition != "")
            {
                this.currentPage = 1;//根据条件查询
                this.bindingNavigatorPositionItem.Text = this.currentPage.ToString();
            }
            //箱子编码/追溯码/型号
            var checkPackageProduct = (await serviceClient.SelectPackageProductCheckAsync(currentQueryCondition, state,false,currentPage,pageSize));
            if (checkPackageProduct.CheckPackageCaseNumber % pageSize > 0)
            {
                pageCount = checkPackageProduct.CheckPackageCaseNumber / pageSize + 1;
            }
            else
            {
                pageCount = checkPackageProduct.CheckPackageCaseNumber / pageSize;
            }
            var dtSource = InitBindRowSource();
            this.radGridViewCheck.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.None;
            this.radGridViewCheck.BeginEdit();
            var dt = checkPackageProduct.CheckPackageCaseData.Tables[0];
            this.radGridViewCheck.DataSource = dt;
            this.bindingSource1.DataSource = dtSource;
            this.bindingNavigator1.BindingSource = this.bindingSource1;
            this.radGridViewCheck.EndEdit();
            this.radGridViewCheck.BestFitColumns();
        }

        private void InitDataTable()
        {
            if (dataSourceMaterialBasic == null)
            {
                dataSourceMaterialBasic = new DataTable();
                dataSourceMaterialBasic.Columns.Add(DATA_ORDER);
                dataSourceMaterialBasic.Columns.Add(MATERIAL_PN);
                dataSourceMaterialBasic.Columns.Add(MATERIAL_LOT);
                dataSourceMaterialBasic.Columns.Add(MATERIAL_RID);
                dataSourceMaterialBasic.Columns.Add(MATERIAL_DC);
                dataSourceMaterialBasic.Columns.Add(MATERIAL_NAME);
                dataSourceMaterialBasic.Columns.Add(PRODUCT_TYPENO);
                dataSourceMaterialBasic.Columns.Add(SN_PCBA);
                dataSourceMaterialBasic.Columns.Add(SN_OUTTER);
                dataSourceMaterialBasic.Columns.Add(MATERIAL_QTY);
                dataSourceMaterialBasic.Columns.Add(USE_AMOUNTED);
                dataSourceMaterialBasic.Columns.Add(CURRENT_RESIDUE_STOCK);
                dataSourceMaterialBasic.Columns.Add(RESIDUE_STOCK);
            }
        }

        private DataTable InitBindRowSource()
        {
            if (bindRowSource == null)
            {
                bindRowSource = new DataTable();
                bindRowSource.Columns.Add("ID");
            }
            if (pageCount < 1)
                return bindRowSource;
            if (this.bindRowSource.Rows.Count != this.pageCount)
            {
                this.bindRowSource.Clear();
                for (int i = 0; i < pageCount; i++)
                {
                    DataRow dr = bindRowSource.NewRow();
                    dr["ID"] = i + 1;
                    bindRowSource.Rows.Add(dr);
                }
            }
            return bindRowSource;
        }

        async private void SelectOfMaterialQuanlity()
        {
            currentQueryCondition = this.tb_quanlity_filter.Text;
            if (this.currentQueryCondition != "")
            {
                this.currentPage = 1;//根据条件查询
                this.bindingNavigatorPositionItem.Text = currentPage.ToString();
            }
            var quanlityObj = (await serviceClient.SelectQuanlityManagerAsync(currentQueryCondition,currentPage,pageSize));
            this.radGridViewQuanlity.DataSource = null;
            this.radGridViewQuanlity.Update();
            if (quanlityObj.HistoryNumber % pageSize > 0)
            {
                pageCount = quanlityObj.HistoryNumber / pageSize + 1;
            }
            else
            {
                pageCount = quanlityObj.HistoryNumber / pageSize;
            }
            var dtSource = InitBindRowSource();
            this.radGridViewQuanlity.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.None;
            this.radGridViewQuanlity.BeginEdit();
            var dt = quanlityObj.QuanlityHistoryData.Tables[0];
            this.radGridViewQuanlity.DataSource = dt;
            this.bindingSource1.DataSource = dtSource;
            this.bindingNavigator1.BindingSource = this.bindingSource1;
            this.radGridViewQuanlity.BestFitColumns();
        }

        /// <summary>
        /// 根据物料编码查询物料被使用于哪些产品
        /// </summary>
        async private void SelectOfMaterial()
        {
            this.currentQueryCondition = this.tb_material.Text;
            if (this.currentQueryCondition != "")
            {
                this.currentPage = 1;//根据条件查询
                this.bindingNavigatorPositionItem.Text = currentPage.ToString();
            }
            //物料信息表
            //物料编码+物料名称+所属型号+在哪个工序/站位消耗+该位置消耗数量
            var materialObj = await serviceClient.SelectMaterialBasicMsgAsync(this.tb_material.Text,currentPage,pageSize);
            this.radGridViewMaterial.DataSource = null;
            this.radGridViewMaterial.Update();
            if (materialObj.MaterialRowCount % pageSize > 0)
            {
                pageCount = materialObj.MaterialRowCount / pageSize + 1;
            }
            else
            {
                pageCount = materialObj.MaterialRowCount / pageSize;
            }
            var dtSource = InitBindRowSource();
            this.radGridViewMaterial.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.None;
            this.radGridViewMaterial.BeginEdit();
            var dt = materialObj.MaterialResultData.Tables[0];
            this.radGridViewMaterial.DataSource = dt;
            bindingSource1.DataSource = dtSource;
            this.bindingNavigator1.BindingSource = bindingSource1;
            this.radGridViewMaterial.EndEdit();
            //this.radGridViewMaterial.Columns[0].BestFit();
            this.radGridViewMaterial.BestFitColumns();
        }

        private void Btn_selectOfSn_Click(object sender, EventArgs e)
        {
            ResetCurrentPage();
            SelectOfSn();
        }

        private void Btn_selectOfPackage_Click(object sender, EventArgs e)
        {
            SelectOfPackage("1");
        }

        private void BindingNavigatorPositionItem_TextChanged(object sender, EventArgs e)
        {
            //this.bindingNavigatorPositionItem.Text = currentPage.ToString();
        }

        private void BindingNavigatorCountItem_TextChanged(object sender, EventArgs e)
        {
            //this.bindingNavigatorCountItem.Text = "/" + pageCount;
        }

        private void BindingNavigator1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "删除")
            {
                //删除页
            }
            else if (e.ClickedItem.Text == "上一页")
            {
                if (currentPage > 1)
                {
                    currentPage--;
                }
            }
            else if (e.ClickedItem.Text == "下一页")
            {
                if (currentPage < pageCount)
                {
                    currentPage++;
                }
            }
            else if (e.ClickedItem.Text == "首页")
            {
                currentPage = 1;
            }
            else if (e.ClickedItem.Text == "尾页")
            {
                currentPage = pageCount;
            }
            else if (e.ClickedItem.Text == "新添")
            {

            }
            UpdateQueryPageData();
        }

        private void ResetCurrentPage()
        {
            this.currentPage = 1;//根据条件查询/点击查询-刷新最新数据
            this.bindingNavigatorPositionItem.Text = currentPage.ToString();
        }
        private void UpdateQueryPageData()
        {
            if (this.radDock1.ActiveWindow == this.dw_snHistory)
            {
                SelectOfSn();
            }
            else if (this.radDock1.ActiveWindow == this.dw_packageProduct)
            {
                SelectOfPackage("1");
            }
            else if (this.radDock1.ActiveWindow == this.dw_materialUse)
            {
                SelectOfMaterial();
            }
            else if (this.radDock1.ActiveWindow == this.dw_productCheck)
            {
                SelectOfPackageCheck("0");
            }
            else if (this.radDock1.ActiveWindow == this.dw_materialExcept)
            {
                SelectOfMaterialQuanlity();
            }
        }
    }
}
