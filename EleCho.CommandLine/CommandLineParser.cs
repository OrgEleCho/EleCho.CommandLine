using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EleCho.CommandLine
{

    public static class CommandLineParser
    {
        private static char escapeChar = '`';

        /// <summary>
        /// 是否是标识符
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsIdentifier(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            if (!char.IsLetter(text[0]) && text[0] != '_')
                return false;

            for (int i = 1; i < text.Length; i++)
            {
                if (!char.IsLetterOrDigit(text[i]) && text[i] != '_')
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 判断类型是否是可转换的
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsConvertible(Type type)
        {
            return typeof(IConvertible).IsAssignableFrom(type);
        }

        /// <summary>
        /// 判断类型是可转换的数组
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsConvertibleArray(Type type)
        {
            return type.IsArray && IsConvertible(type.GetElementType()!);
        }

        /// <summary>
        /// 判断参数是否是可转换的 (是可转换的基本类型, 是可变长参数且数组元素可转换
        /// </summary>
        /// <param name="paramInfo"></param>
        /// <param name="qwq"></param>
        /// <returns></returns>
        public static bool IsConvertible(ParameterInfo paramInfo, params string[] qwq)
        {
            if (IsConvertible(paramInfo.ParameterType))
                return true;

            if (paramInfo.GetCustomAttribute<ParamArrayAttribute>() != null)
                return IsConvertibleArray(paramInfo.ParameterType);

            return false;
        }

        /// <summary>
        /// 分割命令行
        /// </summary>
        /// <param name="commandline"></param>
        /// <param name="segments"></param>
        public static void Split(string commandline, out List<CommandLineSegment> segments)
        {
            List<CommandLineSegment> rstBuilder = new();
            StringBuilder temp = new();
            bool escape = false, quote = false;

            foreach (char i in commandline)
            {
                if (escape)
                {
                    escape = false;
                    temp.Append(i switch
                    {
                        'a' => '\a',
                        'b' => '\b',
                        'f' => '\f',
                        'n' => '\n',
                        'r' => '\r',
                        't' => '\t',
                        'v' => '\v',
                        _ => i
                    });
                }
                else
                {
                    if (i == escapeChar)
                    {
                        escape = true;
                    }
                    else
                    {
                        if (quote)
                        {
                            if (i == '"')
                            {
                                rstBuilder.Add(new CommandLineSegment(temp.ToString(), true));
                                temp.Clear();

                                quote = false;
                            }
                            else
                            {
                                temp.Append(i);
                            }
                        }
                        else
                        {
                            if (i == '"')
                            {
                                if (temp.Length > 0)
                                {
                                    rstBuilder.Add(new CommandLineSegment(temp.ToString(), false));
                                    temp.Clear();
                                }

                                quote = true;
                            }
                            else if (char.IsWhiteSpace(i))
                            {
                                if (temp.Length > 0)
                                {
                                    rstBuilder.Add(new CommandLineSegment(temp.ToString(), false));
                                    temp.Clear();
                                }
                            }
                            else
                            {
                                temp.Append(i);
                            }
                        }
                    }
                }
            }

            if (temp.Length > 0)
                rstBuilder.Add(new CommandLineSegment(temp.ToString(), quote));

            segments = rstBuilder;
        }

        /// <summary>
        /// 解析命令行
        /// </summary>
        /// <param name="commandLine"></param>
        /// <param name="commandName"></param>
        /// <param name="argvSegments"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void Parse(string commandLine, out string commandName, out List<CommandLineSegment> argvSegments)
        {
            Split(commandLine, out var segments);
            if (segments.Count == 0)
                throw new ArgumentException("Invalid commandline");

            var commandNameSegment = segments[0];

            if (commandNameSegment.IsQuoted)
                throw new ArgumentException("Invalid commandline, command name cannot be quoted");

            if (!IsIdentifier(commandNameSegment.Value))
                throw new ArgumentException("Invalid commandline, command name must be an identifier");

            commandName = commandNameSegment.Value;
            argvSegments = new List<CommandLineSegment>(segments.Count - 1);
            argvSegments.AddRange(segments.Skip(1));
        }

        public static int FindSegmentForOption(IList<CommandLineSegment> segments, OptionAttribute optionAttribute, StringComparison stringComparison)
        {
            for (int i = 0; i < segments.Count; i++)
            {
                CommandLineSegment s = segments[i];
                if (!s.IsOption)
                    continue;

                if (s.OptionName.Equals(optionAttribute.Name, stringComparison) ||
                    s.OptionShortName == optionAttribute.ShortName)
                    return i;
            }

            return -1;
        }

        public static int FindSegmentForValue(IList<CommandLineSegment> segments)
        {
            for (int i = 0; i < segments.Count; i++)
            {
                CommandLineSegment segment = segments[i];
                if (segment.IsValue)
                    return i;
            }

            return -1;
        }
    }
}