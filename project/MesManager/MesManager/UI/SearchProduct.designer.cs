namespace MesManager.UI
{
    partial class SearchProduct
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
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.tb_search = new Telerik.WinControls.UI.RadTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radGridView1 = new Telerik.WinControls.UI.RadGridView();
            this.btn_apply = new Telerik.WinControls.UI.RadButton();
            this.btn_cancel = new Telerik.WinControls.UI.RadButton();
            this.rb_binded = new Telerik.WinControls.UI.RadRadioButton();
            this.rb_unbinded = new Telerik.WinControls.UI.RadRadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_search)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_apply)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_cancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rb_binded)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rb_unbinded)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(14, 35);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(74, 21);
            this.radLabel1.TabIndex = 0;
            this.radLabel1.Text = "产品SN：";
            this.radLabel1.ThemeName = "Material";
            // 
            // tb_search
            // 
            this.tb_search.Location = new System.Drawing.Point(94, 20);
            this.tb_search.Name = "tb_search";
            this.tb_search.Size = new System.Drawing.Size(664, 36);
            this.tb_search.TabIndex = 1;
            this.tb_search.ThemeName = "Material";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rb_unbinded);
            this.panel1.Controls.Add(this.rb_binded);
            this.panel1.Controls.Add(this.radLabel1);
            this.panel1.Controls.Add(this.tb_search);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(770, 98);
            this.panel1.TabIndex = 3;
            // 
            // radGridView1
            // 
            this.radGridView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.radGridView1.Location = new System.Drawing.Point(0, 98);
            // 
            // 
            // 
            this.radGridView1.MasterTemplate.ViewDefinition = tableViewDefinition1;
            this.radGridView1.Name = "radGridView1";
            this.radGridView1.Size = new System.Drawing.Size(770, 339);
            this.radGridView1.TabIndex = 4;
            this.radGridView1.ThemeName = "Breeze";
            // 
            // btn_apply
            // 
            this.btn_apply.Location = new System.Drawing.Point(469, 454);
            this.btn_apply.Name = "btn_apply";
            this.btn_apply.Size = new System.Drawing.Size(129, 31);
            this.btn_apply.TabIndex = 5;
            this.btn_apply.Text = "确定";
            this.btn_apply.ThemeName = "Breeze";
            // 
            // btn_cancel
            // 
            this.btn_cancel.Location = new System.Drawing.Point(629, 454);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(129, 31);
            this.btn_cancel.TabIndex = 6;
            this.btn_cancel.Text = "取消";
            this.btn_cancel.ThemeName = "Breeze";
            // 
            // rb_binded
            // 
            this.rb_binded.Location = new System.Drawing.Point(94, 74);
            this.rb_binded.Name = "rb_binded";
            this.rb_binded.Size = new System.Drawing.Size(56, 18);
            this.rb_binded.TabIndex = 2;
            this.rb_binded.Text = "已绑定";
            // 
            // rb_unbinded
            // 
            this.rb_unbinded.Location = new System.Drawing.Point(179, 74);
            this.rb_unbinded.Name = "rb_unbinded";
            this.rb_unbinded.Size = new System.Drawing.Size(56, 18);
            this.rb_unbinded.TabIndex = 3;
            this.rb_unbinded.Text = "已解绑";
            // 
            // SearchProduct
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Lavender;
            this.ClientSize = new System.Drawing.Size(770, 497);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_apply);
            this.Controls.Add(this.radGridView1);
            this.Controls.Add(this.panel1);
            this.Name = "SearchProduct";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "查询";
            this.ThemeName = "Material";
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_search)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_apply)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_cancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rb_binded)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rb_unbinded)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadTextBox tb_search;
        private System.Windows.Forms.Panel panel1;
        private Telerik.WinControls.UI.RadGridView radGridView1;
        private Telerik.WinControls.UI.RadButton btn_apply;
        private Telerik.WinControls.UI.RadButton btn_cancel;
        private Telerik.WinControls.UI.RadRadioButton rb_unbinded;
        private Telerik.WinControls.UI.RadRadioButton rb_binded;
    }
}
