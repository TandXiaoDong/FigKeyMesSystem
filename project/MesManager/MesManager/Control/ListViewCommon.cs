using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.WinControls.UI;
using System.Windows.Forms;
using System.Drawing;

namespace MesManager.Control
{
    class ListViewCommon
    {
        /// <summary>
        /// System.Windows.Forms.ListView
        /// </summary>
        /// <param name="listView"></param>
        public static void InitListView(ListView listView)
        {
            listView.GridLines = false; //显示表格线
            listView.View = View.Details;
            listView.FullRowSelect = false;//是否可以选择行
            listView.LabelEdit = false;
            listView.CheckBoxes = true;

            //设置行高
            ImageList image = new ImageList();
            image.ImageSize = new Size(1, 25);
            listView.SmallImageList = image;
        }
    }
}
