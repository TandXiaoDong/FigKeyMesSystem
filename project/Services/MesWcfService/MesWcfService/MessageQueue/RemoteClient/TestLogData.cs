using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonUtils.DB;
using CommonUtils.Logger;
using MesWcfService.DB;

namespace MesWcfService.MessageQueue.RemoteClient
{

    public class TestLogData
    {
        public const string BurnStation = "烧录工站";
        public const string SensibilityStation = "灵敏度测试工站";
        public const string ShellStation = "外壳装配工站";
        public const string AirtageStation = "气密测试工站";
        public const string StentStation = "支架装配工站";
        public const string ProductTestStation = "成品测试工站";

        public const string Turn_TurnItem = "烧录";
        public const string Turn_Voltage_12V_Item = "13.5V电压测试";
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
        public const string Product_Inspect_Result = "目检结果";
        public const string Product_InspectItem = "目检";

        private static List<string> BurnTestItemList()
        {
            List<string> testItemList = new List<string>();
            testItemList.Add(Turn_TurnItem);
            testItemList.Add(Turn_Voltage_12V_Item);
            testItemList.Add(Turn_Voltage_5V_Item);
            testItemList.Add(Turn_Voltage_33_1V_Item);
            testItemList.Add(Turn_Voltage_33_2V_Item);
            testItemList.Add(Turn_SoftVersion);
            return testItemList;
        }

        private static List<string> SensibilityTestItemList()
        {
            List<string> testItemList = new List<string>();
            testItemList.Add(Sen_Work_Electric_Test);
            testItemList.Add(Sen_PartNumber);
            testItemList.Add(Sen_HardWareVersion);
            testItemList.Add(Sen_SoftVersion);
            testItemList.Add(Sen_ECUID);
            testItemList.Add(Sen_BootloaderVersion);
            testItemList.Add(Sen_RadioFreq);
            testItemList.Add(Sen_DormantElect);
            return testItemList;
        }

        private static List<string> ShellTestItemList()
        {
            List<string> testItemList = new List<string>();
            testItemList.Add(Shell_FrontCover);
            testItemList.Add(Shell_BackCover);
            testItemList.Add(Shell_PCBScrew1);
            testItemList.Add(Shell_PCBScrew2);
            testItemList.Add(Shell_PCBScrew3);
            testItemList.Add(Shell_PCBScrew4);
            testItemList.Add(Shell_ShellScrew1);
            testItemList.Add(Shell_ShellScrew2);
            testItemList.Add(Shell_ShellScrew3);
            testItemList.Add(Shell_ShellScrew4);
            return testItemList;
        }

        private static List<string> AirtageTestItemList()
        {
            List<string> testItemList = new List<string>();
            testItemList.Add(Air_AirtightTest);
            return testItemList;
        }

        private static List<string> StentTestItemList()
        {
            List<string> testItemList = new List<string>();
            testItemList.Add(Stent_Screw1);
            testItemList.Add(Stent_Screw2);
            testItemList.Add(Stent_Stent);
            testItemList.Add(Stent_LeftStent);
            testItemList.Add(Stent_RightStent);
            return testItemList;
        }

        private static List<string> ProductTestItemList()
        {
            List<string> testItemList = new List<string>();
            testItemList.Add(Product_Work_Electric_Test);
            testItemList.Add(Product_DormantElect);
            testItemList.Add(Product_Inspect_Result);
            testItemList.Add(Product_InspectItem);
            return testItemList;
        }

