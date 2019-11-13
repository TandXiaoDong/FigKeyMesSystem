using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MesAPI.Model
{
    public enum MaterialStockEnum
    {
        STATUS_SUCCESS,
        STATUS_NONE_MODIFY,
        STATUS_FAIL,
        ERROR_MATERIAL_IS_NOT_EXIST,
        STATUS_NOT_ZERO_STOCK,
        STATUS_STOCK_NOT_SMALLER_AMOUNTED
    }

    public enum MaterialStockState
    {
        PUT_IN_STOCK = 1,
        STOCK_USE_COMPLED = 2,
        STOCK_STATEMETN = 3,
        PUT_IN_STOCK_AND_STATEMENT = 4
    }
}