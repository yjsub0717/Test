﻿using System;
using System.Collections;
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
    public partial class Account : Form
    {
        private int sortColumn = -1;
        String strConn = "Server=13.124.90.82; Port=3306; Database=rntp; Uid=root; Pwd=rntprntp;";
        DataSet ds = new DataSet();
        private String[] listview_columnTitle = { "", "거래처명", "전화번호", "휴대전화", "팩스", "사업자상호", "사업자번호", "주소" };

        public Account()
        {
            InitializeComponent();
            this.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.TopLevel = false;
            comboBox1.SelectedIndex = 0;;
            printList();
        }

        private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            Color c = Color.FromArgb(119,199,224);
            e.Graphics.FillRectangle(new SolidBrush(c), e.Bounds);
            //e.Graphics.FillRectangle(Brushes.Aqua, e.Bounds);
            e.DrawText();
        }


        // 신규 버튼
        private void button1_Click(object sender, EventArgs e)
        {
            newAccount addAccount = new newAccount();

            if (addAccount.ShowDialog() == DialogResult.OK)
            {
                using (MySqlConnection conn = new MySqlConnection(strConn))
                {
                    conn.Open();

                    MySqlCommand insertCommand = new MySqlCommand();
                    insertCommand.Connection = conn;
                    insertCommand.CommandText = "INSERT INTO account(name, phone, cellphone, fax, shopname, shopid, address) VALUES(@name, @phone, @cellphone, @fax, @shopname, @shopid, @address)";
                    insertCommand.Parameters.AddWithValue("@name", addAccount.name);
                    insertCommand.Parameters.AddWithValue("@phone", addAccount.phone);
                    insertCommand.Parameters.AddWithValue("@cellphone", addAccount.cellphone);
                    insertCommand.Parameters.AddWithValue("@fax", addAccount.fax);
                    insertCommand.Parameters.AddWithValue("@shopname", addAccount.shopname);
                    insertCommand.Parameters.AddWithValue("@shopid", addAccount.shopid);
                    insertCommand.Parameters.AddWithValue("@address", addAccount.address);

                    insertCommand.ExecuteNonQuery();

                    conn.Close();
                }

                //listView1.Items.Add(new ListViewItem(new string[] { (listView1.Items.Count + 1).ToString(), addAccount.name, addAccount.phone, addAccount.cellphone, addAccount.fax, addAccount.shopname, addAccount.shopid, addAccount.address }));
                printList();
            }
        }

        // 검색 버튼
        private void button4_Click(object sender, EventArgs e)
        {
            printList();
        }

        // 정렬
        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column != sortColumn)
            {
                sortColumn = e.Column;
                listView1.Sorting = SortOrder.Ascending;

                if (sortColumn != 0)
                    listView1.Columns[sortColumn].Text = listview_columnTitle[sortColumn] + " ▲";
            }
            else
            {
                if (listView1.Sorting == SortOrder.Ascending)
                {
                    listView1.Sorting = SortOrder.Descending;
                    if (sortColumn != 0)
                        listView1.Columns[sortColumn].Text = listview_columnTitle[sortColumn] + " ▼"; 
                }
                else
                {
                    listView1.Sorting = SortOrder.Ascending;
                    if (sortColumn != 0)
                        listView1.Columns[sortColumn].Text = listview_columnTitle[sortColumn] + " ▲";
                }

            }
            listView1.Sort();
            this.listView1.ListViewItemSorter = new MyListViewComparer(e.Column, listView1.Sorting);
        }

        // 삭제 버튼
        private void button3_Click(object sender, EventArgs e)
        {
            foreach(ListViewItem selectedItem in listView1.SelectedItems)
            {
                using (MySqlConnection conn = new MySqlConnection(strConn))
                {
                    conn.Open();

                    MySqlCommand insertCommand = new MySqlCommand();
                    insertCommand.Connection = conn;
                    insertCommand.CommandText = "DELETE FROM `account` WHERE `shopid` = '" + selectedItem.SubItems[6].Text + "'";

                    insertCommand.ExecuteNonQuery();

                    conn.Close();

                    //listView1.Items.Remove(selectedItem);
                }
                
            }
            printList();
        }

        // 수정 버튼
        private void button2_Click(object sender, EventArgs e)
        {
            newAccount EditAccount = new newAccount();

            if (listView1.SelectedItems.Count != 0)
            {
                using (MySqlConnection conn = new MySqlConnection(strConn))
                {
                    EditAccount.SetName(listView1.SelectedItems[0].SubItems[1].Text);
                    EditAccount.SetPhone(listView1.SelectedItems[0].SubItems[2].Text);
                    EditAccount.SetCellPhone(listView1.SelectedItems[0].SubItems[3].Text);
                    EditAccount.SetFax(listView1.SelectedItems[0].SubItems[4].Text);
                    EditAccount.SetShopName(listView1.SelectedItems[0].SubItems[5].Text);
                    EditAccount.SetShopId(listView1.SelectedItems[0].SubItems[6].Text);
                    EditAccount.SetAddress(listView1.SelectedItems[0].SubItems[7].Text);

                    conn.Open();

                    string sql = "SELECT * FROM `account` WHERE `shopid` LIKE '%" + listView1.SelectedItems[0].SubItems[6].Text + "%'";

                    MySqlDataAdapter adpt = new MySqlDataAdapter(sql, conn);
                    adpt.Fill(ds);

                    string id = null;
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        id = row["id"].ToString();
                        Console.WriteLine(id);
                    }

                    if (EditAccount.ShowDialog() == DialogResult.OK)
                    {

                        MySqlCommand insertCommand = new MySqlCommand();
                        insertCommand.Connection = conn;
                        //update info set id='kch' where id='admin';
                        insertCommand.CommandText = "UPDATE account set name = '" + EditAccount.name +
                            "', phone = ' " + EditAccount.phone +
                            "', cellphone = '" + EditAccount.cellphone +
                            "', fax = '" + EditAccount.fax +
                            "', shopname = '" + EditAccount.shopname +
                            "', shopid = '" + EditAccount.shopid +
                            "', address = '" + EditAccount.address +
                            "' where id="+ id;
                      //  insertCommand.Parameters.AddWithValue("@name", EditAccount.name);
                      //  insertCommand.Parameters.AddWithValue("@phone", EditAccount.phone);
                      //  insertCommand.Parameters.AddWithValue("@cellphone", EditAccount.cellphone);
                        //insertCommand.Parameters.AddWithValue("@fax", EditAccount.fax);
                        //insertCommand.Parameters.AddWithValue("@shopname", EditAccount.shopname);
                        //insertCommand.Parameters.AddWithValue("@shopid", EditAccount.shopid);
                        //insertCommand.Parameters.AddWithValue("@address", EditAccount.address);
                        //insertCommand.Parameters.AddWithValue("@id", id);

                        insertCommand.ExecuteNonQuery();


                        //listView1.Items.Add(new ListViewItem(new string[] { (listView1.Items.Count + 1).ToString(), addAccount.name, addAccount.phone, addAccount.cellphone, addAccount.fax, addAccount.shopname, addAccount.shopid, addAccount.address }));
                        printList();
                    }

                    conn.Close();
                }
            }
        }

        private void printList()
        {
            ds.Clear();
            using (MySqlConnection conn = new MySqlConnection(strConn))
            {
                string sql = "SELECT * FROM account";
                if (textBox1.Text.Equals(""))
                {
                    sql = "SELECT * FROM account";
                }
                else
                {
                    switch (comboBox1.SelectedIndex)
                    {
                        case 0:
                            sql = "SELECT * FROM `account` WHERE `name` LIKE '%" + textBox1.Text + "%'";
                            break;
                        case 1:
                            sql = "SELECT * FROM `account` WHERE `phone` LIKE '%" + textBox1.Text + "%'";
                            break;
                        case 2:
                            sql = "SELECT * FROM `account` WHERE `cellphone` LIKE '%" + textBox1.Text + "%'";
                            break;
                        case 3:
                            sql = "SELECT * FROM `account` WHERE `fax` LIKE '%" + textBox1.Text + "%'";
                            break;
                        case 4:
                            sql = "SELECT * FROM `account` WHERE `shopname` LIKE '%" + textBox1.Text + "%'";
                            break;
                        case 5:
                            sql = "SELECT * FROM `account` WHERE `shopid` LIKE '%" + textBox1.Text + "%'";
                            break;
                        case 6:
                            sql = "SELECT * FROM `account` WHERE `address` LIKE '%" + textBox1.Text + "%'";
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
                listView1.Items.Add(new ListViewItem(new string[] { (listView1.Items.Count + 1).ToString(), 
                    row["name"].ToString(), 
                    row["phone"].ToString(),
                    row["cellphone"].ToString(),
                    row["fax"].ToString(),
                    row["shopname"].ToString(),
                    row["shopid"].ToString(),
                    row["address"].ToString() }));

            }
            ds.Clear();
        }
    }


    class MyListViewComparer : IComparer
    {
        private int col;
        private SortOrder order;
        public MyListViewComparer()
        {
            col = 0;
            order = SortOrder.Ascending;
        }
        public MyListViewComparer(int column, SortOrder order)
        {
            col = column;
            this.order = order;
        }
        public int Compare(object x, object y)
        {
            int returnVal = -1;
            returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
                                    ((ListViewItem)y).SubItems[col].Text);
            // Determine whether the sort order is descending.
            if (order == SortOrder.Descending)
                // Invert the value returned by String.Compare.
                returnVal *= -1;
            return returnVal;
        }
    }
}
