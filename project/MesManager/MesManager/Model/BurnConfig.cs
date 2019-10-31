using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesManager.Model
{
    public class BurnConfig
    {
        #region 参数常量--键
        public const string PowerValueKey = "电源";
        public const string LocalAddressKey = "本机地址";
        public const string AvometerAddressKey = "万用表";
        public const string AutoSweepCodeComKey = "自动扫码枪";
        public const string BurnerKey = "烧录器";
        public const string FirstVoltageMaxKey = "第一个电压测试点最大值";
        public const string FirstVoltageMinKey = "第一个电压测试点最小值";
        public const string SecondVoltageMaxKey = "第二个电压测试点最大值";
        public const string SecondVoltageMinKey = "第二个电压测试点最小值";
        public const string HardWareVersionKey = "硬件版本";
        public const string SoftWareVersionKey = "软件版本";
        public const string PartNumberKey = "零件号";
        public const string ProgramePathKey = "烧录程序路径";
        public const string ProgrameNameKey = "烧录程序名称";
        public const string PorterRateKey = "配置波特率";
        public const string CanIdKey = "配置CAN ID";
        public const string ProductIdKey = "配置产品ID";
        public const string SerialNumberKey = "测试序列";
        #endregion

        //配置参数值

        public string PowerValue { get; set; }

        public string LocalAddress { get; set; }

        public string AvometerAddress { get; set; }

        public string AutoSweepCodeCom { get; set; }

        public string Burner { get; set; }

        public string FirstVoltageMax { get; set; }

        public string FirstVoltageMin { get; set; }

        public string SecondVoltageMax { get; set; }

        public string SecondVoltageMin { get; set; }

        public string HardWareVersion { get; set; }

        public string SoftWareVersion { get; set; }

        public string PartNumber { get; set; }

        public string ProgramePath { get; set; }

        public string ProgrameName { get; set; }

        public string PorterRate { get; set; }

        public string CanId { get; set; }

        public string ProductId { get; set; }

        public string SerialNumber { get; set; }


    }
}
