# EleCho.CommandLine [![nuget](https://img.shields.io/nuget/v/EleCho.CommandLine)](https://www.nuget.org/packages/EleCho.CommandLine)

简易命令行框架.

## 使用

创建一个命令行应用.

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

从用户输入执行命令.

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

预览:

```txt
/echo "hello world"
hello world
/echo --to-upper "hello world"
HELLO WORLD
```

使用自定义 Option:

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

对 Option 使用短名称:

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

可变长参数也是支持的:

```csharp
[Command]
public int Sum(params int[] nums)
{
    return nums.Sum();
}
```

> 所有基础类型诸如数字, 字符串, 字符, 枚举都是受支持的.