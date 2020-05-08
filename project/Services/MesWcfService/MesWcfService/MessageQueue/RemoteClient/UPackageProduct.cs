using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MesWcfService.DB;
using MesWcfService.Model;
using CommonUtils.Logger;
using CommonUtils.DB;

namespace MesWcfService.MessageQueue.RemoteClient
{
    public class UPackageProduct
    {
        private bool IsRecordExist;
        private bool IsProductBinding;
        private bool IsExistOtherBindingRecord;
        private string bindedOtherOutcasecode;


        public enum ProductPackageEnum
        {
            PARAMS_NOT_LONG_ENOUGH,
            BINDING_STATE_VALUE_ERROR,
            STATUS_FAIL
        }

        /// <summary>
        /// 更新数据前check箱子装入总数是否超过总容量，更新成功后，同时更新箱子计数
        /// </summary>
        /// <param name="ppQueue"></param>
        /// <returns></returns>
        public static string UpdatePackageProduct(Queue<string[]> ppQueue)
        {
            try
            {
                var array = ppQueue.Dequeue();
                if (array.Length < 8)
                    return ProductPackageEnum.PARAMS_NOT_LONG_ENOUGH.ToString();

                var outCaseCode = array[0];
                var snOutter = array[1];
                var typeNo = array[2];
                var stationName = array[3];
                var bindingState = array[4];
                var remark = array[5];
                var teamdLeader = array[6];
                var admin = array[7];

                string insertSQL = $"INSERT INTO {DbTable.F_OUT_CASE_PRODUCT_NAME}(" +
                    $"{DbTable.F_Out_Case_Product.OUT_CASE_CODE}," +
                    $"{DbTable.F_Out_Case_Product.SN_OUTTER}," +
                    $"{DbTable.F_Out_Case_Product.TYPE_NO}," +
                    $"{DbTable.F_Out_Case_Product.STATION_NAME}," +
                    $"{DbTable.F_Out_Case_Product.BINDING_STATE}," +
                    $"{DbTable.F_Out_Case_Product.REMARK}," +
                    $"{DbTable.F_Out_Case_Product.TEAM_LEADER}," +
                    $"{DbTable.F_Out_Case_Product.ADMIN}," +
                    $"{DbTable.F_Out_Case_Product.BINDING_DATE}) VALUES(" +
                    $"'{outCaseCode}','{snOutter}','{typeNo}','{stationName}'," +
                    $"'{bindingState}','{remark}','{teamdLeader}','{admin}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";

                string updateSQL = $"UPDATE {DbTable.F_OUT_CASE_PRODUCT_NAME} SET " +
                $"{DbTable.F_Out_Case_Product.TYPE_NO} = '{typeNo}'," +
                $"{DbTable.F_Out_Case_Product.BINDING_STATE} = '{bindingState}'," +
                $"{DbTable.F_Out_Case_Product.REMARK} = '{remark}'," +
                $"{DbTable.F_Out_Case_Product.TEAM_LEADER} = '{teamdLeader}'," +
                $"{DbTable.F_Out_Case_Product.ADMIN} = '{admin}'," +
                $"{DbTable.F_Out_Case_Product.UPDATE_DATE} = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' " +
                $"WHERE {DbTable.F_Out_Case_Product.OUT_CASE_CODE} = '{outCaseCode}' AND " +
                $"{DbTable.F_Out_Case_Product.SN_OUTTER} = '{snOutter}' ";

                int res = 0;
                var upackageProduct = IsExist(outCaseCode, snOutter);
                if (upackageProduct.IsRecordExist)
                {
                    //update
                    //是否绑定
                    res = SQLServer.ExecuteNonQuery(updateSQL);
                    if (upackageProduct.IsProductBinding)
                    {
                        //已绑定
                        if (bindingState == "0")
                        {
                            UpdateBindingAmount(typeNo, int.Parse(bindingState));
                            //将解绑数据添加到记录
                            InsertProductCheckRecord(outCaseCode,snOutter,typeNo,stationName,bindingState,remark,teamdLeader,admin);
                        }
                    }
                    else
                    {
                        //未绑定
                        if(bindingState == "1")
                            UpdateBindingAmount(typeNo, int.Parse(bindingState));
                    }
                }
                else
                {
                    //insert
                    //未满，继续
                    if (!IsTypeNoExist(typeNo))
                        return "ERROR_NOT_TYPENO";
                    if (IsContinue(typeNo))
                    {
                        res = SQLServer.ExecuteNonQuery(insertSQL);
                        if (res > 0)
                        {
                            //更新计数
                            UpdateBindingAmount(typeNo,int.Parse(bindingState));
                        }
                    }
                    else
                    {
                        return "FULL";
                    }
                }
                if (res > 0)
                    return "OK";
                return "FAIL";
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error(ex.Message);
                return "ERROR";
            }
        }

