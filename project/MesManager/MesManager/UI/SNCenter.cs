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
using MesManager.TelerikWinform.GridViewCommon.GridViewDataExport;
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
        private DataTable dataSourceProductCheck;
        private DataTable dataSourceQuanlity;
        private DataTable dataSourceProductPackage;
        private const string DATA_ORDER = "序号";

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

        #region 成品抽检字段
        private const string CHECK_ORDER = "序号";
        private const string CHECK_SN = "产品SN";
        private const string CHECK_CASE_CODE = "箱子编码";
        private const string CHECK_TYPE_NO = "产品型号";
        private const string CHECK_NUMBER = "数量";
        private const string CHECK_BINDING_DATE = "修改日期";
        private const string CHECK_BINDING_STATE = "产品状态";
        private const string CHECK_REMARK = "描述";
        private const string CHECK_LEADER = "班组长";
        private const string CHECK_ADMIN = "管理员";
        #endregion

        #region 产品打包
        public const string OUT_CASE_CODE = "箱子编码";
        public const string CASE_PRODUCT_TYPE_NO = "产品型号";
        public const string CASE_STORAGE_CAPACITY = "箱子容量";
        public const string CASE_AMOUNTED = "产品实际数据";
        #endregion

        private enum ExportFormat
        {
            EXCEL,
            HTML,
            PDF,
            CSV
        }

        public SNCenter()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void SNCenter_Load(object sender, EventArgs e)
        {
            Init();
            SelectOfSn();
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
            this.menu_sn_result.Click += Menu_sn_result_Click;
            this.menu_material.Click += Menu_material_Click;
            this.menu_package.Click += Menu_package_Click;
            this.menu_productCheck.Click += Menu_productCheck_Click;
            this.menu_quanlity.Click += Menu_quanlity_Click;

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
            ExportGridViewData(this.tool_package_exportFilter.SelectedIndex, this.radGridViewPackage);
        }

        private void Tool_quanlity_export_Click(object sender, EventArgs e)
        {
            ExportGridViewData(this.tool_quanlity_exportFilter.SelectedIndex, this.radGridViewQuanlity);
        }

        private void Tool_productCheck_export_Click(object sender, EventArgs e)
        {
            ExportGridViewData(this.tool_productCheck_exportFilter.SelectedIndex, this.radGridViewCheck);
        }

        private void Tool_material_export_Click(object sender, EventArgs e)
        {
            ExportGridViewData(this.tool_material_exportFilter.SelectedIndex, this.radGridViewMaterial);
        }

        private void Tool_sn_export_Click(object sender, EventArgs e)
        {
            ExportGridViewData(this.tool_sn_exportFilter.SelectedIndex,this.radGridViewSn);
        }

        private void ExportGridViewData(int selectIndex,RadGridView radGridView)
        {
            var filter = "Excel (*.xls)|*.xls";
            if (selectIndex == (int)ExportFormat.EXCEL)
            {
                filter = "Excel (*.xls)|*.xls";
                var path = FileSelect.SaveAs(filter, "C:\\");
                if (path == "")
                    return;
                ExportData.RunExportToExcelML(path, radGridView);
            }
            else if (selectIndex == (int)ExportFormat.HTML)
            {
                filter = "Html File (*.htm)|*.htm";
                var path = FileSelect.SaveAs(filter, "C:\\");
                if (path == "")
                    return;
                ExportData.RunExportToHTML(path, radGridView);
            }
            else if (selectIndex == (int)ExportFormat.PDF)
            {
                filter = "PDF file (*.pdf)|*.pdf";
                var path = FileSelect.SaveAs(filter, "C:\\");
                if (path == "")
                    return;
                ExportData.RunExportToPDF(path, radGridView);
            }
            else if (selectIndex == (int)ExportFormat.CSV)
            {
                filter = "PDF file (*.pdf)|*.csv";
                var path = FileSelect.SaveAs(filter, "C:\\");
                if (path == "")
                    return;
                ExportData.RunExportToCSV(path, radGridView);
            }
        }

        private void Btn_materialSelect_Click(object sender, EventArgs e)
        {
            SelectOfMaterial();
        }

        private void Menu_quanlity_Click(object sender, EventArgs e)
        {
            this.panel_quanlity.Dock = DockStyle.Fill;
            SetPanelFalse();
            this.panel_quanlity.Visible = true;
            SelectOfMaterialQuanlity();
        }

        private void Menu_productCheck_Click(object sender, EventArgs e)
        {
            this.panel_productCheck.Dock = DockStyle.Fill;
            SetPanelFalse();
            this.panel_productCheck.Visible = true;
            SelectOfPackageCheck("0");
        }

        private void Menu_package_Click(object sender, EventArgs e)
        {
            this.panel_package.Dock = DockStyle.Fill;
            SetPanelFalse();
            this.panel_package.Visible = true;
            SelectOfPackage("1");
        }

        private void Menu_material_Click(object sender, EventArgs e)
        {
            this.panel_material.Dock = DockStyle.Fill;
            SetPanelFalse();
            this.panel_material.Visible = true;
            SelectOfMaterial();
        }

        private void Menu_sn_result_Click(object sender, EventArgs e)
        {
            this.panel_sn.Dock = DockStyle.Fill;
            SetPanelFalse();
            this.panel_sn.Visible = true;
            SelectOfSn();
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
            SetPanelFalse();
            InitDataTable();
            this.panel_sn.Visible = true;
            this.panel_sn.Dock = DockStyle.Fill;
            this.tool_sn_exportFilter.Items.Clear();
            this.tool_sn_exportFilter.Items.Add(ExportFormat.EXCEL);
            this.tool_sn_exportFilter.Items.Add(ExportFormat.HTML);
            this.tool_sn_exportFilter.Items.Add(ExportFormat.PDF);
            this.tool_sn_exportFilter.Items.Add(ExportFormat.CSV);
            this.tool_sn_exportFilter.SelectedIndex = 0;
            this.tool_package_exportFilter.Items.Clear();
            this.tool_package_exportFilter.Items.Add(ExportFormat.EXCEL);
            this.tool_package_exportFilter.Items.Add(ExportFormat.HTML);
            this.tool_package_exportFilter.Items.Add(ExportFormat.PDF);
            this.tool_package_exportFilter.Items.Add(ExportFormat.CSV);
            this.tool_package_exportFilter.SelectedIndex = 0;
            this.tool_material_exportFilter.Items.Clear();
            this.tool_material_exportFilter.Items.Add(ExportFormat.EXCEL);
            this.tool_material_exportFilter.Items.Add(ExportFormat.HTML);
            this.tool_material_exportFilter.Items.Add(ExportFormat.PDF);
            this.tool_material_exportFilter.Items.Add(ExportFormat.CSV);
            this.tool_material_exportFilter.SelectedIndex = 0;
            this.tool_productCheck_exportFilter.Items.Clear();
            this.tool_productCheck_exportFilter.Items.Add(ExportFormat.EXCEL);
            this.tool_productCheck_exportFilter.Items.Add(ExportFormat.HTML);
            this.tool_productCheck_exportFilter.Items.Add(ExportFormat.PDF);
            this.tool_productCheck_exportFilter.Items.Add(ExportFormat.CSV);
            this.tool_productCheck_exportFilter.SelectedIndex = 0;
            this.tool_quanlity_exportFilter.Items.Clear();
            this.tool_quanlity_exportFilter.Items.Add(ExportFormat.EXCEL);
            this.tool_quanlity_exportFilter.Items.Add(ExportFormat.HTML);
            this.tool_quanlity_exportFilter.Items.Add(ExportFormat.PDF);
            this.tool_quanlity_exportFilter.Items.Add(ExportFormat.CSV);
            this.tool_quanlity_exportFilter.SelectedIndex = 0;
        }

        private void SetPanelFalse()
        {
            this.panel_sn.Visible = false;
            this.panel_material.Visible = false;
            this.panel_package.Visible = false;
            this.panel_productCheck.Visible = false;
            this.panel_quanlity.Visible = false;
        }

        async private void SelectOfSn()
        {
            var filter = tb_sn.Text;
            //DataSet ds = (await serviceClient.SelectTestResultUpperAsync(filter, filter, filter, true));
            DataSet ds = await serviceClient.SelectTestResultDetailAsync(filter);
            if (ds.Tables.Count < 1)
            {
                this.radGridViewSn.DataSource = null;
                return;
            }
            DataTable dt = ds.Tables[0];
            this.radGridViewSn.DataSource = null; 
            radGridViewSn.DataSource = dt;
            this.radGridViewSn.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.None;
            this.radGridViewSn.BestFitColumns();
        }

        async private void SelectOfPackage(string state)
        {
            var filter = tb_package.Text;
            //箱子编码/追溯码/型号
            DataTable dt = (await serviceClient.SelectPackageStorageAsync(filter)).Tables[0];
            dataSourceProductPackage.Clear();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dataSourceProductPackage.NewRow();
                    dr[DATA_ORDER] = i + 1;
                    dr[OUT_CASE_CODE] = dt.Rows[i][0].ToString();
                    dr[CASE_PRODUCT_TYPE_NO] = dt.Rows[i][1].ToString();
                    dr[CASE_STORAGE_CAPACITY] = dt.Rows[i][2].ToString();
                    dr[CASE_AMOUNTED] = dt.Rows[i][3].ToString();
                    dataSourceProductPackage.Rows.Add(dr);
                }
            }
            this.radGridViewPackage.DataSource = dataSourceProductPackage;
            this.radGridViewPackage.Columns[0].BestFit();
        }

        async private void SelectOfPackageCheck(string state)
        {
            var filter = tb_productCheck.Text;
            //箱子编码/追溯码/型号
            DataTable dt = (await serviceClient.SelectPackageProductCheckAsync(filter, state,false)).Tables[0];
            this.dataSourceProductCheck.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var orderID = i + 1;
                var caseCode = dt.Rows[i][0].ToString();
                var sn = dt.Rows[i][1].ToString();
                var typeNo = dt.Rows[i][2].ToString();
                var teamLeader = dt.Rows[i][3].ToString();
                var admin = dt.Rows[i][4].ToString();
                var remark = dt.Rows[i][5].ToString();
                var bindingDate = dt.Rows[i][6].ToString();
                var productState = "已解包";
                DataRow dr = dataSourceProductCheck.NewRow();
                dr[CHECK_ORDER] = orderID;
                dr[CHECK_CASE_CODE] = caseCode;
                dr[CHECK_SN] = sn;
                dr[CHECK_TYPE_NO] = typeNo;
                dr[CHECK_BINDING_DATE] = bindingDate;
                dr[CHECK_LEADER] = teamLeader;
                dr[CHECK_ADMIN] = admin;
                dr[CHECK_REMARK] = remark;
                dr[CHECK_BINDING_STATE] = productState;
                dr[CHECK_NUMBER] = 1;
                dataSourceProductCheck.Rows.Add(dr);
            }
            this.radGridViewCheck.DataSource = dataSourceProductCheck;
            this.radGridViewCheck.Columns[0].BestFit();
            this.radGridViewCheck.Columns[2].BestFit();
            this.radGridViewCheck.Columns[9].BestFit();
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
            if (dataSourceProductCheck == null)
            {
                dataSourceProductCheck = new DataTable();
                dataSourceProductCheck.Columns.Add(CHECK_ORDER);
                dataSourceProductCheck.Columns.Add(CHECK_CASE_CODE);
                dataSourceProductCheck.Columns.Add(CHECK_SN);
                dataSourceProductCheck.Columns.Add(CHECK_TYPE_NO);
                dataSourceProductCheck.Columns.Add(CHECK_BINDING_STATE);
                dataSourceProductCheck.Columns.Add(CHECK_NUMBER);
                dataSourceProductCheck.Columns.Add(CHECK_REMARK);
                dataSourceProductCheck.Columns.Add(CHECK_LEADER);
                dataSourceProductCheck.Columns.Add(CHECK_ADMIN);
                dataSourceProductCheck.Columns.Add(CHECK_BINDING_DATE);
            }
            if (dataSourceQuanlity == null)
            {
                dataSourceQuanlity = new DataTable();
                dataSourceQuanlity.Columns.Add(DATA_ORDER);
                dataSourceQuanlity.Columns.Add(MATERIAL_PN);
                dataSourceQuanlity.Columns.Add(MATERIAL_LOT);
                dataSourceQuanlity.Columns.Add(MATERIAL_RID);
                dataSourceQuanlity.Columns.Add(MATERIAL_DC);
                dataSourceQuanlity.Columns.Add(MATERIAL_NAME);
                dataSourceQuanlity.Columns.Add(EXCEPT_TYPE);
                dataSourceQuanlity.Columns.Add(MATERIAL_QTY);
                dataSourceQuanlity.Columns.Add(ACTUAL_STOCK);
                dataSourceQuanlity.Columns.Add(EXCEPT_STOCK);
                dataSourceQuanlity.Columns.Add(MATERIAL_STATE);
                dataSourceQuanlity.Columns.Add(SHUT_REASON);
                dataSourceQuanlity.Columns.Add(USER_NAME);
                dataSourceQuanlity.Columns.Add(STATEMENT_DATE);
            }
            if (dataSourceProductPackage == null)
            {
                dataSourceProductPackage = new DataTable();
                dataSourceProductPackage.Columns.Add(DATA_ORDER);
                dataSourceProductPackage.Columns.Add(OUT_CASE_CODE);
                dataSourceProductPackage.Columns.Add(CASE_PRODUCT_TYPE_NO);
                dataSourceProductPackage.Columns.Add(CASE_STORAGE_CAPACITY);
                dataSourceProductPackage.Columns.Add(CASE_AMOUNTED);
            }
        }

        async private void SelectOfMaterialQuanlity()
        {
            var materialCodeFilter = this.tb_quanlity_filter.Text;
            var dt = (await serviceClient.SelectQuanlityManagerAsync(materialCodeFilter)).Tables[0];
            this.dataSourceQuanlity.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dataSourceQuanlity.NewRow();
                var materialCode = dt.Rows[i][0].ToString();
                if (!materialCode.Contains("&"))
                    continue;
                AnalysisMaterialCode analysisMaterial = AnalysisMaterialCode.GetMaterialDetail(materialCode);
                var pnCode = analysisMaterial.MaterialPN;
                var lotCode = analysisMaterial.MaterialLOT;
                var ridCode = analysisMaterial.MaterialRID;
                var dcCode = analysisMaterial.MaterialDC;
                var qtyCode = analysisMaterial.MaterialQTY;
                dr[DATA_ORDER] = i + 1;
                dr[MATERIAL_PN] = pnCode;
                dr[MATERIAL_LOT] = lotCode;
                dr[MATERIAL_RID] = ridCode;
                dr[MATERIAL_DC] = dcCode;
                dr[MATERIAL_QTY] = qtyCode;
                dr[MATERIAL_NAME] = serviceClient.SelectMaterialName(pnCode);
                var exType = dt.Rows[i][1].ToString();
                if (exType == "0")
                {
                    dr[EXCEPT_TYPE] = "库存物料异常";
                }
                else if (exType == "1")
                {
                    dr[EXCEPT_TYPE] = "生产物料异常";
                }
                else if (exType == "2")
                {
                    dr[EXCEPT_TYPE] = "生产过程异常";
                }
                dr[EXCEPT_STOCK] = dt.Rows[i][2].ToString();
                dr[ACTUAL_STOCK] = dt.Rows[i][3].ToString();
                var materialState = dt.Rows[i][4].ToString();
                if (materialState == "3")
                    materialState = "已结单";
                dr[MATERIAL_STATE] = materialState;
                dr[SHUT_REASON] = dt.Rows[i][5].ToString();
                dr[USER_NAME] = dt.Rows[i][6].ToString();
                dr[STATEMENT_DATE] = dt.Rows[i][7].ToString();
                dataSourceQuanlity.Rows.Add(dr);
            }
            this.radGridViewQuanlity.DataSource = dataSourceQuanlity;
            this.radGridViewQuanlity.Columns[0].BestFit();
        }

        /// <summary>
        /// 根据物料编码查询物料被使用于哪些产品
        /// </summary>
        async private void SelectOfMaterial()
        {
            //物料信息表
            //物料编码+物料名称+所属型号+在哪个工序/站位消耗+该位置消耗数量
            var ds = await serviceClient.SelectMaterialBasicMsgAsync(this.tb_material.Text);
            if (ds.Tables.Count < 1)
            {
                this.dataSourceMaterialBasic.Clear();
                return;
            }
            var dt = ds.Tables[0];
            this.dataSourceMaterialBasic.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dataSourceMaterialBasic.NewRow();
                var materialCode = dt.Rows[i][0].ToString();//pn/lot/rid/dc/qty
                //var materialName = dt.Rows[i][1].ToString();
                var productTypeNo = dt.Rows[i][2].ToString();
                var useAmounted = dt.Rows[i][3].ToString();
                var sn = dt.Rows[i][4].ToString();
                var amountTotal = dt.Rows[i][5].ToString();
                var putInStorage = dt.Rows[i][6].ToString();
                var currentRemain = dt.Rows[i][7].ToString();
                var snPCBA = serviceClient.GetPCBASn(sn);
                var snOutter = serviceClient.GetProductSn(sn);
                if (!materialCode.Contains("&"))
                    continue;
                AnalysisMaterialCode analysisMaterial = AnalysisMaterialCode.GetMaterialDetail(materialCode);
                var pnCode = analysisMaterial.MaterialPN;
                var lotCode = analysisMaterial.MaterialLOT;
                var ridCode = analysisMaterial.MaterialRID;
                var dcCode = analysisMaterial.MaterialDC;
                //var qtyCode = analysisMaterial.MaterialQTY;
                var materialName = serviceClient.SelectMaterialName(pnCode);
                dr[DATA_ORDER] = i + 1;
                dr[MATERIAL_PN] = pnCode;
                dr[MATERIAL_LOT] = lotCode;
                dr[MATERIAL_RID] = ridCode;
                dr[MATERIAL_DC] = dcCode;
                dr[MATERIAL_QTY] = putInStorage;
                dr[MATERIAL_NAME] = materialName;
                dr[PRODUCT_TYPENO] = productTypeNo;
                dr[USE_AMOUNTED] = useAmounted;
                dr[RESIDUE_STOCK] = int.Parse(putInStorage) - int.Parse(amountTotal);
                dr[CURRENT_RESIDUE_STOCK] = currentRemain;

                dr[SN_PCBA] = snPCBA;
                dr[SN_OUTTER] = snOutter;
                dataSourceMaterialBasic.Rows.Add(dr);
            }
            this.radGridViewMaterial.DataSource = dataSourceMaterialBasic;
            this.radGridViewMaterial.Columns[0].BestFit();
        }

        private void Btn_selectOfSn_Click(object sender, EventArgs e)
        {
            SelectOfSn();
        }

        private void Btn_selectOfPackage_Click(object sender, EventArgs e)
        {
            SelectOfPackage("1");
        }
    }
}
