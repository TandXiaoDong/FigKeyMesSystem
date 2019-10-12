using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using MesManager.Control;

namespace MesManager.RadView
{
    public partial class PackageProduct : Telerik.WinControls.UI.RadForm
    {
        private MesService.MesServiceClient serviceClient;
        private DataTable dataSource;
        private const string CASE_CODE = "箱子编码";
        private const string SN_CODE = "追溯码";
        private const string TYPE_NO = "产品型号";
        private const string PICTURE = "图片";
        private const string BINDING_STATE = "绑定状态";
        private const string BINDING_DATE = "绑定日期";
        public PackageProduct()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void PackageProduct_Load(object sender, EventArgs e)
        {
            Init();
            cb_caseCode.SelectedIndexChanged += Cb_caseCode_SelectedIndexChanged;
            cb_caseCode.TextChanged += Cb_caseCode_TextChanged;
            tb_sn.TextChanged += Tb_sn_TextChanged;
        }

        private void Tb_sn_TextChanged(object sender, EventArgs e)
        {
            //输入完成,由条码长度决定
            if (tb_sn.Text.Length == 13)
            {
                //查询产品型号

                //自动执行绑定
                if (ch_auto_bingding.CheckState == CheckState.Checked)
                {
                    CommitBinding();
                }
            }
        }

        private void Cb_caseCode_TextChanged(object sender, EventArgs e)
        {
            foreach (var v in cb_caseCode.Items)
            {
                if (cb_caseCode.Text == v.ToString())
                {
                    UpdateCaseAmount(cb_caseCode.Text.Trim());
                }
            }
            if (cb_caseCode.Text.Length == 13)
            {
                //编码长度固定时有效
            }
        }

        private void Cb_caseCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCaseAmount(cb_caseCode.SelectedItem.ToString());
        }

