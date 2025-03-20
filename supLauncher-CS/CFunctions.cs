using System;

namespace HiMenu
{
    internal class CFunctions
    {
        private static CFunctions m_Obj = new CFunctions();

        public enum GetDriveFolderFileMode
        {
            PathName,
            CommandLineOption
        }

        public enum GetPathAndFileMode
        {
            PathName,
            FileName
        }

        private CFunctions()
        {
        }

        internal static CFunctions GetInstance()
        {
            return m_Obj;
        }

        /// <summary>
        /// ドライブ名＋フォルダ名＋ファイル名を取り出し
        /// </summary>
        internal string GetDriveFolderFile(string strCommandLine, GetDriveFolderFileMode Mode)
        {
            string strFileNameSave;
            int intPosition;

            if (strCommandLine.StartsWith("\"") && strCommandLine.IndexOf("\"", 1) != -1)
            {
                strFileNameSave = strCommandLine.Substring(1, strCommandLine.IndexOf("\"", 1) - 1);
                intPosition = strCommandLine.IndexOf("\"", 1) + 1;
            }
            else if (strCommandLine.StartsWith("'") && strCommandLine.IndexOf("'", 1) != -1)
            {
                strFileNameSave = strCommandLine.Substring(1, strCommandLine.IndexOf("'", 1) - 1);
                intPosition = strCommandLine.IndexOf("'", 1) + 1;
            }
            else
            {
                if (strCommandLine.IndexOf(" ") != -1)
                {
                    strFileNameSave = strCommandLine.Substring(0, strCommandLine.IndexOf(" "));
                    intPosition = strCommandLine.IndexOf(" ") + 1;
                }
                else
                {
                    strFileNameSave = strCommandLine;
                    intPosition = 0;
                }
            }

            if (strFileNameSave.Trim().IndexOf("/") != -1)
            {
                strFileNameSave = strFileNameSave.Trim().Substring(0, strFileNameSave.Trim().IndexOf("/"));
                intPosition = strFileNameSave.Trim().IndexOf("/");
            }

            if (Mode == GetDriveFolderFileMode.PathName)
            {
                return strFileNameSave;
            }
            else
            {
                if (intPosition == 0)
                {
                    return "";
                }
                else
                {
                    return strCommandLine.Trim().Substring(intPosition);
                }
            }
        }

        /// <summary>
        /// パス名・ファイル名を取り出し
        /// </summary>
        internal string GetPathAndFile(string strAllFileName, GetPathAndFileMode Mode)
        {
            int intLoopCnt;
            string strPathName;
            string strFileName;

            strPathName = "";
            strFileName = strAllFileName.Trim();
            for (intLoopCnt = strFileName.Length - 1; intLoopCnt >= 1; intLoopCnt--)
            {
                if (strFileName[intLoopCnt] == '\\')
                {
                    strPathName = strFileName.Substring(0, intLoopCnt);
                    strFileName = strFileName.Substring(intLoopCnt + 1);
                    break;
                }
                if (strFileName[intLoopCnt] == ':')
                {
                    strPathName = strFileName.Substring(0, intLoopCnt + 1);
                    strFileName = strFileName.Substring(intLoopCnt + 1);
                    break;
                }
            }

            if (Mode == GetPathAndFileMode.PathName)
            {
                return strPathName;
            }
            else
            {
                return strFileName;
            }
        }

        /// <summary>
        /// パス名に空白が含まれる場合にダブルコーテーションで囲む
        /// </summary>
        internal string QuateFullPath(string TargetPath)
        {
            if (TargetPath.IndexOf(" ") != -1)
            {
                return "\"" + TargetPath + "\"";
            }
            else
            {
                return TargetPath;
            }
        }
    }
}
