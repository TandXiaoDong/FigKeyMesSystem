using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesManager.Model
{
    class StentConfig
    {
        public const string LocalAddressConMesKey = "本机地址（连接MES）";
        public const string TestSerialKey = "测试序列";

        public string LocalAddressConMes { get; set; }

        public string TestSerial { get; set; }
    }
}
