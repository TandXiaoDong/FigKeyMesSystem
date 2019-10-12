using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mesdemo
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        public int result = 0;

        private void Button1_Click_1(object sender, EventArgs e)
        {
            if (txtPwd.Text == "20190526")
            {
                result = 1;
                this.Close();
            }
            else
            {
                MessageBox.Show("密码错误！");
                txtPwd.SelectAll();
                txtPwd.Focus();
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            result = 0;
            this.Close();
        }
    }
}
