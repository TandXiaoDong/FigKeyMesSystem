using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using CommonUtils.Logger;
using MesManager.Control;
using MesManager.RadView;
using System.Configuration;
using WindowsFormTelerik.GridViewExportData;
using CommonUtils.FileHelper;
using MesManager.Common;

namespace MesManager.UI
{
    /// <summary>
    /// 管理产品型号、基础物料、产线站位信息
    /// </summary>
    public partial class BasicConfig : RadForm
    {
        #region 私有变量
        private MesService.MesServiceClient serviceClient;
        private MesServiceTest.MesServiceClient serviceClientTest;
        private DataTable typeNoData,materialData;
        private List<string> modifyTypeNoTemp;
        private List<BasicConfig> modifyProductTypeNoList;
        private List<BasicConfig> materialCodeTemp;//存储用户修改的物料编码
        private string modifyMaterialPn;
        private string modifyMaterialDescible;
        private string keyOldTypeNo;
        private string keyNewTypeNo;
        private string keyMaterialCode;//记录修改前的编码
        private string keyDescrible;
        private string keyMaterialName;
        private string keyProductStorage;
        private string curMaterialCode;//记录鼠标右键选中行编码
        private string curRowTypeNo;
        private const string DATA_ORDER = "序号";
        private const string DATA_MATERIAL_CODE = "物料号";
        private const string DATA_MATERIAL_NAME = "物料名称";
        private const string DATA_TYPENO_NAME = "型号名称";
        private const string DATA_CONTAINER_CAPACITY = "容器容量";
        private const string DATA_USER_NAME = "用户名";
        private const string DATA_UPDATE_DATE = "更新日期";
        private const string DATA_DESCRIBLE = "描述说明";
        private int materialCodeLength;
        private int IsAutoAdd;
        #endregion

        public BasicConfig()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void BasicConfig_Load(object sender, EventArgs e)
        {
            Init();
            EventHandlers();
            RefreshControl();
        }

        private void RefreshControl()
        {
            var userType = MESMainForm.currentUsetType;
            if (userType != 0)
            {
                //没有权限，设置不可修改
                this.menu_add.Enabled = false;
                this.menu_del.Enabled = false;
                this.menu_clear_db.Enabled = false;
                this.menu_commit.Enabled = false;
                this.menu_clear_db.Enabled = false;
                this.radGridView1.Enabled = false;
            }
        }

