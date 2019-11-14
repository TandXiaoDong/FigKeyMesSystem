using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MesManager.Model;
using CommonUtils.Logger;
using System.Configuration;
using System.Windows.Forms;

namespace MesManager.Common
{
    class InitStandConfig
    {
        private static MesServiceTest.MesServiceClient serviceClientTest;
        private static List<string> burnProductDirList = new List<string>();

        public enum StandConfigType
        {
            burn,
            sensibility,
            shell,
            airtage,
            stent,
            productTest,
            productCheck
        }


        public static bool InitDirectory(StandConfigType configType,bool IsCreateDirectory)
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
            var currentProcess = serviceClientTest.SelectCurrentTProcess();

            if (currentProcess == "" || currentProcess == "NULL")
            {
                MessageBox.Show("请先设置工艺流程！","提示",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return false;
            }
            //创建当前工艺文件夹
            var burnProductType = defaultRoot + StandCommon.TurnStationConfigPath + currentProcess + "\\" + StandCommon.TurnStationFWName;
            var sensibilityProductType = defaultRoot + StandCommon.SensibilityStationConfigPath + currentProcess;
            var shellProductType = defaultRoot + StandCommon.ShellStationConfigPath + currentProcess;
            var airtageProductType = defaultRoot + StandCommon.AirtageStationConfigPath + currentProcess;
            var stentProductType = defaultRoot + StandCommon.StentStationConfigPath + currentProcess;
            var productTestProductType = defaultRoot + StandCommon.ProductFinishStationConfigPath + currentProcess;
            var productCheckProductType = defaultRoot + StandCommon.CheckProductStationConfigPath + currentProcess;

            if (IsCreateDirectory)
            {
                if (configType == StandConfigType.burn)
                    CreateNewDirectory(burnProductType);
                else if (configType == StandConfigType.sensibility)
                    CreateNewDirectory(sensibilityProductType);
                else if (configType == StandConfigType.shell)
                    CreateNewDirectory(shellProductType);
                else if (configType == StandConfigType.airtage)
                    CreateNewDirectory(airtageProductType);
                else if (configType == StandConfigType.stent)
                    CreateNewDirectory(stentProductType);
                else if (configType == StandConfigType.productTest)
                    CreateNewDirectory(productTestProductType);
                else if (configType == StandConfigType.productCheck)
                    CreateNewDirectory(defaultRoot + StandCommon.CheckProductStationConfigPath + currentProcess);
            }
            return true;
        }

        public static void CreateCurrentProcessDirectory(string currentProcess,StandConfigType configType)
        {
            var defaultRoot = ConfigurationManager.AppSettings["standConfigRoot"].ToString();
            //创建当前工艺文件夹
            var burnProductType = defaultRoot + StandCommon.TurnStationConfigPath + currentProcess + "\\" + StandCommon.TurnStationFWName;
            var sensibilityProductType = defaultRoot + StandCommon.SensibilityStationConfigPath + currentProcess;
            var shellProductType = defaultRoot + StandCommon.ShellStationConfigPath + currentProcess;
            var airtageProductType = defaultRoot + StandCommon.AirtageStationConfigPath + currentProcess;
            var stentProductType = defaultRoot + StandCommon.StentStationConfigPath + currentProcess;
            var productTestProductType = defaultRoot + StandCommon.ProductFinishStationConfigPath + currentProcess;
            var productCheckProductType = defaultRoot + StandCommon.CheckProductStationConfigPath + currentProcess;

            if (configType == StandConfigType.burn)
                CreateNewDirectory(burnProductType);
            else if (configType == StandConfigType.sensibility)
                CreateNewDirectory(sensibilityProductType);
            else if (configType == StandConfigType.shell)
                CreateNewDirectory(shellProductType);
            else if (configType == StandConfigType.airtage)
                CreateNewDirectory(airtageProductType);
            else if (configType == StandConfigType.stent)
                CreateNewDirectory(stentProductType);
            else if (configType == StandConfigType.productTest)
                CreateNewDirectory(productTestProductType);
            else if (configType == StandConfigType.productCheck)
                CreateNewDirectory(defaultRoot + StandCommon.CheckProductStationConfigPath + currentProcess);
        }

        public static void CreateCurrentProcessDirectory(string currentProcess)
        {
            var defaultRoot = ConfigurationManager.AppSettings["standConfigRoot"].ToString();
            //创建当前工艺文件夹
            var burnProductType = defaultRoot + StandCommon.TurnStationConfigPath + currentProcess + "\\" + StandCommon.TurnStationFWName;
            var sensibilityProductType = defaultRoot + StandCommon.SensibilityStationConfigPath + currentProcess;
            var shellProductType = defaultRoot + StandCommon.ShellStationConfigPath + currentProcess;
            var airtageProductType = defaultRoot + StandCommon.AirtageStationConfigPath + currentProcess;
            var stentProductType = defaultRoot + StandCommon.StentStationConfigPath + currentProcess;
            var productTestProductType = defaultRoot + StandCommon.ProductFinishStationConfigPath + currentProcess;
            var productCheckProductType = defaultRoot + StandCommon.CheckProductStationConfigPath + currentProcess;
            MesServiceTest.MesServiceClient mesServiceClient = new MesServiceTest.MesServiceClient();
            var stationList = mesServiceClient.SelectStationList(currentProcess);
            foreach (var station in stationList)
            {
                if (station == GetStationName(StandCommon.TurnStationIniName))
                    CreateNewDirectory(burnProductType);
                else if (station == GetStationName(StandCommon.SensibilityStationIniName))
                    CreateNewDirectory(sensibilityProductType);
                else if (station == GetStationName(StandCommon.ShellStationIniName))
                    CreateNewDirectory(shellProductType);
                else if (station == GetStationName(StandCommon.AirtageStationIniName))
                    CreateNewDirectory(airtageProductType);
                else if (station == GetStationName(StandCommon.StentStationIniName))
                    CreateNewDirectory(stentProductType);
                else if (station == GetStationName(StandCommon.ProductFinishStationIniName))
                    CreateNewDirectory(productTestProductType);
                else if (station == GetStationName(StandCommon.CheckProductStationIniName))
                    CreateNewDirectory(defaultRoot + StandCommon.CheckProductStationConfigPath + currentProcess);
            }
        }

        private static string GetStationName(string stationName)
        {
            return stationName.Substring(0,stationName.IndexOf('_'));
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
        public static void QueryCurrentStandProductDirectory(string defaultRoot)
        {
            if(defaultRoot == "")
                defaultRoot = ConfigurationManager.AppSettings["standConfigRoot"].ToString();
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
            if (!Directory.Exists(standPath))
                return;
            DirectoryInfo directoryInfo = new DirectoryInfo(standPath);
            DirectoryInfo[] dirInfo = directoryInfo.GetDirectories();
            FileInfo[] fileInfos = directoryInfo.GetFiles();
            foreach (var dir in dirInfo)
            {
                if (!productList.Contains(dir.Name))
                {
                    Directory.Delete(dir.FullName,true);
                }
            }
            foreach (var file in fileInfos)
            {
                file.Delete();
            }
        }
    }
}
