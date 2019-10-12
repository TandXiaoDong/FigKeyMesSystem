using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using MesManager.Control;
using CommonUtils.Logger;
using Telerik.WinControls.UI;
using NetronLight;

namespace MesManager.UI
{
    public partial class TProcess : RadForm
    {
        private MesService.MesServiceClient serviceClient;
        private MesServiceTest.MesServiceClient serviceClientTest;
        private DataTable stationData;
        private List<string> stationListTemp;
        private string keyStation;        //记录修改前的编码
        private string curRowStationName;//记录鼠标右键选中行编码
        private const string DATA_ORDER = "序号";
        private const string DATA_STATION_NAME = "工序名称";
        private const string DATA_USER_NAME = "操作用户";
        private const string DATA_UPDATE_DATE = "更新日期";

        public TProcess()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
        }
        private void TProcess_Load(object sender, EventArgs e)
        {
            Init();
            EventHandlers();
            RefreshProcessData();
            RefreshControl();
        }


        private void Init()
        {
            this.status_username.Text = MESMainForm.currentUser;
            this.cb_curprocess.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cb_processItem.DropDownStyle = ComboBoxStyle.DropDownList;
            serviceClient = new MesService.MesServiceClient();
            serviceClientTest = new MesServiceTest.MesServiceClient();
            stationListTemp = new List<string>();
            DataGridViewCommon.SetRadGridViewProperty(this.radGridView1, true);
            this.radGridView1.AllowRowHeaderContextMenu = false;
            DataSource();
            RefreshCurrentProcess();
            this.cb_processItem.Text = serviceClientTest.SelectCurrentTProcess();
        }

        private void RefreshControl()
        {
            var userType = MESMainForm.currentUsetType;
            if (userType != 0)
            {
                //没有权限，设置不可修改
                this.menu_add.Enabled = false;
                this.menu_del.Enabled = false;
                this.menu_clear_db.Enabled = false;
                this.menu_commit.Enabled = false;
                this.radGridView1.Enabled = false;
                this.btn_setprocess.Enabled = false;
            }
        }

        private void RefreshCurrentProcess()
        {
            var currentProcess = serviceClientTest.SelectCurrentTProcess();
            this.cb_processItem.Text = currentProcess;
            this.cb_curprocess.Text = currentProcess;
        }

        private void DataSource()
        {
            if (stationData == null)
            {
                stationData = new DataTable();
                stationData.Columns.Add(DATA_ORDER);
                stationData.Columns.Add(DATA_STATION_NAME);
                stationData.Columns.Add(DATA_USER_NAME);
                stationData.Columns.Add(DATA_UPDATE_DATE);
            }
        }

        private void EventHandlers()
        {
            this.menu_refresh.Click += Menu_refresh_Click;
            this.menu_grid.Click += Menu_grid_Click;
            this.menu_clear_db.Click += Menu_clear_db_Click;
            this.menu_del.Click += Menu_del_Click;
            this.menu_add.Click += Menu_add_Click;
            this.menu_commit.Click += Menu_commit_Click;
            //this.menu_up_insert.Click += Menu_up_insert_Click;
            this.cb_processItem.SelectedIndexChanged += Cb_processItem_SelectedIndexChanged;

            this.radGridView1.CellBeginEdit += RadGridView1_CellBeginEdit;
            this.radGridView1.CellEndEdit += RadGridView1_CellEndEdit;

            this.btn_setprocess.Click += Btn_setprocess_Click;
        }

        private void Menu_up_insert_Click(object sender, EventArgs e)
        {
            //向上插入行
            var curIndex = this.radGridView1.CurrentRow.Index;
            if (this.radGridView1.Rows.Count < 1)
            {
                this.radGridView1.Rows.AddNew();
                return;
            }
            foreach (GridViewRowInfo rInfo in this.radGridView1.Rows)
            {
                this.radGridView1.Rows.Insert(curIndex, rInfo);
                break;
            }
        }

        private void Btn_setprocess_Click(object sender, EventArgs e)
        {
            SetCurrentPorcess();
        }

