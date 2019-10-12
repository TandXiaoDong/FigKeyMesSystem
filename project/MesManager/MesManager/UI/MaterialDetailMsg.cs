using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using MesManager.Control;
using CommonUtils.FileHelper;
using MesManager.TelerikWinform.GridViewCommon.GridViewDataExport;
using Telerik.WinControls.UI;
using MesManager.Common;

namespace MesManager.UI
{
    public partial class MaterialDetailMsg : RadForm
    {
        private MesService.MesServiceClient serviceClient;
        private DataTable dataSourceMaterialDetail;
        private const string MATERIAL_PN = "物料号";
        private const string MATERIAL_LOT = "批次号";
        private const string MATERIAL_RID = "料盘号";
        private const string MATERIAL_DC = "收料日期";
        private const string MATERIAL_QTY = "入库库存";
        private const string MATERIAL_NAME = "物料名称";
        private const string PRODUCT_TYPENO = "产品型号";
        private const string STATION_NAME = "工站名称";
        private const string USE_AMOUNTED = "当前使用数量";
        private const string RESIDUE_STOCK = "入库剩余库存";
        private const string CURRENT_REMAIN_STOCK = "当前剩余库存";
        private const string TEAM_LEADER = "班组长";
        private const string ADMIN = "管理员";
        private const string UPDATE_DATE = "更新日期";
        private const string SN_PCBA = "PCBA";
        private const string SN_OUTTER = "外壳";

        private string materialCode;
        public MaterialDetailMsg(string materialCode)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            this.materialCode = materialCode;
        }

        private enum ExportFormat
        {
            EXCEL,
            HTML,
            PDF,
            CSV
        }

        private void InitDataTable()
        {
            if (dataSourceMaterialDetail == null)
            {
                dataSourceMaterialDetail = new DataTable();
                dataSourceMaterialDetail.Columns.Add(MATERIAL_PN);
                dataSourceMaterialDetail.Columns.Add(MATERIAL_LOT);
                dataSourceMaterialDetail.Columns.Add(MATERIAL_RID);
                dataSourceMaterialDetail.Columns.Add(MATERIAL_DC);
                dataSourceMaterialDetail.Columns.Add(MATERIAL_NAME);
                dataSourceMaterialDetail.Columns.Add(PRODUCT_TYPENO);
                dataSourceMaterialDetail.Columns.Add(SN_PCBA);
                dataSourceMaterialDetail.Columns.Add(SN_OUTTER);
                dataSourceMaterialDetail.Columns.Add(STATION_NAME);
                dataSourceMaterialDetail.Columns.Add(MATERIAL_QTY);
                dataSourceMaterialDetail.Columns.Add(USE_AMOUNTED);
                dataSourceMaterialDetail.Columns.Add(CURRENT_REMAIN_STOCK);
                dataSourceMaterialDetail.Columns.Add(RESIDUE_STOCK);
                dataSourceMaterialDetail.Columns.Add(TEAM_LEADER);
                dataSourceMaterialDetail.Columns.Add(ADMIN);
                dataSourceMaterialDetail.Columns.Add(UPDATE_DATE);
            }
        }

        private void MaterialDetailMsg_Load(object sender, EventArgs e)
        {
            this.tool_exportFilter.Items.Clear();
            this.tool_exportFilter.Items.Add(ExportFormat.EXCEL);
            this.tool_exportFilter.Items.Add(ExportFormat.HTML);
            this.tool_exportFilter.Items.Add(ExportFormat.PDF);
            this.tool_exportFilter.Items.Add(ExportFormat.CSV);
            this.tool_exportFilter.SelectedIndex = 0;
            serviceClient = new MesService.MesServiceClient();
            DataGridViewCommon.SetRadGridViewProperty(this.radGridView1,false);
            this.radGridView1.ReadOnly = true;
            InitDataTable();
            SelectMaterialDetail(this.materialCode);
        }

        async private void SelectMaterialDetail(string inMaterialCode)
        {
            var dt = (await serviceClient.SelectMaterialDetailMsgAsync(inMaterialCode)).Tables[0];
            if (dt.Rows.Count < 1)
                return;
            this.dataSourceMaterialDetail.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dataSourceMaterialDetail.NewRow();
                var materialCode = dt.Rows[i][0].ToString();//pn/lot/rid/dc/qty
                var materialName = dt.Rows[i][1].ToString();
                var productTypeNo = dt.Rows[i][2].ToString();
                var stationName = dt.Rows[i][3].ToString();
                var useAmounted = dt.Rows[i][4].ToString();
                var teamLeader = dt.Rows[i][5].ToString();
                var admin = dt.Rows[i][6].ToString();
                var updateDate = dt.Rows[i][7].ToString();
                var sn = dt.Rows[i][8].ToString();
                var amountedTotal = dt.Rows[i][9].ToString();
                var putInStorage = dt.Rows[i][10].ToString();
                var currentRemain = dt.Rows[i][11].ToString();
                var snPCBA = serviceClient.GetPCBASn(sn);
                var snOutter = serviceClient.GetProductSn(sn);
                AnalysisMaterialCode analysisMaterialCode = AnalysisMaterialCode.GetMaterialDetail(materialCode);
                var pnCode = analysisMaterialCode.MaterialPN;
                var lotCode = analysisMaterialCode.MaterialLOT;
                var ridCode = analysisMaterialCode.MaterialRID;
                var dcCode = analysisMaterialCode.MaterialDC;
                //var qtyCode = analysisMaterialCode.MaterialQTY;
                materialName = serviceClient.SelectMaterialName(pnCode);
                dr[MATERIAL_PN] = pnCode;
                dr[MATERIAL_LOT] = lotCode;
                dr[MATERIAL_RID] = ridCode;
                dr[MATERIAL_DC] = dcCode;
                dr[MATERIAL_QTY] = putInStorage;
                dr[MATERIAL_NAME] = materialName;
                dr[PRODUCT_TYPENO] = productTypeNo;
                dr[STATION_NAME] = stationName;
                dr[USE_AMOUNTED] = useAmounted;
                dr[TEAM_LEADER] = teamLeader;
                dr[ADMIN] = admin;
                dr[UPDATE_DATE] = updateDate;
                dr[SN_PCBA] = snPCBA;
                dr[SN_OUTTER] = snOutter;
                dr[RESIDUE_STOCK] = int.Parse(putInStorage) - int.Parse(amountedTotal);
                dr[CURRENT_REMAIN_STOCK] = currentRemain;
                dataSourceMaterialDetail.Rows.Add(dr);
            }
            this.radGridView1.DataSource = dataSourceMaterialDetail;
        }

        private void ExportGridViewData(int selectIndex, RadGridView radGridView)
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

        private void Tool_sn_export_Click(object sender, EventArgs e)
        {
            ExportGridViewData(this.tool_exportFilter.SelectedIndex,this.radGridView1);
        }
    }
}
