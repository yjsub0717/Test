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
    public partial class selectProduct : Form
    {
        String strConn = "Server=13.124.90.82; Port=3306; Database=rntp; Uid=root; Pwd=rntprntp;";
        DataSet ds = new DataSet();

        public string name = null;
        public string standard = null;
        public string maker = null;
        public string unit = null;
        public string str_school_price = null;
        public string str_estimate_price = null;
        public string account = null;

        public selectProduct()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        private void printList()
        {
            ds.Clear();
            using (MySqlConnection conn = new MySqlConnection(strConn))
            {
                string sql = "SELECT * FROM product";
                if (textBox1.Text.Equals(""))
                {
                    sql = "SELECT * FROM product";
                }
                else
                {
                    switch (comboBox1.SelectedIndex)
                    {
                        case 0:
                            sql = "SELECT * FROM `product` WHERE `name` LIKE '%" + textBox1.Text + "%'";
                            break;
                        case 1:
                            sql = "SELECT * FROM `product` WHERE `maker` LIKE '%" + textBox1.Text + "%'";
                            break;
                        case 2:
                            sql = "SELECT * FROM `product` WHERE `account` LIKE '%" + textBox1.Text + "%'";
                            break;
                        default:
                            break;
                    }
                }
                MySqlDataAdapter adpt = new MySqlDataAdapter(sql, conn);
                adpt.Fill(ds);
            }

            listView1.Items.Clear();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                listView1.Items.Add(new ListViewItem(new string[] { row["id"].ToString(), 
                    row["name"].ToString(), 
                    row["standard"].ToString(),
                    row["maker"].ToString(),
                    row["unit"].ToString(),
                    row["school_price"].ToString(),
                    row["estimate_price"].ToString(),
                    row["rate_1"].ToString(),
                    row["rate_2"].ToString(),
                    row["original_price"].ToString(),
                    row["rate_original"].ToString(),
                    row["account"].ToString(),
                    row["tax"].Equals(true) ? "과세" : "면세" }));

            }
            ds.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            printList();
        }

        public void setKeyword(String str)
        {
            textBox1.Text = str;
            printList();
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
                ListViewItem lvItem = items[0];
                name = lvItem.SubItems[1].Text;
                standard = lvItem.SubItems[2].Text;
                maker = lvItem.SubItems[3].Text;
                unit = lvItem.SubItems[4].Text;
                str_school_price = lvItem.SubItems[5].Text;
                str_estimate_price = lvItem.SubItems[6].Text;

                this.DialogResult = DialogResult.OK;
            }
        }
    }

}
