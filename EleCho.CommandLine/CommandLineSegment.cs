namespace EleCho.CommandLine
{
    public struct CommandLineSegment
    {
        public CommandLineSegment(string value, bool isQuoted)
        {
            Value = value;
            IsQuoted = isQuoted;
            IsOption = false;

            if (!isQuoted)
            {
                if (value.StartsWith("--"))
                {
                    IsOption = true;
                    OptionName = value.Substring(2);
                }
                else if (value.StartsWith("-"))
                {
                    IsOption = true;
                    if (value.Length != 2)
                        throw new ArgumentException("Invalid Option");
                    OptionShortName = value[1];
                }
            }
        }

        public string Value { get; }
        public bool IsQuoted { get; }
        public bool IsOption { get; }
        public bool IsValue => !IsOption;

        public char OptionShortName { get; } = '\0';
        public string OptionName { get; } = string.Empty;
    }
}