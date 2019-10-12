using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonUtils.FileHelper;
using System.IO;
using TestAPI.Properties;

namespace TestAPI
{
    public partial class TestTreeView : Form
    {
        public TestTreeView()
        {
            InitializeComponent();
        }

        private void TestTreeView_Load(object sender, EventArgs e)
        {
            var path = @"D:\work\project\FigKey\RetrospectiveSystem\project\IIS\FTP";
            //var res = FileOperate.GetFoldAll();
            PopulateTreeView(path,this.treeView1);
        }
        private static TreeView treeViews;
        public static void PopulateTreeView(string path, TreeView treeView)
        {
            TreeNode rootNode;
            DirectoryInfo info = new DirectoryInfo(path);
            if (info.Exists)
            {
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                GetDirectories(info, rootNode,treeView);
                InitImageList(treeView);
                treeView.Nodes.Add(rootNode);
                treeView.ExpandAll();
            }
        }

        private static void GetDirectories(DirectoryInfo subDirs, TreeNode nodeToAddTo,TreeView treeView)
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
                    GetDirectories(subDir, aNode,treeView);
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

        private static void InitImageList(TreeView treeView)
        {
            //ImageList imageList = new ImageList();
            //imageList.Images.Add("extension_xls", Resources.file_extension_xls);
            //imageList.Images.Add("export", Resources.Export_16x16);
            //treeView.ImageList = imageList;

            //string fileName = "tmp.";
            //File.Create(fileName).Close();
            //Image img = System.Drawing.Icon.ExtractAssociatedIcon(fileName).ToBitmap();
            //File.Delete(fileName);
        }
    }
}
