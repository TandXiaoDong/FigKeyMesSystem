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
        private static string pcbaSN;//查询PCBA时记录SN

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
            pcbaSN = sn;
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
        public string UpdateMaterialStatistics(string typeNo,string stationName,string materialCode,string amounted,string teamLeader,string admin)
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
        [SwaggerWcfResponse("0X03", "ERROR_NULL_MATERIAL_PN")]
        [SwaggerWcfResponse("0X04", "ERROR_NULL_ACTUAL_MATERIAL_PN")]
        [SwaggerWcfResponse("0X05", "ERROR_BOTH_MATERIAL_PN_IS_NOT_MATCH")]
        [SwaggerWcfResponse("0X06", "ERROR_LAST_MATERIAL_PN_IS_NOT_USED_UP")]
        [SwaggerWcfResponse("0X07", "STATUS_CURRENT_MATERIAL_AMOUNT_END_OF_USE")]
        public string CheckMaterialMatch(string productTypeNo,string materialPN,string actualMaterialPN,string materialCode)
        {
            LogHelper.Log.Info("物料防错 "+productTypeNo+" "+materialPN+" "+actualMaterialPN+" "+materialCode);
            if (string.IsNullOrEmpty(productTypeNo))
                return MaterialStatistics.ConvertCheckMaterialMatch(MaterialCheckMatchReturnCode.ERROR_NULL_PRODUCT_TYPENO);
            if (string.IsNullOrEmpty(materialPN))
                return MaterialStatistics.ConvertCheckMaterialMatch(MaterialCheckMatchReturnCode.ERROR_NULL_MATERIAL_PN);
            if (string.IsNullOrEmpty(actualMaterialPN))
                return MaterialStatistics.ConvertCheckMaterialMatch(MaterialCheckMatchReturnCode.ERROR_NULL_ACTUAL_MATERIAL_PN);
            if (materialPN != actualMaterialPN)
                return MaterialStatistics.ConvertCheckMaterialMatch(MaterialCheckMatchReturnCode.ERROR_BOTH_MATERIAL_PN_IS_NOT_MATCH);

            checkMaterialMatchQueue.Enqueue(new string[] { productTypeNo,materialPN, materialCode });
            return MaterialStatistics.CheckMaterialMatch(checkMaterialMatchQueue);
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
        [SwaggerWcfResponse("数值","更新数据成功，返回更新更新数据")]
        [SwaggerWcfResponse("PARAMS_NOT_LONG_ENOUGH", "数组长度不足")]
        [SwaggerWcfResponse("BINDING_STATE_VALUE_ERROR", "绑定状态值有误，只能是0或1；1-绑定，0-解绑")]
        [SwaggerWcfResponse("STATUS_FAIL", "更新失败，发生异常错误")]
        public string UpdatePackageProductBindingMsg(string outCaseCode,string[] snOutter,string typeNo,string stationName,
            string bindingState,string remark,string teamLeader,string admin)
        {
            foreach (var sn in snOutter)
            {
                string[] array = new string[] { outCaseCode,sn,typeNo,stationName,bindingState,remark,teamLeader,admin};
                updatePackageProductQueue.Enqueue(array);
            }
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
            var count = SQLServer.ExecuteNonQuery(updateSQL);
            if (count == 1)
                return "OK";
            return "FAIL";
        }
        #endregion
    }
}
