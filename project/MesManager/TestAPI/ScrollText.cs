using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.WinControls.UI;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;

namespace TestAPI
{
    class ScrollText
    {
        private int roll;
        private int parentWidth;
        private int lbxXPoint;
        private int lbxYPoint;
        private int lbxWidth;//要滚动的宽度
        private System.Timers.Timer timer;
        private RadLabel radLabel;
        private RoolDirection roolDirectionEnum;

        public enum RoolDirection
        {
            Left,
            Right
        }

        public ScrollText(RadLabel radLabel, Panel panel,RoolDirection roolDirection)
        {
            this.radLabel = radLabel;
            this.roolDirectionEnum = roolDirection;
            parentWidth = panel.Width;
            this.lbxWidth = radLabel.Size.Width;
            lbxXPoint = radLabel.Location.X; 
            lbxYPoint = radLabel.Location.Y;
            timer = new System.Timers.Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            timer.Interval = 30;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (roolDirectionEnum == RoolDirection.Right)
            {
                radLabel.Location = new Point(lbxXPoint + roll, lbxYPoint);
                if (roll == lbxWidth)
                {
                    radLabel.Location = new Point(0, lbxYPoint);
                    roll = 0;
                }
            }
            else if (roolDirectionEnum == RoolDirection.Left)
            {
                radLabel.Location = new Point(lbxXPoint - roll,lbxYPoint);
                if (roll == lbxWidth)
                {
                    radLabel.Location = new Point(parentWidth,lbxYPoint);
                    roll = 0;
                }
            }
            roll++;
        }
    }
}
