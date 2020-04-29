using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CommonUtils.Logger;
using CommonUtils.DB;
using CommonUtils.FileHelper;
using MesManager.Model;
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections;
using System.Configuration;
using System.Windows.Forms;


namespace MesManager.DB
{
    public class TestResultQuery
    {
        //烧录工位/灵敏度工位/外壳工位/气密工位/支架装配工位/成品测试工位
        private static string STATION_TURN = "烧录工站";
        private static string STATION_SENSIBLITY = "灵敏度测试工站";
        private static string STATION_SHELL = "外壳装配工站";
        private static string STATION_AIR = "气密测试工站";
        private static string STATION_STENT = "支架装配工站";
        private static string STATION_PRODUCT = "成品测试工站";
        private static List<TestResultHistory> pcbaCacheList = new List<TestResultHistory>();//用于缓存pcba数据
        private static DataTable pcbaCacheDataSource = new DataTable();//用于缓存PCBA的所有数据信息
        private static DataTable testResultDataSource = new DataTable();


        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="querySN"></param>
        /// <param name="startIndex"></param>
        /// <param name="dCount"></param>
        /// <param name="IsQueryLatest"></param>
        /// <returns></returns>
        public static TestResultHistory SelectTestResultDetail(string querySN, int pageIndex, int pageSize)
        {
            LogHelper.Log.Info("开始查询");
            testResultDataSource = InitTestResultDataTable(true);
            return SelectUseAllPcbaSN(querySN, pageIndex, pageSize);//查询所有数据
        }

