using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MesManager.CommonEnum;

namespace MesManager.Common
{
    class AirtageTestItem
    {
        public static List<string> AirtagePressureUnitItem()
        {
            List<string> pressureList = new List<string>();
            pressureList.Add(AirtagePressureUnitEnum.Pa.ToString());
            pressureList.Add(AirtagePressureUnitEnum.Bar.ToString());
            pressureList.Add(AirtagePressureUnitEnum.Kpa.ToString());
            pressureList.Add(AirtagePressureUnitEnum.PSI.ToString());
            return pressureList;
        }

        public static List<string> AirtageSpreadUnitItem()
        {
            List<string> spreadList = new List<string>();
            spreadList.Add(AirtageSpreadUnitEnum.Pa.ToString());
            spreadList.Add(AirtageSpreadUnitEnum.Pa_S.ToString().Replace("_","/"));
            return spreadList;
        }
    }
}
