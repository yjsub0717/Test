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
    public partial class product : Form
    {
        String strConn = "Server=13.124.90.82; Port=3306; Database=rntp; Uid=root; Pwd=rntprntp;";
        DataSet ds = new DataSet();

        public product()
        {
            InitializeComponent();
            this.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.TopLevel = false;

            comboBox1.SelectedIndex = 0;
            printList();
        }

        private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            Color c = Color.FromArgb(119, 199, 224);
            e.Graphics.FillRectangle(new SolidBrush(c), e.Bounds);
            //e.Graphics.FillRectangle(Brushes.Aqua, e.Bounds);
            e.DrawText();
        }


        // 신규 버튼
        private void button1_Click(object sender, EventArgs e)
        {
            newProduct addProduct = new newProduct();

            if (addProduct.ShowDialog() == DialogResult.OK)
            {
                using (MySqlConnection conn = new MySqlConnection(strConn))
                {
                    conn.Open();

                    MySqlCommand insertCommand = new MySqlCommand();
                    insertCommand.Connection = conn;
                    insertCommand.CommandText = "INSERT INTO product(name, standard, maker, unit, kg, ea, school_price, estimate_price, rate_1, rate_2, original_price, rate_original, account, tax) VALUES(@name, @standard, @maker, @unit, @kg, @ea, @school_price, @estimate_price, @rate_1, @rate_2, @original_price, @rate_original, @account, @tax)";
                    insertCommand.Parameters.AddWithValue("@name", addProduct.name);
                    insertCommand.Parameters.AddWithValue("@standard", addProduct.standard);
                    insertCommand.Parameters.AddWithValue("@maker", addProduct.maker);
                    insertCommand.Parameters.AddWithValue("@unit", addProduct.unit);
                    insertCommand.Parameters.AddWithValue("@kg", addProduct.kg);
                    insertCommand.Parameters.AddWithValue("@ea", addProduct.ea);
                    insertCommand.Parameters.AddWithValue("@school_price", addProduct.str_school_price);
                    insertCommand.Parameters.AddWithValue("@estimate_price", addProduct.str_estimate_price);
                    insertCommand.Parameters.AddWithValue("@rate_1", addProduct.rate_1);
                    insertCommand.Parameters.AddWithValue("@rate_2", addProduct.rate_2);
                    insertCommand.Parameters.AddWithValue("@original_price", addProduct.str_original_price);
                    insertCommand.Parameters.AddWithValue("@rate_original", addProduct.rate_original);
                    insertCommand.Parameters.AddWithValue("@account", addProduct.account);
                    insertCommand.Parameters.AddWithValue("@tax", addProduct.i_tax);

                    insertCommand.ExecuteNonQuery();

                    conn.Close();
                }

                //listView1.Items.Add(new ListViewItem(new string[] { (listView1.Items.Count + 1).ToString(), addAccount.name, addAccount.phone, addAccount.cellphone, addAccount.fax, addAccount.shopname, addAccount.shopid, addAccount.address }));
                printList();
            }
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
                    row["kg"].ToString(),
                    row["ea"].ToString(),
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

        // 삭제 버튼
        private void button3_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem selectedItem in listView1.SelectedItems)
            {
                using (MySqlConnection conn = new MySqlConnection(strConn))
                {
                    conn.Open();

                    MySqlCommand insertCommand = new MySqlCommand();
                    insertCommand.Connection = conn;
                    insertCommand.CommandText = "DELETE FROM `product` WHERE `id` = " + selectedItem.SubItems[0].Text;

                    insertCommand.ExecuteNonQuery();

                    conn.Close();

                    //listView1.Items.Remove(selectedItem);
                }

            }
            printList();
        }

        // 검색 버튼
        private void button4_Click(object sender, EventArgs e)
        {
            printList();
        }

        // 수정 버튼
        private void button2_Click(object sender, EventArgs e)
        {
            newProduct EditProduct = new newProduct();

            if (listView1.SelectedItems.Count != 0)
            {
                using (MySqlConnection conn = new MySqlConnection(strConn))
                {
                    string id = listView1.SelectedItems[0].SubItems[0].Text;
                    EditProduct.SetName(listView1.SelectedItems[0].SubItems[1].Text);
                    EditProduct.SetStandard(listView1.SelectedItems[0].SubItems[2].Text);
                    EditProduct.SetMaker(listView1.SelectedItems[0].SubItems[3].Text);
                    EditProduct.SetUnit(listView1.SelectedItems[0].SubItems[4].Text);
                    EditProduct.SetKg(listView1.SelectedItems[0].SubItems[5].Text);
                    EditProduct.SetEa(listView1.SelectedItems[0].SubItems[6].Text);
                    EditProduct.SetSchoolPrice(listView1.SelectedItems[0].SubItems[7].Text);
                    EditProduct.SetEstimatePrice(listView1.SelectedItems[0].SubItems[8].Text);
                    EditProduct.SetRate1(listView1.SelectedItems[0].SubItems[9].Text);
                    EditProduct.SetRate2(listView1.SelectedItems[0].SubItems[10].Text);
                    EditProduct.SetOriginalPrice(listView1.SelectedItems[0].SubItems[11].Text);
                    EditProduct.SetRateOriginal(listView1.SelectedItems[0].SubItems[12].Text);
                    EditProduct.SetAccount(listView1.SelectedItems[0].SubItems[13].Text);
                    EditProduct.SetTax(listView1.SelectedItems[0].SubItems[14].Text);

                    conn.Open();

                    if (EditProduct.ShowDialog() == DialogResult.OK)
                    {

                        MySqlCommand insertCommand = new MySqlCommand();
                        insertCommand.Connection = conn;
                        //update info set id='kch' where id='admin';
                        insertCommand.CommandText = "UPDATE product set name = '" + EditProduct.name +
                            "', standard = ' " + EditProduct.standard +
                            "', maker = '" + EditProduct.maker +
                            "', unit = '" + EditProduct.unit +
                            "', kg = '" + EditProduct.kg +
                            "', ea = '" + EditProduct.ea +
                            "', school_price = '" + EditProduct.str_school_price +
                            "', estimate_price = '" + EditProduct.str_estimate_price +
                            "', rate_1 = '" + EditProduct.rate_1 +
                            "', rate_2 = '" + EditProduct.rate_2 +
                            "', original_price = '" + EditProduct.str_original_price +
                            "', rate_original = '" + EditProduct.rate_original +
                            "', account = '" + EditProduct.account +
                            "', tax = '" + EditProduct.i_tax +
                            "' where id=" + id;
                        //  insertCommand.Parameters.AddWithValue("@name", EditProduct.name);
                        //  insertCommand.Parameters.AddWithValue("@phone", EditProduct.phone);
                        //  insertCommand.Parameters.AddWithValue("@cellphone", EditProduct.cellphone);
                        //insertCommand.Parameters.AddWithValue("@fax", EditProduct.fax);
                        //insertCommand.Parameters.AddWithValue("@shopname", EditProduct.shopname);
                        //insertCommand.Parameters.AddWithValue("@shopid", EditProduct.shopid);
                        //insertCommand.Parameters.AddWithValue("@address", EditProduct.address);
                        //insertCommand.Parameters.AddWithValue("@id", id);

                        insertCommand.ExecuteNonQuery();


                        //listView1.Items.Add(new ListViewItem(new string[] { (listView1.Items.Count + 1).ToString(), addAccount.name, addAccount.phone, addAccount.cellphone, addAccount.fax, addAccount.shopname, addAccount.shopid, addAccount.address }));
                        printList();
                    }

                    conn.Close();
                }
            }
        }


    }
}
