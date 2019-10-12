using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using CommonUtils.CalculateAndString;
using CommonUtils.Logger;
using Telerik.WinControls.UI;

namespace MesManager
{
    public partial class Station : RadForm
    {
        private DataTable dataSource;
        private MesService.MesServiceClient mesService;
        private const string DATA_ORDER_NAME = "序号";
        private const string DATA_STATION_NAME = "站位名称";
        private string keyOrder;
        private string keyStation;
        private List<Station> stationListTemp;
        public Station()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            rlbx_explain.Text = "按顺序添加生产线包含的站位，也可修改站位信息";
        }

        private DataTable DataSource()
        {
            if (dataSource == null)
            {
                dataSource = new DataTable();
                dataSource.Columns.Add(DATA_ORDER_NAME);
                dataSource.Columns.Add(DATA_STATION_NAME);
            }
            return dataSource;
        }

        private string KeyOrder { get; set; }

        private string KeyStationName { get; set; }

        async private void SetProduce_Load(object sender, EventArgs e)
        {
            mesService = new MesService.MesServiceClient();
            DataSource();
            SetRadGridViewProperty();
            radGridView1.DataSource = dataSource;
            stationListTemp = new List<Station>();
            SelectData();

            btn_cancel.Click += Btn_cancel_Click;
            btn_select.Click += Btn_select_Click;
            btn_apply.Click += Btn_apply_Click;
            btn_clear_dgv.Click += Btn_clear_dgv_Click;
            btn_clear_server_data.Click += Btn_clear_server_data_Click;

            radGridView1.MouseDown += RadGridView1_MouseDown;
            radGridView1.ContextMenuOpening += RadGridView1_ContextMenuOpening;
            radGridView1.CellBeginEdit += RadGridView1_CellBeginEdit;
            radGridView1.CellEndEdit += RadGridView1_CellEndEdit;
        }

        private void RadGridView1_CellEndEdit(object sender, GridViewCellEventArgs e)
        {
            var order = this.radGridView1.CurrentRow.Cells[0].Value;
            var name = this.radGridView1.CurrentRow.Cells[1].Value;
            if (order == null)
                return;
            if (name == null)
                return;
            if (order.ToString() != keyOrder || name.ToString() != keyStation)
            {
                Station station = new Station();
                station.keyOrder = order.ToString();
                station.KeyStationName = name.ToString();
                stationListTemp.Add(station);
            }
        }

        private void RadGridView1_CellBeginEdit(object sender, GridViewCellCancelEventArgs e)
        {
            var order = this.radGridView1.CurrentRow.Cells[0].Value;
            var name = this.radGridView1.CurrentRow.Cells[1].Value;
            if (order == null)
                return;
            if (name == null)
                return;
            keyOrder = order.ToString();
            keyStation = name.ToString();
        }

