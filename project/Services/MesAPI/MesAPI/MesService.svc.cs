using SwaggerWcf.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Data;
using CommonUtils.DB;
using CommonUtils.Logger;
using System.Configuration;
using System.Collections;
using MesAPI.MessageQueue.RemoteClient;
using MesAPI.DB;
using MesAPI.Model;
using System.Data.SqlClient;
using MesAPI.Common;

namespace MesAPI
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [SwaggerWcf("/MesService")]
    public class MesService : IMesService
    {
        private Queue<string[]> fcQueue = new Queue<string[]>();
        private Queue<string[]> insertDataQueue = new Queue<string[]>();
        private Queue<string[]> selectDataQueue = new Queue<string[]>();
        private Queue<string[]> insertMaterialStatistics = new Queue<string[]>();
        //烧录工位/灵敏度工位/外壳工位/气密工位/支架装配工位/成品测试工位
        private static string STATION_TURN = "烧录";
        private static string STATION_SENSIBLITY = "灵敏度";
        private static string STATION_SHELL = "外壳";
        private static string STATION_AIR = "气密";
        private static string STATION_STENT = "支架";
        private static string STATION_PRODUCT = "成品";

        private string GetDateTimeNow()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        #region 测试通讯
        [SwaggerWcfTag("MesServcie 服务")]
        public string TestCommunication(string value)
        {
            //通讯正常返回原值
            //客户端与接口异常：收不到返回值
            //接口与数据库异常：
            var testRes = SQLServer.TestSQLConnectState();
            if (testRes != "")
            {
                return "【SQLServer】" + testRes;
            }
            return value;
        }
        #endregion

        #region 用户信息接口

        #region 用户登录
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="username">用户名/手机号/邮箱</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public LoginResult Login(string username, string password)
        {
            //暂未处理用户角色
            try
            {
                DataTable dt = GetUserInfo(username).Tables[0];
                if (dt.Rows.Count < 1)
                {
                    //用户不存在
                    LogHelper.Log.Info($"用户名{username}不存在，验证失败！");
                    return LoginResult.USER_NAME_ERR;
                }
                else
                {
                    //用户存在
                    //验证登录密码
                    var selectSQL = $"SELECT * FROM {DbTable.F_USER_NAME} WHERE " +
                        $"{DbTable.F_User.USER_NAME} = '{username}' AND " +
                        $"{DbTable.F_User.PASS_WORD} = '{password}'";
                    DataTable dtRes = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                    if (dtRes.Rows.Count < 1)
                    {
                        //密码验证失败
                        LogHelper.Log.Info($"用户{username}密码验证失败！");
                        return LoginResult.USER_PWD_ERR;
                    }
                    else
                    {
                        //通过验证
                        LogHelper.Log.Info(username + " 登录进入 " + DateTime.Now);
                        return LoginResult.SUCCESS;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error("用户登录异常..." + ex.Message);
                return LoginResult.FAIL_EXCEP;
            }
        }

        #endregion

        #region 查询用户信息
        /// <summary>
        /// 查询用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public DataSet GetUserInfo(string username)
        {
            var selectSQL = $"SELECT {DbTable.F_User.ROLE_NAME},{DbTable.F_User.PASS_WORD}" +
                $" FROM {DbTable.F_USER_NAME} " +
                     $"WHERE {DbTable.F_User.USER_NAME} = '{username}'";
            return SQLServer.ExecuteDataSet(selectSQL);
        }
        #endregion

        #region 查询所有用户
        /// <summary>
        /// 查询所有用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public DataSet GetAllUserInfo()
        {
            string sqlString = $"SELECT {DbTable.F_User.USER_NAME}," +
                    $"{DbTable.F_User.ROLE_NAME}," +
                    $"{DbTable.F_User.STATUS}," +
                    $"{DbTable.F_User.UPDATE_DATE} " +
                    $"FROM {DbTable.F_USER_NAME} ";
            return SQLServer.ExecuteDataSet(sqlString);
        }
        #endregion

        #region 注册
        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="phone"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public RegisterResult Register(string username, string pwd, string phone, string email, int userType)
        {
            try
            {
                DataTable dt = GetUserInfo(username).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    //用户已存在
                    return RegisterResult.REGISTER_EXIST_USER;
                }
                else
                {
                    //用户不存在，可以注册
                    string dateTimeNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string insertString = $"INSERT INTO {DbTable.F_USER_NAME}" +
                        $"({DbTable.F_User.USER_NAME}," +
                        $"{DbTable.F_User.PASS_WORD} ," +
                        $"{DbTable.F_User.PHONE}," +
                        $"{DbTable.F_User.EMAIL} ," +
                        $"{DbTable.F_User.UPDATE_DATE} ," +
                        $"{DbTable.F_User.ROLE_NAME}) " +
                        $"VALUES('{username}', '{pwd}', '{phone}', '{email}', '{dateTimeNow}','{userType}')";
                    int executeResult = SQLServer.ExecuteNonQuery(insertString);
                    if (executeResult < 1)
                    {
                        return RegisterResult.REGISTER_FAIL_SQL;
                    }
                    return RegisterResult.REGISTER_SUCCESS;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error("注册失败..." + ex.Message);
                return RegisterResult.REGISTER_ERR;
            }
        }
        #endregion

        #region 修改密码
        public int ModifyUserPassword(string username, string pwd)
        {
            var updateSQL = $"UPDATE {DbTable.F_USER_NAME} SET " +
                $"{DbTable.F_User.PASS_WORD} = '{pwd}'," +
                $"{DbTable.F_User.UPDATE_DATE} = '{GetDateTimeNow()}' " +
                $"WHERE {DbTable.F_User.USER_NAME} = '{username}'";
            return SQLServer.ExecuteNonQuery(updateSQL);
        }
        #endregion

        #region 删除用户
        public int DeleteUser(string username)
        {
            var deleteSQL = $"DELETE FROM {DbTable.F_USER_NAME} WHERE {DbTable.F_User.USER_NAME} = '{username}'";
            return SQLServer.ExecuteNonQuery(deleteSQL);
        }
        #endregion

        #endregion

        #region 工艺流程

        /// <summary>
        /// 配置产线包含哪些站位，按顺序插入
        /// </summary>
        /// <param name="dctData"></param>
        /// <returns>成功返回1，失败返回0+空格+序号+键+空格+值</returns>
        public int InsertStation(List<Station> stationList)
        {
            foreach (var station in stationList)
            {
                if (!IsExistStation(station))
                {
                    //不存在，插入
                    string insertSQL = $"INSERT INTO {DbTable.F_TECHNOLOGICAL_PROCESS_NAME}(" +
                        $"{DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME}," +
                        $"{DbTable.F_TECHNOLOGICAL_PROCESS.STATION_ORDER}," +
                        $"{DbTable.F_TECHNOLOGICAL_PROCESS.STATION_NAME}," +
                        $"{DbTable.F_TECHNOLOGICAL_PROCESS.USER_NAME}," +
                        $"{DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_STATE}) " +
                    $"VALUES('{station.ProcessName}','{station.StationID}','{station.StationName}','{station.UserName}','{station.ProcessState}')";
                    LogHelper.Log.Info(insertSQL);
                    if (SQLServer.ExecuteNonQuery(insertSQL) < 1)
                        return 0;
                }
            }
            return 1;
        }

        /// <summary>
        /// 查询当前某工艺的站位记录
        /// </summary>
        /// <returns></returns>
        public DataSet SelectStationList(string processName)
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

        public DataSet SelectProcessList()
        {
            var selectSQL = $"SELECT DISTINCT {DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME} " +
                $"FROM {DbTable.F_TECHNOLOGICAL_PROCESS_NAME}";
            return SQLServer.ExecuteDataSet(selectSQL);
        }

        /// <summary>
        /// 删除某条记录
        /// </summary>
        /// <param name="stationName"></param>
        /// <returns></returns>
        public int DeleteStation(string processName, string stationName)
        {
            string deleteSQL = $"DELETE FROM {DbTable.F_TECHNOLOGICAL_PROCESS_NAME} " +
                $"WHERE {DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME} = '{processName}' AND " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.STATION_NAME} = '{stationName}'";
            return SQLServer.ExecuteNonQuery(deleteSQL);
        }

        /// <summary>
        /// 删除所有站位记录
        /// </summary>
        /// <returns></returns>
        public int DeleteAllStation(string processName)
        {
            if (string.IsNullOrEmpty(processName))
                return 0;
            string deleteSQL = $"DELETE FROM {DbTable.F_TECHNOLOGICAL_PROCESS_NAME} WHERE " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME} = '{processName}'";
            return SQLServer.ExecuteNonQuery(deleteSQL);
        }

        /// <summary>
        /// 产线序号是否为空
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool IsExistStation(Station station)
        {
            string selectSQL = $"SELECT * FROM {DbTable.F_TECHNOLOGICAL_PROCESS_NAME} WHERE " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME} = '{station.ProcessName}' AND " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.STATION_NAME} = '{station.StationName}' AND " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.STATION_ORDER} = '{station.StationID}'";
            DataTable dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        public int SetCurrentProcess(string processName, int state)
        {
            var updateSQL = $"UPDATE {DbTable.F_TECHNOLOGICAL_PROCESS_NAME} SET " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_STATE} = '{state}' WHERE " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME} = '{processName}'";
            LogHelper.Log.Info(updateSQL);
            return SQLServer.ExecuteNonQuery(updateSQL);
        }

        #endregion

        #region 测试结果数据接口
        /// <summary>
        /// 上位机查询测试结果
        /// </summary>
        /// <param name="sn">追溯号，可为空</param>
        /// <param name="typeNo">型号，可为空</param>
        /// <param name="station">站位名，可为空</param>
        /// <param name="IsSnFuzzy">true-模糊查询，false-非模糊查询</param>
        /// <returns></returns>
        public DataSet SelectTestResultUpper(string sn, string typeNo, string station, bool IsSnFuzzy)
        {
            //查询时返回
            //通过SN查询，传入sn_outter,查询sn_pcba
            var snTemp = "";
            if (sn != "")
            {
                snTemp = SelectSN(sn);
                if (snTemp == "")
                {
                    //该SN装配工站未绑定
                    //直接查询记录
                    snTemp = sn;
                }
            }
            string selectSQL = "";
            if (string.IsNullOrEmpty(sn) && string.IsNullOrEmpty(typeNo) && string.IsNullOrEmpty(station))
            {
                selectSQL = $"SELECT ROW_NUMBER() OVER(ORDER BY {DbTable.F_Test_Result.UPDATE_DATE} DESC) 序号," +
                    $"{DbTable.F_Test_Result.PROCESS_NAME} 工艺流程," +
                    $"{DbTable.F_Test_Result.SN} AS 产品追溯码," +
                    $"{DbTable.F_Test_Result.TYPE_NO} AS 产品型号," +
                    $"{DbTable.F_Test_Result.STATION_NAME} AS 站位名称," +
                    $"{DbTable.F_Test_Result.TEST_RESULT} AS 测试结果," +
                    $"{DbTable.F_Test_Result.REMARK} AS 备注, " +
                    $"{DbTable.F_Test_Result.TEAM_LEADER} 班组长, " +
                    $"{DbTable.F_Test_Result.ADMIN} 管理员," +
                    $"{DbTable.F_Test_Result.UPDATE_DATE} 更新日期 " +
                    $"FROM {DbTable.F_TEST_RESULT_NAME}";
            }
            else
            {
                if (IsSnFuzzy)
                {
                    selectSQL = $"SELECT ROW_NUMBER() OVER(ORDER BY {DbTable.F_Test_Result.UPDATE_DATE} DESC) 序号," +
                        $"{DbTable.F_Test_Result.PROCESS_NAME} 工艺流程," +
                        $"{DbTable.F_Test_Result.SN} 产品追溯码," +
                        $"{DbTable.F_Test_Result.TYPE_NO} 产品型号," +
                        $"{DbTable.F_Test_Result.STATION_NAME} 站位名称," +
                        $"{DbTable.F_Test_Result.TEST_RESULT} 测试结果," +
                        $"{DbTable.F_Test_Result.REMARK} 备注, " +
                        $"{DbTable.F_Test_Result.TEAM_LEADER} 班组长, " +
                        $"{DbTable.F_Test_Result.ADMIN} 管理员," +
                        $"{DbTable.F_Test_Result.UPDATE_DATE} 更新日期 " +
                        $"FROM {DbTable.F_TEST_RESULT_NAME} " +
                        $"WHERE {DbTable.F_Test_Result.SN} like '%{sn}%' OR " +
                        $"{DbTable.F_Test_Result.SN} like '%{snTemp}%' OR " +
                        $"{DbTable.F_Test_Result.TYPE_NO} like '{typeNo}' OR " +
                        $"{DbTable.F_Test_Result.STATION_NAME} like '{station}'";
                }
                else
                {
                    selectSQL = $"SELECT ROW_NUMBER() OVER(ORDER BY {DbTable.F_Test_Result.UPDATE_DATE} DESC) 序号," +
                        $"{DbTable.F_Test_Result.PROCESS_NAME} 工艺流程," +
                        $"{DbTable.F_Test_Result.SN} 产品追溯码," +
                        $"{DbTable.F_Test_Result.TYPE_NO} 产品型号," +
                        $"{DbTable.F_Test_Result.STATION_NAME} 站位名称," +
                        $"{DbTable.F_Test_Result.TEST_RESULT} 测试结果," +
                        $"{DbTable.F_Test_Result.REMARK} 备注, " +
                        $"{DbTable.F_Test_Result.TEAM_LEADER} 班组长, " +
                        $"{DbTable.F_Test_Result.ADMIN} 管理员," +
                        $"{DbTable.F_Test_Result.UPDATE_DATE} 更新日期 " +
                        $"FROM {DbTable.F_TEST_RESULT_NAME} " +
                        $"WHERE {DbTable.F_Test_Result.SN} = '%{sn}%' OR " +
                        $"{DbTable.F_Test_Result.SN} = '%{snTemp}%' OR " +
                        $"{DbTable.F_Test_Result.TYPE_NO} = '{typeNo}' OR " +
                        $"{DbTable.F_Test_Result.STATION_NAME} = '{station}'";
                }
            }
            LogHelper.Log.Info(selectSQL);
            return SQLServer.ExecuteDataSet(selectSQL);
        }

        public string SelectSN(string sn)
        {
            //两种情况
            //sn= snoutter;
            var selectSQL = $"SELECT {DbTable.F_BINDING_PCBA.SN_PCBA} FROM  {DbTable.F_BINDING_PCBA_NAME} " +
                $"WHERE " +
                $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{sn}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            //sn= snPcba
            selectSQL = $"SELECT {DbTable.F_BINDING_PCBA.SN_OUTTER} FROM {DbTable.F_BINDING_PCBA_NAME} " +
                $"WHERE " +
                $"{DbTable.F_BINDING_PCBA.SN_PCBA} = '{sn}'";
            dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();
            return "";
        }

        /// <summary>
        /// 上位机查询上一站位的所有记录
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="typeNo"></param>
        /// <param name="station"></param>
        /// <returns></returns>
        public DataSet SelectLastTestResultUpper(string sn, string typeNo, string station)
        {
            //根据型号与站位，查询其上一站位
            LogHelper.Log.Info("上位机查询测试结果,传入站位为" + station);
            string selectOrderSQL = $"SELECT {DbTable.F_Product_Station.STATION_ORDER} FROM {DbTable.F_PRODUCT_STATION_NAME} " +
                $"WHERE {DbTable.F_Product_Station.STATION_NAME} = '{station}'";
            LogHelper.Log.Info(selectOrderSQL);
            DataTable dt = SQLServer.ExecuteDataSet(selectOrderSQL).Tables[0];
            int lastOrder = int.Parse(dt.Rows[0][0].ToString()) - 1;
            selectOrderSQL = $"SELECT {DbTable.F_Product_Station.STATION_NAME} FROM {DbTable.F_PRODUCT_STATION_NAME} " +
                $"WHERE {DbTable.F_Product_Station.STATION_ORDER} = '{lastOrder}'";
            dt = SQLServer.ExecuteDataSet(selectOrderSQL).Tables[0];
            station = dt.Rows[0][0].ToString();
            LogHelper.Log.Info("测试端查询测试结果,上一站位为" + station);
            //由上一站位查询记录
            string selectSQL = $"SELECT {DbTable.F_Test_Result.SN},{DbTable.F_Test_Result.TYPE_NO}," +
                 $"{DbTable.F_Test_Result.STATION_NAME},{DbTable.F_Test_Result.TEST_RESULT}," +
                 $"{DbTable.F_Test_Result.UPDATE_DATE},{DbTable.F_Test_Result.REMARK} " +
                 $"FROM {DbTable.F_TEST_RESULT_NAME} " +
                 $"WHERE {DbTable.F_Test_Result.SN} = '{sn}' AND {DbTable.F_Test_Result.TYPE_NO} = '{typeNo}' AND " +
                 $"{DbTable.F_Test_Result.STATION_NAME} = '{station}'" +
                 $"ORDER BY {DbTable.F_Test_Result.UPDATE_DATE}";
            return SQLServer.ExecuteDataSet(selectSQL);
        }

        /// <summary>
        /// 第一步，查询所有过站产品SN
        /// </summary>
        /// <returns></returns>
        private List<string> SelectUseAllPcbaSN()
        {
            var selectSQL = $"SELECT DISTINCT {DbTable.F_Test_Result.SN} " +
                $"FROM " +
                $"{DbTable.F_TEST_RESULT_NAME} ";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            List<string> pcbaList = new List<string>();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var pcbaSN = dt.Rows[i][0].ToString();
                    if (!IsProductSN(pcbaSN))
                    {
                        //是PCBA
                        pcbaList.Add(pcbaSN);
                    }
                }
            }
            return pcbaList;
        }

        /// <summary>
        /// 根据传入sn去查询该SN是否是外壳SN
        /// </summary>
        /// <param name="pcbaSN"></param>
        /// <returns></returns>
        private bool IsProductSN(string pcbaSN)
        {
            var selectSQL = $"SELECT * FROM {DbTable.F_BINDING_PCBA_NAME} " +
                $"WHERE {DbTable.F_BINDING_PCBA.SN_OUTTER} = '{pcbaSN}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        private List<TestResultBasic> SelectTestResultBasic()
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
        public DataSet SelectTestResultDetail(string querySN)
        {
            //更新当前工站名称
            STATION_TURN = UpdateCurrentStation(STATION_TURN);
            STATION_STENT = UpdateCurrentStation(STATION_STENT);
            STATION_SHELL = UpdateCurrentStation(STATION_SHELL);
            STATION_SENSIBLITY = UpdateCurrentStation(STATION_SENSIBLITY);
            STATION_PRODUCT = UpdateCurrentStation(STATION_PRODUCT);
            STATION_AIR = UpdateCurrentStation(STATION_AIR);
            DataTable dt = InitTestResultDataTable(true);
            DataSet dataSet = new DataSet();
            List<string> pcbaList = new List<string>();
            if (querySN != "")
            {
                pcbaList.Add(querySN.Trim());
            }
            else
            {
                pcbaList = SelectUseAllPcbaSN();
            }
            //List<TestReulstDetail> testReulstDetailsList = new List<TestReulstDetail>();
            //List<TestResultBasic> testResultBasicsList = SelectTestResultBasic();
            int count = 1;
            if (pcbaList.Count > 0)
            {
                foreach (var pcbaSN in pcbaList)
                {
                    //查询外壳编码
                    //计算最终结果
                    //查询测试项
                    //TestReulstDetail testReulstDetail = new TestReulstDetail();
                    //烧录工位/灵敏度工位/外壳工位/气密工位/支架装配工位/成品测试工位
                    DataRow dr = dt.NewRow();
                    var pcbsn = GetPCBASn(pcbaSN);
                    var productsn = GetProductSn(pcbaSN);
                    dr[TestResultItemContent.Order] = count;
                    dr[TestResultItemContent.PcbaSN] = pcbsn;
                    dr[TestResultItemContent.ProductSN] = productsn;
                    dr[TestResultItemContent.FinalResultValue] = GetProductTestFinalResult(pcbsn,productsn);

                    #region 烧录工位信息
                    var testResultTurn = SelectTestResultOfSN(pcbsn,productsn,STATION_TURN);
                    //dr[TestResultItemContent.StationName_turn] = STATION_TURN;
                    dr[TestResultItemContent.ProductTypeNo] = testResultTurn.ProductTypeNo;
                    dr[STATION_TURN + TestResultItemContent.StationInDate_turn] = testResultTurn.StationInDate;
                    dr[STATION_TURN + TestResultItemContent.StationOutDate_turn] = testResultTurn.StationOutDate;
                    dr[STATION_TURN + TestResultItemContent.UserTeamLeader_turn] = testResultTurn.UserTeamLeader;
                    dr[STATION_TURN + TestResultItemContent.TestResultValue_turn] = testResultTurn.TestResultValue;
                    dr[STATION_TURN + TestResultItemContent.Turn_TurnItem] = SelectTestItemValue(pcbsn,productsn, STATION_TURN, TestResultItemContent.Turn_TurnItem);
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
                    #endregion

                    dt.Rows.Add(dr);
                    count++;
                }
                dataSet.Tables.Add(dt);
            }
            return dataSet;
        }
        private static TestReulstDetail SelectTestResultOfSN(string pcbasn,string productsn,string stationName)
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
        private static DataTable InitTestResultDataTable(bool IsShowFinalResult)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(TestResultItemContent.Order);
            dt.Columns.Add(TestResultItemContent.PcbaSN);
            dt.Columns.Add(TestResultItemContent.ProductSN);
            dt.Columns.Add(TestResultItemContent.ProductTypeNo);
            if(IsShowFinalResult)
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
            return dt;
        }
        //查询测试项值与结果
        private static string SelectTestItemValue(string pcbasn,string productSN,string stationName,string testItem)
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

        private static string SelectTestItemValue(string pcbasn,string productSN, string stationName, string testItem,string joinDateTime)
        {
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
                $"AND " +
                $"{DbTable.F_TEST_LOG_DATA.JOIN_DATE_TIME} = '{joinDateTime}'" +
                $"ORDER BY " +
                $"{DbTable.F_TEST_LOG_DATA.UPDATE_DATE} DESC";
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
                $"AND " +
                $"{DbTable.F_TEST_LOG_DATA.JOIN_DATE_TIME} = '{joinDateTime}'" +
                $"ORDER BY " +
                $"{DbTable.F_TEST_LOG_DATA.UPDATE_DATE} DESC";
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

        public string GetPCBASn(string sn)
        {
            var selectSQL = $"SELECT * FROM {DbTable.F_BINDING_PCBA_NAME} WHERE {DbTable.F_BINDING_PCBA.SN_PCBA} = '{sn}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return sn;//传入值为PCBA
            }
            else
            {
                //传入值为外壳值
                selectSQL = $"SELECT {DbTable.F_BINDING_PCBA.SN_PCBA} FROM {DbTable.F_BINDING_PCBA_NAME} WHERE " +
                    $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{sn}'";
                dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0][0].ToString();
                }
            }
            //LogHelper.Log.Info("【查PCBA失败！】"+sn);
            return sn;
        }

        public string GetProductSn(string sn)
        {
            var selectSQL = $"SELECT * FROM {DbTable.F_BINDING_PCBA_NAME} WHERE {DbTable.F_BINDING_PCBA.SN_OUTTER} = '{sn}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return sn;//传入值为PCBA
            }
            else
            {
                //传入值为外壳值
                selectSQL = $"SELECT {DbTable.F_BINDING_PCBA.SN_OUTTER} FROM {DbTable.F_BINDING_PCBA_NAME} WHERE " +
                    $"{DbTable.F_BINDING_PCBA.SN_PCBA} = '{sn}'";
                dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0][0].ToString();
                }
            }
            //LogHelper.Log.Info("【查外壳码失败！】" + sn);
            return "";
        }

        public string GetProductTestFinalResult(string pcbsn,string productsn)
        {
            bool IsFinalResultPass = true;
            var productTypeNo = GetProductTypeNoOfSN(pcbsn);
            DataTable stationList = SelectStationList(productTypeNo).Tables[0];
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
                        testResultBasic.TestResultValue = dtResult.Rows[0][5].ToString();
                        var currentTestResult = testResultBasic.TestResultValue.Trim().ToLower();
                        if (currentTestResult != "pass")
                        {
                            IsFinalResultPass = false;
                        }
                    }
                    else
                    {
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

        private string UpdateCurrentStation(string station)
        {
            var selectSQL = $"SELECT DISTINCT {DbTable.F_TECHNOLOGICAL_PROCESS.STATION_NAME} " +
                $"FROM {DbTable.F_TECHNOLOGICAL_PROCESS_NAME} " +
                $"WHERE {DbTable.F_TECHNOLOGICAL_PROCESS.STATION_NAME} like '%{station}%'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }
            return station;
        }

        /// <summary>
        /// 查询当前PCBASN的产品型号
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        private string GetProductTypeNoOfSN(string sn)
        {
            var selectSQL = $"SELECT TOP 1 {DbTable.F_Test_Result.TYPE_NO} FROM " +
                $"{DbTable.F_TEST_RESULT_NAME} WHERE {DbTable.F_Test_Result.SN} = '{sn}' " +
                $"ORDER BY {DbTable.F_Test_Result.UPDATE_DATE} DESC";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();
            return "";
        }

        public DataSet SelectTestResultLogDetail(string queryFilter,string startTime,string endTime)
        {
            //更新当前工站名称
            STATION_TURN = UpdateCurrentStation(STATION_TURN);
            STATION_STENT = UpdateCurrentStation(STATION_STENT);
            STATION_SHELL = UpdateCurrentStation(STATION_SHELL);
            STATION_SENSIBLITY = UpdateCurrentStation(STATION_SENSIBLITY);
            STATION_PRODUCT = UpdateCurrentStation(STATION_PRODUCT);
            STATION_AIR = UpdateCurrentStation(STATION_AIR);

            DataSet ds = new DataSet();
            var dt = InitTestResultDataTable(true);
            var selectTestResultSQL = "";
            if (queryFilter == "")
            {
                selectTestResultSQL = $"SELECT " +
                            $"{DbTable.F_Test_Result.SN}," +
                            $"{DbTable.F_Test_Result.TYPE_NO}," +
                            $"{DbTable.F_Test_Result.STATION_NAME}," +
                            $"{DbTable.F_Test_Result.TEST_RESULT}," +
                            $"{DbTable.F_Test_Result.STATION_IN_DATE}," +
                            $"{DbTable.F_Test_Result.STATION_OUT_DATE}," +
                            $"{DbTable.F_Test_Result.TEAM_LEADER}," +
                            $"{DbTable.F_Test_Result.JOIN_DATE_TIME} " +
                            $"FROM " +
                            $"{DbTable.F_TEST_RESULT_NAME} " +
                            $"WHERE " +
                            $"{DbTable.F_Test_Result.UPDATE_DATE} >= '{startTime}' " +
                            $"AND " +
                            $"{DbTable.F_Test_Result.UPDATE_DATE} <= '{endTime}' " +
                            $"ORDER BY " +
                            $"{DbTable.F_Test_Result.UPDATE_DATE} " +
                            $"DESC";
            }
            else
            {
                var pcbaSN = GetPCBASn(queryFilter);
                var productSN = GetProductSn(queryFilter);
                selectTestResultSQL = $"SELECT " +
                            $"{DbTable.F_Test_Result.SN}," +
                            $"{DbTable.F_Test_Result.TYPE_NO}," +
                            $"{DbTable.F_Test_Result.STATION_NAME}," +
                            $"{DbTable.F_Test_Result.TEST_RESULT}," +
                            $"{DbTable.F_Test_Result.STATION_IN_DATE}," +
                            $"{DbTable.F_Test_Result.STATION_OUT_DATE}," +
                            $"{DbTable.F_Test_Result.TEAM_LEADER}," +
                            $"{DbTable.F_Test_Result.JOIN_DATE_TIME} " +
                            $"FROM " +
                            $"{DbTable.F_TEST_RESULT_NAME} " +
                            $"WHERE " +
                            $"{DbTable.F_Test_Result.UPDATE_DATE} >= '{startTime}' " +
                            $"AND " +
                            $"{DbTable.F_Test_Result.UPDATE_DATE} <= '{endTime}' " +
                            $"AND " +
                            $"{DbTable.F_Test_Result.SN} = '{pcbaSN}' " +
                            $"OR " +
                            $"{DbTable.F_Test_Result.SN} = '{productSN}' " +
                            $"OR " +
                            $"{DbTable.F_Test_Result.STATION_NAME} = '{queryFilter}'";
            }
            LogHelper.Log.Info("【开始查询过站历史】"+selectTestResultSQL);
            var dtResult = SQLServer.ExecuteDataSet(selectTestResultSQL).Tables[0];
            if (dtResult.Rows.Count > 0)
            {
                var count = 0;
                foreach (DataRow dataRow in dtResult.Rows)
                {
                    DataRow dr = dt.NewRow();
                    var pcbaSNTemp = dataRow[0].ToString();
                    var productTypeNo = dataRow[1].ToString();
                    var stationName = dataRow[2].ToString();
                    var testResult = dataRow[3].ToString();
                    var stationInDate = dataRow[4].ToString();
                    var stationOutDate = dataRow[5].ToString();
                    var teamLeader = dataRow[6].ToString();
                    var joinDateTime = dataRow[7].ToString();
                    var pcbaSN = GetPCBASn(pcbaSNTemp);
                    var productSN = GetProductSn(pcbaSNTemp);

                    dr[TestResultItemContent.Order] = count + 1;
                    dr[TestResultItemContent.PcbaSN] = GetPCBASn(pcbaSN);
                    dr[TestResultItemContent.ProductSN] = GetProductSn(pcbaSN);
                    dr[TestResultItemContent.ProductTypeNo] = productTypeNo;
                    dr[TestResultItemContent.FinalResultValue] = GetProductTestFinalResult(pcbaSN, productSN);

                    #region 烧录工位信息
                    if (STATION_TURN == stationName)
                    {
                        //只更新烧录工站
                        dr[STATION_TURN + TestResultItemContent.StationInDate_turn] = stationInDate;
                        dr[STATION_TURN + TestResultItemContent.StationOutDate_turn] = stationOutDate;
                        dr[STATION_TURN + TestResultItemContent.UserTeamLeader_turn] = teamLeader;
                        dr[STATION_TURN + TestResultItemContent.TestResultValue_turn] = testResult;
                        dr[STATION_TURN + TestResultItemContent.Turn_TurnItem] = SelectTestItemValue(pcbaSN, productSN, STATION_TURN, TestResultItemContent.Turn_TurnItem,joinDateTime);
                        dr[STATION_TURN + TestResultItemContent.Turn_Voltage_12V_Item] = SelectTestItemValue(pcbaSN, productSN, STATION_TURN, TestResultItemContent.Turn_Voltage_12V_Item, joinDateTime);
                        dr[STATION_TURN + TestResultItemContent.Turn_Voltage_5V_Item] = SelectTestItemValue(pcbaSN, productSN, STATION_TURN, TestResultItemContent.Turn_Voltage_5V_Item, joinDateTime);
                        dr[STATION_TURN + TestResultItemContent.Turn_Voltage_33_1V_Item] = SelectTestItemValue(pcbaSN, productSN, STATION_TURN, TestResultItemContent.Turn_Voltage_33_1V_Item, joinDateTime);
                        dr[STATION_TURN + TestResultItemContent.Turn_Voltage_33_2V_Item] = SelectTestItemValue(pcbaSN, productSN, STATION_TURN, TestResultItemContent.Turn_Voltage_33_2V_Item, joinDateTime);
                        dr[STATION_TURN + TestResultItemContent.Turn_SoftVersion] = SelectTestItemValue(pcbaSN, productSN, STATION_TURN, TestResultItemContent.Turn_SoftVersion,joinDateTime);
                    }
                    #endregion

                    #region 灵敏度
                    if (STATION_SENSIBLITY == stationName)
                    {
                        dr[STATION_SENSIBLITY + TestResultItemContent.StationInDate_sen] = stationInDate;
                        dr[STATION_SENSIBLITY + TestResultItemContent.StationOutDate_sen] = stationOutDate;
                        dr[STATION_SENSIBLITY + TestResultItemContent.UserTeamLeader_sen] = teamLeader;
                        dr[STATION_SENSIBLITY + TestResultItemContent.TestResultValue_sen] = testResult;

                        dr[STATION_SENSIBLITY + TestResultItemContent.Sen_Work_Electric_Test] = SelectTestItemValue(pcbaSN, productSN, STATION_SENSIBLITY, TestResultItemContent.Sen_Work_Electric_Test,joinDateTime);
                        dr[STATION_SENSIBLITY + TestResultItemContent.Sen_PartNumber] = SelectTestItemValue(pcbaSN, productSN, STATION_SENSIBLITY, TestResultItemContent.Sen_PartNumber,joinDateTime);
                        dr[STATION_SENSIBLITY + TestResultItemContent.Sen_HardWareVersion] = SelectTestItemValue(pcbaSN, productSN, STATION_SENSIBLITY, TestResultItemContent.Sen_HardWareVersion,joinDateTime);
                        dr[STATION_SENSIBLITY + TestResultItemContent.Sen_SoftVersion] = SelectTestItemValue(pcbaSN, productSN, STATION_SENSIBLITY, TestResultItemContent.Sen_SoftVersion,joinDateTime);
                        dr[STATION_SENSIBLITY + TestResultItemContent.Sen_ECUID] = SelectTestItemValue(pcbaSN, productSN, STATION_SENSIBLITY, TestResultItemContent.Sen_ECUID,joinDateTime);
                        dr[STATION_SENSIBLITY + TestResultItemContent.Sen_BootloaderVersion] = SelectTestItemValue(pcbaSN, productSN, STATION_SENSIBLITY, TestResultItemContent.Sen_BootloaderVersion,joinDateTime);
                        dr[STATION_SENSIBLITY + TestResultItemContent.Sen_RadioFreq] = SelectTestItemValue(pcbaSN, productSN, STATION_SENSIBLITY, TestResultItemContent.Sen_RadioFreq,joinDateTime);
                        dr[STATION_SENSIBLITY + TestResultItemContent.Sen_DormantElect] = SelectTestItemValue(pcbaSN, productSN, STATION_SENSIBLITY, TestResultItemContent.Sen_DormantElect,joinDateTime);
                    }
                    #endregion

                    #region 外壳
                    if (stationName == STATION_SHELL)
                    {
                        dr[STATION_SHELL + TestResultItemContent.StationInDate_shell] = stationInDate;
                        dr[STATION_SHELL + TestResultItemContent.StationOutDate_shell] = stationOutDate;
                        dr[STATION_SHELL + TestResultItemContent.UserTeamLeader_shell] = teamLeader;
                        dr[STATION_SHELL + TestResultItemContent.TestResultValue_shell] = testResult;
                        dr[STATION_SHELL + TestResultItemContent.Shell_FrontCover] = SelectTestItemValue(pcbaSN, productSN, STATION_SHELL, TestResultItemContent.Shell_FrontCover,joinDateTime);
                        dr[STATION_SHELL + TestResultItemContent.Shell_BackCover] = SelectTestItemValue(pcbaSN, productSN, STATION_SHELL, TestResultItemContent.Shell_BackCover, joinDateTime);
                        dr[STATION_SHELL + TestResultItemContent.Shell_PCBScrew1] = SelectTestItemValue(pcbaSN, productSN, STATION_SHELL, TestResultItemContent.Shell_PCBScrew1, joinDateTime);
                        dr[STATION_SHELL + TestResultItemContent.Shell_PCBScrew2] = SelectTestItemValue(pcbaSN, productSN, STATION_SHELL, TestResultItemContent.Shell_PCBScrew2, joinDateTime);
                        dr[STATION_SHELL + TestResultItemContent.Shell_PCBScrew3] = SelectTestItemValue(pcbaSN, productSN, STATION_SHELL, TestResultItemContent.Shell_PCBScrew3, joinDateTime);
                        dr[STATION_SHELL + TestResultItemContent.Shell_PCBScrew4] = SelectTestItemValue(pcbaSN, productSN, STATION_SHELL, TestResultItemContent.Shell_PCBScrew4, joinDateTime);
                        dr[STATION_SHELL + TestResultItemContent.Shell_ShellScrew1] = SelectTestItemValue(pcbaSN, productSN, STATION_SHELL, TestResultItemContent.Shell_ShellScrew1, joinDateTime);
                        dr[STATION_SHELL + TestResultItemContent.Shell_ShellScrew2] = SelectTestItemValue(pcbaSN, productSN, STATION_SHELL, TestResultItemContent.Shell_ShellScrew2, joinDateTime);
                        dr[STATION_SHELL + TestResultItemContent.Shell_ShellScrew3] = SelectTestItemValue(pcbaSN, productSN, STATION_SHELL, TestResultItemContent.Shell_ShellScrew3, joinDateTime);
                        dr[STATION_SHELL + TestResultItemContent.Shell_ShellScrew4] = SelectTestItemValue(pcbaSN, productSN, STATION_SHELL, TestResultItemContent.Shell_ShellScrew4, joinDateTime);
                    }
                    #endregion

                    #region 气密
                    if (stationName == STATION_AIR)
                    {
                        dr[STATION_AIR + TestResultItemContent.StationInDate_air] = stationInDate;
                        dr[STATION_AIR + TestResultItemContent.StationOutDate_air] = stationOutDate;
                        dr[STATION_AIR + TestResultItemContent.UserTeamLeader_air] = teamLeader;
                        dr[STATION_AIR + TestResultItemContent.TestResultValue_air] = testResult;
                        dr[STATION_AIR + TestResultItemContent.Air_AirtightTest] = SelectTestItemValue(pcbaSN, productSN, STATION_AIR, TestResultItemContent.Air_AirtightTest,joinDateTime);
                    }
                    #endregion

                    #region 支架
                    if (stationName == STATION_STENT)
                    {
                        dr[STATION_STENT + TestResultItemContent.StationInDate_stent] = stationInDate;
                        dr[STATION_STENT + TestResultItemContent.StationOutDate_stent] = stationOutDate;
                        dr[STATION_STENT + TestResultItemContent.UserTeamLeader_stent] = teamLeader;
                        dr[STATION_STENT + TestResultItemContent.TestResultValue_stent] = testResult;
                        dr[STATION_STENT + TestResultItemContent.Stent_Screw1] = SelectTestItemValue(pcbaSN, productSN, STATION_STENT, TestResultItemContent.Stent_Screw1,joinDateTime);
                        dr[STATION_STENT + TestResultItemContent.Stent_Screw2] = SelectTestItemValue(pcbaSN, productSN, STATION_STENT, TestResultItemContent.Stent_Screw1,joinDateTime);
                        dr[STATION_STENT + TestResultItemContent.Stent_Stent] = SelectTestItemValue(pcbaSN, productSN, STATION_STENT, TestResultItemContent.Stent_Stent,joinDateTime);
                        dr[STATION_STENT + TestResultItemContent.Stent_LeftStent] = SelectTestItemValue(pcbaSN, productSN, STATION_STENT, TestResultItemContent.Stent_LeftStent,joinDateTime);
                        dr[STATION_STENT + TestResultItemContent.Stent_RightStent] = SelectTestItemValue(pcbaSN, productSN, STATION_STENT, TestResultItemContent.Stent_RightStent,joinDateTime);
                    }
                    #endregion

                    #region 成品
                    if (stationName == STATION_PRODUCT)
                    {
                        dr[STATION_PRODUCT + TestResultItemContent.StationInDate_product] = stationInDate;
                        dr[STATION_PRODUCT + TestResultItemContent.StationOutDate_product] = stationOutDate;
                        dr[STATION_PRODUCT + TestResultItemContent.UserTeamLeader_product] = teamLeader;
                        dr[STATION_PRODUCT + TestResultItemContent.TestResultValue_product] = testResult;
                        dr[STATION_PRODUCT + TestResultItemContent.Product_Work_Electric_Test] = SelectTestItemValue(pcbaSN, productSN, STATION_PRODUCT, TestResultItemContent.Product_Work_Electric_Test,joinDateTime);
                        dr[STATION_PRODUCT + TestResultItemContent.Product_DormantElect] = SelectTestItemValue(pcbaSN, productSN, STATION_PRODUCT, TestResultItemContent.Product_DormantElect,joinDateTime);
                    }
                    #endregion

                    dt.Rows.Add(dr);
                    count++;
                }
                LogHelper.Log.Info("【查询过站历史完成】");
                ds.Tables.Add(dt);
            }
            return ds;
        }

        #endregion

        #region 物料信息表
        private List<MaterialMsg> CommitMaterial(List<MaterialMsg> list)
        {
            List<MaterialMsg> materialMsgList = new List<MaterialMsg>();
            foreach (var item in list)
            {
                if (!IsExistMaterial(item.MaterialCode))
                {
                    //insert
                    InsertMaterial(item, materialMsgList);
                }
                else
                {
                    //update
                    UpdateMaterial(item, materialMsgList);
                }
            }
            return materialMsgList;
        }
        public int DeleteMaterial(string materialCode)
        {
            string deleteSQL = $"DELETE FROM {DbTable.F_MATERIAL_NAME} " +
                $"WHERE {DbTable.F_Material.MATERIAL_CODE} = '{materialCode}'";
            LogHelper.Log.Info(deleteSQL);
            return SQLServer.ExecuteNonQuery(deleteSQL);
        }
        public int DeleteAllMaterial()
        {
            string deleteSQL = $"DELETE FROM {DbTable.F_MATERIAL_NAME}";
            return SQLServer.ExecuteNonQuery(deleteSQL);
        }
        public DataSet SelectMaterial(string codeRid)
        {
            var selectSQL = "";
            if (codeRid == "")
            {
                selectSQL = $"SELECT " +
                $"{DbTable.F_Material.MATERIAL_CODE}," +
                $"{DbTable.F_Material.MATERIAL_NAME}," +
                $"{DbTable.F_Material.MATERIAL_USERNAME}," +
                $"{DbTable.F_Material.MATERIAL_UPDATE_DATE}," +
                $"{DbTable.F_Material.MATERIAL_DESCRIBLE}," +
                $"{DbTable.F_Material.MATERIAL_STOCK} " +
                $"FROM {DbTable.F_MATERIAL_NAME}";
            }
            else
            {
                selectSQL = $"SELECT " +
                $"{DbTable.F_Material.MATERIAL_CODE}," +
                $"{DbTable.F_Material.MATERIAL_NAME}," +
                $"{DbTable.F_Material.MATERIAL_USERNAME}," +
                $"{DbTable.F_Material.MATERIAL_UPDATE_DATE}," +
                $"{DbTable.F_Material.MATERIAL_DESCRIBLE}," +
                $"{DbTable.F_Material.MATERIAL_STOCK} " +
                $"FROM {DbTable.F_MATERIAL_NAME} " +
                $"WHERE " +
                $"{DbTable.F_Material.MATERIAL_CODE} like '%{codeRid}%'";
            }
            return SQLServer.ExecuteDataSet(selectSQL);
        }
        private bool IsExistMaterial(string materialCode)
        {
            string selectSQL = $"SELECT * FROM {DbTable.F_MATERIAL_NAME} WHERE {DbTable.F_Material.MATERIAL_CODE} = '{materialCode}'";
            DataTable dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return true;
            else
                return false;
        }
        private void InsertMaterial(MaterialMsg material,List<MaterialMsg> materialMsgList)
        {
            MaterialMsg materialMsg = new MaterialMsg();
            string insertSQL = $"INSERT INTO {DbTable.F_MATERIAL_NAME}(" +
                $"{DbTable.F_Material.MATERIAL_CODE}," +
                $"{DbTable.F_Material.MATERIAL_NAME}," +
                $"{DbTable.F_Material.MATERIAL_USERNAME}," +
                $"{DbTable.F_Material.MATERIAL_DESCRIBLE}) " +
                $"VALUES('{material.MaterialCode}','{material.MaterialName}','{material.UserName}','{material.Describle}')";
            LogHelper.Log.Info($"InsertMaterial={insertSQL}");
            materialMsg.MaterialCode = material.MaterialCode;
            materialMsg.Result = SQLServer.ExecuteNonQuery(insertSQL);

            var materialQty = "";
            if (material.MaterialCode.Contains("&"))
            {
                materialQty = AnalysisMaterialCode.GetMaterialDetail(material.MaterialCode).MaterialQTY;
            }
            else
            {
                LogHelper.Log.Error($"【物料编码不包含&，未解析到物料数量】+materialQty={materialQty}");
            }
            int qty = 0;
            if (materialMsg.Result > 0)
            {
                qty = UpdateMaterialStock(material.MaterialCode, materialQty);
                if (qty < 1)
                {
                    LogHelper.Log.Error("【更新物料库存失败！】");
                }
            }
            materialMsgList.Add(materialMsg);
        }
        private void UpdateMaterial(MaterialMsg material,List<MaterialMsg> materialMsgList)
        {
            string updateSQL = $"UPDATE {DbTable.F_MATERIAL_NAME} SET " +
                $"{DbTable.F_Material.MATERIAL_NAME} = '{material.MaterialName}'," +
                $"{DbTable.F_Material.MATERIAL_USERNAME} = '{material.UserName}'," +
                $"{DbTable.F_Material.MATERIAL_DESCRIBLE} = '{material.Describle}'," +
                $"{DbTable.F_Material.MATERIAL_UPDATE_DATE} = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' " +
                $"WHERE {DbTable.F_Material.MATERIAL_CODE} = '{material.MaterialCode}'";
            LogHelper.Log.Info(updateSQL);
            var selectSQL = $"SELECT * FROM {DbTable.F_MATERIAL_NAME} WHERE " +
                $"{DbTable.F_Material.MATERIAL_CODE} = '{material.MaterialCode}' AND " +
                $"{DbTable.F_Material.MATERIAL_NAME} = '{material.MaterialName}'";
            if (SQLServer.ExecuteDataSet(selectSQL).Tables[0].Rows.Count > 0)
            {
                //存在 没有可更新信息
                return ;
            }
            MaterialMsg materialMsg = new MaterialMsg();
            materialMsg.MaterialCode = material.MaterialCode;
            materialMsg.Result = SQLServer.ExecuteNonQuery(updateSQL);
            materialMsgList.Add(materialMsg);
        }

        private int UpdateMaterialStock(string materialCode,string stock)
        {
            var updateSQL = $"UPDATE {DbTable.F_MATERIAL_NAME} SET " +
                $"{DbTable.F_Material.MATERIAL_STOCK} = '{stock}' WHERE " +
                $"{DbTable.F_Material.MATERIAL_CODE} = '{materialCode}'";
            return SQLServer.ExecuteNonQuery(updateSQL);
        }

        public int UpdateMaterialPN(string materialPN,string materialName,string username)
        {
            if (IsExistMaterialPN(materialPN))
            {
                //update name
                var updateSQL = $"UPDATE {DbTable.F_MATERIAL_PN_NAME} SET " +
                    $"{DbTable.F_MATERIAL_PN.MATERIAL_NAME} = '{materialName}'," +
                    $"{DbTable.F_MATERIAL_PN.USER_NAME} = '{username}' " +
                    $"WHERE " +
                    $"{DbTable.F_MATERIAL_PN.MATERIAL_PN} = '{materialPN}'";
                LogHelper.Log.Info("【更新物料PN-名称】" + updateSQL);
                return SQLServer.ExecuteNonQuery(updateSQL);
            }
            else
            {
                //insert
                var insertSQL = $"INSERT INTO {DbTable.F_MATERIAL_PN_NAME}(" +
                    $"{DbTable.F_MATERIAL_PN.MATERIAL_PN}," +
                    $"{DbTable.F_MATERIAL_PN.MATERIAL_NAME}," +
                    $"{DbTable.F_MATERIAL_PN.USER_NAME}) VALUES(" +
                    $"'{materialPN}','{materialName}','{username}')";
                LogHelper.Log.Info("【更新物料PN】"+insertSQL);
                return SQLServer.ExecuteNonQuery(insertSQL);
            }
        }

        public DataSet SelectMaterialPN()
        {
            var selectSQL = $"SELECT {DbTable.F_MATERIAL_PN.MATERIAL_PN}," +
                $"{DbTable.F_MATERIAL_PN.MATERIAL_NAME}," +
                $"{DbTable.F_MATERIAL_PN.USER_NAME}," +
                $"{DbTable.F_MATERIAL_PN.UPDATE_DATE}," +
                $"{DbTable.F_MATERIAL_PN.DESCRIBLE} FROM " +
                $"{DbTable.F_MATERIAL_PN_NAME} ";
            return SQLServer.ExecuteDataSet(selectSQL);
        }

        public string SelectMaterialName(string materialPN)
        {
            if (materialPN == "")
                return "";
            var selectSQL = $"SELECT {DbTable.F_MATERIAL_PN.MATERIAL_NAME} FROM " +
                $"{DbTable.F_MATERIAL_PN_NAME} WHERE {DbTable.F_MATERIAL_PN.MATERIAL_PN} = '{materialPN}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count < 1)
                return "";
            return dt.Rows[0][0].ToString();
        }

        private bool IsExistMaterialPN(string materialPN)
        {
            var selectSQL = $"SELECT * FROM {DbTable.F_MATERIAL_PN_NAME} WHERE " +
                $"{DbTable.F_MATERIAL_PN.MATERIAL_PN} = '{materialPN}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }

        private bool IsExistMaterialPN_Name(string materialPN,string materialName)
        {
            var selectSQL = $"SELECT * FROM {DbTable.F_MATERIAL_PN_NAME} WHERE " +
                $"{DbTable.F_MATERIAL_PN.MATERIAL_PN} = '{materialPN}' AND " +
                $"{DbTable.F_MATERIAL_PN.MATERIAL_NAME} = '{materialName}' ";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }

        #endregion

        #region 产品物料绑定
        public List<ProductMaterial> CommitProductMaterial(List<ProductMaterial> pmList)
        {
            List<ProductMaterial> productMaterialList = new List<ProductMaterial>();
            foreach (var material in pmList)
            {
                if (IsExistMaterial(material))
                {
                    //更新
                    string updateSQL = $"UPDATE {DbTable.F_PRODUCT_MATERIAL_NAME} SET " +
                        $"{DbTable.F_PRODUCT_MATERIAL.Describle} = '{material.Describle}'," +
                        $"{DbTable.F_PRODUCT_MATERIAL.USERNAME} = '{material.UserName}' " +
                        $"WHERE {DbTable.F_PRODUCT_MATERIAL.TYPE_NO} = '{material.TypeNo}' AND " +
                        $"{DbTable.F_PRODUCT_MATERIAL.MATERIAL_CODE} = '{material.MaterialCode}'";

                    string selectSQL = $"SELECT * FROM {DbTable.F_PRODUCT_MATERIAL_NAME} WHERE " +
                        $"{DbTable.F_PRODUCT_MATERIAL.TYPE_NO} = '{material.TypeNo}' AND " +
                        $"{DbTable.F_PRODUCT_MATERIAL.MATERIAL_CODE} = '{material.MaterialCode}' AND " +
                        $"{DbTable.F_PRODUCT_MATERIAL.Describle} = '{material.Describle}' AND " +
                        $"{DbTable.F_PRODUCT_MATERIAL.USERNAME} = '{material.UserName}'";
                    ProductMaterial productMaterial = new ProductMaterial();
                    if (SQLServer.ExecuteDataSet(selectSQL).Tables[0].Rows.Count < 1)
                    {
                        productMaterial.Result = SQLServer.ExecuteNonQuery(updateSQL);
                        productMaterial.MaterialCode = material.MaterialCode;
                        productMaterial.TypeNo = material.TypeNo;
                    }
                }
                else
                {
                    //insert
                    if (InsertProductMaterial(material, productMaterialList) > 0)
                    {
                        //插入成功，更新库存
                        UpdateMaterialStock(material.TypeNo, material.MaterialCode, material.Stock);
                    }
                }
                UpdateMaterialPN(material.MaterialCode,material.MaterialName,material.UserName);
            }
            return productMaterialList;
        }
        public int DeleteProductMaterial(ProductMaterial material)
        {
            if (material.MaterialCode == "" || material.TypeNo == "")
                return 0;
            string deleteSQL = $"DELETE FROM {DbTable.F_PRODUCT_MATERIAL_NAME} " +
                $"WHERE {DbTable.F_PRODUCT_MATERIAL.TYPE_NO} = '{material.TypeNo}' AND " +
                $"{DbTable.F_PRODUCT_MATERIAL.MATERIAL_CODE} = '{material.MaterialCode}'";
            LogHelper.Log.Info(deleteSQL);
            return SQLServer.ExecuteNonQuery(deleteSQL);
        }
        public DataSet SelectProductMaterial()
        {
            var selectSQL = $"SELECT " +
                            $"a.{DbTable.F_PRODUCT_MATERIAL.TYPE_NO}," +
                            $"a.{DbTable.F_PRODUCT_MATERIAL.MATERIAL_CODE}," +
                            $"b.{DbTable.F_MATERIAL_PN.MATERIAL_NAME}," +
                            $"a.{DbTable.F_PRODUCT_MATERIAL.Describle}," +
                            $"a.{DbTable.F_PRODUCT_MATERIAL.USERNAME}," +
                            $"a.{DbTable.F_PRODUCT_MATERIAL.UpdateDate} " +
                            $"FROM " +
                            $"{DbTable.F_PRODUCT_MATERIAL_NAME} a," +
                            $"{DbTable.F_MATERIAL_PN_NAME} b " +
                            $"WHERE " +
                            $"a.{DbTable.F_PRODUCT_MATERIAL.MATERIAL_CODE} = b.{DbTable.F_MATERIAL_PN.MATERIAL_PN} " +
                            $"ORDER BY {DbTable.F_PRODUCT_MATERIAL.TYPE_NO} ";
            LogHelper.Log.Info(selectSQL);
            return SQLServer.ExecuteDataSet(selectSQL);
        }
        private bool IsExistMaterial(ProductMaterial material)
        {
            string selectSQL = $"SELECT * FROM {DbTable.F_PRODUCT_MATERIAL_NAME} " +
                $"WHERE {DbTable.F_PRODUCT_MATERIAL.MATERIAL_CODE} = '{material.MaterialCode}' AND " +
                $"{DbTable.F_PRODUCT_MATERIAL.TYPE_NO} = '{material.TypeNo}'";
            DataTable dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return true;
            else
                return false;
        }
        private int InsertProductMaterial(ProductMaterial material,List<ProductMaterial> productMaterialList)
        {
            ProductMaterial productMaterial = new ProductMaterial();
            string insertSQL = $"INSERT INTO {DbTable.F_PRODUCT_MATERIAL_NAME}(" +
                $"{DbTable.F_PRODUCT_MATERIAL.TYPE_NO}," +
                $"{DbTable.F_PRODUCT_MATERIAL.MATERIAL_CODE}," +
                $"{DbTable.F_PRODUCT_MATERIAL.Describle}," +
                $"{DbTable.F_PRODUCT_MATERIAL.USERNAME}) " +
                $"VALUES('{material.TypeNo}','{material.MaterialCode}','{material.Describle}','{material.UserName}')";
            LogHelper.Log.Info(insertSQL);
            productMaterial.Result = SQLServer.ExecuteNonQuery(insertSQL);
            productMaterial.MaterialCode = material.MaterialCode;
            productMaterial.TypeNo = material.TypeNo;
            productMaterialList.Add(productMaterial);
            return productMaterial.Result;
        }

        #region 添加绑定时更新物料库存
        private int UpdateMaterialStock(string typeNo, string materialCode, string stock)
        {
            var updateSQL = $"UPDATE {DbTable.F_PRODUCT_MATERIAL_NAME} SET " +
                $"{DbTable.F_PRODUCT_MATERIAL.STOCK} = '{stock}' WHERE " +
                $"{DbTable.F_PRODUCT_MATERIAL.TYPE_NO} = '{typeNo}' AND " +
                $"{DbTable.F_PRODUCT_MATERIAL.MATERIAL_CODE} = '{materialCode}'";
            return SQLServer.ExecuteNonQuery(updateSQL);
        }
        #endregion
        #endregion

        #region 物料统计表
        [SwaggerWcfTag("MesServcie 服务")]
        /// <summary>
        /// 测试端传入装配消耗物料计数
        /// </summary>
        /// <param name="snInner"></param>
        /// <param name="snOutter"></param>
        /// <param name="typeNo"></param>
        /// <param name="stationName"></param>
        /// <param name="materialCode"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public string InsertMaterialStatistics(string snInner, string snOutter, string typeNo, string stationName, string materialCode, string amount)
        {
            string[] array = new string[] { snInner, snOutter, typeNo, stationName, materialCode, amount };
            insertMaterialStatistics.Enqueue(array);
            return MaterialStatistics.InsertMaterialStatistics(insertMaterialStatistics);
        }
        #endregion

        #region 物料综合查询
        public DataSet SelectMaterialBasicMsg(string queryCondition)
        {
            var selectSQL = "";
            var pcbaSN = GetPCBASn(queryCondition);
            var productSN = GetProductSn(queryCondition);
            selectSQL = $"SELECT DISTINCT " +
                $"a.{DbTable.F_Material_Statistics.MATERIAL_CODE} 物料编码," +
                $"b.{DbTable.F_Material.MATERIAL_NAME} 物料名称," +
                $"a.{DbTable.F_Material_Statistics.PRODUCT_TYPE_NO} 产品型号," +
                $"SUM(a.{DbTable.F_Material_Statistics.MATERIAL_AMOUNT}) 使用总数量," +
                $"a.{DbTable.F_Material_Statistics.PCBA_SN}," +
                $"b.{DbTable.F_Material.MATERIAL_AMOUNTED}," +
                $"b.{DbTable.F_Material.MATERIAL_STOCK}," +
                $"a.{DbTable.F_Material_Statistics.MATERIAL_CURRENT_REMAIN} " +
                $"FROM " +
                $"{DbTable.F_MATERIAL_STATISTICS_NAME} a," +
                $"{DbTable.F_MATERIAL_NAME} b " +
                $"WHERE " +
                $"a.{DbTable.F_Material_Statistics.MATERIAL_CODE} = b.{DbTable.F_Material.MATERIAL_CODE} AND " +
                $"a.{DbTable.F_Material_Statistics.MATERIAL_CODE} like '%{queryCondition}%' OR " +
                $"a.{DbTable.F_Material_Statistics.PCBA_SN} = '{pcbaSN}' OR " +
                $"a.{DbTable.F_Material_Statistics.PCBA_SN} = '{productSN}' " +
                $"GROUP BY " +
                $"a.{DbTable.F_Material_Statistics.MATERIAL_CODE}," +
                $"b.{DbTable.F_Material.MATERIAL_NAME}," +
                $"a.{DbTable.F_Material_Statistics.PRODUCT_TYPE_NO}," +
                $"a.{DbTable.F_Material_Statistics.PCBA_SN}," +
                $"b.{DbTable.F_Material.MATERIAL_AMOUNTED}," +
                $"b.{DbTable.F_Material.MATERIAL_STOCK}," +
                $"a.{DbTable.F_Material_Statistics.MATERIAL_CURRENT_REMAIN}";

            LogHelper.Log.Info(selectSQL);
            return SQLServer.ExecuteDataSet(selectSQL);
        }

        public DataSet SelectMaterialDetailMsg(string materialCode)
        {
            var selectSQL = $"SELECT DISTINCT " +
                           $"a.{DbTable.F_Material_Statistics.MATERIAL_CODE} 物料编码," +
                           $"b.{DbTable.F_Material.MATERIAL_NAME} 物料名称," +
                           $"a.{DbTable.F_Material_Statistics.PRODUCT_TYPE_NO} 产品型号," +
                           $"a.{DbTable.F_Material_Statistics.STATION_NAME} 工站名称," +
                           $"a.{DbTable.F_Material_Statistics.MATERIAL_AMOUNT} 使用数量," +
                           $"a.{DbTable.F_Material_Statistics.TEAM_LEADER}," +
                           $"a.{DbTable.F_Material_Statistics.ADMIN}," +
                           $"a.{DbTable.F_Material_Statistics.UPDATE_DATE}," +
                           $"a.{DbTable.F_Material_Statistics.PCBA_SN}," +
                           $"b.{DbTable.F_Material.MATERIAL_AMOUNTED}," +
                           $"b.{DbTable.F_Material.MATERIAL_STOCK}," +
                           $"a.{DbTable.F_Material_Statistics.MATERIAL_CURRENT_REMAIN} " +
                           $"FROM " +
                           $"{DbTable.F_MATERIAL_STATISTICS_NAME} a," +
                           $"{DbTable.F_MATERIAL_NAME} b " +
                           $"WHERE " +
                           $"a.{DbTable.F_Material_Statistics.MATERIAL_CODE} = b.{DbTable.F_Material.MATERIAL_CODE} AND " +
                           $"a.{DbTable.F_Material_Statistics.MATERIAL_CODE} like '{materialCode}' ";
            LogHelper.Log.Info(selectSQL);
            return SQLServer.ExecuteDataSet(selectSQL);
        }
        #endregion

        #region 产品合格率统计接口
        #endregion

        #region 验证传入工站是否可以生产/测试
        /// <summary>
        /// flash为首站，传入二维码后，查询二维码是否存在，不存在-则根据型号和二维码创建信息，存在-判断是不是在本站生产
        /// </summary>
        /// <param name="sn">追溯号/条码</param>
        /// <param name="sTypeNumber">型号/零件号</param>
        /// <param name="sStationName">工站名称/站位名称</param>
        public string FirstCheck(string sn_inner, string sn_outter, string sTypeNumber, string sStationName)
        {
            //加入队列
            string[] array = new string[] { sn_inner, sn_outter, sTypeNumber, sStationName };
            fcQueue.Enqueue(array);
            LogHelper.Log.Info($"FirstCheck接口被调用，传入参数[{sn_inner},{sn_outter},{sTypeNumber},{sStationName}] 当前队列count={fcQueue.Count}");
            return FirstCheckQueue.CheckPass(fcQueue);
        }

        #endregion

        #region 型号所属站位增删改查
        public string CommitTypeStation(Dictionary<string, string[]> dctData)
        {
            LogHelper.Log.Info($"接口被调用-CommitTypeStation");
            foreach (var typeNumber in dctData.Keys)
            {
                string[] arrayStation = dctData[typeNumber];
                //插入数据库
                //插入SQL
                if (IsExistTypeStation(typeNumber))
                {
                    //ID已存在
                    //update
                    LogHelper.Log.Info("product type_number is exist, Excute UpdateProduceDB...");
                    UpdateTypeStation(typeNumber, arrayStation);
                }
                else
                {
                    //不存在，插入
                    LogHelper.Log.Info($"product type number is not exist value={typeNumber}, Excute Insert Into Table...");
                    string insertSQL = "INSERT INTO [WT_SCL].[dbo].[Product_Process]" +
                            "([Type_Number],[Station_Name_1],[Station_Name_2],[Station_Name_3],[Station_Name_4],[Station_Name_5]," +
                            "[Station_Name_6],[Station_Name_7],[Station_Name_8],[Station_Name_9],[Station_Name_10]) " +
                            $"VALUES('{typeNumber}','{arrayStation[0]}','{arrayStation[1]}','{arrayStation[2]}','{arrayStation[3]}'," +
                            $"'{arrayStation[4]}','{arrayStation[5]}','{arrayStation[6]}','{arrayStation[7]}','{arrayStation[8]}','{arrayStation[9]}')";

                    int res = SQLServer.ExecuteNonQuery(insertSQL);
                    if (res < 1)
                    {
                        //插入失败
                        LogHelper.Log.Info($"product type number table insert fail {insertSQL}");
                        return "0" + $" {typeNumber}";
                    }
                }
            }
            return "1";
        }

        /// <summary>
        /// 产品型号是否为空
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool IsExistTypeStation(string typeNumber)
        {
            string selectSQL = "SELECT  * " +
                    "FROM [WT_SCL].[dbo].[Product_Process] " +
                    $"WHERE [Type_Number] = '{typeNumber}'";
            LogHelper.Log.Info($"Exist Product type number = {selectSQL}");
            DataTable dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 执行更新数据库产品型号所属站位表
        /// </summary>
        /// <param name="order"></param>
        /// <param name="stationName"></param>
        /// <returns></returns>
        private bool UpdateTypeStation(string typeNumber, string[] arrayStation)
        {
            string updateSQL = "UPDATE [WT_SCL].[dbo].[Product_Process] " +
                $"SET Station_Name_1 = '{arrayStation[0]}',Station_Name_2='{arrayStation[1]}',Station_Name_3='{arrayStation[2]}'," +
                $"Station_Name_4='{arrayStation[3]}',Station_Name_5='{arrayStation[4]}',Station_Name_6 = '{arrayStation[5]}'," +
                $"Station_Name_7 = '{arrayStation[6]}',Station_Name_8 = '{arrayStation[7]}',Station_Name_9 = '{arrayStation[8]}',Station_Name_10 = '{arrayStation[9]}' " +
                $"WHERE Type_Number = '{typeNumber}'";
            int r = SQLServer.ExecuteNonQuery(updateSQL);
            LogHelper.Log.Info($"Update Product Type Number={updateSQL}");
            if (r > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 查询当前型号所属站位列表
        /// </summary>
        /// <returns></returns>
        public DataSet SelectTypeStation(string typeNumber)
        {
            string selectSQL = "";
            if (string.IsNullOrEmpty(typeNumber.Trim()))
            {
                selectSQL = $"SELECT * FROM [WT_SCL].[dbo].[Product_Process] ORDER BY [Type_Number]";
            }
            else
            {
                selectSQL = $"SELECT * FROM [WT_SCL].[dbo].[Product_Process] WHERE [Type_Number] = '{typeNumber}' ORDER BY [Type_Number]";
            }

            LogHelper.Log.Info($"Select Product Type Number={selectSQL}");
            return SQLServer.ExecuteDataSet(selectSQL);
        }

        /// <summary>
        /// 清除所有数据
        /// </summary>
        /// <returns></returns>
        public int DeleteAllTypeStation()
        {
            string deleteSQL = "DELETE FROM [WT_SCL].[dbo].[Product_Process]";
            return SQLServer.ExecuteNonQuery(deleteSQL);
        }

        /// <summary>
        /// 删除某条记录
        /// </summary>
        /// <param name="stationName"></param>
        /// <returns></returns>
        public int DeleteTypeStation(string typeNumber)
        {
            string deleteSQL = $"DELETE FROM [WT_SCL].[dbo].[Product_Process] WHERE [Type_Number] = '{typeNumber}'";
            LogHelper.Log.Info($"DeleteProductType={deleteSQL}");
            return SQLServer.ExecuteNonQuery(deleteSQL);
        }
        #endregion

        #region 成品打包绑定
        [SwaggerWcfTag("MesServcie 服务")]
        public int CommitPackageProduct(List<PackageProduct> packageProductList)
        {
            string imageName = "@imageData";
            for(int i = 0;i< packageProductList.Count;i++)
            {
                string insertSQL = $"INSERT INTO {DbTable.F_PRODUCT_PACKAGE_NAME}({DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE}," +
                $"{DbTable.F_PRODUCT_PACKAGE.SN_OUTTER},{DbTable.F_PRODUCT_PACKAGE.TYPE_NO}," +
                $"{DbTable.F_PRODUCT_PACKAGE.PICTURE},{DbTable.F_PRODUCT_PACKAGE.BINDING_STATE}," +
                $"{DbTable.F_PRODUCT_PACKAGE.BINDING_DATE},{DbTable.F_PRODUCT_PACKAGE.REMARK}) " +
                $"VALUES('{packageProductList[i].CaseCode}','{packageProductList[i].SnOutter}','{packageProductList[i].TypeNo}',{imageName}," +
                $"'{packageProductList[i].BindingState}','{packageProductList[i].BindingDate}','{packageProductList[i].Remark}')";

                LogHelper.Log.Info($"CommitPackageProduct Init Insert={insertSQL}");
                if (IsExistPackageProduct(packageProductList[i].CaseCode, packageProductList[i].SnOutter))
                {
                    //update
                    UpdatePackageProduct(packageProductList[i]);
                }
                else
                {
                    try
                    {
                        SqlParameter[] sqlParameters = new SqlParameter[1];
                        SqlParameter sqlParameter = new SqlParameter();
                        sqlParameter.ParameterName = imageName;
                        sqlParameter.SqlDbType = SqlDbType.Binary;
                        sqlParameter.Value = packageProductList[i].Picture;
                        sqlParameters[0] = sqlParameter;
                        SQLServer.ExecuteNonQuery(insertSQL, sqlParameters);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Log.Error($"Err start ExecuteNonQuery={ex.Message}\r\n{ex.StackTrace}");
                        return -1;
                    }
                    //return ExecuteSQL(insertSQL,packageProduct.Picture);
                }
            }
            return 1;
        }
        private bool IsExistPackageProduct(string caseCode, string snOutter)
        {
            string selectSQL = $"SELECT * FROM {DbTable.F_PRODUCT_PACKAGE_NAME} " +
                $"WHERE {DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE} = '{caseCode}' AND " +
                $"{DbTable.F_PRODUCT_PACKAGE.SN_OUTTER} = '{snOutter}'";
            DataTable dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }
        #endregion

        #region 更新打包产品
        public int UpdatePackageProduct(PackageProduct packageProduct)
        {
            string updateSQL = $"UPDATE {DbTable.F_PRODUCT_PACKAGE_NAME} SET " +
                $"{DbTable.F_PRODUCT_PACKAGE.BINDING_STATE} = '{packageProduct.BindingState}' " +
                $"WHERE {DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE} = '{packageProduct.CaseCode}' AND " +
                $"{DbTable.F_PRODUCT_PACKAGE.SN_OUTTER} = '{packageProduct.SnOutter}' ";
            LogHelper.LogInfo($"UpdatePackageProduct={updateSQL}");
            return SQLServer.ExecuteNonQuery(updateSQL);
        }
        #endregion

        #region 删除打包产品
        [SwaggerWcfTag("MesServcie 服务")]
        public int DeletePackageProduct(PackageProduct packageProduct)
        {
            string deleteSQL = "";
            if (packageProduct.SnOutter == "" && packageProduct.CaseCode == "")
            {
                deleteSQL = $"DELETE FROM {DbTable.F_PRODUCT_PACKAGE_NAME} ";
            }
            else
            {
                deleteSQL = $"DELETE FROM {DbTable.F_PRODUCT_PACKAGE_NAME} " +
                $"WHERE {DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE} = '{packageProduct.CaseCode}' AND " +
                $"{DbTable.F_PRODUCT_PACKAGE.SN_OUTTER} = '{packageProduct.SnOutter}'";
            }
            return SQLServer.ExecuteNonQuery(deleteSQL);
        }
        #endregion

        #region 查询打包产品
        [SwaggerWcfTag("MesServcie 服务")]
        public DataSet SelectPackageProduct(string casecode,string queryFilter,string state,bool IsShowNumber)
        {
            //箱子编码/追溯码查询/产品型号
            var rowNumber = "";
            if (IsShowNumber)
            {
                rowNumber = $"ROW_NUMBER() OVER(ORDER BY {DbTable.F_PRODUCT_PACKAGE.BINDING_DATE} DESC) 序号,";
            }
            string selectSQL = "";
            if (string.IsNullOrEmpty(queryFilter))
            {
                //查询所有已绑定记录
                selectSQL = $"SELECT {rowNumber}{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE} 箱子编码," +
                    $"{DbTable.F_PRODUCT_PACKAGE.SN_OUTTER} 产品SN," +
                    $"{DbTable.F_PRODUCT_PACKAGE.TYPE_NO} 产品型号," +
                    $"{DbTable.F_PRODUCT_PACKAGE.TEAM_LEADER} 班组长," +
                    $"{DbTable.F_PRODUCT_PACKAGE.ADMIN} 管理员," +
                    $"{DbTable.F_PRODUCT_PACKAGE.REMARK} 描述," +
                    $"{DbTable.F_PRODUCT_PACKAGE.BINDING_DATE} 绑定日期 " +
                    $"FROM " +
                    $"{DbTable.F_PRODUCT_PACKAGE_NAME} " +
                    $"WHERE " +
                    $"{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE} = '{casecode}'" +
                    $"AND " +
                    $"{DbTable.F_PRODUCT_PACKAGE.BINDING_STATE} = '{state}'";
            }
            else
            {
                selectSQL = $"SELECT {rowNumber}{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE} 箱子编码," +
                     $"{DbTable.F_PRODUCT_PACKAGE.SN_OUTTER} 产品SN," +
                     $"{DbTable.F_PRODUCT_PACKAGE.TYPE_NO} 产品型号," +
                     $"{DbTable.F_PRODUCT_PACKAGE.TEAM_LEADER} 班组长," +
                     $"{DbTable.F_PRODUCT_PACKAGE.ADMIN} 管理员," +
                     $"{DbTable.F_PRODUCT_PACKAGE.REMARK} 描述," +
                     $"{DbTable.F_PRODUCT_PACKAGE.BINDING_DATE} 绑定日期 " +
                     $"FROM " +
                     $"{DbTable.F_PRODUCT_PACKAGE_NAME} " +
                     $"WHERE " +
                     $"{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE} = '{casecode}' AND " +
                     $"{DbTable.F_PRODUCT_PACKAGE.SN_OUTTER} like '%{queryFilter}%' AND " +
                     $"{DbTable.F_PRODUCT_PACKAGE.BINDING_STATE} = '{state}'";
            }
            LogHelper.Log.Info(selectSQL);
            return SQLServer.ExecuteDataSet(selectSQL);
        }

        public DataSet SelectPackageProductCheck(string queryFilter, string state, bool IsShowNumber)
        {
            //箱子编码/追溯码查询/产品型号
            var rowNumber = "";
            if (IsShowNumber)
            {
                rowNumber = $"ROW_NUMBER() OVER(ORDER BY {DbTable.F_PRODUCT_PACKAGE.BINDING_DATE} DESC) 序号,";
            }
            string selectSQL = "";
            if (string.IsNullOrEmpty(queryFilter))
            {
                //查询所有已绑定记录
                selectSQL = $"SELECT {rowNumber}{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE} 箱子编码," +
                    $"{DbTable.F_PRODUCT_PACKAGE.SN_OUTTER} 产品SN," +
                    $"{DbTable.F_PRODUCT_PACKAGE.TYPE_NO} 产品型号," +
                    $"{DbTable.F_PRODUCT_PACKAGE.TEAM_LEADER} 班组长," +
                    $"{DbTable.F_PRODUCT_PACKAGE.ADMIN} 管理员," +
                    $"{DbTable.F_PRODUCT_PACKAGE.REMARK} 描述," +
                    $"{DbTable.F_PRODUCT_PACKAGE.BINDING_DATE} 绑定日期 " +
                    $"FROM " +
                    $"{DbTable.F_PRODUCT_PACKAGE_NAME} " +
                    $"WHERE " +
                    $"{DbTable.F_PRODUCT_PACKAGE.BINDING_STATE} = '{state}'";
            }
            else
            {
                selectSQL = $"SELECT {rowNumber}{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE} 箱子编码," +
                     $"{DbTable.F_PRODUCT_PACKAGE.SN_OUTTER} 产品SN," +
                     $"{DbTable.F_PRODUCT_PACKAGE.TYPE_NO} 产品型号," +
                     $"{DbTable.F_PRODUCT_PACKAGE.TEAM_LEADER} 班组长," +
                     $"{DbTable.F_PRODUCT_PACKAGE.ADMIN} 管理员," +
                     $"{DbTable.F_PRODUCT_PACKAGE.REMARK} 描述," +
                     $"{DbTable.F_PRODUCT_PACKAGE.BINDING_DATE} 绑定日期 " +
                     $"FROM " +
                     $"{DbTable.F_PRODUCT_PACKAGE_NAME} " +
                     $"WHERE " +
                     $"{DbTable.F_PRODUCT_PACKAGE.BINDING_STATE} = '{state}' AND " +
                     $"{DbTable.F_PRODUCT_PACKAGE.SN_OUTTER} like '%{queryFilter}%' OR " +
                     $"{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE} like '%{queryFilter}%' ";
            }
            LogHelper.Log.Info(selectSQL);
            return SQLServer.ExecuteDataSet(selectSQL);
        }

        public DataSet SelectPackageProductOfCaseCode(string queryFilter, string state, bool IsShowNumber)
        {
            //箱子编码/追溯码查询/产品型号
            var rowNumber = "";
            if (IsShowNumber)
            {
                rowNumber = $"ROW_NUMBER() OVER(ORDER BY {DbTable.F_PRODUCT_PACKAGE.BINDING_DATE} DESC) 序号,";
            }
            string selectSQL = $"SELECT {rowNumber}{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE} 箱子编码," +
                     $"{DbTable.F_PRODUCT_PACKAGE.SN_OUTTER} 产品SN," +
                     $"{DbTable.F_PRODUCT_PACKAGE.TYPE_NO} 产品型号," +
                     $"{DbTable.F_PRODUCT_PACKAGE.TEAM_LEADER} 班组长," +
                     $"{DbTable.F_PRODUCT_PACKAGE.ADMIN} 管理员," +
                     $"{DbTable.F_PRODUCT_PACKAGE.REMARK} 描述," +
                     $"{DbTable.F_PRODUCT_PACKAGE.BINDING_DATE} 绑定日期 " +
                     $"FROM " +
                     $"{DbTable.F_PRODUCT_PACKAGE_NAME} " +
                     $"WHERE " +
                     $"{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE} = '{queryFilter}' AND " +
                     $"{DbTable.F_PRODUCT_PACKAGE.BINDING_STATE} = '{state}'";
            LogHelper.Log.Info(selectSQL);
            return SQLServer.ExecuteDataSet(selectSQL);
        }
        public DataSet SelectPackageStorage(string queryFilter)
        {
            var selectSQL = "";
            if (queryFilter == "")
            {
                selectSQL = $"SELECT " +
                $"a.{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE} 箱子编码," +
                $"a.{DbTable.F_PRODUCT_PACKAGE.TYPE_NO} 产品型号," +
                $"b.{DbTable.F_PRODUCT_PACKAGE_STORAGE.STORAGE_CAPACITY} 箱子容量," +
                $"COUNT(a.{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE}) 产品实际数量 " +
                $"FROM " +
                $"{DbTable.F_PRODUCT_PACKAGE_NAME}  a," +
                $"{DbTable.F_PRODUCT_PACKAGE_STORAGE_NAME} b WHERE " +
                $"a.{DbTable.F_PRODUCT_PACKAGE.TYPE_NO} = b.{DbTable.F_PRODUCT_PACKAGE_STORAGE.PRODUCT_TYPE_NO} AND " +
                $"a.{DbTable.F_PRODUCT_PACKAGE.BINDING_STATE} = '1' " +
                $"GROUP BY " +
                $"{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE}," +
                $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.STORAGE_CAPACITY}," +
                $"a.{DbTable.F_PRODUCT_PACKAGE.TYPE_NO} ";
            }
            else
            {
                selectSQL = $"SELECT " +
                $"a.{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE} 箱子编码," +
                $"a.{DbTable.F_PRODUCT_PACKAGE.TYPE_NO} 产品型号," +
                $"b.{DbTable.F_PRODUCT_PACKAGE_STORAGE.STORAGE_CAPACITY} 箱子容量," +
                $"COUNT(a.{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE}) 产品实际数量 FROM " +
                $"{DbTable.F_PRODUCT_PACKAGE_NAME}  a," +
                $"{DbTable.F_PRODUCT_PACKAGE_STORAGE_NAME} b WHERE " +
                $"a.{DbTable.F_PRODUCT_PACKAGE.TYPE_NO} = b.{DbTable.F_PRODUCT_PACKAGE_STORAGE.PRODUCT_TYPE_NO} AND " +
                $"a.{DbTable.F_PRODUCT_PACKAGE.BINDING_STATE} = '1' AND " +
                $"a.{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE} = '{queryFilter}' OR " +
                $"a.{DbTable.F_PRODUCT_PACKAGE.TYPE_NO} = '{queryFilter}' AND " +
                $"b.{DbTable.F_PRODUCT_PACKAGE_STORAGE.PRODUCT_TYPE_NO} = '{queryFilter}' " +
                $"GROUP BY " +
                $"{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE}," +
                $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.STORAGE_CAPACITY}," +
                $"a.{DbTable.F_PRODUCT_PACKAGE.TYPE_NO} ";
            }
            LogHelper.Log.Info(selectSQL);
            return SQLServer.ExecuteDataSet(selectSQL);
        }
        #endregion

        #region 查询绑定状态
        public DataSet SelectProductBindingState(string sn)
        {
            //箱子编码/追溯码查询/产品型号
            string selectSQL = $"SELECT BINDING_STATE 绑定状态 " +
                    $"FROM {DbTable.F_PRODUCT_PACKAGE_NAME} WHERE " +
                    $"{DbTable.F_PRODUCT_PACKAGE.SN_OUTTER} = '{sn}'";
            return SQLServer.ExecuteDataSet(selectSQL);
        }
        #endregion

        #region 查询绑定数量
        /// <summary>
        /// 由箱子编码查询，该箱子已装数量；已绑定/绑定后解绑
        /// </summary>
        /// <param name="casecode"></param>
        /// <param name="bindingState"></param>
        /// <returns></returns>
        public DataSet SelectProductBindingRecord(string casecode,string bindingState)
        {
            string selectSQL = $"SELECT {DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE}," +
                $"{DbTable.F_PRODUCT_PACKAGE.SN_OUTTER} " +
                $"FROM {DbTable.F_PRODUCT_PACKAGE_NAME} " +
                $"WHERE {DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE} = '{casecode}' AND " +
                $"{DbTable.F_PRODUCT_PACKAGE.BINDING_STATE} = '{bindingState}'";
            return SQLServer.ExecuteDataSet(selectSQL);
        }
        #endregion

        #region 删除绑定记录
        /// <summary>
        /// 由箱子编码删除所有绑定记录
        /// </summary>
        /// <param name="casecode"></param>
        /// <returns></returns>
        public int DeleteProductBindingData(string casecode)
        {
            string deleteSQL = $"DELETE FROM {DbTable.F_PRODUCT_PACKAGE_NAME} WHERE " +
                $"{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE} = '{casecode}'";
            return SQLServer.ExecuteNonQuery(deleteSQL);
        }
        #endregion

        #region 产品/容器容量
        public int CommitProductContinairCapacity(string productTypeNo, string capacity,string username,string describle)
        {
            string insertSQL = $"INSERT INTO {DbTable.F_PRODUCT_PACKAGE_STORAGE_NAME}(" +
                $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.PRODUCT_TYPE_NO}," +
                $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.STORAGE_CAPACITY}," +
                $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.USER_NAME}," +
                $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.DESCRIBLE}) " +
                $"VALUES('{productTypeNo}','{capacity}','{username}','{describle}')";
            if (IsExistOutCaseBoxStorage(productTypeNo))
            {
                //update
                return UpdateProductContinairCapacity(productTypeNo, capacity, username,describle);
            }
            else
            {
                //insert
                LogHelper.Log.Info(insertSQL);
                return SQLServer.ExecuteNonQuery(insertSQL);
            }
        }
        public int UpdateProductContinairCapacity(string productTypeNo, string amount,string username,string describle)
        {
            string updateSQL = $"UPDATE {DbTable.F_PRODUCT_PACKAGE_STORAGE_NAME} SET " +
                $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.STORAGE_CAPACITY} = '{amount}'," +
                $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.USER_NAME} = '{username}'," +
                $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.DESCRIBLE} = '{describle}'" +
                $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.UPDATE_DATE_U} = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' " +
                $"WHERE {DbTable.F_PRODUCT_PACKAGE_STORAGE.PRODUCT_TYPE_NO} = '{productTypeNo}'";
            string selectSQL = $"SELECT * FROM {DbTable.F_PRODUCT_PACKAGE_STORAGE_NAME} WHERE " +
                $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.PRODUCT_TYPE_NO} = '{productTypeNo}' AND " +
                $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.STORAGE_CAPACITY} = '{amount}'";

            if (SQLServer.ExecuteDataSet(selectSQL).Tables[0].Rows.Count > 0)
            {
                return 1;
            }
            return SQLServer.ExecuteNonQuery(updateSQL); ;
        }
        public DataSet SelectProductContinairCapacity(string productTypeNo)
        {
            string selectSQL = "";
            if (string.IsNullOrEmpty(productTypeNo))
            {
                selectSQL = $"SELECT {DbTable.F_PRODUCT_PACKAGE_STORAGE.PRODUCT_TYPE_NO}," +
                    $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.STORAGE_CAPACITY}," +
                    $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.USER_NAME}," +
                    $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.UPDATE_DATE_U}," +
                    $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.DESCRIBLE} " +
                    $"FROM {DbTable.F_PRODUCT_PACKAGE_STORAGE_NAME}";
            }
            else
            {
                selectSQL = $"SELECT {DbTable.F_PRODUCT_PACKAGE_STORAGE.PRODUCT_TYPE_NO}," +
                    $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.STORAGE_CAPACITY}," +
                    $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.USER_NAME}," +
                    $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.UPDATE_DATE_U}," +
                    $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.DESCRIBLE} " +
                    $"FROM {DbTable.F_PRODUCT_PACKAGE_STORAGE_NAME} " +
                    $"WHERE {DbTable.F_PRODUCT_PACKAGE_STORAGE.PRODUCT_TYPE_NO} = '{productTypeNo}'";
            }
            return SQLServer.ExecuteDataSet(selectSQL);
        }

        public int DeleteProductContinairCapacity(string productTypeNo)
        {
            var deleteSQL = $"DELETE FROM {DbTable.F_PRODUCT_PACKAGE_STORAGE_NAME} WHERE " +
                $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.PRODUCT_TYPE_NO} = '{productTypeNo}'";
            return SQLServer.ExecuteNonQuery(deleteSQL);
        }

        public int DeleteAllProductContinairCapacity()
        {
            var deleteSQL = $"DELETE FROM {DbTable.F_PRODUCT_PACKAGE_STORAGE_NAME} ";
            return SQLServer.ExecuteNonQuery(deleteSQL);
        }
        private bool IsExistOutCaseBoxStorage(string productTypeNo)
        {
            string selectSQL = $"SELECT * FROM {DbTable.F_PRODUCT_PACKAGE_STORAGE_NAME} WHERE " +
                $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.PRODUCT_TYPE_NO} = '{productTypeNo}'";
            DataTable dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }
        #endregion

        #region 测试台数据
        public DataSet SelectTestLimitConfig(string productTypeNo)
        {
            var selectSQL = "";
            if (productTypeNo == "")
            {
                selectSQL = $"SELECT ROW_NUMBER() OVER(ORDER BY {DbTable.F_TEST_LIMIT_CONFIG.UPDATE_DATE} DESC) 序号," +
                    $"{DbTable.F_TEST_LIMIT_CONFIG.TYPE_NO} 产品型号," +
                    $"{DbTable.F_TEST_LIMIT_CONFIG.STATION_NAME} 工站名称," +
                    $"{DbTable.F_TEST_LIMIT_CONFIG.TEST_ITEM} 测试项," +
                    $"{DbTable.F_TEST_LIMIT_CONFIG.LIMIT} LIMIT值," +
                    $"{DbTable.F_TEST_LIMIT_CONFIG.TEAM_LEADER} 班组长," +
                    $"{DbTable.F_TEST_LIMIT_CONFIG.ADMIN} 管理员," +
                    $"{DbTable.F_TEST_LIMIT_CONFIG.UPDATE_DATE} 更新日期 FROM " +
                    $"{DbTable.F_TEST_LIMIT_CONFIG_NAME} ";
            }
            else
            {
                selectSQL = $"SELECT ROW_NUMBER() OVER(ORDER BY {DbTable.F_TEST_LIMIT_CONFIG.UPDATE_DATE}) 序号," +
                    $"{DbTable.F_TEST_LIMIT_CONFIG.TYPE_NO} 产品型号," +
                    $"{DbTable.F_TEST_LIMIT_CONFIG.STATION_NAME} 工站名称," +
                    $"{DbTable.F_TEST_LIMIT_CONFIG.TEST_ITEM} 测试项," +
                    $"{DbTable.F_TEST_LIMIT_CONFIG.LIMIT} LIMIT值," +
                    $"{DbTable.F_TEST_LIMIT_CONFIG.TEAM_LEADER} 班组长," +
                    $"{DbTable.F_TEST_LIMIT_CONFIG.ADMIN} 管理员," +
                    $"{DbTable.F_TEST_LIMIT_CONFIG.UPDATE_DATE} 更新日期 FROM " +
                    $"{DbTable.F_TEST_LIMIT_CONFIG_NAME} WHERE " +
                    $"{DbTable.F_TEST_LIMIT_CONFIG.TYPE_NO} = '{productTypeNo}' ";
            }
            
            return SQLServer.ExecuteDataSet(selectSQL);
        }

        public DataSet SelectTestProgrameVersion(string productTypeNo)
        {
            var selectSQL = "";
            if (productTypeNo == "")
            {
                selectSQL = $"SELECT ROW_NUMBER() OVER(ORDER BY {DbTable.F_TEST_PROGRAME_VERSION.UPDATE_DATE } DESC) 序号," +
                    $"{DbTable.F_TEST_PROGRAME_VERSION.TYPE_NO} 产品型号," +
                    $"{DbTable.F_TEST_PROGRAME_VERSION.STATION_NAME} 工站名称," +
                    $"{DbTable.F_TEST_PROGRAME_VERSION.PROGRAME_NAME} 程序路径," +
                    $"{DbTable.F_TEST_PROGRAME_VERSION.PROGRAME_VERSION} 程序名称," +
                    $"{DbTable.F_TEST_PROGRAME_VERSION.TEAM_LEADER} 班组长," +
                    $"{DbTable.F_TEST_PROGRAME_VERSION.ADMIN} 管理员," +
                    $"{DbTable.F_TEST_PROGRAME_VERSION.UPDATE_DATE} 更新日期 FROM " +
                    $"{DbTable.F_TEST_PROGRAME_VERSION_NAME} ";
            }
            else
            {
                selectSQL = $"SELECT ROW_NUMBER() OVER(ORDER BY {DbTable.F_TEST_PROGRAME_VERSION.UPDATE_DATE} DESC) 序号," +
                    $"{DbTable.F_TEST_PROGRAME_VERSION.TYPE_NO} 产品型号," +
                    $"{DbTable.F_TEST_PROGRAME_VERSION.STATION_NAME} 工站名称," +
                    $"{DbTable.F_TEST_PROGRAME_VERSION.PROGRAME_NAME} 程序路径," +
                    $"{DbTable.F_TEST_PROGRAME_VERSION.PROGRAME_VERSION} 程序名称," +
                    $"{DbTable.F_TEST_PROGRAME_VERSION.TEAM_LEADER} 班组长," +
                    $"{DbTable.F_TEST_PROGRAME_VERSION.ADMIN} 管理员," +
                    $"{DbTable.F_TEST_PROGRAME_VERSION.UPDATE_DATE} 更新日期 FROM " +
                    $"{DbTable.F_TEST_PROGRAME_VERSION_NAME} WHERE " +
                    $"{DbTable.F_TEST_PROGRAME_VERSION.TYPE_NO} = '{productTypeNo}' ";
            }
            
            return SQLServer.ExecuteDataSet(selectSQL);
        }

        public DataSet SelectTodayTestLogData(string queryFilter,string startTime,string endTime)
        {
            //string productSn,string productTypeNo,string stationName
            var selectSQL = "";
            if (string.IsNullOrEmpty(queryFilter))
            {
                selectSQL = $"SELECT DISTINCT " +
                    $"{DbTable.F_TEST_LOG_DATA.TYPE_NO} 产品型号," +
                    $"{DbTable.F_TEST_LOG_DATA.PRODUCT_SN} 产品SN," +
                    $"{DbTable.F_TEST_LOG_DATA.STATION_NAME} 工站名称 " +
                    $"FROM {DbTable.F_TEST_LOG_DATA_NAME} " +
                    $"WHERE " +
                    $"{DbTable.F_TEST_LOG_DATA.UPDATE_DATE} >= '{startTime}' AND " +
                    $"{DbTable.F_TEST_LOG_DATA.UPDATE_DATE} <= '{endTime}' ";
            }
            else
            {
                selectSQL = $"SELECT DISTINCT " +
                    $"{DbTable.F_TEST_LOG_DATA.TYPE_NO} 产品型号," +
                    $"{DbTable.F_TEST_LOG_DATA.PRODUCT_SN} 产品SN," +
                    $"{DbTable.F_TEST_LOG_DATA.STATION_NAME} 工站名称 " +
                    $"FROM {DbTable.F_TEST_LOG_DATA_NAME} " +
                    $"WHERE {DbTable.F_TEST_LOG_DATA.PRODUCT_SN} like '%{queryFilter}%' OR " +
                    $"{DbTable.F_TEST_LOG_DATA.TYPE_NO} like '%{queryFilter}%' OR " +
                    $"{DbTable.F_TEST_LOG_DATA.STATION_NAME} like '%{queryFilter}%' AND " +
                    $"{DbTable.F_TEST_LOG_DATA.UPDATE_DATE} >= '{startTime}' AND " +
                    $"{DbTable.F_TEST_LOG_DATA.UPDATE_DATE} <= '{endTime}' ";
            }
            LogHelper.Log.Info(selectSQL);
            return SQLServer.ExecuteDataSet(selectSQL);
        }

        public DataSet SelectTestLogDataDetail(string queryFilter,string startDate,string endDate)
        {
            var selectSQL = "";
            if (string.IsNullOrEmpty(queryFilter) || queryFilter == "")
                return null;
            if (!string.IsNullOrEmpty(startDate) || !string.IsNullOrEmpty(endDate))
            {
                selectSQL = $"SELECT ROW_NUMBER() OVER(ORDER BY {DbTable.F_TEST_LOG_DATA.UPDATE_DATE} DESC) 序号," +
                    $"{DbTable.F_TEST_LOG_DATA.TYPE_NO} 产品型号," +
                    $"{DbTable.F_TEST_LOG_DATA.PRODUCT_SN} 产品SN," +
                    $"{DbTable.F_TEST_LOG_DATA.STATION_NAME} 工站名称," +
                    $"{DbTable.F_TEST_LOG_DATA.TEST_ITEM} 测试项," +
                    $"{DbTable.F_TEST_LOG_DATA.TEST_RESULT} 测试结果," +
                    $"{DbTable.F_TEST_LOG_DATA.LIMIT} LIMIT," +
                    $"{DbTable.F_TEST_LOG_DATA.CURRENT_VALUE} 当前值," +
                    $"{DbTable.F_TEST_LOG_DATA.TEAM_LEADER} 班组长," +
                    $"{DbTable.F_TEST_LOG_DATA.ADMIN} 管理员," +
                    $"{DbTable.F_TEST_LOG_DATA.UPDATE_DATE} 更新日期 " +
                    $"FROM {DbTable.F_TEST_LOG_DATA_NAME} " +
                    $"WHERE " +
                    $"{DbTable.F_TEST_LOG_DATA.PRODUCT_SN} = '{queryFilter}' AND " +
                    $"{DbTable.F_TEST_LOG_DATA.UPDATE_DATE} >= '{startDate}' AND " +
                    $"{DbTable.F_TEST_LOG_DATA.UPDATE_DATE} <= '{endDate}' " +
                    $"ORDER BY {DbTable.F_TEST_LOG_DATA.UPDATE_DATE} ASC";
            }
            else
            {
                selectSQL = $"SELECT ROW_NUMBER() OVER(ORDER BY {DbTable.F_TEST_LOG_DATA.UPDATE_DATE} DESC) 序号," +
                    $"{DbTable.F_TEST_LOG_DATA.TYPE_NO} 产品型号," +
                    $"{DbTable.F_TEST_LOG_DATA.PRODUCT_SN} 产品SN," +
                    $"{DbTable.F_TEST_LOG_DATA.STATION_NAME} 工站名称," +
                    $"{DbTable.F_TEST_LOG_DATA.TEST_ITEM} 测试项," +
                    $"{DbTable.F_TEST_LOG_DATA.TEST_RESULT} 测试结果," +
                    $"{DbTable.F_TEST_LOG_DATA.LIMIT} LIMIT," +
                    $"{DbTable.F_TEST_LOG_DATA.CURRENT_VALUE} 当前值," +
                    $"{DbTable.F_TEST_LOG_DATA.TEAM_LEADER} 班组长," +
                    $"{DbTable.F_TEST_LOG_DATA.ADMIN} 管理员," +
                    $"{DbTable.F_TEST_LOG_DATA.UPDATE_DATE} 更新日期 " +
                    $"FROM {DbTable.F_TEST_LOG_DATA_NAME} " +
                    $"WHERE " +
                    $"{DbTable.F_TEST_LOG_DATA.PRODUCT_SN} = '{queryFilter}' " +
                    $"ORDER BY {DbTable.F_TEST_LOG_DATA.UPDATE_DATE} ASC";
            }
            LogHelper.Log.Info(selectSQL);
            return SQLServer.ExecuteDataSet(selectSQL);
        }

        public string SelectLastLogTestResult(string productSN)
        {
            var selectSQL = $"SELECT {DbTable.F_TEST_LOG_DATA.TEST_RESULT} FROM {DbTable.F_TEST_LOG_DATA_NAME} WHERE " +
                $"{DbTable.F_TEST_LOG_DATA.PRODUCT_SN} = '{productSN}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();
            return "";
        }
        #endregion

        #region 品质异常管理
        public int UpdateQuanlityData(string eType,string mCode,string sDate,string estock,string aStock,string station,
            string state,string reason,string user)
        {
            var insertSQL = $"INSERT INTO {DbTable.F_QUANLITY_MANAGER_NAME}(" +
                $"{DbTable.F_QUANLITY_MANAGER.EXCEPT_TYPE}," +
                $"{DbTable.F_QUANLITY_MANAGER.MATERIAL_CODE}," +
                $"{DbTable.F_QUANLITY_MANAGER.STATEMENT_DATE}," +
                $"{DbTable.F_QUANLITY_MANAGER.EXCEPT_STOCK}," +
                $"{DbTable.F_QUANLITY_MANAGER.ACTUAL_STOCK}," +
                $"{DbTable.F_QUANLITY_MANAGER.STATION_NAME}," +
                $"{DbTable.F_QUANLITY_MANAGER.MATERIAL_STATE}," +
                $"{DbTable.F_QUANLITY_MANAGER.STATEMENT_REASON}," +
                $"{DbTable.F_QUANLITY_MANAGER.STATEMENT_USER}," +
                $"{DbTable.F_QUANLITY_MANAGER.UPDATE_DATE}) VALUES(" +
                $"'{eType}','{mCode}','{sDate}','{estock}','{aStock}','{station}','{state}','{reason}','{user}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
            LogHelper.Log.Info(insertSQL);
            return SQLServer.ExecuteNonQuery(insertSQL); 
        }

        public int UpdateMaterialStateMent(string materialCode,int state)
        {
            var updateSQL = $"UPDATE {DbTable.F_MATERIAL_NAME} SET " +
                $"{DbTable.F_Material.MATERIAL_STATE} = '{state}' WHERE " +
                $"{DbTable.F_Material.MATERIAL_CODE} = '{materialCode}'";
            return SQLServer.ExecuteNonQuery(updateSQL);
        }

        public DataSet SelectQuanlityManager(string materialCode)
        {
            var selectSQL = "";
            if (materialCode == "")
            {
                selectSQL = $"SELECT {DbTable.F_QUANLITY_MANAGER.MATERIAL_CODE}," +
                            $"{DbTable.F_QUANLITY_MANAGER.EXCEPT_TYPE}," +
                            $"{DbTable.F_QUANLITY_MANAGER.EXCEPT_STOCK}," +
                            $"{DbTable.F_QUANLITY_MANAGER.ACTUAL_STOCK}," +
                            $"{DbTable.F_QUANLITY_MANAGER.MATERIAL_STATE}," +
                            $"{DbTable.F_QUANLITY_MANAGER.STATEMENT_REASON}," +
                            $"{DbTable.F_QUANLITY_MANAGER.STATEMENT_USER}," +
                            $"{DbTable.F_QUANLITY_MANAGER.UPDATE_DATE} " +
                            $"FROM {DbTable.F_QUANLITY_MANAGER_NAME} " +
                            $"ORDER BY {DbTable.F_QUANLITY_MANAGER.UPDATE_DATE} DESC";
            }
            else
            {
                selectSQL = $"SELECT {DbTable.F_QUANLITY_MANAGER.MATERIAL_CODE}," +
                            $"{DbTable.F_QUANLITY_MANAGER.EXCEPT_TYPE}," +
                            $"{DbTable.F_QUANLITY_MANAGER.EXCEPT_STOCK}," +
                            $"{DbTable.F_QUANLITY_MANAGER.ACTUAL_STOCK}," +
                            $"{DbTable.F_QUANLITY_MANAGER.MATERIAL_STATE}," +
                            $"{DbTable.F_QUANLITY_MANAGER.STATEMENT_REASON}," +
                            $"{DbTable.F_QUANLITY_MANAGER.STATEMENT_USER}," +
                            $"{DbTable.F_QUANLITY_MANAGER.UPDATE_DATE} " +
                            $"FROM {DbTable.F_QUANLITY_MANAGER_NAME} WHERE " +
                            $"{DbTable.F_QUANLITY_MANAGER.MATERIAL_CODE} like '%{materialCode}%'" +
                            $"ORDER BY {DbTable.F_QUANLITY_MANAGER.UPDATE_DATE} DESC";
            }

            return SQLServer.ExecuteDataSet(selectSQL);
        }
        #endregion

        #region 物料库存
        public MaterialStockEnum ModifyMaterialStock(string materialCode,int stock,string admin)
        {
            var selectSQL = $"SELECT * FROM {DbTable.F_MATERIAL_NAME} " +
                $"WHERE " +
                $"{DbTable.F_Material.MATERIAL_CODE} = '{materialCode}' ";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                //物料存在
                selectSQL = $"SELECT * FROM {DbTable.F_MATERIAL_NAME} " +
                $"WHERE " +
                $"{DbTable.F_Material.MATERIAL_CODE} = '{materialCode}' " +
                $"AND " +
                $"{DbTable.F_Material.MATERIAL_STOCK} = '{stock}'";
                dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                if (dt.Rows.Count < 1)
                {
                    //该库存为修改库存
                    //更新
                    var updateSQL = $"UPDATE {DbTable.F_MATERIAL_NAME} SET " +
                        $"{DbTable.F_Material.MATERIAL_STOCK} = '{stock}'," +
                        $"{DbTable.F_Material.MATERIAL_USERNAME} = '{admin}'," +
                        $"{DbTable.F_Material.MATERIAL_UPDATE_DATE} = '{GetDateTimeNow()}'" +
                        $"WHERE " +
                        $"{DbTable.F_Material.MATERIAL_CODE} = '{materialCode}'";
                    var res = SQLServer.ExecuteNonQuery(updateSQL);
                    if (res == 1)
                    {
                        //更新库存后，更新物料状态
                        var count = UpdateMaterialState(materialCode);
                        if (count == 1)
                        {
                            LogHelper.Log.Info("【修改库存-更新物料状态-成功】");
                        }
                        else if (count == 0)
                        {
                            LogHelper.Log.Info("【修改库存-更新物料状态-失败】");
                        }
                        return MaterialStockEnum.STATUS_SUCCESS;
                    }
                    return MaterialStockEnum.STATUS_FAIL;
                }
                //库存未修改
                return MaterialStockEnum.STATUS_NONE_MODIFY;
            }
            return MaterialStockEnum.ERROR_MATERIAL_IS_NOT_EXIST;
        }

        private int GetPutInStock(string materialCode)
        {
            var selectSQL = $"SELECT {DbTable.F_Material.MATERIAL_STOCK} FROM {DbTable.F_MATERIAL_NAME} " +
                $"WHERE {DbTable.F_Material.MATERIAL_CODE} = '{materialCode}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            int stock = 0;
            if (dt.Rows.Count > 0)
            {
                int.TryParse(dt.Rows[0][0].ToString(),out stock);
            }
            return stock;
        }

        private static int UpdateMaterialState(string materialCode)
        {
            var selectSQL = $"SELECT {DbTable.F_Material.MATERIAL_STOCK}," +
                $"{DbTable.F_Material.MATERIAL_AMOUNTED} " +
                $"FROM {DbTable.F_MATERIAL_NAME} " +
                $"WHERE " +
                $"{DbTable.F_Material.MATERIAL_CODE} = '{materialCode}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                var stock = int.Parse(dt.Rows[0][0].ToString());
                var amounted = int.Parse(dt.Rows[0][1].ToString());
                if (stock <= amounted)
                {
                    //入库库存大于实际库存
                    //当发现盒子没有物料时，去修改入库库存为实际库存
                    //同时更新物料状态为2
                    //物料已使用完，更新状态为2
                    var updateSQL = $"UPDATE {DbTable.F_MATERIAL_NAME} SET " +
                        $"{DbTable.F_Material.MATERIAL_STATE} = '2' WHERE " +
                        $"{DbTable.F_Material.MATERIAL_CODE} = '{materialCode}'";
                    return SQLServer.ExecuteNonQuery(updateSQL);
                }
                else
                {
                    //入库库存小于实际库存
                    //当系统提示物料用完（状态值已为2），扫描下一箱时，发现实际还有数量，去修改入库库存为实际库存
                    //同时更新物料状态正常-1
                    var updateSQL = $"UPDATE {DbTable.F_MATERIAL_NAME} SET " +
                       $"{DbTable.F_Material.MATERIAL_STATE} = '1' WHERE " +
                       $"{DbTable.F_Material.MATERIAL_CODE} = '{materialCode}'";
                    return SQLServer.ExecuteNonQuery(updateSQL);
                }
            }
            return 0;
        }
        #endregion

        public DataSet SelectTypeNoList()
        {
            var selectSQL = $"SELECT {DbTable.F_PRODUCT_PACKAGE_STORAGE.PRODUCT_TYPE_NO} " +
                $"FROM " +
                $"{DbTable.F_PRODUCT_PACKAGE_STORAGE_NAME} ";
            return SQLServer.ExecuteDataSet(selectSQL);
        }
        public string GetMaterialPN(string materialCode)
        {
            //A19083100008&S2.118&1.2.11.111&20&20190831&1T20190831001
            //RID & &PN & QTY$DC & LOT
            materialCode = materialCode.Substring(materialCode.IndexOf('&') + 1);
            materialCode = materialCode.Substring(materialCode.IndexOf('&') + 1);
            materialCode = materialCode.Substring(0, materialCode.IndexOf('&'));
            return materialCode;
        }

        public string GetMaterialCode(string materialRID)
        {
            var selectSQL = $"SELECT {DbTable.F_Material.MATERIAL_CODE} FROM {DbTable.F_MATERIAL_NAME} WHERE " +
                $"{DbTable.F_Material.MATERIAL_CODE} like '%{materialRID}%'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();
            return "";
        }
    }
}
