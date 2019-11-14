using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesManager.Model
{
    class AirtageConfig
    {
        public const string LocalAddressConMesKey = "本机地址（连接MES）";
        public const string AirTesterKey = "气密测试仪";
        public const string InflateAirTimeKey = "充气时间";
        public const string StableTimeKey = "稳定时间";
        public const string TestTimeKey = "测试时间";
        public const string PressureUnitKey = "压力单位";
        public const string SpreadUnitKey = "泄露单位";
        public const string MaxInflateKey = "最大充气";
        public const string MinInflateKey = "最小充气";
        public const string BigLeakMaxKey = "大漏最大值";
        public const string BigLeakMinKey = "大漏最小值";
        public const string TestConditionValueKey = "测试条件允收值";
        public const string ReferenceConditionValueKey = "参考条件允收值";
        public const string TestSerialKey = "测试序列";


        /// <summary>
        /// 测试序列绝对路径,获取的实际值
        /// </summary>
        public string ProductSerialPath { get; set; }

        /// <summary>
        /// 测试序列名称（路径中的名称），可变
        /// 保存时为序列绝对路径，显示时为序列名
        /// </summary>
        public string TestSerial { get; set; }

        /// <summary>
        /// 该序列号的供电电压，用于提醒用户
        /// </summary>
        public string SupplyVoltage { get; set; }


        public string LocalAddressConMes { get; set; }

        public string AirTester { get; set; }

        public string InflateAirTime { get; set; }

        public string StableTime { get; set; }

        public string TestTime { get; set; }

        public string PressureUnit { get; set; }

        public string SpreadUnit { get; set; }

        public string MaxInflate { get; set; }

        public string MinInflate { get; set; }

        public string BigLeakMax { get; set; }

        public string BigLeakMin { get; set; }

        public string TestConditionValue { get; set; }

        public string ReferenceConditionValue { get; set; }
    }
}
