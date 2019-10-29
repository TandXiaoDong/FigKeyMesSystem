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

        private void QuanlityAnomaly_Load(object sender, EventArgs e)
        {
            InitConfig();
            RefreshControl();
            QueryPcbaMsg();
            this.tb_pcbasn.TextChanged += Tb_pcbasn_TextChanged;
            this.tb_pcbasn.KeyDown += Tb_pcbasn_KeyDown;
            this.btn_apply.Click += Btn_apply_Click;
            this.btn_cancel.Click += Btn_cancel_Click;
            this.btn_repaireComplete.Click += Btn_repaireComplete_Click;
        }

        private void Btn_repaireComplete_Click(object sender, EventArgs e)
        {
            ReCoverPcbaBinding();
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Btn_apply_Click(object sender, EventArgs e)
        {
            CommitQuanlityData();
        }

        private void Tb_pcbasn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                QueryPcbaMsg();
            }
        }

        private void Tb_pcbasn_TextChanged(object sender, EventArgs e)
        {
            QueryPcbaMsg();
        }

        async private void QueryPcbaMsg()
        {
            var dt = (await serviceClientTest.QueryPCBAMesAsync(this.tb_pcbasn.Text)).Tables[0];
            this.radGridView1.DataSource = dt;
            DataGridViewCommon.SetRadGridViewProperty(this.radGridView1, false);
            this.radGridView1.ReadOnly = true;
            this.radGridView1.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.None;
            this.radGridView1.BestFitColumns();
            foreach (var rowInfo in this.radGridView1.Rows)
            {
                var bindingState = rowInfo.Cells[5].Value.ToString();
                var pcbaState = rowInfo.Cells[6].Value.ToString();
                var outterState = rowInfo.Cells[7].Value.ToString();
                if (bindingState == "已解除绑定")
                    rowInfo.Cells[5].Style.ForeColor = Color.Red;
                if (pcbaState == "异常")
                    rowInfo.Cells[6].Style.ForeColor = Color.PaleVioletRed;
                if (outterState == "异常")
                    rowInfo.Cells[7].Style.ForeColor = Color.PaleVioletRed;
            }
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

        private void btn_exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ReCoverPcbaBinding()
        {
            if (this.radGridView1.RowCount < 1)
                return;
            var pcbaValue = this.radGridView1.CurrentRow.Cells[3].Value.ToString().Trim();
            var outterShellValue = this.radGridView1.CurrentRow.Cells[4].Value.ToString().Trim();
            var bindingState = this.radGridView1.CurrentRow.Cells[5].Value.ToString().Trim();
            var pcbaState = this.radGridView1.CurrentRow.Cells[6].Value.ToString().Trim();
            var shellState = this.radGridView1.CurrentRow.Cells[7].Value.ToString().Trim();

            //是否选择要恢复正常的数据行
            if (pcbaValue == null)
            {
                MessageBox.Show("请选择已维修完成的PCBA或外壳！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //是否选择异常类型
            if (this.cb_pcba.CheckState != CheckState.Checked && this.cb_shell.CheckState != CheckState.Checked)
            {
                MessageBox.Show("请选择异常类型！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //验证选择是否正确
            if (this.cb_pcba.CheckState == CheckState.Checked && this.cb_shell.CheckState != CheckState.Checked)
            {
                //选择的pcba异常，外壳正常
                if (pcbaState != "异常")
                {
                    MessageBox.Show($"该PCBA【{pcbaValue.ToString()}】没有异常！请重新选择异常PCBA","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    return;
                }
                //验证通过，执行更新
                if (MessageBox.Show($"确定【{pcbaValue.ToString()}】已维修完成？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                    return;
                var bindingResult = serviceClientTest.UpdatePcbaBindingState(pcbaValue.ToString(),
                        outterShellValue.ToString(), int.Parse(bindingState.ToString()), 1, int.Parse(shellState.ToString()));
                if (bindingResult)
                {
                    this.cb_pcba.CheckState = CheckState.Unchecked;
                    this.cb_shell.CheckState = CheckState.Unchecked;
                    MessageBox.Show("更新维修完成状态成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    QueryPcbaMsg();
                    return;
                }
                MessageBox.Show("更新维修完成状态成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (this.cb_pcba.CheckState == CheckState.Checked && cb_shell.CheckState == CheckState.Checked)
            {
                if (pcbaState.ToString() != "异常" && shellState.ToString() == "异常")
                {
                    MessageBox.Show($"该PCBA【{pcbaValue.ToString()}】没有异常！，", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (pcbaState.ToString() == "异常" && shellState.ToString() != "异常")
                {
                    MessageBox.Show($"该外壳【{outterShellValue.ToString()}】没有异常！，", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (pcbaState.ToString() != "异常" && shellState.ToString() != "异常")
                {
                    MessageBox.Show($"该PCBA【{pcbaValue.ToString()}】与外壳【{outterShellValue.ToString()}】没有异常！，", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (pcbaState.ToString() == "异常" && shellState.ToString() == "异常")
                {
                    //验证通过，执行更新
                    if (MessageBox.Show($"确定PCBA【{pcbaValue.ToString()}】与外壳【{outterShellValue.ToString()}】已维修完成？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                        return;
                    var bindingResult = serviceClientTest.UpdatePcbaBindingState(pcbaValue.ToString(),
                        outterShellValue.ToString(), int.Parse(bindingState.ToString()), 1, 1);
                    if (bindingResult)
                    {
                        this.cb_pcba.CheckState = CheckState.Unchecked;
                        this.cb_shell.CheckState = CheckState.Unchecked;
                        MessageBox.Show("更新维修完成状态成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        QueryPcbaMsg();
                        return;
                    }
                    MessageBox.Show("更新维修完成状态成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else if (this.cb_pcba.CheckState != CheckState.Checked && cb_shell.CheckState == CheckState.Checked)
            {
                if (outterShellValue.ToString() != "异常")
                {
                    MessageBox.Show($"该外壳【{outterShellValue.ToString()}】没有异常！请重新选择外壳，", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                //验证通过，执行更新
                if (MessageBox.Show($"确定外壳【{outterShellValue.ToString()}】已维修完成？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                    return;
                var bindingResult = serviceClientTest.UpdatePcbaBindingState(pcbaValue.ToString(),
                        outterShellValue.ToString(), int.Parse(bindingState.ToString()), int.Parse(pcbaState.ToString()), 1);
                if (bindingResult)
                {
                    this.cb_pcba.CheckState = CheckState.Unchecked;
                    this.cb_shell.CheckState = CheckState.Unchecked;
                    MessageBox.Show("更新维修完成状态成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    QueryPcbaMsg();
                    return;
                }
                MessageBox.Show("更新维修完成状态成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (this.cb_pcba.CheckState != CheckState.Checked && cb_shell.CheckState != CheckState.Checked)
            {
                MessageBox.Show($"没有选择已维修完成的PCBA!，", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void CancelPcbaBinding()
        {
            if (this.radGridView1.RowCount < 1)
                return;
            var pcbaValue = this.radGridView1.CurrentRow.Cells[3].Value;
            var outterShellValue = this.radGridView1.CurrentRow.Cells[4].Value;
            var bindingState = this.radGridView1.CurrentRow.Cells[5].Value;

            //是否选择要解除绑定的数据行
            if (pcbaValue == null)
            {
                MessageBox.Show("请选择要解绑的PCBA！","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            //是否选择异常类型
            if (this.cb_pcba.CheckState != CheckState.Checked && this.cb_shell.CheckState != CheckState.Checked)
            {
                MessageBox.Show("请选择异常类型！","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            //更新完成后
            var pcbaState = 1;
            var shellState = 1;
            if (this.cb_pcba.CheckState == CheckState.Checked)
                pcbaState = 0;
            if (this.cb_shell.CheckState == CheckState.Checked)
                shellState = 0;
            if (bindingState.ToString().Equals("已解除绑定"))
            {
                MessageBox.Show("该PCBA已解除绑定，请勿重复操作！","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show($"确定要解除【{pcbaValue.ToString()}】绑定？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                return;
            var bindingResult = serviceClientTest.UpdatePcbaBindingState(pcbaValue.ToString(), 
                outterShellValue.ToString(),0, pcbaState, shellState);
            if(bindingResult)
            {
                this.cb_pcba.CheckState = CheckState.Unchecked;
                this.cb_shell.CheckState = CheckState.Unchecked;
                MessageBox.Show("解除绑定成功！","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                QueryPcbaMsg();
                return;
            }
            MessageBox.Show("解除绑定失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btn_cancelBinding_Click(object sender, EventArgs e)
        {
            CancelPcbaBinding();
        }

        private void btn_query_Click(object sender, EventArgs e)
        {
            SearchForm searchForm = new SearchForm();
            searchForm.ShowDialog();
            this.cb_materialCode.Text = SearchForm.currentMaterialCode;
        }
    }
}
