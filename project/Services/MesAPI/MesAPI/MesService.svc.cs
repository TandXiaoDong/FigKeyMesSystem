﻿using SwaggerWcf.Attributes;
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
using System.Data.Common;
using CommonUtils.DEncrypt;
using CommonUtils.FileHelper;

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
        private static string STATION_TURN = "烧录工站";
        private static string STATION_SENSIBLITY = "灵敏度测试工站";
        private static string STATION_SHELL = "外壳装配工站";
        private static string STATION_AIR = "气密测试工站";
        private static string STATION_STENT = "支架装配工站";
        private static string STATION_PRODUCT = "成品测试工站";

        private static List<string> pcbaCacheList = new List<string>();//用于缓存pcba数据
        private static DataTable pcbaCacheDataSource = new DataTable();//用于缓存PCBA的所有数据信息

        private int logCount = 0;

        #region 物料统计字段
        private const string DATA_ORDER = "序号";
        private const string MATERIAL_PN = "物料号";
        private const string MATERIAL_LOT = "批次号";
        private const string MATERIAL_RID = "料盘号";
        private const string MATERIAL_DC = "收料日期";
        private const string MATERIAL_QTY = "入库库存";
        private const string MATERIAL_NAME = "物料名称";
        private const string PRODUCT_TYPENO = "产品型号";
        private const string SN_PCBA = "PCBA";
        private const string SN_OUTTER = "外壳";
        private const string STATION_NAME = "工站名称";
        private const string USE_AMOUNTED = "当前使用数量";
        private const string RESIDUE_STOCK = "入库剩余库存";
        private const string CURRENT_RESIDUE_STOCK = "当前剩余库存";
        private const string TEAM_LEADER = "班组长";
        private const string ADMIN = "管理员";
        private const string UPDATE_DATE = "更新日期";
        private const string PCBA_STATUS = "PCBA绑定状态";

        private const string EXCEPT_TYPE = "异常类型";
        private const string EXCEPT_STOCK = "异常数量";
        private const string ACTUAL_STOCK = "实际库存";
        private const string MATERIAL_STATE = "物料状态";
        private const string SHUT_REASON = "结单原因";
        private const string USER_NAME = "结单用户";
        private const string STATEMENT_DATE = "结单日期";
        #endregion

        #region 产品打包
        public const string OUT_CASE_CODE = "箱子编码";
        public const string CASE_PRODUCT_TYPE_NO = "产品型号";
        public const string CASE_STORAGE_CAPACITY = "箱子容量";
        public const string CASE_AMOUNTED = "产品实际数据";
        #endregion

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
            var selectSQL = $"SELECT {DbTable.F_User.ROLE_NAME},{DbTable.F_User.PASS_WORD},{DbTable.F_User.USER_ID} " +
                $" FROM {DbTable.F_USER_NAME} " +
                     $"WHERE {DbTable.F_User.USER_NAME} = '{username}'";
            return SQLServer.ExecuteDataSet(selectSQL);
        }

        public string GetUserID(string username)
        {
            var selectSQL = $"SELECT {DbTable.F_User.USER_ID} " +
                $" FROM {DbTable.F_USER_NAME} " +
                     $"WHERE {DbTable.F_User.USER_NAME} = '{username}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();
            return "";
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
            string sqlString = $"SELECT " +
                    $"{DbTable.F_User.USER_NAME}," +
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
        public RegisterResult Register(string username, string pwd, int userType)
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
                    var userID = MD5OTool.Encrypt(username+pwd);
                    string dateTimeNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string insertString = $"INSERT INTO {DbTable.F_USER_NAME}" +
                        $"({DbTable.F_User.USER_NAME}," +
                        $"{DbTable.F_User.PASS_WORD} ," +
                        $"{DbTable.F_User.UPDATE_DATE} ," +
                        $"{DbTable.F_User.ROLE_NAME}," +
                        $"{DbTable.F_User.USER_ID}) " +
                        $"VALUES('{username}', '{pwd}', '{dateTimeNow}','{userType}','{userID}')";
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
        public int ModifyUserPassword(string userID,string username, string pwd)
        {
            var updateSQL = $"UPDATE {DbTable.F_USER_NAME} SET " +
                $"{DbTable.F_User.PASS_WORD} = '{pwd}'," +
                $"{DbTable.F_User.USER_NAME} = '{username}'," +
                $"{DbTable.F_User.UPDATE_DATE} = '{GetDateTimeNow()}' " +
                $"WHERE {DbTable.F_User.USER_ID} = '{userID}'";
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
        public List<Station> InsertStation(List<Station> stationList)
        {
            List<Station> updateResult = new List<Station>();
            foreach (var station in stationList)
            {
                Station stationResult = new Station();
                if (!IsExistStation(station))
                {
                    //不存在，插入
                    string insertSQL = $"INSERT INTO {DbTable.F_TECHNOLOGICAL_PROCESS_NAME}(" +
                        $"{DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME}," +
                        $"{DbTable.F_TECHNOLOGICAL_PROCESS.STATION_ORDER}," +
                        $"{DbTable.F_TECHNOLOGICAL_PROCESS.STATION_NAME}," +
                        $"{DbTable.F_TECHNOLOGICAL_PROCESS.USER_NAME}," +
                        $"{DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_STATE}," +
                        $"{DbTable.F_TECHNOLOGICAL_PROCESS.UPDATE_DATE}) " +
                    $"VALUES('{station.ProcessName}','{station.StationID}','{station.StationName}','{station.UserName}','{station.ProcessState}','{station.UpdateDate}')";
                    LogHelper.Log.Info("【工艺更新】插入新数据"+insertSQL);
                    stationResult.Result = SQLServer.ExecuteNonQuery(insertSQL);
                }
                else
                {
                    LogHelper.Log.Info("【工艺更新】更新数据");
                    //stationResult.Result = UpdateProcessOrder(station.ProcessName,station.StationName,int.Parse(station.StationID),station.UserName);
                }

                stationResult.StationID = station.StationID;
                stationResult.StationName = station.StationName;
                updateResult.Add(stationResult);
            }
            return updateResult;
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

        public DataSet SelectCirticalStationList(string processName,int cirticalID,bool IsBefore)
        {
            var ds = SelectStationList(processName);
            if (ds.Tables.Count > 0)
            {
                var dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    for (int i = dt.Rows.Count - 1;i >= 0; i--)
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
                            if (i < cirticalID - 1 )
                            {
                                dt.Rows.RemoveAt(i);
                            }
                        }
                    }
                }
            }
            return ds;
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
            LogHelper.Log.Info("【删除工艺-工站】"+deleteSQL);
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
            LogHelper.Log.Info("【删除工艺数据】"+deleteSQL);
            return SQLServer.ExecuteNonQuery(deleteSQL);
        }

        /// <summary>
        /// 产线序号是否为空
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool IsExistStation(Station station)
        {
            string selectSQL = $"SELECT * FROM {DbTable.F_TECHNOLOGICAL_PROCESS_NAME} " +
                $"WHERE " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME} = '{station.ProcessName}' AND " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.STATION_NAME} = '{station.StationName}' ";
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

        public int UpdateProcessOrder(string process,string station,int id,string user)
        {
            var updateSQL = $"UPDATE {DbTable.F_TECHNOLOGICAL_PROCESS_NAME} SET " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.STATION_ORDER} = '{id}'," +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.USER_NAME} = '{user}'" +
                $"WHERE " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME} = '{process}' " +
                $"AND " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.STATION_NAME} = '{station}' ";
            LogHelper.Log.Info("【更新工艺序号】"+updateSQL);
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
        public List<string> SelectUseAllPcbaSN()
        {
            //List<string> pcbaList = new List<string>();
            var selectSQL = $"select {DbTable.F_Test_Result.SN} from {DbTable.F_TEST_RESULT_NAME} " +
                $"order by {DbTable.F_Test_Result.STATION_IN_DATE} desc";
            try
            {
                var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    //去重
                    DataView dv = new DataView(dt);
                    dt = dv.ToTable(true,"sn");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var pcbaSN = dt.Rows[i][0].ToString();                                                                                                                      
                        if (!IsProductSN(pcbaSN))
                        {
                            //是PCBA
                            //pcbaList.Add(pcbaSN);
                            if (!pcbaCacheList.Contains(pcbaSN))
                                pcbaCacheList.Add(pcbaSN);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error(ex.Message);
            }
            return pcbaCacheList;
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

        private int ReadShellCodeLength()
        {
            try
            {
                int shellLen = 0;
                var defaultRoot = ConfigurationManager.AppSettings["shellCodeRoot"].ToString();
                var process = SelectCurrentTProcess();
                string configPath = defaultRoot + ":\\StationConfig\\外壳装配工站\\" + process + "\\" + "外壳装配工站_" + process + "_config.ini";
                int.TryParse(INIFile.GetValue(process, "设置外壳条码长度位数", configPath).Trim(), out shellLen);
                //LogHelper.Log.Info("【配置文件路径】" + configPath + "len="+shellLen);
                return shellLen;
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error("读取配置长度错误！"+ex.Message+ex.StackTrace+"\r\n");
                return 0;
            }
        }

        /// <summary>
        /// 一次性按条件查询所有
        /// </summary>
        /// <param name="querySN"></param>
        /// <returns></returns>
        public DataSet SelectTestResultDetail(string querySN)
        {
            LogHelper.Log.Info("开始查询");
            DataTable dt = InitTestResultDataTable(true);
            DataSet dataSet = new DataSet();
            List<string> pcbaList = new List<string>();
            if (querySN != "" && querySN != null)
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
                var shellLen = ReadShellCodeLength();
                LogHelper.Log.Info("开始添加数据");
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
                    dr[TestResultItemContent.FinalResultValue] = GetProductTestFinalResult(pcbsn, productsn, shellLen);
                    var currentProductType = GetProductTypeNoOfSN(pcbsn, productsn);
                    if (currentProductType == "")
                        continue;//当前SN不存在
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

                    dt.Rows.Add(dr);
                    count++;
                }
            }
            dataSet.Tables.Add(dt);
            LogHelper.Log.Info("查询结束");
            return dataSet;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="querySN"></param>
        /// <param name="startIndex"></param>
        /// <param name="dCount"></param>
        /// <param name="IsQueryLatest"></param>
        /// <returns></returns>
        public DataSet SelectTestResultDetail(string querySN,int pageNumber,int pageSize,bool IsQueryLatest)
        {
            /*
             *分页查询，根据PCBA索引查询数据
             * 
             */
            LogHelper.Log.Info("开始查询");
            DataTable dt = InitTestResultDataTable(true);
            DataSet dataSet = new DataSet();
            try
            {
                int startIndex = (pageNumber - 1) * pageSize;
                string[] pcbaArray = new string[pageSize];
                if (querySN != "" && querySN != null)
                {
                    pcbaArray = new string[1];
                    pcbaArray[0] = querySN.Trim();
                }
                else
                {
                    if (IsQueryLatest)
                    {
                        pcbaCacheList = SelectUseAllPcbaSN();//更新PCBA数据
                        if (pageNumber * pageSize > pcbaCacheList.Count)
                        {
                            pageSize = pcbaCacheList.Count - ((pageNumber - 1) * pageSize);
                            pcbaArray = new string[pageSize];
                            pcbaCacheList.CopyTo(startIndex, pcbaArray, 0, pageSize);
                        }
                        else
                        {
                            pcbaCacheList.CopyTo(startIndex, pcbaArray, 0, pageSize);
                        }
                    }
                    else
                    {
                        //pcbaCacheList.CopyTo(startIndex, pcbaArray, startIndex, dCount);
                        int index = 0;
                        try
                        {
                            LogHelper.Log.Info("pcbaCacheDataSource " + pcbaCacheDataSource.Rows.Count);
                            foreach (DataRow dr in pcbaCacheDataSource.Rows)
                            {
                                if (index >= startIndex && index < pageSize)
                                {
                                    dt.Rows[index][0] = dr[0].ToString();
                                    dt.Rows[index][1] = dr[1].ToString();
                                }
                                index++;
                            }
                            dataSet.Tables.Add(dt);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Log.Error(ex.Message + ex.StackTrace);
                        }
                        return dataSet;
                    }
                }
                //List<TestReulstDetail> testReulstDetailsList = new List<TestReulstDetail>();
                //List<TestResultBasic> testResultBasicsList = SelectTestResultBasic();
                int count = 1;
                if (pcbaArray.Length > 0)
                {
                    var shellLen = ReadShellCodeLength();
                    LogHelper.Log.Info("开始添加数据");
                    foreach (var pcbaSN in pcbaArray)
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
                        dr[TestResultItemContent.FinalResultValue] = GetProductTestFinalResult(pcbsn, productsn, shellLen);
                        var currentProductType = GetProductTypeNoOfSN(pcbsn, productsn);
                        if (currentProductType == "")
                            continue;//当前SN不存在
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

                        dt.Rows.Add(dr);
                        count++;
                    }
                }
                pcbaCacheDataSource = dt.Copy();
                dataSet.Tables.Add(dt);
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error(ex.Message + ex.StackTrace);
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
            dt.Columns.Add(STATION_PRODUCT + TestResultItemContent.Product_Inspect_Result);

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

        public string GetProductSn(string sn)
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

        public string GetProductTestFinalResult(string pcbsn,string productsn,int shellLen)
        {
            bool IsFinalResultPass = true;
            var productTypeNo = GetProductTypeNoOfSN(pcbsn,productsn);
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
                    LogHelper.Log.Info("【只计算外壳装配之后的工站】"+pcbsn);
                    stationList = SelectCirticalStationList(productTypeNo, cirticalID, false).Tables[0];
                }
                else if (pcbsn.Length < shellLen && productsn == "")
                {
                    //只计算外壳装配之前的工站
                    LogHelper.Log.Info("【只计算外壳装配之前的工站】" + pcbsn);
                    stationList = SelectCirticalStationList(productTypeNo,cirticalID, true).Tables[0];
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
        private string GetProductTypeNoOfSN(string pcbsn,string productSn)
        {
            var selectSQL = $"SELECT TOP 1 {DbTable.F_Test_Result.TYPE_NO} FROM " +
                $"{DbTable.F_TEST_RESULT_NAME} WHERE {DbTable.F_Test_Result.SN} like '%{pcbsn}%' " +
                $"ORDER BY {DbTable.F_Test_Result.UPDATE_DATE} DESC";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();
            else
            {
                selectSQL = $"SELECT TOP 1 {DbTable.F_Test_Result.TYPE_NO} FROM " +
                $"{DbTable.F_TEST_RESULT_NAME} WHERE {DbTable.F_Test_Result.SN} like '%{productSn}%' " +
                $"ORDER BY {DbTable.F_Test_Result.UPDATE_DATE} DESC";
                if (dt.Rows.Count > 0)
                    return dt.Rows[0][0].ToString();
            }
            return "";
        }

        public DataSet SelectTestResultLogDetail(string queryFilter,string startTime,string endTime)
        {
            //更新当前工站名称
            DataTable dtTestResult = new DataTable();//要查询的所有SN主要信息
            DataSet ds = new DataSet();
            try
            {
                var dt = InitTestResultDataTable(true);//最终结果
                if (STATION_TURN.Contains(queryFilter) && queryFilter != "")
                {
                    AddTestLogDetail(STATION_TURN, queryFilter, startTime, endTime, dt, dtTestResult, true);
                }
                else if (STATION_SENSIBLITY.Contains(queryFilter) && queryFilter != "")
                {
                    AddTestLogDetail(STATION_SENSIBLITY, queryFilter, startTime, endTime, dt, dtTestResult, true);
                }
                else if (STATION_SHELL.Contains(queryFilter) && queryFilter != "")
                {
                    AddTestLogDetail(STATION_SHELL, queryFilter, startTime, endTime, dt, dtTestResult, true);
                }
                else if (STATION_AIR.Contains(queryFilter) && queryFilter != "")
                {
                    AddTestLogDetail(STATION_AIR, queryFilter, startTime, endTime, dt, dtTestResult, true);
                }
                else if (STATION_STENT.Contains(queryFilter) && queryFilter != "")
                {
                    AddTestLogDetail(STATION_STENT, queryFilter, startTime, endTime, dt, dtTestResult, true);
                }
                else if (STATION_PRODUCT.Contains(queryFilter) && queryFilter != "")
                {
                    AddTestLogDetail(STATION_PRODUCT, queryFilter, startTime, endTime, dt, dtTestResult, true);
                }
                else
                {
                    AddTestLogDetail(STATION_TURN, queryFilter, startTime, endTime, dt, dtTestResult, false);
                    AddTestLogDetail(STATION_SENSIBLITY, queryFilter, startTime, endTime, dt, dtTestResult, false);
                    AddTestLogDetail(STATION_SHELL, queryFilter, startTime, endTime, dt, dtTestResult, false);
                    AddTestLogDetail(STATION_AIR, queryFilter, startTime, endTime, dt, dtTestResult, false);
                    AddTestLogDetail(STATION_STENT, queryFilter, startTime, endTime, dt, dtTestResult, false);
                    AddTestLogDetail(STATION_PRODUCT, queryFilter, startTime, endTime, dt, dtTestResult, false);
                }
                //LogHelper.Log.Info("【查询过站记录完成--开始查询log明细】"+dtTestResult.Rows.Count);
                //开始分页查询
                StartSelectLogDetail(dt,dtTestResult);
                //LogHelper.Log.Info("【查询过站记录完成--查询log明细完毕】" + dtTestResult.Rows.Count + "  " + dt.Rows.Count);
                ds.Tables.Add(dt);
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error(ex.Message+ex.StackTrace);
            }
            return ds;
        }

        private void AddTestLogDetail(string station,string queryFilter, string startTime, string endTime,DataTable dt,DataTable dtTestResult, bool IsQueryOfStation)
        {
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
                            $"AND {DbTable.F_Test_Result.STATION_NAME} = '{station}'" +
                            $"ORDER BY " +
                            $"{DbTable.F_Test_Result.UPDATE_DATE} " +
                            $"DESC";
            }
            else
            {
                //按工站查询
                if (IsQueryOfStation)
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
                            $"AND " +
                            $"{DbTable.F_Test_Result.STATION_NAME} like '%{queryFilter}%' ";
                    //dtResult = SQLServer.ExecuteDataSet(selectTestResultSQL).Tables[0];
                }
                else
                {
                    //按SN查询
                    var pcbaSN = GetPCBASn(queryFilter);
                    var productSN = GetProductSn(queryFilter);
                    if (pcbaSN != "" && productSN != "")
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
                            $"AND " +
                            $"({DbTable.F_Test_Result.STATION_NAME} = '{station}' AND {DbTable.F_Test_Result.SN} = '{pcbaSN}') " +
                            $"OR " +
                            $"({DbTable.F_Test_Result.STATION_NAME} = '{station}' AND {DbTable.F_Test_Result.SN} = '{productSN}')";
                    }
                    else if (pcbaSN != "" && productSN == "")
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
                                $"{DbTable.F_Test_Result.STATION_NAME} = '{station}' AND " +
                                $"{DbTable.F_Test_Result.UPDATE_DATE} >= '{startTime}' " +
                                $"AND " +
                                $"{DbTable.F_Test_Result.UPDATE_DATE} <= '{endTime}' " +
                                $"AND " +
                                $"{DbTable.F_Test_Result.SN} = '{pcbaSN}' ";
                    }
                    else if (pcbaSN == "" && productSN != "")
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
                                $"{DbTable.F_Test_Result.STATION_NAME} = '{station}' AND " +
                                $"{DbTable.F_Test_Result.UPDATE_DATE} >= '{startTime}' " +
                                $"AND " +
                                $"{DbTable.F_Test_Result.UPDATE_DATE} <= '{endTime}' " +
                                $"AND " +
                                $"{DbTable.F_Test_Result.SN} = '{productSN}' ";
                    }
                    else
                    {
                        //都为空
                        //return new DataTable();//查询条件不存在
                    }
                }
            }
            var dtResult = SQLServer.ExecuteDataSet(selectTestResultSQL).Tables[0];
            foreach (DataRow dr in dtResult.Rows)
            {
                dtTestResult.ImportRow(dr);
            }
        }

        private void StartSelectLogDetail(DataTable dt, DataTable dtTestResult)
        {
            if (dtTestResult.Rows.Count > 0)
            {
                var shellLen = ReadShellCodeLength();
                foreach (DataRow dataRow in dtTestResult.Rows)
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
                    var pcbsn = GetPCBASn(pcbaSN);
                    var productsn = GetProductSn(pcbaSN);
                    if (pcbsn.Length == shellLen && productsn == "")
                    {
                        //只有外壳
                        LogHelper.Log.Info($"【只有外壳】配置外壳长度={shellLen} 实际外壳长度={pcbsn}");
                        dr[TestResultItemContent.PcbaSN] = productsn;
                        dr[TestResultItemContent.ProductSN] = pcbsn;
                    }
                    else
                    {
                        dr[TestResultItemContent.PcbaSN] = pcbsn;
                        dr[TestResultItemContent.ProductSN] = productsn;
                    }
                    dr[TestResultItemContent.Order] = logCount + 1;
                    dr[TestResultItemContent.ProductTypeNo] = productTypeNo;
                    dr[TestResultItemContent.FinalResultValue] = GetProductTestFinalResult(pcbaSN, productSN, shellLen);

                    #region 烧录工位信息
                    if (STATION_TURN == stationName)
                    {
                        //只更新烧录工站
                        dr[STATION_TURN + TestResultItemContent.StationInDate_turn] = stationInDate;
                        dr[STATION_TURN + TestResultItemContent.StationOutDate_turn] = stationOutDate;
                        dr[STATION_TURN + TestResultItemContent.UserTeamLeader_turn] = teamLeader;
                        dr[STATION_TURN + TestResultItemContent.TestResultValue_turn] = testResult;
                        dr[STATION_TURN + TestResultItemContent.Turn_TurnItem] = SelectTestItemValue(pcbaSN, productSN, STATION_TURN, TestResultItemContent.Turn_TurnItem, joinDateTime);
                        dr[STATION_TURN + TestResultItemContent.Turn_Voltage_12V_Item] = SelectTestItemValue(pcbaSN, productSN, STATION_TURN, TestResultItemContent.Turn_Voltage_12V_Item, joinDateTime);
                        dr[STATION_TURN + TestResultItemContent.Turn_Voltage_5V_Item] = SelectTestItemValue(pcbaSN, productSN, STATION_TURN, TestResultItemContent.Turn_Voltage_5V_Item, joinDateTime);
                        dr[STATION_TURN + TestResultItemContent.Turn_Voltage_33_1V_Item] = SelectTestItemValue(pcbaSN, productSN, STATION_TURN, TestResultItemContent.Turn_Voltage_33_1V_Item, joinDateTime);
                        dr[STATION_TURN + TestResultItemContent.Turn_Voltage_33_2V_Item] = SelectTestItemValue(pcbaSN, productSN, STATION_TURN, TestResultItemContent.Turn_Voltage_33_2V_Item, joinDateTime);
                        dr[STATION_TURN + TestResultItemContent.Turn_SoftVersion] = SelectTestItemValue(pcbaSN, productSN, STATION_TURN, TestResultItemContent.Turn_SoftVersion, joinDateTime);
                    }
                    #endregion

                    #region 灵敏度
                    if (STATION_SENSIBLITY == stationName)
                    {
                        dr[STATION_SENSIBLITY + TestResultItemContent.StationInDate_sen] = stationInDate;
                        dr[STATION_SENSIBLITY + TestResultItemContent.StationOutDate_sen] = stationOutDate;
                        dr[STATION_SENSIBLITY + TestResultItemContent.UserTeamLeader_sen] = teamLeader;
                        dr[STATION_SENSIBLITY + TestResultItemContent.TestResultValue_sen] = testResult;

                        dr[STATION_SENSIBLITY + TestResultItemContent.Sen_Work_Electric_Test] = SelectTestItemValue(pcbaSN, productSN, STATION_SENSIBLITY, TestResultItemContent.Sen_Work_Electric_Test, joinDateTime);
                        dr[STATION_SENSIBLITY + TestResultItemContent.Sen_PartNumber] = SelectTestItemValue(pcbaSN, productSN, STATION_SENSIBLITY, TestResultItemContent.Sen_PartNumber, joinDateTime);
                        dr[STATION_SENSIBLITY + TestResultItemContent.Sen_HardWareVersion] = SelectTestItemValue(pcbaSN, productSN, STATION_SENSIBLITY, TestResultItemContent.Sen_HardWareVersion, joinDateTime);
                        dr[STATION_SENSIBLITY + TestResultItemContent.Sen_SoftVersion] = SelectTestItemValue(pcbaSN, productSN, STATION_SENSIBLITY, TestResultItemContent.Sen_SoftVersion, joinDateTime);
                        dr[STATION_SENSIBLITY + TestResultItemContent.Sen_ECUID] = SelectTestItemValue(pcbaSN, productSN, STATION_SENSIBLITY, TestResultItemContent.Sen_ECUID, joinDateTime);
                        dr[STATION_SENSIBLITY + TestResultItemContent.Sen_BootloaderVersion] = SelectTestItemValue(pcbaSN, productSN, STATION_SENSIBLITY, TestResultItemContent.Sen_BootloaderVersion, joinDateTime);
                        dr[STATION_SENSIBLITY + TestResultItemContent.Sen_RadioFreq] = SelectTestItemValue(pcbaSN, productSN, STATION_SENSIBLITY, TestResultItemContent.Sen_RadioFreq, joinDateTime);
                        dr[STATION_SENSIBLITY + TestResultItemContent.Sen_DormantElect] = SelectTestItemValue(pcbaSN, productSN, STATION_SENSIBLITY, TestResultItemContent.Sen_DormantElect, joinDateTime);
                    }
                    #endregion

                    #region 外壳
                    if (stationName == STATION_SHELL)
                    {
                        dr[STATION_SHELL + TestResultItemContent.StationInDate_shell] = stationInDate;
                        dr[STATION_SHELL + TestResultItemContent.StationOutDate_shell] = stationOutDate;
                        dr[STATION_SHELL + TestResultItemContent.UserTeamLeader_shell] = teamLeader;
                        dr[STATION_SHELL + TestResultItemContent.TestResultValue_shell] = testResult;
                        dr[STATION_SHELL + TestResultItemContent.Shell_FrontCover] = SelectTestItemValue(pcbaSN, productSN, STATION_SHELL, TestResultItemContent.Shell_FrontCover, joinDateTime);
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
                        dr[STATION_AIR + TestResultItemContent.Air_AirtightTest] = SelectTestItemValue(pcbaSN, productSN, STATION_AIR, TestResultItemContent.Air_AirtightTest, joinDateTime);
                    }
                    #endregion

                    #region 支架
                    if (stationName == STATION_STENT)
                    {
                        dr[STATION_STENT + TestResultItemContent.StationInDate_stent] = stationInDate;
                        dr[STATION_STENT + TestResultItemContent.StationOutDate_stent] = stationOutDate;
                        dr[STATION_STENT + TestResultItemContent.UserTeamLeader_stent] = teamLeader;
                        dr[STATION_STENT + TestResultItemContent.TestResultValue_stent] = testResult;
                        dr[STATION_STENT + TestResultItemContent.Stent_Screw1] = SelectTestItemValue(pcbaSN, productSN, STATION_STENT, TestResultItemContent.Stent_Screw1, joinDateTime);
                        dr[STATION_STENT + TestResultItemContent.Stent_Screw2] = SelectTestItemValue(pcbaSN, productSN, STATION_STENT, TestResultItemContent.Stent_Screw1, joinDateTime);
                        dr[STATION_STENT + TestResultItemContent.Stent_Stent] = SelectTestItemValue(pcbaSN, productSN, STATION_STENT, TestResultItemContent.Stent_Stent, joinDateTime);
                        dr[STATION_STENT + TestResultItemContent.Stent_LeftStent] = SelectTestItemValue(pcbaSN, productSN, STATION_STENT, TestResultItemContent.Stent_LeftStent, joinDateTime);
                        dr[STATION_STENT + TestResultItemContent.Stent_RightStent] = SelectTestItemValue(pcbaSN, productSN, STATION_STENT, TestResultItemContent.Stent_RightStent, joinDateTime);
                    }
                    #endregion

                    #region 成品
                    if (stationName == STATION_PRODUCT)
                    {
                        dr[STATION_PRODUCT + TestResultItemContent.StationInDate_product] = stationInDate;
                        dr[STATION_PRODUCT + TestResultItemContent.StationOutDate_product] = stationOutDate;
                        dr[STATION_PRODUCT + TestResultItemContent.UserTeamLeader_product] = teamLeader;
                        dr[STATION_PRODUCT + TestResultItemContent.TestResultValue_product] = testResult;
                        dr[STATION_PRODUCT + TestResultItemContent.Product_Work_Electric_Test] = SelectTestItemValue(pcbaSN, productSN, STATION_PRODUCT, TestResultItemContent.Product_Work_Electric_Test, joinDateTime);
                        dr[STATION_PRODUCT + TestResultItemContent.Product_DormantElect] = SelectTestItemValue(pcbaSN, productSN, STATION_PRODUCT, TestResultItemContent.Product_DormantElect, joinDateTime);
                    }
                    #endregion

                    dt.Rows.Add(dr);
                    logCount++;
                }
            }
            else
            {
                LogHelper.Log.Info("dtTestResult.row=0");
            }
        }

        public string DeleteTestLogData(string queryCondition,string startTime,string endTime)
        {
            var delTestResult = "";
            var selectTestResultSQL = "";
            DataTable dtResult = new DataTable();
            try
            {
                if (queryCondition == "")
                {
                    //删除测试log数据
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
                    delTestResult = $"delete FROM " +
                            $"{DbTable.F_TEST_RESULT_NAME} " +
                            $"WHERE " +
                            $"{DbTable.F_Test_Result.UPDATE_DATE} >= '{startTime}' " +
                            $"AND " +
                            $"{DbTable.F_Test_Result.UPDATE_DATE} <= '{endTime}' ";
                    dtResult = SQLServer.ExecuteDataSet(selectTestResultSQL).Tables[0];
                }
                else
                {
                    //按工站查询
                    if (STATION_TURN.Contains(queryCondition) || STATION_SENSIBLITY.Contains(queryCondition) || STATION_SHELL.Contains(queryCondition) || STATION_AIR.Contains(queryCondition) || STATION_STENT.Contains(queryCondition) || STATION_PRODUCT.Contains(queryCondition))
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
                            $"AND " +
                            $"{DbTable.F_Test_Result.STATION_NAME} like '%{queryCondition}%' ";
                        dtResult = SQLServer.ExecuteDataSet(selectTestResultSQL).Tables[0];
                        delTestResult = $"delete FROM " +
                            $"{DbTable.F_TEST_RESULT_NAME} " +
                            $"WHERE " +
                            $"{DbTable.F_Test_Result.UPDATE_DATE} >= '{startTime}' " +
                            $"AND " +
                            $"{DbTable.F_Test_Result.UPDATE_DATE} <= '{endTime}' "+
                            $"AND " +
                            $"{DbTable.F_Test_Result.STATION_NAME} like '%{queryCondition}%' ";
                    }
                    else
                    {
                        //按PCBASN查询
                        var pcbaSN = GetPCBASn(queryCondition);
                        var productSN = GetProductSn(queryCondition);
                        if (pcbaSN != "" && productSN != "")
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
                                $"AND " +
                                $"{DbTable.F_Test_Result.SN} = '{pcbaSN}' OR {DbTable.F_Test_Result.SN} = '{productSN}'";
                            delTestResult = $"delete FROM " +
                                $"{DbTable.F_TEST_RESULT_NAME} " +
                                $"WHERE " +
                                $"{DbTable.F_Test_Result.UPDATE_DATE} >= '{startTime}' " +
                                $"AND " +
                                $"{DbTable.F_Test_Result.UPDATE_DATE} <= '{endTime}' " +
                                $"AND " +
                                $"{DbTable.F_Test_Result.SN} = '{pcbaSN}' OR {DbTable.F_Test_Result.SN} = '{productSN}'";
                        }
                        else if (pcbaSN != "" && productSN == "")
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
                                    $"AND " +
                                    $"{DbTable.F_Test_Result.SN} = '{pcbaSN}' ";
                            delTestResult = $"delete FROM " +
                                   $"{DbTable.F_TEST_RESULT_NAME} " +
                                   $"WHERE " +
                                   $"{DbTable.F_Test_Result.UPDATE_DATE} >= '{startTime}' " +
                                   $"AND " +
                                   $"{DbTable.F_Test_Result.UPDATE_DATE} <= '{endTime}' " +
                                   $"AND " +
                                   $"{DbTable.F_Test_Result.SN} = '{pcbaSN}' ";
                        }
                        else if (pcbaSN == "" && productSN != "")
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
                                    $"AND " +
                                    $"{DbTable.F_Test_Result.SN} = '{productSN}' ";
                            delTestResult = $"delete FROM " +
                                   $"{DbTable.F_TEST_RESULT_NAME} " +
                                   $"WHERE " +
                                   $"{DbTable.F_Test_Result.UPDATE_DATE} >= '{startTime}' " +
                                   $"AND " +
                                   $"{DbTable.F_Test_Result.UPDATE_DATE} <= '{endTime}' " +
                                   $"AND " +
                                   $"{DbTable.F_Test_Result.SN} = '{productSN}' ";
                        }
                        else
                        {
                            //都为空
                            return "0X07";//查询条件不存在
                        }
                        dtResult = SQLServer.ExecuteDataSet(selectTestResultSQL).Tables[0];
                    }
                }
                LogHelper.Log.Info("【查询-删除测试log数据】selectTestResultSQL=" + selectTestResultSQL);
                if (dtResult.Rows.Count > 0)
                {
                    int count = 0;//删除LOG数据数量
                    foreach (DataRow dr in dtResult.Rows)
                    {
                        var sn = dr[0].ToString();
                        var typeNo = dr[1].ToString();
                        var stationName = dr[2].ToString();
                        var joinTime = dr[7].ToString();
                        var pcbaSN = GetPCBASn(sn);
                        var productSN = GetProductSn(sn);
                        //根据PCBA-SN删除数据
                        var delLogSQL = $"delete from {DbTable.F_TEST_LOG_DATA_NAME} where " +
                            $"{DbTable.F_TEST_LOG_DATA.TYPE_NO}='{typeNo}' " +
                            $"and " +
                            $"{DbTable.F_TEST_LOG_DATA.PRODUCT_SN}='{pcbaSN}' " +
                            $"and " +
                            $"{DbTable.F_TEST_LOG_DATA.STATION_NAME}='{stationName}' " +
                            $"and " +
                            $"{DbTable.F_TEST_LOG_DATA.JOIN_DATE_TIME}='{joinTime}'";
                        var delRow = SQLServer.ExecuteNonQuery(delLogSQL);
                        if (delRow < 1)
                        {
                            //根据外壳SN删除数据
                            delLogSQL = $"delete from {DbTable.F_TEST_LOG_DATA_NAME} where " +
                            $"{DbTable.F_TEST_LOG_DATA.TYPE_NO}='{typeNo}' " +
                            $"and " +
                            $"{DbTable.F_TEST_LOG_DATA.PRODUCT_SN}='{productSN}' " +
                            $"and " +
                            $"{DbTable.F_TEST_LOG_DATA.STATION_NAME}='{stationName}' " +
                            $"and " +
                            $"{DbTable.F_TEST_LOG_DATA.JOIN_DATE_TIME}='{joinTime}'";
                            delRow += SQLServer.ExecuteNonQuery(delLogSQL);
                        }
                        count += delRow;
                    }
                    //执行删除测试结果数据
                    LogHelper.Log.Info("【查询-删除测试log数据】log数据执行完毕，开始删除测试数据=" + delTestResult);
                    var delRes = SQLServer.ExecuteNonQuery(delTestResult);
                    if (delRes > 0 && count > 0)
                    {
                        LogHelper.Log.Info($"【删除测试数据】0X01 完成{delRes}条");
                        LogHelper.Log.Info($"【删除测试LOG数据】0X01 完成{count}条");
                        DeleteBindMsgOfTestLog(queryCondition, startTime, endTime, dtResult);
                        if (queryCondition == "")
                        {
                            var delR = CheckTestLogDeleteRemainData();
                            LogHelper.Log.Info($"【删除测试LOG数据】异常数据  0X01 完成{delR}条");
                        }
                        return "0X01";
                    }
                    else if (delRes > 0 && count <= 0)
                    {
                        LogHelper.Log.Info($"【删除测试数据】0X02 完成{delRes}条");
                        LogHelper.Log.Info($"【删除测试LOG数据】0X02 未完成 CODE=0X02");
                        return "0X02";
                    }
                    else if (delRes <= 0 && count > 0)
                    {
                        LogHelper.Log.Info($"【删除测试数据】0X03 未完成 CODE=0X03");
                        LogHelper.Log.Info($"【删除测试LOG数据】0X03 完成 {count}条");
                        return "0X03";
                    }
                    LogHelper.Log.Info($"【删除测试数据及log数据】都未完成 CODE=0X04");
                    return "0X04";
                }
                else
                {
                    LogHelper.Log.Info($"【查询log测试数据】查询失败-删除失败delTestResult={delTestResult}selectTestResultSQL={selectTestResultSQL}");
                    return "0X05";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error(ex.Message + ex.StackTrace);
                return "0X06";
            }
        }

        private int CheckTestLogDeleteRemainData()
        {
            var deleteSQL = $"delete from {DbTable.F_TEST_LOG_DATA_NAME}";
            return SQLServer.ExecuteNonQuery(deleteSQL);
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

        /// <summary>
        /// 删除测试log数据后，检查是否删除/执行删除绑定数据
        /// </summary>
        /// <param name="queryCondition"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private int DeleteBindMsgOfTestLog(string queryCondition,string startTime,string endTime,DataTable dtRes)
        {
            /*
             * 根据查询条件中的SN
             * 1）查询物料统计中是否还存在记录
             * 2）在查询当前条件之外的过站log是否还存在数据
             * 都不存在时，删除绑定关系数据
             */
            int delCount = 0;
            foreach (DataRow dataRow in dtRes.Rows)
            {
                var sn = dataRow[0].ToString();
                var snPCBA = GetPCBASnOfBind(sn);//不区分解绑状态
                var snOutter = GetOutterSnOfBind(sn);
                if (snPCBA != "" && snOutter != "")
                {
                    //都不为空，一定是有绑定关系（不考虑解绑）
                    //1）查询物料统计是否存在数据
                    var selectMaterialMsg = $"SELECT * FROM {DbTable.F_MATERIAL_STATISTICS_NAME} WHERE " +
                        $"{DbTable.F_Material_Statistics.PCBA_SN} = '{snPCBA}' " +
                        $"OR " +
                        $"{DbTable.F_Material_Statistics.PCBA_SN} = '{snOutter}'";
                    var materialData = SQLServer.ExecuteDataSet(selectMaterialMsg).Tables[0];
                    if (materialData.Rows.Count < 1)
                    {
                        //结果：物料统计不存在数据
                        //2）查询当前条件之外是否存在数据
                        var selectResultSQL = $"SELECT * FROM {DbTable.F_TEST_RESULT_NAME} WHERE " +
                            $"({DbTable.F_Test_Result.UPDATE_DATE} < '{startTime}' AND {DbTable.F_Test_Result.SN} = '{snPCBA}') OR " +
                            $"({DbTable.F_Test_Result.UPDATE_DATE} < '{startTime}' AND {DbTable.F_Test_Result.SN} = '{snOutter}') OR " +
                            $"({DbTable.F_Test_Result.UPDATE_DATE} > '{endTime}'   AND {DbTable.F_Test_Result.SN} = '{snPCBA}') OR " +
                            $"({DbTable.F_Test_Result.UPDATE_DATE} > '{endTime}'   AND {DbTable.F_Test_Result.SN} = '{snOutter}') ";
                        var otherTestResultData = SQLServer.ExecuteDataSet(selectResultSQL).Tables[0];
                        if (otherTestResultData.Rows.Count < 1)
                        {
                            //已不存在数据，删除绑定记录
                            var deleteBindSQL = $"DELETE FROM {DbTable.F_BINDING_PCBA_NAME} WHERE " +
                                $"{DbTable.F_BINDING_PCBA.SN_PCBA} = '{snPCBA}' " +
                                $"AND " +
                                $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{snOutter}'";
                            var delRow = SQLServer.ExecuteNonQuery(deleteBindSQL);
                            delCount += delRow;
                        }
                    }
                }
                else if (snPCBA == "" && snOutter != "")
                {
                    //PCBA为空，外壳不为空，可能是跳站
                    //1）查询物料统计是否存在数据
                    var selectMaterialMsg = $"SELECT * FROM {DbTable.F_MATERIAL_STATISTICS_NAME} WHERE " +
                        $"{DbTable.F_Material_Statistics.PCBA_SN} = '{snOutter}'";
                    var materialData = SQLServer.ExecuteDataSet(selectMaterialMsg).Tables[0];
                    if (materialData.Rows.Count < 1)
                    {
                        //结果：物料统计不存在数据
                        //2）查询当前条件之外是否存在数据
                        var selectResultSQL = $"SELECT * FROM {DbTable.F_TEST_RESULT_NAME} WHERE " +
                            $"{DbTable.F_Test_Result.SN} = '{snOutter}' AND " +
                            $"{DbTable.F_Test_Result.UPDATE_DATE} < '{startTime}' OR " +
                            $"{DbTable.F_Test_Result.UPDATE_DATE} > '{endTime}'";
                        var otherTestResultData = SQLServer.ExecuteDataSet(selectResultSQL).Tables[0];
                        if (otherTestResultData.Rows.Count < 0)
                        {
                            //已不存在数据，删除绑定记录
                            var deleteBindSQL = $"DELETE FROM {DbTable.F_BINDING_PCBA_NAME} WHERE " +
                                $"{DbTable.F_BINDING_PCBA.SN_PCBA} = '{snPCBA}' " +
                                $"AND " +
                                $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{snOutter}'";
                            var delRow = SQLServer.ExecuteNonQuery(deleteBindSQL);
                            delCount += delRow;
                        }
                    }
                }
                else
                {
                    continue;
                }
            }
            LogHelper.Log.Info("【删除过站记录-查询删除绑定记录】删除绑定记录="+delCount);
            return delCount;
        }

        private string GetPCBASnOfBind(string sn)
        {
            var selectSQL = $"SELECT TOP 1 {DbTable.F_BINDING_PCBA.SN_PCBA} FROM {DbTable.F_BINDING_PCBA_NAME} " +
                $"WHERE " +
                $"{DbTable.F_BINDING_PCBA.SN_PCBA} = '{sn}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return sn;
            else
            {
                selectSQL = $"SELECT TOP 1 {DbTable.F_BINDING_PCBA.SN_PCBA} FROM {DbTable.F_BINDING_PCBA_NAME} " +
                $"WHERE " +
                $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{sn}'";
                dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                if (dt.Rows.Count > 0)
                    return dt.Rows[0][0].ToString();
            }
            return "";
        }

        private string GetOutterSnOfBind(string sn)
        {
            var selectSQL = $"SELECT TOP 1 {DbTable.F_BINDING_PCBA.SN_OUTTER} FROM {DbTable.F_BINDING_PCBA_NAME} " +
                $"WHERE " +
                $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{sn}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return sn;
            else
            {
                selectSQL = $"SELECT TOP 1 {DbTable.F_BINDING_PCBA.SN_OUTTER} FROM {DbTable.F_BINDING_PCBA_NAME} " +
                $"WHERE " +
                $"{DbTable.F_BINDING_PCBA.SN_PCBA} = '{sn}'";
                dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                if (dt.Rows.Count > 0)
                    return dt.Rows[0][0].ToString();
            }
            return "";
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

        public DataSet SelectMaterial(string codeRid, MaterialStockState stockStateEnum)
        {
            //stockState=1-在线库存，2/3-出库库存
            var selectSQL = "";
            var stockStateCondition = "";
            if(stockStateEnum == MaterialStockState.PUT_IN_STOCK)
                stockStateCondition = $"{DbTable.F_Material.MATERIAL_STATE} = '1'";
            else if(stockStateEnum == MaterialStockState.STOCK_USE_COMPLED)
                stockStateCondition = $"{DbTable.F_Material.MATERIAL_STATE} = '2'";
            else if(stockStateEnum == MaterialStockState.STOCK_STATEMETN)
                stockStateCondition = $"{DbTable.F_Material.MATERIAL_STATE} = '3'";
            else if(stockStateEnum == MaterialStockState.PUT_IN_STOCK_AND_STATEMENT)
                stockStateCondition = $"{DbTable.F_Material.MATERIAL_STATE} = '1' OR {DbTable.F_Material.MATERIAL_STATE} = '3'";
            if (codeRid == "")
            {
                selectSQL = $"SELECT " +
                $"{DbTable.F_Material.MATERIAL_CODE}," +
                $"{DbTable.F_Material.MATERIAL_NAME}," +
                $"{DbTable.F_Material.MATERIAL_USERNAME}," +
                $"{DbTable.F_Material.MATERIAL_UPDATE_DATE}," +
                $"{DbTable.F_Material.MATERIAL_DESCRIBLE}," +
                $"{DbTable.F_Material.MATERIAL_STOCK}," +
                $"{DbTable.F_Material.MATERIAL_STATE} " +
                $"FROM " +
                $"{DbTable.F_MATERIAL_NAME} " +
                $"WHERE " +
                $"{stockStateCondition}";
            }
            else
            {
                selectSQL = $"SELECT " +
                $"{DbTable.F_Material.MATERIAL_CODE}," +
                $"{DbTable.F_Material.MATERIAL_NAME}," +
                $"{DbTable.F_Material.MATERIAL_USERNAME}," +
                $"{DbTable.F_Material.MATERIAL_UPDATE_DATE}," +
                $"{DbTable.F_Material.MATERIAL_DESCRIBLE}," +
                $"{DbTable.F_Material.MATERIAL_STOCK}," +
                $"{DbTable.F_Material.MATERIAL_STATE} " +
                $"FROM {DbTable.F_MATERIAL_NAME} " +
                $"WHERE " +
                $"{DbTable.F_Material.MATERIAL_CODE} like '%{codeRid}%' " +
                $"AND " +
                $"{stockStateCondition}";
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
            try
            {
                foreach (var material in pmList)
                {
                    if (IsExistMaterial(material))
                    {
                        //更新
                        string updateSQL = $"UPDATE {DbTable.F_PRODUCT_MATERIAL_NAME} SET " +
                            $"{DbTable.F_PRODUCT_MATERIAL.Describle} = '{material.Describle}'," +
                            $"{DbTable.F_PRODUCT_MATERIAL.USERNAME} = '{material.UserName}' " +
                            $"WHERE {DbTable.F_PRODUCT_MATERIAL.TYPE_NO} = '{material.TypeNo}' AND " +
                            $"{DbTable.F_PRODUCT_MATERIAL.MATERIAL_PN} = '{material.MaterialCode}'";

                        string selectSQL = $"SELECT * FROM {DbTable.F_PRODUCT_MATERIAL_NAME} WHERE " +
                            $"{DbTable.F_PRODUCT_MATERIAL.TYPE_NO} = '{material.TypeNo}' AND " +
                            $"{DbTable.F_PRODUCT_MATERIAL.MATERIAL_PN} = '{material.MaterialCode}' AND " +
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
                            //已修改为装配时扫码入库
                            //UpdateMaterialStock(material.TypeNo, material.MaterialCode, material.Stock);
                        }
                    }
                    UpdateMaterialPN(material.MaterialCode, material.MaterialName, material.UserName);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error(ex.Message+ex.StackTrace);
            }
            return productMaterialList;
        }
        public int DeleteProductMaterial(ProductMaterial material)
        {
            if (material.MaterialCode == "" || material.TypeNo == "")
                return 0;
            string deleteSQL = $"DELETE FROM {DbTable.F_PRODUCT_MATERIAL_NAME} " +
                $"WHERE {DbTable.F_PRODUCT_MATERIAL.TYPE_NO} = '{material.TypeNo}' AND " +
                $"{DbTable.F_PRODUCT_MATERIAL.MATERIAL_PN} = '{material.MaterialCode}'";
            LogHelper.Log.Info(deleteSQL);
            return SQLServer.ExecuteNonQuery(deleteSQL);
        }

        public DataSet SelectProductMaterial(string condition)
        {
            var selectSQL = "";
            if (condition == "")
            {
                selectSQL = $"SELECT " +
                            $"a.{DbTable.F_PRODUCT_MATERIAL.TYPE_NO}," +
                            $"a.{DbTable.F_PRODUCT_MATERIAL.MATERIAL_PN}," +
                            $"b.{DbTable.F_MATERIAL_PN.MATERIAL_NAME}," +
                            $"a.{DbTable.F_PRODUCT_MATERIAL.Describle}," +
                            $"a.{DbTable.F_PRODUCT_MATERIAL.USERNAME}," +
                            $"a.{DbTable.F_PRODUCT_MATERIAL.UpdateDate} " +
                            $"FROM " +
                            $"{DbTable.F_PRODUCT_MATERIAL_NAME} a," +
                            $"{DbTable.F_MATERIAL_PN_NAME} b " +
                            $"WHERE " +
                            $"a.{DbTable.F_PRODUCT_MATERIAL.MATERIAL_PN} = b.{DbTable.F_MATERIAL_PN.MATERIAL_PN} " +
                            $"ORDER BY {DbTable.F_PRODUCT_MATERIAL.TYPE_NO} ";
            }
            else
            {
                selectSQL = $"SELECT " +
                            $"a.{DbTable.F_PRODUCT_MATERIAL.TYPE_NO}," +
                            $"a.{DbTable.F_PRODUCT_MATERIAL.MATERIAL_PN}," +
                            $"b.{DbTable.F_MATERIAL_PN.MATERIAL_NAME}," +
                            $"a.{DbTable.F_PRODUCT_MATERIAL.Describle}," +
                            $"a.{DbTable.F_PRODUCT_MATERIAL.USERNAME}," +
                            $"a.{DbTable.F_PRODUCT_MATERIAL.UpdateDate} " +
                            $"FROM " +
                            $"{DbTable.F_PRODUCT_MATERIAL_NAME} a," +
                            $"{DbTable.F_MATERIAL_PN_NAME} b " +
                            $"WHERE " +
                            $"a.{DbTable.F_PRODUCT_MATERIAL.MATERIAL_PN} = b.{DbTable.F_MATERIAL_PN.MATERIAL_PN} " +
                            $"AND a.{DbTable.F_PRODUCT_MATERIAL.TYPE_NO} like '%{condition}%' " +
                            $"ORDER BY {DbTable.F_PRODUCT_MATERIAL.TYPE_NO} ";
            }
            LogHelper.Log.Info(selectSQL);
            return SQLServer.ExecuteDataSet(selectSQL);
        }

        private bool IsExistMaterial(ProductMaterial material)
        {
            string selectSQL = $"SELECT * FROM {DbTable.F_PRODUCT_MATERIAL_NAME} " +
                $"WHERE {DbTable.F_PRODUCT_MATERIAL.MATERIAL_PN} = '{material.MaterialCode}' AND " +
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
                $"{DbTable.F_PRODUCT_MATERIAL.MATERIAL_PN}," +
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
        private DataTable InitMaterialBasic()
        {
            #region init datatable
            DataTable dataSourceMaterialBasic = new DataTable();
            dataSourceMaterialBasic.Columns.Add(DATA_ORDER);
            dataSourceMaterialBasic.Columns.Add(MATERIAL_PN);
            dataSourceMaterialBasic.Columns.Add(MATERIAL_LOT);
            dataSourceMaterialBasic.Columns.Add(MATERIAL_RID);
            dataSourceMaterialBasic.Columns.Add(MATERIAL_DC);
            dataSourceMaterialBasic.Columns.Add(MATERIAL_NAME);
            dataSourceMaterialBasic.Columns.Add(PRODUCT_TYPENO);
            dataSourceMaterialBasic.Columns.Add(SN_PCBA);
            dataSourceMaterialBasic.Columns.Add(SN_OUTTER);
            dataSourceMaterialBasic.Columns.Add(MATERIAL_QTY);
            dataSourceMaterialBasic.Columns.Add(USE_AMOUNTED);
            dataSourceMaterialBasic.Columns.Add(CURRENT_RESIDUE_STOCK);
            dataSourceMaterialBasic.Columns.Add(RESIDUE_STOCK);
            dataSourceMaterialBasic.Columns.Add(PCBA_STATUS);
            #endregion
            return dataSourceMaterialBasic;
        }

        public DataSet SelectMaterialBasicMsg(string queryCondition)
        {
            if (queryCondition == null)
                queryCondition = "";
            var dataSourceMaterialBasic = InitMaterialBasic();
            var pcbaSN = GetPCBASn(queryCondition);
            var productSN = GetProductSn(queryCondition);
            var selectMsgOfMaterialSQL = "";
            var selectMsgOfSn = "";
            /*
             * 查询条件：物料编号/PCBSN/外壳SN
             * 判断查询条件的类型
             */
            #region SQL
            var selectSQL = $"SELECT DISTINCT " +
                $"a.{DbTable.F_Material_Statistics.MATERIAL_CODE} 物料编码," +
                $"a.{DbTable.F_Material_Statistics.PRODUCT_TYPE_NO} 产品型号," +
                $"a.{DbTable.F_Material_Statistics.PCBA_SN}," +
                $"a.{DbTable.F_Material_Statistics.MATERIAL_AMOUNT} 当前使用数量," +
                $"a.{DbTable.F_Material_Statistics.MATERIAL_CURRENT_REMAIN} 当前剩余数量 " +
                $"FROM " +
                $"{DbTable.F_MATERIAL_STATISTICS_NAME} a ";
            selectMsgOfMaterialSQL = $"SELECT DISTINCT " +
                $"{DbTable.F_Material_Statistics.MATERIAL_CODE} ," +
                $"{DbTable.F_Material_Statistics.PRODUCT_TYPE_NO}," +
                $"{DbTable.F_Material_Statistics.PCBA_SN}, " +
                $"{DbTable.F_Material_Statistics.MATERIAL_AMOUNT}, " +
                $"{DbTable.F_Material_Statistics.MATERIAL_CURRENT_REMAIN} " +
                $"FROM " +
                $"{DbTable.F_MATERIAL_STATISTICS_NAME} " +
                $"WHERE " +
                $"{DbTable.F_Material_Statistics.MATERIAL_CODE} like '%{queryCondition}%'";
            if (pcbaSN != "" && productSN == "")
            {
                selectMsgOfSn = $"SELECT DISTINCT " +
                    $"{DbTable.F_Material_Statistics.MATERIAL_CODE} ," +
                    $"{DbTable.F_Material_Statistics.PRODUCT_TYPE_NO}," +
                    $"{DbTable.F_Material_Statistics.PCBA_SN}, " +
                    $"{DbTable.F_Material_Statistics.MATERIAL_AMOUNT}, " +
                    $"{DbTable.F_Material_Statistics.MATERIAL_CURRENT_REMAIN} " +
                    $"FROM " +
                    $"{DbTable.F_MATERIAL_STATISTICS_NAME} " +
                    $"WHERE " +
                    $"{DbTable.F_Material_Statistics.PCBA_SN} like '%{pcbaSN}%' ";
            }
            else if (pcbaSN == "" && productSN != "")
            {
                selectMsgOfSn = $"SELECT DISTINCT " +
                    $"{DbTable.F_Material_Statistics.MATERIAL_CODE} ," +
                    $"{DbTable.F_Material_Statistics.PRODUCT_TYPE_NO}," +
                    $"{DbTable.F_Material_Statistics.PCBA_SN}, " +
                    $"{DbTable.F_Material_Statistics.MATERIAL_AMOUNT}, " +
                    $"{DbTable.F_Material_Statistics.MATERIAL_CURRENT_REMAIN} " +
                    $"FROM " +
                    $"{DbTable.F_MATERIAL_STATISTICS_NAME} " +
                    $"WHERE " +
                    $"{DbTable.F_Material_Statistics.PCBA_SN} like '%{productSN}%' ";
            }
            else if (pcbaSN != "" && productSN != "")
            {
                selectMsgOfSn = $"SELECT DISTINCT " +
                    $"{DbTable.F_Material_Statistics.MATERIAL_CODE} ," +
                    $"{DbTable.F_Material_Statistics.PRODUCT_TYPE_NO}," +
                    $"{DbTable.F_Material_Statistics.PCBA_SN}, " +
                    $"{DbTable.F_Material_Statistics.MATERIAL_AMOUNT}, " +
                    $"{DbTable.F_Material_Statistics.MATERIAL_CURRENT_REMAIN} " +
                    $"FROM " +
                    $"{DbTable.F_MATERIAL_STATISTICS_NAME} " +
                    $"WHERE " +
                    $"{DbTable.F_Material_Statistics.PCBA_SN} like '%{productSN}%' " +
                    $"OR " +
                    $"{DbTable.F_Material_Statistics.PCBA_SN} like '%{pcbaSN}%'";
            }
            else
            {
                selectMsgOfSn = selectSQL;
            }
            #endregion

            if (queryCondition == "")
            {
                //查询所有信息
                LogHelper.Log.Info("【物料查询】--查询所有 " + selectSQL);
                return SelectMaterialDetail(dataSourceMaterialBasic, selectSQL);
            }
            else
            {
                //根据物料编码查询
                //根据PCBA/外壳编码查询

                //根据物料查询
                LogHelper.Log.Info("【物料查询--selectMsgOfMaterialSQL】" + selectMsgOfMaterialSQL);
                var detailResult = SelectMaterialDetail(dataSourceMaterialBasic, selectMsgOfMaterialSQL);
                if (detailResult.Tables[0].Rows.Count < 1)
                {
                    //根据SN查询
                    LogHelper.Log.Info("【物料查询--selectMsgOfSn】" + selectMsgOfSn);
                    dataSourceMaterialBasic = InitMaterialBasic();
                    return SelectMaterialDetail(dataSourceMaterialBasic, selectMsgOfSn);
                }
                return detailResult;
            }
        }

        public int DeleteMaterialBasicMsg(string queryCondition)
        {
            var deleteAllMaterialMsg = "";
            var deleteOfMaterialCode = "";
            var deleteOfSN = "";
            var delRow = 0;
            if (queryCondition == "")
            {
                deleteAllMaterialMsg = $"DELETE FROM {DbTable.F_MATERIAL_STATISTICS_NAME}";
                delRow = SQLServer.ExecuteNonQuery(deleteAllMaterialMsg);
                LogHelper.Log.Info($"【删除所有物料使用数据】删除{delRow}条");
                if(delRow > 0)
                    DeleteBindedDataOfDeleteMaterial(queryCondition);
                return delRow;
            }
            else
            {
                deleteOfMaterialCode = $"DELETE FROM {DbTable.F_MATERIAL_STATISTICS_NAME} " +
                    $"WHERE " +
                    $"{DbTable.F_Material_Statistics.MATERIAL_CODE} like '%{queryCondition}%'";
                delRow = SQLServer.ExecuteNonQuery(deleteOfMaterialCode);
                if(delRow > 0)
                    DeleteBindedDataOfDeleteMaterial(queryCondition);
                LogHelper.Log.Info($"【删除所有物料使用数据】删除{delRow}条");
                if (delRow < 1)
                {
                    var pcbaSN = GetPCBASn(queryCondition);
                    var productSN = GetProductSn(queryCondition);
                    if (pcbaSN == "" && productSN != "")
                    {
                        deleteOfSN = $"DELETE FROM {DbTable.F_MATERIAL_STATISTICS_NAME} WHERE " +
                            $"{DbTable.F_Material_Statistics.PCBA_SN} like '%{productSN}%'";
                    }
                    else if (pcbaSN != "" && productSN == "")
                    {
                        deleteOfSN = $"DELETE FROM {DbTable.F_MATERIAL_STATISTICS_NAME} WHERE " +
                            $"{DbTable.F_Material_Statistics.PCBA_SN} like '%{pcbaSN}%'";
                    }
                    else if (pcbaSN != "" && productSN != "")
                    {
                        deleteOfSN = $"DELETE FROM {DbTable.F_MATERIAL_STATISTICS_NAME} WHERE " +
                            $"{DbTable.F_Material_Statistics.PCBA_SN} like '%{productSN}%' OR " +
                            $"{DbTable.F_Material_Statistics.PCBA_SN} like '%{pcbaSN}%'";
                    }
                    else
                    {
                        LogHelper.Log.Info("【删除物料】PCBA/外壳为空--无删除数据");
                    }
                    delRow = SQLServer.ExecuteNonQuery(deleteOfSN);
                    if(delRow > 0)
                        DeleteBindedDataOfDeleteMaterial(queryCondition);
                    LogHelper.Log.Info($"【删除所有物料使用数据】删除{delRow}条");
                }
                return delRow;
            }
        }

        /// <summary>
        /// 删除物料结束时，查询是否还存在过站记录,不存在时，删除绑定记录
        /// </summary>
        /// <param name="queryCondition"></param>
        /// <returns></returns>
        private int DeleteBindedDataOfDeleteMaterial(string queryCondition)
        {
            ///查询条件为空/物料编码/PCBASN/外壳SN
            var selectTestResult = "";
            var deleteBindedMsg = "";
            int delCount = 0;
            if (queryCondition == "")
            {
                selectTestResult = $"SELECT * FROM {DbTable.F_TEST_RESULT_NAME} ";
                deleteBindedMsg = $"DELETE FROM {DbTable.F_BINDING_PCBA_NAME}";
                var dt = SQLServer.ExecuteDataSet(selectTestResult).Tables[0];
                if (dt.Rows.Count < 1)
                {
                    LogHelper.Log.Info("【删除绑定记录-删除全部】已删除过站记录，删除全部绑定记录");
                    delCount = SQLServer.ExecuteNonQuery(deleteBindedMsg);
                }
            }
            else
            {
                var selectBindMsg = $"SELECT {DbTable.F_BINDING_PCBA.SN_PCBA},{DbTable.F_BINDING_PCBA.SN_OUTTER},{DbTable.F_BINDING_PCBA.MATERIAL_CODE} FROM {DbTable.F_BINDING_PCBA_NAME} WHERE " +
                    $"{DbTable.F_BINDING_PCBA.MATERIAL_CODE} like '%{queryCondition}%'";
                var bindData = SQLServer.ExecuteDataSet(selectBindMsg).Tables[0];
                if (bindData.Rows.Count > 0)
                {
                    //1)条件为物料编码,根据物料编码查询SN
                    //根据SN查询过站记录是否已删除
                    foreach (DataRow dataRow in bindData.Rows)
                    {
                        var pcbSN = dataRow[0].ToString();//可能为空
                        var shellSN = dataRow[1].ToString();//可能为空
                        var materialCode = dataRow[2].ToString();
                        DataTable testResultData = null;
                        //pcbaSN 查询记录
                        if (pcbSN != "" && shellSN == "")
                        {
                            LogHelper.Log.Info("【删除绑定记录-物料-条件】PCBA不为空-外壳为空，理论上不存在");
                        }
                        else if (pcbSN == "" && shellSN != "")
                        {
                            //针对跳站的情况，支架装配绑定时，只有外壳，PCBA为空
                            LogHelper.Log.Info("【删除绑定记录-物料-条件】PCBA为空-外壳不为空，可能是跳站外壳装配");
                            selectTestResult = $"SELECT * FROM {DbTable.F_TEST_RESULT_NAME} WHERE " +
                                $"{DbTable.F_Test_Result.SN} = '{shellSN}' ";
                            testResultData = SQLServer.ExecuteDataSet(selectTestResult).Tables[0];
                            if (testResultData.Rows.Count < 1)
                            {
                                //pcbaSN 查询无记录
                                //删除绑定记录
                                deleteBindedMsg = $"DELETE FROM {DbTable.F_BINDING_PCBA_NAME} WHERE " +
                                         $"{DbTable.F_BINDING_PCBA.MATERIAL_CODE} = '{materialCode}'" +
                                         $"AND " +
                                         $"{DbTable.F_BINDING_PCBA.SN_PCBA} = '{pcbSN}' " +
                                         $"AND " +
                                         $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{shellSN}'";
                                var delRow = SQLServer.ExecuteNonQuery(deleteBindedMsg);
                                delCount += delRow;
                            }
                        }
                        else if (pcbSN != "" && shellSN != "")
                        {
                            //针对不跳站的情况，都不会为空
                            selectTestResult = $"SELECT * FROM {DbTable.F_TEST_RESULT_NAME} WHERE " +
                                $"{DbTable.F_Test_Result.SN} = '{pcbSN}' " +
                                $"OR " +
                                $"{DbTable.F_Test_Result.SN} = '{shellSN}'";
                            testResultData = SQLServer.ExecuteDataSet(selectTestResult).Tables[0];
                            if (testResultData.Rows.Count < 1)
                            {
                                //pcbaSN 查询无记录
                                //删除绑定记录
                                deleteBindedMsg = $"DELETE FROM {DbTable.F_BINDING_PCBA_NAME} WHERE " +
                                         $"{DbTable.F_BINDING_PCBA.MATERIAL_CODE} = '{materialCode}'" +
                                         $"AND " +
                                         $"{DbTable.F_BINDING_PCBA.SN_PCBA} = '{pcbSN}' " +
                                         $"AND " +
                                         $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{shellSN}'";
                                var delRow = SQLServer.ExecuteNonQuery(deleteBindedMsg);
                                delCount += delRow;
                            }
                        }
                    }
                }
                else
                {
                    //2)条件为物料编码查询无记录，条件为SN
                    //查询是否有过站记录
                    //条件为PCBASN
                    selectBindMsg = $"SELECT {DbTable.F_BINDING_PCBA.SN_PCBA},{DbTable.F_BINDING_PCBA.SN_OUTTER},{DbTable.F_BINDING_PCBA.MATERIAL_CODE} FROM {DbTable.F_BINDING_PCBA_NAME} WHERE " +
                        $"{DbTable.F_BINDING_PCBA.SN_PCBA} like '%{queryCondition}%'";
                    bindData = SQLServer.ExecuteDataSet(selectBindMsg).Tables[0];
                    if (bindData.Rows.Count > 0)
                    {
                        foreach (DataRow dataRow in bindData.Rows)
                        {
                            var snPCBA = dataRow[0].ToString();
                            var snOutter = dataRow[1].ToString();
                            var materialCode = dataRow[2].ToString();
                            if (snPCBA != "" && snOutter != "")
                            {
                                selectTestResult = $"SELECT * FROM {DbTable.F_TEST_RESULT_NAME} WHERE " +
                                    $"{DbTable.F_Test_Result.SN} = '{snPCBA}' OR {DbTable.F_Test_Result.SN} = '{snOutter}'";
                                var testResultData = SQLServer.ExecuteDataSet(selectTestResult).Tables[0];
                                if (testResultData.Rows.Count < 1)
                                {
                                    //无过站记录
                                    //删除绑定记录
                                    deleteBindedMsg = $"DELETE FROM {DbTable.F_BINDING_PCBA_NAME} WHERE " +
                                        $"{DbTable.F_BINDING_PCBA.SN_PCBA}  = '{snPCBA}' " +
                                        $"AND " +
                                        $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{snOutter}' " +
                                        $"AND " +
                                        $"{DbTable.F_BINDING_PCBA.MATERIAL_CODE} = '{materialCode}'";
                                    var delRow = SQLServer.ExecuteNonQuery(deleteBindedMsg);
                                    delCount += delRow;
                                }
                            }
                            else
                            {
                                LogHelper.Log.Info("【删除绑定记录-SNPCBA-条件】PCBA或外壳有为空 "+snPCBA+" "+snOutter);
                            }
                        }
                    }
                    else
                    {
                        //条件为外壳SN
                        selectBindMsg = $"SELECT {DbTable.F_BINDING_PCBA.SN_PCBA},{DbTable.F_BINDING_PCBA.SN_OUTTER},{DbTable.F_BINDING_PCBA.MATERIAL_CODE} FROM {DbTable.F_BINDING_PCBA_NAME} WHERE " +
                            $"{DbTable.F_BINDING_PCBA.SN_OUTTER} like '%{queryCondition}%'";
                        bindData = SQLServer.ExecuteDataSet(selectBindMsg).Tables[0];
                        if (bindData.Rows.Count > 0)
                        {
                            foreach (DataRow dataRow in bindData.Rows)
                            {
                                var snPCBA = dataRow[0].ToString();
                                var snOutter = dataRow[1].ToString();
                                var materialCode = dataRow[2].ToString();
                                if (snPCBA != "" && snOutter != "")
                                {
                                    selectTestResult = $"SELECT * FROM {DbTable.F_TEST_RESULT_NAME} WHERE " +
                                        $"{DbTable.F_Test_Result.SN} = '{snPCBA}' OR {DbTable.F_Test_Result.SN} = '{snOutter}'";
                                    var testResultData = SQLServer.ExecuteDataSet(selectTestResult).Tables[0];
                                    if (testResultData.Rows.Count < 1)
                                    {
                                        //无过站记录
                                        //删除绑定记录
                                        deleteBindedMsg = $"DELETE FROM {DbTable.F_BINDING_PCBA_NAME} WHERE " +
                                            $"{DbTable.F_BINDING_PCBA.SN_PCBA}  = '{snPCBA}' " +
                                            $"AND " +
                                            $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{snOutter}' " +
                                            $"AND " +
                                            $"{DbTable.F_BINDING_PCBA.MATERIAL_CODE} = '{materialCode}'";
                                        var delRow = SQLServer.ExecuteNonQuery(deleteBindedMsg);
                                        delCount += delRow;
                                    }
                                }
                                else
                                {
                                    LogHelper.Log.Info("【删除绑定记录-SN外壳-条件】PCBA或外壳有为空 " + snPCBA + " " + snOutter);
                                }
                            }
                        }
                    }
                }
            }
            LogHelper.Log.Info("【删除物料统计后--查询删除绑定记录】删除绑定记录="+delCount);
            return delCount;
        }

        private DataSet SelectMaterialDetail(DataTable dataSourceMaterialBasic,string selectSQL)
        {
            DataSet ds = new DataSet();
            DbDataReader dataReader = SQLServer.ExecuteDataReader(selectSQL);
            if (!dataReader.HasRows)
            {
                ds.Tables.Add(dataSourceMaterialBasic);
                return ds;
            }
            var i = 0;
            var shellLen = ReadShellCodeLength();
            //物料编码/产品型号/产品SN/当前使用数量/当前剩余数量
            while (dataReader.Read())
            {
                DataRow dr = dataSourceMaterialBasic.NewRow();
                var materialCode = dataReader[0].ToString();//pn/lot/rid/dc/qty
                var productTypeNo = dataReader[1].ToString();
                var sn = dataReader[2].ToString();
                var useAmounted = dataReader[3].ToString();//当前使用数量
                var currentRemain = dataReader[4].ToString();
                var snPCBA = GetPCBASn(sn);
                var snOutter = GetProductSn(sn);
                if (!materialCode.Contains("&"))
                    continue;
                AnalysisMaterialCode analysisMaterial = AnalysisMaterialCode.GetMaterialDetail(materialCode);
                var pnCode = analysisMaterial.MaterialPN;
                var lotCode = analysisMaterial.MaterialLOT;
                var ridCode = analysisMaterial.MaterialRID;
                var dcCode = analysisMaterial.MaterialDC;
                //var qtyCode = analysisMaterial.MaterialQTY;
                var materialName = SelectMaterialName(pnCode);
                var stockMsg = SelectStockMsg(materialCode);
                dr[DATA_ORDER] = i + 1;
                dr[MATERIAL_PN] = pnCode;
                dr[MATERIAL_LOT] = lotCode;
                dr[MATERIAL_RID] = ridCode;
                dr[MATERIAL_DC] = dcCode;
                dr[MATERIAL_QTY] = stockMsg.ActualStock;
                dr[MATERIAL_NAME] = materialName;
                dr[PRODUCT_TYPENO] = productTypeNo;
                dr[USE_AMOUNTED] = useAmounted;
                dr[RESIDUE_STOCK] = stockMsg.ActualStock - stockMsg.UseAmounted;
                dr[CURRENT_RESIDUE_STOCK] = currentRemain;

                dr[SN_PCBA] = snPCBA;
                dr[SN_OUTTER] = snOutter;
                dr[PCBA_STATUS] = SelectPcbaMsg(snPCBA, snOutter);
                dataSourceMaterialBasic.Rows.Add(dr);
                i++;
            }
            dataReader.Close();
            ds.Tables.Add(dataSourceMaterialBasic);
            return ds;
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

        private string SelectPcbaMsg(string snPCBA,string snOutter)
        {
            var selectSQL = $"SELECT top 1" +
                $"{DbTable.F_BINDING_PCBA.BINDING_STATE}," +
                $"{DbTable.F_BINDING_PCBA.PCBA_STATE}," +
                $"{DbTable.F_BINDING_PCBA.OUTTER_STATE} " +
                $"FROM {DbTable.F_BINDING_PCBA_NAME} WHERE " +
                $"{DbTable.F_BINDING_PCBA.SN_PCBA} = '{snPCBA}' " +
                $"AND " +
                $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{snOutter}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                var bindingState = dt.Rows[0][0].ToString();
                var pcbaState = dt.Rows[0][1].ToString();
                var outterState = dt.Rows[0][2].ToString();
                if (bindingState == "1")
                {
                    //已绑定
                    return "【已绑定】状态正常";
                }
                else if (bindingState == "0")
                {
                    //已解绑
                    if (pcbaState == "0" && outterState == "1")
                    {
                        //pcba异常
                        return "【已解绑】PCBA异常";
                    }
                    else if (pcbaState == "1" && outterState == "0")
                    {
                        //外壳异常
                        return "【已解绑】外壳异常";
                    }
                    else if (pcbaState == "0" && outterState == "0")
                    {
                        //pcba与外壳都异常
                        return "【已解绑】都异常";
                    }
                    else
                        return "";
                }
                else
                    return "";
            }
            return "【未绑定】";
        }

        private StockMsg SelectStockMsg(string materialCode)
        {
            StockMsg stockMsg = new StockMsg();
            var selectSQL = $"SELECT {DbTable.F_Material.MATERIAL_STOCK} ," +
                $"{DbTable.F_Material.MATERIAL_AMOUNTED} " +
                $"FROM " +
                $"{DbTable.F_MATERIAL_NAME} " +
                $"WHERE " +
                $"{DbTable.F_Material.MATERIAL_CODE} = '{materialCode}'";
            var ds = SQLServer.ExecuteDataSet(selectSQL);
            if (ds.Tables.Count > 0)
            {
                var dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    stockMsg.ActualStock = int.Parse(dt.Rows[0][0].ToString());
                    stockMsg.UseAmounted = int.Parse(dt.Rows[0][1].ToString());
                }
            }
            return stockMsg;
        }
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
                     $"{DbTable.F_PRODUCT_PACKAGE.SN_OUTTER} like '%{queryFilter}%'";
            }
            LogHelper.Log.Info(selectSQL);
            var ds = SQLServer.ExecuteDataSet(selectSQL);
            if (ds.Tables[0].Rows.Count < 1)
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
                     $"{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE} like '%{queryFilter}%'";
                return SQLServer.ExecuteDataSet(selectSQL);
            }
            return ds;
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
            DataTable dataSourceProductPackage = new DataTable();
            dataSourceProductPackage.Columns.Add(DATA_ORDER);
            dataSourceProductPackage.Columns.Add(OUT_CASE_CODE);
            dataSourceProductPackage.Columns.Add(CASE_PRODUCT_TYPE_NO);
            dataSourceProductPackage.Columns.Add(CASE_STORAGE_CAPACITY);
            dataSourceProductPackage.Columns.Add(CASE_AMOUNTED);
            DataSet ds = new DataSet();
            DbDataReader dataReader = null;
            if (queryFilter == "")
            {
                selectSQL = $"SELECT " +
                $"a.{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE} 箱子编码," +
                $"a.{DbTable.F_PRODUCT_PACKAGE.TYPE_NO} 产品型号," +
                $"COUNT(a.{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE}) 产品实际数量 " +
                $"FROM " +
                $"{DbTable.F_PRODUCT_PACKAGE_NAME}  a " +
                $"WHERE " +
                $"a.{DbTable.F_PRODUCT_PACKAGE.BINDING_STATE} = '1' " +
                $"GROUP BY " +
                $"{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE}," +
                $"{DbTable.F_PRODUCT_PACKAGE.TYPE_NO} ";
            }
            else
            {
                selectSQL = $"SELECT " +
                    $"a.{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE} 箱子编码," +
                    $"a.{DbTable.F_PRODUCT_PACKAGE.TYPE_NO} 产品型号, " +
                    $"COUNT(a.{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE}) 产品实际数量 FROM " +
                    $"{DbTable.F_PRODUCT_PACKAGE_NAME}  a " +
                    $"WHERE " +
                    $"a.{DbTable.F_PRODUCT_PACKAGE.BINDING_STATE} = '1' AND " +
                    $"a.{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE} = '{queryFilter}' " +
                    $"GROUP BY " +
                    $"{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE}," +
                    $"{DbTable.F_PRODUCT_PACKAGE.TYPE_NO} ";
            }
            dataReader = SQLServer.ExecuteDataReader(selectSQL);
            if (!dataReader.HasRows)
            {
                selectSQL = $"SELECT " +
                $"a.{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE} 箱子编码," +
                $"a.{DbTable.F_PRODUCT_PACKAGE.TYPE_NO} 产品型号, " +
                $"COUNT(a.{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE}) 产品实际数量 FROM " +
                $"{DbTable.F_PRODUCT_PACKAGE_NAME}  a " +
                $"WHERE " +
                $"a.{DbTable.F_PRODUCT_PACKAGE.BINDING_STATE} = '1' AND " +
                $"a.{DbTable.F_PRODUCT_PACKAGE.TYPE_NO} = '{queryFilter}' " +
                $"GROUP BY " +
                $"{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE}," +
                $"{DbTable.F_PRODUCT_PACKAGE.TYPE_NO} ";
                dataReader = SQLServer.ExecuteDataReader(selectSQL);
                if (!dataReader.HasRows)
                {
                    ds.Tables.Add(dataSourceProductPackage);
                    return ds;
                }
            }
            int i = 0;
            while (dataReader.Read())
            {
                DataRow dr = dataSourceProductPackage.NewRow();
                var productTypeNo = dataReader[1].ToString();
                dr[DATA_ORDER] = i + 1;
                dr[OUT_CASE_CODE] = dataReader[0].ToString();
                dr[CASE_PRODUCT_TYPE_NO] = productTypeNo;
                dr[CASE_AMOUNTED] = dataReader[2].ToString();
                dr[CASE_STORAGE_CAPACITY] = GetProductStorage(productTypeNo);
                dataSourceProductPackage.Rows.Add(dr);
                i++;
            }
            dataReader.Close();
            LogHelper.Log.Info(selectSQL);
            ds.Tables.Add(dataSourceProductPackage);
            return ds;
        }

        private string GetProductStorage(string typeNo)
        {
            var selectSQL = $"SELECT {DbTable.F_PRODUCT_PACKAGE_STORAGE.STORAGE_CAPACITY} FROM {DbTable.F_PRODUCT_PACKAGE_STORAGE_NAME} WHERE " +
                $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.PRODUCT_TYPE_NO} = '{typeNo}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();
            return "";
        }

        public int DeleteProductPackage(string queryCondition,int state)
        {
            var deleteSQL = $"";
            var delRow = 0;
            if (queryCondition == "")
            {
                deleteSQL = $"DELETE FROM {DbTable.F_PRODUCT_PACKAGE_NAME} WHERE " +
                    $"{DbTable.F_PRODUCT_PACKAGE.BINDING_STATE} = '{state}'";
                delRow = SQLServer.ExecuteNonQuery(deleteSQL);
            }
            else
            {
                deleteSQL = $"DELETE FROM {DbTable.F_PRODUCT_PACKAGE_NAME} WHERE " +
                    $"{DbTable.F_PRODUCT_PACKAGE.TYPE_NO} = '{queryCondition}' " +
                    $"AND " +
                    $"{DbTable.F_PRODUCT_PACKAGE.BINDING_STATE} = '{state}'";
                delRow = SQLServer.ExecuteNonQuery(deleteSQL);
                if (delRow < 1)
                {
                    deleteSQL = $"DELETE FROM {DbTable.F_PRODUCT_PACKAGE_NAME} WHERE " +
                    $"{DbTable.F_PRODUCT_PACKAGE.OUT_CASE_CODE} = '{queryCondition}' " +
                    $"AND " +
                    $"{DbTable.F_PRODUCT_PACKAGE.BINDING_STATE} = '{state}'";
                    delRow = SQLServer.ExecuteNonQuery(deleteSQL);
                }
            }

            if(state == 0)
                LogHelper.Log.Info("【删除抽检数据】" + delRow);
            else if(state == 1)
                LogHelper.Log.Info("【删除打包数据】" + delRow);
            return delRow;
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
                $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.DESCRIBLE} = '{describle}'," +
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

        public int DeleteProcess(string processName)
        {
            //删除工艺
            var deleteSQL = $"DELETE FROM {DbTable.F_TECHNOLOGICAL_PROCESS_NAME} WHERE " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME} = '{processName}'";
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

        public int DeleteTestLimitConfig(string productTypeNo)
        {
            var deleteSQL = "";
            if (productTypeNo == "")
            {
                deleteSQL = $"delete from {DbTable.F_TEST_LIMIT_CONFIG_NAME} ";
            }
            else
            {
                deleteSQL = $"delete from {DbTable.F_TEST_LIMIT_CONFIG_NAME} where {DbTable.F_TEST_LIMIT_CONFIG.TYPE_NO} = '{productTypeNo}'";
            }
            return SQLServer.ExecuteNonQuery(deleteSQL);
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

        public int DeleteTestProgrameVersion(string productTypeNo)
        {
            var deleteSQL = $"";
            if (productTypeNo == "")
            {
                deleteSQL = $"delete from {DbTable.F_TEST_PROGRAME_VERSION_NAME} ";
            }
            else
            {
                deleteSQL = $"delete from {DbTable.F_TEST_PROGRAME_VERSION_NAME} where " +
                    $"{DbTable.F_TEST_PROGRAME_VERSION.TYPE_NO} = '{productTypeNo}'";
            }
            return SQLServer.ExecuteNonQuery(deleteSQL);
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

        #region 品质管理
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
            if (IsExistQuanlityMaterial(mCode))
            {
                //update
                var updateSQL = $"UPDATE {DbTable.F_QUANLITY_MANAGER_NAME} SET " +
                    $"{DbTable.F_QUANLITY_MANAGER.EXCEPT_TYPE} = '{eType}'," +
                    $"{DbTable.F_QUANLITY_MANAGER.STATEMENT_DATE} = '{sDate}'," +
                    $"{DbTable.F_QUANLITY_MANAGER.EXCEPT_STOCK} = '{estock}'," +
                    $"{DbTable.F_QUANLITY_MANAGER.ACTUAL_STOCK} = '{aStock}'," +
                    $"{DbTable.F_QUANLITY_MANAGER.STATION_NAME} = '{station}'," +
                    $"{DbTable.F_QUANLITY_MANAGER.MATERIAL_STATE} = '{state}'," +
                    $"{DbTable.F_QUANLITY_MANAGER.STATEMENT_REASON} = '{reason}'," +
                    $"{DbTable.F_QUANLITY_MANAGER.STATEMENT_USER} = '{user}'," +
                    $"{DbTable.F_QUANLITY_MANAGER.UPDATE_DATE} = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' " +
                    $"WHERE " +
                    $"{DbTable.F_QUANLITY_MANAGER.MATERIAL_CODE} = '{mCode}'";
                LogHelper.Log.Info(insertSQL);
                return SQLServer.ExecuteNonQuery(updateSQL);
            }
            else
            {
                LogHelper.Log.Info(insertSQL);
                return SQLServer.ExecuteNonQuery(insertSQL);
            }
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

        private bool IsExistQuanlityMaterial(string materialCode)
        {
            var selectSQL = $"SELECT * FROM {DbTable.F_QUANLITY_MANAGER_NAME} " +
                $"WHERE {DbTable.F_QUANLITY_MANAGER.MATERIAL_CODE} = '{materialCode}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }

        public int DeleteQuanlityMsg(string materialCode)
        {
            var deleteSQL = $"DELETE FROM {DbTable.F_QUANLITY_MANAGER_NAME} WHERE " +
                $"{DbTable.F_QUANLITY_MANAGER.MATERIAL_CODE} = '{materialCode}'";
            return SQLServer.ExecuteNonQuery(deleteSQL);
        }
        #endregion

        #region 物料库存
        public MaterialStockEnum ModifyMaterialStock(string materialCode,int stock,string describle,string admin)
        {
            var selectSQL = $"SELECT {DbTable.F_Material.MATERIAL_STOCK},{DbTable.F_Material.MATERIAL_AMOUNTED} " +
                $"FROM {DbTable.F_MATERIAL_NAME} " +
                $"WHERE " +
                $"{DbTable.F_Material.MATERIAL_CODE} = '{materialCode}' ";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            var amountedStock = int.Parse(dt.Rows[0][1].ToString());

            if (dt.Rows.Count > 0)
            {
                //物料存在
                selectSQL = $"SELECT * FROM {DbTable.F_MATERIAL_NAME} " +
                $"WHERE " +
                $"{DbTable.F_Material.MATERIAL_CODE} = '{materialCode}' " +
                $"AND " +
                $"{DbTable.F_Material.MATERIAL_STOCK} = '{stock}' " +
                $"AND " +
                $"{DbTable.F_Material.MATERIAL_DESCRIBLE} = '{describle}'";
                dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                if (dt.Rows.Count < 1)
                {
                    //该库存为修改库存
                    //更新前判断修改数量是否合法
                    if (stock < 1)
                        return MaterialStockEnum.STATUS_NOT_ZERO_STOCK;
                    else if (stock < amountedStock)
                        return MaterialStockEnum.STATUS_STOCK_NOT_SMALLER_AMOUNTED;

                    var updateSQL = $"UPDATE {DbTable.F_MATERIAL_NAME} SET " +
                        $"{DbTable.F_Material.MATERIAL_STOCK} = '{stock}'," +
                        $"{DbTable.F_Material.MATERIAL_DESCRIBLE} = '{describle}'," +
                        $"{DbTable.F_Material.MATERIAL_USERNAME} = '{admin}'," +
                        $"{DbTable.F_Material.MATERIAL_UPDATE_DATE} = '{GetDateTimeNow()}'" +
                        $"WHERE " +
                        $"{DbTable.F_Material.MATERIAL_CODE} = '{materialCode}'";
                    var res = SQLServer.ExecuteNonQuery(updateSQL);
                    LogHelper.Log.Info(updateSQL);
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

        #region query function
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

        public string UpdateInsern(string sn)
        {
            var updateSQL = $"update {DbTable.F_BINDING_PCBA_NAME} set  " +
                $"{DbTable.F_BINDING_PCBA.PRODUCT_TYPE_NO} = 'A03' where {DbTable.F_BINDING_PCBA.SN_PCBA} = '017 B19922002104'";
            return SQLServer.ExecuteNonQuery(updateSQL).ToString();
        }
        #endregion

        /// <summary>
        /// 修改产品型号时，更新所有有记录的产品型号
        /// </summary>
        /// <param name="oldTypeNo"></param>
        /// <param name="newTypeNo"></param>
        public void UpdateAllProductTypeNo(string oldTypeNo,string newTypeNo)
        {
            var updateSQL = "";
            var row = 0;
            //var updateSQL = $"UPDATE {DbTable.F_MATERIAL_STATISTICS_NAME} SET " +
            //    $"{DbTable.F_Material_Statistics.PRODUCT_TYPE_NO} = '{newTypeNo}' " +
            //    $"WHERE " +
            //    $"{DbTable.F_Material_Statistics.PRODUCT_TYPE_NO} = '{oldTypeNo}'";
            //var row = SQLServer.ExecuteNonQuery(updateSQL);
            //LogHelper.Log.Info("【产品型号】F_MATERIAL_STATISTICS_NAME更新数量=" + row);

            //updateSQL = $"UPDATE {DbTable.F_PASS_RATE_STATISTICS_NAME} SET " +
            //    $"{DbTable.F_Pass_Rate_Statistics.TYPE_NO} = '{newTypeNo}' " +
            //    $"WHERE " +
            //    $"{DbTable.F_Pass_Rate_Statistics.TYPE_NO} = '{oldTypeNo}'";
            //row = SQLServer.ExecuteNonQuery(updateSQL);
            //LogHelper.Log.Info("【产品型号】F_PASS_RATE_STATISTICS_NAME更新数量=" + row);

            //updateSQL = $"UPDATE {DbTable.F_PRODUCT_CHECK_RECORD_NAME} SET " +
            //    $"{DbTable.F_Product_Check_Record.TYPE_NO} = '{newTypeNo}' " +
            //    $"WHERE " +
            //    $"{DbTable.F_Product_Check_Record.TYPE_NO} = '{oldTypeNo}'";
            //row = SQLServer.ExecuteNonQuery(updateSQL);
            //LogHelper.Log.Info("【产品型号】F_PRODUCT_CHECK_RECORD_NAME 更新数量=" + row);

            updateSQL = $"UPDATE {DbTable.F_PRODUCT_MATERIAL_NAME} SET " +
                $"{DbTable.F_PRODUCT_MATERIAL.TYPE_NO} = '{newTypeNo}' " +
                $"WHERE " +
                $"{DbTable.F_PRODUCT_MATERIAL.TYPE_NO} = '{oldTypeNo}'";
            row = SQLServer.ExecuteNonQuery(updateSQL);
            LogHelper.Log.Info("【产品型号】F_PRODUCT_MATERIAL_NAME 更新数量=" + row);


            //updateSQL = $"UPDATE {DbTable.F_PRODUCT_PACKAGE_NAME} SET " +
            //    $"{DbTable.F_PRODUCT_PACKAGE.TYPE_NO} = '{newTypeNo}' " +
            //    $"WHERE " +
            //    $"{DbTable.F_PRODUCT_PACKAGE.TYPE_NO} = '{oldTypeNo}'";
            //row = SQLServer.ExecuteNonQuery(updateSQL);
            //LogHelper.Log.Info("【产品型号】F_PRODUCT_PACKAGE_NAME 更新数量=" + row);

            //updateSQL = $"UPDATE {DbTable.F_PRODUCT_PACKAGE_STORAGE_NAME} SET " +
            //    $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.PRODUCT_TYPE_NO} = '{newTypeNo}' " +
            //    $"WHERE " +
            //    $"{DbTable.F_PRODUCT_PACKAGE_STORAGE.PRODUCT_TYPE_NO} = '{oldTypeNo}'";
            //row = SQLServer.ExecuteNonQuery(updateSQL);
            //LogHelper.Log.Info("【产品型号】F_PRODUCT_PACKAGE_STORAGE_NAME 更新数量=" + row);

            updateSQL = $"UPDATE {DbTable.F_TECHNOLOGICAL_PROCESS_NAME} SET " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME} = '{newTypeNo}' " +
                $"WHERE " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME} = '{oldTypeNo}'";
            row = SQLServer.ExecuteNonQuery(updateSQL);
            LogHelper.Log.Info("【产品型号】F_TECHNOLOGICAL_PROCESS_NAME 更新数量=" + row);

            //updateSQL = $"UPDATE {DbTable.F_TEST_LIMIT_CONFIG_NAME} SET " +
            //    $"{DbTable.F_TEST_LIMIT_CONFIG.TYPE_NO} = '{newTypeNo}' " +
            //    $"WHERE " +
            //    $"{DbTable.F_TEST_LIMIT_CONFIG.TYPE_NO} = '{oldTypeNo}'";
            //row = SQLServer.ExecuteNonQuery(updateSQL);
            //LogHelper.Log.Info("【产品型号】F_TEST_LIMIT_CONFIG_NAME 更新数量=" + row);

            //updateSQL = $"UPDATE {DbTable.F_TEST_LOG_DATA_NAME} SET " +
            //    $"{DbTable.F_TEST_LOG_DATA.TYPE_NO} = '{newTypeNo}' " +
            //    $"WHERE " +
            //    $"{DbTable.F_TEST_LOG_DATA.TYPE_NO} = '{oldTypeNo}'";
            //row = SQLServer.ExecuteNonQuery(updateSQL);
            //LogHelper.Log.Info("【产品型号】F_TEST_LOG_DATA_NAME 更新数量=" + row);

            //updateSQL = $"UPDATE {DbTable.F_TEST_PROGRAME_VERSION_NAME} SET " +
            //    $"{DbTable.F_TEST_PROGRAME_VERSION.TYPE_NO} = '{newTypeNo}' " +
            //    $"WHERE " +
            //    $"{DbTable.F_TEST_PROGRAME_VERSION.TYPE_NO} = '{oldTypeNo}'";
            //row = SQLServer.ExecuteNonQuery(updateSQL);
            //LogHelper.Log.Info("【产品型号】F_TEST_PROGRAME_VERSION_NAME 更新数量=" + row);

            //updateSQL = $"UPDATE {DbTable.F_TEST_RESULT_NAME} SET " +
            //    $"{DbTable.F_Test_Result.TYPE_NO} = '{newTypeNo}'," +
            //    $"{DbTable.F_Test_Result.PROCESS_NAME} = '{newTypeNo}' " +
            //    $"WHERE " +
            //    $"{DbTable.F_Test_Result.TYPE_NO} = '{oldTypeNo}'";
            //row = SQLServer.ExecuteNonQuery(updateSQL);
            //LogHelper.Log.Info("【产品型号】F_TEST_RESULT_NAME 更新数量=" + row);

            //updateSQL = $"UPDATE {DbTable.F_BINDING_PCBA_NAME} SET " +
            //    $"{DbTable.F_BINDING_PCBA.PRODUCT_TYPE_NO} = '{newTypeNo}' " +
            //    $"WHERE " +
            //    $"{DbTable.F_BINDING_PCBA.PRODUCT_TYPE_NO} = '{oldTypeNo}'";
            //row = SQLServer.ExecuteNonQuery(updateSQL);
            //LogHelper.Log.Info("【产品型号】F_BINDING_PCBA 更新数量=" + row);
        }

        private string SelectCurrentTProcess()
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
    }
}
