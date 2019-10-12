using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace MesManager.UI
{
    public partial class QuanlityAnomaly : RadForm
    {
        private MesService.MesServiceClient serviceClient;
        private MesServiceTest.MesServiceClient serviceClientTest;
        public QuanlityAnomaly()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
        }

        public enum ExceptType
        {
            MATERIAL_STOCK = 0,
            MATERIAL_PRODUCE = 1,
            MATERIAL_PROCESS = 2
        }

        public enum MaterialState
        {
            NORMAL_USING = 1,
            NORMAL_OVER = 2,
            FORCE_OVER = 3
        }

        private void Btn_apply_Click(object sender, EventArgs e)
        {
            CommitQuanlityData();
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void QuanlityAnomaly_Load(object sender, EventArgs e)
        {
            InitConfig();
            RefreshControl();
        }

        private void RefreshControl()
        {
            var userType = MESMainForm.currentUsetType;
            if (userType != 0)
            {
                //没有权限，设置不可修改
                this.btn_apply.Enabled = false;
            }
        }

        async private void InitConfig()
        {
            serviceClient = new MesService.MesServiceClient();
            serviceClientTest = new MesServiceTest.MesServiceClient();
            this.rbtn_material_stock.CheckState = CheckState.Checked;
            this.cb_materialState.Items.Add("关闭");
            this.cb_materialState.SelectedIndex = 0;
            this.cb_materialState.ForeColor = Color.Red;

            var currentProcess = await serviceClientTest.SelectCurrentTProcessAsync();
            string[] array = await serviceClientTest.SelectStationListAsync(currentProcess);
            if (array.Length > 0)
            {
                cb_station.Items.Clear();
                foreach (var station in array)
                {
                    cb_station.Items.Add(station);
                }
            }
            DataTable dt = (await serviceClient.SelectMaterialAsync("")).Tables[0];
            if (dt.Rows.Count < 1)
                return;
            this.cb_materialCode.Items.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                this.cb_materialCode.Items.Add(dt.Rows[i][0].ToString());
            }
            this.cb_materialCode.AutoCompleteSource = AutoCompleteSource.ListItems;
            this.cb_materialCode.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        }

        async private void CommitQuanlityData()
        {
            int eType = 0;
            if (this.rbtn_material_stock.CheckState == CheckState.Checked)
            {
                eType = (int)ExceptType.MATERIAL_STOCK;
            }
            else if (this.rbtn_material_produce.CheckState == CheckState.Checked)
            {
                eType = (int)ExceptType.MATERIAL_PRODUCE;
            }
            else if (this.rbtn_material_process.CheckState == CheckState.Checked)
            {
                eType = (int)ExceptType.MATERIAL_PROCESS;
            }
            var materialCode = this.cb_materialCode.Text;
            var statementDate = this.radDateTimePicker1.Text;
            var tStock = this.tb_stock.Text.Trim();
            var aStock = this.tb_matualStock.Text.Trim();
            var station = this.cb_station.Text;
            var state = (int)MaterialState.FORCE_OVER;
            if (this.cb_materialState.SelectedIndex == 0)
                state = (int)MaterialState.FORCE_OVER;
            var reason = this.tb_reason.Text;
            
            var res = await serviceClient.UpdateQuanlityDataAsync(eType+"",materialCode,statementDate,tStock,aStock,station,state+"",reason,MESMainForm.currentUser);
            var sRes = await serviceClient.UpdateMaterialStateMentAsync(materialCode,3);
            if (sRes > 0)
            {
                MessageBox.Show("结单成功！", "提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }else
            {
                MessageBox.Show("结单失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
