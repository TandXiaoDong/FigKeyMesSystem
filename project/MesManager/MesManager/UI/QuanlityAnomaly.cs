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
using CommonUtils.Logger;

namespace MesManager.UI
{
    public partial class QuanlityAnomaly : RadForm
    {
        private MesService.MesServiceClient serviceClient;
        private MesServiceTest.MesServiceClient serviceClientTest;
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

        private enum MaterialStateMentType
        {
            AllMaterial,
            SignalMaterial
        }

        private void QuanlityAnomaly_Load(object sender, EventArgs e)
        {
            InitConfig();
            RefreshControl();
            //QueryPcbaMsg();
            this.tb_pcbasn.TextChanged += Tb_pcbasn_TextChanged;
            this.tb_pcbasn.KeyDown += Tb_pcbasn_KeyDown;
            this.btn_apply.Click += Btn_apply_Click;
            this.btn_allApply.Click += Btn_allApply_Click;
            this.btn_cancel.Click += Btn_cancel_Click;
            this.btn_repaireComplete.Click += Btn_repaireComplete_Click;
            this.btn_bing.Click += Btn_bing_Click;
            this.btn_unbind.Click += Btn_unbind_Click;
            this.cb_typeNo.SelectedIndexChanged += Cb_typeNo_SelectedIndexChanged;
            this.btn_searchCaseMsg.Click += Btn_searchCaseMsg_Click;
            this.btn_queryPCBA.Click += Btn_queryPCBA_Click;
            this.radDock1.ActiveWindow = this.dw_pcba;
            this.btn_query.Click += Btn_query_Click;
            this.bindingNavigator.ItemClicked += BindingNavigator1_ItemClicked;
            this.bindingNavigatorCountItem.TextChanged += BindingNavigatorCountItem_TextChanged;
            this.bindingNavigatorPositionItem.TextChanged += BindingNavigatorPositionItem_TextChanged;
        }

        private void Btn_query_Click(object sender, EventArgs e)
        {
            SearchForm searchForm = new SearchForm();
            searchForm.ShowDialog();
            this.cb_materialCode.Text = SearchForm.currentMaterialCode;
        }

        private void Btn_queryPCBA_Click(object sender, EventArgs e)
        {
            QueryPcbaMsg();
        }

        private void Btn_searchCaseMsg_Click(object sender, EventArgs e)
        {
            SearchProduct searchProduct = new SearchProduct();
            if (searchProduct.ShowDialog() == DialogResult.OK)
            {
                this.tb_caseSN.Text = SearchProduct.currentCaseSN;
                this.tb_productSN.Text = SearchProduct.currentProductSN;
            }
        }

        async private void Cb_typeNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            var currentTypeNo = this.cb_typeNo.Text;
            if (currentTypeNo == "")
                return;
            var stationArray = await serviceClientTest.SelectStationListAsync(currentTypeNo);
            this.cb_stationName.EditorControl.Rows.Clear();
            foreach (var station in stationArray)
            {
                this.cb_stationName.EditorControl.Rows.Add(station);
            }
            this.cb_stationName.EditorControl.ShowColumnHeaders = false;
            this.cb_stationName.BestFitColumns();
            this.cb_stationName.Text = "";
        }

        private void Btn_unbind_Click(object sender, EventArgs e)
        {
            UpdateBindState(0);
        }

        private void Btn_bing_Click(object sender, EventArgs e)
        {
            UpdateBindState(1);
        }

