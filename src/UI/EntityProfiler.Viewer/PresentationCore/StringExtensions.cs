using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace EntityProfiler.Viewer.PresentationCore
{
    [Flags]
    public enum EllipsisFormat
    {
        // Text is not modified.
        None = 0,
        // Text is trimmed at the end of the string. An ellipsis (...) 
        // is drawn in place of remaining text.
        End = 1,
        // Text is trimmed at the beginning of the string. 
        // An ellipsis (...) is drawn in place of remaining text. 
        Start = 2,
        // Text is trimmed in the middle of the string. 
        // An ellipsis (...) is drawn in place of remaining text.
        Middle = 3,
        // Preserve as much as possible of the drive and filename information. 
        // Must be combined with alignment information.
        Path = 4,
        // Text is trimmed at a word boundary. 
        // Must be combined with alignment information.
        Word = 8
    }

    public static class StringExtensions
    {

        /// <summary>
        /// Masks the username and password from a connection string
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        internal static string RemoveConnectionStringSecurity(this string connectionString)
        {
            var securityQualifiers = new[] { "user", "uid", "password", "pwd", "user id" };

            return securityQualifiers.Aggregate(connectionString, (current, qualifier)
                => Regex.Replace(current, qualifier + "\\s*=([^;]*)(?:$|;)", qualifier + "=********;", RegexOptions.IgnoreCase));
        }

        internal static string GetFriendlyName(this Type type)
        {
            if (type == typeof(int))
                return "int";
            else if (type == typeof(short))
                return "short";
            else if (type == typeof(byte))
                return "byte";
            else if (type == typeof(bool))
                return "bool";
            else if (type == typeof(long))
                return "long";
            else if (type == typeof(float))
                return "float";
            else if (type == typeof(double))
                return "double";
            else if (type == typeof(decimal))
                return "decimal";
            else if (type == typeof(string))
                return "string";
            else if (type.IsGenericType)
                return type.Name.Split('`')[0] + "<" + string.Join(", ", type.GetGenericArguments().Select(GetFriendlyName).ToArray()) + ">";
            else
                return type.Name;
        }

        internal static string ToSingleWordsSpace(this string text)
        {
            return Regex.Replace(text, @"\s+", " ");
        }
        
        private static readonly char[] _keepEndingChars = { '\"', '\'', ')', ']', '*', '@', '&' };
        internal static string PostfixLongLiteral(this string text, int maxLength = 128, string suffix = "...")
        {
            text = Regex.Replace(text, @"\s+", " ");
            if (maxLength < 3)
                throw new ArgumentOutOfRangeException("maxLength");

            var parts = text.Split(' ');
            var result = string.Empty;
            foreach (var part in parts)
            {
                var partLength = part.Length;
                if (partLength > maxLength)
                {
                    var end = suffix;
                    var penultimateChar = part[partLength - 2];
                    if (_keepEndingChars.Contains(penultimateChar))
                        end += penultimateChar;
                    var lastChar = part[partLength - 1];
                    if (_keepEndingChars.Contains(lastChar))
                        end += lastChar;
                    result += part.Substring(0, maxLength - end.Length) + end + " ";
                    continue;
                }
                result += part + " ";
            }
            return result;
        }

        public const string EllipsisChars = "...";
        private static readonly Regex _prevWord = new Regex(@"\W*\w*$");
        private static readonly Regex _nextWord = new Regex(@"\w*\W*");

        internal static string Ellipsis(this string text, int length, EllipsisFormat options = EllipsisFormat.Start | EllipsisFormat.None)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            // no aligment information
            if ((EllipsisFormat.Middle & options) == 0)
                return text;

            if (length <= 0)
                throw new ArgumentOutOfRangeException("length");

            // control is large enough to display the whole text
            if (text.Length <= length)
                return text;

            var pre = "";
            var mid = text;
            var post = "";

            var isPath = (EllipsisFormat.Path & options) != 0;

            // split path string into <drive><directory><filename>
            if (isPath)
            {
                pre = Path.GetPathRoot(text);
                mid = Path.GetDirectoryName(text).Substring(pre.Length);
                post = Path.GetFileName(text);
            }

            var len = 0;
            var seg = mid.Length;
            var fit = "";

            // find the longest string that fits into 
            // the control boundaries using bisection method
            while (seg > 1)
            {
                seg -= seg / 2;

                int left = len + seg;
                int right = mid.Length;

                if (left > right)
                    continue;

                if ((EllipsisFormat.Middle & options) == EllipsisFormat.Middle)
                {
                    right -= left / 2;
                    left -= left / 2;
                }
                else if ((EllipsisFormat.Start & options) != 0)
                {
                    right -= left;
                    left = 0;
                }

                // trim at a word boundary using regular expressions
                if ((EllipsisFormat.Word & options) != 0)
                {
                    if ((EllipsisFormat.End & options) != 0)
                    {
                        left -= _prevWord.Match(mid, 0, left).Length;
                    }
                    if ((EllipsisFormat.Start & options) != 0)
                    {
                        right += _nextWord.Match(mid, right).Length;
                    }
                }

                // build and measure a candidate string with ellipsis
                var tst = mid.Substring(0, left) + EllipsisChars + mid.Substring(right);

                // restore path with <drive> and <filename>
                if (isPath)
                {
                    tst = Path.Combine(Path.Combine(pre, tst), post);
                }
                //s = TextRenderer.MeasureText(dc, tst, ctrl.Font);

                // candidate string fits into control boundaries, try a longer string
                // stop when seg <= 1
                if (tst.Length <= length)
                {
                    len += seg;
                    fit = tst;
                }
            }

            if (len == 0) // string can't fit into control
            {
                // "path" mode is off, just return ellipsis characters
                if (!isPath)
                    return EllipsisChars;

                // <drive> and <directory> are empty, return <filename>
                if (pre.Length == 0 && mid.Length == 0)
                    return post;

                // measure "C:\...\filename.ext"
                fit = Path.Combine(Path.Combine(pre, EllipsisChars), post);

                // if still not fit then return "...\filename.ext"
                if (fit.Length > length)
                    fit = Path.Combine(EllipsisChars, post);
            }
            return fit;
        }
    }
}