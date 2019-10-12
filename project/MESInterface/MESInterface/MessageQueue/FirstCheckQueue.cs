using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using MESInterface;
using CommonUtils.DB;
using CommonUtils.Logger;
using MESInterface.Molde;

namespace MESInterface.MessageQueue.RemoteClient
{
    public class FirstCheckQueue
    {
        public static string CheckPass(Queue<string[]> queue)
        {
            /*
             * 1、判断传入站是不是首站，首站的判断：根据设置的首站判断
             * 2、判断上一站是否通过
             * 3、判断传入站追溯号是否存在
             * 4、判断传入站型号是否存在
             * 5、判断传入站站位名称是否存在
             * 
             */
            //根据传入站-查询该站的产线流程中的上一站
            //
            //判断首站
            //取出队列
            string[] array = queue.Dequeue();
            string sn_inner = array[0];
            string sn_outter = array[1];
            string sTypeNumber = array[2];
            string sStationName = array[3];
            MesService mesService = new MesService();

            DataTable dataSet = mesService.SelectStation(sStationName, "").Tables[0];
            var station = dataSet.Rows[0][1].ToString().Trim();
            var order = int.Parse(dataSet.Rows[0][0].ToString().Trim());
            if (sStationName == station)
            {
                //插入数据库
                LogHelper.Log.Info("判断结果为首站，将插入到数据库");
                string insertSQL = "INSERT INTO [WT_SCL].[dbo].[Product_Data] " +
                    "([SN]," +
                    "[Type_Number]," +
                    "[Station_Name]," +
                    "[Test_Result]," +
                    "[CreateDate]," +
                    "[UpdateDate]," +
                    "[Remark]) " +
                    $"VALUES('{sn_inner}-{sn_outter}','{sTypeNumber}','{sStationName}','','{GetDateTimeNow()}','{GetDateTimeNow()}','首站插入')";
                int ins = SQLServer.ExecuteNonQuery(insertSQL);
                if (ins > 0)
                {
                    //插入成功
                    return (int)FirstCheckResultEnum.STATUS_FIRST_STATION_INSERT_SUCCESS + "";
                }
                return (int)FirstCheckResultEnum.ERR_FIRST_STATION_INSERT_FAIL + "";
            }
            else
            {
                //非首站
                //判断上一站位是否通过
                //查询该产品型号的所属站位
                DataTable data = mesService.SelectTypeStation(sTypeNumber).Tables[0];
                string lastTestResult = "";
                int lastIndex = 0;
                for (int i = 0; i < data.Columns.Count; i++)
                {
                    if (data.Rows[0][i].ToString().Trim() == sStationName)
                    {
                        lastIndex = i - 1;
                        break;
                    }
                }
                //查询到上一个站位
                string lastStation = data.Rows[0][lastIndex].ToString().Trim();
                //验证传入参数是否存在：追溯号+型号+站位号
                if (!IsExistRecord(sn_inner, sn_outter,sTypeNumber,sStationName))
                {
                    //不存在记录，则插入
                    //return (int)FirstCheckResultEnum.ERR_RECORD_NOT_EXIST + "";
                }

                //查询上一个站位的测试结果
                DataTable dt = SelectProductData(sn_inner.Trim(), sn_outter.Trim(), sTypeNumber, sStationName).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    lastTestResult = dt.Rows[0][3].ToString().Trim();
                }
                if (lastTestResult == "PASS")
                {
                    LogHelper.Log.Info("last test result is pass...");
                    return (int)FirstCheckResultEnum.STATUS_LAST_TEST_PASS + "";
                }
                else
                {
                    LogHelper.Log.Info("last test result is fail...");
                    return (int)FirstCheckResultEnum.STATUS_LAST_TEST_FAIL + "";
                }
            }
        }

        private static string GetDateTimeNow()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }


        /// <summary>
        /// 查询结果
        /// </summary>
        /// <param name="snInner"></param>
        /// <param name="snOutter"></param>
        /// <param name="typeNumber"></param>
        /// <param name="stationName"></param>
        /// <returns></returns>
        public static DataSet SelectProductData(string snInner, string snOutter, string typeNumber, string stationName)
        {
            string selectSQL = "SELECT [SN],[Type_Number],[Station_Name],[Test_Result],[CreateDate],[UpdateDate],[Remark] " +
                "FROM [WT_SCL].[dbo].[Product_Data] " +
                $"WHERE [SN] = '{snInner}' OR [SN]='{snInner}-{snOutter}' AND [Type_Number]='{typeNumber}' AND [Station_Name]='{stationName}'";
            return SQLServer.ExecuteDataSet(selectSQL);
        }

        private static bool IsExistSn(string snInner, string snOutter)
        {
            string selectSQL = "SELECT [SN],[Type_Number],[Station_Name],[Test_Result],[CreateDate],[UpdateDate],[Remark] " +
                "FROM [WT_SCL].[dbo].[Product_Data] " +
                $"WHERE [SN]='{snInner}-{snOutter}'";
            DataTable dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return true;
            else
                return false;
        }

        private static bool IsExistTypeNumber(string typeNumber)
        {
            string selectSQL = "SELECT [SN],[Type_Number],[Station_Name],[Test_Result],[CreateDate],[UpdateDate],[Remark] " +
                "FROM [WT_SCL].[dbo].[Product_Data] " +
                $"WHERE [Type_Number]='{typeNumber}'";
            DataTable dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }

        private static bool IsExistStation(string stationName)
        {
            string selectSQL = "SELECT [SN],[Type_Number],[Station_Name],[Test_Result],[CreateDate],[UpdateDate],[Remark] " +
                "FROM [WT_SCL].[dbo].[Product_Data] " +
                $"WHERE [Station_Name]='{stationName}'";
            DataTable dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }

        private static bool IsExistRecord(string snInner, string snOutter,string typeNumber,string stationName)
        {
            string selectSQL = "SELECT [SN],[Type_Number],[Station_Name],[Test_Result],[CreateDate],[UpdateDate],[Remark] " +
                "FROM [WT_SCL].[dbo].[Product_Data] " +
                $"WHERE " +
                $"[SN]='{snInner}-{snOutter}' " +
                $"AND " +
                $"[Type_Number]='{typeNumber}' " +
                $"AND " +
                $"[Station_Name]='{stationName}'";
            DataTable dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return true;
            else
                return false;
        }
    }
}