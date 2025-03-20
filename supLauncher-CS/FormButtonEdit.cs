using System;
using System.Windows.Forms;

namespace HiMenu
{
    public partial class FormButtonEdit : Form
    {
        private CMenuPage m_CMenuPage = CMenuPage.GetInstance();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FormButtonEdit()
        {
            InitializeComponent();
            this.Load += FormButtonEdit_Load;
            this.cmdOK.Click += cmdOK_Click;
            this.cmdBrowse.Click += cmdBrowse_Click;
        }

        /// <summary>
        /// フォームロード時のイベント
        /// </summary>
        private void FormButtonEdit_Load(object sender, EventArgs e)
        {
            int CurrentButton = m_CMenuPage.CurrentButton;

            CMenuPage.CMenuFileItemInf menuItem = m_CMenuPage.MenuFileItem(CurrentButton);

            txtTitle.Text = menuItem.Title;
            txtComment.Text = menuItem.Comment;
            txtCommand.Text = menuItem.Command;

            switch (menuItem.Attribute)
            {
                case CMenuPage.CMenuFileItemInf.ItemAttribute.ExecApplication:
                    optAttributeExec.Checked = true;
                    break;
                case CMenuPage.CMenuFileItemInf.ItemAttribute.OpenNextMenu:
                    optAttributeNext.Checked = true;
                    break;
                case CMenuPage.CMenuFileItemInf.ItemAttribute.BackPrevMenu:
                    optAttributeBack.Checked = true;
                    break;
            }

            switch (menuItem.After)
            {
                case CMenuPage.CMenuFileItemInf.ItemAfter.ContinueHiMenu:
                    optAfterContinue.Checked = true;
                    break;
                case CMenuPage.CMenuFileItemInf.ItemAfter.EndHiMenu:
                    optAfterEnd.Checked = true;
                    break;
                case CMenuPage.CMenuFileItemInf.ItemAfter.MinimizedHiMenu:
                    optAfterMinimize.Checked = true;
                    break;
            }

            chkNoUse.Checked = menuItem.NoUse;
        }

        /// <summary>
        /// OKボタンクリック時のイベント
        /// </summary>
        private void cmdOK_Click(object sender, EventArgs e)
        {
            int CurrentButton = m_CMenuPage.CurrentButton;

            CMenuPage.CMenuFileItemInf menuItem = m_CMenuPage.MenuFileItem(CurrentButton);

            menuItem.Title = txtTitle.Text;
            menuItem.Comment = txtComment.Text;
            menuItem.Command = txtCommand.Text;

            if (optAttributeExec.Checked)
            {
                menuItem.Attribute = CMenuPage.CMenuFileItemInf.ItemAttribute.ExecApplication;
            }
            else if (optAttributeNext.Checked)
            {
                menuItem.Attribute = CMenuPage.CMenuFileItemInf.ItemAttribute.OpenNextMenu;
            }
            else if (optAttributeBack.Checked)
            {
                menuItem.Attribute = CMenuPage.CMenuFileItemInf.ItemAttribute.BackPrevMenu;
            }

            if (optAfterContinue.Checked)
            {
                menuItem.After = CMenuPage.CMenuFileItemInf.ItemAfter.ContinueHiMenu;
            }
            else if (optAfterEnd.Checked)
            {
                menuItem.After = CMenuPage.CMenuFileItemInf.ItemAfter.EndHiMenu;
            }
            else if (optAfterMinimize.Checked)
            {
                menuItem.After = CMenuPage.CMenuFileItemInf.ItemAfter.MinimizedHiMenu;
            }

            menuItem.NoUse = chkNoUse.Checked;

            m_CMenuPage.MenuFileItem(CurrentButton) = menuItem;
        }

        /// <summary>
        /// 参照ボタンクリック時のイベント
        /// </summary>
        private void cmdBrowse_Click(object sender, EventArgs e)
        {
            using (var dlgOpen = new OpenFileDialog())
            {
                dlgOpen.Title = "実行するプログラムファイルまたはメニューファイルを指定してください";
                dlgOpen.Filter = "すべてのファイル (*.*)|*.*|実行ファイル(*.exe)|*.exe|メニューファイル (*.mnu)|*.mnu";
                dlgOpen.FilterIndex = 1;
                dlgOpen.InitialDirectory = Environment.CurrentDirectory;
                dlgOpen.FileName = "";

                if (dlgOpen.ShowDialog(this) == DialogResult.OK)
                {
                    if (optAttributeExec.Checked)
                    {
                        txtCommand.Text = dlgOpen.FileName;
                    }
                    else
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
                            txtCommand.Text = strFilePath.Substring(strCurrentDir.Length);
                        }
                        else
                        {
                            txtCommand.Text = strFilePath;
                        }
                    }
                }
            }
        }
    }
}
