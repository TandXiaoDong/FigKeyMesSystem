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

        /// <summary>
        /// 绑定分页总页数
        /// </summary>
        private DataTable bindRowSource;

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
            this.bindingNavigator1.ItemClicked += BindingNavigator1_ItemClicked;
            this.bindingNavigatorCountItem.TextChanged += BindingNavigatorCountItem_TextChanged;
            this.bindingNavigatorPositionItem.TextChanged += BindingNavigatorPositionItem_TextChanged;

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
        async private void SelectProductBindMsg()
        {
            var queryCondition = this.tb_search.Text.Trim();
            this.radGridView1.DataSource = null;
            this.radGridView1.Update();
            if (queryCondition != "")
            {
                this.currentPage = 1;//根据条件查询
                this.bindingNavigatorPositionItem.Text = currentPage.ToString();
            }
            var bindState = "1";
            if (this.rb_binded.CheckState == CheckState.Checked)
                bindState = "1";
            else if (rb_unbinded.CheckState == CheckState.Checked)
                bindState = "0";
            var checkPackage = await serviceClient.SelectPackageProductCheckAsync(queryCondition,bindState,false,currentPage,pageSize);
            RadGridViewProperties.SetRadGridViewProperty(this.radGridView1, false,true,0);
            this.radGridView1.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.None;
            this.radGridView1.ReadOnly = true;
            if (checkPackage.CheckPackageCaseNumber % pageSize > 0)
            {
                pageCount = checkPackage.CheckPackageCaseNumber / pageSize + 1;
            }
            else
            {
                pageCount = checkPackage.CheckPackageCaseNumber / pageSize;
            }
            var dtSource = InitBindRowSource();
            this.radGridView1.BeginEdit();
            var dt = checkPackage.CheckPackageCaseData.Tables[0];
            this.radGridView1.DataSource = dt;
            this.bindingSource1.DataSource = dtSource;
            this.bindingNavigator1.BindingSource = this.bindingSource1; 
            this.radGridView1.EndEdit();
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

        private void BindingNavigatorPositionItem_TextChanged(object sender, EventArgs e)
        {
            //this.bindingNavigatorPositionItem.Text = currentPage.ToString();
        }

        private void BindingNavigatorCountItem_TextChanged(object sender, EventArgs e)
        {
            //this.bindingNavigatorCountItem.Text = "/" + pageCount;
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
            SelectProductBindMsg();
        }
    }
}