        private void Init()
        {
            serviceClient = new MesService.MesServiceClient();
            serviceClientTest = new MesServiceTest.MesServiceClient();
            modifyTypeNoTemp = new List<string>();
            modifyProductTypeNoList = new List<BasicConfig>();
            materialCodeTemp = new List<BasicConfig>();
            DataGridViewCommon.SetRadGridViewProperty(this.radGridView1,true);
            this.radGridView1.AllowRowHeaderContextMenu = false;
            var bMaterialCode = int.TryParse(ConfigurationManager.AppSettings["materialLength"].ToString(), out materialCodeLength);
            int.TryParse(ConfigurationManager.AppSettings["IsAutoAdd"].ToString(),out IsAutoAdd);
            this.menu_exportCondition.Items.Add(GridViewExport.ExportFormat.EXCEL.ToString());
            this.menu_exportCondition.Items.Add(GridViewExport.ExportFormat.HTML.ToString());
            this.menu_exportCondition.Items.Add(GridViewExport.ExportFormat.PDF.ToString());
            this.menu_exportCondition.Items.Add(GridViewExport.ExportFormat.CSV.ToString());
            this.menu_exportCondition.Text = GridViewExport.ExportFormat.EXCEL.ToString();
            DataSource();
            RefreshData();
            if (!bMaterialCode)
            {
                MessageBox.Show("配置参数格式错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DataSource()
        {
            if (materialData == null)
            {
                materialData = new DataTable();
                materialData.Columns.Add(DATA_ORDER);
                materialData.Columns.Add(DATA_MATERIAL_CODE);
                materialData.Columns.Add(DATA_MATERIAL_NAME);
                materialData.Columns.Add(DATA_USER_NAME);
                materialData.Columns.Add(DATA_UPDATE_DATE);
                materialData.Columns.Add(DATA_DESCRIBLE);
            }
            if (typeNoData == null)
            {
                typeNoData = new DataTable();
                typeNoData.Columns.Add(DATA_ORDER);
                typeNoData.Columns.Add(DATA_TYPENO_NAME);
                typeNoData.Columns.Add(DATA_CONTAINER_CAPACITY);
                typeNoData.Columns.Add(DATA_USER_NAME);
                typeNoData.Columns.Add(DATA_UPDATE_DATE);
                typeNoData.Columns.Add(DATA_DESCRIBLE);
            }
        }

        private void EventHandlers()
        {
            this.menu_refresh.Click += Menu_refresh_Click;
            this.menu_grid.Click += Menu_grid_Click;
            this.menu_clear_db.Click += Menu_clear_db_Click;
            this.menu_del.Click += Menu_del_Click;
            this.menu_add.Click += Menu_add_Click;
            this.menu_export.Click += Menu_export_Click;

            this.radGridView1.CellBeginEdit += RadGridView1_CellBeginEdit;
            this.radGridView1.CellEndEdit += RadGridView1_CellEndEdit;
        }

        private void Tb_materialInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)//扫码有回车符
            {
                UpdateAutoAddMaterialRow();
            }
        }

        private void Menu_export_Click(object sender, EventArgs e)
        {
            GridViewExport.ExportFormat exportFormat = GridViewExport.ExportFormat.EXCEL;
            Enum.TryParse(this.menu_exportCondition.Text,out exportFormat);
            GridViewExport.ExportGridViewData(exportFormat,this.radGridView1);
        }

        private void Tb_materialInput_TextChanged(object sender, EventArgs e)
        {
            //输入完成,由条码长度决定
            //自动添加到行
            //if (OptionMaterialCode(this.tb_materialInput.Text.Trim()))
            //    UpdateAutoAddMaterialRow();
            
        }

        /// <summary>
        /// 扫码无回车符的自动添加
        /// </summary>
        /// <param name="inputText"></param>
        /// <returns></returns>
        private bool OptionMaterialCode(string inputText)
        {
           // LogHelper.Log.Info($"【扫描物料编码】code={this.tb_materialInput.Text} len=" + this.tb_materialInput.Text.Length);
            var materialCode = inputText;

            if (materialCode.Contains("&"))
            {
                var materialRID = materialCode.Substring(0, materialCode.IndexOf('&'));
                materialCode = materialCode.Substring(materialCode.IndexOf('&') + 1);
                if (materialCode.Length <= 1)
                    return false;
            }
            if (materialCode.Contains("&"))
            {
                var materialSID = materialCode.Substring(0, materialCode.IndexOf('&'));
                materialCode = materialCode.Substring(materialCode.IndexOf('&') + 1);
                if (materialCode.Length <= 1)
                    return false;
            }
            if (materialCode.Contains("&"))
            {
                var materialPN = materialCode.Substring(0, materialCode.IndexOf('&'));
                materialCode = materialCode.Substring(materialCode.IndexOf('&') + 1);
                if (materialCode.Length <= 1)
                    return false;
            }
            if (materialCode.Contains("&"))
            {
                var materialQTY = materialCode.Substring(0, materialCode.IndexOf('&'));
                materialCode = materialCode.Substring(materialCode.IndexOf('&') + 1);
                if (materialCode.Length <= 1)
                    return false;
            }
            if (materialCode.Contains("&"))
            {
                var materialDC = materialCode.Substring(0, materialCode.IndexOf('&'));
                materialCode = materialCode.Substring(materialCode.IndexOf('&') + 1);
                if (materialCode.Length <= 1)
                    return false;
            }
            if (materialCode.Length == 13)
            {
                return true;
            }
            return false;
        }

        private void UpdateAutoAddMaterialRow()
        {
            this.radGridView1.Rows.AddNew();
            int rIndex = this.radGridView1.Rows.Count;
            this.radGridView1.Rows[rIndex - 1].Cells[0].Value = rIndex;
            //this.radGridView1.Rows[rIndex - 1].Cells[1].Value = this.tb_materialInput.Text;
        }

        private void Menu_add_Click(object sender, EventArgs e)
        {
            this.radGridView1.Rows.AddNew();
        }

        async private void Menu_del_Click(object sender, EventArgs e)
        {
            //删除当前行
            var typeNo = this.radGridView1.CurrentRow.Cells[1].Value.ToString();
            if (string.IsNullOrEmpty(typeNo))
            {
                this.radGridView1.CurrentRow.Delete();
            }
            else
            {
                if (MessageBox.Show($"确认要删除产品【{typeNo}】？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information,MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    int row = await serviceClient.DeleteProductContinairCapacityAsync(typeNo);
                    LogHelper.Log.Info($"【删除产品】产品名称={typeNo}影响行数={row} 操作用户={MESMainForm.currentUser}");
                    if (row > 0)
                    {
                        //删除产品后，同时将工艺删除
                        row = await serviceClient.DeleteProcessAsync(typeNo);
                        LogHelper.Log.Info($"【删除工艺】工艺名称={typeNo}影响行数={row} 操作用户={MESMainForm.currentUser}");
                        RefreshData();
                    }
                }
            }
        }

        private void Menu_clear_db_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要清空服务所有数据？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
                return;
            //型号
            int row = 0;// await serviceClient.DeleteAllProductTypeNoAsync();
            RefreshData();
        }

        private void Menu_grid_Click(object sender, EventArgs e)
        {
            for (int i = this.radGridView1.Rows.Count - 1; i >= 0; i--)
            {
                this.radGridView1.Rows[i].Delete();
            }
        }

        private void RadGridView1_CellEndEdit(object sender, GridViewCellEventArgs e)
        {
            var key = this.radGridView1.CurrentRow.Cells[1].Value;
            var keyName = this.radGridView1.CurrentRow.Cells[2].Value;
            var kdescrible = this.radGridView1.CurrentRow.Cells[5].Value;
            if (key == null || kdescrible == null || keyName == null)//行不存在
                return;
            if (keyOldTypeNo != key.ToString() || keyDescrible != kdescrible.ToString() || keyProductStorage != keyName.ToString())
            {
                modifyTypeNoTemp.Add(this.keyOldTypeNo);
                if (keyOldTypeNo != key.ToString())
                {
                    BasicConfig basicConfig = new BasicConfig();
                    basicConfig.keyOldTypeNo = keyOldTypeNo;
                    basicConfig.keyNewTypeNo = key.ToString();
                    modifyProductTypeNoList.Add(basicConfig);
                }
            }
        }

        private void RadGridView1_CellBeginEdit(object sender, GridViewCellCancelEventArgs e)
        {
            var key = this.radGridView1.CurrentRow.Cells[1].Value;
            var key_name = this.radGridView1.CurrentRow.Cells[2].Value;//名称/容量
            var key_describle = this.radGridView1.CurrentRow.Cells[5].Value;
            if (key == null && key_describle == null || key_name == null)//行不存在
                return;

            this.keyOldTypeNo = key.ToString();
            this.keyDescrible = key_describle.ToString();
            this.keyProductStorage = key_name.ToString();
        }

        private void Cb_cfgType_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void Menu_refresh_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void Menu_commit_Click(object sender, EventArgs e)
        {
            //this.cb_cfgType.Focus();
            CommitData();
        }

        private void RefreshData()
        {
            SelectProductTypeData();
            //物料
            //this.menu_del.Enabled = false;
            //this.menu_clear_db.Enabled = false;
            //this.menu_add.Enabled = false;
            //SelectMaterial();
        }

        private void CommitData()
        {
            CommitTypeNoMesService();
        }

        #region 调用接口

        private void SelectProductTypeData()
        {
            //调用查询接口
            radGridView1.DataSource = null;
            DataSet dataSet = serviceClient.SelectProductContinairCapacity("");
            DataTable dataTable = dataSet.Tables[0];
            typeNoData.Clear();
            if (dataTable.Rows.Count > 0)
            {
                //显示数据
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    DataRow dr = typeNoData.NewRow();
                    dr[DATA_ORDER] = i + 1;
                    dr[DATA_TYPENO_NAME] = dataTable.Rows[i][0].ToString();
                    dr[DATA_CONTAINER_CAPACITY] = dataTable.Rows[i][1].ToString();
                    dr[DATA_USER_NAME] = dataTable.Rows[i][2].ToString();
                    dr[DATA_UPDATE_DATE] = dataTable.Rows[i][3].ToString();
                    dr[DATA_DESCRIBLE] = dataTable.Rows[i][4].ToString();
                    typeNoData.Rows.Add(dr);
                }
                radGridView1.DataSource = typeNoData;
                SetGridReadOnly();
            }
            else
            {
                typeNoData.Clear();
                radGridView1.DataSource = typeNoData;
            }
            DataGridViewCommon.SetRadGridViewProperty(this.radGridView1, true);
            SetGridReadOnly();
        }

