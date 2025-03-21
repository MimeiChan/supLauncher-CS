using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace HiMenu
{
    /// <summary>
    /// C#とJavaScript間でデータをやり取りするためのブリッジクラス
    /// </summary>
    [ComVisible(true)]
    public class WebViewBridge
    {
        private readonly CMenuPage m_CMenuPage;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="menuPage">CMenuPageのインスタンス</param>
        public WebViewBridge(CMenuPage menuPage)
        {
            m_CMenuPage = menuPage;
        }

        /// <summary>
        /// すべてのメニュー項目を取得する
        /// </summary>
        /// <returns>メニュー項目のリスト</returns>
        public List<MenuItemDto> GetMenuItems()
        {
            var items = new List<MenuItemDto>();
            int totalItems = m_CMenuPage.MenuRows * m_CMenuPage.MenuCols;

            for (int i = 0; i < totalItems; i++)
            {
                var item = m_CMenuPage[i];
                items.Add(new MenuItemDto
                {
                    Title = item.Title,
                    Comment = item.Comment,
                    Command = item.Command,
                    Flag = item.Flag,
                    NoUse = item.NoUse,
                    Attribute = (int)item.Attribute,
                    After = (int)item.After
                });
            }

            return items;
        }

        /// <summary>
        /// 現在のロックモード（編集不可モード）状態を取得する
        /// </summary>
        /// <returns>ロック状態</returns>
        public bool IsLockMode()
        {
            return m_CMenuPage.LockOn;
        }

        /// <summary>
        /// キャンセルボタンのインデックスを取得する
        /// </summary>
        /// <returns>キャンセルボタンのインデックス</returns>
        public int GetCancelButtonIndex()
        {
            return m_CMenuPage.CancelButton;
        }

        /// <summary>
        /// テーマカラー情報を取得する
        /// </summary>
        /// <returns>テーマカラー情報</returns>
        public ThemeDto GetThemeColors()
        {
            return new ThemeDto
            {
                PrimaryColor = ColorTranslator.ToHtml(ColorTranslator.FromOle(m_CMenuPage.ButtonColor)),
                TextColor = ColorTranslator.ToHtml(ColorTranslator.FromOle(m_CMenuPage.TextColor)),
                HighlightTextColor = ColorTranslator.ToHtml(ColorTranslator.FromOle(m_CMenuPage.HighLightTextColor)),
                BackgroundColor = ColorTranslator.ToHtml(ColorTranslator.FromOle(m_CMenuPage.BackColor))
            };
        }
    }

    /// <summary>
    /// JavaScript用のメニュー項目DTOクラス
    /// </summary>
    [ComVisible(true)]
    public class MenuItemDto
    {
        public string Title { get; set; } = "";
        public string Comment { get; set; } = "";
        public string Command { get; set; } = "";
        public string Flag { get; set; } = "";
        public bool NoUse { get; set; }
        public int Attribute { get; set; }
        public int After { get; set; }
    }

    /// <summary>
    /// JavaScript用のテーマカラーDTOクラス
    /// </summary>
    [ComVisible(true)]
    public class ThemeDto
    {
        public string PrimaryColor { get; set; } = "#4361ee";
        public string PrimaryHover { get; set; } = "#3a56d4";
        public string SecondaryColor { get; set; } = "#f72585";
        public string TextColor { get; set; } = "#2b2d42";
        public string HighlightTextColor { get; set; } = "#ffffff";
        public string BackgroundColor { get; set; } = "#f8f9fa";
        public string ButtonText { get; set; } = "#ffffff";
    }

    /// <summary>
    /// WebViewからのメッセージを解析するためのクラス
    /// </summary>
    public class WebViewMessage
    {
        public string EventType { get; set; }
        public int ButtonIndex { get; set; }
        public ThemeDto Theme { get; set; }
        public string Command { get; set; }
    }
}