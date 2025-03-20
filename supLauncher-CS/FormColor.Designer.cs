namespace HiMenu
{
    partial class FormColor
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.lblRed = new System.Windows.Forms.Label();
            this.lblGreen = new System.Windows.Forms.Label();
            this.lblBlue = new System.Windows.Forms.Label();
            this.lblPreview = new System.Windows.Forms.Label();
            this.hsbRed = new System.Windows.Forms.HScrollBar();
            this.hsbGreen = new System.Windows.Forms.HScrollBar();
            this.hsbBlue = new System.Windows.Forms.HScrollBar();
            this.txtRed = new System.Windows.Forms.TextBox();
            this.txtGreen = new System.Windows.Forms.TextBox();
            this.txtBlue = new System.Windows.Forms.TextBox();
            this.picPreview = new System.Windows.Forms.PictureBox();
            this.cmdStandard = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // lblRed
            // 
            this.lblRed.AutoSize = true;
            this.lblRed.Location = new System.Drawing.Point(12, 20);
            this.lblRed.Name = "lblRed";
            this.lblRed.Size = new System.Drawing.Size(17, 12);
            this.lblRed.TabIndex = 0;
            this.lblRed.Text = "赤:";
            // 
            // lblGreen
            // 
            this.lblGreen.AutoSize = true;
            this.lblGreen.Location = new System.Drawing.Point(12, 50);
            this.lblGreen.Name = "lblGreen";
            this.lblGreen.Size = new System.Drawing.Size(17, 12);
            this.lblGreen.TabIndex = 1;
            this.lblGreen.Text = "緑:";
            // 
            // lblBlue
            // 
            this.lblBlue.AutoSize = true;
            this.lblBlue.Location = new System.Drawing.Point(12, 80);
            this.lblBlue.Name = "lblBlue";
            this.lblBlue.Size = new System.Drawing.Size(17, 12);
            this.lblBlue.TabIndex = 2;
            this.lblBlue.Text = "青:";
            // 
            // lblPreview
            // 
            this.lblPreview.AutoSize = true;
            this.lblPreview.Location = new System.Drawing.Point(12, 117);
            this.lblPreview.Name = "lblPreview";
            this.lblPreview.Size = new System.Drawing.Size(53, 12);
            this.lblPreview.TabIndex = 3;
            this.lblPreview.Text = "プレビュー:";
            // 
            // hsbRed
            // 
            this.hsbRed.LargeChange = 16;
            this.hsbRed.Location = new System.Drawing.Point(35, 20);
            this.hsbRed.Maximum = 264;
            this.hsbRed.Name = "hsbRed";
            this.hsbRed.Size = new System.Drawing.Size(158, 17);
            this.hsbRed.TabIndex = 0;
            // 
            // hsbGreen
            // 
            this.hsbGreen.LargeChange = 16;
            this.hsbGreen.Location = new System.Drawing.Point(35, 50);
            this.hsbGreen.Maximum = 264;
            this.hsbGreen.Name = "hsbGreen";
            this.hsbGreen.Size = new System.Drawing.Size(158, 17);
            this.hsbGreen.TabIndex = 1;
            // 
            // hsbBlue
            // 
            this.hsbBlue.LargeChange = 16;
            this.hsbBlue.Location = new System.Drawing.Point(35, 80);
            this.hsbBlue.Maximum = 264;
            this.hsbBlue.Name = "hsbBlue";
            this.hsbBlue.Size = new System.Drawing.Size(158, 17);
            this.hsbBlue.TabIndex = 2;
            // 
            // txtRed
            // 
            this.txtRed.Location = new System.Drawing.Point(199, 20);
            this.txtRed.MaxLength = 3;
            this.txtRed.Name = "txtRed";
            this.txtRed.Size = new System.Drawing.Size(33, 19);
            this.txtRed.TabIndex = 3;
            this.txtRed.Text = "0";
            this.txtRed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtGreen
            // 
            this.txtGreen.Location = new System.Drawing.Point(199, 50);
            this.txtGreen.MaxLength = 3;
            this.txtGreen.Name = "txtGreen";
            this.txtGreen.Size = new System.Drawing.Size(33, 19);
            this.txtGreen.TabIndex = 4;
            this.txtGreen.Text = "0";
            this.txtGreen.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtBlue
            // 
            this.txtBlue.Location = new System.Drawing.Point(199, 80);
            this.txtBlue.MaxLength = 3;
            this.txtBlue.Name = "txtBlue";
            this.txtBlue.Size = new System.Drawing.Size(33, 19);
            this.txtBlue.TabIndex = 5;
            this.txtBlue.Text = "0";
            this.txtBlue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // picPreview
            // 
            this.picPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picPreview.Location = new System.Drawing.Point(71, 117);
            this.picPreview.Name = "picPreview";
            this.picPreview.Size = new System.Drawing.Size(161, 64);
            this.picPreview.TabIndex = 10;
            this.picPreview.TabStop = false;
            // 
            // cmdStandard
            // 
            this.cmdStandard.Location = new System.Drawing.Point(14, 158);
            this.cmdStandard.Name = "cmdStandard";
            this.cmdStandard.Size = new System.Drawing.Size(51, 23);
            this.cmdStandard.TabIndex = 6;
            this.cmdStandard.Text = "標準";
            this.cmdStandard.UseVisualStyleBackColor = true;
            // 
            // cmdOK
            // 
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(76, 196);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 7;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(157, 196);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 8;
            this.cmdCancel.Text = "キャンセル";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // FormColor
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(244, 231);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.cmdStandard);
            this.Controls.Add(this.picPreview);
            this.Controls.Add(this.txtBlue);
            this.Controls.Add(this.txtGreen);
            this.Controls.Add(this.txtRed);
            this.Controls.Add(this.hsbBlue);
            this.Controls.Add(this.hsbGreen);
            this.Controls.Add(this.hsbRed);
            this.Controls.Add(this.lblPreview);
            this.Controls.Add(this.lblBlue);
            this.Controls.Add(this.lblGreen);
            this.Controls.Add(this.lblRed);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormColor";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "色の選択";
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblRed;
        private System.Windows.Forms.Label lblGreen;
        private System.Windows.Forms.Label lblBlue;
        private System.Windows.Forms.Label lblPreview;
        private System.Windows.Forms.HScrollBar hsbRed;
        private System.Windows.Forms.HScrollBar hsbGreen;
        private System.Windows.Forms.HScrollBar hsbBlue;
        private System.Windows.Forms.TextBox txtRed;
        private System.Windows.Forms.TextBox txtGreen;
        private System.Windows.Forms.TextBox txtBlue;
        private System.Windows.Forms.PictureBox picPreview;
        private System.Windows.Forms.Button cmdStandard;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdCancel;
    }
}
