using System.Reflection;

namespace EleCho.CommandLine
{
    public sealed record class CommandLineMethodInfo(MethodInfo Method, ParameterInfo[] Parameters, CommandAttribute Attribute, SymbolAttribute[] SymbolAttributes);
}