        private void Btn_allApply_Click(object sender, EventArgs e)
        {
            var materialList = new List<string>();
            var ds = serviceClient.SelectMaterial("",MesService.MaterialStockState.PUT_IN_STOCK);
            foreach (DataRow dataRow in ds.Tables[0].Rows)
            {
                var materialCode = dataRow[0].ToString();
                if(materialCode != "")
                    materialList.Add(materialCode);
            }

            CommitQuanlityData(materialList,MaterialStateMentType.AllMaterial);
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
            var materialCode = this.cb_materialCode.Text;
            var materialList = new List<string>();
            if(materialCode != "")
                materialList.Add(materialCode);
            CommitQuanlityData(materialList,MaterialStateMentType.SignalMaterial);
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
        async private void QueryPcbaMsg()
        {
            LogHelper.Log.Info("start...");
            if (this.tb_pcbasn.Text != "")
            {
                this.currentPage = 1;//根据条件查询
                this.bindingNavigatorPositionItem.Text = currentPage.ToString();
            }
            this.radGridView1.DataSource = null;
            this.radGridView1.Update();
            var pcbaMesObj = (await serviceClientTest.QueryPCBAMesAsync(this.tb_pcbasn.Text,currentPage,pageSize));
            if (pcbaMesObj.BindNumber % pageSize > 0)
            {
                pageCount = pcbaMesObj.BindNumber / pageSize + 1;
            }
            else
            {
                pageCount = pcbaMesObj.BindNumber / pageSize;
            }
            var dtSource = InitBindRowSource();
            this.radGridView1.BeginEdit();
            DataGridViewCommon.SetRadGridViewProperty(this.radGridView1, false);
            this.radGridView1.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.None;
            var dt = pcbaMesObj.BindHistoryData.Tables[0];
            this.radGridView1.DataSource = dt;
            bindingSource.DataSource = dtSource;
            this.bindingNavigator.BindingSource = bindingSource;
            this.radGridView1.ReadOnly = true;
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
            this.radGridView1.EndEdit();
            this.radGridView1.BestFitColumns();
        }

        private void RefreshControl()
        {
            var userType = MESMainForm.currentUsetType;
            if (userType != 0)
            {
                //没有权限，设置不可修改
                this.btn_apply.Enabled = false;
                this.btn_repaireComplete.Enabled = false;
                this.btn_cancelBinding.Enabled = false;
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
            this.radDateTimePicker1.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

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
            DataTable dt = (await serviceClient.SelectMaterialAsync("",MesService.MaterialStockState.PUT_IN_STOCK_AND_STATEMENT)).Tables[0];
            if (dt.Rows.Count < 1)
                return;
            this.cb_materialCode.Items.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                this.cb_materialCode.Items.Add(dt.Rows[i][0].ToString());
            }
            this.cb_materialCode.AutoCompleteSource = AutoCompleteSource.ListItems;
            this.cb_materialCode.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

            //productTypeNo
            var productTypeNoList = await serviceClientTest.SelectAllTProcessAsync();
            this.cb_typeNo.MultiColumnComboBoxElement.Columns.Add("typeNo");
            foreach (var typeNo in productTypeNoList)
            {
                this.cb_typeNo.EditorControl.Rows.Add(typeNo);
            }
            this.cb_typeNo.EditorControl.ShowColumnHeaders = false;
            this.cb_typeNo.BestFitColumns();
            this.cb_typeNo.Text = "";

            this.cb_stationName.MultiColumnComboBoxElement.Columns.Add("stationName");
        }

        async private void CommitQuanlityData(List<string> materialCodeList,MaterialStateMentType stateMentType)
        {
            if (materialCodeList.Count < 1)
            {
                MessageBox.Show("没有可结单的物料！","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
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
            //var materialCode = this.cb_materialCode.Text;
            var statementDate = this.radDateTimePicker1.Text;
            var tStock = this.tb_stock.Text.Trim();
            var aStock = this.tb_matualStock.Text.Trim();
            var station = this.cb_station.Text;
            var state = (int)MaterialState.FORCE_OVER;
            if (this.cb_materialState.SelectedIndex == 0)
                state = (int)MaterialState.FORCE_OVER;
            var reason = this.tb_reason.Text;
            if (reason == "")
            {
                MessageBox.Show("结单原因不能为空！","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            if (stateMentType == MaterialStateMentType.SignalMaterial)
            {
                if (MessageBox.Show("确认要将当前物料结单？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                    return;
            }
            else if (stateMentType == MaterialStateMentType.AllMaterial)
            {
                if (MessageBox.Show("确认要将所有物料结单？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                    return;
            }
            Dictionary<string, int> dicListResult = new Dictionary<string, int>();
            foreach (var materialCode in materialCodeList)
            {
                var res = await serviceClient.UpdateQuanlityDataAsync(eType + "", materialCode, statementDate, tStock, aStock, station, state + "", reason, MESMainForm.currentUser);
                var sRes = await serviceClient.UpdateMaterialStateMentAsync(materialCode, 3);
                dicListResult.Add(materialCode,sRes);
            }
            //结单结果
            bool IsSuccess = true;
            foreach (var kv in dicListResult)
            {
                if (kv.Value < 1)
                {
                    IsSuccess = false;
                    MessageBox.Show($"【{kv.Key}】结单失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            if (IsSuccess)
            {
                MessageBox.Show("结单成功！", "提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
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
            if (bindingState == "已解除绑定")
                bindingState = "0";
            else if (bindingState == "已绑定")
                bindingState = "1";
            if (shellState == "异常")
                shellState = "0";
            else if (shellState == "正常")
                shellState = "1";
            if (pcbaState == "异常")
                pcbaState = "0";
            else if (pcbaState == "正常")
                pcbaState = "1";

            //验证选择是否正确
            if (this.cb_pcba.CheckState == CheckState.Checked && this.cb_shell.CheckState != CheckState.Checked)
            {
                //选择的pcba异常，外壳正常
                if (pcbaState != "0")
                {
                    MessageBox.Show($"该PCBA【{pcbaValue.ToString()}】没有异常！请重新选择异常PCBA","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    return;
                }
                //验证通过，执行更新
                if (MessageBox.Show($"确定【{pcbaValue.ToString()}】已维修完成？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning,MessageBoxDefaultButton.Button2) != DialogResult.OK)
                    return;
                var bindingResult = serviceClientTest.UpdatePCBABindingRepaireState(pcbaValue,outterShellValue, 
                    int.Parse(bindingState), 1, int.Parse(shellState)); 
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
                if (pcbaState != "0" && shellState == "0")
                {
                    MessageBox.Show($"该PCBA【{pcbaValue.ToString()}】没有异常！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (pcbaState == "0" && shellState != "0")
                {
                    MessageBox.Show($"该外壳【{outterShellValue.ToString()}】没有异常！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (pcbaState != "0" && shellState != "0")
                {
                    MessageBox.Show($"该PCBA【{pcbaValue.ToString()}】与外壳【{outterShellValue.ToString()}】没有异常！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else if (pcbaState == "0" && shellState == "0")
                {
                    //验证通过，执行更新
                    if (MessageBox.Show($"确定PCBA【{pcbaValue.ToString()}】与外壳【{outterShellValue.ToString()}】已维修完成？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning,MessageBoxDefaultButton.Button2) != DialogResult.OK)
                        return;

                    var bindingResult = serviceClientTest.UpdatePCBABindingRepaireState(pcbaValue,outterShellValue, 
                        int.Parse(bindingState), 1,1);
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
                if (shellState != "0")
                {
                    MessageBox.Show($"该外壳【{outterShellValue.ToString()}】没有异常！请重新选择外壳，", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                //验证通过，执行更新
                if (MessageBox.Show($"确定外壳【{outterShellValue.ToString()}】已维修完成？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning,MessageBoxDefaultButton.Button2) != DialogResult.OK)
                    return;
                var bindingResult = serviceClientTest.UpdatePCBABindingRepaireState(pcbaValue,
                        outterShellValue, int.Parse(bindingState), int.Parse(pcbaState), 1);
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
            if (bindingState.ToString().Equals("已解除绑定") && pcbaState.ToString().Equals("异常"))
            {
                MessageBox.Show("该PCBA已解除绑定，请勿重复操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (bindingState.ToString().Equals("已解除绑定") && shellState.ToString().Equals("异常"))
            {
                MessageBox.Show("该PCBA已解除绑定，请勿重复操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show($"确定要解除【{pcbaValue.ToString()}】绑定？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning,MessageBoxDefaultButton.Button2) != DialogResult.OK)
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

        private void UpdateBindState(int state)
        {
            var caseSN = this.tb_caseSN.Text;
            var productSN = this.tb_productSN.Text;
            var productTypeNo = this.cb_typeNo.Text;
            var stationName = this.cb_stationName.Text;
            var remark = this.tb_remark.Text;
            if (caseSN == "")
            {
                MessageBox.Show("箱子SN不能为空！","Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            if (productSN == "")
            {
                MessageBox.Show("产品SN不能为空！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (productTypeNo == "")
            {
                MessageBox.Show("产品型号不能为空！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (stationName == "")
            {
                MessageBox.Show("工站名称不能为空！", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string[] result = serviceClientTest.UpdatePackageProductBindingMsg(caseSN, productSN, productTypeNo, stationName, state.ToString(), remark, MESMainForm.currentUser,MESMainForm.currentUser);

            if (result[0] == "0X01")
            {
                MessageBox.Show("该箱子已放满！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (result[0] == "0X02")
            {
                MessageBox.Show($"该产品已绑定箱子{result[1]}！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (result[0] == "0X03")
            {
                if (state == 0)
                {
                    MessageBox.Show("解绑成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if(state == 1)
                {
                    MessageBox.Show("已存在绑定记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else if (result[0] == "0X04")
            {
                //已经存在绑定记录，更新失败
            }
            else if (result[0] == "0X05")
            {
                //不存在绑定记录--解除绑定--更新成功
                MessageBox.Show("已解除绑定，更新成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (result[0] == "0X06")
            {
                //不存在绑定记录--解除绑定--更新失败
            }
            else if (result[0] == "0X07")
            {
                MessageBox.Show("重新绑定成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (result[0] == "0X08")
            {
                MessageBox.Show("重新绑定失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (result[0] == "0X09")
            {
                MessageBox.Show("绑定成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (result[0] == "0X10")
            {
                MessageBox.Show("绑定失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (result[0] == "0X11")
            {

            }
            else if (result[0] == "0X12")
            {

            }
            else if (result[0] == "0X13")
            {

            }
            else if (result[0] == "0X14")
            {

            }
            else if (result[0] == "0X15")
            {

            }
            else if (result[0] == "0X16")
            {

            }
        }
        private void BindingNavigatorPositionItem_TextChanged(object sender, EventArgs e)
        {
            //this.bindingNavigatorPositionItem.Text = currentPage.ToString();
        }

        private void BindingNavigatorCountItem_TextChanged(object sender, EventArgs e)
        {
            //this.bindingNavigatorCountItem.Text = "/"+pageCount;
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
            QueryPcbaMsg();
        }
    }
}
