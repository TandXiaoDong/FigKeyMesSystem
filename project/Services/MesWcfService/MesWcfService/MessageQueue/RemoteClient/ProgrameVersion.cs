using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MesWcfService.Model;
using MesWcfService.DB;
using CommonUtils.Logger;
using CommonUtils.DB;

namespace MesWcfService.MessageQueue.RemoteClient
{
    public class ProgrameVersion
    {
        public static string UpdateProgrameVersion(Queue<string[]> pvqueue)
        {
            try
            {
                string[] array = pvqueue.Dequeue();
                var typeNo = array[0];
                var stationName = array[1];
                var programePath = array[2];
                var programeName = array[3];
                var teamLeader = array[4];
                var admin = array[5];
                LogHelper.Log.Info(programePath);
                if (programePath.Contains("\\"))
                {
                    programePath = programePath.Replace("\\", "\\\\");
                    LogHelper.Log.Info(programePath);
                }
                var insertSQL = $"INSERT INTO {DbTable.F_TEST_PROGRAME_VERSION_NAME}(" +
                    $"{DbTable.F_TEST_PROGRAME_VERSION.TYPE_NO}," +
                    $"{DbTable.F_TEST_PROGRAME_VERSION.STATION_NAME}," +
                    $"{DbTable.F_TEST_PROGRAME_VERSION.PROGRAME_NAME}," +
                    $"{DbTable.F_TEST_PROGRAME_VERSION.PROGRAME_VERSION}," +
                    $"{DbTable.F_TEST_PROGRAME_VERSION.TEAM_LEADER}," +
                    $"{DbTable.F_TEST_PROGRAME_VERSION.ADMIN}," +
                    $"{DbTable.F_TEST_PROGRAME_VERSION.UPDATE_DATE}) VALUES(" +
                    $"'{typeNo}','{stationName}','{programePath}','{programeName}'," +
                    $"'{teamLeader}','{admin}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
                //if (IsExistVersion(typeNo, stationName, programeName, programeVersion, teamLeader, admin))
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

        private static bool IsExistVersion(string typeNo,string stationName,string programeName,string version,string teamLeader,string admin)
        {
            var selectSQL = $"SELECT * FROM {DbTable.F_TEST_PROGRAME_VERSION_NAME} WHERE " +
                $"{DbTable.F_TEST_PROGRAME_VERSION.TYPE_NO} = '{typeNo}' AND " +
                $"{DbTable.F_TEST_PROGRAME_VERSION.STATION_NAME} = '{stationName}' AND " +
                $"{DbTable.F_TEST_PROGRAME_VERSION.PROGRAME_NAME} = '{programeName}' AND " +
                $"{DbTable.F_TEST_PROGRAME_VERSION.PROGRAME_VERSION} = '{version}' AND " +
                $"{DbTable.F_TEST_PROGRAME_VERSION.TEAM_LEADER} = '{teamLeader}' AND " +
                $"{DbTable.F_TEST_PROGRAME_VERSION.ADMIN} = '{admin}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }
    }
}