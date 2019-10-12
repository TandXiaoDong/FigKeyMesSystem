using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesManager.DB
{
    class Config
    {
        /// <summary>
        /// Account have permission to create database
        /// 用有建库权限的数据库账号
        /// </summary>
        /// //<add name="oledbSqlServer" connectionString="Provider=SQLOLEDB.1;Initial Catalog=prot;Data Source=123.123.123.250;User ID=hisfyj;Password=fyjhis"/>
        //Data Source={0};Integrated Security=True;Initial Catalog={3}";
        public static string ConnectionString = "Initial Catalog=WT_SCL;Data Source=localhost;User ID=sa;Password=az13132323251..;Integrated Security=True";
        /// <summary>
        /// Account have permission to create database
        /// 用有建库权限的数据库账号
        /// </summary>
        public static string ConnectionString2 = "server=.;uid=sa;pwd=haosql;database=SQLSUGAR4XTEST2";
        /// <summary>
        /// Account have permission to create database
        /// 用有建库权限的数据库账号
        /// </summary>
        public static string ConnectionString3 = "server=.;uid=sa;pwd=haosql;database=SQLSUGAR4XTEST3";
    }
}
