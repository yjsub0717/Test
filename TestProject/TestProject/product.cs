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
    public partial class product : Form
    {
        public product()
        {
            InitializeComponent();
            this.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.TopLevel = false;

            comboBox1.SelectedIndex = 0;
        }

        private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            Color c = Color.FromArgb(119, 199, 224);
            e.Graphics.FillRectangle(new SolidBrush(c), e.Bounds);
            //e.Graphics.FillRectangle(Brushes.Aqua, e.Bounds);
            e.DrawText();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            newProduct addProduct = new newProduct();

            if (addProduct.ShowDialog() == DialogResult.OK)
            {

            }
        }
    }
}
