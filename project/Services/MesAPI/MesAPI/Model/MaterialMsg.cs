using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace MesAPI.Model
{
    [Description("材料信息")]
    public class MaterialMsg
    {
        [Description("物料编码")]
        public string MaterialCode { get; set; }

        [Description("LOT")]
        public string MaterialLOT { get; set; }

        [Description("RID")]
        public string MaterialRID { get; set; }

        [Description("PN")]
        public string MaterialPN { get; set; }

        [Description("物料名称")]
        public string MaterialName { get; set; }

        [Description("操作用户")]
        public string UserName { get; set; }

        [Description("描述说明")]
        public string Describle { get; set; }

        [Description("修改日期")]
        public string UpdateDate { get; set; }

        [Description("执行结果")]
        public int Result { get; set; }

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