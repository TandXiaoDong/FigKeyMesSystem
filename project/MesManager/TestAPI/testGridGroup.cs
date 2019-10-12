using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.Pivot.Core;
using Telerik.WinControls.UI;
using Telerik.Pivot.Core.Aggregates;
using Telerik.WinControls;
using Telerik.WinControls.UI.Data;
using Telerik.WinControls.UI.Design;
using Telerik.WinControls.Data;

namespace TestAPI
{
    public partial class testGridGroup : Form
    {
        private DataTable dt;
        public testGridGroup()
        {
            InitializeComponent();
        }

        private void TestGridGroup_Load(object sender, EventArgs e)
        {
            this.radGridView1.Dock = DockStyle.Fill;
            SetRadGridViewProperty(this.radGridView1,true);
            InitDataSource();
            SetConditions();
        }

        public static void SetRadGridViewProperty(RadGridView radGridView, bool allowAddNewRow)
        {
            radGridView.AllowDrop = true;
            radGridView.AllowRowReorder = true;
            //显示新行
            radGridView.AddNewRowPosition = SystemRowPosition.Bottom;
            radGridView.ShowRowHeaderColumn = true;
            radGridView.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
            //radGridView.AutoSizeRows = true;
            //radGridView.BestFitColumns();
            radGridView.ReadOnly = false;
            //gridView.ColumnChooserSortOrder = RadSortOrder.Ascending;
            //dgv.AllowRowHeaderContextMenu = false;
            radGridView.ShowGroupPanel = true;
            radGridView.MasterTemplate.EnableGrouping = true;
            radGridView.MasterTemplate.AllowDragToGroup = false;
            radGridView.MasterTemplate.AutoExpandGroups = true;
            radGridView.MasterTemplate.AllowAddNewRow = allowAddNewRow;
            radGridView.EnableHotTracking = true;
            radGridView.MasterTemplate.SelectLastAddedRow = false;
            radGridView.ShowGroupPanelScrollbars = true;

        }

        private void SetConditions()
        {
            GroupDescriptor descriptor = new GroupDescriptor();
            descriptor.GroupNames.Add("data1", ListSortDirection.Ascending);
            descriptor.Aggregates.Add("Count(data1)");
            descriptor.Format = "{0}: {1} has {2} records in its group.";
            this.radGridView1.GroupDescriptors.Add(descriptor);
        }

        private void InitDataSource()
        {
            dt = new DataTable();
            dt.Columns.Add("data1");
            dt.Columns.Add("data2");
            dt.Columns.Add("data3");
            dt.Columns.Add("data4");
            //
            for (int i = 0; i < 30; i++)
            {
                DataRow dr = dt.NewRow();
                dr["data1"] = i + "data1_" + i;
                dr["data2"] = i + "data2_" + i;
                dr["data3"] = i + "data3_" + i;
                dr["data4"] = i + "data4_" + i;
                dt.Rows.Add(dr);
            }
            this.radGridView1.DataSource = dt;
        }

    }
}
