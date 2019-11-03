using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace MesManager.Common
{
    class ScrollLeftAndRight
    {
        private int mPostionX, mPostionY;
        private int mHeight, mWidth;
        private double mTime = 0;
        private Label mLabel = new Label();

        // 初始化label显示
        private void InitScrollShow()
        {
            //mHeight = panelScreen.Height;
            //mWidth = panelScreen.Width;
            mLabel.Font = new Font("宋体", 20);
            mHeight -= mLabel.Font.Height;  //label显示需要减去本身的高度
            mPostionX = mWidth;
            mPostionY = mHeight;
            mLabel.Location = new Point(mPostionX, mPostionY);
            mLabel.BackColor = Color.OrangeRed;
            mLabel.Text = "测试滚动新闻资讯,以及防汛防洪";
            mLabel.AutoSize = true;
            //panelScreen.Controls.Add(mLabel);
            mLabel.Visible = true;
        }

        // 设置底栏从右向左滚动显示
        private void ScrollShow()
        {
            mPostionX = mPostionX - 3;
            mLabel.Location = new Point(mPostionX, mPostionY);
            if (mPostionX <= -mLabel.Size.Width)
            {
                mPostionX = mWidth;
            }
            mLabel.Visible = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            mTime += 0.1;
            ScrollShow();
        }
    }
}
