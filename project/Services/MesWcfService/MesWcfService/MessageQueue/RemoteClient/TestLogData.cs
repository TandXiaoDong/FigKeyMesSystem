using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonUtils.DB;
using CommonUtils.Logger;
using MesWcfService.DB;

namespace MesWcfService.MessageQueue.RemoteClient
{
    public class TestLogData
    {
        public static string UpdateTestLogData(Queue<string[]> tlQueue)
        {
            try
            {//typeNo,stationName,productSN,testItem,limit,currentValue,testResult,teamLeader,admin
                string[] array = tlQueue.Dequeue();
                var typeNo = array[0];
                var stationName = array[1];
                var productSN = array[2];
                var testItem = array[3];
                var limit = array[4];
                var currentValue = array[5];
                var testResult = array[6];
                var teamLeader = array[7];
                var admin = array[8];
                var joinDateTime = array[9];

                var insertSQL = $"INSERT INTO {DbTable.F_TEST_LOG_DATA_NAME}(" +
                    $"{DbTable.F_TEST_LOG_DATA.TYPE_NO}," +
                    $"{DbTable.F_TEST_LOG_DATA.STATION_NAME}," +
                    $"{DbTable.F_TEST_LOG_DATA.PRODUCT_SN}," +
                    $"{DbTable.F_TEST_LOG_DATA.TEST_ITEM}," +
                    $"{DbTable.F_TEST_LOG_DATA.LIMIT}," +
                    $"{DbTable.F_TEST_LOG_DATA.CURRENT_VALUE}," +
                    $"{DbTable.F_TEST_LOG_DATA.TEST_RESULT}," +
                    $"{DbTable.F_TEST_LOG_DATA.TEAM_LEADER}," +
                    $"{DbTable.F_TEST_LOG_DATA.ADMIN}," +
                    $"{DbTable.F_TEST_LOG_DATA.UPDATE_DATE}," +
                    $"{DbTable.F_TEST_LOG_DATA.JOIN_DATE_TIME}) VALUES(" +
                    $"'{typeNo}','{stationName}','{productSN}','{testItem}','{limit}'," +
                    $"'{currentValue}','{testResult}','{teamLeader}','{admin}'," +
                    $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','{joinDateTime}')";
                //if (IsExistLogData(array))
                //    return "OK";
                var res = SQLServer.ExecuteNonQuery(insertSQL);
                if (res > 0)
                    return "OK";
                else
                {
                    LogHelper.Log.Info(insertSQL);
                    return "FAIL";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error(ex.Message);
                return "ERROR";
            }
        }

        private static bool IsExistLogData(string[] logArray)
        {
            var selectSQL = $"SELECT * FROM {DbTable.F_TEST_LOG_DATA_NAME} WHERE " +
                $"{DbTable.F_TEST_LOG_DATA.TYPE_NO} = '{logArray[0]}' AND " +
                $"{DbTable.F_TEST_LOG_DATA.STATION_NAME} = '{logArray[1]}' AND " +
                $"{DbTable.F_TEST_LOG_DATA.PRODUCT_SN} = '{logArray[2]}' AND " +
                $"{DbTable.F_TEST_LOG_DATA.TEST_ITEM} = '{logArray[3]}' AND " +
                $"{DbTable.F_TEST_LOG_DATA.LIMIT} = '{logArray[4]}' AND " +
                $"{DbTable.F_TEST_LOG_DATA.CURRENT_VALUE} = '{logArray[5]}' AND " +
                $"{DbTable.F_TEST_LOG_DATA.TEST_RESULT} = '{logArray[6]}' AND " +
                $"{DbTable.F_TEST_LOG_DATA.TEAM_LEADER} = '{logArray[7]}' AND " +
                $"{DbTable.F_TEST_LOG_DATA.ADMIN} = '{logArray[8]}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }
    }
}