﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using CommonUtils.Logger;
using CommonUtils.FileHelper;
using MesManager.Model;

namespace MesManager.Common
{
    class StandConfigData
    {
        private StandCommon standCommon;
        private CommConfig commConfig;
        private BurnConfig burnConfig;
        private SensibilityConfig sensibilityConfig;
        private ShellConfig shellConfig;
        private StentConfig stentConfig;
        private ProductTestConfig productTestConfig;
        private ProductCheckConfig productCheckConfig;
        private AirtageConfig airtageConfig;
        private string standMapRoot;
        private string defaultRoot;
        private List<BurnConfig> burnConfigList = new List<BurnConfig>();
        private List<SensibilityConfig> sensibilityConfigList = new List<SensibilityConfig>();
        private List<ShellConfig> shellConfigList = new List<ShellConfig>();
        private List<AirtageConfig> airtageConfigList = new List<AirtageConfig>();
        private List<StentConfig> stentConfigList = new List<StentConfig>();
        private List<ProductTestConfig> productTestConfigList = new List<ProductTestConfig>();
        private List<ProductCheckConfig> productCheckConfigList = new List<ProductCheckConfig>();
        private bool IsSavePrivateConfig;

        public StandConfigData(StandCommon standCommon,CommConfig commConfig, BurnConfig burnConfig, SensibilityConfig sensibility,ShellConfig shellConfig,StentConfig stentConfig,
            ProductTestConfig productTest,ProductCheckConfig checkConfig,AirtageConfig airtage)
        {
            this.commConfig = commConfig;
            this.burnConfig = burnConfig;
            this.sensibilityConfig = sensibility;
            this.shellConfig = shellConfig;
            this.stentConfig = stentConfig;
            this.productTestConfig = productTest;
            this.productCheckConfig = checkConfig;
            this.airtageConfig = airtage;
            Init();
        }

        public StandConfigData()
        {
            standCommon = new StandCommon();
            commConfig = new CommConfig();
            burnConfig = new BurnConfig();
            sensibilityConfig = new SensibilityConfig();
            shellConfig = new ShellConfig();
            stentConfig = new StentConfig();
            airtageConfig = new AirtageConfig();
            productCheckConfig = new ProductCheckConfig();
            productTestConfig = new ProductTestConfig();
            Init();
        }

        private void Init()
        {
            var serialPath = AppDomain.CurrentDomain.BaseDirectory + CommConfig.DeafaultConfigRoot + CommConfig.TestStandSerialNumber;
            //read file content
            int serialCount;
            //读取烧录序列
            int.TryParse(INIFile.GetValue(CommConfig.BurnStationSection, CommConfig.CountKey, serialPath).ToString(), out serialCount);
            for (int i = 0; i < serialCount; i++)
            {
                BurnConfig burnConfig = new BurnConfig();
                var serialValue = INIFile.GetValue(CommConfig.BurnStationSection, CommConfig.SerialNumberKey + i, serialPath).ToString();
                var serialVoltage = INIFile.GetValue(CommConfig.BurnStationSection, CommConfig.SerialVoltageKey + i, serialPath).ToString();
                burnConfig.ProductSerialPath = serialValue;
                burnConfig.SerialNumber = GetProductTestSerial(serialValue);
                burnConfig.SupplyVoltage = serialVoltage;
                burnConfigList.Add(burnConfig);
            }

            //读取灵敏度序列
            int.TryParse(INIFile.GetValue(CommConfig.SensibilityStationSection, CommConfig.CountKey, serialPath).ToString(), out serialCount);
            for (int i = 0; i < serialCount; i++)
            {
                SensibilityConfig sensibilityConfig = new SensibilityConfig();
                var serialValue = INIFile.GetValue(CommConfig.SensibilityStationSection, CommConfig.SerialNumberKey + i, serialPath).ToString();
                var serialVoltage = INIFile.GetValue(CommConfig.SensibilityStationSection, CommConfig.SerialVoltageKey + i, serialPath).ToString();
                sensibilityConfig.ProductSerialPath = serialValue;
                sensibilityConfig.ProductSerial = GetProductTestSerial(serialValue);
                sensibilityConfig.SupplyVoltage = serialVoltage;
                sensibilityConfigList.Add(sensibilityConfig);
            }
            //读取外壳装配序列
            int.TryParse(INIFile.GetValue(CommConfig.ShellStationSection, CommConfig.CountKey, serialPath).ToString(), out serialCount);
            for (int i = 0; i < serialCount; i++)
            {
                ShellConfig shellConfig = new ShellConfig();
                var serialValue = INIFile.GetValue(CommConfig.ShellStationSection, CommConfig.SerialNumberKey + i, serialPath).ToString();
                var serialVoltage = INIFile.GetValue(CommConfig.ShellStationSection, CommConfig.SerialVoltageKey + i, serialPath).ToString();
                shellConfig.ProductSerialPath = serialValue;
                shellConfig.TestSerialNumber = GetProductTestSerial(serialValue);
                shellConfig.SupplyVoltage = serialVoltage;
                shellConfigList.Add(shellConfig);
            }
            //读取气密序列
            int.TryParse(INIFile.GetValue(CommConfig.AirtageStationSection, CommConfig.CountKey, serialPath).ToString(), out serialCount);
            for (int i = 0; i < serialCount; i++)
            {
                AirtageConfig airtageConfig = new AirtageConfig();
                var serialValue = INIFile.GetValue(CommConfig.AirtageStationSection, CommConfig.SerialNumberKey + i, serialPath).ToString();
                var serialVoltage = INIFile.GetValue(CommConfig.AirtageStationSection, CommConfig.SerialVoltageKey + i, serialPath).ToString();
                airtageConfig.ProductSerialPath = serialValue;
                airtageConfig.TestSerial = GetProductTestSerial(serialValue);
                airtageConfig.SupplyVoltage = serialVoltage;
                airtageConfigList.Add(airtageConfig);
            }
            //读取支架装配序列
            int.TryParse(INIFile.GetValue(CommConfig.StentStationSection, CommConfig.CountKey, serialPath).ToString(), out serialCount);
            for (int i = 0; i < serialCount; i++)
            {
                StentConfig stentConfig = new StentConfig();
                var serialValue = INIFile.GetValue(CommConfig.StentStationSection, CommConfig.SerialNumberKey + i, serialPath).ToString();
                var serialVoltage = INIFile.GetValue(CommConfig.StentStationSection, CommConfig.SerialVoltageKey + i, serialPath).ToString();
                stentConfig.ProductSerialPath = serialValue;
                stentConfig.TestSerial = GetProductTestSerial(serialValue);
                stentConfig.SupplyVoltage = serialVoltage;
                stentConfigList.Add(stentConfig);
            }
            //读取成品测试序列
            int.TryParse(INIFile.GetValue(CommConfig.ProductFinishStationSection, CommConfig.CountKey, serialPath).ToString(), out serialCount);
            for (int i = 0; i < serialCount; i++)
            {
                ProductTestConfig productTestConfig = new ProductTestConfig();
                var serialValue = INIFile.GetValue(CommConfig.ProductFinishStationSection, CommConfig.SerialNumberKey + i, serialPath).ToString();
                var serialVoltage = INIFile.GetValue(CommConfig.ProductFinishStationSection, CommConfig.SerialVoltageKey + i, serialPath).ToString();
                productTestConfig.ProductSerialPath = serialValue;
                productTestConfig.TestSerial = GetProductTestSerial(serialValue);
                productTestConfig.SupplyVoltage = serialVoltage;
                productTestConfigList.Add(productTestConfig);
            }
            //读取成品抽检序列
            int.TryParse(INIFile.GetValue(CommConfig.CheckProductStationSection, CommConfig.CountKey, serialPath).ToString(), out serialCount);
            for (int i = 0; i < serialCount; i++)
            {
                ProductCheckConfig productCheckConfig = new ProductCheckConfig();
                var serialValue = INIFile.GetValue(CommConfig.CheckProductStationSection, CommConfig.SerialNumberKey + i, serialPath).ToString();
                var serialVoltage = INIFile.GetValue(CommConfig.CheckProductStationSection, CommConfig.SerialVoltageKey + i, serialPath).ToString();
                productCheckConfig.ProductSerialPath = serialValue;
                productCheckConfig.TestSerial = GetProductTestSerial(serialValue);
                productCheckConfig.SupplyVoltage = serialVoltage;
                productCheckConfigList.Add(productCheckConfig);
            }
            //读取机台映射根目录
            standMapRoot = ConfigurationManager.AppSettings["standMapRoot"].ToString();
            if (standMapRoot == "")
                standMapRoot = "Z";
            defaultRoot = ConfigurationManager.AppSettings["standConfigRoot"].ToString();
            if (defaultRoot == "")
                defaultRoot = "F";
        }

