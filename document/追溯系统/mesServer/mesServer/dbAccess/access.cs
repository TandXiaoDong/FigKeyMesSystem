using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dbAccess
{
      //using System.Data.OleDb;
    public class Access
    {
        private static int MLENGH = 9;
 
        OleDbConnection oleDb = null;
        //OleDbConnection oleDb = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\Administrator\Desktop\CSDN.mdb");
       
        private string[] st = new string[300*9];
        public List<string> mList;
        public string[] ST_CONTENT
        {
            get { return st; }
            set { st = value; }
        }

        public Access() //构造函数
        {
         
        }
        public void init(string sDbPath)
        {
            oleDb = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source="+sDbPath);

            oleDb.Open();
            mList = this.GetTableNameList();
        }
        public List<string> Get(int i = 0)
        {
            List<string> mLst = new List<string>();
            string sql =string.Format( "select * from {0}", mList[i]);
            //获取flowData的内容
            OleDbDataAdapter dbDataAdapter = new OleDbDataAdapter(sql, oleDb); //创建适配对象
            DataTable dt = new DataTable(); //新建表对象
            dbDataAdapter.Fill(dt); //用适配对象填充表对象
            
            foreach (DataRow item in dt.Rows)
            {
                mLst.Add(item[1].ToString());
                //Console.WriteLine(item[0] + " | " + item[1]);
            }

            return mLst;
        }
        public List<string> GetAllPartLst(int i = 0)
        {
            List<string> mLst = new List<string>();
            string sql = string.Format("select * from {0}", mList[i]);
            //获取flowData的内容
            OleDbDataAdapter dbDataAdapter = new OleDbDataAdapter(sql, oleDb); //创建适配对象
            DataTable dt = new DataTable(); //新建表对象
            dbDataAdapter.Fill(dt); //用适配对象填充表对象

            foreach (DataRow item in dt.Rows)
            {
                mLst.Add(item[0].ToString());
                //Console.WriteLine(item[0] + " | " + item[1]);
            }

            return mLst;
        }
        public string[] Find(string sSearch,int iIndex=0)
        {
            string sql = string.Format("select top 1 * from {1} WHERE DMC='{0}' order by ID desc", sSearch, mList[iIndex]);

            for (int n = 0; n < 300 * 9; n++)
            {
                st[n] = string.Empty;
            }
            //获取flowData中昵称为LanQ的内容
            OleDbDataAdapter dbDataAdapter = new OleDbDataAdapter(sql, oleDb); //创建适配对象
            DataTable dt = new DataTable(); //新建表对象 
            dbDataAdapter.Fill(dt); //用适配对象填充表对象
             
            foreach (DataRow item in dt.Rows)
            {
                for (int i = 0; i < MLENGH; i++)
                {
                    st[i] = item[i].ToString();
                }
            }
            return st;
        }
        public string[] FindLst(string sSearch, int iIndex = 0)
        {
            string sql = string.Format("select top 1 * from {1} WHERE DMC='{0}' order by ID desc", sSearch, mList[iIndex]);

            string[] st1 = new string[2700];
            for (int n = 0; n < 300 * 9; n++)
            {
                st1[n] = string.Empty;
            }
            //获取flowData中昵称为LanQ的内容
            OleDbDataAdapter dbDataAdapter = new OleDbDataAdapter(sql, oleDb); //创建适配对象
            DataTable dt = new DataTable(); //新建表对象 
            dbDataAdapter.Fill(dt); //用适配对象填充表对象

            foreach (DataRow item in dt.Rows)
            {
                for (int i = 0; i < MLENGH; i++)
                {
                    st1[i] = item[i].ToString();
                }
            }
            return st1;
        }
        /// <summary>
        /// 查询平台的站位流程
        /// </summary>
        /// <param name="sSearch"></param>
        /// <param name="sSerachItem"></param>
        /// <param name="iIndex"></param>
        /// <returns></returns>
        public string[] FindPlatLst(string sSearch, string sSerachItem = "DMC", int iIndex = 0)
        {
            string sql = string.Format("select * from {1} WHERE {2}='{0}'", sSearch, mList[iIndex], sSerachItem);
            string[] st1 = new string[2700];
            for (int n = 0; n < 300 * 9; n++)
            {
                st1[n] = string.Empty;
            }
            //获取flowData中昵称为LanQ的内容
            OleDbDataAdapter dbDataAdapter = new OleDbDataAdapter(sql, oleDb); //创建适配对象
            DataTable dt = new DataTable(); //新建表对象 
            dbDataAdapter.Fill(dt); //用适配对象填充表对象

            int mCounter = 0;

            foreach (DataRow item in dt.Rows)
            {
                for (int i = 0; i < MLENGH; i++)
                {
                    st1[i + 9 * mCounter] = item[i].ToString();
                }
                mCounter++;
            }
            return st1;
        }
        public string[] FindAll(string sSearch,string sSerachItem ="DMC", int iIndex = 0)
        {
            string sql = string.Format("select * from {1} WHERE {2}='{0}'", sSearch, mList[iIndex],sSerachItem);

            for (int n = 0; n < 300 * 9; n++)
            {
                st[n] = string.Empty;
            }
            try
            {
                //获取flowData中昵称为LanQ的内容
                OleDbDataAdapter dbDataAdapter = new OleDbDataAdapter(sql, oleDb); //创建适配对象
                DataTable dt = new DataTable(); //新建表对象 
                dbDataAdapter.Fill(dt); //用适配对象填充表对象

                int mCounter = 0;

                foreach (DataRow item in dt.Rows)
                {
                    for (int i = 0; i < MLENGH; i++)
                    {
                        st[i + 9 * mCounter] = item[i].ToString();
                    }
                    mCounter++;
                }
            }
            catch (Exception)
            {
                //throw new Exception("fail to read dataBase");
                ;
            }

            return st;
        }
        public void deleteData(string sSearch, string sSerachItem = "平台号", int iIndex = 2)
        {
            string sql = string.Format("delete * from {1} WHERE {2}='{0}'", sSearch, mList[iIndex], sSerachItem);

            for (int n = 0; n < 300 * 9; n++)
            {
                st[n] = string.Empty;
            }
            //获取flowData中昵称为LanQ的内容
            OleDbCommand oleDbCommand = new OleDbCommand(sql, oleDb);
            oleDbCommand.ExecuteNonQuery(); //返回被修改的数目

            //int mCounter = 0;

            //foreach (DataRow item in dt.Rows)
            //{
            //    for (int i = 0; i < MLENGH; i++)
            //    {
            //        st[i + 9 * mCounter] = item[i].ToString();
            //    }
            //    mCounter++;
            //}
            //return st;
        }
   
        public string setFirstStation(string sFamily, string sType, string dmc, string sRemark1 = "", string sRemark2 = "", int i = 0)
        { 
            Find(dmc); 

            if (st[0] == string.Empty)
            {
                //dt.Rows.INSERT INTO Persons VALUES
                string sql = string.Format("insert into {9} values ({0},'{1}','{2}','{3}',{4},'{5}','{6}','{7}','{8}')",
                    0, sFamily, sType, dmc, 0, "PASS", DateTime.Now.ToString(), sRemark1, sRemark2, mList[i]);
                //往flowData添加一条记录，昵称是LanQ，账号是2545493686

                try
                {
                    OleDbCommand oleDbCommand = new OleDbCommand(sql, oleDb);
                    oleDbCommand.ExecuteNonQuery(); //返回被修改的数目
                }
                catch (Exception)
                {
                    throw new Exception("首站设置失败!");
                }
                return "首站设置成功!";
            }
            else
                return "该追溯码已经存在!";
        }

        public string setStation(string dmc, int mStation, string sRemark1 = "", string sRemark2 = "admin set", int i = 0)
        {
            Find(dmc);

            if (st[0] != string.Empty)
            {
                //dt.Rows.INSERT INTO Persons VALUES
                string sql = string.Format("insert into {9} values ({0},'{1}','{2}','{3}',{4},'{5}','{6}','{7}','{8}')",
                    int.Parse(st[0]) + 1, st[1], st[2], st[3], mStation - 1, "PASS", DateTime.Now.ToString(), sRemark1, sRemark2, mList[i]);
                
                try
                {
                    OleDbCommand oleDbCommand = new OleDbCommand(sql, oleDb);
                    oleDbCommand.ExecuteNonQuery(); //返回被修改的数目
                }
                catch (Exception)
                {
                    throw new Exception("站位设置失败!");
                }
                return "站位设置成功!";
            }
            else
                return "该追溯码不存在!";
        }
        //public string Add(string[] lastData, string dmc,int mStation, string status)
        //{
        //    //dt.Rows.INSERT INTO Persons VALUES
        //    string sql = string.Format("insert into flowData values ({0},'{1}','{2}','{3}',{4},'{5}','{6}','{7}','{8}')",
        //        int.Parse(lastData[0]) + 1, lastData[1], lastData[2], lastData[3], mStation, status, DateTime.Now.ToString(), "", "");
        //    //往flowData添加一条记录，昵称是LanQ，账号是2545493686

        //    try
        //    {
        //        OleDbCommand oleDbCommand = new OleDbCommand(sql, oleDb);
        //        oleDbCommand.ExecuteNonQuery(); //返回被修改的数目
        //    }
        //    catch (Exception)
        //    {
        //        throw new Exception("添加数据失败!");
        //    }
        //    return "OK";
        //}
        public string Add(string[] lastData, string dmc, int mStation, string status, string sRemark1, string sRemark2, int i = 0)
        {
            //dt.Rows.INSERT INTO Persons VALUES
            string sql = string.Format("insert into {9} values ({0},'{1}','{2}','{3}',{4},'{5}','{6}','{7}','{8}')",
                int.Parse(lastData[0]) + 1, lastData[1], lastData[2], lastData[3], mStation, status, DateTime.Now.ToString(), sRemark1, sRemark2, mList[i]);
            //往flowData添加一条记录，昵称是LanQ，账号是2545493686

            try
            {
                OleDbCommand oleDbCommand = new OleDbCommand(sql, oleDb);
                oleDbCommand.ExecuteNonQuery(); //返回被修改的数目
            }
            catch (Exception)
            {
                throw new Exception("添加数据失败!");
            }
            return "OK";
        }
        public string AddPlatLst(string sPlatName, string[] lastData, int i = 2)
        {
            //dt.Rows.INSERT INTO Persons VALUES
            string sql = string.Format("insert into {9} values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                sPlatName, lastData[0], lastData[1], lastData[2], lastData[3], lastData[4], lastData[5], lastData[6], lastData[7], mList[i]);
            //往flowData添加一条记录，昵称是LanQ，账号是2545493686

            try
            {
                OleDbCommand oleDbCommand = new OleDbCommand(sql, oleDb);
                oleDbCommand.ExecuteNonQuery(); //返回被修改的数目
            }
            catch (Exception)
            {
                throw new Exception("添加数据失败!");
            }
            FindAll(sPlatName, "平台号", 2);

            return "OK";
        }

        public string checkPart(int mStation, string sDmc)
        {
            Find(sDmc);
            if (st[0] != string.Empty)
            {
                //上一站位pass
                if ((int.Parse(st[4]) == mStation - 1) && (st[5] == "PASS"))
                    return "OK";
                ////当前站位fail
                //else if ((int.Parse(st[4]) == mStation) && (st[5] == "FAIL"))
                //    return "OK";
                else
                    return "Part is not for this station!";
            } 
            else
                return "this identifier does not exist!";
        }
        public string insertData(int mStation, string sDmc, string sResult, string sRemark1 = "", string sRemark2 = "", int i = 0)
        {
            string s =checkPart(mStation,sDmc);
            if (s == "OK")
                return Add(st, sDmc, mStation, sResult, sRemark1, sRemark2, i);
            else
                return s;

            ////读最后一条记录
            //Find(sDmc);
            //if (st[0] != string.Empty)
            //{
            //    //上一站位pass
            //    if ((int.Parse(st[4]) == mStation - 1) && (st[5] == "PASS"))
            //        return Add(st, sDmc, mStation, sResult, sRemark1, sRemark2,i);
            //    //当前站位fail
            //    else if ((int.Parse(st[4]) == mStation) && (st[5] == "FAIL"))
            //        return Add(st, sDmc, mStation, sResult, sRemark1, sRemark2,i);
            //    else
            //        return "Part is not for this station!";
            //}
            //else
            //    return "this identifier does not exist!";
        }

        public string insertPlatData(string sPlatName, string[] sList)
        {
            FindAll(sPlatName, "平台号", 2);
            if (st[0] != string.Empty)
            {
                deleteData(sPlatName);
                return AddPlatLst(sPlatName, sList);
            }
            else
                return AddPlatLst(sPlatName, sList);
        }

        public List<string> GetTableNameList()
        {
            List<string> list = new List<string>();
            //OleDbConnection Conn = new OleDbConnection(oleDb);
            try
            {
                if (oleDb.State == ConnectionState.Closed)
                    oleDb.Open();
                DataTable dt = oleDb.GetSchema("Tables");
                foreach (DataRow row in dt.Rows)
                {
                    if (row[3].ToString() == "TABLE")
                        list.Add(row[2].ToString());
                }
                return list;
            }
            catch (Exception e)
            { throw e; }
           
        }

        //public bool Del()
        //{
        //    string sql = "delete from flowData where 昵称='LANQ'";
        //    //删除昵称为LanQ的记录
        //    OleDbCommand oleDbCommand = new OleDbCommand(sql, oleDb);
        //    int i = oleDbCommand.ExecuteNonQuery();
        //    return i > 0;
        //}

        //public bool Change()
        //{
        //    string sql = "update flowData set 账号='233333' where 昵称='东熊'";
        //    //将flowData中昵称为东熊的账号修改成233333
        //    OleDbCommand oleDbCommand = new OleDbCommand(sql, oleDb);
        //    int i = oleDbCommand.ExecuteNonQuery();
        //    return i > 0;
        //}
    }
}
