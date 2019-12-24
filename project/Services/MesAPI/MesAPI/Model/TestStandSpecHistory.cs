using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MesAPI.Model
{
    public class TestStandSpecHistory
    {
        public int SpecHistoryNumber { get; set; }

        public System.Data.DataSet SpecDataSet { get; set; }

        //查询/删除条件
        public string ProductTypeNo { get; set; }

        public string StationName { get; set; }

        public string TestItem { get; set; }

        public string LimitValue { get; set; }

        public string TeamLeader { get; set; }

        public string Admin { get; set; }

        public string UpdateDate { get; set; }
    }
}