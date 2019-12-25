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

        public string OutCaseCode { get; set; }

        public string ProductSN { get; set; }

        public string ProductTypeNo { get; set; }

        /// <summary>
        /// 1-已绑定；0-已解绑
        /// </summary>
        public string BindState { get; set; }
    }
}