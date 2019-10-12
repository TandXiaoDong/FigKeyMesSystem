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
using MesManager.Common;

namespace MesManager.UI
{
    //物料绑定信息：序号+物料编码（可选）+ 产品型号（可选）+ 工站名称（可选）+描述
    public partial class ProductMaterial : RadForm
    {
        MesService.MesServiceClient serviceClient;
        private string keyMaterialCode;
        private string keyTypeNo;
        private string keyDescrible;
        private string keyOldMaterialStock;

        #region 物料统计字段
        private const string MATERIAL_STOCK_ORDER = "序号";
        private const string MATERIAL_PN = "物料号";
        private const string MATERIAL_LOT = "批次号";
        private const string MATERIAL_RID = "料盘号";
        private const string MATERIAL_DC = "收料日期";
        private const string MATERIAL_NAME = "物料名称";
        private const string MATERIAL_QTY = "入库库存";
        private const string ADMIN = "管理员";
        private const string UPDATE_DATE = "更新日期";
        #endregion

        private List<ProductMaterial> pmListTemp;
        private List<ProductMaterial> pmStockList;
        private DataTable materialStockData;
        private int editRowIndex;
        private MaterialType materialType;

        public ProductMaterial()
        {
            InitializeComponent();
            //this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private enum MaterialType
        {
            MATERIAL_BINDING,
            MATERIAL_STOCK_MODIFY
        }

        private void ProductMaterial_Load(object sender, EventArgs e)
        {
            Init();
            RefreshControl();
            EventHandlers();
        }

        private void Init()
        {
            serviceClient = new MesService.MesServiceClient();
            pmListTemp = new List<ProductMaterial>();
            pmStockList = new List<ProductMaterial>();
            DataGridViewCommon.SetRadGridViewProperty(this.radGridView1, true);
            DataGridViewCommon.SetRadGridViewProperty(this.radGridView2,false);
            BindingDataSource();
            InitDataTable();
            InitMaterialRID();
        }

        private void RefreshControl()
        {
            var userType = MESMainForm.currentUsetType;
            if (userType != 0)
            {
                //没有权限，设置不可修改
                this.menu_add_row.Enabled = false;
                this.menu_delete.Enabled = false;
                this.menu_clear_db.Enabled = false;
                this.menu_update.Enabled = false;
                this.menu_clear_db.Enabled = false;
                this.radGridView1.Enabled = false;
            }
        }

        private void InitMaterialRID()
        {
            var dt = serviceClient.SelectMaterial("").Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var materialCode = dt.Rows[i][0].ToString();
                    AnalysisMaterialCode analysisMaterialCode = AnalysisMaterialCode.GetMaterialDetail(materialCode);
                    var materialRID = analysisMaterialCode.MaterialRID;
                    this.tool_queryFilter.Items.Add(materialRID);
                }
                this.tool_queryFilter.AutoCompleteSource = AutoCompleteSource.ListItems;
                this.tool_queryFilter.AutoCompleteMode = AutoCompleteMode.Suggest;
            }
        }

        private void InitDataTable()
        {
            materialStockData = new DataTable();
            materialStockData.Columns.Add(MATERIAL_STOCK_ORDER);
            materialStockData.Columns.Add(MATERIAL_PN);
            materialStockData.Columns.Add(MATERIAL_LOT);
            materialStockData.Columns.Add(MATERIAL_RID);
            materialStockData.Columns.Add(MATERIAL_DC);
            materialStockData.Columns.Add(MATERIAL_NAME);
            materialStockData.Columns.Add(MATERIAL_QTY);
            materialStockData.Columns.Add(ADMIN);
            materialStockData.Columns.Add(UPDATE_DATE);
        }

        private void EventHandlers()
        {
            this.menu_add_row.Click += Menu_add_row_Click;
            this.menu_delete.Click += Menu_delete_Click;
            this.menu_update.Click += Menu_update_Click;
            this.menu_refresh.Click += Menu_refresh_Click;
            this.menu_clear_db.Click += Menu_clear_db_Click;
            this.menu_grid.Click += Menu_grid_Click;
            this.tool_material_binding.Click += Tool_material_binding_Click;
            this.tool_material_stock.Click += Tool_material_stock_Click;

            this.radGridView1.ContextMenuOpening += RadGridView1_ContextMenuOpening;
            this.radGridView1.CellBeginEdit += RadGridView1_CellBeginEdit;
            this.radGridView1.CellEndEdit += RadGridView1_CellEndEdit;
            this.radGridView2.CellBeginEdit += RadGridView2_CellBeginEdit;
            this.radGridView2.CellEndEdit += RadGridView2_CellEndEdit;
        }

