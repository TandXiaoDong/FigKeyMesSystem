using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesManager.Model
{
    class CommConfig
    {
        public const string DeafaultConfigRoot = "config\\";
        public const string TestStandSerialNumber = "serialConfig.ini";
        public const string BurnStationSection = "烧录工站";
        public const string SensibilityStationSection = "灵敏度测试工站";
        public const string ShellStationSection = "外壳装配工站";
        public const string AirtageStationSection = "气密测试工站";
        public const string StentStationSection = "支架装配工站";
        public const string ProductFinishStationSection = "成品测试工站";
        public const string CheckProductStationSection = "抽检工站";
        public const string SerialNumberKey = "serialNumber_";
        public const string SerialVoltageKey = "serialVoltage_";
        public const string CountKey = "count";
    }
}