        /*
         *将测试项log插入到测试结果表
         * 1）查询出进站时间
         * 2）插入测试项，包含进站时间
         * 3）更新出站结果
         * 
         * 
         */
        public static string UpdateTestLogData(Queue<string[]> tlQueue)
        {
            try
            {//typeNo,stationName,productSN,testItem,limit,currentValue,testResult,teamLeader,admin
                string[] array = tlQueue.Dequeue();
                var typeNo = array[0];
                var stationName = array[1];
                var productSN = array[2];
                var testItem = array[3];
                var limit = array[4];
                var currentValue = array[5];
                var testResult = array[6];
                var teamLeader = array[7];
                var admin = array[8];
                var joinDateTime = array[9];

                if (stationName == BurnStation)
                {
                    if (!BurnTestItemList().Contains(testItem))
                        return "OK";
                }
                else if (stationName == SensibilityStation)
                {
                    if (!SensibilityTestItemList().Contains(testItem))
                    {
                        return "OK";
                    }
                }
                else if (stationName == ShellStation)
                {
                    if (!ShellTestItemList().Contains(testItem))
                        return "OK";
                }
                else if (stationName == StentStation)
                {
                    if (!StentTestItemList().Contains(testItem))
                        return "OK";
                }
                else if (stationName == AirtageStation)
                {
                    if (!AirtageTestItemList().Contains(testItem))
                        return "OK";
                }
                else if (stationName == ProductTestStation)
                {
                    if (!ProductTestItemList().Contains(testItem))
                        return "OK";
                }
                var insertSQL = $"INSERT INTO {DbTable.F_TEST_LOG_DATA_NAME}(" +
                    $"{DbTable.F_TEST_LOG_DATA.TYPE_NO}," +
                    $"{DbTable.F_TEST_LOG_DATA.STATION_NAME}," +
                    $"{DbTable.F_TEST_LOG_DATA.PRODUCT_SN}," +
                    $"{DbTable.F_TEST_LOG_DATA.TEST_ITEM}," +
                    $"{DbTable.F_TEST_LOG_DATA.LIMIT}," +
                    $"{DbTable.F_TEST_LOG_DATA.CURRENT_VALUE}," +
                    $"{DbTable.F_TEST_LOG_DATA.TEST_RESULT}," +
                    $"{DbTable.F_TEST_LOG_DATA.TEAM_LEADER}," +
                    $"{DbTable.F_TEST_LOG_DATA.ADMIN}," +
                    $"{DbTable.F_TEST_LOG_DATA.UPDATE_DATE}," +
                    $"{DbTable.F_TEST_LOG_DATA.JOIN_DATE_TIME}) VALUES(" +
                    $"'{typeNo}','{stationName}','{productSN}','{testItem}','{limit}'," +
                    $"'{currentValue}','{testResult}','{teamLeader}','{admin}'," +
                    $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','{joinDateTime}')";
                //if (IsExistLogData(array))
                //    return "OK";
                var res = SQLServer.ExecuteNonQuery(insertSQL);
                if (res > 0)
                    return "OK";
                else
                {
                    LogHelper.Log.Info(insertSQL);
                    return "FAIL";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error(ex.Message);
                return "ERROR";
            }
        }

        private static bool IsExistLogData(string[] logArray)
        {
            var selectSQL = $"SELECT * FROM {DbTable.F_TEST_LOG_DATA_NAME} WHERE " +
                $"{DbTable.F_TEST_LOG_DATA.TYPE_NO} = '{logArray[0]}' AND " +
                $"{DbTable.F_TEST_LOG_DATA.STATION_NAME} = '{logArray[1]}' AND " +
                $"{DbTable.F_TEST_LOG_DATA.PRODUCT_SN} = '{logArray[2]}' AND " +
                $"{DbTable.F_TEST_LOG_DATA.TEST_ITEM} = '{logArray[3]}' AND " +
                $"{DbTable.F_TEST_LOG_DATA.LIMIT} = '{logArray[4]}' AND " +
                $"{DbTable.F_TEST_LOG_DATA.CURRENT_VALUE} = '{logArray[5]}' AND " +
                $"{DbTable.F_TEST_LOG_DATA.TEST_RESULT} = '{logArray[6]}' AND " +
                $"{DbTable.F_TEST_LOG_DATA.TEAM_LEADER} = '{logArray[7]}' AND " +
                $"{DbTable.F_TEST_LOG_DATA.ADMIN} = '{logArray[8]}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }
    }
}