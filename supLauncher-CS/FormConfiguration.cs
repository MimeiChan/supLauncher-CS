using System;
using System.Drawing;
using System.Windows.Forms;

namespace HiMenu
{
    public partial class FormConfiguration : Form
    {
        private CMenuPage m_CMenuPage = CMenuPage.GetInstance();

        private int m_savedBackColor;
        private int m_savedButtonColor;
        private int m_savedTextColor;
        private int m_savedHighLightTextColor;

        private bool m_formChanged = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FormConfiguration()
        {
            InitializeComponent();
            this.Load += FormConfiguration_Load;
            this.cmdOK.Click += cmdOK_Click;
            this.cmdBGFileBrowse.Click += cmdBGFileBrowse_Click;
            this.cmdBackColor.Click += cmdBackColor_Click;
            this.cmdButtonColor.Click += cmdButtonColor_Click;
            this.cmdTextColor.Click += cmdTextColor_Click;
            this.cmdHighlightTextColor.Click += cmdHighlightTextColor_Click;

            // フォント名リストを取得
            foreach (FontFamily font in FontFamily.Families)
            {
                cmbFontName.Items.Add(font.Name);
            }
        }

        /// <summary>
        /// フォームロード時のイベント
        /// </summary>
        private void FormConfiguration_Load(object sender, EventArgs e)
        {
            // 現在の設定をフォームに反映
            txtTitle.Text = m_CMenuPage.MenuTitle;
            nudRows.Value = m_CMenuPage.MenuRows;
            nudCols.Value = m_CMenuPage.MenuCols;
            nudWidth.Value = m_CMenuPage.MenuWidth;
            nudHeight.Value = m_CMenuPage.MenuHeight;

            // メニュー表示位置
            switch (m_CMenuPage.DispPosition)
            {
                case CMenuPage.MenuDispPosition.RootMenu:
                    optDispositionRoot.Checked = true;
                    break;
                case CMenuPage.MenuDispPosition.CurrentMenu:
                    optDispositionCurrent.Checked = true;
                    break;
                case CMenuPage.MenuDispPosition.ScreenCenter:
                    optDispositionCenter.Checked = true;
                    break;
            }

            // メニューオプション
            txtBGFile.Text = m_CMenuPage.BGFile;
            chkBGTile.Checked = m_CMenuPage.BGTile;
            chkMenuVisible.Checked = m_CMenuPage.MenuVisible;
            chkStatusVisible.Checked = m_CMenuPage.StatusBarVisible;

            // フォント設定
            cmbFontName.Text = m_CMenuPage.FontName;
            nudFontSize.Value = (decimal)m_CMenuPage.FontSize;
            chkFontBold.Checked = m_CMenuPage.FontBold;
            chkFontItalic.Checked = m_CMenuPage.FontItalic;
            chkFontUnderline.Checked = m_CMenuPage.FontUnderline;

            // 色設定
            m_savedBackColor = m_CMenuPage.BackColor;
            m_savedButtonColor = m_CMenuPage.ButtonColor;
            m_savedTextColor = m_CMenuPage.TextColor;
            m_savedHighLightTextColor = m_CMenuPage.HighLightTextColor;

            cmdBackColor.BackColor = ColorTranslator.FromOle(m_savedBackColor);
            cmdButtonColor.BackColor = ColorTranslator.FromOle(m_savedButtonColor);
            cmdTextColor.BackColor = ColorTranslator.FromOle(m_savedTextColor);
            cmdHighlightTextColor.BackColor = ColorTranslator.FromOle(m_savedHighLightTextColor);

            // ボタン色によってテキスト色を変更
            if (cmdTextColor.BackColor.GetBrightness() < 0.5)
            {
                cmdTextColor.ForeColor = Color.White;
            }
            else
            {
                cmdTextColor.ForeColor = Color.Black;
            }

            if (cmdHighlightTextColor.BackColor.GetBrightness() < 0.5)
            {
                cmdHighlightTextColor.ForeColor = Color.White;
            }
            else
            {
                cmdHighlightTextColor.ForeColor = Color.Black;
            }

            m_formChanged = false;
        }

        /// <summary>
        /// OKボタンクリック時のイベント
        /// </summary>
        private void cmdOK_Click(object sender, EventArgs e)
        {
            // フォームの値を設定値に反映
            m_CMenuPage.MenuTitle = txtTitle.Text;
            m_CMenuPage.MenuRows = (int)nudRows.Value;
            m_CMenuPage.MenuCols = (int)nudCols.Value;
            m_CMenuPage.MenuWidth = (int)nudWidth.Value;
            m_CMenuPage.MenuHeight = (int)nudHeight.Value;

            // メニュー表示位置
            if (optDispositionRoot.Checked)
            {
                m_CMenuPage.DispPosition = CMenuPage.MenuDispPosition.RootMenu;
            }
            else if (optDispositionCurrent.Checked)
            {
                m_CMenuPage.DispPosition = CMenuPage.MenuDispPosition.CurrentMenu;
            }
            else if (optDispositionCenter.Checked)
            {
                m_CMenuPage.DispPosition = CMenuPage.MenuDispPosition.ScreenCenter;
            }

            // メニューオプション
            m_CMenuPage.BGFile = txtBGFile.Text;
            m_CMenuPage.BGTile = chkBGTile.Checked;
            m_CMenuPage.MenuVisible = chkMenuVisible.Checked;
            m_CMenuPage.StatusBarVisible = chkStatusVisible.Checked;

            // フォント設定
            m_CMenuPage.FontName = cmbFontName.Text;
            m_CMenuPage.FontSize = (float)nudFontSize.Value;
            m_CMenuPage.FontBold = chkFontBold.Checked;
            m_CMenuPage.FontItalic = chkFontItalic.Checked;
            m_CMenuPage.FontUnderline = chkFontUnderline.Checked;

            // 色設定
            m_CMenuPage.BackColor = m_savedBackColor;
            m_CMenuPage.ButtonColor = m_savedButtonColor;
            m_CMenuPage.TextColor = m_savedTextColor;
            m_CMenuPage.HighLightTextColor = m_savedHighLightTextColor;
        }