        private void Cb_processItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.cb_processItem.Text))
            {
                ClearGridView();
                this.groupbox_graph.Controls.Clear();
                this.radGridView1.Rows.AddNew();
                return;
            }
            SelectStationList(this.cb_processItem.Text.Trim());
        }

        private void ClearGridView()
        {
            for (int i = this.radGridView1.Rows.Count - 1; i >= 0; i--)
            {
                this.radGridView1.Rows[i].Delete();
            }
        }

        private void Menu_add_Click(object sender, EventArgs e)
        {
            this.radGridView1.Rows.AddNew();
        }

        async private void Menu_del_Click(object sender, EventArgs e)
        {
            //删除当前行
            var stationName = this.radGridView1.CurrentRow.Cells[1].Value.ToString();
            if (string.IsNullOrEmpty(stationName))
            {
                this.radGridView1.CurrentRow.Delete();
            }
            else
            {
                if (MessageBox.Show("确认要删除当前行记录？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    int row = await serviceClient.DeleteStationAsync(cb_processItem.Text.Trim(), stationName);
                    if (row > 0)
                    {
                        //查询删除成功
                        RefreshProcessData();
                    }
                    else
                    {
                        //查询删除失败
                        this.radGridView1.CurrentRow.Delete();
                    }
                    //tool_status.Text = "【型号】删除1行记录 【删除】完成";
                }
            }
        }

        async private void Menu_clear_db_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"确定要清空工艺【{this.cb_processItem.Text}】的所有数据？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
                return;
            await serviceClient.DeleteAllStationAsync(this.cb_processItem.Text.Trim());
            SelectStationList(this.cb_processItem.Text.Trim());
        }

        private void Menu_grid_Click(object sender, EventArgs e)
        {
            for (int i = this.radGridView1.Rows.Count - 1; i >= 0; i--)
            {
                this.radGridView1.Rows[i].Delete();
            }
        }
        private bool IsClickNewRow;
        private void RadGridView1_CellEndEdit(object sender, GridViewCellEventArgs e)
        {
            //click hear add new row:当前行存在，但行计数未记录此行
            var key = this.radGridView1.CurrentRow.Cells[1].Value;
            var index = this.radGridView1.RowCount;
            if (key == null)
                return;
            if (keyStation != key.ToString())
            {
                stationListTemp.Add(keyStation);
            }
            //结束编辑，同时更新新增列序号
            if (index <= 0)
            {
                this.radGridView1.Rows[0].Cells[0].Value = 1;
            }
            else
            {
                //this.radGridView1.Rows[index - 1].Cells[0].Value = index;
                //当行不存在时，click新增行
                if (!IsClickNewRow)
                    return;
                this.radGridView1.CurrentRow.Cells[0].Value = index + 1;
            }
        }

        private void RadGridView1_CellBeginEdit(object sender, GridViewCellCancelEventArgs e)
        {
            var key = this.radGridView1.CurrentRow.Cells[1].Value;
            var index = this.radGridView1.RowCount;
            var rIndex = this.radGridView1.CurrentRow.Index;
            if (key == null)//行不存在
            {
                IsClickNewRow = true;
                return;
            }
            IsClickNewRow = false;
            this.keyStation = key.ToString();
            if (index <= 0)
            {
                this.radGridView1.Rows[0].Cells[0].Value = 1;
            }
            else
            {
                this.radGridView1.Rows[rIndex].Cells[0].Value = rIndex + 1;
            }
        }

        private void Menu_refresh_Click(object sender, EventArgs e)
        {
            RefreshProcessData();
        }

        private void RefreshProcessData()
        {
            UpdateProcesList();
            SelectStationList(this.cb_processItem.Text.Trim());
            stationListTemp.Clear();
        }

        private void Menu_commit_Click(object sender, EventArgs e)
        {
            this.cb_processItem.Focus();
            CommitStationMesService();
        }

        async private void SelectStationList(string processName)
        {
            //调用查询接口
            if (string.IsNullOrEmpty(processName))
                return;
            radGridView1.DataSource = null;
            DataSet dataSet = await serviceClient.SelectStationListAsync(processName);
            DataTable dataTable = dataSet.Tables[0];
            stationData.Clear();
            this.groupbox_graph.Controls.Clear();
            if (dataTable.Rows.Count > 0)
            {
                this.radGridView1.BeginEdit();
                //显示数据
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    DataRow dr = stationData.NewRow();
                    dr[DATA_ORDER] = dataTable.Rows[i][0].ToString();
                    dr[DATA_STATION_NAME] = dataTable.Rows[i][1].ToString();
                    dr[DATA_USER_NAME] = dataTable.Rows[i][2].ToString();
                    dr[DATA_UPDATE_DATE] = dataTable.Rows[i][3].ToString();
                    stationData.Rows.Add(dr);
                    //this.radGridView1.Rows[i].Cells[0].Value = dataTable.Rows[i][0].ToString();
                }
                this.radGridView1.DataSource = stationData;
                this.radGridView1.EndEdit();
                NetronLightGraph();
            }
            else
            {
                stationData.Clear();
                radGridView1.DataSource = stationData;
                this.radGridView1.Rows.AddNew();
            }
            DataGridViewCommon.SetRadGridViewProperty(this.radGridView1, true);
            this.radGridView1.AllowRowHeaderContextMenu = false;
            this.radGridView1.Columns[0].ReadOnly = true;
            this.radGridView1.Columns[2].ReadOnly = true;
            this.radGridView1.Columns[3].ReadOnly = true;
        }

        private void UpdateProcesList()
        {
            this.cb_processItem.Items.Clear();
            this.cb_curprocess.Items.Clear();
            DataSet dataSet = serviceClient.SelectTypeNoList();
            DataTable dataTable = dataSet.Tables[0];
            stationData.Clear();
            if (dataTable.Rows.Count > 0)
            {
                //显示数据
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    this.cb_processItem.Items.Add(dataTable.Rows[i][0]);
                    this.cb_curprocess.Items.Add(dataTable.Rows[i][0]);
                }
                if (!cb_processItem.Items.Contains(this.cb_processItem.Text))
                    cb_processItem.Text = "";
            }
            this.cb_curprocess.Text = serviceClientTest.SelectCurrentTProcess();
            this.cb_processItem.Text = this.cb_curprocess.Text;
        }

        async private void CommitStationMesService()
        {
            try
            {
                int row = radGridView1.RowCount;
                MesService.Station[] stationsArray = new MesService.Station[row];
                //将新增数据
                for (int i = 0; i < row; i++)
                {
                    MesService.Station station = new MesService.Station();
                    var ID = radGridView1.Rows[i].Cells[0].Value.ToString().Trim();
                    var stationName = radGridView1.Rows[i].Cells[1].Value.ToString().Trim();
                    if (!string.IsNullOrEmpty(stationName))
                    {
                        station.ProcessName = this.cb_processItem.Text.Trim();
                        station.StationID = int.Parse(ID);
                        station.StationName = stationName;
                        if (cb_curprocess.Text == cb_processItem.Text)
                        {
                            station.ProcessState = 1;
                        }
                        else
                        {
                            station.ProcessState = 0;
                        }
                        station.UserName = MESMainForm.currentUser;
                        stationsArray[i] = station;
                    }
                }
                //修改数据
                if (stationListTemp.Count > 0)
                {
                    foreach (var station in stationListTemp)
                    {
                        await serviceClient.DeleteStationAsync(this.cb_processItem.Text.Trim(),station);
                    }
                }
                int res = await serviceClient.InsertStationAsync(stationsArray);
                if (res == 1)
                {
                    RefreshProcessData();
                    MessageBox.Show("更新成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"更新失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error(ex.Message + "\r\n" + ex.StackTrace);
                MessageBox.Show($"{ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        async private void SetCurrentPorcess()
        {
            if (!this.cb_curprocess.Items.Contains(this.cb_curprocess.Text))
            {
                MessageBox.Show(this.cb_curprocess.Text+"不存在，请重新选择！","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            foreach (string process in this.cb_curprocess.Items)
            {
                if (process.ToString() == this.cb_curprocess.Text)
                {
                    //设置为当前工艺
                    int upt = await serviceClient.SetCurrentProcessAsync(this.cb_curprocess.Text.Trim(),1);
                    if (upt > 0)
                    {
                        this.cb_processItem.Text = this.cb_curprocess.Text;
                        MessageBox.Show("设置成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("设置失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    await serviceClient.SetCurrentProcessAsync(process.ToString(), 0);
                }
            }
            RefreshProcessData();
        }

        /// <summary>
        /// 动态流程图
        /// </summary>
        private void NetronLightGraph()
        {
            this.groupbox_graph.Controls.Clear();
            GraphControl graphControl1 = new GraphControl();
            graphControl1.Dock = DockStyle.Fill;
            graphControl1.Enabled = false;
            graphControl1.ShowGrid = false;
            graphControl1.BackColor = Color.SteelBlue;
            graphControl1.Font = new Font("宋体",10);
            this.groupbox_graph.Controls.Add(graphControl1);
            int x = 50;//this is left margin
            int y = 30;
            SimpleRectangle srSharpLast = null;
            
            for (int i = 0; i < this.radGridView1.Rows.Count; i++)
            {
                var stationName = this.radGridView1.Rows[i].Cells[1].Value.ToString();
                
                SimpleRectangle srSharp = graphControl1.AddShape(ShapeTypes.Rectangular, new Point(x, y)) as SimpleRectangle;
                Graphics graphics = CreateGraphics();
                SizeF sizeF = graphics.MeasureString(stationName, new Font("宋体", 10));
                srSharp.Text = stationName;
                srSharp.Height = 50;
                srSharp.Width = (int)sizeF.Width + (int)sizeF.Width / 2;
                graphics.Dispose();
                srSharp.ShapeColor = Color.LightSteelBlue;
                if (i % 2 == 0  && i>1)
                {
                    graphControl1.AddConnection(srSharpLast.Connectors[2], srSharp.Connectors[1]);
                }
                if (i + 1 < this.radGridView1.Rows.Count)
                {
                    x += srSharp.Width + 80;
                    var stationNameLast = this.radGridView1.Rows[i + 1].Cells[1].Value.ToString();
                    Graphics graphicsLast = CreateGraphics();
                    SizeF sizeFLast = graphicsLast.MeasureString(stationNameLast, new Font("宋体", 10));

                    srSharpLast = graphControl1.AddShape(ShapeTypes.Rectangular, new Point(x, y)) as SimpleRectangle;
                    srSharpLast.Text = stationNameLast;
                    srSharpLast.Height = 50;
                    srSharpLast.Width = (int)sizeFLast.Width + (int)sizeFLast.Width / 2;
                    graphicsLast.Dispose();
                    srSharpLast.ShapeColor = Color.LightSteelBlue;

                    graphControl1.AddConnection(srSharp.Connectors[2],srSharpLast.Connectors[1]);
                    i++;
                }
                x += srSharpLast.Width + 80;
            }
        }
    }
}
