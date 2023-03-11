using System;

namespace EleCho.CommandLine
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class ValueAttribute : SymbolAttribute
    {

    }
}