using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using MesManager.Control;

namespace MesManager.UI
{
    public partial class SearchForm : Telerik.WinControls.UI.RadForm
    {
        MesService.MesServiceClient serviceClient;
        public static string currentMaterialCode = "";
        public SearchForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
            serviceClient = new MesService.MesServiceClient();
            SelectMaterialMsg();
            this.tb_inputMsg.TextChanged += Tb_inputMsg_TextChanged;
            this.radGridView1.CellClick += RadGridView1_CellClick;
        }

        private void RadGridView1_CellClick(object sender, Telerik.WinControls.UI.GridViewCellEventArgs e)
        {
            var materialCode = this.radGridView1.CurrentRow.Cells[1].Value;
            if (materialCode == null)
                return;
            this.tb_selectCode.Text = materialCode.ToString();
            currentMaterialCode = this.tb_selectCode.Text;
        }

        private void Tb_inputMsg_TextChanged(object sender, EventArgs e)
        {
            SelectMaterialMsg();
        }

        async private void SelectMaterialMsg()
        {
            DataTable dt = (await serviceClient.SelectMaterialAsync(this.tb_inputMsg.Text,1)).Tables[0];
            DataTable data = new DataTable();
            data.Columns.Add("序号");
            data.Columns.Add("物料编码");
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = data.NewRow();
                    dr["序号"] = i + 1;
                    dr["物料编码"] = dt.Rows[i][0].ToString();
                    data.Rows.Add(dr);
                }
            }
            this.radGridView1.DataSource = data;
            DataGridViewCommon.SetRadGridViewProperty(this.radGridView1, false);
            this.radGridView1.ReadOnly = true;
            this.radGridView1.Columns[0].BestFit();
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            currentMaterialCode = this.tb_selectCode.Text;
            this.Close();
        }
    }
}
