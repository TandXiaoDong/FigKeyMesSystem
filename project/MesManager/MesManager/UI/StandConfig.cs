using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using CommonUtils.FileHelper;
using CommonUtils.Logger;
using MesManager.Model;
using MesManager.CommonEnum;
using MesManager.Common;
using System.IO;

namespace MesManager.UI
{
    public partial class StandConfig : RadForm
    {
        private StandCommon standCommon;
        private BurnConfig burnConfig;
        private SensibilityConfig sensibilityConfig;
        private ShellConfig shellConfig;
        private AirtageConfig airtageConfig;
        private StentConfig stentConfig;
        private ProductTestConfig productTestConfig;
        private ProductCheckConfig productCheckConfig;
        private bool IsSavePrivateConfig;
        Dictionary<int, string> directoryBurn = new Dictionary<int, string>();
        Dictionary<int, string> directorySensibility = new Dictionary<int, string>();
        Dictionary<int, string> directoryAirtage = new Dictionary<int, string>();
        Dictionary<int, string> directoryShell = new Dictionary<int, string>();
        Dictionary<int, string> directoryStent = new Dictionary<int, string>();
        Dictionary<int, string> directoryProductTest = new Dictionary<int, string>();
        Dictionary<int, string> directoryProductCheck = new Dictionary<int, string>();
        List<BurnConfig> burnConfigList = new List<BurnConfig>();

        private MesServiceTest.MesServiceClient serviceClientTest;
        private MesService.MesServiceClient serviceClient;

        public StandConfig()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void StandConfig_Load(object sender, EventArgs e)
        {
            Init();
            ReadDefaultConfig();
            ReadStandConfig();
            RefreshUI();
            EventHandler();
        }

        private void EventHandler()
        {
            this.btn_common_refresh.Click += Btn_common_refresh_Click;
            this.btn_burn_refresh.Click += Btn_burn_refresh_Click;
            this.btn_sensibility_refresh.Click += Btn_sensibility_refresh_Click;
            this.btn_shell_refresh.Click += Btn_shell_refresh_Click;
            this.btn_stent_refresh.Click += Btn_stent_refresh_Click;
            this.btn_airtage_refresh.Click += Btn_airtage_refresh_Click;
            this.btn_productTest_refresh.Click += Btn_productTest_refresh_Click;
            this.btn_productCheck_refresh.Click += Btn_productCheck_refresh_Click;

            this.btn_common_save.Click += Btn_common_save_Click;
            this.btn_burn_save.Click += Btn_burn_save_Click;
            this.btn_sensilibity_save.Click += Btn_sensilibity_save_Click;
            this.btn_shell_save.Click += Btn_shell_save_Click;
            this.btn_stent_save.Click += Btn_stent_save_Click;
            this.btn_airtage_save.Click += Btn_airtage_save_Click;
            this.btn_productCheck_save.Click += Btn_productCheck_save_Click;
            this.btn_productTest_save.Click += Btn_productTest_save_Click;

            this.lbx_burn_openFile.Click += Lbx_burn_openFile_Click;

            this.FormClosed += StandConfig_FormClosed;
        }

        private void Lbx_burn_openFile_Click(object sender, EventArgs e)
        {
            FileContent fileContent = FileSelect.GetSelectFileContent("(*.*)|*.*","选择文件");
            this.tb_burn_programePath.Text = fileContent.FileName;
            this.tb_burn_programeName.Text = fileContent.FileSafeName;
        }

        #region 保存配置
        private void Btn_productTest_save_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认要保存修改的配置？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                return;
            CheckProductTestConfigParams();
            SaveProductTestConfig();
            CheckCommonConfigParams();
            SaveCommonStandConfig();
        }

