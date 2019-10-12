using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using MesManager.Control;
using MesManager.TelerikWinform.GridViewCommon.GridViewDataExport;
using Telerik.WinControls.UI.Export;
using Telerik.WinControls.UI;
using CommonUtils.Logger;
using CommonUtils.FileHelper;
using Telerik.WinControls.Data;

namespace MesManager.UI
{
    public partial class TestLogDetail : RadForm
    {
        private string startTime;
        private string productSn;
        private string endTime;
        private MesService.MesServiceClient serviceClient;
        private DataTable dataSourceLog;
        #region log详细数据
        private const string LOG_ORDER = "序号";
        private const string LOG_TEST_GROUP_ID = "测试次数";
        private const string LOG_PRODUCT_TYPE_NO = "产品型号";
        private const string LOG_PRODUCT_SN = "产品SN";
        private const string LOG_STATION_NAME = "工站名称";
        private const string LOG_TEST_ITEM = "测试项";
        private const string LOG_TEST_RESULT = "测试结果";
        private const string LOG_LIMIT = "LIMIT";
        private const string LOG_CURRENT_VALUE = "当前值";
        private const string LOG_LEADER = "班组长";
        private const string LOG_ADMIN = "管理员";
        private const string LOG_UPDATE_DATE = "更新日期";
        #endregion
        public TestLogDetail(string sn)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            this.productSn = sn;
        }

        private void TestLogDetail_Load(object sender, EventArgs e)
        {
            InitLogDataSource();
            serviceClient = new MesService.MesServiceClient();
            DataGridViewCommon.SetRadGridViewProperty(this.radGridView1, false);
            SetGroup();
            this.radGridView1.ReadOnly = true;
            this.pickerStartTime.Text = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
            this.pickerEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
            var ds = serviceClient.SelectTestLogDataDetail(productSn, "", "");
            SetConditions();
            LoadDataSource(ds);
        }

        private void LoadDataSource(DataSet ds)
        {
            if (ds == null)
                return;
            dataSourceLog.Clear();
            var dt = ds.Tables[0];
            int groupID = 1;//测试次数默认为第一组开始
            var order = 1;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var testItem = dt.Rows[i][4].ToString().Trim();
                var testResult = dt.Rows[i][5].ToString().Trim();
                var limit = dt.Rows[i][6].ToString().Trim();
                var currentLimit = dt.Rows[i][7].ToString().Trim();
                if (testItem == "" && testResult == "" && limit == "" && currentLimit == "")
                {
                    //第一组结束
                    groupID++;
                    order = 1;//新一组数据的起始序号
                }
                else
                {
                    DataRow dr = dataSourceLog.NewRow();
                    dr[LOG_ORDER] = order; 
                    dr[LOG_TEST_GROUP_ID] = groupID.ToString().PadLeft(3,'0');
                    dr[LOG_PRODUCT_TYPE_NO] = dt.Rows[i][1].ToString();
                    dr[LOG_PRODUCT_SN] = dt.Rows[i][2].ToString();
                    dr[LOG_STATION_NAME] = dt.Rows[i][3].ToString();
                    dr[LOG_TEST_ITEM] = testItem;
                    dr[LOG_TEST_RESULT] = testResult;
                    dr[LOG_LIMIT] = limit;
                    dr[LOG_CURRENT_VALUE] = currentLimit;
                    dr[LOG_LEADER] = dt.Rows[i][8].ToString();
                    dr[LOG_ADMIN] = dt.Rows[i][9].ToString();
                    dr[LOG_UPDATE_DATE] = dt.Rows[i][10].ToString();
                    dataSourceLog.Rows.Add(dr);
                    order++;
                }
            }
            this.radGridView1.DataSource = null;
            this.radGridView1.DataSource = dataSourceLog;
            this.radGridView1.Columns[0].BestFit();

        }

        private void InitLogDataSource()
        {
            dataSourceLog = new DataTable();
            dataSourceLog.Columns.Add(LOG_ORDER);
            dataSourceLog.Columns.Add(LOG_TEST_GROUP_ID);
            dataSourceLog.Columns.Add(LOG_PRODUCT_TYPE_NO);
            dataSourceLog.Columns.Add(LOG_PRODUCT_SN);
            dataSourceLog.Columns.Add(LOG_STATION_NAME);
            dataSourceLog.Columns.Add(LOG_TEST_ITEM);
            dataSourceLog.Columns.Add(LOG_TEST_RESULT);
            dataSourceLog.Columns.Add(LOG_LIMIT);
            dataSourceLog.Columns.Add(LOG_CURRENT_VALUE);
            dataSourceLog.Columns.Add(LOG_LEADER);
            dataSourceLog.Columns.Add(LOG_ADMIN);
            dataSourceLog.Columns.Add(LOG_UPDATE_DATE);
        }

        private void SetGroup()
        {
            this.radGridView1.ShowGroupPanel = false;
            this.radGridView1.MasterTemplate.EnableGrouping = true;
            this.radGridView1.MasterTemplate.AllowDragToGroup = false;
            this.radGridView1.MasterTemplate.AutoExpandGroups = false;
        }

        private void SetConditions()
        {
            GroupDescriptor descriptor = new GroupDescriptor();
            descriptor.GroupNames.Add(LOG_TEST_GROUP_ID, ListSortDirection.Ascending);
            descriptor.Aggregates.Add($"Count({LOG_TEST_GROUP_ID})");
            descriptor.Format = "{0}: 第【{1}】次测试，包含【{2}】条数据 ";
            this.radGridView1.GroupDescriptors.Add(descriptor);
        }

        async private void Btn_search_Click(object sender, EventArgs e)
        {
            var startTime = Convert.ToDateTime(this.pickerStartTime.Text);
            var endTime = Convert.ToDateTime(this.pickerEndTime.Text);
            var ds = await serviceClient.SelectTestLogDataDetailAsync(productSn,startTime.ToString(),endTime.ToString());
            LoadDataSource(ds);
        }

        private void Btn_export_Click(object sender, EventArgs e)
        {
            ExportGridViewData(0,this.radGridView1);
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

        private enum ExportFormat
        {
            EXCEL,
            HTML,
            PDF,
            CSV
        }
    }
}
