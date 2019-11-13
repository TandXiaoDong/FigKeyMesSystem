namespace MesManager.UI
{
    partial class SearchForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchForm));
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.tb_inputMsg = new Telerik.WinControls.UI.RadTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radGridView1 = new Telerik.WinControls.UI.RadGridView();
            this.tb_selectCode = new Telerik.WinControls.UI.RadTextBox();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.btn_ok = new Telerik.WinControls.UI.RadButton();
            this.btn_cancel = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_inputMsg)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_selectCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_ok)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_cancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radLabel1
            // 
            this.radLabel1.ForeColor = System.Drawing.Color.White;
            this.radLabel1.Location = new System.Drawing.Point(3, 24);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(86, 21);
            this.radLabel1.TabIndex = 1;
            this.radLabel1.Text = "物料编码：";
            this.radLabel1.ThemeName = "Material";
            // 
            // tb_inputMsg
            // 
            this.tb_inputMsg.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_inputMsg.Location = new System.Drawing.Point(98, 20);
            this.tb_inputMsg.Name = "tb_inputMsg";
            this.tb_inputMsg.Size = new System.Drawing.Size(570, 37);
            this.tb_inputMsg.TabIndex = 2;
            this.tb_inputMsg.ThemeName = "Material";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radLabel1);
            this.panel1.Controls.Add(this.tb_inputMsg);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(680, 69);
            this.panel1.TabIndex = 3;
            // 
            // radGridView1
            // 
            this.radGridView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.radGridView1.Location = new System.Drawing.Point(0, 69);
            // 
            // 
            // 
            this.radGridView1.MasterTemplate.ViewDefinition = tableViewDefinition1;
            this.radGridView1.Name = "radGridView1";
            this.radGridView1.Size = new System.Drawing.Size(680, 393);
            this.radGridView1.TabIndex = 4;
            this.radGridView1.ThemeName = "Breeze";
            // 
            // tb_selectCode
            // 
            this.tb_selectCode.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_selectCode.Location = new System.Drawing.Point(127, 469);
            this.tb_selectCode.Name = "tb_selectCode";
            this.tb_selectCode.Size = new System.Drawing.Size(541, 37);
            this.tb_selectCode.TabIndex = 5;
            this.tb_selectCode.ThemeName = "Material";
            // 
            // radLabel2
            // 
            this.radLabel2.ForeColor = System.Drawing.Color.White;
            this.radLabel2.Location = new System.Drawing.Point(3, 471);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(118, 21);
            this.radLabel2.TabIndex = 6;
            this.radLabel2.Text = "已选物料编码：";
            this.radLabel2.ThemeName = "Material";
            // 
            // btn_ok
            // 
            this.btn_ok.Image = global::MesManager.Properties.Resources.Apply_16x16;
            this.btn_ok.Location = new System.Drawing.Point(462, 535);
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.Size = new System.Drawing.Size(83, 30);
            this.btn_ok.TabIndex = 7;
            this.btn_ok.Text = "确定";
            this.btn_ok.ThemeName = "Breeze";
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.Image = global::MesManager.Properties.Resources.Cancel_16x16;
            this.btn_cancel.Location = new System.Drawing.Point(584, 535);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(84, 30);
            this.btn_cancel.TabIndex = 8;
            this.btn_cancel.Text = "取消";
            this.btn_cancel.ThemeName = "Breeze";
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(680, 582);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.tb_selectCode);
            this.Controls.Add(this.radGridView1);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SearchForm";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "检索物料编码";
            this.ThemeName = "Material";
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_inputMsg)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_selectCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_ok)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_cancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadTextBox tb_inputMsg;
        private System.Windows.Forms.Panel panel1;
        private Telerik.WinControls.UI.RadGridView radGridView1;
        private Telerik.WinControls.UI.RadTextBox tb_selectCode;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadButton btn_ok;
        private Telerik.WinControls.UI.RadButton btn_cancel;
    }
}
