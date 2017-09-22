using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using MySql.Data.MySqlClient;

namespace TestProject
{
    public partial class indirect : Form
    {

        ListViewItem.ListViewSubItem SelectedLSI;
        bool isDigit = false;
        string name;
        string date;


        ThreadedSplashFormController<nowLoading, nowLoading.ProgressChangedEventArgs> splash = null;
        String strConn = "Server=13.124.90.82; Port=3306; Database=rntp; Uid=root; Pwd=rntprntp;";
        DataSet ds = new DataSet();

        public indirect()
        {
            InitializeComponent();
            this.TopLevel = false;
        }

        private void TxtEdit_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                HideTextEditor();
        }

        private void TxtEdit_Leave(object sender, EventArgs e)
        {

            HideTextEditor();
        }

        private void HideTextEditor()
        {
            //ListViewHitTestInfo i = listView1.HitTest(e.X, e.Y);

            //if (i.SubItem == i.Item.SubItems[6])
            //{
            TxtEdit.Visible = false;
            if (SelectedLSI != null)
            {
                if (isDigit) SelectedLSI.Text = textTrans(TxtEdit.Text);
                else  SelectedLSI.Text = TxtEdit.Text;
            }
               
            SelectedLSI = null;
            TxtEdit.Text = "";
            listView1.Focus();
            //}
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

        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo i = listView1.HitTest(e.X, e.Y);
            if (i.SubItem == null) return;
            if (i.SubItem == i.Item.SubItems[6] || i.SubItem == i.Item.SubItems[7])
            {
                SelectedLSI = i.SubItem;
                if (SelectedLSI == null)
                    return;

                int border = 0;
                switch (listView1.BorderStyle)
                {
                    case BorderStyle.FixedSingle:
                        border = 1;
                        break;
                    case BorderStyle.Fixed3D:
                        border = 2;
                        break;
                }

                int CellWidth = SelectedLSI.Bounds.Width;
                int CellHeight = SelectedLSI.Bounds.Height;
                int CellLeft = border + listView1.Left + i.SubItem.Bounds.Left;
                int CellTop = listView1.Top + i.SubItem.Bounds.Top;
                //int CellTop = SelectedLSI.Bounds.Top;
                // First Column
                if (i.SubItem == i.Item.SubItems[0])
                    CellWidth = listView1.Columns[0].Width;

                TxtEdit.Location = new Point(CellLeft, CellTop);
                TxtEdit.Size = new Size(CellWidth, CellHeight);
                TxtEdit.Visible = true;
                TxtEdit.BringToFront();
                TxtEdit.Text = i.SubItem.Text;
                TxtEdit.Select();
                TxtEdit.SelectAll();
                if (i.SubItem == i.Item.SubItems[6]) isDigit = true;
                else isDigit = false;
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
                ListViewItem lvItem = items[0];

                selectProduct newForm = new selectProduct();

                //newForm.setKeyword("");

                if (newForm.ShowDialog() == DialogResult.OK)
                {
                    lvItem.SubItems[1].Text = newForm.name;
                    lvItem.SubItems[2].Text = newForm.maker;
                    lvItem.SubItems[3].Text = newForm.standard;
                    lvItem.SubItems[4].Text = newForm.unit;
                    lvItem.SubItems[5].Text = newForm.tax;
                    lvItem.SubItems[6].Text = textTrans(newForm.str_school_price);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Add(new ListViewItem(new string[] { (listView1.Items.Count + 1).ToString(), "", "", "", "", "", "", "" }));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem selectedItem in listView1.SelectedItems)
            {
                listView1.Items.Remove(selectedItem);
            }
            int i = 1;
            foreach (ListViewItem item in listView1.Items)
            {
                item.SubItems[0].Text = i.ToString();
                i++;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();

            newEstimate newForm = new newEstimate("indirectList");

            if (newForm.ShowDialog() == DialogResult.OK)
            {
                this.name = newForm.name;
                this.date = newForm.date;

                listView1.Items.Add(new ListViewItem(new string[] { (listView1.Items.Count + 1).ToString(), "", "", "", "", "", "", "" }));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(strConn))
            {
                try
                {
                    conn.Open();

                    string sql = "SELECT * FROM `indirectList` WHERE account = '" + this.name + "' AND date = '" + this.date + "'";

                    ds.Clear();
                    MySqlDataAdapter adpt = new MySqlDataAdapter(sql, conn);
                    adpt.Fill(ds);

                    int id = 0;

                    if (ds.Tables[0].Rows.Count == 1)
                    {
                        id = (int)ds.Tables[0].Rows[0]["id"];

                        foreach (ListViewItem item in listView1.Items)
                        {
                            MySqlCommand insertCommand = new MySqlCommand();
                            insertCommand.Connection = conn;
                            sql = "UPDATE indirectItem set product_name = '" + item.SubItems[1].Text +
                            "', maker = ' " + item.SubItems[2].Text +
                            "', standard = '" + item.SubItems[3].Text +
                            "', unit = '" + item.SubItems[4].Text;

                            if (item.SubItems[5].Text.Equals(""))
                                sql += "', total = null";
                            else
                                sql += "', total = " + float.Parse(item.SubItems[5].Text);

                            if (item.SubItems[6].Text.Equals(""))
                                sql += ", original_price = null";
                            else
                                sql += ", original_price = " + float.Parse(item.SubItems[6].Text);

                            if (item.SubItems[7].Text.Equals(""))
                                sql += ", estimate_price = null";
                            else
                                sql += ", estimate_price = " + float.Parse(item.SubItems[7].Text);

                            if (item.SubItems[8].Text.Equals(""))
                                sql += ", school_price = null";
                            else
                                sql += ", school_price = " + float.Parse(item.SubItems[8].Text);

                            if (item.SubItems[9].Text.Equals(""))
                                sql += ", total_price = null";
                            else
                                sql += ", total_price = " + float.Parse(item.SubItems[9].Text);

                            //if (item.SubItems[10].Text.Equals(""))
                            //    sql += ", base_price = null";
                            //else
                            //    sql += ", base_price = " + float.Parse(item.SubItems[10].Text);

                            //if (item.SubItems[11].Text.Equals(""))
                            //    sql += ", bid_price = null";
                            //else
                            //    sql += ", bid_price = " + float.Parse(item.SubItems[11].Text);

                            //if (item.SubItems[12].Text.Equals(""))
                            //    sql += ", rate_bid = null";
                            //else
                            //    sql += ", rate_bid = " + float.Parse(item.SubItems[12].Text);

                            sql += ", name_excel = '" + item.SubItems[10].Text +
                            "', standard_excel = '" + item.SubItems[11].Text +
                            "', unit_excel = '" + item.SubItems[12].Text;

                            if (item.SubItems[13].Text.Equals(""))
                                sql += "', total_excel = null";
                            else
                                sql += "', total_excel = " + float.Parse(item.SubItems[13].Text);

                            sql += ", text_excel = '" + item.SubItems[14].Text +
                            "' where estimate_id=" + id + " AND no = " + item.SubItems[0].Text;

                            insertCommand.CommandText = sql;
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                    else if (ds.Tables[0].Rows.Count == 0)
                    {
                        MySqlCommand insertCommand = new MySqlCommand();
                        insertCommand.Connection = conn;
                        insertCommand.CommandText = "INSERT INTO estimateList(account, date, bid, base) VALUES(@account, @date, @bid, @base)";
                        insertCommand.Parameters.AddWithValue("@account", this.name);
                        insertCommand.Parameters.AddWithValue("@date", this.date);

                        insertCommand.ExecuteNonQuery();

                        sql = "SELECT * FROM `estimateList` WHERE account = '" + this.name + "' AND date = '" + this.date + "'";

                        ds.Clear();
                        adpt = new MySqlDataAdapter(sql, conn);
                        adpt.Fill(ds);

                        id = 0;

                        if (ds.Tables[0].Rows.Count == 1)
                        {
                            id = (int)ds.Tables[0].Rows[0]["id"];
                        }

                        foreach (ListViewItem item in listView1.Items)
                        {
                            insertCommand = new MySqlCommand();
                            insertCommand.Connection = conn;
                            insertCommand.CommandText = "INSERT INTO estimateItem(estimate_id, no, product_name, maker, standard, unit, total, original_price, estimate_price, school_price, total_price, name_excel, standard_excel, unit_excel, total_excel, text_excel) VALUES(@estimate_id, @no, @product_name, @maker, @standard, @unit, @total, @original_price, @estimate_price, @school_price, @total_price, @name_excel, @standard_excel, @unit_excel, @total_excel, @text_excel)";
                            insertCommand.Parameters.AddWithValue("@estimate_id", id);
                            insertCommand.Parameters.AddWithValue("@no", Int32.Parse(item.SubItems[0].Text));
                            insertCommand.Parameters.AddWithValue("@product_name", item.SubItems[1].Text);
                            insertCommand.Parameters.AddWithValue("@maker", item.SubItems[2].Text);
                            insertCommand.Parameters.AddWithValue("@standard", item.SubItems[3].Text);
                            insertCommand.Parameters.AddWithValue("@unit", item.SubItems[4].Text);
                            if (item.SubItems[5].Text.Equals(""))
                                insertCommand.Parameters.AddWithValue("@total", null);
                            else
                                insertCommand.Parameters.AddWithValue("@total", float.Parse(item.SubItems[5].Text));

                            if (item.SubItems[6].Text.Equals(""))
                                insertCommand.Parameters.AddWithValue("@original_price", null);
                            else
                                insertCommand.Parameters.AddWithValue("@original_price", float.Parse(item.SubItems[6].Text));

                            if (item.SubItems[7].Text.Equals(""))
                                insertCommand.Parameters.AddWithValue("@estimate_price", null);
                            else
                                insertCommand.Parameters.AddWithValue("@estimate_price", float.Parse(item.SubItems[7].Text));

                            if (item.SubItems[8].Text.Equals(""))
                                insertCommand.Parameters.AddWithValue("@school_price", null);
                            else
                                insertCommand.Parameters.AddWithValue("@school_price", float.Parse(item.SubItems[8].Text));
                            if (item.SubItems[9].Text.Equals(""))
                                insertCommand.Parameters.AddWithValue("@total_price", null);
                            else
                                insertCommand.Parameters.AddWithValue("@total_price", float.Parse(item.SubItems[9].Text));
                            //if (item.SubItems[10].Text.Equals(""))
                            //    insertCommand.Parameters.AddWithValue("@base_price", null);
                            //else
                            //    insertCommand.Parameters.AddWithValue("@base_price", float.Parse(item.SubItems[10].Text));
                            //if (item.SubItems[11].Text.Equals(""))
                            //    insertCommand.Parameters.AddWithValue("@bid_price", null);
                            //else
                            //    insertCommand.Parameters.AddWithValue("@bid_price", float.Parse(item.SubItems[11].Text));
                            //if (item.SubItems[12].Text.Equals(""))
                            //    insertCommand.Parameters.AddWithValue("@rate_bid", null);
                            //else
                            //    insertCommand.Parameters.AddWithValue("@rate_bid", float.Parse(item.SubItems[12].Text));
                            insertCommand.Parameters.AddWithValue("@name_excel", item.SubItems[10].Text);
                            insertCommand.Parameters.AddWithValue("@standard_excel", item.SubItems[11].Text);
                            insertCommand.Parameters.AddWithValue("@unit_excel", item.SubItems[12].Text);
                            if (item.SubItems[13].Text.Equals(""))
                                insertCommand.Parameters.AddWithValue("@total_excel", null);
                            else
                                insertCommand.Parameters.AddWithValue("@total_excel", float.Parse(item.SubItems[13].Text));
                            insertCommand.Parameters.AddWithValue("@text_excel", item.SubItems[14].Text);

                            insertCommand.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("성공적으로 저장되었습니다.");
                }
                catch (Exception eee)
                {
                    MessageBox.Show("저장에 실패하였습니다.");
                }

                conn.Close();
            }   
        }
    }
}
