using System;
using System.IO;
using CommandLine;

namespace gitfree
{
    public class Program
    {
        static void Main(string[] args)
        {
            var path = BackUpHosts();
            System.Console.WriteLine($"DNS配置({path})将被重写，原文件已备份在{path}");
            ReWriteHosts();
        }

        private static void ReWriteHosts()
        {
             var os = System.Environment.OSVersion.Platform;
            var path = Free.GetDNSPath(os);
                Free.Set();
        }

        private static string BackUpHosts()
        {
            var os = System.Environment.OSVersion.Platform;
            var hostPath = Free.GetDNSPath(os);
            var currentDir = Directory.GetCurrentDirectory();
            ;
            var path = Path.Combine(currentDir, $"Hostbackup_{DateTime.Now.ToString("mm_dd_hh_ss")}.text");
            File.Copy(hostPath,path);
            return path;
        }
    }
}
