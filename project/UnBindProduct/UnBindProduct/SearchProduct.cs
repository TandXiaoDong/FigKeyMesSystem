using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;

namespace UnBindProduct
{
    public partial class SearchProduct : Telerik.WinControls.UI.RadForm
    {
        MesServiceTest.MesServiceClient serviceClient;
        public static string currentMaterialCode = "";

        public SearchProduct()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            serviceClient = new MesServiceTest.MesServiceClient();
            SelectMaterialMsg();
            //this.tb_inputMsg.TextChanged += Tb_inputMsg_TextChanged;
            this.radGridView1.CellClick += RadGridView1_CellClick;
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
            SelectMaterialMsg();
        }

        async private void SelectMaterialMsg()
        {
            DataTable dt = null;//(await serviceClient.SelectMaterialAsync(this.tb_inputMsg.Text, MesService.MaterialStockState.PUT_IN_STOCK_AND_STATEMENT)).Tables[0];

            DataTable data = new DataTable();
            data.Columns.Add("序号");
            data.Columns.Add("物料编码");
            data.Columns.Add("物料名称");
            data.Columns.Add("库存状态");

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = data.NewRow();
                    var materialCode = dt.Rows[i][0].ToString();
                    //var materialPN = AnalysisMaterialCode.GetMaterialPN(materialCode);
                    dr["序号"] = i + 1;
                    dr["物料编码"] = materialCode;
                    //dr["物料名称"] = serviceClient.SelectMaterialName(materialPN);
                    var stockState = dt.Rows[i][6].ToString();
                    if (stockState == "2")
                        stockState = "已使用完成";
                    else if (stockState == "3")
                        stockState = "已经结单";
                    else if (stockState == "1")
                        stockState = "正常使用";
                    dr["库存状态"] = stockState;
                    data.Rows.Add(dr);
                }
            }
            this.radGridView1.DataSource = data;
            //DataGridViewCommon.SetRadGridViewProperty(this.radGridView1, false);
            this.radGridView1.ReadOnly = true;
            this.radGridView1.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.None;
            this.radGridView1.BestFitColumns();
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            //currentMaterialCode = this.tb_selectCode.Text;
            this.Close();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
