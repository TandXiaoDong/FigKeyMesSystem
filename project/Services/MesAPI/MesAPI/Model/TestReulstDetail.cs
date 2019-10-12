using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MesAPI.Model
{
    public class TestResultItemContent
    {
        public const string Turn_TurnItem = "烧录";
        public const string Turn_Voltage_12V_Item = "12V电压测试";
        public const string Turn_Voltage_5V_Item = "5V电压测试";
        public const string Turn_Voltage_33_1V_Item = "3.3V电压测试点-1";
        public const string Turn_Voltage_33_2V_Item = "3.3V电压测试点-2";
        public const string Turn_SoftVersion = "软件版本";

        public const string Sen_Work_Electric_Test = "工作电流";
        public const string Sen_PartNumber = "零件号";
        public const string Sen_HardWareVersion = "硬件版本";
        public const string Sen_SoftVersion = "软件版本";
        public const string Sen_ECUID = "ECU ID";
        public const string Sen_BootloaderVersion = "报文Bootloader版本号";
        public const string Sen_RadioFreq = "射频测试";
        public const string Sen_DormantElect = "休眠电流";

        public const string Shell_FrontCover = "前盖组装";
        public const string Shell_BackCover = "后盖组装";
        public const string Shell_PCBScrew1 = "PCB螺丝1";
        public const string Shell_PCBScrew2 = "PCB螺丝2";
        public const string Shell_PCBScrew3 = "PCB螺丝3";
        public const string Shell_PCBScrew4 = "PCB螺丝4";
        public const string Shell_ShellScrew1 = "外壳螺丝1";
        public const string Shell_ShellScrew2 = "外壳螺丝2";
        public const string Shell_ShellScrew3 = "外壳螺丝3";
        public const string Shell_ShellScrew4 = "外壳螺丝4";

        public const string Air_AirtightTest = "气密测试";

        public const string Stent_Screw1 = "支架螺丝1";
        public const string Stent_Screw2 = "支架螺丝2";
        public const string Stent_Stent = "支架";
        public const string Stent_LeftStent = "左支架";
        public const string Stent_RightStent = "右支架";

        public const string Product_Work_Electric_Test = "工作电流";
        public const string Product_DormantElect = "休眠电流";

        public const string Order = "序号";
        public const string PcbaSN = "PCBA_SN";
        public const string ProductSN = "成品SN";
        public const string ProductTypeNo = "产品型号";
        public const string FinalResultValue = "最终结果";
        ////烧录工位/灵敏度工位/外壳工位/气密工位/支架装配工位/成品测试工位
        //烧录
        public const string StationName_turn = "工位名称";
        public const string StationInDate_turn = "进站时间";
        public const string StationOutDate_turn = "出站时间";
        public const string TestResultValue_turn = "工位结果";
        public const string UserTeamLeader_turn = "操作员";
        public const string UserAdmin_turn = "管理员";
        //灵敏度
        public const string StationName_sen = "工位名称";
        public const string StationInDate_sen = "进站时间";
        public const string StationOutDate_sen = "出站时间";
        public const string TestResultValue_sen = "工位结果";
        public const string UserTeamLeader_sen = "操作员";
        public const string UserAdmin_sen = "管理员";
        //外壳
        public const string StationName_shell = "工位名称";
        public const string StationInDate_shell = "进站时间";
        public const string StationOutDate_shell = "出站时间";
        public const string TestResultValue_shell = "工位结果";
        public const string UserTeamLeader_shell = "操作员";
        public const string UserAdmin_shell = "管理员";
        //气密
        public const string StationName_air = "工位名称";
        public const string StationInDate_air = "进站时间";
        public const string StationOutDate_air = "出站时间";
        public const string TestResultValue_air = "工位结果";
        public const string UserTeamLeader_air = "操作员";
        public const string UserAdmin_air = "管理员";

        //支架
        public const string StationName_stent = "工位名称";
        public const string StationInDate_stent = "进站时间";
        public const string StationOutDate_stent = "出站时间";
        public const string TestResultValue_stent = "工位结果";
        public const string UserTeamLeader_stent = "操作员";
        public const string UserAdmin_stent = "管理员";

        //成品
        public const string StationName_product = "工位名称";
        public const string StationInDate_product = "进站时间";
        public const string StationOutDate_product = "出站时间";
        public const string TestResultValue_product = "工位结果";
        public const string UserTeamLeader_product = "操作员";
        public const string UserAdmin_product = "管理员";


    }
    public class TestReulstDetail
    {
        #region basicParams
        public int Order { get; set; }
        public string PcbaSn { get; set; }
        public string ProductSn { get; set; }
        public string ProductTypeNo { get; set; }
        public string FinalResultValue { get; set; }
        public string StationName { get; set; }
        public string StationInDate { get; set; }
        public string StationOutDate { get; set; }
        public string TestResultValue { get; set; }
        public string UserTeamLeader { get; set; }
        public string UserAdmin { get; set; }
        #endregion

        //各个工位测试项
        public string BurnItem { get; set; }
        public string Voltage12VItem { get; set; }
        public string Voltage5VItem { get; set; }
        public string Voltage_33_1V_Item { get; set; }
        public string Voltage_33_2V_Item { get; set; }
        public string MainSoftVersion { get; set; }
        public string WorkElectricTest { get; set; }

        public string PartNumber { get; set; }
        public string HardWareVersion { get; set; }
        public string SoftVersion { get; set; }
        public string ECUID { get; set; }
        public string BootloaderVersion { get; set; }
        public string RadioFreq { get; set; }
        public string DormantElect { get; set; }
        public string FrontCover { get; set; }
        public string BackCover { get; set; }
        public string PCBScrew1 { get; set; }
        public string PCBScrew2 { get; set; }
        public string PCBScrew3 { get; set; }
        public string PCBScrew4 { get; set; }
        public string ShellScrew1 { get; set; }
        public string ShellScrew2 { get; set; }
        public string ShellScrew3 { get; set; }
        public string ShellScrew4 { get; set; }
        public string AirtightTest { get; set; }

        public string StentScrew1 { get; set; }
        public string StentScrew2 { get; set; }
        public string Stent { get; set; }
        public string LeftStent { get; set; }
        public string RightStent { get; set; }
    }
}