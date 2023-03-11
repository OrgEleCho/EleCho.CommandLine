using System.Reflection;

namespace EleCho.CommandLine
{
    public sealed record class CommandLineMethodInfo
    {
        public CommandLineMethodInfo(MethodInfo method, ParameterInfo[] parameters, CommandAttribute attribute, SymbolAttribute[] symbolAttributes)
        {
            this.Method = method;
            this.Parameters = parameters;
            this.Attribute = attribute;
            this.SymbolAttributes = symbolAttributes;
        }

        public MethodInfo Method { get; }
        public ParameterInfo[] Parameters { get; }
        public CommandAttribute Attribute { get; }
        public SymbolAttribute[] SymbolAttributes { get; }
    }
}