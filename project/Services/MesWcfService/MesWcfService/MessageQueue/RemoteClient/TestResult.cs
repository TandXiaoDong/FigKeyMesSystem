using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonUtils.DB;
using CommonUtils.Logger;
using MesWcfService.DB;
using System;
using MesWcfService;

namespace MesWcfService.MessageQueue.RemoteClient
{
    public class TestResult
    {
        private enum UpdateTestResultEnum
        {
            STATUS_SUCCESS = 0,
            ERROR_FAIL = 1,
            ERROR_SN_IS_NULL = 2,
            ERROR_STATION_IS_NULL = 3,
            ERROR_FIRST_STATION = 4
        }

        private static string ConvertTestResultCode(UpdateTestResultEnum rCode)
        {
            return "0X" + Convert.ToString((int)rCode, 16).PadLeft(2, '0');
        }

        /*
 * 测试结果要记录进站时间与出站时间
 * 进站时间：开始测试的时间，查询上一工位测试结果为PASS的时间
 * 出站时间：测试完成的时间
 * 流程：
 * 1）新增接口记录第一个工站开始的时间，更新进站日期；
 *  更新：第一次插入数据后，下一个更新进站日期时，查询该工位最新的一条数据进行更新
 * 2）传入测试数据结果的接口用于更新出站时间
 * 3）第1个工站之后的所有工站的进站时间为该工站查询上一工站返回结果为PASS的时间
 *      当查询上一站位成功后，将信息插入到数据库（带进站时间），待测试完成后进行更新
 * 
 */
        public static string CommitTestResult(Queue<string[]> queue)
        {
            string[] array = queue.Dequeue();
            var sn = array[0].Trim();
            var typeNo = array[1].Trim();
            var station = array[2].Trim();
            var result = array[3].Trim();
            var teamLeader = array[4].Trim();
            var admin = array[5].Trim();
            var joinDateTime = array[6].Trim();
            if (sn == "")
                return ConvertTestResultCode(UpdateTestResultEnum.ERROR_SN_IS_NULL);
            if (station == "")
                return ConvertTestResultCode(UpdateTestResultEnum.ERROR_STATION_IS_NULL);

            if (IsFirstStation(typeNo,station))
            {
                //传入工站为第一个工站，插入数据，更新该工站的进站时间
                LogHelper.Log.Info("【传入工站为第一个工站,插入进站记录】");
                if (result == "")//进站
                {
                    //未插入进站记录
                    LogHelper.Log.Info("【第一个工位-未插入进站记录-开始插入进站记录】");
                    var row = InsertTestResult(sn, typeNo, station);
                    if (row > 0)
                    {
                        return ConvertTestResultCode(UpdateTestResultEnum.STATUS_SUCCESS);
                    }
                    else
                    {
                        return ConvertTestResultCode(UpdateTestResultEnum.ERROR_FAIL);
                    }
                }
                else
                {
                    //已插入进站记录
                    //更新出站时间
                    LogHelper.Log.Info("【第一个工位-已插入进站记录-更新出站记录】");
                    var row = UpdateTestResult(sn, typeNo, station, result, teamLeader, admin, joinDateTime);
                    if (row > 0)
                    {
                        return ConvertTestResultCode(UpdateTestResultEnum.STATUS_SUCCESS);
                    }
                    else
                    {
                        return ConvertTestResultCode(UpdateTestResultEnum.ERROR_FAIL);
                    }
                }
            }
            else
            {
                //传入工站为第一个工站之后的工站，更新测试结果与出站时间
                LogHelper.Log.Info("【传入工位不为第一个工位-更新出站记录】");
                var row = UpdateTestResult(sn,typeNo,station,result,teamLeader,admin, joinDateTime);
                if (row > 0)
                {
                    return ConvertTestResultCode(UpdateTestResultEnum.STATUS_SUCCESS);
                }
                else
                {
                    return ConvertTestResultCode(UpdateTestResultEnum.ERROR_FAIL);
                }
            }
        }