        private static UPackageProduct IsExist(string caseCode,string snOutter)
        {
            var selectSQL = $"SELECT {DbTable.F_Out_Case_Product.BINDING_STATE} " +
                $"FROM {DbTable.F_OUT_CASE_PRODUCT_NAME} WHERE " +
                $"{DbTable.F_Out_Case_Product.OUT_CASE_CODE} = '{caseCode}' AND " +
                $"{DbTable.F_Out_Case_Product.SN_OUTTER} = '{snOutter}'";
            LogHelper.Log.Info(selectSQL);
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            UPackageProduct uPackageProduct = new UPackageProduct();
            if (dt.Rows.Count > 0)
            {
                uPackageProduct.IsRecordExist = true;
                var value = dt.Rows[0][0].ToString();
                if (value == "0")
                    uPackageProduct.IsProductBinding = false;
                else if (value == "1")
                    uPackageProduct.IsProductBinding = true;
            }
            else
                uPackageProduct.IsRecordExist = false;
            return uPackageProduct;
        }

        //是否可以继续更新数据，未装满时可以继续，装满时不能继续，提示装满
        private static bool IsTypeNoExist(string typeNo)
        {
            var selectSQL = $"SELECT {DbTable.F_Out_Case_Storage.STORAGE_CAPACITY}," +
                    $"{DbTable.F_Out_Case_Storage.AMOUNTED} " +
                    $"FROM {DbTable.F_OUT_CASE_STORAGE_NAME} WHERE " +
                    $"{DbTable.F_Out_Case_Storage.TYPE_NO} = '{typeNo}'";
            LogHelper.Log.Info(selectSQL);
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count < 1)
            {
                //型号不存在
                return false;
            }
            return true;
        }

