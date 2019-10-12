using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using MesManager.Properties;

namespace MesManager
{
    public partial class WelcomeForm : Telerik.WinControls.UI.RadForm
    {
        public WelcomeForm()
        {
            InitializeComponent();
            this.radPanorama1.PanelImage = Resources.movieLab_bg;
            this.radPanorama1.PanelImageSize = new Size(this.radPanorama1.Size.Width,this.radPanorama1.Size.Height);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
        }
    }
}
