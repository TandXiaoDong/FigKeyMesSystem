﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MesWcfService.DB
{
    public class DbTable
    {
        public const string F_USER_NAME                     = "[WT_SCL].[dbo].[f_user]";
        public const string F_TECHNOLOGICAL_PROCESS_NAME    = "[WT_SCL].[dbo].[f_technological_process]";
        public const string F_TEST_RESULT_NAME              = "[WT_SCL].[dbo].[f_test_result_data]";
        public const string F_PRODUCT_MATERIAL_NAME         = "[WT_SCL].[dbo].[f_product_material]";
        public const string F_MATERIAL_NAME                 = "[WT_SCL].[dbo].[f_material]";
        public const string F_MATERIAL_STATISTICS_NAME      = "[WT_SCL].[dbo].[f_material_statistics]";
        public const string F_OUT_CASE_STORAGE_NAME         = "[WT_SCL].[dbo].[f_product_package_storage]";
        public const string F_OUT_CASE_PRODUCT_NAME         = "[WT_SCL].[dbo].[f_product_package]";
        public const string F_PRODUCT_CHECK_RECORD_NAME     = "[WT_SCL].[dbo].[f_product_check_record]";
        public const string F_PASS_RATE_STATISTICS_NAME     = "[WT_SCL].[dbo].[f_pass_rate_statistics]";
        public const string F_TEST_PROGRAME_VERSION_NAME    = "[WT_SCL].[dbo].[f_test_programe_version]";
        public const string F_TEST_LIMIT_CONFIG_NAME        = "[WT_SCL].[dbo].[f_test_limit_config]";
        public const string F_TEST_LOG_DATA_NAME            = "[WT_SCL].[dbo].[f_test_log_data]";
        public const string F_BINDING_PCBA_NAME             = "[WT_SCL].[dbo].[f_binding_pcba]";
        public const string F_MATERIAL_PN_NAME              = "[WT_SCL].[dbo].[f_material_pn]";
        public const string F_PCBA_NAME                     = "[WT_SCL].[dbo].[f_pcba]";
        public const string F_TEST_PCBA_NAME                = "[WT_SCL].[dbo].[f_testPCBA]";
        public const string F_TEST_RESULT_HISTORY_NAME      = "[WT_SCL].[dbo].[f_testResultHistory]";

        public class F_User
        {
            public const string USER_ID         = "[userID]";
            public const string USER_NAME       = "[username]";
            public const string PASS_WORD       = "[password]";
            public const string PHONE           = "[phone]";
            public const string EMAIL           = "[email]";
            public const string CREATE_DATE     = "[create_date]";
            public const string UPDATE_DATE     = "[update_date]";
            public const string STATUS          = "[status]";
            public const string ROLE_NAME       = "[role_name]";
        }

        public class F_TECHNOLOGICAL_PROCESS
        {
            public const string PROCESS_NAME    = "[process_name]";
            public const string STATION_ORDER   = "[station_order]";
            public const string STATION_NAME    = "[station_name]";
            public const string UPDATE_DATE     = "[update_date]";
            public const string USER_NAME       = "[username]";
            public const string PSTATE           = "[pstate]";
        }

        public class F_Test_Result
        {
            public const string PROCESS_NAME    = "[process_name]";
            public const string SN              = "[sn]";
            public const string TYPE_NO         = "[type_no]";
            public const string STATION_NAME    = "[station_name]";
            public const string TEST_RESULT     = "[test_result]";
            public const string CREATE_DATE     = "[create_date]";
            public const string UPDATE_DATE     = "[update_date]";
            public const string REMARK          = "[remark]";
            public const string TEAM_LEADER     = "team_leader";
            public const string ADMIN           = "[admin]";
            public const string STATION_IN_DATE = "[station_in_date]";
            public const string STATION_OUT_DATE = "[station_out_date]";
            public const string JOIN_DATE_TIME = "[joinDateTime]";
        }

        public class F_Material
        {
            public const string MATERIAL_CODE           = "[material_code]";
            public const string MATERIAL_NAME           = "[material_name]";
            public const string MATERIAL_STOCK          = "[material_stock]";
            public const string MATERIAL_AMOUNTED       = "[material_amounted]";
            public const string MATERIAL_ACTUAL_AMOUNT  = "[material_actualAmount]";
            public const string MATERIAL_BREAK_AMOUNT   = "[material_breakAmount]";
            public const string MATERIAL_STATE          = "[material_state]";
            public const string MATERIAL_DESCRIBLE      = "[material_describle]";
            public const string MATERIAL_USERNAME       = "[material_username]";
            public const string MATERIAL_UPDATE_DATE    = "[material_update_date]";
            public const string MATERIAL_TEAM_LEADER    = "[team_leader]";
            public const string MATERIAL_ADMIN          = "[admin]";
        }

        public class F_MATERIAL_PN
        {
            public const string MATERIAL_PN = "[material_pn]";
            public const string MATERIAL_NAME = "[material_name]";
            public const string USER_NAME = "[user_name]";
            public const string UPDATE_DATE = "[update_date]";
            public const string DESCRIBLE = "[describle]";
        }

        public class F_PRODUCT_MATERIAL
        {
            public const string TYPE_NO         = "[type_no]";
            public const string MATERIAL_CODE   = "[material_code]";
            public const string AMOUNTED        = "[amounted]";
            public const string Describle       = "[describle]";
            public const string UpdateDate      = "[update_date]";
            public const string USERNAME        = "[username]";
        }

        public class F_Material_Statistics
        {
            public const string PRODUCT_TYPE_NO             = "[type_no]";
            public const string STATION_NAME                = "[station_name]";
            public const string MATERIAL_CODE               = "[material_code]";
            public const string MATERIAL_AMOUNT             = "[material_amounted]";
            public const string TEAM_LEADER                 = "[team_leader]";
            public const string ADMIN                       = "[admin]";
            public const string UPDATE_DATE                 = "[update_date]";
            public const string PCBA_SN                     = "[pcbaSN]";
            public const string MATERIAL_CURRENT_REMAIN     = "[material_current_remain]";
        }

        public class F_Out_Case_Storage
        {
            public const string TYPE_NO             = "[type_no]";
            public const string OUT_CASE_CODE       = "[out_case_code]";
            public const string STORAGE_CAPACITY    = "[storage_capacity]";
            public const string AMOUNTED            = "[amounted]";
            public const string USER_NAME           = "username";
            public const string TEAM_LEADER         = "[team_leader]";
            public const string ADMIN               = "[admin]";
            public const string UPDATE_DATE_U       = "update_date_u";
            public const string UPDATE_DATE_T       = "update_date_t";
        }

        public class F_Out_Case_Product
        {
            public const string OUT_CASE_CODE       = "[out_case_code]";
            public const string SN_OUTTER           = "[sn_outter]";
            public const string TYPE_NO             = "[type_no]";
            public const string STATION_NAME        = "station_name";
            public const string PICTURE             = "[picture]";
            public const string BINDING_STATE       = "[binding_state]";
            public const string BINDING_DATE        = "[binding_date]";
            public const string REMARK              = "[remark]";
            public const string TEAM_LEADER         = "[team_leader]";
            public const string ADMIN               = "[admin]";
            public const string UPDATE_DATE         = "[update_date]";
        }

        public class F_Product_Check_Record
        {
            public const string OUT_CASE_CODE = "[out_case_code]";
            public const string SN_OUTTER = "[sn_outter]";
            public const string TYPE_NO = "[type_no]";
            public const string STATION_NAME = "station_name";
            public const string PICTURE = "[picture]";
            public const string BINDING_STATE = "[binding_state]";
            public const string BINDING_DATE = "[binding_date]";
            public const string REMARK = "[remark]";
            public const string TEAM_LEADER = "[team_leader]";
            public const string ADMIN = "[admin]";
            public const string UPDATE_DATE = "[update_date]";
        }

        public class F_Pass_Rate_Statistics
        {
            public const string OUT_CASE_CODE       = "[out_case_code]";
            public const string SN_OUTTER           = "[sn_outter]";
            public const string TYPE_NO             = "[type_no]";
            public const string PRIORITY            = "[priority]";
            public const string AMOUNT              = "[amount]";
            public const string STORAGE_CAPACITY    = "[storage_capacity]";
            public const string UPDATE_DATE         = "[update_date]";
        }

        public class F_TEST_PROGRAME_VERSION
        {
            public const string TYPE_NO             = "[type_no]";
            public const string STATION_NAME        = "[station_name]";
            public const string PROGRAME_NAME       = "[programe_name]";
            public const string PROGRAME_VERSION    = "[programe_version]";
            public const string TEAM_LEADER         = "[team_leader]";
            public const string ADMIN               = "[admin]";
            public const string UPDATE_DATE         = "[update_date]";
        }

        public class F_TEST_LIMIT_CONFIG
        {
            public const string STATION_NAME    = "[station_name]";
            public const string TYPE_NO         = "[type_no]";
            public const string TEST_ITEM       = "test_item";
            public const string LIMIT           = "[limit]";
            public const string TEAM_LEADER     = "[team_leader]";
            public const string ADMIN           = "[admin]";
            public const string UPDATE_DATE     = "[update_date]";
        }

        public class F_TEST_LOG_DATA
        {
            public const string STATION_NAME = "[stationName]";
            public const string PRODUCT_SN = "[productSn]";
            public const string TYPE_NO = "[productTypeNo]";
            public const string TEST_ITEM = "[testItem]";
            public const string LIMIT = "[limit]";
            public const string CURRENT_VALUE = "[currentValue]";
            public const string TEST_RESULT = "[testResult]";
            public const string TEAM_LEADER = "[teamLeader]";
            public const string ADMIN = "[admin]";
            public const string UPDATE_DATE = "[updateDate]";
            public const string JOIN_DATE_TIME = "[joinDateTime]";
        }

        public class F_BINDING_PCBA
        {
            public const string PRODUCT_TYPE_NO = "[type_no]";
            public const string SN_PCBA = "[sn_pcba]";
            public const string SN_OUTTER = "[sn_outter]";
            public const string MATERIAL_CODE = "[material_code]";
            public const string UPDATE_DATE = "[update_date]";
            public const string BINDING_STATE = "[binding_state]";
            public const string PCBA_STATE = "[pcba_state]";
            public const string OUTTER_STATE = "[outter_state]";
        }

        public class F_PCBA
        {
            public const string SN_PCBA = "[sn_pcba]";
            public const string SN_OUTTER = "sn_outter";
            public const string PCBA_STATE = "pcba_state";
            public const string OUTTER_STATE = "shell_state";
        }

        public class F_TEST_PCBA
        {
            public const string PCBA_SN = "[pcbaSN]";
            //public const string PRODUCT_SN = "[productSN]";
            public const string UPDATE_DATE = "[updateDate]";
        }

        public class F_TEST_RESULT_HISTORY
        {
            //65 column
            public const string id = "[ID]";
            public const string updateDate = "[updateDate]";
            public const string pcbaSN = "[pcbaSN]";
            public const string productSN = "[productSN]";
            public const string bindState = "[bindState]";
            public const string productTypeNo = "[productTypeNo]";
            public const string burnStationName = "[burnStationName]";
            public const string burnDateIn = "[burnDateIn]";
            public const string burnDateOut = "[burnDateOut]";
            public const string burnTestResult = "[burnTestResult]";
            public const string burnOperator = "[burnOperator]";
            public const string burnItem_burn = "[burnItem_burn]";
            public const string burnItem_voltage13_5 = "[burnItem_voltage13-5]";
            public const string burnItem_voltage5 = "[burnItem_voltage5]";
            public const string burnItem_voltage3_3_1 = "[burnItem_voltage3-3_1]";
            public const string burnItem_voltage3_3_2 = "[burnItem_voltage3-3_2]";
            public const string burnItem_softVersion = "[burnItem_softVersion]";
            public const string sensibilityStationName = "[sensibilityStationName]";
            public const string sensibilityDateIn = "[sensibilityDateIn]";
            public const string sensibilityDateOut = "[sensibilityDateOut]";
            public const string sensibilityTestResult = "[sensibilityTestResult]";
            public const string sensibilityOperator = "[sensibilityOperator]";
            public const string sensibilityItem_workElect = "[sensibilityItem_workElect]";
            public const string sensibilityItem_partNumber = "[sensibilityItem_partNumber]";
            public const string sensibilityItem_hardwareVersion = "[sensibilityItem_hardwareVersion]";
            public const string sensibilityItem_softVersion = "[sensibilityItem_softVersion]";
            public const string sensibilityItem_EcuID = "[sensibilityItem_EcuID]";
            public const string sensibilityItem_bootloader = "[sensibilityItem_bootloader]";
            public const string sensibilityItem_radioFreq = "[sensibilityItem_radioFreq]";
            public const string sensibilityItem_dormantElect = "[sensibilityItem_dormantElect]";
            public const string shellStationName = "[shellStationName]";
            public const string shellDateIn = "[shellDateIn]";
            public const string shellDateOut = "[shellDateOut]";
            public const string shellTestResult = "[shellTestResult]";
            public const string shellOperator = "[shellOperator]";
            public const string shellItem_frontCover = "[shellItem_frontCover]";
            public const string shellItem_backCover = "[shellItem_backCover]";
            public const string shellItem_pcbScrew1 = "[shellItem_pcbScrew1]";
            public const string shellItem_pcbScrew2 = "[shellItem_pcbScrew2]";
            public const string shellItem_pcbScrew3 = "[shellItem_pcbScrew3]";
            public const string shellItem_pcbScrew4 = "[shellItem_pcbScrew4]";
            public const string shellItem_shellScrew1 = "[shellItem_shellScrew1]";
            public const string shellItem_shellScrew2 = "[shellItem_shellScrew2]";
            public const string shellItem_shellScrew3 = "[shellItem_shellScrew3]";
            public const string shellItem_shellScrew4 = "[shellItem_shellScrew4]";
            public const string airtageStationName = "[airtageStationName]";
            public const string airtageDateIn = "[airtageDateIn]";
            public const string airtageDateOut = "[airtageDateOut]";
            public const string airtageTestResult = "[airtageTestResult]";
            public const string airtageOperator = "[airtageOperator]";
            public const string airtageItem_airTest = "[airtageItem_airTest]";
            public const string stentStationName = "[stentStationName]";
            public const string stentDateIn = "[stentDateIn]";
            public const string stentDateOut = "[stentDateOut]";
            public const string stentTestResult = "[stentTestResult]";
            public const string stentOperator = "[stentOperator]";
            public const string stentItem_stentScrew1 = "[stentItem_stentScrew1]";
            public const string stentItem_stentScrew2 = "[stentItem_stentScrew2]";
            public const string stentItem_stent = "[stentItem_stent]";
            public const string stentItem_leftStent = "[stentItem_leftStent]";
            public const string stentItem_rightStent = "[stentItem_rightStent]";
            public const string productStationName = "[productStationName]";
            public const string productDateIn = "[productDateIn]";
            public const string productDateOut = "[productDateOut]";
            public const string productTestResult = "[productTestResult]";
            public const string productOperator = "[productOperator]";
            public const string productItem_workElect = "[productItem_workElect]";
            public const string productItem_dormantElect = "[productItem_dormantElect]";
            public const string productItem_inspectResult = "[productItem_inspectResult]";
        }
    }
}