using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesManager.Model
{
    class ProductCheckConfig
    {
        public const string PlcAddressKey = "PLC";
        public const string LocalAddressKey = "本机地址";
        public const string AvometerKey = "万用表";
        public const string TestBoardKey = "万通测试板";
        public const string ControlPowerKey = "程控电源";
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
        public const string CycleCanIDKey = "周期CAN_ID";
        public const string RF_CAN_IDKey = "RF_CAN_ID";
        public const string TestSerialKey = "测试序列";

        public string PlcAddress { get; set; }

        public string LocalAddress { get; set; }

        public string TestBoard { get; set; }

        public string ControlPower { get; set; }

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

        public string CycleCanID { get; set; }

        public string RF_CAN_ID { get; set; }

        public string TestSerial { get; set; }

        public string Avometer { get; set; }
    }
}
