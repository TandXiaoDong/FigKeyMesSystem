using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using MySql.Data.MySqlClient;
using WeifenLuo.WinFormsUI.Docking;

namespace TestAPI
{
    public partial class Form3 : DockContent
    {
        private DockPanel dockPanel;
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            ScrollText scrollText = new ScrollText(this.radLabel1,this.panel1,ScrollText.RoolDirection.Left);
            ScrollText scrollText2 = new ScrollText(this.radLabel2, this.panel1, ScrollText.RoolDirection.Left);
        }

        public void LSOSQL()
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["sqlconstring"].ToString();
                var selectSQL = ConfigurationManager.ConnectionStrings["selectPrescNo"].ToString();
                Task task = new Task(() =>
                {
                    while (true)
                    {
                        int index = 0;
                        //LogHelper.Log.Info("开始执行...");
                        MySqlDataReader mySqlDataReader = MySqlHelper.ExecuteReader(connectionString, CommandType.Text, selectSQL);
                        while (mySqlDataReader.Read())
                        {
                            var prescNo = mySqlDataReader["PrescriptionNo"].ToString();
                            //LogHelper.Log.Info("prescNo:" + prescNo);
                            //执行删除
                            var deletePrescList = $"delete from prescriptionlist where PrescriptionNo='{prescNo}'";
                            var deletePrescDetail = $"delete from prescriptiondetail where PrescriptionNo = '{prescNo}'";
                            MySqlHelper.ExecuteNonQuery(connectionString, CommandType.Text, deletePrescList);
                            MySqlHelper.ExecuteNonQuery(connectionString, CommandType.Text, deletePrescDetail);
                            index++;
                        }
                        //int row = MySqlHelper.ExecuteNonQuery(connectionString, CommandType.Text, selectSQL);

                    }
                });
                task.Start();
            }
            catch (Exception ex)
            {
                //LogHelper.Log.Error("异常：" + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

    }
}
