using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssaultCube__
{
    public partial class Form1 : Form
    {
        LocalEntityBase player;
        EntityList entities;
        public Form1(LocalEntityBase player, EntityList ent)
        {
            this.player = player;
            this.entities = ent;
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
            //this.FormBorderStyle = FormBorderStyle.None;

            int initialStyle = (int)GetWindowLongPtr(this.Handle, -20);
            SetWindowLong32(this.Handle, -20, initialStyle | 0x80000 | 0x20);

            while (true)
            {
                updatewindow();

                this.Invalidate();
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

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.ResumeLayout(false);

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (entities.entities.Count != 0)
                return;


            foreach (var ent in entities.entities)
            {
                //var feet3 = new vector3()
                Point feet = player.WorldToScreen(new vector3({ x = ent.x, y = ent.y, z = ent.z }), this.Height, this.Width);
                Point head = player.WorldToScreen(new vector3({ x = ent.headx, y = ent.heady, z = ent.headz } ), this.Height, this.Width);

            }


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
