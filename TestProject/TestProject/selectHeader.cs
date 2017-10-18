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
    public partial class selectHeader : Form
    {
        public bool[] checkedList = new bool[16];

        public selectHeader()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            checkedList[0] = checkBox1.Checked;
            checkedList[1] = checkBox2.Checked;
            checkedList[2] = checkBox3.Checked;
            checkedList[3] = checkBox4.Checked;
            checkedList[4] = checkBox5.Checked;
            checkedList[5] = checkBox6.Checked;
            checkedList[6] = checkBox7.Checked;
            checkedList[7] = checkBox8.Checked;
            checkedList[8] = checkBox9.Checked;
            checkedList[9] = checkBox10.Checked;
            checkedList[10] = checkBox11.Checked;
            checkedList[11] = checkBox12.Checked;
            checkedList[12] = checkBox13.Checked;
            checkedList[13] = checkBox14.Checked;
            checkedList[14] = checkBox15.Checked;
            checkedList[15] = checkBox16.Checked;
        }
    }
}
