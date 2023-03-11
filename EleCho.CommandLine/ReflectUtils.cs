using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EleCho.CommandLine
{
    internal static class ReflectUtils
    {
        public static bool IsParams(this ParameterInfo parameterInfo) => parameterInfo.GetCustomAttribute<ParamArrayAttribute>() is not null;
    }
}