        private static void SelectTestResultDetail1(TestResultHistory testResult,int count)
        {
            #region 查询明细
            DataRow dr = testResultDataSource.NewRow();
            var pcbsn = testResult.PcbaSN;//GetPCBASn(pcbaSN);
            var productsn = GetProductSn(testResult.PcbaSN);
            //GetProductSNOfShell(testRestul.PcbaSN);

            dr[TestResultItemContent.Order] = count;
            dr[TestResultItemContent.PcbaSN] = pcbsn;
            dr[TestResultItemContent.ProductSN] = productsn;
            dr[TestResultItemContent.FinalResultValue] = GetProductTestFinalResult(pcbsn, productsn, testResult.ShellCodeLen, testResult.ProductTypeNo);
            var currentProductType = testResult.ProductTypeNo;//GetProductTypeNoOfSN(pcbsn, productsn);
            dr[TestResultItemContent.ProductTypeNo] = currentProductType;

            #region 烧录工位信息
            var testResultTurn = SelectTestResultOfSN(pcbsn, productsn, STATION_TURN);
            //dr[TestResultItemContent.StationName_turn] = STATION_TURN;
            dr[STATION_TURN + TestResultItemContent.StationInDate_turn] = testResultTurn.StationInDate;
            dr[STATION_TURN + TestResultItemContent.StationOutDate_turn] = testResultTurn.StationOutDate;
            dr[STATION_TURN + TestResultItemContent.UserTeamLeader_turn] = testResultTurn.UserTeamLeader;
            dr[STATION_TURN + TestResultItemContent.TestResultValue_turn] = testResultTurn.TestResultValue;
            dr[STATION_TURN + TestResultItemContent.Turn_TurnItem] = SelectTestItemValue(pcbsn, productsn, STATION_TURN, TestResultItemContent.Turn_TurnItem);
            dr[STATION_TURN + TestResultItemContent.Turn_Voltage_12V_Item] = SelectTestItemValue(pcbsn, productsn, STATION_TURN, TestResultItemContent.Turn_Voltage_12V_Item);
            dr[STATION_TURN + TestResultItemContent.Turn_Voltage_5V_Item] = SelectTestItemValue(pcbsn, productsn, STATION_TURN, TestResultItemContent.Turn_Voltage_5V_Item);
            dr[STATION_TURN + TestResultItemContent.Turn_Voltage_33_1V_Item] = SelectTestItemValue(pcbsn, productsn, STATION_TURN, TestResultItemContent.Turn_Voltage_33_1V_Item);
            dr[STATION_TURN + TestResultItemContent.Turn_Voltage_33_2V_Item] = SelectTestItemValue(pcbsn, productsn, STATION_TURN, TestResultItemContent.Turn_Voltage_33_2V_Item);
            dr[STATION_TURN + TestResultItemContent.Turn_SoftVersion] = SelectTestItemValue(pcbsn, productsn, STATION_TURN, TestResultItemContent.Turn_SoftVersion);
            #endregion

            #region 灵敏度
            var testResultSen = SelectTestResultOfSN(pcbsn, productsn, STATION_SENSIBLITY);
            //dr[TestResultItemContent.StationName_sen] = STATION_SENSIBLITY;
            dr[STATION_SENSIBLITY + TestResultItemContent.StationInDate_sen] = testResultSen.StationInDate;
            dr[STATION_SENSIBLITY + TestResultItemContent.StationOutDate_sen] = testResultSen.StationOutDate;
            dr[STATION_SENSIBLITY + TestResultItemContent.UserTeamLeader_sen] = testResultSen.UserTeamLeader;
            dr[STATION_SENSIBLITY + TestResultItemContent.TestResultValue_sen] = testResultSen.TestResultValue;

            dr[STATION_SENSIBLITY + TestResultItemContent.Sen_Work_Electric_Test] = SelectTestItemValue(pcbsn, productsn, STATION_SENSIBLITY, TestResultItemContent.Sen_Work_Electric_Test);
            dr[STATION_SENSIBLITY + TestResultItemContent.Sen_PartNumber] = SelectTestItemValue(pcbsn, productsn, STATION_SENSIBLITY, TestResultItemContent.Sen_PartNumber);
            dr[STATION_SENSIBLITY + TestResultItemContent.Sen_HardWareVersion] = SelectTestItemValue(pcbsn, productsn, STATION_SENSIBLITY, TestResultItemContent.Sen_HardWareVersion);
            dr[STATION_SENSIBLITY + TestResultItemContent.Sen_SoftVersion] = SelectTestItemValue(pcbsn, productsn, STATION_SENSIBLITY, TestResultItemContent.Sen_SoftVersion);
            dr[STATION_SENSIBLITY + TestResultItemContent.Sen_ECUID] = SelectTestItemValue(pcbsn, productsn, STATION_SENSIBLITY, TestResultItemContent.Sen_ECUID);
            dr[STATION_SENSIBLITY + TestResultItemContent.Sen_BootloaderVersion] = SelectTestItemValue(pcbsn, productsn, STATION_SENSIBLITY, TestResultItemContent.Sen_BootloaderVersion);
            dr[STATION_SENSIBLITY + TestResultItemContent.Sen_RadioFreq] = SelectTestItemValue(pcbsn, productsn, STATION_SENSIBLITY, TestResultItemContent.Sen_RadioFreq);
            dr[STATION_SENSIBLITY + TestResultItemContent.Sen_DormantElect] = SelectTestItemValue(pcbsn, productsn, STATION_SENSIBLITY, TestResultItemContent.Sen_DormantElect);
            #endregion

            #region 外壳
            var testResultShell = SelectTestResultOfSN(pcbsn, productsn, STATION_SHELL);
            //dr[TestResultItemContent.StationName_shell] = STATION_SHELL;
            dr[STATION_SHELL + TestResultItemContent.StationInDate_shell] = testResultShell.StationInDate;
            dr[STATION_SHELL + TestResultItemContent.StationOutDate_shell] = testResultShell.StationOutDate;
            dr[STATION_SHELL + TestResultItemContent.UserTeamLeader_shell] = testResultShell.UserTeamLeader;
            dr[STATION_SHELL + TestResultItemContent.TestResultValue_shell] = testResultShell.TestResultValue;
            dr[STATION_SHELL + TestResultItemContent.Shell_FrontCover] = SelectTestItemValue(pcbsn, productsn, STATION_SHELL, TestResultItemContent.Shell_FrontCover);
            dr[STATION_SHELL + TestResultItemContent.Shell_BackCover] = SelectTestItemValue(pcbsn, productsn, STATION_SHELL, TestResultItemContent.Shell_BackCover);
            dr[STATION_SHELL + TestResultItemContent.Shell_PCBScrew1] = SelectTestItemValue(pcbsn, productsn, STATION_SHELL, TestResultItemContent.Shell_PCBScrew1);
            dr[STATION_SHELL + TestResultItemContent.Shell_PCBScrew2] = SelectTestItemValue(pcbsn, productsn, STATION_SHELL, TestResultItemContent.Shell_PCBScrew2);
            dr[STATION_SHELL + TestResultItemContent.Shell_PCBScrew3] = SelectTestItemValue(pcbsn, productsn, STATION_SHELL, TestResultItemContent.Shell_PCBScrew3);
            dr[STATION_SHELL + TestResultItemContent.Shell_PCBScrew4] = SelectTestItemValue(pcbsn, productsn, STATION_SHELL, TestResultItemContent.Shell_PCBScrew4);
            dr[STATION_SHELL + TestResultItemContent.Shell_ShellScrew1] = SelectTestItemValue(pcbsn, productsn, STATION_SHELL, TestResultItemContent.Shell_ShellScrew1);
            dr[STATION_SHELL + TestResultItemContent.Shell_ShellScrew2] = SelectTestItemValue(pcbsn, productsn, STATION_SHELL, TestResultItemContent.Shell_ShellScrew2);
            dr[STATION_SHELL + TestResultItemContent.Shell_ShellScrew3] = SelectTestItemValue(pcbsn, productsn, STATION_SHELL, TestResultItemContent.Shell_ShellScrew3);
            dr[STATION_SHELL + TestResultItemContent.Shell_ShellScrew4] = SelectTestItemValue(pcbsn, productsn, STATION_SHELL, TestResultItemContent.Shell_ShellScrew4);

            #endregion

            #region 气密
            var testResultAir = SelectTestResultOfSN(pcbsn, productsn, STATION_AIR);
            //dr[TestResultItemContent.StationName_air] = STATION_AIR;
            dr[STATION_AIR + TestResultItemContent.StationInDate_air] = testResultAir.StationInDate;
            dr[STATION_AIR + TestResultItemContent.StationOutDate_air] = testResultAir.StationOutDate;
            dr[STATION_AIR + TestResultItemContent.UserTeamLeader_air] = testResultAir.UserTeamLeader;
            dr[STATION_AIR + TestResultItemContent.TestResultValue_air] = testResultAir.TestResultValue;
            dr[STATION_AIR + TestResultItemContent.Air_AirtightTest] = SelectTestItemValue(pcbsn, productsn, STATION_AIR, TestResultItemContent.Air_AirtightTest);
            #endregion

            #region 支架
            var testResultStent = SelectTestResultOfSN(pcbsn, productsn, STATION_STENT);
            //drTestResultItemContent.StationName_stent] = STATION_STENT;
            dr[STATION_STENT + TestResultItemContent.StationInDate_stent] = testResultStent.StationInDate;
            dr[STATION_STENT + TestResultItemContent.StationOutDate_stent] = testResultStent.StationOutDate;
            dr[STATION_STENT + TestResultItemContent.UserTeamLeader_stent] = testResultStent.UserTeamLeader;
            dr[STATION_STENT + TestResultItemContent.TestResultValue_stent] = testResultStent.TestResultValue;
            dr[STATION_STENT + TestResultItemContent.Stent_Screw1] = SelectTestItemValue(pcbsn, productsn, STATION_STENT, TestResultItemContent.Stent_Screw1);
            dr[STATION_STENT + TestResultItemContent.Stent_Screw2] = SelectTestItemValue(pcbsn, productsn, STATION_STENT, TestResultItemContent.Stent_Screw1);
            dr[STATION_STENT + TestResultItemContent.Stent_Stent] = SelectTestItemValue(pcbsn, productsn, STATION_STENT, TestResultItemContent.Stent_Stent);
            dr[STATION_STENT + TestResultItemContent.Stent_LeftStent] = SelectTestItemValue(pcbsn, productsn, STATION_STENT, TestResultItemContent.Stent_LeftStent);
            dr[STATION_STENT + TestResultItemContent.Stent_RightStent] = SelectTestItemValue(pcbsn, productsn, STATION_STENT, TestResultItemContent.Stent_RightStent);
            #endregion

            #region 成品
            var testResultProduct = SelectTestResultOfSN(pcbsn, productsn, STATION_PRODUCT);
            //dr[TestResultItemContent.StationName_product] = STATION_PRODUCT;
            dr[STATION_PRODUCT + TestResultItemContent.StationInDate_product] = testResultProduct.StationInDate;
            dr[STATION_PRODUCT + TestResultItemContent.StationOutDate_product] = testResultProduct.StationOutDate;
            dr[STATION_PRODUCT + TestResultItemContent.UserTeamLeader_product] = testResultProduct.UserTeamLeader;
            dr[STATION_PRODUCT + TestResultItemContent.TestResultValue_product] = testResultProduct.TestResultValue;
            dr[STATION_PRODUCT + TestResultItemContent.Product_Work_Electric_Test] = SelectTestItemValue(pcbsn, productsn, STATION_PRODUCT, TestResultItemContent.Product_Work_Electric_Test);
            dr[STATION_PRODUCT + TestResultItemContent.Product_DormantElect] = SelectTestItemValue(pcbsn, productsn, STATION_PRODUCT, TestResultItemContent.Product_DormantElect);
            dr[STATION_PRODUCT + TestResultItemContent.Product_Inspect_Result] = SelectTestItemValue(pcbsn, productsn, STATION_PRODUCT, TestResultItemContent.Product_InspectItem);
            #endregion

            testResultDataSource.Rows.Add(dr);
            #endregion
        }

