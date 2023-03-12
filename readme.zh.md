# EleCho.CommandLine [![nuget](https://img.shields.io/nuget/v/EleCho.CommandLine)](https://www.nuget.org/packages/EleCho.CommandLine)

���������п��.

## ʹ��

����һ��������Ӧ��.

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

���û�����ִ������.

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

Ԥ��:

```txt
/echo "hello world"
hello world
/echo --to-upper "hello world"
HELLO WORLD
```

ʹ���Զ��� Option:

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

�� Option ʹ�ö�����:

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

�ɱ䳤����Ҳ��֧�ֵ�:

```csharp
[Command]
public int Sum(params int[] nums)
{
    return nums.Sum();
}
```

> ���л���������������, �ַ���, �ַ�, ö�ٶ�����֧�ֵ�.