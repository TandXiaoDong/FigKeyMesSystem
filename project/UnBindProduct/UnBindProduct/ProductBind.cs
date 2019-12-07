using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace UnBindProduct
{
    public partial class ProductBind : RadForm
    {
        MesServiceTest.MesServiceClient serviceClient;

        public ProductBind()
        {
            InitializeComponent();
        }

        private void ProductBind_Load(object sender, EventArgs e)
        {
            serviceClient = new MesServiceTest.MesServiceClient();
            this.btn_search.Click += Btn_search_Click;
            this.btn_bing.Click += Btn_bing_Click;
            this.btn_unbind.Click += Btn_unbind_Click;
        }

        private void Btn_unbind_Click(object sender, EventArgs e)
        {
            UpdateBindState(0);
        }

        private void Btn_bing_Click(object sender, EventArgs e)
        {
            UpdateBindState(1);
        }

        private void Btn_search_Click(object sender, EventArgs e)
        {
            
        }

        private void UpdateBindState(int state)
        {
            var caseSN = this.tb_caseSN.Text;
            var productSN = this.tb_productSN.Text;
            var productTypeNo = this.cb_typeNo.Text;
            var stationName = this.cb_station.Text;
            if (caseSN == "")
                return;
            if (productSN == "")
                return;
            if (productTypeNo == "")
                return;
            if (stationName == "")
                return;
            string[] result = serviceClient.UpdatePackageProductBindingMsg(caseSN, productSN, productTypeNo, stationName, state.ToString(), "单独修改", "wtsys", "wtsys");
            MessageBox.Show(result[0]);
            if (result[0] == "0X03")
            {
                MessageBox.Show("解除绑定成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (result[0] == "0X09")
            {
                MessageBox.Show("添加绑定成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
