using System;
using System.Windows.Forms;

namespace HiMenu
{
    public partial class FormPassword : Form
    {
        private CMenuPage m_CMenuPage = CMenuPage.GetInstance();

        public enum ModeDefs
        {
            None,
            SetToLock,
            SetToUnlock
        }

        private ModeDefs m_Mode = ModeDefs.None;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FormPassword()
        {
            InitializeComponent();
            this.Load += FormPassword_Load;
            this.cmdOK.Click += cmdOK_Click;
        }

        /// <summary>
        /// 動作モード設定プロパティ
        /// </summary>
        public ModeDefs SetMode
        {
            get { return m_Mode; }
            set { m_Mode = value; }
        }

        /// <summary>
        /// フォームロード時のイベント
        /// </summary>
        private void FormPassword_Load(object sender, EventArgs e)
        {
            switch (m_Mode)
            {
                case ModeDefs.SetToLock:
                    // ロックする場合は両方のフィールドを表示（新しいパスワード設定）
                    this.Text = "パスワード設定";
                    lblPassword1.Text = "新しいパスワード:";
                    lblPassword2.Text = "パスワード確認:";
                    lblPassword2.Visible = true;
                    txtPasswordConfirm.Visible = true;
                    break;
                case ModeDefs.SetToUnlock:
                    // ロック解除の場合は1つめだけ表示（確認入力不要）
                    this.Text = "パスワード入力";
                    lblPassword1.Text = "パスワード:";
                    lblPassword2.Visible = false;
                    txtPasswordConfirm.Visible = false;
                    break;
                default:
                    // デフォルトはロック解除モード
                    m_Mode = ModeDefs.SetToUnlock;
                    this.Text = "パスワード入力";
                    lblPassword1.Text = "パスワード:";
                    lblPassword2.Visible = false;
                    txtPasswordConfirm.Visible = false;
                    break;
            }
        }

        /// <summary>
        /// OKボタンクリック時のイベント
        /// </summary>
        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text.Length == 0)
            {
                MessageBox.Show("パスワードが入力されていません", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.DialogResult = DialogResult.None;
                return;
            }

            switch (m_Mode)
            {
                case ModeDefs.SetToLock:
                    // パスワード設定モード（ロックする）
                    if (txtPassword.Text != txtPasswordConfirm.Text)
                    {
                        MessageBox.Show("確認用パスワードが一致しません", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.DialogResult = DialogResult.None;
                        return;
                    }
                    m_CMenuPage.LockPassword = txtPassword.Text;
                    break;

                case ModeDefs.SetToUnlock:
                    // パスワード確認モード（ロック解除）
                    if (txtPassword.Text != m_CMenuPage.LockPassword)
                    {
                        MessageBox.Show("パスワードが違います", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.DialogResult = DialogResult.None;
                        return;
                    }
                    break;
            }
        }
    }
}
