using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.WinControls.UI;

namespace MesManager.Control
{
    class TreeViewControl
    {
        private RadTreeView treeView;
        public TreeViewControl(RadTreeView radTreeView)
        {
            this.treeView = radTreeView;
        }
        public void LoadTreeView()
        {
            //数据库读取初始化视图
            RadTreeNode hardWardRoot = treeView.AddNodeByPath("产品");
            //hardWardRoot.Nodes.Add(TreeViewData.HardWare.COMMENT);
            RadTreeNode nodeProductA = hardWardRoot.Nodes.Add("产品A");
            nodeProductA.Nodes.Add("内壳");
            nodeProductA.Nodes.Add("外壳");
            nodeProductA.Nodes.Add("螺丝");
            nodeProductA.Nodes.Add("外箱");

            RadTreeNode nodeProductB = hardWardRoot.Nodes.Add("产品B");
            nodeProductB.Nodes.Add("内壳");
            nodeProductB.Nodes.Add("外壳");
            nodeProductB.Nodes.Add("螺丝");
            nodeProductB.Nodes.Add("外箱");
        }
    }
}