        private void SetGridReadOnly()
        {
            this.radGridView1.Columns[0].ReadOnly = true;
            this.radGridView1.Columns[3].ReadOnly = true;
            this.radGridView1.Columns[4].ReadOnly = true;
        }

        private void SelectMaterial()
        {
            //调用查询接口
            radGridView1.DataSource = null;
            DataSet dataSet = serviceClient.SelectMaterialPN();
            DataTable dataTable = dataSet.Tables[0];
            materialData.Clear();
            if (dataTable.Rows.Count > 0)
            {
                //显示数据
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    DataRow dr = materialData.NewRow();
                    dr[DATA_ORDER] = i + 1;
                    dr[DATA_MATERIAL_CODE] = dataTable.Rows[i][0].ToString();
                    dr[DATA_MATERIAL_NAME] = dataTable.Rows[i][1].ToString();
                    dr[DATA_USER_NAME]     = dataTable.Rows[i][2].ToString();
                    dr[DATA_UPDATE_DATE]   = dataTable.Rows[i][3].ToString();
                    dr[DATA_DESCRIBLE]     = dataTable.Rows[i][4].ToString();
                    materialData.Rows.Add(dr);
                }
                radGridView1.DataSource = materialData;
            }
            else
            {
                materialData.Clear();
                radGridView1.DataSource = materialData;
            }
            DataGridViewCommon.SetRadGridViewProperty(this.radGridView1, false);
            this.radGridView1.Columns[0].ReadOnly = true;
            this.radGridView1.Columns[1].ReadOnly = true;
            materialCodeTemp.Clear();
            modifyTypeNoTemp.Clear();
            modifyProductTypeNoList.Clear();
        }

