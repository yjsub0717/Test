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
    public partial class newAccount : Form
    {
        public string name = null;
        public string phone = null;
        public string cellphone = null;
        public string fax = null;
        public string shopname = null;
        public string shopid = null;
        public string address = null;

        public newAccount()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            name = textBox1.Text;
            phone = textBox2.Text;
            cellphone = textBox3.Text;
            fax = textBox4.Text;
            shopname = textBox5.Text;
            shopid = textBox6.Text;
            address = textBox7.Text;
        }

        
    }
}