        public void ReadProductTypeConfigToSaveAs(string readTypeNo,string saveAsTypeNo)
        {
            standCommon.ProductTypeNo = readTypeNo;
            //read config  查询每个工站路径是否存在
            //1）不存在，则不创建新路径，不删除旧路径
            //2）存在，则创建新路径，同时删除旧路径
            ReadStandConfig(saveAsTypeNo);
            LogHelper.Log.Info("【修改产品型号-修改配置型号】读取数据完成-产品型号="+readTypeNo);
            standCommon.ProductTypeNo = saveAsTypeNo;
            //创建新型号目录
            //InitStandConfig.CreateCurrentProcessDirectory(saveAsTypeNo);
            SaveStandConfig();
            LogHelper.Log.Info("【修改产品型号-修改配置型号】保存数据完成-产品型号=" + saveAsTypeNo);
        }

        #region read stand config 

        public void ReadStandConfig(string newTypeNo)
        {
            ReadCommonStandConfig();
            ReadBurnStandConfig(newTypeNo);
            ReadSensibilityStandConfig(newTypeNo);
            ReadShellStandConfig(newTypeNo);
            ReadAirtageStandConfig(newTypeNo);
            ReadStentStandConfig(newTypeNo);
            ReadProductTestStandConfig(newTypeNo);
            ReadProductCheckStandConfig(newTypeNo);
        }

