namespace HiMenu
{
    partial class FormButtonEdit
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
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.txtComment = new System.Windows.Forms.TextBox();
            this.txtCommand = new System.Windows.Forms.TextBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblComment = new System.Windows.Forms.Label();
            this.lblCommand = new System.Windows.Forms.Label();
            this.chkNoUse = new System.Windows.Forms.CheckBox();
            this.optAttributeExec = new System.Windows.Forms.RadioButton();
            this.optAttributeNext = new System.Windows.Forms.RadioButton();
            this.optAttributeBack = new System.Windows.Forms.RadioButton();
            this.optAfterContinue = new System.Windows.Forms.RadioButton();
            this.optAfterEnd = new System.Windows.Forms.RadioButton();
            this.optAfterMinimize = new System.Windows.Forms.RadioButton();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdBrowse = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtTitle
            // 
            this.txtTitle.Location = new System.Drawing.Point(127, 12);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(354, 19);
            this.txtTitle.TabIndex = 0;
            // 
            // txtComment
            // 
            this.txtComment.Location = new System.Drawing.Point(127, 37);
            this.txtComment.Name = "txtComment";
            this.txtComment.Size = new System.Drawing.Size(354, 19);
            this.txtComment.TabIndex = 1;
            // 
            // txtCommand
            // 
            this.txtCommand.Location = new System.Drawing.Point(127, 62);
            this.txtCommand.Name = "txtCommand";
            this.txtCommand.Size = new System.Drawing.Size(300, 19);
            this.txtCommand.TabIndex = 2;
            // 
            // lblTitle
            // 
            this.lblTitle.Location = new System.Drawing.Point(13, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(108, 19);
            this.lblTitle.TabIndex = 3;
            this.lblTitle.Text = "タイトル(&T)";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblComment
            // 
            this.lblComment.Location = new System.Drawing.Point(13, 37);
            this.lblComment.Name = "lblComment";
            this.lblComment.Size = new System.Drawing.Size(108, 19);
            this.lblComment.TabIndex = 4;
            this.lblComment.Text = "コメント(&C)";
            this.lblComment.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblCommand
            // 
            this.lblCommand.Location = new System.Drawing.Point(13, 62);
            this.lblCommand.Name = "lblCommand";
            this.lblCommand.Size = new System.Drawing.Size(108, 19);
            this.lblCommand.TabIndex = 5;
            this.lblCommand.Text = "実行コマンド(&E)";
            this.lblCommand.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkNoUse
            // 
            this.chkNoUse.Location = new System.Drawing.Point(12, 345);
            this.chkNoUse.Name = "chkNoUse";
            this.chkNoUse.Size = new System.Drawing.Size(131, 24);
            this.chkNoUse.TabIndex = 9;
            this.chkNoUse.Text = "表示しない(&N)";
            this.chkNoUse.UseVisualStyleBackColor = true;
            // 
            // optAttributeExec
            // 
            this.optAttributeExec.Checked = true;
            this.optAttributeExec.Location = new System.Drawing.Point(83, 14);
            this.optAttributeExec.Name = "optAttributeExec";
            this.optAttributeExec.Size = new System.Drawing.Size(180, 20);
            this.optAttributeExec.TabIndex = 3;
            this.optAttributeExec.TabStop = true;
            this.optAttributeExec.Text = "アプリケーションの実行";
            this.optAttributeExec.UseVisualStyleBackColor = true;
            // 
            // optAttributeNext
            // 
            this.optAttributeNext.Location = new System.Drawing.Point(83, 40);
            this.optAttributeNext.Name = "optAttributeNext";
            this.optAttributeNext.Size = new System.Drawing.Size(180, 20);
            this.optAttributeNext.TabIndex = 4;
            this.optAttributeNext.Text = "次のメニューを開く";
            this.optAttributeNext.UseVisualStyleBackColor = true;
            // 
            // optAttributeBack
            // 
            this.optAttributeBack.Location = new System.Drawing.Point(83, 66);
            this.optAttributeBack.Name = "optAttributeBack";
            this.optAttributeBack.Size = new System.Drawing.Size(180, 20);
            this.optAttributeBack.TabIndex = 5;
            this.optAttributeBack.Text = "前のメニューに戻る";
            this.optAttributeBack.UseVisualStyleBackColor = true;
            // 
            // optAfterContinue
            // 
            this.optAfterContinue.Checked = true;
            this.optAfterContinue.Location = new System.Drawing.Point(83, 20);
            this.optAfterContinue.Name = "optAfterContinue";
            this.optAfterContinue.Size = new System.Drawing.Size(228, 19);
            this.optAfterContinue.TabIndex = 6;
            this.optAfterContinue.TabStop = true;
            this.optAfterContinue.Text = "ランチャーを開いたままにする";
            this.optAfterContinue.UseVisualStyleBackColor = true;
            // 
            // optAfterEnd
            // 
            this.optAfterEnd.Location = new System.Drawing.Point(83, 45);
            this.optAfterEnd.Name = "optAfterEnd";
            this.optAfterEnd.Size = new System.Drawing.Size(228, 19);
            this.optAfterEnd.TabIndex = 7;
            this.optAfterEnd.Text = "ランチャーを終了する";
            this.optAfterEnd.UseVisualStyleBackColor = true;
            // 
            // optAfterMinimize
            // 
            this.optAfterMinimize.Location = new System.Drawing.Point(83, 70);
            this.optAfterMinimize.Name = "optAfterMinimize";
            this.optAfterMinimize.Size = new System.Drawing.Size(228, 19);
            this.optAfterMinimize.TabIndex = 8;
            this.optAfterMinimize.Text = "ランチャーを最小化する";
            this.optAfterMinimize.UseVisualStyleBackColor = true;
            // 
            // cmdOK
            // 
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(326, 336);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 10;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(407, 336);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 11;
            this.cmdCancel.Text = "キャンセル";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // cmdBrowse
            // 
            this.cmdBrowse.Location = new System.Drawing.Point(427, 62);
            this.cmdBrowse.Name = "cmdBrowse";
            this.cmdBrowse.Size = new System.Drawing.Size(54, 23);
            this.cmdBrowse.TabIndex = 12;
            this.cmdBrowse.Text = "参照...";
            this.cmdBrowse.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.optAttributeExec);
            this.groupBox1.Controls.Add(this.optAttributeNext);
            this.groupBox1.Controls.Add(this.optAttributeBack);
            this.groupBox1.Location = new System.Drawing.Point(43, 87);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(438, 107);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ボタンの属性";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.optAfterContinue);
            this.groupBox2.Controls.Add(this.optAfterEnd);
            this.groupBox2.Controls.Add(this.optAfterMinimize);
            this.groupBox2.Location = new System.Drawing.Point(43, 200);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(438, 106);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "アプリ起動後のランチャーの状態";
            // 
            // FormButtonEdit
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(514, 371);
            this.Controls.Add(this.cmdBrowse);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.chkNoUse);
            this.Controls.Add(this.lblCommand);
            this.Controls.Add(this.lblComment);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.txtCommand);
            this.Controls.Add(this.txtComment);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormButtonEdit";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ボタン詳細設定";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.TextBox txtComment;
        private System.Windows.Forms.TextBox txtCommand;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblComment;
        private System.Windows.Forms.Label lblCommand;
        private System.Windows.Forms.CheckBox chkNoUse;
        private System.Windows.Forms.RadioButton optAttributeExec;
        private System.Windows.Forms.RadioButton optAttributeNext;
        private System.Windows.Forms.RadioButton optAttributeBack;
        private System.Windows.Forms.RadioButton optAfterContinue;
        private System.Windows.Forms.RadioButton optAfterEnd;
        private System.Windows.Forms.RadioButton optAfterMinimize;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdBrowse;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}
