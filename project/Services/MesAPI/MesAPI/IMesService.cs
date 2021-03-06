﻿using SwaggerWcf.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;
using MesAPI.Model;

namespace MesAPI
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    [ServiceContract]
    public interface IMesService
    {
        #region 【接口】TestCommunication 测试通讯
        [OperationContract]
        [SwaggerWcfPath("TestCommunication", "测试通讯")]
        [WebInvoke(Method = "GET", UriTemplate = "TestCommunication?value={value}",
            BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string TestCommunication([SwaggerWcfParameter(Description = "传入任意字符串=返回值")]string value);
        #endregion

        //用户信息
        [OperationContract]
        LoginResult Login(string username, string password);

        [OperationContract]
        DataSet GetUserInfo(string username);

        [OperationContract]
        string GetUserID(string username);

        [OperationContract]
        DataSet GetAllUserInfo();

        [OperationContract]
        RegisterResult Register(string username, string pwd, int userType);

        [OperationContract]
        int ModifyUserPassword(string userID,string username,string pwd);

        [OperationContract]
        int DeleteUser(string username);

        //站位信息
        [OperationContract]
        int DeleteStation(string processName, string stationName);

        [OperationContract]
        DataSet SelectStationList(string processName);

        [OperationContract]
        DataSet SelectProcessList();

        [OperationContract]
        int DeleteAllStation(string processName);

        [OperationContract]
        List<Station> InsertStation(List<Station> stationList);

        [OperationContract]
        int SetCurrentProcess(string processName, int state);

        [OperationContract]
        int UpdateProcessOrder(string process, string station, int id,string user);

        //站位接口
        [OperationContract]
        int DeleteAllTypeStation();

        [OperationContract]
        int DeleteTypeStation(string typeNumber);

        [OperationContract]
        DataSet SelectTypeStation(string typeNumber);

        [OperationContract]
        string CommitTypeStation(Dictionary<string, string[]> dctData);

       //测试数据
        [OperationContract]
        DataSet SelectLastTestResultUpper(string sn, string typeNo, string station);

        [OperationContract]
        DataSet SelectTestResultUpper(string sn, string typeNo, string station, bool IsSnFuzzy);

        //物料信息接口
        [OperationContract]
        DataSet SelectMaterial(string codeRID, MaterialStockState stockStateEnum);

        [OperationContract]
        DataSet SelectMaterialPN();
        //[OperationContract]
        //List<MaterialMsg> CommitMaterial(List<MaterialMsg> list);
        [OperationContract]
        int DeleteMaterialBasicMsg(string queryCondition);

        [OperationContract]
        int DeleteQuanlityMsg(List<QuanlityHistory> quanlityList);

        [OperationContract]
        int DeleteMaterial(string materialCode);

        [OperationContract]
        int DeleteAllMaterial();

        [OperationContract]
        List<ProductMaterial> CommitProductMaterial(List<ProductMaterial> pmList);
        [OperationContract]
        DataSet SelectProductMaterial(string queryCondition);

        [OperationContract]
        int DeleteProductMaterial(ProductMaterial material);

        [OperationContract]
        int UpdateMaterialPN(string materialPN, string materialName, string username);

        //物料统计
        [OperationContract]
        [SwaggerWcfPath("InsertMaterialStatistics", "查询上一站位最新记录")]
        [WebInvoke(Method = "GET", UriTemplate = "InsertMaterialStatistics?snInner={snInner}&snOutter={snOutter}&typeNo={typeNo}&materialCode={materialCode}&amount={amount}",
            BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string InsertMaterialStatistics(string snInner, string snOutter, string typeNo, string stationName,
            string materialCode, string amount);

        [OperationContract]
        MaterialResultInfo SelectMaterialBasicMsg(string materialCode, int pageIndex, int pageSize);

        [OperationContract]
        DataSet SelectMaterialDetailMsg(string materialCode);

        //外箱容量
        [OperationContract]
        int CommitProductContinairCapacity(string productTypeNo, string amount,string username,string describle);
        [OperationContract]
        DataSet SelectProductContinairCapacity(string productTypeNo);
        
        [OperationContract]
        DataSet SelectProductBindingState(string sn);

        [OperationContract]
        int DeleteProductContinairCapacity(string productTypeNo);

        [OperationContract]
        int DeleteAllProductContinairCapacity();

        [OperationContract]
        int DeleteProductPackage(string queryCondition, int state);

        #region 【接口】查询已绑定数据
        [OperationContract]
        [SwaggerWcfPath("SelectProductBindingCount", "查询打包产品记录")]
        [WebInvoke(Method = "GET", UriTemplate = "SelectProductBindingCount&casecode={casecode}&bindingState={bindingState}",
            BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataSet SelectProductBindingRecord(string casecode, string bindingState);
        #endregion

        #region 【接口】SelectPackageProduct 查询打包产品记录
        [OperationContract]
        [SwaggerWcfPath("SelectPackageProduct", "查询打包产品记录")]
        [WebInvoke(Method = "POST", UriTemplate = "SelectPackageProduct",
            BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        DataSet SelectPackageProduct(string casecode,string queryFilter, string state,bool IsShowNumber);
        #endregion

        [OperationContract]
        ProgramVersionHistory SelectTestProgrameVersion(string productTypeNo, int pageIndex, int pageSize);

        [OperationContract]
        int DeleteTestProgrameVersion(List<ProgramVersionHistory> programeList);

        [OperationContract]
        TestStandSpecHistory SelectTestLimitConfig(string productTypeNo, int pageIndex, int pageSize);

        [OperationContract]
        int DeleteTestLimitConfig(List<TestStandSpecHistory> specList);

        [OperationContract]
        DataSet SelectTestLogDataDetail(string queryFilter,string startDate, string endDate);

        [OperationContract]
        DataSet SelectTodayTestLogData(string queryFilter, string startTime, string endTime);

        [OperationContract]
        int UpdateQuanlityData(string eType, string mCode, string sDate, string stock, string aStock, string station,
            string state, string reason, string user);

        [OperationContract]
        int UpdateMaterialStateMent(string materialCode, int state);

        [OperationContract]
        QuanlityHistory SelectQuanlityManager(string materialCode, int pageIndex, int pageSize);

        [OperationContract]
        string SelectMaterialName(string materialPN);

        [OperationContract]
        string GetMaterialCode(string materialRID);

        [OperationContract]
        string SelectLastLogTestResult(string productSN);

        [OperationContract]
        DataSet SelectTypeNoList();

        [OperationContract]
        PackageProductHistory SelectPackageStorage(string queryFilter, int pageIndex, int pageSize);

        [OperationContract]
        TestResultHistory SelectTestResultDetail(string querySN, int pageIndex, int pageSize);

        //[OperationContract]
        //DataSet SelectTestResultDetail(string querySN);
        [OperationContract]
        int SelectTestResultLogLatestPage(string queryFilter, string startTime, string endTime);


        [OperationContract]
        DataSet SelectTestResultLogDetail(int pageNumber, int pageSize);

        [OperationContract]
        string DeleteTestLogData(string queryCondition, string startTime, string endTime);

        [OperationContract]
        string GetPCBASn(string sn);

        [OperationContract]
        string GetProductSn(string sn);

        [OperationContract]
        DataSet SelectPackageProductOfCaseCode(string queryFilter, string state, bool IsShowNumber);

        [OperationContract]
        CheckPackageProductHistory SelectPackageProductCheck(string queryFilter, string state, bool IsShowNumber, int pageIndex, int pageSize);

        [OperationContract]
        MaterialStockEnum ModifyMaterialStock(string materialCode, int stock,string describle,string username);

        [OperationContract]
        string UpdateInsern(string sn);

        [OperationContract]
        void UpdateAllProductTypeNo(string oldTypeNo, string newTypeNo);

        [OperationContract]
        int DeleteProcess(string processName);

        [OperationContract]
        List<TestResultHistory> SelectUseAllPcbaSN();

        [OperationContract]
        int DeleteTestLogHistory(string queryStr,string startTime,string endTime);

        [OperationContract]
        int DeleteMaterialBasicHistory(List<MaterialResultInfo> materialList);

        [OperationContract]
        int DeleteProductPackageHistory(List<PackageProductHistory> packageList);

        [OperationContract]
        int DeleteCheckProductPackageHistory(List<CheckPackageProductHistory> packageList);

        [OperationContract]
        TestResultHistory SelectTestResultLogHistory(string querySN, string startTime, string endTime, int pageIndex, int pageSize);

        [OperationContract]
        TestResultHistory SelectTestResultHistory(string querySN, int pageIndex, int pageSize);

        [OperationContract]
        TestResultHistory SelectAllTestResultLogHistory(string querySN, string startTime, string endTime);

        [OperationContract]
        TestStandSpecHistory SelectAllTestLimitConfig(string productTypeNo);

        [OperationContract]
        ProgramVersionHistory SelectAllTestProgrameVersion(string productTypeNo);
    }
}
