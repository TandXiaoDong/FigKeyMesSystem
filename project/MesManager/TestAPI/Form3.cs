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
        private MesServiceTest.MesServiceClient mesServiceTest;

        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            //ScrollText scrollText = new ScrollText(this.radLabel1,this.panel1,ScrollText.RoolDirection.Left);
            //ScrollText scrollText2 = new ScrollText(this.radLabel2, this.panel1, ScrollText.RoolDirection.Left);
            mesServiceTest = new MesServiceTest.MesServiceClient();
          
        }

        private void InsertTestResult()
        {
            var count = int.Parse(this.tb_testResult_num.Text);
            var sign = this.tb_sign.Text;
            for (int i = 0; i < count; i++)
            {
                var sn = "017 B19A18030104_"+sign+i.ToString().PadLeft(4,'0');
                var snOutter = sn + "outter";
                var typeNo = "HTS-B2004-02-05";
                var joinDateTime1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss_s1");
                var joinDateTime2 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss_s2");
                var joinDateTime3 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss_s3");
                var joinDateTime4 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss_s4");
                var joinDateTime5 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss_s5");

                mesServiceTest.SelectLastTestResult(sn,"烧录工站");
                mesServiceTest.UpdateTestResultData(sn, typeNo,  "烧录工站","PASS","testUser","testUser",joinDateTime1);

                mesServiceTest.SelectLastTestResult(sn, "灵敏度测试工站");
                mesServiceTest.UpdateTestResultData(sn, typeNo, "灵敏度测试工站", "PASS", "testUser", "testUser", joinDateTime2);

                mesServiceTest.SelectLastTestResult(sn, "外壳装配工站");
                mesServiceTest.UpdateTestResultData(sn, typeNo, "外壳装配工站", "PASS", "testUser", "testUser", joinDateTime3);

                mesServiceTest.SelectLastTestResult(snOutter, "支架装配工站");
                mesServiceTest.UpdateTestResultData(snOutter, typeNo, "支架装配工站", "PASS", "testUser", "testUser", joinDateTime4);

                mesServiceTest.SelectLastTestResult(snOutter, "成品测试工站");
                mesServiceTest.UpdateTestResultData(snOutter, typeNo, "成品测试工站", "PASS", "testUser", "testUser", joinDateTime5);

                BindPcbaSN(sn,snOutter,typeNo, "A19090200008&S2.118&1.2.11.148&20&20190902&1T20190902001_" +sign);

                InsertTestItemLog(typeNo, "烧录工站",sn,snOutter,joinDateTime1);
                InsertTestItemLog(typeNo, "灵敏度测试工站", sn, snOutter, joinDateTime2);
                InsertTestItemLog(typeNo, "外壳装配工站", sn, snOutter, joinDateTime3);
                InsertTestItemLog(typeNo, "支架装配工站", sn, snOutter, joinDateTime4);
                InsertTestItemLog(typeNo, "成品测试工站", sn, snOutter, joinDateTime5);
            }
        }

        private void InsertTestItemLog(string typeNo,string station,string sn,string snOutter,string joinDate)
        {
            if (station == "烧录工站")
            {
                mesServiceTest.UpdateTestLog(typeNo, station, sn, "13.5V电压测试", "13.000-14.000", "13.338","Passed","testUser","testUser",joinDate);
                mesServiceTest.UpdateTestLog(typeNo, station, sn, "5V电压测试", "13.000-14.000", "13.338", "Passed", "testUser", "testUser", joinDate);
                mesServiceTest.UpdateTestLog(typeNo, station, sn, "软件版本", "13.000-14.000", "13.338", "Passed", "testUser", "testUser", joinDate);
                mesServiceTest.UpdateTestLog(typeNo, station, sn, "烧录", "13.000-14.000", "13.338", "Passed", "testUser", "testUser", joinDate);
            }
            else if (station == "灵敏度测试工站")
            {
                mesServiceTest.UpdateTestLog(typeNo, station, sn, "工作电流", "13.000-14.000", "13.338", "Passed", "testUser", "testUser", joinDate);
                mesServiceTest.UpdateTestLog(typeNo, station, sn, "零件号", "13.000-14.000", "13.338", "Passed", "testUser", "testUser", joinDate);
                mesServiceTest.UpdateTestLog(typeNo, station, sn, "软件版本", "13.000-14.000", "13.338", "Passed", "testUser", "testUser", joinDate);
                mesServiceTest.UpdateTestLog(typeNo, station, sn, "射频测试", "13.000-14.000", "13.338", "Passed", "testUser", "testUser", joinDate);
                mesServiceTest.UpdateTestLog(typeNo, station, sn, "休眠电流", "13.000-14.000", "13.338", "Passed", "testUser", "testUser", joinDate);
                mesServiceTest.UpdateTestLog(typeNo, station, sn, "硬件版本", "13.000-14.000", "13.338", "Passed", "testUser", "testUser", joinDate);
            }
            else if (station == "外壳装配工站")
            {
                mesServiceTest.UpdateTestLog(typeNo, station, sn, "后盖组装", "13.000-14.000", "13.338", "Passed", "testUser", "testUser", joinDate);
                mesServiceTest.UpdateTestLog(typeNo, station, sn, "前盖组装", "13.000-14.000", "13.338", "Passed", "testUser", "testUser", joinDate);
            }
            else if (station == "支架装配工站")
            {
                mesServiceTest.UpdateTestLog(typeNo, station, snOutter, "右支架", "13.000-14.000", "13.338", "Passed", "testUser", "testUser", joinDate);
                mesServiceTest.UpdateTestLog(typeNo, station, snOutter, "左支架", "13.000-14.000", "13.338", "Passed", "testUser", "testUser", joinDate);
            }
            else if (station == "成品测试工站")
            {
                mesServiceTest.UpdateTestLog(typeNo, station, snOutter, "工作电流", "13.000-14.000", "13.338", "Passed", "testUser", "testUser", joinDate);
                mesServiceTest.UpdateTestLog(typeNo, station, snOutter, "目检", "13.000-14.000", "13.338", "Passed", "testUser", "testUser", joinDate);
                mesServiceTest.UpdateTestLog(typeNo, station, snOutter, "休眠电流", "13.000-14.000", "13.338", "Passed", "testUser", "testUser", joinDate);
            }
        }

        private void BindPcbaSN(string sn,string snOutter,string productTypeNo,string materialCode)
        {
            mesServiceTest.BindingPCBA(sn,snOutter,materialCode,productTypeNo);
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

        private void btn_insert_Click(object sender, EventArgs e)
        {
            InsertTestResult();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //进站
            var sn = textBox1.Text;
            mesServiceTest.SelectLastTestResult(sn, "灵敏度测试工站");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //出站
            var sn = textBox1.Text;
            var result = mesServiceTest.UpdateTestResultData("017 B19C20033703", "HTS-B2004-03-02", "灵敏度测试工站", "PASS","user1","","2019-12-16-01");
            MessageBox.Show(result);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var dr = mesServiceTest.UpdateTestLog("HTS-B2004-03-02","烧录工站", "017 B19C20033703", "13.5V电压测试", "12848","23","Passed","ad","ds", "2019-12-16-01");
            MessageBox.Show(dr);
        }
    }
}
