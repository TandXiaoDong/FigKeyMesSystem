using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonUtils.DB;
using CommonUtils.Logger;
using MesWcfService.DB;
using MesWcfService.Model;
using MesWcfService.Common;

namespace MesWcfService.MessageQueue.RemoteClient
{
    public static class MaterialStatistics
    {
        //物料统计
        //插入
        private static string GetDateTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string ConvertMaterialStatisticsCode(MaterialStatisticsReturnCode rCode)
        {
            return "0X" + Convert.ToString((int)rCode, 16).PadLeft(2, '0');
        }

        public static string ConvertCheckMaterialStateCode(MaterialStateReturnCode msCode)
        {
            return "0X"+Convert.ToString((int)msCode,16).PadLeft(2,'0');
        }

        public static string ConvertCheckMaterialMatch(MaterialCheckMatchReturnCode mcmCode)
        {
            return "0X" + Convert.ToString((int)mcmCode,16).PadLeft(2,'0');
        }

        public static string ConvertCheckMaterialPutInStorage(MaterialCheckPutStorageEnum mcpsCode)
        {
            return "0X" + Convert.ToString((int)mcpsCode,16).PadLeft(2,'0');
        }

        #region 物料入库
        /*
         * 1）check是否入库
         * 2）未入库则入库，更新数据
         */
        public static string CheckMaterialPutInStorage(Queue<string[]> queue)
        {
            var array = queue.Dequeue();
            var materialCode = array[0];
            var teamLeader = array[1];
            var admin = array[2];
            var selectSQL = $"SELECT {DbTable.F_Material.MATERIAL_STATE} FROM " +
                $"{DbTable.F_MATERIAL_NAME} WHERE {DbTable.F_Material.MATERIAL_CODE} = '{materialCode}'";
            LogHelper.Log.Info("【查询入库记录】" + selectSQL);
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                //查询有记录，已经入过库
                return ConvertCheckMaterialPutInStorage(MaterialCheckPutStorageEnum.STATUS_IS_PUTED_IN_STORAGE);
            }
            //记录不存在-未入库
            //更新入库记录：
            //插入物料信息-更新物料状态1+更新物料库存
            //
            var insertSQL = $"INSERT INTO {DbTable.F_MATERIAL_NAME}(" +
                $"{DbTable.F_Material.MATERIAL_CODE}," +
                $"{DbTable.F_Material.MATERIAL_STOCK}," +
                $"{DbTable.F_Material.MATERIAL_TEAM_LEADER}," +
                $"{DbTable.F_Material.MATERIAL_ADMIN}," +
                $"{DbTable.F_Material.MATERIAL_UPDATE_DATE}) VALUES(" +
                $"'{materialCode}'," +
                $"'{MaterialCodeMsg.GetMaterialDetail(materialCode).MaterialQTY}'," +
                $"'{teamLeader}'," +
                $"'{admin}'," +
                $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
            LogHelper.Log.Info("【入库】"+insertSQL);
            var iRes = SQLServer.ExecuteNonQuery(insertSQL);
            if (iRes == 1)//新入库成功
            {
                //更新物料PN
                UpdateMaterialPN(MaterialCodeMsg.GetMaterialPN(materialCode));
                return ConvertCheckMaterialPutInStorage(MaterialCheckPutStorageEnum.STATUS_IS_NEW_PUT_INT_STORAGE);
            }
            return ConvertCheckMaterialPutInStorage(MaterialCheckPutStorageEnum.STATUS_IS_PUT_IN_FAIL_STORAGE);
        }

        private static void UpdateMaterialPN(string materialPN)
        {
            if (!IsExistMaterialPN(materialPN))
            {
                //insert new data
                var insertSQL = $"INSERT INTO {DbTable.F_MATERIAL_PN_NAME}(" +
                    $"{DbTable.F_MATERIAL_PN.MATERIAL_PN}," +
                    $"{DbTable.F_MATERIAL_PN.UPDATE_DATE}) VALUES(" +
                    $"'{materialPN}'," +
                    $"'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
                LogHelper.Log.Info("【更新物料PN】"+insertSQL);
                SQLServer.ExecuteNonQuery(insertSQL);
            }
        }

