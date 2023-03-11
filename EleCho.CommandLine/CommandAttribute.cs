using System;

namespace EleCho.CommandLine
{
    [AttributeUsage(AttributeTargets.Method , AllowMultiple = false)]
    public class CommandAttribute : Attribute
    {

    }
}