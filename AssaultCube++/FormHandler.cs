using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssaultCube__
{
    public class FormHandler
    {
        public static Form1 form;
        public static Thread formThread;
        public FormHandler()
        {
            createForm();
        }

        private void createForm()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
    public partial class Form1 : Form
    {
        public Form1()
        {
            CheckForIllegalCrossThreadCalls = false;
            FormHandler.form = this;
            FormHandler.formThread = new Thread(Main) { IsBackground = true };
            FormHandler.formThread.Start();            
        }

        public IntPtr id;
        public Color tranparentcolor = Color.Wheat;
        private void Main()
        {
            //this.TopMost = true;
            id = GetWindowName("ac_client");

            this.TransparencyKey = tranparentcolor;
            this.BackColor = tranparentcolor;

            //the border style
            this.FormBorderStyle = FormBorderStyle.None;

            int initialStyle = (int)GetWindowLongPtr(this.Handle, -20);
            SetWindowLong32(this.Handle, -20, initialStyle | 0x80000 | 0x20);

            while (true)
            {
                updatewindow();


                Thread.Sleep(20);
            }
        }

        private void updatewindow()
        {
            var wndRect = new Rect();
            //Console.WriteLine(wndid);
            GetWindowRect(id, ref wndRect);

            this.Width = wndRect.Right - wndRect.Left;
            this.Height = wndRect.Bottom - wndRect.Top;
            this.Top = wndRect.Top;
            this.Left = wndRect.Left;
        }

        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }

        private IntPtr GetWindowName(string wndname)
        {
            Process[] processes = Process.GetProcessesByName(wndname);

            if (processes.Count() != 0)
            {
                IntPtr WindowHandle = processes[0].MainWindowHandle;
                return WindowHandle;
            }
            else
            {
                Console.WriteLine("Game not found");
                return new IntPtr();
            }

        }

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);


        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);
    }
}
