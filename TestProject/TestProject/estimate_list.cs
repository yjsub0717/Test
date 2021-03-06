﻿using System;
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
    public partial class estimate_list : Form
    {
        String strConn = "Server=13.124.90.82; Port=3306; Database=rntp; Uid=root; Pwd=rntprntp;";
        DataSet ds = new DataSet();

        public string name = null;
        public string date = null;
        public string bid_price = null;
        public string base_price = null;
        public int select = 0;

        bool estimate = true; // true : estimate, false : delivery

        public estimate_list(bool estimate)
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            printList();
            this.estimate = estimate;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            printList();
        }

        private void printList()
        {
            ds.Clear();
            using (MySqlConnection conn = new MySqlConnection(strConn))
            {
                string sql = "SELECT * FROM estimateList";
                if (textBox1.Text.Equals(""))
                {
                    sql = "SELECT * FROM estimateList";
                }
                else
                {
                    switch (comboBox1.SelectedIndex)
                    {
                        case 0:
                            sql = "SELECT * FROM `estimateList` WHERE `account` LIKE '%" + textBox1.Text + "%'";
                            break;
                        case 1:
                            sql = "SELECT * FROM `estimateList` WHERE `date` LIKE '%" + textBox1.Text + "%'";
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
                    row["bid"].ToString(),
                    row["base"].ToString()}));

            }
            ds.Clear();
        }

        private void estimate_list_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
                ListViewItem lvItem = items[0];
                if (estimate)
                {
                    name = lvItem.SubItems[2].Text;
                    date = lvItem.SubItems[3].Text;
                    bid_price = lvItem.SubItems[4].Text;
                    base_price = lvItem.SubItems[5].Text;

                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    ds.Clear();
                    using (MySqlConnection conn = new MySqlConnection(strConn))
                    {
                        string sql = "SELECT morning, launch, dinner FROM estimateList where account = '" + lvItem.SubItems[2].Text + "' AND date = '" + lvItem.SubItems[3].Text + "'";

                        MySqlDataAdapter adpt = new MySqlDataAdapter(sql, conn);
                        adpt.Fill(ds);
                        conn.Close();
                    }

                    if(ds.Tables[0].Rows.Count == 1)
                    {
                        newDelivery newForm = new newDelivery(ds.Tables[0].Rows[0]["morning"].ToString() != "" , ds.Tables[0].Rows[0]["launch"].ToString() != "", ds.Tables[0].Rows[0]["dinner"].ToString() != "");

                        if(newForm.ShowDialog() == DialogResult.OK)
                        {
                            name = lvItem.SubItems[2].Text;
                            date = lvItem.SubItems[3].Text;
                            this.select = newForm.select;
                            this.DialogResult = DialogResult.OK;
                        }
                    }
                    ds.Clear();
                }
            }
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                printList();
        }
    }
}
