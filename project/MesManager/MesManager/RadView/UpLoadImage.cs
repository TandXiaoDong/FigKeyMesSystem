using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using CommonUtils.FileHelper;
using CommonUtils.Logger;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace MesManager.RadView
{
    public partial class UpLoadImage : Telerik.WinControls.UI.RadForm
    {
        public static byte[] ProductImage;
        public UpLoadImage()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void LoadImage()
        {
            try
            {
                string filter = "Image(*.jpg;*.png;*.bmp;*.tiff;*.jepg;*.gif;)|*.jpg;*.png;*.bmp;*.tiff;*.jepg;*.gif|" +
                    "jpg(*.jpg)|*.jpg|png(*.png)|*.png|bmp(*.bmp)|*.bmp";
                FileContent fileContent = FileSelect.GetSelectFileContent(filter,"选择图片");
                if (fileContent == null)
                {
                    return;
                }
                if (string.IsNullOrEmpty(fileContent.FileName))
                    return;
                radLabel1.Text = fileContent.FileName.Split('.')[1];
                pictureBox1.Image = Image.FromFile(fileContent.FileName);
                OpenFileImage(fileContent.FileName);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Btn_sectImage_Click(object sender, EventArgs e)
        {
            LoadImage();
        }

        private void Btn_apply_Click(object sender, EventArgs e)
        {
            Image image = pictureBox1.Image;
            MemoryStream ms = new MemoryStream();
            new BinaryFormatter().Serialize(ms, (object)image);
            ms.Close();
            //ProductImage = ms.ToArray();
            //LogHelper.Log.Info($"productImageByte Len={ProductImage.Length} data="+BitConverter.ToString(ProductImage));
            this.Close();
        }

        private void OpenFileImage(string path)
        {
            FileStream byteStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] byteImage = new byte[byteStream.Length];  //图像文件转换成二进制流
            byteStream.Read(byteImage, 0, (int)byteStream.Length);
            ProductImage = byteImage;
            //LogHelper.Log.Info($"FileStream Len = {byteImage.Length} data = "+BitConverter.ToString(byteImage));
        }
    }
}
