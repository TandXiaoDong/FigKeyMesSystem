using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MesAPI.Model
{
    public class ProgramVersionHistory
    {
        public int ProgrameHistoryNumber { get; set; }

        public System.Data.DataSet ProgrameDataSet { get; set; }

        public string ProductTypeNo { get; set; }

        public string StationName { get; set; }

        public string ProgramePath { get; set; }

        public string ProgrameName { get; set; }

        public string TeamLeader { get; set; }

        public string Admin { get; set; }

        public string UpdateDate { get; set; }
    }
}