        /// <summary>
        /// 主要是插入基础参数与进站时间
        /// </summary>
        /// <param name="stationInArray"></param>
        /// <returns></returns>
        private static int InsertTestResult(string sn,string typeNo,string station)
        {
            var processName = new MesService().SelectCurrentTProcess();
            var insertSQL = $"INSERT INTO {DbTable.F_TEST_RESULT_NAME}(" +
                        $"{DbTable.F_Test_Result.SN}," +
                        $"{DbTable.F_Test_Result.TYPE_NO}," +
                        $"{DbTable.F_Test_Result.STATION_NAME}," +
                        $"{DbTable.F_Test_Result.PROCESS_NAME}," +
                        $"{DbTable.F_Test_Result.UPDATE_DATE}," +
                        $"{DbTable.F_Test_Result.STATION_IN_DATE}) " +
                        $"VALUES('{sn}','{typeNo}','{station}','{processName}'," +
                        $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'," +
                        $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
            return SQLServer.ExecuteNonQuery(insertSQL);
        }

        private static int UpdateTestResult(string sn ,string typeNo,string station,
            string result,string teamder,string admin,string joinDateTime)
        {
            var stationInTime = SelectStationInDate(sn,station,typeNo);
            var updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_NAME} SET {DbTable.F_Test_Result.TEST_RESULT} = '{result}'," +
                $"{DbTable.F_Test_Result.TEAM_LEADER} = '{teamder}'," +
                $"{DbTable.F_Test_Result.ADMIN} = '{admin}'," +
                $"{DbTable.F_Test_Result.STATION_OUT_DATE} = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'," +
                $"{DbTable.F_Test_Result.JOIN_DATE_TIME} = '{joinDateTime}' " +
                $"WHERE " +
                $"{DbTable.F_Test_Result.SN} = '{sn}' AND " +
                $"{DbTable.F_Test_Result.TYPE_NO} = '{typeNo}' AND " +
                $"{DbTable.F_Test_Result.STATION_NAME} = '{station}' AND " +
                $"{DbTable.F_Test_Result.STATION_IN_DATE} = '{stationInTime}'";
            LogHelper.Log.Info("【更新出站记录】"+updateSQL);
            return SQLServer.ExecuteNonQuery(updateSQL);
        }

        private static string SelectStationInDate(string sn,string stationName,string typeNo)
        {
            var selectSQL = $"SELECT TOP 1 {DbTable.F_Test_Result.STATION_IN_DATE} FROM " +
                $"{DbTable.F_TEST_RESULT_NAME} " +
                $"WHERE " +
                $"{DbTable.F_Test_Result.SN} = '{sn}' AND " +
                $"{DbTable.F_Test_Result.STATION_NAME} = '{stationName}' AND " +
                $"{DbTable.F_Test_Result.TYPE_NO} = '{typeNo}' " +
                $"ORDER BY {DbTable.F_Test_Result.STATION_IN_DATE} DESC";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();
            return "";
        }

