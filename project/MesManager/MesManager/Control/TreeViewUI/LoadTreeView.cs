using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MesManager.Control.TreeViewUI
{
    class LoadTreeView
    {
        #region  treeview 绑定文件夹和文件

        /// <summary>
        /// 根据文件夹绑定到树
        /// </summary>
        /// <param name="treeview"></param>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static bool SetTreeNoByFilePath(TreeView treeview, string FilePath, ImageList imgs)
        {
            treeview.Nodes.Clear();
            treeview.ImageList = imgs;
            try
            {
                foreach (DirectoryInfo direc in new DirectoryInfo(FilePath).GetDirectories())
                {
                    TreeNode tn = new TreeNode(direc.Name);
                    tn.Text = direc.Name;
                    SetTreeNodeIco(tn, "dir", imgs);
                    tn.Tag = direc.FullName;
                    SetSubDirectoryTreenode(direc, tn, imgs);
                    treeview.Nodes.Add(tn);
                }
                foreach (FileInfo finfo in new DirectoryInfo(FilePath).GetFiles())
                {
                    TreeNode temptreenode = new TreeNode(finfo.Name);
                    temptreenode.Tag = finfo.FullName;
                    temptreenode.Text = finfo.Name;
                    SetTreeNodeIco(temptreenode, finfo.Extension, imgs);
                    treeview.Nodes.Add(temptreenode);
                }
                return true;
            }
            catch
            {
                return false;
            }

        }
        /// <summary>
        /// 设置子目录的
        /// </summary>
        /// <param name="direc">目录路径</param>
        /// <param name="tn"></param>
        /// <param name="imglist"></param>
        private static void SetSubDirectoryTreenode(DirectoryInfo direc, TreeNode tn, ImageList imglist)
        {
            foreach (DirectoryInfo dir in new DirectoryInfo(direc.FullName).GetDirectories())
            {
                TreeNode temptn = new TreeNode(dir.Name);
                temptn.Tag = dir.FullName;
                SetTreeNodeIco(temptn, "dir", imglist);
                temptn.Text = dir.Name;
                tn.Nodes.Add(temptn);
                foreach (FileInfo fileinfo in new DirectoryInfo(dir.FullName).GetFiles())
                {
                    TreeNode temptreenode = new TreeNode(fileinfo.Name);
                    temptreenode.Tag = fileinfo.FullName;
                    temptreenode.Text = fileinfo.Name;
                    SetTreeNodeIco(temptreenode, fileinfo.Extension, imglist);
                    temptn.Nodes.Add(temptreenode);
                }
                SetSubDirectoryTreenode(dir, temptn, imglist);

            }
        }

        /// <summary>
        /// 为treeview设置小图标
        /// </summary>
        /// <param name="tn"></param>
        /// <param name="strExt"></param>
        /// <param name="imgs"></param>
        private static void SetTreeNodeIco(TreeNode tn, string strExt, ImageList imgs)
        {
            string ext = strExt.Replace(".", "");
            if (ext.ToLower() == "dir")
            {
                tn.ImageIndex = imgs.Images.IndexOfKey("close");
                tn.SelectedImageIndex = imgs.Images.IndexOfKey("open");
            }
            else if (ext.ToLower() == "doc" || ext.ToLower() == "rar" || ext.ToLower() == "txt")
            {
                tn.ImageIndex = imgs.Images.IndexOfKey(ext);
                tn.SelectedImageIndex = imgs.Images.IndexOfKey(ext);
            }
            else
            {
                tn.ImageIndex = imgs.Images.IndexOfKey("other");
                tn.SelectedImageIndex = imgs.Images.IndexOfKey("other");
            }
        }

        #endregion


        #region 只绑定文件夹

        /// <summary>
        /// 根据文件夹绑定到树
        /// </summary>
        /// <param name="treeview"></param>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public bool SetTreeNoByFilePath(TreeView treeview, string FilePath)
        {
            treeview.Nodes.Clear();

            try
            {
                foreach (DirectoryInfo direc in new DirectoryInfo(FilePath).GetDirectories())
                {
                    TreeNode tn = new TreeNode(direc.Name);
                    tn.Text = direc.FullName;

                    tn.Tag = direc.FullName;
                    SetSubDirectoryTreenode(direc, tn);
                    treeview.Nodes.Add(tn);


                }
                return true;
            }
            catch
            {
                return false;


            }

        }
        /// <summary>
        /// 设置子目录的
        /// </summary>
        /// <param name="direc">目录路径</param>
        /// <param name="tn"></param>
        /// <param name="imglist"></param>
        private void SetSubDirectoryTreenode(DirectoryInfo direc, TreeNode tn)
        {
            foreach (DirectoryInfo dir in new DirectoryInfo(direc.FullName).GetDirectories())
            {
                TreeNode temptn = new TreeNode(dir.Name);
                temptn.Tag = dir.FullName;
                temptn.Text = dir.Name;
                tn.Nodes.Add(temptn);
                foreach (FileInfo fileinfo in new DirectoryInfo(dir.FullName).GetFiles())
                {
                    TreeNode temptreenode = new TreeNode(fileinfo.Name);
                    temptreenode.Tag = fileinfo.FullName;
                    temptreenode.Text = fileinfo.Name;

                    temptn.Nodes.Add(temptreenode);

                }
                SetSubDirectoryTreenode(dir, temptn);

            }
        }

        #endregion
    }
}
