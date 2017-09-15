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
    public partial class deliveryList : Form
    {
        String strConn = "Server=13.124.90.82; Port=3306; Database=rntp; Uid=root; Pwd=rntprntp;";
        DataSet ds = new DataSet();

        public string name = null;
        public string date = null;
        public Boolean launch = true;

        public deliveryList()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            printList();
        }

        private void printList()
        {
            ds.Clear();
            using (MySqlConnection conn = new MySqlConnection(strConn))
            {
                string sql = "SELECT * FROM deliveryList";
                if (textBox1.Text.Equals(""))
                {
                    sql = "SELECT * FROM deliveryList";
                }
                else
                {
                    switch (comboBox1.SelectedIndex)
                    {
                        case 0:
                            sql = "SELECT * FROM `deliveryList` WHERE `account` LIKE '%" + textBox1.Text + "%'";
                            break;
                        case 1:
                            sql = "SELECT * FROM `deliveryList` WHERE `date` LIKE '%" + textBox1.Text + "%'";
                            break;
                        default:
                            break;
                    }
                }
                MySqlDataAdapter adpt = new MySqlDataAdapter(sql, conn);
                adpt.Fill(ds);
                conn.Close();
            }

            listView1.Items.Clear();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                listView1.Items.Add(new ListViewItem(new string[] {"", row["id"].ToString(), 
                    row["account"].ToString(), 
                    row["date"].ToString(),
                    row["launch"].Equals(true) ? "중식" : "석식" }));

            }
            ds.Clear();
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
                ListViewItem lvItem = items[0];
                name = lvItem.SubItems[2].Text;
                date = lvItem.SubItems[3].Text;
                launch = lvItem.SubItems[4].Text.Equals("중식");

                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
