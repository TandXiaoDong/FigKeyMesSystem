using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MesWcfService.Model
{
    public class PackageProduct
    {
        public string CaseCode { get; set; }

        public string SnOutter { get; set; }

        public string TypeNo { get; set; }

        public byte[] Picture { get; set; }

        public int BindingState { get; set; }

        public string BindingDate { get; set; }

        public string Remark { get; set; }
    }
}