        private static TestResultHistory SelectUseAllPcbaSN(string querySN, int pageIndex,int pageSize)
        {
            //List<string> pcbaList = new List<string>();
            DataSet ds = new DataSet();
            TestResultHistory testResultHistory = new TestResultHistory();
            var selectSQL = $"select {DbTable.F_Test_Result.SN},{DbTable.F_Test_Result.PROCESS_NAME} " +
                $"from {DbTable.F_TEST_RESULT_NAME} " +
                $"order by {DbTable.F_Test_Result.STATION_IN_DATE} desc";
            int count = 1;
            var pcbaLen = ReadPCBACodeLength();
            if (pcbaLen == 0)
                pcbaLen = 16;
            var shellLen = ReadShellCodeLength();

            if (querySN != "" && querySN != null)
            {
                TestResultHistory testResult = new TestResultHistory();
                testResult.PcbaSN = querySN.Trim();
                testResult.ProductTypeNo = GetProductTypeNo(querySN);
                testResult.ShellCodeLen = shellLen;
                testResult.PcbaCodeLen = pcbaLen;
                SelectTestResultDetail1(testResult, count);

                testResultHistory.TestResultNumber = 1;
                ds.Tables.Add(testResultDataSource);
                testResultHistory.TestResultDataSet = ds;
                return testResultHistory;
            }
            else
            {
                var dbReader = SQLServer.ExecuteDataReader(selectSQL);
                if (dbReader.HasRows)
                {
                    pcbaCacheList.Clear();
                    int i = 0;
                    int startIndex = (pageIndex - 1) * pageSize;
                    while(dbReader.Read())
                    {
                        var pcbaSN = dbReader[0].ToString();
                        if (pcbaSN.Length == pcbaLen)
                        {
                            //是PCBA
                            var productTypeNo = dbReader[1].ToString();
                            TestResultHistory queryTestObj = new TestResultHistory();

                            queryTestObj.PcbaSN = pcbaSN;
                            queryTestObj.ProductTypeNo = productTypeNo;
                            queryTestObj.ShellCodeLen = shellLen;
                            queryTestObj.PcbaCodeLen = pcbaLen;
                            var pcbaObj = pcbaCacheList.Find(m => m.PcbaSN == pcbaSN);
                            if (pcbaObj == null)
                            {
                                pcbaCacheList.Add(queryTestObj);
                                i++;
                            }
                        }
                    }
                    LogHelper.Log.Info("开始查询明细...");
                    //开始查询明细
                    int j = 0;
                    foreach (var pcbaItem in pcbaCacheList)
                    {
                        if (j >= startIndex && j < pageIndex * pageSize)
                        {
                            TestResultHistory testResult = new TestResultHistory();
                            testResult.PcbaSN = pcbaItem.PcbaSN;
                            testResult.ProductTypeNo = pcbaItem.ProductTypeNo;
                            testResult.ShellCodeLen = shellLen;
                            testResult.PcbaCodeLen = pcbaLen;
                            SelectTestResultDetail1(testResult, count);
                            count++;
                        }
                        j++;
                    }
                    LogHelper.Log.Info("查询明细结束...");
                    ds.Tables.Add(testResultDataSource);
                    testResultHistory.TestResultNumber = j;
                    testResultHistory.TestResultDataSet = ds;
                }
            }
            LogHelper.Log.Info("查询结束...");
            return testResultHistory;
        }

