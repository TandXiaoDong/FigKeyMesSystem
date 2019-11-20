using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.Docking;
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
        private string keyOldMaterialDescrible;

        #region 物料统计字段
        private const string MATERIAL_STOCK_ORDER = "序号";
        private const string MATERIAL_PN = "物料号";
        private const string MATERIAL_LOT = "批次号";
        private const string MATERIAL_RID = "料盘号";
        private const string MATERIAL_DC = "收料日期";
        private const string MATERIAL_NAME = "物料名称";
        private const string MATERIAL_QTY = "入库库存";
        private const string MATERIAL_STOCK_OUT = "出库库存";
        private const string MATERIAL_DECRIBLE = "备注";
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
            this.radDock1.ActiveWindow = this.dw_materialBind;
            DataGridViewCommon.SetRadGridViewProperty(this.radGridViewBind, true);
            DataGridViewCommon.SetRadGridViewProperty(this.radGridViewStock,false);
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
                this.tool_bind_delete.Enabled = false;
                this.tool_bind_update.Enabled = false;
                this.tool_bind_cleardb.Enabled = false;
                this.tool_stockManager_deleteSignal.Enabled = false;
                this.tool_stockManager_ClearDB.Enabled = false;
                this.tool_stockManager_update.Enabled = false;
            }
        }

        private void InitMaterialRID()
        {
            var dt = serviceClient.SelectMaterial("",MesService.MaterialStockState.PUT_IN_STOCK).Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var materialCode = dt.Rows[i][0].ToString();
                    AnalysisMaterialCode analysisMaterialCode = AnalysisMaterialCode.GetMaterialDetail(materialCode);
                    var materialRID = analysisMaterialCode.MaterialRID;
                    this.tool_stock_queryCondition.Items.Add(materialRID);
                }
                this.tool_stock_queryCondition.AutoCompleteSource = AutoCompleteSource.ListItems;
                this.tool_stock_queryCondition.AutoCompleteMode = AutoCompleteMode.Suggest;
                this.tool_bind_queryCondition.AutoCompleteSource = AutoCompleteSource.ListItems;
                this.tool_bind_queryCondition.AutoCompleteMode = AutoCompleteMode.Suggest;
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
            materialStockData.Columns.Add(MATERIAL_DECRIBLE);
            materialStockData.Columns.Add(ADMIN);
            materialStockData.Columns.Add(UPDATE_DATE);
        }

        private void EventHandlers()
        {
            this.radGridViewBind.ContextMenuOpening += RadGridView1_ContextMenuOpening;
            this.radGridViewBind.CellBeginEdit += RadGridView1_CellBeginEdit;
            this.radGridViewBind.CellEndEdit += RadGridView1_CellEndEdit;
            this.radGridViewStock.CellBeginEdit += RadGridView2_CellBeginEdit;
            this.radGridViewStock.CellEndEdit += RadGridView2_CellEndEdit;

            this.tool_bind_add.Click += Tool_bind_add_Click;
            this.tool_bind_delete.Click += Tool_bind_delete_Click;
            this.tool_bind_cleardb.Click += Tool_bind_cleardb_Click;
            this.tool_bind_cleargrid.Click += Tool_bind_cleargrid_Click;
            this.tool_bind_query.Click += Tool_bind_query_Click;
            this.tool_bind_update.Click += Tool_bind_update_Click;

            this.tool_stockManager_add.Click += Tool_stockManager_add_Click;
            this.tool_stockManager_deleteSignal.Click += Tool_stockManager_deleteSignal_Click;
            this.tool_stockManager_ClearGrid.Click += Tool_stockManager_ClearGrid_Click;
            this.tool_stockManager_ClearDB.Click += Tool_stockManager_ClearDB_Click;
            this.tool_stockManager_query.Click += Tool_stockManager_query_Click;
            this.tool_stockManager_update.Click += Tool_stockManager_update_Click;

            this.radDock1.ActiveWindowChanged += RadDock1_ActiveWindowChanged;
        }

        private void RadDock1_ActiveWindowChanged(object sender, Telerik.WinControls.UI.Docking.DockWindowEventArgs e)
        {
            if (this.radDock1.ActiveWindow == this.dw_materialBind)
            {
                materialType = MaterialType.MATERIAL_BINDING;
                BindingDataSource();
            }
            else if (this.radDock1.ActiveWindow == this.dw_stockManager)
            {
                materialType = MaterialType.MATERIAL_STOCK_MODIFY;
                BindingMaterialStock(this.tool_stock_queryCondition.Text);
            }
        }

        private void Tool_bind_update_Click(object sender, EventArgs e)
        {
            CommitData();
        }

        private void Tool_stockManager_update_Click(object sender, EventArgs e)
        {
            CommitData();
        }

        private void Tool_stockManager_query_Click(object sender, EventArgs e)
        {
            RefreshGridView();
        }

        private void Tool_stockManager_ClearDB_Click(object sender, EventArgs e)
        {
            DeleteAllRowData();
        }

        private void Tool_stockManager_ClearGrid_Click(object sender, EventArgs e)
        {
            for (int i = this.radGridViewStock.Rows.Count - 1; i >= 0; i--)
            {
                this.radGridViewStock.Rows[i].Delete();
            }
        }

        private void Tool_stockManager_deleteSignal_Click(object sender, EventArgs e)
        {
            DeleteRowData();
        }

        private void Tool_stockManager_add_Click(object sender, EventArgs e)
        {
            //新增空行
            this.radGridViewStock.CurrentRow = this.radGridViewStock.Rows[this.radGridViewStock.Rows.Count - 1];
            this.radGridViewStock.Rows.AddNew();
        }

        private void Tool_bind_query_Click(object sender, EventArgs e)
        {
            RefreshGridView();
        }

        private void Tool_bind_cleargrid_Click(object sender, EventArgs e)
        {
            for (int i = this.radGridViewBind.Rows.Count - 1; i >= 0; i--)
            {
                this.radGridViewBind.Rows[i].Delete();
            }
        }

        private void Tool_bind_cleardb_Click(object sender, EventArgs e)
        {
            DeleteAllRowData();
        }

        private void Tool_bind_delete_Click(object sender, EventArgs e)
        {
            DeleteRowData();
        }

        private void Tool_bind_add_Click(object sender, EventArgs e)
        {
            //新增空行
            this.radGridViewBind.CurrentRow = this.radGridViewBind.Rows[this.radGridViewBind.Rows.Count - 1];
            this.radGridViewBind.Rows.AddNew();
        }

        private void RadGridView2_CellEndEdit(object sender, GridViewCellEventArgs e)
        {
            var materialStock = this.radGridViewStock.CurrentRow.Cells[6].Value;
            var describle = this.radGridViewStock.CurrentRow.Cells[7].Value;
            var materialRid = this.radGridViewStock.CurrentRow.Cells[3].Value;
            if (materialStock == null)
                return;
            if (materialStock.ToString() != keyOldMaterialStock || describle.ToString() != keyOldMaterialDescrible)
            {
                ProductMaterial productMaterial = new ProductMaterial();
                var materialCode = serviceClient.GetMaterialCode(materialRid.ToString());
                productMaterial.keyMaterialCode = materialCode;
                productMaterial.keyOldMaterialStock = materialStock.ToString();
                productMaterial.keyOldMaterialDescrible = describle.ToString();
                this.pmStockList.Add(productMaterial);
            }
        }

        private void RadGridView2_CellBeginEdit(object sender, GridViewCellCancelEventArgs e)
        {
            var materialStock = this.radGridViewStock.CurrentRow.Cells[6].Value;
            var materialDescrible = this.radGridViewStock.CurrentRow.Cells[7].Value;
            if (materialStock != null)
            {
                keyOldMaterialStock = materialStock.ToString();
            }
            if (materialDescrible != null)
            {
                keyOldMaterialDescrible = materialDescrible.ToString();
            }
        }

        private void RadGridView1_CellEndEdit(object sender, GridViewCellEventArgs e)
        {
            //结束编辑，记录下value;与编辑前比较，值改变则执行修改
            var typeNo = this.radGridViewBind.CurrentRow.Cells[1].Value;
            var materialPN = this.radGridViewBind.CurrentRow.Cells[2].Value;
            var describle = this.radGridViewBind.CurrentRow.Cells[3].Value;
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
            var typeNo = this.radGridViewBind.CurrentRow.Cells[1].Value;
            var materialPN = this.radGridViewBind.CurrentRow.Cells[2].Value;
            var describle = this.radGridViewBind.CurrentRow.Cells[3].Value;
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

        private void Menu_clear_db_Click(object sender, EventArgs e)
        {
            
        }

        private void RefreshGridView()
        {
            if (materialType == MaterialType.MATERIAL_BINDING)
            {
                SelectData();
            }
            else if (materialType == MaterialType.MATERIAL_STOCK_MODIFY)
            {
                BindingMaterialStock(this.tool_stock_queryCondition.Text);
            }
        }

        private void CommitData()
        {
            if (materialType == MaterialType.MATERIAL_BINDING)
            {
                this.tool_bind_queryCondition.Focus();
                UpdateData();
            }
            else if (materialType == MaterialType.MATERIAL_STOCK_MODIFY)
            {
                this.tool_stock_queryCondition.Focus();
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
            this.radGridViewBind.CurrentRow = this.radGridViewBind.Rows[this.radGridViewBind.Rows.Count-1];
            this.radGridViewBind.Rows.AddNew();
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
            this.radGridViewBind.Dock = DockStyle.Fill;
            this.radGridViewBind.Visible = true;
            this.radGridViewStock.Visible = false;
            this.radGridViewBind.DataSource = null;
            GridViewTextBoxColumn order = this.radGridViewBind.Columns[DataGridViewColumnName.rdvc_order.ToString()] as GridViewTextBoxColumn;
            //GridViewComboBoxColumn materialCode = this.radGridView1.Columns[DataGridViewColumnName.rdvc_materialCode.ToString()] as GridViewComboBoxColumn;
            GridViewComboBoxColumn productTypeNo = this.radGridViewBind.Columns[DataGridViewColumnName.rdvc_typeNo.ToString()] as GridViewComboBoxColumn;
            GridViewTextBoxColumn describle = this.radGridViewBind.Columns[DataGridViewColumnName.rdvc_describle.ToString()] as GridViewTextBoxColumn;
            DataTable materialCodeDt = (await serviceClient.SelectMaterialPNAsync()).Tables[0];//0
            DataTable typeNoDt = (await serviceClient.SelectProductContinairCapacityAsync("")).Tables[0];//1

            List<string> materialListTemp = new List<string>();
            List<string> typeNoListTemp = new List<string>();

            this.radGridViewBind.BeginEdit();

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
            this.radGridViewBind.EndEdit();
            //设置第一列不可编辑
            this.radGridViewBind.Columns[0].ReadOnly = true;
            this.radGridViewBind.Columns[5].ReadOnly = true;
            this.radGridViewBind.Columns[6].ReadOnly = true;
            this.pmListTemp.Clear();
        }

        async private void BindingMaterialStock(string queryCondition)
        {
            this.radGridViewStock.Dock = DockStyle.Fill;
            this.radGridViewStock.Visible = true;
            this.radGridViewBind.Visible = false;

            var dt = (await serviceClient.SelectMaterialAsync(queryCondition,MesService.MaterialStockState.PUT_IN_STOCK)).Tables[0];
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
                    dr[MATERIAL_DECRIBLE] = dt.Rows[i][4].ToString();
                    this.materialStockData.Rows.Add(dr);
                }
            }
            this.radGridViewStock.DataSource = materialStockData;
            this.radGridViewStock.Columns[0].ReadOnly = true;
            this.radGridViewStock.Columns[1].ReadOnly = true;
            this.radGridViewStock.Columns[2].ReadOnly = true;
            this.radGridViewStock.Columns[3].ReadOnly = true;
            this.radGridViewStock.Columns[4].ReadOnly = true;
            this.radGridViewStock.Columns[5].ReadOnly = true;
            this.radGridViewStock.Columns[8].ReadOnly = true;
            this.radGridViewStock.Columns[0].BestFit();
        }

        async private void UpdateData()
        {
            this.radGridViewBind.CurrentRow = this.radGridViewBind.Rows[this.radGridViewBind.Rows.Count - 1];
            int rowCount = CalRowCount();
            MesService.ProductMaterial[] productMaterialList = new MesService.ProductMaterial[rowCount];
            int row = 0;
            foreach (var rowInfo in this.radGridViewBind.Rows)
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
            List<string> materialCodeTemp = new List<string>();
            List<string> materialCodeTemp2 = new List<string>();
            foreach (var productMaterial in pmStockList)
            {
                int modifyStock;
                int.TryParse(productMaterial.keyOldMaterialStock.Trim(), out modifyStock);
                //更新库存前防错：修改库存必须大于0小于实际用的数量
                MesService.MaterialStockEnum materialStockEnum = serviceClient.ModifyMaterialStock(productMaterial.keyMaterialCode,
                    modifyStock, productMaterial.keyOldMaterialDescrible, MESMainForm.currentUser);
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
                else if (materialStockEnum == MesService.MaterialStockEnum.STATUS_NOT_ZERO_STOCK)
                {
                    updateRes = false;
                    
                    if (!materialCodeTemp.Contains(productMaterial.keyMaterialCode))
                    {
                        MessageBox.Show("修改库存必须大于0！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        materialCodeTemp.Add(productMaterial.keyMaterialCode);
                    }
                }
                else if (materialStockEnum == MesService.MaterialStockEnum.STATUS_STOCK_NOT_SMALLER_AMOUNTED)
                {
                    updateRes = false;
                    if (!materialCodeTemp2.Contains(productMaterial.keyMaterialCode))
                    {
                        MessageBox.Show($"【{productMaterial.keyMaterialCode}】修改库存未大于实际使用数量！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        materialCodeTemp2.Add(productMaterial.keyMaterialCode);
                    }
                }
            }
            pmStockList.Clear();
            materialCodeTemp2.Clear();
            materialCodeTemp.Clear();
            if (updateRes)
                MessageBox.Show("修改成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            BindingMaterialStock("");
        }

        async private void DeleteRowData()
        {
            if (materialType == MaterialType.MATERIAL_BINDING)
            {
                if (this.radGridViewBind.RowCount < 1)
                {
                    MessageBox.Show("当前没有可删除的数据！","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    return;
                }
                MesService.ProductMaterial productMaterial = new MesService.ProductMaterial();
                if (this.radGridViewBind.CurrentRow.Cells[2].Value != null)
                    productMaterial.MaterialCode = this.radGridViewBind.CurrentRow.Cells[2].Value.ToString();
                if (this.radGridViewBind.CurrentRow.Cells[1].Value != null)
                    productMaterial.TypeNo = this.radGridViewBind.CurrentRow.Cells[1].Value.ToString();
                if (this.radGridViewBind.CurrentRow.Cells[1].Value == null && this.radGridViewBind.CurrentRow.Cells[2].Value == null
                    && this.radGridViewBind.CurrentRow.Cells[3].Value == null)
                {
                    this.radGridViewBind.CurrentRow.Delete();
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
            else if (materialType == MaterialType.MATERIAL_STOCK_MODIFY)
            {
                if (this.radGridViewStock.RowCount < 1)
                {
                    MessageBox.Show("当前没有可删除的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var materialRid = this.radGridViewStock.CurrentRow.Cells[3].Value.ToString();
                if (MessageBox.Show($"确认删除料盘号为【{materialRid}】的物料？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    var materialCode = serviceClient.GetMaterialCode(materialRid);
                    int res = serviceClient.DeleteMaterial(materialCode);
                    if (res > 0)
                    {
                        MessageBox.Show("删除物料成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("删除物料失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    SelectData();
                }
            }
        }

        async private void DeleteAllRowData()
        {
            if (materialType == MaterialType.MATERIAL_BINDING)
            {
                if (this.radGridViewBind.RowCount < 1)
                {
                    MessageBox.Show("没有可以删除的数据！","提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    return;
                }
                if (MessageBox.Show("确认删除【物料绑定】的所有数据？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
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
            }
            else if (materialType == MaterialType.MATERIAL_STOCK_MODIFY)
            {
                if (this.radGridViewBind.RowCount < 1)
                {
                    MessageBox.Show("没有可以删除的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (MessageBox.Show("确认删除【库存管理】的所有数据？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                    return;
                int res = await serviceClient.DeleteAllMaterialAsync();
                if (res > 0)
                {
                    MessageBox.Show("清除服务数据完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("清除服务数据失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            SelectData();
        }

        async private void SelectData()
        {
            var bindCondition = this.tool_bind_queryCondition.Text;
            DataTable dt = (await serviceClient.SelectProductMaterialAsync(bindCondition)).Tables[0];
            this.radGridViewBind.DataSource = null;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                this.radGridViewBind.Rows.AddNew();//序号/型号/物料号/描述/操作员/更新日期
                this.radGridViewBind.Rows[i].Cells[0].Value = i + 1;
                this.radGridViewBind.Rows[i].Cells[1].Value = dt.Rows[i][0].ToString();
                var materialPN = dt.Rows[i][1].ToString();
                var materialName = dt.Rows[i][2].ToString();
                this.radGridViewBind.Rows[i].Cells[3].Value = materialName;//物料名称
                this.radGridViewBind.Rows[i].Cells[4].Value = dt.Rows[i][3].ToString();//描述
                this.radGridViewBind.Rows[i].Cells[5].Value = dt.Rows[i][4].ToString();//用户
                this.radGridViewBind.Rows[i].Cells[6].Value = dt.Rows[i][5].ToString();//日期
                //var materialName = serviceClient.SelectMaterialName(materialPN);
                //if (materialName != "")
                //{
                //    materialName = "(" + materialName + ")";
                //}
                this.radGridViewBind.Rows[i].Cells[2].Value = materialPN;
            }
            //删除空行
            int startIndex = dt.Rows.Count;
            int rowCount = this.radGridViewBind.Rows.Count;
            if (this.radGridViewBind.Rows.Count > dt.Rows.Count)
            {
                for (int i = rowCount - 1; i >= startIndex; i--)
                {
                    this.radGridViewBind.Rows[i].Delete();
                }
            }    
        }

        private int CalRowCount()
        {
            int count = 0;
            this.radGridViewBind.CurrentRow = this.radGridViewBind.Rows[this.radGridViewBind.Rows.Count - 1];

            foreach (var rowInfo in this.radGridViewBind.Rows)
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
