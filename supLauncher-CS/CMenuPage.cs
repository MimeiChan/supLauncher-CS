using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace HiMenu
{
    internal class CMenuPage
    {
        private static CMenuPage mObj = new CMenuPage();

        private FormHiMenu m_MenuForm;

        private MenuDispPosition m_RootMenuDispPosition; // ルートメニューフォームの初期表示位置
        private string m_RootMenuFileName = ""; // ルートメニューのファイル名

        #region メニューアイテム用ローカルクラス

        [Serializable()]
        public class CMenuFileItemInf
        {
            public enum ItemAttribute
            {
                ExecApplication,
                OpenNextMenu,
                BackPrevMenu
            }
            public enum ItemAfter
            {
                ContinueHiMenu,
                EndHiMenu,
                MinimizedHiMenu
            }

            public string Title { get; set; } = "";
            public string Comment { get; set; } = "";
            public string Command { get; set; } = "";
            public ItemAttribute Attribute { get; set; }
            public ItemAfter After { get; set; }
            public bool NoUse { get; set; } = false;

            public string Flag
            {
                get
                {
                    string[] strFlag = new string[] { "", "", "" };

                    switch (this.Attribute)
                    {
                        case ItemAttribute.ExecApplication:
                            strFlag[0] = "-";
                            break;
                        case ItemAttribute.OpenNextMenu:
                            strFlag[0] = "*";
                            break;
                        case ItemAttribute.BackPrevMenu:
                            strFlag[0] = "B";
                            break;
                    }

                    switch (this.After)
                    {
                        case ItemAfter.ContinueHiMenu:
                            strFlag[1] = "-";
                            break;
                        case ItemAfter.EndHiMenu:
                            strFlag[1] = "*";
                            break;
                        case ItemAfter.MinimizedHiMenu:
                            strFlag[1] = "I";
                            break;
                    }

                    switch (this.NoUse)
                    {
                        case true:
                            strFlag[2] = "*";
                            break;
                        case false:
                            strFlag[2] = "-";
                            break;
                    }

                    return string.Join("", strFlag);
                }
                set
                {
                    string strFlag = value + "---";

                    switch (strFlag[0])
                    {
                        case '-':
                            this.Attribute = ItemAttribute.ExecApplication;
                            break;
                        case '*':
                            this.Attribute = ItemAttribute.OpenNextMenu;
                            break;
                        case 'B':
                            this.Attribute = ItemAttribute.BackPrevMenu;
                            break;
                    }

                    switch (strFlag[1])
                    {
                        case '-':
                            this.After = ItemAfter.ContinueHiMenu;
                            break;
                        case '*':
                            this.After = ItemAfter.EndHiMenu;
                            break;
                        case 'I':
                            this.After = ItemAfter.MinimizedHiMenu;
                            break;
                    }

                    switch (strFlag[2])
                    {
                        case '*':
                            this.NoUse = true;
                            break;
                        case '-':
                            this.NoUse = false;
                            break;
                    }
                }
            }
        }

        #endregion

        private List<CMenuFileItemInf> m_MenuFileItem = new List<CMenuFileItemInf>();

        private bool m_LockOn; // 現在のメニュー内容の変更可・不可フラグ
        private string m_LockPassword; // 現在のメニュー内容の変更ﾊﾟｽﾜｰﾄﾞ

        private bool m_Changed;

        public enum MenuDispPosition
        {
            RootMenu,
            CurrentMenu,
            ScreenCenter
        }

        private string m_MenuFileName; // 現在のメニューファイル名

        private string m_MenuTitle; // 現在のメニュータイトル
        private int m_MenuRows; // 現在のメニューの行数
        private int m_MenuCols; // 現在のメニューの列数
        private int m_MenuWidth; // 現在のメニューフォームの幅
        private int m_MenuHeight; // 現在のメニューフォームの高さ

        private string m_FontName; // 現在のボタンのフォント名
        private float m_FontSize; // 現在のボタンのフォントサイズ
        private bool m_FontBold; // 現在のボタンのフォントの設定
        private bool m_FontItalic; // 現在のボタンのフォントの設定
        private bool m_FontUnderline; // 現在のボタンのフォントの設定

        private MenuDispPosition m_DispPosition; // 現在のメニューフォームの初期表示位置
        private string m_BGFile; // 現在のメニューフォームの背景画像ファイル名
        private bool m_BGTile; // 現在のメニューフォームの背景画像をならべるか
        private bool m_MenuVisible; // 現在のメニューフォームのメニューを表示するか
        private bool m_StatusBarVisible; // 現在のメニューフォームのステータスバーを表示するか
        private int m_BackColor; // メニューの背景色
        private int m_ButtonColor; // ボタンの表面色
        private int m_TextColor; // ボタンの文字色（非選択時）
        private int m_HighLightTextColor; // ボタンの文字色（選択時）

        private int m_CurrentX;
        private int m_CurrentY;

        private int m_CancelButton; // 現在のメニューのキャンセルボタンのインデックス
        private int m_CurrentButton; // 現在のメニューのアクティブボタンのインデックス

        /// <summary>
        /// コンストラクタ（privateでシングルトンパターンを実現）
        /// </summary>
        private CMenuPage()
        {
        }

        /// <summary>
        /// シングルトンインスタンスを取得するメソッド
        /// </summary>
        /// <returns>CMenuPageのインスタンス</returns>
        internal static CMenuPage GetInstance()
        {
            return mObj;
        }

        #region メニューファイル情報関連

        /// <summary>
        /// メニューページの設定を初期化する
        /// デフォルトの値をすべてのプロパティに設定する
        /// </summary>
        internal void Initalize()
        {
            m_MenuRows = 10;
            m_MenuCols = 2;
            m_MenuWidth = 559;
            m_MenuHeight = 406;
            m_MenuTitle = "";

            m_CurrentX = 0;
            m_CurrentY = 0;
            m_CurrentButton = 0;

            m_FontName = "ＭＳ Ｐゴシック";
            m_FontSize = 12;
            m_FontBold = true;
            m_FontItalic = false;
            m_FontUnderline = false;

            m_DispPosition = MenuDispPosition.RootMenu;
            m_BGFile = "";
            m_BGTile = false;
            m_MenuVisible = true;
            m_StatusBarVisible = true;
            m_LockOn = false;
            m_LockPassword = "";
            m_BackColor = ColorTranslator.ToOle(SystemColors.ControlDark);
            m_ButtonColor = ColorTranslator.ToOle(SystemColors.Control);
            m_TextColor = ColorTranslator.ToOle(SystemColors.ControlText);
            m_HighLightTextColor = ColorTranslator.ToOle(SystemColors.Highlight);
            m_CancelButton = -1;

            m_MenuFileItem.Clear();

            for (int intLoop = 1; intLoop <= m_MenuRows * m_MenuCols; intLoop++)
            {
                m_MenuFileItem.Add(new CMenuFileItemInf());
            }

            m_Changed = false;
        }

        /// <summary>
        /// メニューファイルを読み込み、設定を適用する
        /// ファイルが存在しない場合は初期化のみを行う
        /// </summary>
        internal void MenuFileRead()
        {
            // 変数初期化
            this.Initalize();

            if (!File.Exists(m_MenuFileName)) return;

            try
            {
                XDocument doc = XDocument.Load(m_MenuFileName);
                XElement root = doc.Root;

                // EnvironmentTitleセクションの読み込み
                XElement envTitle = root.Element("EnvironmentTitle");
                if (envTitle != null)
                {
                    // メニュータイトル
                    XElement title = envTitle.Element("Title");
                    if (title != null)
                    {
                        m_MenuTitle = title.Value;
                    }

                    // メニュー項目
                    XElement items = envTitle.Element("Items");
                    if (items != null)
                    {
                        foreach (XElement item in items.Elements("Item"))
                        {
                            int index;
                            if (item.Attribute("index") != null && int.TryParse(item.Attribute("index").Value, out index) && index > 0)
                            {
                                index--; // XMLでは1から始まるインデックスを使用
                                MenuItemsCountSet(index + 1); // 必要に応じてアイテム数を調整

                                XElement itemTitle = item.Element("Title");
                                if (itemTitle != null)
                                {
                                    m_MenuFileItem[index].Title = itemTitle.Value;
                                }

                                XElement itemComment = item.Element("Comment");
                                if (itemComment != null)
                                {
                                    m_MenuFileItem[index].Comment = itemComment.Value;
                                }

                                XElement itemCommand = item.Element("Command");
                                if (itemCommand != null)
                                {
                                    m_MenuFileItem[index].Command = itemCommand.Value;
                                }

                                XElement itemFlag = item.Element("Flag");
                                if (itemFlag != null)
                                {
                                    m_MenuFileItem[index].Flag = itemFlag.Value;
                                }
                            }
                        }
                    }
                }

                // ExecuteEnvironmentセクションの読み込み
                XElement execEnv = root.Element("ExecuteEnvironment");
                if (execEnv != null)
                {
                    XElement rows = execEnv.Element("Rows");
                    if (rows != null) m_MenuRows = Convert.ToInt32(rows.Value);

                    XElement cols = execEnv.Element("Cols");
                    if (cols != null) m_MenuCols = Convert.ToInt32(cols.Value);

                    XElement width = execEnv.Element("Width");
                    if (width != null) m_MenuWidth = Convert.ToInt32(width.Value);

                    XElement height = execEnv.Element("Height");
                    if (height != null) m_MenuHeight = Convert.ToInt32(height.Value);

                    XElement fontName = execEnv.Element("FontName");
                    if (fontName != null) m_FontName = fontName.Value;

                    XElement fontSize = execEnv.Element("FontSize");
                    if (fontSize != null) m_FontSize = float.Parse(fontSize.Value);

                    XElement fontBold = execEnv.Element("FontBold");
                    if (fontBold != null) m_FontBold = Convert.ToBoolean(fontBold.Value);

                    XElement fontItalic = execEnv.Element("FontItalic");
                    if (fontItalic != null) m_FontItalic = Convert.ToBoolean(fontItalic.Value);

                    XElement fontUnderline = execEnv.Element("FontUnderline");
                    if (fontUnderline != null) m_FontUnderline = Convert.ToBoolean(fontUnderline.Value);

                    XElement dispPosition = execEnv.Element("DispPosition");
                    if (dispPosition != null) m_DispPosition = (MenuDispPosition)(Convert.ToInt32(dispPosition.Value) - 1);

                    XElement bgFile = execEnv.Element("BGFile");
                    if (bgFile != null) m_BGFile = bgFile.Value;

                    XElement bgTile = execEnv.Element("BGTile");
                    if (bgTile != null) m_BGTile = Convert.ToBoolean(bgTile.Value);

                    XElement menuVisible = execEnv.Element("MenuVisible");
                    if (menuVisible != null) m_MenuVisible = Convert.ToBoolean(menuVisible.Value);

                    XElement statusBarVisible = execEnv.Element("StatusBarVisible");
                    if (statusBarVisible != null) m_StatusBarVisible = Convert.ToBoolean(statusBarVisible.Value);

                    XElement backColor = execEnv.Element("BackColor");
                    if (backColor != null) m_BackColor = Convert.ToInt32(backColor.Value);

                    XElement buttonColor = execEnv.Element("ButtonColor");
                    if (buttonColor != null) m_ButtonColor = Convert.ToInt32(buttonColor.Value);

                    XElement textColor = execEnv.Element("TextColor");
                    if (textColor != null) m_TextColor = Convert.ToInt32(textColor.Value);

                    XElement highLightTextColor = execEnv.Element("HighLightTextColor");
                    if (highLightTextColor != null) m_HighLightTextColor = Convert.ToInt32(highLightTextColor.Value);

                    XElement cancelButton = execEnv.Element("CancelButton");
                    if (cancelButton != null) m_CancelButton = Convert.ToInt32(cancelButton.Value);

                    XElement lockPassword = execEnv.Element("LockPassword");
                    if (lockPassword != null) m_LockPassword = lockPassword.Value;
                }

                // Currentセクションの読み込み
                XElement current = root.Element("Current");
                if (current != null)
                {
                    XElement currentX = current.Element("CurrentX");
                    if (currentX != null) m_CurrentX = Convert.ToInt32(currentX.Value);

                    XElement currentY = current.Element("CurrentY");
                    if (currentY != null) m_CurrentY = Convert.ToInt32(currentY.Value);
                }

                // メニュー項目数の調整
                MenuItemsCountSet();

                // ルートメニュー情報を記憶
                if (CMenuChain.GetInstance().IsRootMenu())
                {
                    m_RootMenuFileName = m_MenuFileName;
                    m_RootMenuDispPosition = m_DispPosition;
                }

                m_LockOn = true;
                m_Changed = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("XMLファイルの読み込みに失敗しました: " + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // 読み込みに失敗した場合は初期化
                Initalize();
            }
        }

        /// <summary>
        /// メニューの設定をファイルに保存する
        /// 変更がある場合はユーザーに保存の確認を行う
        /// </summary>
        /// <returns>保存処理の結果を示すDialogResult</returns>
        internal DialogResult MenuFileWrite()
        {
            if (m_Changed)
            {
                DialogResult result = MessageBox.Show("変更を保存しますか？", "保存の確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                switch (result)
                {
                    case DialogResult.Yes:
                        if (MenuFileWriteBody())
                        {
                            MenuFileWriteLocateOnly();
                        }
                        else
                        {
                            return DialogResult.Cancel;
                        }
                        break;
                    case DialogResult.No:
                        return DialogResult.No;
                    case DialogResult.Cancel:
                        return DialogResult.Cancel;
                }
            }
            else
            {
                MenuFileWriteLocateOnly();
            }

            m_Changed = false;

            return DialogResult.OK;
        }

        /// <summary>
        /// メニューの設定内容をファイルに書き込む
        /// </summary>
        /// <returns>書き込みが成功した場合はtrue</returns>
        internal bool MenuFileWriteBody()
        {
            try
            {
                // バージョン情報を含めたHiMenu要素の作成
                XDocument doc = new XDocument(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("HiMenu",
                        new XAttribute("version", Application.ProductVersion)
                    )
                );

                // EnvironmentTitleセクションの作成
                XElement envTitle = new XElement("EnvironmentTitle",
                    new XElement("Title", m_MenuTitle),
                    new XElement("Items")
                );
                doc.Root.Add(envTitle);

                // メニュー項目の追加
                XElement items = envTitle.Element("Items");
                for (int intIndex = 0; intIndex < (m_MenuRows * m_MenuCols); intIndex++)
                {
                    CMenuFileItemInf item = m_MenuFileItem[intIndex];
                    if (item.Title.Length != 0 || item.Comment.Length != 0 || item.Command.Length != 0 || item.Flag != "---")
                    {
                        XElement menuItem = new XElement("Item",
                            new XAttribute("index", (intIndex + 1).ToString()),
                            new XElement("Title", item.Title)
                        );

                        if (item.Comment.Length != 0)
                        {
                            menuItem.Add(new XElement("Comment", item.Comment));
                        }

                        if (item.Command.Length != 0)
                        {
                            menuItem.Add(new XElement("Command", item.Command));
                        }

                        if (item.Flag != "---")
                        {
                            menuItem.Add(new XElement("Flag", item.Flag));
                        }

                        items.Add(menuItem);
                    }
                }

                // ExecuteEnvironmentセクションの作成
                XElement execEnv = new XElement("ExecuteEnvironment",
                    new XElement("Rows", m_MenuRows),
                    new XElement("Cols", m_MenuCols),
                    new XElement("Width", m_MenuWidth),
                    new XElement("Height", m_MenuHeight),
                    new XElement("FontName", m_FontName),
                    new XElement("FontSize", m_FontSize),
                    new XElement("FontBold", m_FontBold),
                    new XElement("FontItalic", m_FontItalic),
                    new XElement("FontUnderline", m_FontUnderline),
                    new XElement("DispPosition", ((int)m_DispPosition) + 1),
                    new XElement("BGFile", m_BGFile),
                    new XElement("BGTile", m_BGTile),
                    new XElement("MenuVisible", m_MenuVisible),
                    new XElement("StatusBarVisible", m_StatusBarVisible),
                    new XElement("BackColor", m_BackColor),
                    new XElement("ButtonColor", m_ButtonColor),
                    new XElement("TextColor", m_TextColor),
                    new XElement("HighLightTextColor", m_HighLightTextColor)
                );

                if (m_CancelButton >= 0 && m_CancelButton < m_MenuRows * m_MenuCols)
                {
                    execEnv.Add(new XElement("CancelButton", m_CancelButton));
                }

                if (m_LockOn)
                {
                    execEnv.Add(new XElement("LockPassword", m_LockPassword));
                }

                doc.Root.Add(execEnv);

                // Currentセクションの作成
                XElement current = new XElement("Current",
                    new XElement("CurrentX", m_CurrentX),
                    new XElement("CurrentY", m_CurrentY)
                );
                doc.Root.Add(current);

                // XMLファイルへの保存
                doc.Save(m_MenuFileName);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("XMLファイルの書き込みに失敗しました: " + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// メニューの位置情報のみをファイルに書き込む
        /// </summary>
        internal void MenuFileWriteLocateOnly()
        {
            if (m_RootMenuDispPosition == MenuDispPosition.ScreenCenter) return;

            string strFileName = "";
            m_MenuForm.WindowState = FormWindowState.Normal;

            string strLeftPnt = m_MenuForm.Left.ToString();
            string strTopPnt = m_MenuForm.Top.ToString();

            switch (m_RootMenuDispPosition)
            {
                case MenuDispPosition.RootMenu:
                    strFileName = m_RootMenuFileName;
                    break;
                case MenuDispPosition.CurrentMenu:
                    strFileName = m_MenuFileName;
                    break;
                case MenuDispPosition.ScreenCenter:
                    break;
            }

            try
            {
                if (!string.IsNullOrEmpty(strFileName) && File.Exists(strFileName))
                {
                    XDocument doc = XDocument.Load(strFileName);

                    // Currentセクションの取得または作成
                    XElement current = doc.Root.Element("Current");
                    if (current == null)
                    {
                        current = new XElement("Current");
                        doc.Root.Add(current);
                    }

                    // CurrentX要素の取得または作成
                    XElement currentX = current.Element("CurrentX");
                    if (currentX == null)
                    {
                        currentX = new XElement("CurrentX");
                        current.Add(currentX);
                    }
                    currentX.Value = strLeftPnt;

                    // CurrentY要素の取得または作成
                    XElement currentY = current.Element("CurrentY");
                    if (currentY == null)
                    {
                        currentY = new XElement("CurrentY");
                        current.Add(currentY);
                    }
                    currentY.Value = strTopPnt;

                    // ファイルに保存
                    doc.Save(strFileName);
                }
            }
            catch (Exception ex)
            {
                Debug.Print("XML位置情報の書き込みに失敗しました: " + ex.Message);
            }

            Debug.Print(strFileName + " " + strLeftPnt + " " + strTopPnt);
        }

        /// <summary>
        /// メニュー項目の数を設定された行数と列数に合わせて調整する
        /// </summary>
        /// <param name="intTargetItemCountArg">設定する項目数（-1の場合は現在の行数×列数）</param>
        internal void MenuItemsCountSet(int intTargetItemCountArg = -1)
        {
            int intNowItemCount = m_MenuFileItem.Count;
            int intTargetItemCount = (intTargetItemCountArg == -1) ? m_MenuRows * m_MenuCols : intTargetItemCountArg;

            if (intTargetItemCount > intNowItemCount)
            {
                for (int intLoop = intNowItemCount; intLoop < intTargetItemCount; intLoop++)
                {
                    m_MenuFileItem.Add(new CMenuFileItemInf());
                }
            }
            else if (intTargetItemCount < intNowItemCount)
            {
                for (int intLoop = intNowItemCount - 1; intLoop >= intTargetItemCount; intLoop--)
                {
                    m_MenuFileItem.RemoveAt(intLoop);
                }
            }
        }

        #endregion

        #region プロパティー(保存対象：変更チェック無し)

        /// <summary>
        /// メニューフォームのX座標を取得または設定
        /// </summary>
        internal int CurrentX
        {
            get { return m_CurrentX; }
            set { m_CurrentX = value; }
        }

        /// <summary>
        /// メニューフォームのY座標を取得または設定
        /// </summary>
        internal int CurrentY
        {
            get { return m_CurrentY; }
            set { m_CurrentY = value; }
        }

        /// <summary>
        /// メニューのロック状態を取得または設定
        /// </summary>
        internal bool LockOn
        {
            get { return m_LockOn; }
            set
            {
                if (m_LockOn != value)
                {
                    m_LockOn = value;
                }
            }
        }

        #endregion

        #region プロパティー(保存対象：変更チェック有り)

        /// <summary>
        /// メニューの行数を取得または設定
        /// 値が変更された場合は変更フラグを設定する
        /// </summary>
        internal int MenuRows
        {
            get { return m_MenuRows; }
            set
            {
                if (m_MenuRows != value)
                {
                    m_MenuRows = value;
                    m_Changed = true;
                }
            }
        }

        /// <summary>
        /// メニューの列数を取得または設定
        /// 値が変更された場合は変更フラグを設定する
        /// </summary>
        internal int MenuCols
        {
            get { return m_MenuCols; }
            set
            {
                if (m_MenuCols != value)
                {
                    m_MenuCols = value;
                    m_Changed = true;
                }
            }
        }

        /// <summary>
        /// メニューフォームの幅を取得または設定
        /// 値が変更された場合は変更フラグを設定する
        /// </summary>
        internal int MenuWidth
        {
            get { return m_MenuWidth; }
            set
            {
                if (m_MenuWidth != value)
                {
                    m_MenuWidth = value;
                    m_Changed = true;
                }
            }
        }

        /// <summary>
        /// メニューフォームの高さを取得または設定
        /// 値が変更された場合は変更フラグを設定する
        /// </summary>
        internal int MenuHeight
        {
            get { return m_MenuHeight; }
            set
            {
                if (m_MenuHeight != value)
                {
                    m_MenuHeight = value;
                    m_Changed = true;
                }
            }
        }

        /// <summary>
        /// メニューのタイトルを取得または設定
        /// 値が変更された場合は変更フラグを設定する
        /// </summary>
        internal string MenuTitle
        {
            get { return m_MenuTitle; }
            set
            {
                if (m_MenuTitle != value)
                {
                    m_MenuTitle = value;
                    m_Changed = true;
                }
            }
        }

        /// <summary>
        /// キャンセルボタンのインデックスを取得または設定
        /// 値が変更された場合は変更フラグを設定する
        /// </summary>
        internal int CancelButton
        {
            get { return m_CancelButton; }
            set
            {
                if (m_CancelButton != value)
                {
                    m_CancelButton = value;
                    m_Changed = true;
                }
            }
        }

        /// <summary>
        /// フォント名を取得または設定
        /// 値が変更された場合は変更フラグを設定する
        /// </summary>
        internal string FontName
        {
            get { return m_FontName; }
            set
            {
                if (m_FontName != value)
                {
                    m_FontName = value;
                    m_Changed = true;
                }
            }
        }

        /// <summary>
        /// フォントサイズを取得または設定
        /// 値が変更された場合は変更フラグを設定する
        /// </summary>
        internal float FontSize
        {
            get { return m_FontSize; }
            set
            {
                if (m_FontSize != value)
                {
                    m_FontSize = value;
                    m_Changed = true;
                }
            }
        }

        /// <summary>
        /// フォントの太字設定を取得または設定
        /// 値が変更された場合は変更フラグを設定する
        /// </summary>
        internal bool FontBold
        {
            get { return m_FontBold; }
            set
            {
                if (m_FontBold != value)
                {
                    m_FontBold = value;
                    m_Changed = true;
                }
            }
        }

        /// <summary>
        /// フォントの斜体設定を取得または設定
        /// 値が変更された場合は変更フラグを設定する
        /// </summary>
        internal bool FontItalic
        {
            get { return m_FontItalic; }
            set
            {
                if (m_FontItalic != value)
                {
                    m_FontItalic = value;
                    m_Changed = true;
                }
            }
        }

        /// <summary>
        /// フォントの下線設定を取得または設定
        /// 値が変更された場合は変更フラグを設定する
        /// </summary>
        internal bool FontUnderline
        {
            get { return m_FontUnderline; }
            set
            {
                if (m_FontUnderline != value)
                {
                    m_FontUnderline = value;
                    m_Changed = true;
                }
            }
        }

        /// <summary>
        /// メニューの表示位置を取得または設定
        /// 値が変更された場合は変更フラグを設定し、ルートメニューの場合は表示位置を記憶する
        /// </summary>
        internal MenuDispPosition DispPosition
        {
            get { return m_DispPosition; }
            set
            {
                if (m_DispPosition != value)
                {
                    m_DispPosition = value;
                    // ルートメニュー情報を記憶
                    if (CMenuChain.GetInstance().IsRootMenu())
                    {
                        m_RootMenuDispPosition = m_DispPosition;
                    }
                    m_Changed = true;
                }
            }
        }

        /// <summary>
        /// 背景画像ファイル名を取得または設定
        /// 値が変更された場合は変更フラグを設定する
        /// </summary>
        internal string BGFile
        {
            get { return m_BGFile; }
            set
            {
                if (m_BGFile != value)
                {
                    m_BGFile = value;
                    m_Changed = true;
                }
            }
        }

        /// <summary>
        /// 背景画像のタイル表示設定を取得または設定
        /// 値が変更された場合は変更フラグを設定する
        /// </summary>
        internal bool BGTile
        {
            get { return m_BGTile; }
            set
            {
                if (m_BGTile != value)
                {
                    m_BGTile = value;
                    m_Changed = true;
                }
            }
        }

        /// <summary>
        /// メニューの表示状態を取得または設定
        /// 値が変更された場合は変更フラグを設定する
        /// </summary>
        internal bool MenuVisible
        {
            get { return m_MenuVisible; }
            set
            {
                if (m_MenuVisible != value)
                {
                    m_MenuVisible = value;
                    m_Changed = true;
                }
            }
        }

        /// <summary>
        /// ステータスバーの表示状態を取得または設定
        /// 値が変更された場合は変更フラグを設定する
        /// </summary>
        internal bool StatusBarVisible
        {
            get { return m_StatusBarVisible; }
            set
            {
                if (m_StatusBarVisible != value)
                {
                    m_StatusBarVisible = value;
                    m_Changed = true;
                }
            }
        }

        /// <summary>
        /// メニューのロックパスワードを取得または設定
        /// 値が変更された場合は変更フラグを設定する
        /// </summary>
        internal string LockPassword
        {
            get { return m_LockPassword; }
            set
            {
                if (m_LockPassword != value)
                {
                    m_LockPassword = value;
                    m_Changed = true;
                }
            }
        }

        /// <summary>
        /// メニューの背景色を取得または設定
        /// 値が変更された場合は変更フラグを設定する
        /// </summary>
        internal int BackColor
        {
            get { return m_BackColor; }
            set
            {
                if (m_BackColor != value)
                {
                    m_BackColor = value;
                    m_Changed = true;
                }
            }
        }

        /// <summary>
        /// ボタンの背景色を取得または設定
        /// 値が変更された場合は変更フラグを設定する
        /// </summary>
        internal int ButtonColor
        {
            get { return m_ButtonColor; }
            set
            {
                if (m_ButtonColor != value)
                {
                    m_ButtonColor = value;
                    m_Changed = true;
                }
            }
        }

        /// <summary>
        /// ボタンのテキスト色を取得または設定
        /// 値が変更された場合は変更フラグを設定する
        /// </summary>
        internal int TextColor
        {
            get { return m_TextColor; }
            set
            {
                if (m_TextColor != value)
                {
                    m_TextColor = value;
                    m_Changed = true;
                }
            }
        }

        /// <summary>
        /// ボタンの選択時テキスト色を取得または設定
        /// 値が変更された場合は変更フラグを設定する
        /// </summary>
        internal int HighLightTextColor
        {
            get { return m_HighLightTextColor; }
            set
            {
                if (m_HighLightTextColor != value)
                {
                    m_HighLightTextColor = value;
                    m_Changed = true;
                }
            }
        }

        /// <summary>
        /// メニュー項目を取得または設定するインデクサー
        /// 値が変更された場合は変更フラグを設定する
        /// </summary>
        /// <param name="Index">メニュー項目のインデックス</param>
        internal CMenuFileItemInf this[int Index]
        {
            get { return m_MenuFileItem[Index]; }
            set
            {
                m_MenuFileItem[Index] = value;
                m_Changed = true;
            }
        }

        /// <summary>
        /// メニュー項目を取得する
        /// </summary>
        /// <param name="Index">メニュー項目のインデックス</param>
        /// <returns>指定されたインデックスのメニュー項目</returns>
        internal CMenuFileItemInf GetMenuFileItem(int Index)
        {
            return m_MenuFileItem[Index];
        }

        /// <summary>
        /// メニュー項目を設定する
        /// </summary>
        /// <param name="Index">メニュー項目のインデックス</param>
        /// <param name="value">設定するメニュー項目</param>
        internal void SetMenuFileItem(int Index, CMenuFileItemInf value)
        {
            m_MenuFileItem[Index] = value;
            m_Changed = true;
        }

        /// <summary>
        /// メニュー項目の表示状態が変更されたことを通知する
        /// </summary>
        internal void MenuFileItemHiddenChanged()
        {
            m_Changed = true;
        }

        #endregion

        #region プロパティー

        /// <summary>
        /// メニューフォームへの参照を取得または設定
        /// </summary>
        internal FormHiMenu MenuForm
        {
            get { return m_MenuForm; }
            set { m_MenuForm = value; }
        }

        /// <summary>
        /// 現在選択されているボタンのインデックスを取得または設定
        /// </summary>
        internal int CurrentButton
        {
            get { return m_CurrentButton; }
            set { m_CurrentButton = value; }
        }

        /// <summary>
        /// メニューファイル名を取得または設定
        /// ファイル名が空の場合はデフォルトのパスを設定する
        /// </summary>
        internal string MenuFileName
        {
            get { return m_MenuFileName; }
            set
            {
                if (value.Length == 0)
                {
                    m_MenuFileName = Path.GetDirectoryName(Application.ExecutablePath);
                    if (!m_MenuFileName.EndsWith("\\")) m_MenuFileName = m_MenuFileName + "\\";
                    
                    // デフォルトファイル名をXML形式に変更
                    m_MenuFileName += "MenuFile.xml";
                    
                    // 必要に応じてディレクトリを作成
                    string directory = Path.GetDirectoryName(m_MenuFileName);
                    if (!Directory.Exists(directory))
                    {
                        try
                        {
                            Directory.CreateDirectory(directory);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("ディレクトリの作成に失敗しました: " + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    m_MenuFileName = value;
                }

                int intIndex0 = m_MenuFileName.Length;
                int intIndex1 = 0;
                int intIndex2 = 0;

                if (m_MenuFileName.StartsWith("\"")) intIndex1 = 1;
                if (m_MenuFileName.EndsWith("\"")) intIndex2 = 1;

                m_MenuFileName = m_MenuFileName.Substring(intIndex1, intIndex0 - intIndex1 - intIndex2);
            }
        }

        /// <summary>
        /// メニューで使用するフォントを取得
        /// 現在の設定に基づいてフォントオブジェクトを生成する
        /// </summary>
        internal Font Font
        {
            get
            {
                FontStyle MyFontStyle = FontStyle.Regular;
                if (this.FontBold) MyFontStyle |= FontStyle.Bold;
                if (this.FontItalic) MyFontStyle |= FontStyle.Italic;
                if (this.FontUnderline) MyFontStyle |= FontStyle.Underline;
                return new Font(this.FontName, this.FontSize, MyFontStyle);
            }
        }

        /// <summary>
        /// メニューの内容が変更されているかどうかを取得
        /// </summary>
        internal bool Changed
        {
            get { return m_Changed; }
        }

        /// <summary>
        /// ルートメニューの表示位置を取得
        /// </summary>
        internal MenuDispPosition RootMenuDispPosition
        {
            get { return m_RootMenuDispPosition; }
        }

        #endregion
    }
}