        private string curRowStationName;
        private string curStationID;
        private void RadGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (this.radGridView1.CurrentRow.Index < 0)
                    return;
                curRowStationName = this.radGridView1.CurrentRow.Cells[1].Value.ToString().Trim();
                curStationID = this.radGridView1.CurrentRow.Cells[0].Value.ToString().Trim();
            }
        }

        private void RadGridView1_ContextMenuOpening(object sender, ContextMenuOpeningEventArgs e)
        {
            for (int i = 0; i < e.ContextMenu.Items.Count; i++)
            {
                String contextMenuText = e.ContextMenu.Items[i].Text;
                switch (contextMenuText)
                {
                    case "Conditional Formatting":
                        e.ContextMenu.Items[i].Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
                        e.ContextMenu.Items[i + 1].Visibility = ElementVisibility.Collapsed;
                        break;
                    case "Hide Column":
                        e.ContextMenu.Items[i].Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
                        break;
                    case "Pinned state":
                        e.ContextMenu.Items[i].Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
                        break;
                    case "Best Fit":
                        e.ContextMenu.Items[i].Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
                        break;
                    case "Cut":
                        e.ContextMenu.Items[i].Click += DeleteStationRow_Click;
                        break;
                    case "Copy":
                        break;
                    case "Paste":
                        break;
                    case "Edit":
                        break;
                    case "Clear Value":
                        break;
                    case "Delete Row":
                        e.ContextMenu.Items[i].Click += DeleteStationRow_Click; ;
                        break;
                }
            }
        }

        private void DeleteStationRow_Click(object sender, EventArgs e)
        {
            DeleteProduceData();
        }

        private void Btn_clear_dgv_Click(object sender, EventArgs e)
        {
            dataSource.Clear();
            this.radGridView1.DataSource = dataSource;
        }

        async private void DeleteProduceData()
        {
            //cut 执行delete 服务数据
            if (MessageBox.Show("是否删除该行数据", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                //int del = await mesService.DeleteStationAsync(curRowStationName);
            }
            SelectData();
        }

        async private void Btn_clear_server_data_Click(object sender, EventArgs e)
        {
            //清除所有数据
            if (dataSource.Rows.Count == 0)
                return;
            DialogResult dialogResult = MessageBox.Show("是否删除数据库服务中de数据", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.OK)
            {
                int del = await mesService.DeleteAllTypeStationAsync();
                //更新显示
                SelectData();
            }
        }

        private void Btn_apply_Click(object sender, EventArgs e)
        {
            //将新增数据提交到服务
            //将修改数据更新到服务
            CommitMesService();
        }

        async private void CommitMesService()
        {
            //将新增数据提交到服务
            try
            {
                int row = radGridView1.RowCount;
                MesService.Station[] stationsArray = new MesService.Station[row];
                for (int i = 0; i < row; i++)
                {
                    MesService.Station station = new MesService.Station();
                    var ID = radGridView1.Rows[i].Cells[0].Value.ToString().Trim();
                    var stationName = radGridView1.Rows[i].Cells[1].Value.ToString().Trim();
                    station.StationID = int.Parse(ID);
                    station.StationName = stationName;
                    stationsArray[i] = station;
                }
                if (stationListTemp.Count > 0)
                {
                    foreach (var station in stationListTemp)
                    {
                        //await mesService.DeleteStationAsync(station.KeyStationName);
                    }
                }
                int res = await mesService.InsertStationAsync(stationsArray);
                if (res == 1)
                {
                    MessageBox.Show("更新成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"更新失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error(ex.Message+"\r\n"+ex.StackTrace);
            }
        }

        private void Btn_select_Click(object sender, EventArgs e)
        {
            SelectData();
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 设置视图属性
        /// </summary>
        private void SetRadGridViewProperty()
        {
            radGridView1.EnableGrouping = false;
            radGridView1.AllowDrop = true;
            radGridView1.AllowRowReorder = true;
            /////显示每行前面的标记
            radGridView1.AddNewRowPosition = Telerik.WinControls.UI.SystemRowPosition.Bottom;
            radGridView1.ShowRowHeaderColumn = true;
            radGridView1.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;
            radGridView1.ReadOnly = false;
            //gridView.ColumnChooserSortOrder = RadSortOrder.Ascending;
            //dgv.AllowRowHeaderContextMenu = false;
        }

        /// <summary>
        /// 查询数据，并显示
        /// </summary>
        async private void SelectData()
        {
            //调用查询接口
            DataSet dataSet = null;// await mesService.SelectStationAsync("","");
            DataTable dataTable = dataSet.Tables[0];
            dataSource.Clear();
            if (dataTable.Rows.Count > 0)
            {
                //显示数据
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    DataRow dr = dataSource.NewRow();
                    dr[DATA_ORDER_NAME] = dataTable.Rows[i][0].ToString();
                    dr[DATA_STATION_NAME] = dataTable.Rows[i][1].ToString();
                    dataSource.Rows.Add(dr);
                }
                radGridView1.DataSource = dataSource;
            }
            else
            {
                dataSource.Clear();
                radGridView1.DataSource = dataSource;
            }
        }
    }
}
