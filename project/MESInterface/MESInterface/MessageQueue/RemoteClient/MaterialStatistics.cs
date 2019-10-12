using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonUtils.DB;
using MESInterface.DB;

namespace MESInterface.MessageQueue.RemoteClient
{
    public class MaterialStatistics
    {
        //物料统计
        //插入
        private static string GetDateTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static string InsertMaterialStatistics(Queue<string[]> queue)
        {
            string[] array = queue.Dequeue();
            string sn_inner = array[0];
            string sn_outter = array[1];
            string type_no = array[2];
            string station_name = array[3];
            string material_code = array[4];
            string material_amount = array[5];
            string insertSQL = $"INSERT INTO {DbTable.F_MATERIAL_STATISTICS_NAME}() " +
                $"VALUES('{sn_inner}','{sn_outter}','{type_no}','{station_name}','{material_code}','{material_amount}','{GetDateTime()}')";
            int row = SQLServer.ExecuteNonQuery(insertSQL);
            if (row > 0)
                return "1";
            return "0";
        }
    }
}