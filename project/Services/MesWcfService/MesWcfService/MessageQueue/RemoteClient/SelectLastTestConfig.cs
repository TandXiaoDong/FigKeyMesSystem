using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonUtils.DB;
using CommonUtils.Logger;
using MesWcfService.DB;

namespace MesWcfService.MessageQueue.RemoteClient
{
    public class SelectLastTestConfig
    {
        public static string[] SelectSpecLimit(Queue<string[]> queue)
        {
            var array = queue.Dequeue();
            var productTypeNo = array[0];
            var productStation = array[1];
            var productItem = array[2];
            var selectSQL = $"SELECT top 1 {DbTable.F_TEST_LIMIT_CONFIG.TYPE_NO}," +
                $"{DbTable.F_TEST_LIMIT_CONFIG.STATION_NAME}," +
                $"{DbTable.F_TEST_LIMIT_CONFIG.TEST_ITEM}," +
                $"{DbTable.F_TEST_LIMIT_CONFIG.LIMIT}," +
                $"{DbTable.F_TEST_LIMIT_CONFIG.TEAM_LEADER}," +
                $"{DbTable.F_TEST_LIMIT_CONFIG.ADMIN} FROM " +
                $"{DbTable.F_TEST_LIMIT_CONFIG_NAME} " +
                $"WHERE " +
                $"{DbTable.F_TEST_LIMIT_CONFIG.TYPE_NO} = '{productTypeNo}' AND " +
                $"{DbTable.F_TEST_LIMIT_CONFIG.STATION_NAME} = '{productStation}' AND " +
                $"{DbTable.F_TEST_LIMIT_CONFIG.TEST_ITEM} = '{productItem}' " +
                $"ORDER BY " +
                $"{DbTable.F_TEST_LIMIT_CONFIG.UPDATE_DATE} ASC";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count < 1)
                return new string[] { "query result is null"};
            var typeNo = dt.Rows[0][0].ToString();
            var stationName = dt.Rows[0][1].ToString();
            var item = dt.Rows[0][2].ToString();
            var limit = dt.Rows[0][3].ToString();
            var teamLeader = dt.Rows[0][4].ToString();
            var admin = dt.Rows[0][5].ToString();
            return new string[] { typeNo,stationName,item,limit,teamLeader,admin};
        }

        public static string[] SelectPVersion(Queue<string[]> queue)
        {
            var array = queue.Dequeue();
            var productTypeNo = array[0];
            var productStation = array[1];
            var selectSQL = $"SELECT top 1 {DbTable.F_TEST_PROGRAME_VERSION.TYPE_NO}," +
                $"{DbTable.F_TEST_PROGRAME_VERSION.STATION_NAME}," +
                $"{DbTable.F_TEST_PROGRAME_VERSION.PROGRAME_NAME}," +
                $"{DbTable.F_TEST_PROGRAME_VERSION.PROGRAME_VERSION}," +
                $"{DbTable.F_TEST_PROGRAME_VERSION.TEAM_LEADER}," +
                $"{DbTable.F_TEST_PROGRAME_VERSION.ADMIN} FROM " +
                $"{DbTable.F_TEST_PROGRAME_VERSION_NAME} " +
                $"WHERE " +
                $"{DbTable.F_TEST_PROGRAME_VERSION.TYPE_NO} = '{productTypeNo}' AND " +
                $"{DbTable.F_TEST_PROGRAME_VERSION.STATION_NAME} = '{productStation}' "+
                $"ORDER BY " +
                $"{DbTable.F_TEST_PROGRAME_VERSION.UPDATE_DATE} ASC";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count < 1)
                return new string[] { "query result is null" };
            var typeNo = dt.Rows[0][0].ToString();
            var stationName = dt.Rows[0][1].ToString();
            var programeName = dt.Rows[0][2].ToString();
            var programeVersion = dt.Rows[0][3].ToString();
            var teamLeader = dt.Rows[0][4].ToString();
            var admin = dt.Rows[0][5].ToString();
            return new string[] { typeNo, stationName, programeName, programeVersion, teamLeader, admin };
        }
    }
}