using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MesAPI.Model
{
    public class Station
    {
        public string ProcessName { get; set; }
        public int StationID { get; set; }
        public string StationName { get; set; }
        public string UserName { get; set; }
        public int ProcessState { get; set; }
    }
}