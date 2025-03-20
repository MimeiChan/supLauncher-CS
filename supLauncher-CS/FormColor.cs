using System;
using System.Drawing;
using System.Windows.Forms;

namespace HiMenu
{
    public partial class FormColor : Form
    {
        private Color m_SelectedColor = Color.White;
        private bool m_FormChanging = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FormColor()
        {
            InitializeComponent();
            this.Load += FormColor_Load;
            this.cmdStandard.Click += cmdStandard_Click;
            this.cmdOK.Click += cmdOK_Click;

            // イベントハンドラ設定
            this.hsbRed.ValueChanged += Slider_ValueChanged;
            this.hsbGreen.ValueChanged += Slider_ValueChanged;
            this.hsbBlue.ValueChanged += Slider_ValueChanged;

            this.txtRed.TextChanged += TextBox_TextChanged;
            this.txtGreen.TextChanged += TextBox_TextChanged;
            this.txtBlue.TextChanged += TextBox_TextChanged;
        }

        /// <summary>
        /// 選択された色
        /// </summary>
        public Color SelectedColor
        {
            get { return m_SelectedColor; }
            set { m_SelectedColor = value; }
        }

        /// <summary>
        /// フォームロード時のイベント
        /// </summary>
        private void FormColor_Load(object sender, EventArgs e)
        {
            // 色を表示
            SetColorToControls(SelectedColor);
        }

        /// <summary>
        /// 標準ボタンクリック時のイベント
        /// </summary>
        private void cmdStandard_Click(object sender, EventArgs e)
        {
            using (var colorDialog = new ColorDialog())
            {
                colorDialog.Color = SelectedColor;
                if (colorDialog.ShowDialog(this) == DialogResult.OK)
                {
                    SetColorToControls(colorDialog.Color);
                }
            }
        }

        /// <summary>
        /// OKボタンクリック時のイベント
        /// </summary>
        private void cmdOK_Click(object sender, EventArgs e)
        {
            // 現在の色を設定
            SelectedColor = GetColorFromControls();
        }

        /// <summary>
        /// スライダーの値が変更された時のイベント
        /// </summary>
        private void Slider_ValueChanged(object sender, EventArgs e)
        {
            if (m_FormChanging)
                return;

            m_FormChanging = true;

            // スライダーの値をテキストボックスに反映
            int red = hsbRed.Value;
            int green = hsbGreen.Value;
            int blue = hsbBlue.Value;

            txtRed.Text = red.ToString();
            txtGreen.Text = green.ToString();
            txtBlue.Text = blue.ToString();

            // プレビューを更新
            UpdatePreview();

            m_FormChanging = false;
        }

        /// <summary>
        /// テキストボックスの値が変更された時のイベント
        /// </summary>
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (m_FormChanging)
                return;

            m_FormChanging = true;

            // テキストボックスの値をスライダーに反映
            int red = 0;
            int green = 0;
            int blue = 0;

            // 無効な値のチェック
            if (!int.TryParse(txtRed.Text, out red) || red < 0 || red > 255)
            {
                red = 0;
                txtRed.Text = "0";
            }

            if (!int.TryParse(txtGreen.Text, out green) || green < 0 || green > 255)
            {
                green = 0;
                txtGreen.Text = "0";
            }

            if (!int.TryParse(txtBlue.Text, out blue) || blue < 0 || blue > 255)
            {
                blue = 0;
                txtBlue.Text = "0";
            }

            hsbRed.Value = red;
            hsbGreen.Value = green;
            hsbBlue.Value = blue;

            // プレビューを更新
            UpdatePreview();

            m_FormChanging = false;
        }

        /// <summary>
        /// 現在の色をコントロールに設定
        /// </summary>
        private void SetColorToControls(Color color)
        {
            m_FormChanging = true;

            hsbRed.Value = color.R;
            hsbGreen.Value = color.G;
            hsbBlue.Value = color.B;

            txtRed.Text = color.R.ToString();
            txtGreen.Text = color.G.ToString();
            txtBlue.Text = color.B.ToString();

            UpdatePreview();

            m_FormChanging = false;
        }

        /// <summary>
        /// コントロールから色を取得
        /// </summary>
        private Color GetColorFromControls()
        {
            int red = hsbRed.Value;
            int green = hsbGreen.Value;
            int blue = hsbBlue.Value;

            return Color.FromArgb(red, green, blue);
        }

        /// <summary>
        /// プレビュー更新
        /// </summary>
        private void UpdatePreview()
        {
            Color previewColor = GetColorFromControls();
            picPreview.BackColor = previewColor;
        }
    }
}
