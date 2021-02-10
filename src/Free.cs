using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace gitfree
{
    public class DNSInfo
    {
        public string type { get; set; }
        public string result { get; set; }
        public string ipaddress { get; set; }
        public double ttl { get; set; }
    }

    public class DNSRes
    {
        public int state { get; set; }
        public int id { get; set; }
        public List<DNSInfo> list { get; set; }
    }

    public class DNSFinder
    {

        private static int type = 1;
        private static int total = 10;
        private static int process = 6;
        private static int right = 1;

        private static string randId = "21577784";
        private static string randCallBackId = "jQuery111306995288151498782_1612855484422";
        private static string[] servers = new[]{
            "SdvPAD3yuuSxbfQd0I7XrQ==",
            "JACYvxRvL1|CnyK9sCL7~g==",
            "wfe~baph0aWgdKEg1reS7Q==",
            "Rv90~Ksj1L6304nG0ss~ew==",
            "rJ308W2|fyO0ceLbYQxTGg==",
            "nHVFzrsxoP9DQALMZq2FgQ==",
            "lkun8RYau8UDOnT7Ctg8Ww==",
            };
        private static string[] hosts = new[]
        {
            "gitlab.com",
            "github.com",
            "github.githubassets.com",
            "avatars8.githubusercontent.com",
            "avatars7.githubusercontent.com",
            "avatars6.githubusercontent.com",
            "avatars5.githubusercontent.com",
            "avatars4.githubusercontent.com",
            "avatars3.githubusercontent.com",
            "avatars2.githubusercontent.com",
            "avatars1.githubusercontent.com",
            "avatars0.githubusercontent.com",
            "avatars0.githubusercontent.com",
            "avatars0.githubusercontent.com",
            "avatars0.githubusercontent.com",
            "avatars0.githubusercontent.com",
            "avatars0.githubusercontent.com",
            "avatars.githubusercontent.com",
            "avatars.githubusercontent.com",
            "github.global.ssl.fastly.net",
            "camo.githubusercontent.com",
            "cloud.githubusercontent.com",
            "gist.githubusercontent.com",
            "raw.githubusercontent.com",
            "assets-cdn.github.com",
        };
        public static (string host, DNSInfo dns)[] FindFstests()
        {
           var tasks = hosts.Select(host => {
               var task = Task.Run(async() => {
                   var dns = await FindFastestAsync(host);
                    return (host,dns);
               });
               return task;
           });
           var allTask = Task.WhenAll(tasks);
           return allTask.Result.ToArray();
        }
        public static async Task<DNSInfo> FindFastestAsync(string host)
        {
            var dnsInfos = new List<DNSInfo>();
            foreach (var server in servers)
            {
                var url = $"http://tool.chinaz.com/AjaxSeo.aspx?t=dns&server={server}&id={randId}&callback={randCallBackId}";
                var req = WebRequest.CreateHttp(url);
                req.Method = "post";
                req.ContentType = "application/x-www-form-urlencoded";
                var postData = $"host={host}&type={type}&total={total}&process={process}&right={right}";

                byte[] data = Encoding.UTF8.GetBytes(postData);

                req.ContentLength = data.Length;

                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }
                HttpWebResponse res;
                Stream stream;
                try
                {
                    res = (HttpWebResponse)await req.GetResponseAsync();
                    stream = res.GetResponseStream();
                }
                catch (System.Exception)
                {
                    continue;
                }
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    var resStr = reader.ReadToEnd();
                    var leftIdx = resStr.IndexOf("{");

                    resStr = resStr.Substring(leftIdx);
                    resStr = resStr.Substring(0, resStr.Length - 1);
                    var dnsRes = JsonConvert.DeserializeObject<DNSRes>(resStr);
                    if (dnsRes.list != null)
                    {
                        dnsInfos.AddRange(dnsRes.list);
                    }
                }
            }

            var fastest = dnsInfos?.OrderBy(dns => dns.ttl)?.FirstOrDefault();
            System.Console.WriteLine($"{host} -> {fastest.result}:{fastest.ipaddress}:ttl{fastest.ttl}");
            return fastest;
        }
    }
    public class Free
    {
        // private static string _backUpPath = "host_backup.txt";
        public static void Set()
        {
            var os = System.Environment.OSVersion.Platform;
            var dnss = DNSFinder.FindFstests();
            var path = GetDNSPath(os);
            SetDNS(path, dnss);
            System.Console.WriteLine("DNS缓存已刷新，开始git clone 吧！ :)");
            FlushDNS(os);
        }
        public static string GetDNSPath(PlatformID os)
        {
            var path = "";
            switch (os)
            {
                case PlatformID.Unix:
                    path = "/etc/hosts";
                    break;
                case PlatformID.Win32NT:
                    path = @"C:\WINDOWS\system32\drivers\etc\Hosts";
                    break;
                default:
                    break;
            }
            return path;
        }


        private static void SetDNS(string path, (string host, DNSInfo dns)[] dnss)
        {
            var lines = File.ReadAllLines(path);
            var newLines = new List<string>();

            foreach (var line in lines)
            {
                bool contains = false;
                foreach (var host in dnss.Select(d => d.host).ToArray())
                {
                    if (line.Trim().ToUpper().Contains(host.Trim().ToUpper()))
                    {
                        contains = true;
                        break;
                    }
                }
                if (contains == false)
                {
                    newLines.Add(line);
                }
            }
            var needAdded = dnss.Select(d => $"{d.dns.result} {d.host}").ToArray();
            newLines.AddRange(needAdded);
            var text = string.Join(Environment.NewLine, newLines);
            File.WriteAllText(path, text);
        }
        private static void FlushDNS(PlatformID os)
        {
            if (os == PlatformID.Unix)
            {
                return;
            }
            else if (os == PlatformID.Win32NT)
            {
                Process p = new Process();
                //设置要启动的应用程序
                p.StartInfo.FileName = "cmd.exe";
                //是否使用操作系统shell启动
                p.StartInfo.UseShellExecute = false;
                // 接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardInput = true;
                //输出信息
                p.StartInfo.RedirectStandardOutput = true;
                // 输出错误
                p.StartInfo.RedirectStandardError = true;
                //不显示程序窗口
                p.StartInfo.CreateNoWindow = true;
                //启动程序
                p.Start();

                //向cmd窗口发送输入信息
                p.StandardInput.WriteLine("ipconfig/flushdns&exit");

                p.StandardInput.AutoFlush = true;

                //获取输出信息
                string strOuput = p.StandardOutput.ReadToEnd();
                //等待程序执行完退出进程
                p.WaitForExit();
                p.Close();
            }
            else
            {
                // System.Console.WriteLine("ignore");
            }
        }
    }
}