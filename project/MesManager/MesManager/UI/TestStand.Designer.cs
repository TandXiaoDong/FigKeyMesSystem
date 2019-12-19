namespace MesManager.UI
{
    partial class TestStand
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestStand));
            Telerik.WinControls.UI.TableViewDefinition tableViewDefinition1 = new Telerik.WinControls.UI.TableViewDefinition();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tool_queryCondition = new System.Windows.Forms.ToolStripComboBox();
            this.tool_query = new System.Windows.Forms.ToolStripButton();
            this.tool_programv = new System.Windows.Forms.ToolStripButton();
            this.tool_specCfg = new System.Windows.Forms.ToolStripButton();
            this.tool_logData = new System.Windows.Forms.ToolStripButton();
            this.tool_clearDB = new System.Windows.Forms.ToolStripButton();
            this.tool_exportCondition = new System.Windows.Forms.ToolStripComboBox();
            this.tool_export = new System.Windows.Forms.ToolStripButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.rbtn_threeMonth = new System.Windows.Forms.RadioButton();
            this.rbtn_oneMonth = new System.Windows.Forms.RadioButton();
            this.rbtn_today = new System.Windows.Forms.RadioButton();
            this.rbtn_oneYear = new System.Windows.Forms.RadioButton();
            this.rbtn_custom = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pickerEndTime = new Telerik.WinControls.UI.RadDateTimePicker();
            this.pickerStartTime = new Telerik.WinControls.UI.RadDateTimePicker();
            this.bindingNavigator1 = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorAddNewItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorDeleteItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.radGridView1 = new Telerik.WinControls.UI.RadGridView();
            this.toolStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pickerEndTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pickerStartTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigator1)).BeginInit();
            this.bindingNavigator1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.CornflowerBlue;
            this.toolStrip1.Font = new System.Drawing.Font("Microsoft YaHei UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.tool_queryCondition,
            this.tool_query,
            this.tool_programv,
            this.tool_specCfg,
            this.tool_logData,
            this.tool_clearDB,
            this.tool_exportCondition,
            this.tool_export});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1276, 27);
            this.toolStrip1.TabIndex = 13;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.ForeColor = System.Drawing.Color.White;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(66, 24);
            this.toolStripLabel1.Text = "SN/工站";
            // 
            // tool_queryCondition
            // 
            this.tool_queryCondition.ForeColor = System.Drawing.Color.SteelBlue;
            this.tool_queryCondition.Name = "tool_queryCondition";
            this.tool_queryCondition.Size = new System.Drawing.Size(121, 27);
            // 
            // tool_query
            // 
            this.tool_query.ForeColor = System.Drawing.Color.White;
            this.tool_query.Image = global::MesManager.Properties.Resources.Search_16x16;
            this.tool_query.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_query.Name = "tool_query";
            this.tool_query.Size = new System.Drawing.Size(63, 24);
            this.tool_query.Text = "查询";
            // 
            // tool_programv
            // 
            this.tool_programv.ForeColor = System.Drawing.Color.White;
            this.tool_programv.Image = global::MesManager.Properties.Resources.Version_16x16;
            this.tool_programv.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_programv.Name = "tool_programv";
            this.tool_programv.Size = new System.Drawing.Size(93, 24);
            this.tool_programv.Text = "程序版本";
            // 
            // tool_specCfg
            // 
            this.tool_specCfg.ForeColor = System.Drawing.Color.White;
            this.tool_specCfg.Image = global::MesManager.Properties.Resources.configure_cluster;
            this.tool_specCfg.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_specCfg.Name = "tool_specCfg";
            this.tool_specCfg.Size = new System.Drawing.Size(99, 24);
            this.tool_specCfg.Text = "SPEC配置";
            // 
            // tool_logData
            // 
            this.tool_logData.ForeColor = System.Drawing.Color.White;
            this.tool_logData.Image = ((System.Drawing.Image)(resources.GetObject("tool_logData.Image")));
            this.tool_logData.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_logData.Name = "tool_logData";
            this.tool_logData.Size = new System.Drawing.Size(94, 24);
            this.tool_logData.Text = "LOG记录";
            // 
            // tool_clearDB
            // 
            this.tool_clearDB.ForeColor = System.Drawing.Color.White;
            this.tool_clearDB.Image = global::MesManager.Properties.Resources.DeleteDataSource_16x16;
            this.tool_clearDB.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_clearDB.Name = "tool_clearDB";
            this.tool_clearDB.Size = new System.Drawing.Size(63, 24);
            this.tool_clearDB.Text = "清除";
            // 
            // tool_exportCondition
            // 
            this.tool_exportCondition.Name = "tool_exportCondition";
            this.tool_exportCondition.Size = new System.Drawing.Size(121, 27);
            // 
            // tool_export
            // 
            this.tool_export.ForeColor = System.Drawing.Color.White;
            this.tool_export.Image = global::MesManager.Properties.Resources.Export_16x16;
            this.tool_export.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool_export.Name = "tool_export";
            this.tool_export.Size = new System.Drawing.Size(63, 24);
            this.tool_export.Text = "导出";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tableLayoutPanel1);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.pickerEndTime);
            this.panel2.Controls.Add(this.pickerStartTime);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 27);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1276, 55);
            this.panel2.TabIndex = 14;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 39.79058F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60.20942F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 107F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 96F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 194F));
            this.tableLayoutPanel1.Controls.Add(this.rbtn_threeMonth, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.rbtn_oneMonth, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.rbtn_today, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.rbtn_oneYear, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.rbtn_custom, 4, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(583, 55);
            this.tableLayoutPanel1.TabIndex = 16;
            // 
            // rbtn_threeMonth
            // 
            this.rbtn_threeMonth.AutoSize = true;
            this.rbtn_threeMonth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbtn_threeMonth.ForeColor = System.Drawing.Color.White;
            this.rbtn_threeMonth.Location = new System.Drawing.Point(188, 3);
            this.rbtn_threeMonth.Name = "rbtn_threeMonth";
            this.rbtn_threeMonth.Size = new System.Drawing.Size(101, 49);
            this.rbtn_threeMonth.TabIndex = 16;
            this.rbtn_threeMonth.TabStop = true;
            this.rbtn_threeMonth.Text = "最近三个月";
            this.rbtn_threeMonth.UseVisualStyleBackColor = true;
            // 
            // rbtn_oneMonth
            // 
            this.rbtn_oneMonth.AutoSize = true;
            this.rbtn_oneMonth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbtn_oneMonth.ForeColor = System.Drawing.Color.White;
            this.rbtn_oneMonth.Location = new System.Drawing.Point(77, 3);
            this.rbtn_oneMonth.Name = "rbtn_oneMonth";
            this.rbtn_oneMonth.Size = new System.Drawing.Size(105, 49);
            this.rbtn_oneMonth.TabIndex = 15;
            this.rbtn_oneMonth.TabStop = true;
            this.rbtn_oneMonth.Text = "最近一个月";
            this.rbtn_oneMonth.UseVisualStyleBackColor = true;
            // 
            // rbtn_today
            // 
            this.rbtn_today.AutoSize = true;
            this.rbtn_today.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbtn_today.ForeColor = System.Drawing.Color.White;
            this.rbtn_today.Location = new System.Drawing.Point(3, 3);
            this.rbtn_today.Name = "rbtn_today";
            this.rbtn_today.Size = new System.Drawing.Size(68, 49);
            this.rbtn_today.TabIndex = 14;
            this.rbtn_today.TabStop = true;
            this.rbtn_today.Text = "当天";
            this.rbtn_today.UseVisualStyleBackColor = true;
            // 
            // rbtn_oneYear
            // 
            this.rbtn_oneYear.AutoSize = true;
            this.rbtn_oneYear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbtn_oneYear.ForeColor = System.Drawing.Color.White;
            this.rbtn_oneYear.Location = new System.Drawing.Point(295, 3);
            this.rbtn_oneYear.Name = "rbtn_oneYear";
            this.rbtn_oneYear.Size = new System.Drawing.Size(90, 49);
            this.rbtn_oneYear.TabIndex = 18;
            this.rbtn_oneYear.TabStop = true;
            this.rbtn_oneYear.Text = "最近一年";
            this.rbtn_oneYear.UseVisualStyleBackColor = true;
            // 
            // rbtn_custom
            // 
            this.rbtn_custom.AutoSize = true;
            this.rbtn_custom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbtn_custom.ForeColor = System.Drawing.Color.White;
            this.rbtn_custom.Location = new System.Drawing.Point(391, 3);
            this.rbtn_custom.Name = "rbtn_custom";
            this.rbtn_custom.Size = new System.Drawing.Size(189, 49);
            this.rbtn_custom.TabIndex = 13;
            this.rbtn_custom.TabStop = true;
            this.rbtn_custom.Text = "自定义日期";
            this.rbtn_custom.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(589, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "开始日期：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(856, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "结束日期：";
            // 
            // pickerEndTime
            // 
            this.pickerEndTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.pickerEndTime.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pickerEndTime.ForeColor = System.Drawing.Color.SteelBlue;
            this.pickerEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.pickerEndTime.Location = new System.Drawing.Point(940, 20);
            this.pickerEndTime.Name = "pickerEndTime";
            this.pickerEndTime.Size = new System.Drawing.Size(169, 23);
            this.pickerEndTime.TabIndex = 5;
            this.pickerEndTime.TabStop = false;
            this.pickerEndTime.Text = "2019-08-26 17:09:41";
            this.pickerEndTime.Value = new System.DateTime(2019, 8, 26, 17, 9, 41, 544);
            // 
            // pickerStartTime
            // 
            this.pickerStartTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.pickerStartTime.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pickerStartTime.ForeColor = System.Drawing.Color.SteelBlue;
            this.pickerStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.pickerStartTime.Location = new System.Drawing.Point(673, 19);
            this.pickerStartTime.Name = "pickerStartTime";
            this.pickerStartTime.Size = new System.Drawing.Size(171, 23);
            this.pickerStartTime.TabIndex = 4;
            this.pickerStartTime.TabStop = false;
            this.pickerStartTime.Text = "2019-08-26 17:09:35";
            this.pickerStartTime.Value = new System.DateTime(2019, 8, 26, 17, 9, 35, 395);
            // 
            // bindingNavigator1
            // 
            this.bindingNavigator1.AddNewItem = this.bindingNavigatorAddNewItem;
            this.bindingNavigator1.CountItem = this.bindingNavigatorCountItem;
            this.bindingNavigator1.DeleteItem = this.bindingNavigatorDeleteItem;
            this.bindingNavigator1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bindingNavigator1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.bindingNavigator1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2,
            this.bindingNavigatorAddNewItem,
            this.bindingNavigatorDeleteItem});
            this.bindingNavigator1.Location = new System.Drawing.Point(0, 629);
            this.bindingNavigator1.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bindingNavigator1.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bindingNavigator1.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bindingNavigator1.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bindingNavigator1.Name = "bindingNavigator1";
            this.bindingNavigator1.PositionItem = this.bindingNavigatorPositionItem;
            this.bindingNavigator1.Size = new System.Drawing.Size(1276, 27);
            this.bindingNavigator1.TabIndex = 16;
            this.bindingNavigator1.Text = "bindingNavigator1";
            // 
            // bindingNavigatorAddNewItem
            // 
            this.bindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorAddNewItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorAddNewItem.Image")));
            this.bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
            this.bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorAddNewItem.Size = new System.Drawing.Size(24, 24);
            this.bindingNavigatorAddNewItem.Text = "新添";
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(32, 24);
            this.bindingNavigatorCountItem.Text = "/ {0}";
            this.bindingNavigatorCountItem.ToolTipText = "总页数";
            // 
            // bindingNavigatorDeleteItem
            // 
            this.bindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorDeleteItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorDeleteItem.Image")));
            this.bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
            this.bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorDeleteItem.Size = new System.Drawing.Size(24, 24);
            this.bindingNavigatorDeleteItem.Text = "删除";
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(24, 24);
            this.bindingNavigatorMoveFirstItem.Text = "首页";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(24, 24);
            this.bindingNavigatorMovePreviousItem.Text = "上一页";
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 27);
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "位置";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 27);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "当前页";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(24, 24);
            this.bindingNavigatorMoveNextItem.Text = "下一页";
            this.bindingNavigatorMoveNextItem.ToolTipText = "下一页";
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(24, 24);
            this.bindingNavigatorMoveLastItem.Text = "尾页";
            this.bindingNavigatorMoveLastItem.ToolTipText = "尾页";
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // radGridView1
            // 
            this.radGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radGridView1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radGridView1.Location = new System.Drawing.Point(0, 82);
            // 
            // 
            // 
            this.radGridView1.MasterTemplate.ViewDefinition = tableViewDefinition1;
            this.radGridView1.Name = "radGridView1";
            this.radGridView1.Size = new System.Drawing.Size(1276, 547);
            this.radGridView1.TabIndex = 18;
            this.radGridView1.ThemeName = "Breeze";
            // 
            // TestStand
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(1276, 656);
            this.Controls.Add(this.radGridView1);
            this.Controls.Add(this.bindingNavigator1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TestStand";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "测试台";
            this.ThemeName = "Material";
            this.Load += new System.EventHandler(this.TestStand_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pickerEndTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pickerStartTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigator1)).EndInit();
            this.bindingNavigator1.ResumeLayout(false);
            this.bindingNavigator1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox tool_queryCondition;
        private System.Windows.Forms.ToolStripButton tool_export;
        private System.Windows.Forms.ToolStripButton tool_programv;
        private System.Windows.Forms.ToolStripButton tool_specCfg;
        private System.Windows.Forms.ToolStripButton tool_logData;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private Telerik.WinControls.UI.RadDateTimePicker pickerEndTime;
        private Telerik.WinControls.UI.RadDateTimePicker pickerStartTime;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.RadioButton rbtn_threeMonth;
        private System.Windows.Forms.RadioButton rbtn_oneMonth;
        private System.Windows.Forms.RadioButton rbtn_today;
        private System.Windows.Forms.RadioButton rbtn_oneYear;
        private System.Windows.Forms.RadioButton rbtn_custom;
        private System.Windows.Forms.ToolStripComboBox tool_exportCondition;
        private System.Windows.Forms.ToolStripButton tool_clearDB;
        private System.Windows.Forms.ToolStripButton tool_query;
        private System.Windows.Forms.BindingNavigator bindingNavigator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorAddNewItem;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorDeleteItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.BindingSource bindingSource1;
        private Telerik.WinControls.UI.RadGridView radGridView1;
    }
}
