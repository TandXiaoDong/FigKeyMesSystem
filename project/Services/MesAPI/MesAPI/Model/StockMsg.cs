using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MesAPI.Model
{
    public class StockMsg
    {
        public string MaterialCode { get; set; }

        public int ActualStock { get; set; }

        /// <summary>
        /// 使用总数
        /// </summary>
        public int UseAmounted { get; set; }
    }
}