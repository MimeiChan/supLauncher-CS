using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TheArtOfDev.HtmlRenderer.WinForms;

namespace HiMenu
{
    /// <summary>
    /// HTMLレンダリングを使用したメニュー表示クラス
    /// </summary>
    public class HtmlMenuRenderer
    {
        private readonly HtmlPanel htmlPanel;
        private readonly CMenuPage menuPage;
        private string cssContent;

        /// <summary>
        /// ボタンクリックイベントデリゲート
        /// </summary>
        /// <param name="sender">送信元オブジェクト</param>
        /// <param name="buttonIndex">クリックされたボタンのインデックス</param>
        public delegate void ButtonClickEventHandler(object sender, int buttonIndex);

        /// <summary>
        /// ボタンクリックイベント
        /// </summary>
        public event ButtonClickEventHandler ButtonClick;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="panel">HTMLパネルコントロール</param>
        /// <param name="menuPage">CMenuPageインスタンス</param>
        public HtmlMenuRenderer(HtmlPanel panel, CMenuPage menuPage)
        {
            this.htmlPanel = panel;
            this.menuPage = menuPage;
            this.htmlPanel.BaseStylesheet = LoadCssContent();
            
            // HTMLパネルのイベントハンドラを設定
            this.htmlPanel.Click += HtmlPanel_Click;
            this.htmlPanel.MouseMove += HtmlPanel_MouseMove;
        }

        /// <summary>
        /// HTMLメニューを更新
        /// </summary>
        public void RefreshMenu()
        {
            htmlPanel.Text = GenerateMenuHtml();
        }

        /// <summary>
        /// CSSファイルの内容を読み込み
        /// </summary>
        private string LoadCssContent()
        {
            try
            {
                string cssPath = Path.Combine(Application.StartupPath, "HtmlUI", "style.css");
                if (File.Exists(cssPath))
                {
                    cssContent = File.ReadAllText(cssPath);
                    return cssContent;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CSS読み込みエラー: " + ex.Message);
            }

            // デフォルトのCSS（CSSファイルが見つからない場合）
            cssContent = @"
                .menu-container { display: flex; flex-wrap: wrap; gap: 10px; padding: 10px; }
                .menu-button { 
                    display: inline-block; margin: 5px; padding: 15px; min-width: 120px; 
                    min-height: 60px; text-align: center; color: #ffffff; 
                    background-color: #4361ee; border-radius: 8px; cursor: pointer; 
                    font-weight: bold; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                }
                .menu-button:hover { background-color: #3a56d4; }
                .menu-button.hidden { opacity: 0.5; background-color: #6c757d; }
                .menu-button.escape { background-color: #f72585; }
            ";
            return cssContent;
        }

        /// <summary>
        /// メニューボタンのHTMLを生成
        /// </summary>
        private string GenerateMenuHtml()
        {
            var html = new StringBuilder();
            
            html.Append("<div class='menu-container'>");
            
            int totalItems = menuPage.MenuRows * menuPage.MenuCols;
            for (int i = 0; i < totalItems; i++)
            {
                var item = menuPage[i];
                
                if (menuPage.LockOn && item.NoUse)
                {
                    // 実行モードで非表示項目は表示しない
                    continue;
                }
                
                string buttonClass = "menu-button";
                string buttonText = item.Title;
                
                // 編集モードでの表示
                if (!menuPage.LockOn)
                {
                    if (menuPage.CancelButton == i)
                    {
                        buttonClass += " escape";
                        buttonText = "＜ＥＳＣ＞" + buttonText;
                    }
                    
                    if (item.NoUse)
                    {
                        buttonClass += " hidden";
                        buttonText = "＜非表示＞" + buttonText;
                    }
                }
                
                // ボタンHTMLを生成
                // href属性にJavaScriptコードを埋め込み、クリックイベントをトリガー
                html.AppendFormat(
                    "<a class='{0}' href=\"buttonClick:{1}\" title=\"{2}\">{3}</a>",
                    buttonClass,
                    i,
                    item.Comment,
                    buttonText
                );
            }
            
            html.Append("</div>");
            
            return html.ToString();
        }

        /// <summary>
        /// HTMLパネルのクリックイベントハンドラ
        /// </summary>
        private void HtmlPanel_Click(object sender, EventArgs e)
        {
            // クリックされたリンクを取得
            if (e is MouseEventArgs mouseEvent)
            {
                string link = htmlPanel.GetLinkAt(mouseEvent.Location);
                if (link != null && link.StartsWith("buttonClick:"))
                {
                    int buttonIndex = int.Parse(link.Substring("buttonClick:".Length));
                    
                    // イベントを発火
                    ButtonClick?.Invoke(this, buttonIndex);
                }
            }
        }

        /// <summary>
        /// HTMLパネルのマウス移動イベントハンドラ
        /// </summary>
        private void HtmlPanel_MouseMove(object sender, MouseEventArgs e)
        {
            // マウスホバー時のツールチップ表示など追加の機能をここに実装できます
        }

        /// <summary>
        /// リソースの解放
        /// </summary>
        public void Dispose()
        {
            htmlPanel.Click -= HtmlPanel_Click;
            htmlPanel.MouseMove -= HtmlPanel_MouseMove;
        }
    }
}