using SwaggerWcf.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;
using MesWcfService.Model;

namespace MesWcfService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    [ServiceContract]
    public interface IMesService
    {
        // TODO: 在此添加您的服务操作
        //测试数据接口

        #region 【接口】TestCommunication 测试通讯
        [OperationContract]
        [SwaggerWcfPath("TestCommunication", "测试通讯")]
        [WebInvoke(Method = "GET", UriTemplate = "TestCommunication?value={value}",
            BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string TestCommunication([SwaggerWcfParameter(Description = "传入任意字符串=返回值")]string value);
        #endregion

        #region 【接口】UpdateTestResultData 更新测试数据
        [OperationContract]
        [SwaggerWcfPath("UpdateTestResultData", "更新测试数据")]
        [WebInvoke(Method = "GET", UriTemplate = "UpdateTestResultData?sn={sn}&typeNO={typeNo}&station={station}" +
            "&result={result}&teamLeader={teamLeader}&admin={admin}&joinDateTime={joinDateTime}",
            BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string UpdateTestResultData([SwaggerWcfParameter(Description = "追溯码*")]string sn, 
            [SwaggerWcfParameter(Description = "产品型号*")]string typeNo, 
            [SwaggerWcfParameter(Description = "站位名称*")]string station,  
            [SwaggerWcfParameter(Description = "测试结果*，PASS/FAIL")]string result,
            [SwaggerWcfParameter(Description = "班组长")]string teamLeader,
            [SwaggerWcfParameter(Description = "管理员")]string admin,
            [SwaggerWcfParameter(Description = "连接同步日期")]string joinDateTime);
        #endregion

        #region 【接口】SelectLastTestResult 查询上一站位最新记录
        [OperationContract]
        [SwaggerWcfPath("SelectLastTestResult", "查询上一站位最新记录;测试结果：【成功】返回数组,len = 4;array[0] = sn;array[1] = station;array[2] = testRes;")]
        [WebInvoke(Method = "GET", UriTemplate = "SelectLastTestResult?sn={sn}&station={station}",
            BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json),Description]
        string[] SelectLastTestResult([SwaggerWcfParameter(Description = "追溯码*")]string sn, 
            [SwaggerWcfParameter(Description = "当前站位名称*")]string station);
        #endregion

        #region【接口】UpdateMaterialStatistics 更新物料计数
        [OperationContract]
        [SwaggerWcfPath("UpdateMaterialStatistics", "装配物料统计")]
        [WebInvoke(Method = "GET", UriTemplate = "UpdateMaterialStatistics?typeNo={typeNo}&stationName={stationName}&" +
            "materialCode={materialCode}&amounted={amounted}&teamLeader={teamLeader}&admin={admin}",
            BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string UpdateMaterialStatistics(
            [SwaggerWcfParameter(Description = "产品型号")]string typeNo,
            [SwaggerWcfParameter(Description = "工站名称")]string stationName,
            [SwaggerWcfParameter(Description = "物料编码")]string materialCode,
            [SwaggerWcfParameter(Description = "使用数量")]string amounted,
            [SwaggerWcfParameter(Description = "班组长")]string teamLeader,
            [SwaggerWcfParameter(Description = "管理员")]string admin);
        #endregion

        #region 【接口】物料入库
        [OperationContract]
        [SwaggerWcfPath("CheckMaterialPutStorage", "检查物料状态，1-正常，2-正常使用完，3-强制使用完；2/3都为使用完")]
        [WebInvoke(Method = "GET", UriTemplate = "CheckMaterialPutStorage?materialCode={materialCode}&teamLeader={teamLeader}&admin={admin}", 
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string CheckMaterialPutStorage(
            [SwaggerWcfParameter(Description = "物料编码")]string materialCode,
            [SwaggerWcfParameter(Description = "班组长")]string teamLeader,
            [SwaggerWcfParameter(Description = "管理员")]string admin);
        #endregion

        #region 【接口】查询物料剩余数量
        [OperationContract]
        [SwaggerWcfPath("SelectMaterialSurplusAmount", "查询物料剩余数量")]
        [WebInvoke(Method = "GET", UriTemplate = "SelectMaterialSurplusAmount?materialCode={materialCode}",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string SelectMaterialSurplusAmount(
            [SwaggerWcfParameter(Description = "物料编码")]string materialCode);
        #endregion

        #region 【接口】物料数量防错
        [OperationContract]
        [SwaggerWcfPath("CheckMaterialState","检查物料状态，1-正常，2-正常使用完，3-强制使用完；2/3都为使用完")]
        [WebInvoke(Method = "GET",UriTemplate = "CheckMaterialState?productTypeNo={productTypeNo}&materialCode={materialCode}", BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,ResponseFormat = WebMessageFormat.Json)]
        string CheckMaterialUseState(
            [SwaggerWcfParameter(Description = "产品型号")]string productTypeNo,
            [SwaggerWcfParameter(Description = "物料编码")]string materialCode);
        #endregion

        #region 【接口】物料号防错
        [OperationContract]
        [SwaggerWcfPath("CheckMaterialMatch", "检查物料码是否匹配，0-不匹配；1-匹配")]
        [WebInvoke(Method = "GET", UriTemplate = "CheckMaterialMatch?productTypeNo={productTypeNo}&" +
            "materialPN={materialPN}&actualMaterialPn={actualMaterialPn}&materialCode={materialCode}", 
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string CheckMaterialMatch(
            [SwaggerWcfParameter(Description = "产品型号")]string productTypeNo,
            [SwaggerWcfParameter(Description = "防错的物料号")]string materialPN,
            [SwaggerWcfParameter(Description = "实际扫码的物料号")]string actualMaterialPn,
            [SwaggerWcfParameter(Description = "完整物料编码")]string materialCode);
        #endregion

        #region【接口】UpdatePackageProductBindingMsg 【打包/抽检】添加绑定信息/更新绑定信息                
        [OperationContract]
        [SwaggerWcfPath("UpdatePackageProductBindingMsg", "成品打包/成品抽检-更新数据/绑定/解绑")]
        [WebInvoke(Method = "POST", UriTemplate = "UpdatePackageProductBindingMsg",
            BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string UpdatePackageProductBindingMsg([SwaggerWcfParameter(Description = "箱子编码*")]string outCaseCode,
            [SwaggerWcfParameter(Description = "追溯码*")]string[] snOutter,
            [SwaggerWcfParameter(Description = "产品型号*")]string typeNo,
            [SwaggerWcfParameter(Description = "工序名称*")]string stationName,
            [SwaggerWcfParameter(Description = "绑定或解绑,0-解除绑定,1-添加绑定*")]string bindingState,
            [SwaggerWcfParameter(Description = "备注(解绑时要写明原因)")]string remark,
            [SwaggerWcfParameter(Description = "班组长")]string teamLeader,
            [SwaggerWcfParameter(Description = "管理员")]string admin);
        #endregion

        #region 【接口】 UpdateProgrameVersion 更新测试程序版本号
        [OperationContract]
        [SwaggerWcfPath("UpdateProgrameVersion", "更新测试程序版本号")]
        [WebInvoke(Method = "GET",UriTemplate = "UpdateProgrameVersion?typeNo={typeNo}&stationName={stationName}" +
            "&programePath={programePath}&programeName={programeName}&teamLeader={teamLeader}&admin={admin}",
            BodyStyle = WebMessageBodyStyle.Bare,ResponseFormat = WebMessageFormat.Json,RequestFormat = WebMessageFormat.Json)]
        string UpdateProgrameVersion([SwaggerWcfParameter(Description = "产品型号*")]string typeNo,
            [SwaggerWcfParameter(Description = "工站名称*")]string stationName,
            [SwaggerWcfParameter(Description = "程序路径*")]string programePath,
            [SwaggerWcfParameter(Description = "程序名称*")]string programeName,
            [SwaggerWcfParameter(Description = "班组长")]string teamLeader,
            [SwaggerWcfParameter(Description = "管理员")]string admin);
        #endregion

        #region 【接口】 UpdateLimitConfig 更新limit配置
        [OperationContract]
        [SwaggerWcfPath("UpdateProgrameVersion", "更新SPEC配置信息")]
        [WebInvoke(Method = "GET", UriTemplate = "UpdateLimitConfig?stationName={stationName}&typeNo={typeNo}&" +
            "testItem={testItem}&limit={limit}&teamLeader={teamLeader}&admin={admin}",
            BodyStyle = WebMessageBodyStyle.Bare,RequestFormat = WebMessageFormat.Json,ResponseFormat = WebMessageFormat.Json)]
        string UpdateLimitConfig([SwaggerWcfParameter(Description = "工站名称*")]string stationName,
            [SwaggerWcfParameter(Description = "产品型号*")]string typeNo,
            [SwaggerWcfParameter(Description = "测试项")]string testItem,
            [SwaggerWcfParameter(Description = "limit")]string limit,
            [SwaggerWcfParameter(Description = "班组长")]string teamLeader,
            [SwaggerWcfParameter(Description = "管理员")]string admin);
        #endregion

        #region 【接口】 UpdateTestLog 更新测试台log记录
        [OperationContract]
        [SwaggerWcfPath("UpdateTestLog", "更新测试台log记录")]
        [WebInvoke(Method = "GET", UriTemplate = "UpdateTestLog?typeNo={typeNo}&stationName={stationName}&" +
            "productSN={productSN}&testItem={testItem}&limit={limit}&currentValue={currentValue}&testResult={testResult}&" +
            "teamLeader={teamLeader}&admin={admin}&joinDateTime={joinDateTime}",
            BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string UpdateTestLog(
            [SwaggerWcfParameter(Description = "产品型号")]string typeNo,
            [SwaggerWcfParameter(Description = "工站名称")]string stationName,
            [SwaggerWcfParameter(Description = "产品SN")]string productSN,
            [SwaggerWcfParameter(Description = "测试项")]string testItem,
            [SwaggerWcfParameter(Description = "limit")]string limit,
            [SwaggerWcfParameter(Description = "当前值")]string currentValue,
            [SwaggerWcfParameter(Description = "测试结果")]string testResult,
            [SwaggerWcfParameter(Description = "班组长")]string teamLeader,
            [SwaggerWcfParameter(Description = "管理员")]string admin,
            [SwaggerWcfParameter(Description = "连接同步日期")]string joinDateTime);
        #endregion

        #region 【接口】 SelectCurrentTProcess 查询当前工艺流程
        [OperationContract]
        [SwaggerWcfPath("SelectCurrentTProcess", "查询当前工艺流程")]
        [WebInvoke(Method = "GET", UriTemplate = "SelectCurrentTProcess",BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,ResponseFormat = WebMessageFormat.Json)]
        string SelectCurrentTProcess();
        #endregion

        #region 【接口】 SelectAllTProcess 查询所有工艺流程
        [OperationContract]
        [SwaggerWcfPath("SelectAllTProcess", "查询所有工艺流程")]
        [WebInvoke(Method = "GET", UriTemplate = "SelectAllTProcess", BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string[] SelectAllTProcess();
        #endregion

        #region 【接口】 SelectStationList 查询所有工序列表
        [OperationContract]
        [SwaggerWcfPath("SelectStationList", "查询所有工艺流程")]
        [WebInvoke(Method = "GET", UriTemplate = "SelectStationList?processName={processName}", BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string[] SelectStationList([SwaggerWcfParameter(Description = "工序名称")]string processName);
        #endregion

        #region 【接口】 SelectTypeNoList 查询所有产品型号
        [OperationContract]
        [SwaggerWcfPath("SelectTypeNoList", "查询所有产品型号")]
        [WebInvoke(Method = "GET", UriTemplate = "SelectTypeNoList", BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string[] SelectTypeNoList();
        #endregion

        #region 【接口】 SelectMaterialList 查询所有物料
        [OperationContract]
        [SwaggerWcfPath("SelectMaterialList", "产线所有物料/根据产品型号查询")]
        [WebInvoke(Method = "GET", UriTemplate = "SelectMaterialList?productTypeNo={productTypeNo}", BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string[] SelectMaterialList([SwaggerWcfParameter(Description = "产品型号")]string productTypeNo);
        #endregion

        #region 【接口】 绑定PCBA
        [OperationContract]
        [SwaggerWcfPath("BindingPCBA", "绑定产品PCBA")]
        [WebInvoke(Method = "GET", UriTemplate = "BindingPCBA?snPCBA={snPCBA}&snOutter={snOutter}&materialCode={materialCode}&productTypeNo={productTypeNo}", BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string BindingPCBA([SwaggerWcfParameter(Description = "PCBA")]string snPCBA,
            [SwaggerWcfParameter(Description = "外壳码")]string snOutter,
            [SwaggerWcfParameter(Description = "物料编码")]string materialCode,
            [SwaggerWcfParameter(Description = "产品型号")]string productTypeNo);
        #endregion

        #region 【接口】 SPEC-LIMIT配置
        [OperationContract]
        [SwaggerWcfPath("SelectLimitConfig", "查询SPEC-LIMIT配置")]
        [WebInvoke(Method = "GET", UriTemplate = "SelectLimitConfig?productTypeNo={productTypeNo}" +
            "&stationName={stationName}&item={item}", BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string[] SelectLimitConfig(
            [SwaggerWcfParameter(Description = "产品型号")]string productTypeNo,
            [SwaggerWcfParameter(Description = "站位名称")]string stationName,
            [SwaggerWcfParameter(Description = "ITEM项")]string item);
        #endregion

        #region 【接口】 程序版本查询
        [OperationContract]
        [SwaggerWcfPath("SelectProgrameVersion", "程序版本查询")]
        [WebInvoke(Method = "GET", UriTemplate = "SelectProgrameVersion?productTypeNo={productTypeNo}" +
            "&stationName={stationName}", BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string[] SelectProgrameVersion(
            [SwaggerWcfParameter(Description = "产品型号")]string productTypeNo,
            [SwaggerWcfParameter(Description = "站位名称")]string stationName);
        #endregion

        #region 【接口】 查询产品容量
        [OperationContract]
        [SwaggerWcfPath("SelectPackageStorage", "查询产品容量")]
        [WebInvoke(Method = "GET", UriTemplate = "SelectPackageStorage?productTypeNo={productTypeNo}", BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        int SelectPackageStorage(
            [SwaggerWcfParameter(Description = "产品型号")]string productTypeNo);
        #endregion

        #region 【接口】 更新产品容量
        [OperationContract]
        [SwaggerWcfPath("UpdatePackageStorage", "更新产品容量")]
        [WebInvoke(Method = "GET", UriTemplate = "UpdatePackageStorage?productTypeNo={productTypeNo}&capacity={capacity}", BodyStyle = WebMessageBodyStyle.Bare,
    RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string UpdatePackageStorage(string productTypeNo, int capacity);
        #endregion
    }

    // 使用下面示例中说明的数据约定将复合类型添加到服务操作。
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }

    [DataContract]
    public struct MaterialType
    {
        [DataMember]
        [Description("PCBA")]
        public string MaterialPCBA { get; set; }

        [DataMember]
        [Description("外壳")]
        public string MaterialOutterShell { get; set; }

        [DataMember]
        [Description("产品型号")]
        public string ProductTypeNo { get; set; }

        [DataMember]
        [Description("站位名称")]
        public string StationName { get; set; }

        [DataMember]
        [Description("上盖")]
        public string MaterialTopCover { get; set; }

        [DataMember]
        [Description("上壳")]
        public string MaterialUpperShell { get; set; }

        [DataMember]
        [Description("下壳")]
        public string MaterialLowerShell { get; set; }

        [DataMember]
        [Description("线束")]
        public string MaterialWirebean { get; set; }

        [DataMember]
        [Description("支架板")]
        public string MaterialSupportPlate { get; set; }

        [DataMember]
        [Description("泡棉")]
        public string MaterialBubbleCotton { get; set; }

        [DataMember]
        [Description("临时支架")]
        public string MaterialTempStent { get; set; }

        [DataMember]
        [Description("最终支架")]
        public string MaterialFinalStent { get; set; }

        [DataMember]
        [Description("小螺钉")]
        public string MaterialLittleScrew { get; set; }

        [DataMember]
        [Description("长螺钉")]
        public string MaterialLongScrew { get; set; }

        [DataMember]
        [Description("螺丝/螺母")]
        public string MaterialScrewNut { get; set; }

        [DataMember]
        [Description("防水圈")]
        public string MaterialWaterProofRing { get; set; }

        [DataMember]
        [Description("密封圈")]
        public string MaterialSealRing { get; set; }

        [DataMember]
        [Description("使用数量")]
        public string MaterialUseAmount { get; set; }

        [DataMember]
        [Description("班组长")]
        public string TeamLeader { get; set; }

        [DataMember]
        [Description("管理员")]
        public string Admin { get; set; }
    }
}