        private static bool IsExistMaterialPN(string materialPN)
        {
            var selectSQL = $"SELECT * FROM {DbTable.F_MATERIAL_PN_NAME} WHERE " +
                $"{DbTable.F_MATERIAL_PN.MATERIAL_PN} = '{materialPN}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }

        #endregion

        #region 物料号防错
        /// <summary>
        /// 物料号防错
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        public static string CheckMaterialMatch(Queue<string[]> queue)
        {
            var materialArray = queue.Dequeue();
            var productTypeNo = materialArray[0];
            var materialPN = materialArray[1];
            var materialCode = materialArray[2];
            return CheckMaterialTypeMatch(productTypeNo,materialPN, materialCode);
        }

        private static string CheckMaterialTypeMatch(string productTypeNo,string materialPN, string materialCode)
        {
            var selectSQL = $"SELECT * FROM {DbTable.F_PRODUCT_MATERIAL_NAME}  WHERE " +
                $"{DbTable.F_PRODUCT_MATERIAL.TYPE_NO} = '{productTypeNo}' AND " +
                $"{DbTable.F_PRODUCT_MATERIAL.MATERIAL_CODE} = '{materialPN}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count == 1)
            {
                //传入物料号存在，且要防错的物料号与实际扫描物料号相同
                //当扫描的物料号相同，但为新的一箱时，判断是否可以扫描新的一箱
                //即使用完了才能继续扫描下一箱物料

                //检查传入编码是否已经存在，已经存在了就直接返回，不需要再次判断
                //如果不存在，则为新扫描的同种物料的不同条码，需要判断已经存在的同种条码是否还有未使用完的，有就返回6
                if (IsExistMaterial(materialCode))
                {   //第二次之后扫描相同编码，存在-返回
                    //新增防错：当之后扫描已经入库的物料时，如果该物料已经使用完成，则返回其他代码，因为继续使用没有意义
                    var materialState = SelectCurrentMaterialState(materialCode);
                    if (materialState == "2" || materialState == "3")
                        return ConvertCheckMaterialMatch(MaterialCheckMatchReturnCode.STATUS_CURRENT_MATERIAL_AMOUNT_END_OF_USE);
                    return ConvertCheckMaterialMatch(MaterialCheckMatchReturnCode.IS_MATCH);
                }
                else
                {
                    //根据PN号查询是否有入库记录
                    if (!IsExistMaterialCodePN(materialPN))
                    {
                        //没有入库记录，则为第一次入库，返回
                        if (!IsExistMaterial(materialCode))
                        {
                            return ConvertCheckMaterialMatch(MaterialCheckMatchReturnCode.IS_MATCH);
                        }
                    }
                    else
                    {
                        //有入库记录，则查询是否有使用记录

                        if (!IsExistStatistic(productTypeNo, materialPN))
                        {
                            //如果没有统计记录，则说明都是入库了没有使用，返回正常即可
                            return ConvertCheckMaterialMatch(MaterialCheckMatchReturnCode.IS_MATCH);
                        }
                        else
                        {
                            //如果有统计记录，说明某一箱已经使用了，在判断这一箱是否使用完即可
                            var cStateRes = CheckMaterialUseState(productTypeNo, materialCode);
                            LogHelper.Log.Info("【物料号防错】cStateRes="+cStateRes);
                            if (ConvertCheckMaterialStateCode(MaterialStateReturnCode.STATUS_USING) == cStateRes)
                            {
                                //物料未使用完，不能扫描新的同种物料
                                return ConvertCheckMaterialMatch(MaterialCheckMatchReturnCode.ERROR_LAST_MATERIAL_PN_IS_NOT_USED_UP);
                            }
                            else if (ConvertCheckMaterialStateCode(MaterialStateReturnCode.STATUS_OTHER_COMPLETE) == cStateRes)
                            {
                                return ConvertCheckMaterialMatch(MaterialCheckMatchReturnCode.IS_MATCH);
                            }
                            else
                            {
                                LogHelper.Log.Info("【物料号匹配其他情况】"); 
                            }
                        }
                    }
                }
            }
            LogHelper.Log.Info("【物料号防错】不匹配");
            return ConvertCheckMaterialMatch(MaterialCheckMatchReturnCode.IS_NOT_MATCH);
        }

        private static string SelectCurrentMaterialState(string materialCode)
        {
            var selectSQL = $"SELECT {DbTable.F_Material.MATERIAL_STATE} FROM {DbTable.F_MATERIAL_NAME} WHERE " +
                $"{DbTable.F_Material.MATERIAL_CODE} = '{materialCode}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            LogHelper.Log.Info(selectSQL);
            if (dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();
            return "";
        }

        private static bool IsExistUsedComplete(string materialCode)
        {
            var selectSQl = $"SELECT {DbTable.F_Material.MATERIAL_STATE} " +
               $"FROM {DbTable.F_MATERIAL_NAME} WHERE " +
               $"{DbTable.F_Material.MATERIAL_CODE} = '{materialCode}' AND " +
               $"{DbTable.F_Material.MATERIAL_STATE} = '1' ";
            LogHelper.Log.Info(selectSQl);
            var dt = SQLServer.ExecuteDataSet(selectSQl).Tables[0];
            if (dt.Rows.Count > 0)
                return false;
            return true;
        }

        private static bool IsExistStatistic(string productTypeNo,string materialPN)
        {
            //根据物料号+产品型号查询是否有统计计数记录
            var selectRecordSQL = $"SELECT {DbTable.F_Material_Statistics.MATERIAL_CODE} FROM " +
                $"{DbTable.F_MATERIAL_STATISTICS_NAME} WHERE " +
                $"{DbTable.F_Material_Statistics.PRODUCT_TYPE_NO} = '{productTypeNo}' AND " +
                $"{DbTable.F_Material_Statistics.MATERIAL_CODE} like '%{materialPN}%'";
            var dt = SQLServer.ExecuteDataSet(selectRecordSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }

        private static bool IsExistMaterial(string materialCode)
        {
            var selectSQL = $"SELECT * FROM {DbTable.F_MATERIAL_NAME} WHERE " +
                $"{DbTable.F_Material.MATERIAL_CODE} = '{materialCode}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }

        private static bool IsExistMaterialCodePN(string materialCode)
        {
            var selectSQL = $"SELECT * FROM {DbTable.F_MATERIAL_NAME} WHERE " +
                $"{DbTable.F_Material.MATERIAL_CODE} like '%{materialCode}%'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }
        #endregion

        #region 物料数量防错
        /*物料数量防错：即该种物料使用完后才能继续扫描新的物料使用
         * 防错原理：
         * 1）传入产品型号+物料完整编码
         * 2）根据传入的产品型号+物料号=》查询产品统计表，是否有使用记录？
         *      （1）有使用记录：
         *              则继续查询出使用记录中的所有物料编码
         *              将传入物料编码与查询出的编码匹配，是否有一样的？
         *                  （1）匹配到相同的编码：则说明正在使用该物料编码；此时，在物料信息表中查询该物料状态反馈即可
         *                  （2）没有匹配到相同编码：说明传入的物料编码为新扫描的物料编码，
         *                       此时，将物料统计表中查询出的所有物料编码，查询出不一致的状态反馈
         *      （2）无使用记录：则为第一次扫描该物料，已经入库，只需在物料信息表查询该物料编码的状态反馈即可
         */
        public static string CheckMaterialState(Queue<string[]> queue)
        {
            var array = queue.Dequeue();
            var productTypeNo = array[0];
            var materialCode = array[1];

            return CheckMaterialUseState(productTypeNo,materialCode);
        }

        private static string CheckMaterialUseState(string productTypeNo,string materialCode)
        {
            if (!materialCode.Contains("&"))
                return ConvertCheckMaterialStateCode(MaterialStateReturnCode.ERROR_FORMAT_MATERIAL_CODE);
            //查询当前物料是否使用完成，完成返回1
            LogHelper.Log.Info("【物料数量防错开始查询】");
            var  stateValue = SelectCurrentMaterialState(materialCode);
            LogHelper.Log.Info("【物料数量防错开始查询-状态=】"+stateValue);
            if (stateValue == "2" || stateValue == "3")
            {
                return ConvertCheckMaterialStateCode(MaterialStateReturnCode.STATUS_COMPLETE_NORMAL);
            }
            var materialPN = MaterialCodeMsg.GetMaterialPN(materialCode);

            //根据物料号+产品型号查询是否有统计计数记录
            var selectRecordSQL = $"SELECT {DbTable.F_Material_Statistics.MATERIAL_CODE} FROM " +
                $"{DbTable.F_MATERIAL_STATISTICS_NAME} WHERE " +
                $"{DbTable.F_Material_Statistics.PRODUCT_TYPE_NO} = '{productTypeNo}' AND " +
                $"{DbTable.F_Material_Statistics.MATERIAL_CODE} like '%{materialPN}%'";
            var dt = SQLServer.ExecuteDataSet(selectRecordSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                /* 有统计记录
                 * 查询统计记录中的所有物料编码
                 * 比对传入物料编码在统计记录中是否存在
                 */
                bool IsSameMaterialCode = false;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var mCode = dt.Rows[i][0].ToString();
                    if (materialCode == mCode)
                    {
                        IsSameMaterialCode = true;
                    }
                }
                if (IsSameMaterialCode)
                {
                    //存在相同编码
                    LogHelper.Log.Info("【物料数量防错-存在相同编码】");
                    return SelectMaterialState(materialCode);
                }
                else
                {
                    /* 不存在相同编码
                     * 则说明传入物料编码为新扫描编码
                     * 则查询所有已知编码的状态，是否都是使用完成状态，否则提示不能继续使用新扫描的物料
                     */
                    LogHelper.Log.Info("【物料数量防错-不存在相同编码】");
                    bool IsUseComplete = true;//默认使用完成
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var mCode = dt.Rows[i][0].ToString();

                        var selectSQl = $"SELECT {DbTable.F_Material.MATERIAL_STATE} " +
                                        $"FROM {DbTable.F_MATERIAL_NAME} WHERE " +
                                        $"{DbTable.F_Material.MATERIAL_CODE} = '{mCode}'";
                        var mdt = SQLServer.ExecuteDataSet(selectSQl).Tables[0];
                        if (mdt.Rows.Count > 0)
                        {
                            var mState = mdt.Rows[0][0].ToString();
                            if (mState == "1")
                            {
                                //有物料未使用完，请使用完了在扫描别的箱使用
                                IsUseComplete = false;
                            }
                        }
                    }
                    if (IsUseComplete)
                    {
                        //使用完成-其他物料都使用完成,则当前物料
                        LogHelper.Log.Info("【物料数量防错-不存在相同编码】-"+0);
                        return ConvertCheckMaterialStateCode(MaterialStateReturnCode.STATUS_OTHER_COMPLETE);
                    }
                    else
                    {
                        //有未使用完成物料
                        return ConvertCheckMaterialStateCode(MaterialStateReturnCode.STATUS_USING);
                    }
                }
            }
            else
            {
                /* 无统计记录/为防止改物料在之前未进行物料号防错，此步骤可再次验证物料防错
                 * 则为第一次扫描该物料编码
                 * 直接查询物料信息表中该物料状态反馈即可
                 */
                LogHelper.Log.Info("【物料防错-状态-无统计记录-直接查询状态】");
                //string cRes = CheckMaterialTypeMatch(productTypeNo, materialPN, materialCode);
                //if (cRes == ConvertCheckMaterialMatch(MaterialCheckMatchReturnCode.IS_NOT_MATCH))
                //{
                //    LogHelper.Log.Info("【物料防错-状态-物料号与当前产品不匹配】");
                //    return ConvertCheckMaterialStateCode(MaterialStateReturnCode.ERROR_MATRIAL_CODE_IS_NOT_MATCH_WITH_PRODUCT_TYPENO);
                //}
                return SelectMaterialState(materialCode);
            }
        }

        private static string SelectMaterialState(string materialCode)
        {
            var selectSQl = $"SELECT {DbTable.F_Material.MATERIAL_STATE} " +
               $"FROM {DbTable.F_MATERIAL_NAME} WHERE " +
               $"{DbTable.F_Material.MATERIAL_CODE} = '{materialCode}'";
            LogHelper.Log.Info(selectSQl);
            var dt = SQLServer.ExecuteDataSet(selectSQl).Tables[0];
            if (dt.Rows.Count < 1)
                return ConvertCheckMaterialStateCode(MaterialStateReturnCode.STATUS_NULL_QUERY);
            var queryRes = dt.Rows[0][0].ToString();
            return "0X" + Convert.ToString(int.Parse(queryRes),16).PadLeft(2, '0');
        }

        public static string UpdateMaterialStatistics(Queue<string[]> queue)
        {
            //更新物料统计：插入/更新
            //更新产品-物料：使用物料数量
            //更新物料库存：使用数量，物料状态，使用完成更新结单状态为2
            //typeNo,stationName,materialCode,amounted,teamLeader,admin
            try
            {
                #region material params
                string[] array = queue.Dequeue();
                var productTypeNo = array[0];
                var stationName = array[1];
                var materialCode = array[2];
                var amounted = array[3];
                var teamLeader = array[4];
                var admin = array[5];
                var pcbaSN = array[6];
                #endregion

                #region insert sql
                var insertSQL = $"INSERT INTO {DbTable.F_MATERIAL_STATISTICS_NAME}(" +
                    $"{DbTable.F_Material_Statistics.PRODUCT_TYPE_NO}," +
                    $"{DbTable.F_Material_Statistics.STATION_NAME}," +
                    $"{DbTable.F_Material_Statistics.MATERIAL_CODE}," +
                    $"{DbTable.F_Material_Statistics.MATERIAL_AMOUNT}," +
                    $"{DbTable.F_Material_Statistics.TEAM_LEADER}," +
                    $"{DbTable.F_Material_Statistics.ADMIN}," +
                    $"{DbTable.F_Material_Statistics.UPDATE_DATE}," +
                    $"{DbTable.F_Material_Statistics.PCBA_SN}) VALUES(" +
                    $"'{productTypeNo}','{stationName}','{materialCode}','{amounted}'," +
                    $"'{teamLeader}','{admin}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','{pcbaSN}')";

                #endregion

                int row = 0;
                if (!IsExistMaterialData(array))
                {
                    //插入
                    row = SQLServer.ExecuteNonQuery(insertSQL);
                    if (row > 0)
                    {
                        //插入成功
                        var iuRes = UpdateMaterialAmounted(materialCode, int.Parse(amounted));//更新总的计数
                        var isRes = UpdateMaterialState(materialCode);//更新状态
                        UpdateCurrentMaterialRemain(materialCode, productTypeNo, stationName, pcbaSN);
                        if (iuRes > 0 && isRes > 0)
                        {
                            //更新物料使用数量成功
                            return ConvertMaterialStatisticsCode(MaterialStatisticsReturnCode.STATUS_USCCESS);
                        }
                    }
                    LogHelper.Log.Error("【插入失败】"+insertSQL);
                    return ConvertMaterialStatisticsCode(MaterialStatisticsReturnCode.STATUS_FAIL);
                }

                //更新物料统计
                //int originNum = SelectLastInsertAmount(productTypeNo, stationName, materialCode);
                var mRes = UpdateMaterialStatisticAmounted(pcbaSN,productTypeNo,stationName,materialCode,int.Parse(amounted));
                var uRes = UpdateMaterialAmounted(materialCode, int.Parse(amounted));//更新计数
                var sRes = UpdateMaterialState(materialCode);//更新状态
                UpdateCurrentMaterialRemain(materialCode, productTypeNo, stationName, pcbaSN);
                if (uRes > 0 && sRes > 0 && mRes > 0)
                {
                    //更新物料使用数量成功
                    LogHelper.Log.Info("【物料计数统计更新成功】");
                    return ConvertMaterialStatisticsCode(MaterialStatisticsReturnCode.STATUS_USCCESS);
                }
                return ConvertMaterialStatisticsCode(MaterialStatisticsReturnCode.STATUS_FAIL);
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error(ex.Message);
                return "ERROR";
            }
        }

        private static bool IsExistMaterialData(string[] array)
        {
            var productTypeNo = array[0];
            var stationName = array[1];
            var materialCode = array[2];
            var amounted = array[3];
            var pcbaSn = array[6];
            var selectSQL = $"SELECT * FROM {DbTable.F_MATERIAL_STATISTICS_NAME} WHERE " +
                $"{DbTable.F_Material_Statistics.PRODUCT_TYPE_NO} = '{productTypeNo}' AND " +
                $"{DbTable.F_Material_Statistics.STATION_NAME} = '{stationName}' AND " +
                $"{DbTable.F_Material_Statistics.MATERIAL_CODE} = '{materialCode}' AND " +
                $"{DbTable.F_Material_Statistics.PCBA_SN} = '{pcbaSn}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
                return true;
            return false;
        }

        private static int SelectLastInsertAmount(string type_no, string stationName, string code)
        {
            var selectSQL = $"SELECT {DbTable.F_Material_Statistics.MATERIAL_AMOUNT} " +
                $"FROM {DbTable.F_MATERIAL_STATISTICS_NAME} WHERE " +
                $"{DbTable.F_Material_Statistics.STATION_NAME} = '{stationName}' AND " +
                $"{DbTable.F_Material_Statistics.PRODUCT_TYPE_NO} = '{type_no}' AND " +
                $"{DbTable.F_Material_Statistics.MATERIAL_CODE} = '{code}'";
            var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return int.Parse(dt.Rows[0][0].ToString());
            }
            return 0;
        }

        //更新产品物料数量
        private static int UpdateMaterialAmounted(string materialCode,int amounted)
        {
            var updateSQL = $"UPDATE {DbTable.F_MATERIAL_NAME} SET " +
                $"{DbTable.F_Material.MATERIAL_AMOUNTED} += '{amounted}' " +
                $"WHERE " +
                $"{DbTable.F_Material.MATERIAL_CODE} = '{materialCode}'";
            LogHelper.Log.Info("【更新物料使用总数量】"+updateSQL);
            return SQLServer.ExecuteNonQuery(updateSQL);
        }

        private static int UpdateMaterialStatisticAmounted(string pcbaSN,string productTypeNo,string stationName,string materialCode, int amounted)
        {
            var updateSQL = $"UPDATE {DbTable.F_MATERIAL_STATISTICS_NAME} SET " +
                $"{DbTable.F_Material_Statistics.MATERIAL_AMOUNT} += '{amounted}' " +
                $"WHERE " +
                $"{DbTable.F_Material_Statistics.MATERIAL_CODE} = '{materialCode}' AND " +
                $"{DbTable.F_Material_Statistics.PRODUCT_TYPE_NO} = '{productTypeNo}' AND " +
                $"{DbTable.F_Material_Statistics.STATION_NAME} = '{stationName}' AND " +
                $"{DbTable.F_Material_Statistics.PCBA_SN} = '{pcbaSN}'";
            LogHelper.Log.Info("【更新物料统计使用总数量】" + updateSQL);
            return SQLServer.ExecuteNonQuery(updateSQL);
        }

        /// <summary>
        /// 更新物料状态 0-fail,1-success
        /// </summary>
        /// <param name="materialCode"></param>
        /// <returns></returns>
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
                    //物料已使用完，更新状态为2
                    var updateSQL = $"UPDATE {DbTable.F_MATERIAL_NAME} SET " +
                        $"{DbTable.F_Material.MATERIAL_STATE} = '2' WHERE " +
                        $"{DbTable.F_Material.MATERIAL_CODE} = '{materialCode}'";
                    return  SQLServer.ExecuteNonQuery(updateSQL);
                }
                return 1;
            }
            return 0;
        }

        private static void UpdateCurrentMaterialRemain(string materialCode,string typeNo,string stationName,string pcbaSN)
        {
            try
            {
                var selectSQL = $"SELECT {DbTable.F_Material.MATERIAL_STOCK},{DbTable.F_Material.MATERIAL_AMOUNTED} " +
                    $"FROM {DbTable.F_MATERIAL_NAME} WHERE {DbTable.F_Material.MATERIAL_CODE} = '{materialCode}'";
                var dtOrigin = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                if (dtOrigin.Rows.Count > 0)
                {
                    var stock = dtOrigin.Rows[0][0].ToString();
                    var amount = dtOrigin.Rows[0][1].ToString();
                    var remainTotal = int.Parse(stock) - int.Parse(amount);
                    //更新当前物料剩余库存
                    var updateRemain = $"UPDATE {DbTable.F_MATERIAL_STATISTICS_NAME} SET " +
                        $"{DbTable.F_Material_Statistics.MATERIAL_CURRENT_REMAIN} = '{remainTotal}' " +
                        $"WHERE {DbTable.F_Material_Statistics.MATERIAL_CODE} = '{materialCode}' " +
                        $"AND " +
                        $"{DbTable.F_Material_Statistics.PRODUCT_TYPE_NO} = '{typeNo}' " +
                        $"AND " +
                        $"{DbTable.F_Material_Statistics.STATION_NAME} = '{stationName}' " +
                        $"AND " +
                        $"{DbTable.F_Material_Statistics.PCBA_SN} = '{pcbaSN}'";
                    var dtRemain = SQLServer.ExecuteNonQuery(updateRemain);
                    if (dtRemain > 0)
                    {
                        LogHelper.Log.Info("【当前物料剩余数量更新成功！】");
                    }
                    else
                    {
                        LogHelper.Log.Info("【当前物料剩余数量更新失败！】");
                    }
                }
                else
                {
                    LogHelper.Log.Info("【更新物料剩余数量-查询库存与使用总数失败】");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error("更新物料剩余数量异常-"+ex.Message);
            }
        }
        #endregion

        #region 查询物料剩余数量
        public static string SelectMaterialSurplus(Queue<string> queue)
        {
            try
            {
                var materialCode = queue.Dequeue();
                var selectSQL = $"SELECT {DbTable.F_Material.MATERIAL_STOCK}," +
                    $"{DbTable.F_Material.MATERIAL_AMOUNTED} FROM {DbTable.F_MATERIAL_NAME} WHERE " +
                    $"{DbTable.F_Material.MATERIAL_CODE} = '{materialCode}'";
                var dt = SQLServer.ExecuteDataSet(selectSQL).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    var stock = dt.Rows[0][0].ToString();
                    var amounted = dt.Rows[0][1].ToString();
                    return (int.Parse(stock) - int.Parse(amounted)).ToString();
                }
                return "";
            }
            catch (Exception ex)
            {
                LogHelper.Log.Error("【查询数量失败！】"+ex.Message);
                return "";
            }
        }
        #endregion
    }
    public class MaterialParams
    {
        /// <summary>
        /// PCBA
        /// </summary>
        public string MaterialPCBA { get; set; }

        /// <summary>
        /// 外壳
        /// </summary>
        public string MaterialOutterShell { get; set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        public string ProductTypeNo { get; set; }

        /// <summary>
        /// 站位名称
        /// </summary>
        public string StationName { get; set; }

        /// <summary>
        /// 上盖
        /// </summary>
        public string MaterialTopCover { get; set; }

        /// <summary>
        /// 上壳
        /// </summary>
        public string MaterialUpperShell { get; set; }

        /// <summary>
        /// 下壳
        /// </summary>
        public string MaterialLowerShell { get; set; }

        /// <summary>
        /// 线束
        /// </summary>
        public string MaterialWirebean { get; set; }

        /// <summary>
        /// 支架板
        /// </summary>
        public string MaterialSupportPlate { get; set; }

        /// <summary>
        /// 泡棉
        /// </summary>
        public string MaterialBubbleCotton { get; set; }

        /// <summary>
        /// 临时支架
        /// </summary>
        public string MaterialTempStent { get; set; }

        /// <summary>
        /// 最终支架
        /// </summary>
        public string MaterialFinalStent { get; set; }

        /// <summary>
        /// 小螺钉
        /// </summary>
        public string MaterialLittleScrew { get; set; }

        /// <summary>
        /// 长螺钉
        /// </summary>
        public string MaterialLongScrew { get; set; }

        /// <summary>
        /// 螺丝/螺母
        /// </summary>
        public string MaterialScrewNut { get; set; }

        /// <summary>
        /// 防水圈
        /// </summary>
        public string MaterialWaterProofRing { get; set; }

        /// <summary>
        /// 密封圈
        /// </summary>
        public string MaterialSealRing { get; set; }

        /// <summary>
        /// 使用数量
        /// </summary>
        public string MaterialUseAmount { get; set; }

        /// <summary>
        /// 班组长
        /// </summary>
        public string TeamLeader { get; set; }

        /// <summary>
        /// 管理员
        /// </summary>
        public string Admin { get; set; }
    }
}