        private void RadGridView2_CellEndEdit(object sender, GridViewCellEventArgs e)
        {
            var materialStock = this.radGridView2.CurrentRow.Cells[6].Value;
            var materialRid = this.radGridView2.CurrentRow.Cells[3].Value;
            if (materialStock == null)
                return;
            if (materialStock.ToString() != keyOldMaterialStock)
            {
                ProductMaterial productMaterial = new ProductMaterial();
                productMaterial.keyOldMaterialStock = materialStock.ToString();
                productMaterial.keyMaterialCode = serviceClient.GetMaterialCode(materialRid.ToString());
                this.pmStockList.Add(productMaterial);
            }
        }

        private void RadGridView2_CellBeginEdit(object sender, GridViewCellCancelEventArgs e)
        {
            var materialStock = this.radGridView2.CurrentRow.Cells[6].Value;
            if (materialStock == null)
                return;
            keyOldMaterialStock = materialStock.ToString();
        }

        private void Tool_material_stock_Click(object sender, EventArgs e)
        {
            materialType = MaterialType.MATERIAL_STOCK_MODIFY;
            BindingMaterialStock(this.tool_queryFilter.Text);
        }

        private void Tool_material_binding_Click(object sender, EventArgs e)
        {
            materialType = MaterialType.MATERIAL_BINDING;
            BindingDataSource();
        }

        private void RadGridView1_CellEndEdit(object sender, GridViewCellEventArgs e)
        {
            //结束编辑，记录下value;与编辑前比较，值改变则执行修改
            var typeNo = this.radGridView1.CurrentRow.Cells[1].Value;
            var materialPN = this.radGridView1.CurrentRow.Cells[2].Value;
            var describle = this.radGridView1.CurrentRow.Cells[3].Value;
            if (typeNo == null || materialPN == null || describle == null)
                return;
            if (materialPN.ToString().Contains("("))
                materialPN = materialPN.ToString().Substring(0, materialPN.ToString().IndexOf('('));

            if (materialPN.ToString() != keyMaterialCode || typeNo.ToString() != keyTypeNo || this.keyDescrible != describle.ToString())
            {
                ProductMaterial productMaterial = new ProductMaterial();
                productMaterial.keyMaterialCode = keyMaterialCode;

                productMaterial.keyTypeNo = keyTypeNo;
                pmListTemp.Add(productMaterial);
            }
        }

        private void RadGridView1_CellBeginEdit(object sender, GridViewCellCancelEventArgs e)
        {
            //开始编辑，记录下value
            var typeNo = this.radGridView1.CurrentRow.Cells[1].Value;
            var materialPN = this.radGridView1.CurrentRow.Cells[2].Value;
            var describle = this.radGridView1.CurrentRow.Cells[3].Value;
            if (typeNo == null || materialPN == null || describle == null)
                return;
            this.keyTypeNo = typeNo.ToString();
            if (materialPN.ToString().Contains("("))
                materialPN = materialPN.ToString().Substring(0, materialPN.ToString().IndexOf('('));
            this.keyMaterialCode = materialPN.ToString();
            this.keyDescrible = describle.ToString();
        }

        private void RadGridView1_ContextMenuOpening(object sender, ContextMenuOpeningEventArgs e)
        {
            for (int i = 0; i < e.ContextMenu.Items.Count; i++)
            {
                String contextMenuText = e.ContextMenu.Items[i].Text;
                switch (contextMenuText)
                {
                    case "Conditional Formatting":
                        e.ContextMenu.Items[i].Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
                        e.ContextMenu.Items[i + 1].Visibility = ElementVisibility.Collapsed;
                        break;
                    case "Hide Column":
                        e.ContextMenu.Items[i].Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
                        break;
                    case "Pinned state":
                        e.ContextMenu.Items[i].Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
                        break;
                    case "Best Fit":
                        e.ContextMenu.Items[i].Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
                        break;
                    case "Cut":
                        e.ContextMenu.Items[i].Click += Delete_Row_Click;
                        break;
                    case "Copy":
                        break;
                    case "Paste":
                        break;
                    case "Edit":
                        break;
                    case "Clear Value":
                        break;
                    case "Delete Row":
                        e.ContextMenu.Items[i].Click += Delete_Row_Click;
                        break;
                }
            }
        }

