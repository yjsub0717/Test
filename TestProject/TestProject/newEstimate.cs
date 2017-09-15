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
        private string str;

        public newEstimate(string str)
        {
            InitializeComponent();
            this.str = str;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            checkValidate();
        }

        private void textBox2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                checkValidate();
        }


        private void checkValidate()
        {
            using (MySqlConnection conn = new MySqlConnection(strConn))
            {
                if (textBox1.Text.Equals("") || textBox2.Text.Equals(""))
                {
                    label3.Text = "빈칸을 모두 채워주세요.";
                }
                else
                {
                    ds.Clear();
                    string sql = "SELECT * FROM `" + str + "` WHERE account = '" + textBox1.Text + "' AND date = '" + textBox2.Text + "'";
                    MySqlDataAdapter adpt = new MySqlDataAdapter(sql, conn);
                    adpt.Fill(ds);
                    conn.Close();

                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        name = textBox1.Text;
                        date = textBox2.Text;

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
}
