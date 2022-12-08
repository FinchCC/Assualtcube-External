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
        public FormHandler(LocalEntityBase player, EntityList entities)
        {
            createForm(player, entities);
        }

        private void createForm(LocalEntityBase player, EntityList ent)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(player, ent));
        }
    }
    
}
