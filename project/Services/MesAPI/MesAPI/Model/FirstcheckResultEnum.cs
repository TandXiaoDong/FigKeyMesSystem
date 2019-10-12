using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MesAPI.Molde
{
    public enum FirstCheckResultEnum
    {
        /// <summary>
        /// 上次测试通过，可以测试
        /// </summary>
        STATUS_LAST_TEST_PASS = 100,
        /// <summary>
        /// 上次测试失败，不能测试
        /// </summary>
        STATUS_LAST_TEST_FAIL = 101,
        /// <summary>
        /// 判断传入站位为首站，插入成功
        /// </summary>
        STATUS_FIRST_STATION_INSERT_SUCCESS = 102,
        /// <summary>
        /// 判断传入站位为首战，插入失败
        /// </summary>
        ERR_FIRST_STATION_INSERT_FAIL = 103,
        /// <summary>
        /// 上一个站位失败
        /// </summary>
        ERR_LAST_STATION_FAIL = 104,
        /// <summary>
        /// 不是第一个站位
        /// </summary>
        ERR_NOT_FIRST_STATION = 105,
        /// <summary>
        /// 追溯码不存在
        /// </summary>
        ERR_SN_NOT_EXIST = 106,
        /// <summary>
        /// 型号不存在
        /// </summary>
        ERR_MODEL_NOT_EXIST = 107,
        /// <summary>
        /// 工站不存在
        /// </summary>
        ERR_STATION_NOT_EXIST = 108,
        /// <summary>
        /// 传入时，判断是否存在记录
        /// </summary>
        ERR_RECORD_NOT_EXIST = 109
    }
}