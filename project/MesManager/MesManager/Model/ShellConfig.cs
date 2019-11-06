using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesManager.Model
{
    class ShellConfig
    {
        public const string LocalAddressConMesKey = "本机地址（连接MES）";
        public const string LocalAddressConPLCKey = "本机地址（连接PLC）";
        public const string PLCAddressKey = "PLC IP地址";
        public const string SmallScrewSetTimeKey = "小螺丝枪不计数时间设定";
        public const string LargeScrewSetTimeKey = "大螺丝枪不计数时间设定";
        public const string TestSerialNumberKey = "测试序列";
        public const string FrontCoverKey = "前盖";
        public const string BackCoverKey = "后盖";
        public const string PCBScrewKey = "PCB螺丝";
        public const string ShellScrewKey = "外壳螺丝";
        public const string TopCoverKey = "上盖";
        public const string ShellKey = "外壳";
        public const string SealRingWireKey = "密封圈和线束";
        public const string BubbleCottonKey = "泡棉";

        /// <summary>
        /// 测试序列绝对路径,获取的实际值
        /// </summary>
        public string ProductSerialPath { get; set; }

        /// <summary>
        /// 测试序列名称（路径中的名称），可变
        /// 保存时为序列绝对路径，显示时为序列名
        /// </summary>
        public string TestSerialNumber { get; set; }

        /// <summary>
        /// 该序列号的供电电压，用于提醒用户
        /// </summary>
        public string SupplyVoltage { get; set; }

        public string LocalAddressConMes { get; set; }

        public string LocalAddressConPLC { get; set; }

        public string PLCAddress { get; set; }

        public string SmallScrewSetTime { get; set; }

        public string LargeScrewSetTime { get; set; }

        public string FrontCover { get; set; }

        public string BackCover { get; set; }

        public string PCBScrew { get; set; }

        public string ShellScrew { get; set; }

        public string TopCover { get; set; }

        public string Shell { get; set; }

        public string SealRingWire { get; set; }

        public string BubbleCotton { get; set; }
    }
}
