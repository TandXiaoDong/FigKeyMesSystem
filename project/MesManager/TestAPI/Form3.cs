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
            //dockPanel = new DockPanel();
            //dockPanel.Dock = DockStyle.Fill;
            //this.Controls.Add(dockPanel);
            //Form2 form2 = new Form2();
            //form2.Show(this.dockPanel,DockState.DockBottom);
            //测试数据
            MesServiceTest.MesServiceClient mesServiceTest = new MesServiceTest.MesServiceClient();
            MesService.MesServiceClient cst = new MesService.MesServiceClient();
            //var res = mesServiceTest.UpdatePackageProductBindingMsg("13","0012","A01","","0","","","");
            //var msg = mesServiceTest.CheckPcbaState("017 B19922001901", "");
            var res = mesServiceTest.BindingPCBA("", "A571E20311K091910027DE00112029", "jorgte", "HTSB20040000");
            MessageBox.Show(res);
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
