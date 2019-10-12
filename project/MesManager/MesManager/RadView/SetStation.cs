using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;

namespace MesManager
{
    public partial class SetStation : Telerik.WinControls.UI.RadForm
    {
        MesService.MesServiceClient serviceClient;
        public SetStation()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void Btn_apply_Click(object sender, EventArgs e)
        {

        }

        private void SetStation_Load(object sender, EventArgs e)
        {
            serviceClient = new MesService.MesServiceClient();
            OptionRemoteData();
        }

        async private void OptionRemoteData()
        {
            serviceClient = new MesService.MesServiceClient();
            //获取零件号可选项
            DataSet dataSet = null;//await serviceClient.SelectProductTypeNoAsync("");
            DataTable dataSource = dataSet.Tables[0];
            cb_typeNo.Items.Clear();
            for (int i = 0; i < dataSource.Rows.Count; i++)
            {
                cb_typeNo.Items.Add(dataSource.Rows[i][1].ToString().Trim());
            }
            //获取所有站位可选项
            DataTable stations = new DataTable();//(await serviceClient.SelectProduceAsync("", "")).Tables[0];
            cb_station.Items.Clear();
            for (int i = 0; i < stations.Rows.Count; i++)
            {
                cb_station.Items.Add(stations.Rows[i][1].ToString().Trim());
            }
            cb_testRes.Items.Clear();
            cb_testRes.Items.Add("PASS");
            cb_testRes.Items.Add("FAIL");
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
