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
    public partial class product : Form
    {
        private int sortColumn = -1;

        String strConn = "Server=13.124.90.82; Port=3306; Database=rntp; Uid=root; Pwd=rntprntp;";
        DataSet ds = new DataSet();
        ThreadedSplashFormController<nowLoading, nowLoading.ProgressChangedEventArgs> splash = null;

        private String[] listview_columnTitle;

        public product()
        {
            InitializeComponent();
            this.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.TopLevel = false;

            comboBox1.SelectedIndex = 0;
            listview_columnTitle = new String[listView1.Columns.Count];

            for (int i = 0; i < listView1.Columns.Count; i++)
            {
                listview_columnTitle[i] = listView1.Columns[i].Text;
            }

            //printList();
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
                    insertCommand.Parameters.AddWithValue("@kg", addProduct.kg == "" ? null : addProduct.kg);
                    insertCommand.Parameters.AddWithValue("@ea", addProduct.ea == "" ? null : addProduct.ea);
                    insertCommand.Parameters.AddWithValue("@school_price", addProduct.str_school_price == "" ? null : addProduct.str_school_price);
                    insertCommand.Parameters.AddWithValue("@estimate_price", addProduct.str_estimate_price == "" ? null : addProduct.str_estimate_price);
                    insertCommand.Parameters.AddWithValue("@rate_1", addProduct.rate_1 == "" ? null : addProduct.rate_1);
                    insertCommand.Parameters.AddWithValue("@rate_2", addProduct.rate_2 == "" ? null : addProduct.rate_2);
                    insertCommand.Parameters.AddWithValue("@original_price", addProduct.str_original_price == "" ? null : addProduct.str_original_price);
                    insertCommand.Parameters.AddWithValue("@rate_original", addProduct.rate_original == "" ? null : addProduct.rate_original);
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
            splash = new ThreadedSplashFormController<nowLoading, nowLoading.ProgressChangedEventArgs>(x => x.ProgressChanged);
            splash.Show();
            nowLoading.ProgressChangedEventArgs p = new nowLoading.ProgressChangedEventArgs();

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
                conn.Close();
            }
            int maxNumber = ds.Tables[0].Rows.Count;
            int highestPercentageReached = 0;

            int percentComplete = 0;
            int i = 0;

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

                percentComplete = (int)((float)i / (float)maxNumber * 100);
                if (percentComplete > highestPercentageReached)
                {
                    p.Progress = percentComplete;
                    splash.OnProgressChanged(this, p);
                    highestPercentageReached = percentComplete;
                    //bw.ReportProgress(percentComplete);
                }
                i++;
            }
            ds.Clear();
            splash.Close();
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

        private void editProduct()
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
                            "', kg = " + EditProduct.kg +
                            ", ea = " + EditProduct.ea +
                            ", school_price = " + EditProduct.str_school_price +
                            ", estimate_price = " + EditProduct.str_estimate_price +
                            ", rate_1 = " + EditProduct.rate_1 +
                            ", rate_2 = " + EditProduct.rate_2 +
                            ", original_price = " + EditProduct.str_original_price +
                            ", rate_original = " + EditProduct.rate_original +
                            ", account = '" + EditProduct.account +
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


        // 수정 버튼
        private void button2_Click(object sender, EventArgs e)
        {
            editProduct();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            newProducts newForm = new newProducts();

            if (newForm.ShowDialog() == DialogResult.OK)
            {
                //printList();
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            editProduct();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                printList();
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (listView1.Items.Count < 2) return;
            splash = new ThreadedSplashFormController<nowLoading, nowLoading.ProgressChangedEventArgs>(x => x.ProgressChanged);
            splash.Show();
            nowLoading.ProgressChangedEventArgs p = new nowLoading.ProgressChangedEventArgs();

            if (e.Column != sortColumn)
            {
                sortColumn = e.Column;
                listView1.Sorting = SortOrder.Ascending;

                //if (sortColumn != 0)
                for (int i = 0; i < listView1.Columns.Count; i++)
                {
                    if (i == sortColumn)
                        listView1.Columns[i].Text = listview_columnTitle[i] + " ▲";
                    else
                        listView1.Columns[i].Text = listview_columnTitle[i];
                }
            }
            else
            {
                if (listView1.Sorting == SortOrder.Ascending)
                {
                    listView1.Sorting = SortOrder.Descending;
                    //if (sortColumn != 0)
                    for (int i = 0; i < listView1.Columns.Count; i++)
                    {
                        if (i == sortColumn)
                            listView1.Columns[i].Text = listview_columnTitle[i] + " ▼";
                        else
                            listView1.Columns[i].Text = listview_columnTitle[i];
                    }
                }
                else
                {
                    listView1.Sorting = SortOrder.Ascending;
                    //if (sortColumn != 0)
                    for (int i = 0; i < listView1.Columns.Count; i++)
                    {
                        if (i == sortColumn)
                            listView1.Columns[i].Text = listview_columnTitle[i] + " ▲";
                        else
                            listView1.Columns[i].Text = listview_columnTitle[i];
                    }
                }

            }

            listView1.Sort();
            bool isDigit = false;
            switch (sortColumn)
            {
                case 0:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                    isDigit = true;
                    break;
                default:
                    isDigit = false;
                    break;
            }

            this.listView1.ListViewItemSorter = new MyListViewComparer(e.Column, listView1.Sorting, isDigit);

            splash.Close();
        }


        private string textTrans(string str)
        {
            string result = "";

            if (str != "")
            {
                result = str.Replace(",", "");//숫자변환시 콤마로 발생하는 에러 방지
                result = String.Format("{0:#,###}", Convert.ToInt32(result));
            }

            return result;
        }

    }
}
