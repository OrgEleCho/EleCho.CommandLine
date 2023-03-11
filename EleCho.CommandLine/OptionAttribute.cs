namespace EleCho.CommandLine
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class OptionAttribute : SymbolAttribute
    {
        public OptionAttribute()
        {

        }

        public OptionAttribute(string name)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            Name = name;
            if (name.Length > 1)
                ShortName = name[0];
        }

        public OptionAttribute(string name, char shortName)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ShortName = shortName;
        }

        public string Name { get; set; } = string.Empty;
        public char ShortName { get; set; } = '\0';
    }
}