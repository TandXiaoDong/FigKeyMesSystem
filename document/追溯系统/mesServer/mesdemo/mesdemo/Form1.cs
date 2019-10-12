using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using dbAccess;

namespace mesdemo
{
    public partial class Form1 : Form
    {
        private static byte[] result = new byte[1024];
        public int mStationNumber = 5;

        private const int BufferSize = 1500;
        private const int port = 8088;
        //private static string IpStr = "174.34.222.6";
        private static string IpStr = "127.0.0.1";
        private static TcpListener server;
        private static string xmlPath = @"partProcessedResponse.xml";
        private int mStationCount = 0;
        private int mStationIndex = 0;
        public static List<TcpClient> clients = new List<TcpClient>();

        public static Access mDb = new Access();
        public delegate void action(string sMsg);
        static event action action_event;
        public List<string> sLst;
        private bool bIsAdmin = false;
        private RadioButton[] lstStation;

        public Form1()
        {
            InitializeComponent();
        }

        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.txtMsg.InvokeRequired)
            {
                action d = new action(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.txtMsg.Text += text + "\n";
            }
        }

        private static void DoAcceptTcpclient(IAsyncResult ar)
        {
            // Get the listener that handles the client request.
            TcpListener listener = (TcpListener)ar.AsyncState;

            // End the operation and display the received data on 
            // the //Console.
            TcpClient client = listener.EndAcceptTcpClient(ar);

            clients.Add(client);

            // Process the connection here. (Add the client to a
            // server table, read data, etc.)
            //Console.WriteLine("Client connected completed,id:{0}", client.Client.RemoteEndPoint.ToString());
            action_event(string.Format("Client connected completed,id:{0}", client.Client.RemoteEndPoint.ToString()));
            //开启线程用来不断接收来自客户端的数据
            Thread t = new Thread(new ParameterizedThreadStart(ReceiveMessageFromClient));
            t.Start(client);

            server.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpclient), server);
        }
        private static void ReceiveMessageFromClient(object reciveClient)
        {
            TcpClient client = reciveClient as TcpClient;
            if (client == null)
            {
                ////Console.WriteLine("client error");
                action_event("client error");
                return;
            }

            while (true)
            {
                try
                {
                    //接收字符串
                    byte[] buffer = new byte[BufferSize];

                    NetworkStream streamToServer = client.GetStream();
                    streamToServer.ReadTimeout = -1;
                    action_event("waiting for data...");
                    byte[] sData = new byte[100];
                    sData[0] = 100;
                    //streamToServer.Write(sData, 0, 5);
                    //int num = streamToServer.Read(result, 0, result.Length); //将数据读到result中     
                    int bytesRead = streamToServer.Read(buffer, 0, 4);
                    if (bytesRead > 0)
                    {
                        bytesRead = streamToServer.Read(buffer, 0, BufferSize);
                        if (bytesRead > 0)
                        {
                            string rMsg = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                            int sStation = -1;
                            string[] s = analysisResponse(rMsg, out sStation);
                            /// s[0] 
                            /// 

                            if (s[3] == "partProcessed")
                            {//partProcessed
                                string sTemp = mDb.insertData(sStation, s[0], s[1], s[4]);
                                if (s[2] != string.Empty)
                                    partProcessedResponse(streamToServer, sStation.ToString(), sTemp, mDb.ST_CONTENT[2], s[0]);
                                else
                                    partProcessedResponse(streamToServer, sStation.ToString(), sTemp, "N/A", s[0]);
                            }
                            else if (s[3] == "partReceived")
                            {//partReceived
                                string sTemp = mDb.checkPart(sStation, s[0]);
                                if ((sTemp == "this identifier does not exist!") && sStation == 1)
                                {
                                    mDb.setFirstStation("", s[5], s[0]);
                                    partProcessedResponse(streamToServer, sStation.ToString(), "OK", mDb.ST_CONTENT[2], s[0]);
                                }

                                partProcessedResponse(streamToServer, sStation.ToString(), sTemp, mDb.ST_CONTENT[2], s[0]);
                            }
                        }
                    }
                    else
                    {   //这里需要注意 当num=0时表明客户端已经断开连接，需要结束循环，不然会死循环一直卡住 
                        action_event("客户端关闭");
                        break;
                    }
                }
                catch (Exception e)
                {
                    clients.Remove(client);
                    //action_event("error:" + e.ToString());
                    break;
                }
            }
        }

        public static string[] analysisResponse(string rMsg, out int result)
        {
            string[] sData = new string[6];
            for (int i = 0; i < 6; i++)
            {
                sData[i] = string.Empty;
            }

            result = -1;

            string sStationName = string.Empty;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rMsg);
            XmlNodeList ndlist = xmlDoc.SelectNodes("root/header");
            if (ndlist.Count > 0)
            {
                if (ndlist.Item(0).Attributes["eventName"].Value == "partProcessed")
                {//partProcessed
                    ndlist = xmlDoc.SelectNodes("root/event/partProcessed");
                    if (ndlist.Count > 0)
                    {
                        //Console.WriteLine(ndlist.Item(0).Attributes["identifier"].Value);
                        sData[0] = ndlist.Item(0).Attributes["identifier"].Value;  //s[0]产品dmc
                    }
                    ndlist = xmlDoc.SelectNodes("root/header/location");
                    {
                        sStationName = ndlist.Item(0).Attributes["processName"].Value;  //s[0]产品dmc
                    }
                    ndlist = xmlDoc.SelectNodes("root/body/structs/resHead");
                    if (ndlist.Count > 0)
                    {
                        ////Console.WriteLine(ndlist.Item(0).Attributes["result"].Value);
                        sData[3] = ndlist.Item(0).Attributes["process"].Value;   //process
                        sData[1] = ndlist.Item(0).Attributes["result"].Value;   //s[1]产品测试结果
                        sData[4] = ndlist.Item(0).Attributes["workingCode"].Value;   //log name
                        sData[5] = ndlist.Item(0).Attributes["typeNo"].Value;   //log name

                        //if (sData[3] == "partReceived")
                        //{
                        mDb.FindAll(sData[5], "平台号", 2);

                        for (int i = 0; i < mDb.ST_CONTENT.Length; i++)
                        {
                            if (mDb.ST_CONTENT[i] == sStationName)
                            {
                                result = i;
                                break;
                            }

                        }
                        //}
                    }
                }
                ndlist = xmlDoc.SelectNodes("root/header/location");
                if (ndlist.Count > 0)
                {
                    sData[2] = ndlist.Item(0).Attributes["statNo"].Value;       //s[2]当前站位
                    //result = 0;
                }
            }

            return sData;
        }
        //private static void ReceiveMessageFromClient(object reciveClient)
        //{
        //    TcpClient client = reciveClient as TcpClient;
        //    if (client == null)
        //    {
        //        ////Console.WriteLine("client error");
        //        action_event("client error");
        //        return;
        //    }

        //    while (true)
        //    {
        //        try
        //        {
        //            //接收字符串
        //            byte[] buffer = new byte[BufferSize];

        //            NetworkStream streamToServer = client.GetStream();
        //            streamToServer.ReadTimeout = -1;
        //            action_event("waiting for data...");
        //            byte[] sData = new byte[100];
        //            sData[0] = 100;
        //            //streamToServer.Write(sData, 0, 5);
        //            //int num = streamToServer.Read(result, 0, result.Length); //将数据读到result中     
        //            int bytesRead = streamToServer.Read(buffer, 0, 4);
        //            if (bytesRead > 0)
        //            {
        //                bytesRead = streamToServer.Read(buffer, 0, BufferSize);
        //                if (bytesRead > 0)
        //                {
        //                    string rMsg = Encoding.ASCII.GetString(buffer, 0, bytesRead);

        //                    string sResult = string.Empty;
        //                    string[] s = analysisResponse(rMsg, out sResult);
        //                    /// s[0] 
        //                    /// 

        //                    if (s[3] == "partProcessed")
        //                    {//partProcessed
        //                        string sTemp = mDb.insertData(int.Parse(s[2]), s[0], s[1],s[4]);
        //                        if (s[2] != string.Empty)
        //                            partProcessedResponse(streamToServer, s[2], sTemp, mDb.ST_CONTENT[2], s[0]);
        //                        else
        //                            partProcessedResponse(streamToServer, s[2], sTemp, "N/A", s[0]);
        //                    }
        //                    else if (s[3] == "partReceived")
        //                    {//partReceived
        //                        string sTemp = mDb.checkPart(int.Parse(s[2]), s[0]);

        //                        partProcessedResponse(streamToServer, s[2], sTemp, mDb.ST_CONTENT[2], s[0]);
        //                    }
        //                }
        //            }
        //            else
        //            {   //这里需要注意 当num=0时表明客户端已经断开连接，需要结束循环，不然会死循环一直卡住 
        //                action_event("客户端关闭");
        //                break;
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            clients.Remove(client);
        //            //action_event("error:" + e.ToString());
        //            break;
        //        }
        //    }
        //}

        //public static string[] analysisResponse(string rMsg, out string result)
        //{
        //    string[] sData = new string[5];
        //    for (int i = 0; i < 5; i++)
        //    {
        //        sData[i] = string.Empty;
        //    }

        //    result = "NoK";

        //    XmlDocument xmlDoc = new XmlDocument();
        //    xmlDoc.LoadXml(rMsg);
        //    XmlNodeList ndlist = xmlDoc.SelectNodes("root/header");
        //    if (ndlist.Count > 0)
        //    {
        //        if (ndlist.Item(0).Attributes["eventName"].Value == "partProcessed")
        //        {//partProcessed
        //            ndlist = xmlDoc.SelectNodes("root/event/partProcessed");
        //            if (ndlist.Count > 0)
        //            {
        //                //Console.WriteLine(ndlist.Item(0).Attributes["identifier"].Value);
        //                sData[0] = ndlist.Item(0).Attributes["identifier"].Value;  //s[0]产品dmc
        //            }
        //            ndlist = xmlDoc.SelectNodes("root/body/structs/resHead");
        //            if (ndlist.Count > 0)
        //            {
        //                ////Console.WriteLine(ndlist.Item(0).Attributes["result"].Value);
        //                sData[3] = ndlist.Item(0).Attributes["process"].Value;   //process
        //                sData[1] = ndlist.Item(0).Attributes["result"].Value;   //s[1]产品测试结果
        //                sData[4] = ndlist.Item(0).Attributes["workingCode"].Value;   //log name
        //            } 
        //        }
        //        ndlist = xmlDoc.SelectNodes("root/header/location");
        //        if (ndlist.Count > 0)
        //        {
        //            sData[2] = ndlist.Item(0).Attributes["statNo"].Value;       //s[2]当前站位
        //            result = "OK";
        //        }
        //    }

        //    return sData;
        //}

        public static string partProcessedResponse(NetworkStream streamToServer, string sStation, string result, string sTypeNo, string sIdentifier)
        {
            string ss = string.Empty;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
            XmlNodeList ndlist = xmlDoc.SelectNodes("root/header");
            if (ndlist.Count > 0)
            {
                ndlist[0].Attributes["timeStamp"].Value = System.DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
                //ndlist[0].Attributes["eventId"].Value = (evenID++).ToString();
            }

            ndlist = xmlDoc.SelectNodes("root/header/location");
            if (ndlist.Count > 0)
            {
                ndlist[0].Attributes["statNo"].Value = sStation;
            }
            ndlist = xmlDoc.SelectNodes("root/event/result ");
            if (ndlist.Count > 0)
            {
                ndlist[0].Attributes["revdescription"].Value = result;
                ndlist[0].Attributes["typeNo"].Value = sTypeNo;
                ndlist[0].Attributes["identifier"].Value = sIdentifier;
            }

            string str1 = xmlDoc.InnerXml;

            int mLen = str1.Length + 4;
            byte[] buffer1 = new byte[1000];
            buffer1[0] = (byte)(mLen / 16777216);
            buffer1[1] = (byte)(mLen / 65536);
            buffer1[2] = (byte)(mLen / 256);
            buffer1[3] = (byte)(mLen);
            //发送字符串
            byte[] buffer = Encoding.ASCII.GetBytes(str1); //msg为发送的字符串   

            buffer.CopyTo(buffer1, 4);
            try
            {
                streamToServer.ReadTimeout = 5000;
                lock (streamToServer)
                {
                    streamToServer.Write(buffer1, 0, mLen);     // 发往服务器
                }
            }
            catch (Exception e)
            {
                throw (new Exception(e.Message));
                //return e.Message;
            }

            return "OK";
        }

        private void button2Click()
        {
            this.Close();
        }

        //private void setView()
        //{
        //    if (cBoxFirstStation.Checked)
        //    {
        //        txtFamily.Visible = true;
        //        txtTypeNo.Visible = true;
        //        label2.Visible = true;
        //        label3.Visible = true;

        //        cBoxStation.Visible = false;
        //        label4.Visible = false;
        //    }
        //    else
        //    {
        //        txtFamily.Visible = false;
        //        txtTypeNo.Visible = false;
        //        label2.Visible = false;
        //        label3.Visible = false;

        //        cBoxStation.Visible = true;
        //        label4.Visible = true;
        //    }
        //}

        //private void cBoxFirstStation_CheckedChanged(object sender, EventArgs e)
        //{
        //    setView();
        //}

        private void btnSetClick()
        {

            adminSet();
            //string sResult = string.Empty;
            ////if (cBoxFirstStation.Checked)
            ////    sResult=mDb.setFirstStation(txtFamily.Text, txtTypeNo.Text, txtIdentifier.Text);
            ////else
            //    sResult=mDb.setStation(txtIdentifier.Text, cBoxStation.SelectedIndex + 1);

            //MessageBox.Show(sResult);
        }

        private void btnSearchClick()
        {
            if (txtSeachId.Text != string.Empty)
            {
                listView1.Items.Clear();
                string[] sLst1 = new string[100];
                for (int n = 0; n < 100; n++)
                {
                    sLst1[n] = string.Empty;
                }
                //找该零件号的第一个数据
                sLst1 = mDb.FindLst(txtSeachId.Text.Trim());
                Thread.Sleep(200);

                if (sLst1[0] != string.Empty)
                {
                    //找该零件号的对应平台号的站位列表
                    string[] sLst2 = mDb.FindPlatLst(sLst1[2], "平台号", 2);

                    ListViewItem lt = new ListViewItem();
                    lt.Text = sLst1[1];
                    for (int i = 2; i < sLst1.Length; i++)
                    {
                        if (i != 4)
                            lt.SubItems.Add(sLst1[i]);
                        else
                        {
                            int mIndex = int.Parse(sLst1[i]);
                            if (mIndex != 0)
                                lt.SubItems.Add(sLst2[mIndex]);
                            else
                                lt.SubItems.Add("pre-Station");
                        }
                    }
                    //将lt数据添加到listView1控件中
                    listView1.Items.Add(lt);
                }
                else
                    MessageBox.Show("查询不到产品信息！");
            }

        }

        private void skinButton1Click()
        {
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close();
            }
            this.Close();
        }

        private void skinButton2Click()
        {
            if (txtSeachId.Text != string.Empty)
            {
                listView1.Items.Clear();
                string[] sLst1 = new string[100];
                for (int n = 0; n < 100; n++)
                {
                    sLst1[n] = string.Empty;
                }
                //找该零件号的第一个数据
                sLst1 = mDb.FindLst(txtSeachId.Text.Trim());
                Thread.Sleep(200);

                if (sLst1[0] != string.Empty)
                {
                    string[] sLst2 = new string[100];
                    for (int n = 0; n < 100; n++)
                    {
                        sLst2[n] = string.Empty;
                    }
                    //找该零件号的对应平台号的站位列表
                    sLst2 = mDb.FindPlatLst(sLst1[2], "平台号", 2);

                    Thread.Sleep(200);
                    //找该零件号所有的数据
                    mDb.FindAll(txtSeachId.Text.Trim());

                    for (int m = 0; m < 300; m++)
                    {
                        if (mDb.ST_CONTENT[m * 9] != string.Empty)
                        {
                            ListViewItem lt = new ListViewItem();
                            lt.Text = mDb.ST_CONTENT[1];
                            for (int i = 2; i < 9; i++)
                            {
                                if (i != 4)
                                    lt.SubItems.Add(mDb.ST_CONTENT[i + m * 9]);
                                else
                                {
                                    int mIndex = int.Parse(mDb.ST_CONTENT[i + m * 9]);
                                    if (mIndex != 0)
                                        lt.SubItems.Add(sLst2[mIndex]);
                                    else
                                        lt.SubItems.Add("pre-Station");
                                }
                            }
                            //将lt数据添加到listView1控件中
                            listView1.Items.Add(lt);
                        }
                    }
                }
                else
                    MessageBox.Show("查询不到产品信息！");
            }
        }

        private void skinButton3Click()
        {
            if ((txtBarc.Visible) && (txtBarc.Text == string.Empty))
            {
                MessageBox.Show("标签内容不能为空！");
            }
            else
            {
                string s = operatorSet();

                MessageBox.Show(s);

                if ((s == "OK") && (cBoxResult1.Text == "PASS"))
                {
                    mStationIndex++;

                    //清除text的内容
                    txtDmc1.Text = string.Empty;
                    txtBarc.Text = string.Empty;
                    txtDmc1.Focus();

                    //if (cBoxStat1.SelectedIndex < cBoxStat1.Items.Count - 1)
                    //    cBoxStat1.SelectedIndex = cBoxStat1.SelectedIndex + 1;
                    if (mStationIndex >= mStationCount)
                    {
                        mStationIndex = 0;

                        MessageBox.Show("已经完成全部站位");
                    }

                    ((RadioButton)(groupBox1.Controls[mStationIndex])).Checked = true;
                }
            }
        }

        private string operatorSet()
        {
            string sResult = string.Empty;

            if (mStationIndex == 0)
                sResult = mDb.setFirstStation(txtFamily1.Text, txtTypeNo1.Text, txtDmc1.Text);
            sResult = mDb.insertData(mStationIndex + 1, txtDmc1.Text, cBoxResult1.Text, txtBarc.Text);

            return sResult;
        }

        private void skinButton6Click()
        {
            if (txtSearchPlatNo.Text != string.Empty)
            {
                lstViewPlatInfo.Items.Clear();
                mDb.FindAll(txtSearchPlatNo.Text.Trim(), "平台号", 2);

                if (mDb.ST_CONTENT[0] != string.Empty)
                {
                    ListViewItem lt = new ListViewItem();
                    lt.Text = mDb.ST_CONTENT[0];
                    for (int m = 1; m < mDb.ST_CONTENT.Length; m++)
                    {
                        if (mDb.ST_CONTENT[m] != string.Empty)
                        {
                            lt.SubItems.Add(mDb.ST_CONTENT[m]);
                        }
                        else
                            break;
                    }
                    //将lt数据添加到listView1控件中       
                    lstViewPlatInfo.Items.Add(lt);
                }
                else
                    MessageBox.Show("查询不到产品信息！");
            }
            else
                MessageBox.Show("平台号为空！");
        }

        private void rBtn1_CheckedChanged(object sender, EventArgs e)
        {
            if (rBtn1.Checked)
            {
                txtTypeNo.Items.Clear();
                List<string> sLst = mDb.GetAllPartLst(2);
                for (int i = 0; i < sLst.Count; i++)
                {
                    txtTypeNo.Items.Insert(i, sLst[i]);
                }

                txtTypeNo.SelectedIndex = 0;
                setViewSetting(0);
            }
            else
            {
                txtTypeNo.Items.Clear();
                txtTypeNo.Text = String.Empty;

                setViewSetting(1);
            }
        }

        private void txtTypeNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            mDb.FindAll(txtTypeNo.Text, "平台号", 2);

            cBoxStation.Items.Clear();
            //List<string> sLst = mDb.GetAllPartLst(2);
            for (int i = 1; i < mDb.ST_CONTENT.Length; i++)
            {
                if (mDb.ST_CONTENT[i] != string.Empty)
                {
                    cBoxStation.Items.Insert(i - 1, mDb.ST_CONTENT[i]);
                }
                else
                    break;
            }

            cBoxStation.SelectedIndex = 0;
        }

        /// <summary>
        /// 设置管理员界面的 控件状态
        /// </summary>
        /// <param name="m"></param>
        private void setViewSetting(int m)
        {
            if (m == 1)
            {
                label4.Visible = false;
                cBoxStation.Visible = false;

                label1.Visible = false;
                txtIdentifier.Visible = false;

                label3.Visible = false;
                txtFamily.Visible = false;

                label12.Visible = true;
                lstViewFlow.Visible = true;
            }
            else if (m == 0)
            {
                label4.Visible = true;
                cBoxStation.Visible = true;

                label1.Visible = true;
                txtIdentifier.Visible = true;

                label3.Visible = true;
                txtFamily.Visible = true;

                label12.Visible = false;
                lstViewFlow.Visible = false;
            }
        }

        /// <summary>
        /// 更新 操作员 界面中的 零件号列表
        /// </summary>
        private void updateOperatorPartlst()
        {
            txtTypeNo1.Items.Clear();
            List<string> sLst = mDb.GetAllPartLst(2);
            for (int i = 0; i < sLst.Count; i++)
            {
                txtTypeNo1.Items.Insert(i, sLst[i]);
            }
            txtTypeNo1.SelectedIndex = 0;
        }

        /// <summary>
        /// 管理员界面的 set按钮
        /// </summary>
        private void adminSet()
        {
            if (rBtn2.Checked)
            {

                if (txtTypeNo.Text == string.Empty)
                    MessageBox.Show("零件号不能为空！");
                else
                {
                    ListView.SelectedListViewItemCollection mList = lstViewFlow.SelectedItems;

                    string[] sLst = new string[10];
                    for (int i = 0; i < 10; i++)
                    {
                        sLst[i] = string.Empty;
                    }
                    if (mList.Count >= 0)
                    {
                        for (int i = 0; i < mList.Count; i++)
                        {
                            sLst[i] = mList[i].Text;
                        }
                        mDb.insertPlatData(txtTypeNo.Text, sLst);

                        //更新操作员界面的 零件号列表
                        updateOperatorPartlst();
                    }
                    else
                        MessageBox.Show("请选择流程！");
                }
            }
            else if (rBtn1.Checked)
            {
                if (txtIdentifier.Text == string.Empty)
                    MessageBox.Show("追溯码不能为空！");
                else
                    MessageBox.Show(mDb.setStation(txtIdentifier.Text, cBoxStation.SelectedIndex + 1));
            }
        }

        private void txtIdentifier_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                adminSet();
            }
        }

        private void txtTypeNo1_SelectedIndexChanged(object sender, EventArgs e)
        {
            mDb.FindAll(txtTypeNo1.Text, "平台号", 2);

            //cBoxStat1.Items.Clear();
            for (int i = 0; i < 7; i++)
            {
                groupBox1.Controls[i].Visible = false;
            }

            mStationCount = 0;
            mStationIndex = 0;
            //List<string> sLst = mDb.GetAllPartLst(2);
            for (int i = 1; i < mDb.ST_CONTENT.Length; i++)
            {
                if (mDb.ST_CONTENT[i] != string.Empty)
                {
                    //cBoxStat1.Items.Insert(i - 1, mDb.ST_CONTENT[i]);

                    ListViewItem lt = new ListViewItem();
                    lt.Text = mDb.ST_CONTENT[i];


                    groupBox1.Controls[i - 1].Text = mDb.ST_CONTENT[i];
                    groupBox1.Controls[i - 1].Visible = true;

                    mStationCount++;
                }
                else
                    break;
            }
            ((RadioButton)(groupBox1.Controls[mStationIndex])).Checked = true;
        }

        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) //当按下回车键
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if ((RadioButton)sender == radioButton1)
                mStationIndex = 0;
            else if ((RadioButton)sender == radioButton2)
                mStationIndex = 1;
            else if ((RadioButton)sender == radioButton3)
                mStationIndex = 2;
            else if ((RadioButton)sender == radioButton4)
                mStationIndex = 3;
            else if ((RadioButton)sender == radioButton5)
                mStationIndex = 4;
            else if ((RadioButton)sender == radioButton6)
                mStationIndex = 5;
            else if ((RadioButton)sender == radioButton7)
                mStationIndex = 6;

            if ((((RadioButton)sender).Text) == "Label")
            {
                label7.Visible = true;
                txtBarc.Visible = true;
            }
            else
            {
                label7.Visible = false;
                txtBarc.Visible = false;
            }

        }

        private void cBoxResult1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) //当按下回车键
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void btnLoginClick()
        {
            FrmLogin mLogin = new FrmLogin();

            mLogin.ShowDialog();
            if (mLogin.result == 1)
            {
                btnSet.Enabled = true;
                btnLogin.Text = "已登录";
            }
            else
            {
                btnSet.Enabled = false;

                btnLogin.Text = "未登录";
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //设置界面状态
            //setView();
            lstStation = new RadioButton[7];
            lstStation[0] = radioButton1;

            //数据库初始化
            mDb.init(@"..\debug\csdn.mdb");
            sLst = mDb.Get(1);
            for (int i = 0; i < sLst.Count; i++)
            {
                cBoxStation.Items.Add(sLst[i]);
            }
            List<string> mLst = mDb.Get(1);
            for (int i = 0; i < mLst.Count; i++)
            {
                ListViewItem lt = new ListViewItem();
                lt.Text = mLst[i];

                //将lt数据添加到listView1控件中
                lstViewFlow.Items.Add(lt);
            }

            updateOperatorPartlst();

            cBoxResult1.SelectedIndex = 0;
            //绑定委托函数
            action_event += SetText;

            IPAddress ip = IPAddress.Parse(IpStr);
            IPEndPoint ip_end_point = new IPEndPoint(ip, port);
            //=============================================================
            server = new TcpListener(ip_end_point);
            server.Start();
            //Console.WriteLine("启动监听成功");
            //txtMsg.Text = "启动监听成功";
            action_event("启动监听成功");
            server.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpclient), server);//异步接收 递归循环接收多个客户端
            //Console.ReadKey();
        }

        private void SkinButton3_Click(object sender, EventArgs e)
        {
            skinButton3Click();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            btnLoginClick();
        }

        private void BtnSet_Click(object sender, EventArgs e)
        {
            btnSetClick();
        }

        private void SkinButton6_Click(object sender, EventArgs e)
        {
            skinButton6Click();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            btnSearchClick();
        }

        private void SkinButton2_Click(object sender, EventArgs e)
        {
            skinButton2Click();
        }
    }
}
