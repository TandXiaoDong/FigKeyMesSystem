namespace MesManager.RadView
{
    partial class UpLoadImage
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
            this.btn_sectImage = new Telerik.WinControls.UI.RadButton();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.btn_cancel = new Telerik.WinControls.UI.RadButton();
            this.btn_apply = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.btn_sectImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_cancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_apply)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_sectImage
            // 
            this.btn_sectImage.Location = new System.Drawing.Point(12, 12);
            this.btn_sectImage.Name = "btn_sectImage";
            this.btn_sectImage.Size = new System.Drawing.Size(110, 24);
            this.btn_sectImage.TabIndex = 0;
            this.btn_sectImage.Text = "选择文件";
            this.btn_sectImage.Click += new System.EventHandler(this.Btn_sectImage_Click);
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(139, 15);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(42, 18);
            this.radLabel1.TabIndex = 1;
            this.radLabel1.Text = "文件名";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 56);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(417, 292);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // radLabel2
            // 
            this.radLabel2.Location = new System.Drawing.Point(271, 12);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(148, 18);
            this.radLabel2.TabIndex = 3;
            this.radLabel2.Text = "支持格式：PNG、JPG、ICO";
            // 
            // btn_cancel
            // 
            this.btn_cancel.Location = new System.Drawing.Point(319, 360);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(110, 24);
            this.btn_cancel.TabIndex = 4;
            this.btn_cancel.Text = "取消";
            // 
            // btn_apply
            // 
            this.btn_apply.Location = new System.Drawing.Point(176, 360);
            this.btn_apply.Name = "btn_apply";
            this.btn_apply.Size = new System.Drawing.Size(110, 24);
            this.btn_apply.TabIndex = 5;
            this.btn_apply.Text = "确定";
            this.btn_apply.Click += new System.EventHandler(this.Btn_apply_Click);
            // 
            // UpLoadImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(441, 386);
            this.Controls.Add(this.btn_apply);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.radLabel1);
            this.Controls.Add(this.btn_sectImage);
            this.Name = "UpLoadImage";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "添加产品图片";
            ((System.ComponentModel.ISupportInitialize)(this.btn_sectImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_cancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btn_apply)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadButton btn_sectImage;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadButton btn_cancel;
        private Telerik.WinControls.UI.RadButton btn_apply;
    }
}
