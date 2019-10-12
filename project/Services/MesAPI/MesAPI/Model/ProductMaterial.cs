using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MesAPI.Model
{
    public class ProductMaterial
    {
        public string TypeNo { get; set; }

        public string MaterialCode { get; set; }

        public string MaterialName { get; set; }

        public string Stock { get; set; }

        public string Describle { get; set; }

        public string UserName { get; set; }

        public string Update_Date { get; set; }

        public int Result { get; set; }
    }
}