using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace File_comp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.MdiParent = this;
            form2.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form frm in this.MdiChildren)
            {
                frm.Dispose();
                frm.Close();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            object[][] proj_inf = { Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false), Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false), Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false) };
            AssemblyTitleAttribute title = (AssemblyTitleAttribute)proj_inf[0][0];
            AssemblyProductAttribute prod = (AssemblyProductAttribute)proj_inf[1][0];
            string ver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            AssemblyCopyrightAttribute copy_r = (AssemblyCopyrightAttribute)proj_inf[2][0];
            MessageBox.Show(this, prod.Product + " " + ver + " " + copy_r.Copyright, title.Title);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "File compress!";
        }
    }
}