        private void Delete_Row_Click(object sender, EventArgs e)
        {
            DeleteRowData();
        }

        private void Menu_grid_Click(object sender, EventArgs e)
        {
            for (int i = this.radGridView1.Rows.Count - 1;i>=0; i--)
            {
                this.radGridView1.Rows[i].Delete();
            }
        }

        private void Menu_clear_db_Click(object sender, EventArgs e)
        {
            DeleteAllRowData();
        }

        private void Menu_refresh_Click(object sender, EventArgs e)
        {
            if (materialType == MaterialType.MATERIAL_BINDING)
            {
                SelectData();
            }
            else if (materialType == MaterialType.MATERIAL_STOCK_MODIFY)
            {
                BindingMaterialStock("");
            }
        }

        private void Menu_update_Click(object sender, EventArgs e)
        {
            if (materialType == MaterialType.MATERIAL_BINDING)
                UpdateData();
            else if (materialType == MaterialType.MATERIAL_STOCK_MODIFY)
            {
                this.tool_queryFilter.Focus();
                UpdateMaterialStock();
            }
        }

        private void Menu_delete_Click(object sender, EventArgs e)
        {
            DeleteRowData();
        }

        private void Menu_add_row_Click(object sender, EventArgs e)
        {
            //新增空行
            this.radGridView1.CurrentRow = this.radGridView1.Rows[this.radGridView1.Rows.Count-1];
            this.radGridView1.Rows.AddNew();
        }

        public enum DataGridViewColumnName
        {
            rdvc_order,
            rdvc_materialCode,
            rdvc_typeNo,
            rdvc_station,
            rdvc_describle
        }
        async private void BindingDataSource()
        {
            this.radGridView1.Dock = DockStyle.Fill;
            this.radGridView1.Visible = true;
            this.radGridView2.Visible = false;
            this.radGridView1.DataSource = null;
            GridViewTextBoxColumn order = this.radGridView1.Columns[DataGridViewColumnName.rdvc_order.ToString()] as GridViewTextBoxColumn;
            //GridViewComboBoxColumn materialCode = this.radGridView1.Columns[DataGridViewColumnName.rdvc_materialCode.ToString()] as GridViewComboBoxColumn;
            GridViewComboBoxColumn productTypeNo = this.radGridView1.Columns[DataGridViewColumnName.rdvc_typeNo.ToString()] as GridViewComboBoxColumn;
            GridViewTextBoxColumn describle = this.radGridView1.Columns[DataGridViewColumnName.rdvc_describle.ToString()] as GridViewTextBoxColumn;
            DataTable materialCodeDt = (await serviceClient.SelectMaterialPNAsync()).Tables[0];//0
            DataTable typeNoDt = (await serviceClient.SelectProductContinairCapacityAsync("")).Tables[0];//1

            List<string> materialListTemp = new List<string>();
            List<string> typeNoListTemp = new List<string>();

            this.radGridView1.BeginEdit();

            if (materialCodeDt.Rows.Count > 0)
            {
                for (int i = 0; i < materialCodeDt.Rows.Count; i++)
                {
                    var materialPN = materialCodeDt.Rows[i][0].ToString();
                    var materialName = materialCodeDt.Rows[i][1].ToString();

                    if (!materialListTemp.Contains(materialPN))
                    {
                        if (!string.IsNullOrEmpty(materialName))
                        {
                            materialListTemp.Add(materialPN + "(" + materialName + ")");
                        }
                        else
                        {
                            materialListTemp.Add(materialPN);
                        }
                    }
                }
            }
            if (typeNoDt.Rows.Count > 0)
            {
                for (int i = 0; i < typeNoDt.Rows.Count; i++)
                {
                    typeNoListTemp.Add(typeNoDt.Rows[i][0].ToString());
                }
            }
            //materialCode.DataSource = materialListTemp;
            productTypeNo.DataSource = typeNoListTemp;
            SelectData();
            this.radGridView1.EndEdit();
            //设置第一列不可编辑
            this.radGridView1.Columns[0].ReadOnly = true;
            this.radGridView1.Columns[5].ReadOnly = true;
            this.radGridView1.Columns[6].ReadOnly = true;
            this.pmListTemp.Clear();
        }

