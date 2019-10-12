using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesManager.Common
{
    class AnalysisMaterialCode
    {
        //A19083100008&S2.118&1.2.11.111&20&20190831&1T20190831001
        //RID & &PN & QTY$DC & LOT
        public string MaterialRID { get; set; }
        public string MaterialSID { get; set; }
        public string MaterialPN { get; set; }
        public string MaterialQTY { get; set; }
        public string MaterialDC { get; set; }
        public string MaterialLOT { get; set; }

        public static AnalysisMaterialCode GetMaterialDetail(string materialCode)
        {
            AnalysisMaterialCode analysisMaterialCode = new AnalysisMaterialCode();
            analysisMaterialCode.MaterialRID = materialCode.Substring(0,materialCode.IndexOf('&'));
            materialCode = materialCode.Substring(materialCode.IndexOf('&') + 1);
            analysisMaterialCode.MaterialSID = materialCode.Substring(0,materialCode.IndexOf('&'));
            materialCode = materialCode.Substring(materialCode.IndexOf('&') + 1);
            analysisMaterialCode.MaterialPN = materialCode.Substring(0,materialCode.IndexOf('&'));
            materialCode = materialCode.Substring(materialCode.IndexOf('&') + 1);
            analysisMaterialCode.MaterialQTY = materialCode.Substring(0,materialCode.IndexOf('&'));
            materialCode = materialCode.Substring(materialCode.IndexOf('&') + 1);
            analysisMaterialCode.MaterialDC = materialCode.Substring(0,materialCode.IndexOf('&'));
            materialCode = materialCode.Substring(materialCode.IndexOf('&') + 1);
            analysisMaterialCode.MaterialLOT = materialCode;
            return analysisMaterialCode;
        }

        public static string GetMaterialPN(string materialCode)
        {
            //A19083100008&S2.118&1.2.11.111&20&20190831&1T20190831001
            //RID & &PN & QTY$DC & LOT
            materialCode = materialCode.Substring(materialCode.IndexOf('&') + 1);
            materialCode = materialCode.Substring(materialCode.IndexOf('&') + 1);
            materialCode = materialCode.Substring(0, materialCode.IndexOf('&'));
            return materialCode;
        }
    }
}
