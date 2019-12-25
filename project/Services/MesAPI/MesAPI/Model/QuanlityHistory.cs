using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MesAPI.Model
{
    public class QuanlityHistory
    {
        public int HistoryNumber { get; set; }

        public System.Data.DataSet QuanlityHistoryData { get; set; }

        public string MaterialCode { get; set; }
    }
}