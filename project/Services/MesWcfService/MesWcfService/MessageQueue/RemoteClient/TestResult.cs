using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonUtils.DB;
using CommonUtils.Logger;
using CommonUtils.FileHelper;
using MesWcfService.DB;
using System;
using MesWcfService;
using System.Configuration;

namespace MesWcfService.MessageQueue.RemoteClient
{
    public class TestResult
    {
        private PcbaTestResultStatusEnum pcbaTestResultStatusEnum;
        private string pcbaID = "";
        private string productSN = "";
        private string pcbaSN = "";
        private string bindState  = "";


        private enum PcbaTestResultStatusEnum
        {
            none,
            pcbaNotExist,
            currentStationExist,
            currentStationNotExist
        }
        private enum UpdateTestResultEnum
        {
            STATUS_SUCCESS = 0,
            ERROR_FAIL = 1,
            ERROR_SN_IS_NULL = 2,
            ERROR_STATION_IS_NULL = 3,
            ERROR_FIRST_STATION = 4,
            ERROR_RESULT_IS_NULL = 5,
            ERROR_JOINT_TIME_IS_NULL =6
        }

        private static string ConvertTestResultCode(UpdateTestResultEnum rCode)
        {
            return "0X" + Convert.ToString((int)rCode, 16).PadLeft(2, '0');
        }

        /*
 * 测试结果要记录进站时间与出站时间
 * 进站时间：开始测试的时间，查询上一工位测试结果为PASS的时间
 * 出站时间：测试完成的时间
 * 流程：
 * 1）新增接口记录第一个工站开始的时间，更新进站日期；
 *  更新：第一次插入数据后，下一个更新进站日期时，查询该工位最新的一条数据进行更新
 * 2）传入测试数据结果的接口用于更新出站时间
 * 3）第1个工站之后的所有工站的进站时间为该工站查询上一工站返回结果为PASS的时间
 *      当查询上一站位成功后，将信息插入到数据库（带进站时间），待测试完成后进行更新
 * 
 */
        public static string CommitTestResult(Queue<string[]> queue)
        {
            string[] array = queue.Dequeue();
            var sn = array[0].Trim();
            var typeNo = array[1].Trim();
            var station = array[2].Trim();
            var result = array[3].Trim();
            var teamLeader = array[4].Trim();
            var admin = array[5].Trim();
            var joinDateTime = array[6].Trim();
            var currentDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            LogHelper.Log.Info("【更新出站记录】");
            if (sn == "")
            {
                LogHelper.Log.Info("【更新出站结果】ERROR_SN_IS_NULL=0X02");
                return ConvertTestResultCode(UpdateTestResultEnum.ERROR_SN_IS_NULL);
            }
            if (station == "")
            {
                LogHelper.Log.Info("【更新出站结果】ERROR_STATION_IS_NULL=0X03");
                return ConvertTestResultCode(UpdateTestResultEnum.ERROR_STATION_IS_NULL);
            }
            if (result == "")
            {
                LogHelper.Log.Info("【更新出站结果】ERROR_RESULT_IS_NULL=0X05");
                return ConvertTestResultCode(UpdateTestResultEnum.ERROR_RESULT_IS_NULL);
            }
            if (joinDateTime == "")
            {
                LogHelper.Log.Info("【更新出站结果】ERROR_JOINT_TIME_IS_NULL=0X06");
                return ConvertTestResultCode(UpdateTestResultEnum.ERROR_JOINT_TIME_IS_NULL);
            }

            var row = UpdateTestResult(sn, typeNo, station, result, teamLeader, admin, joinDateTime);
            if (row > 0)
            {
                LogHelper.Log.Info("【更新出站结果】STATUS_SUCCESS=0X00");
                UpdateStationOutResult(sn,typeNo,station,result,currentDate,currentDate,teamLeader);
                return ConvertTestResultCode(UpdateTestResultEnum.STATUS_SUCCESS);
            }
            else
            {
                LogHelper.Log.Info("【更新出站结果】ERROR_FAIL=0X01 ");
                return ConvertTestResultCode(UpdateTestResultEnum.ERROR_FAIL);
            }
        }

        /// <summary>
        /// 主要是插入基础参数与进站时间
        /// </summary>
        /// <param name="stationInArray"></param>
        /// <returns></returns>
        private static int InsertTestResult(string sn,string processName,string station)
        {
            var insertSQL = $"INSERT INTO {DbTable.F_TEST_RESULT_NAME}(" +
                        $"{DbTable.F_Test_Result.SN}," +
                        $"{DbTable.F_Test_Result.TYPE_NO}," +
                        $"{DbTable.F_Test_Result.STATION_NAME}," +
                        $"{DbTable.F_Test_Result.PROCESS_NAME}," +
                        $"{DbTable.F_Test_Result.UPDATE_DATE}," +
                        $"{DbTable.F_Test_Result.STATION_IN_DATE}) " +
                        $"VALUES('{sn}','{processName}','{station}','{processName}'," +
                        $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'," +
                        $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
            LogHelper.Log.Info("【插入进站记录】"+insertSQL);
            return SQLServer.ExecuteNonQuery(insertSQL);
        }

