using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MesAPI.Model
{
    public class CheckPackageProductHistory
    {
        public int CheckPackageCaseNumber { get; set; }

        public System.Data.DataSet CheckPackageCaseData { get; set; }
    }
}