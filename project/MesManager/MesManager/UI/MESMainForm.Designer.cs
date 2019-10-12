namespace MesManager.UI
{
    partial class MESMainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MESMainForm));
            this.radPanorama1 = new Telerik.WinControls.UI.RadPanorama();
            this.ledControl1 = new LEDLib.LEDControl();
            this.btn_user_manger = new Telerik.WinControls.UI.RadButton();
            this.btn_user_login = new Telerik.WinControls.UI.RadButton();
            this.toolsGroup = new Telerik.WinControls.UI.TileGroupElement();
            this.mainBasicInfo = new Telerik.WinControls.UI.RadTileElement();
            this.mainMaterialManager = new Telerik.WinControls.UI.RadTileElement();
            this.mainProcess = new Telerik.WinControls.UI.RadTileElement();
            this.mainQuanlityAnomaly = new Telerik.WinControls.UI.RadTileElement();
            this.mainReportData = new Telerik.WinControls.UI.RadTileElement();
            this.mainTestStandData = new Telerik.WinControls.UI.RadTileElement();
            this.breezeTheme1 = new Telerik.WinControls.Themes.BreezeTheme();
            ((System.ComponentModel.ISupportInitialize)(this.radPanorama1)).BeginInit();
            this.radPanorama1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btn_user_manger)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_user_login)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radPanorama1
            // 
            this.radPanorama1.AutoArrangeNewTiles = false;
            this.radPanorama1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(35)))), ((int)(((byte)(49)))));
            this.radPanorama1.Controls.Add(this.ledControl1);
            this.radPanorama1.Controls.Add(this.btn_user_manger);
            this.radPanorama1.Controls.Add(this.btn_user_login);
            this.radPanorama1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radPanorama1.Groups.AddRange(new Telerik.WinControls.RadItem[] {
            this.toolsGroup});
            this.radPanorama1.Location = new System.Drawing.Point(0, 0);
            this.radPanorama1.Name = "radPanorama1";
            this.radPanorama1.PanelImageSize = new System.Drawing.Size(1024, 768);
            this.radPanorama1.RowsCount = 2;
            this.radPanorama1.ShowGroups = true;
            this.radPanorama1.Size = new System.Drawing.Size(1288, 1054);
            this.radPanorama1.TabIndex = 1;
            this.radPanorama1.ThemeName = "ControlDefault";
            ((Telerik.WinControls.UI.RadPanoramaElement)(this.radPanorama1.GetChildAt(0))).BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(23)))), ((int)(((byte)(117)))));
            // 
            // ledControl1
            // 
            this.ledControl1.LEDCenterColor = System.Drawing.Color.Lime;
            this.ledControl1.LEDCircleColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.ledControl1.LEDClickEnable = true;
            this.ledControl1.LEDSurroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.ledControl1.LEDSwitch = true;
            this.ledControl1.Location = new System.Drawing.Point(12, 54);
            this.ledControl1.Name = "ledControl1";
            this.ledControl1.Size = new System.Drawing.Size(43, 37);
            this.ledControl1.TabIndex = 3;
            // 
            // btn_user_manger
            // 
            this.btn_user_manger.BackColor = System.Drawing.Color.Navy;
            this.btn_user_manger.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_user_manger.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btn_user_manger.Location = new System.Drawing.Point(12, 712);
            this.btn_user_manger.Name = "btn_user_manger";
            this.btn_user_manger.Size = new System.Drawing.Size(120, 48);
            this.btn_user_manger.TabIndex = 2;
            this.btn_user_manger.Text = "用户管理";
            // 
            // btn_user_login
            // 
            this.btn_user_login.BackColor = System.Drawing.Color.Navy;
            this.btn_user_login.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_user_login.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btn_user_login.Location = new System.Drawing.Point(12, 795);
            this.btn_user_login.Name = "btn_user_login";
            this.btn_user_login.Size = new System.Drawing.Size(120, 48);
            this.btn_user_login.TabIndex = 1;
            this.btn_user_login.Text = "用户登录";
            // 
            // toolsGroup
            // 
            this.toolsGroup.Alignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolsGroup.BorderBottomWidth = 1F;
            this.toolsGroup.CellSize = new System.Drawing.Size(155, 155);
            this.toolsGroup.DisabledTextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.toolsGroup.Font = new System.Drawing.Font("Segoe UI Light", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.toolsGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(172)))), ((int)(((byte)(255)))));
            this.toolsGroup.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.mainBasicInfo,
            this.mainMaterialManager,
            this.mainProcess,
            this.mainQuanlityAnomaly,
            this.mainReportData,
            this.mainTestStandData});
            this.toolsGroup.Margin = new System.Windows.Forms.Padding(400, 200, 200, 0);
            this.toolsGroup.Name = "toolsGroup";
            this.toolsGroup.RowsCount = 2;
            this.toolsGroup.Text = "主功能";
            this.toolsGroup.TextOrientation = System.Windows.Forms.Orientation.Horizontal;
            this.toolsGroup.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.toolsGroup.UseCompatibleTextRendering = false;
            // 
            // mainBasicInfo
            // 
            this.mainBasicInfo.AccessibleDescription = "mainBasicInfo";
            this.mainBasicInfo.AccessibleName = "mainBasicInfo";
            this.mainBasicInfo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(126)))), ((int)(((byte)(216)))));
            this.mainBasicInfo.CellPadding = new System.Windows.Forms.Padding(5);
            this.mainBasicInfo.DisabledTextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.mainBasicInfo.DrawBorder = true;
            this.mainBasicInfo.ImageAlignment = System.Drawing.ContentAlignment.BottomLeft;
            this.mainBasicInfo.ImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mainBasicInfo.Name = "mainBasicInfo";
            this.mainBasicInfo.Padding = new System.Windows.Forms.Padding(15, 15, 0, 10);
            this.mainBasicInfo.Text = "<html>产品管理 <br>";
            this.mainBasicInfo.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.mainBasicInfo.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.mainBasicInfo.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.mainBasicInfo.UseCompatibleTextRendering = false;
            // 
            // mainMaterialManager
            // 
            this.mainMaterialManager.AccessibleDescription = "mainMaterialManager";
            this.mainMaterialManager.AccessibleName = "mainMaterialManager";
            this.mainMaterialManager.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(33)))));
            this.mainMaterialManager.CellPadding = new System.Windows.Forms.Padding(5);
            this.mainMaterialManager.Column = 2;
            this.mainMaterialManager.DisabledTextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.mainMaterialManager.DrawBorder = true;
            this.mainMaterialManager.ImageAlignment = System.Drawing.ContentAlignment.BottomLeft;
            this.mainMaterialManager.ImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mainMaterialManager.Name = "mainMaterialManager";
            this.mainMaterialManager.Padding = new System.Windows.Forms.Padding(15, 15, 0, 10);
            this.mainMaterialManager.Text = "<html>物料管理 <br>";
            this.mainMaterialManager.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.mainMaterialManager.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.mainMaterialManager.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.mainMaterialManager.UseCompatibleTextRendering = false;
            // 
            // mainProcess
            // 
            this.mainProcess.AccessibleDescription = "mainProductPackage";
            this.mainProcess.AccessibleName = "mainProductPackage";
            this.mainProcess.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(33)))));
            this.mainProcess.CellPadding = new System.Windows.Forms.Padding(5);
            this.mainProcess.Column = 1;
            this.mainProcess.DisabledTextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.mainProcess.DrawBorder = true;
            this.mainProcess.ImageAlignment = System.Drawing.ContentAlignment.BottomLeft;
            this.mainProcess.ImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mainProcess.Name = "mainProcess";
            this.mainProcess.Padding = new System.Windows.Forms.Padding(15, 15, 0, 10);
            this.mainProcess.Text = "<html>工艺流程 <br>";
            this.mainProcess.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.mainProcess.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.mainProcess.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.mainProcess.UseCompatibleTextRendering = false;
            // 
            // mainQuanlityAnomaly
            // 
            this.mainQuanlityAnomaly.AccessibleDescription = "mainQuanlityAnomaly";
            this.mainQuanlityAnomaly.AccessibleName = "mainQuanlityAnomaly";
            this.mainQuanlityAnomaly.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(33)))));
            this.mainQuanlityAnomaly.CellPadding = new System.Windows.Forms.Padding(5);
            this.mainQuanlityAnomaly.Column = 1;
            this.mainQuanlityAnomaly.DisabledTextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.mainQuanlityAnomaly.DrawBorder = true;
            this.mainQuanlityAnomaly.ImageAlignment = System.Drawing.ContentAlignment.BottomLeft;
            this.mainQuanlityAnomaly.ImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mainQuanlityAnomaly.Name = "mainQuanlityAnomaly";
            this.mainQuanlityAnomaly.Padding = new System.Windows.Forms.Padding(15, 15, 0, 10);
            this.mainQuanlityAnomaly.Row = 1;
            this.mainQuanlityAnomaly.Text = "<html>品质管理 <br>";
            this.mainQuanlityAnomaly.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.mainQuanlityAnomaly.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.mainQuanlityAnomaly.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.mainQuanlityAnomaly.UseCompatibleTextRendering = false;
            // 
            // mainReportData
            // 
            this.mainReportData.AccessibleDescription = "mainReportData";
            this.mainReportData.AccessibleName = "mainReportData";
            this.mainReportData.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(33)))));
            this.mainReportData.CellPadding = new System.Windows.Forms.Padding(5);
            this.mainReportData.DisabledTextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.mainReportData.DrawBorder = true;
            this.mainReportData.ImageAlignment = System.Drawing.ContentAlignment.BottomLeft;
            this.mainReportData.ImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mainReportData.Name = "mainReportData";
            this.mainReportData.Padding = new System.Windows.Forms.Padding(15, 15, 0, 10);
            this.mainReportData.Row = 1;
            this.mainReportData.Text = "<html>追溯管理 <br>";
            this.mainReportData.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.mainReportData.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.mainReportData.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.mainReportData.UseCompatibleTextRendering = false;
            // 
            // mainTestStandData
            // 
            this.mainTestStandData.AccessibleDescription = "mainTestStandData";
            this.mainTestStandData.AccessibleName = "mainTestStandData";
            this.mainTestStandData.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(126)))), ((int)(((byte)(216)))));
            this.mainTestStandData.CellPadding = new System.Windows.Forms.Padding(5);
            this.mainTestStandData.Column = 2;
            this.mainTestStandData.DisabledTextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.mainTestStandData.DrawBorder = true;
            this.mainTestStandData.ImageAlignment = System.Drawing.ContentAlignment.BottomLeft;
            this.mainTestStandData.ImageLayout = System.Windows.Forms.ImageLayout.None;
            this.mainTestStandData.Name = "mainTestStandData";
            this.mainTestStandData.Padding = new System.Windows.Forms.Padding(15, 15, 0, 10);
            this.mainTestStandData.Row = 1;
            this.mainTestStandData.Text = "<html>测试台数据 <br>";
            this.mainTestStandData.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.mainTestStandData.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.mainTestStandData.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            this.mainTestStandData.UseCompatibleTextRendering = false;
            // 
            // MESMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1288, 1054);
            this.Controls.Add(this.radPanorama1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MESMainForm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.MESMainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.radPanorama1)).EndInit();
            this.radPanorama1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btn_user_manger)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_user_login)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadPanorama radPanorama1;
        private Telerik.WinControls.UI.TileGroupElement toolsGroup;
        private Telerik.WinControls.UI.RadTileElement mainBasicInfo;//基础信息
        private Telerik.WinControls.UI.RadTileElement mainMaterialManager;//物料管理
        private Telerik.WinControls.UI.RadTileElement mainProcess;//成品装箱
        private Telerik.WinControls.UI.RadTileElement mainReportData;//报表中心
        private Telerik.WinControls.UI.RadTileElement mainQuanlityAnomaly;//品质异常管理
        private Telerik.WinControls.UI.RadTileElement mainTestStandData;//测试台数据
        private Telerik.WinControls.UI.RadButton btn_user_login;
        private Telerik.WinControls.UI.RadButton btn_user_manger;
        private LEDLib.LEDControl ledControl1;
        private Telerik.WinControls.Themes.BreezeTheme breezeTheme1;
    }
}