        private void Btn_productCheck_save_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认要保存修改的配置？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                return;
            CheckProductCheckConfigParams();
            SaveProductCheckConfig();
            CheckCommonConfigParams();
            SaveCommonStandConfig();
        }

        private void Btn_airtage_save_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认要保存修改的配置？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                return;
            CheckAirtageConfigParams();
            SaveAirtageStandConfig();
            CheckCommonConfigParams();
            SaveCommonStandConfig();
        }

        private void Btn_stent_save_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认要保存修改的配置？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                return;
            CheckStentConfigParams();
            SaveStentStandConfig();
            CheckCommonConfigParams();
            SaveCommonStandConfig();
        }

        private void Btn_shell_save_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认要保存修改的配置？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                return;
            CheckShellConfigParams();
            SaveShellStandConfig();
            CheckCommonConfigParams();
            SaveCommonStandConfig();
        }

        private void Btn_sensilibity_save_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认要保存修改的配置？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                return;
            CheckSensibilityConfigParams();
            SaveSensibilityStandConfig();
            CheckCommonConfigParams();
            SaveCommonStandConfig();
        }

        private void Btn_burn_save_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认要保存修改的配置？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                return;
            CheckBurnConfigParams();
            SaveBurnStandConfig();
            CheckCommonConfigParams();
            SaveCommonStandConfig();
        }

        private void Btn_common_save_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认要保存修改的配置？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                return;
            CheckCommonConfigParams();
            SaveCommonStandConfig();
        }
        #endregion

        #region 读取配置
        private void Btn_productCheck_refresh_Click(object sender, EventArgs e)
        {
            ReadProductCheckStandConfig();
            RefreshUIProductCheckStation();
        }

        private void Btn_productTest_refresh_Click(object sender, EventArgs e)
        {
            ReadProductTestStandConfig();
            RefreshUIProductTestStation();
        }

        private void Btn_airtage_refresh_Click(object sender, EventArgs e)
        {
            ReadAirtageStandConfig();
            RefreshUIAirtageStation();
        }

        private void Btn_stent_refresh_Click(object sender, EventArgs e)
        {
            ReadStentStandConfig();
            RefreshUIStentStation();
        }

        private void Btn_shell_refresh_Click(object sender, EventArgs e)
        {
            ReadShellStandConfig();
            RefreshUIShellStation();
        }

        private void Btn_sensibility_refresh_Click(object sender, EventArgs e)
        {
            ReadSensibilityStandConfig();
            RefreshUISensibilityStation();
        }

        private void Btn_burn_refresh_Click(object sender, EventArgs e)
        {
            ReadBurnStandConfig();
            RefreshUIBurnStation();
        }

        private void Btn_common_refresh_Click(object sender, EventArgs e)
        {
            ReadCommonStandConfig();
            RefreshUICommonConfig();
        }
        #endregion

        private void StandConfig_FormClosed(object sender, FormClosedEventArgs e)
        {
            //关闭时自动保存所有
            //SaveBurnStandConfig();
        }

        private void Init()
        {
            standCommon = new StandCommon();
            burnConfig = new BurnConfig();
            sensibilityConfig = new SensibilityConfig();
            shellConfig = new ShellConfig();
            airtageConfig = new AirtageConfig();
            stentConfig = new StentConfig();
            productTestConfig = new ProductTestConfig();
            productCheckConfig = new ProductCheckConfig();

            serviceClientTest = new MesServiceTest.MesServiceClient();
            serviceClient = new MesService.MesServiceClient();
            //查询当前工艺/产品型号
            standCommon.ProductTypeNo = serviceClientTest.SelectCurrentTProcess();
            if (standCommon.ProductTypeNo == "")
            {
                LogHelper.Log.Error("【机台配置】查询当前工艺流程失败");
            }
            var processList = serviceClientTest.SelectAllTProcess();
            standCommon.ProductTypeNoList = new List<string>();
            foreach (var productType in processList)
            {
                standCommon.ProductTypeNoList.Add(productType);
            }
            //初始化波特率
            this.cb_burn_porterRate.DataSource = PorterRate.PorterRateDataSource();
            this.tb_sen_porterRate.DataSource = PorterRate.PorterRateDataSource();
            this.tb_productCheck_porterRate.DataSource = PorterRate.PorterRateDataSource();
            this.tb_product_porterRate.DataSource = PorterRate.PorterRateDataSource();
        }

        private void ReadDefaultConfig()
        {
            List<string> testSerialList;
            var serialPath = AppDomain.CurrentDomain.BaseDirectory +CommConfig.DeafaultConfigRoot + CommConfig.TestStandSerialNumber;
            if (!File.Exists(serialPath))
            {
                using (File.Create(serialPath))
                {
                }
                //创建文件模板
                int count = 2;
                //烧录
                for (int i = 0; i < count; i++)
                {
                    INIFile.SetValue(CommConfig.BurnStationSection,CommConfig.SerialKey+i,"turntestserial_"+i,serialPath);
                }
                INIFile.SetValue(CommConfig.BurnStationSection,CommConfig.CountKey,count.ToString(),serialPath);
                //灵敏度
                for (int i = 0; i < count; i++)
                {
                    INIFile.SetValue(CommConfig.SensibilityStationSection, CommConfig.SerialKey + i, "sensibilitytestserial_" + i, serialPath);
                }
                INIFile.SetValue(CommConfig.SensibilityStationSection, CommConfig.CountKey, count.ToString(), serialPath);
                //外壳装配
                for (int i = 0; i < count; i++)
                {
                    INIFile.SetValue(CommConfig.ShellStationSection, CommConfig.SerialKey + i, "shell testserial_" + i, serialPath);
                }
                INIFile.SetValue(CommConfig.ShellStationSection, CommConfig.CountKey, count.ToString(), serialPath);
                //气密
                for (int i = 0; i < count; i++)
                {
                    INIFile.SetValue(CommConfig.AirtageStationSection, CommConfig.SerialKey + i, "airtagetestserial_" + i, serialPath);
                }
                INIFile.SetValue(CommConfig.AirtageStationSection, CommConfig.CountKey, count.ToString(), serialPath);
                //支架装配
                for (int i = 0; i < count; i++)
                {
                    INIFile.SetValue(CommConfig.StentStationSection, CommConfig.SerialKey + i, "stenttestserial_" + i, serialPath);
                }
                INIFile.SetValue(CommConfig.StentStationSection, CommConfig.CountKey, count.ToString(), serialPath);
                //成品测试
                for (int i = 0; i < count; i++)
                {
                    INIFile.SetValue(CommConfig.ProductFinishStationSection, CommConfig.SerialKey + i, "producttestserial_" + i, serialPath);
                }
                INIFile.SetValue(CommConfig.ProductFinishStationSection, CommConfig.CountKey, count.ToString(), serialPath);
                //成品抽检
                for (int i = 0; i < count; i++)
                {
                    INIFile.SetValue(CommConfig.CheckProductStationSection, CommConfig.SerialKey + i, "productchecktestserial_" + i, serialPath);
                }
                INIFile.SetValue(CommConfig.CheckProductStationSection, CommConfig.CountKey, count.ToString(), serialPath);
            }
            //read file content
            int serialCount;
            //读取烧录序列
            testSerialList = new List<string>();
            int.TryParse(INIFile.GetValue(CommConfig.BurnStationSection,CommConfig.CountKey,serialPath),out serialCount);
            for (int i = 0; i < serialCount; i++)
            {
                BurnConfig burnConfig = new BurnConfig();
                var serialValue = INIFile.GetValue(CommConfig.BurnStationSection,CommConfig.SerialKey+i,serialPath);
                burnConfig.ProductSerialPath = serialPath;
                burnConfig.SerialNumber = GetProductTestSerial(serialValue);
                testSerialList.Add(burnConfig.SerialNumber);
                burnConfigList.Add(burnConfig);
            }
            this.cb_burn_serialNumber.DataSource = testSerialList;

            //读取灵敏度序列
            testSerialList = new List<string>();
            int.TryParse(INIFile.GetValue(CommConfig.SensibilityStationSection, CommConfig.CountKey, serialPath), out serialCount);
            for (int i = 0; i < serialCount; i++)
            {
                var serialValue = INIFile.GetValue(CommConfig.SensibilityStationSection, CommConfig.SerialKey + i, serialPath);
                testSerialList.Add(serialValue);
                directorySensibility.Add(i,serialValue);
            }
            this.cb_sen_serialNumber.DataSource = testSerialList;
            //读取外壳装配序列
            testSerialList = new List<string>();
            int.TryParse(INIFile.GetValue(CommConfig.ShellStationSection, CommConfig.CountKey, serialPath), out serialCount);
            for (int i = 0; i < serialCount; i++)
            {
                var serialValue = INIFile.GetValue(CommConfig.ShellStationSection, CommConfig.SerialKey + i, serialPath);
                testSerialList.Add(serialValue);
                directoryShell.Add(i, serialValue);
            }
            this.cb_shell_testSerial.DataSource = testSerialList;
            //读取气密序列
            testSerialList = new List<string>();
            int.TryParse(INIFile.GetValue(CommConfig.AirtageStationSection, CommConfig.CountKey, serialPath), out serialCount);
            for (int i = 0; i < serialCount; i++)
            {
                var serialValue = INIFile.GetValue(CommConfig.AirtageStationSection, CommConfig.SerialKey + i, serialPath);
                testSerialList.Add(serialValue);
                directoryAirtage.Add(i, serialValue);
            }
            this.tb_airtage_testSerial.DataSource = testSerialList;
            //读取支架装配序列
            testSerialList = new List<string>();
            int.TryParse(INIFile.GetValue(CommConfig.StentStationSection, CommConfig.CountKey, serialPath), out serialCount);
            for (int i = 0; i < serialCount; i++)
            {
                var serialValue = INIFile.GetValue(CommConfig.StentStationSection, CommConfig.SerialKey + i, serialPath);
                testSerialList.Add(serialValue);
                directoryStent.Add(i, serialValue);
            }
            this.cb_stent_testSerial.DataSource = testSerialList;
            //读取成品测试序列
            testSerialList = new List<string>();
            int.TryParse(INIFile.GetValue(CommConfig.ProductFinishStationSection, CommConfig.CountKey, serialPath), out serialCount);
            for (int i = 0; i < serialCount; i++)
            {
                var serialValue = INIFile.GetValue(CommConfig.ProductFinishStationSection, CommConfig.SerialKey + i, serialPath);
                testSerialList.Add(serialValue);
                directoryProductTest.Add(i, serialValue);
            }
            this.cb_product_testSerial.DataSource = testSerialList;
            //读取成品抽检序列
            testSerialList = new List<string>();
            int.TryParse(INIFile.GetValue(CommConfig.CheckProductStationSection, CommConfig.CountKey, serialPath), out serialCount);
            for (int i = 0; i < serialCount; i++)
            {
                var serialValue = INIFile.GetValue(CommConfig.CheckProductStationSection, CommConfig.SerialKey + i, serialPath);
                testSerialList.Add(serialValue);
                directoryProductCheck.Add(i, serialValue);
            }
            this.tb_productCheck_testSerial.DataSource = testSerialList;
        }

        #region read stand config 

        private void ReadStandConfig()
        {
            ReadBurnStandConfig();
            ReadCommonStandConfig();
            ReadSensibilityStandConfig();
            ReadShellStandConfig();
            ReadAirtageStandConfig();
            ReadStentStandConfig();
            ReadProductTestStandConfig();
            ReadProductCheckStandConfig();
        }

        /// <summary>
        /// 读取当前型号得所有工站配置
        /// </summary>
        private void ReadBurnStandConfig()
        {
            //读取烧录配置
            //var burnSavePath = AppDomain.CurrentDomain.BaseDirectory + CommConfig.DeafaultConfigRoot + CommConfig.BurnConfigIniName;
            //var burnCurrentPath = INIFile.GetValue(standCommon.ProductTypeNo, CommConfig.BurnConfigPathKey,burnSavePath);
            //根据路径读取配置
            var burnInitPath = StandCommon.TurnStationConfigPath + standCommon.ProductTypeNo + "\\";
            var burnFileName = StandCommon.TurnStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var burnSavePath = burnInitPath + burnFileName;
            burnConfig.PowerValue = INIFile.GetValue(standCommon.ProductTypeNo,BurnConfig.PowerValueKey,burnSavePath);
            burnConfig.LocalAddress = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.LocalAddressKey, burnSavePath);
            burnConfig.AvometerAddress = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.AvometerAddressKey, burnSavePath);
            burnConfig.AutoSweepCodeCom = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.AutoSweepCodeComKey, burnSavePath);
            burnConfig.Burner = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.BurnerKey, burnSavePath);
            burnConfig.PorterRate = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.PorterRateKey, burnSavePath);
            burnConfig.CanId = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.CanIdKey, burnSavePath);
            burnConfig.ProductId = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.ProductIdKey, burnSavePath);
            burnConfig.FirstVoltageMax = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.FirstVoltageMaxKey, burnSavePath);
            burnConfig.FirstVoltageMin = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.FirstVoltageMinKey, burnSavePath);
            burnConfig.SecondVoltageMax = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.SecondVoltageMaxKey, burnSavePath);
            burnConfig.SecondVoltageMin = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.SecondVoltageMinKey, burnSavePath);
            burnConfig.HardWareVersion = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.HardWareVersionKey, burnSavePath);
            burnConfig.SoftWareVersion = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.SoftWareVersionKey, burnSavePath);
            burnConfig.PartNumber = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.PartNumberKey, burnSavePath);
            burnConfig.ProgramePath = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.ProgramePathKey, burnSavePath);
            burnConfig.ProgrameName = INIFile.GetValue(standCommon.ProductTypeNo,BurnConfig.ProgrameNameKey,burnSavePath);
            burnConfig.SerialNumber = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.SerialNumberKey, burnSavePath);
            burnConfig.SerialNumber = GetProductTestSerial(burnConfig.SerialNumber);//更加路径返回序列名
            //读取灵敏度配置
        }

        private void ReadCommonStandConfig()
        {
            var burnInitPath = StandCommon.TurnStationConfigPath + standCommon.ProductTypeNo + "\\";
            var burnFileName = StandCommon.TurnStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var burnSavePath = burnInitPath + burnFileName;
            //读取烧录工站通用配置
            standCommon.PCBABarCodeLength = INIFile.GetValue(standCommon.ProductTypeNo, StandCommon.PCBABarCodeLengthKey, burnSavePath);
            standCommon.CaseBarCodeLength = INIFile.GetValue(standCommon.ProductTypeNo, StandCommon.CaseBarCodeLengthKey, burnSavePath);
            standCommon.ShellBarCodeLength = INIFile.GetValue(standCommon.ProductTypeNo, StandCommon.ShellBarCodeLengthKey, burnSavePath);
            standCommon.PackageCaseAmount = INIFile.GetValue(standCommon.ProductTypeNo, StandCommon.PackageCaseAmountKey, burnSavePath);
        }

        private void ReadSensibilityStandConfig()
        {
            var senInitPath = StandCommon.SensibilityStationConfigPath + standCommon.ProductTypeNo + "\\";
            var senFileName = StandCommon.SensibilityStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var senSavePath = senInitPath + senFileName;
            sensibilityConfig.PLCAddress = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.PLCAddressKey, senSavePath);
            sensibilityConfig.LocalAddress = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.LocalAddressKey, senSavePath);
            sensibilityConfig.AvometerAddress = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.AvometerAddressKey, senSavePath);
            sensibilityConfig.AutoSweepCode = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.AutoSweepCodeKey, senSavePath);
            sensibilityConfig.ProgrameControlPower = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.ProgrameControlPowerKey, senSavePath);
            sensibilityConfig.WorkElectricMin = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.WorkElectricMinKey, senSavePath);
            sensibilityConfig.WorkElectricMax = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.WorkElectricMaxKey, senSavePath);
            sensibilityConfig.PartNumber = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.PartNumberKey, senSavePath);
            sensibilityConfig.HardWareVersion = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.HardWareVersionKey, senSavePath);
            sensibilityConfig.SoftWareVersion = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.SoftWareVersionKey, senSavePath);
            sensibilityConfig.DormantElectricMin = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.DormantElectricMinKey, senSavePath);
            sensibilityConfig.DormantElectricMax = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.DormantElectricMaxKey, senSavePath);
            sensibilityConfig.BootLoader = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.BootLoaderKey, senSavePath);
            sensibilityConfig.PorterRate = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.PorterRateKey, senSavePath);
            sensibilityConfig.SendCanID = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.SendCanIDKey, senSavePath);
            sensibilityConfig.ReceiveCanID = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.ReceiveCanIDKey, senSavePath);
            sensibilityConfig.CyclyCanID = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.CyclyCanIDKey, senSavePath);
            sensibilityConfig.RfCanID = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.RfCanIDKey, senSavePath);
            sensibilityConfig.ProductSerial = INIFile.GetValue(standCommon.ProductTypeNo, SensibilityConfig.ProductSerialKey, senSavePath);
        }

        private void ReadShellStandConfig()
        {
            var shellInitPath = StandCommon.ShellStationConfigPath + standCommon.ProductTypeNo + "\\";
            var shellFileName = StandCommon.ShellStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var shellSavePath = shellInitPath + shellFileName;
            shellConfig.LocalAddressConMes = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.LocalAddressConMesKey, shellSavePath);
            shellConfig.LocalAddressConPLC = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.LocalAddressConPLCKey, shellSavePath);
            shellConfig.PLCAddress = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.PLCAddressKey, shellSavePath);
            shellConfig.SmallScrewSetTime = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.SmallScrewSetTimeKey, shellSavePath);
            shellConfig.LargeScrewSetTime = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.LargeScrewSetTimeKey, shellSavePath);
            shellConfig.TestSerialNumber = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.TestSerialNumberKey, shellSavePath);
        }

        private void ReadAirtageStandConfig()
        {
            var airtageInitPath = StandCommon.AirtageStationConfigPath + standCommon.ProductTypeNo + "\\";
            var airtageFileName = StandCommon.AirtageStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var airtageSavePath = airtageInitPath + airtageFileName;
            airtageConfig.LocalAddressConMes = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.LocalAddressConMesKey, airtageSavePath);
            airtageConfig.AirTester = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.LocalAddressConMesKey, airtageSavePath);
            airtageConfig.InflateAirTime = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.LocalAddressConMesKey, airtageSavePath);
            airtageConfig.StableTime = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.LocalAddressConMesKey, airtageSavePath);
            airtageConfig.PressureUnit = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.LocalAddressConMesKey, airtageSavePath);
            airtageConfig.SpreadUnit = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.LocalAddressConMesKey, airtageSavePath);
            airtageConfig.MaxInflate = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.LocalAddressConMesKey, airtageSavePath);
            airtageConfig.MinInflate = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.LocalAddressConMesKey, airtageSavePath);
            airtageConfig.TestConditionValue = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.LocalAddressConMesKey, airtageSavePath);
            airtageConfig.ReferenceConditionValue = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.LocalAddressConMesKey, airtageSavePath);
            airtageConfig.TestSerial = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.LocalAddressConMesKey, airtageSavePath);
            airtageConfig.TestTime = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.TestTimeKey, airtageSavePath);
        }

        private void ReadStentStandConfig()
        {
            var stentInitPath = StandCommon.StentStationConfigPath + standCommon.ProductTypeNo + "\\";
            var stentFileName = StandCommon.StentStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var stentSavePath = stentInitPath + stentFileName;
            stentConfig.LocalAddressConMes = INIFile.GetValue(standCommon.ProductTypeNo, StentConfig.LocalAddressConMesKey, stentSavePath);
            stentConfig.TestSerial = INIFile.GetValue(standCommon.ProductTypeNo, StentConfig.TestSerialKey, stentSavePath);
        }

        private void ReadProductTestStandConfig()
        {
            var productInitPath = StandCommon.ProductFinishStationConfigPath + standCommon.ProductTypeNo + "\\";
            var productFileName = StandCommon.ProductFinishStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var productSavePath = productInitPath + productFileName;
            productTestConfig.PlcAddress = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.PlcAddressKey, productSavePath);
            productTestConfig.LocalAddress = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.LocalAddressKey, productSavePath);
            productTestConfig.Avometer = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.AvometerKey, productSavePath);
            productTestConfig.AutoSweepCode = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.AutoSweepCodeKey, productSavePath);
            productTestConfig.TestBoard = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.TestBoardKey, productSavePath);
            productTestConfig.WorkElectricMin = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.WorkElectricMinKey, productSavePath);
            productTestConfig.WorkElectricMax = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.WorkElectricMaxKey, productSavePath);
            productTestConfig.PartNumber = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.PartNumberKey, productSavePath);
            productTestConfig.HardWareVersion = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.HardWareVersionKey, productSavePath);
            productTestConfig.SoftWareVersion = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.SoftWareVersionKey, productSavePath);
            productTestConfig.DormantElectricMin = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.DormantElectricMinKey, productSavePath);
            productTestConfig.DormantElectricMax = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.DormantElectricMaxKey, productSavePath);
            productTestConfig.BootLoader = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.BootLoaderKey, productSavePath);
            productTestConfig.PorterRate = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.PorterRateKey, productSavePath);
            productTestConfig.SendCanID = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.SendCanIDKey, productSavePath);
            productTestConfig.ReceiveCanID = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.ReceiveCanIDKey, productSavePath);
            productTestConfig.CycleCanID = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.CycleCanIDKey, productSavePath);
            productTestConfig.RF_CAN_ID = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.RF_CAN_IDKey, productSavePath);
            productTestConfig.TestSerial = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.TestSerialKey, productSavePath);
            productTestConfig.ControlPower = INIFile.GetValue(standCommon.ProductTypeNo, ProductTestConfig.ControlPowerKey, productSavePath);
        }

        private void ReadProductCheckStandConfig()
        {
            var productCheckInitPath = StandCommon.CheckProductStationConfigPath + standCommon.ProductTypeNo + "\\";
            var productCheckFileName = StandCommon.CheckProductStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var productCheckSavePath = productCheckInitPath + productCheckFileName;
            productCheckConfig.PlcAddress = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.PlcAddressKey, productCheckSavePath);
            productCheckConfig.LocalAddress = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.LocalAddressKey, productCheckSavePath);
            productCheckConfig.Avometer = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.AvometerKey, productCheckSavePath);
            productCheckConfig.TestBoard = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.TestBoardKey, productCheckSavePath);
            productCheckConfig.WorkElectricMin = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.WorkElectricMinKey, productCheckSavePath);
            productCheckConfig.WorkElectricMax = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.WorkElectricMaxKey, productCheckSavePath);
            productCheckConfig.PartNumber = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.PartNumberKey, productCheckSavePath);
            productCheckConfig.HardWareVersion = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.HardWareVersionKey, productCheckSavePath);
            productCheckConfig.SoftWareVersion = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.SoftWareVersionKey, productCheckSavePath);
            productCheckConfig.DormantElectricMin = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.DormantElectricMinKey, productCheckSavePath);
            productCheckConfig.DormantElectricMax = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.DormantElectricMaxKey, productCheckSavePath);
            productCheckConfig.BootLoader = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.BootLoaderKey, productCheckSavePath);
            productCheckConfig.PorterRate = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.PorterRateKey, productCheckSavePath);
            productCheckConfig.SendCanID = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.SendCanIDKey, productCheckSavePath);
            productCheckConfig.ReceiveCanID = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.ReceiveCanIDKey, productCheckSavePath);
            productCheckConfig.CycleCanID = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.CycleCanIDKey, productCheckSavePath);
            productCheckConfig.RF_CAN_ID = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.RF_CAN_IDKey, productCheckSavePath);
            productCheckConfig.TestSerial = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.TestSerialKey, productCheckSavePath);
            productCheckConfig.ControlPower = INIFile.GetValue(standCommon.ProductTypeNo, ProductCheckConfig.ControlPowerKey, productCheckSavePath);
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
        private void SaveBurnStandConfig()
        {
            //单独保存烧录文件路径
            //var burnSavePath = AppDomain.CurrentDomain.BaseDirectory +CommConfig.DeafaultConfigRoot + CommConfig.BurnConfigIniName;
            //INIFile.SetValue(standCommon.ProductTypeNo,CommConfig.BurnConfigPathKey,burnConfig.ProgramePath,burnSavePath);
            var burnInitPath = StandCommon.TurnStationConfigPath + standCommon.ProductTypeNo + "\\";
            var burnFileName = StandCommon.TurnStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var burnSavePath = burnInitPath + burnFileName;
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.PowerValueKey, burnConfig.PowerValue, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.LocalAddressKey,burnConfig.LocalAddress, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.AvometerAddressKey, burnConfig.AvometerAddress, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.AutoSweepCodeComKey, burnConfig.AutoSweepCodeCom, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.BurnerKey, burnConfig.Burner, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.PorterRateKey, burnConfig.PorterRate, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.CanIdKey, burnConfig.CanId, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.ProductIdKey, burnConfig.ProductId, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.FirstVoltageMaxKey, burnConfig.FirstVoltageMax, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.FirstVoltageMinKey, burnConfig.FirstVoltageMin, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.SecondVoltageMaxKey, burnConfig.SecondVoltageMin, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.SecondVoltageMinKey, burnConfig.SecondVoltageMin, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.HardWareVersionKey, burnConfig.HardWareVersion, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.SoftWareVersionKey, burnConfig.SoftWareVersion, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.PartNumberKey, burnConfig.PartNumber, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.ProgramePathKey, burnConfig.ProgramePath, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.ProgrameNameKey, burnConfig.ProgrameName, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.SerialNumberKey, burnConfig.SerialNumber, burnSavePath);
            MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            IsSavePrivateConfig = true;
        }

        /// <summary>
        /// 保存通用配置，不同工站下的当前型号
        /// </summary>
        private void SaveCommonStandConfig()
        {
            //烧录文件保存通用配置
            var burnInitPath = StandCommon.TurnStationConfigPath + standCommon.ProductTypeNo + "\\";
            var burnFileName = StandCommon.TurnStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var burnSavePath = burnInitPath + burnFileName;
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PCBABarCodeLengthKey, standCommon.PCBABarCodeLength, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.ShellBarCodeLengthKey, standCommon.ShellBarCodeLength, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.CaseBarCodeLengthKey, standCommon.CaseBarCodeLength, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PackageCaseAmountKey, standCommon.PackageCaseAmount, burnSavePath);

            //灵敏度文件保存通用配置
            var senInitPath = StandCommon.SensibilityStationConfigPath + standCommon.ProductTypeNo + "\\";
            var senFileName = StandCommon.SensibilityStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var senSavePath = senInitPath + senFileName;
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PCBABarCodeLengthKey, standCommon.PCBABarCodeLength, senSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.ShellBarCodeLengthKey, standCommon.ShellBarCodeLength, senSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.CaseBarCodeLengthKey, standCommon.CaseBarCodeLength, senSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PackageCaseAmountKey, standCommon.PackageCaseAmount, senSavePath);

            //外壳装配文件保存通用配置
            var shellInitPath = StandCommon.ShellStationConfigPath + standCommon.ProductTypeNo + "\\";
            var shellFileName = StandCommon.ShellStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var shellSavePath = shellInitPath + shellFileName;
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PCBABarCodeLengthKey, standCommon.PCBABarCodeLength, shellSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.ShellBarCodeLengthKey, standCommon.ShellBarCodeLength, shellSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.CaseBarCodeLengthKey, standCommon.CaseBarCodeLength, shellSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PackageCaseAmountKey, standCommon.PackageCaseAmount, shellSavePath);

            //气密文件保存通用配置
            var airtageInitPath = StandCommon.AirtageStationConfigPath + standCommon.ProductTypeNo + "\\";
            var airtageFileName = StandCommon.AirtageStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var airtageSavePath = airtageInitPath + airtageFileName;
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PCBABarCodeLengthKey, standCommon.PCBABarCodeLength, airtageSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.ShellBarCodeLengthKey, standCommon.ShellBarCodeLength, airtageSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.CaseBarCodeLengthKey, standCommon.CaseBarCodeLength, airtageSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PackageCaseAmountKey, standCommon.PackageCaseAmount, airtageSavePath);

            //支架装配文件保存通用配置
            var stentInitPath = StandCommon.StentStationConfigPath + standCommon.ProductTypeNo + "\\";
            var stentFileName = StandCommon.StentStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var stentSavePath = stentInitPath + stentFileName;
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PCBABarCodeLengthKey, standCommon.PCBABarCodeLength, stentSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.ShellBarCodeLengthKey, standCommon.ShellBarCodeLength, stentSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.CaseBarCodeLengthKey, standCommon.CaseBarCodeLength, stentSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PackageCaseAmountKey, standCommon.PackageCaseAmount, stentSavePath);

            //成品测试文件保存通用配置
            var productTestInitPath = StandCommon.ProductFinishStationConfigPath + standCommon.ProductTypeNo + "\\";
            var productTestFileName = StandCommon.ProductFinishStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var productTestSavePath = productTestInitPath + productTestFileName;
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PCBABarCodeLengthKey, standCommon.PCBABarCodeLength, productTestSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.ShellBarCodeLengthKey, standCommon.ShellBarCodeLength, productTestSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.CaseBarCodeLengthKey, standCommon.CaseBarCodeLength, productTestSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PackageCaseAmountKey, standCommon.PackageCaseAmount, productTestSavePath);

            //成品抽检文件保存通用配置
            var productCheckInitPath = StandCommon.CheckProductStationConfigPath + standCommon.ProductTypeNo + "\\";
            var productCheckFileName = StandCommon.CheckProductStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var productCheckSavePath = productCheckInitPath + productCheckFileName;
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PCBABarCodeLengthKey, standCommon.PCBABarCodeLength, productCheckSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.ShellBarCodeLengthKey, standCommon.ShellBarCodeLength, productCheckSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.CaseBarCodeLengthKey, standCommon.CaseBarCodeLength, productCheckSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StandCommon.PackageCaseAmountKey, standCommon.PackageCaseAmount, productCheckSavePath);
            if (IsSavePrivateConfig)
            {
                IsSavePrivateConfig = false;
                return;
            }
            MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SaveSensibilityStandConfig()
        {
            var senInitPath = StandCommon.SensibilityStationConfigPath + standCommon.ProductTypeNo + "\\";
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
            INIFile.SetValue(standCommon.ProductTypeNo, SensibilityConfig.ProductSerialKey, sensibilityConfig.ProductSerial, senSavePath);
            MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            IsSavePrivateConfig = true;
        }

        private void SaveShellStandConfig()
        {
            var shellInitPath = StandCommon.ShellStationConfigPath + standCommon.ProductTypeNo + "\\";
            var shellFileName = StandCommon.ShellStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var shellSavePath = shellInitPath + shellFileName;
            INIFile.SetValue(standCommon.ProductTypeNo, ShellConfig.LocalAddressConMesKey, shellConfig.LocalAddressConMes, shellSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ShellConfig.LocalAddressConPLCKey, shellConfig.LocalAddressConPLC, shellSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ShellConfig.PLCAddressKey, shellConfig.PLCAddress, shellSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ShellConfig.SmallScrewSetTimeKey, shellConfig.SmallScrewSetTime, shellSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ShellConfig.LargeScrewSetTimeKey, shellConfig.LargeScrewSetTime, shellSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, ShellConfig.TestSerialNumberKey, shellConfig.TestSerialNumber, shellSavePath);
            MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            IsSavePrivateConfig = true;
        }

        private void SaveAirtageStandConfig()
        {
            var airtageInitPath = StandCommon.AirtageStationConfigPath + standCommon.ProductTypeNo + "\\";
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
            INIFile.SetValue(standCommon.ProductTypeNo, AirtageConfig.TestConditionValueKey, airtageConfig.TestConditionValue, airtageSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, AirtageConfig.ReferenceConditionValueKey, airtageConfig.ReferenceConditionValue, airtageSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, AirtageConfig.TestSerialKey, airtageConfig.TestSerial, airtageSavePath);
            MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            IsSavePrivateConfig = true;
        }

        private void SaveStentStandConfig()
        {
            var stentInitPath = StandCommon.StentStationConfigPath + standCommon.ProductTypeNo + "\\";
            var stentFileName = StandCommon.StentStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var stentSavePath = stentInitPath + stentFileName;
            INIFile.SetValue(standCommon.ProductTypeNo, StentConfig.LocalAddressConMesKey, stentConfig.LocalAddressConMes, stentSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, StentConfig.TestSerialKey, stentConfig.TestSerial, stentSavePath);
            MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            IsSavePrivateConfig = true;
        }

        private void SaveProductTestConfig()
        {
            var productInitPath = StandCommon.ProductFinishStationConfigPath + standCommon.ProductTypeNo + "\\";
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
            INIFile.SetValue(standCommon.ProductTypeNo, ProductTestConfig.TestSerialKey, productTestConfig.TestSerial, productSavePath);
            MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            IsSavePrivateConfig = true;
        }

        private void SaveProductCheckConfig()
        {
            var productCheckInitPath = StandCommon.CheckProductStationConfigPath + standCommon.ProductTypeNo + "\\";
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
            INIFile.SetValue(standCommon.ProductTypeNo, ProductCheckConfig.TestSerialKey, productCheckConfig.TestSerial, productCheckSavePath);
            MessageBox.Show("保存成功！","提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
            IsSavePrivateConfig = true;
        }
        #endregion

        //需要进一步验证
        #region Check UI params

        private void CheckConfigParams()
        {
            CheckBurnConfigParams();
            CheckCommonConfigParams();
            CheckSensibilityConfigParams();
            CheckShellConfigParams();
            CheckAirtageConfigParams();
            CheckStentConfigParams();
            CheckProductTestConfigParams();
            CheckProductCheckConfigParams();
        }

        private void CheckBurnConfigParams()
        {
            burnConfig.PowerValue = this.tb_burn_power.Text;
            burnConfig.LocalAddress = this.tb_turn_localIP.Text;
            burnConfig.AvometerAddress = this.tb_burn_avometer.Text;
            burnConfig.AutoSweepCodeCom = this.tb_burn_autoSweepCode.Text;
            burnConfig.Burner = this.tb_burn_burner.Text;
            PorterRateEnum porterRateEnum;
            Enum.TryParse(PorterRate.PorterStringToEnum(this.cb_burn_porterRate.Text),out porterRateEnum);
            burnConfig.PorterRate = (int)porterRateEnum + "";
            burnConfig.CanId = this.tb_burn_canID.Text;
            burnConfig.ProductId = this.tb_burn_productID.Text;
            burnConfig.FirstVoltageMax = this.tb_burn_firstVoltageMax.Text;
            burnConfig.FirstVoltageMin = this.tb_burn_firstVoltageMin.Text;
            burnConfig.SecondVoltageMax = this.tb_burn_secondVoltageMax.Text;
            burnConfig.SecondVoltageMin = this.tb_burn_secondVoltageMin.Text;
            burnConfig.HardWareVersion = this.tb_burn_hardWareVersion.Text;
            burnConfig.SoftWareVersion = this.tb_burn_softWareVersion.Text;
            burnConfig.PartNumber = this.tb_burn_partNumber.Text;
            burnConfig.ProgramePath = this.tb_burn_programePath.Text;
            burnConfig.ProgrameName = this.tb_burn_programeName.Text;
            burnConfig.SerialNumber = GetBurnSerial(this.cb_burn_serialNumber.Text).ProductSerialPath;
        }

        private void CheckCommonConfigParams()
        {
            standCommon.PCBABarCodeLength = this.tb_common_pcbCodeLen.Text;
            standCommon.ShellBarCodeLength = this.tb_common_shellCodeLen.Text;
            standCommon.CaseBarCodeLength = this.tb_common_caseCodeLen.Text;
            standCommon.PackageCaseAmount = this.tb_common_packageAmount.Text;
        }

        private void CheckSensibilityConfigParams()
        {
            sensibilityConfig.PLCAddress = this.tb_sen_plc.Text;
            sensibilityConfig.LocalAddress = this.tb_sen_localAddress.Text;
            sensibilityConfig.AvometerAddress = this.tb_sen_avometer.Text;
            sensibilityConfig.AutoSweepCode = this.tb_sen_autoSweepCode.Text;
            sensibilityConfig.ProgrameControlPower = this.tb_sen_power.Text;
            sensibilityConfig.WorkElectricMin = this.tb_sen_workElectricMin.Text;
            sensibilityConfig.WorkElectricMax = this.tb_sen_workElectricMax.Text;
            sensibilityConfig.PartNumber = this.tb_sen_partNumber.Text;
            sensibilityConfig.HardWareVersion = this.tb_sen_hardWareVersion.Text;
            sensibilityConfig.SoftWareVersion = this.tb_sen_softWareVersion.Text;
            sensibilityConfig.DormantElectricMin = this.tb_sen_dormantElectricMin.Text;
            sensibilityConfig.DormantElectricMax = this.tb_sen_dormantElectricMax.Text;
            sensibilityConfig.BootLoader = this.tb_sen_boootloader.Text;
            PorterRateEnum porterRateEnum;
            Enum.TryParse(PorterRate.PorterStringToEnum(this.tb_sen_porterRate.Text), out porterRateEnum);
            sensibilityConfig.PorterRate = (int)porterRateEnum + "";
            sensibilityConfig.SendCanID = this.tb_sen_sendCanID.Text;
            sensibilityConfig.ProductSerial = this.cb_sen_serialNumber.Text;
            sensibilityConfig.ReceiveCanID = this.tb_sen_receiveCanID.Text;
            sensibilityConfig.RfCanID = this.tb_sen_rfCanID.Text;
            sensibilityConfig.CyclyCanID = this.tb_sen_cycleCanID.Text;
        }

        private void CheckShellConfigParams()
        {
            shellConfig.LocalAddressConMes = this.tb_shell_localConMes.Text;
            shellConfig.LocalAddressConPLC = this.tb_shell_localIPConPLC.Text;
            shellConfig.PLCAddress = this.tb_shell_plcAddress.Text;
            shellConfig.SmallScrewSetTime = this.tb_shell_smallScrewSetTime.Text;
            shellConfig.LargeScrewSetTime = this.tb_shell_largeScrewSetTime.Text;
            shellConfig.TestSerialNumber = this.cb_shell_testSerial.Text;
        }

        private void CheckAirtageConfigParams()
        {
            airtageConfig.LocalAddressConMes = this.tb_airtage_localIPConMes.Text;
            airtageConfig.AirTester = this.tb_airtage_tester.Text;
            airtageConfig.InflateAirTime = this.tb_airtage_inflateTime.Text;
            airtageConfig.StableTime = this.tb_airtage_stableTime.Text;
            airtageConfig.TestTime = this.tb_airtage_testTime.Text;
            airtageConfig.PressureUnit = this.tb_airtage_pressureUnit.Text;
            airtageConfig.SpreadUnit = this.tb_airtage_spread.Text;
            airtageConfig.MaxInflate = this.tb_airtage_maxInflate.Text;
            airtageConfig.MinInflate = this.tb_airtage_minFlate.Text;
            airtageConfig.TestConditionValue = this.tb_airtage_testConditionValue.Text;
            airtageConfig.ReferenceConditionValue = this.tb_airtage_referenceConditionValue.Text;
            airtageConfig.TestSerial = this.tb_airtage_testSerial.Text;
        }

        private void CheckStentConfigParams()
        {
            stentConfig.LocalAddressConMes = this.tb_stent_localIPConMes.Text;
            stentConfig.TestSerial = this.cb_stent_testSerial.Text;
        }

        private void CheckProductTestConfigParams()
        {
            productTestConfig.PlcAddress = this.tb_product_plcAddress.Text;
            productTestConfig.LocalAddress = this.tb_product_localAddress.Text;
            productTestConfig.Avometer = this.tb_product_avometer.Text;
            productTestConfig.AutoSweepCode = this.tb_product_autoSweepCode.Text;
            productTestConfig.TestBoard = this.tb_product_testBoard.Text;
            productTestConfig.ControlPower = this.tb_product_controlPower.Text;
            productTestConfig.WorkElectricMin = this.tb_product_workElectricMin.Text;
            productTestConfig.WorkElectricMax = this.tb_product_workElectricMax.Text;
            productTestConfig.PartNumber = this.tb_product_partNumber.Text;
            productTestConfig.HardWareVersion = this.tb_product_hardWareVersion.Text;
            productTestConfig.SoftWareVersion = this.tb_product_softWareVersion.Text;
            productTestConfig.DormantElectricMin = this.tb_product_dormantElectricMin.Text;
            productTestConfig.DormantElectricMax = this.tb_product_dormantElectricMax.Text;
            productTestConfig.BootLoader = this.tb_prouct_bootLoader.Text;
            PorterRateEnum porterRateEnum;
            Enum.TryParse(PorterRate.PorterStringToEnum(this.tb_product_porterRate.Text), out porterRateEnum);
            productTestConfig.PorterRate = (int)porterRateEnum + "";
            productTestConfig.SendCanID = this.tb_product_sendCanID.Text;
            productTestConfig.ReceiveCanID = this.tb_product_receiveCanID.Text;
            productTestConfig.CycleCanID = this.tb_product_cyclyCanID.Text;
            productTestConfig.RF_CAN_ID = this.tb_product_rfCanID.Text;
            productTestConfig.TestSerial = this.cb_product_testSerial.Text;
        }

        private void CheckProductCheckConfigParams()
        {
            productCheckConfig.PlcAddress = this.tb_productCheck_plcAddress.Text;
            productCheckConfig.LocalAddress = this.tb_productCheck_localAddress.Text;
            productCheckConfig.Avometer = this.tb_productCheck_avometer.Text;
            productCheckConfig.TestBoard = this.tb_productCheck_testBoard.Text;
            productCheckConfig.ControlPower = this.tb_productCheck_controlPower.Text;
            productCheckConfig.WorkElectricMin = this.tb_productCheck_workElectricMin.Text;
            productCheckConfig.WorkElectricMax = this.tb_productCheck_workElectricMax.Text;
            productCheckConfig.PartNumber = this.tb_productCheck_partNumber.Text;
            productCheckConfig.HardWareVersion = this.tb_productCheck_hardWareVersion.Text;
            productCheckConfig.SoftWareVersion = this.tb_productCheck_softWareVersion.Text;
            productCheckConfig.DormantElectricMin = this.tb_productCheck_dormantElectricMin.Text;
            productCheckConfig.DormantElectricMax = this.tb_productCheck_dormantElectricMax.Text;
            productCheckConfig.BootLoader = this.tb_productCheck_bootLoader.Text;
            PorterRateEnum porterRateEnum;
            Enum.TryParse(PorterRate.PorterStringToEnum(this.tb_productCheck_porterRate.Text), out porterRateEnum);
            productCheckConfig.PorterRate = (int)porterRateEnum + "";
            productCheckConfig.SendCanID = this.tb_productCheck_sendCanID.Text;
            productCheckConfig.ReceiveCanID = this.tb_productCheck_receiveCanID.Text;
            productCheckConfig.CycleCanID = this.tb_productCheck_cycleCanID.Text;
            productCheckConfig.RF_CAN_ID = this.tb_productCheck_rfCanID.Text;
            productCheckConfig.TestSerial = this.tb_productCheck_testSerial.Text;
        }
        #endregion

        #region refreshUI

        private void RefreshUI()
        {
            RefreshUIBurnStation();
            RefreshUICommonConfig();
            RefreshUISensibilityStation();
            RefreshUIShellStation();
            RefreshUIAirtageStation();
            RefreshUIAirtageStation();
            RefreshUIStentStation();
            RefreshUIProductTestStation();
            RefreshUIProductCheckStation();
        }

        private void RefreshUIBurnStation()
        {
            this.tb_burn_power.Text = burnConfig.PowerValue;
            this.tb_turn_localIP.Text = burnConfig.LocalAddress;
            this.tb_burn_avometer.Text = burnConfig.AvometerAddress;
            this.tb_burn_autoSweepCode.Text = burnConfig.AutoSweepCodeCom;
            this.tb_burn_burner.Text = burnConfig.Burner;
            PorterRateEnum porterRateEnum;
            Enum.TryParse(burnConfig.PorterRate,out porterRateEnum);
            this.cb_burn_porterRate.Text = PorterRate.PorterEnumToString(porterRateEnum.ToString());
            this.tb_burn_canID.Text = burnConfig.CanId;
            this.tb_burn_productID.Text = burnConfig.ProductId;
            this.tb_burn_firstVoltageMax.Text = burnConfig.FirstVoltageMax;
            this.tb_burn_firstVoltageMin.Text = burnConfig.FirstVoltageMin;
            this.tb_burn_secondVoltageMax.Text = burnConfig.SecondVoltageMax;
            this.tb_burn_secondVoltageMin.Text = burnConfig.SecondVoltageMin;
            this.tb_burn_hardWareVersion.Text = burnConfig.HardWareVersion;
            this.tb_burn_softWareVersion.Text = burnConfig.SoftWareVersion;
            this.tb_burn_partNumber.Text = burnConfig.PartNumber;
            this.cb_burn_serialNumber.Text = burnConfig.SerialNumber;
            this.tb_burn_programePath.Text = burnConfig.ProgramePath;
            this.tb_burn_programeName.Text = burnConfig.ProgrameName;
        }

        private void RefreshUICommonConfig()
        {
            this.tb_common_pcbCodeLen.Text = standCommon.PCBABarCodeLength;
            this.tb_common_shellCodeLen.Text = standCommon.ShellBarCodeLength;
            this.tb_common_caseCodeLen.Text = standCommon.CaseBarCodeLength;
            this.tb_common_packageAmount.Text = standCommon.PackageCaseAmount;
        }

        private void RefreshUISensibilityStation()
        {
            this.tb_sen_plc.Text = sensibilityConfig.PLCAddress;
            this.tb_sen_localAddress.Text = sensibilityConfig.LocalAddress;
            this.tb_sen_avometer.Text = sensibilityConfig.AvometerAddress;
            this.tb_sen_autoSweepCode.Text = sensibilityConfig.AutoSweepCode;
            this.tb_sen_power.Text = sensibilityConfig.ProgrameControlPower;
            this.tb_sen_workElectricMin.Text = sensibilityConfig.WorkElectricMin;
            this.tb_sen_workElectricMax.Text = sensibilityConfig.WorkElectricMax;
            this.tb_sen_partNumber.Text = sensibilityConfig.PartNumber;
            this.tb_sen_hardWareVersion.Text = sensibilityConfig.HardWareVersion;
            this.tb_sen_softWareVersion.Text = sensibilityConfig.SoftWareVersion;
            this.tb_sen_dormantElectricMin.Text = sensibilityConfig.DormantElectricMin;
            this.tb_sen_dormantElectricMax.Text = sensibilityConfig.DormantElectricMax;
            this.tb_sen_boootloader.Text = sensibilityConfig.BootLoader;
            PorterRateEnum porterRateEnum;
            Enum.TryParse(sensibilityConfig.PorterRate, out porterRateEnum);
            this.tb_sen_porterRate.Text = PorterRate.PorterEnumToString(porterRateEnum.ToString());
            this.tb_sen_sendCanID.Text = sensibilityConfig.SendCanID;
            this.cb_sen_serialNumber.Text = sensibilityConfig.ProductSerial;
            this.tb_sen_sendCanID.Text = sensibilityConfig.SendCanID;
            this.tb_sen_receiveCanID.Text = sensibilityConfig.ReceiveCanID;
            this.tb_sen_cycleCanID.Text = sensibilityConfig.CyclyCanID;
            this.tb_sen_rfCanID.Text = sensibilityConfig.RfCanID;
        }

        private void RefreshUIShellStation()
        {
            this.tb_shell_localConMes.Text = shellConfig.LocalAddressConMes ;
            this.tb_shell_localIPConPLC.Text = shellConfig.LocalAddressConPLC;
            this.tb_shell_plcAddress.Text = shellConfig.PLCAddress;
            this.tb_shell_smallScrewSetTime.Text = shellConfig.SmallScrewSetTime ;
            this.tb_shell_largeScrewSetTime.Text = shellConfig.LargeScrewSetTime;
            this.cb_shell_testSerial.Text = shellConfig.TestSerialNumber;
        }

        private void RefreshUIAirtageStation()
        {
            this.tb_airtage_localIPConMes.Text = airtageConfig.LocalAddressConMes;
            this.tb_airtage_tester.Text = airtageConfig.AirTester;
            this.tb_airtage_inflateTime.Text = airtageConfig.InflateAirTime;
            this.tb_airtage_stableTime.Text = airtageConfig.StableTime;
            this.tb_airtage_testTime.Text = airtageConfig.TestTime;
            this.tb_airtage_pressureUnit.Text = airtageConfig.PressureUnit;
            this.tb_airtage_spread.Text = airtageConfig.SpreadUnit;
            this.tb_airtage_maxInflate.Text = airtageConfig.MaxInflate;
            this.tb_airtage_minFlate.Text = airtageConfig.MinInflate;
            this.tb_airtage_testConditionValue.Text = airtageConfig.TestConditionValue;
            this.tb_airtage_referenceConditionValue.Text = airtageConfig.ReferenceConditionValue;
            this.tb_airtage_testSerial.Text = airtageConfig.TestSerial;
        }

        private void RefreshUIStentStation()
        {
            this.tb_stent_localIPConMes.Text = stentConfig.LocalAddressConMes;
            this.cb_stent_testSerial.Text = stentConfig.TestSerial;
        }

        private void RefreshUIProductTestStation()
        {
            this.tb_product_plcAddress.Text= productTestConfig.PlcAddress;
            this.tb_product_localAddress.Text = productTestConfig.LocalAddress;
            this.tb_product_avometer.Text = productTestConfig.Avometer ;
            this.tb_product_autoSweepCode.Text = productTestConfig.AutoSweepCode;
            this.tb_product_testBoard.Text = productTestConfig.TestBoard;
            this.tb_product_controlPower.Text = productTestConfig.ControlPower;
            this.tb_product_workElectricMin.Text = productTestConfig.WorkElectricMin;
            this.tb_product_workElectricMax.Text = productTestConfig.WorkElectricMax;
            this.tb_product_partNumber.Text = productTestConfig.PartNumber;
            this.tb_product_hardWareVersion.Text = productTestConfig.HardWareVersion;
            this.tb_product_softWareVersion.Text = productTestConfig.SoftWareVersion;
            this.tb_product_dormantElectricMin.Text = productTestConfig.DormantElectricMin;
            this.tb_product_dormantElectricMax.Text = productTestConfig.DormantElectricMax;
            this.tb_prouct_bootLoader.Text = productTestConfig.BootLoader;
            PorterRateEnum porterRateEnum;
            Enum.TryParse(productTestConfig.PorterRate, out porterRateEnum);
            this.tb_product_porterRate.Text = PorterRate.PorterEnumToString(porterRateEnum.ToString());
            this.tb_product_sendCanID.Text = productTestConfig.SendCanID;
            this.tb_product_receiveCanID.Text = productTestConfig.ReceiveCanID;
            this.tb_product_cyclyCanID.Text = productTestConfig.CycleCanID;
            this.tb_product_rfCanID.Text = productTestConfig.RF_CAN_ID;
            this.cb_product_testSerial.Text = productTestConfig.TestSerial;
        }

        private void RefreshUIProductCheckStation()
        {
            this.tb_productCheck_plcAddress.Text = productCheckConfig.PlcAddress;
            this.tb_productCheck_localAddress.Text = productCheckConfig.LocalAddress;
            this.tb_productCheck_avometer.Text = productCheckConfig.Avometer;
            this.tb_productCheck_testBoard.Text = productCheckConfig.TestBoard;
            this.tb_productCheck_controlPower.Text = productCheckConfig.ControlPower;
            this.tb_productCheck_workElectricMin.Text = productCheckConfig.WorkElectricMin;
            this.tb_productCheck_workElectricMax.Text = productCheckConfig.WorkElectricMax;
            this.tb_productCheck_partNumber.Text = productCheckConfig.PartNumber;
            this.tb_productCheck_hardWareVersion.Text = productCheckConfig.HardWareVersion;
            this.tb_productCheck_softWareVersion.Text = productCheckConfig.SoftWareVersion;
            this.tb_productCheck_dormantElectricMin.Text = productCheckConfig.DormantElectricMin;
            this.tb_productCheck_dormantElectricMax.Text = productCheckConfig.DormantElectricMax;
            this.tb_productCheck_bootLoader.Text = productCheckConfig.BootLoader;
            PorterRateEnum porterRateEnum;
            Enum.TryParse(productCheckConfig.PorterRate, out porterRateEnum);
            this.tb_productCheck_porterRate.Text = PorterRate.PorterEnumToString(porterRateEnum.ToString());
            this.tb_productCheck_sendCanID.Text = productCheckConfig.SendCanID;
            this.tb_productCheck_receiveCanID.Text = productCheckConfig.ReceiveCanID;
            this.tb_productCheck_cycleCanID.Text = productTestConfig.CycleCanID;
            this.tb_productCheck_rfCanID.Text = productCheckConfig.RF_CAN_ID;
            this.tb_productCheck_testSerial.Text = productCheckConfig.TestSerial;
        }
        #endregion

        private string GetProductTestSerial(string serialPath)
        {
            if (!serialPath.Contains("\\") && !serialPath.Contains("\\\\"))
            {
                LogHelper.Log.Error("【产品序列路径不合法】");
                return "";
            }
            return serialPath.Substring(serialPath.LastIndexOf('\\') + 1);
        }

        private BurnConfig GetBurnSerial(string serialName)
        {
            return burnConfigList.Find(m => m.SerialNumber == serialName);
        }
    }
}
