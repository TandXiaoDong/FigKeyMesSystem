using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using MesManager.Properties;
using MesManager.Control;

namespace MesManager.UI
{
    public partial class ProductPackage : RadForm
    {
        private MesService.MesServiceClient serviceClient;
        private int snLength1, snLength2;
        public ProductPackage()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void ProductPackage_Load(object sender, EventArgs e)
        {
            Init();
            EventHandlers();
        }

        private void Init()
        {
            serviceClient = new MesService.MesServiceClient();
            DataGridViewCommon.SetRadGridViewProperty(this.radGridView1,false);
            this.radGridView1.Columns[0].Width = 15;
            
            var len1 = ConfigurationManager.AppSettings["snLength1"].ToString();
            var len2 = ConfigurationManager.AppSettings["snLength2"].ToString();
            if (!string.IsNullOrEmpty(len1) && !string.IsNullOrEmpty(len2))
            {
                if (!int.TryParse(len1, out snLength1) || !int.TryParse(len2,out snLength2))
                {
                    MessageBox.Show("配置文件格式错误！","提示",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
        }

        private void EventHandlers()
        {
            this.tb_sn.TextChanged += Tb_sn_TextChanged;
            this.radGridView1.CellClick += RadGridView1_CellClick;
            this.menu_update.Click += Menu_update_Click;
            this.menu_delete.Click += Menu_delete_Click;
            this.menu_grid.Click += Menu_grid_Click;
            this.menu_add.Click += Menu_add_Click;
            this.menu_clear_db.Click += Menu_clear_db_Click;
        }

        private void Menu_add_Click(object sender, EventArgs e)
        {
            AddBindingData();
        }

        async private void Menu_clear_db_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.tb_caseCode.Text))
                return;
            if (MessageBox.Show($"确定清除箱子编码为{this.tb_caseCode.Text}的所有绑定记录？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                int del = 0;//await serviceClient.DeleteProductBindingDataAsync(this.tb_caseCode.Text.Trim());
                if (del > 0)
                    MessageBox.Show($"已清除{del}条绑定数据！","提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }


        private void Menu_grid_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.radGridView1.Rows.Count; i++)
            {
                this.radGridView1.Rows[i].Delete();
            }
        }

        private void Menu_delete_Click(object sender, EventArgs e)
        {
            this.radGridView1.CurrentRow.Delete();
        }

        private void Menu_update_Click(object sender, EventArgs e)
        {
            UpdateData();
        }

        private void RadGridView1_CellClick(object sender, GridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            if (e.ColumnIndex == 0)
            {
                this.radGridView1.Rows[e.RowIndex].Delete();
            }
        }

        private void Tb_sn_TextChanged(object sender, EventArgs e)
        {
            tb_sn.Text = tb_sn.Text.Trim();
            if (tb_sn.TextLength == snLength1 || tb_sn.TextLength == snLength2)
            {
                AddBindingData();
            }
        }

        /// <summary>
        /// 扫描SN，查询该产品信息，添加到列表
        /// </summary>
        private void AddBindingData()
        {
            //查询信息：序号+SN+型号+绑定状态+备注
            if (IsExistSn())
                return;
            this.radGridView1.Rows.AddNew();
            int startIndex = this.radGridView1.Rows.Count - 1;
            this.radGridView1.Rows[startIndex].Cells[0].Value = Resources.bullet_delete;
            this.radGridView1.Rows[startIndex].Cells[1].Value = startIndex + 1;
            this.radGridView1.Rows[startIndex].Cells[2].Value = tb_sn.Text.Trim();
            var bindingRes = SelectBindingState();
            this.radGridView1.Rows[startIndex].Cells[3].Value = bindingRes;
            if (bindingRes == "已绑定")
                this.radGridView1.Rows[startIndex].Cells[3].Style.ForeColor = Color.Red;
            this.tb_sn.Clear();
        }

        private bool IsExistSn()
        {
            if (this.radGridView1.Rows.Count < 1)
                return false;
            foreach (var rowInfo in this.radGridView1.Rows)
            {
                var sn = rowInfo.Cells[2].Value.ToString();
                if (tb_sn.Text.Trim().Equals(sn))
                    return true;
            }
            return false;
        }

        private string SelectBindingState()
        {
            var sn = tb_sn.Text.Trim();
            if (sn == "")
                return "";
            var ds = serviceClient.SelectProductBindingState(sn).Tables[0];
            if (ds.Rows.Count < 1)
                return "未绑定";
            var state = ds.Rows[0][0].ToString();
            if (state == "0")
                return "未绑定";
            else if (state == "1")
            {
                return "已绑定";
            }
            else
                return "";
        }

        async private void UpdateData()
        {
            //MesService.PackageProduct[] packageProducts = new MesService.PackageProduct[this.radGridView1.Rows.Count];
            int index = 0;
            if (this.radGridView1.Rows.Count < 1)
                return;
            var caseCode = this.tb_caseCode.Text.Trim();
            if (string.IsNullOrEmpty(caseCode))
            {
                MessageBox.Show("箱子编码不能为空！","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }
            foreach (var rowInfo in this.radGridView1.Rows)
            {
                var productSN = rowInfo.Cells[2].Value.ToString();
                var bindingState = rowInfo.Cells[3].Value.ToString();
                var remark = "";
                if (rowInfo.Cells[4].Value != null)
                    remark = rowInfo.Cells[4].Value.ToString();

                //MesService.PackageProduct packageProduct = new MesService.PackageProduct();
                //packageProduct.CaseCode = caseCode;
                //packageProduct.SnOutter = productSN;
                //packageProduct.BindingState = 1;
                //packageProduct.BindingDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //packageProduct.Remark = remark;
                //packageProducts[index] = packageProduct;
                index++;
            }
            var res = 0;// await serviceClient.CommitPackageProductAsync(packageProducts);
            if (res == 1)
            {
                for (int i = 0; i < this.radGridView1.Rows.Count; i++)
                {
                    this.radGridView1.Rows[i].Delete();
                }
                var dt = 0;// (await serviceClient.SelectProductBindingCountAsync(this.tb_caseCode.Text.Trim(),"1")).Tables[0];
                this.tool_curNumber.Text = "";// dt.Rows.Count.ToString();
                this.tool_materialCode.Text = tb_caseCode.Text;
                MessageBox.Show("更新成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("更新失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
