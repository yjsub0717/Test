using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace TestProject
{
    public partial class newEstimate : Form
    {
        String strConn = "Server=13.124.90.82; Port=3306; Database=rntp; Uid=root; Pwd=rntprntp;";
        DataSet ds = new DataSet();

        public string name;
        public string date;
        public Boolean launch; // true == 1, false == 0

        public newEstimate()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(strConn))
            {
                ds.Clear();
                string sql = null;
                if (radioButton1.Checked)
                    sql = "SELECT * FROM `estimateList` WHERE account = '" + textBox1.Text + "' AND date = '" + textBox2.Text + "' AND launch = 1";
                else
                    sql = "SELECT * FROM `estimateList` WHERE account = '" + textBox1.Text + "' AND date = '" + textBox2.Text + "' AND launch = 0";
                MySqlDataAdapter adpt = new MySqlDataAdapter(sql, conn);
                adpt.Fill(ds);
                conn.Close();

                if (ds.Tables[0].Rows.Count == 0)
                {
                    name = textBox1.Text;
                    date = textBox2.Text;
                    if (radioButton1.Checked) launch = true;
                    else launch = false;

                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    label3.Text = "중복된 견적서입니다.";
                }
            }
        }
    }
}
