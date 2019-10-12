namespace MesManager.UI
{
    partial class TestLogDetail
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
            Telerik.WinControls.UI.TableViewDefinition tableViewDefinition1 = new Telerik.WinControls.UI.TableViewDefinition();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestLogDetail));
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pickerStartTime = new Telerik.WinControls.UI.RadDateTimePicker();
            this.pickerEndTime = new Telerik.WinControls.UI.RadDateTimePicker();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.tb_queryFiler = new System.Windows.Forms.TextBox();
            this.btn_export = new Telerik.WinControls.UI.RadButton();
            this.btn_search = new Telerik.WinControls.UI.RadButton();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.rbtn_today = new System.Windows.Forms.RadioButton();
            this.rbtn_oneMonth = new System.Windows.Forms.RadioButton();
            this.rbtn_threeMonth = new System.Windows.Forms.RadioButton();
            this.rbtn_oneYear = new System.Windows.Forms.RadioButton();
            this.rbtn_custom = new System.Windows.Forms.RadioButton();
            this.radGridView1 = new Telerik.WinControls.UI.RadGridView();
            this.breezeTheme1 = new Telerik.WinControls.Themes.BreezeTheme();
            ((System.ComponentModel.ISupportInitialize)(this.pickerStartTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pickerEndTime)).BeginInit();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btn_export)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_search)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(786, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "结束日期";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(527, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "开始日期";
            // 
            // pickerStartTime
            // 
            this.pickerStartTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.pickerStartTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pickerStartTime.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pickerStartTime.ForeColor = System.Drawing.Color.Black;
            this.pickerStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.pickerStartTime.Location = new System.Drawing.Point(611, 3);
            this.pickerStartTime.Name = "pickerStartTime";
            this.pickerStartTime.Size = new System.Drawing.Size(169, 23);
            this.pickerStartTime.TabIndex = 4;
            this.pickerStartTime.TabStop = false;
            this.pickerStartTime.Text = "2019-08-26 17:09:35";
            this.pickerStartTime.Value = new System.DateTime(2019, 8, 26, 17, 9, 35, 395);
            // 
            // pickerEndTime
            // 
            this.pickerEndTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.pickerEndTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pickerEndTime.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pickerEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.pickerEndTime.Location = new System.Drawing.Point(864, 3);
            this.pickerEndTime.Name = "pickerEndTime";
            this.pickerEndTime.Size = new System.Drawing.Size(174, 23);
            this.pickerEndTime.TabIndex = 5;
            this.pickerEndTime.TabStop = false;
            this.pickerEndTime.Text = "2019-08-26 17:09:41";
            this.pickerEndTime.Value = new System.DateTime(2019, 8, 26, 17, 9, 41, 544);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Controls.Add(this.tableLayoutPanel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1272, 76);
            this.panel1.TabIndex = 8;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51.64179F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.35821F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 837F));
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tb_queryFiler, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btn_export, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.btn_search, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 36);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1272, 34);
            this.tableLayoutPanel1.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(166, 34);
            this.label3.TabIndex = 7;
            this.label3.Text = "PCBA/成品SN/工位名称";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tb_queryFiler
            // 
            this.tb_queryFiler.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb_queryFiler.Location = new System.Drawing.Point(175, 3);
            this.tb_queryFiler.Name = "tb_queryFiler";
            this.tb_queryFiler.Size = new System.Drawing.Size(156, 21);
            this.tb_queryFiler.TabIndex = 1;
            // 
            // btn_export
            // 
            this.btn_export.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_export.ForeColor = System.Drawing.Color.White;
            this.btn_export.Image = global::MesManager.Properties.Resources.Export_16x16;
            this.btn_export.Location = new System.Drawing.Point(437, 3);
            this.btn_export.Name = "btn_export";
            this.btn_export.Size = new System.Drawing.Size(72, 24);
            this.btn_export.TabIndex = 9;
            this.btn_export.Text = "导出";
            this.btn_export.ThemeName = "Breeze";
            this.btn_export.Click += new System.EventHandler(this.Btn_export_Click);
            // 
            // btn_search
            // 
            this.btn_search.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_search.ForeColor = System.Drawing.Color.White;
            this.btn_search.Image = global::MesManager.Properties.Resources.Search_16x16;
            this.btn_search.Location = new System.Drawing.Point(337, 3);
            this.btn_search.Name = "btn_search";
            this.btn_search.Size = new System.Drawing.Size(74, 24);
            this.btn_search.TabIndex = 8;
            this.btn_search.Text = "查询";
            this.btn_search.ThemeName = "Breeze";
            this.btn_search.Click += new System.EventHandler(this.Btn_search_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 10;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34.63415F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65.36585F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 116F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 101F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 123F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 84F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 175F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 78F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 230F));
            this.tableLayoutPanel2.Controls.Add(this.rbtn_today, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.rbtn_oneMonth, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.rbtn_threeMonth, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.rbtn_oneYear, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.rbtn_custom, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.label1, 5, 0);
            this.tableLayoutPanel2.Controls.Add(this.pickerStartTime, 6, 0);
            this.tableLayoutPanel2.Controls.Add(this.label2, 7, 0);
            this.tableLayoutPanel2.Controls.Add(this.pickerEndTime, 8, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1272, 36);
            this.tableLayoutPanel2.TabIndex = 16;
            // 
            // rbtn_today
            // 
            this.rbtn_today.AutoSize = true;
            this.rbtn_today.ForeColor = System.Drawing.Color.White;
            this.rbtn_today.Location = new System.Drawing.Point(3, 3);
            this.rbtn_today.Name = "rbtn_today";
            this.rbtn_today.Size = new System.Drawing.Size(58, 21);
            this.rbtn_today.TabIndex = 14;
            this.rbtn_today.TabStop = true;
            this.rbtn_today.Text = "当天";
            this.rbtn_today.UseVisualStyleBackColor = true;
            // 
            // rbtn_oneMonth
            // 
            this.rbtn_oneMonth.AutoSize = true;
            this.rbtn_oneMonth.ForeColor = System.Drawing.Color.White;
            this.rbtn_oneMonth.Location = new System.Drawing.Point(67, 3);
            this.rbtn_oneMonth.Name = "rbtn_oneMonth";
            this.rbtn_oneMonth.Size = new System.Drawing.Size(106, 21);
            this.rbtn_oneMonth.TabIndex = 15;
            this.rbtn_oneMonth.TabStop = true;
            this.rbtn_oneMonth.Text = "最近一个月";
            this.rbtn_oneMonth.UseVisualStyleBackColor = true;
            // 
            // rbtn_threeMonth
            // 
            this.rbtn_threeMonth.AutoSize = true;
            this.rbtn_threeMonth.ForeColor = System.Drawing.Color.White;
            this.rbtn_threeMonth.Location = new System.Drawing.Point(187, 3);
            this.rbtn_threeMonth.Name = "rbtn_threeMonth";
            this.rbtn_threeMonth.Size = new System.Drawing.Size(106, 21);
            this.rbtn_threeMonth.TabIndex = 16;
            this.rbtn_threeMonth.TabStop = true;
            this.rbtn_threeMonth.Text = "最近三个月";
            this.rbtn_threeMonth.UseVisualStyleBackColor = true;
            // 
            // rbtn_oneYear
            // 
            this.rbtn_oneYear.AutoSize = true;
            this.rbtn_oneYear.ForeColor = System.Drawing.Color.White;
            this.rbtn_oneYear.Location = new System.Drawing.Point(303, 3);
            this.rbtn_oneYear.Name = "rbtn_oneYear";
            this.rbtn_oneYear.Size = new System.Drawing.Size(90, 21);
            this.rbtn_oneYear.TabIndex = 18;
            this.rbtn_oneYear.TabStop = true;
            this.rbtn_oneYear.Text = "最近一年";
            this.rbtn_oneYear.UseVisualStyleBackColor = true;
            // 
            // rbtn_custom
            // 
            this.rbtn_custom.AutoSize = true;
            this.rbtn_custom.ForeColor = System.Drawing.Color.White;
            this.rbtn_custom.Location = new System.Drawing.Point(404, 3);
            this.rbtn_custom.Name = "rbtn_custom";
            this.rbtn_custom.Size = new System.Drawing.Size(106, 21);
            this.rbtn_custom.TabIndex = 13;
            this.rbtn_custom.TabStop = true;
            this.rbtn_custom.Text = "自定义日期";
            this.rbtn_custom.UseVisualStyleBackColor = true;
            // 
            // radGridView1
            // 
            this.radGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radGridView1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radGridView1.Location = new System.Drawing.Point(0, 76);
            // 
            // 
            // 
            this.radGridView1.MasterTemplate.ViewDefinition = tableViewDefinition1;
            this.radGridView1.Name = "radGridView1";
            this.radGridView1.Size = new System.Drawing.Size(1272, 508);
            this.radGridView1.TabIndex = 9;
            this.radGridView1.ThemeName = "Breeze";
            // 
            // TestLogDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(1272, 584);
            this.Controls.Add(this.radGridView1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TestLogDetail";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "LOG详细记录";
            this.Load += new System.EventHandler(this.TestLogDetail_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pickerStartTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pickerEndTime)).EndInit();
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btn_export)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_search)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private Telerik.WinControls.UI.RadDateTimePicker pickerStartTime;
        private Telerik.WinControls.UI.RadDateTimePicker pickerEndTime;
        private System.Windows.Forms.Panel panel1;
        private Telerik.WinControls.UI.RadGridView radGridView1;
        private Telerik.WinControls.UI.RadButton btn_search;
        private Telerik.WinControls.UI.RadButton btn_export;
        private Telerik.WinControls.Themes.BreezeTheme breezeTheme1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.RadioButton rbtn_threeMonth;
        private System.Windows.Forms.RadioButton rbtn_oneMonth;
        private System.Windows.Forms.RadioButton rbtn_today;
        private System.Windows.Forms.RadioButton rbtn_oneYear;
        private System.Windows.Forms.RadioButton rbtn_custom;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tb_queryFiler;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