        /// <summary>
        /// 进站时更新进站记录
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="processName"></param>
        /// <param name="station"></param>
        public static void UpdateTestResultHistory(string sn, string processName,string station,string stationInDate,string updateDate)
        {
            UpdatePcbaTestHistory(sn);
            var insertSQL = "";
            var updateSQL = "";
            //int shellCodeLen = ReadShellCodeLength();
            //int pcbaCodeLen = ReadPCBACodeLength();

            if (station == "烧录工站")
            {
                var recordStatus = IsExistBurnStationRecord(sn, station);
                insertSQL = $"INSERT INTO {DbTable.F_TEST_RESULT_HISTORY_NAME}({DbTable.F_TEST_RESULT_HISTORY.pcbaSN}," +
                         $"{DbTable.F_TEST_RESULT_HISTORY.productTypeNo},{DbTable.F_TEST_RESULT_HISTORY.burnStationName}," +
                         $"{DbTable.F_TEST_RESULT_HISTORY.burnDateIn},{DbTable.F_TEST_RESULT_HISTORY.updateDate}," +
                         $"{DbTable.F_TEST_RESULT_HISTORY.productSN},{DbTable.F_TEST_RESULT_HISTORY.bindState}) " +
                         $"VALUES('{sn}','{processName}','{station}','{stationInDate}','{updateDate}','{recordStatus.productSN}','{recordStatus.bindState}')";
                if (recordStatus.pcbaTestResultStatusEnum == PcbaTestResultStatusEnum.currentStationExist || recordStatus.pcbaTestResultStatusEnum == PcbaTestResultStatusEnum.pcbaNotExist)
                {
                    //insert new row
                    int row = SQLServer.ExecuteNonQuery(insertSQL);
                }
                else if(recordStatus.pcbaTestResultStatusEnum == PcbaTestResultStatusEnum.currentStationNotExist)
                {
                    //update
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.burnStationName} = '{station}'," +
                    $"{DbTable.F_TEST_RESULT_HISTORY.burnDateIn} = '{stationInDate}'," +
                    $"{DbTable.F_TEST_RESULT_HISTORY.updateDate} = '{updateDate}' WHERE " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productTypeNo} = '{processName}' AND " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.id} = '{recordStatus.pcbaID}'";
                    int row = SQLServer.ExecuteNonQuery(updateSQL);
                }
            }
            else if (station == "灵敏度测试工站")
            {
                var recordStatus = IsExistSensibilityStationRecord(sn, station);
                insertSQL = $"INSERT INTO {DbTable.F_TEST_RESULT_HISTORY_NAME}({DbTable.F_TEST_RESULT_HISTORY.pcbaSN}," +
                       $"{DbTable.F_TEST_RESULT_HISTORY.productTypeNo},{DbTable.F_TEST_RESULT_HISTORY.sensibilityStationName}," +
                       $"{DbTable.F_TEST_RESULT_HISTORY.sensibilityDateIn},{DbTable.F_TEST_RESULT_HISTORY.updateDate}," +
                       $"{DbTable.F_TEST_RESULT_HISTORY.productSN},'{DbTable.F_TEST_RESULT_HISTORY.bindState}') " +
                       $"VALUES('{sn}','{processName}','{station}','{stationInDate}','{updateDate}','{recordStatus.productSN}','{recordStatus.bindState}')";
                if (recordStatus.pcbaTestResultStatusEnum == PcbaTestResultStatusEnum.currentStationExist || recordStatus.pcbaTestResultStatusEnum == PcbaTestResultStatusEnum.pcbaNotExist)
                {
                    //insert new row
                    int row = SQLServer.ExecuteNonQuery(insertSQL);
                }
                else if (recordStatus.pcbaTestResultStatusEnum == PcbaTestResultStatusEnum.currentStationNotExist)
                {
                    //update
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.sensibilityStationName} = '{station}'," +
                    $"{DbTable.F_TEST_RESULT_HISTORY.sensibilityDateIn} = '{stationInDate}'," +
                    $"{DbTable.F_TEST_RESULT_HISTORY.updateDate} = '{updateDate}' WHERE " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productTypeNo} = '{processName}' AND " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.id} = '{recordStatus.pcbaID}'";
                    int row = SQLServer.ExecuteNonQuery(updateSQL);
                }
            }
            else if (station == "外壳装配工站")
            {
                var recordStatus = IsExistShellStationRecord(sn, station);
                insertSQL = $"INSERT INTO {DbTable.F_TEST_RESULT_HISTORY_NAME}({DbTable.F_TEST_RESULT_HISTORY.pcbaSN}," +
                          $"{DbTable.F_TEST_RESULT_HISTORY.productTypeNo},{DbTable.F_TEST_RESULT_HISTORY.shellStationName}," +
                          $"{DbTable.F_TEST_RESULT_HISTORY.shellDateIn},{DbTable.F_TEST_RESULT_HISTORY.updateDate}," +
                          $"{DbTable.F_TEST_RESULT_HISTORY.productSN},{DbTable.F_TEST_RESULT_HISTORY.bindState}) " +
                          $"VALUES('{sn}','{processName}','{station}','{stationInDate}','{updateDate}','{recordStatus.productSN}','{recordStatus.bindState}')";
                if (recordStatus.pcbaTestResultStatusEnum == PcbaTestResultStatusEnum.currentStationExist || recordStatus.pcbaTestResultStatusEnum == PcbaTestResultStatusEnum.pcbaNotExist)
                {
                    //insert new row
                    int row = SQLServer.ExecuteNonQuery(insertSQL);
                }
                else if (recordStatus.pcbaTestResultStatusEnum == PcbaTestResultStatusEnum.currentStationNotExist)
                {
                    //update
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.shellStationName} = '{station}'," +
                    $"{DbTable.F_TEST_RESULT_HISTORY.shellDateIn} = '{stationInDate}'," +
                    $"{DbTable.F_TEST_RESULT_HISTORY.updateDate} = '{updateDate}' WHERE " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productTypeNo} = '{processName}' AND " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.id} = '{recordStatus.pcbaID}'";
                    int row = SQLServer.ExecuteNonQuery(updateSQL);
                }
            }
            else if (station == "气密测试工站")
            {
                var recordStatus = IsExistAirtageStationRecord(sn, station);
                insertSQL = $"INSERT INTO {DbTable.F_TEST_RESULT_HISTORY_NAME}({DbTable.F_TEST_RESULT_HISTORY.productSN}," +
                      $"{DbTable.F_TEST_RESULT_HISTORY.productTypeNo},{DbTable.F_TEST_RESULT_HISTORY.airtageStationName}," +
                      $"{DbTable.F_TEST_RESULT_HISTORY.airtageDateIn},{DbTable.F_TEST_RESULT_HISTORY.updateDate}," +
                      $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN},{DbTable.F_TEST_RESULT_HISTORY.bindState}) " +
                      $"VALUES('{sn}','{processName}','{station}','{stationInDate}','{updateDate}','{recordStatus.pcbaSN}','{recordStatus.bindState}')";
                if (recordStatus.pcbaTestResultStatusEnum == PcbaTestResultStatusEnum.currentStationExist || recordStatus.pcbaTestResultStatusEnum == PcbaTestResultStatusEnum.pcbaNotExist)
                {
                    //insert new row
                    int row = SQLServer.ExecuteNonQuery(insertSQL);
                }
                else if (recordStatus.pcbaTestResultStatusEnum == PcbaTestResultStatusEnum.currentStationNotExist)
                {
                    //update
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.airtageStationName} = '{station}'," +
                    $"{DbTable.F_TEST_RESULT_HISTORY.airtageDateIn} = '{stationInDate}'," +
                    $"{DbTable.F_TEST_RESULT_HISTORY.updateDate} = '{updateDate}' WHERE " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{sn}' AND " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productTypeNo} = '{processName}' AND " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.id} = '{recordStatus.pcbaID}'";
                    int row = SQLServer.ExecuteNonQuery(updateSQL);
                }
            }
            else if (station == "支架装配工站")
            {
                var recordStatus = IsExistStentStationRecord(sn, station);
                insertSQL = $"INSERT INTO {DbTable.F_TEST_RESULT_HISTORY_NAME}({DbTable.F_TEST_RESULT_HISTORY.productSN}," +
                        $"{DbTable.F_TEST_RESULT_HISTORY.productTypeNo},{DbTable.F_TEST_RESULT_HISTORY.stentStationName}," +
                        $"{DbTable.F_TEST_RESULT_HISTORY.stentDateIn},{DbTable.F_TEST_RESULT_HISTORY.updateDate}," +
                        $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN},{DbTable.F_TEST_RESULT_HISTORY.bindState}) " +
                        $"VALUES('{sn}','{processName}','{station}','{stationInDate}','{updateDate}','{recordStatus.pcbaSN}','{recordStatus.bindState}')";
                if (recordStatus.pcbaTestResultStatusEnum == PcbaTestResultStatusEnum.currentStationExist || recordStatus.pcbaTestResultStatusEnum == PcbaTestResultStatusEnum.pcbaNotExist)
                {
                    //insert new row
                    int row = SQLServer.ExecuteNonQuery(insertSQL);
                }
                else if (recordStatus.pcbaTestResultStatusEnum == PcbaTestResultStatusEnum.currentStationNotExist)
                {
                    //update
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.stentStationName} = '{station}'," +
                    $"{DbTable.F_TEST_RESULT_HISTORY.stentDateIn} = '{stationInDate}'," +
                    $"{DbTable.F_TEST_RESULT_HISTORY.updateDate} = '{updateDate}' WHERE " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{sn}' AND " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productTypeNo} = '{processName}' AND " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.id} = '{recordStatus.pcbaID}'";
                    int row = SQLServer.ExecuteNonQuery(updateSQL);
                }
            }
            else if (station == "成品测试工站")
            {
                var recordStatus = IsExistProductStationRecord(sn, station);
                insertSQL = $"INSERT INTO {DbTable.F_TEST_RESULT_HISTORY_NAME}({DbTable.F_TEST_RESULT_HISTORY.productSN}," +
                        $"{DbTable.F_TEST_RESULT_HISTORY.productTypeNo},{DbTable.F_TEST_RESULT_HISTORY.productStationName}," +
                        $"{DbTable.F_TEST_RESULT_HISTORY.productDateIn},{DbTable.F_TEST_RESULT_HISTORY.updateDate}," +
                        $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN},{DbTable.F_TEST_RESULT_HISTORY.bindState}) " +
                        $"VALUES('{sn}','{processName}','{station}','{stationInDate}','{updateDate}','{recordStatus.pcbaSN}','{recordStatus.bindState}')";
                if (recordStatus.pcbaTestResultStatusEnum == PcbaTestResultStatusEnum.currentStationExist || recordStatus.pcbaTestResultStatusEnum == PcbaTestResultStatusEnum.pcbaNotExist)
                {
                    //insert new row
                    int row = SQLServer.ExecuteNonQuery(insertSQL);
                }
                else if (recordStatus.pcbaTestResultStatusEnum == PcbaTestResultStatusEnum.currentStationNotExist)
                {
                    //update
                    updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productStationName} = '{station}'," +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productDateIn} = '{stationInDate}'," +
                    $"{DbTable.F_TEST_RESULT_HISTORY.updateDate} = '{updateDate}' WHERE " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{sn}' AND " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productTypeNo} = '{processName}' AND " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.id} = '{recordStatus.pcbaID}'";
                    int row = SQLServer.ExecuteNonQuery(updateSQL);
                }
            }
        }

        public static void UpdatePcbaTestHistory(string sn)
        {
            //根据传入SN查询是否已经存在
            //历史表不存在且绑定记录表不存在-则插入
            //历史表不存在且绑定记录表存在--此SN为外壳SN--不插入
            //历史表存在--不插入
            //进站-插入/更新；出站-更新；外壳工站绑定后更新
            var selectSQL = $"select top 1 * from {DbTable.F_TEST_PCBA_NAME} where " +
                $"{DbTable.F_TEST_PCBA.PCBA_SN} = '{sn}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            var currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (dt.Rows.Count > 0)
            {
                //已存在数据/不插入/更新日期
                //烧录-灵敏度-外壳
                var updateSQL = $"update {DbTable.F_TEST_PCBA_NAME} set {DbTable.F_TEST_PCBA.UPDATE_DATE} ='{currentDate}' " +
                    $"where {DbTable.F_TEST_PCBA.PCBA_SN}='{sn}'";
                SQLServer.ExecuteNonQuery(updateSQL);
            }
            else
            {
                //不存在数据，查询是否有绑定记录/已经绑定或者已经解绑
                selectSQL = $"select top 1 {DbTable.F_BINDING_PCBA.SN_PCBA} from {DbTable.F_BINDING_PCBA_NAME} where " +
                    $"({DbTable.F_BINDING_PCBA.SN_OUTTER} = '{sn}' OR {DbTable.F_BINDING_PCBA.SN_PCBA} = '{sn}') ";
                dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    //已存在绑定记录/不插入/更新日期
                    var updateSQL = $"update {DbTable.F_TEST_PCBA_NAME} set {DbTable.F_TEST_PCBA.UPDATE_DATE} ='{currentDate}' " +
                        $"where {DbTable.F_TEST_PCBA.PCBA_SN}='{dt.Rows[0][0].ToString()}'";
                    SQLServer.ExecuteNonQuery(updateSQL);
                }
                else
                {
                    //不存在绑定记录/插入
                    var insertSQL = $"insert into {DbTable.F_TEST_PCBA_NAME}({DbTable.F_TEST_PCBA.PCBA_SN}," +
                        $"{DbTable.F_TEST_PCBA.UPDATE_DATE}) values('{sn}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
                    int row = SQLServer.ExecuteNonQuery(insertSQL);
                }
            }
        }

        //更新出站结果
        public static void UpdateStationOutResult(string sn, string typeNo, string station, string testResult, string dateOut, string updateDate,string user)
        {
            var updateSQL = "";
            UpdatePcbaTestHistory(sn);//出站时更新最新的PCB日期
            //var currentDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var testLog = TestLogData.IsExistStationInRecord(sn, typeNo, station);

            #region station type
            if (station == "烧录工站")
            {
                updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET {DbTable.F_TEST_RESULT_HISTORY.burnTestResult} = '{testResult}'," +
                    $"{DbTable.F_TEST_RESULT_HISTORY.burnDateOut} = '{dateOut}',{DbTable.F_TEST_RESULT_HISTORY.updateDate} = '{updateDate}',{DbTable.F_TEST_RESULT_HISTORY.burnOperator} = '{user}' WHERE " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
            }
            else if (station == "灵敏度测试工站")
            {
                updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET {DbTable.F_TEST_RESULT_HISTORY.sensibilityTestResult} = '{testResult}'," +
                    $"{DbTable.F_TEST_RESULT_HISTORY.sensibilityDateOut} = '{dateOut}',{DbTable.F_TEST_RESULT_HISTORY.updateDate} = '{updateDate}',{DbTable.F_TEST_RESULT_HISTORY.sensibilityOperator} = '{user}' WHERE " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
            }
            else if (station == "外壳装配工站")
            {
                updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET {DbTable.F_TEST_RESULT_HISTORY.shellTestResult} = '{testResult}'," +
                    $"{DbTable.F_TEST_RESULT_HISTORY.shellDateOut} = '{dateOut}',{DbTable.F_TEST_RESULT_HISTORY.updateDate} = '{updateDate}',{DbTable.F_TEST_RESULT_HISTORY.shellOperator} = '{user}' WHERE " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
            }
            else if (station == "气密测试工站")
            {
                updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET {DbTable.F_TEST_RESULT_HISTORY.airtageTestResult} = '{testResult}'," +
                    $"{DbTable.F_TEST_RESULT_HISTORY.airtageDateOut} = '{dateOut}',{DbTable.F_TEST_RESULT_HISTORY.updateDate} = '{updateDate}',{DbTable.F_TEST_RESULT_HISTORY.airtageOperator}='{user}' WHERE " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
            }
            else if (station == "支架装配工站")
            {
                updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET {DbTable.F_TEST_RESULT_HISTORY.stentTestResult} = '{testResult}'," +
                    $"{DbTable.F_TEST_RESULT_HISTORY.stentDateOut} = '{dateOut}',{DbTable.F_TEST_RESULT_HISTORY.updateDate} = '{updateDate}',{DbTable.F_TEST_RESULT_HISTORY.stentOperator}='{user}' WHERE " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
            }
            else if (station == "成品测试工站")
            {
                updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_HISTORY_NAME} SET {DbTable.F_TEST_RESULT_HISTORY.productTestResult} = '{testResult}'," +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productDateOut} = '{dateOut}',{DbTable.F_TEST_RESULT_HISTORY.updateDate} = '{updateDate}',{DbTable.F_TEST_RESULT_HISTORY.productOperator}='{user}' WHERE " +
                    $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{sn}' AND {DbTable.F_TEST_RESULT_HISTORY.id} = '{testLog.pcbaID}'";
            }
            #endregion

            if (testLog.pcbaTestStationInStatus == TestLogData.PcbaTestStationInStatusEnum.stationIn_not_stationOut)
            {
                //update testItem
                var upRow = SQLServer.ExecuteNonQuery(updateSQL);
                //LogHelper.Log.Info($"【更新测试项新表-出站结果】{station} 影响行数=" + upRow);
            }
        }

        private static int ReadShellCodeLength()
        {
            try
            {
                int shellLen = 0;
                var defaultRoot = ConfigurationManager.AppSettings["shellCodeRoot"].ToString();
                var process = new MesService().SelectCurrentTProcess();
                string configPath = defaultRoot + ":\\StationConfig\\外壳装配工站\\" + process + "\\" + "外壳装配工站_" + process + "_config.ini";
                int.TryParse(INIFile.GetValue(process, "设置外壳条码长度位数", configPath).Trim(), out shellLen);
                //LogHelper.Log.Info("【配置文件路径】" + configPath + "len="+shellLen);
                return shellLen;
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error("读取配置长度错误！" + ex.Message + ex.StackTrace + "\r\n");
                return 0;
            }
        }

        private static int ReadPCBACodeLength()
        {
            try
            {
                int shellLen = 0;
                var defaultRoot = ConfigurationManager.AppSettings["shellCodeRoot"].ToString();
                var process = new MesService().SelectCurrentTProcess();
                string configPath = defaultRoot + ":\\StationConfig\\外壳装配工站\\" + process + "\\" + "外壳装配工站_" + process + "_config.ini";
                int.TryParse(INIFile.GetValue(process, "设置PCB条码长度位数", configPath).Trim(), out shellLen);
                LogHelper.Log.Info("【配置文件路径】" + configPath + "len=" + shellLen);
                return shellLen;
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error("读取配置长度错误！" + ex.Message + ex.StackTrace + "\r\n");
                return 16;
            }
        }

        #region 查询工站是否有记录

        /// <summary>
        /// 查询烧录工站是否存在记录
        /// </summary>
        /// <param name="pcba"></param>
        /// <param name="station"></param>
        /// <returns></returns>
        private static TestResult IsExistBurnStationRecord(string pcba, string station)
        {
            //根据PCBA查询最新记录
            //存在-查询当前工站，是否已插入数据
            //不存在数据，则更新上去；否则插入数据
            TestResult testResult = new TestResult();
            var selectPCBALastestSQL = $"SELECT TOP 1 {DbTable.F_TEST_RESULT_HISTORY.id}," +
                $"{DbTable.F_TEST_RESULT_HISTORY.productSN},{DbTable.F_TEST_RESULT_HISTORY.bindState} " +
                $"FROM {DbTable.F_TEST_RESULT_HISTORY_NAME} WHERE " +
                $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{pcba}' ORDER BY " +
                $"{DbTable.F_TEST_RESULT_HISTORY.id} DESC";
            var dtPcbaData = SQLServer.ExecuteDataSet(selectPCBALastestSQL).Tables[0];
            if (dtPcbaData.Rows.Count > 0)
            {
                //查询该工站是否有数据
                var stationSQL = $"SELECT TOP 1 {DbTable.F_TEST_RESULT_HISTORY.id} " +
                $"FROM {DbTable.F_TEST_RESULT_HISTORY_NAME} WHERE " +
                $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{pcba}' AND " +
                $"{DbTable.F_TEST_RESULT_HISTORY.burnStationName} = '{station}' " +
                $"ORDER BY {DbTable.F_TEST_RESULT_HISTORY.id} DESC";
                var dtStationData = SQLServer.ExecuteDataSet(stationSQL).Tables[0];
                if (dtStationData.Rows.Count > 0)
                {
                    testResult.pcbaTestResultStatusEnum = PcbaTestResultStatusEnum.currentStationExist;
                    testResult.productSN = dtPcbaData.Rows[0][1].ToString();
                    testResult.bindState = dtPcbaData.Rows[0][2].ToString();
                    return testResult;
                }
                testResult.pcbaTestResultStatusEnum =  PcbaTestResultStatusEnum.currentStationNotExist;
                testResult.pcbaID = dtPcbaData.Rows[0][0].ToString();
                return testResult;
            }
            testResult.pcbaTestResultStatusEnum =  PcbaTestResultStatusEnum.pcbaNotExist;
            return testResult;
        }

        /// <summary>
        /// 查询灵敏度工站是否存在记录
        /// </summary>
        /// <param name="pcba"></param>
        /// <param name="station"></param>
        /// <returns></returns>
        private static TestResult IsExistSensibilityStationRecord(string pcba,string station)
        {
            //根据PCBA查询最新记录
            //存在-查询当前工站，是否已插入数据
            //不存在数据，则更新上去；否则插入数据
            TestResult testResult = new TestResult();
            var selectPCBALastestSQL = $"SELECT TOP 1 {DbTable.F_TEST_RESULT_HISTORY.id}," +
                $"{DbTable.F_TEST_RESULT_HISTORY.productSN}," +
                $"{DbTable.F_TEST_RESULT_HISTORY.bindState} " +
                $"FROM {DbTable.F_TEST_RESULT_HISTORY_NAME} WHERE " +
                $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{pcba}' ORDER BY " +
                $"{DbTable.F_TEST_RESULT_HISTORY.id} DESC";
            var dtPcbaData = SQLServer.ExecuteDataSet(selectPCBALastestSQL).Tables[0];
            if (dtPcbaData.Rows.Count > 0)
            {
                //查询该工站是否有数据
                var stationSQL = $"SELECT TOP 1 {DbTable.F_TEST_RESULT_HISTORY.sensibilityStationName} " +
                $"FROM {DbTable.F_TEST_RESULT_HISTORY_NAME} WHERE " +
                $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{pcba}' AND " +
                $"{DbTable.F_TEST_RESULT_HISTORY.sensibilityStationName} = '{station}' " +
                $"ORDER BY {DbTable.F_TEST_RESULT_HISTORY.id} DESC";
                var dtStationData = SQLServer.ExecuteDataSet(stationSQL).Tables[0];
                if (dtStationData.Rows.Count > 0)
                {
                    testResult.pcbaTestResultStatusEnum = PcbaTestResultStatusEnum.currentStationExist;
                    testResult.productSN = dtPcbaData.Rows[0][1].ToString();
                    testResult.bindState = dtPcbaData.Rows[0][2].ToString();
                    return testResult;
                }
                testResult.pcbaTestResultStatusEnum =  PcbaTestResultStatusEnum.currentStationNotExist;
                testResult.pcbaID = dtPcbaData.Rows[0][0].ToString();
                return testResult;
            }
            testResult.pcbaTestResultStatusEnum =  PcbaTestResultStatusEnum.pcbaNotExist;
            return testResult;
        }

        /// <summary>
        /// 查询外壳工站是否存在记录
        /// </summary>
        /// <param name="pcba"></param>
        /// <param name="station"></param>
        /// <returns></returns>
        private static TestResult IsExistShellStationRecord(string pcba, string station)
        {
            //根据PCBA查询最新记录
            //存在-查询当前工站，是否已插入数据
            //不存在数据，则更新上去；否则插入数据
            TestResult testResult = new TestResult();
            var selectPCBALastestSQL = $"SELECT TOP 1 {DbTable.F_TEST_RESULT_HISTORY.id}," +
                $"{DbTable.F_TEST_RESULT_HISTORY.productSN},{DbTable.F_TEST_RESULT_HISTORY.bindState} " +
                $"FROM {DbTable.F_TEST_RESULT_HISTORY_NAME} WHERE " +
                $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{pcba}' ORDER BY " +
                $"{DbTable.F_TEST_RESULT_HISTORY.id} DESC";
            var dtPcbaData = SQLServer.ExecuteDataSet(selectPCBALastestSQL).Tables[0];
            if (dtPcbaData.Rows.Count > 0)
            {
                //查询该工站是否有数据
                var stationSQL = $"SELECT TOP 1 {DbTable.F_TEST_RESULT_HISTORY.shellStationName} " +
                $"FROM {DbTable.F_TEST_RESULT_HISTORY_NAME} WHERE " +
                $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN} = '{pcba}' AND " +
                $"{DbTable.F_TEST_RESULT_HISTORY.shellStationName} = '{station}' " +
                 $"ORDER BY {DbTable.F_TEST_RESULT_HISTORY.id} DESC";
                var dtStationData = SQLServer.ExecuteDataSet(stationSQL).Tables[0];
                if (dtStationData.Rows.Count > 0)
                {
                    testResult.pcbaTestResultStatusEnum = PcbaTestResultStatusEnum.currentStationExist;
                    testResult.productSN = dtPcbaData.Rows[0][1].ToString();
                    testResult.bindState = dtPcbaData.Rows[0][2].ToString();
                    return testResult;
                }
                testResult.pcbaTestResultStatusEnum =  PcbaTestResultStatusEnum.currentStationNotExist;
                testResult.pcbaID = dtPcbaData.Rows[0][0].ToString();
                return testResult;
            }
            testResult.pcbaTestResultStatusEnum =  PcbaTestResultStatusEnum.pcbaNotExist;
            return testResult;
        }

        /// <summary>
        /// 查询气密工站是否存在记录
        /// </summary>
        /// <param name="pcba"></param>
        /// <param name="station"></param>
        /// <returns></returns>
        private static TestResult IsExistAirtageStationRecord(string sn, string station)
        {
            //根据PCBA查询最新记录
            //存在-查询当前工站，是否已插入数据
            //不存在数据，则更新上去；否则插入数据
            TestResult testResult = new TestResult();
            var selectPCBALastestSQL = $"SELECT TOP 1 {DbTable.F_TEST_RESULT_HISTORY.id}," +
                $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN},{DbTable.F_TEST_RESULT_HISTORY.bindState} " +
                $"FROM {DbTable.F_TEST_RESULT_HISTORY_NAME} WHERE " +
                $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{sn}' ORDER BY " +
                $"{DbTable.F_TEST_RESULT_HISTORY.id} DESC";
            var dtPcbaData = SQLServer.ExecuteDataSet(selectPCBALastestSQL).Tables[0];
            if (dtPcbaData.Rows.Count > 0)
            {
                //查询该工站是否有数据
                var stationSQL = $"SELECT TOP 1 {DbTable.F_TEST_RESULT_HISTORY.airtageStationName} " +
                $"FROM {DbTable.F_TEST_RESULT_HISTORY_NAME} WHERE " +
                $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{sn}' AND " +
                $"{DbTable.F_TEST_RESULT_HISTORY.airtageStationName} = '{station}' " +
                $"ORDER BY {DbTable.F_TEST_RESULT_HISTORY.id} DESC";
                var dtStationData = SQLServer.ExecuteDataSet(stationSQL).Tables[0];
                if (dtStationData.Rows.Count > 0)
                {
                    testResult.pcbaTestResultStatusEnum = PcbaTestResultStatusEnum.currentStationExist;
                    testResult.pcbaSN = dtPcbaData.Rows[0][1].ToString();
                    testResult.bindState = dtPcbaData.Rows[0][2].ToString();
                    return testResult;
                }
                testResult.pcbaTestResultStatusEnum =  PcbaTestResultStatusEnum.currentStationNotExist;
                testResult.pcbaID = dtPcbaData.Rows[0][0].ToString();
                return testResult;
            }
            testResult.pcbaTestResultStatusEnum =  PcbaTestResultStatusEnum.pcbaNotExist;
            return testResult;
        }

        /// <summary>
        /// 查询支架工站是否存在记录
        /// </summary>
        /// <param name="pcba"></param>
        /// <param name="station"></param>
        /// <returns></returns>
        private static TestResult IsExistStentStationRecord(string sn, string station)
        {
            //根据PCBA查询最新记录
            //存在-查询当前工站，是否已插入数据
            //不存在数据，则更新上去；否则插入数据
            TestResult testResult = new TestResult();
            var selectPCBALastestSQL = $"SELECT TOP 1 {DbTable.F_TEST_RESULT_HISTORY.id}," +
                $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN},{DbTable.F_TEST_RESULT_HISTORY.bindState} " +
                $"FROM {DbTable.F_TEST_RESULT_HISTORY_NAME} WHERE " +
                $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{sn}' ORDER BY " +
                $"{DbTable.F_TEST_RESULT_HISTORY.id} DESC";
            var dtPcbaData = SQLServer.ExecuteDataSet(selectPCBALastestSQL).Tables[0];
            if (dtPcbaData.Rows.Count > 0)
            {
                //查询该工站是否有数据
                var stationSQL = $"SELECT TOP 1 {DbTable.F_TEST_RESULT_HISTORY.stentStationName} " +
                $"FROM {DbTable.F_TEST_RESULT_HISTORY_NAME} WHERE " +
                $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{sn}' AND " +
                $"{DbTable.F_TEST_RESULT_HISTORY.stentStationName} = '{station}' " +
                 $"ORDER BY {DbTable.F_TEST_RESULT_HISTORY.id} DESC";
                var dtStationData = SQLServer.ExecuteDataSet(stationSQL).Tables[0];
                if (dtStationData.Rows.Count > 0)
                {
                    testResult.pcbaTestResultStatusEnum = PcbaTestResultStatusEnum.currentStationExist;
                    testResult.pcbaSN = dtPcbaData.Rows[0][1].ToString();
                    testResult.bindState = dtPcbaData.Rows[0][2].ToString();
                    return testResult;
                }
                testResult.pcbaTestResultStatusEnum =  PcbaTestResultStatusEnum.currentStationNotExist;
                testResult.pcbaID = dtPcbaData.Rows[0][0].ToString();
                return testResult;
            }
            testResult.pcbaTestResultStatusEnum =  PcbaTestResultStatusEnum.pcbaNotExist;
            return testResult;
        }

        /// <summary>
        /// 查询成品工站是否存在记录
        /// </summary>
        /// <param name="pcba"></param>
        /// <param name="station"></param>
        /// <returns></returns>
        private static TestResult IsExistProductStationRecord(string sn, string station)
        {
            //根据PCBA查询最新记录
            //存在-查询当前工站，是否已插入数据
            //不存在数据，则更新上去；否则插入数据
            TestResult testResult = new TestResult();
            var selectPCBALastestSQL = $"SELECT TOP 1 {DbTable.F_TEST_RESULT_HISTORY.id}," +
                $"{DbTable.F_TEST_RESULT_HISTORY.pcbaSN},{DbTable.F_TEST_RESULT_HISTORY.bindState} " +
                $"FROM {DbTable.F_TEST_RESULT_HISTORY_NAME} WHERE " +
                $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{sn}' ORDER BY " +
                $"{DbTable.F_TEST_RESULT_HISTORY.id} DESC";
            var dtPcbaData = SQLServer.ExecuteDataSet(selectPCBALastestSQL).Tables[0];
            if (dtPcbaData.Rows.Count > 0)
            {
                //查询该工站是否有数据
                var stationSQL = $"SELECT TOP 1 {DbTable.F_TEST_RESULT_HISTORY.productStationName} " +
                $"FROM {DbTable.F_TEST_RESULT_HISTORY_NAME} WHERE " +
                $"{DbTable.F_TEST_RESULT_HISTORY.productSN} = '{sn}' AND " +
                $"{DbTable.F_TEST_RESULT_HISTORY.productStationName} = '{station}' " +
                 $"ORDER BY {DbTable.F_TEST_RESULT_HISTORY.id} DESC";
                var dtStationData = SQLServer.ExecuteDataSet(stationSQL).Tables[0];
                if (dtStationData.Rows.Count > 0)
                {
                    testResult.pcbaTestResultStatusEnum = PcbaTestResultStatusEnum.currentStationExist;
                    testResult.pcbaSN = dtPcbaData.Rows[0][1].ToString();
                    testResult.bindState = dtPcbaData.Rows[0][2].ToString();
                    return testResult;
                }
                testResult.pcbaTestResultStatusEnum =  PcbaTestResultStatusEnum.currentStationNotExist;
                testResult.pcbaID = dtPcbaData.Rows[0][0].ToString();
                return testResult;
            }
            testResult.pcbaTestResultStatusEnum =  PcbaTestResultStatusEnum.pcbaNotExist;
            return testResult;
        }
        #endregion
        
        private static int UpdateTestResult(string sn ,string typeNo,string station,
            string result,string teamder,string admin,string joinDateTime)
        {
            var stationInTime = SelectStationInDate(sn,station,typeNo);
            var updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_NAME} SET {DbTable.F_Test_Result.TEST_RESULT} = '{result}'," +
                $"{DbTable.F_Test_Result.TEAM_LEADER} = '{teamder}'," +
                $"{DbTable.F_Test_Result.ADMIN} = '{admin}'," +
                $"{DbTable.F_Test_Result.STATION_OUT_DATE} = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'," +
                $"{DbTable.F_Test_Result.JOIN_DATE_TIME} = '{joinDateTime}' " +
                $"WHERE " +
                $"{DbTable.F_Test_Result.SN} = '{sn}' AND " +
                $"{DbTable.F_Test_Result.TYPE_NO} = '{typeNo}' AND " +
                $"{DbTable.F_Test_Result.STATION_NAME} = '{station}' AND " +
                $"{DbTable.F_Test_Result.STATION_IN_DATE} = '{stationInTime}'";
            LogHelper.Log.Info("【更新出站记录】"+updateSQL);
            return SQLServer.ExecuteNonQuery(updateSQL);
        }

        private static string SelectStationInDate(string sn,string stationName,string typeNo)
        {
            var selectSQL = $"SELECT TOP 1 {DbTable.F_Test_Result.STATION_IN_DATE} FROM " +
                $"{DbTable.F_TEST_RESULT_NAME} " +
                $"WHERE " +
                $"{DbTable.F_Test_Result.SN} = '{sn}' AND " +
                $"{DbTable.F_Test_Result.STATION_NAME} = '{stationName}' AND " +
                $"{DbTable.F_Test_Result.TYPE_NO} = '{typeNo}' " +
                $"ORDER BY {DbTable.F_Test_Result.STATION_IN_DATE} DESC";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            LogHelper.Log.Info("【查询进站日期】"+selectSQL);
            if (dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();
            return "";
        }

        private static bool IsExistTestResult(string typeNo,string station,string dateTime,string result)
        {
            string selectSQL = $"SELECT * FROM {DbTable.F_TEST_RESULT_NAME} WHER " +
                    $"{DbTable.F_Test_Result.TYPE_NO} = '{typeNo}' AND " +
                    $"{DbTable.F_Test_Result.STATION_NAME} = '{station}' AND " +
                    $"{DbTable.F_Test_Result.UPDATE_DATE} = '{dateTime}' AND " +
                    $"{DbTable.F_Test_Result.TEST_RESULT} = '{result}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }

        public static string[] SelectTestResult(Queue<string[]> queue)
        {
            string[] queryResult;
            try
            {
                string[] array = queue.Dequeue();
                string sn = array[0];
                string currentStation = array[1];
                var currentDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                LogHelper.Log.Info("测试端查询测试结果,站位为" + currentStation + " SN="+sn);
                //根据当前工艺与站位，查询其上一站位
                //查询当前工艺流程
                MesService mesService = new MesService();
                //判断是不是第一个工站
                var processName = mesService.SelectCurrentTProcess();
                LogHelper.Log.Info("【查询工艺结果】"+processName);
                if (IsFirstStation(processName, currentStation))
                {
                    //插入进站记录
                    int row = InsertTestResult(sn, processName, currentStation);
                    if (row > 0)
                    {
                        UpdateTestResultHistory(sn,processName,currentStation,currentDate,currentDate);
                        LogHelper.Log.Info("【插入进站记录-第一站】成功");
                    }
                    else
                    {
                        LogHelper.Log.Info("【插入进站记录-第一站】失败");
                    }
                    //默认返回结果为PASS
                    LogHelper.Log.Info("【第一站-查询结果】PASS");
                    queryResult = new string[3];
                    queryResult[0] = sn;
                    queryResult[1] = currentStation;
                    queryResult[2] = "PASS";
                    return queryResult;
                }
                string selectOrderSQL = $"SELECT {DbTable.F_TECHNOLOGICAL_PROCESS.STATION_ORDER} " +
                    $"FROM {DbTable.F_TECHNOLOGICAL_PROCESS_NAME} " +
                    $"WHERE {DbTable.F_TECHNOLOGICAL_PROCESS.STATION_NAME} = '{currentStation}' AND " +
                    $"{DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME} = '{processName}'";
                LogHelper.Log.Info("【查询当前站ID】"+selectOrderSQL);
                DataTable dt = SQLServer.ExecuteDataSet(selectOrderSQL).Tables[0];
                if (dt.Rows.Count < 1)
                {
                    queryResult = new string[1];
                    queryResult[0] = "ERR_LAST_STATION_ID";
                    return queryResult;
                }
                int currentID = int.Parse(dt.Rows[0][0].ToString());
                LogHelper.Log.Info("查询当前工站ID="+currentID);
                int lastOrder = currentID - 1;
                LogHelper.Log.Info($"【上一站ID】={lastOrder}");
                selectOrderSQL = $"SELECT {DbTable.F_TECHNOLOGICAL_PROCESS.STATION_NAME} " +
                    $"FROM {DbTable.F_TECHNOLOGICAL_PROCESS_NAME} " +
                    $"WHERE {DbTable.F_TECHNOLOGICAL_PROCESS.STATION_ORDER} = '{lastOrder}' AND " +
                    $"{DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME} = '{processName}'";
                dt = SQLServer.ExecuteDataSet(selectOrderSQL).Tables[0];
                if (dt.Rows.Count < 1)
                {
                    queryResult = new string[1];
                    queryResult[0] = "ERR_LAST_STATION_NAME";
                    return queryResult;
                }
                var lastStation = dt.Rows[0][0].ToString();
                LogHelper.Log.Info($"【上一站工站】={lastStation}");
                //由外壳码查询关联SN
                /*
                 * 由传入SN查询是否绑定PCBA
                 * 查询有结果-已绑定PCBA：
                 *  1）装配时存入的是PCBA
                 *  2）装配时存入的是外壳SN
                 * 查询无结果-未绑定PCBA：直接由传入SN查询
                 */ 
                var snPCBA = SelectSN(sn);//不为空-正常绑定
                LogHelper.Log.Info("【查询是否绑定PCBA】snPCBA="+snPCBA+"--判断是否有解绑记录");
                if (snPCBA == "")//查询无正常绑定记录
                {
                    /*
                     * 1)无绑定记录：继续下一步查询
                     * 2）已解绑--PCBA异常/外壳异常：
                     * 1、PCBA异常：
                     *      1）传入PCBA时：查询出PCBA异常--不能继续
                     *      2）传入外壳时：查询出PCBA异常--不能继续
                     * 2、外壳异常：
                     *      1)传入PCBA时：
                     *      2)传入外壳时：
                     *      
                     */
                    if (IsExistBindRecord(sn))
                    {
                        //存在绑定记录：可能正常/异常
                        if (IsBindedShellExcept(sn))//判断是否有异常绑定记录
                        {
                            LogHelper.Log.Info("【查询是否有解绑记录】存在异常解绑--外壳异常");
                            queryResult = new string[3];
                            queryResult[0] = sn;
                            queryResult[1] = lastStation;
                            queryResult[2] = "FAIL";
                            return queryResult;
                        }
                    }
                }
                //根据上一站位在查询该站位的最后一条记录
                string selectSQL = $"SELECT " +
                    $"{DbTable.F_Test_Result.TEST_RESULT}," +
                    $"{DbTable.F_Test_Result.SN}," +
                    $"{DbTable.F_Test_Result.STATION_NAME}," +
                    $"{DbTable.F_Test_Result.TYPE_NO}," +
                    $"{DbTable.F_Test_Result.STATION_IN_DATE} " +
                    $"FROM " +
                    $"{DbTable.F_TEST_RESULT_NAME} " +
                    $"WHERE " +
                    $"{DbTable.F_Test_Result.SN} = '{sn}' AND " +
                    $"{DbTable.F_Test_Result.STATION_NAME} = '{lastStation}' " +
                    $"ORDER BY " +
                    $"{DbTable.F_Test_Result.UPDATE_DATE} " +
                    $"DESC ";
                LogHelper.Log.Info("【第一次SN查询】"+selectSQL);
                dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                if (dt.Rows.Count < 1)
                {
                    //查询失败
                    //尝试用PCBA去查询
                    LogHelper.Log.Info("【SN查询上一站数据结果为空，】");
                    selectSQL = $"SELECT {DbTable.F_Test_Result.TEST_RESULT}," +
                    $"{DbTable.F_Test_Result.SN}," +
                    $"{DbTable.F_Test_Result.STATION_NAME}," +
                    $"{DbTable.F_Test_Result.TYPE_NO}," +
                    $"{DbTable.F_Test_Result.STATION_IN_DATE}" +
                    $"FROM " +
                    $"{DbTable.F_TEST_RESULT_NAME} " +
                    $"WHERE " +
                    $"{DbTable.F_Test_Result.SN} = '{snPCBA}' AND " +
                    $"{DbTable.F_Test_Result.STATION_NAME} = '{lastStation}' " +
                    $"ORDER BY " +
                    $"{DbTable.F_Test_Result.UPDATE_DATE} " +
                    $"DESC ";
                    dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                    LogHelper.Log.Info("【第二次SN查询】"+selectSQL);
                    if (dt.Rows.Count < 1)
                    {
                        //当PCBA未完成绑定时，两次查询均会失败
                        /*
                         * 进一步判断
                         * 1）是否没有外壳装配工站
                         * 2）没有，则查询烧录工站/灵敏度工站是否 存在的四种情况
                         * 3）根据其ID+1/+2判断临界点工站ID
                         * 4）若当前工站的ID=临界点ID 默认通过，同时插入进站记录
                         * 5）若当前工站的ID>临界点ID 则不通过
                         */
                        LogHelper.Log.Info("【第二次SN查询】-无结果，判断是否包含外壳装配工站");
                        var stationList = new MesService().SelectStationList(processName);
                        if (!stationList.Contains("外壳装配工站"))
                        {
                            LogHelper.Log.Info("【第二次SN查询】查询结果-不包含外壳装配工站---查询临界点工站ID");
                            var criticalID = CalCirticalStationID();
                            if (currentID == criticalID)
                            {
                                LogHelper.Log.Info("查询当前工站ID等于临界点ID---默认通过--开始插入进站记录");
                                int row = InsertTestResult(sn, processName, currentStation);
                                if (row > 0)
                                {
                                    UpdateTestResultHistory(sn, processName, currentStation,currentDate,currentDate);
                                    LogHelper.Log.Info("【插入进站记录-】成功");
                                }
                                else
                                {
                                    LogHelper.Log.Info("【插入进站记录-】失败");
                                }
                                queryResult = new string[3];
                                queryResult[0] = sn;
                                queryResult[1] = lastStation;
                                queryResult[2] = "PASS";
                                return queryResult;
                            }
                        }
                        //包含绑定工站，即是绑定失败等原因
                        queryResult = new string[1];
                        queryResult[0] = "QUERY_NONE";
                        return queryResult;
                    }
                }
                var testRes = dt.Rows[0][0].ToString();
                //var productSN = dt.Rows[0][1].ToString();
                //var stationName = dt.Rows[0][2].ToString();
                var productTypeNo = dt.Rows[0][3].ToString();
                //var stationInDate = dt.Rows[0][4].ToString();
                //查询结果为PASS后，插入过站进站记录
                if (testRes.Trim().ToLower() == "pass")
                {
                    LogHelper.Log.Info("【查询结果为PASS，插入进站记录】");
                    int row = InsertTestResult(sn,processName,currentStation);
                    if (row > 0)
                    {
                        UpdateTestResultHistory(sn, processName, currentStation,currentDate,currentDate);
                        LogHelper.Log.Info("【插入进站记录】成功");
                    }
                    else
                    {
                        LogHelper.Log.Info("【插入进站记录】失败");
                    }
                }
                else
                {
                    LogHelper.Log.Info("【查询结果不为PASS，不插入进站记录】"+testRes);
                }
                //返回上一个站位的最后一条记录
                queryResult = new string[3];
                queryResult[0] = sn;
                queryResult[1] = lastStation;
                queryResult[2] = testRes;
                return queryResult;
            }
            catch (Exception ex)
            {
                queryResult = new string[1];
                queryResult[0] = "ERR_EXCEPTION";
                LogHelper.Log.Error(ex.Message+"\r\n"+ex.StackTrace);
                return queryResult;
            }
        }

        public static string SelectSN(string snOutter)
        {
            /*
             * 查询是否有绑定关系
             * 1）不存在绑定记录
             * 2）存在绑定记录-已绑定
             * 3）存在绑定记录-已解绑--PCBA异常/外壳异常
             */ 
            //两种情况
            //sn= snoutter;
            var selectSQL = $"SELECT {DbTable.F_BINDING_PCBA.SN_PCBA} FROM  {DbTable.F_BINDING_PCBA_NAME} " +
                $"WHERE " +
                $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{snOutter}' " +
                $"AND " +
                $"{DbTable.F_BINDING_PCBA.BINDING_STATE} = '1'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            //sn= snPcba
            selectSQL = $"SELECT {DbTable.F_BINDING_PCBA.SN_OUTTER} FROM {DbTable.F_BINDING_PCBA_NAME} " +
                $"WHERE " +
                $"{DbTable.F_BINDING_PCBA.SN_PCBA} = '{snOutter}' AND " +
                $"{DbTable.F_BINDING_PCBA.BINDING_STATE} = '1'";

            dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();
            return "";
        }

        private static bool IsBindedShellExcept(string sn)
        {
            var selectSQL = $"SELECT " +
                $"{DbTable.F_BINDING_PCBA.PCBA_STATE}," +
                $"{DbTable.F_BINDING_PCBA.OUTTER_STATE} " +
                $"FROM " +
                $"{DbTable.F_BINDING_PCBA_NAME} " +
                $"WHERE " +
                $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{sn}'" +
                $"OR " +
                $"{DbTable.F_BINDING_PCBA.SN_PCBA} = '{sn}' " +
                $"AND " +
                $"{DbTable.F_BINDING_PCBA.PCBA_STATE} = '0' ";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                //存在绑定记录---已解绑-外壳异常
                return true;
            }
            else
            {
                //外壳异常情况
                //根据传入SN不同
                //查询PCBA状态
                selectSQL = $"SELECT " +
                    $"{DbTable.F_BINDING_PCBA.PCBA_STATE}," +
                    $"{DbTable.F_BINDING_PCBA.OUTTER_STATE} " +
                    $"FROM " +
                    $"{DbTable.F_BINDING_PCBA_NAME} " +
                    $"WHERE " +
                    $"{DbTable.F_BINDING_PCBA.SN_PCBA} = '{sn}'" +
                    $"AND " +
                    $"{DbTable.F_BINDING_PCBA.PCBA_STATE} = '1' "; 
                dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    return false;
                }
                else
                {
                    //查询外壳状态
                    selectSQL = $"SELECT " +
                    $"{DbTable.F_BINDING_PCBA.PCBA_STATE}," +
                    $"{DbTable.F_BINDING_PCBA.OUTTER_STATE} " +
                    $"FROM " +
                    $"{DbTable.F_BINDING_PCBA_NAME} " +
                    $"WHERE " +
                    $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{sn}'" +
                    $"AND " +
                    $"{DbTable.F_BINDING_PCBA.OUTTER_STATE} = '0' ";
                    dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                    if (dt.Rows.Count > 0)
                        return true;
                }
            }
            return false;
        }

        private static bool IsExistBindRecord(string sn)
        {
            var selectSQL = $"SELECT * FROM {DbTable.F_BINDING_PCBA_NAME} WHERE " +
                $"{DbTable.F_BINDING_PCBA.SN_PCBA} = '{sn}' " +
                $"OR " +
                $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{sn}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }

        private static bool IsFirstStation(string typeNo,string stationName)
        {
            var selectSQL = $"SELECT {DbTable.F_TECHNOLOGICAL_PROCESS.STATION_ORDER} " +
                $"FROM " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS_NAME} " +
                $"WHERE " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME} = '{typeNo}' " +
                $"AND " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.STATION_NAME} = '{stationName}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                var stationOrder = dt.Rows[0][0].ToString();
                if (stationOrder == "1")
                    return true;//是第一个工站
            }
            return false;
        }

        private static bool IsFirstStationLastTest(string sn,string typeNo,string stationName)
        {
            var selectSQL = $"SELECT {DbTable.F_Test_Result.TEST_RESULT}," +
                $"{DbTable.F_Test_Result.STATION_IN_DATE} " +
                $"FROM " +
                $"{DbTable.F_TEST_RESULT_NAME} " +
                $"WHERE " +
                $"{DbTable.F_Test_Result.SN} = '{sn}' AND " +
                $"{DbTable.F_Test_Result.TYPE_NO} = '{typeNo}' AND " +
                $"{DbTable.F_Test_Result.STATION_NAME} = '{stationName}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                var testResult = dt.Rows[0][0].ToString().Trim();
                var stationINDate = dt.Rows[0][1].ToString().Trim();
                if (testResult == "" && stationINDate != "")
                {
                    return true;
                }
            }
            return false;
        }

        private static int CalCirticalStationID()
        {
            //计算当前工艺的临界点ID
            //查询烧录工站是否存在
            //查询灵敏度工站是否存在
            int cirticalID = 0;
            if (IsExistStation("烧录工站"))
            {
                //存在烧录工站
                cirticalID += 1;
                if (IsExistStation("灵敏度测试工站"))
                {
                    cirticalID += 1;
                }
            }
            else
            {
                if (IsExistStation("灵敏度测试工站"))
                {
                    cirticalID += 1;
                }
            }
            return cirticalID + 1;
        }

        private static bool IsExistStation(string stationName)
        {
            var currentProcess = new MesService().SelectCurrentTProcess();
            var selectSQL = $"SELECT * FROM {DbTable.F_TECHNOLOGICAL_PROCESS_NAME} " +
                $"WHERE " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME} = '{currentProcess}' " +
                $"AND " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.STATION_NAME} = '{stationName}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL);
            if (dt.Tables.Count > 0)
            {
                if (dt.Tables[0].Rows.Count > 0)
                    return true;
            }
            return false;
        }
    }
}