using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using CommonUtils.Logger;

namespace MesManager
{
    public partial class SetStationAdmin : RadForm
    {
        private MesService.MesServiceClient mesService;
        public SetStationAdmin()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.lbx_explain_sn.Text = "\t当该站不能测试时（上一站位测试结果为失败），\r\n 可手动设置到当前站位";
        }

        private void SetStationAdmin_Load(object sender, EventArgs e)
        {
            InitListView();
            OptionRemoteData();
            rdb_sn.CheckStateChanged += Rdb_sn_CheckStateChanged;
            rdb_type_no.CheckStateChanged += Rdb_type_no_CheckStateChanged;
            btn_apply.Click += Btn_apply_Click;
            btn_cancel.Click += Btn_cancel_Click;
            cb_type_no.SelectedIndexChanged += Cb_type_no_SelectedIndexChanged;
            this.listView_select_station.ItemChecked += ListView_select_station_ItemChecked;
            this.listView_select_station.ItemSelectionChanged += ListView_select_station_ItemSelectionChanged;
        }

        private void ListView_select_station_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            //if (e.Item.Selected)
            //{
            //    e.Item.Checked = true;
            //}
            //else
            //{
            //    e.Item.Checked = false;
            //}
        }

        private void ListView_select_station_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
           

        }

        private void Cb_type_no_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCurProductType(cb_type_no.SelectedItem.ToString().Trim());
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        async private void Btn_apply_Click(object sender, EventArgs e)
        {
            string res = "";
            if (string.IsNullOrEmpty(cb_type_no.SelectedItem.ToString()))
            {
                MessageBox.Show("零件号不能为空！","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            if (rdb_type_no.CheckState == CheckState.Checked)
            {
                res = await mesService.CommitTypeStationAsync(GetProductTypeNumOfStation());
            }
            else if (rdb_sn.CheckState == CheckState.Checked)
            {
                //按二维码设置站位
            }
            if (res == "0")
                MessageBox.Show("更新失败！");
            else if (res == "1")
                MessageBox.Show("更新成功！");
            else
                MessageBox.Show("更新状态："+res);
        }

        private Dictionary<string, string[]> GetProductTypeNumOfStation()
        {
            Dictionary<string, string[]> keyValuePairs = new Dictionary<string, string[]>();
            string[] arrayStation = new string[10];
            //按型号设置该型号所属站位
            int j = 0;
            foreach (ListViewItem item in this.listView_select_station.Items)
            {
                for (int i = 0; i < item.SubItems.Count; i++)
                {
                    if (item.Checked)
                    {
                        arrayStation[j] = item.Text;
                    }
                }
                j++;
            }
            var sectTypeNum = cb_type_no.SelectedItem.ToString().Trim();
            keyValuePairs.Add(sectTypeNum, arrayStation);
            return keyValuePairs;
        }

        /// <summary>
        /// 查询并更新当前选中的型号对应的站位流程
        /// </summary>
        /// <param name="typeNumber"></param>
        async private void UpdateCurProductType(string typeNumber)
        {
            //当前型号的站位流程
            DataTable curTypeNumberData = (await mesService.SelectTypeStationAsync(typeNumber)).Tables[0];
            //所有
            DataTable dataSource = new DataTable(); //(await mesService.SelectProduceAsync("","")).Tables[0];
            //更新listview
            if (curTypeNumberData.Rows.Count < 1)
            {
                //设置不可选
                foreach (ListViewItem item in this.listView_select_station.Items)
                {
                    item.Checked = false;
                }
                return;
            }
            for (int i = 0; i < curTypeNumberData.Columns.Count; i++)
            {
                var v1 = curTypeNumberData.Rows[0][i].ToString().Trim();
                for (int j = 0; j < dataSource.Rows.Count; j++)
                {
                    var v2 = dataSource.Rows[j][1].ToString().Trim();
                    if (v1.Equals(v2))
                    {
                        //设置选中
                        this.listView_select_station.Items[j].Checked = true;
                    }
                }
            }
        }

        async private void OptionRemoteData()
        {
            mesService = new MesService.MesServiceClient();
            //获取零件号可选项
            DataSet dataSet = null;// await mesService.SelectProductTypeNoAsync("");
            DataTable dataSource = dataSet.Tables[0];
            cb_type_no.Items.Clear();
            cb_sn_type_num.Items.Clear();
            for (int i = 0; i < dataSource.Rows.Count; i++)
            {
                cb_type_no.Items.Add(dataSource.Rows[i][1].ToString().Trim());
                cb_sn_type_num.Items.Add(dataSource.Rows[i][1].ToString().Trim());
            }
            //获取所有站位可选项
            DataTable stations = new DataTable();//(await mesService.SelectProduceAsync("","")).Tables[0];
            cb_sn_station.Items.Clear();
            listView_select_station.Items.Clear();
            for (int i = 0; i < stations.Rows.Count; i++)
            {
                cb_sn_station.Items.Add(stations.Rows[i][1].ToString().Trim());
                var stationName = stations.Rows[i][1].ToString().Trim();
                LoadListViewData(stationName);
            }
        }

        private void InitListView()
        {
            this.listView_select_station.GridLines = false; //显示表格线
            this.listView_select_station.View = View.Details;
            this.listView_select_station.FullRowSelect = false;//是否可以选择行
            this.listView_select_station.LabelEdit = false;
            this.listView_select_station.CheckBoxes = true;

            //设置行高
            ImageList image = new ImageList();
            image.ImageSize = new Size(1, 25);
            this.listView_select_station.SmallImageList = image;

            //listview
            this.listView_select_station.Columns.Add("站位名称",this.listView_select_station.Width-5, HorizontalAlignment.Left);
        }

        private void LoadListViewData(string stationName)
        {
            ListViewItem lvi = new ListViewItem();
            //lvi.ImageIndex = i;
            lvi.Text = stationName;
            this.listView_select_station.Items.Add(lvi);
        }

        private void Rdb_type_no_CheckStateChanged(object sender, EventArgs e)
        {
            if (rdb_type_no.CheckState == CheckState.Checked)
            {
                radGroupBox_type.Visible = true;
                radGroupBox_sn.Visible = false;
            }
        }

        private void Rdb_sn_CheckStateChanged(object sender, EventArgs e)
        {
            if (rdb_sn.CheckState == CheckState.Checked)
            {
                radGroupBox_sn.Visible = true;
                radGroupBox_type.Visible = false;
            }
        }

        private void Btn_apply_Click_1(object sender, EventArgs e)
        {

        }
    }
}
