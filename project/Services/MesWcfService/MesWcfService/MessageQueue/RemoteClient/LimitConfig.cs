using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonUtils.DB;
using CommonUtils.Logger;
using MesWcfService.DB;

namespace MesWcfService.MessageQueue.RemoteClient
{
    public class LimitConfig
    {
        public static string UpdateLimitConfig(Queue<string[]> pvqueue)
        {
            try
            {
                string[] array = pvqueue.Dequeue();
                var stationName = array[0];
                var typeNo = array[1];
                var testItem = array[2];
                var limitValue = array[3];
                var teamLeader = array[4];
                var admin = array[5];
                //limit可能为路径
                LogHelper.Log.Info(limitValue);
                if (limitValue.Contains("\\"))
                {
                    limitValue = limitValue.Replace("\\", "\\\\");
                    LogHelper.Log.Info(limitValue);
                }
                var insertSQL = $"INSERT INTO {DbTable.F_TEST_LIMIT_CONFIG_NAME}(" +
                    $"{DbTable.F_TEST_LIMIT_CONFIG.STATION_NAME}," +
                    $"{DbTable.F_TEST_LIMIT_CONFIG.TYPE_NO}," +
                    $"{DbTable.F_TEST_LIMIT_CONFIG.TEST_ITEM}," +
                    $"{DbTable.F_TEST_LIMIT_CONFIG.LIMIT}," +
                    $"{DbTable.F_TEST_LIMIT_CONFIG.TEAM_LEADER}," +
                    $"{DbTable.F_TEST_LIMIT_CONFIG.ADMIN}," +
                    $"{DbTable.F_TEST_LIMIT_CONFIG.UPDATE_DATE}) VALUES(" +
                    $"'{stationName}','{typeNo}','{testItem}','{limitValue}','{teamLeader}','{admin}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
                //if (IsExistLimit(typeNo, stationName, testItem, limitValue, teamLeader, admin))
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

        private static bool IsExistLimit(string typeNo,string stationName,string testItem,string limit,string teamLeader,string admin)
        {
            var selectSQL = $"SELECT * FROM {DbTable.F_TEST_LIMIT_CONFIG_NAME} WHERE " +
                $"{DbTable.F_TEST_LIMIT_CONFIG.TYPE_NO} = '{typeNo}' AND " +
                $"{DbTable.F_TEST_LIMIT_CONFIG.STATION_NAME} = '{stationName}' AND " +
                $"{DbTable.F_TEST_LIMIT_CONFIG.TEST_ITEM} = '{testItem}' AND " +
                $"{DbTable.F_TEST_LIMIT_CONFIG.LIMIT} = '{limit}' AND " +
                $"{DbTable.F_TEST_LIMIT_CONFIG.TEAM_LEADER} = '{teamLeader}' AND " +
                $"{DbTable.F_TEST_LIMIT_CONFIG.ADMIN} = '{admin}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }

    }
}