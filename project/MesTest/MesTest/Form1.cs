using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MesTest
{
    public partial class Form1 : Form
    {
        private MesTestService.MesServiceClient serviceClient;
        public Form1()
        {
            InitializeComponent();
            serviceClient = new MesTestService.MesServiceClient();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var outCaseCode = "20000";
            var snOutter = "A0000191";
            var typeNo = "Test-package";
            var station = "成品测试工站";
            var bindState = "1";
            var remark = "";

            var result = serviceClient.UpdatePackageProductBindingMsg(outCaseCode,snOutter,typeNo,station,bindState,remark,"","");
            foreach (var str in result)
            {
                this.textBox1.Text += "result:" + str + "\r\n";
            }
        }
    }
}
