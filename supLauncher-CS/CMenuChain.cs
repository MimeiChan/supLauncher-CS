using System.Collections.Generic;

namespace HiMenu
{
    internal class CMenuChain
    {
        private static CMenuChain m_Obj = new CMenuChain();

        private List<string> m_MenuFileSearch = new List<string>();
        private List<string> m_MenuFile = new List<string>();
        private List<int> m_LastItem = new List<int>();

        private CMenuChain()
        {
        }

        internal static CMenuChain GetInstance()
        {
            return m_Obj;
        }

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
