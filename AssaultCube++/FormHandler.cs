using System;
using System.Collections.Generic;
using System.Linq;
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
            formThread = new Thread(createForm) { IsBackground = true };
            formThread.Start(); 
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
            FormHandler.form = this;
        }
    }
}