        private static TestReulstDetail SelectTestResultOfSN(string pcbasn, string productsn, string stationName)
        {
            TestReulstDetail testResult = new TestReulstDetail();
            var selectSQL = $"SELECT TOP 1 " +
                $"{DbTable.F_Test_Result.STATION_IN_DATE}," +
                $"{DbTable.F_Test_Result.STATION_OUT_DATE}," +
                $"{DbTable.F_Test_Result.TEAM_LEADER}," +
                $"{DbTable.F_Test_Result.TEST_RESULT}," +
                $"{DbTable.F_Test_Result.TYPE_NO} " +
                $"FROM " +
                $"{DbTable.F_TEST_RESULT_NAME} " +
                $"WHERE " +
                $"{DbTable.F_Test_Result.SN} = '{pcbasn}' " +
                $"AND " +
                $"{DbTable.F_Test_Result.STATION_NAME} = '{stationName}' " +
                $"ORDER BY {DbTable.F_Test_Result.UPDATE_DATE} DESC";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {//PCBA查询
                testResult.StationInDate = dt.Rows[0][0].ToString();
                testResult.StationOutDate = dt.Rows[0][1].ToString();
                testResult.UserTeamLeader = dt.Rows[0][2].ToString();
                testResult.TestResultValue = dt.Rows[0][3].ToString();
                testResult.ProductTypeNo = dt.Rows[0][4].ToString();
            }
            else
            {//外壳查询
                selectSQL = $"SELECT TOP 1 " +
                $"{DbTable.F_Test_Result.STATION_IN_DATE}," +
                $"{DbTable.F_Test_Result.STATION_OUT_DATE}," +
                $"{DbTable.F_Test_Result.TEAM_LEADER}," +
                $"{DbTable.F_Test_Result.TEST_RESULT}," +
                $"{DbTable.F_Test_Result.TYPE_NO} " +
                $"FROM " +
                $"{DbTable.F_TEST_RESULT_NAME} " +
                $"WHERE " +
                $"{DbTable.F_Test_Result.SN} = '{productsn}' " +
                $"AND " +
                $"{DbTable.F_Test_Result.STATION_NAME} = '{stationName}' " +
                $"ORDER BY {DbTable.F_Test_Result.UPDATE_DATE} DESC";
                dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    testResult.StationInDate = dt.Rows[0][0].ToString();
                    testResult.StationOutDate = dt.Rows[0][1].ToString();
                    testResult.UserTeamLeader = dt.Rows[0][2].ToString();
                    testResult.TestResultValue = dt.Rows[0][3].ToString();
                    testResult.ProductTypeNo = dt.Rows[0][4].ToString();
                }
            }
            return testResult;
        }

