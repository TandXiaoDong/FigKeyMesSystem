using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MesAPI.Model
{
    public enum InsertWIPResultEnum
    {
        STATUS_SUCCESS,
        ERR_RETROACTIVE_CODE_ISNULLOREMPTY,
        ERR_MODEL_ISNULLOREMPTY,
        ERR_STATION_ISNULLOREMPTY,
        ERR_TEST_RESULT_ISNULLOREMPTY,

    }
}