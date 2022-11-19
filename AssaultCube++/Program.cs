using System;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;

namespace AssaultCube__
{
    public class Program
    {

        const int PROCESS_VM_READ = 0x0010;
        const int PROCESS_VM_WRITE = 0x0020;
        const int PROCESS_VM_OPERATION = 0x0008;

        public static IntPtr pHandleOverlay = IntPtr.Zero;
        public const string gamename = "ac_client";
        static int gamehandle;
        public static void Main()
        {

            gamehandle = GetWindowId(gamename);
            logV("gamehandle", gamehandle);


            pHandleOverlay = OpenProcessByHandle(gamehandle);
            //IntPtr ptr = (IntPtr)0x02ACA4E8;
            //long longValue = Marshal.ReadInt64(ptr);

            

            while (true)
            {
                int bytesRead = 0;
                byte[] buffer = new byte[4];
                ReadProcessMemory((int)pHandleOverlay, 0x02ACA4E8, buffer, buffer.Length, out bytesRead);

                int health = BitConverter.ToInt32(buffer, 0);

                logV("Health", health);

                Thread.Sleep(500);
            }

            //Console.WriteLine(longValue);
            Console.ReadLine();




        }


        public static int GetWindowId(string wndname)
        {
            Process[] prl = Process.GetProcessesByName(wndname);
            if (prl == null)
                return 0;

            Process pr = prl[0];

            if (pr != null)
                return pr.Id;

            Console.WriteLine("Game not found(call from id)");
            return 0;

        }

        public static void logV(string name, int value)
        {
            Console.WriteLine(name + " : " + value);
        }

        public static Process GetProcessFromId(int appid)
        {
            Console.WriteLine("aoppid: " + appid);
            return Process.GetProcessById(appid);
        }

        public static IntPtr OpenProcessByHandle(int processid)
        {
            return OpenProcess(PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_VM_OPERATION, true, GetProcessFromId(processid).Id);
        }


        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, [Out] byte[] lpBuffer, int dwSize,
    out int lpNumberOfBytesRead);

    }
}
