using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MesWcfService.Common
{
    public class CheckMaterialCode
    {
        public static bool IsRightMaterialCode(string materialCode)
        {
            if (materialCode.Contains("&"))
            {
                var materialRID = materialCode.Substring(0, materialCode.IndexOf('&'));
                materialCode = materialCode.Substring(materialCode.IndexOf('&') + 1);
                if (materialCode.Length <= 1)
                    return false;
            }
            if (materialCode.Contains("&"))
            {
                var materialSID = materialCode.Substring(0, materialCode.IndexOf('&'));
                materialCode = materialCode.Substring(materialCode.IndexOf('&') + 1);
                if (materialCode.Length <= 1)
                    return false;
            }
            if (materialCode.Contains("&"))
            {
                var materialPN = materialCode.Substring(0, materialCode.IndexOf('&'));
                materialCode = materialCode.Substring(materialCode.IndexOf('&') + 1);
                if (materialCode.Length <= 1)
                    return false;
            }
            if (materialCode.Contains("&"))
            {
                var materialQTY = materialCode.Substring(0, materialCode.IndexOf('&'));
                materialCode = materialCode.Substring(materialCode.IndexOf('&') + 1);
                if (materialCode.Length <= 1)
                    return false;
            }
            if (materialCode.Contains("&"))
            {
                var materialDC = materialCode.Substring(0, materialCode.IndexOf('&'));
                materialCode = materialCode.Substring(materialCode.IndexOf('&') + 1);
                if (materialCode.Length <= 1)
                    return false;
            }
            if (materialCode.Length == 13)
            {
                return true;
            }
            return false;
        }
    }
}