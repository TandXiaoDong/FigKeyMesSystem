using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace MesWcfService.Model
{
    public class PCBABindHistory
    {
        public int BindNumber { get; set; }

        public DataSet BindHistoryData { get; set; }
    }
}