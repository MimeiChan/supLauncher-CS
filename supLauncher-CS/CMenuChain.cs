using System.Collections.Generic;

namespace HiMenu
{
    /// <summary>
    /// メニュー画面遷移履歴を管理するクラス
    /// メニュー階層の履歴を保持し、前のメニューに戻るための情報を管理する
    /// </summary>
    internal class CMenuChain
    {
        /// <summary>
        /// シングルトンインスタンス
        /// </summary>
        private static CMenuChain m_Obj = new CMenuChain();

        /// <summary>
        /// メニューファイル名の検索用リスト（大文字変換済み）
        /// </summary>
        private List<string> m_MenuFileSearch = new List<string>();
        
        /// <summary>
        /// メニューファイル名のリスト（元の文字列）
        /// </summary>
        private List<string> m_MenuFile = new List<string>();
        
        /// <summary>
        /// 各メニュー画面での最後に選択されていた項目のインデックスリスト
        /// </summary>
        private List<int> m_LastItem = new List<int>();

        /// <summary>
        /// コンストラクタ（privateでシングルトンパターンを実現）
        /// </summary>
        private CMenuChain()
        {
        }

        /// <summary>
        /// シングルトンインスタンスを取得するメソッド
        /// </summary>
        /// <returns>CMenuChainのインスタンス</returns>
        internal static CMenuChain GetInstance()
        {
            return m_Obj;
        }

        /// <summary>
        /// メニュー階層履歴に現在のメニュー情報を追加する
        /// 同じメニューに戻った場合は、以降の履歴を削除する
        /// </summary>
        /// <param name="LastItem">現在のメニューで最後に選択されていた項目のインデックス</param>
        internal void Push(int LastItem)
        {
            string strMenuFileName = CMenuPage.GetInstance().MenuFileName;
            string strMenuFileNameSearch = strMenuFileName.ToUpper();

            int intFindItem = m_MenuFileSearch.LastIndexOf(strMenuFileNameSearch);

            if (intFindItem > 0)
            {
                for (int intLoop = m_MenuFileSearch.Count - 1; intLoop >= intFindItem; intLoop--)
                {
                    m_MenuFileSearch.RemoveAt(intLoop);
                    m_MenuFile.RemoveAt(intLoop);
                    m_LastItem.RemoveAt(intLoop);
                }
            }

            m_MenuFileSearch.Add(strMenuFileNameSearch);
            m_MenuFile.Add(strMenuFileName);
            m_LastItem.Add(LastItem);
        }

        /// <summary>
        /// メニュー階層履歴から一つ前のメニュー情報を取得し、履歴から削除する
        /// </summary>
        /// <returns>前のメニューで最後に選択されていた項目のインデックス</returns>
        internal int Pop()
        {
            int intIndex = m_MenuFileSearch.Count - 1;
            int intLastItem;

            if (intIndex >= 0)
            {
                CMenuPage.GetInstance().MenuFileName = m_MenuFile[intIndex];
                intLastItem = m_LastItem[intIndex];

                m_MenuFileSearch.RemoveAt(intIndex);
                m_MenuFile.RemoveAt(intIndex);
                m_LastItem.RemoveAt(intIndex);
            }
            else
            {
                CMenuPage.GetInstance().MenuFileName = "";
                intLastItem = -1;
            }

            return intLastItem;
        }

        /// <summary>
        /// 現在のメニューがルートメニュー（最上位階層）かどうかを判定する
        /// </summary>
        /// <returns>ルートメニューの場合はtrue、それ以外はfalse</returns>
        internal bool IsRootMenu()
        {
            if (m_MenuFileSearch.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
