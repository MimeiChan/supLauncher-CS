using System;

namespace HiMenu
{
    /// <summary>
    /// VB.NETの一部の関数をC#で再現するための拡張メソッド
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 文字列の左端から指定された文字数分の文字列を返します。
        /// </summary>
        public static string Left(this string str, int length)
        {
            if (str == null)
            {
                return null;
            }
            if (str.Length <= length)
            {
                return str;
            }
            return str.Substring(0, length);
        }

        /// <summary>
        /// 文字列の右端から指定された文字数分の文字列を返します。
        /// </summary>
        public static string Right(this string str, int length)
        {
            if (str == null)
            {
                return null;
            }
            if (str.Length <= length)
            {
                return str;
            }
            return str.Substring(str.Length - length);
        }

        /// <summary>
        /// 文字列の指定された位置から指定された文字数分の文字列を返します。
        /// </summary>
        public static string Mid(this string str, int startIndex, int length = -1)
        {
            if (str == null)
            {
                return null;
            }
            
            // VB.NETのMidは1から始まるので調整
            startIndex = startIndex - 1;
            
            if (startIndex < 0)
            {
                startIndex = 0;
            }
            
            if (startIndex >= str.Length)
            {
                return string.Empty;
            }
            
            if (length < 0 || (startIndex + length) > str.Length)
            {
                return str.Substring(startIndex);
            }
            
            return str.Substring(startIndex, length);
        }

        /// <summary>
        /// 指定した文字列の長さを返します。
        /// </summary>
        public static int Len(this string str)
        {
            if (str == null)
            {
                return 0;
            }
            return str.Length;
        }

        /// <summary>
        /// VB.NETのVal関数と同様に、文字列から数値を抽出します。
        /// </summary>
        public static double Val(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return 0;
            }
            
            // 数値の部分だけを抽出
            string numStr = string.Empty;
            bool foundDecimal = false;
            bool foundSign = false;
            
            foreach (char c in str)
            {
                if (char.IsDigit(c))
                {
                    numStr += c;
                }
                else if (c == '.' && !foundDecimal)
                {
                    numStr += c;
                    foundDecimal = true;
                }
                else if ((c == '-' || c == '+') && numStr.Length == 0 && !foundSign)
                {
                    numStr += c;
                    foundSign = true;
                }
                else if (numStr.Length > 0)
                {
                    break;
                }
            }
            
            if (string.IsNullOrEmpty(numStr))
            {
                return 0;
            }
            
            if (double.TryParse(numStr, out double result))
            {
                return result;
            }
            
            return 0;
        }

        /// <summary>
        /// 指定した値によって、値1または値2を返します（VB.NETのIIf関数相当）
        /// </summary>
        public static T IIf<T>(this bool condition, T trueValue, T falseValue)
        {
            return condition ? trueValue : falseValue;
        }

        /// <summary>
        /// 文字列が指定した文字で始まるかどうかを大文字小文字を区別せずに判定します。
        /// </summary>
        public static bool StartsWith(this string str, string value, StringComparison comparisonType)
        {
            if (str == null || value == null)
            {
                return false;
            }
            return str.StartsWith(value, comparisonType);
        }
    }
}
