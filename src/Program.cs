using System;
using CommandLine;

namespace gitfree
{
    public class Program
    {
        static void Main(string[] args)
        {

            Parser.Default.ParseArguments<SetOptions,ReSetOptions>(args) 
            .WithParsed<SetOptions>(setOpt => {
                var os = System.Environment.OSVersion.Platform;
                var path = Free.GetDNSPath(os);
                System.Console.WriteLine($"DNS配置({path})将被重写，还原请使用 gitfree --reset命令");
                Free.Set();
            })
            .WithParsed<ReSetOptions>(resetOpt => {
                System.Console.WriteLine("该功能将在后续添加...");
            });
        }
    }
    [Verb("set", HelpText = "配置github与gitlab的DNS")]
    public class SetOptions
    {

    }
    [Verb("reset", HelpText = "还原DNS配置")]
    public class ReSetOptions
    {
        
    }
}
