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
        Boolean tax = true; // true : 과세, false : 면세

        public string name = null;
        public string standard = null;
        public string maker = null;
        public string unit = null;
        public string kg = null;
        public string ea = null;
        public string str_school_price = null;
        public string str_estimate_price = null;
        public string rate_1 = null;
        public string rate_2 = null;
        public string str_original_price = null;
        public string rate_original = null;
        public string account = null;
        public int i_tax = 1;

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
            float discountRate = 0;
            float discountRate2 = 0;
            try
            {
                if (Int32.TryParse(textBox8.Text, out school_price))
                {
                    if (float.TryParse(textBox10.Text, out discountRate))
                    {
                        if (float.TryParse(textBox9.Text, out discountRate2))
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
        private void origin_price_Calculate()
        {
            float discountRate = 0;
            try
            {
                if (Int32.TryParse(textBox8.Text, out school_price))
                {
                    if (float.TryParse(textBox12.Text, out discountRate))
                    {
                        origin_price = (int)(school_price * ((100 - discountRate) / 100.0));

                        textBox11.Text = origin_price.ToString();
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

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            origin_price_Calculate();
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            price_Calculate();
            origin_price_Calculate();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            tax = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            tax = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            name = textBox1.Text;
            maker = textBox2.Text;
            standard = textBox3.Text;
            unit = textBox4.Text;
            ea = textBox5.Text;
            kg = textBox6.Text;
            str_estimate_price = textBox7.Text;
            str_school_price = textBox8.Text;
            rate_2 = textBox9.Text;
            rate_1 = textBox10.Text;
            str_original_price = textBox11.Text;
            rate_original = textBox12.Text;
            account = textBox13.Text;
            i_tax = tax ? 1 : 0;
        }

        public void SetName(String param)
        {
            textBox1.Text = param;
        }

        public void SetMaker(String param)
        {
            textBox2.Text = param;
        }

        public void SetStandard(String param)
        {
            textBox3.Text = param;
        }

        public void SetUnit(String param)
        {
            textBox4.Text = param;
        }

        public void SetEa(String param)
        {
            textBox5.Text = param;
        }

        public void SetKg(String param)
        {
            textBox6.Text = param;
        }

        public void SetEstimatePrice(String param)
        {
            textBox7.Text = param;
        }

        public void SetSchoolPrice(String param)
        {
            textBox8.Text = param;
        }

        public void SetRate2(String param)
        {
            textBox9.Text = param;
        }

        public void SetRate1(String param)
        {
            textBox10.Text = param;
        }

        public void SetOriginalPrice(String param)
        {
            textBox11.Text = param;
        }

        public void SetRateOriginal(String param)
        {
            textBox12.Text = param;
        }

        public void SetAccount(String param)
        {
            textBox13.Text = param;
        }

        public void SetTax(String param)
        {
            if(param.Equals("과세"))
            {
                radioButton1.Checked = true;
            }
            else
            {
                radioButton2.Checked = true;
            }
        }


    }
}
