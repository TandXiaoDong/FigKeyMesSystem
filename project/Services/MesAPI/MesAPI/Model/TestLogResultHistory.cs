using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MesAPI.Model
{
    public class TestLogResultHistory
    {
        public int HistoryLogNumber { get; set; }

        public System.Data.DataSet HistoryDataSet { get; set; }

        public string ProcessName { get; set; }

        public string PcbaSN { get; set; }

        public string ProductSN { get; set; }

        public string StationName { get; set; }

        public string StationInDate { get; set; }

        public string JoinDateTime { get; set; }
    }
}