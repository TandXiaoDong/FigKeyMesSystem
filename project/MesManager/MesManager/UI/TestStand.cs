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
        private const string LOG_ORDER = "序号";
        private const string LOG_TYPE_NO = "产品型号";
        private const string LOG_SN = "产品SN";
        private const string LOG_STATION_NAME = "工站名称";
        private const string LOG_TEST_RESULT = "测试结果";
        private DataTable dataSource;
        private string currentQueryCondition;
        private string startTime = "";
        private string endTime = "";
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

        /// <summary>
        /// 绑定分页总页数
        /// </summary>
        private DataTable bindRowSource;

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
            this.radDock1.ActiveWindowChanged += RadDock1_ActiveWindowChanged;
            tool_queryCondition.SelectedIndexChanged += Tool_productTypeNo_SelectedIndexChanged;
            this.radGridView1.CellDoubleClick += RadGridView1_CellDoubleClick;
            this.tool_export.Click += Tool_export_Click;
            this.rbtn_today.Click += Rbtn_today_Click;
            this.rbtn_oneMonth.Click += Rbtn_oneMonth_Click;
            this.rbtn_threeMonth.Click += Rbtn_threeMonth_Click;
            this.rbtn_oneYear.Click += Rbtn_oneYear_Click;
            this.tool_clearDB.Click += Tool_clearDB_Click;
            this.tool_query.Click += Tool_query_Click;
            this.bindingNavigator1.ItemClicked += BindingNavigator1_ItemClicked;
            this.bindingNavigatorCountItem.TextChanged += BindingNavigatorCountItem_TextChanged;
            this.bindingNavigatorPositionItem.TextChanged += BindingNavigatorPositionItem_TextChanged;
        }

        private void Tool_query_Click(object sender, EventArgs e)
        {
            ResetCurrentPage();
            RefreshUI();
        }

        private void Tool_clearDB_Click(object sender, EventArgs e)
        {
            if (this.radDock1.ActiveWindow == this.tool_logData)
            {
                TestLogDetailDelete();
            }
            else if (this.radDock1.ActiveWindow == this.tool_programv)
            {
                DeleteTestProgramVersion();
            }
            else if (this.radDock1.ActiveWindow == this.tool_specCfg)
            {
                DeleteTestLimitConfig();
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

        private void RadDock1_ActiveWindowChanged(object sender, Telerik.WinControls.UI.Docking.DockWindowEventArgs e)
        {
            if (this.radDock1.ActiveWindow == this.tool_logData)
            {
                
            }
            else if (this.radDock1.ActiveWindow == this.tool_specCfg)
            {
                
            }
            else if (this.radDock1.ActiveWindow == this.tool_programv)
            {
                
            }
        }

        private void RefreshUI()
        {
            currentQueryCondition = this.tool_queryCondition.Text;
            if (this.radDock1.ActiveWindow == this.tool_logData)
            {
                SelectTestResultDetail();
            }
            else if (this.radDock1.ActiveWindow == this.tool_specCfg)
            {
                SelectTestLimitConfig();
            }
            else if (this.radDock1.ActiveWindow == this.tool_programv)
            {
                SelectTestProgrameVersion();
            }
        }

        async private void Init()
        {
            serviceClient = new MesService.MesServiceClient();
            if (MESMainForm.currentUsetType != 0)
                this.tool_clearDB.Enabled = false;
            rbtn_today.Checked = true;
            this.radGridView1.Dock = DockStyle.Fill;
            DataGridViewCommon.SetRadGridViewProperty(this.radGridView1,false);
            DataGridViewCommon.SetRadGridViewProperty(this.gridProgrameVersion, false);
            DataGridViewCommon.SetRadGridViewProperty(this.gridSpec, false);
            this.radGridView1.ReadOnly = true;
            this.gridSpec.ReadOnly = true;
            this.gridProgrameVersion.ReadOnly = true;
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
            //LoadTreeView.SetTreeNoByFilePath(this.treeView1,path,new ImageList());
            //TreeViewData.PopulateTreeView(path, this.treeView1);
            this.pickerStartTime.Text = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
            this.pickerEndTime.Text = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
            this.tool_exportCondition.Items.Add(GridViewExport.ExportFormat.EXCEL.ToString());
            this.tool_exportCondition.Items.Add(GridViewExport.ExportFormat.HTML.ToString());
            this.tool_exportCondition.Items.Add(GridViewExport.ExportFormat.PDF.ToString());
            this.tool_exportCondition.Items.Add(GridViewExport.ExportFormat.CSV.ToString());
            this.tool_exportCondition.SelectedIndex = 0;

            this.radDock1.ActiveWindow = this.tool_logData;
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

        async private void SelectTestLimitConfig()
        {
            if (this.tool_queryCondition.Text != "")
            {
                this.currentPage = 1;//根据条件查询
                this.bindingNavigatorPositionItem.Text = currentPage.ToString();
            }
            this.gridSpec.DataSource = null;
            this.gridSpec.Update();
            var specLimitObj = (await serviceClient.SelectTestLimitConfigAsync(this.tool_queryCondition.Text, currentPage,pageSize));
            if (specLimitObj.SpecHistoryNumber % pageSize > 0)
            {
                pageCount = specLimitObj.SpecHistoryNumber / pageSize + 1;
            }
            else
            {
                pageCount = specLimitObj.SpecHistoryNumber / pageSize;
            }
            //bindSource
            var dtSource = InitBindRowSource();
            bindingSource1.DataSource = dtSource;
            this.bindingNavigator1.BindingSource = bindingSource1;
            this.gridSpec.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
            this.gridSpec.BeginEdit();
            this.gridSpec.DataSource = null;
            var dt = specLimitObj.SpecDataSet.Tables[0];
            this.gridSpec.DataSource = dt;
            this.gridSpec.EndEdit();
            this.gridSpec.Columns[0].BestFit();
        }

        async private void SelectTestProgrameVersion()
        {
            if (this.tool_queryCondition.Text != "")
            {
                this.currentPage = 1;//根据条件查询
                this.bindingNavigatorPositionItem.Text = currentPage.ToString();
            }
            this.gridProgrameVersion.DataSource = null;
            this.gridProgrameVersion.Update();
            var programeObj = (await serviceClient.SelectTestProgrameVersionAsync(this.tool_queryCondition.Text,currentPage,pageSize));
            if (programeObj.ProgrameHistoryNumber % pageSize > 0)
            {
                pageCount = programeObj.ProgrameHistoryNumber / pageSize + 1;
            }
            else
            {
                pageCount = programeObj.ProgrameHistoryNumber / pageSize;
            }
            //bindSource
            var dtSource = InitBindRowSource();
            bindingSource1.DataSource = dtSource;
            this.bindingNavigator1.BindingSource = bindingSource1;
            var dt = programeObj.ProgrameDataSet.Tables[0];
            this.gridProgrameVersion.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
            this.gridProgrameVersion.BeginEdit();
            this.gridProgrameVersion.DataSource = null;
            this.gridProgrameVersion.DataSource = dt;
            this.gridProgrameVersion.EndEdit();
            this.gridProgrameVersion.Columns[0].BestFit();
        }

        async private void SelectTestResultDetail()
        {
            LogHelper.Log.Info("log查询-开始");

            if (this.tool_queryCondition.Text != "")
            {
                this.currentPage = 1;//根据条件查询
                this.bindingNavigatorPositionItem.Text = currentPage.ToString();
            }
            this.radGridView1.DataSource = null;
            this.radGridView1.Update();
            if (currentPage == 1)
            {
                #region update select date
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
                #endregion

                int totalNumber = await serviceClient.SelectTestResultLogLatestPageAsync(this.tool_queryCondition.Text, startTime, endTime);
                if (totalNumber % pageSize > 0)
                {
                    pageCount = totalNumber / pageSize + 1;
                }
                else
                {
                    pageCount = totalNumber / pageSize;
                }
                //bindSource
                var dtSource = InitBindRowSource();
                bindingSource1.DataSource = dtSource;
                this.bindingNavigator1.BindingSource = bindingSource1;
            }
            var ds = await serviceClient.SelectTestResultLogDetailAsync(currentPage,pageSize);
            if (ds.Tables.Count < 1)
            {
                this.radGridView1.DataSource = null;
                return;
            }
            LogHelper.Log.Info("log查询-结果查询完毕");
            var dt = ds.Tables[0];
            this.radGridView1.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.None;
            this.radGridView1.BeginEdit();
            this.radGridView1.DataSource = null;
            this.radGridView1.DataSource = dt;
            //this.bindingSource1.DataSource = dt;
            //this.bindingNavigator1.BindingSource = bindingSource1;
            this.radGridView1.EndEdit();
            this.radGridView1.BestFitColumns();
            LogHelper.Log.Info("log查询-显示完成");
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

        private void BindingNavigatorPositionItem_TextChanged(object sender, EventArgs e)
        {
            //this.bindingNavigatorPositionItem.Text = currentPage.ToString();
        }

        private void BindingNavigatorCountItem_TextChanged(object sender, EventArgs e)
        {
            //this.bindingNavigatorCountItem.Text = "/" + pageCount;
        }

        private void ResetCurrentPage()
        {
            this.currentPage = 1;//根据条件查询/点击查询-刷新最新数据
            this.bindingNavigatorPositionItem.Text = currentPage.ToString();
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
                RefreshUI();
            }
            else if (e.ClickedItem.Text == "下一页")
            {
                if (currentPage < pageCount)
                {
                    currentPage++;
                }
                RefreshUI();
            }
            else if (e.ClickedItem.Text == "首页")
            {
                currentPage = 1;
                RefreshUI();
            }
            else if (e.ClickedItem.Text == "尾页")
            {
                currentPage = pageCount;
                RefreshUI();
            }
            else if (e.ClickedItem.Text == "新添")
            {

            }
        }

        private void ExportGridViewData()
        {
            GridViewExport.ExportFormat exportFormat = GridViewExport.ExportFormat.EXCEL;
            Enum.TryParse(tool_queryCondition.Text, out exportFormat);
            if (this.radDock1.ActiveWindow == this.tool_logData)
            {
                GridViewExport.ExportGridViewData(exportFormat, this.radGridView1);
            }
            else if (this.radDock1.ActiveWindow == this.tool_specCfg)
            {
                GridViewExport.ExportGridViewData(exportFormat, this.gridSpec);
            }
            else if (this.radDock1.ActiveWindow == this.tool_programv)
            {
                GridViewExport.ExportGridViewData(exportFormat, this.gridProgrameVersion);
            }
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
