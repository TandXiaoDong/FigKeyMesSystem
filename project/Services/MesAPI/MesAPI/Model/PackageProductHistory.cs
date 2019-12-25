using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MesAPI.Model
{
    public class PackageProductHistory
    {
        public int PackageCaseNumber { get; set; }

        public System.Data.DataSet PackageCaseData { get; set; }

        public string OutCaseCode { get; set; }

        public string ProductTypeNo { get; set; }

        /// <summary>
        /// 1-已绑定；0-已解绑
        /// </summary>
        public string BindState { get; set; }
    }
}