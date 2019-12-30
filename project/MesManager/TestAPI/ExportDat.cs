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

        public ExportDat()
        {
            InitializeComponent();
        }

        private void ExportDat_Load(object sender, EventArgs e)
        {
            serverClient = new MesService.MesServiceClient();
            serverTest = new MesServiceTest.MesServiceClient();
            SQLServer.SqlConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            SetRadGridViewProperty(this.radGridView1,false);
            this.bindingNavigator1.ItemClicked += BindingNavigator1_ItemClicked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //查询数据
            ResetCurrentPage();
            SelectOfSn();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //导出数据
            serverTest.CopyDataSourceAsync();
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
            SelectOfSn();
        }
    }
}
