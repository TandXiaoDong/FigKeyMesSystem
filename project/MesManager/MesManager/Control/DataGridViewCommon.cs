using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.WinControls.UI;

namespace MesManager.Control
{
    class DataGridViewCommon
    {

        public static void BindGrid(RadGridView radGridView1)
        {

            radGridView1.TableElement.BeginUpdate();

            //this.customersTableAdapter.Fill(this.northwindDataSet.Customers);

            radGridView1.MasterTemplate.AutoExpandGroups = true;
            radGridView1.MasterTemplate.EnableFiltering = true;
            radGridView1.ShowGroupPanel = true;
            radGridView1.EnableHotTracking = true;


            radGridView1.TableElement.EndUpdate(false);

            radGridView1.TableElement.CellSpacing = -1;
            radGridView1.TableElement.TableHeaderHeight = 35;
            radGridView1.TableElement.GroupHeaderHeight = 30;
            radGridView1.TableElement.RowHeight = 25;
        }


        public static void SetRadGridViewProperty(RadGridView radGridView,bool allowAddNewRow)
        {
            radGridView.EnableGrouping = false;
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
            radGridView.ShowGroupPanel = false;
            radGridView.MasterTemplate.EnableGrouping = false;
            radGridView.MasterTemplate.AllowAddNewRow = allowAddNewRow;
            radGridView.EnableHotTracking = true;
            radGridView.MasterTemplate.SelectLastAddedRow = false;
            //radRadioDataReader.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On;
            //this.radGridView1.CurrentRow = this.radGridView1.Rows[0];//设置某行为当前行

        }
    }
}
