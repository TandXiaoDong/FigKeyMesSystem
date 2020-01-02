using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using MesManager.DB;
using System.Configuration;
using CommonUtils.DB;
using CommonUtils.Logger;

namespace TestAPI
{
    public partial class ExportDat : Form
    {
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

        private MesService.MesServiceClient serverClient;
        private MesServiceTest.MesServiceClient serverTest;
        //private DataTable dataSource = null;
        private QueryType queryType;

        public ExportDat()
        {
            InitializeComponent();
        }

        private enum QueryType
        {
            oldTable,
            newTable,
            newTableLog

        }

        private void ExportDat_Load(object sender, EventArgs e)
        {
            serverClient = new MesService.MesServiceClient();
            serverTest = new MesServiceTest.MesServiceClient();
            SQLServer.SqlConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            SetRadGridViewProperty(this.radGridView1,false);
            this.bindingNavigator1.ItemClicked += BindingNavigator1_ItemClicked;
            //this.radGridView1.VirtualMode = true;
            this.radGridView1.CellValuePushed += RadGridView1_CellValuePushed;
            this.radGridView1.CellValueNeeded += RadGridView1_CellValueNeeded;
            this.comboBox1.Items.AddRange(new string[] { "烧录工站", "灵敏度测试工站", "外壳装配工站", "气密测试工站", "支架装配工站", "成品测试工站" });
        }

        private void RadGridView1_CellValueNeeded(object sender, GridViewCellValueEventArgs e)
        {
            //e.Value = this.dataSource.Rows[e.RowIndex][e.ColumnIndex].ToString();
        }

        private void RadGridView1_CellValuePushed(object sender, GridViewCellValueEventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //查询数据
            ResetCurrentPage();
            queryType = QueryType.oldTable;
            SelectOfSn();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            //更新数据
            this.button2.Enabled = false;
            await Task.Run(() =>
            {
             serverTest.CopyDataSource();
            });
            this.button2.Enabled = true;
        }
        private void ResetCurrentPage()
        {
            this.currentPage = 1;//根据条件查询/点击查询-刷新最新数据
            this.bindingNavigatorPositionItem.Text = currentPage.ToString();
        }

