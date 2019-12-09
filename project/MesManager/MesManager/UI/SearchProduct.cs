using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using WindowsFormTelerik.ControlCommon;

namespace MesManager.UI
{
    public partial class SearchProduct : Telerik.WinControls.UI.RadForm
    {
        MesService.MesServiceClient serviceClient;
        public static string currentCaseSN = "";
        public static string currentProductSN = "";

        public SearchProduct()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.rb_binded.CheckState = CheckState.Checked;
            serviceClient = new MesService.MesServiceClient();
            SelectProductBindMsg();
            this.tb_search.TextChanged += Tb_inputMsg_TextChanged;
            this.radGridView1.CellClick += RadGridView1_CellClick;
            this.radGridView1.CellDoubleClick += RadGridView1_CellDoubleClick;
            this.rb_binded.CheckStateChanged += Rb_binded_CheckStateChanged;
            this.rb_unbinded.CheckStateChanged += Rb_unbinded_CheckStateChanged;
            this.btn_apply.Click += Btn_apply_Click;
            this.btn_cancel.Click += Btn_cancel_Click;
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Btn_apply_Click(object sender, EventArgs e)
        {
            SelectSearchProduct();
        }

        private void Rb_unbinded_CheckStateChanged(object sender, EventArgs e)
        {
            SelectProductBindMsg();
        }

        private void Rb_binded_CheckStateChanged(object sender, EventArgs e)
        {
            SelectProductBindMsg();
        }

        private void RadGridView1_CellDoubleClick(object sender, Telerik.WinControls.UI.GridViewCellEventArgs e)
        {
            SelectSearchProduct();
        }

        private void RadGridView1_CellClick(object sender, Telerik.WinControls.UI.GridViewCellEventArgs e)
        {
            var materialCode = this.radGridView1.CurrentRow.Cells[1].Value;
            if (materialCode == null)
                return;
            //this.tb_selectCode.Text = materialCode.ToString();
            //currentMaterialCode = this.tb_selectCode.Text;
        }

        private void Tb_inputMsg_TextChanged(object sender, EventArgs e)
        {
            SelectProductBindMsg();
        }

        async private void SelectProductBindMsg()
        {
            var queryCondition = this.tb_search.Text.Trim();
            var bindState = "1";
            if (this.rb_binded.CheckState == CheckState.Checked)
                bindState = "1";
            else if (rb_unbinded.CheckState == CheckState.Checked)
                bindState = "0";
            DataSet ds = await serviceClient.SelectPackageProductCheckAsync(queryCondition,bindState,true);
            this.radGridView1.DataSource = ds.Tables[0];
            RadGridViewProperties.SetRadGridViewProperty(this.radGridView1, false);
            this.radGridView1.ReadOnly = true;
            this.radGridView1.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.None;
            this.radGridView1.BestFitColumns();
        }

        private void SelectSearchProduct()
        {
            if (this.radGridView1.RowCount < 1)
                return;
            if (this.radGridView1.CurrentRow.Cells[1].Value != null)
                currentCaseSN = this.radGridView1.CurrentRow.Cells[1].Value.ToString();
            if (this.radGridView1.CurrentRow.Cells[2].Value != null)
                currentProductSN = this.radGridView1.CurrentRow.Cells[2].Value.ToString();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
