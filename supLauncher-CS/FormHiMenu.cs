using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace HiMenu
{
    public partial class FormHiMenu : Form
    {
        private CFunctions m_CFunctions = CFunctions.GetInstance();
        private CMenuPage m_CMenuPage = CMenuPage.GetInstance();
        private CMenuChain m_CMenuChain = CMenuChain.GetInstance();

        private List<Button> m_MenuButtons = new List<Button>();

        // XML関連メニュー項目
        private ToolStripMenuItem mnuFileMemberNewXml;
        private ToolStripMenuItem mnuFileMemberOpenXml;

        // WebView2関連
        private WebView2 webView;
        private WebViewBridge webViewBridge;
        private bool useWebView = true; // WebViewモードの切り替えフラグ

        private int SaveCurrentButton; // マウス移動によるアイテムコメント表示中ボタンインデックス

        #region フォームオーバーライド関数

        /// <summary>
        /// キーボード入力
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            // WebViewモードの場合はキー処理をスキップ
            if (useWebView)
                return base.ProcessDialogKey(keyData);

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

        private async void FormHiMenu_Load(object sender, EventArgs e)
        {
            // XML関連メニュー項目の追加
            AddXmlMenuItems();
            
            // WebView2/従来UIモード切り替えメニューの追加
            AddUIModeSwitchMenuItem();

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

            // WebView2の初期化
            if (useWebView)
            {
                await InitializeWebView();
            }

            CreateMenuScreenMain(strFileName, true);
        }

        private void FormHiMenu_Shown(object sender, EventArgs e)
        {
            if (!useWebView)
            {
                GoButton(0);
            }
        }

        #endregion

        #region WebView2関連

        /// <summary>
        /// WebView2コントロールを初期化する
        /// </summary>
        private async Task InitializeWebView()
        {
            try
            {
                // WebView2の作成と初期化
                webView = new WebView2();
                webView.Dock = DockStyle.Fill;
                picContainer.Controls.Add(webView);

                // WebView2コアの初期化を待機
                await webView.EnsureCoreWebView2Async();

                // WebView2とC#間のブリッジを設定
                webViewBridge = new WebViewBridge(m_CMenuPage);
                // COMオブジェクトとしてスクリプトに追加
                webView.CoreWebView2.AddHostObjectToScript("menuData", webViewBridge);

                // メッセージハンドラを登録
                webView.WebMessageReceived += WebView_WebMessageReceived;

                // ローカルWebUIを読み込む
                string baseDirectory = Path.GetDirectoryName(Application.ExecutablePath);
                string webUiPath = Path.Combine(baseDirectory, "WebUI");
                
                // WebUIフォルダが存在しない場合は作成
                if (!Directory.Exists(webUiPath))
                {
                    Directory.CreateDirectory(webUiPath);
                    // HTMLとCSSファイルのコピー処理を追加する必要があります
                }

                // HTML/CSS/JSファイルをコピー
                string htmlContent = GetEmbeddedResource("menu.html");
                string cssContent = GetEmbeddedResource("styles.css");
                string jsContent = GetEmbeddedResource("menu.js");

                if (!string.IsNullOrEmpty(htmlContent))
                {
                    File.WriteAllText(Path.Combine(webUiPath, "menu.html"), htmlContent);
                }
                
                if (!string.IsNullOrEmpty(cssContent))
                {
                    File.WriteAllText(Path.Combine(webUiPath, "styles.css"), cssContent);
                }
                
                if (!string.IsNullOrEmpty(jsContent))
                {
                    File.WriteAllText(Path.Combine(webUiPath, "menu.js"), jsContent);
                }

                // WebUIを読み込む
                string htmlPath = Path.Combine(webUiPath, "menu.html");
                if (File.Exists(htmlPath))
                {
                    webView.CoreWebView2.Navigate("file:///" + htmlPath);
                }
                else
                {
                    // HTML直接インライン読み込み（ファイルが見つからない場合のフォールバック）
                    webView.CoreWebView2.NavigateToString(@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <meta charset='UTF-8'>
                            <style>
                                body { font-family: 'Segoe UI', sans-serif; margin: 0; padding: 20px; background-color: #f0f0f0; }
                                .menu-container { display: grid; grid-template-columns: repeat(auto-fill, minmax(150px, 1fr)); gap: 10px; }
                                .menu-button { background-color: #4361ee; color: white; border: none; border-radius: 8px; padding: 15px; cursor: pointer; }
                                .menu-button:hover { background-color: #3a56d4; }
                            </style>
                        </head>
                        <body>
                            <div id='menu-container' class='menu-container'></div>
                            <script>
                                document.addEventListener('DOMContentLoaded', async function() {
                                    try {
                                        // JSON文字列として取得してからパース
                                        const menuItemsJson = await window.chrome.webview.hostObjects.menuData.getMenuItemsJson();
                                        const menuItems = JSON.parse(menuItemsJson);
                                        
                                        // 配列として直接取得する別の方法
                                        // const menuItems = await window.chrome.webview.hostObjects.menuData.getMenuItems();
                                        
                                        const container = document.getElementById('menu-container');
                                        
                                        for (let i = 0; i < menuItems.length; i++) {
                                            const item = menuItems[i];
                                            if (!item.noUse) {
                                                const button = document.createElement('button');
                                                button.className = 'menu-button';
                                                button.textContent = item.title;
                                                button.addEventListener('click', function() {
                                                    window.chrome.webview.postMessage({
                                                        eventType: 'buttonClick',
                                                        buttonIndex: i
                                                    });
                                                });
                                                container.appendChild(button);
                                            }
                                        }
                                    } catch (error) {
                                        console.error('Error:', error);
                                    }
                                });
                            </script>
                        </body>
                        </html>
                    ");
                }
            }
            catch (Exception ex)
            {
                // WebView2の初期化に失敗した場合は従来のUIに戻す
                MessageBox.Show("WebView2の初期化に失敗しました。従来のUIに戻します。\n\n" + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                useWebView = false;
                webView = null;
            }
        }

        /// <summary>
        /// リソースファイルから内容を取得（開発時はファイルからそのまま読み込む）
        /// </summary>
        private string GetEmbeddedResource(string filename)
        {
            try
            {
                // 開発中はプロジェクトフォルダ内のファイルを直接使用
                string devPath = Path.Combine(Application.StartupPath, "WebUI", filename);
                if (File.Exists(devPath))
                {
                    return File.ReadAllText(devPath);
                }

                // 埋め込みリソースからの読み込み（ビルド後）
                // ※実際のビルド設定で調整が必要
                return "";
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// WebViewからのメッセージを処理するイベントハンドラ
        /// </summary>
        private void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string jsonMessage = e.WebMessageAsJson;
                WebViewMessage message = JsonSerializer.Deserialize<WebViewMessage>(jsonMessage, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                switch (message.EventType)
                {
                    case "buttonClick":
                        // ボタンクリックを処理
                        ButtonClick(message.ButtonIndex);
                        break;

                    case "buttonHover":
                        // ホバー状態を処理
                        ToolStripStatusLabelGuide.Text = m_CMenuPage[message.ButtonIndex].Comment;
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WebMessageReceived Error: " + ex.Message);
            }
        }

        /// <summary>
        /// WebViewのメニュー表示を更新する
        /// </summary>
        private async Task RefreshWebViewMenu()
        {
            if (webView != null && webView.CoreWebView2 != null)
            {
                try
                {
                    // JS側にメニュー再読み込み命令を送信
                    await webView.CoreWebView2.ExecuteScriptAsync("loadMenuItems()");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("RefreshWebViewMenu Error: " + ex.Message);
                }
            }
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
                        
                        if (!useWebView)
                        {
                            GoButton(0);
                        }
                    }
                }
            }
            else if (MenuItem == mnuFileMemberExit || MenuItem == ToolStripMenuItemExit)
            {
                this.Close();
            }
            else if (MenuItem == mnuFileMemberNewXml)
            {
                using (SaveFileDialog dlg = new SaveFileDialog())
                {
                    dlg.Filter = "XML定義ファイル (*.xml)|*.xml|すべてのファイル (*.*)|*.*";
                    dlg.Title = "新規XMLファイルの保存先";
                    dlg.DefaultExt = "xml";
                    
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        if (m_CMenuPage.MenuFileWrite() == DialogResult.Cancel)
                        {
                            return;
                        }
                        
                        m_CMenuPage.MenuFileName = dlg.FileName;
                        m_CMenuPage.Initalize();
                        SetFormObject();
                        
                        if (!useWebView)
                        {
                            GoButton(0);
                        }
                    }
                }
            }
            else if (MenuItem == mnuFileMemberOpenXml)
            {
                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Filter = "XML定義ファイル (*.xml)|*.xml|すべてのファイル (*.*)|*.*";
                    dlg.Title = "XMLファイルを開く";
                    
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        if (m_CMenuPage.MenuFileWrite() == DialogResult.Cancel)
                        {
                            return;
                        }
                        
                        CreateMenuScreenMain(dlg.FileName);
                        
                        if (!useWebView)
                        {
                            GoButton(0);
                        }
                    }
                }
            }
            else if (MenuItem.Text.StartsWith("モダンUI"))
            {
                if (!useWebView)
                {
                    // 従来UIからモダンUIに切り替え
                    useWebView = true;
                    MenuItem.Text = "従来のUIに切り替え(&W)";
                    HideTraditionalButtons();
                    InitializeWebView().GetAwaiter();
                }
                else
                {
                    // モダンUIから従来UIに切り替え
                    useWebView = false;
                    MenuItem.Text = "モダンUIに切り替え(&W)";
                    if (webView != null)
                    {
                        picContainer.Controls.Remove(webView);
                        webView.Dispose();
                        webView = null;
                    }
                    SetFormObject();
                    GoButton(0);
                }
            }
        }

        /// <summary>
        /// モダンUI/従来UI切り替えメニューの追加
        /// </summary>
        private void AddUIModeSwitchMenuItem()
        {
            // UIモード切り替えメニュー項目
            var uiModeMenuItem = new ToolStripMenuItem(useWebView ? "従来のUIに切り替え(&W)" : "モダンUIに切り替え(&W)");
            
            // メニューの「表示」セクションに追加
            int insertIndex = mnuFile.DropDownItems.IndexOf(mnuFileMemberOption);
            if (insertIndex >= 0)
            {
                mnuFile.DropDownItems.Insert(insertIndex + 1, new ToolStripSeparator());
                mnuFile.DropDownItems.Insert(insertIndex + 2, uiModeMenuItem);
            }
            else
            {
                mnuFile.DropDownItems.Add(new ToolStripSeparator());
                mnuFile.DropDownItems.Add(uiModeMenuItem);
            }
        }

        /// <summary>
        /// 従来のボタンを非表示にする
        /// </summary>
        private void HideTraditionalButtons()
        {
            foreach (Button button in m_MenuButtons)
            {
                button.Visible = false;
            }
        }

        /// <summary>
        /// 「ファイル」「編集機能」フォームメニュー・サブメニュークリック（イベント）
        /// </summary>
        public async void mnuFileMemberLockMode_DropDownItemClicked(object Sender, ToolStripItemClickedEventArgs e)
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
                    
                    if (useWebView)
                    {
                        await RefreshWebViewMenu();
                    }
                    else
                    {
                        SetFormObject();
                    }
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
            // WebViewモードではメニュー項目を無効化
            if (useWebView)
            {
                foreach (ToolStripItem item in mnuEdit.DropDownItems)
                {
                    item.Enabled = false;
                }
                return;
            }

            int CurrentButton = m_MenuButtons.IndexOf((Button)this.ActiveControl);

            mnuEditMemberPaste.Enabled = (Clipboard.GetData("HiMenuItem") != null);

            mnuEditMemberHidden.Checked = m_CMenuPage[CurrentButton].NoUse;
        }

        /// <summary>
        /// 「編集」フォームメニューのDropDownClosed（イベント）
        /// </summary>
        private void mnuEdit_DropDownClosed(object sender, EventArgs e)
        {
            if (!useWebView)
            {
                mnuEditMemberPaste.Enabled = true;
            }
        }

        /// <summary>
        /// 「編集」フォームメニュー・サブメニュークリック（イベント）
        /// </summary>
        private async void mnuEdit_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (useWebView || !(e.ClickedItem is ToolStripMenuItem))
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
                if (useWebView)
                {
                    await RefreshWebViewMenu();
                }
                else
                {
                    SetFormObject();
                }
            }
            else if (MenuItem == mnuEditMemberCopy || MenuItem == ToolStripMenuItemCopy)
            {
                Clipboard.SetData("HiMenuItem", m_CMenuPage[CurrentButton]);
                blnMove = false;
            }
            else if (MenuItem == mnuEditMemberPaste || MenuItem == ToolStripMenuItemPaste)
            {
                m_CMenuPage[CurrentButton] = (CMenuPage.CMenuFileItemInf)Clipboard.GetData("HiMenuItem");
                if (useWebView)
                {
                    await RefreshWebViewMenu();
                }
                else
                {
                    SetFormObject();
                }
            }
            else if (MenuItem == mnuEditMemberDelete || MenuItem == ToolStripMenuItemDelete)
            {
                m_CMenuPage[CurrentButton] = new CMenuPage.CMenuFileItemInf();
                if (useWebView)
                {
                    await RefreshWebViewMenu();
                }
                else
                {
                    SetFormObject();
                }
            }
            else if (MenuItem == mnuEditMemberHidden || MenuItem == ToolStripMenuItemHidden)
            {
                m_CMenuPage.MenuFileItemHiddenChanged();
                if (useWebView)
                {
                    await RefreshWebViewMenu();
                }
                else
                {
                    SetButtonDisp(CurrentButton, m_CMenuPage[CurrentButton].NoUse);
                }
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
                if (useWebView)
                {
                    await RefreshWebViewMenu();
                }
                else
                {
                    SetFormObject();
                }
            }

            if (!useWebView && blnMove)
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
        private async void CreateMenuScreenMain(string FileName, bool Starting = false)
        {
            // 現在のサイズを記憶
            int intOldWidth = this.Width;
            int intOldHeight = this.Height;

            // メニューファイル名を設定（未指定時、デフォルトセット）
            m_CMenuPage.MenuFileName = FileName;

            // メニューファイル読み込み
            m_CMenuPage.MenuFileRead();

            // WebViewモードの場合はWebViewを更新
            if (useWebView && webView != null && webView.CoreWebView2 != null)
            {
                await RefreshWebViewMenu();
            }
            else
            {
                // メニューフォーム内のボタン等の設定
                SetFormObject();
            }

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
            // WebViewモードでは従来のボタン処理をスキップ
            if (useWebView)
            {
                // キャプションとサイズ設定のみ行う
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

                SetMenuCheckEdit();
                SetMenuCheckMode();

                // 背景色の設定
                pnlContainer.BackColor = ColorTranslator.FromOle(m_CMenuPage.BackColor);
                picContainer.BackColor = ColorTranslator.FromOle(m_CMenuPage.BackColor);

                return;
            }

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
                this.CancelButton = m_MenuButtons[m_CMenuPage.CancelButton];
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
        private async void SetButtonDisp(int intIndex, bool blnMode)
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
            if (useWebView)
            {
                await RefreshWebViewMenu();
            }
            else
            {
                SetFormObject();
            }
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
        private async void ButtonEdit(int Index)
        {
            using (var frmDialog = new FormButtonEdit())
            {
                if (frmDialog.ShowDialog(this) == DialogResult.OK)
                {
                    if (useWebView)
                    {
                        await RefreshWebViewMenu();
                    }
                    else
                    {
                        SetFormObject();
                        GoButton(Index);
                    }
                }
            }
        }

        /// <summary>
        /// メニューボタンクリック時の、実行モードの処理
        /// </summary>
        private async void ButtonExec(int Index)
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

                                if (!useWebView)
                                {
                                    if (item.Attribute == CMenuPage.CMenuFileItemInf.ItemAttribute.BackPrevMenu)
                                    {
                                        GoButton(intRevIndex);
                                    }
                                    else
                                    {
                                        GoButton(0);
                                    }
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
            // WebViewモードではキー処理をスキップ
            if (useWebView)
                return;

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
            // WebViewモードでは処理をスキップ
            if (useWebView)
                return;

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

        #region XML関連メニュー
        
        /// <summary>
        /// メニューにXML関連の項目を追加する
        /// </summary>
        private void AddXmlMenuItems()
        {
            // XML形式で新規作成
            mnuFileMemberNewXml = new ToolStripMenuItem("新規作成(&N)...");
            mnuFileMemberNewXml.ShortcutKeys = Keys.Control | Keys.N;
            
            // XML形式を開く
            mnuFileMemberOpenXml = new ToolStripMenuItem("開く(&O)...");
            mnuFileMemberOpenXml.ShortcutKeys = Keys.Control | Keys.O;
            
            // メニューに追加
            int insertIndex = mnuFile.DropDownItems.IndexOf(mnuFileMemberOption);
            if (insertIndex >= 0)
            {
                mnuFile.DropDownItems.Insert(insertIndex, mnuFileMemberOpenXml);
                mnuFile.DropDownItems.Insert(insertIndex, mnuFileMemberNewXml);
                mnuFile.DropDownItems.Insert(insertIndex + 2, new ToolStripSeparator());
            }
            else
            {
                mnuFile.DropDownItems.Insert(0, new ToolStripSeparator());
                mnuFile.DropDownItems.Insert(0, mnuFileMemberOpenXml);
                mnuFile.DropDownItems.Insert(0, mnuFileMemberNewXml);
            }
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