        async private void BindingMaterialStock(string queryCondition)
        {
            this.radGridView2.Dock = DockStyle.Fill;
            this.radGridView2.Visible = true;
            this.radGridView1.Visible = false;

            var dt = (await serviceClient.SelectMaterialAsync(queryCondition)).Tables[0];
            materialStockData.Clear();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var materialCode = dt.Rows[i][0].ToString();
                    var materialStock = dt.Rows[i][5].ToString();
                    AnalysisMaterialCode analysisMaterialCode = AnalysisMaterialCode.GetMaterialDetail(materialCode);
                    DataRow dr = materialStockData.NewRow();
                    dr[MATERIAL_STOCK_ORDER] = i + 1;
                    dr[MATERIAL_PN] = analysisMaterialCode.MaterialPN;
                    dr[MATERIAL_LOT] = analysisMaterialCode.MaterialLOT;
                    dr[MATERIAL_RID] = analysisMaterialCode.MaterialRID;
                    dr[MATERIAL_DC] = analysisMaterialCode.MaterialDC;
                    dr[MATERIAL_NAME] = serviceClient.SelectMaterialName(analysisMaterialCode.MaterialPN);
                    dr[MATERIAL_QTY] = materialStock;
                    dr[UPDATE_DATE] = dt.Rows[i][3].ToString();
                    dr[ADMIN] = dt.Rows[i][2].ToString();
                    this.materialStockData.Rows.Add(dr);
                }
            }
            this.radGridView2.DataSource = materialStockData;
            this.radGridView2.Columns[0].ReadOnly = true;
            this.radGridView2.Columns[1].ReadOnly = true;
            this.radGridView2.Columns[2].ReadOnly = true;
            this.radGridView2.Columns[3].ReadOnly = true;
            this.radGridView2.Columns[4].ReadOnly = true;
            this.radGridView2.Columns[5].ReadOnly = true;
            this.radGridView2.Columns[7].ReadOnly = true;
            this.radGridView2.Columns[8].ReadOnly = true;
            this.radGridView2.Columns[0].BestFit();
        }

        async private void UpdateData()
        {
            this.radGridView1.CurrentRow = this.radGridView1.Rows[this.radGridView1.Rows.Count - 1];
            int rowCount = CalRowCount();
            MesService.ProductMaterial[] productMaterialList = new MesService.ProductMaterial[rowCount];
            int row = 0;
            foreach (var rowInfo in this.radGridView1.Rows)
            {
                MesService.ProductMaterial productMaterial = new MesService.ProductMaterial();
                if (rowInfo.Cells[1].Value != null)
                    productMaterial.TypeNo = rowInfo.Cells[1].Value.ToString();
                if (rowInfo.Cells[2].Value != null)
                {
                    var materialPn = rowInfo.Cells[2].Value.ToString();
                    if(materialPn.Contains("("))
                        materialPn = materialPn.Substring(0, materialPn.IndexOf('('));
                    productMaterial.MaterialCode = materialPn;
                    //更新编码库存
                    if (productMaterial.MaterialCode.Contains("&"))
                    {
                        productMaterial.Stock = AnalysisMaterialCode.GetMaterialDetail(productMaterial.MaterialCode).MaterialQTY;
                    }
                }
                if (rowInfo.Cells[3].Value != null)
                    productMaterial.MaterialName = rowInfo.Cells[3].Value.ToString();
                if (rowInfo.Cells[4].Value != null)
                    productMaterial.Describle = rowInfo.Cells[4].Value.ToString();
                productMaterial.UserName = MESMainForm.currentUser;

                if (rowInfo.Cells[1].Value != null && rowInfo.Cells[2].Value != null)
                    productMaterialList[row] = productMaterial;
                row++;
            }
            //delete修改数据
            foreach (var item in pmListTemp)
            {
                MesService.ProductMaterial productMaterial = new MesService.ProductMaterial();
                productMaterial.MaterialCode = item.keyMaterialCode;
                productMaterial.TypeNo = item.keyTypeNo;
                int del = await serviceClient.DeleteProductMaterialAsync(productMaterial);
            }
            pmListTemp.Clear();
            MesService.ProductMaterial[] materialList = await serviceClient.CommitProductMaterialAsync(productMaterialList);
            foreach (var material in materialList)
            {
                if (material.Result != 1)
                {
                    MessageBox.Show("更新失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            MessageBox.Show("更新成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SelectData();
        }

        private void UpdateMaterialStock()
        {
            if (pmStockList.Count < 1)
                return;
            bool updateRes = true;
            foreach (var productMaterial in pmStockList)
            {
                int modifyStock;
                int.TryParse(productMaterial.keyOldMaterialStock.Trim(),out modifyStock);
                MesService.MaterialStockEnum materialStockEnum = serviceClient.ModifyMaterialStock(productMaterial.keyMaterialCode,modifyStock,MESMainForm.currentUser);
                if (materialStockEnum == MesService.MaterialStockEnum.STATUS_FAIL)
                {
                    updateRes = false;
                    MessageBox.Show("更新失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (materialStockEnum == MesService.MaterialStockEnum.ERROR_MATERIAL_IS_NOT_EXIST)
                {
                    updateRes = false;
                    MessageBox.Show("物料编码错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            if(updateRes)
                MessageBox.Show("修改成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            BindingMaterialStock("");
        }

        async private void DeleteRowData()
        {
            MesService.ProductMaterial productMaterial = new MesService.ProductMaterial();
            if(this.radGridView1.CurrentRow.Cells[2].Value != null)
                productMaterial.MaterialCode = this.radGridView1.CurrentRow.Cells[2].Value.ToString();
            if(this.radGridView1.CurrentRow.Cells[1].Value != null)
                productMaterial.TypeNo = this.radGridView1.CurrentRow.Cells[1].Value.ToString();
            if (this.radGridView1.CurrentRow.Cells[1].Value == null && this.radGridView1.CurrentRow.Cells[2].Value == null 
                && this.radGridView1.CurrentRow.Cells[3].Value == null)
            {
                this.radGridView1.CurrentRow.Delete();
                return;
            }
            if (MessageBox.Show($"确认解除物料【{productMaterial.MaterialCode}】与产品【{productMaterial.TypeNo}】的绑定？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                int res = await serviceClient.DeleteProductMaterialAsync(productMaterial);
                if (res > 0)
                {
                    MessageBox.Show("解除绑定成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("解除绑定失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                SelectData();
            }
        }

        async private void DeleteAllRowData()
        {
            if (MessageBox.Show("确认删除所有数据？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                return;
            int res = await serviceClient.DeleteAllProductContinairCapacityAsync();
            if (res > 0)
            {
                MessageBox.Show("清除服务数据完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("清除服务数据失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            SelectData();
        }

        async private void SelectData()
        {
            DataTable dt = (await serviceClient.SelectProductMaterialAsync()).Tables[0];
            this.radGridView1.DataSource = null;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                this.radGridView1.Rows.AddNew();//序号/型号/物料号/描述/操作员/更新日期
                this.radGridView1.Rows[i].Cells[0].Value = i + 1;
                this.radGridView1.Rows[i].Cells[1].Value = dt.Rows[i][0].ToString();
                var materialPN = dt.Rows[i][1].ToString();
                var materialName = dt.Rows[i][2].ToString();
                this.radGridView1.Rows[i].Cells[3].Value = materialName;//物料名称
                this.radGridView1.Rows[i].Cells[4].Value = dt.Rows[i][3].ToString();//描述
                this.radGridView1.Rows[i].Cells[5].Value = dt.Rows[i][4].ToString();//用户
                this.radGridView1.Rows[i].Cells[6].Value = dt.Rows[i][5].ToString();//日期
                //var materialName = serviceClient.SelectMaterialName(materialPN);
                //if (materialName != "")
                //{
                //    materialName = "(" + materialName + ")";
                //}
                this.radGridView1.Rows[i].Cells[2].Value = materialPN;
            }
            //删除空行
            int startIndex = dt.Rows.Count;
            int rowCount = this.radGridView1.Rows.Count;
            if (this.radGridView1.Rows.Count > dt.Rows.Count)
            {
                for (int i = rowCount - 1; i >= startIndex; i--)
                {
                    this.radGridView1.Rows[i].Delete();
                }
            }    
        }

        private int CalRowCount()
        {
            int count = 0;
            this.radGridView1.CurrentRow = this.radGridView1.Rows[this.radGridView1.Rows.Count - 1];

            foreach (var rowInfo in this.radGridView1.Rows)
            {
                if (rowInfo.Cells[1].Value != null && rowInfo.Cells[2].Value != null)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