        /// <summary>
        /// 背景画像参照ボタンクリック時のイベント
        /// </summary>
        private void cmdBGFileBrowse_Click(object sender, EventArgs e)
        {
            using (var dlgOpen = new OpenFileDialog())
            {
                dlgOpen.Title = "背景画像ファイルを選択してください";
                dlgOpen.Filter = "画像ファイル (*.bmp;*.jpg;*.jpeg;*.gif;*.png)|*.bmp;*.jpg;*.jpeg;*.gif;*.png|すべてのファイル (*.*)|*.*";
                dlgOpen.FilterIndex = 1;
                dlgOpen.InitialDirectory = Environment.CurrentDirectory;
                dlgOpen.FileName = "";

                if (dlgOpen.ShowDialog(this) == DialogResult.OK)
                {
                    // メニューは相対パスに変換
                    string strCurrentDir = Environment.CurrentDirectory;
                    if (!strCurrentDir.EndsWith("\\"))
                    {
                        strCurrentDir += "\\";
                    }

                    string strFilePath = dlgOpen.FileName;
                    if (strFilePath.ToLower().StartsWith(strCurrentDir.ToLower()))
                    {
                        txtBGFile.Text = strFilePath.Substring(strCurrentDir.Length);
                    }
                    else
                    {
                        txtBGFile.Text = strFilePath;
                    }

                    m_formChanged = true;
                }
            }
        }

        /// <summary>
        /// 背景色変更ボタンクリック時のイベント
        /// </summary>
        private void cmdBackColor_Click(object sender, EventArgs e)
        {
            using (var colorDialog = new FormColor())
            {
                colorDialog.SelectedColor = cmdBackColor.BackColor;
                
                if (colorDialog.ShowDialog(this) == DialogResult.OK)
                {
                    cmdBackColor.BackColor = colorDialog.SelectedColor;
                    m_savedBackColor = ColorTranslator.ToOle(colorDialog.SelectedColor);
                    m_formChanged = true;
                }
            }
        }

        /// <summary>
        /// ボタン色変更ボタンクリック時のイベント
        /// </summary>
        private void cmdButtonColor_Click(object sender, EventArgs e)
        {
            using (var colorDialog = new FormColor())
            {
                colorDialog.SelectedColor = cmdButtonColor.BackColor;
                
                if (colorDialog.ShowDialog(this) == DialogResult.OK)
                {
                    cmdButtonColor.BackColor = colorDialog.SelectedColor;
                    m_savedButtonColor = ColorTranslator.ToOle(colorDialog.SelectedColor);
                    m_formChanged = true;
                }
            }
        }

        /// <summary>
        /// テキスト色変更ボタンクリック時のイベント
        /// </summary>
        private void cmdTextColor_Click(object sender, EventArgs e)
        {
            using (var colorDialog = new FormColor())
            {
                colorDialog.SelectedColor = cmdTextColor.BackColor;
                
                if (colorDialog.ShowDialog(this) == DialogResult.OK)
                {
                    cmdTextColor.BackColor = colorDialog.SelectedColor;
                    m_savedTextColor = ColorTranslator.ToOle(colorDialog.SelectedColor);
                    
                    // ボタン色によってテキスト色を変更
                    if (cmdTextColor.BackColor.GetBrightness() < 0.5)
                    {
                        cmdTextColor.ForeColor = Color.White;
                    }
                    else
                    {
                        cmdTextColor.ForeColor = Color.Black;
                    }
                    
                    m_formChanged = true;
                }
            }
        }

        /// <summary>
        /// ハイライトテキスト色変更ボタンクリック時のイベント
        /// </summary>
        private void cmdHighlightTextColor_Click(object sender, EventArgs e)
        {
            using (var colorDialog = new FormColor())
            {
                colorDialog.SelectedColor = cmdHighlightTextColor.BackColor;
                
                if (colorDialog.ShowDialog(this) == DialogResult.OK)
                {
                    cmdHighlightTextColor.BackColor = colorDialog.SelectedColor;
                    m_savedHighLightTextColor = ColorTranslator.ToOle(colorDialog.SelectedColor);
                    
                    // ボタン色によってテキスト色を変更
                    if (cmdHighlightTextColor.BackColor.GetBrightness() < 0.5)
                    {
                        cmdHighlightTextColor.ForeColor = Color.White;
                    }
                    else
                    {
                        cmdHighlightTextColor.ForeColor = Color.Black;
                    }
                    
                    m_formChanged = true;
                }
            }
        }
    }
}
