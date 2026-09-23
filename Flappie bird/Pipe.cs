using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Flappie_bird
{
    public partial class Pipe : Form
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80;
                return cp;
            }
        }
        public Pipe()
        {
            InitializeComponent();
        }

        public void SetAdText(string text)
        {
            if (label1 != null)
            {
                label1.Text = text;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {
                
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }
    }
}
