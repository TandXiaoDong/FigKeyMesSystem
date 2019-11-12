using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonUtils.DB;
using CommonUtils.Logger;
using MesWcfService.DB;
using MesWcfService.Common;
using System.Data;

namespace MesWcfService.MessageQueue.RemoteClient
{
    public class AddBindingPCBA
    {
        public static string BindingPCBA(Queue<string[]> queue)
        {
            var array = queue.Dequeue();
            var sn_pcba = array[0].Trim();
            var sn_outter = array[1].Trim();
            var materialCode = array[2].Trim();
            var productTypeNo = array[3].Trim();
            if (sn_pcba == "")
            {
                sn_pcba = SelectPcba(sn_outter);
                if (sn_pcba == "")
                {
                    LogHelper.Log.Info("【PCBA绑定-PCBA编码传入为空，查询PCBA是否与外壳无绑定】继续查询是否包含外壳工站");
                    //查询本工艺是否不包含外壳装配工站
                    var stationDs = SelectStationList(productTypeNo);
                    if (stationDs.Tables.Count > 0)
                    {
                        var dataRowList = stationDs.Tables[0].Select($"{DbTable.F_TECHNOLOGICAL_PROCESS.STATION_NAME} = '外壳装配工站'");
                        if (dataRowList.Length > 0)
                        {
                            LogHelper.Log.Info("【PCBA绑定-查询是否包含外壳工站】包含外壳工站，绑定不能继续");
                            return "FAIL";
                        }
                        LogHelper.Log.Info("【PCBA绑定-查询是否包含外壳工站】不包含外壳工站，绑定可以继续");
                    }
                }
                LogHelper.Log.Info("【PCBA绑定-PCBA编码传入为空，查询PCBA是否与外壳已经绑定】pcba="+sn_pcba);
            }
            if (sn_outter == "")
            {
                LogHelper.Log.Info("【PCBA绑定-外壳编码传入为空】");
                return "FAIL";
            }
            if (materialCode == "")
            {
                LogHelper.Log.Info("【PCBA绑定-物料编码传入为空】");
                return "FAIL";
            }
            if (productTypeNo == "")
            {
                LogHelper.Log.Info("【PCBA绑定-产品型号传入为空】");
                return "FAIL";
            }
            if (IsExistPCBA(sn_pcba,sn_outter,materialCode,productTypeNo))
            {
                //update
                LogHelper.Log.Info("【更新PCBA绑定状态】针对已解绑再次恢复绑定的情况");
                var upDateSQL = $"UPDATE {DbTable.F_BINDING_PCBA_NAME} SET " +
                    $"{DbTable.F_BINDING_PCBA.BINDING_STATE} = '1' " +
                    $"WHERE " +
                    $"{DbTable.F_BINDING_PCBA.SN_PCBA} = '{sn_pcba}' " +
                    $"AND " +
                    $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{sn_outter}' " +
                    //$"AND " +
                    //$"{DbTable.F_BINDING_PCBA.MATERIAL_CODE} = '{materialCode}'" +
                    $"AND " +
                    $"{DbTable.F_BINDING_PCBA.PRODUCT_TYPE_NO} = '{productTypeNo}'";
                var row = SQLServer.ExecuteNonQuery(upDateSQL);
                if (row > 0)
                {
                    LogHelper.Log.Info("【PCB恢复绑定】成功");
                    return "OK";
                }
                LogHelper.Log.Info("【PCBA恢复绑定】失败"+upDateSQL);
                return "FAIL";
            }
            else
            {
                //insert
                //插入前先判断PCBA、外壳的状态
                var insertSQL = $"INSERT INTO {DbTable.F_BINDING_PCBA_NAME}(" +
                    $"{DbTable.F_BINDING_PCBA.SN_PCBA}," +
                    $"{DbTable.F_BINDING_PCBA.SN_OUTTER}," +
                    $"{DbTable.F_BINDING_PCBA.UPDATE_DATE}," +
                    $"{DbTable.F_BINDING_PCBA.MATERIAL_CODE}," +
                    $"{DbTable.F_BINDING_PCBA.PRODUCT_TYPE_NO}," +
                    $"{DbTable.F_BINDING_PCBA.PCBA_STATE}," +
                    $"{DbTable.F_BINDING_PCBA.OUTTER_STATE}" +
                    $") VALUES(" +
                    $"'{sn_pcba}','{sn_outter}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'," +
                    $"'{materialCode}','{productTypeNo}','{SelectPcbaState(sn_pcba)}','{SelectShellState(sn_outter)}')";
                int isRes = SQLServer.ExecuteNonQuery(insertSQL);
                if (isRes > 0)
                {
                    LogHelper.Log.Info("【PCB绑定成功】" + insertSQL);
                    return "OK";
                }
                else
                {
                    LogHelper.Log.Info("【PCB绑定失败】" + insertSQL);
                    return "FAIL";
                }
            }
        }

