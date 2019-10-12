using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace MesManager.RadView
{
    public partial class MaterialOutBox : Telerik.WinControls.UI.RadForm
    {
        public MaterialOutBox()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
        }
    }
}
