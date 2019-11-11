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
using MesManager.Control.TreeViewUI;
using MesManager.Properties;
using WindowsFormTelerik.GridViewExportData;
using Telerik.WinControls.UI.Export;
using CommonUtils.Logger;
using CommonUtils.FileHelper;

namespace MesManager.UI
{
    public partial class TestStand : RadForm
    {
        private MesService.MesServiceClient serviceClient;
        private TestStandDataType currentDataType;
        private const string LOG_ORDER = "序号";
        private const string LOG_TYPE_NO = "产品型号";
        private const string LOG_SN = "产品SN";
        private const string LOG_STATION_NAME = "工站名称";
        private const string LOG_TEST_RESULT = "测试结果";
        private DataTable dataSource;
        private string currentQueryCondition;

        public TestStand()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
        }

        enum TestStandDataType
        {
            TEST_LIMIT_CONFIG,
            TEST_PROGRAME_VERSION,
            TEST_LOG_DATA
        }

        private void TestStand_Load(object sender, EventArgs e)
        {
            Init();
            InitDataTable();
            EventHandlers();
        }

        private void EventHandlers()
        {
            tool_logData.Click += Tool_logData_Click;
            tool_specCfg.Click += Tool_specCfg_Click;
            tool_programv.Click += Tool_refresh_Click;
            tool_queryCondition.SelectedIndexChanged += Tool_productTypeNo_SelectedIndexChanged;
            this.radGridView1.CellDoubleClick += RadGridView1_CellDoubleClick;
            this.tool_export.Click += Tool_export_Click;
            this.rbtn_today.Click += Rbtn_today_Click;
            this.rbtn_oneMonth.Click += Rbtn_oneMonth_Click;
            this.rbtn_threeMonth.Click += Rbtn_threeMonth_Click;
            this.rbtn_oneYear.Click += Rbtn_oneYear_Click;
            this.tool_clearDB.Click += Tool_clearDB_Click;
            this.tool_query.Click += Tool_query_Click;
        }

        private void Tool_query_Click(object sender, EventArgs e)
        {
            RefreshUI();
        }

        private void Tool_clearDB_Click(object sender, EventArgs e)
        {
            if (currentDataType == TestStandDataType.TEST_LIMIT_CONFIG)
            {
                DeleteTestLimitConfig();
            }
            else if (currentDataType == TestStandDataType.TEST_LOG_DATA)
            {
                TestLogDetailDelete();
            }
            else if (currentDataType == TestStandDataType.TEST_PROGRAME_VERSION)
            {
                DeleteTestProgramVersion();
            }
        }

        private void Rbtn_oneYear_Click(object sender, EventArgs e)
        {
            RefreshUI();
        }

        private void Rbtn_threeMonth_Click(object sender, EventArgs e)
        {
            RefreshUI();
        }

        private void Rbtn_oneMonth_Click(object sender, EventArgs e)
        {
            RefreshUI();
        }

        private void Rbtn_today_Click(object sender, EventArgs e)
        {
            RefreshUI();
        }

        private void InitDataTable()
        {
            if (dataSource == null)
            {
                dataSource = new DataTable();
                dataSource.Columns.Add(LOG_ORDER);
                dataSource.Columns.Add(LOG_TYPE_NO);
                dataSource.Columns.Add(LOG_SN);
                dataSource.Columns.Add(LOG_STATION_NAME);
                dataSource.Columns.Add(LOG_TEST_RESULT);
            }
        }

        private void Tool_export_Click(object sender, EventArgs e)
        {
            ExportGridViewData();
        }

        private void RadGridView1_CellDoubleClick(object sender, GridViewCellEventArgs e)
        {
            if (currentDataType != TestStandDataType.TEST_LOG_DATA)
                return;
            var productTypeNo = this.radGridView1.CurrentRow.Cells[1].Value.ToString();
            var productSN = this.radGridView1.CurrentRow.Cells[2].Value.ToString();
            var stationName = this.radGridView1.CurrentRow.Cells[3].Value.ToString();
            TestLogDetail testLogDetail = new TestLogDetail(productSN);
            //testLogDetail.ShowDialog();
        }