        public static DataSet SelectStationList(string processName)
        {
            string selectSQL = $"SELECT " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.STATION_ORDER}," +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.STATION_NAME}," +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.USER_NAME}," +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.UPDATE_DATE} " +
                $"FROM {DbTable.F_TECHNOLOGICAL_PROCESS_NAME} " +
                $"WHERE {DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME} = '{processName}' " +
                $"ORDER BY {DbTable.F_TECHNOLOGICAL_PROCESS.STATION_ORDER}";
            return SQLServer.ExecuteDataSet(selectSQL);
        }

        private static string SelectPcbaState(string snPCBA)
        {
            var selectSQL = $"SELECT top 1 {DbTable.F_BINDING_PCBA.PCBA_STATE} FROM " +
                $"{DbTable.F_BINDING_PCBA_NAME} " +
                $"WHERE " +
                $"{DbTable.F_BINDING_PCBA.SN_PCBA} = '{snPCBA}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();
            return "1";
        }

        private static string SelectShellState(string snOutter)
        {
            var selectSQL = $"SELECT top 1 {DbTable.F_BINDING_PCBA.OUTTER_STATE} FROM " +
                $"{DbTable.F_BINDING_PCBA_NAME} " +
                $"WHERE " +
                $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{snOutter}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();
            return "1";
        }

        private static bool IsExistPCBA(string snPcba,string snOutter,string materialCode,string productTypeNo)
        {
            var selectSQL = $"SELECT * FROM {DbTable.F_BINDING_PCBA_NAME} WHERE " +
                $"{DbTable.F_BINDING_PCBA.SN_PCBA} = '{snPcba}' AND " +
                $"{DbTable.F_BINDING_PCBA.SN_OUTTER}= '{snOutter}' AND " +
                $"{DbTable.F_BINDING_PCBA.MATERIAL_CODE} = '{materialCode}' AND " +
                $"{DbTable.F_BINDING_PCBA.PRODUCT_TYPE_NO} = '{productTypeNo}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }

        /// <summary>
        /// PCB与外壳一一对应，可能会出现更新外壳的情况
        /// </summary>
        /// <param name="snPcba"></param>
        /// <param name="snOutter"></param>
        /// <returns></returns>
        private static bool IsExistPCBAShell(string snPcba, string snOutter)
        {
            var selectSQL = $"SELECT * FROM {DbTable.F_BINDING_PCBA_NAME} WHERE " +
                $"{DbTable.F_BINDING_PCBA.SN_PCBA} = '{snPcba}' AND " +
                $"{DbTable.F_BINDING_PCBA.SN_OUTTER}= '{snOutter}' ";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }

        private static string SelectPcba(string snOutter)
        {
            var selectSQL = $"SELECT {DbTable.F_BINDING_PCBA.SN_PCBA} FROM {DbTable.F_BINDING_PCBA_NAME} WHERE " +
                $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{snOutter}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();
            return "";
        }
    }
}