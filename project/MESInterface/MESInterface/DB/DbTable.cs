using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MESInterface.DB
{
    public class DbTable
    {
        public const string F_USER_NAME                 = "[WT_SCL].[dbo].[f_user]";
        public const string F_STATION_NAME              = "[WT_SCL].[dbo].[f_station]";
        public const string F_TEST_RESULT_NAME          = "[WT_SCL].[dbo].[f_test_result_data]";
        public const string F_PRODUCT_STATION_NAME      = "[WT_SCL].[dbo].[f_product_station]";
        public const string F_PRODUCT_TYPE_NO_NAME      = "[WT_SCL].[dbo].[f_product_typeNo]";
        public const string F_PRODUCT_MATERIAL_NAME     = "[WT_SCL].[dbo].[f_product_material]";
        public const string F_MATERIAL_NAME             = "[WT_SCL].[dbo].[f_material]";
        public const string F_MATERIAL_STATISTICS_NAME  = "[WT_SCL].[dbo].[f_material_statistics]";
        public const string F_OUT_CASE_STORAGE_NAME     = "[WT_SCL].[dbo].[f_out_case_storage]";
        public const string F_OUT_CASE_PRODUCT_NAME     = "[WT_SCL].[dbo].[f_out_case_product]";
        public const string F_PASS_RATE_STATISTICS_NAME = "[WT_SCL].[dbo].[f_pass_rate_statistics]";

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

        public class F_Station
        {
            public const string STATION_ORDER = "[station_order]";
            public const string STATION_NAME = "[station_name]";
        }

        public class F_Product_Station
        {
            public const string TYPE_NO = "[type_no]";
            public const string STATION_ORDER = "[station_order]";
            public const string STATION_NAME = "[station_name]";
        }

        public class F_Test_Result
        {
            public const string SN = "[sn]";
            public const string TYPE_NO = "[type_no]";
            public const string STATION_NAME = "[station_name]";
            public const string TEST_RESULT = "[test_result]";
            public const string CREATE_DATE = "[create_date]";
            public const string UPDATE_DATE = "[update_date]";
            public const string REMARK = "[remark]";
        }

        public class F_TypeNo
        {
            public const string ID = "[id]";
            public const string TYPE_NO = "[type_no]";
        }

        public class F_Material
        {
            public const string MATERIAL_CODE = "[material_code]";
            public const string MATERIAL_AMOUNT = "[amount]";
        }

        public class F_PRODUCT_MATERIAL
        {
            public const string TYPE_NO = "[type_no]";
            public const string MATERIAL_CODE = "[material_code]";
        }


        public class F_Material_Statistics
        {
            public const string SN_INNER = "[sn_inner]";
            public const string SN_OUTTER = "[sn_outter]";
            public const string TYPE_NO = "[type_no]";
            public const string STATION_NAME = "[station_name]";
            public const string MATERIAL_CODE = "[material_code]";
            public const string MATERIAL_AMOUNT = "[material_amount]";
        }

        public class F_Out_Case_Storage
        {
            public const string OUT_CASE_CODE = "[out_case_code]";
            public const string STORAGE_CAPACITY = "[storage_capacity]";
        }

        public class F_Out_Case_Product
        {
            public const string OUT_CASE_CODE = "[out_case_code]";
            public const string SN_OUTTER = "[sn_outter]";
            public const string TYPE_NO = "[type_no]";
            public const string PICTURE = "[picture]";
            public const string BINDING_STATE = "[binding_state]";
            public const string BINDING_DATE = "[binding_date]";
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
    }
}