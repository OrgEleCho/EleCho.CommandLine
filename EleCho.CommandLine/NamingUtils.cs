using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleCho.CommandLine
{
    internal static class NamingUtils
    {
        public static List<string> SplitWords(string origin)
        {
            List<string> words = new List<string>();
            StringBuilder sb = new StringBuilder();
            bool allUpperCase = true;

            for (int i = 0; i < origin.Length; i++)
            {
                char c = origin[i];
                if (char.IsUpper(c))
                {
                    if (!allUpperCase && sb.Length > 0)
                    {
                        words.Add(sb.ToString());
                        sb.Clear();
                        allUpperCase = true;
                    }

                    sb.Append(c);
                }
                else if (c == '-' || c == '_' || char.IsWhiteSpace(c))
                {
                    if (sb.Length > 0)
                    {
                        words.Add(sb.ToString());

                        sb.Clear();
                        allUpperCase = true;
                    }
                    continue;
                }
                else
                {
                    if (allUpperCase && sb.Length > 1)
                    {
                        char tail = sb[sb.Length - 1];
                        sb.Remove(sb.Length - 1, 1);

                        words.Add(sb.ToString());

                        sb.Clear();
                        sb.Append(tail);
                        sb.Append(c);
                        allUpperCase = false;
                    }
                    else
                    {
                        sb.Append(c);
                        allUpperCase = false;
                    } 
                }
            }

            if (sb.Length > 0)
                words.Add(sb.ToString());

            return words;
        }

        public static string ToKebabCase(string origin)
        {
            if (string.IsNullOrEmpty(origin))
                return origin;
 
            List<string> words = SplitWords(origin);
            return string.Join("-", words.Select(x => x.ToLower()));
        }
    }
}
