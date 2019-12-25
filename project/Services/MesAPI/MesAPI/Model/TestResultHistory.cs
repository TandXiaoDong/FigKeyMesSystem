using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MesAPI.Model
{
    public class TestResultHistory
    {
        public int TestResultNumber { get; set; }

        public System.Data.DataSet TestResultDataSet { get; set; }

        public string PcbaSN { get; set; }

        public string ProductTypeNo { get; set; }
    }
}