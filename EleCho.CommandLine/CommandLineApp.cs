using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EleCho.CommandLine
{
    public abstract class CommandLineApp
    {
        private readonly List<CommandLineMethodInfo> methods;

        public CommandLineApp()
        {
            InitMethods(out methods);
        }

        private void InitSymbol(ParameterInfo paramInfo, SymbolAttribute symbolAttribute)
        {
            if (symbolAttribute is OptionAttribute optionAttribute)
            {
                if (paramInfo.Name == null)
                    return;

                if (string.IsNullOrWhiteSpace(optionAttribute.Name))
                    optionAttribute.Name = NamingUtils.ToKebabCase(paramInfo.Name);
            }
        }

        private void InitMethods(out List<CommandLineMethodInfo> methodInfos)
        {
            Type thisType = GetType();
            methodInfos = new List<CommandLineMethodInfo>();

            foreach (MethodInfo method in thisType.GetMethods())
            {
                if (method.GetCustomAttribute<CommandAttribute>() is not CommandAttribute commandAttribute)
                    continue;

                if (methodInfos.Any(methodInfo => methodInfo.Method.Name.Equals(method.Name)))
                    throw new InvalidOperationException($"Method '{method.Name}' is duplicated");

                ParameterInfo[] paramInfos = method.GetParameters();
                SymbolAttribute[] symbolAttributes = new SymbolAttribute[paramInfos.Length];

                for (int i = 0; i < paramInfos.Length; i++)
                {
                    ParameterInfo paramInfo = paramInfos[i];
                    if (paramInfo.GetCustomAttribute<SymbolAttribute>() is not SymbolAttribute symbolAttribute)
                    {
                        if (!CommandLineParser.IsConvertible(paramInfo))
                            throw new InvalidOperationException($"Method '{method.Name}', Parameter '{paramInfo.Name}', Type is not convertible type");

                        if (paramInfo.HasDefaultValue || paramInfo.ParameterType == typeof(bool))
                            symbolAttribute = new OptionAttribute();
                        else
                            symbolAttribute = new ValueAttribute();
                    }

                    InitSymbol(paramInfo, symbolAttribute);
                    symbolAttributes[i] = symbolAttribute;
                }

                methodInfos.Add(new CommandLineMethodInfo(method, paramInfos, commandAttribute, symbolAttributes));
            }
        }

        private void GetValueForArgument(
            string originValue, Type type, out object? paramValue,
            string commandName, ParameterInfo paramInfo, string? optionName)
        {
            try
            {
                paramValue = Convert.ChangeType(originValue, type);
            }
            catch (FormatException)
            {
                if (optionName != null)
                    throw new ArgumentException($"Command '{commandName}', Parameter '{paramInfo.Name}', Option '{optionName}' cannot convert to type '{paramInfo.ParameterType.Name}'");
                else
                    throw new ArgumentException($"Command '{commandName}', Parameter '{paramInfo.Name}', cannot convert to type '{paramInfo.ParameterType.Name}'");
            }
            catch (OverflowException)
            {
                if (optionName != null)
                    throw new ArgumentException($"Command '{commandName}', Parameter '{paramInfo.Name}', Option '{optionName}' number is to large or too small for type '{paramInfo.ParameterType.Name}'");
                else
                    throw new ArgumentException($"Command '{commandName}', Parameter '{paramInfo.Name}', number is to large or too small for type '{paramInfo.ParameterType.Name}'");
            }
        }

        private void FillValueForArgument(
            ParameterInfo paramInfo, SymbolAttribute symbolAttribute, List<CommandLineSegment> argvSegments, out object? paramValue,
            string commandName, StringComparison stringComparison)
        {
            if (symbolAttribute is OptionAttribute optionAttribute)
            {
                int optionSegmentIndex =
                        CommandLineParser.FindSegmentForOption(argvSegments, optionAttribute, stringComparison);

                if (optionSegmentIndex < 0)
                {
                    if (paramInfo.HasDefaultValue)
                        paramValue = paramInfo.DefaultValue;
                    else if (paramInfo.ParameterType == typeof(bool))
                        paramValue = false;
                    else
                        throw new ArgumentException($"Command '{commandName}', Parameter '{paramInfo.Name}' is required");
                }
                else
                {
                    CommandLineSegment optionSegment = argvSegments[optionSegmentIndex];

                    if (paramInfo.ParameterType == typeof(bool))
                    {
                        paramValue = true;
                        argvSegments.RemoveAt(optionSegmentIndex);    // remove option segment
                    }
                    else
                    {
                        int optionValueSegmentIndex = optionSegmentIndex + 1;
                        if (optionValueSegmentIndex >= argvSegments.Count)
                            throw new ArgumentException($"Command '{commandName}', Parameter '{paramInfo.Name}', Option '{optionSegment.OptionName}' value is required");

                        CommandLineSegment optionValueSegment = argvSegments[optionValueSegmentIndex];

                        GetValueForArgument(optionValueSegment.Value, paramInfo.ParameterType, out paramValue, commandName, paramInfo, optionSegment.OptionName);
                        argvSegments.RemoveAt(optionSegmentIndex);    // remove option segment
                        argvSegments.RemoveAt(optionSegmentIndex);    // remove option value segment
                    }
                }
            }
            else if (symbolAttribute is ValueAttribute valueAttribute)
            {
                if (paramInfo.IsParams())
                {
                    Type elementType = paramInfo.ParameterType.GetElementType()!;
                    List<object?> args = new List<object?>();
                    for (int j = 0; j < argvSegments.Count;)
                    {
                        CommandLineSegment segment = argvSegments[j];

                        if (!segment.IsValue)
                        {
                            j++;
                            continue;
                        }

                        object? arrElemValue;
                        GetValueForArgument(segment.Value, elementType, out arrElemValue, commandName, paramInfo, null);
                        argvSegments.RemoveAt(j);

                        args.Add(arrElemValue);
                    }

                    Array paramValueArr = Array.CreateInstance(elementType, args.Count);
                    for (int j = 0; j < args.Count; j++)
                        paramValueArr.SetValue(args[j], j);
                    paramValue = paramValueArr;
                }
                else
                {
                    int valueSegmentIndex =
                        CommandLineParser.FindSegmentForValue(argvSegments);

                    if (valueSegmentIndex < 0)
                        throw new ArgumentException($"Command '{commandName}', Parameter '{paramInfo.Name}' is required");

                    CommandLineSegment valueSegment = argvSegments[valueSegmentIndex];

                    GetValueForArgument(valueSegment.Value, paramInfo.ParameterType, out paramValue, commandName, paramInfo, null);
                    argvSegments.RemoveAt(valueSegmentIndex);    // remove value segment
                }
            }
            else
            {
                throw new InvalidOperationException($"Unknown SymbolAttribute '{symbolAttribute}'");
            }
        }

        public object? Execute(string commandline, StringComparison stringComparison)
        {
            CommandLineParser.Parse(commandline, out string commandName, out List<CommandLineSegment> argvSegments);

            CommandLineMethodInfo? methodInfo = methods.FirstOrDefault(m => m.Method.Name.Equals(commandName, stringComparison));
            if (methodInfo is null)
                throw new ArgumentException($"Command '{commandName}' not found");

            object?[] paramValues = new object?[methodInfo.Parameters.Length];

            // 先填充选项参数
            for (int i = 0; i < methodInfo.Parameters.Length; i++)
            {
                ParameterInfo paramInfo = methodInfo.Parameters[i];
                SymbolAttribute symbolAttribute = methodInfo.SymbolAttributes[i];

                if (symbolAttribute is not OptionAttribute)
                    continue;

                FillValueForArgument(paramInfo, symbolAttribute, argvSegments, out object? paramValue, commandName, stringComparison);
                paramValues[i] = paramValue;
            }

            // 再填充值参数
            for (int i = 0; i < methodInfo.Parameters.Length; i++)
            {
                ParameterInfo paramInfo = methodInfo.Parameters[i];
                SymbolAttribute symbolAttribute = methodInfo.SymbolAttributes[i];

                if (symbolAttribute is not ValueAttribute)
                    continue;

                FillValueForArgument(paramInfo, symbolAttribute, argvSegments, out object? paramValue, commandName, stringComparison);
                paramValues[i] = paramValue;
            }

            // 如果有多余的参数就报错
            foreach (var argvSegment in argvSegments)
            {
                if (argvSegment.IsOption)
                    throw new ArgumentException($"Command '{commandName}', Option '{argvSegment.OptionName}' is not defined");
                else
                    throw new ArgumentException($"Command '{commandName}', Value '{argvSegment.Value}' is unexpected");
            }

            return methodInfo.Method.Invoke(this, paramValues);
        }

        public object? Execute(string commandline)
        {
            return Execute(commandline, StringComparison.OrdinalIgnoreCase);
        }
    }
}