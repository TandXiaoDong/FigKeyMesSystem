using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using Telerik.WinControls.Data;

namespace TestAPI
{
    public partial class GridViewGroup : RadForm
    {
        public GridViewGroup()
        {
            InitializeComponent();
        }

        private void GridViewGroup_Load(object sender, EventArgs e)
        {
            this.radGridView1.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
            this.radGridView1.ShowGroupPanel = false;
            this.radGridView1.MasterTemplate.EnableGrouping = true;
            this.radGridView1.MasterTemplate.AllowDragToGroup = false;
            this.radGridView1.MasterTemplate.AutoExpandGroups = false;

            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("name");
            SetConditions();

            for (int i = 0; i < 20; i++)
            {
                DataRow dr = dt.NewRow();
                dr["ID"] = i + 1;
                dr["name"] = "NAME_" + i + 1;
                dt.Rows.Add(dr);
            }
            this.radGridView1.DataSource = dt;
        }

        private void SetConditions()
        {
            GroupDescriptor descriptor = new GroupDescriptor();
            descriptor.GroupNames.Add("ID", ListSortDirection.Ascending);
            descriptor.Aggregates.Add($"Count(ID)");
            descriptor.Format = "{0}: 第【{1}】次测试，包含【{2}】条数据 ";
            this.radGridView1.GroupDescriptors.Add(descriptor);
        }
    }
}
