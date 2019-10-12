using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;
using CommonUtils.FileHelper;
using System.IO;

namespace MesManager.Control.TreeViewUI
{
    class TreeViewData
    {
        static DataTable dt;
        public static void LoadTreeView(TreeView treeView1)
        {
            treeView1.Nodes.Clear();

            TreeNode root1 = new TreeNode("General");
            root1.Name = "general_";
            treeView1.Nodes.Add(root1);
            DirOperate dirOperate = new DirOperate();
            
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string slop_name = dt.Rows[i]["slop_name"].ToString();
                string station = dt.Rows[i]["station_name"].ToString();

                TreeNode unitNode = new TreeNode();
                unitNode.Name = slop_name;
                unitNode.Text = slop_name;
                if (!root1.Nodes.ContainsKey(unitNode.Name))
                    root1.Nodes.Add(unitNode);
                NodeStation(slop_name, unitNode);
            }
            root1.Expand();
            root1.ExpandAll();
            dt.Clear();
        }
        public static void NodeStation(string slop_name, TreeNode node)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string stationName = "";
                string slopName = dt.Rows[i]["slop_name"].ToString();
                if (slopName.Equals(slop_name))
                {
                    stationName = dt.Rows[i]["station_name"].ToString();
                    TreeNode snode = new TreeNode(stationName);
                    node.Nodes.Add(snode);
                }
            }
        }

        public static void PopulateTreeView(string path, TreeView treeView)
        {
            TreeNode rootNode;
            DirectoryInfo info = new DirectoryInfo(path);
            if (info.Exists)
            {
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                GetDirectories(info, rootNode);
                treeView.Nodes.Add(rootNode);
                treeView.ExpandAll();
            }
        }

        private static void GetDirectories(DirectoryInfo subDirs, TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs = subDirs.GetDirectories();
            foreach (DirectoryInfo subDir in subSubDirs)
            {
                aNode = new TreeNode(subDir.Name, 0, 0);
                //aNode.Tag = subDir;
                aNode.ImageKey = "Folder";
                nodeToAddTo.Nodes.Add(aNode);

                if (subSubDirs.Length != 0)
                {
                    GetDirectories(subDir, aNode);
                }
            }
            FileInfo[] fileInfo = subDirs.GetFiles();   //目录下的文件
            int fIndex = 0;
            foreach (FileInfo fInfo in fileInfo)
            {
                var fName = fInfo.Name.ToString();
                var fExtension = fInfo.Extension;
                aNode = new TreeNode(fName);

                //aNode.ImageKey = imageName;
                //aNode.SelectedImageKey = imageName;
                //aNode.Tag = fInfo;
                //if (fExtension == ".txt")
                //{
                //    aNode.ImageKey = "export";
                //    aNode.SelectedImageKey = "export";
                //}
                //else if (fExtension == ".xls")
                //{
                //    aNode.ImageKey = "extension_xls";
                //    aNode.SelectedImageKey = "extension_xls";
                //}
                nodeToAddTo.Nodes.Add(aNode);
                fIndex++;
            }
        }
    }
}
