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
    public partial class saleData : Form
    {
        DateTime CurrentDate1;
        DateTime CurrentDate2;

        public saleData()
        {
            InitializeComponent();
            this.TopLevel = false;
            dateTimePicker2.Value = new DateTime(int.Parse(DateTime.Now.ToString("yyyy")),
                                     int.Parse(DateTime.Now.ToString("MM")),
                                     DateTime.DaysInMonth(dateTimePicker2.Value.Year,dateTimePicker2.Value.Month));

            CurrentDate1 = dateTimePicker1.Value;
            CurrentDate2 = dateTimePicker2.Value;
        }

        private void dateTimePicker1_CloseUp(object sender, EventArgs e)
        {
            if (CurrentDate1.Month != dateTimePicker1.Value.Month)
            {
                DateTime temp = new DateTime(dateTimePicker1.Value.Year,
                                         dateTimePicker1.Value.Month,
                                         DateTime.DaysInMonth(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month));

                dateTimePicker2.Value = temp;
            }
            CurrentDate1 = dateTimePicker1.Value;
            dateTimePicker2.Select();
            SendKeys.Send("%{DOWN}");
        }

        private void dateTimePicker2_CloseUp(object sender, EventArgs e)
        {
            if ((CurrentDate2.Month != dateTimePicker2.Value.Month) || (dateTimePicker1.Value.Day >= dateTimePicker2.Value.Day))
            {
                DateTime temp = new DateTime(dateTimePicker2.Value.Year,
                                         dateTimePicker2.Value.Month,
                                         1);

                dateTimePicker1.Value = temp;
            }
            CurrentDate2 = dateTimePicker2.Value;
        }


    }
}
