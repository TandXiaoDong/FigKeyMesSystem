using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MesAPI.DB
{
    public class DbTable
    {
        public const string F_USER_NAME                         = "[WT_SCL].[dbo].[f_user]";
        public const string F_TECHNOLOGICAL_PROCESS_NAME        = "[WT_SCL].[dbo].[f_technological_process]";
        public const string F_TEST_RESULT_NAME                  = "[WT_SCL].[dbo].[f_test_result_data]";
        public const string F_PRODUCT_STATION_NAME              = "[WT_SCL].[dbo].[f_product_station]";
        //public const string F_PRODUCT_TYPE_NO_NAME = "[WT_SCL].[dbo].[f_product_typeNo]";
        public const string F_PRODUCT_MATERIAL_NAME             = "[WT_SCL].[dbo].[f_product_material]";
        public const string F_MATERIAL_NAME                     = "[WT_SCL].[dbo].[f_material]";
        public const string F_MATERIAL_STATISTICS_NAME          = "[WT_SCL].[dbo].[f_material_statistics]";
        public const string F_PRODUCT_PACKAGE_STORAGE_NAME      = "[WT_SCL].[dbo].[f_product_package_storage]";
        public const string F_PRODUCT_PACKAGE_NAME              = "[WT_SCL].[dbo].[f_product_package]";
        public const string F_PRODUCT_CHECK_RECORD_NAME         = "[WT_SCL].[dbo].[f_product_check_record]";
        public const string F_PASS_RATE_STATISTICS_NAME         = "[WT_SCL].[dbo].[f_pass_rate_statistics]";
        public const string F_TEST_PROGRAME_VERSION_NAME        = "[WT_SCL].[dbo].[f_test_programe_version]";
        public const string F_TEST_LIMIT_CONFIG_NAME            = "[WT_SCL].[dbo].[f_test_limit_config]";
        public const string F_TEST_LOG_DATA_NAME                = "[WT_SCL].[dbo].[f_test_log_data]";
        public const string F_QUANLITY_MANAGER_NAME             = "[WT_SCL].[dbo].[f_quanlity_manager]";
        public const string F_MATERIAL_PN_NAME                  = "[WT_SCL].[dbo].[f_material_pn]";
        public const string F_BINDING_PCBA_NAME                 = "[WT_SCL].[dbo].[f_binding_pcba]";

        public class F_User
        {
            public const string USER_NAME = "[username]";
            public const string PASS_WORD = "[password]";
            public const string PHONE = "[phone]";
            public const string EMAIL = "[email]";
            public const string CREATE_DATE = "[create_date]";
            public const string UPDATE_DATE = "[update_date]";
            public const string STATUS = "[status]";
            public const string ROLE_NAME = "[role_name]";
        }

        public class F_TECHNOLOGICAL_PROCESS
        {
            public const string PROCESS_NAME = "[process_name]";
            public const string STATION_ORDER = "[station_order]";
            public const string STATION_NAME = "[station_name]";
            public const string UPDATE_DATE = "[update_date]";
            public const string USER_NAME = "[username]";
            public const string PROCESS_STATE = "[pstate]";
        }

        public class F_Product_Station
        {
            public const string TYPE_NO = "[type_no]";
            public const string STATION_ORDER = "[station_order]";
            public const string STATION_NAME = "[station_name]";
        }

        public class F_Test_Result
        {
            public const string PROCESS_NAME = "[process_name]";
            public const string SN = "[sn]";
            public const string TYPE_NO = "[type_no]";
            public const string STATION_NAME = "[station_name]";
            public const string TEST_RESULT = "[test_result]";
            public const string STATION_IN_DATE = "[station_in_date]";
            public const string STATION_OUT_DATE = "[station_out_date]";
            public const string CREATE_DATE = "[create_date]";
            public const string UPDATE_DATE = "[update_date]";
            public const string REMARK = "[remark]";
            public const string TEAM_LEADER = "team_leader";
            public const string ADMIN = "[admin]";
            public const string JOIN_DATE_TIME = "[joinDateTime]";
        }

        public class F_TypeNo
        {
            public const string ID = "[id]";
            public const string TYPE_NO = "[type_no]";
        }

        public class F_Material
        {
            public const string MATERIAL_CODE = "[material_code]";
            public const string MATERIAL_NAME = "[material_name]";
            public const string MATERIAL_STOCK = "[material_stock]";
            public const string MATERIAL_AMOUNTED = "[material_amounted]";
            public const string MATERIAL_ACTUAL_AMOUNT = "[material_actualAmount]";
            public const string MATERIAL_BREAK_AMOUNT = "[material_breakAmount]";
            public const string MATERIAL_STATE = "[material_state]";
            public const string MATERIAL_DESCRIBLE = "[material_describle]";
            public const string MATERIAL_USERNAME = "[material_username]";
            public const string MATERIAL_UPDATE_DATE = "[material_update_date]";

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
            public const string TYPE_NO = "[type_no]";
            public const string MATERIAL_CODE = "[material_code]";//material_pn
            public const string STOCK = "[stock]";
            public const string AMOUNTED = "[amounted]";
            public const string Describle = "[describle]";
            public const string UpdateDate = "[update_date]";
            public const string USERNAME = "[username]";
        }

        public class F_Material_Statistics
        {
            public const string PRODUCT_TYPE_NO = "[type_no]";
            public const string STATION_NAME = "[station_name]";
            public const string MATERIAL_CODE = "[material_code]";
            public const string MATERIAL_AMOUNT = "[material_amounted]";
            public const string TEAM_LEADER = "[team_leader]";
            public const string ADMIN = "[admin]";
            public const string UPDATE_DATE = "[update_date]";
            public const string PCBA_SN = "[pcbaSN]";
            public const string MATERIAL_CURRENT_REMAIN = "[material_current_remain]";
        }

        public class F_PRODUCT_PACKAGE_STORAGE
        {
            public const string PRODUCT_TYPE_NO = "[type_no]";
            public const string USER_NAME = "[username]";
            public const string STORAGE_CAPACITY = "[storage_capacity]";
            public const string UPDATE_DATE_U = "[update_date_u]";
            public const string DESCRIBLE = "[describle]";
        }

        public class F_PRODUCT_PACKAGE
        {
            public const string OUT_CASE_CODE = "[out_case_code]";
            public const string SN_OUTTER = "[sn_outter]";
            public const string TYPE_NO = "[type_no]";
            public const string PICTURE = "[picture]";
            public const string BINDING_STATE = "[binding_state]";
            public const string BINDING_DATE = "[binding_date]";
            public const string REMARK = "[remark]";
            public const string TEAM_LEADER = "[team_leader]";
            public const string ADMIN = "[admin]";
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
            public const string OUT_CASE_CODE = "[out_case_code]";
            public const string SN_OUTTER = "[sn_outter]";
            public const string TYPE_NO = "[type_no]";
            public const string PRIORITY = "[priority]";
            public const string AMOUNT = "[amount]";
            public const string STORAGE_CAPACITY = "[storage_capacity]";
            public const string UPDATE_DATE = "[update_date]";
        }

        public class F_TEST_PROGRAME_VERSION
        {
            public const string TYPE_NO = "[type_no]";
            public const string STATION_NAME = "[station_name]";
            public const string PROGRAME_NAME = "[programe_name]";
            public const string PROGRAME_VERSION = "[programe_version]";
            public const string TEAM_LEADER = "[team_leader]";
            public const string ADMIN = "[admin]";
            public const string UPDATE_DATE = "[update_date]";
        }

        public class F_TEST_LIMIT_CONFIG
        {
            public const string STATION_NAME = "[station_name]";
            public const string TYPE_NO = "[type_no]";
            public const string TEST_ITEM = "test_item";
            public const string LIMIT = "[limit]";
            public const string TEAM_LEADER = "[team_leader]";
            public const string ADMIN = "[admin]";
            public const string UPDATE_DATE = "[update_date]";
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

        public class F_QUANLITY_MANAGER
        {
            public const string EXCEPT_TYPE = "[except_type]";
            public const string MATERIAL_CODE = "[material_code]";
            public const string STATEMENT_DATE = "[statement_date]";
            public const string EXCEPT_STOCK = "[except_stock]";
            public const string ACTUAL_STOCK = "[actual_stock]";
            public const string STATION_NAME = "[station_name]";
            public const string MATERIAL_STATE = "[material_state]";
            public const string STATEMENT_REASON = "[statement_reason]";
            public const string STATEMENT_USER = "[statement_user]";
            public const string UPDATE_DATE = "[update_date]";
        }

        public class F_BINDING_PCBA
        {
            public const string PRODUCT_TYPE_NO = "[type_no]";
            public const string SN_PCBA = "[sn_pcba]";
            public const string SN_OUTTER = "[sn_outter]";
            public const string MATERIAL_CODE = "[material_code]";
            public const string UPDATE_DATE = "update_date";
        }
    }
}