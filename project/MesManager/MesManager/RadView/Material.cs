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

namespace MesManager.RadView
{
    public partial class Material : RadForm
    {
        private MesService.MesServiceClient serviceClient;
        private DataTable dataSource;
        private const string DATA_ORDER = "序号";
        private const string DATA_MATERIAL = "物料编码";
        private const string DATA_AMOUNT = "物料库存";
        private string keyMaterialCode;//记录修改前的编码
        private List<string> materialCodeTemp;//存储用户修改的物料编码
        private string curMaterialCode;//记录鼠标右键选中行编码

        public Material()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
        }

        async private void Material_Load(object sender, EventArgs e)
        {
            serviceClient = new MesService.MesServiceClient();
            DataGridViewCommon.SetRadGridViewProperty(this.radGridView1,true);
            materialCodeTemp = new List<string>();
            rlbx_explain.Text = "在新行添加物料名称、物料库存，右键行头可删除行数据";
            //设置第一列为只读
            this.radGridView1.DataSource = DataSource();
            SelectMaterial();//查询数据
            this.radGridView1.Columns[0].ReadOnly = true;

            this.radGridView1.CellBeginEdit += RadGridView1_CellBeginEdit;
            this.radGridView1.CellEndEdit += RadGridView1_CellEndEdit;
            this.radGridView1.ContextMenuOpening += RadGridView1_ContextMenuOpening;
            this.radGridView1.MouseDown += RadGridView1_MouseDown;
        }

        private void RadGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (this.radGridView1.CurrentRow.Index < 0)
                    return;
                curMaterialCode = this.radGridView1.CurrentRow.Cells[1].Value.ToString().Trim();
            }
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
                        e.ContextMenu.Items[i].Click += Delete_Material_Click;
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
                        e.ContextMenu.Items[i].Click += Delete_Material_Click;
                        break;
                }
            }
        }

        async private void Delete_Material_Click(object sender, EventArgs e)
        {
            //cut 执行delete 服务数据
            if (MessageBox.Show("是否删除该行数据", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                int del = await serviceClient.DeleteMaterialAsync(curMaterialCode);
            }
            //刷新一下
            SelectMaterial();
        }

        private void RadGridView1_CellEndEdit(object sender, GridViewCellEventArgs e)
        {
            //结束编辑，记录下value;与编辑前比较，值改变则执行修改
            string curMaterialCode = "";
            if (this.radGridView1.CurrentRow.Cells[1].Value != null)
                curMaterialCode = this.radGridView1.CurrentRow.Cells[1].Value.ToString();

            if (curMaterialCode != keyMaterialCode)
            {
                materialCodeTemp.Add(keyMaterialCode);
            }
        }

        private void RadGridView1_CellBeginEdit(object sender, GridViewCellCancelEventArgs e)
        {
            //开始编辑，记录下value
            if(this.radGridView1.CurrentRow.Cells[1].Value != null)
                keyMaterialCode = this.radGridView1.CurrentRow.Cells[1].Value.ToString();
        }

        private DataTable DataSource()
        {
            if (dataSource == null)
            {
                dataSource = new DataTable();
                dataSource.Columns.Add(DATA_ORDER);
                dataSource.Columns.Add(DATA_MATERIAL);
                dataSource.Columns.Add(DATA_AMOUNT);
            }
            return dataSource;
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Btn_apply_Click(object sender, EventArgs e)
        {
            CommitMesService();
            SelectMaterial();
        }

        private void Btn_clear_dgv_Click(object sender, EventArgs e)
        {
            dataSource.Clear();
            this.radGridView1.DataSource = dataSource;
        }

        async private void Btn_clear_server_data_Click(object sender, EventArgs e)
        {
            //清除所有数据库数据
            DialogResult dialogResult = MessageBox.Show("是否清除数据库数据","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.OK)
            {
                await serviceClient.DeleteMaterialAsync("");
            }
        }

        private void Btn_select_Click(object sender, EventArgs e)
        {
            SelectMaterial();
        }

        #region 调用接口
        async private void SelectMaterial()
        {
            //调用查询接口
            DataSet dataSet = await serviceClient.SelectMaterialAsync("");
            DataTable dataTable = dataSet.Tables[0];
            dataSource.Clear();
            if (dataTable.Rows.Count > 0)
            {
                //显示数据
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    DataRow dr = dataSource.NewRow();
                    dr[DATA_ORDER] = i + 1;
                    dr[DATA_MATERIAL] = dataTable.Rows[i][0].ToString();
                    dr[DATA_AMOUNT] = dataTable.Rows[i][1].ToString();
                    dataSource.Rows.Add(dr);
                }
                radGridView1.DataSource = dataSource;
            }
            else
            {
                dataSource.Clear();
                radGridView1.DataSource = dataSource;
            }
        }

        async private void CommitMesService()
        {
            try
            {
                //提交新增行记录、修改非主键记录
                int row = radGridView1.RowCount;
                //MesService.MaterialMsg[] materialMsg = new MesService.MaterialMsg[row];
                for (int i = 0; i < row; i++)
                {
                    //MesService.MaterialMsg material = new MesService.MaterialMsg();
                    var materialCode = radGridView1.Rows[i].Cells[1].Value.ToString().Trim();
                    var amount = radGridView1.Rows[i].Cells[2].Value.ToString().Trim();
                    //material.MaterialCode = materialCode;
                    //material.MaterialName = "";
                    //materialMsg[i] = material;
                }
                //判断主键是否有修改，将原记录删除后，再执行其他更新
                foreach (var code in materialCodeTemp)
                {
                    await serviceClient.DeleteMaterialAsync(code);
                }
                string res = "";//await serviceClient.CommitMaterialAsync(materialMsg);
                if (res == "1")
                {
                    MessageBox.Show("更新成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"更新失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        #endregion
    }
}
