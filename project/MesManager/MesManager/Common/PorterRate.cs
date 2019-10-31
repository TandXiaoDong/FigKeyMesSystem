using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MesManager.CommonEnum;

namespace MesManager.Common
{
    public class PorterRate
    {
        public static List<string> PorterRateDataSource()
        {
            List<string> porterRateList = new List<string>();
            porterRateList.Add(PorterEnumToString(PorterRateEnum.porter_5Kbps.ToString()));
            porterRateList.Add(PorterEnumToString(PorterRateEnum.porter_10Kbps.ToString()));
            porterRateList.Add(PorterEnumToString(PorterRateEnum.porter_20Kbps.ToString()));
            porterRateList.Add(PorterEnumToString(PorterRateEnum.porter_40Kbps.ToString()));
            porterRateList.Add(PorterEnumToString(PorterRateEnum.porter_50Kbps.ToString()));
            porterRateList.Add(PorterEnumToString(PorterRateEnum.porter_80Kbps.ToString()));
            porterRateList.Add(PorterEnumToString(PorterRateEnum.porter_100Kbps.ToString()));
            porterRateList.Add(PorterEnumToString(PorterRateEnum.porter_125Kbps.ToString()));
            porterRateList.Add(PorterEnumToString(PorterRateEnum.porter_200Kbps.ToString()));
            porterRateList.Add(PorterEnumToString(PorterRateEnum.porter_250Kbps.ToString()));
            porterRateList.Add(PorterEnumToString(PorterRateEnum.porter_400Kbps.ToString()));
            porterRateList.Add(PorterEnumToString(PorterRateEnum.porter_500Kbps.ToString()));
            porterRateList.Add(PorterEnumToString(PorterRateEnum.porter_666Kbps.ToString()));
            porterRateList.Add(PorterEnumToString(PorterRateEnum.porter_1000Kbps.ToString()));
            return porterRateList;
        }

        private static string PorterEnumToString(string porterRate)
        {
            return porterRate.Substring(porterRate.LastIndexOf('_') + 1);
        }
    }
}
