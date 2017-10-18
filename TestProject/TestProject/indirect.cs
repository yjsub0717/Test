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
        int product_id = -1;


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
            if (i.Item.SubItems[2].Text == "") return;
            if ((i.SubItem == i.Item.SubItems[7] || i.SubItem == i.Item.SubItems[8]) || i.SubItem == i.Item.SubItems[6])
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
                if (i.SubItem == i.Item.SubItems[1])
                    CellWidth = listView1.Columns[1].Width;

                if (i.SubItem == i.Item.SubItems[6])
                {
                    comboBox1.Location = new Point(CellLeft, CellTop);
                    comboBox1.Size = new Size(CellWidth, CellHeight);
                    comboBox1.Visible = true;
                    comboBox1.BringToFront();
                    if (i.SubItem.Text == "과세")
                        comboBox1.SelectedIndex = 0;
                    else
                        comboBox1.SelectedIndex = 1;
                    comboBox1.DroppedDown = true;
                    if (i.Item.SubItems[0].Text != "")
                        product_id = Int32.Parse(i.Item.SubItems[0].Text);
                    else
                        product_id = -1;
                }
                else
                {
                    TxtEdit.Location = new Point(CellLeft, CellTop);
                    TxtEdit.Size = new Size(CellWidth, CellHeight);
                    TxtEdit.Visible = true;
                    TxtEdit.BringToFront();
                    TxtEdit.Text = i.SubItem.Text;
                    TxtEdit.Select();
                    TxtEdit.SelectAll();
                }
                if (i.SubItem == i.Item.SubItems[7]) isDigit = true;
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
                    lvItem.SubItems[0].Text = newForm.id;
                    lvItem.SubItems[2].Text = newForm.name;
                    lvItem.SubItems[3].Text = newForm.maker;
                    lvItem.SubItems[4].Text = newForm.standard;
                    lvItem.SubItems[5].Text = newForm.unit;
                    lvItem.SubItems[6].Text = newForm.tax;
                    lvItem.SubItems[7].Text = textTrans(newForm.str_school_price);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Add(new ListViewItem(new string[] {"",  (listView1.Items.Count + 1).ToString(), "", "", "", "", "", "", "" }));
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
                item.SubItems[1].Text = i.ToString();
                i++;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            indirect_list newForm = new indirect_list();

            if (newForm.ShowDialog() == DialogResult.OK)
            {
                this.name = newForm.name;
                this.date = newForm.date;
                using (MySqlConnection conn = new MySqlConnection(strConn))
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
                    }

                    sql = "SELECT * FROM `indirectItem` WHERE indirect_id = " + id;

                    ds.Clear();
                    adpt = new MySqlDataAdapter(sql, conn);
                    adpt.Fill(ds);
                    // AND product_id = (SELECT estimateItem.product_id FROM 'estimateItem' WHERE estimateItem.estimate_id = 22)
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        if (row["product_id"].ToString() != "")
                        {
                            DataSet tempDs = new DataSet();
                            MySqlDataAdapter tempAdpt = new MySqlDataAdapter("SELECT name, maker, standard, unit, tax FROM `product` WHERE id = " + row["product_id"].ToString(), conn);
                            tempAdpt.Fill(tempDs);

                            listView1.Items.Add(new ListViewItem(new string[] {
                                row["product_id"].ToString(),
                                row["no"].ToString(), 
                                tempDs.Tables[0].Rows[0]["name"].ToString(),
                                tempDs.Tables[0].Rows[0]["maker"].ToString(),
                                tempDs.Tables[0].Rows[0]["standard"].ToString(),
                                tempDs.Tables[0].Rows[0]["unit"].ToString(),
                                tempDs.Tables[0].Rows[0]["tax"].Equals(true) ? "과세" : "면세",
                                textTrans(row["school_price"].ToString()),
                                row["school_name"].ToString() }));
                        }
                        else
                        {
                            listView1.Items.Add(new ListViewItem(new string[] {
                                "",
                                row["no"].ToString(), 
                                "",
                                "",
                                "",
                                "",
                                "",
                                textTrans(row["school_price"].ToString()),
                                row["school_name"].ToString() }));
                        }
                    }

                    conn.Close();
                }
                button1.Enabled = true;
                button5.Enabled = true;
                listView1.Focus();

            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();

            newEstimate newForm = new newEstimate("indirectList");

            if (newForm.ShowDialog() == DialogResult.OK)
            {
                this.name = newForm.name;
                this.date = newForm.date;

                listView1.Items.Add(new ListViewItem(new string[] { "", (listView1.Items.Count + 1).ToString(), "", "", "", "", "", "", "" }));

                button1.Enabled = true;
                button5.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(strConn))
            {
                try
                {
                    conn.Open();
                    splash = new ThreadedSplashFormController<nowLoading, nowLoading.ProgressChangedEventArgs>(x => x.ProgressChanged);
                    splash.Show();
                    nowLoading.ProgressChangedEventArgs p = new nowLoading.ProgressChangedEventArgs();

                    int maxNumber = listView1.Items.Count;
                    int highestPercentageReached = 0;

                    int percentComplete = 0;
                    int i = 0;

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
                            percentComplete = (int)((float)i / (float)maxNumber * 100);
                            if (percentComplete > highestPercentageReached)
                            {
                                p.Progress = percentComplete;
                                splash.OnProgressChanged(this, p);
                                highestPercentageReached = percentComplete;
                                //bw.ReportProgress(percentComplete);
                            }
                            i++;
                            MySqlCommand insertCommand = new MySqlCommand();
                            insertCommand.Connection = conn;
                            sql = "UPDATE indirectItem set ";

                            if (item.SubItems[0].Text.Equals(""))
                                sql += "product_id = null";
                            else
                                sql += "product_id = " + item.SubItems[0].Text;

                            if (item.SubItems[7].Text.Equals(""))
                                sql += ", school_price = null";
                            else
                                sql += ", school_price = " + Int32.Parse(item.SubItems[7].Text);

                            sql += ", school_name = '" + item.SubItems[8].Text +
                            "' where indirect_id=" + id + " AND no = " + item.SubItems[1].Text;

                            insertCommand.CommandText = sql;
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                    else if (ds.Tables[0].Rows.Count == 0)
                    {
                        MySqlCommand insertCommand = new MySqlCommand();
                        insertCommand.Connection = conn;
                        insertCommand.CommandText = "INSERT INTO indirectList(account, date) VALUES(@account, @date)";
                        insertCommand.Parameters.AddWithValue("@account", this.name);
                        insertCommand.Parameters.AddWithValue("@date", this.date);

                        insertCommand.ExecuteNonQuery();

                        sql = "SELECT * FROM `indirectList` WHERE account = '" + this.name + "' AND date = '" + this.date + "'";

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
                            percentComplete = (int)((float)i / (float)maxNumber * 100);
                            if (percentComplete > highestPercentageReached)
                            {
                                p.Progress = percentComplete;
                                splash.OnProgressChanged(this, p);
                                highestPercentageReached = percentComplete;
                                //bw.ReportProgress(percentComplete);
                            }
                            i++;
                            insertCommand = new MySqlCommand();
                            insertCommand.Connection = conn;
                            insertCommand.CommandText = "INSERT INTO indirectItem(indirect_id, no, product_id, school_price, school_name) VALUES(@indirect_id, @no, @product_id, @school_price, @school_name)";
                            insertCommand.Parameters.AddWithValue("@indirect_id", id);
                            if (item.SubItems[0].Text.Equals(""))
                                insertCommand.Parameters.AddWithValue("@product_id", null);
                            else
                                insertCommand.Parameters.AddWithValue("@product_id", Int32.Parse(item.SubItems[0].Text));
                            insertCommand.Parameters.AddWithValue("@no", item.SubItems[1].Text);
                            insertCommand.Parameters.AddWithValue("@school_name", item.SubItems[8].Text);
                            if (item.SubItems[7].Text.Equals(""))
                                insertCommand.Parameters.AddWithValue("@school_price", null);
                            else
                                insertCommand.Parameters.AddWithValue("@school_price", Int32.Parse(item.SubItems[7].Text.Replace(",", "")));

                            insertCommand.ExecuteNonQuery();
                        }
                    }

                    splash.Close();
                    //MessageBox.Show("저장 완료");
                }
                catch (Exception eee)
                {
                    splash.Close();
                    new alarm("저장에 실패하였습니다.", false).ShowDialog();
                }

                conn.Close();
            }   
        }

        private void HideComboEditor()
        {
            //ListViewHitTestInfo i = listView1.HitTest(e.X, e.Y);

            //if (i.SubItem == i.Item.SubItems[6])
            //{
            comboBox1.Visible = false;
            if (SelectedLSI != null)
            {
               if(product_id != -1)
               {

                   using (MySqlConnection conn = new MySqlConnection(strConn))
                   {
                       try
                       {
                           conn.Open();

                           MySqlCommand insertCommand = new MySqlCommand();
                           insertCommand.Connection = conn;
                           string sql = "UPDATE product set ";

                           if (comboBox1.SelectedIndex == 0)
                               sql += "tax = 1";
                           else
                               sql += "tax = 0";

                           sql += " where id=" + product_id;

                           insertCommand.CommandText = sql;
                           insertCommand.ExecuteNonQuery();

                           SelectedLSI.Text = comboBox1.SelectedIndex == 0 ? "과세" : "면세";
                       }
                       catch (Exception eee)
                       {
                           new alarm("연결에 실패하였습니다.", false).ShowDialog();
                       }

                       conn.Close();
                   }   
               }
            }

            SelectedLSI = null;
            listView1.Focus();
            //}
        }

        private void comboBox1_Leave(object sender, EventArgs e)
        {
           //HideComboEditor();
        }
        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //HideComboEditor();
        }

        private void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
            HideComboEditor();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (new alarm("간접납품 견적서가 삭제됩니다. 계속 하시겠습니까?", true).ShowDialog() == DialogResult.OK)
            {
                using (MySqlConnection conn = new MySqlConnection(strConn))
                {
                    conn.Open();

                    string sql = "DELETE FROM `indirectList` WHERE account = '" + this.name + "' AND date = '" + this.date + "'";

                    MySqlCommand insertCommand = new MySqlCommand();
                    insertCommand.Connection = conn;
                    insertCommand.CommandText = sql;

                    insertCommand.ExecuteNonQuery();

                    listView1.Items.Clear();
                    this.name = null;
                    this.date = null;

                    conn.Close();
                    //listView1.Items.Remove(selectedItem);
                }
                button1.Enabled = false;
                button5.Enabled = false;
            }
        }
    }
}
