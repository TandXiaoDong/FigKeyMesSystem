using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonUtils.DB;
using CommonUtils.Logger;
using MesWcfService.DB;
using System.Data;

namespace MesWcfService.MessageQueue.RemoteClient
{

    public class TestLogData
    {
        public PcbaTestStationInStatusEnum pcbaTestStationInStatus;
        public string pcbaID = "";
        private string testItem, limit, currentValue, testResult;

        //进站-出站/进站-未出站/未进站
        public enum PcbaTestStationInStatusEnum
        {
            none,
            stationIn_not_stationOut,
            staionIn_stationOut,
            not_stationIn
        }

        #region testItem name
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
        public const string Product_Inspect_Result = "目检";
        public const string Product_InspectItem = "目检";
        #endregion

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
                {
                    UpdateTestLogHistory(productSN,typeNo,stationName,testItem,limit,currentValue,testResult);
                    return "OK";
                }
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

        private static void UpdateTestLogHistory(string sn, string typeNo, string station, string testItem, string limit,
            string curValue, string testResult)
        {
            //根据pcbaSN 与产品型号/ 工站名称 查询是否已经进站，并且未出站，此时，更新测试项，并更新最终结果
            //否则，未进站--不更新测试项与结果；进站但已经更新测试项与结果--进站失败 - 不能修改以前的进站记录
            //testItem+limit+currentValue+testItemResult 逗号隔开
            var updateSQL = "";
            var testItemString = testItem + "," + limit + "," + curValue + "," + testResult;
            var testLog = IsExistStationInRecord(sn, typeNo, station);
            if (station == "烧录工站")
            {
                #region testItem type
                if (testItem == Turn_TurnItem)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.burnItem_burn} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Turn_SoftVersion)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.burnItem_softVersion} = '{testItemString}'  WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Turn_Voltage_12V_Item)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.burnItem_voltage13_5} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Turn_Voltage_5V_Item)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.burnItem_voltage5} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Turn_Voltage_33_1V_Item)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.burnItem_voltage3_3_1} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Turn_Voltage_33_2V_Item)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.burnItem_voltage3_3_2} = '{testItemString}' WHERE " +
                      $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                #endregion
            }
            else if (station == "灵敏度测试工站")
            {
                #region testItem sen type
                if (testItem == Sen_BootloaderVersion)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.sensibilityItem_bootloader} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Sen_DormantElect)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.sensibilityItem_dormantElect} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Sen_ECUID)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.sensibilityItem_EcuID} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Sen_HardWareVersion)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.sensibilityItem_hardwareVersion} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Sen_PartNumber)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.sensibilityItem_partNumber} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Sen_RadioFreq)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.sensibilityItem_radioFreq} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Sen_SoftVersion)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.sensibilityItem_softVersion} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Sen_Work_Electric_Test)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.sensibilityItem_workElect} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                #endregion
            }
            else if (station == "外壳装配工站")
            {
                #region shell testItem type
                if (testItem == Shell_BackCover)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.shellItem_backCover} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Shell_FrontCover)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.shellItem_frontCover} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Shell_PCBScrew1)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.shellItem_pcbScrew1} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Shell_PCBScrew2)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.shellItem_pcbScrew2} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Shell_PCBScrew3)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.shellItem_pcbScrew3} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Shell_PCBScrew4)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.shellItem_pcbScrew4} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Shell_ShellScrew1)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.shellItem_shellScrew1} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Shell_ShellScrew2)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.shellItem_shellScrew2} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Shell_ShellScrew3)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.shellItem_shellScrew3} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Shell_ShellScrew4)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.shellItem_shellScrew4} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                #endregion
            }
            else if (station == "气密测试工站")
            {
                if (testItem == Air_AirtightTest)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.airtageItem_airTest} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
            }
            else if (station == "支架装配工站")
            {
                #region stent testItem type
                if (testItem == Stent_LeftStent)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.stentItem_leftStent} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Stent_RightStent)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.stentItem_rightStent} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Stent_Screw1)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.stentItem_stentScrew1} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Stent_Screw2)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.stentItem_stentScrew2} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Stent_Stent)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.stentItem_stent} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                #endregion
            }
            else if (station == "成品测试工站")
            {
                #region product testItem type
                if (testItem == Product_DormantElect)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.productItem_dormantElect} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Product_InspectItem)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.productItem_inspectItem} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Product_Inspect_Result)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.productItem_inspectResult} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                else if (testItem == Product_Work_Electric_Test)
                {
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " + $"{DbTable.F_TEST_RESULT_HISTORY.productItem_workElect} = '{testItemString}' WHERE " +
                        $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
                }
                #endregion
            }

            if (testLog.pcbaTestStationInStatus == PcbaTestStationInStatusEnum.stationIn_not_stationOut)
            {
                //update testItem
                var upRow = SQLServer.ExecuteNonQuery(updateSQL);
                //LogHelper.Log.Info($"【更新测试项新表】{station} 影响行数=" + upRow);
            }
        }

        #region 查询工站是否进站且未出站

        /// <summary>
        /// 查询烧录工站是否进站且未出站
        /// </summary>
        /// <param name="pcba"></param>
        /// <param name="productTypeNo"></param>
        /// <param name="station"></param>
        /// <returns></returns>
        public static TestLogData IsExistStationInRecord(string pcba, string productTypeNo, string station)
        {
            //根据PCBA查询最新记录
            //存在-查询当前工站，是否已插入数据
            //不存在数据，则更新上去；否则插入数据
            TestLogData testLogResult = new TestLogData();
            var selectPCBALastestSQL = "";
            if (station == "烧录工站")
            {
                selectPCBALastestSQL = $"SELECT TOP 1 {DbTable.F_TEST_RESULT_HISTORY.id}," +
                    $"{DbTable.F_TEST_RESULT_HISTORY.burnDateOut},{DbTable.F_TEST_RESULT_HISTORY.burnTestResult} " +
                    $"FROM {DbTable.F_TEST_RESULT_HISTORY_NAME} WHERE " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{pcba}' AND " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productTypeNo} = '{productTypeNo}' AND " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.burnStationName} = '{station}' ORDER BY " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.updateDate} DESC";
            }
            else if (station == "灵敏度测试工站")
            {
                selectPCBALastestSQL = $"SELECT TOP 1 {DbTable.F_TEST_RESULT_HISTORY.id}," +
                    $"{DbTable.F_TEST_RESULT_HISTORY.sensibilityDateOut},{DbTable.F_TEST_RESULT_HISTORY.sensibilityTestResult} " +
                    $"FROM {DbTable.F_TEST_RESULT_HISTORY_NAME} WHERE " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{pcba}' AND " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productTypeNo} = '{productTypeNo}' AND " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.sensibilityStationName} = '{station}' ORDER BY " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.updateDate} DESC";
            }
            else if (station == "外壳装配工站")
            {
                selectPCBALastestSQL = $"SELECT TOP 1 {DbTable.F_TEST_RESULT_HISTORY.id}," +
                    $"{DbTable.F_TEST_RESULT_HISTORY.shellDateOut},{DbTable.F_TEST_RESULT_HISTORY.shellTestResult} " +
                    $"FROM {DbTable.F_TEST_RESULT_HISTORY_NAME} WHERE " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{pcba}' AND " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productTypeNo} = '{productTypeNo}' AND " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.shellStationName} = '{station}' ORDER BY " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.updateDate} DESC";
            }
            else if (station == "气密测试工站")
            {
                selectPCBALastestSQL = $"SELECT TOP 1 {DbTable.F_TEST_RESULT_HISTORY.id}," +
                    $"{DbTable.F_TEST_RESULT_HISTORY.airtageDateOut},{DbTable.F_TEST_RESULT_HISTORY.airtageTestResult} " +
                    $"FROM {DbTable.F_TEST_RESULT_HISTORY_NAME} WHERE " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{pcba}' AND " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productTypeNo} = '{productTypeNo}' AND " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.airtageStationName} = '{station}' ORDER BY " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.updateDate} DESC";
            }
            else if (station == "支架装配工站")
            {
                selectPCBALastestSQL = $"SELECT TOP 1 {DbTable.F_TEST_RESULT_HISTORY.id}," +
                    $"{DbTable.F_TEST_RESULT_HISTORY.stentDateOut},{DbTable.F_TEST_RESULT_HISTORY.stentTestResult} " +
                    $"FROM {DbTable.F_TEST_RESULT_HISTORY_NAME} WHERE " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{pcba}' AND " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productTypeNo} = '{productTypeNo}' AND " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.stentStationName} = '{station}' ORDER BY " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.updateDate} DESC";
            }
            else if (station == "成品测试工站")
            {
                selectPCBALastestSQL = $"SELECT TOP 1 {DbTable.F_TEST_RESULT_HISTORY.id}," +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productDateOut},{DbTable.F_TEST_RESULT_HISTORY.productTestResult} " +
                    $"FROM {DbTable.F_TEST_RESULT_HISTORY_NAME} WHERE " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{pcba}' AND " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productTypeNo} = '{productTypeNo}' AND " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productStationName} = '{station}' ORDER BY " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.updateDate} DESC";
            }

            var dtPcbaData = SQLServer.ExecuteDataSet(selectPCBALastestSQL).Tables[0];
            if (dtPcbaData.Rows.Count > 0)
            {
                //已进站
                //判断是否出站
                var pid = dtPcbaData.Rows[0][0].ToString();
                var stationOutDate = dtPcbaData.Rows[0][1].ToString();
                var testResultValue = dtPcbaData.Rows[0][2].ToString();
                if (stationOutDate == "" && testResultValue == "")
                {
                    //未出站，更新数据
                    testLogResult.pcbaID = pid;
                    testLogResult.pcbaTestStationInStatus = PcbaTestStationInStatusEnum.stationIn_not_stationOut;
                }
                else
                {
                    //已出站/不更新
                    testLogResult.pcbaTestStationInStatus = PcbaTestStationInStatusEnum.staionIn_stationOut;
                }
            }
            else
            {
                //未进站
                testLogResult.pcbaTestStationInStatus = PcbaTestStationInStatusEnum.not_stationIn;
            }
            return testLogResult;
        }

        #endregion

        #region 复制旧表数据到新表
        public static async void CopyDataSource2NewTable()
        {
            try
            {
                var selectSQL = $"select * from {DbTable.F_TEST_RESULT_NAME} ";
                var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    int i = 0;
                    await System.Threading.Tasks.Task.Run(() =>
                    {

                    foreach (DataRow dbReader in dt.Rows)
                    {
                        var typeNo = dbReader[0].ToString();
                        var pid = dbReader[1].ToString();
                            if (pid != "017 B19C19017501")
                                continue;
                        var station = dbReader[3].ToString();
                        var result = dbReader[4].ToString();
                        var dateIn = dbReader[5].ToString();
                        if(dateIn != "")
                            dateIn = Convert.ToDateTime(dateIn).ToString("yyyy-MM-dd HH:mm:ss");
                        var dateOut = dbReader[6].ToString();
                        if(dateOut != "")
                            dateOut = Convert.ToDateTime(dateOut).ToString("yyyy-MM-dd HH:mm:ss");
                        var updateDate = dbReader[8].ToString();
                        if(updateDate != "")
                            updateDate = Convert.ToDateTime(updateDate).ToString("yyyy-MM-dd HH:mm:ss");
                        var user = dbReader[10].ToString();
                        var joinDate = dbReader[12].ToString();

                        LogHelper.Log.Info($"开始更新...第{i}条 {pid} {typeNo} {station} {result} {dateIn} {dateOut} {updateDate} {user}");
                        //1）更新进站记录
                        TestResult.UpdateTestResultHistory(pid, typeNo, station, dateIn, updateDate);

                        //4)更新绑定关系
                        AddTestResultHistoryBindSN(pid);//pcb/shell
                        //2）更新测试项
                        #region 烧录工站
                        if (station == "烧录工站")
                        {

                            SelectTestItem(pid, typeNo, station, Turn_TurnItem, joinDate);
                            SelectTestItem(pid, typeNo, station, Turn_SoftVersion, joinDate);
                            SelectTestItem(pid, typeNo, station, Turn_Voltage_12V_Item, joinDate);
                            SelectTestItem(pid, typeNo, station, Turn_Voltage_5V_Item, joinDate);
                            SelectTestItem(pid, typeNo, station, Turn_Voltage_33_1V_Item, joinDate);
                            SelectTestItem(pid, typeNo, station, Turn_Voltage_33_2V_Item, joinDate);
                        }
                        #endregion

                        #region 灵敏度
                        if (station == "灵敏度测试工站")
                        {
                            SelectTestItem(pid, typeNo, station, Sen_BootloaderVersion, joinDate);
                            SelectTestItem(pid, typeNo, station, Sen_DormantElect, joinDate);
                            SelectTestItem(pid, typeNo, station, Sen_ECUID, joinDate);
                            SelectTestItem(pid, typeNo, station, Sen_HardWareVersion, joinDate);
                            SelectTestItem(pid, typeNo, station, Sen_PartNumber, joinDate);
                            SelectTestItem(pid, typeNo, station, Sen_RadioFreq, joinDate);
                            SelectTestItem(pid, typeNo, station, Sen_SoftVersion, joinDate);
                            SelectTestItem(pid, typeNo, station, Sen_Work_Electric_Test, joinDate);
                        }
                        #endregion

                        #region 外壳
                        if (station == "外壳装配工站")
                        {
                            SelectTestItem(pid, typeNo, station, Shell_BackCover, joinDate);
                            SelectTestItem(pid, typeNo, station, Shell_FrontCover, joinDate);
                            SelectTestItem(pid, typeNo, station, Shell_PCBScrew1, joinDate);
                            SelectTestItem(pid, typeNo, station, Shell_PCBScrew2, joinDate);
                            SelectTestItem(pid, typeNo, station, Shell_PCBScrew3, joinDate);
                            SelectTestItem(pid, typeNo, station, Shell_PCBScrew4, joinDate);
                            SelectTestItem(pid, typeNo, station, Shell_ShellScrew1, joinDate);
                            SelectTestItem(pid, typeNo, station, Shell_ShellScrew2, joinDate);
                            SelectTestItem(pid, typeNo, station, Shell_ShellScrew3, joinDate);
                            SelectTestItem(pid, typeNo, station, Shell_ShellScrew4, joinDate);
                        }
                        #endregion

                        #region 气密
                        if (station == "气密测试工站")
                        {
                            SelectTestItem(pid, typeNo, station, Air_AirtightTest, joinDate);
                        }
                        #endregion

                        #region 支架
                        if (station == "支架装配工站")
                        {
                            SelectTestItem(pid, typeNo, station, Stent_LeftStent, joinDate);
                            SelectTestItem(pid, typeNo, station, Stent_RightStent, joinDate);
                            SelectTestItem(pid, typeNo, station, Stent_Screw1, joinDate);
                            SelectTestItem(pid, typeNo, station, Stent_Screw2, joinDate);
                            SelectTestItem(pid, typeNo, station, Stent_Stent, joinDate);
                        }
                        #endregion

                        #region 成品
                        if (station == "成品测试工站")
                        {
                            SelectTestItem(pid, typeNo, station, Product_DormantElect, joinDate);
                            SelectTestItem(pid, typeNo, station, Product_Inspect_Result, joinDate);
                            SelectTestItem(pid, typeNo, station, Product_Work_Electric_Test, joinDate);
                        }
                        #endregion

                        //3）更新出战结果
                        TestResult.UpdateStationOutResult(pid, typeNo, station, result, dateOut, updateDate,user);

                        //更新完毕
                        i++;
                    }

                    });

                    UpdateAllPcbBind();
                    UpdateHistoryBindStatus();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log.Info(ex.Message+ex.StackTrace);
            }
        }

        private static void AddTestResultHistoryBindSN(string sn)
        {
            var selectPidSQL = $"select top 1 {DbTable.F_TEST_RESULT_HISTORY.id}," +
                $"{DbTable.F_TEST_RESULT_HISTORY.productSN} from {DbTable.F_TEST_RESULT_HISTORY_NAME} where " +
                $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}'";
            var selectTidSQL = $"select top 1 {DbTable.F_TEST_RESULT_HISTORY.id}," +
               $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} from {DbTable.F_TEST_RESULT_HISTORY_NAME} where " +
               $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{sn}'";

            var selectPidBind = $"select top 1 {DbTable.F_BINDING_PCBA.SN_OUTTER} from {DbTable.F_BINDING_PCBA_NAME} " +
                $"where {DbTable.F_BINDING_PCBA.SN_PCBA} = '{sn}'";
            var selectTidBind = $"select top 1 {DbTable.F_BINDING_PCBA.SN_PCBA} from {DbTable.F_BINDING_PCBA_NAME} " +
                $"where {DbTable.F_BINDING_PCBA.SN_OUTTER} = '{sn}'";

            var dt = SQLServer.ExecuteDataSet(selectPidSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dbReader in dt.Rows)
                {
                    var kid = dbReader[0].ToString();
                    var tid = dbReader[1].ToString();
                    if (tid == "")
                    {
                        //pcb=null,shell is not null
                        //新表不存在绑定关系
                        //查询旧表是否存在绑定关系
                        dt = SQLServer.ExecuteDataSet(selectPidBind).Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            //有绑定关系，也许后面会解绑，最后更新所有数据的最终关系
                            foreach (DataRow dr in dt.Rows)
                            {
                                var shellID = dr[0].ToString();
                                if (shellID != "")
                                {
                                    var updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " +
                                   $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{shellID}' " +
                                   $"WHERE {DbTable.F_TEST_RESULT_HISTORY.id} = '{kid}'";
                                    SQLServer.ExecuteNonQuery(updateSQL);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                dt = SQLServer.ExecuteDataSet(selectTidSQL).Tables[0];
                foreach (DataRow dbReader in dt.Rows)
                {
                    var kid = dbReader[0].ToString();
                    var pid = dbReader[1].ToString();
                    if (pid == "")
                    {
                        //pcb=null,shell is not null
                        //新表不存在绑定关系
                        //查询旧表是否存在绑定关系
                        dt = SQLServer.ExecuteDataSet(selectTidBind).Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            //有绑定关系，也许后面会解绑，最后更新所有数据的最终关系
                            foreach (DataRow dr in dt.Rows)
                            {
                                var pcbID = dr[0].ToString();
                                if (pcbID != "")
                                {
                                    var updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " +
                                   $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{pcbID}' " +
                                   $"WHERE {DbTable.F_TEST_RESULT_HISTORY.id} = '{kid}'";
                                    SQLServer.ExecuteNonQuery(updateSQL);
                                }
                            }
                        }
                    }
                }
            }
        }
        private static void SelectTestItem(string pid,string typeNo, string stationName, string testItem, string joinDateTime)
        {
            var selectSQL = $"select top 1 {DbTable.F_TEST_LOG_DATA.TEST_ITEM},{DbTable.F_TEST_LOG_DATA.LIMIT}," +
                $"{DbTable.F_TEST_LOG_DATA.CURRENT_VALUE},{DbTable.F_TEST_LOG_DATA.TEST_RESULT} from " +
                $"{DbTable.F_TEST_LOG_DATA_NAME} where {DbTable.F_TEST_LOG_DATA.PRODUCT_SN} = '{pid}' AND " +
                $"{DbTable.F_TEST_LOG_DATA.STATION_NAME} = '{stationName}' AND " +
                $"{DbTable.F_TEST_LOG_DATA.TEST_ITEM} like '%{testItem}%' AND " +
                $"{DbTable.F_TEST_LOG_DATA.JOIN_DATE_TIME} = '{joinDateTime}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dbReader in dt.Rows)
                {
                    var tstItem = dbReader[0].ToString();
                    var limit = dbReader[1].ToString();
                    var currentValue = dbReader[2].ToString();
                    var testResult = dbReader[3].ToString();

                    UpdateTestLogHistory(pid, typeNo, stationName, tstItem, limit, currentValue, testResult);
                }
            }
        }

        private static void UpdateHistoryBindStatus()
        {
            var selectSQL = $"select {DbTable.F_TEST_RESULT_HISTORY.pcbaSN},{DbTable.F_TEST_RESULT_HISTORY.productSN} from " +
                $"{DbTable.F_TEST_RESULT_HISTORY_NAME} ";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var pid = dr[0].ToString();
                    var tid = dr[1].ToString();
                    selectSQL = $"select top 1 {DbTable.F_BINDING_PCBA.BINDING_STATE} from {DbTable.F_BINDING_PCBA_NAME} " +
                        $"where {DbTable.F_BINDING_PCBA.SN_PCBA} = '{pid}' and {DbTable.F_BINDING_PCBA.SN_OUTTER}='{tid}'";
                    if (pid != "" && tid != "")
                    {
                        dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                        var bindState = dt.Rows[0][0].ToString();
                        var updateSQL = $"update {DbTable.F_TEST_RESULT_HISTORY_NAME} set " +
                            $"{DbTable.F_TEST_RESULT_HISTORY.bindState} = '{bindState}' where " +
                            $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{pid}' and " +
                            $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{tid}'";
                        var row = SQLServer.ExecuteNonQuery(updateSQL);
                    }
                }
            }
        }

        private static void UpdateAllPcbBind()
        {
            var selectSQL = $"select {DbTable.F_Test_Result.SN} from {DbTable.F_TEST_RESULT_NAME}";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var pid = dr[0].ToString();
                    selectSQL = $"select * from {DbTable.F_TEST_PCBA_NAME} where {DbTable.F_TEST_PCBA.PCBA_SN}= '{pid}'";
                    dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                    if (dt.Rows.Count < 1)
                    {
                        //本表没有数据
                        //查询是否是外壳
                        selectSQL = $"select * from {DbTable.F_BINDING_PCBA_NAME} where {DbTable.F_BINDING_PCBA.SN_OUTTER} = '{pid}'";
                        dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                        if (dt.Rows.Count < 1)
                        {
                            //插入本地
                            var insertSQL = $"insert into {DbTable.F_TEST_PCBA_NAME}({DbTable.F_TEST_PCBA.PCBA_SN},{DbTable.F_TEST_PCBA.UPDATE_DATE}) values(" +
                                $"'{pid}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
                            int row = SQLServer.ExecuteNonQuery(insertSQL);
                        }
                    }
                }
            }
        }
        #endregion
    }
}