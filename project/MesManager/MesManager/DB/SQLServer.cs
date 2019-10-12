using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using CommonUtils.Logger;

namespace MesManager.DB
{
    class SQLServer
    {
        public SQLServer()
        {
            Init();
        }

        public static void Init()
        {
            SqlSugarClient();//Create db
            //DbContext();//Optimizing SqlSugarClient usage
            //SingletonPattern();//Singleten Pattern
            //DistributedTransactionExample();
            //MasterSlave();//Read-write separation 
            //CustomAttribute();
        }

        private static void SqlSugarClient()
        {
            //Create db
            LogHelper.Log.Info("#### SqlSugarClient Start ####");
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.SqlServer,
                ConnectionString = Config.ConnectionString,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
                AopEvents = new AopEvents
                {
                    OnLogExecuting = (sql, p) =>
                    {
                        LogHelper.Log.Info(sql);
                        LogHelper.Log.Info(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
                    }
                }
            });

            //If no exist create datebase 
            db.DbMaintenance.CreateDatabase();

            //Use db query
            var dt = db.Ado.GetDataTable("select 1");

            //Create tables
            db.CodeFirst.InitTables(typeof(OrderItem), typeof(Order));
            var id = db.Insertable(new Order() { Name = "order1", CustomId = 1, Price = 0, CreateTime = DateTime.Now }).ExecuteReturnIdentity();

            //Insert data
            db.Insertable(new OrderItem() { OrderId = id, Price = 0, CreateTime = DateTime.Now }).ExecuteCommand();
            LogHelper.Log.Info("#### SqlSugarClient End ####");

        }
    }
}