        private void Tool_productTypeNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshUI();
        }

        private void Tool_refresh_Click(object sender, EventArgs e)
        {
            currentDataType = TestStandDataType.TEST_PROGRAME_VERSION;
            this.panel2.Visible = false;
            RefreshUI();
        }

        private void Tool_logData_Click(object sender, EventArgs e)
        {
            currentDataType = TestStandDataType.TEST_LOG_DATA;
            this.panel2.Visible = true;
            RefreshUI();
        }

        private void Tool_specCfg_Click(object sender, EventArgs e)
        {
            currentDataType = TestStandDataType.TEST_LIMIT_CONFIG;
            this.panel2.Visible = false;
            RefreshUI();
        }

        private void RefreshUI()
        {
            currentQueryCondition = this.tool_queryCondition.Text;
            if (currentDataType == TestStandDataType.TEST_LIMIT_CONFIG)
            {
                SelectTestLimitConfig(this.tool_queryCondition.Text);
                this.radGridView1.Dock = DockStyle.Fill;
                this.radGridView1.Visible = true;
                this.panel1.Visible = false;
            }
            else if (currentDataType == TestStandDataType.TEST_LOG_DATA)
            {
                var startTime = "";
                var endTime = "";
                if (rbtn_today.Checked)
                {
                    startTime = DateTime.Now.ToString("yyyy-MM-dd")+" 00:00:00";
                    endTime = DateTime.Now.ToString("yyyy-MM-dd")+" 23:59:59";
                }
                else if (rbtn_oneMonth.Checked)
                {
                    startTime = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd") + " 00:00:00";
                    endTime = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
                }
                else if (rbtn_threeMonth.Checked)
                {
                    startTime = DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd") + " 00:00:00";
                    endTime = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
                }
                else if (rbtn_oneYear.Checked)
                {
                    startTime = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd") + " 00:00:00";
                    endTime = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
                }
                else if (rbtn_custom.Checked)
                {
                    startTime = this.pickerStartTime.Text;
                    endTime = this.pickerEndTime.Text;
                }
                //SelectTestLogData(this.tool_queryCondition.Text,startTime,endTime);
                SelectTestResultDetail(this.tool_queryCondition.Text,startTime,endTime);
                this.radGridView1.Dock = DockStyle.Fill;
                this.radGridView1.Visible = true;
                this.panel1.Visible = false;
            }
            else if (currentDataType == TestStandDataType.TEST_PROGRAME_VERSION)
            {
                SelectTestProgrameVersion(this.tool_queryCondition.Text);
                this.radGridView1.Dock = DockStyle.Fill;
                this.radGridView1.Visible = true;
                this.panel1.Visible = false;
            }
        }

        async private void Init()
        {
            serviceClient = new MesService.MesServiceClient();
            if (MESMainForm.currentUsetType != 0)
                this.tool_clearDB.Enabled = false;
            this.panel1.Visible = false;
            rbtn_today.Checked = true;
            this.radGridView1.Dock = DockStyle.Fill;
            DataGridViewCommon.SetRadGridViewProperty(this.radGridView1,false);
            this.radGridView1.ReadOnly = true;
            var dt = (await serviceClient.SelectProductContinairCapacityAsync("")).Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    this.tool_queryCondition.Items.Add(dt.Rows[i][0].ToString());
                }
            }
            //init treeview
            string path = @"D:\work\project\FigKey\RetrospectiveSystem\project\IIS";
            ImageList imageList = new ImageList();
            imageList.Images.Add("open", Resources.FolderList32);
            LoadTreeView.SetTreeNoByFilePath(this.treeView1,path,new ImageList());
            //TreeViewData.PopulateTreeView(path, this.treeView1);

            currentDataType = TestStandDataType.TEST_LOG_DATA;
            this.panel2.Visible = true;
            this.pickerStartTime.Text = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
            this.pickerEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
            this.tool_exportCondition.Items.Add(GridViewExport.ExportFormat.EXCEL.ToString());
            this.tool_exportCondition.Items.Add(GridViewExport.ExportFormat.HTML.ToString());
            this.tool_exportCondition.Items.Add(GridViewExport.ExportFormat.PDF.ToString());
            this.tool_exportCondition.Items.Add(GridViewExport.ExportFormat.CSV.ToString());
            this.tool_exportCondition.SelectedIndex = 0;
            RefreshUI();
        }

        async private void SelectTestLimitConfig(string productTypeNo)
        {
            var dt = (await serviceClient.SelectTestLimitConfigAsync(productTypeNo)).Tables[0];
            this.radGridView1.DataSource = null;
            this.radGridView1.DataSource = dt;
            this.radGridView1.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
            this.radGridView1.Columns[0].BestFit();
        }

        async private void SelectTestProgrameVersion(string productTypeNo)
        {
            var dt = (await serviceClient.SelectTestProgrameVersionAsync(productTypeNo)).Tables[0];
            this.radGridView1.DataSource = null;
            this.radGridView1.DataSource = dt;
            this.radGridView1.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
            this.radGridView1.Columns[0].BestFit();
        }

        async private void SelectTestLogData(string queryFilter,string startTime,string endTime)
        {
            var dt = (await serviceClient.SelectTodayTestLogDataAsync(queryFilter,startTime,endTime)).Tables[0];
            dataSource.Clear();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dataSource.NewRow();
                    dr[LOG_ORDER] = i + 1;
                    dr[LOG_TYPE_NO] = dt.Rows[i][0].ToString();
                    var productSn = dt.Rows[i][1].ToString();
                    dr[LOG_SN] = productSn;
                    dr[LOG_STATION_NAME] = dt.Rows[i][2].ToString();
                    dr[LOG_TEST_RESULT] = serviceClient.SelectLastLogTestResult(productSn);
                    dataSource.Rows.Add(dr);
                }
            }
            this.radGridView1.DataSource = dataSource;
            this.radGridView1.Columns[0].BestFit();
        }

        async private void SelectTestResultDetail(string queryFilter, string startTime, string endTime)
        {
            var ds = await serviceClient.SelectTestResultLogDetailAsync(queryFilter, startTime, endTime);
            if (ds.Tables.Count < 1)
            {
                this.radGridView1.DataSource = null;
                return;
            }
            var dt = ds.Tables[0];
            this.radGridView1.DataSource = null;
            this.radGridView1.DataSource = dt;
            this.radGridView1.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.None;
            this.radGridView1.BestFitColumns();
        }

        private void ExportGridViewData()
        {
            GridViewExport.ExportFormat exportFormat = GridViewExport.ExportFormat.EXCEL;
            Enum.TryParse(tool_queryCondition.Text, out exportFormat);
            GridViewExport.ExportGridViewData(exportFormat, this.radGridView1);
        }

        private void TestLogDetailDelete()
        {
            var startTime = "";
            var endTime = "";
            if (rbtn_today.Checked)
            {
                startTime = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
                endTime = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
            }
            else if (rbtn_oneMonth.Checked)
            {
                startTime = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd") + " 00:00:00";
                endTime = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
            }
            else if (rbtn_threeMonth.Checked)
            {
                startTime = DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd") + " 00:00:00";
                endTime = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
            }
            else if (rbtn_oneYear.Checked)
            {
                startTime = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd") + " 00:00:00";
                endTime = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
            }
            else if (rbtn_custom.Checked)
            {
                startTime = this.pickerStartTime.Text;
                endTime = this.pickerEndTime.Text;
            }
            if (this.radGridView1.RowCount < 1)
            {
                MessageBox.Show("没有可以清除的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("是否确认清除当前所有数据？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning,MessageBoxDefaultButton.Button2) != DialogResult.OK)
                return;
            var returnRes = serviceClient.DeleteTestLogData(this.currentQueryCondition.Trim(),startTime,endTime);
            if (returnRes == "0X01")
            {
                RefreshUI();
                MessageBox.Show("清除数据完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("清除数据未完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DeleteTestProgramVersion()
        {
            if (this.radGridView1.RowCount < 1)
            {
                MessageBox.Show("没有可以清除的数据！","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("是否确认清除当前所有数据？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                return;
            var returnRes = serviceClient.DeleteTestProgrameVersion(this.currentQueryCondition.Trim());
            if (returnRes > 0)
            {
                RefreshUI();
                MessageBox.Show("清除数据完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            MessageBox.Show("清除数据未完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DeleteTestLimitConfig()
        {
            if (this.radGridView1.RowCount < 1)
            {
                MessageBox.Show("没有可以清除的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("是否确认清除当前所有数据？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                return;
            var returnRes = serviceClient.DeleteTestLimitConfig(this.currentQueryCondition.Trim());
            if (returnRes > 0)
            {
                RefreshUI();
                MessageBox.Show("清除数据完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            MessageBox.Show("清除数据未完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
