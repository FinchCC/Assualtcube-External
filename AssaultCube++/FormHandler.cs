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
        public LocalEntityBase p;
        public EntityList ents;
        public FormHandler(LocalEntityBase player, EntityList entities)
        {
            //formThread = new Thread(createForm) { IsBackground = true };
            //formThread.Start();
            //createForm();
            //formThread = new Thread(() => new Form1.ShowDialog()).Start();
            p = player;
            ents = entities;
            formThread = new Thread(new ThreadStart(createForm));
            formThread.Start();
        }

        private void createForm()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(p, ents));
        }
    }
    
}
