# EleCho.CommandLine

Simple commandline framework.

## Usage

Create a commandline app.

```csharp
class MyCommandLineApp : CommandLineApp
{
    [Command]
    public void Echo(string text, bool toUpper)
    {
        if (toUpper)
            text = text.ToUpper();

        Console.WriteLine(text);
    }
}
```

Execute commands from user input.

```csharp
MyCommandLineApp app = new MyCommandLineApp();
while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    if (input == null)
        return;

    try
    {
        object? rst =
            app.Execute(input);

        if (rst != null)
            Console.WriteLine(rst);
    }
    catch(Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}
```

Preview:

```txt
/echo "hello world"
hello world
/echo --to-upper "hello world"
HELLO WORLD
```

Use custom Option:

```csharp
[Command]
public void Echo(string text, bool toUpper, [Option("name")] string? myName = null)
{
    if (toUpper)
        text = text.ToUpper();

    if (myName != null)
        Console.Write($"{myName}: ");

    Console.WriteLine(text);
}
```

Use short name for Option:

```csharp
[Command]
public void Echo(string text, bool toUpper, [Option("name", 'n')] string? myName = null)
{
    if (toUpper)
        text = text.ToUpper();

    if (myName != null)
        Console.Write($"{myName}: ");

    Console.WriteLine(text);
}
```

Variable number of arguments are also supported:

```csharp
[Command]
public int Sum(params int[] nums)
{
    return nums.Sum();
}
```

> All basic types like numbers, string, char, enum are supported.