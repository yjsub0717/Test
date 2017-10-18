using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestProject
{
    public partial class alarm : Form
    {
        public alarm(string text, bool btn_OK)
        {
            InitializeComponent();

            label1.Text = text;

            if (btn_OK)
            {
                panel1.Visible = true;
                panel2.Visible = false;
            }
            else
            {
                panel2.Visible = true;
                panel1.Visible = false;
            }
        }
    }
}
