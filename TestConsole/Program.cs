using EleCho.CommandLine;
using System.Text;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

MyCommandLineApp app = new MyCommandLineApp();
while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    if (input == null)
        return;

    try
    {
        object? rst = app.Execute(input, StringComparison.OrdinalIgnoreCase);

        if (rst != null)
        Console.WriteLine(rst);
    }
    catch(Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

class MyCommandLineApp : CommandLineApp
{
    [Command]
    public void Write(string text, bool toUpper)
    {
        if (toUpper)
            text = text.ToUpper();

        Console.WriteLine(text);
    }

    [Command]
    public int Add(int a, int b)
    {
        return a + b;
    }
}