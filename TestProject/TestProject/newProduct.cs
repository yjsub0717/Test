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
    public partial class newProduct : Form
    {
        int school_price = 0;
        int price = 0;
        int origin_price = 0;



        public newProduct()
        {
            InitializeComponent();
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            price_Calculate();
        }

        private void price_Calculate()
        {
            int discountRate = 0;
            int discountRate2 = 0;
            try
            {
                if (Int32.TryParse(textBox8.Text, out school_price))
                {
                    if (Int32.TryParse(textBox10.Text, out discountRate))
                    {
                        if (Int32.TryParse(textBox9.Text, out discountRate2))
                        {
                            price = (int)((school_price * ((100 - discountRate) / 100.0)) * ((100 - discountRate2) / 100.0));

                            textBox7.Text = price.ToString();
                        }
                    }
                }
            }
            catch (Exception ee)
            {

            }
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            price_Calculate();
        }
    }
}
