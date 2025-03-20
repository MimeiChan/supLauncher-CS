using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;

namespace HiMenu
{
    internal class CMenuPage
    {
        [DllImport("KERNEL32.DLL", EntryPoint = "WritePrivateProfileString", CharSet = CharSet.Auto)]
        private static extern int API_WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpString, string lpFileName);

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
        private bool m_Saved;

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

        private enum MenuFileSection
        {
            NotSetGroup,
            EnvironmentTitle,
            ExecuteEnvironment,
            Current
        }

        private CMenuPage()
        {
        }

        internal static CMenuPage GetInstance()
        {
            return mObj;
        }

        #region メニューファイル情報関連

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

        internal void MenuFileRead()
        {
            // 変数初期化
            this.Initalize();

            if (!File.Exists(m_MenuFileName)) return;

            Encoding encoding = Encoding.GetEncoding("Shift-JIS");
            MenuFileSection Section = MenuFileSection.NotSetGroup;
            StringComparison StringComparison = StringComparison.CurrentCultureIgnoreCase;

            using (StreamReader txReader = new StreamReader(m_MenuFileName, encoding))
            {
                while (!txReader.EndOfStream)
                {
                    string strReadLine = txReader.ReadLine();

                    if (!strReadLine.StartsWith(";"))
                    {
                        // INIファイルのセクションを判定
                        if (strReadLine.IndexOf("[EnvironmentTitle]", StringComparison) == 0) Section = MenuFileSection.EnvironmentTitle;
                        if (strReadLine.IndexOf("[ExecuteEnvironment]", StringComparison) == 0) Section = MenuFileSection.ExecuteEnvironment;
                        if (strReadLine.IndexOf("[Current]", StringComparison) == 0) Section = MenuFileSection.Current;

                        // セクションのメンバを判定
                        int intIndex = strReadLine.IndexOf("=", StringComparison) + 1;

                        if (intIndex > 0)
                        {
                            string strValue = strReadLine.Substring(intIndex);

                            switch (Section)
                            {
                                case MenuFileSection.EnvironmentTitle:
                                    //--------------------------------------------------
                                    // [EnvironmentTitle]セクション
                                    //--------------------------------------------------
                                    string keyName = strReadLine.Substring(0, intIndex - 1);
                                    if (keyName.StartsWith("Title", StringComparison))
                                    {
                                        string idxStr = keyName.Substring(5, 2);
                                        if (int.TryParse(idxStr, out int intValIndex))
                                        {
                                            if (intValIndex == 0)
                                            {
                                                m_MenuTitle = strValue;
                                            }
                                            else
                                            {
                                                MenuItemsCountSet(intValIndex);

                                                intValIndex = intValIndex - 1;

                                                if (keyName.StartsWith("Title", StringComparison))
                                                {
                                                    m_MenuFileItem[intValIndex].Title = strValue;
                                                }
                                            }
                                        }
                                    }
                                    else if (keyName.StartsWith("Comment", StringComparison))
                                    {
                                        string idxStr = keyName.Substring(7, 2);
                                        if (int.TryParse(idxStr, out int intValIndex) && intValIndex > 0)
                                        {
                                            intValIndex = intValIndex - 1;
                                            m_MenuFileItem[intValIndex].Comment = strValue;
                                        }
                                    }
                                    else if (keyName.StartsWith("Command", StringComparison))
                                    {
                                        string idxStr = keyName.Substring(7, 2);
                                        if (int.TryParse(idxStr, out int intValIndex) && intValIndex > 0)
                                        {
                                            intValIndex = intValIndex - 1;
                                            m_MenuFileItem[intValIndex].Command = strValue;
                                        }
                                    }
                                    else if (keyName.StartsWith("Flag", StringComparison))
                                    {
                                        string idxStr = keyName.Substring(4, 2);
                                        if (int.TryParse(idxStr, out int intValIndex) && intValIndex > 0)
                                        {
                                            intValIndex = intValIndex - 1;
                                            m_MenuFileItem[intValIndex].Flag = strValue;
                                        }
                                    }
                                    break;

                                case MenuFileSection.ExecuteEnvironment:
                                    //--------------------------------------------------
                                    // [ExecuteEnvironment]セクション
                                    //--------------------------------------------------
                                    if (strReadLine.IndexOf("Rows", StringComparison) == 0) m_MenuRows = Convert.ToInt32(strValue);
                                    else if (strReadLine.IndexOf("Cols", StringComparison) == 0) m_MenuCols = Convert.ToInt32(strValue);
                                    else if (strReadLine.IndexOf("Width", StringComparison) == 0) m_MenuWidth = Convert.ToInt32(strValue);
                                    else if (strReadLine.IndexOf("Height", StringComparison) == 0) m_MenuHeight = Convert.ToInt32(strValue);
                                    else if (strReadLine.IndexOf("FontName", StringComparison) == 0) m_FontName = strValue;
                                    else if (strReadLine.IndexOf("FontSize", StringComparison) == 0) m_FontSize = float.Parse(strValue);
                                    else if (strReadLine.IndexOf("FontBold", StringComparison) == 0) m_FontBold = Convert.ToBoolean(strValue.Trim());
                                    else if (strReadLine.IndexOf("FontItalic", StringComparison) == 0) m_FontItalic = Convert.ToBoolean(strValue.Trim());
                                    else if (strReadLine.IndexOf("FontUnderline", StringComparison) == 0) m_FontUnderline = Convert.ToBoolean(strValue.Trim());
                                    else if (strReadLine.IndexOf("LockPassword", StringComparison) == 0) m_LockPassword = strValue;
                                    else if (strReadLine.IndexOf("DispPosition", StringComparison) == 0) m_DispPosition = (MenuDispPosition)(Convert.ToInt32(strValue) - 1);
                                    else if (strReadLine.IndexOf("BGFile", StringComparison) == 0) m_BGFile = strValue;
                                    else if (strReadLine.IndexOf("BGTile", StringComparison) == 0) m_BGTile = Convert.ToBoolean(strValue.Trim());
                                    else if (strReadLine.IndexOf("MenuVisible", StringComparison) == 0) m_MenuVisible = Convert.ToBoolean(strValue.Trim());
                                    else if (strReadLine.IndexOf("StatusBarVisible", StringComparison) == 0) m_StatusBarVisible = Convert.ToBoolean(strValue.Trim());
                                    else if (strReadLine.IndexOf("BackColor", StringComparison) == 0) m_BackColor = Convert.ToInt32(strValue);
                                    else if (strReadLine.IndexOf("ButtonColor", StringComparison) == 0) m_ButtonColor = Convert.ToInt32(strValue);
                                    else if (strReadLine.IndexOf("TextColor", StringComparison) == 0) m_TextColor = Convert.ToInt32(strValue);
                                    else if (strReadLine.IndexOf("HighLigitTextColor", StringComparison) == 0) m_HighLightTextColor = Convert.ToInt32(strValue);
                                    else if (strReadLine.IndexOf("CancelButton", StringComparison) == 0) m_CancelButton = Convert.ToInt32(strValue);
                                    break;

                                case MenuFileSection.Current:
                                    //--------------------------------------------------
                                    // [Current]セクション
                                    //--------------------------------------------------
                                    if (strReadLine.IndexOf("CurrentX", StringComparison) == 0) m_CurrentX = Convert.ToInt32(strValue);
                                    else if (strReadLine.IndexOf("CurrentY", StringComparison) == 0) m_CurrentY = Convert.ToInt32(strValue);
                                    break;
                            }
                        }
                    }
                }
            }

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

        internal bool MenuFileWriteBody()
        {
            Encoding encoding = Encoding.GetEncoding("Shift-JIS");

            using (StreamWriter txWriter = new StreamWriter(m_MenuFileName, false, encoding))
            {
                txWriter.WriteLine("; TDC Menu Version" + Application.ProductVersion.Split('.')[0] + "." + Application.ProductVersion.Split('.')[1] + Application.ProductVersion.Split('.')[2] + " 用定義ファイル");
                txWriter.WriteLine("[EnvironmentTitle]");
                txWriter.WriteLine("Title00=" + m_MenuTitle);
                for (int intIndex = 0; intIndex < (m_MenuRows * m_MenuCols); intIndex++)
                {
                    CMenuFileItemInf item = m_MenuFileItem[intIndex];
                    string strNo = (intIndex + 1).ToString("00");
                    if (item.Title.Length != 0)
                    {
                        txWriter.WriteLine("Title" + strNo + "=" + item.Title);
                    }
                    if (item.Comment.Length != 0)
                    {
                        txWriter.WriteLine("Comment" + strNo + "=" + item.Comment);
                    }
                    if (item.Command.Length != 0)
                    {
                        txWriter.WriteLine("Command" + strNo + "=" + item.Command);
                    }
                    if (item.Flag != "---")
                    {
                        txWriter.WriteLine("Flag" + strNo + "=" + item.Flag);
                    }
                }
                txWriter.WriteLine("[ExecuteEnvironment]");
                txWriter.WriteLine("Rows=" + m_MenuRows.ToString());
                txWriter.WriteLine("Cols=" + m_MenuCols.ToString());
                txWriter.WriteLine("Width=" + m_MenuWidth.ToString());
                txWriter.WriteLine("Height=" + m_MenuHeight.ToString());
                txWriter.WriteLine("FontName=" + m_FontName);
                txWriter.WriteLine("FontSize=" + m_FontSize.ToString());
                txWriter.WriteLine("FontBold=" + m_FontBold.ToString());
                txWriter.WriteLine("FontItalic=" + m_FontItalic.ToString());
                txWriter.WriteLine("FontUnderline=" + m_FontUnderline.ToString());
                txWriter.WriteLine("DispPosition=" + (((int)m_DispPosition) + 1).ToString());
                txWriter.WriteLine("BGFile=" + m_BGFile);
                txWriter.WriteLine("BGTile=" + m_BGTile.ToString());
                txWriter.WriteLine("MenuVisible=" + m_MenuVisible.ToString());
                txWriter.WriteLine("StatusBarVisible=" + m_StatusBarVisible.ToString());
                txWriter.WriteLine("BackColor=" + m_BackColor.ToString());
                txWriter.WriteLine("ButtonColor=" + m_ButtonColor.ToString());
                txWriter.WriteLine("TextColor=" + m_TextColor.ToString());
                txWriter.WriteLine("HighLigitTextColor=" + m_HighLightTextColor.ToString());
                if (m_CancelButton >= 0 && m_CancelButton < m_MenuRows * m_MenuCols)
                {
                    txWriter.WriteLine("CancelButton=" + m_CancelButton.ToString());
                }

                if (m_LockOn)
                {
                    txWriter.WriteLine("LockPassword=" + m_LockPassword);
                }
            }

            return true;
        }

        internal void MenuFileWriteLocateOnly()
        {
            if (m_RootMenuDispPosition == MenuDispPosition.ScreenCenter) return;

            int lngRetVal;
            string strLeftPnt;
            string strTopPnt;
            string strFileName = "";

            m_MenuForm.WindowState = FormWindowState.Normal;

            strLeftPnt = m_MenuForm.Left.ToString();
            strTopPnt = m_MenuForm.Top.ToString();

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

            lngRetVal = API_WritePrivateProfileString("Current", "CurrentX", strLeftPnt, strFileName);
            lngRetVal = API_WritePrivateProfileString("Current", "CurrentY", strTopPnt, strFileName);

            Debug.Print(strFileName + " " + strLeftPnt + " " + strTopPnt);
        }

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

        internal int CurrentX
        {
            get { return m_CurrentX; }
            set { m_CurrentX = value; }
        }

        internal int CurrentY
        {
            get { return m_CurrentY; }
            set { m_CurrentY = value; }
        }

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

        // C#でインデクサーを使用したプロパティとして正しく実装
        internal CMenuFileItemInf this[int Index]
        {
            get { return m_MenuFileItem[Index]; }
            set
            {
                m_MenuFileItem[Index] = value;
                m_Changed = true;
            }
        }

        // 既存のコードとの互換性のために、メソッドも提供
        internal CMenuFileItemInf GetMenuFileItem(int Index)
        {
            return m_MenuFileItem[Index];
        }

        internal void SetMenuFileItem(int Index, CMenuFileItemInf value)
        {
            m_MenuFileItem[Index] = value;
            m_Changed = true;
        }

        internal void MenuFileItemHiddenChanged()
        {
            m_Changed = true;
        }

        #endregion

        #region プロパティー

        internal FormHiMenu MenuForm
        {
            get { return m_MenuForm; }
            set { m_MenuForm = value; }
        }

        internal int CurrentButton
        {
            get { return m_CurrentButton; }
            set { m_CurrentButton = value; }
        }

        internal string MenuFileName
        {
            get { return m_MenuFileName; }
            set
            {
                if (value.Length == 0)
                {
                    m_MenuFileName = Path.GetDirectoryName(Application.ExecutablePath);
                    if (!m_MenuFileName.EndsWith("\\")) m_MenuFileName = m_MenuFileName + "\\";
#if DEBUG
                    m_MenuFileName += "..\\Release\\";
                    Directory.SetCurrentDirectory(m_MenuFileName);
#endif
                    m_MenuFileName += "MenuFile.MNU";
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

        internal bool Changed
        {
            get { return m_Changed; }
        }

        internal MenuDispPosition RootMenuDispPosition
        {
            get { return m_RootMenuDispPosition; }
        }

        #endregion
    }
}