        private static bool IsExistTestResult(string typeNo,string station,string dateTime,string result)
        {
            string selectSQL = $"SELECT * FROM {DbTable.F_TEST_RESULT_NAME} WHER " +
                    $"{DbTable.F_Test_Result.TYPE_NO} = '{typeNo}' AND " +
                    $"{DbTable.F_Test_Result.STATION_NAME} = '{station}' AND " +
                    $"{DbTable.F_Test_Result.UPDATE_DATE} = '{dateTime}' AND " +
                    $"{DbTable.F_Test_Result.TEST_RESULT} = '{result}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }

        public static string[] SelectTestResult(Queue<string[]> queue)
        {
            string[] queryResult;
            try
            {
                string[] array = queue.Dequeue();
                string sn = array[0];
                string currentStation = array[1];
                LogHelper.Log.Info("测试端查询测试结果,站位为" + currentStation + " SN="+sn);
                //根据当前工艺与站位，查询其上一站位
                //查询当前工艺流程
                MesService mesService = new MesService();
                var processName = mesService.SelectCurrentTProcess();
                string selectOrderSQL = $"SELECT {DbTable.F_TECHNOLOGICAL_PROCESS.STATION_ORDER} " +
                    $"FROM {DbTable.F_TECHNOLOGICAL_PROCESS_NAME} " +
                    $"WHERE {DbTable.F_TECHNOLOGICAL_PROCESS.STATION_NAME} = '{currentStation}' AND " +
                    $"{DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME} = '{processName}'";
                LogHelper.Log.Info("【查询当前站ID】"+selectOrderSQL);
                DataTable dt = SQLServer.ExecuteDataSet(selectOrderSQL).Tables[0];
                if (dt.Rows.Count < 1)
                {
                    queryResult = new string[1];
                    queryResult[0] = "ERR_LAST_STATION_ID";
                    return queryResult;
                }
                int lastOrder = int.Parse(dt.Rows[0][0].ToString()) - 1;
                LogHelper.Log.Info($"【上一站ID】={lastOrder}");
                selectOrderSQL = $"SELECT {DbTable.F_TECHNOLOGICAL_PROCESS.STATION_NAME} " +
                    $"FROM {DbTable.F_TECHNOLOGICAL_PROCESS_NAME} " +
                    $"WHERE {DbTable.F_TECHNOLOGICAL_PROCESS.STATION_ORDER} = '{lastOrder}' AND " +
                    $"{DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME} = '{processName}'";
                dt = SQLServer.ExecuteDataSet(selectOrderSQL).Tables[0];
                if (dt.Rows.Count < 1)
                {
                    queryResult = new string[1];
                    queryResult[0] = "ERR_LAST_STATION_NAME";
                    return queryResult;
                }
                var lastStation = dt.Rows[0][0].ToString();
                LogHelper.Log.Info($"【上一站工站】={lastStation}");
                //由外壳码查询关联SN
                /*
                 * 由传入SN查询是否绑定PCBA
                 * 查询有结果-已绑定PCBA：
                 *  1）装配时存入的是PCBA
                 *  2）装配时存入的是外壳SN
                 * 查询无结果-未绑定PCBA：直接由传入SN查询
                 */ 
                var snPCBA = SelectSN(sn);
                LogHelper.Log.Info("【查询是否绑定PCBA】snPCBA="+snPCBA);
                //根据上一站位在查询该站位的最后一条记录
                string selectSQL = $"SELECT " +
                    $"{DbTable.F_Test_Result.TEST_RESULT}," +
                    $"{DbTable.F_Test_Result.SN}," +
                    $"{DbTable.F_Test_Result.STATION_NAME}," +
                    $"{DbTable.F_Test_Result.TYPE_NO}," +
                    $"{DbTable.F_Test_Result.STATION_IN_DATE} " +
                    $"FROM " +
                    $"{DbTable.F_TEST_RESULT_NAME} " +
                    $"WHERE " +
                    $"{DbTable.F_Test_Result.SN} = '{sn}' AND " +
                    $"{DbTable.F_Test_Result.STATION_NAME} = '{lastStation}' " +
                    $"ORDER BY " +
                    $"{DbTable.F_Test_Result.UPDATE_DATE} " +
                    $"DESC ";
                LogHelper.Log.Info("【第一次SN查询】"+selectSQL);
                dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                if (dt.Rows.Count < 1)
                {
                    //查询失败
                    //尝试用PCBA去查询
                    LogHelper.Log.Info("【SN查询上一站数据结果为空，】");
                    selectSQL = $"SELECT {DbTable.F_Test_Result.TEST_RESULT}," +
                    $"{DbTable.F_Test_Result.SN}," +
                    $"{DbTable.F_Test_Result.STATION_NAME}," +
                    $"{DbTable.F_Test_Result.TYPE_NO}," +
                    $"{DbTable.F_Test_Result.STATION_IN_DATE}" +
                    $"FROM " +
                    $"{DbTable.F_TEST_RESULT_NAME} " +
                    $"WHERE " +
                    $"{DbTable.F_Test_Result.SN} = '{snPCBA}' AND " +
                    $"{DbTable.F_Test_Result.STATION_NAME} = '{lastStation}' " +
                    $"ORDER BY " +
                    $"{DbTable.F_Test_Result.UPDATE_DATE} " +
                    $"DESC ";
                    dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                    LogHelper.Log.Info("【第二次SN查询】"+selectSQL);
                    if (dt.Rows.Count < 1)
                    {
                        queryResult = new string[1];
                        queryResult[0] = "QUERY_NONE";
                        return queryResult;
                    }
                }
                var testRes = dt.Rows[0][0].ToString();
                //var productSN = dt.Rows[0][1].ToString();
                //var stationName = dt.Rows[0][2].ToString();
                var productTypeNo = dt.Rows[0][3].ToString();
                //var stationInDate = dt.Rows[0][4].ToString();
                //查询结果为PASS后，插入过站进站记录
                if (testRes.Trim().ToLower() == "pass")
                {
                    LogHelper.Log.Info("【查询结果为PASS，插入进站记录】");
                    InsertTestResult(sn,productTypeNo,currentStation);
                }
                else
                {
                    LogHelper.Log.Info("【查询结果不为PASS，不插入进站记录】"+testRes);
                }
                //返回上一个站位的最后一条记录
                queryResult = new string[3];
                queryResult[0] = sn;
                queryResult[1] = lastStation;
                queryResult[2] = testRes;
                return queryResult;
            }
            catch (Exception ex)
            {
                queryResult = new string[1];
                queryResult[0] = "ERR_EXCEPTION";
                LogHelper.Log.Error(ex.Message+"\r\n"+ex.StackTrace);
                return queryResult;
            }
        }

        public static string SelectSN(string snOutter)
        {
            //两种情况
            //sn= snoutter;
            var selectSQL = $"SELECT {DbTable.F_BINDING_PCBA.SN_PCBA} FROM  {DbTable.F_BINDING_PCBA_NAME} " +
                $"WHERE " +
                $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{snOutter}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            //sn= snPcba
            selectSQL = $"SELECT {DbTable.F_BINDING_PCBA.SN_OUTTER} FROM {DbTable.F_BINDING_PCBA_NAME} " +
                $"WHERE " +
                $"{DbTable.F_BINDING_PCBA.SN_PCBA} = '{snOutter}'";

            dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();
            return "";
        }

        private static bool IsFirstStation(string typeNo,string stationName)
        {
            var selectSQL = $"SELECT {DbTable.F_TECHNOLOGICAL_PROCESS.STATION_ORDER} " +
                $"FROM " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS_NAME} " +
                $"WHERE " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME} = '{typeNo}' " +
                $"AND " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.STATION_NAME} = '{stationName}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                var stationOrder = dt.Rows[0][0].ToString();
                if (stationOrder == "1")
                    return true;//是第一个工站
            }
            return false;
        }

        private static bool IsFirstStationLastTest(string sn,string typeNo,string stationName)
        {
            var selectSQL = $"SELECT {DbTable.F_Test_Result.TEST_RESULT}," +
                $"{DbTable.F_Test_Result.STATION_IN_DATE} " +
                $"FROM " +
                $"{DbTable.F_TEST_RESULT_NAME} " +
                $"WHERE " +
                $"{DbTable.F_Test_Result.SN} = '{sn}' AND " +
                $"{DbTable.F_Test_Result.TYPE_NO} = '{typeNo}' AND " +
                $"{DbTable.F_Test_Result.STATION_NAME} = '{stationName}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                var testResult = dt.Rows[0][0].ToString().Trim();
                var stationINDate = dt.Rows[0][1].ToString().Trim();
                if (testResult == "" && stationINDate != "")
                {
                    return true;
                }
            }
            return false;
        }
    }
}