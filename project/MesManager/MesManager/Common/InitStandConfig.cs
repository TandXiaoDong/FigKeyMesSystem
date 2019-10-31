using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MesManager.Model;
using CommonUtils.Logger;

namespace MesManager.Common
{
    class InitStandConfig
    {
        private static MesServiceTest.MesServiceClient serviceClientTest;

        public static void InitDirectory()
        {
            serviceClientTest = new MesServiceTest.MesServiceClient();
            var globalConfigPath = AppDomain.CurrentDomain.BaseDirectory + CommConfig.DeafaultConfigRoot;
            if (!Directory.Exists(globalConfigPath))
                Directory.CreateDirectory(globalConfigPath);
            //根据工站新建目录：每个工站下包含所有产品型号得子目录
            var processList = serviceClientTest.SelectAllTProcess();
            if (processList.Length < 1)
                LogHelper.Log.Info("【InitStandConfig】未查询到所有产品");
            foreach (var productType in processList)
            {
                CreateNewDirectory(StandCommon.TurnStationConfigPath + productType+"\\"+StandCommon.TurnStationFWName);
                CreateNewDirectory(StandCommon.SensibilityStationConfigPath + productType);
                CreateNewDirectory(StandCommon.ShellStationConfigPath + productType);
                CreateNewDirectory(StandCommon.AirtageStationConfigPath + productType);
                CreateNewDirectory(StandCommon.StentStationConfigPath + productType);
                CreateNewDirectory(StandCommon.ProductFinishStationConfigPath + productType);
                CreateNewDirectory(StandCommon.CheckProductStationConfigPath + productType);
            }
        }

        private static void CreateNewDirectory(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }
}
