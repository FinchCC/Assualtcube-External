using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AssaultCube__
{


    public partial class Program
    {

        const int PROCESS_VM_READ = 0x0010;
        const int PROCESS_VM_WRITE = 0x0020;
        const int PROCESS_VM_OPERATION = 0x0008;

        public static IntPtr pHandleOverlay = IntPtr.Zero;
        public const string gamename = "ac_client";

        static int gamehandle;
        public static EntityList entities;

        public static LocalEntityBase entity;
        public static FormHandler frmHandler;

        public static void Main()
        {
            frmHandler = new FormHandler();
            Control.CheckForIllegalCrossThreadCalls = false;
            gamehandle = GetWindowId(gamename);
            logV("gamehandle", gamehandle);


            pHandleOverlay = OpenProcessByHandle(gamehandle);
            //IntPtr ptr = (IntPtr)0x02ACA4E8;
            //long longValue = Marshal.ReadInt64(ptr);

            entity = new LocalEntityBase();
             entities = new EntityList(entity);

            Thread thread = new Thread(main) { IsBackground = true };
            thread.Start();
            while (true)
            {
                Console.WriteLine(readViewMatrix(LocalEntityBase.memory.viewMatrix).ToString());
                //int bytesRead = 0;
                //byte[] buffer = new byte[4];
                //ReadProcessMemory((int)pHandleOverlay, 0x02ACA4E8, buffer, buffer.Length, out bytesRead);

                //int health = BitConverter.ToInt32(buffer, 0);

                //logV("Health", health);

                //int health = entity.localHealth;
                //Thread.Sleep(5000);
                //entity.localHealth = 500;
                //vector3 cor = entity.getPos();
                ////Console.WriteLine(cor.getCords());
                //entities.UpdateEnteties();
                //entities.PrintAllEnteties();
                Console.ReadLine();
            }

            //Console.WriteLine(longValue);
            Console.ReadLine();




        }

        public static void main()
        {
            while(true)
            {
                entities.UpdateEnteties();
                entity.updateEnt();


                if(GetAsyncKeyState(Keys.Q)<0)
                {
                    if(entities.entities.Count != 0)
                    {
                        EntityList.Entity target = null;
                        for (int i = 0; i < entities.entities.Count; i++)
                        {
                            if (entities.entities[i].teamid == entity.teamid)
                                continue;
                            if (i == 0 || target == null)
                                target = entities.entities[i];
                            else if (entities.entities[i].magnitude < target.magnitude)
                                target = entities.entities[i];
                        }

                        var angles = LocalEntityBase.CalcAngles(target, entity);
                        entity.setAim(entity, angles);
                    }
                }
                if (GetAsyncKeyState(Keys.C) < 0)
                {
                    if (entities.entities.Count != 0)
                    {
                        EntityList.Entity target = null;
                        for (int i = 0; i < entities.entities.Count; i++)
                        {
                            if (entities.entities[i].teamid == entity.teamid)
                                continue;
                            if (i == 0 || target == null)
                                target = entities.entities[i];
                            else if (entities.entities[i].magnitude < target.magnitude)
                                target = entities.entities[i];
                        }

                        var angles = LocalEntityBase.CalcAngles(target, entity);
                        entity.setSilentAim(entity, angles);
                    }
                }
                Thread.Sleep(25);
            }
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

        public static int ReadInt(int adress)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[4];
            ReadProcessMemory((int)pHandleOverlay, adress, buffer, buffer.Length, out bytesRead);

            int num = BitConverter.ToInt32(buffer, 0);
            //logV("Health", num);
            return num;
        }

        public static Matrix4x4 readViewMatrix(int adress)
        {
            byte[] buffer = new byte[sizeof(float) * 16];
            int bytesRead = 0;

            ReadProcessMemory((int)pHandleOverlay, adress, buffer, buffer.Length, out bytesRead);

            float[] matrixlist = new float[sizeof(float) * 16];
            for (int i = 0; i < 16; i++)
            {
                float f = BitConverter.ToSingle(buffer, i);
                matrixlist[i] = f;
            }

            Matrix4x4 matrix = new Matrix4x4(matrixlist[0], matrixlist[1], matrixlist[2], matrixlist[3],
                matrixlist[4], matrixlist[5], matrixlist[6], matrixlist[7], matrixlist[8], matrixlist[9],
                matrixlist[10], matrixlist[11], matrixlist[12], matrixlist[13], matrixlist[14],
                matrixlist[15]);

            return matrix;

        }
        public static float ReadFloat(int adress)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[sizeof(float)];
            ReadProcessMemory((int)pHandleOverlay, adress, buffer, buffer.Length, out bytesRead);

            float num = BitConverter.ToSingle(buffer, 0);
            //logV("Float", Convert.ToInt32(num));
            return num;
        }

        public static string ReadString(int adress)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[16];
            ReadProcessMemory((int)pHandleOverlay, adress, buffer, buffer.Length, out bytesRead);

            string s = UTF8Encoding.UTF8.GetString(buffer).Trim();
            //logV("Float", Convert.ToInt32(num));
            return s;
        }

        public static void WriteInt(int adress, int value)
        {
            int bytesRead = 0;
            var toWrite = new byte[] { (byte)value };
            int s = 0;
            bool sucess = WriteProcessMemory((int)pHandleOverlay, adress, toWrite, toWrite.Length, out bytesRead);
            if (sucess)
                s = 1;
            logV("Write Sucess", s);

        }

        public static void WriteFloat(int adress, float value)
        {
            int bytesRead = 0;
            var toWrite = BitConverter.GetBytes(value);
            int s = 0;
            bool sucess = WriteProcessMemory((int)pHandleOverlay, adress, toWrite, toWrite.Length, out bytesRead);
            if (sucess)
                s = 1;
            logV("Write Sucess", s);

        }

        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(Keys key);

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


        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize,
    out int lpNumberOfBytesRead);

    }
}
