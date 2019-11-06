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
using System.Configuration;

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
        List<BurnConfig> burnConfigList = new List<BurnConfig>();
        List<SensibilityConfig> sensibilityConfigList = new List<SensibilityConfig>();
        List<ShellConfig> shellConfigList = new List<ShellConfig>();
        List<AirtageConfig> airtageConfigList = new List<AirtageConfig>();
        List<StentConfig> stentConfigList = new List<StentConfig>();
        List<ProductTestConfig> productTestConfigList = new List<ProductTestConfig>();
        List<ProductCheckConfig> productCheckConfigList = new List<ProductCheckConfig>();
        private string standMapRoot;//机台使用映射根目录
        private string defaultRoot;//机台配置本地根目录

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
            bool IsContinue = false;
            var productTest = GetProductTestConfig(this.cb_product_testSerial.Text);
            if (productTest == null)
            {
                if(MessageBox.Show("该序列是无效序列，是否继续保存？","提示",MessageBoxButtons.OKCancel,MessageBoxIcon.Warning,MessageBoxDefaultButton.Button2) != DialogResult.OK)
                    return;
                IsContinue = true;
            }
            if (!IsContinue)
            {
                if (MessageBox.Show($"序列【{productTest.TestSerial}】的供电电压为{productTest.SupplyVoltage}\r\n确认选择无误，并保存配置？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                    return;
            }
            if(!InitStandConfig.InitDirectory(InitStandConfig.StandConfigType.productTest))
                return;
            CheckProductTestConfigParams();
            SaveProductTestConfig();
            CheckCommonConfigParams();
            SaveCommonStandConfig();
        }

        private void Btn_productCheck_save_Click(object sender, EventArgs e)
        {
            bool IsContinue = false;
            var productCheck = GetProductCheckConfig(this.tb_productCheck_testSerial.Text);
            if (productCheck == null)
            {
                if (MessageBox.Show("该序列是无效序列，是否继续保存？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                    return;
                IsContinue = true;
            }
            if (!IsContinue)
            {
                if (MessageBox.Show($"序列【{productCheck.TestSerial}】的供电电压为{productCheck.SupplyVoltage}\r\n确认选择无误，并保存配置？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                    return;
            }
            if(!InitStandConfig.InitDirectory(InitStandConfig.StandConfigType.productCheck))
                return;
            CheckProductCheckConfigParams();
            SaveProductCheckConfig();
            CheckCommonConfigParams();
            SaveCommonStandConfig();
        }

        private void Btn_airtage_save_Click(object sender, EventArgs e)
        {
            bool IsContinue = false;
            var airtage = GetAirtageConfig(this.tb_airtage_testSerial.Text);
            if (airtage == null)
            {
                if (MessageBox.Show("该序列是无效序列，是否继续保存？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                    return;
                IsContinue = true;
            }
            if (!IsContinue)
            {
                if (MessageBox.Show($"序列【{airtage.TestSerial}】的供电电压为{airtage.SupplyVoltage}\r\n确认选择无误，并保存配置？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                    return;
            }
            if(!InitStandConfig.InitDirectory(InitStandConfig.StandConfigType.airtage))
                return;
            CheckAirtageConfigParams();
            SaveAirtageStandConfig();
            CheckCommonConfigParams();
            SaveCommonStandConfig();
        }

        private void Btn_stent_save_Click(object sender, EventArgs e)
        {
            bool IsContinue = false;
            var stent = GetStentConfig(this.cb_stent_testSerial.Text);
            if (stent == null)
            {
                if (MessageBox.Show("该序列是无效序列，是否继续保存？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                    return;
                IsContinue = true;
            }
            if (!IsContinue)
            {
                if (MessageBox.Show($"序列【{stent.TestSerial}】的供电电压为{stent.SupplyVoltage}\r\n确认选择无误，并保存配置？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                    return;
            }
            if(!InitStandConfig.InitDirectory(InitStandConfig.StandConfigType.stent))
                return;
            CheckStentConfigParams();
            SaveStentStandConfig();
            CheckCommonConfigParams();
            SaveCommonStandConfig();
        }

        private void Btn_shell_save_Click(object sender, EventArgs e)
        {
            bool IsContinue = false;
            var shell = GetShellConfig(this.cb_shell_testSerial.Text);
            if (shell == null)
            {
                if (MessageBox.Show("该序列是无效序列，是否继续保存？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                    return;
                IsContinue = true;
            }
            if (!IsContinue)
            {
                if (MessageBox.Show($"序列【{shell.TestSerialNumber}】的供电电压为{shell.SupplyVoltage}\r\n确认选择无误，并保存配置？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                    return;
            }
            if(!InitStandConfig.InitDirectory(InitStandConfig.StandConfigType.shell))
                return;
            CheckShellConfigParams();
            SaveShellStandConfig();
            CheckCommonConfigParams();
            SaveCommonStandConfig();
        }

        private void Btn_sensilibity_save_Click(object sender, EventArgs e)
        {
            bool IsContinue = false;
            var sen = GetSensibilityConfig(this.cb_sen_serialNumber.Text);
            if (sen == null)
            {
                if (MessageBox.Show("该序列是无效序列，是否继续保存？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                    return;
                IsContinue = true;
            }
            if (!IsContinue)
            {
                if (MessageBox.Show($"序列【{sen.ProductSerial}】的供电电压为{sen.SupplyVoltage}\r\n确认选择无误，并保存配置？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                    return;
            }
            if(!InitStandConfig.InitDirectory(InitStandConfig.StandConfigType.sensibility))
                return;
            CheckSensibilityConfigParams();
            SaveSensibilityStandConfig();
            CheckCommonConfigParams();
            SaveCommonStandConfig();
        }

        private void Btn_burn_save_Click(object sender, EventArgs e)
        {
            bool IsContinue = false;
            var burn = GetBurnSerialConfig(this.cb_burn_serialNumber.Text);
            if (burn == null)
            {
                if (MessageBox.Show("该序列是无效序列，是否继续保存？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                    return;
                IsContinue = true;
            }
            if (!IsContinue)
            {
                if (MessageBox.Show($"序列【{burn.SerialNumber}】的供电电压为{burn.SupplyVoltage}\r\n确认选择无误，并保存配置？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                    return;
            }
            if(!InitStandConfig.InitDirectory(InitStandConfig.StandConfigType.burn))
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
            //初始化压力/泄露单位
            this.tb_airtage_spread.DataSource = AirtageTestItem.AirtageSpreadUnitItem();
            this.tb_airtage_pressureUnit.DataSource = AirtageTestItem.AirtagePressureUnitItem();
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
                    INIFile.SetValue(CommConfig.BurnStationSection,CommConfig.SerialNumberKey+i,"turntestserial_"+i,serialPath);
                    INIFile.SetValue(CommConfig.BurnStationSection, CommConfig.SerialVoltageKey + i, "turntestserial_" + i, serialPath);
                }
                INIFile.SetValue(CommConfig.BurnStationSection,CommConfig.CountKey,count.ToString(),serialPath);
                //灵敏度
                for (int i = 0; i < count; i++)
                {
                    INIFile.SetValue(CommConfig.SensibilityStationSection, CommConfig.SerialNumberKey + i, "sensibilitytestserial_" + i, serialPath);
                    INIFile.SetValue(CommConfig.SensibilityStationSection, CommConfig.SerialVoltageKey + i, "sensibilitytestserial_" + i, serialPath);
                }
                INIFile.SetValue(CommConfig.SensibilityStationSection, CommConfig.CountKey, count.ToString(), serialPath);
                //外壳装配
                for (int i = 0; i < count; i++)
                {
                    INIFile.SetValue(CommConfig.ShellStationSection, CommConfig.SerialNumberKey + i, "shell testserial_" + i, serialPath);
                    INIFile.SetValue(CommConfig.ShellStationSection, CommConfig.SerialVoltageKey + i, "sensibilitytestserial_" + i, serialPath);
                }
                INIFile.SetValue(CommConfig.ShellStationSection, CommConfig.CountKey, count.ToString(), serialPath);
                //气密
                for (int i = 0; i < count; i++)
                {
                    INIFile.SetValue(CommConfig.AirtageStationSection, CommConfig.SerialNumberKey + i, "airtagetestserial_" + i, serialPath);
                    INIFile.SetValue(CommConfig.AirtageStationSection, CommConfig.SerialVoltageKey + i, "sensibilitytestserial_" + i, serialPath);
                }
                INIFile.SetValue(CommConfig.AirtageStationSection, CommConfig.CountKey, count.ToString(), serialPath);
                //支架装配
                for (int i = 0; i < count; i++)
                {
                    INIFile.SetValue(CommConfig.StentStationSection, CommConfig.SerialNumberKey + i, "stenttestserial_" + i, serialPath);
                    INIFile.SetValue(CommConfig.StentStationSection, CommConfig.SerialVoltageKey + i, "sensibilitytestserial_" + i, serialPath);
                }
                INIFile.SetValue(CommConfig.StentStationSection, CommConfig.CountKey, count.ToString(), serialPath);
                //成品测试
                for (int i = 0; i < count; i++)
                {
                    INIFile.SetValue(CommConfig.ProductFinishStationSection, CommConfig.SerialNumberKey + i, "producttestserial_" + i, serialPath);
                    INIFile.SetValue(CommConfig.ProductFinishStationSection, CommConfig.SerialVoltageKey + i, "sensibilitytestserial_" + i, serialPath);
                }
                INIFile.SetValue(CommConfig.ProductFinishStationSection, CommConfig.CountKey, count.ToString(), serialPath);
                //成品抽检
                for (int i = 0; i < count; i++)
                {
                    INIFile.SetValue(CommConfig.CheckProductStationSection, CommConfig.SerialNumberKey + i, "productchecktestserial_" + i, serialPath);
                    INIFile.SetValue(CommConfig.CheckProductStationSection, CommConfig.SerialVoltageKey + i, "sensibilitytestserial_" + i, serialPath);
                }
                INIFile.SetValue(CommConfig.CheckProductStationSection, CommConfig.CountKey, count.ToString(), serialPath);
            }
            //read file content
            int serialCount;
            //读取烧录序列
            testSerialList = new List<string>();
            int.TryParse(INIFile.GetValue(CommConfig.BurnStationSection,CommConfig.CountKey,serialPath),out serialCount);
            this.lbx_burn_tip.Text = "";
            for (int i = 0; i < serialCount; i++)
            {
                BurnConfig burnConfig = new BurnConfig();
                var serialValue = INIFile.GetValue(CommConfig.BurnStationSection,CommConfig.SerialNumberKey+i,serialPath);
                var serialVoltage = INIFile.GetValue(CommConfig.BurnStationSection, CommConfig.SerialVoltageKey + i, serialPath);
                burnConfig.ProductSerialPath = serialValue;
                burnConfig.SerialNumber = GetProductTestSerial(serialValue);
                burnConfig.SupplyVoltage = serialVoltage;
                burnConfigList.Add(burnConfig);
                testSerialList.Add(burnConfig.SerialNumber);
                if (burnConfig.SerialNumber != "" & serialVoltage != "")
                {
                    this.lbx_burn_tip.Text += $"序列【{burnConfig.SerialNumber}】是{serialVoltage}供电\r\n";
                }    
            }
            if (this.lbx_burn_tip.Text == "")
                this.lbx_burn_sign.Visible = false;
            this.cb_burn_serialNumber.DataSource = testSerialList;

            //读取灵敏度序列
            testSerialList = new List<string>();
            this.lbx_sensibility_tip.Text = "";
            int.TryParse(INIFile.GetValue(CommConfig.SensibilityStationSection, CommConfig.CountKey, serialPath), out serialCount);
            for (int i = 0; i < serialCount; i++)
            {
                SensibilityConfig sensibilityConfig = new SensibilityConfig();
                var serialValue = INIFile.GetValue(CommConfig.SensibilityStationSection, CommConfig.SerialNumberKey + i, serialPath);
                var serialVoltage = INIFile.GetValue(CommConfig.SensibilityStationSection, CommConfig.SerialVoltageKey + i, serialPath);
                sensibilityConfig.ProductSerialPath = serialValue;
                sensibilityConfig.ProductSerial = GetProductTestSerial(serialValue);
                sensibilityConfig.SupplyVoltage = serialVoltage;
                sensibilityConfigList.Add(sensibilityConfig);
                testSerialList.Add(sensibilityConfig.ProductSerial);
                if (sensibilityConfig.ProductSerial != "" & serialVoltage != "")
                {
                    this.lbx_sensibility_tip.Text += $"序列【{sensibilityConfig.ProductSerial}】是{serialVoltage}供电\r\n";
                }
            }
            if (this.lbx_sensibility_tip.Text == "")
                this.lbx_sensibility_sign.Visible = false;
            this.cb_sen_serialNumber.DataSource = testSerialList;
            //读取外壳装配序列
            testSerialList = new List<string>();
            this.lbx_shell_tip.Text = "";
            int.TryParse(INIFile.GetValue(CommConfig.ShellStationSection, CommConfig.CountKey, serialPath), out serialCount);
            for (int i = 0; i < serialCount; i++)
            {
                ShellConfig shellConfig = new ShellConfig();
                var serialValue = INIFile.GetValue(CommConfig.ShellStationSection, CommConfig.SerialNumberKey + i, serialPath);
                var serialVoltage = INIFile.GetValue(CommConfig.ShellStationSection, CommConfig.SerialVoltageKey + i, serialPath);
                shellConfig.ProductSerialPath = serialValue;
                shellConfig.TestSerialNumber = GetProductTestSerial(serialValue);
                shellConfig.SupplyVoltage = serialVoltage;
                shellConfigList.Add(shellConfig);
                testSerialList.Add(shellConfig.TestSerialNumber);
                if (shellConfig.TestSerialNumber != "" & serialVoltage != "")
                {
                    this.lbx_shell_tip.Text += $"序列【{shellConfig.TestSerialNumber}】是{serialVoltage}供电\r\n";
                }
            }
            if (this.lbx_shell_tip.Text == "")
                this.lbx_shell_sign.Visible = false;
            this.cb_shell_testSerial.DataSource = testSerialList;
            //读取气密序列
            testSerialList = new List<string>();
            this.lbx_airtage_tip.Text = "";
            int.TryParse(INIFile.GetValue(CommConfig.AirtageStationSection, CommConfig.CountKey, serialPath), out serialCount);
            for (int i = 0; i < serialCount; i++)
            {
                AirtageConfig airtageConfig = new AirtageConfig();
                var serialValue = INIFile.GetValue(CommConfig.AirtageStationSection, CommConfig.SerialNumberKey + i, serialPath);
                var serialVoltage = INIFile.GetValue(CommConfig.AirtageStationSection, CommConfig.SerialVoltageKey + i, serialPath);
                airtageConfig.ProductSerialPath = serialValue;
                airtageConfig.TestSerial = GetProductTestSerial(serialValue);
                airtageConfig.SupplyVoltage = serialVoltage;
                airtageConfigList.Add(airtageConfig);
                testSerialList.Add(airtageConfig.TestSerial);
                if (airtageConfig.TestSerial != "" & serialVoltage != "")
                {
                    this.lbx_airtage_tip.Text += $"序列【{airtageConfig.TestSerial}】是{serialVoltage}供电\r\n";
                }
            }
            if (this.lbx_airtage_tip.Text == "")
                this.lbx_airtage_sign.Visible = false;
            this.tb_airtage_testSerial.DataSource = testSerialList;
            //读取支架装配序列
            testSerialList = new List<string>();
            this.lbx_stent_tip.Text = "";
            int.TryParse(INIFile.GetValue(CommConfig.StentStationSection, CommConfig.CountKey, serialPath), out serialCount);
            for (int i = 0; i < serialCount; i++)
            {
                StentConfig stentConfig = new StentConfig();
                var serialValue = INIFile.GetValue(CommConfig.StentStationSection, CommConfig.SerialNumberKey + i, serialPath);
                var serialVoltage = INIFile.GetValue(CommConfig.StentStationSection, CommConfig.SerialVoltageKey + i, serialPath);
                stentConfig.ProductSerialPath = serialValue;
                stentConfig.TestSerial = GetProductTestSerial(serialValue);
                stentConfig.SupplyVoltage = serialVoltage;
                stentConfigList.Add(stentConfig);
                testSerialList.Add(stentConfig.TestSerial);
                if (stentConfig.TestSerial != "" & serialVoltage != "")
                {
                    this.lbx_stent_tip.Text += $"序列【{stentConfig.TestSerial}】是{serialVoltage}供电\r\n";
                }
            }
            if (this.lbx_stent_tip.Text == "")
                this.lbx_stent_sign.Visible = false;
            this.cb_stent_testSerial.DataSource = testSerialList;
            //读取成品测试序列
            testSerialList = new List<string>();
            this.lbx_productTest_tip.Text = "";
            int.TryParse(INIFile.GetValue(CommConfig.ProductFinishStationSection, CommConfig.CountKey, serialPath), out serialCount);
            for (int i = 0; i < serialCount; i++)
            {
                ProductTestConfig productTestConfig = new ProductTestConfig();
                var serialValue = INIFile.GetValue(CommConfig.ProductFinishStationSection, CommConfig.SerialNumberKey + i, serialPath);
                var serialVoltage = INIFile.GetValue(CommConfig.ProductFinishStationSection, CommConfig.SerialVoltageKey + i, serialPath);
                productTestConfig.ProductSerialPath = serialValue;
                productTestConfig.TestSerial = GetProductTestSerial(serialValue);
                productTestConfig.SupplyVoltage = serialVoltage;
                productTestConfigList.Add(productTestConfig);
                testSerialList.Add(productTestConfig.TestSerial);
                if (productTestConfig.TestSerial != "" & serialVoltage != "")
                {
                    this.lbx_productTest_tip.Text += $"序列【{productTestConfig.TestSerial}】是{serialVoltage}供电\r\n";
                }
            }
            if (this.lbx_productTest_tip.Text == "")
                this.lbx_productTest_sign.Visible = false;
            this.cb_product_testSerial.DataSource = testSerialList;
            //读取成品抽检序列
            testSerialList = new List<string>();
            this.lbx_productCheck_tip.Text = "";
            int.TryParse(INIFile.GetValue(CommConfig.CheckProductStationSection, CommConfig.CountKey, serialPath), out serialCount);
            for (int i = 0; i < serialCount; i++)
            {
                ProductCheckConfig productCheckConfig = new ProductCheckConfig();
                var serialValue = INIFile.GetValue(CommConfig.CheckProductStationSection, CommConfig.SerialNumberKey + i, serialPath);
                var serialVoltage = INIFile.GetValue(CommConfig.CheckProductStationSection, CommConfig.SerialVoltageKey + i, serialPath);
                productCheckConfig.ProductSerialPath = serialValue;
                productCheckConfig.TestSerial = GetProductTestSerial(serialValue);
                productCheckConfig.SupplyVoltage = serialVoltage;
                productCheckConfigList.Add(productCheckConfig);
                testSerialList.Add(productCheckConfig.TestSerial);
                if (productCheckConfig.TestSerial != "" & serialVoltage != "")
                {
                    this.lbx_productCheck_tip.Text += $"序列【{productCheckConfig.TestSerial}】是{serialVoltage}供电\r\n";
                }
            }
            if (this.lbx_productCheck_tip.Text == "")
                this.lbx_productCheck_sign.Visible = false;
            this.tb_productCheck_testSerial.DataSource = testSerialList;

            //读取机台映射根目录
            standMapRoot = ConfigurationManager.AppSettings["standMapRoot"].ToString();
            if (standMapRoot == "")
                standMapRoot = "Z";
            defaultRoot = ConfigurationManager.AppSettings["standConfigRoot"].ToString();
            if (defaultRoot == "")
                defaultRoot = "F";
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
            var burnInitPath = defaultRoot + StandCommon.TurnStationConfigPath + standCommon.ProductTypeNo + "\\";
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
            burnConfig.ProgrameActualPath = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.ProgrameActualPathKey, burnSavePath);
            burnConfig.ProgrameName = INIFile.GetValue(standCommon.ProductTypeNo,BurnConfig.ProgrameNameKey,burnSavePath);
            burnConfig.SerialNumber = INIFile.GetValue(standCommon.ProductTypeNo, BurnConfig.SerialNumberKey, burnSavePath);
            burnConfig.SerialNumber = GetProductTestSerial(burnConfig.SerialNumber);//更加路径返回序列名
        }

        private void ReadCommonStandConfig()
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

        private void ReadCommonConfig(string savePath)
        {
            standCommon.PCBABarCodeLength = INIFile.GetValue(standCommon.ProductTypeNo, StandCommon.PCBABarCodeLengthKey, savePath);
            standCommon.CaseBarCodeLength = INIFile.GetValue(standCommon.ProductTypeNo, StandCommon.CaseBarCodeLengthKey, savePath);
            standCommon.ShellBarCodeLength = INIFile.GetValue(standCommon.ProductTypeNo, StandCommon.ShellBarCodeLengthKey, savePath);
            standCommon.PackageCaseAmount = INIFile.GetValue(standCommon.ProductTypeNo, StandCommon.PackageCaseAmountKey, savePath);
        }

        private void ReadSensibilityStandConfig()
        {
            var senInitPath = defaultRoot + StandCommon.SensibilityStationConfigPath + standCommon.ProductTypeNo + "\\";
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
            sensibilityConfig.ProductSerial = GetProductTestSerial(sensibilityConfig.ProductSerial);
        }

        private void ReadShellStandConfig()
        {
            var shellInitPath = defaultRoot + StandCommon.ShellStationConfigPath + standCommon.ProductTypeNo + "\\";
            var shellFileName = StandCommon.ShellStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var shellSavePath = shellInitPath + shellFileName;
            shellConfig.LocalAddressConMes = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.LocalAddressConMesKey, shellSavePath);
            shellConfig.LocalAddressConPLC = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.LocalAddressConPLCKey, shellSavePath);
            shellConfig.PLCAddress = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.PLCAddressKey, shellSavePath);
            shellConfig.SmallScrewSetTime = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.SmallScrewSetTimeKey, shellSavePath);
            shellConfig.LargeScrewSetTime = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.LargeScrewSetTimeKey, shellSavePath);
            shellConfig.FrontCover = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.FrontCoverKey,shellSavePath);
            shellConfig.BackCover = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.BackCoverKey, shellSavePath);
            shellConfig.PCBScrew = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.PCBScrewKey, shellSavePath);
            shellConfig.ShellScrew = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.ShellScrewKey, shellSavePath);
            shellConfig.TopCover = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.TopCoverKey, shellSavePath);
            shellConfig.Shell = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.ShellKey, shellSavePath);
            shellConfig.SealRingWire = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.SealRingWireKey, shellSavePath);
            shellConfig.BubbleCotton = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.BubbleCottonKey, shellSavePath);
            shellConfig.TestSerialNumber = INIFile.GetValue(standCommon.ProductTypeNo, ShellConfig.TestSerialNumberKey, shellSavePath);
            shellConfig.TestSerialNumber = GetProductTestSerial(shellConfig.TestSerialNumber);
        }

        private void ReadAirtageStandConfig()
        {
            var airtageInitPath = defaultRoot + StandCommon.AirtageStationConfigPath + standCommon.ProductTypeNo + "\\";
            var airtageFileName = StandCommon.AirtageStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var airtageSavePath = airtageInitPath + airtageFileName;
            airtageConfig.LocalAddressConMes = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.LocalAddressConMesKey, airtageSavePath);
            airtageConfig.AirTester = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.AirTesterKey, airtageSavePath);
            airtageConfig.InflateAirTime = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.InflateAirTimeKey, airtageSavePath);
            airtageConfig.StableTime = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.StableTimeKey, airtageSavePath);
            airtageConfig.PressureUnit = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.PressureUnitKey, airtageSavePath);
            airtageConfig.SpreadUnit = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.SpreadUnitKey, airtageSavePath);
            airtageConfig.MaxInflate = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.MaxInflateKey, airtageSavePath);
            airtageConfig.MinInflate = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.MinInflateKey, airtageSavePath);
            airtageConfig.TestConditionValue = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.TestConditionValueKey, airtageSavePath);
            airtageConfig.ReferenceConditionValue = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.ReferenceConditionValueKey, airtageSavePath);
            airtageConfig.TestTime = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.TestTimeKey, airtageSavePath);
            airtageConfig.TestSerial = INIFile.GetValue(standCommon.ProductTypeNo, AirtageConfig.TestSerialKey, airtageSavePath);
            airtageConfig.TestSerial = GetProductTestSerial(airtageConfig.TestSerial);
        }

        private void ReadStentStandConfig()
        {
            var stentInitPath = defaultRoot + StandCommon.StentStationConfigPath + standCommon.ProductTypeNo + "\\";
            var stentFileName = StandCommon.StentStationIniName + standCommon.ProductTypeNo + "_config.ini";
            var stentSavePath = stentInitPath + stentFileName;
            stentConfig.LocalAddressConMes = INIFile.GetValue(standCommon.ProductTypeNo, StentConfig.LocalAddressConMesKey, stentSavePath);
            stentConfig.LeftStent = INIFile.GetValue(standCommon.ProductTypeNo, StentConfig.LeftStentKey, stentSavePath);
            stentConfig.RightStent = INIFile.GetValue(standCommon.ProductTypeNo, StentConfig.RightStentKey, stentSavePath);
            stentConfig.UnionStent = INIFile.GetValue(standCommon.ProductTypeNo, StentConfig.UnionStentKey, stentSavePath);
            stentConfig.Stent = INIFile.GetValue(standCommon.ProductTypeNo, StentConfig.StentKey, stentSavePath);
            stentConfig.StentScrew = INIFile.GetValue(standCommon.ProductTypeNo, StentConfig.StentSrcrewKey, stentSavePath);
            stentConfig.StentNut = INIFile.GetValue(standCommon.ProductTypeNo, StentConfig.StentNutKey, stentSavePath);
            stentConfig.TestSerial = INIFile.GetValue(standCommon.ProductTypeNo, StentConfig.TestSerialKey, stentSavePath);
            stentConfig.TestSerial = GetProductTestSerial(stentConfig.TestSerial);
        }

        private void ReadProductTestStandConfig()
        {
            var productInitPath = defaultRoot + StandCommon.ProductFinishStationConfigPath + standCommon.ProductTypeNo + "\\";
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
            productTestConfig.TestSerial = GetProductTestSerial(productTestConfig.TestSerial);
        }

        private void ReadProductCheckStandConfig()
        {
            var productCheckInitPath = defaultRoot + StandCommon.CheckProductStationConfigPath + standCommon.ProductTypeNo + "\\";
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
            productCheckConfig.TestSerial = GetProductTestSerial(productCheckConfig.TestSerial);
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
            var burnInitPath = defaultRoot + StandCommon.TurnStationConfigPath + standCommon.ProductTypeNo + "\\";
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
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.ProgrameActualPathKey, burnConfig.ProgrameActualPath, burnSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, BurnConfig.ProgrameMapPathKey, burnConfig.ProgrameMapPath, burnSavePath);
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
            bool IsBurn = true, IsSen = true, IsAir = true, IsStent = true,IsShell =  true, IsPtest = true, IsPcheck = true;
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
                return;
            }
            if (!IsBurn && !IsSen && !IsAir && !IsShell && !IsStent && !IsPtest && !IsPcheck)
                return;
            MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SaveSensibilityStandConfig()
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
            INIFile.SetValue(standCommon.ProductTypeNo, SensibilityConfig.ProductSerialKey, sensibilityConfig.ProductSerial, senSavePath);
            MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            IsSavePrivateConfig = true;
        }

        private void SaveShellStandConfig()
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
            INIFile.SetValue(standCommon.ProductTypeNo, ShellConfig.TestSerialNumberKey, shellConfig.TestSerialNumber, shellSavePath);
            MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            IsSavePrivateConfig = true;
        }

        private void SaveAirtageStandConfig()
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
            INIFile.SetValue(standCommon.ProductTypeNo, AirtageConfig.TestConditionValueKey, airtageConfig.TestConditionValue, airtageSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, AirtageConfig.ReferenceConditionValueKey, airtageConfig.ReferenceConditionValue, airtageSavePath);
            INIFile.SetValue(standCommon.ProductTypeNo, AirtageConfig.TestSerialKey, airtageConfig.TestSerial, airtageSavePath);
            MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            IsSavePrivateConfig = true;
        }

        private void SaveStentStandConfig()
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
            INIFile.SetValue(standCommon.ProductTypeNo, StentConfig.TestSerialKey, stentConfig.TestSerial, stentSavePath);
            MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            IsSavePrivateConfig = true;
        }

        private void SaveProductTestConfig()
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
            INIFile.SetValue(standCommon.ProductTypeNo, ProductTestConfig.TestSerialKey, productTestConfig.TestSerial, productSavePath);
            MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            IsSavePrivateConfig = true;
        }

        private void SaveProductCheckConfig()
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
            burnConfig.ProgrameActualPath = this.tb_burn_programePath.Text;
            burnConfig.ProgrameMapPath = LocalPathConvertToMapPath(this.tb_burn_programePath.Text);
            burnConfig.ProgrameName = this.tb_burn_programeName.Text;
            var burn = GetBurnSerialConfig(this.cb_burn_serialNumber.Text);
            if (burn == null)
                return;
            burnConfig.SerialNumber = burn.ProductSerialPath;
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
            sensibilityConfig.ReceiveCanID = this.tb_sen_receiveCanID.Text;
            sensibilityConfig.RfCanID = this.tb_sen_rfCanID.Text;
            sensibilityConfig.CyclyCanID = this.tb_sen_cycleCanID.Text;
            var sensibility = GetSensibilityConfig(this.cb_sen_serialNumber.Text);
            if (sensibility == null)
                return;
            sensibilityConfig.ProductSerial = sensibility.ProductSerialPath;
        }

        private void CheckShellConfigParams()
        {
            shellConfig.LocalAddressConMes = this.tb_shell_localConMes.Text;
            shellConfig.LocalAddressConPLC = this.tb_shell_localIPConPLC.Text;
            shellConfig.PLCAddress = this.tb_shell_plcAddress.Text;
            shellConfig.SmallScrewSetTime = this.tb_shell_smallScrewSetTime.Text;
            shellConfig.LargeScrewSetTime = this.tb_shell_largeScrewSetTime.Text;
            shellConfig.FrontCover = this.tb_shell_frontCover.Text;
            shellConfig.BackCover = this.tb_shell_backCover.Text;
            shellConfig.PCBScrew = this.tb_shell_pcbScrew.Text;
            shellConfig.ShellScrew = this.tb_shell_shellScrew.Text;
            shellConfig.TopCover = this.tb_shell_topCover.Text;
            shellConfig.Shell = this.tb_shell_shell.Text;
            shellConfig.SealRingWire = this.tb_shell_sealRingWire.Text;
            shellConfig.BubbleCotton = this.tb_shell_bubbleCotton.Text;
            var shell = GetShellConfig(this.cb_shell_testSerial.Text);
            if (shell == null)
                return;
            shellConfig.TestSerialNumber = shell.ProductSerialPath;
        }

        private void CheckAirtageConfigParams()
        {
            airtageConfig.LocalAddressConMes = this.tb_airtage_localIPConMes.Text;
            airtageConfig.AirTester = this.tb_airtage_tester.Text;
            airtageConfig.InflateAirTime = this.tb_airtage_inflateTime.Text;
            airtageConfig.StableTime = this.tb_airtage_stableTime.Text;
            airtageConfig.TestTime = this.tb_airtage_testTime.Text;
            AirtageSpreadUnitEnum spreadUnitEnum;
            Enum.TryParse(this.tb_airtage_spread.Text.Replace("/", "_"),out spreadUnitEnum);
            airtageConfig.SpreadUnit = (int)spreadUnitEnum + "";
            AirtagePressureUnitEnum pressureUnitEnum;
            Enum.TryParse(this.tb_airtage_pressureUnit.Text,out pressureUnitEnum);
            airtageConfig.PressureUnit = (int)pressureUnitEnum + "";
            airtageConfig.MaxInflate = this.tb_airtage_maxInflate.Text;
            airtageConfig.MinInflate = this.tb_airtage_minFlate.Text;
            airtageConfig.TestConditionValue = this.tb_airtage_testConditionValue.Text;
            airtageConfig.ReferenceConditionValue = this.tb_airtage_referenceConditionValue.Text;
            var airtage = GetAirtageConfig(this.tb_airtage_testSerial.Text);
            if (airtage == null)
                return;
            airtageConfig.TestSerial = airtage.ProductSerialPath;
        }

        private void CheckStentConfigParams()
        {
            stentConfig.LocalAddressConMes = this.tb_stent_localIPConMes.Text;
            stentConfig.LeftStent = this.tb_stent_leftStent.Text;
            stentConfig.RightStent = this.tb_stent_rightStent.Text;
            stentConfig.UnionStent = this.tb_stent_unionStent.Text;
            stentConfig.Stent = this.tb_stent_stent.Text;
            stentConfig.StentScrew = this.tb_stent_stentScrew.Text;
            stentConfig.StentNut = this.tb_stent_stentNut.Text;
            var stent = GetStentConfig(this.cb_stent_testSerial.Text);
            if (stent == null)
                return;
            stentConfig.TestSerial = stent.ProductSerialPath;
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
            var productTest = GetProductTestConfig(this.cb_product_testSerial.Text);
            if (productTest == null)
                return;
            productTestConfig.TestSerial = productTest.ProductSerialPath;
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
            var productCheck = GetProductCheckConfig(this.tb_productCheck_testSerial.Text);
            if (productCheck == null)
                return;
            productCheckConfig.TestSerial = productCheck.ProductSerialPath;
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
            this.tb_burn_programePath.Text = burnConfig.ProgrameActualPath;
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
            this.tb_shell_frontCover.Text = shellConfig.FrontCover;
            this.tb_shell_backCover.Text = shellConfig.BackCover;
            this.tb_shell_pcbScrew.Text = shellConfig.PCBScrew;
            this.tb_shell_shellScrew.Text = shellConfig.ShellScrew;
            this.tb_shell_topCover.Text = shellConfig.TopCover;
            this.tb_shell_shell.Text = shellConfig.Shell;
            this.tb_shell_sealRingWire.Text = shellConfig.SealRingWire;
            this.tb_shell_bubbleCotton.Text = shellConfig.BubbleCotton;
        }

        private void RefreshUIAirtageStation()
        {
            this.tb_airtage_localIPConMes.Text = airtageConfig.LocalAddressConMes;
            this.tb_airtage_tester.Text = airtageConfig.AirTester;
            this.tb_airtage_inflateTime.Text = airtageConfig.InflateAirTime;
            this.tb_airtage_stableTime.Text = airtageConfig.StableTime;
            this.tb_airtage_testTime.Text = airtageConfig.TestTime;
            AirtagePressureUnitEnum pressureUnitEnum;
            Enum.TryParse(airtageConfig.PressureUnit,out pressureUnitEnum);
            this.tb_airtage_pressureUnit.Text = pressureUnitEnum.ToString();
            AirtageSpreadUnitEnum spreadUnitEnum;
            Enum.TryParse(airtageConfig.SpreadUnit,out spreadUnitEnum);
            this.tb_airtage_spread.Text = spreadUnitEnum.ToString().Replace("_","/");
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
            this.tb_stent_leftStent.Text = stentConfig.LeftStent;
            this.tb_stent_rightStent.Text = stentConfig.RightStent;
            this.tb_stent_unionStent.Text = stentConfig.UnionStent;
            this.tb_stent_stent.Text = stentConfig.Stent;
            this.tb_stent_stentScrew.Text = stentConfig.StentScrew;
            this.tb_stent_stentNut.Text = stentConfig.StentNut;
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