        async private void CommitTypeNoMesService()
        {
            try
            {
                //修改行数据
                if (modifyTypeNoTemp.Count > 0)
                {
                    foreach (var val in this.modifyTypeNoTemp)
                    {
                        await serviceClient.DeleteProductContinairCapacityAsync(val);
                    }
                }

                int row = radGridView1.RowCount;
                string[] array = new string[row];
                //新增行数据
                for (int i = 0; i < row; i++)
                {
                    var ID = radGridView1.Rows[i].Cells[0].Value.ToString().Trim();
                    var productName = radGridView1.Rows[i].Cells[1].Value.ToString().Trim();
                    var storage = radGridView1.Rows[i].Cells[2].Value.ToString().Trim();
                    var describle = radGridView1.Rows[i].Cells[5].Value.ToString().Trim();
                    if (!string.IsNullOrEmpty(productName))
                    {
                        array[i] = productName;
                        var res = await serviceClient.CommitProductContinairCapacityAsync(productName,storage,MESMainForm.currentUser,describle);
                        if (res < 1)
                        {
                            MessageBox.Show($"【{productName}】更新失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                }
                MessageBox.Show("更新成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshData();
                //将其他产品型号替换成新的型号
                foreach (var basic in this.modifyProductTypeNoList)
                {
                    serviceClient.UpdateAllProductTypeNo(basic.keyOldTypeNo,basic.keyNewTypeNo);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error(ex.Message + "\r\n" + ex.StackTrace);
                MessageBox.Show($"{ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        async private void CommitMaterialMesService()
        {
            try
            {
                if (materialCodeTemp.Count > 0)
                {
                    foreach (var material in materialCodeTemp)
                    {
                        int res = await serviceClient.UpdateMaterialPNAsync(material.keyMaterialCode,
                            material.keyMaterialName,MESMainForm.currentUser);
                    }
                }
                RefreshData();
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error(ex.Message + "\r\n" + ex.StackTrace);
                MessageBox.Show($"{ex.Message}","ERROR",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void ModifyStandConfigProductType(string oldTypeNo,string newTypeNo)
        {
            StandConfigData standConfigData = new StandConfigData();
            standConfigData.ReadProductTypeConfigToSaveAs(oldTypeNo,newTypeNo);
        }
        #endregion
    }
}
