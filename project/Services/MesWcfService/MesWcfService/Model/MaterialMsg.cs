using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace MesWcfService.Model
{
    [Description("材料信息")]
    public class MaterialMsg
    {
        [Description("物料编码")]
        public string MaterialCode { get; set; }

        [Description("物料数量")]
        public int MaterialAmount { get; set; }

        [Description("内壳码")]
        public string Sn_Inner { get; set; }

        [Description("外壳码")]
        public string Sn_Outter { get; set; }

        [Description("产品型号")]
        public string TypeNo { get; set; }

        [Description("站位名称")]
        public string StationName { get; set; }
    }
}