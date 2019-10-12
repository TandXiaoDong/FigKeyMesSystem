using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dbAccess;
namespace dbAccessTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        { 
            dbAccess.Access mDb = new dbAccess.Access();
            //mDb.Get();

            //客户端请求，可以插入当前站的信息
           string s = mDb.insertData(1, "123445454545","PASS");

            //服务器端才能设首站
           s= mDb.setFirstStation("BCM2.1", "F03H00A009", "1111111111");
           //服务器端才能设站
           s = mDb.setStation("123445454545", 3);
           //服务器端才能设站
           s = mDb.setStation("1111111334", 3);

           mDb.Get(1);
        }
    }
}
