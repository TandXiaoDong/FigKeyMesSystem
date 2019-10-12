using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonUtils.DB;
using CommonUtils.Logger;
using MesWcfService.DB;
using MesWcfService.Common;

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
                    LogHelper.Log.Info("【PCBA绑定-PCBA编码传入为空，查询PCBA是否与外壳无绑定】");
                    return "FAIL"; 
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

            var upDateSQL = $"UPDATE {DbTable.F_BINDING_PCBA_NAME} SET " +
                $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{sn_outter}' WHERE " +
                $"{DbTable.F_BINDING_PCBA.SN_PCBA} = '{sn_pcba}'";

            if (IsExistPCBA(sn_pcba,sn_outter,materialCode,productTypeNo))
            {
                //update
                //暂时不考虑更新外壳编码
                return "OK";
            }
            else
            {
                //insert
                var insertSQL = $"INSERT INTO {DbTable.F_BINDING_PCBA_NAME}(" +
                    $"{DbTable.F_BINDING_PCBA.SN_PCBA}," +
                    $"{DbTable.F_BINDING_PCBA.SN_OUTTER}," +
                    $"{DbTable.F_BINDING_PCBA.UPDATE_DATE}," +
                    $"{DbTable.F_BINDING_PCBA.MATERIAL_CODE}," +
                    $"{DbTable.F_BINDING_PCBA.PRODUCT_TYPE_NO}) VALUES(" +
                    $"'{sn_pcba}','{sn_outter}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','{materialCode}','{productTypeNo}')";
                int isRes = SQLServer.ExecuteNonQuery(insertSQL);
                if (isRes > 0)
                    return "OK";
                else
                {
                    LogHelper.Log.Info("【PCB绑定失败】" + insertSQL);
                    return "FAIL";
                }
            }
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