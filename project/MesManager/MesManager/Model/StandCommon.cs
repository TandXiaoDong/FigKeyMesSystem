using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesManager.Model
{
    class StandCommon
    {
        public const string TurnStationConfigPath           = "E:\\StationConfig\\烧录工站\\";
        public const string TurnStationFWName = "FW\\";
        public const string SensibilityStationConfigPath    = "E:\\StationConfig\\灵敏度测试工站\\";
        public const string ShellStationConfigPath          = "E:\\StationConfig\\外壳装配工站\\";
        public const string AirtageStationConfigPath        = "E:\\StationConfig\\气密测试工站\\";
        public const string StentStationConfigPath          = "E:\\StationConfig\\支架装配工站\\";
        public const string ProductFinishStationConfigPath  = "E:\\StationConfig\\成品测试工站\\";
        public const string CheckProductStationConfigPath   = "E:\\StationConfig\\抽检工站\\";

        //不完整文件名称
        public const string TurnStationIniName = "烧录工站_";
        public const string SensibilityStationIniName = "灵敏度测试工站_";
        public const string ShellStationIniName = "外壳装配工站_";
        public const string AirtageStationIniName = "气密测试工站_";
        public const string StentStationIniName = "支架装配工站_";
        public const string ProductFinishStationIniName = "成品测试工站_";
        public const string CheckProductStationIniName = "抽检工站_";

        //公用配置
        public const string PCBABarCodeLengthKey = "新设置PCB条码长度位数";
        public const string ShellBarCodeLengthKey = "新设置外壳条码长度位数";
        public const string CaseBarCodeLengthKey = "箱子条码长度";
        public const string PackageCaseAmountKey = "包装箱数量";

        public string ProductTypeNo { get; set; }

        public List<string> ProductTypeNoList { get; set; }

        /// <summary>
        /// 所有最新不重复工站
        /// </summary>
        public List<string> StationList { get; set; }

        /// <summary>
        /// 配置文件路径
        /// </summary>
        public string StandConfigPath { get; set; }

        #region 要保存得机台共用配置

        public string PCBABarCodeLength { get; set; }

        public string ShellBarCodeLength { get; set; }

        public string CaseBarCodeLength { get; set; }

        public string PackageCaseAmount { get; set; }

        #endregion
    }
}
