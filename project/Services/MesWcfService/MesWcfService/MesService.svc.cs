using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Data;
using CommonUtils.DB;
using CommonUtils.Logger;
using CommonUtils.CalculateAndString;
using System.Configuration;
using System.Collections;
using MesWcfService.MessageQueue.RemoteClient;
using MesWcfService.DB;
using MesWcfService.Model;
using System.Data.SqlClient;
using SwaggerWcf.Attributes;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace MesWcfService
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
        private Queue<string[]> insertMaterialStatisticsQueue = new Queue<string[]>();
        private Queue<string[]> updateProgrameVersionQueue = new Queue<string[]>();
        private Queue<string[]> updateLimitConfigQueue = new Queue<string[]>();
        private Queue<string[]> updateLogDataQueue = new Queue<string[]>();
        private Queue<string[]> updatePackageProductQueue = new Queue<string[]>();
        private Queue<string[]> checkMaterialStateQueue = new Queue<string[]>();
        private Queue<string[]> checkMaterialMatchQueue = new Queue<string[]>();
        private Queue<string[]> checkMaterialPutInStorageQueue = new Queue<string[]>();
        private Queue<string[]> bindingSnPcbaQueue = new Queue<string[]>();
        private Queue<string[]> selectPVersionQueue = new Queue<string[]>();
        private Queue<string[]> selectSpecLimitQueue = new Queue<string[]>();
        private Queue<string> selectMaterialSurplusQueue = new Queue<string>();
        private Queue<string> selectProductPackageStorageQueue = new Queue<string>();
        private int materialLength = 20;
        //private static string pcbaSN;//查询PCBA时记录SN

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
                return "【SQL Server】"+testRes;
            }
            return value;
        }
        #endregion

        #region 用户信息接口

        #region 用户登录
        [SwaggerWcfTag("MesServcie 服务")]
        [SwaggerWcfResponse("0X00", "FAIL_USER_NAME_ERROR")]//用户名不存在或错误
        [SwaggerWcfResponse("0X01", "FAIL_USER_PASSWORD_ERROR")]//用户密码不存在或错误
        [SwaggerWcfResponse("0X02", "STATUS_SUCCESSFUL")]//登录成功
        [SwaggerWcfResponse("0X03", "FAIL_ERROR")]//捕获未知异常
        [SwaggerWcfResponse("代码解释", "array[0]=状态代码，array[1]=角色(CODE=0X02)")]
        public string[] Login(string username, string password)
        {
            string[] userResult = new string[2];
            try
            {
                string[] userInfo = GetUserInfo(username);
                if (userInfo[0] != "0X01")
                {
                    //用户不存在
                    LogHelper.Log.Info($"用户名{username}不存在，验证失败！");
                    userResult[0] = "0X00";//FAIL_USER_NAME_ERROR
                    return userResult;
                }
                else
                {
                    //用户存在
                    //验证登录密码
                    var selectSQL = $"SELECT {DbTable.F_User.ROLE_NAME} FROM {DbTable.F_USER_NAME} WHERE " +
                        $"{DbTable.F_User.USER_NAME} = '{username}' AND " +
                        $"{DbTable.F_User.PASS_WORD} = '{password}'";
                    DataTable dtRes = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                    if (dtRes.Rows.Count < 1)
                    {
                        //密码验证失败
                        LogHelper.Log.Info($"用户{username}密码验证失败！");
                        userResult[0] = "0X01";//FAIL_USER_PASSWORD_ERROR
                        return userResult;
                    }
                    else
                    {
                        //通过验证
                        LogHelper.Log.Info(username + " 登录进入 " + DateTime.Now);
                        userResult[0] = "0X02";
                        userResult[1] = dtRes.Rows[0][0].ToString();
                        return userResult;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error("用户登录异常..." + ex.Message);
                userResult[0] = "0X03";
                return userResult;
            }
        }
        #endregion

        #region 查询用户信息
        [SwaggerWcfTag("MesServcie 服务")]
        [SwaggerWcfResponse("0X00", "FAIL_QUERY")]//查询失败，传入用户名不存在
        [SwaggerWcfResponse("0X01", "查询成功")]//
        [SwaggerWcfResponse("代码解释", "array[0]=状态码，CODE=0X01时，array[1]=角色，array[2]=用户密码")]
        public string[] GetUserInfo(string username)
        {
            string[] userResult = new string[3];
            var selectSQL = $"SELECT {DbTable.F_User.ROLE_NAME},{DbTable.F_User.PASS_WORD}" +
                $" FROM {DbTable.F_USER_NAME} " +
                     $"WHERE {DbTable.F_User.USER_NAME} = '{username}'";
            var ds = SQLServer.ExecuteDataSet(selectSQL);
            if (ds.Tables.Count > 0)
            {
                var dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    userResult[0] = "0X01";
                    userResult[1] = ds.Tables[0].Rows[0][0].ToString();
                    userResult[2] = ds.Tables[0].Rows[0][1].ToString();
                    return userResult;
                }
            }
            userResult[0] = "0X00";
            return userResult;
        }
        #endregion

        #region 查询所有用户
        [SwaggerWcfTag("MesServcie 服务")]
        private DataSet GetAllUserInfo()
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
        [SwaggerWcfTag("MesServcie 服务")]
        [SwaggerWcfResponse("0X00", "FAIL_ERROR_SQL")]
        [SwaggerWcfResponse("0X01", "STATUS_SUCCESS")]//注册成功
        [SwaggerWcfResponse("0X02", "FAIL_USER_EXIST")]//用户已经存在，请勿重复注册
        [SwaggerWcfResponse("0X03", "FAIL_ERROR")]
        public string[] Register(string username, string pwd, int userType)
        {
            string[] registerResult = new string[1];
            try
            {
                string[] queryResult = GetUserInfo(username);
                if (queryResult[0] == "0X01")
                {
                    LogHelper.Log.Info("【用户已存在】"+username);
                    registerResult[0] = "0X02";
                    return registerResult;
                }
                else
                {
                    //用户不存在，可以注册
                    string dateTimeNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string insertString = $"INSERT INTO {DbTable.F_USER_NAME}" +
                        $"({DbTable.F_User.USER_NAME}," +
                        $"{DbTable.F_User.PASS_WORD} ," +
                        $"{DbTable.F_User.UPDATE_DATE} ," +
                        $"{DbTable.F_User.ROLE_NAME}) " +
                        $"VALUES('{username}', '{pwd}', '{dateTimeNow}','{userType}')";
                    int executeResult = SQLServer.ExecuteNonQuery(insertString);
                    if (executeResult < 1)
                    {
                        registerResult[0] = "0X00";
                        LogHelper.Log.Error("【注册用户】失败"+insertString);
                        return registerResult;
                    }
                    registerResult[0] = "0X01";
                    return registerResult;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error("注册失败..." + ex.Message);
                registerResult[0] = "0X03";
                return registerResult;
            }
        }
        #endregion

        #region 修改密码
        [SwaggerWcfTag("MesServcie 服务")]
        [SwaggerWcfResponse("0X00", "FAIL_USER_NOT_EXIST")]//要修改的用户不存在
        [SwaggerWcfResponse("0X01", "STATUS_SUCCESS")]//修改密码成功
        [SwaggerWcfResponse("0X02", "FAIL_MODIFY")]//修改密码失败
        public string[] ModifyUserPassword(string username, string pwd)
        {
            string[] userResult = GetUserInfo(username);
            string[] statusResult = new string[2];
            if (userResult[0] == "0X01")
            {
                //用户存在
                var updateSQL = $"UPDATE {DbTable.F_USER_NAME} SET " +
                            $"{DbTable.F_User.PASS_WORD} = '{pwd}'," +
                            $"{DbTable.F_User.UPDATE_DATE} = '{GetDateTimeNow()}' " +
                            $"WHERE {DbTable.F_User.USER_NAME} = '{username}'";
                var row = SQLServer.ExecuteNonQuery(updateSQL);
                if(row > 0)
                {
                    statusResult[0] = "0X01";
                    return statusResult;
                }
                statusResult[0] = "0X02";
                return statusResult;
            }
            else
            {
                //用户不存在
                statusResult[0] = "0X00";
                return statusResult;
            }
        }
        #endregion

        #region 删除用户
        [SwaggerWcfTag("MesServcie 服务")]
        [SwaggerWcfResponse("0X00", "FAIL_USER_NOT_EXIST")]//要删除的用户不存在
        [SwaggerWcfResponse("0X01", "STATUS_SUCCESS")]//删除成功
        [SwaggerWcfResponse("0X02", "FAIL_DELETE")]//删除失败
        public string[] DeleteUser(string username)
        {
            string[] statusResult = new string[1];
            var userResult = GetUserInfo(username);
            if (userResult[0] != "0X01")
            {
                statusResult[0] = "0X00";
                return statusResult;
            }
            var deleteSQL = $"DELETE FROM {DbTable.F_USER_NAME} WHERE {DbTable.F_User.USER_NAME} = '{username}'";
            var row = SQLServer.ExecuteNonQuery(deleteSQL);
            if (row > 0)
            {
                //删除成功
                statusResult[0] = "0X01";
                return statusResult;
            }
            statusResult[0] = "0X02";
            return statusResult;
        }
        #endregion

        #endregion

        #region 更新测试结果
        /*
         * 测试结果要记录进站时间与出站时间
         * 进站时间：开始测试的时间，查询上一工位测试结果为PASS的时间
         * 出站时间：测试完成的时间
         * 流程：
         * 1）新增接口记录第一个工站开始的时间，更新进站日期
         * 2）传入测试数据结果的接口用于更新出站时间
         * 3）第1个工站之后的所有工站的进站时间为该工站查询上一工站返回结果为PASS的时间
         */
        [SwaggerWcfTag("MesServcie 服务")]
        [SwaggerWcfResponse("0X00", "STATUS_SUCCESS")]
        [SwaggerWcfResponse("0X01", "ERROR_FAIL")]
        [SwaggerWcfResponse("0X02", "ERROR_SN_IS_NULL")]
        [SwaggerWcfResponse("0X03", "ERROR_STATION_IS_NULL")]
        [SwaggerWcfResponse("0X04", "ERROR_FIRST_STATION")]
        [SwaggerWcfResponse("0X05", "ERROR_RESULT_IS_NULL")]
        [SwaggerWcfResponse("0X06", "ERROR_JOINT_TIME_IS_NULL")]
        [SwaggerWcfResponse(HttpStatusCode.Unused)]
        public string UpdateTestResultData(string sn, string typeNo, string station,string result,string teamLeader, string admin,string joinDateTime)
        {
            string[] array = new string[] { sn, typeNo, station,result,teamLeader,admin, joinDateTime };
            insertDataQueue.Enqueue(array);
            return TestResult.CommitTestResult(insertDataQueue);
        }
        #endregion

        #region 查询测试结果
        [SwaggerWcfTag("MesServcie 服务")]
        [SwaggerWcfResponse("ERR_LAST_STATION_ID","查询上一站位失败")]
        [SwaggerWcfResponse("ERR_LAST_STATION_NAME","查询上一站位ID失败")]
        [SwaggerWcfResponse("QUERY_NONE", "未查询到结果")]
        [SwaggerWcfResponse("ERR_EXCEPTION", "异常错误")]
        public string[] SelectLastTestResult(string sn,string station)
        {
            string[] array = new string[] { sn,station};
            //pcbaSN = sn;
            selectDataQueue.Enqueue(array);
            return TestResult.SelectTestResult(selectDataQueue);
        }
        #endregion

        #region 更新物料统计表
        [SwaggerWcfTag("MesServcie 服务")]
        [SwaggerWcfResponse("0X00", "STATUS_FAIL")]
        [SwaggerWcfResponse("0X01", "STATUS_USCCESS")]
        [SwaggerWcfResponse("0X02", "ERROR_IS_NULL_TYPNO")]
        [SwaggerWcfResponse("0X03", "ERROR_IS_NULL_STATION_NAME")]
        [SwaggerWcfResponse("0X04", "ERROR_IS_NULL_MATERIAL_CODE")]
        [SwaggerWcfResponse("0X05", "ERROR_IS_NULL_AMOUNTED")]
        [SwaggerWcfResponse("0X06", "ERROR_USE_AMOUNT_NOT_INT")]
        [SwaggerWcfResponse("0X07", "ERROR_NOT_MATCH_MATERIAL_PN")]
        [SwaggerWcfResponse("0X08", "ERROR_NOT_AMOUNT_STATE")]
        [SwaggerWcfResponse("0X09", "STATUS_NOT_PUT_IN")]
        public string UpdateMaterialStatistics(string typeNo,string stationName,string materialCode,string amounted,string teamLeader,string admin,string pcbaSN)
        {
            if (string.IsNullOrEmpty(typeNo))
                return MaterialStatistics.ConvertMaterialStatisticsCode(MaterialStatisticsReturnCode.ERROR_IS_NULL_TYPNO);
            if (string.IsNullOrEmpty(stationName))
                return MaterialStatistics.ConvertMaterialStatisticsCode(MaterialStatisticsReturnCode.ERROR_IS_NULL_STATION_NAME);
            if (string.IsNullOrEmpty(materialCode))
                return MaterialStatistics.ConvertMaterialStatisticsCode(MaterialStatisticsReturnCode.ERROR_IS_NULL_MATERIAL_CODE);
            if (!ExamineInputFormat.IsDecimal(amounted))
                return MaterialStatistics.ConvertMaterialStatisticsCode(MaterialStatisticsReturnCode.ERROR_USE_AMOUNT_NOT_INT);
            insertMaterialStatisticsQueue.Enqueue(new string[] { typeNo,stationName,materialCode,amounted,teamLeader,admin,pcbaSN});
            return MaterialStatistics.UpdateMaterialStatistics(insertMaterialStatisticsQueue);
        }
        #endregion

        #region 查询物料剩余数量
        [SwaggerWcfTag("MesServcie 服务")]
        public string SelectMaterialSurplusAmount(string materialCode)
        {
            if (string.IsNullOrEmpty(materialCode))
                return "";
            selectMaterialSurplusQueue.Enqueue(materialCode);
            return MaterialStatistics.SelectMaterialSurplus(selectMaterialSurplusQueue);
        }
        #endregion

        #region 物料数量防错
        [SwaggerWcfTag("MesServcie 服务")]
        [SwaggerWcfResponse("0X00", "STATUS_OTHER_COMPLETE")]
        [SwaggerWcfResponse("0X01", "STATUS_USING")]
        [SwaggerWcfResponse("0X02", "STATUS_COMPLETE_NORMAL")]
        [SwaggerWcfResponse("0X03", "STATUS_COMPLETE_UNUSUAL")]
        [SwaggerWcfResponse("0X04", "ERROR_NULL_PRODUCT_TYPENO")]
        [SwaggerWcfResponse("0X05", "ERROR_NULL_MATERIAL_CODE")]
        [SwaggerWcfResponse("0X06", "STATUS_NULL_QUERY")]
        [SwaggerWcfResponse("0X07", "ERROR_FORMAT_MATERIAL_CODE")]
        [SwaggerWcfResponse("0X08", "ERROR_MATRIAL_CODE_IS_NOT_MATCH_WITH_PRODUCT_TYPENO")]
        public string CheckMaterialUseState(string productTypeNo, string materialCode)
        {
            if (string.IsNullOrEmpty(materialCode))
                return MaterialStatistics.ConvertCheckMaterialStateCode(MaterialStateReturnCode.ERROR_NULL_MATERIAL_CODE);
            if (string.IsNullOrEmpty(productTypeNo))
                return MaterialStatistics.ConvertCheckMaterialStateCode(MaterialStateReturnCode.ERROR_NULL_PRODUCT_TYPENO);

            checkMaterialStateQueue.Enqueue(new string[] { productTypeNo,materialCode});
            return MaterialStatistics.CheckMaterialState(checkMaterialStateQueue);
        }
        #endregion

        #region 物料号防错
        [SwaggerWcfTag("MesServcie 服务")]
        [SwaggerWcfResponse("0X00", "IS_NOT_MATCH")]
        [SwaggerWcfResponse("0X01", "IS_MATCH")]
        [SwaggerWcfResponse("0X02", "ERROR_NULL_PRODUCT_TYPENO")]
        [SwaggerWcfResponse("0X03", "ERROR_NULL_MATERIAL_PN")]//已去掉，多余
        [SwaggerWcfResponse("0X04", "ERROR_NULL_ACTUAL_MATERIAL_PN")]
        [SwaggerWcfResponse("0X05", "ERROR_BOTH_MATERIAL_PN_IS_NOT_MATCH")]
        [SwaggerWcfResponse("0X06", "ERROR_LAST_MATERIAL_PN_IS_NOT_USED_UP")]
        [SwaggerWcfResponse("0X07", "STATUS_CURRENT_MATERIAL_AMOUNT_END_OF_USE")]
        public string CheckMaterialMatch(string productTypeNo,string testMaterialPN,string actualMaterialPN,string materialCode)
        {
            LogHelper.Log.Info($"物料防错 "+productTypeNo+ $" testMaterialPN={testMaterialPN} actualMaterialPN={actualMaterialPN} " +" "+materialCode);
            if (string.IsNullOrEmpty(productTypeNo))
                return MaterialStatistics.ConvertCheckMaterialMatch(MaterialCheckMatchReturnCode.ERROR_NULL_PRODUCT_TYPENO);
            if (string.IsNullOrEmpty(actualMaterialPN))
                return MaterialStatistics.ConvertCheckMaterialMatch(MaterialCheckMatchReturnCode.ERROR_NULL_ACTUAL_MATERIAL_PN);
            //var materialPNDataSet = SelectMaterialPNOfProductTypeNo(productTypeNo);
            //if (materialPNDataSet.Tables.Count > 0)
            //{
            //    var dataRowList = materialPNDataSet.Tables[0].Select($"material_code = '{actualMaterialPN}'");
            //    if(dataRowList.Length < 1)
            //        return MaterialStatistics.ConvertCheckMaterialMatch(MaterialCheckMatchReturnCode.ERROR_BOTH_MATERIAL_PN_IS_NOT_MATCH);
            //}
            if(testMaterialPN == "")
                return MaterialStatistics.ConvertCheckMaterialMatch(MaterialCheckMatchReturnCode.ERROR_NULL_MATERIAL_PN);
            if (testMaterialPN != actualMaterialPN)
                return MaterialStatistics.ConvertCheckMaterialMatch(MaterialCheckMatchReturnCode.ERROR_BOTH_MATERIAL_PN_IS_NOT_MATCH);
            checkMaterialMatchQueue.Enqueue(new string[] { productTypeNo, actualMaterialPN, materialCode });
            return MaterialStatistics.CheckMaterialMatch(checkMaterialMatchQueue);
        }

        private DataSet SelectMaterialPNOfProductTypeNo(string productTypeNo)
        {
            var selectPn = $"select {DbTable.F_PRODUCT_MATERIAL.MATERIAL_CODE} from {DbTable.F_PRODUCT_MATERIAL_NAME} " +
                $"where {DbTable.F_PRODUCT_MATERIAL.TYPE_NO} = '{productTypeNo}'";
            return SQLServer.ExecuteDataSet(selectPn);
        }

        #endregion

        #region 物料入库
        [SwaggerWcfTag("MesServcie 服务")]
        [SwaggerWcfResponse("0X00", "STATUS_IS_NOT_PUT_IN_STORAGE")]
        [SwaggerWcfResponse("0X01", "STATUS_IS_PUTED_IN_STORAGE")]
        [SwaggerWcfResponse("0X02", "STATUS_IS_NEW_PUT_INT_STORAGE")]
        [SwaggerWcfResponse("0X03", "STATUS_IS_PUT_IN_FAIL_STORAGE")]
        [SwaggerWcfResponse("0X04", "ERROR_MATERIAL_CODE_IS_NULL")]
        [SwaggerWcfResponse("0X05", "ERROR_MATERIAL_CODE_FORMAT_NOT_RIGHT")]
        public string CheckMaterialPutStorage(string materialCode,string teamLeader,string admin)
        {
            if (string.IsNullOrEmpty(materialCode))
                return MaterialStatistics.ConvertCheckMaterialPutInStorage(MaterialCheckPutStorageEnum.ERROR_MATERIAL_CODE_IS_NULL);
            if (!materialCode.Contains("&"))
                return MaterialStatistics.ConvertCheckMaterialPutInStorage(MaterialCheckPutStorageEnum.ERROR_MATERIAL_CODE_FORMAT_NOT_RIGHT);

            checkMaterialPutInStorageQueue.Enqueue(new string[] {materialCode,teamLeader,admin });
            return MaterialStatistics.CheckMaterialPutInStorage(checkMaterialPutInStorageQueue);
        }

        #endregion

        #region 成品打包接口/成品抽检-更新绑定信息
        [SwaggerWcfTag("MesServcie 服务")]
        [SwaggerWcfResponse("0X01", "STATUS_FULL")] //该产品已放满
        [SwaggerWcfResponse("0X02", "STATUS_BINDED_OTHER_CASE")]  //该产品已绑定其他箱子
        [SwaggerWcfResponse("0X03", "STATUS_EXIST_BINDED_UPDATE_SUCCESS")] //已经存在绑定记录-更新成功
        [SwaggerWcfResponse("0X04", "STATUS_EXIST_BINDED_UPDATE_FAIL")]    //已经存在绑定记录-更新失败
        [SwaggerWcfResponse("0X05", "STATUS_NONE_EXIST_UNBIND_UPDATE_SUCCESS")] //不存在绑定记录--解除绑定--更新成功
        [SwaggerWcfResponse("0X06", "STATUS_NONE_EXIST_UNBIND_UPDATE_FAIL")]    //不存在绑定记录--解除绑定--更新失败
        [SwaggerWcfResponse("0X07", "STATUS_NONE_EXIST_REBIND_SUCCESS")]    //不存在绑定记录-重新绑定-更新成功
        [SwaggerWcfResponse("0X08", "STATUS_NONE_EXIST_REBIND_FAIL")]       //不存在绑定记录-重新绑定-更新失败
        [SwaggerWcfResponse("0X09", "STATUS_INSERT_SUCCESS")]               //插入成功，可以继续
        [SwaggerWcfResponse("0X10", "STATUS_INSERT_FAIL")]                  //插入失败
        [SwaggerWcfResponse("0X11", "STATUS_EXCEPT_FAIL")]                  //异常
        [SwaggerWcfResponse("0X12", "STATUS_NONE_EXIST_BINDING_STATE")]     //传入绑定状态无效0/1
        [SwaggerWcfResponse("0X13", "STATUS_INSERT_BINDING_STATE_INVALID")] //插入绑定产品，传入绑定状态无效
        [SwaggerWcfResponse("0X14", "STATUS_NONE_OUTCASE_CODE")] //传入箱子编码为空
        [SwaggerWcfResponse("0X15", "STATUS_NONE_SN")] //传入产品SN为空
        [SwaggerWcfResponse("0X16", "STATUS_NONE_TYPE_NO")] //传入产品型号为空
        public string[] UpdatePackageProductBindingMsg(string outCaseCode,string snOutter,string typeNo,string stationName,
            string bindingState,string remark,string teamLeader,string admin)
        {
            //1）判断当前箱子是否装满
            //2）已经绑定的产品，从另一个已经绑定其他箱子取出来绑定
            //3）当产品装满后，将剩余的产品返回
            //foreach (var sn in snOutter)
            //{
                
            //}
            string[] array = new string[] { outCaseCode.Trim(), snOutter.Trim(), typeNo.Trim(), stationName.Trim(), bindingState.Trim(), remark.Trim(), teamLeader.Trim(), admin.Trim() };
            updatePackageProductQueue.Enqueue(array);
            return UPackageProduct.PackageProductMsg(updatePackageProductQueue);
        }
        #endregion

        #region UpdateProgrameVersion 更新程序版本
        [SwaggerWcfTag("MesServcie 服务")]
        [SwaggerWcfResponse("OK", "更新测试程序版本成功")]
        [SwaggerWcfResponse("FAIL", "更新测试程序版本失败")]
        [SwaggerWcfResponse("ERROR", "异常错误")]
        public string UpdateProgrameVersion(string typeNo,string stationName,string programePath,
            string programeName,string teamLeader,string admin)
        {
            string[] array = new string[] { typeNo, stationName, programePath, programeName, teamLeader, admin };
            updateProgrameVersionQueue.Enqueue(array);
            return ProgrameVersion.UpdateProgrameVersion(updateProgrameVersionQueue);
        }
        #endregion

        #region 更新LIMIT配置
        [SwaggerWcfTag("MesServcie 服务")]
        [SwaggerWcfResponse("OK", "更新LIMIT配置成功")]
        [SwaggerWcfResponse("FAIL", "更新LIMIT配置失败")]
        [SwaggerWcfResponse("ERROR", "异常错误")]
        public string UpdateLimitConfig(string stationName,string typeNo,string testItem,string limit,string teamLeader,string admin)
        {
            string[] array = new string[] { stationName,typeNo,testItem,limit,teamLeader,admin};
            updateLimitConfigQueue.Enqueue(array);
            return LimitConfig.UpdateLimitConfig(updateLimitConfigQueue);
        }
        #endregion

        #region 更新测试log数据
        [SwaggerWcfTag("MesServcie 服务")]
        [SwaggerWcfResponse("OK", "更新LIMIT配置成功")]
        [SwaggerWcfResponse("FAIL", "更新LIMIT配置失败")]
        [SwaggerWcfResponse("ERROR", "异常错误")]
        public string UpdateTestLog(string typeNo, string stationName,string productSN, 
            string testItem,string limit,string currentValue,string testResult, string teamLeader, string admin,string joinDateTime)
        {
            typeNo = typeNo.Trim();
            stationName = stationName.Trim();
            productSN = productSN.Trim();
            if (typeNo == "")
                return "product typeno is null";
            if (stationName == "")
                return "stationName is null";
            if (productSN == "")
                return "product sn is null";
            string[] array = new string[] {typeNo,stationName,productSN,testItem,limit,currentValue,testResult,teamLeader,admin, joinDateTime };
            updateLogDataQueue.Enqueue(array);
            return TestLogData.UpdateTestLogData(updateLogDataQueue);
        }
        #endregion

        #region 查询当前工艺流程
        [SwaggerWcfTag("MesServcie 服务")]
        [SwaggerWcfResponse("NULL","查询结果为空！")]
        public string SelectCurrentTProcess()
        {
            string selectSQL = $"SELECT DISTINCT {DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME} " +
                $"FROM {DbTable.F_TECHNOLOGICAL_PROCESS_NAME} WHERE {DbTable.F_TECHNOLOGICAL_PROCESS.PSTATE} = '1'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            LogHelper.Log.Info("【查询当前工艺流程】"+selectSQL);
            if (dt.Rows.Count > 0)
            {
                var currentProcess = dt.Rows[0][0].ToString();
                return currentProcess;
            }
            else
                return "NULL";
        }
        #endregion

        #region 查询所有工艺流程
        [SwaggerWcfTag("MesServcie 服务")]
        [SwaggerWcfResponse("NULL", "查询结果为空！")]
        public string[] SelectAllTProcess()
        {
            string selectSQL = $"SELECT DISTINCT {DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME} FROM " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS_NAME}";
            CheckProductProcess();
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                string[] arrayRes = new string[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    arrayRes[i] = dt.Rows[i][0].ToString();
                }
                return arrayRes;
            }
            return new string[] { "NULL"};
        }

        /// <summary>
        /// 检查工艺与产品是否匹配
        /// </summary>
        private void CheckProductProcess()
        {
            var selectProcessSQL = $"select distinct {DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME} from {DbTable.F_TECHNOLOGICAL_PROCESS_NAME}";
            var dt = SQLServer.ExecuteDataSet(selectProcessSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var productTypeNo = dr[0].ToString();
                    selectProcessSQL = $"select * from {DbTable.F_OUT_CASE_STORAGE_NAME} where " +
                        $"{DbTable.F_Out_Case_Storage.TYPE_NO} = '{productTypeNo}'";
                    dt = SQLServer.ExecuteDataSet(selectProcessSQL).Tables[0];
                    if (dt.Rows.Count < 1)
                    {
                        var deleteSQL = $"delete from {DbTable.F_TECHNOLOGICAL_PROCESS_NAME} where " +
                            $"{DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME} = '{productTypeNo}'";
                        var delRow = SQLServer.ExecuteNonQuery(deleteSQL);
                        LogHelper.Log.Info($"【删除工艺】删除不存在工艺,工艺名称={productTypeNo}");
                    }
                }
            }
        }
        #endregion

        #region 查询所有工序列表
        [SwaggerWcfTag("MesServcie 服务")]
        [SwaggerWcfResponse("NULL", "查询结果为空！请检查传入参数是否正确")]
        public string[] SelectStationList(string processName)
        {
            var selectSQL = $"SELECT {DbTable.F_TECHNOLOGICAL_PROCESS.STATION_NAME} FROM " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS_NAME} WHERE " +
                $"{DbTable.F_TECHNOLOGICAL_PROCESS.PROCESS_NAME} = '{processName}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                string[] arrayRes = new string[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    arrayRes[i] = dt.Rows[i][0].ToString();
                }
                return arrayRes;
            }
            return new string[] { "NULL"};
        }
        #endregion

        #region 查询所有产品型号
        [SwaggerWcfTag("MesServcie 服务")]
        [SwaggerWcfResponse("NULL", "查询结果为空！请检查传入参数是否正确")]
        public string[] SelectTypeNoList()
        {
            var selectSQL = $"SELECT {DbTable.F_Out_Case_Storage.TYPE_NO} FROM {DbTable.F_OUT_CASE_STORAGE_NAME} ";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                string[] array = new string[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    array[i] = dt.Rows[i][0].ToString();
                }
                return array;
            }
            return new string[] { "NULL"};
        }
        #endregion

        #region 查询所有物料
        [SwaggerWcfTag("MesServcie 服务")]
        [SwaggerWcfResponse("NULL", "查询结果为空！请检查传入参数是否正确")]
        public string[] SelectMaterialList(string productTypeNo)
        {
            var selectSQL = $"SELECT {DbTable.F_PRODUCT_MATERIAL.MATERIAL_CODE} FROM {DbTable.F_PRODUCT_MATERIAL_NAME} WHERE " +
                $"{DbTable.F_PRODUCT_MATERIAL.TYPE_NO} = '{productTypeNo}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                string[] array = new string[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    array[i] = dt.Rows[i][0].ToString();
                }
                return array;
            }
            return new string[] { "NULL" };
        }
        #endregion

        #region PCBA与外壳与其他物料的绑定
        [SwaggerWcfTag("MesServcie 服务")]
        [SwaggerWcfResponse("OK", "外壳与PCBA等绑定成功")]
        [SwaggerWcfResponse("FAIL", "外壳与PCBA等绑定失败")]
        public string BindingPCBA(string snPCBA,string snOutter,string materialCode,string productTypeNo)
        {
            bindingSnPcbaQueue.Enqueue(new string[] { snPCBA,snOutter,materialCode,productTypeNo});
            return AddBindingPCBA.BindingPCBA(bindingSnPcbaQueue);
        }
        #endregion

        #region 查询LIMIT
        [SwaggerWcfTag("MesServcie 服务")]
        public string[] SelectLimitConfig(string productTypeNo, string stationName,string item)
        {
            selectSpecLimitQueue.Enqueue(new string[] { productTypeNo,stationName,item });
            return SelectLastTestConfig.SelectSpecLimit(selectSpecLimitQueue);
        }
        #endregion

        #region 查询程序版本
        [SwaggerWcfTag("MesServcie 服务")]
        public string[] SelectProgrameVersion(string productTypeNo, string stationName)
        {
            selectPVersionQueue.Enqueue(new string[] { productTypeNo, stationName});
            return SelectLastTestConfig.SelectPVersion(selectPVersionQueue);
        }
        #endregion

        #region 查询成品装箱配置的容量
        [SwaggerWcfTag("MesServcie 服务")]
        public int SelectPackageStorage(string productTypeNo)
        {
            selectProductPackageStorageQueue.Enqueue(productTypeNo);
            return UPackageProduct.SelectPackageStorage(selectProductPackageStorageQueue);
        }
        [SwaggerWcfTag("MesServcie 服务")]
        public string UpdatePackageStorage(string productTypeNo,int capacity)
        {
            var updateSQL = $"UPDATE {DbTable.F_OUT_CASE_STORAGE_NAME} SET " +
                $"{DbTable.F_Out_Case_Storage.STORAGE_CAPACITY} = '{capacity}' " +
                $"WHERE " +
                $"{DbTable.F_Out_Case_Storage.TYPE_NO} = '{productTypeNo}'";
            LogHelper.Log.Info("【更新产品容量】"+updateSQL);
            var count = SQLServer.ExecuteNonQuery(updateSQL);
            if (count > 0)
                return "OK";
            return "FAIL";
        }
        #endregion

        #region pcba解绑与查询
        [SwaggerWcfTag("MesServcie 服务")]
        public PCBABindHistory QueryPCBAMes(string sn,int pageIndex,int pageSize)
        {
            DataSet ds = new DataSet();

            #region init datatable
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("序号");
            dataTable.Columns.Add("产品型号");
            dataTable.Columns.Add("当前工位");
            dataTable.Columns.Add("PCBASN");
            dataTable.Columns.Add("外壳SN");
            dataTable.Columns.Add("绑定状态");
            dataTable.Columns.Add("PCBA状态");
            dataTable.Columns.Add("外壳状态");
            dataTable.Columns.Add("绑定日期");
            #endregion

            PCBABindHistory pcbaBindHistory = new PCBABindHistory();
            var selectSQL = $"select distinct " +
                $"{DbTable.F_BINDING_PCBA.PRODUCT_TYPE_NO} 产品型号," +
                $"{DbTable.F_BINDING_PCBA.SN_PCBA} PCBASN," +
                $"{DbTable.F_BINDING_PCBA.SN_OUTTER} 外壳SN," +
                $"{DbTable.F_BINDING_PCBA.BINDING_STATE}," +
                $"{DbTable.F_BINDING_PCBA.PCBA_STATE}," +
                $"{DbTable.F_BINDING_PCBA.OUTTER_STATE} " +
                $"from " +
                $"{DbTable.F_BINDING_PCBA_NAME}  " +
                $"where " +
                $"{DbTable.F_BINDING_PCBA.SN_PCBA} like '%{sn}%' " +
                $"or " +
                $"{DbTable.F_BINDING_PCBA.SN_OUTTER} like '%{sn}%'";
            //LogHelper.Log.Info("【查询PCBA】"+selectSQL);
            System.Data.Common.DbDataReader dbReader = SQLServer.ExecuteDataReader(selectSQL);
            if (!dbReader.HasRows)
            {
                ds.Tables.Add(dataTable);
                pcbaBindHistory.BindNumber = 0;
                pcbaBindHistory.BindHistoryData = ds;
                return pcbaBindHistory;
            }
            int id = 0;
            int i = 0;
            int startIndex = (pageIndex - 1) * pageSize;
            while (dbReader.Read())
            {
                dbReader[0].ToString();
                if (i >= startIndex && i < pageIndex * pageSize)
                {
                    DataRow dr = dataTable.NewRow();
                    dr["序号"] = id + 1;
                    dr["产品型号"] = dbReader[0].ToString();
                    dr["当前工位"] = "外壳装配工位";
                    var snPCBA = dbReader[1].ToString();
                    var outterSn = dbReader[2].ToString();
                    dr["PCBASN"] = snPCBA;
                    dr["外壳SN"] = outterSn;
                    dr["绑定日期"] = SelectBindingDate(snPCBA, outterSn);
                    var bindingState = dbReader[3].ToString();
                    if (bindingState == "1")
                        dr["绑定状态"] = "已绑定";
                    else if (bindingState == "0")
                        dr["绑定状态"] = "已解除绑定";
                    var pcbaState = dbReader[4].ToString();
                    if (pcbaState == "0")
                        dr["PCBA状态"] = "异常";
                    else if (pcbaState == "1")
                        dr["PCBA状态"] = "正常";
                    var outterState = dbReader[5].ToString();
                    if (outterState == "0")
                        dr["外壳状态"] = "异常";
                    else if (outterState == "1")
                        dr["外壳状态"] = "正常";
                    dataTable.Rows.Add(dr);
                    id++;
                }
                i++;
            }
            ds.Tables.Add(dataTable);
            pcbaBindHistory.BindHistoryData = ds;
            pcbaBindHistory.BindNumber = i;
            return pcbaBindHistory;
        }

        private string SelectBindingDate(string pcbaSn,string outterSn)
        {
            var selectSQL = $"select top 1 {DbTable.F_BINDING_PCBA.UPDATE_DATE} " +
                $"from {DbTable.F_BINDING_PCBA_NAME} " +
                $"where {DbTable.F_BINDING_PCBA.SN_PCBA}='{pcbaSn}' " +
                $"and " +
                $"{DbTable.F_BINDING_PCBA.SN_OUTTER}='{outterSn}' " +
                $"order by {DbTable.F_BINDING_PCBA.UPDATE_DATE} desc";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();
            return "";
        }

        [SwaggerWcfTag("MesServcie 服务")]
        public bool UpdatePcbaBindingState(string pcbaSn,string outterSn,int bindingState,int pcbaState,int outterState)
        {
            /*
             * 更新PCBA与外壳状态
             * 解绑的三种情况：
             * 1）PCBA异常：更新所有PCBA状态异常
             * 2）外壳异常：更新所有外壳状态
             * 3）PCBA与外壳都异常：更新所有PCBA与所有外壳异常状态
             */
            var updateBindState = "";//更新绑定状态
            var updateState = "";//更新PCBA/外壳状态
            if (pcbaState == 0 && outterState == 1)
            {
                updateState = $"update {DbTable.F_BINDING_PCBA_NAME} SET " +
                    $"{DbTable.F_BINDING_PCBA.PCBA_STATE} = '0' " +
                    $"WHERE {DbTable.F_BINDING_PCBA.SN_PCBA} = '{pcbaSn}'";
            }
            else if (pcbaState == 1 && outterState == 0)
            {
                updateState = $"update {DbTable.F_BINDING_PCBA_NAME} SET " +
                  $"{DbTable.F_BINDING_PCBA.OUTTER_STATE} = '0' " +
                  $"WHERE {DbTable.F_BINDING_PCBA.SN_OUTTER} = '{outterSn}'";
            }
            else if (pcbaState == 0 && outterState == 0)
            {
                updateState = $"update {DbTable.F_BINDING_PCBA_NAME} SET " +
                    $"{DbTable.F_BINDING_PCBA.OUTTER_STATE} = '0' " +
                    $"WHERE " +
                    $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{outterSn}'";
                SQLServer.ExecuteNonQuery(updateState);
                updateState = $"update {DbTable.F_BINDING_PCBA_NAME} SET " +
                    $"{DbTable.F_BINDING_PCBA.PCBA_STATE} = '0' " +
                    $"WHERE " +
                    $"{DbTable.F_BINDING_PCBA.SN_PCBA} = '{pcbaSn}'";
            }
            updateBindState = $"update {DbTable.F_BINDING_PCBA_NAME} SET " +
                    $"{DbTable.F_BINDING_PCBA.BINDING_STATE} = '{bindingState}' " +
                    $"WHERE {DbTable.F_BINDING_PCBA.SN_PCBA} = '{pcbaSn}' " +
                    $"AND {DbTable.F_BINDING_PCBA.SN_OUTTER} =  '{outterSn}'";
            LogHelper.Log.Info("【更新解除绑定状态】updateBindState=" + updateBindState);
            LogHelper.Log.Info("【更新解除绑定状态】updateState=" + updateState);
            var bindRow = SQLServer.ExecuteNonQuery(updateBindState);
            var stateRow = SQLServer.ExecuteNonQuery(updateState);
            if (bindRow > 0 && stateRow > 0)
            {
                //state update successful
                return true;
            }
            //update failed
            return false;
        }

        [SwaggerWcfTag("MesServcie 服务")]
        public bool UpdatePCBABindingRepaireState(string pcbaSn, string outterSn, int bindingState, int pcbaState, int outterState)
        {
            /*
             * 更新PCBA与外壳状态
             * 解绑的三种情况：
             * 1）PCBA异常：更新所有PCBA状态异常
             * 2）外壳异常：更新所有外壳状态
             * 3）PCBA与外壳都异常：更新所有PCBA与所有外壳异常状态
             */
            try
            {
                var bindStateUpdate = "";
                var stateUpdate = "";
                if (pcbaState == 1 && outterState == 0)
                {
                    stateUpdate = $"update {DbTable.F_BINDING_PCBA_NAME} SET " +
                        $"{DbTable.F_BINDING_PCBA.PCBA_STATE} = '1' " +
                        $"WHERE {DbTable.F_BINDING_PCBA.SN_PCBA} = '{pcbaSn}'";
                }
                else if (pcbaState == 0 && outterState == 1)
                {
                    stateUpdate = $"update {DbTable.F_BINDING_PCBA_NAME} SET " +
                        $"{DbTable.F_BINDING_PCBA.OUTTER_STATE} = '1' " +
                        $"WHERE {DbTable.F_BINDING_PCBA.SN_OUTTER} = '{outterSn}'";
                }
                else if (pcbaState == 1 && outterState == 1)
                {
                    stateUpdate = $"update {DbTable.F_BINDING_PCBA_NAME} SET " +
                        $"{DbTable.F_BINDING_PCBA.OUTTER_STATE} = '1' " +
                        $"WHERE " +
                        $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{outterSn}'";
                    SQLServer.ExecuteNonQuery(stateUpdate);
                    stateUpdate = $"update {DbTable.F_BINDING_PCBA_NAME} SET " +
                        $"{DbTable.F_BINDING_PCBA.PCBA_STATE} = '1' " +
                        $"WHERE " +
                        $"{DbTable.F_BINDING_PCBA.SN_PCBA} = '{pcbaSn}'";
                }
                bindStateUpdate = $"update {DbTable.F_BINDING_PCBA_NAME} SET " +
                        $"{DbTable.F_BINDING_PCBA.BINDING_STATE} = '{bindingState}' " +
                        $"WHERE {DbTable.F_BINDING_PCBA.SN_PCBA} = '{pcbaSn}' " +
                        $"AND {DbTable.F_BINDING_PCBA.SN_OUTTER} = '{outterSn}'";
                LogHelper.Log.Info("【PCBA绑定-维修完成】bindStateUpdate=" + bindStateUpdate);
                LogHelper.Log.Info("【PCBA绑定-维修完成】stateUpdate=" + stateUpdate);
                var bindRow = SQLServer.ExecuteNonQuery(bindStateUpdate);
                var stateRow = SQLServer.ExecuteNonQuery(stateUpdate);
                if (bindRow > 0 && stateRow > 0)
                {
                    //state update successful
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error(ex.Message+ex.StackTrace);
            }
            //update failed
            return false;
        }
        #endregion

        #region check pcba state
        [SwaggerWcfTag("MesServcie 服务")]
        [SwaggerWcfResponse("array", "array[0]=状态结果代码，array[1]=具体值，当前只有0X07才有值")]
        [SwaggerWcfResponse("0X01", "STATUS_BINDED")]//传入的外壳已绑定 -- 可以继续
        [SwaggerWcfResponse("0X02", "STATUS_UNBIND_SHELL_EXCEPT")]//传入的外壳已解绑---此外壳异常---不能继续使用
        [SwaggerWcfResponse("0X03", "STATUS_UNBIND_PCBA_EXCEPT")]//传入的外壳已解绑---此PCBA异常--不能继续使用
        [SwaggerWcfResponse("0X04", "STATUS_UNDIND_PCBA_SHELL_EXCEPT")]//传入的外壳已解绑---此外壳与PCBA都异常---不能继续使用
        [SwaggerWcfResponse("0X05", "STATUS_PCBA_SHELL_REPAIRE_COMPLETE")]//已解绑，但是PCBA与外壳都修复好
        [SwaggerWcfResponse("0X06", "FAIL_UNKNOWN_ERROR")]//未知异常
        [SwaggerWcfResponse("0X07", "STATUS_BINDED_SHELL_FOR_OTHER")]//传入的PCBA已绑定其他外壳，未绑定当前外壳 ---可以继续
        [SwaggerWcfResponse("0X08", "STATUS_NONE_BINDING")]//传入的PCBA未绑定任何外壳----可以继续
        [SwaggerWcfResponse("0X09", "STATUS_EXECUTE_SQL_EXCEPT")]//执行SQL异常
        [SwaggerWcfResponse("0X10", "FAIL_PCBA_NULL")]//传入PCBA为空--不能继续
        ////一下两种情况传入参数PCBA不为空，外壳为空
        [SwaggerWcfResponse("0X11", "STATUS_PCBA_UNBIND_EXCEPT")]//PCBA已解绑，且已异常
        [SwaggerWcfResponse("0X12", "STATUS_PCBA_NORMAL")]// PCBA正常（可能解绑，也可能是绑定，但未品质正常）
        [SwaggerWcfResponse("0X13", "STATUS_NONE_BINDING_SHELL_EXCEPT")]//外壳异常
        [SwaggerWcfResponse("0X14", "STATUS_SHELL_BINDED_OTHER_PCBA")]//该外壳有绑定其他PCBA
        public string[] CheckPcbaState(string snPcba,string snOutter)
        {
            /*
             * 传入外壳的三种情况：
             * 1）新的外壳：
             * 2）旧的外壳
             * 3）坏的外壳
             * 验证流程：
             * 1）pcba与外壳查询是否绑定
             *  1-绑定：查询结果已绑定，该外壳为旧外壳
             *  2-未绑定/解绑：两者无绑定记录，无解绑记录；
             *              判定为新外壳，查询pcba是否绑定其他外壳
             *              1-绑定其他外壳：返回绑定的外壳SN
             *              2-也未绑定其他外壳：该PCBA未绑定任何外壳
             *  3-无记录            
             *              
             */
            #region sql
            var selectPcba = $"select TOP 1 " +
                $"{DbTable.F_BINDING_PCBA.BINDING_STATE}," +
                $"{DbTable.F_BINDING_PCBA.PCBA_STATE}," +
                $"{DbTable.F_BINDING_PCBA.OUTTER_STATE} " +
                $"from " +
                $"{DbTable.F_BINDING_PCBA_NAME} " +
                $"where " +
                $"{DbTable.F_BINDING_PCBA.SN_PCBA} ='{snPcba}' " +
                $"and " +
                $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{snOutter}'";
            #endregion
            var bindingState = "1";
            var pcbaState = "1";
            var outterState = "1";
            string[] arrayList = new string[2];
            try
            {
                if (snPcba == "")
                {
                    arrayList[0] = "0X10";
                    LogHelper.Log.Info($"【查询PCBA绑定状态】"+arrayList[0]);
                    return arrayList;
                }
                if (snPcba != "" && snOutter == "")
                {
                    //查询PCBA的状态
                    var selectPCBA = $"SELECT {DbTable.F_BINDING_PCBA.BINDING_STATE}," +
                        $"{DbTable.F_BINDING_PCBA.PCBA_STATE} " +
                        $"FROM {DbTable.F_BINDING_PCBA_NAME} " +
                        $"WHERE " +
                        $"{DbTable.F_BINDING_PCBA.SN_PCBA} = '{snPcba}' " +
                        $"AND {DbTable.F_BINDING_PCBA.BINDING_STATE} = '0' " +
                        $"AND {DbTable.F_BINDING_PCBA.PCBA_STATE} = '0'";
                    var dtPcba = SQLServer.ExecuteDataSet(selectPCBA).Tables[0];
                    if (dtPcba.Rows.Count > 0)
                    {
                        //PCBA已解绑，且已异常
                        arrayList[0] = "0X11";//STATUS_PCBA_UNBIND_EXCEPT
                        LogHelper.Log.Info($"【查询PCBA绑定状态】" + arrayList[0]);
                        return arrayList;
                    }
                    arrayList[0] = "0X12";//STATUS_PCBA_NORMAL
                    LogHelper.Log.Info($"【查询PCBA绑定状态】" + arrayList[0]);
                    return arrayList;
                }
                var dt = SQLServer.ExecuteDataSet(selectPcba).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    //有绑定记录：已绑定/已解绑
                    //已绑定-旧外壳
                    //已解绑-坏外壳
                    bindingState = dt.Rows[0][0].ToString();
                    pcbaState = dt.Rows[0][1].ToString();
                    outterState = dt.Rows[0][2].ToString();
                    if (bindingState == "1")
                    {
                        //已绑定 STATUS_BINDED
                        arrayList[0] = "0X01";
                        LogHelper.Log.Info($"【查询PCBA绑定状态】" + arrayList[0]);
                        return arrayList;
                    }
                    else if (bindingState == "0")
                    {
                        //已解绑
                        if (outterState == "0" && pcbaState == "1")
                        {
                            //外壳坏了 STATUS_UNBIND_SHELL_EXCEPT
                            arrayList[0] = "0X02";
                            LogHelper.Log.Info($"【查询PCBA绑定状态】" + arrayList[0]);
                            return arrayList;
                        }
                        else if (outterState == "1" && pcbaState == "0")
                        {
                            //PCBA坏了 STATUS_UNBIND_PCBA_EXCEPT
                            arrayList[0] = "0X03";
                            LogHelper.Log.Info($"【查询PCBA绑定状态】" + arrayList[0]);
                            return arrayList;
                        }
                        else if (outterState == "0" && pcbaState == "0")
                        {
                            //PCBA与外壳都坏了，STATUS_UNDIND_PCBA_SHELL_EXCEPT
                            arrayList[0] = "0X04";
                            LogHelper.Log.Info($"【查询PCBA绑定状态】" + arrayList[0]);
                            return arrayList;
                        }
                        else
                        {
                            //已解绑，但是PCBA与外壳都修复好 STATUS_PCBA_SHELL_REPAIRE_COMPLETE
                            arrayList[0] = "0X05";
                            LogHelper.Log.Info($"【查询PCBA绑定状态】" + arrayList[0]);
                            return arrayList;
                        }
                    }
                    else
                    {
                        //未知错误 FAIL_UNKNOWN_ERROR
                        arrayList[0] = "0X06";
                        LogHelper.Log.Info($"【查询PCBA绑定状态】" + arrayList[0]);
                        return arrayList;
                    }
                }
                else
                {
                    //无绑定记录，判定为新外壳，查询pcba是否绑定其他外壳
                    var selectOther = $"SELECT {DbTable.F_BINDING_PCBA.SN_OUTTER} FROM {DbTable.F_BINDING_PCBA_NAME} " +
                        $"WHERE " +
                        $"{DbTable.F_BINDING_PCBA.SN_PCBA} = '{snPcba}' " +
                        $"AND {DbTable.F_BINDING_PCBA.BINDING_STATE} = '1'";
                    var dtOther = SQLServer.ExecuteDataSet(selectOther).Tables[0];
                    if (dtOther.Rows.Count > 0)
                    {
                        //已绑定其他外壳 STATUS_BINDED_SHELL_FOR_OTHER
                        var otherShell = dtOther.Rows[0][0].ToString();
                        arrayList[0] = "0X07";
                        arrayList[1] = otherShell;
                        LogHelper.Log.Info($"【查询PCBA绑定状态】" + arrayList[0]);
                        return arrayList;
                    }
                    else
                    {
                        //无查询记录，未绑定任何外壳 
                        //进一步判断传入外壳是否异常
                        var selectShell = $"SELECT {DbTable.F_BINDING_PCBA.BINDING_STATE}," +
                        $"{DbTable.F_BINDING_PCBA.PCBA_STATE} " +
                        $"FROM {DbTable.F_BINDING_PCBA_NAME} " +
                        $"WHERE " +
                        $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{snOutter}' " +
                        $"AND {DbTable.F_BINDING_PCBA.OUTTER_STATE} = '0'";
                        var shelldt = SQLServer.ExecuteDataSet(selectShell).Tables[0];
                        if (shelldt.Rows.Count > 0)
                        {
                            //外壳异常
                            arrayList[0] = "0X13";//STATUS_NONE_BINDING_SHELL_EXCEPT
                            LogHelper.Log.Info($"【查询PCBA绑定状态】" + arrayList[0]);
                            return arrayList;
                        }
                        //外壳正常
                        //在判断此外壳是否有绑定其他PCBA
                        var selectOtherPcba = $"SELECT * " +
                        $"FROM {DbTable.F_BINDING_PCBA_NAME} " +
                        $"WHERE " +
                        $"{DbTable.F_BINDING_PCBA.SN_OUTTER} = '{snOutter}' " +
                        $"AND " +
                        $"{DbTable.F_BINDING_PCBA.BINDING_STATE} = '1'";
                        var dtotherPcba = SQLServer.ExecuteDataSet(selectOtherPcba).Tables[0];
                        if (dtotherPcba.Rows.Count > 0)
                        {
                            //该外壳有绑定其他PCBA
                            arrayList[0] = "0X14"; //STATUS_SHELL_BINDED_OTHER_PCBA
                            LogHelper.Log.Info($"【查询PCBA绑定状态】" + arrayList[0]);
                            return arrayList;
                        }
                        arrayList[0] = "0X08"; //STATUS_NONE_BINDING_NORMAL
                        LogHelper.Log.Info($"【查询PCBA绑定状态】" + arrayList[0]);
                        return arrayList;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error(ex.Message+ex.StackTrace);
                LogHelper.Log.Info($"【查询PCBA绑定状态】" + arrayList[0]);
                arrayList[0] = "0X09";//STATUS_EXECUTE_SQL_EXCEPT
                return arrayList;
            }
        }
        #endregion
    }
}
