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
    public partial class newDelivery : Form
    {
        public int select = 0; // 0 1 2 조 중 석


        public newDelivery(bool morning, bool launch, bool dinner)
        {
            InitializeComponent();

            if (dinner ) radioButton3.Enabled = false;
            else radioButton3.Checked = true;

            if (launch ) radioButton2.Enabled = false;
            else radioButton2.Checked = true;

            if (morning ) radioButton1.Enabled = false;
            else radioButton1.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked) select = 0;
            else if (radioButton2.Checked) select = 1;
            else select = 2;

            this.DialogResult = DialogResult.OK;
        }
    }
}