        async private void UpdateCaseAmount(string caseCode)
        {
            //由箱子编码查询箱子容量，更新
            if (string.IsNullOrEmpty(caseCode))
                return;
            DataTable dt = (await serviceClient.SelectProductContinairCapacityAsync(caseCode)).Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    tb_case_amount.Text = dt.Rows[i][1].ToString();
                }
            }
        }

        async private void Init()
        {
            serviceClient = new MesService.MesServiceClient();
            //packageProduct = new MesService.PackageProduct();
            InitCaseCodeList();
            //获取型号
            cb_typeNo.Items.Clear();
            DataTable dt = null;// (await serviceClient.SelectProductTypeNoAsync("")).Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                cb_typeNo.Items.Add(dt.Rows[i][0].ToString());
            }
            cb_typeNo.Items.Add("");
            DataSource();
            DataGridViewCommon.SetRadGridViewProperty(this.radGridView1,false);
        }

        async private void InitCaseCodeList()
        {
            DataTable dt = (await serviceClient.SelectProductContinairCapacityAsync("")).Tables[0];
            cb_caseCode.Items.Clear();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cb_caseCode.Items.Add(dt.Rows[i][0].ToString());
                }
                cb_caseCode.Items.Add("");
            }
        }

        private DataTable DataSource()
        {
            if (dataSource == null)
            {
                dataSource = new DataTable();
                dataSource.Columns.Add(CASE_CODE);
                dataSource.Columns.Add(SN_CODE);
                dataSource.Columns.Add(TYPE_NO);
                dataSource.Columns.Add(PICTURE);
                dataSource.Columns.Add(BINDING_STATE);
                dataSource.Columns.Add(BINDING_DATE);
            }
            return dataSource;
        }

        async private void CommitBinding()
        {
            string caseCode = cb_caseCode.Text.Trim();
            string caseAmount = tb_case_amount.Text.Trim();
            string sn = tb_sn.Text.Trim();
            string typeNo = cb_typeNo.Text.Trim();
            if (string.IsNullOrEmpty(caseCode))
            {
                tb_sn.Focus();
                MessageBox.Show("箱子编码不能为空!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(sn))
            {
                tb_sn.Focus();
                MessageBox.Show("箱子容量不能为空!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(sn))
            {
                tb_sn.Focus();
                MessageBox.Show("条码不能为空!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(typeNo))
            {
                tb_sn.Focus();
                MessageBox.Show("零件号不能为空!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //提交箱子容量
            //MesService.PackageProduct[] packageProducts = new MesService.PackageProduct[10];
            //await serviceClient.CommitProductContinairCapacityAsync(caseCode,caseAmount,"","");
            //packageProduct.CaseCode = caseCode;
            //packageProduct.SnOutter = sn;
            //packageProduct.TypeNo = typeNo;
            //packageProduct.BindingState = 1;
            //packageProduct.BindingDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //packageProduct.Picture = UpLoadImage.ProductImage;
            int x = 0;//await serviceClient.CommitPackageProductAsync(packageProducts);
            //绑定完成后，添加到显示列表
            UpLoadImage.ProductImage = null;
            if (x < 1)
            {
                MessageBox.Show("绑定失败！","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
            //packageProduct.BindingState = 1;//查询绑定成功的记录
            //packageProduct.CaseCode = "";
            //packageProduct.SnOutter = "";
            //SelectBindingData(packageProduct.CaseCode,packageProduct.SnOutter);
        }

        /// <summary>
        /// 解除绑定set state = 0;
        /// </summary>
        async private void DelBindingRow()
        {
            var casecode = this.radGridView1.CurrentRow.Cells[0].Value.ToString();
            var sncode = this.radGridView1.CurrentRow.Cells[1].Value.ToString();
            //packageProduct.CaseCode = casecode;
            //packageProduct.SnOutter = sncode;
            //packageProduct.BindingState = 0;
            ////await serviceClient.UpdatePackageProductAsync(packageProduct);
            ////更新查询结果,查询所有已绑定的数据
            //packageProduct.BindingState = 1;
            //packageProduct.CaseCode = "";
            //packageProduct.SnOutter = "";
            //SelectBindingData(packageProduct.CaseCode,packageProduct.SnOutter);
        }

        async private void SelectBindingData(string caseCode,string snOutter)
        {
            //packageProduct.CaseCode = caseCode;
            //packageProduct.SnOutter = snOutter;
            //DataTable dt = (await serviceClient.SelectPackageProductAsync(packageProduct)).Tables[0];
            //if (dt.Rows.Count < 1)
            //    return;
            //DataRow[] dataRows = dt.Select("","binding_date desc");
            //dataSource.Clear();
            //foreach (DataRow dataRow in dataRows)
            //{
            //    dataSource.Rows.Add(dataRow.ItemArray);
            //}
            //this.radGridView1.DataSource = dataSource;
            //this.radGridView1.CurrentRow = this.radGridView1.Rows[0];
        }

        private void Btn_apply_Click(object sender, EventArgs e)
        {
            CommitBinding();
        }

        private void Btn_upLoad_Click(object sender, EventArgs e)
        {
            UpLoadImage upLoadImage = new UpLoadImage();
            upLoadImage.ShowDialog();
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            //解除绑定
            DelBindingRow();
        }

        private void Btn_refresh_Click(object sender, EventArgs e)
        {
            InitCaseCodeList();
            //更新查询结果,查询所有已绑定的数据
            //packageProduct.BindingState = 1;
            //packageProduct.CaseCode = "";
            //packageProduct.SnOutter = "";
            //SelectBindingData(packageProduct.CaseCode, packageProduct.SnOutter);
        }

        async private void Btn_delRow_Click(object sender, EventArgs e)
        {
            //删除选择行数据
            var casecode = this.radGridView1.CurrentRow.Cells[0].Value.ToString();
            var sncode = this.radGridView1.CurrentRow.Cells[1].Value.ToString();
            //packageProduct.CaseCode = casecode;
            //packageProduct.SnOutter = sncode;
            ////await serviceClient.DeletePackageProductAsync(packageProduct);
            ////更新查询
            //packageProduct.BindingState = 1;
            //packageProduct.CaseCode = "";
            //packageProduct.SnOutter = "";
            //SelectBindingData(packageProduct.CaseCode, packageProduct.SnOutter);
        }

        private void Btn_clearLocal_Click(object sender, EventArgs e)
        {
            //清空表数据
            dataSource.Clear();
            this.radGridView1.DataSource = dataSource;
        }

        async private void Btn_clearServer_Click(object sender, EventArgs e)
        {
            //清空数据库
            //packageProduct.CaseCode = "";
            //packageProduct.SnOutter = "";
            ////await serviceClient.DeletePackageProductAsync(packageProduct);
            ////更新查询
            //packageProduct.BindingState = 1;
            //packageProduct.CaseCode = "";
            //packageProduct.SnOutter = "";
            //SelectBindingData(packageProduct.CaseCode, packageProduct.SnOutter);
        }
    }
}
