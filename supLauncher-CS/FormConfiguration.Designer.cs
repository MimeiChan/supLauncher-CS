namespace HiMenu
{
    partial class FormConfiguration
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.lblRows = new System.Windows.Forms.Label();
            this.lblCols = new System.Windows.Forms.Label();
            this.lblWidth = new System.Windows.Forms.Label();
            this.lblHeight = new System.Windows.Forms.Label();
            this.nudRows = new System.Windows.Forms.NumericUpDown();
            this.nudCols = new System.Windows.Forms.NumericUpDown();
            this.nudWidth = new System.Windows.Forms.NumericUpDown();
            this.nudHeight = new System.Windows.Forms.NumericUpDown();
            this.gbMenuDispPosition = new System.Windows.Forms.GroupBox();
            this.optDispositionCenter = new System.Windows.Forms.RadioButton();
            this.optDispositionCurrent = new System.Windows.Forms.RadioButton();
            this.optDispositionRoot = new System.Windows.Forms.RadioButton();
            this.gbMenuOption = new System.Windows.Forms.GroupBox();
            this.chkStatusVisible = new System.Windows.Forms.CheckBox();
            this.chkMenuVisible = new System.Windows.Forms.CheckBox();
            this.chkBGTile = new System.Windows.Forms.CheckBox();
            this.cmdBGFileBrowse = new System.Windows.Forms.Button();
            this.txtBGFile = new System.Windows.Forms.TextBox();
            this.lblBGFile = new System.Windows.Forms.Label();
            this.gbFont = new System.Windows.Forms.GroupBox();
            this.chkFontUnderline = new System.Windows.Forms.CheckBox();
            this.chkFontItalic = new System.Windows.Forms.CheckBox();
            this.chkFontBold = new System.Windows.Forms.CheckBox();
            this.nudFontSize = new System.Windows.Forms.NumericUpDown();
            this.cmbFontName = new System.Windows.Forms.ComboBox();
            this.lblFontSize = new System.Windows.Forms.Label();
            this.lblFontName = new System.Windows.Forms.Label();
            this.gbColors = new System.Windows.Forms.GroupBox();
            this.cmdHighlightTextColor = new System.Windows.Forms.Button();
            this.cmdTextColor = new System.Windows.Forms.Button();
            this.cmdButtonColor = new System.Windows.Forms.Button();
            this.cmdBackColor = new System.Windows.Forms.Button();
            this.lblHighlightTextColor = new System.Windows.Forms.Label();
            this.lblTextColor = new System.Windows.Forms.Label();
            this.lblButtonColor = new System.Windows.Forms.Label();
            this.lblBackColor = new System.Windows.Forms.Label();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudRows)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCols)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).BeginInit();
            this.gbMenuDispPosition.SuspendLayout();
            this.gbMenuOption.SuspendLayout();
            this.gbFont.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFontSize)).BeginInit();
            this.gbColors.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Location = new System.Drawing.Point(13, 13);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(100, 19);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "タイトル:";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtTitle
            // 
            this.txtTitle.Location = new System.Drawing.Point(119, 13);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(354, 19);
            this.txtTitle.TabIndex = 0;
            // 
            // lblRows
            // 
            this.lblRows.Location = new System.Drawing.Point(13, 38);
            this.lblRows.Name = "lblRows";
            this.lblRows.Size = new System.Drawing.Size(100, 19);
            this.lblRows.TabIndex = 2;
            this.lblRows.Text = "メニュー行数:";
            this.lblRows.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblCols
            // 
            this.lblCols.Location = new System.Drawing.Point(13, 63);
            this.lblCols.Name = "lblCols";
            this.lblCols.Size = new System.Drawing.Size(100, 19);
            this.lblCols.TabIndex = 3;
            this.lblCols.Text = "メニュー列数:";
            this.lblCols.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblWidth
            // 
            this.lblWidth.Location = new System.Drawing.Point(13, 88);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Size = new System.Drawing.Size(100, 19);
            this.lblWidth.TabIndex = 4;
            this.lblWidth.Text = "メニュー幅:";
            this.lblWidth.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblHeight
            // 
            this.lblHeight.Location = new System.Drawing.Point(13, 113);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Size = new System.Drawing.Size(100, 19);
            this.lblHeight.TabIndex = 5;
            this.lblHeight.Text = "メニュー高さ:";
            this.lblHeight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // nudRows
            // 
            this.nudRows.Location = new System.Drawing.Point(119, 38);
            this.nudRows.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.nudRows.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRows.Name = "nudRows";
            this.nudRows.Size = new System.Drawing.Size(70, 19);
            this.nudRows.TabIndex = 1;
            this.nudRows.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nudCols
            // 
            this.nudCols.Location = new System.Drawing.Point(119, 63);
            this.nudCols.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudCols.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudCols.Name = "nudCols";
            this.nudCols.Size = new System.Drawing.Size(70, 19);
            this.nudCols.TabIndex = 2;
            this.nudCols.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // nudWidth
            // 
            this.nudWidth.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudWidth.Location = new System.Drawing.Point(119, 88);
            this.nudWidth.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.nudWidth.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudWidth.Name = "nudWidth";
            this.nudWidth.Size = new System.Drawing.Size(70, 19);
            this.nudWidth.TabIndex = 3;
            this.nudWidth.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // nudHeight
            // 
            this.nudHeight.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudHeight.Location = new System.Drawing.Point(119, 113);
            this.nudHeight.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.nudHeight.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudHeight.Name = "nudHeight";
            this.nudHeight.Size = new System.Drawing.Size(70, 19);
            this.nudHeight.TabIndex = 4;
            this.nudHeight.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // gbMenuDispPosition
            // 
            this.gbMenuDispPosition.Controls.Add(this.optDispositionCenter);
            this.gbMenuDispPosition.Controls.Add(this.optDispositionCurrent);
            this.gbMenuDispPosition.Controls.Add(this.optDispositionRoot);
            this.gbMenuDispPosition.Location = new System.Drawing.Point(13, 139);
            this.gbMenuDispPosition.Name = "gbMenuDispPosition";
            this.gbMenuDispPosition.Size = new System.Drawing.Size(227, 100);
            this.gbMenuDispPosition.TabIndex = 5;
            this.gbMenuDispPosition.TabStop = false;
            this.gbMenuDispPosition.Text = "メニュー表示位置";
            // 
            // optDispositionCenter
            // 
            this.optDispositionCenter.AutoSize = true;
            this.optDispositionCenter.Location = new System.Drawing.Point(16, 67);
            this.optDispositionCenter.Name = "optDispositionCenter";
            this.optDispositionCenter.Size = new System.Drawing.Size(104, 16);
            this.optDispositionCenter.TabIndex = 2;
            this.optDispositionCenter.TabStop = true;
            this.optDispositionCenter.Text = "画面中央に表示";
            this.optDispositionCenter.UseVisualStyleBackColor = true;
            // 
            // optDispositionCurrent
            // 
            this.optDispositionCurrent.AutoSize = true;
            this.optDispositionCurrent.Location = new System.Drawing.Point(16, 43);
            this.optDispositionCurrent.Name = "optDispositionCurrent";
            this.optDispositionCurrent.Size = new System.Drawing.Size(154, 16);
            this.optDispositionCurrent.TabIndex = 1;
            this.optDispositionCurrent.TabStop = true;
            this.optDispositionCurrent.Text = "現在の同じ位置に表示する";
            this.optDispositionCurrent.UseVisualStyleBackColor = true;
            // 
            // optDispositionRoot
            // 
            this.optDispositionRoot.AutoSize = true;
            this.optDispositionRoot.Location = new System.Drawing.Point(16, 19);
            this.optDispositionRoot.Name = "optDispositionRoot";
            this.optDispositionRoot.Size = new System.Drawing.Size(178, 16);
            this.optDispositionRoot.TabIndex = 0;
            this.optDispositionRoot.TabStop = true;
            this.optDispositionRoot.Text = "最初のメニューの位置に表示する";
            this.optDispositionRoot.UseVisualStyleBackColor = true;
            // 
            // gbMenuOption
            // 
            this.gbMenuOption.Controls.Add(this.chkStatusVisible);
            this.gbMenuOption.Controls.Add(this.chkMenuVisible);
            this.gbMenuOption.Controls.Add(this.chkBGTile);
            this.gbMenuOption.Controls.Add(this.cmdBGFileBrowse);
            this.gbMenuOption.Controls.Add(this.txtBGFile);
            this.gbMenuOption.Controls.Add(this.lblBGFile);
            this.gbMenuOption.Location = new System.Drawing.Point(246, 139);
            this.gbMenuOption.Name = "gbMenuOption";
            this.gbMenuOption.Size = new System.Drawing.Size(227, 100);
            this.gbMenuOption.TabIndex = 6;
            this.gbMenuOption.TabStop = false;
            this.gbMenuOption.Text = "メニューオプション";
            // 
            // chkStatusVisible
            // 
            this.chkStatusVisible.AutoSize = true;
            this.chkStatusVisible.Location = new System.Drawing.Point(61, 83);
            this.chkStatusVisible.Name = "chkStatusVisible";
            this.chkStatusVisible.Size = new System.Drawing.Size(122, 16);
            this.chkStatusVisible.TabIndex = 4;
            this.chkStatusVisible.Text = "ステータスバーを表示";
            this.chkStatusVisible.UseVisualStyleBackColor = true;
            // 
            // chkMenuVisible
            // 
            this.chkMenuVisible.AutoSize = true;
            this.chkMenuVisible.Location = new System.Drawing.Point(61, 63);
            this.chkMenuVisible.Name = "chkMenuVisible";
            this.chkMenuVisible.Size = new System.Drawing.Size(112, 16);
            this.chkMenuVisible.TabIndex = 3;
            this.chkMenuVisible.Text = "メニューバーを表示";
            this.chkMenuVisible.UseVisualStyleBackColor = true;
            // 
            // chkBGTile
            // 
            this.chkBGTile.AutoSize = true;
            this.chkBGTile.Location = new System.Drawing.Point(61, 43);
            this.chkBGTile.Name = "chkBGTile";
            this.chkBGTile.Size = new System.Drawing.Size(151, 16);
            this.chkBGTile.TabIndex = 2;
            this.chkBGTile.Text = "背景画像をタイル表示する";
            this.chkBGTile.UseVisualStyleBackColor = true;
            // 
            // cmdBGFileBrowse
            // 
            this.cmdBGFileBrowse.Location = new System.Drawing.Point(190, 17);
            this.cmdBGFileBrowse.Name = "cmdBGFileBrowse";
            this.cmdBGFileBrowse.Size = new System.Drawing.Size(28, 20);
            this.cmdBGFileBrowse.TabIndex = 1;
            this.cmdBGFileBrowse.Text = "...";
            this.cmdBGFileBrowse.UseVisualStyleBackColor = true;
            // 
            // txtBGFile
            // 
            this.txtBGFile.Location = new System.Drawing.Point(61, 18);
            this.txtBGFile.Name = "txtBGFile";
            this.txtBGFile.Size = new System.Drawing.Size(123, 19);
            this.txtBGFile.TabIndex = 0;
            // 
            // lblBGFile
            // 
            this.lblBGFile.AutoSize = true;
            this.lblBGFile.Location = new System.Drawing.Point(7, 21);
            this.lblBGFile.Name = "lblBGFile";
            this.lblBGFile.Size = new System.Drawing.Size(53, 12);
            this.lblBGFile.TabIndex = 0;
            this.lblBGFile.Text = "背景画像";
            // 
            // gbFont
            // 
            this.gbFont.Controls.Add(this.chkFontUnderline);
            this.gbFont.Controls.Add(this.chkFontItalic);
            this.gbFont.Controls.Add(this.chkFontBold);
            this.gbFont.Controls.Add(this.nudFontSize);
            this.gbFont.Controls.Add(this.cmbFontName);
            this.gbFont.Controls.Add(this.lblFontSize);
            this.gbFont.Controls.Add(this.lblFontName);
            this.gbFont.Location = new System.Drawing.Point(13, 245);
            this.gbFont.Name = "gbFont";
            this.gbFont.Size = new System.Drawing.Size(227, 100);
            this.gbFont.TabIndex = 7;
            this.gbFont.TabStop = false;
            this.gbFont.Text = "フォント設定";
            // 
            // chkFontUnderline
            // 
            this.chkFontUnderline.AutoSize = true;
            this.chkFontUnderline.Location = new System.Drawing.Point(118, 75);
            this.chkFontUnderline.Name = "chkFontUnderline";
            this.chkFontUnderline.Size = new System.Drawing.Size(60, 16);
            this.chkFontUnderline.TabIndex = 4;
            this.chkFontUnderline.Text = "下線付";
            this.chkFontUnderline.UseVisualStyleBackColor = true;
            // 
            // chkFontItalic
            // 
            this.chkFontItalic.AutoSize = true;
            this.chkFontItalic.Location = new System.Drawing.Point(64, 75);
            this.chkFontItalic.Name = "chkFontItalic";
            this.chkFontItalic.Size = new System.Drawing.Size(48, 16);
            this.chkFontItalic.TabIndex = 3;
            this.chkFontItalic.Text = "斜体";
            this.chkFontItalic.UseVisualStyleBackColor = true;
            // 
            // chkFontBold
            // 
            this.chkFontBold.AutoSize = true;
            this.chkFontBold.Location = new System.Drawing.Point(10, 75);
            this.chkFontBold.Name = "chkFontBold";
            this.chkFontBold.Size = new System.Drawing.Size(48, 16);
            this.chkFontBold.TabIndex = 2;
            this.chkFontBold.Text = "太字";
            this.chkFontBold.UseVisualStyleBackColor = true;
            // 
            // nudFontSize
            // 
            this.nudFontSize.DecimalPlaces = 1;
            this.nudFontSize.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.nudFontSize.Location = new System.Drawing.Point(66, 48);
            this.nudFontSize.Maximum = new decimal(new int[] {
            72,
            0,
            0,
            0});
            this.nudFontSize.Minimum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.nudFontSize.Name = "nudFontSize";
            this.nudFontSize.Size = new System.Drawing.Size(70, 19);
            this.nudFontSize.TabIndex = 1;
            this.nudFontSize.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // cmbFontName
            // 
            this.cmbFontName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFontName.FormattingEnabled = true;
            this.cmbFontName.Location = new System.Drawing.Point(66, 20);
            this.cmbFontName.Name = "cmbFontName";
            this.cmbFontName.Size = new System.Drawing.Size(155, 20);
            this.cmbFontName.TabIndex = 0;
            // 
            // lblFontSize
            // 
            this.lblFontSize.AutoSize = true;
            this.lblFontSize.Location = new System.Drawing.Point(8, 50);
            this.lblFontSize.Name = "lblFontSize";
            this.lblFontSize.Size = new System.Drawing.Size(50, 12);
            this.lblFontSize.TabIndex = 1;
            this.lblFontSize.Text = "フォント幅";
            // 
            // lblFontName
            // 
            this.lblFontName.AutoSize = true;
            this.lblFontName.Location = new System.Drawing.Point(8, 23);
            this.lblFontName.Name = "lblFontName";
            this.lblFontName.Size = new System.Drawing.Size(50, 12);
            this.lblFontName.TabIndex = 0;
            this.lblFontName.Text = "フォント名";
            // 
            // gbColors
            // 
            this.gbColors.Controls.Add(this.cmdHighlightTextColor);
            this.gbColors.Controls.Add(this.cmdTextColor);
            this.gbColors.Controls.Add(this.cmdButtonColor);
            this.gbColors.Controls.Add(this.cmdBackColor);
            this.gbColors.Controls.Add(this.lblHighlightTextColor);
            this.gbColors.Controls.Add(this.lblTextColor);
            this.gbColors.Controls.Add(this.lblButtonColor);
            this.gbColors.Controls.Add(this.lblBackColor);
            this.gbColors.Location = new System.Drawing.Point(246, 245);
            this.gbColors.Name = "gbColors";
            this.gbColors.Size = new System.Drawing.Size(227, 131);
            this.gbColors.TabIndex = 8;
            this.gbColors.TabStop = false;
            this.gbColors.Text = "色設定";
            // 
            // cmdHighlightTextColor
            // 
            this.cmdHighlightTextColor.BackColor = System.Drawing.SystemColors.Highlight;
            this.cmdHighlightTextColor.ForeColor = System.Drawing.SystemColors.Control;
            this.cmdHighlightTextColor.Location = new System.Drawing.Point(97, 100);
            this.cmdHighlightTextColor.Name = "cmdHighlightTextColor";
            this.cmdHighlightTextColor.Size = new System.Drawing.Size(124, 19);
            this.cmdHighlightTextColor.TabIndex = 3;
            this.cmdHighlightTextColor.Text = "変更...";
            this.cmdHighlightTextColor.UseVisualStyleBackColor = false;
            // 
            // cmdTextColor
            // 
            this.cmdTextColor.BackColor = System.Drawing.SystemColors.ControlText;
            this.cmdTextColor.ForeColor = System.Drawing.SystemColors.Control;
            this.cmdTextColor.Location = new System.Drawing.Point(97, 74);
            this.cmdTextColor.Name = "cmdTextColor";
            this.cmdTextColor.Size = new System.Drawing.Size(124, 19);
            this.cmdTextColor.TabIndex = 2;
            this.cmdTextColor.Text = "変更...";
            this.cmdTextColor.UseVisualStyleBackColor = false;
            // 
            // cmdButtonColor
            // 
            this.cmdButtonColor.BackColor = System.Drawing.SystemColors.Control;
            this.cmdButtonColor.Location = new System.Drawing.Point(97, 48);
            this.cmdButtonColor.Name = "cmdButtonColor";
            this.cmdButtonColor.Size = new System.Drawing.Size(124, 19);
            this.cmdButtonColor.TabIndex = 1;
            this.cmdButtonColor.Text = "変更...";
            this.cmdButtonColor.UseVisualStyleBackColor = false;
            // 
            // cmdBackColor
            // 
            this.cmdBackColor.BackColor = System.Drawing.SystemColors.ControlDark;
            this.cmdBackColor.Location = new System.Drawing.Point(97, 22);
            this.cmdBackColor.Name = "cmdBackColor";
            this.cmdBackColor.Size = new System.Drawing.Size(124, 19);
            this.cmdBackColor.TabIndex = 0;
            this.cmdBackColor.Text = "変更...";
            this.cmdBackColor.UseVisualStyleBackColor = false;
            // 
            // lblHighlightTextColor
            // 
            this.lblHighlightTextColor.AutoSize = true;
            this.lblHighlightTextColor.Location = new System.Drawing.Point(7, 103);
            this.lblHighlightTextColor.Name = "lblHighlightTextColor";
            this.lblHighlightTextColor.Size = new System.Drawing.Size(91, 12);
            this.lblHighlightTextColor.TabIndex = 3;
            this.lblHighlightTextColor.Text = "文字色(選択中)：";
            // 
            // lblTextColor
            // 
            this.lblTextColor.AutoSize = true;
            this.lblTextColor.Location = new System.Drawing.Point(7, 77);
            this.lblTextColor.Name = "lblTextColor";
            this.lblTextColor.Size = new System.Drawing.Size(79, 12);
            this.lblTextColor.TabIndex = 2;
            this.lblTextColor.Text = "文字色(通常)：";
            // 
            // lblButtonColor
            // 
            this.lblButtonColor.AutoSize = true;
            this.lblButtonColor.Location = new System.Drawing.Point(7, 51);
            this.lblButtonColor.Name = "lblButtonColor";
            this.lblButtonColor.Size = new System.Drawing.Size(50, 12);
            this.lblButtonColor.TabIndex = 1;
            this.lblButtonColor.Text = "ボタン色：";
            // 
            // lblBackColor
            // 
            this.lblBackColor.AutoSize = true;
            this.lblBackColor.Location = new System.Drawing.Point(7, 25);
            this.lblBackColor.Name = "lblBackColor";
            this.lblBackColor.Size = new System.Drawing.Size(47, 12);
            this.lblBackColor.TabIndex = 0;
            this.lblBackColor.Text = "背景色：";
            // 
            // cmdOK
            // 
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(317, 382);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 9;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(398, 382);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 10;
            this.cmdCancel.Text = "キャンセル";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // FormConfiguration
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(484, 414);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.gbColors);
            this.Controls.Add(this.gbFont);
            this.Controls.Add(this.gbMenuOption);
            this.Controls.Add(this.gbMenuDispPosition);
            this.Controls.Add(this.nudHeight);
            this.Controls.Add(this.nudWidth);
            this.Controls.Add(this.nudCols);
            this.Controls.Add(this.nudRows);
            this.Controls.Add(this.lblHeight);
            this.Controls.Add(this.lblWidth);
            this.Controls.Add(this.lblCols);
            this.Controls.Add(this.lblRows);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfiguration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "環境設定";
            ((System.ComponentModel.ISupportInitialize)(this.nudRows)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCols)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).EndInit();
            this.gbMenuDispPosition.ResumeLayout(false);
            this.gbMenuDispPosition.PerformLayout();
            this.gbMenuOption.ResumeLayout(false);
            this.gbMenuOption.PerformLayout();
            this.gbFont.ResumeLayout(false);
            this.gbFont.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFontSize)).EndInit();
            this.gbColors.ResumeLayout(false);
            this.gbColors.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Label lblRows;
        private System.Windows.Forms.Label lblCols;
        private System.Windows.Forms.Label lblWidth;
        private System.Windows.Forms.Label lblHeight;
        private System.Windows.Forms.NumericUpDown nudRows;
        private System.Windows.Forms.NumericUpDown nudCols;
        private System.Windows.Forms.NumericUpDown nudWidth;
        private System.Windows.Forms.NumericUpDown nudHeight;
        private System.Windows.Forms.GroupBox gbMenuDispPosition;
        private System.Windows.Forms.RadioButton optDispositionCenter;
        private System.Windows.Forms.RadioButton optDispositionCurrent;
        private System.Windows.Forms.RadioButton optDispositionRoot;
        private System.Windows.Forms.GroupBox gbMenuOption;
        private System.Windows.Forms.CheckBox chkStatusVisible;
        private System.Windows.Forms.CheckBox chkMenuVisible;
        private System.Windows.Forms.CheckBox chkBGTile;
        private System.Windows.Forms.Button cmdBGFileBrowse;
        private System.Windows.Forms.TextBox txtBGFile;
        private System.Windows.Forms.Label lblBGFile;
        private System.Windows.Forms.GroupBox gbFont;
        private System.Windows.Forms.CheckBox chkFontUnderline;
        private System.Windows.Forms.CheckBox chkFontItalic;
        private System.Windows.Forms.CheckBox chkFontBold;
        private System.Windows.Forms.NumericUpDown nudFontSize;
        private System.Windows.Forms.ComboBox cmbFontName;
        private System.Windows.Forms.Label lblFontSize;
        private System.Windows.Forms.Label lblFontName;
        private System.Windows.Forms.GroupBox gbColors;
        private System.Windows.Forms.Button cmdHighlightTextColor;
        private System.Windows.Forms.Button cmdTextColor;
        private System.Windows.Forms.Button cmdButtonColor;
        private System.Windows.Forms.Button cmdBackColor;
        private System.Windows.Forms.Label lblHighlightTextColor;
        private System.Windows.Forms.Label lblTextColor;
        private System.Windows.Forms.Label lblButtonColor;
        private System.Windows.Forms.Label lblBackColor;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdCancel;
    }
}
