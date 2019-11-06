using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesManager.Model
{
    class StentConfig
    {
        public const string LocalAddressConMesKey = "本机地址（连接MES）";
        public const string TestSerialKey = "测试序列";
        public const string LeftStentKey = "左支架";
        public const string RightStentKey = "右支架";
        public const string StentKey = "支架";
        public const string UnionStentKey = "连体支架";
        public const string StentSrcrewKey = "支架螺丝";
        public const string StentNutKey = "支架螺母";

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

        public string LeftStent { get; set; }

        public string RightStent { get; set; }

        public string Stent { get; set; }

        public string UnionStent { get; set; }

        public string StentScrew { get; set; }

        public string StentNut { get; set; }
    }
}