        /// <summary>
        /// 读取当前型号得所有工站配置
        /// </summary>
        public void ReadBurnStandConfig(string newTypeNo)
        {
            //读取烧录配置
            //var burnSavePath = AppDomain.CurrentDomain.BaseDirectory + CommConfig.DeafaultConfigRoot + CommConfig.BurnConfigIniName;
            //var burnCurrentPath = INIFile.GetValue(standCommon.ProductTypeNo, CommConfig.BurnConfigPathKey,burnSavePath);
            //根据路径读取配置
            var burnInitPath = defaultRoot + StandCommon.TurnStationConfigPath + standCommon.ProductTypeNo + "\\";
            var burnFileName = StandCommon.TurnStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var burnSavePath = burnInitPath + burnFileName;
            if (!Directory.Exists(burnInitPath))
                return;
            if (!File.Exists(burnSavePath))
                return;
            InitStandConfig.CreateCurrentProcessDirectory(newTypeNo,InitStandConfig.StandConfigType.burn);//创建新路径
            burnConfig.PowerValue = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.PowerValueKey, burnSavePath).ToString();
            burnConfig.LocalAddress = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.LocalAddressKey, burnSavePath).ToString();
            burnConfig.AvometerAddress = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.AvometerAddressKey, burnSavePath).ToString();
            burnConfig.AutoSweepCodeCom = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.AutoSweepCodeComKey, burnSavePath).ToString();
            burnConfig.Burner = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.BurnerKey, burnSavePath).ToString();
            burnConfig.PorterRate = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.PorterRateKey, burnSavePath).ToString();
            burnConfig.CanId = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.CanIdKey, burnSavePath).ToString();
            burnConfig.ProductId = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.ProductIdKey, burnSavePath).ToString();
            burnConfig.FirstVoltageMax = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.FirstVoltageMaxKey, burnSavePath).ToString();
            burnConfig.FirstVoltageMin = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.FirstVoltageMinKey, burnSavePath).ToString();
            burnConfig.SecondVoltageMax = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.SecondVoltageMaxKey, burnSavePath).ToString();
            burnConfig.SecondVoltageMin = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.SecondVoltageMinKey, burnSavePath).ToString();
            burnConfig.HardWareVersion = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.HardWareVersionKey, burnSavePath).ToString();
            burnConfig.SoftWareVersion = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.SoftWareVersionKey, burnSavePath).ToString();
            burnConfig.PartNumber = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.PartNumberKey, burnSavePath).ToString();
            burnConfig.ProgrameActualPath = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.ProgrameActualPathKey, burnSavePath).ToString();
            burnConfig.ProgrameName = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.ProgrameNameKey, burnSavePath).ToString();
            burnConfig.SendCanID = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.SendCanIDKey, burnSavePath).ToString();
            burnConfig.SerialNumber = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.SerialNumberKey, burnSavePath).ToString();
            burnConfig.SerialNumber = GetProductTestSerial(burnConfig.SerialNumber);//更加路径返回序列名
            //删除旧文件
            Directory.Delete(burnInitPath,true);
        }

        public void ReadCommonStandConfig()
        {
            var initPath = defaultRoot + StandCommon.TurnStationConfigPath + standCommon.ProductTypeNo + "\\";
            var fileName = StandCommon.TurnStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var savePath = initPath + fileName;
            //读取烧录工站通用配置
            ReadCommonConfig(savePath);
            if (standCommon.PCBABarCodeLength == "" && standCommon.CaseBarCodeLength == "" && standCommon.ShellBarCodeLength == "" && standCommon.PackageCaseAmount == "")
            {
                initPath = defaultRoot + StandCommon.SensibilityStationConfigPath + standCommon.ProductTypeNo + "\\";
                fileName = StandCommon.SensibilityStationIniName + standCommon.ProductTypeNo + "_config.ini";
                savePath = initPath + fileName;
                //读取灵敏度通用配置
                ReadCommonConfig(savePath);
            }
            if (standCommon.PCBABarCodeLength == "" && standCommon.CaseBarCodeLength == "" && standCommon.ShellBarCodeLength == "" && standCommon.PackageCaseAmount == "")
            {
                //读取外壳通用配置
                initPath = defaultRoot + StandCommon.ShellStationConfigPath + standCommon.ProductTypeNo + "\\";
                fileName = StandCommon.ShellStationIniName + standCommon.ProductTypeNo + "_config.ini";
                savePath = initPath + fileName;
                ReadCommonConfig(savePath);
            }
            if (standCommon.PCBABarCodeLength == "" && standCommon.CaseBarCodeLength == "" && standCommon.ShellBarCodeLength == "" && standCommon.PackageCaseAmount == "")
            {
                //读取气密通用配置
                initPath = defaultRoot + StandCommon.AirtageStationConfigPath + standCommon.ProductTypeNo + "\\";
                fileName = StandCommon.AirtageStationIniName + standCommon.ProductTypeNo + "_config.ini";
                savePath = initPath + fileName;
                ReadCommonConfig(savePath);
            }
            if (standCommon.PCBABarCodeLength == "" && standCommon.CaseBarCodeLength == "" && standCommon.ShellBarCodeLength == "" && standCommon.PackageCaseAmount == "")
            {
                //读取支架通用配置
                initPath = defaultRoot + StandCommon.StentStationConfigPath + standCommon.ProductTypeNo + "\\";
                fileName = StandCommon.StentStationIniName + standCommon.ProductTypeNo + "_config.ini";
                savePath = initPath + fileName;
                ReadCommonConfig(savePath);
            }
            if (standCommon.PCBABarCodeLength == "" && standCommon.CaseBarCodeLength == "" && standCommon.ShellBarCodeLength == "" && standCommon.PackageCaseAmount == "")
            {
                //读取成品测试通用配置
                initPath = defaultRoot + StandCommon.ProductFinishStationConfigPath + standCommon.ProductTypeNo + "\\";
                fileName = StandCommon.ProductFinishStationIniName + standCommon.ProductTypeNo + "_config.ini";
                savePath = initPath + fileName;
                ReadCommonConfig(savePath);
            }
            if (standCommon.PCBABarCodeLength == "" && standCommon.CaseBarCodeLength == "" && standCommon.ShellBarCodeLength == "" && standCommon.PackageCaseAmount == "")
            {
                //读取成品抽检通用配置
                initPath = defaultRoot + StandCommon.CheckProductStationConfigPath + standCommon.ProductTypeNo + "\\";
                fileName = StandCommon.CheckProductStationIniName + standCommon.ProductTypeNo + "_config.ini";
                savePath = initPath + fileName;
                ReadCommonConfig(savePath);
            }
        }

        public void ReadCommonConfig(string savePath)
        {
            standCommon.PCBABarCodeLength = INIFile.GetValue(standCommon.ProductTypeNo, StandCommon.PCBABarCodeLengthKey, savePath).ToString();
            standCommon.CaseBarCodeLength = INIFile.GetValue(standCommon.ProductTypeNo, StandCommon.CaseBarCodeLengthKey, savePath).ToString();
            standCommon.ShellBarCodeLength = INIFile.GetValue(standCommon.ProductTypeNo, StandCommon.ShellBarCodeLengthKey, savePath).ToString();
            standCommon.PackageCaseAmount = INIFile.GetValue(standCommon.ProductTypeNo, StandCommon.PackageCaseAmountKey, savePath).ToString();
        }

        public void ReadSensibilityStandConfig(string newTypeNo)
        {
            var senInitPath = defaultRoot + StandCommon.SensibilityStationConfigPath + standCommon.ProductTypeNo + "\\";
            var senFileName = StandCommon.SensibilityStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var senSavePath = senInitPath + senFileName;
            if (!Directory.Exists(senInitPath))
                return;
            if (!File.Exists(senSavePath))
                return;
            InitStandConfig.CreateCurrentProcessDirectory(newTypeNo, InitStandConfig.StandConfigType.sensibility);//创建新路径
            sensibilityConfig.PLCAddress = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.PLCAddressKey, senSavePath).ToString();
            sensibilityConfig.LocalAddress = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.LocalAddressKey, senSavePath).ToString();
            sensibilityConfig.AvometerAddress = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.AvometerAddressKey, senSavePath).ToString();
            sensibilityConfig.AutoSweepCode = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.AutoSweepCodeKey, senSavePath).ToString();
            sensibilityConfig.ProgrameControlPower = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.ProgrameControlPowerKey, senSavePath).ToString();
            sensibilityConfig.WorkElectricMin = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.WorkElectricMinKey, senSavePath).ToString();
            sensibilityConfig.WorkElectricMax = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.WorkElectricMaxKey, senSavePath).ToString();
            sensibilityConfig.PartNumber = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.PartNumberKey, senSavePath).ToString();
            sensibilityConfig.HardWareVersion = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.HardWareVersionKey, senSavePath).ToString();
            sensibilityConfig.SoftWareVersion = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.SoftWareVersionKey, senSavePath).ToString();
            sensibilityConfig.DormantElectricMin = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.DormantElectricMinKey, senSavePath).ToString();
            sensibilityConfig.DormantElectricMax = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.DormantElectricMaxKey, senSavePath).ToString();
            sensibilityConfig.BootLoader = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.BootLoaderKey, senSavePath).ToString();
            sensibilityConfig.PorterRate = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.PorterRateKey, senSavePath).ToString();
            sensibilityConfig.SendCanID = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.SendCanIDKey, senSavePath).ToString();
            sensibilityConfig.ReceiveCanID = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.ReceiveCanIDKey, senSavePath).ToString();
            sensibilityConfig.CyclyCanID = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.CyclyCanIDKey, senSavePath).ToString();
            sensibilityConfig.RfCanID = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.RfCanIDKey, senSavePath).ToString();
            sensibilityConfig.ProductSerial = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.ProductSerialKey, senSavePath).ToString();
            sensibilityConfig.ProductSerial = GetProductTestSerial(sensibilityConfig.ProductSerial);
            sensibilityConfig.ProductId = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.ProductIdKey, senSavePath).ToString();
            sensibilityConfig.RadioFrequcy = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.RadioFrequcyKey, senSavePath).ToString();
            Directory.Delete(senInitPath, true);
        }

        public void ReadShellStandConfig(string newTypeNo)
        {
            var shellInitPath = defaultRoot + StandCommon.ShellStationConfigPath + standCommon.ProductTypeNo + "\\";
            var shellFileName = StandCommon.ShellStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var shellSavePath = shellInitPath + shellFileName;
            if (!Directory.Exists(shellInitPath))
                return;
            if (!File.Exists(shellSavePath))
                return;
            InitStandConfig.CreateCurrentProcessDirectory(newTypeNo, InitStandConfig.StandConfigType.shell);//创建新路径
            shellConfig.LocalAddressConMes = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.LocalAddressConMesKey, shellSavePath).ToString();
            shellConfig.LocalAddressConPLC = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.LocalAddressConPLCKey, shellSavePath).ToString();
            shellConfig.PLCAddress = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.PLCAddressKey, shellSavePath).ToString();
            shellConfig.SmallScrewSetTime = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.SmallScrewSetTimeKey, shellSavePath).ToString();
            shellConfig.LargeScrewSetTime = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.LargeScrewSetTimeKey, shellSavePath).ToString();
            shellConfig.FrontCover = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.FrontCoverKey, shellSavePath).ToString();
            shellConfig.BackCover = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.BackCoverKey, shellSavePath).ToString();
            shellConfig.PCBScrew = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.PCBScrewKey, shellSavePath).ToString();
            shellConfig.ShellScrew = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.ShellScrewKey, shellSavePath).ToString();
            shellConfig.TopCover = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.TopCoverKey, shellSavePath).ToString();
            shellConfig.Shell = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.ShellKey, shellSavePath).ToString();
            shellConfig.SealRingWire = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.SealRingWireKey, shellSavePath).ToString();
            shellConfig.BubbleCotton = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.BubbleCottonKey, shellSavePath).ToString();
            shellConfig.TestSerialNumber = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.TestSerialNumberKey, shellSavePath).ToString();
            shellConfig.TestSerialNumber = GetProductTestSerial(shellConfig.TestSerialNumber);
            Directory.Delete(shellInitPath, true);
        }

        public void ReadAirtageStandConfig(string newTypeNo)
        {
            var airtageInitPath = defaultRoot + StandCommon.AirtageStationConfigPath + standCommon.ProductTypeNo + "\\";
            var airtageFileName = StandCommon.AirtageStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var airtageSavePath = airtageInitPath + airtageFileName;
            if (!Directory.Exists(airtageInitPath))
                return;
            if (!File.Exists(airtageSavePath))
                return;
            InitStandConfig.CreateCurrentProcessDirectory(newTypeNo, InitStandConfig.StandConfigType.airtage);//创建新路径
            airtageConfig.LocalAddressConMes = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.LocalAddressConMesKey, airtageSavePath).ToString();
            airtageConfig.AirTester = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.AirTesterKey, airtageSavePath).ToString();
            airtageConfig.InflateAirTime = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.InflateAirTimeKey, airtageSavePath).ToString();
            airtageConfig.StableTime = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.StableTimeKey, airtageSavePath).ToString();
            airtageConfig.PressureUnit = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.PressureUnitKey, airtageSavePath).ToString();
            airtageConfig.SpreadUnit = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.SpreadUnitKey, airtageSavePath).ToString();
            airtageConfig.MaxInflate = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.MaxInflateKey, airtageSavePath).ToString();
            airtageConfig.MinInflate = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.MinInflateKey, airtageSavePath).ToString();
            airtageConfig.LevelMin = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.LevelMinKey, airtageSavePath).ToString();
            airtageConfig.LevelMax = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.LevelMaxKey, airtageSavePath).ToString();
            airtageConfig.BigLeakMax = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.BigLeakMaxKey, airtageSavePath).ToString();
            airtageConfig.BigLeakMin = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.BigLeakMinKey, airtageSavePath).ToString();
            airtageConfig.TestTime = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.TestTimeKey, airtageSavePath).ToString();
            airtageConfig.TestSerial = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.TestSerialKey, airtageSavePath).ToString();
            airtageConfig.TestSerial = GetProductTestSerial(airtageConfig.TestSerial);
            Directory.Delete(airtageInitPath, true);
        }

        public void ReadStentStandConfig(string newTypeNo)
        {
            var stentInitPath = defaultRoot + StandCommon.StentStationConfigPath + standCommon.ProductTypeNo + "\\";
            var stentFileName = StandCommon.StentStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var stentSavePath = stentInitPath + stentFileName;
            if (!Directory.Exists(stentInitPath))
                return;
            if (!File.Exists(stentSavePath))
                return;
            InitStandConfig.CreateCurrentProcessDirectory(newTypeNo, InitStandConfig.StandConfigType.stent);//创建新路径
            stentConfig.LocalAddressConMes = INIFile.GetValue(standCommon.ProductTypeNo, StentConfig.LocalAddressConMesKey, stentSavePath).ToString();
            stentConfig.LeftStent = INIFile.GetValue(standCommon.ProductTypeNo, StentConfig.LeftStentKey, stentSavePath).ToString();
            stentConfig.RightStent = INIFile.GetValue(standCommon.ProductTypeNo, StentConfig.RightStentKey, stentSavePath).ToString();
            stentConfig.UnionStent = INIFile.GetValue(standCommon.ProductTypeNo, StentConfig.UnionStentKey, stentSavePath).ToString();
            stentConfig.Stent = INIFile.GetValue(standCommon.ProductTypeNo, StentConfig.StentKey, stentSavePath).ToString();
            stentConfig.StentScrew = INIFile.GetValue(standCommon.ProductTypeNo, StentConfig.StentSrcrewKey, stentSavePath).ToString();
            stentConfig.StentNut = INIFile.GetValue(standCommon.ProductTypeNo, StentConfig.StentNutKey, stentSavePath).ToString();
            stentConfig.TestSerial = INIFile.GetValue(standCommon.ProductTypeNo, StentConfig.TestSerialKey, stentSavePath).ToString();
            stentConfig.TestSerial = GetProductTestSerial(stentConfig.TestSerial);
            Directory.Delete(stentInitPath,true);
        }

        public void ReadProductTestStandConfig(string newTypeNo)
        {
            var productInitPath = defaultRoot + StandCommon.ProductFinishStationConfigPath + standCommon.ProductTypeNo + "\\";
            var productFileName = StandCommon.ProductFinishStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var productSavePath = productInitPath + productFileName;
            if (!Directory.Exists(productInitPath))
                return;
            if (!File.Exists(productSavePath))
                return;
            InitStandConfig.CreateCurrentProcessDirectory(newTypeNo, InitStandConfig.StandConfigType.productTest);//创建新路径
            productTestConfig.PlcAddress = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.PlcAddressKey, productSavePath).ToString();
            productTestConfig.LocalAddress = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.LocalAddressKey, productSavePath).ToString();
            productTestConfig.Avometer = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.AvometerKey, productSavePath).ToString();
            productTestConfig.AutoSweepCode = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.AutoSweepCodeKey, productSavePath).ToString();
            productTestConfig.TestBoard = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.TestBoardKey, productSavePath).ToString();
            productTestConfig.WorkElectricMin = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.WorkElectricMinKey, productSavePath).ToString();
            productTestConfig.WorkElectricMax = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.WorkElectricMaxKey, productSavePath).ToString();
            productTestConfig.PartNumber = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.PartNumberKey, productSavePath).ToString();
            productTestConfig.HardWareVersion = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.HardWareVersionKey, productSavePath).ToString();
            productTestConfig.SoftWareVersion = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.SoftWareVersionKey, productSavePath).ToString();
            productTestConfig.DormantElectricMin = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.DormantElectricMinKey, productSavePath).ToString();
            productTestConfig.DormantElectricMax = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.DormantElectricMaxKey, productSavePath).ToString();
            productTestConfig.BootLoader = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.BootLoaderKey, productSavePath).ToString();
            productTestConfig.PorterRate = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.PorterRateKey, productSavePath).ToString();
            productTestConfig.SendCanID = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.SendCanIDKey, productSavePath).ToString();
            productTestConfig.ReceiveCanID = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.ReceiveCanIDKey, productSavePath).ToString();
            productTestConfig.CycleCanID = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.CycleCanIDKey, productSavePath).ToString();
            productTestConfig.RF_CAN_ID = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.RF_CAN_IDKey, productSavePath).ToString();
            productTestConfig.TestSerial = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.TestSerialKey, productSavePath).ToString();
            productTestConfig.ControlPower = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.ControlPowerKey, productSavePath).ToString();
            productTestConfig.TestSerial = GetProductTestSerial(productTestConfig.TestSerial);
            productTestConfig.ProductId = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.ProductIdKey, productSavePath).ToString();
            Directory.Delete(productInitPath,true);
        }

        public void ReadProductCheckStandConfig(string newTypeNo)
        {
            var productCheckInitPath = defaultRoot + StandCommon.CheckProductStationConfigPath + standCommon.ProductTypeNo + "\\";
            var productCheckFileName = StandCommon.CheckProductStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var productCheckSavePath = productCheckInitPath + productCheckFileName;
            if (!Directory.Exists(productCheckInitPath))
                return;
            if (!File.Exists(productCheckSavePath))
                return;
            InitStandConfig.CreateCurrentProcessDirectory(newTypeNo, InitStandConfig.StandConfigType.productCheck);//创建新路径
            productCheckConfig.PlcAddress = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.PlcAddressKey, productCheckSavePath).ToString();
            productCheckConfig.LocalAddress = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.LocalAddressKey, productCheckSavePath).ToString();
            productCheckConfig.Avometer = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.AvometerKey, productCheckSavePath).ToString();
            productCheckConfig.TestBoard = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.TestBoardKey, productCheckSavePath).ToString();
            productCheckConfig.WorkElectricMin = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.WorkElectricMinKey, productCheckSavePath).ToString();
            productCheckConfig.WorkElectricMax = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.WorkElectricMaxKey, productCheckSavePath).ToString();
            productCheckConfig.PartNumber = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.PartNumberKey, productCheckSavePath).ToString();
            productCheckConfig.HardWareVersion = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.HardWareVersionKey, productCheckSavePath).ToString();
            productCheckConfig.SoftWareVersion = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.SoftWareVersionKey, productCheckSavePath).ToString();
            productCheckConfig.DormantElectricMin = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.DormantElectricMinKey, productCheckSavePath).ToString();
            productCheckConfig.DormantElectricMax = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.DormantElectricMaxKey, productCheckSavePath).ToString();
            productCheckConfig.BootLoader = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.BootLoaderKey, productCheckSavePath).ToString();
            productCheckConfig.PorterRate = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.PorterRateKey, productCheckSavePath).ToString();
            productCheckConfig.SendCanID = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.SendCanIDKey, productCheckSavePath).ToString();
            productCheckConfig.ReceiveCanID = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.ReceiveCanIDKey, productCheckSavePath).ToString();
            productCheckConfig.CycleCanID = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.CycleCanIDKey, productCheckSavePath).ToString();
            productCheckConfig.RF_CAN_ID = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.RF_CAN_IDKey, productCheckSavePath).ToString();
            productCheckConfig.TestSerial = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.TestSerialKey, productCheckSavePath).ToString();
            productCheckConfig.ControlPower = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.ControlPowerKey, productCheckSavePath).ToString();
            productCheckConfig.TestSerial = GetProductTestSerial(productCheckConfig.TestSerial);
            productCheckConfig.ProductId = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.ProductIdKey, productCheckSavePath).ToString();
            Directory.Delete(productCheckInitPath,true);
        }
        #endregion

        #region save config params to local ini

        private void SaveStandConfig()
        {
            SaveBurnStandConfig();
            SaveCommonStandConfig();
            SaveSensibilityStandConfig();
            SaveShellStandConfig();
            SaveAirtageStandConfig();
            SaveStentStandConfig();
            SaveProductTestConfig();
            SaveProductCheckConfig();
        }

        /// <summary>
        /// 根据当前选择得产品型号，配置每个工站得参数，保存到本地
        /// 烧录文件路径由用户选择生成
        /// 单独保存烧录文件路径
        /// </summary>
        public bool SaveBurnStandConfig()
        {
            //单独保存烧录文件路径
            //var burnSavePath = AppDomain.CurrentDomain.BaseDirectory +CommConfig.DeafaultConfigRoot + CommConfig.BurnConfigIniName;
            //INIFile.SetValue(standCommon.ProductTypeNo,CommConfig.BurnConfigPathKey,burnConfig.ProgramePath,burnSavePath);
            var burnInitPath = defaultRoot + StandCommon.TurnStationConfigPath + standCommon.ProductTypeNo + "\\";
            var burnFileName = StandCommon.TurnStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var burnSavePath = burnInitPath + burnFileName;
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.PowerValueKey, burnConfig.PowerValue, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.LocalAddressKey, burnConfig.LocalAddress, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.AvometerAddressKey, burnConfig.AvometerAddress, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.AutoSweepCodeComKey, burnConfig.AutoSweepCodeCom, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.BurnerKey, burnConfig.Burner, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.PorterRateKey, burnConfig.PorterRate, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.CanIdKey, burnConfig.CanId, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.ProductIdKey, burnConfig.ProductId, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.FirstVoltageMaxKey, burnConfig.FirstVoltageMax, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.FirstVoltageMinKey, burnConfig.FirstVoltageMin, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.SecondVoltageMaxKey, burnConfig.SecondVoltageMax, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.SecondVoltageMinKey, burnConfig.SecondVoltageMin, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.HardWareVersionKey, burnConfig.HardWareVersion, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.SoftWareVersionKey, burnConfig.SoftWareVersion, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.PartNumberKey, burnConfig.PartNumber, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.SendCanIDKey, burnConfig.SendCanID, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.ProgrameActualPathKey, burnConfig.ProgrameActualPath, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.ProgrameMapPathKey, burnConfig.ProgrameMapPath, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.ProgrameNameKey, burnConfig.ProgrameName, burnSavePath);
            if (burnConfig.SerialNumber == "")
                return false;
            var burn = GetBurnSerialConfig(burnConfig.SerialNumber);
            if (burn != null)
            {
                burnConfig.SerialNumber = burn.ProductSerialPath;
            }
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.SerialNumberKey, burnConfig.SerialNumber, burnSavePath);
            IsSavePrivateConfig = true;
            return true;
        }

        /// <summary>
        /// 保存通用配置，不同工站下的当前型号
        /// </summary>
        public bool SaveCommonStandConfig()
        {
            bool IsBurn = true, IsSen = true, IsAir = true, IsStent = true, IsShell = true, IsPtest = true, IsPcheck = true;
            //烧录文件保存通用配置
            var burnInitPath = defaultRoot + StandCommon.TurnStationConfigPath + standCommon.ProductTypeNo + "\\";
            var burnFileName = StandCommon.TurnStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var burnSavePath = burnInitPath + burnFileName;
            if (File.Exists(burnSavePath))
            {
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PCBABarCodeLengthKey, standCommon.PCBABarCodeLength, burnSavePath);
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.ShellBarCodeLengthKey, standCommon.ShellBarCodeLength, burnSavePath);
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.CaseBarCodeLengthKey, standCommon.CaseBarCodeLength, burnSavePath);
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PackageCaseAmountKey, standCommon.PackageCaseAmount, burnSavePath);
            }
            else
            {
                IsBurn = false;
            }
            //灵敏度文件保存通用配置
            var senInitPath = defaultRoot + StandCommon.SensibilityStationConfigPath + standCommon.ProductTypeNo + "\\";
            var senFileName = StandCommon.SensibilityStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var senSavePath = senInitPath + senFileName;
            if (File.Exists(senSavePath))
            {
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PCBABarCodeLengthKey, standCommon.PCBABarCodeLength, senSavePath);
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.ShellBarCodeLengthKey, standCommon.ShellBarCodeLength, senSavePath);
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.CaseBarCodeLengthKey, standCommon.CaseBarCodeLength, senSavePath);
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PackageCaseAmountKey, standCommon.PackageCaseAmount, senSavePath);
            }
            else
                IsSen = false;

            //外壳装配文件保存通用配置
            var shellInitPath = defaultRoot + StandCommon.ShellStationConfigPath + standCommon.ProductTypeNo + "\\";
            var shellFileName = StandCommon.ShellStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var shellSavePath = shellInitPath + shellFileName;
            if (File.Exists(shellSavePath))
            {
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PCBABarCodeLengthKey, standCommon.PCBABarCodeLength, shellSavePath);
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.ShellBarCodeLengthKey, standCommon.ShellBarCodeLength, shellSavePath);
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.CaseBarCodeLengthKey, standCommon.CaseBarCodeLength, shellSavePath);
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PackageCaseAmountKey, standCommon.PackageCaseAmount, shellSavePath);
            }
            else
                IsShell = false;
            //气密文件保存通用配置
            var airtageInitPath = defaultRoot + StandCommon.AirtageStationConfigPath + standCommon.ProductTypeNo + "\\";
            var airtageFileName = StandCommon.AirtageStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var airtageSavePath = airtageInitPath + airtageFileName;
            if (File.Exists(airtageSavePath))
            {
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PCBABarCodeLengthKey, standCommon.PCBABarCodeLength, airtageSavePath);
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.ShellBarCodeLengthKey, standCommon.ShellBarCodeLength, airtageSavePath);
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.CaseBarCodeLengthKey, standCommon.CaseBarCodeLength, airtageSavePath);
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PackageCaseAmountKey, standCommon.PackageCaseAmount, airtageSavePath);
            }
            else
                IsAir = false;
            //支架装配文件保存通用配置
            var stentInitPath = defaultRoot + StandCommon.StentStationConfigPath + standCommon.ProductTypeNo + "\\";
            var stentFileName = StandCommon.StentStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var stentSavePath = stentInitPath + stentFileName;
            if (File.Exists(stentSavePath))
            {
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PCBABarCodeLengthKey, standCommon.PCBABarCodeLength, stentSavePath);
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.ShellBarCodeLengthKey, standCommon.ShellBarCodeLength, stentSavePath);
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.CaseBarCodeLengthKey, standCommon.CaseBarCodeLength, stentSavePath);
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PackageCaseAmountKey, standCommon.PackageCaseAmount, stentSavePath);
            }
            else
                IsStent = false;

            //成品测试文件保存通用配置
            var productTestInitPath = defaultRoot + StandCommon.ProductFinishStationConfigPath + standCommon.ProductTypeNo + "\\";
            var productTestFileName = StandCommon.ProductFinishStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var productTestSavePath = productTestInitPath + productTestFileName;
            if (File.Exists(productTestSavePath))
            {
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PCBABarCodeLengthKey, standCommon.PCBABarCodeLength, productTestSavePath);
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.ShellBarCodeLengthKey, standCommon.ShellBarCodeLength, productTestSavePath);
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.CaseBarCodeLengthKey, standCommon.CaseBarCodeLength, productTestSavePath);
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PackageCaseAmountKey, standCommon.PackageCaseAmount, productTestSavePath);
            }
            else
                IsPtest = false;
            //成品抽检文件保存通用配置
            var productCheckInitPath = defaultRoot + StandCommon.CheckProductStationConfigPath + standCommon.ProductTypeNo + "\\";
            var productCheckFileName = StandCommon.CheckProductStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var productCheckSavePath = productCheckInitPath + productCheckFileName;
            if (File.Exists(productCheckSavePath))
            {
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PCBABarCodeLengthKey, standCommon.PCBABarCodeLength, productCheckSavePath);
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.ShellBarCodeLengthKey, standCommon.ShellBarCodeLength, productCheckSavePath);
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.CaseBarCodeLengthKey, standCommon.CaseBarCodeLength, productCheckSavePath);
                INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PackageCaseAmountKey, standCommon.PackageCaseAmount, productCheckSavePath);
            }
            else
                IsPcheck = false;
            if (IsSavePrivateConfig)
            {
                IsSavePrivateConfig = false;
                return false;
            }
            if (!IsBurn && !IsSen && !IsAir && !IsShell && !IsStent && !IsPtest && !IsPcheck)
                return false;
            return true;
        }

        public bool SaveSensibilityStandConfig()
        {
            var senInitPath = defaultRoot + StandCommon.SensibilityStationConfigPath + standCommon.ProductTypeNo + "\\";
            var senFileName = StandCommon.SensibilityStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var senSavePath = senInitPath + senFileName;
            INIFile.SetValue(standCommon.ProductTypeNo, SensibilityConfig.PLCAddressKey, sensibilityConfig.PLCAddress, senSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, SensibilityConfig.LocalAddressKey, sensibilityConfig.LocalAddress, senSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, SensibilityConfig.AvometerAddressKey, sensibilityConfig.AvometerAddress, senSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, SensibilityConfig.AutoSweepCodeKey, sensibilityConfig.AutoSweepCode, senSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, SensibilityConfig.ProgrameControlPowerKey, sensibilityConfig.ProgrameControlPower, senSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, SensibilityConfig.WorkElectricMinKey, sensibilityConfig.WorkElectricMin, senSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, SensibilityConfig.WorkElectricMaxKey, sensibilityConfig.WorkElectricMax, senSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, SensibilityConfig.PartNumberKey, sensibilityConfig.PartNumber, senSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, SensibilityConfig.HardWareVersionKey, sensibilityConfig.HardWareVersion, senSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, SensibilityConfig.SoftWareVersionKey, sensibilityConfig.SoftWareVersion, senSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, SensibilityConfig.DormantElectricMinKey, sensibilityConfig.DormantElectricMin, senSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, SensibilityConfig.DormantElectricMaxKey, sensibilityConfig.DormantElectricMax, senSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, SensibilityConfig.BootLoaderKey, sensibilityConfig.BootLoader, senSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, SensibilityConfig.PorterRateKey, sensibilityConfig.PorterRate, senSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, SensibilityConfig.SendCanIDKey, sensibilityConfig.SendCanID, senSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, SensibilityConfig.ReceiveCanIDKey, sensibilityConfig.ReceiveCanID, senSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, SensibilityConfig.CyclyCanIDKey, sensibilityConfig.CyclyCanID, senSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, SensibilityConfig.RfCanIDKey, sensibilityConfig.RfCanID, senSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, SensibilityConfig.ProductIdKey, sensibilityConfig.ProductId, senSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, SensibilityConfig.RadioFrequcyKey, sensibilityConfig.RadioFrequcy, senSavePath);
            if (sensibilityConfig.ProductSerial == "")
                return false;
            var sensibility = GetSensibilityConfig(sensibilityConfig.ProductSerial);
            if (sensibility != null)
                sensibilityConfig.ProductSerial = sensibility.ProductSerialPath;
            INIFile.SetValue(standCommon.ProductTypeNo, SensibilityConfig.ProductSerialKey, sensibilityConfig.ProductSerial, senSavePath);
            IsSavePrivateConfig = true;
            return true;
        }

        public bool SaveShellStandConfig()
        {
            var shellInitPath = defaultRoot + StandCommon.ShellStationConfigPath + standCommon.ProductTypeNo + "\\";
            var shellFileName = StandCommon.ShellStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var shellSavePath = shellInitPath + shellFileName;
            INIFile.SetValue(standCommon.ProductTypeNo, ShellConfig.LocalAddressConMesKey, shellConfig.LocalAddressConMes, shellSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ShellConfig.LocalAddressConPLCKey, shellConfig.LocalAddressConPLC, shellSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ShellConfig.PLCAddressKey, shellConfig.PLCAddress, shellSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ShellConfig.SmallScrewSetTimeKey, shellConfig.SmallScrewSetTime, shellSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ShellConfig.LargeScrewSetTimeKey, shellConfig.LargeScrewSetTime, shellSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ShellConfig.FrontCoverKey, shellConfig.FrontCover, shellSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ShellConfig.BackCoverKey, shellConfig.BackCover, shellSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ShellConfig.PCBScrewKey, shellConfig.PCBScrew, shellSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ShellConfig.ShellScrewKey, shellConfig.ShellScrew, shellSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ShellConfig.TopCoverKey, shellConfig.TopCover, shellSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ShellConfig.ShellKey, shellConfig.Shell, shellSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ShellConfig.SealRingWireKey, shellConfig.SealRingWire, shellSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ShellConfig.BubbleCottonKey, shellConfig.BubbleCotton, shellSavePath);
            if (shellConfig.TestSerialNumber == "")
                return false;
            var shell = GetShellConfig(shellConfig.TestSerialNumber);
            if (shell != null)
                shellConfig.TestSerialNumber = shell.ProductSerialPath;
            INIFile.SetValue(standCommon.ProductTypeNo, ShellConfig.TestSerialNumberKey, shellConfig.TestSerialNumber, shellSavePath);
            IsSavePrivateConfig = true;
            return true;
        }

        public bool SaveAirtageStandConfig()
        {
            var airtageInitPath = defaultRoot + StandCommon.AirtageStationConfigPath + standCommon.ProductTypeNo + "\\";
            var airtageFileName = StandCommon.AirtageStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var airtageSavePath = airtageInitPath + airtageFileName;
            INIFile.SetValue(standCommon.ProductTypeNo, AirtageConfig.LocalAddressConMesKey, airtageConfig.LocalAddressConMes, airtageSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, AirtageConfig.AirTesterKey, airtageConfig.AirTester, airtageSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, AirtageConfig.InflateAirTimeKey, airtageConfig.InflateAirTime, airtageSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, AirtageConfig.StableTimeKey, airtageConfig.StableTime, airtageSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, AirtageConfig.TestTimeKey, airtageConfig.TestTime, airtageSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, AirtageConfig.PressureUnitKey, airtageConfig.PressureUnit, airtageSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, AirtageConfig.SpreadUnitKey, airtageConfig.SpreadUnit, airtageSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, AirtageConfig.MaxInflateKey, airtageConfig.MaxInflate, airtageSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, AirtageConfig.MinInflateKey, airtageConfig.MinInflate, airtageSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, AirtageConfig.LevelMinKey, airtageConfig.LevelMin, airtageSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, AirtageConfig.LevelMaxKey, airtageConfig.LevelMax, airtageSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, AirtageConfig.BigLeakMaxKey, airtageConfig.BigLeakMax, airtageSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, AirtageConfig.BigLeakMinKey, airtageConfig.BigLeakMin, airtageSavePath);
            if (airtageConfig.TestSerial == "")
                return false;
            var airtage = GetAirtageConfig(airtageConfig.TestSerial);
            if (airtage != null)
                airtageConfig.TestSerial = airtage.ProductSerialPath;
            INIFile.SetValue(standCommon.ProductTypeNo, AirtageConfig.TestSerialKey, airtageConfig.TestSerial, airtageSavePath);
            IsSavePrivateConfig = true;
            return true;
        }

        public bool SaveStentStandConfig()
        {
            var stentInitPath = defaultRoot + StandCommon.StentStationConfigPath + standCommon.ProductTypeNo + "\\";
            var stentFileName = StandCommon.StentStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var stentSavePath = stentInitPath + stentFileName;
            INIFile.SetValue(standCommon.ProductTypeNo, StentConfig.LocalAddressConMesKey, stentConfig.LocalAddressConMes, stentSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StentConfig.LeftStentKey, stentConfig.LeftStent, stentSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StentConfig.RightStentKey, stentConfig.RightStent, stentSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StentConfig.UnionStentKey, stentConfig.UnionStent, stentSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StentConfig.StentKey, stentConfig.Stent, stentSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StentConfig.StentSrcrewKey, stentConfig.StentScrew, stentSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StentConfig.StentNutKey, stentConfig.StentNut, stentSavePath);
            if (stentConfig.TestSerial == "")
                return false;
            var stent = GetStentConfig(stentConfig.TestSerial);
            if (stent != null)
                stentConfig.TestSerial = stent.ProductSerialPath;
            INIFile.SetValue(standCommon.ProductTypeNo, StentConfig.TestSerialKey, stentConfig.TestSerial, stentSavePath);
            IsSavePrivateConfig = true;
            return true;
        }

        public bool SaveProductTestConfig()
        {
            var productInitPath = defaultRoot + StandCommon.ProductFinishStationConfigPath + standCommon.ProductTypeNo + "\\";
            var productFileName = StandCommon.ProductFinishStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var productSavePath = productInitPath + productFileName;
            INIFile.SetValue(standCommon.ProductTypeNo, ProductTestConfig.PlcAddressKey, productTestConfig.PlcAddress, productSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductTestConfig.LocalAddressKey, productTestConfig.LocalAddress, productSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductTestConfig.AvometerKey, productTestConfig.Avometer, productSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductTestConfig.AutoSweepCodeKey, productTestConfig.AutoSweepCode, productSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductTestConfig.TestBoardKey, productTestConfig.TestBoard, productSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductTestConfig.ControlPowerKey, productTestConfig.ControlPower, productSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductTestConfig.WorkElectricMinKey, productTestConfig.WorkElectricMin, productSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductTestConfig.WorkElectricMaxKey, productTestConfig.WorkElectricMax, productSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductTestConfig.PartNumberKey, productTestConfig.PartNumber, productSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductTestConfig.HardWareVersionKey, productTestConfig.HardWareVersion, productSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductTestConfig.SoftWareVersionKey, productTestConfig.SoftWareVersion, productSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductTestConfig.DormantElectricMinKey, productTestConfig.DormantElectricMin, productSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductTestConfig.DormantElectricMaxKey, productTestConfig.DormantElectricMax, productSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductTestConfig.BootLoaderKey, productTestConfig.BootLoader, productSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductTestConfig.PorterRateKey, productTestConfig.PorterRate, productSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductTestConfig.SendCanIDKey, productTestConfig.SendCanID, productSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductTestConfig.ReceiveCanIDKey, productTestConfig.ReceiveCanID, productSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductTestConfig.CycleCanIDKey, productTestConfig.CycleCanID, productSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductTestConfig.RF_CAN_IDKey, productTestConfig.RF_CAN_ID, productSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductTestConfig.ProductIdKey, productTestConfig.ProductId, productSavePath);
            if (productTestConfig.TestSerial == "")
                return false;
            var productTest = GetProductTestConfig(productTestConfig.TestSerial);
            if (productTest != null)
                productTestConfig.TestSerial = productTest.ProductSerialPath;
            INIFile.SetValue(standCommon.ProductTypeNo, ProductTestConfig.TestSerialKey, productTestConfig.TestSerial, productSavePath);
            IsSavePrivateConfig = true;
            return true;
        }

        public bool SaveProductCheckConfig()
        {
            var productCheckInitPath = defaultRoot + StandCommon.CheckProductStationConfigPath + standCommon.ProductTypeNo + "\\";
            var productCheckFileName = StandCommon.CheckProductStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var productCheckSavePath = productCheckInitPath + productCheckFileName;
            INIFile.SetValue(standCommon.ProductTypeNo, ProductCheckConfig.PlcAddressKey, productCheckConfig.PlcAddress, productCheckSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductCheckConfig.LocalAddressKey, productCheckConfig.LocalAddress, productCheckSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductCheckConfig.AvometerKey, productCheckConfig.Avometer, productCheckSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductCheckConfig.TestBoardKey, productCheckConfig.TestBoard, productCheckSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductCheckConfig.ControlPowerKey, productCheckConfig.ControlPower, productCheckSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductCheckConfig.WorkElectricMinKey, productCheckConfig.WorkElectricMin, productCheckSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductCheckConfig.WorkElectricMaxKey, productCheckConfig.WorkElectricMax, productCheckSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductCheckConfig.PartNumberKey, productCheckConfig.PartNumber, productCheckSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductCheckConfig.HardWareVersionKey, productCheckConfig.HardWareVersion, productCheckSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductCheckConfig.SoftWareVersionKey, productCheckConfig.SoftWareVersion, productCheckSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductCheckConfig.DormantElectricMinKey, productCheckConfig.DormantElectricMin, productCheckSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductCheckConfig.DormantElectricMaxKey, productCheckConfig.DormantElectricMax, productCheckSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductCheckConfig.BootLoaderKey, productCheckConfig.BootLoader, productCheckSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductCheckConfig.PorterRateKey, productCheckConfig.PorterRate, productCheckSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductCheckConfig.SendCanIDKey, productCheckConfig.SendCanID, productCheckSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductCheckConfig.ReceiveCanIDKey, productCheckConfig.ReceiveCanID, productCheckSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductCheckConfig.CycleCanIDKey, productCheckConfig.CycleCanID, productCheckSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductCheckConfig.RF_CAN_IDKey, productCheckConfig.RF_CAN_ID, productCheckSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ProductCheckConfig.ProductIdKey, productCheckConfig.ProductId, productCheckSavePath);
            if (productCheckConfig.TestSerial == "")
                return false;
            var productCheck = GetProductCheckConfig(productCheckConfig.TestSerial);
            if (productCheck != null)
                productCheckConfig.TestSerial = productCheck.ProductSerialPath;
            INIFile.SetValue(standCommon.ProductTypeNo, ProductCheckConfig.TestSerialKey, productCheckConfig.TestSerial, productCheckSavePath);
            IsSavePrivateConfig = true;
            return true;
        }
        #endregion

        #region 产品序列绝对路径/序列名称/匹配电压
        private string LocalPathConvertToMapPath(string path)
        {
            if (path == "" || !path.Contains(":"))
                return "";
            return standMapRoot + path.Substring(path.IndexOf(':'));
        }

        private string GetProductTestSerial(string serialPath)
        {
            if (!serialPath.Contains("\\") && !serialPath.Contains("\\\\"))
            {
                LogHelper.Log.Error("【产品序列路径不合法】");
                return "";
            }
            return serialPath.Substring(serialPath.LastIndexOf('\\') + 1);
        }

        private BurnConfig GetBurnSerialConfig(string serialName)
        {
            return burnConfigList.Find(m => m.SerialNumber == serialName);
        }

        private SensibilityConfig GetSensibilityConfig(string serialName)
        {
            return sensibilityConfigList.Find(m => m.ProductSerial == serialName);
        }

        private ShellConfig GetShellConfig(string serialName)
        {
            return shellConfigList.Find(m => m.TestSerialNumber == serialName);
        }

        private StentConfig GetStentConfig(string serialName)
        {
            return stentConfigList.Find(m => m.TestSerial == serialName);
        }

        private AirtageConfig GetAirtageConfig(string serialName)
        {
            return airtageConfigList.Find(m => m.TestSerial == serialName);
        }

        private ProductTestConfig GetProductTestConfig(string serialName)
        {
            return productTestConfigList.Find(m => m.TestSerial == serialName);
        }

        private ProductCheckConfig GetProductCheckConfig(string serialName)
        {
            return productCheckConfigList.Find(m => m.TestSerial == serialName);
        }
        #endregion
    }
}
