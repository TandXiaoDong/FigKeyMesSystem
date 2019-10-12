using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace MESInterface.Model
{
    [Description("材料信息")]
    public class MaterialMsg
    {
        [Description("物料编码")]
        public string MaterialCode { get; set; }

        [Description("物料数量")]
        public int MaterialAmount { get; set; }
    }
}