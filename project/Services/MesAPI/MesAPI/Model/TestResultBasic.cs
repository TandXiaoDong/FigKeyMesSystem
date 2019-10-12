using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MesAPI.Model
{
    public class TestResultBasic
    {
        public string ProductSN { get; set; }
        public string ProductTypeNo { get; set; }
        public string StationName { get; set; }
        public string StationInDate { get; set; }
        public string StationOutDate { get; set; }
        public string TestResultValue { get; set; }
        public string UserTeamLeader { get; set; }
        public string UserAdmin { get; set; }
        public bool IsFinalResultPass { get; set; }
    }
}