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
using CommonUtils.Logger;
using WeifenLuo.WinFormsUI.Docking;

namespace TestAPI
{
    public partial class Form3 : DockContent
    {
        private MesServiceT.MesServiceClient serviceClient;
        private DockPanel dockPanel;
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            //dockPanel = new DockPanel();
            //dockPanel.Dock = DockStyle.Fill;
            //this.Controls.Add(dockPanel);
            //Form2 form2 = new Form2();
            //form2.Show(this.dockPanel,DockState.DockBottom);
            //测试数据
            MesServiceTest.MesServiceClient mesServiceTest = new MesServiceTest.MesServiceClient();
            var sn = "017 B198230033020";
            var station = "外壳装配工站";
            var code = "A19083000029&S2.118&1.2.11.116&50&20190830&1T20190830001";
            var result = mesServiceTest.SelectLastTestResult(sn,station);
            var up = mesServiceTest.UpdateMaterialStatistics("A01",station,code,"2","8","0");
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
