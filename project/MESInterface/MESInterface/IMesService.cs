using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using MESInterface.Model;
using System.Data;
using SwaggerWcf.Attributes;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace MESInterface
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    [ServiceContract]
    public interface IMesService
    {
        // TODO: 在此添加您的服务操作
        [OperationContract]
        [SwaggerWcfPath("标题GetData", "方法详细说明")]
        [WebInvoke(Method = "GET", UriTemplate = "GetData?value={value}",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string GetData(int value);

        [OperationContract]
        void InitConnectString();
        //用户信息
        [OperationContract]
        LoginResult Login(string username, string password, LoginUser loginUser);

        [OperationContract]
        QueryResult GetUserInfo(string userName,out DataSet dataSet);

        [OperationContract]
        DataSet GetAllUserInfo();

        [OperationContract]
        RegisterResult Register(string username, string pwd, string phone, string email, LoginUser loginUser);

        /// <returns></returns>
        [OperationContract]
        string FirstCheck(string snInner,string snOutter, string sTypeNumber, string sStationName);

        // [OperationContract]
        //string InsertWIP(string sn, string sTypeNumber, string sStationName, string sTestResult, string sTime);

        //站位信息
        [OperationContract]
        int DeleteStation(string order,string stationName);

        [OperationContract]
        DataSet SelectStation(string stationName, string stationOrder);
        [OperationContract]
        int InsertStation(List<Station> stationList);

        //产品型号
        [OperationContract]
        int DeleteProductTypeNo(string productName);

        [OperationContract]
        int DeleteAllProductTypeNo();

        [OperationContract]
        DataSet SelectProductTypeNo(string productName);

        [OperationContract]
        string CommitProductTypeNo(List<string> list);

        //站位接口
        [OperationContract]
        int DeleteAllTypeStation();

        [OperationContract]
        int DeleteTypeStation(string typeNumber);

        [OperationContract]
        DataSet SelectTypeStation(string typeNumber);

        [OperationContract]
        string CommitTypeStation(Dictionary<string, string[]> dctData);


        //测试数据接口
        [OperationContract]
        string InsertTestResultData(string sn, string typeNo, string station, string dateTime, string result);
        [OperationContract]
        string SelectLastTestResult(string sn, string typeNo, string station);
        [OperationContract]
        DataSet SelectLastTestResultUpper(string sn, string typeNo, string station);

        [OperationContract]
        DataSet SelectTestResultUpper(string sn, string typeNo, string station, bool IsSnFuzzy);

        //物料信息接口
        [OperationContract]
        DataSet SelectMaterial();
        [OperationContract]
        string CommitMaterial(List<MaterialMsg> list);

        [OperationContract]
        int DeleteMaterial(string materialCode);

        [OperationContract]
        string CommitProductMaterial(Dictionary<string, List<string>> keyValuePairs);
        [OperationContract]
        DataSet SelectProductMaterial(string typeNo);

        [OperationContract]
        int DeleteProductMaterial(string typeNo, string materialCode);

        //物料统计
        [OperationContract]
        string InsertMaterialStatistics(string snInner, string snOutter, string typeNo, string stationName, string materialCode, string amount);

        [OperationContract]
        DataSet SelectMaterialStatistics(string typeNo);

        //外箱容量
        [OperationContract]
        int CommitOutCaseBoxStorage(string out_case_code, string amount);
        [OperationContract]
        DataSet SelectOutCaseBoxStorage(string caseCode);

        //成品打包
        [OperationContract]
        int CommitPackageProduct(PackageProduct packageProduct);

        [OperationContract]
        int UpdatePackageProduct(PackageProduct packageProduct);

        [OperationContract]
        DataSet SelectPackageProduct(PackageProduct packageProduct);

        [OperationContract]
        int DeletePackageProduct(PackageProduct packageProduct);
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
}
