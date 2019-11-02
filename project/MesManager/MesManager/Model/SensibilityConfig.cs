using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesManager.Model
{
    class SensibilityConfig
    {
        public const string PLCAddressKey = "PLC";
        public const string LocalAddressKey = "本机地址";
        public const string AvometerAddressKey = "万用表";
        public const string AutoSweepCodeKey = "自动扫描枪";
        public const string ProgrameControlPowerKey = "程控电源";
        public const string WorkElectricMinKey = "工作电流最小值";
        public const string WorkElectricMaxKey = "工作电流最大值";
        public const string PartNumberKey = "零件号";
        public const string HardWareVersionKey = "硬件版本号";
        public const string SoftWareVersionKey = "软件版本号";
        public const string DormantElectricMinKey = "休眠电流最小值";
        public const string DormantElectricMaxKey = "休眠电流最大值";
        public const string BootLoaderKey = "BootLoader";
        public const string PorterRateKey = "波特率";
        public const string SendCanIDKey = "诊断Send_CAN_ID";
        public const string ReceiveCanIDKey = "诊断Receive_CAN_ID";
        public const string CyclyCanIDKey = "周期CAN_ID";
        public const string RfCanIDKey = "RF_CAN_ID";
        public const string ProductSerialKey = "测试序列";

        public string PLCAddress { get; set; }

        public string LocalAddress { get; set; }

        public string AvometerAddress { get; set; }

        public string AutoSweepCode { get; set; }

        public string ProgrameControlPower { get; set; }

        public string WorkElectricMin { get; set; }

        public string WorkElectricMax { get; set; }

        public string PartNumber { get; set; }

        public string HardWareVersion { get; set; }

        public string SoftWareVersion { get; set; }

        public string DormantElectricMin { get; set; }

        public string DormantElectricMax { get; set; }

        public string BootLoader { get; set; }

        public string PorterRate { get; set; }

        public string SendCanID { get; set; }

        public string ReceiveCanID { get; set; }

        public string CyclyCanID { get; set; }

        public string RfCanID { get; set; }

        public string ProductSerial { get; set; }
    }
}
