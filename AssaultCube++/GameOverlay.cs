using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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
        Thread formThread;
        Form1 f1;
        public Form1(LocalEntityBase player, EntityList ent)
        {
            this.player = player;
            this.entities = ent;
            CheckForIllegalCrossThreadCalls = false;
            FormHandler.form = this;
            f1 = this;
            formThread = new Thread(new ThreadStart(Main));
            formThread.Start();
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
            this.TopMost = true;

            this.ResizeRedraw = true;


            while (true)
            {
                updatewindow();

                //f1.Refresh();
                Draw();
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
            //g.Clip = new Region(new Rectangle(wndRect.Left, wndRect.Top, (wndRect.Right - wndRect.Left), (wndRect.Bottom - wndRect.Top)));
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

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Form1
            // 
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(800, 400);
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.ResumeLayout(false);

        }

        private Font f = new Font("Arial", 11);
        private SolidBrush b = new SolidBrush(Color.Green);
        private Graphics g;
        private Pen p = new Pen(Color.Yellow, 3f);

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void Draw()
        {
            g = CreateGraphics();
            g.Clear(tranparentcolor);


            if (entities.entities.Count == 0)
                return;

            int h = this.Height;
            int w = this.Width;
            try
            {
                List<EntityList.Entity> ents = entities.entities.ToList();
                //foreach (var ent in ents)
                //{
                //    if (ent == null)
                //        continue;

                //    vector3 feet3 = new vector3 { x = ent.x, y = ent.y, z = ent.z };
                //    Point feet = player.WorldToScreen(feet3, 1000, 800);
                //    vector3 head3 = new vector3 { x = ent.headx, y = ent.heady, z = ent.headz };
                //    Point head = player.WorldToScreen(head3, 1000, 800);

                //    if(feet.X != -99 && feet.Y != -99)
                //    {
                //        Point toDraw = new Point(500, 400);
                //        //g.DrawString(ent.Name, f, b, head);
                //        g.DrawLine(p, toDraw, feet);

                //    }
                //g.DrawLine(p, new Point(w / 2, h), feet)

                for (int i = 0; i < ents.Count; i++)
                {
                    if (ents[i].Health < 0 || ents[i].Health > 100)
                        continue;

                    var feetpos = new Vector3(ents[i].x, ents[i].y, ents[i].z);
                    Vector2 feetInScreen = player.WorldToScreen(feetpos, w, h);
                    Console.WriteLine("2d pos:" + ents[i].Name + " : " + feetInScreen.ToString());
                    Point pScreen = PointToScreen(new Point((int)feetInScreen.X, (int)feetInScreen.Y));
                    g.DrawLine(p, (w / 2), (h / 2), feetInScreen.X, feetInScreen.Y);
                    g.DrawString(ents[i].Name, f, b, new Point((int)feetInScreen.X, (int)feetInScreen.Y));
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Show();
        }

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);


        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);
    }
}
