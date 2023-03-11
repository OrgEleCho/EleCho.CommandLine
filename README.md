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
            app.Execute(input, StringComparison.OrdinalIgnoreCase);

        if (rst != null)
            Console.WriteLine(rst);
    }
    catch(Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}
```