        //查询测试项值与结果
        private static string SelectTestItemValue(string pcbasn, string productSN, string stationName, string testItem)
        {
            #region pcbasn
            var selectSQL = $"SELECT TOP 1 " +
                $"{DbTable.F_TEST_LOG_DATA.TEST_RESULT}," +
                $"{DbTable.F_TEST_LOG_DATA.CURRENT_VALUE} " +
                $"FROM " +
                $"{DbTable.F_TEST_LOG_DATA_NAME} " +
                $"WHERE " +
                $"{DbTable.F_TEST_LOG_DATA.PRODUCT_SN} = '{pcbasn}' " +
                $"AND " +
                $"{DbTable.F_TEST_LOG_DATA.STATION_NAME} = '{stationName}'" +
                $"AND " +
                $"{DbTable.F_TEST_LOG_DATA.TEST_ITEM} like '%{testItem}%' " +
                $"ORDER BY " +
                $"{DbTable.F_TEST_LOG_DATA.UPDATE_DATE} DESC";
            #endregion

            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                var testResult = dt.Rows[0][0].ToString();
                var testCurrentValue = dt.Rows[0][1].ToString();
                var showTestResult = "";
                var currentValue = testCurrentValue.Trim().ToLower();
                if (currentValue == "true" || currentValue == "passed")
                {
                    showTestResult = "Passed";
                }
                else if (currentValue == "failed" || currentValue == "false")
                {
                    showTestResult = "Failed";
                }
                else
                {
                    showTestResult = testResult + "," + testCurrentValue;
                }
                return showTestResult;
            }
            else
            {
                #region productsn
                selectSQL = $"SELECT TOP 1 " +
                $"{DbTable.F_TEST_LOG_DATA.TEST_RESULT}," +
                $"{DbTable.F_TEST_LOG_DATA.CURRENT_VALUE} " +
                $"FROM " +
                $"{DbTable.F_TEST_LOG_DATA_NAME} " +
                $"WHERE " +
                $"{DbTable.F_TEST_LOG_DATA.PRODUCT_SN} = '{productSN}' " +
                $"AND " +
                $"{DbTable.F_TEST_LOG_DATA.STATION_NAME} = '{stationName}'" +
                $"AND " +
                $"{DbTable.F_TEST_LOG_DATA.TEST_ITEM} like '%{testItem}%' " +
                $"ORDER BY " +
                $"{DbTable.F_TEST_LOG_DATA.UPDATE_DATE} DESC";
                #endregion
                dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    var testResult = dt.Rows[0][0].ToString();
                    var testCurrentValue = dt.Rows[0][1].ToString();
                    var showTestResult = "";
                    var currentValue = testCurrentValue.Trim().ToLower();
                    if (currentValue == "true" || currentValue == "passed")
                    {
                        showTestResult = "Passed";
                    }
                    else if (currentValue == "failed" || currentValue == "false")
                    {
                        showTestResult = "Failed";
                    }
                    else
                    {
                        showTestResult = testResult + "," + testCurrentValue;
                    }
                    return showTestResult;
                }
            }
            return "";
        }

        public static string GetProductTestFinalResult(string pcbsn, string productsn, int shellLen, string productTypeNo)
        {
            bool IsFinalResultPass = true;
            //var productTypeNo = GetProductTypeNoOfSN(pcbsn, productsn);
            if (productTypeNo == "")
                return "未完成";
            DataTable stationList = SelectStationList(productTypeNo).Tables[0];
            //判断当前工艺流程是否没有外壳装配工站---即不能完成绑定关系
            /*
             * 1）将有PCBA的几个工站进行计算
             * 2）将有外壳SN的几个工站进行计算
             */
            var stationRow = stationList.Select($"{DbTable.F_Test_Result.STATION_NAME} = '外壳装配工站'");
            if (stationRow.Length < 1)
            {
                //不包含外壳装配工站，重新生成计算工站
                var cirticalID = CalCirticalStationID();
                if (pcbsn.Length == shellLen && productsn == "")
                {
                    //只计算外壳装配之后的工站--临界点
                    LogHelper.Log.Info("【只计算外壳装配之后的工站】" + pcbsn);
                    stationList = SelectCirticalStationList(productTypeNo, cirticalID, false).Tables[0];
                }
                else if (pcbsn.Length < shellLen && productsn == "")
                {
                    //只计算外壳装配之前的工站
                    LogHelper.Log.Info("【只计算外壳装配之前的工站】" + pcbsn);
                    stationList = SelectCirticalStationList(productTypeNo, cirticalID, true).Tables[0];
                }
            }
            if (stationList.Rows.Count > 0)
            {
                for (int j = 0; j < stationList.Rows.Count; j++)
                {
                    var stationName = stationList.Rows[j][1].ToString();
                    var selectResultSQL = $"SELECT TOP 1 " +
                        $"{DbTable.F_Test_Result.SN}," +
                        $"{DbTable.F_Test_Result.TYPE_NO}," +
                        $"{DbTable.F_Test_Result.STATION_NAME}," +
                        $"{DbTable.F_Test_Result.STATION_IN_DATE}," +
                        $"{DbTable.F_Test_Result.STATION_OUT_DATE}," +
                        $"{DbTable.F_Test_Result.TEST_RESULT}," +
                        $"{DbTable.F_Test_Result.TEAM_LEADER}," +
                        $"{DbTable.F_Test_Result.ADMIN} " +
                        $"FROM " +
                        $"{DbTable.F_TEST_RESULT_NAME} " +
                        $"WHERE " +
                        $"{DbTable.F_Test_Result.STATION_NAME} = '{stationName}' " +
                        $"AND " +
                        $"{DbTable.F_Test_Result.SN} = '{pcbsn}' " +
                        $"ORDER BY " +
                        $"{DbTable.F_Test_Result.STATION_IN_DATE} DESC";
                    var dtResult = SQLServer.ExecuteDataSet(selectResultSQL).Tables[0];
                    TestResultBasic testResultBasic = new TestResultBasic();
                    if (dtResult.Rows.Count > 0)
                    {
                        //pcbasn查询结果
                        testResultBasic.TestResultValue = dtResult.Rows[0][5].ToString();
                        var currentTestResult = testResultBasic.TestResultValue.Trim().ToLower();
                        if (currentTestResult != "pass")
                        {
                            IsFinalResultPass = false;
                        }
                    }
                    else
                    {
                        //productsn查询结果
                        selectResultSQL = $"SELECT TOP 1 " +
                        $"{DbTable.F_Test_Result.SN}," +
                        $"{DbTable.F_Test_Result.TYPE_NO}," +
                        $"{DbTable.F_Test_Result.STATION_NAME}," +
                        $"{DbTable.F_Test_Result.STATION_IN_DATE}," +
                        $"{DbTable.F_Test_Result.STATION_OUT_DATE}," +
                        $"{DbTable.F_Test_Result.TEST_RESULT}," +
                        $"{DbTable.F_Test_Result.TEAM_LEADER}," +
                        $"{DbTable.F_Test_Result.ADMIN} " +
                        $"FROM " +
                        $"{DbTable.F_TEST_RESULT_NAME} " +
                        $"WHERE " +
                        $"{DbTable.F_Test_Result.STATION_NAME} = '{stationName}' " +
                        $"AND " +
                        $"{DbTable.F_Test_Result.SN} = '{productsn}' " +
                        $"ORDER BY " +
                        $"{DbTable.F_Test_Result.STATION_IN_DATE} DESC";
                        dtResult = SQLServer.ExecuteDataSet(selectResultSQL).Tables[0];
                        if (dtResult.Rows.Count > 0)
                        {
                            testResultBasic.TestResultValue = dtResult.Rows[0][5].ToString();
                            var currentTestResult = testResultBasic.TestResultValue.Trim().ToLower();
                            if (currentTestResult != "pass")
                            {
                                IsFinalResultPass = false;
                            }
                        }
                        else
                        {
                            //pcbasn/productsn都无查询结果；无进站记录
                            //流程为未完成状态
                            return "未完成";
                        }
                    }
                }
            }

            if (IsFinalResultPass)
            {
                return "PASS";
            }
            else
            {
                return "FAIL";
            }
        }

        public static DataSet SelectCirticalStationList(string processName, int cirticalID, bool IsBefore)
        {
            var ds = SelectStationList(processName);
            if (ds.Tables.Count > 0)
            {
                var dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    for (int i = dt.Rows.Count - 1; i >= 0; i--)
                    {
                        if (IsBefore)
                        {
                            if (i >= cirticalID - 1)
                            {
                                dt.Rows.RemoveAt(i);
                            }
                        }
                        else
                        {
                            if (i < cirticalID - 1)
                            {
                                dt.Rows.RemoveAt(i);
                            }
                        }
                    }
                }
            }
            return ds;
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

        private static string GetProductTypeNo(string pid)
        {
            var selectSQL = $"select top 1 {DbTable.F_Test_Result.TYPE_NO} from {DbTable.F_TEST_RESULT_NAME} " +
                $"where {DbTable.F_Test_Result.SN} like '%{pid}%'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();
            return "";
        }

        private static bool IsExistStation(string stationName)
        {
            var currentProcess = SelectCurrentTProcess();
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

        private static List<TestResultBasic> SelectTestResultBasic()
        {
            DataTable typeNoDt = SelectTypeNoList().Tables[0];
            List<TestResultBasic> testResultsList = new List<TestResultBasic>();
            if (typeNoDt.Rows.Count > 0)
            {
                for (int i = 0; i < typeNoDt.Rows.Count; i++)
                {
                    var typeNo = typeNoDt.Rows[i][0].ToString();

                    DataTable stationList = SelectStationList(typeNo).Tables[0];
                    if (stationList.Rows.Count > 0)
                    {
                        for (int j = 0; j < stationList.Rows.Count; j++)
                        {
                            var stationName = stationList.Rows[j][1].ToString();
                            var selectResultSQL = $"SELECT TOP 1 " +
                                $"{DbTable.F_Test_Result.SN}," +
                                $"{DbTable.F_Test_Result.TYPE_NO}," +
                                $"{DbTable.F_Test_Result.STATION_NAME}," +
                                $"{DbTable.F_Test_Result.STATION_IN_DATE}," +
                                $"{DbTable.F_Test_Result.STATION_OUT_DATE}," +
                                $"{DbTable.F_Test_Result.TEST_RESULT}," +
                                $"{DbTable.F_Test_Result.TEAM_LEADER}," +
                                $"{DbTable.F_Test_Result.ADMIN} " +
                                $"FROM " +
                                $"{DbTable.F_TEST_RESULT_NAME} " +
                                $"WHERE " +
                                $"{DbTable.F_Test_Result.STATION_NAME} = '{stationName}' " +
                                $"AND " +
                                $"{DbTable.F_Test_Result.TYPE_NO} = '{typeNo}' " +
                                $"ORDER BY " +
                                $"{DbTable.F_Test_Result.STATION_IN_DATE} DESC";
                            var dtResult = SQLServer.ExecuteDataSet(selectResultSQL).Tables[0];
                            TestResultBasic testResultBasic = new TestResultBasic();
                            if (dtResult.Rows.Count > 0)
                            {
                                testResultBasic.ProductSN = dtResult.Rows[0][0].ToString();
                                testResultBasic.ProductTypeNo = dtResult.Rows[0][1].ToString();
                                testResultBasic.StationName = dtResult.Rows[0][2].ToString();
                                testResultBasic.StationInDate = dtResult.Rows[0][3].ToString();
                                testResultBasic.StationOutDate = dtResult.Rows[0][4].ToString();
                                testResultBasic.TestResultValue = dtResult.Rows[0][5].ToString();
                                testResultBasic.UserTeamLeader = dtResult.Rows[0][6].ToString();
                                testResultBasic.UserAdmin = dtResult.Rows[0][7].ToString();
                                testResultsList.Add(testResultBasic);
                            }
                        }
                    }
                }
            }
            return testResultsList;
        }

        public static DataSet SelectTypeNoList()
        {
            var selectSQL = $"SELECT {DbTable.F_PRODUCT_PACKAGE_STORAGE.PRODUCT_TYPE_NO} " +
                $"FROM " +
                $"{DbTable.F_PRODUCT_PACKAGE_STORAGE_NAME} ";
            return SQLServer.ExecuteDataSet(selectSQL);
        }

        /// <summary>
        /// 查询当前某工艺的站位记录
        /// </summary>
        /// <returns></returns>
        public static DataSet SelectStationList(string processName)
        {
            string selectSQL = $"SELECT {DbTable.F_TECHNOLOGICAL_PROCESS.STATION_ORDER}," +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.STATION_NAME}," +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.USER_NAME}," +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.UPDATE_DATE} " +
                $"FROM {DbTable.F_TECHNOLOGICAL_PROCESS_NAME} " +
                $"WHERE {DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME} = '{processName}' " +
                $"ORDER BY {DbTable.F_TECHNOLOGICAL_PROCESS.STATION_ORDER}";
            return SQLServer.ExecuteDataSet(selectSQL);
        }

        private static DataTable InitTestResultDataTable(bool IsShowFinalResult)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(TestResultItemContent.Order);
            dt.Columns.Add(TestResultItemContent.PcbaSN);
            dt.Columns.Add(TestResultItemContent.ProductSN);
            dt.Columns.Add(TestResultItemContent.ProductTypeNo);
            if (IsShowFinalResult)
                dt.Columns.Add(TestResultItemContent.FinalResultValue);
            //A01第一个工站
            //烧录工位/灵敏度工位/外壳工位/气密工位/支架装配工位/成品测试工位
            //烧录工位
            //dt.Columns.Add(STATION_TURN);
            dt.Columns.Add(STATION_TURN + TestResultItemContent.StationInDate_turn);
            dt.Columns.Add(STATION_TURN + TestResultItemContent.StationOutDate_turn);
            dt.Columns.Add(STATION_TURN + TestResultItemContent.TestResultValue_turn);
            dt.Columns.Add(STATION_TURN + TestResultItemContent.UserTeamLeader_turn);
            dt.Columns.Add(STATION_TURN + TestResultItemContent.Turn_TurnItem);
            dt.Columns.Add(STATION_TURN + TestResultItemContent.Turn_Voltage_12V_Item);
            dt.Columns.Add(STATION_TURN + TestResultItemContent.Turn_Voltage_5V_Item);
            dt.Columns.Add(STATION_TURN + TestResultItemContent.Turn_Voltage_33_1V_Item);
            dt.Columns.Add(STATION_TURN + TestResultItemContent.Turn_Voltage_33_2V_Item);
            dt.Columns.Add(STATION_TURN + TestResultItemContent.Turn_SoftVersion);
            //灵敏度工位
            //dt.Columns.Add(STATION_SENSIBLITY);
            dt.Columns.Add(STATION_SENSIBLITY + TestResultItemContent.StationInDate_sen);
            dt.Columns.Add(STATION_SENSIBLITY + TestResultItemContent.StationOutDate_sen);
            dt.Columns.Add(STATION_SENSIBLITY + TestResultItemContent.TestResultValue_sen);
            dt.Columns.Add(STATION_SENSIBLITY + TestResultItemContent.UserTeamLeader_sen);
            dt.Columns.Add(STATION_SENSIBLITY + TestResultItemContent.Sen_Work_Electric_Test);
            dt.Columns.Add(STATION_SENSIBLITY + TestResultItemContent.Sen_PartNumber);
            dt.Columns.Add(STATION_SENSIBLITY + TestResultItemContent.Sen_HardWareVersion);
            dt.Columns.Add(STATION_SENSIBLITY + TestResultItemContent.Sen_SoftVersion);
            dt.Columns.Add(STATION_SENSIBLITY + TestResultItemContent.Sen_ECUID);
            dt.Columns.Add(STATION_SENSIBLITY + TestResultItemContent.Sen_BootloaderVersion);
            dt.Columns.Add(STATION_SENSIBLITY + TestResultItemContent.Sen_RadioFreq);
            dt.Columns.Add(STATION_SENSIBLITY + TestResultItemContent.Sen_DormantElect);
            //外壳工位
            //dt.Columns.Add(STATION_SHELL);
            dt.Columns.Add(STATION_SHELL + TestResultItemContent.StationInDate_shell);
            dt.Columns.Add(STATION_SHELL + TestResultItemContent.StationOutDate_shell);
            dt.Columns.Add(STATION_SHELL + TestResultItemContent.TestResultValue_shell);
            dt.Columns.Add(STATION_SHELL + TestResultItemContent.UserTeamLeader_shell);
            dt.Columns.Add(STATION_SHELL + TestResultItemContent.Shell_FrontCover);
            dt.Columns.Add(STATION_SHELL + TestResultItemContent.Shell_BackCover);
            dt.Columns.Add(STATION_SHELL + TestResultItemContent.Shell_PCBScrew1);
            dt.Columns.Add(STATION_SHELL + TestResultItemContent.Shell_PCBScrew2);
            dt.Columns.Add(STATION_SHELL + TestResultItemContent.Shell_PCBScrew3);
            dt.Columns.Add(STATION_SHELL + TestResultItemContent.Shell_PCBScrew4);
            dt.Columns.Add(STATION_SHELL + TestResultItemContent.Shell_ShellScrew1);
            dt.Columns.Add(STATION_SHELL + TestResultItemContent.Shell_ShellScrew2);
            dt.Columns.Add(STATION_SHELL + TestResultItemContent.Shell_ShellScrew3);
            dt.Columns.Add(STATION_SHELL + TestResultItemContent.Shell_ShellScrew4);
            //气密测试
            //dt.Columns.Add(STATION_AIR);
            dt.Columns.Add(STATION_AIR + TestResultItemContent.StationInDate_air);
            dt.Columns.Add(STATION_AIR + TestResultItemContent.StationOutDate_air);
            dt.Columns.Add(STATION_AIR + TestResultItemContent.TestResultValue_air);
            dt.Columns.Add(STATION_AIR + TestResultItemContent.UserTeamLeader_air);
            dt.Columns.Add(STATION_AIR + TestResultItemContent.Air_AirtightTest);
            //支架装配
            //dt.Columns.Add(STATION_STENT);
            dt.Columns.Add(STATION_STENT + TestResultItemContent.StationInDate_stent);
            dt.Columns.Add(STATION_STENT + TestResultItemContent.StationOutDate_stent);
            dt.Columns.Add(STATION_STENT + TestResultItemContent.TestResultValue_stent);
            dt.Columns.Add(STATION_STENT + TestResultItemContent.UserTeamLeader_stent);
            dt.Columns.Add(STATION_STENT + TestResultItemContent.Stent_Screw1);
            dt.Columns.Add(STATION_STENT + TestResultItemContent.Stent_Screw2);
            dt.Columns.Add(STATION_STENT + TestResultItemContent.Stent_Stent);
            dt.Columns.Add(STATION_STENT + TestResultItemContent.Stent_LeftStent);
            dt.Columns.Add(STATION_STENT + TestResultItemContent.Stent_RightStent);
            //成品测试
            //dt.Columns.Add(STATION_PRODUCT);
            dt.Columns.Add(STATION_PRODUCT + TestResultItemContent.StationInDate_product);
            dt.Columns.Add(STATION_PRODUCT + TestResultItemContent.StationOutDate_product);
            dt.Columns.Add(STATION_PRODUCT + TestResultItemContent.TestResultValue_product);
            dt.Columns.Add(STATION_PRODUCT + TestResultItemContent.UserTeamLeader_product);
            dt.Columns.Add(STATION_PRODUCT + TestResultItemContent.Product_Work_Electric_Test);
            dt.Columns.Add(STATION_PRODUCT + TestResultItemContent.Product_DormantElect);
            dt.Columns.Add(STATION_PRODUCT + TestResultItemContent.Product_Inspect_Result);

            return dt;
        }

        private static string SelectCurrentTProcess()
        {
            string selectSQL = $"SELECT DISTINCT {DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME} " +
                $"FROM {DbTable.F_TECHNOLOGICAL_PROCESS_NAME} WHERE {DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_STATE} = '1'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                var currentProcess = dt.Rows[0][0].ToString().Trim();
                return currentProcess;
            }
            else
                return "";
        }

        public static string GetPCBASn(string sn)
        {
            var snLen = 0;
            if (sn == "" || sn == null)
                snLen = 0;
            else
                snLen = sn.Length;
            var selectSQL = $"SELECT * FROM {DbTable.F_BINDING_PCBA_NAME} WHERE " +
                $"{DbTable.F_BINDING_PCBA.SN_PCBA} = '{sn}' AND {DbTable.F_BINDING_PCBA.BINDING_STATE} = '1'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return sn;//传入值为PCBA
            }
            else
            {
                //传入值为外壳值
                selectSQL = $"SELECT {DbTable.F_BINDING_PCBA.SN_PCBA} FROM {DbTable.F_BINDING_PCBA_NAME} WHERE " +
                    $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{sn}' AND {DbTable.F_BINDING_PCBA.BINDING_STATE} = '1'";
                dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0][0].ToString();
                }
            }
            if (ReadShellCodeLength() == snLen)
            {
                return "";
            }
            return sn;
        }

        public static string GetProductSNOfShell(string pcbSN)
        {
            var selectSQL = $"SELECT {DbTable.F_BINDING_PCBA.SN_OUTTER} FROM {DbTable.F_BINDING_PCBA_NAME} " +
                $"WHERE {DbTable.F_BINDING_PCBA.SN_OUTTER} = '{pcbSN}' AND " +
                $"{DbTable.F_BINDING_PCBA.BINDING_STATE} = '1'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            return "";
        }

        public static string GetProductSn(string sn)
        {
            //传入值为PCBA
            var snLen = 0;
            if (sn == "" || sn == null)
                snLen = 0;
            else
                snLen = sn.Length;
            var selectSQL = $"SELECT * FROM {DbTable.F_BINDING_PCBA_NAME} WHERE {DbTable.F_BINDING_PCBA.SN_OUTTER} = '{sn}' AND " +
                $"{DbTable.F_BINDING_PCBA.BINDING_STATE} = '1'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return sn;//返回外壳
            }
            else
            {
                //传入值为外壳值
                selectSQL = $"SELECT {DbTable.F_BINDING_PCBA.SN_OUTTER} FROM {DbTable.F_BINDING_PCBA_NAME} WHERE " +
                    $"{DbTable.F_BINDING_PCBA.SN_PCBA} = '{sn}' AND {DbTable.F_BINDING_PCBA.BINDING_STATE} = '1'";
                dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0][0].ToString();
                }
            }
            //无查询结果-不存在绑定关系/传入值有误
            //当前工艺无外壳装配工站时，并且是外壳码时，返回外壳码
            if (ReadShellCodeLength() == snLen)
            {
                return sn;
            }
            return "";
        }

        private static int ReadShellCodeLength()
        {
            try
            {
                int shellLen = 0;
                var defaultRoot = ConfigurationManager.AppSettings["shellCodeRoot"].ToString();
                var process = SelectCurrentTProcess();
                string configPath = defaultRoot + ":\\StationConfig\\外壳装配工站\\" + process + "\\" + "外壳装配工站_" + process + "_config.ini";
                int.TryParse(INIFile.GetValue(process, "设置外壳条码长度位数", configPath).ToString().Trim(), out shellLen);
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
                var process = SelectCurrentTProcess();
                string configPath = defaultRoot + ":\\StationConfig\\外壳装配工站\\" + process + "\\" + "外壳装配工站_" + process + "_config.ini";
                int.TryParse(INIFile.GetValue(process, "设置PCB条码长度位数", configPath).ToString().Trim(), out shellLen);
                LogHelper.Log.Info("【配置文件路径】" + configPath + "len=" + shellLen);
                return shellLen;
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error("读取配置长度错误！" + ex.Message + ex.StackTrace + "\r\n");
                return 16;
            }
        }

        public class TestResultHistory
        {
            public int TestResultNumber { get; set; }

            public System.Data.DataSet TestResultDataSet { get; set; }

            public string PcbaSN { get; set; }

            public string ProductTypeNo { get; set; }

            public int ShellCodeLen { get; set; }

            public int PcbaCodeLen { get; set; }
        }
    }
}
