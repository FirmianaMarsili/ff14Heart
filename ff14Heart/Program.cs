using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ff14Heart
{
    class Program
    {
        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);
        private static uint WM_KEYDOWN = 0x0100;
        private static uint WM_KEYUP = 0x0101;
        private static uint KEYCODE = 0x7C;
        private static IntPtr hWnd = IntPtr.Zero;
        private static string Tip
        {
            get
            {
                return string.Format("{0}{0}{0}输入指令操作{0}1  -  开启{0}0  -  关闭{0}-1  -  退出{0}{0}当前状态  -  {1}{0}{0}{0}", "\n", flag ? "开启" : "关闭");
            }
        }
        private static string CMD
        {
            get
            {
                return Console.ReadLine();
            }
        }
        private static string enabled_Result = "\n开启";
        private static string disabled_Result = "\n关闭";
        private static string none = "\n无效指令";
        private static Task heartTask = null;
        private static ManualResetEvent reset = new ManualResetEvent(true);
        private static bool flag = false;

        static void Main(string[] args)
        {
            Console.WriteLine(Tip);
            HandleCMD(CMD);
        }

        private static void HandleCMD(string cmd)
        {
            if (cmd == "-1")
            {
                Environment.Exit(0);
            }
            else if (cmd == "1")
            {
                if (flag)
                {
                    Console.WriteLine(none);
                    Console.WriteLine(Tip);
                    HandleCMD(CMD);
                    return;
                }
                flag = true;
                Console.WriteLine(enabled_Result);
                Console.WriteLine(Tip);
                if (heartTask == null)
                {
                    FindWindow();
                    heartTask = Task.Factory.StartNew(Post);
                }
                else
                {
                    if (hWnd == IntPtr.Zero)
                    {
                        FindWindow();
                    }
                    reset.Set();
                }
                HandleCMD(CMD);

            }
            else if (cmd == "0")
            {
                if (!flag)
                {
                    Console.WriteLine(none);
                    Console.WriteLine(Tip);
                    HandleCMD(CMD);
                    return;
                }
                flag = false;
                if (heartTask != null)
                {
                    reset.Reset();
                }
                Console.WriteLine(disabled_Result);
                Console.WriteLine(Tip);
                HandleCMD(CMD);
            }
            else
            {
                Console.WriteLine(none);
                Console.WriteLine(Tip);
                HandleCMD(CMD);
            }
        }

        private static void FindWindow()
        {
            hWnd = FindWindow("FFXIVGAME", "最终幻想XIV");
            if (hWnd == IntPtr.Zero)
            {
                Console.WriteLine("未找到游戏");
            }
        }
        private static void Post()
        {
            while (true)
            {
                reset.WaitOne();
                if (hWnd != IntPtr.Zero)
                {
                    bool flag1 = PostMessage(hWnd, WM_KEYDOWN, KEYCODE, 0);
                    bool flag2 = PostMessage(hWnd, WM_KEYUP, KEYCODE, 0);
                    if (!flag1 || !flag2)
                    {
                        hWnd = IntPtr.Zero;
                        FindWindow();
                    }
                }
                else
                {
                    if (flag)
                    {
                        FindWindow();
                    }
                }
                System.Threading.Thread.Sleep(0x927C0);
            }
        }
    }
}
