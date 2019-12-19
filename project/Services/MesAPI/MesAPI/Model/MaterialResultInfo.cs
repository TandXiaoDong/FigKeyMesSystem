using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace MesAPI.Model
{
    public class MaterialResultInfo
    {
        public int MaterialRowCount { get; set; }

        public DataSet MaterialResultData { get; set; }
    }
}