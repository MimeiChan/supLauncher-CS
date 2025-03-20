using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace HiMenu
{
    public partial class FormHiMenu : Form
    {
        private CFunctions m_CFunctions = CFunctions.GetInstance();
        private CMenuPage m_CMenuPage = CMenuPage.GetInstance();
        private CMenuChain m_CMenuChain = CMenuChain.GetInstance();

        private List<Button> m_MenuButtons = new List<Button>();

        private int SaveCurrentButton; // マウス移動によるアイテムコメント表示中ボタンインデックス

        #region フォームオーバーライド関数

        /// <summary>
        /// キーボード入力
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            // メニュー項目間の矢印キーでの移動処理を実現する
            if (this.ActiveControl is Button)
            {
                int Index = m_MenuButtons.IndexOf((Button)this.ActiveControl);

                if (Index >= 0)
                {
                    switch (keyData)
                    {
                        case Keys.Left:
                            FormKeyEvent(Keys.Left, 0, Index);
                            keyData = Keys.None;
                            break;
                        case Keys.Right:
                            FormKeyEvent(Keys.Right, 0, Index);
                            keyData = Keys.None;
                            break;
                        case Keys.Up:
                        case Keys.Tab | Keys.Shift:
                            FormKeyEvent(Keys.Up, 0, Index);
                            keyData = Keys.None;
                            break;
                        case Keys.Down:
                        case Keys.Tab:
                            FormKeyEvent(Keys.Down, 0, Index);
                            keyData = Keys.None;
                            break;
                    }
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        #endregion

        #region フォームイベント

        private void FormHiMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (m_CMenuPage.MenuFileWrite())
            {
                case DialogResult.Cancel:
                    e.Cancel = true;
                    break;
            }
        }

        private void FormHiMenu_Load(object sender, EventArgs e)
        {
            // ちらつき防止
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            this.cmdTempFocus.Left = this.cmdTempFocus.Width * -1;

            string strCommandLine = "";
            string strPathName = "";
            string strFileName = "";

            if (Environment.GetCommandLineArgs().Length > 1)
            {
                strCommandLine = Environment.GetCommandLineArgs()[1];

                strFileName = Path.GetFileName(strCommandLine);
                strPathName = strCommandLine.Substring(0, strCommandLine.Length - strFileName.Length);

                if (strPathName.Length > 0)
                {
                    Directory.SetCurrentDirectory(strPathName);
                }
            }

            mnuFileMemberAbout.Text = Application.ProductName + " について...";

            m_CMenuPage.MenuForm = this;

            CreateMenuScreenMain(strFileName, true);
        }

        private void FormHiMenu_Shown(object sender, EventArgs e)
        {
            GoButton(0);
        }

        #endregion

        #region メニュー項目背景イベント

        private void picContainer_MouseDown(object sender, MouseEventArgs e)
        {
            // メインメニュー非表示の場合、Shiftキーを押下しながらマウス右クリックでメニュー再表示
            if (m_CMenuPage.MenuVisible == false)
            {
                if (Control.ModifierKeys == Keys.Shift)
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        ContextMenuStripFile.Show(this, e.Location);
                    }
                }
            }
        }

        #endregion

        #region フォームメニューイベント（ファイル）

        /// <summary>
        /// 「ファイル」フォームメニュー・サブメニュークリック（イベント）
        /// </summary>
        private void mnuFile_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (!(e.ClickedItem is ToolStripMenuItem))
                return;

            ToolStripMenuItem MenuItem = (ToolStripMenuItem)e.ClickedItem;

            if (e.ClickedItem.OwnerItem != null)
            {
                ToolStripMenuItem ParentMenu = (ToolStripMenuItem)sender;
                if (ParentMenu.Visible == false || ParentMenu.Enabled == false)
                    return;
                if (MenuItem.DropDownItems.Count == 0)
                {
                    ParentMenu.DropDown.Close();
                    this.Refresh();
                }
            }

            if (MenuItem == mnuFileMemberAbout || MenuItem == ToolStripMenuItemAbout)
            {
                using (var Dialog = new FormAbout())
                {
                    Dialog.ShowDialog(this);
                }
            }
            else if (MenuItem == mnuFileMemberOption || MenuItem == ToolStripMenuItemOption)
            {
                using (var Dialog = new FormConfiguration())
                {
                    if (Dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        m_CMenuPage.MenuItemsCountSet();
                        SetFormObject();
                        GoButton(0);
                    }
                }
            }
            else if (MenuItem == mnuFileMemberExit || MenuItem == ToolStripMenuItemExit)
            {
                this.Close();
            }
        }

        /// <summary>
        /// 「ファイル」「編集機能」フォームメニュー・サブメニュークリック（イベント）
        /// </summary>
        public void mnuFileMemberLockMode_DropDownItemClicked(object Sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripMenuItem MenuItem = (ToolStripMenuItem)e.ClickedItem;

            if (e.ClickedItem.OwnerItem != null)
            {
                ToolStripMenuItem ParentMenu = (ToolStripMenuItem)Sender;
                if (ParentMenu.Visible == false || ParentMenu.Enabled == false)
                    return;
                if (MenuItem.DropDownItems.Count == 0)
                {
                    ParentMenu.DropDown.Close();
                    this.Refresh();
                }
            }
            else
            {
                ((ContextMenuStrip)Sender).Hide();
            }

            using (var Dialog = new FormPassword())
            {
                if (MenuItem == mnuFileMemberLockOff || MenuItem == ToolStripMenuItemLockOff)
                {
                    if (m_CMenuPage.LockOn == true)
                    {
                        Dialog.SetMode = FormPassword.ModeDefs.SetToUnlock;
                        if (Dialog.ShowDialog(this) == DialogResult.OK)
                            m_CMenuPage.LockOn = false;
                    }
                }
                else if (MenuItem == mnuFileMemberLockOn || MenuItem == ToolStripMenuItemLockOn)
                {
                    if (m_CMenuPage.LockOn == false)
                    {
                        Dialog.SetMode = FormPassword.ModeDefs.SetToLock;
                        if (Dialog.ShowDialog(this) == DialogResult.OK)
                            m_CMenuPage.LockOn = true;
                    }
                }

                if (Dialog.DialogResult == DialogResult.OK)
                {
                    SetMenuCheckEdit();
                    SetMenuCheckMode();
                    SetFormObject();
                }
            }
        }

        #endregion

        #region フォームメニューイベント（編集）

        /// <summary>
        /// 「編集」フォームメニューのDropDownOpening（イベント）
        /// </summary>
        private void mnuEdit_DropDownOpening(object sender, EventArgs e)
        {
            int CurrentButton = m_MenuButtons.IndexOf((Button)this.ActiveControl);

            mnuEditMemberPaste.Enabled = (Clipboard.GetData("HiMenuItem") != null);

            mnuEditMemberHidden.Checked = m_CMenuPage[CurrentButton].NoUse;
        }

        /// <summary>
        /// 「編集」フォームメニューのDropDownClosed（イベント）
        /// </summary>
        private void mnuEdit_DropDownClosed(object sender, EventArgs e)
        {
            mnuEditMemberPaste.Enabled = true;
        }

        /// <summary>
        /// 「編集」フォームメニュー・サブメニュークリック（イベント）
        /// </summary>
        private void mnuEdit_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (!(e.ClickedItem is ToolStripMenuItem))
                return;

            ToolStripMenuItem MenuItem = (ToolStripMenuItem)e.ClickedItem;

            if (e.ClickedItem.OwnerItem != null)
            {
                ToolStripMenuItem ParentMenu = (ToolStripMenuItem)sender;
                if (ParentMenu.Visible == false || ParentMenu.Enabled == false)
                    return;
                if (MenuItem.DropDownItems.Count == 0)
                {
                    ParentMenu.DropDown.Close();
                    this.Refresh();
                }
            }
            else
            {
                ((ContextMenuStrip)sender).Hide();
            }

            int CurrentButton = m_MenuButtons.IndexOf((Button)this.ActiveControl);
            bool blnMove = true;

            if (MenuItem == ToolStripMenuItemCancel)
            {
                // No action
            }
            else if (MenuItem == mnuEditMemberCut || MenuItem == ToolStripMenuItemCut)
            {
                Clipboard.SetData("HiMenuItem", m_CMenuPage[CurrentButton]);
                m_CMenuPage[CurrentButton] = new CMenuPage.CMenuFileItemInf();
                SetFormObject();
            }
            else if (MenuItem == mnuEditMemberCopy || MenuItem == ToolStripMenuItemCopy)
            {
                Clipboard.SetData("HiMenuItem", m_CMenuPage[CurrentButton]);
                blnMove = false;
            }
            else if (MenuItem == mnuEditMemberPaste || MenuItem == ToolStripMenuItemPaste)
            {
                m_CMenuPage[CurrentButton] = (CMenuPage.CMenuFileItemInf)Clipboard.GetData("HiMenuItem");
                SetFormObject();
            }
            else if (MenuItem == mnuEditMemberDelete || MenuItem == ToolStripMenuItemDelete)
            {
                m_CMenuPage[CurrentButton] = new CMenuPage.CMenuFileItemInf();
                SetFormObject();
            }
            else if (MenuItem == mnuEditMemberHidden || MenuItem == ToolStripMenuItemHidden)
            {
                m_CMenuPage.MenuFileItemHiddenChanged();
                SetButtonDisp(CurrentButton, m_CMenuPage[CurrentButton].NoUse);
            }
            else if (MenuItem == mnuEditMemberEscape || MenuItem == ToolStripMenuItemEscape)
            {
                if (CurrentButton == m_CMenuPage.CancelButton)
                {
                    m_CMenuPage.CancelButton = -1;
                }
                else
                {
                    m_CMenuPage.CancelButton = CurrentButton;
                }
                SetFormObject();
            }

            if (blnMove)
                GoButton(CurrentButton);
        }

        #endregion

        #region メニュー項目ボタンイベント

        /// <summary>
        /// メニュー項目ボタンClick（イベント）
        /// </summary>
        private void cmdMenuButton_Click(object Sender, EventArgs eventArgs)
        {
            ButtonClick(m_MenuButtons.IndexOf((Button)Sender));
        }

        /// <summary>
        /// メニュー項目ボタンEnter（イベント）
        /// </summary>
        private void cmdMenuButton_Enter(object Sender, EventArgs eventArgs)
        {
            int Index = m_MenuButtons.IndexOf((Button)Sender);
            ToolStripStatusLabelGuide.Text = m_CMenuPage[Index].Comment;
            m_CMenuPage.CurrentButton = Index;
            SaveCurrentButton = Index;

            m_MenuButtons[Index].ForeColor = ColorTranslator.FromOle(m_CMenuPage.HighLightTextColor);
        }

        /// <summary>
        /// メニュー項目ボタンLeave（イベント）
        /// </summary>
        private void cmdMenuButton_Leave(object Sender, EventArgs eventArgs)
        {
            int Index = m_MenuButtons.IndexOf((Button)Sender);
            m_MenuButtons[Index].ForeColor = ColorTranslator.FromOle(m_CMenuPage.TextColor);
            this.Refresh();
        }

        /// <summary>
        /// メニュー項目ボタンMouseMove（イベント）
        /// </summary>
        private void cmdMenuButton_MouseMove(object Sender, MouseEventArgs eventArgs)
        {
            int Index = m_MenuButtons.IndexOf((Button)Sender);
            if (SaveCurrentButton != Index)
            {
                ToolStripStatusLabelGuide.Text = m_CMenuPage[Index].Comment;
                SaveCurrentButton = Index;
            }
        }

        /// <summary>
        /// メニュー項目ボタンMouseDown（イベント）
        /// </summary>
        private void cmdMenuButton_MouseDown(object Sender, MouseEventArgs eventArgs)
        {
            int Index = m_MenuButtons.IndexOf((Button)Sender);
            if (eventArgs.Button == MouseButtons.Right && m_CMenuPage.LockOn == false)
            {
                Point ShowPoint = new Point();
                Button button = m_MenuButtons[Index];
                button.Focus();
                ShowPoint.X += button.Left;
                ShowPoint.Y += button.Top;
                Control parent = button.Parent.Parent;
                ShowPoint.X += parent.Left;
                ShowPoint.Y += parent.Top;
                ShowPoint.X += eventArgs.Location.X;
                ShowPoint.Y += eventArgs.Location.Y;
                ContextMenuStripEdit.Show(this, ShowPoint);
            }
        }

        #endregion

        #region 表示関連

        /// <summary>
        /// メニュー画面作成
        /// </summary>
        /// <param name="FileName">オープンするメニューファイル名</param>
        private void CreateMenuScreenMain(string FileName, bool Starting = false)
        {
            // 現在のサイズを記憶
            int intOldWidth = this.Width;
            int intOldHeight = this.Height;

            // メニューファイル名を設定（未指定時、デフォルトセット）
            m_CMenuPage.MenuFileName = FileName;

            // メニューファイル読み込み
            m_CMenuPage.MenuFileRead();

            // メニューフォーム内のボタン等の設定
            SetFormObject();

            // メニューフォームの表示位置変更
            bool blnSetPos = false;
            int intNewPosX = 0;
            int intNewPosY = 0;
            Rectangle objCurrScreenArea = Screen.GetWorkingArea(this);
            
            if (m_CMenuChain.IsRootMenu() && Starting)
            {
                switch (m_CMenuPage.RootMenuDispPosition)
                {
                    case CMenuPage.MenuDispPosition.RootMenu:
                    case CMenuPage.MenuDispPosition.CurrentMenu:
                        blnSetPos = true;
                        intNewPosX = m_CMenuPage.CurrentX;
                        intNewPosY = m_CMenuPage.CurrentY;
                        break;
                    case CMenuPage.MenuDispPosition.ScreenCenter:
                        blnSetPos = true;
                        intNewPosX = (objCurrScreenArea.Width - this.Width) / 2;
                        intNewPosY = (objCurrScreenArea.Height - this.Height) / 2;
                        break;
                }
            }
            else
            {
                blnSetPos = true;
                switch (m_CMenuPage.RootMenuDispPosition)
                {
                    case CMenuPage.MenuDispPosition.RootMenu:
                        intNewPosX = this.Location.X + (intOldWidth - this.Width) / 2;
                        intNewPosY = this.Location.Y + (intOldHeight - this.Height) / 2;
                        break;
                    case CMenuPage.MenuDispPosition.CurrentMenu:
                        intNewPosX = m_CMenuPage.CurrentX;
                        intNewPosY = m_CMenuPage.CurrentY;
                        break;
                    case CMenuPage.MenuDispPosition.ScreenCenter:
                        intNewPosX = this.Location.X + (intOldWidth - this.Width) / 2;
                        intNewPosY = this.Location.Y + (intOldHeight - this.Height) / 2;
                        break;
                }
            }
            
            if (blnSetPos)
            {
                bool blnHitTopLeft = false;
                bool blnHitLeftBottom = false;
                bool blnHitTopRight = false;
                bool blnHitRightBottom = false;
                
                foreach (Screen item in Screen.AllScreens)
                {
                    Rectangle bounds = item.Bounds;
                    int intCheckX, intCheckY;
                    
                    intCheckY = this.Location.Y;
                    intCheckX = this.Location.X;
                    if (intCheckY >= bounds.Y && intCheckY <= bounds.Y + bounds.Height && intCheckX >= bounds.X && intCheckX <= bounds.X + bounds.Width)
                        blnHitTopLeft = true;
                    
                    intCheckY = this.Location.Y + this.Height;
                    intCheckX = this.Location.X;
                    if (intCheckY >= bounds.Y && intCheckY <= bounds.Y + bounds.Height && intCheckX >= bounds.X && intCheckX <= bounds.X + bounds.Width)
                        blnHitLeftBottom = true;
                    
                    intCheckY = this.Location.Y;
                    intCheckX = this.Location.X + this.Width;
                    if (intCheckY >= bounds.Y && intCheckY <= bounds.Y + bounds.Height && intCheckX >= bounds.X && intCheckX <= bounds.X + bounds.Width)
                        blnHitTopRight = true;
                    
                    intCheckY = this.Location.Y + this.Height;
                    intCheckX = this.Location.X + this.Width;
                    if (intCheckY >= bounds.Y && intCheckY <= bounds.Y + bounds.Height && intCheckX >= bounds.X && intCheckX <= bounds.X + bounds.Width)
                        blnHitRightBottom = true;
                }
                
                if (!blnHitTopLeft || !blnHitLeftBottom || !blnHitTopRight || !blnHitRightBottom)
                {
                    if (intNewPosX + this.Width > objCurrScreenArea.X + objCurrScreenArea.Width)
                    {
                        intNewPosX = objCurrScreenArea.X + objCurrScreenArea.Width - this.Width;
                    }
                    if (intNewPosX < objCurrScreenArea.X)
                    {
                        intNewPosX = objCurrScreenArea.X;
                    }
                    if (intNewPosY + this.Height > objCurrScreenArea.Y + objCurrScreenArea.Height)
                    {
                        intNewPosY = objCurrScreenArea.Y + objCurrScreenArea.Height - this.Height;
                    }
                    if (intNewPosY < objCurrScreenArea.Y)
                    {
                        intNewPosY = objCurrScreenArea.Y;
                    }
                }
                
                Point NewPos = new Point(intNewPosX, intNewPosY);
                this.Location = NewPos;
            }
        }

        /// <summary>
        /// フォーム上のオブジェクトのセット
        /// </summary>
        private void SetFormObject()
        {
            const int intMarginX = 15;
            const int intMarginY = 8;
            int intIndex;
            int intLeft;
            double dblTop;
            double dblWidth;
            double dblHeight;
            int intIndexRow;
            int intIndexCol;
            bool blnUseFlag;
            int intAreaX;
            int intAreaY;

            this.Cursor = Cursors.WaitCursor;

            // フォームｷｬﾌﾟｼｮﾝセット
            if (m_CMenuPage.MenuTitle.Length != 0)
            {
                this.Text = Application.ProductName + " - " + m_CMenuPage.MenuTitle;
            }
            else
            {
                this.Text = Application.ProductName;
            }

            // メニュー・ステータスバーの表示／非表示切り替え
            if (m_CMenuPage.LockOn)
            {
                mnuEdit.Visible = false;
            }
            else
            {
                mnuEdit.Visible = m_CMenuPage.MenuVisible && (m_CMenuPage.LockOn == false);
            }
            
            MainMenu1.Visible = m_CMenuPage.MenuVisible;
            StatusStripInformation.Visible = m_CMenuPage.StatusBarVisible;

            // ﾌｫｰﾑｻｲｽﾞ調整
            this.Width = m_CMenuPage.MenuWidth;
            this.Height = m_CMenuPage.MenuHeight;

            // フォームのレイアウトロジックを中断する
            this.Refresh();
            this.SuspendLayout();

            intAreaX = pnlContainer.Width - (intMarginX * 2) - 4;
            intAreaY = pnlContainer.Height - (intMarginY * 2) - 4;

            // 書き換えを隠す
            pnlContainer.Visible = false;
            pnlContainer.Font = m_CMenuPage.Font;

            SetMenuCheckEdit();
            SetMenuCheckMode();

            // 背景画像の読み込み
            if (m_CMenuPage.BGFile != "")
            {
                string strBGFIle = m_CFunctions.GetDriveFolderFile(m_CMenuPage.BGFile, CFunctions.GetDriveFolderFileMode.PathName);
                if (File.Exists(strBGFIle))
                {
                    picContainer.BackgroundImage = Image.FromFile(strBGFIle);
                    picContainer.BackgroundImageLayout = m_CMenuPage.BGTile ? ImageLayout.Tile : ImageLayout.None;
                }
                else
                {
                    picContainer.BackgroundImage = null;
                    MessageBox.Show("指定された背景ファイルが見つかりません", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                picContainer.BackgroundImage = null;
            }

            // ボタンコントロールを増減
            int intNowButtonCount = m_MenuButtons.Count;
            int intNowItemCount = m_CMenuPage.MenuRows * m_CMenuPage.MenuCols;

            // コントロールと関連イベントハンドラの増減
            if (intNowButtonCount < intNowItemCount)
            {
                for (int intLoop = intNowButtonCount; intLoop < intNowItemCount; intLoop++)
                {
                    Button NewButton = new Button();
                    picContainer.Controls.Add(NewButton);
                    NewButton.Visible = true;
                    NewButton.TabStop = false;
                    NewButton.Margin = new Padding(0);
                    
                    NewButton.Click += cmdMenuButton_Click;
                    NewButton.Enter += cmdMenuButton_Enter;
                    NewButton.Leave += cmdMenuButton_Leave;
                    NewButton.MouseMove += cmdMenuButton_MouseMove;
                    NewButton.MouseDown += cmdMenuButton_MouseDown;
                    
                    m_MenuButtons.Add(NewButton);
                }
            }
            else if (intNowButtonCount > intNowItemCount)
            {
                for (int intLoop = intNowButtonCount - 1; intLoop >= intNowItemCount; intLoop--)
                {
                    Button button = m_MenuButtons[intLoop];
                    
                    button.Click -= cmdMenuButton_Click;
                    button.Enter -= cmdMenuButton_Enter;
                    button.Leave -= cmdMenuButton_Leave;
                    button.MouseMove -= cmdMenuButton_MouseMove;
                    button.MouseDown -= cmdMenuButton_MouseDown;
                    
                    picContainer.Controls.Remove(button);
                    button.Dispose();
                    m_MenuButtons.RemoveAt(intLoop);
                }
            }

            // コントロール幅・高さを設定
            dblWidth = intAreaX / m_CMenuPage.MenuCols;
            dblHeight = intAreaY / m_CMenuPage.MenuRows;
            if (m_CMenuPage.MenuCols > 1)
                dblWidth = dblWidth - Math.Floor(8 * (m_CMenuPage.MenuCols - 1) / (double)m_CMenuPage.MenuCols);

            intIndex = 0;

            for (intIndexCol = 0; intIndexCol < m_CMenuPage.MenuCols; intIndexCol++)
            {
                intLeft = Convert.ToInt32(intMarginX + (dblWidth + 8) * intIndexCol);
                dblTop = intMarginY;

                for (intIndexRow = 0; intIndexRow < m_CMenuPage.MenuRows; intIndexRow++)
                {
                    if (m_CMenuPage.LockOn && m_CMenuPage[intIndex].NoUse)
                    {
                        blnUseFlag = false;
                    }
                    else
                    {
                        blnUseFlag = true;
                    }

                    Button button = m_MenuButtons[intIndex];
                    
                    button.SetBounds(intLeft, Convert.ToInt32(dblTop), Convert.ToInt32(dblWidth), Convert.ToInt32(dblHeight));
                    
                    button.ForeColor = ColorTranslator.FromOle(m_CMenuPage.TextColor);
                    
                    if (ColorTranslator.ToOle(SystemColors.Control) == m_CMenuPage.ButtonColor)
                    {
                        button.BackColor = SystemColors.Control;
                        button.UseVisualStyleBackColor = true;
                    }
                    else
                    {
                        button.BackColor = ColorTranslator.FromOle(m_CMenuPage.ButtonColor);
                        button.UseVisualStyleBackColor = false;
                    }
                    
                    button.Text = m_CMenuPage[intIndex].Title;
                    
                    if (m_CMenuPage.LockOn == false)
                    {
                        if (m_CMenuPage.CancelButton == intIndex)
                        {
                            button.Text = "＜ＥＳＣ＞" + button.Text;
                        }
                        
                        if (m_CMenuPage[intIndex].NoUse)
                        {
                            button.Text = "＜非表示＞" + button.Text;
                        }
                    }
                    
                    button.Font = m_CMenuPage.Font;
                    
                    button.Enabled = blnUseFlag;
                    button.Visible = blnUseFlag;

                    dblTop = dblTop + dblHeight;
                    
                    intIndex++;
                }
            }

            // キャンセルボタンの設定
            this.CancelButton = null;
            if (m_CMenuPage.CancelButton != -1 && intNowItemCount > m_CMenuPage.CancelButton && m_CMenuPage.LockOn)
            {
                this.CancelButton = m_MenuButtons[intIndex - 1];
            }
            
            // 背景色の設定（pnlContainerとpicContainerの両方に設定）
            pnlContainer.BackColor = ColorTranslator.FromOle(m_CMenuPage.BackColor);
            picContainer.BackColor = ColorTranslator.FromOle(m_CMenuPage.BackColor);
            
            // 隠していたボタンを再表示
            pnlContainer.Visible = true;
            
            this.Cursor = Cursors.Default;

            // フォームのレイアウトロジックを再開する
            this.Refresh();
            this.ResumeLayout(true);
        }

        /// <summary>
        /// メインフォームのメニュー／ステータスバーの設定（編集可・不可の切り替え）
        /// </summary>
        private void SetMenuCheckEdit()
        {
            if (m_CMenuPage.LockOn)
            {
                mnuFileMemberLockOff.Checked = false;
                mnuFileMemberLockOn.Checked = true;
                mnuFileMemberOption.Enabled = false;
                ToolStripMenuItemLockOff.Checked = false;
                ToolStripMenuItemLockOn.Checked = true;
                ToolStripMenuItemOption.Enabled = false;
                ToolStripStatusLabelMode.Visible = false;
            }
            else
            {
                mnuFileMemberLockOff.Checked = true;
                mnuFileMemberLockOn.Checked = false;
                mnuFileMemberOption.Enabled = true;
                ToolStripMenuItemLockOff.Checked = true;
                ToolStripMenuItemLockOn.Checked = false;
                ToolStripMenuItemOption.Enabled = true;
                ToolStripStatusLabelMode.Visible = true;
            }
        }

        /// <summary>
        /// メインフォームのメニュー／ステータスバーの設定（モードの切り替え）
        /// </summary>
        private void SetMenuCheckMode()
        {
            switch (m_CMenuPage.LockOn)
            {
                case true:
                    ToolStripStatusLabelMode.Text = "";
                    break;
                case false:
                    ToolStripStatusLabelMode.Text = "編集モード";
                    break;
            }
        }

        /// <summary>
        /// メニューボタンの表示／非表示の切り替え
        /// </summary>
        private void SetButtonDisp(int intIndex, bool blnMode)
        {
            if (blnMode)
            {
                // ボタン表示
                if (m_CMenuPage[intIndex].NoUse)
                {
                    m_CMenuPage[intIndex].NoUse = false;
                }
                else
                {
                    return;
                }
            }
            else
            {
                // ボタン非表示
                if (m_CMenuPage[intIndex].NoUse == false)
                {
                    m_CMenuPage[intIndex].NoUse = true;
                }
                else
                {
                    return;
                }
            }

            // 変更があったボタンのキャプションを表示
            SetFormObject();
        }

        #endregion

        #region メニュー項目実行関連

        /// <summary>
        /// メニューボタンクリック時の処理
        /// </summary>
        private void ButtonClick(int intIndex)
        {
            ToolStripStatusLabelGuide.Text = m_CMenuPage[intIndex].Comment;

            // 動作モードチェック（編集モード／実行モード／移動モード）
            switch (m_CMenuPage.LockOn)
            {
                case true: // 実行モード
                    ButtonExec(intIndex);
                    break;
                case false: // 編集モード
                    ButtonEdit(intIndex);
                    break;
            }
        }

        /// <summary>
        /// メニューボタンクリック時の、編集モードの処理
        /// </summary>
        private void ButtonEdit(int Index)
        {
            using (var frmDialog = new FormButtonEdit())
            {
                if (frmDialog.ShowDialog(this) == DialogResult.OK)
                {
                    SetFormObject();
                    GoButton(Index);
                }
            }
        }

        /// <summary>
        /// メニューボタンクリック時の、実行モードの処理
        /// </summary>
        private void ButtonExec(int Index)
        {
            int intErrorNo = 0;
            string strErrorStr = "";

            string strCommandLine = "";
            string strCurrentPath = "";
            int lngExec = 0;
            string strFileNameSave = "";
            string strFileNameRet = "";
            string strPathName = "";
            string strFileName = "";
            string strOption = "";
            string strNextMenuPath = "";
            int intRevIndex = 0;

            CMenuPage.CMenuFileItemInf item = m_CMenuPage[Index];

            if (item.Title.Length != 0)
            {
                if (item.Attribute == CMenuPage.CMenuFileItemInf.ItemAttribute.BackPrevMenu)
                {
                    // 前のメニューに戻る（１つ前のメニュー定義ファイル名を入手）
                    if (m_CMenuPage.MenuFileWrite() == DialogResult.Cancel)
                    {
                        return;
                    }
                    intRevIndex = m_CMenuChain.Pop();
                    strCommandLine = m_CFunctions.QuateFullPath(m_CMenuPage.MenuFileName);
                }
                else
                {
                    strCommandLine = item.Command; // 実行するコマンド
                    intRevIndex = Index; // 現在のインデックスを保存
                }

                // ドライブ名＋フォルダ名＋ファイル名を取り出し
                strFileNameSave = m_CFunctions.GetDriveFolderFile(strCommandLine, CFunctions.GetDriveFolderFileMode.PathName);

                // パス名・ファイル名を取り出し
                strPathName = m_CFunctions.GetPathAndFile(strFileNameSave, CFunctions.GetPathAndFileMode.PathName);
                strFileName = m_CFunctions.GetPathAndFile(strFileNameSave, CFunctions.GetPathAndFileMode.FileName);

                // オプション取り出し
                strOption = m_CFunctions.GetDriveFolderFile(strCommandLine, CFunctions.GetDriveFolderFileMode.CommandLineOption);

                try
                {
                    if (strCommandLine.Length != 0 || item.Attribute == CMenuPage.CMenuFileItemInf.ItemAttribute.BackPrevMenu)
                    {
                        strCurrentPath = Directory.GetCurrentDirectory(); // カレントパスを保存

                        switch (item.Attribute)
                        {
                            case CMenuPage.CMenuFileItemInf.ItemAttribute.ExecApplication:
                                //------------------------------------------------------------
                                // プログラム起動
                                //------------------------------------------------------------
                                if (strPathName.Length != 0 && string.Compare(strPathName, strCurrentPath, true) != 0)
                                {
                                    intErrorNo = 1; // エラー？
                                    strErrorStr = strPathName;
                                    Directory.SetCurrentDirectory(strPathName); // 実行フォルダを変更（変更できていないときエラー発生）
                                }
                                else
                                {
                                    strCurrentPath = "";
                                }

                                intErrorNo = 2; // エラー？
                                strErrorStr = strFileName;

                                using (var Process = new Process())
                                {
                                    Process.StartInfo.Arguments = strOption.TrimEnd(); // 引数
                                    Process.StartInfo.WorkingDirectory = Path.GetDirectoryName(strFileName); // 作業ディレクトリ
                                    Process.StartInfo.FileName = strFileName; // 実行するファイル (*.exeでなくても良い)
                                    Process.Start();
                                }

                                intErrorNo = 0;

                                if (item.After == CMenuPage.CMenuFileItemInf.ItemAfter.EndHiMenu)
                                { // 実行後、終了
                                    this.Close();
                                    return;
                                }

                                if (strCurrentPath.Length != 0)
                                { // カレントフォルダを元にもどす
                                    Directory.SetCurrentDirectory(strCurrentPath);
                                }

                                this.Refresh();

                                if (item.After == CMenuPage.CMenuFileItemInf.ItemAfter.MinimizedHiMenu)
                                { // 実行後、最小化
                                    this.WindowState = FormWindowState.Minimized;
                                }
                                break;

                            case CMenuPage.CMenuFileItemInf.ItemAttribute.BackPrevMenu:
                            case CMenuPage.CMenuFileItemInf.ItemAttribute.OpenNextMenu:
                                //------------------------------------------------------------
                                // 次／前のメニュー処理
                                //------------------------------------------------------------
                                if (item.Attribute == CMenuPage.CMenuFileItemInf.ItemAttribute.OpenNextMenu)
                                {
                                    if (m_CMenuPage.MenuFileWrite() == DialogResult.Cancel)
                                    {
                                        return;
                                    }
                                }

                                if (strPathName.Length != 0 && string.Compare(strPathName, strCurrentPath, true) != 0)
                                {
                                    intErrorNo = 1; // エラー？
                                    strErrorStr = strPathName;
                                    Directory.SetCurrentDirectory(strPathName); // 実行フォルダを変更（変更できていないときエラー発生）
                                }
                                else
                                {
                                    strCurrentPath = "";
                                }

                                intErrorNo = 0;

                                // 新しいメニューで再表示
                                strNextMenuPath = Directory.GetCurrentDirectory();

                                if (!strNextMenuPath.EndsWith("\\"))
                                {
                                    strNextMenuPath = strNextMenuPath + "\\";
                                }

                                if (item.Attribute == CMenuPage.CMenuFileItemInf.ItemAttribute.OpenNextMenu)
                                {
                                    // 次のメニューに進む
                                    m_CMenuChain.Push(intRevIndex);
                                }

                                CreateMenuScreenMain(strNextMenuPath + strFileName);

                                if (item.Attribute == CMenuPage.CMenuFileItemInf.ItemAttribute.BackPrevMenu)
                                {
                                    GoButton(intRevIndex);
                                }
                                else
                                {
                                    GoButton(0);
                                }
                                break;
                        }
                    }
                    else
                    {
                        // 終了のみ
                        if (item.After == CMenuPage.CMenuFileItemInf.ItemAfter.EndHiMenu)
                        {
                            this.Close();
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    switch (intErrorNo)
                    {
                        case 1:
                            MessageBox.Show("カレントパスの変更に失敗しました\r\n\r\n" + strErrorStr, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            if (strCurrentPath.Length != 0 && string.Compare(strCurrentPath, strErrorStr, true) != 0)
                            {
                                Directory.SetCurrentDirectory(strCurrentPath); // カレントパスを元にもどす
                            }
                            break;

                        case 2:
                            MessageBox.Show("アプリケーションの起動に失敗しました\r\n\r\n" + strErrorStr, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            if (strCurrentPath.Length != 0)
                            {
                                Directory.SetCurrentDirectory(strCurrentPath); // カレントパスを元にもどす
                            }
                            break;

                        default:
                            throw ex;
                    }
                }
            }
        }

        #endregion

        #region 制御関連

        /// <summary>
        /// メインフォームのキーイベント
        /// </summary>
        private void FormKeyEvent(Keys KeyCode, int Shift, int intIndex)
        {
            int intSaveIndex;
            int intNowRow;
            int intNowCol;
            int intLoop;
            bool blnFlag;

            intSaveIndex = intIndex;

            if (KeyCode == Keys.Left || KeyCode == Keys.NumPad4)
            { // 左へ
                intNowCol = Convert.ToInt32(Math.Floor(intSaveIndex / (double)m_CMenuPage.MenuRows));
                intNowRow = intSaveIndex - intNowCol * m_CMenuPage.MenuRows;
                if (intNowCol <= 0)
                {
                    intNowCol = m_CMenuPage.MenuCols - 1;
                }
                else
                {
                    intNowCol = intNowCol - 1;
                }
                GoButton(intNowRow + intNowCol * m_CMenuPage.MenuRows, false);
            }
            else if (KeyCode == Keys.Right || KeyCode == Keys.NumPad6)
            { // 右へ
                intNowCol = Convert.ToInt32(Math.Floor(intSaveIndex / (double)m_CMenuPage.MenuRows));
                intNowRow = intSaveIndex - intNowCol * m_CMenuPage.MenuRows;
                if (intNowCol >= m_CMenuPage.MenuCols - 1)
                {
                    intNowCol = 0;
                }
                else
                {
                    intNowCol = intNowCol + 1;
                }
                GoButton(intNowRow + intNowCol * m_CMenuPage.MenuRows);
            }
            else if (KeyCode == Keys.Up || KeyCode == Keys.NumPad8)
            { // 上へ
                blnFlag = false;
                for (intLoop = intIndex - 1; intLoop >= 0; intLoop--)
                {
                    if (m_MenuButtons[intLoop].Visible)
                    {
                        GoButton(intLoop, false);
                        blnFlag = true;
                        break;
                    }
                }
                if (blnFlag == false)
                {
                    GoButton((m_CMenuPage.MenuCols * m_CMenuPage.MenuRows) - 1, false);
                }
            }
            else if (KeyCode == Keys.Down || KeyCode == Keys.NumPad2)
            { // 下へ
                blnFlag = false;
                for (intLoop = intIndex + 1; intLoop < (m_CMenuPage.MenuRows * m_CMenuPage.MenuCols); intLoop++)
                {
                    if (m_MenuButtons[intLoop].Visible)
                    {
                        GoButton(intLoop);
                        blnFlag = true;
                        break;
                    }
                }
                if (blnFlag == false)
                {
                    GoButton(0);
                }
            }
            else
            {
                if (m_CMenuPage.LockOn == false)
                {
                    if (KeyCode == Keys.Delete)
                    { // ボタン非表示
                        SetButtonDisp(intIndex, false);
                        GoButton(intIndex);
                    }
                    else if (KeyCode == Keys.Insert)
                    { // ボタン表示
                        SetButtonDisp(intIndex, true);
                        GoButton(intIndex);
                    }
                }
            }
        }

        /// <summary>
        /// メニュー間のセットフォーカス
        /// </summary>
        private void GoButton(int intIndex, bool valFlag = false)
        {
            int intLoop;
            Control objControl = this.ActiveControl;

            cmdTempFocus.Focus();

            if (valFlag)
            {
                for (intLoop = intIndex; intLoop < (m_CMenuPage.MenuRows * m_CMenuPage.MenuCols); intLoop++)
                {
                    if (m_MenuButtons[intLoop].Visible)
                    {
                        m_MenuButtons[intLoop].Focus();
                        return;
                    }
                }
                for (intLoop = 0; intLoop <= intIndex; intLoop++)
                {
                    if (m_MenuButtons[intLoop].Visible)
                    {
                        m_MenuButtons[intLoop].Focus();
                        return;
                    }
                }
            }
            else
            {
                for (intLoop = intIndex; intLoop >= 0; intLoop--)
                {
                    if (m_MenuButtons[intLoop].Visible)
                    {
                        m_MenuButtons[intLoop].Focus();
                        return;
                    }
                }
                for (intLoop = (m_CMenuPage.MenuRows * m_CMenuPage.MenuCols) - 1; intLoop >= intIndex; intLoop--)
                {
                    if (m_MenuButtons[intLoop].Visible)
                    {
                        m_MenuButtons[intLoop].Focus();
                        return;
                    }
                }
            }

            objControl.Focus();
        }

        #endregion

        public FormHiMenu()
        {
            InitializeComponent();

            // picContainerのMouseDownイベントハンドラを追加
            this.picContainer.MouseDown += new MouseEventHandler(this.picContainer_MouseDown);

            // mnuFileのDropDownItemClickedイベントハンドラを追加
            this.mnuFile.DropDownItemClicked += new ToolStripItemClickedEventHandler(this.mnuFile_DropDownItemClicked);
            this.ContextMenuStripFile.ItemClicked += new ToolStripItemClickedEventHandler(this.mnuFile_DropDownItemClicked);

            // mnuFileMemberLockModeのDropDownItemClickedイベントハンドラを追加
            this.mnuFileMemberLockMode.DropDownItemClicked += new ToolStripItemClickedEventHandler(this.mnuFileMemberLockMode_DropDownItemClicked);
            this.ToolStripMenuItemLockMode.DropDownItemClicked += new ToolStripItemClickedEventHandler(this.mnuFileMemberLockMode_DropDownItemClicked);

            // mnuEditのイベントハンドラを追加
            this.mnuEdit.DropDownOpening += new EventHandler(this.mnuEdit_DropDownOpening);
            this.mnuEdit.DropDownClosed += new EventHandler(this.mnuEdit_DropDownClosed);
            this.mnuEdit.DropDownItemClicked += new ToolStripItemClickedEventHandler(this.mnuEdit_DropDownItemClicked);
            this.ContextMenuStripEdit.ItemClicked += new ToolStripItemClickedEventHandler(this.mnuEdit_DropDownItemClicked);
        }
    }
}