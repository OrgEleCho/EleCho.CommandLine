using NullLib.ConsoleEx;
using EleCho.CommandLine;
using System.Text;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

MyCommandLineApp app = new MyCommandLineApp();

Task.Run(async () =>
{
    while (true)
    {
        await Task.Delay(1000);
        ConsoleSc.WriteLine("QWQ Running");
    }
});

ConsoleSc.Prompt = "> ";
while (true)
{
    var input = ConsoleSc.ReadLine();
    if (input == null)
        return;

    try
    {
        object? rst =
            app.Execute(input);

        if (rst != null)
            ConsoleSc.WriteLine($"{rst}");
    }
    catch(Exception ex)
    {
        ConsoleSc.WriteLine(ex.Message);
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

    [Command]
    public int Sum(params int[] nums)
    {
        return nums.Sum();
    }
}