        private async void SelectOfSn()
        {
            //LogHelper.Log.Info("开始查询...");
            //page
            var filter = textBox1.Text;
            if (filter != "")
            {
                this.currentPage = 1;//根据条件查询
                this.bindingNavigatorPositionItem.Text = currentPage.ToString();
            }
            this.radGridView1.DataSource = null;
            this.radGridView1.Update();
            //var pcbaHis = (await serviceClient.SelectUseAllPcbaSNAsync());
            //MessageBox.Show(pcbaHis.Length+"");
            //LogHelper.Log.Info("清空显示完毕...");
            //var testResultObj = await serviceClient.SelectTestResultDetailAsync(filter, currentPage, pageSize);
            TestResultQuery.TestResultHistory testResultObj = null;
            await Task.Run(() =>
            {
                testResultObj = TestResultQuery.SelectTestResultDetail(filter, currentPage, pageSize);
            });
            //LogHelper.Log.Info("查询数据完毕...");
            if (testResultObj.TestResultNumber % pageSize > 0)
            {
                pageCount = testResultObj.TestResultNumber / pageSize + 1;
            }
            else
            {
                pageCount = testResultObj.TestResultNumber / pageSize;
            }
            var dtSource = InitBindRowSource();
            this.radGridView1.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.None;
            this.radGridView1.BeginEdit();
            DataTable dt = testResultObj.TestResultDataSet.Tables[0];
            this.radGridView1.DataSource = dt;
            bindingSource1.DataSource = dtSource;
            this.bindingNavigator1.BindingSource = bindingSource1;
            this.radGridView1.EndEdit();
            this.radGridView1.BestFitColumns();
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

        public static void SetRadGridViewProperty(RadGridView radGridView, bool allowAddNewRow)
        {
            radGridView.EnableGrouping = false;
            radGridView.AllowDrop = true;
            radGridView.AllowRowReorder = true;
            //显示新行
            radGridView.AddNewRowPosition = SystemRowPosition.Bottom;
            radGridView.ShowRowHeaderColumn = true;
            radGridView.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
            //radGridView.AutoSizeRows = true;
            //radGridView.BestFitColumns();
            radGridView.ReadOnly = false;
            //gridView.ColumnChooserSortOrder = RadSortOrder.Ascending;
            //dgv.AllowRowHeaderContextMenu = false;
            radGridView.ShowGroupPanel = false;
            radGridView.MasterTemplate.EnableGrouping = false;
            radGridView.MasterTemplate.AllowAddNewRow = allowAddNewRow;
            radGridView.EnableHotTracking = true;
            radGridView.MasterTemplate.SelectLastAddedRow = false;
            //radRadioDataReader.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On;
            //this.radGridView1.CurrentRow = this.radGridView1.Rows[0];//设置某行为当前行

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
            if (queryType == QueryType.oldTable)
            {
                SelectOfSn();
            }
            else if (queryType == QueryType.newTable)
            {
                QueryNewData();
            }
            else if (queryType == QueryType.newTableLog)
            {
                QueryNewLogData();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            queryType = QueryType.newTable;
            QueryNewData();
        }

        private async void QueryNewData()
        {
            //查询新表
            var filter = textBox1.Text;
            if (filter != "")
            {
                this.currentPage = 1;//根据条件查询
                this.bindingNavigatorPositionItem.Text = currentPage.ToString();
            }
            this.radGridView1.DataSource = null;
            this.radGridView1.Update();
            LogHelper.Log.Info("开始查询...");
            var testResultObj = await serverClient.SelectTestResultHistoryAsync(this.textBox1.Text, currentPage, pageSize);
            LogHelper.Log.Info("查询数据完毕...");
            if (testResultObj.TestResultNumber % pageSize > 0)
            {
                pageCount = testResultObj.TestResultNumber / pageSize + 1;
            }
            else
            {
                pageCount = testResultObj.TestResultNumber / pageSize;
            }
            var dtSource = InitBindRowSource();
            this.radGridView1.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.None;
            this.radGridView1.BeginEdit();
            DataTable dt = testResultObj.TestResultDataSet.Tables[0];
            this.radGridView1.DataSource = dt;
            bindingSource1.DataSource = dtSource;
            this.bindingNavigator1.BindingSource = bindingSource1;
            this.radGridView1.EndEdit();
            this.radGridView1.BestFitColumns();
            LogHelper.Log.Info("显示完毕...");
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            //更新PCB
            this.button4.Enabled = false;
            await serverTest.UpdateAllPcbBindAsync();
            this.button4.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            queryType = QueryType.newTableLog;
            QueryNewLogData();
        }

        private async void QueryNewLogData()
        {
            //查询新表
            var filter = textBox1.Text;
            if (filter != "")
            {
                this.currentPage = 1;//根据条件查询
                this.bindingNavigatorPositionItem.Text = currentPage.ToString();
            }
            this.radGridView1.DataSource = null;
            this.radGridView1.Update();
            LogHelper.Log.Info("开始查询...");
            var startDate = "2017-01-01";
            var endDate = "2020-02-02";
            var testResultObj = await serverClient.SelectTestResultLogHistoryAsync(this.textBox1.Text,startDate,endDate ,currentPage, pageSize);
            LogHelper.Log.Info("查询数据完毕...");
            if (testResultObj.TestResultNumber % pageSize > 0)
            {
                pageCount = testResultObj.TestResultNumber / pageSize + 1;
            }
            else
            {
                pageCount = testResultObj.TestResultNumber / pageSize;
            }
            var dtSource = InitBindRowSource();
            this.radGridView1.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.None;
            this.radGridView1.BeginEdit();
            DataTable dt = testResultObj.TestResultDataSet.Tables[0];
            this.radGridView1.DataSource = dt;
            bindingSource1.DataSource = dtSource;
            this.bindingNavigator1.BindingSource = bindingSource1;
            this.radGridView1.EndEdit();
            this.radGridView1.BestFitColumns();
            LogHelper.Log.Info("显示完毕...");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //查询上一站
            //第一个站或不是
            var station = this.comboBox1.SelectedItem.ToString();
            var pid = "017 B19A17011990";
            var tid = "A571E20311K112600998HE00110123";
            if (station == "烧录工站")
            {
                serverTest.SelectLastTestResult(pid, station);
            }
            else if (station == "灵敏度测试工站")
            {
                serverTest.SelectLastTestResult(pid, station);
            }
            else if (station == "外壳装配工站")
            {
                serverTest.SelectLastTestResult(pid, station);
            }
            else if (station == "气密测试工站")
            {
                serverTest.SelectLastTestResult(tid, station);
            }
            else if (station == "支架装配工站")
            {
                serverTest.SelectLastTestResult(tid, station);
            }
            else if (station == "成品测试工站")
            {
                serverTest.SelectLastTestResult(tid, station);
            }
        }

        private void btn_testItem_Click(object sender, EventArgs e)
        {
            var station = this.comboBox1.Text;
            var result = "";
            if (station == "烧录工站")
            {
                result = serverTest.UpdateTestLog("HTS-B2004-03-02", station, "017 B19A17011990", "烧录", "limit001", "passed", "passed", "adm", "adm", "2020-01-01-01");
            }
            else if (station == "灵敏度测试工站")
            {
                result = serverTest.UpdateTestLog("HTS-B2004-03-02", station, "017 B19A17011990", "工作电流", "limit001", "passed", "passed", "adm", "adm", "2020-01-01-02");
            }
            else if (station == "外壳装配工站")
            {
                result = serverTest.UpdateTestLog("HTS-B2004-03-02", station, "017 B19A17011990", "后盖组装", "limit001", "passed", "passed", "adm", "adm", "2020-01-01-03");
            }
            else if (station == "气密测试工站")
            {
                result = serverTest.UpdateTestLog("HTS-B2004-03-02", station, "A571E20311K112600998HE00110123", "工作电流", "limit001", "passed", "passed", "adm", "adm", "2020-01-01-04");
            }
            else if (station == "支架装配工站")
            {
                result = serverTest.UpdateTestLog("HTS-B2004-03-02", station, "A571E20311K112600998HE00110123", "右支架", "limit001", "passed", "passed", "adm", "adm", "2020-01-01-05");
            }
            else if (station == "成品测试工站")
            {
                result = serverTest.UpdateTestLog("HTS-B2004-03-02", station, "A571E20311K112600998HE00110123", "目检", "limit001", "passed", "passed", "adm", "adm", "2020-01-01-06");
            }
            MessageBox.Show(result+"");
        }

        private void btn_upTestResult_Click(object sender, EventArgs e)
        {
            var station = this.comboBox1.Text;
            var result = "";
            if (station == "烧录工站")
            {
                result=serverTest.UpdateTestResultData("017 B19A17011990", "HTS-B2004-03-02", station, "pass", "had", "hs", "2020-01-01-01");
            }
            else if (station == "灵敏度测试工站")
            {
                result=serverTest.UpdateTestResultData("017 B19A17011990", "HTS-B2004-03-02", station, "pass", "had", "hs", "2020-01-01-02");
            }
            else if (station == "外壳装配工站")
            {
                result=serverTest.UpdateTestResultData("017 B19A17011990", "HTS-B2004-03-02", station, "pass", "had", "hs", "2020-01-01-03");
            }
            else if (station == "气密测试工站")
            {
                result=serverTest.UpdateTestResultData("A571E20311K112600998HE00110123", "HTS-B2004-03-02", station, "pass", "had", "hs", "2020-01-01-04");
            }
            else if (station == "支架装配工站")
            {
                result=serverTest.UpdateTestResultData("A571E20311K112600998HE00110123", "HTS-B2004-03-02", station, "pass", "had", "hs", "2020-01-01-05");
            }
            else if (station == "成品测试工站")
            {
                result=serverTest.UpdateTestResultData("A571E20311K112600998HE00110123", "HTS-B2004-03-02", station, "pass", "had", "hs", "2020-01-01-06");
            }
            MessageBox.Show(result+"");
        }

        private void btn_bindPid_Click(object sender, EventArgs e)
        {
            //外壳绑定
            var result =serverTest.BindingPCBA("017 B19A17011990", "A571E20311K112600998HE00110123", "A19083100060&S2.999&1.2.02.182&50&20190831&1T20190831001", "HTS-B2004-03-02");
            MessageBox.Show(result+"");
        }
    }
}
