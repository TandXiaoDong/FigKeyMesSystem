using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MesManager.Model;
using CommonUtils.Logger;
using System.Configuration;

namespace MesManager.Common
{
    class InitStandConfig
    {
        private static MesServiceTest.MesServiceClient serviceClientTest;
        private static List<string> burnProductDirList = new List<string>();

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
            var defaultRoot = ConfigurationManager.AppSettings["standConfigRoot"].ToString();
            QueryCurrentStandProductDirectory(defaultRoot);
            foreach (var productType in processList)
            {
                var burnProductType = defaultRoot + StandCommon.TurnStationConfigPath + productType + "\\" + StandCommon.TurnStationFWName;
                var sensibilityProductType = defaultRoot + StandCommon.SensibilityStationConfigPath + productType;
                var shellProductType = defaultRoot + StandCommon.ShellStationConfigPath + productType;
                var airtageProductType = defaultRoot + StandCommon.AirtageStationConfigPath + productType;
                var stentProductType = defaultRoot + StandCommon.StentStationConfigPath + productType;
                var productTestProductType = defaultRoot + StandCommon.ProductFinishStationConfigPath + productType;
                var productCheckProductType = defaultRoot + StandCommon.CheckProductStationConfigPath + productType;

                CreateNewDirectory(burnProductType);
                CreateNewDirectory(sensibilityProductType);
                CreateNewDirectory(shellProductType);
                CreateNewDirectory(airtageProductType);
                CreateNewDirectory(stentProductType);
                CreateNewDirectory(productTestProductType);
                CreateNewDirectory(defaultRoot + StandCommon.CheckProductStationConfigPath + productType);
            }
        }

        private static void CreateNewDirectory(string dir)
        {

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        /// <summary>
        /// 查询所有工站下的目录
        /// </summary>
        private static void QueryCurrentStandProductDirectory(string defaultRoot)
        {
            var processList = serviceClientTest.SelectAllTProcess();
            var burnStandPath = defaultRoot + StandCommon.TurnStationConfigPath;
            var sensibilityStand = defaultRoot + StandCommon.SensibilityStationConfigPath;
            var shellStand = defaultRoot + StandCommon.ShellStationConfigPath;
            var airtageStand = defaultRoot + StandCommon.AirtageStationConfigPath;
            var stentStand = defaultRoot + StandCommon.StentStationConfigPath;
            var productTestStand = defaultRoot + StandCommon.ProductFinishStationConfigPath;
            var productCheckStand = defaultRoot + StandCommon.CheckProductStationConfigPath;

            DeleteNotExistProductPath(processList, burnStandPath);
            DeleteNotExistProductPath(processList, sensibilityStand);
            DeleteNotExistProductPath(processList, shellStand);
            DeleteNotExistProductPath(processList, airtageStand);
            DeleteNotExistProductPath(processList, stentStand);
            DeleteNotExistProductPath(processList, productTestStand);
            DeleteNotExistProductPath(processList, productCheckStand);
        }

        private static void DeleteNotExistProductPath(string[] productList,string standPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(standPath);
            DirectoryInfo[] dirInfo = directoryInfo.GetDirectories();
            FileInfo[] fileInfos = directoryInfo.GetFiles();
            foreach (var dir in dirInfo)
            {
                if (!productList.Contains(dir.Name))
                {
                    Directory.Delete(dir.FullName);
                }
            }
            foreach (var file in fileInfos)
            {
                file.Delete();
            }
        }
    }
}