        private static bool IsContinue(string typeNo)
        {
            var selectSQL = $"SELECT {DbTable.F_Out_Case_Storage.STORAGE_CAPACITY}," +
                    $"{DbTable.F_Out_Case_Storage.AMOUNTED} " +
                    $"FROM {DbTable.F_OUT_CASE_STORAGE_NAME} WHERE " +
                    $"{DbTable.F_Out_Case_Storage.TYPE_NO} = '{typeNo}'";
            LogHelper.Log.Info(selectSQL);
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                var storage = int.Parse(dt.Rows[0][0].ToString());
                var amounted = int.Parse(dt.Rows[0][1].ToString());
                if (amounted >= storage)
                    return false;
            }
            return true;
        }

        private static void UpdateBindingAmount(string typeNo,int bindingState)
        {
            int value = 0;
            if (bindingState == 0)
                value = -1;
            else if (bindingState == 1)
                value = 1;
            var updateSQL = $"UPDATE {DbTable.F_OUT_CASE_STORAGE_NAME} SET " +
                $"{DbTable.F_Out_Case_Storage.AMOUNTED} += {value} WHERE " +
                $"{DbTable.F_Out_Case_Storage.TYPE_NO} = '{typeNo}'";
            SQLServer.ExecuteNonQuery(updateSQL);
        }

        /// <summary>
        /// 记录抽检时，不合格数据
        /// </summary>
        private static void InsertProductCheckRecord(string outCaseCode,string snOutter,string typeNo,string stationName,
            string bindingState,string remark,string teamdLeader,string admin)
        {
            string insertSQL = $"INSERT INTO {DbTable.F_PRODUCT_CHECK_RECORD_NAME}(" +
                    $"{DbTable.F_Out_Case_Product.OUT_CASE_CODE}," +
                    $"{DbTable.F_Out_Case_Product.SN_OUTTER}," +
                    $"{DbTable.F_Out_Case_Product.TYPE_NO}," +
                    $"{DbTable.F_Out_Case_Product.STATION_NAME}," +
                    $"{DbTable.F_Out_Case_Product.BINDING_STATE}," +
                    $"{DbTable.F_Out_Case_Product.REMARK}," +
                    $"{DbTable.F_Out_Case_Product.TEAM_LEADER}," +
                    $"{DbTable.F_Out_Case_Product.ADMIN}," +
                    $"{DbTable.F_Out_Case_Product.BINDING_DATE}) VALUES(" +
                    $"'{outCaseCode}','{snOutter}','{typeNo}','{stationName}'," +
                    $"'{bindingState}','{remark}','{teamdLeader}','{admin}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
            SQLServer.ExecuteNonQuery(insertSQL);
        }

        public static string[] PackageProductMsg(Queue<string[]> ppQueue)
        {
            string[] resultArray = new string[2];
            try
            {
                //2）已经绑定的产品，从另一个已经绑定其他箱子取出来绑定
                //3）当产品装满后，将剩余的产品返回
                int count = 0; //已插入的数量/已重新绑定的数量
                while (ppQueue.Count > 0)
                {
                    var array = ppQueue.Dequeue();
                    var outCaseCode = array[0];
                    //if (outCaseCode == "")
                    //{
                    //    resultArray[0] = "0X14";//STATUS_NONE_OUTCASE_CODE
                    //    LogHelper.Log.Info($"【更新产品打包】0X14 STATUS_NONE_OUTCASE_CODE");
                    //    return resultArray;
                    //}
                    var snOutter = array[1];
                    if (snOutter == "")
                    {
                        resultArray[0] = "0X15";//STATUS_NONE_SN_OUTTER
                        LogHelper.Log.Info($"【更新产品打包】0X15 STATUS_NONE_SN_OUTTER");
                        return resultArray;
                    }
                    var typeNo = array[2];
                    //if (typeNo == "")
                    //{
                    //    resultArray[0] = "0X16";//STATUS_NONE_TYPE_NO
                    //    LogHelper.Log.Info($"【更新产品打包】0X16 STATUS_NONE_TYPE_NO");
                    //    return resultArray;
                    //}
                    var stationName = array[3];
                    var bindingState = array[4];
                    if (bindingState != "0" && bindingState != "1")
                    {
                        resultArray[0] = "0X12";//STATUS_NONE_EXIST_BINDING_STATE
                        LogHelper.Log.Info($"【更新产品打包】0X12 STATUS_NONE_EXIST_BINDING_STATE");
                        return resultArray;
                    }
                    var remark = array[5];
                    var teamdLeader = array[6];
                    var admin = array[7];

                    #region SQL 
                    string insertSQL = $"INSERT INTO {DbTable.F_OUT_CASE_PRODUCT_NAME}(" +
                        $"{DbTable.F_Out_Case_Product.OUT_CASE_CODE}," +
                        $"{DbTable.F_Out_Case_Product.SN_OUTTER}," +
                        $"{DbTable.F_Out_Case_Product.TYPE_NO}," +
                        $"{DbTable.F_Out_Case_Product.STATION_NAME}," +
                        $"{DbTable.F_Out_Case_Product.BINDING_STATE}," +
                        $"{DbTable.F_Out_Case_Product.REMARK}," +
                        $"{DbTable.F_Out_Case_Product.TEAM_LEADER}," +
                        $"{DbTable.F_Out_Case_Product.ADMIN}," +
                        $"{DbTable.F_Out_Case_Product.BINDING_DATE}," +
                        $"{DbTable.F_Out_Case_Product.UPDATE_DATE}) VALUES(" +
                        $"'{outCaseCode}','{snOutter}','{typeNo}','{stationName}'," +
                        $"'{bindingState}','{remark}','{teamdLeader}','{admin}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";

                    string updateSQL = $"UPDATE " +
                        $"{DbTable.F_OUT_CASE_PRODUCT_NAME} SET " +
                    $"{DbTable.F_Out_Case_Product.TYPE_NO} = '{typeNo}'," +
                    $"{DbTable.F_Out_Case_Product.BINDING_STATE} = '{bindingState}'," +
                    $"{DbTable.F_Out_Case_Product.REMARK} = '{remark}'," +
                    $"{DbTable.F_Out_Case_Product.TEAM_LEADER} = '{teamdLeader}'," +
                    $"{DbTable.F_Out_Case_Product.ADMIN} = '{admin}'," +
                    $"{DbTable.F_Out_Case_Product.UPDATE_DATE} = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'," +
                    $"{DbTable.F_Out_Case_Product.BINDING_DATE} = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' " +
                    $"WHERE " +
                    $"{DbTable.F_Out_Case_Product.OUT_CASE_CODE} = '{outCaseCode}' " +
                    $"AND " +
                    $"{DbTable.F_Out_Case_Product.SN_OUTTER} = '{snOutter}' ";
                    #endregion

                    var otherBindingState = OtherBindingRecordState(snOutter, outCaseCode);
                    if (otherBindingState.IsExistOtherBindingRecord)
                    {
                        //该产品已绑定其他箱子 STATUS_BINDED_OTHER_CASE
                        //返回
                        resultArray[0] = "0X02";
                        resultArray[1] = otherBindingState.bindedOtherOutcasecode;
                        LogHelper.Log.Info($"【更新产品打包】0X02 STATUS_BINDED_OTHER_CASE");
                        return resultArray;
                    }
                    else
                    {
                        //该产品未绑定其他箱子，可以继续下一步
                        //是否存在记录--绑定/未绑定/不存在
                        if (IsExistRecord(outCaseCode, snOutter))
                        {
                            //存在记录
                            if (IsExistBindingRecord(outCaseCode, snOutter))
                            {
                                //存在绑定记录
                                //更新其他信息
                                int usRes = SQLServer.ExecuteNonQuery(updateSQL);
                                if (usRes > 0)
                                {
                                    resultArray[0] = "0X03";//STATUS_EXIST_BINDED_UPDATE_SUCCESS
                                    LogHelper.Log.Info($"【更新产品打包】0X03 STATUS_EXIST_BINDED_UPDATE_SUCCESS");
                                    return resultArray;
                                }
                                else
                                {
                                    resultArray[0] = "0X04";//STATUS_EXIST_BINDED_UPDATE_FAIL
                                    LogHelper.Log.Info($"【更新产品打包】0X04 STATUS_EXIST_BINDED_UPDATE_FAIL");
                                    return resultArray;
                                }
                            }
                            else
                            {
                                //不存在解绑记录
                                //判断是否要解绑或重新绑定
                                if (bindingState == "0")
                                {
                                    //将解绑数据添加到记录
                                    InsertProductCheckRecord(outCaseCode, snOutter, typeNo, stationName, bindingState, remark, teamdLeader, admin);
                                    int usRes = SQLServer.ExecuteNonQuery(updateSQL);
                                    if (usRes > 0)
                                    {
                                        resultArray[0] = "0X05";//STATUS_NONE_EXIST_BINDED_UPDATE_SUCCESS
                                        LogHelper.Log.Info($"【更新产品打包】0X05 STATUS_NONE_EXIST_UNBIND_UPDATE_SUCCESS");
                                        return resultArray;
                                    }
                                    else
                                    {
                                        resultArray[0] = "0X06";//STATUS_NONE_EXIST_BINDED_UPDATE_FAIL
                                        LogHelper.Log.Info($"【更新产品打包】0X06 STATUS_NONE_EXIST_UNBIND_UPDATE_FAIL");
                                        return resultArray;
                                    }
                                }
                                else if (bindingState == "1")
                                {
                                    //重新绑定
                                    int usRes = SQLServer.ExecuteNonQuery(updateSQL);
                                    if (usRes > 0)
                                    {
                                        resultArray[0] = "0X07";//STATUS_NONE_EXIST_REBIND_SUCCESS
                                        LogHelper.Log.Info($"【更新产品打包】0X07 STATUS_NONE_EXIST_REBIND_SUCCESS");
                                        return resultArray;
                                    }
                                    else
                                    {
                                        resultArray[0] = "0X08";//STATUS_NONE_EXIST_REBIND_FAIL
                                        LogHelper.Log.Info($"【更新产品打包】0X08 STATUS_NONE_EXIST_REBIND_FAIL");
                                        return resultArray;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //insert
                            if (outCaseCode == "")
                            {
                                resultArray[0] = "0X17";//STATUS_NONE_OUTCASE_CODE
                                LogHelper.Log.Info($"【更新产品打包】0X17 插入产品时，传入箱子编号为空");
                                return resultArray;
                            }
                            if (IsOutCaseFull(outCaseCode, typeNo))//已装满
                            {
                                LogHelper.Log.Info("【更新产品打包】0X01 STATUS_FULL");
                                resultArray[0] = "0X01"; //STATUS_FULL
                                return resultArray;
                            }
                            if (bindingState != "1")
                            {
                                LogHelper.Log.Info("【更新产品打包】0X13 STATUS_INSERT_BINDING_STATE_INVALID");
                                resultArray[0] = "0X13"; //STATUS_FULL
                                return resultArray;
                            }
                            int isRes = SQLServer.ExecuteNonQuery(insertSQL);
                            if (isRes > 0)
                            {
                                resultArray[0] = "0X09";//STATUS_INSERT_SUCCESS
                                LogHelper.Log.Info($"【更新产品打包】0X09 STATUS_INSERT_SUCCESS");
                                return resultArray;
                            }
                            else
                            {
                                resultArray[0] = "0X10";//STATUS_INSERT_FAIL
                                LogHelper.Log.Info($"【更新产品打包】0X10 STATUS_INSERT_FAIL");
                                return resultArray;
                            }
                        }
                    }
                }
                return resultArray;
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error("【产品绑定异常错误！】"+ex.Message+ex.StackTrace);
                resultArray[0] = "0X11";//STATUS_NONE_EXIST_BINDED_UPDATE_FAIL
                LogHelper.Log.Info($"【更新产品打包】0X11 STATUS_EXCEPT_FAIL");
                return resultArray;
            }
        }


        private static bool IsExistRecord(string caseCode, string snOutter)
        {
            var selectSQL = $"SELECT {DbTable.F_Out_Case_Product.BINDING_STATE} " +
                $"FROM {DbTable.F_OUT_CASE_PRODUCT_NAME} WHERE " +
                $"{DbTable.F_Out_Case_Product.OUT_CASE_CODE} = '{caseCode}' AND " +
                $"{DbTable.F_Out_Case_Product.SN_OUTTER} = '{snOutter}' ";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return false;//已解绑/无绑定记录
        }


        /// <summary>
        /// 该产品是存在绑定记录
        /// </summary>
        /// <param name="caseCode"></param>
        /// <param name="snOutter"></param>
        /// <returns></returns>
        private static bool IsExistBindingRecord(string caseCode, string snOutter)
        {
            var selectSQL = $"SELECT {DbTable.F_Out_Case_Product.BINDING_STATE} " +
                $"FROM {DbTable.F_OUT_CASE_PRODUCT_NAME} WHERE " +
                $"{DbTable.F_Out_Case_Product.OUT_CASE_CODE} = '{caseCode}' AND " +
                $"{DbTable.F_Out_Case_Product.SN_OUTTER} = '{snOutter}' " +
                $"AND " +
                $"{DbTable.F_Out_Case_Product.BINDING_STATE} = '1'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return false;//已解绑/无绑定记录
        }

        private static UPackageProduct OtherBindingRecordState(string productSN,string caseCode)
        {
            UPackageProduct uPackageProduct = new UPackageProduct();
            var selectSQL = $"SELECT distinct " +
                $"{DbTable.F_Out_Case_Product.OUT_CASE_CODE} " +
                $"FROM " +
                $"{DbTable.F_OUT_CASE_PRODUCT_NAME} " +
                $"WHERE " +
                $"{DbTable.F_Out_Case_Product.SN_OUTTER} = '{productSN}' " +
                $"AND " +
                $"{DbTable.F_Out_Case_Product.BINDING_STATE} = '1'";
            var ds = SQLServer.ExecuteDataSet(selectSQL);
            if (ds.Tables.Count < 1)
            {
                LogHelper.Log.Info("【更新产品打包】不存在其他绑定记录"+selectSQL);
                uPackageProduct.IsExistOtherBindingRecord = false;
                return uPackageProduct;
            }
            LogHelper.Log.Info("【更新产品打包】可能存在其他绑定记录");
            System.Data.DataTable dt = ds.Tables[0];
            if (dt.Rows.Count > 0)
            {
                var otherCaseCode = dt.Rows[0][0].ToString();
                if (caseCode != otherCaseCode)
                {
                    //该产品已绑定其他箱子
                    LogHelper.Log.Info("【更新产品打包】存在其他绑定记录");
                    uPackageProduct.IsExistOtherBindingRecord = true;
                    uPackageProduct.bindedOtherOutcasecode = otherCaseCode;
                    return uPackageProduct;
                }
                else
                {
                    LogHelper.Log.Info("【更新产品打包】不存在其他绑定记录<<casecode="+caseCode+"<<nowcase="+ dt.Rows[0][0].ToString());
                    uPackageProduct.IsExistOtherBindingRecord = false;
                    return uPackageProduct;
                }
            }
            //该产品未绑定其他箱子
            uPackageProduct.IsExistOtherBindingRecord = false;
            return uPackageProduct;
        }

        /// <summary>
        /// 根据产品获取已绑定箱子的剩余空间大小
        /// </summary>
        /// <param name="outcaseCode"></param>
        /// <param name="productTypeNo"></param>
        /// <returns></returns>
        public static int SelectProductOutCaseRemain(string outcaseCode,string productTypeNo)
        {
            try
            {
                var totalSize = -1;
                var useSize = -1;
                //查询箱子总数
                var selectTotalSize = $"SELECT {DbTable.F_Out_Case_Storage.STORAGE_CAPACITY} " +
                    $"FROM " +
                    $"{DbTable.F_OUT_CASE_STORAGE_NAME} " +
                    $"WHERE " +
                    $"{DbTable.F_Out_Case_Storage.TYPE_NO} = '{productTypeNo}'";
                var dt = SQLServer.ExecuteDataSet(selectTotalSize).Tables[0];
                if (dt.Rows.Count > 0)
                    int.TryParse(dt.Rows[0][0].ToString(), out totalSize);
                else
                {
                    LogHelper.Log.Info($"【查询箱子总数失败！】{selectTotalSize}");
                }
                //查询箱子剩余数量
                var selectRemain = $"select COUNT(*) " +
                    $"from {DbTable.F_OUT_CASE_PRODUCT_NAME} " +
                    $"where " +
                    $"{DbTable.F_Out_Case_Product.OUT_CASE_CODE} = '{outcaseCode}' " +
                    $"and " +
                    $"{DbTable.F_Out_Case_Product.SN_OUTTER}='{productTypeNo}' " +
                    $"and " +
                    $"{DbTable.F_Out_Case_Product.BINDING_STATE}='1'";
                var useDt = SQLServer.ExecuteDataSet(selectRemain).Tables[0];
                if (useDt.Rows.Count > 0)
                {
                    int.TryParse(useDt.Rows[0][0].ToString(),out useSize);
                }
                else
                {
                    LogHelper.Log.Info($"【查询箱子剩余数量失败！】{selectRemain}");
                }
                return totalSize - useSize;
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error("【根据产品获取已绑定箱子的剩余空间大小！】" + ex.Message+ex.StackTrace);
                return 0;
            }
        }

        public static int SelectPackageStorage(Queue<string> queue)
        {
            try
            {
                var productTypeNo = queue.Dequeue();
                var selectSQL = $"SELECT {DbTable.F_Out_Case_Storage.STORAGE_CAPACITY} FROM {DbTable.F_OUT_CASE_STORAGE_NAME} WHERE " +
                $"{DbTable.F_Out_Case_Storage.TYPE_NO} = '{productTypeNo}'";
                var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                if (dt.Rows.Count > 0)
                    return int.Parse(dt.Rows[0][0].ToString());
                return 0;
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error("【获取产品打包容量异常！】" + ex.Message);
                return 0;
            }
        }

        public static bool IsOutCaseFull(string casecode,string productTypeNo)
        {
            var selectActualAmount = $"select distinct COUNT(*) from " +
                $"{DbTable.F_OUT_CASE_PRODUCT_NAME} " +
                $"where " +
                $"{DbTable.F_Out_Case_Product.OUT_CASE_CODE}='{casecode}' " +
                $"and " +
                $"{DbTable.F_Out_Case_Product.BINDING_STATE} = '1'";

            var selectCapacity = $"select " +
                $"{DbTable.F_Out_Case_Storage.STORAGE_CAPACITY} from " +
                $"{DbTable.F_OUT_CASE_STORAGE_NAME} " +
                $"where " +
                $"{DbTable.F_Out_Case_Storage.TYPE_NO} = '{productTypeNo}'";
            try
            {
                var actualdt = SQLServer.ExecuteDataSet(selectActualAmount).Tables[0];
                var capacitydt = SQLServer.ExecuteDataSet(selectCapacity).Tables[0];
                var actualAmount = 0;
                var capacity = 0;
                if (actualdt.Rows.Count > 0)
                    actualAmount = int.Parse(actualdt.Rows[0][0].ToString());
                if (capacitydt.Rows.Count > 0)
                    capacity = int.Parse(capacitydt.Rows[0][0].ToString());
                if (actualAmount < capacity)
                    return false;
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error(ex.Message+ex.StackTrace);
            }
            return true;
        }
    }
}