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
    public partial class estimate : Form
    {

        Excel.Application excelApp = null;
        Excel.Workbook wb = null;
        Excel.Worksheet ws = null;

        ThreadedSplashFormController<nowLoading, nowLoading.ProgressChangedEventArgs> splash = null;
        ListViewItem.ListViewSubItem SelectedLSI;

        string name;
        string date;

        String strConn = "Server=13.124.90.82; Port=3306; Database=rntp; Uid=root; Pwd=rntprntp;";
        DataSet ds = new DataSet();

        public estimate()
        {
            InitializeComponent();
            this.TopLevel = false; 
        }

        private static void ReleaseExcelObject(object obj)
        {
            try
            {
                if (obj != null)
                {
                    Marshal.ReleaseComObject(obj);
                    obj = null;
                }
            }
            catch (Exception ex)
            {
                obj = null;
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            textBox3.Text = "";
            textBox4.Text = "";

            newEstimate newForm = new newEstimate("estimateList");

            if(newForm.ShowDialog() == DialogResult.OK)
            {
                this.name = newForm.name;
                this.date = newForm.date;

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm|All files|*.*";
                String FileName = null;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    listView1.Items.Clear();
                    FileName = ofd.FileName;

                    splash = new ThreadedSplashFormController<nowLoading, nowLoading.ProgressChangedEventArgs>(x => x.ProgressChanged);
                    splash.Show();
                    nowLoading.ProgressChangedEventArgs p = new nowLoading.ProgressChangedEventArgs();

                    try
                    {
                        excelApp = new Excel.Application();

                        // 엑셀 파일 열기
                        wb = excelApp.Workbooks.Open(FileName);

                        // 첫번째 Worksheet
                        ws = wb.Worksheets.get_Item(1) as Excel.Worksheet;

                        // 현재 Worksheet에서 사용된 Range 전체를 선택
                        Excel.Range rng = ws.UsedRange;

                        // 현재 Worksheet에서 일부 범위만 선택
                        // Excel.Range rng = ws.Range[ws.Cells[2, 1], ws.Cells[5, 3]];

                        // Range 데이타를 배열 (One-based array)로
                        object[,] data = rng.Value;

                        for (int r = 2; r <= data.GetLength(0); r++)
                        {
                            listView1.Items.Add(new ListViewItem(new string[] {
                                "",
                                data[r, 1] == null ? "" : data[r, 1].ToString(), 
                                "",
                                "",
                                "",
                                "",
                                data[r, 5] == null ? "" : data[r, 5].ToString(), 
                                "",
                                "",
                                "",
                                "",
                                data[r, 2] == null ? "" : data[r, 2].ToString(), 
                                data[r, 3] == null ? "" : data[r, 3].ToString(), 
                                data[r, 4] == null ? "" : data[r, 4].ToString(), 
                                data[r, 5] == null ? "" : data[r, 5].ToString(), 
                                data[r, 6] == null ? "" : data[r, 6].ToString() }));

                            //for (int c = 1; c <= data.GetLength(1); c++)
                            //{
                            //    if(data[r, c] == null)
                            //    {
                            //        Console.Write(" ");
                            //    }
                            //    else
                            //    {
                            //        Console.Write(data[r, c].ToString() + " ");
                            //    }
                            //}
                            //Console.WriteLine("");
                        }

                        wb.Close(true);
                        excelApp.Quit();
                    }
                    finally
                    {
                        // Clean up
                        ReleaseExcelObject(ws);
                        ReleaseExcelObject(wb);
                        ReleaseExcelObject(excelApp);
                    }

                    int Year = (Int32.Parse(this.date) / 100);
                    int Month = (Int32.Parse(this.date) % 100) - 1;
                    string strDate = null;

                    if (Month == 0)
                    {
                        Year = Year - 1;
                        Month = 12;
                    }
                    strDate = Year.ToString("D2") + Month.ToString("D2");

                    int maxNumber = listView1.Items.Count;
                    int highestPercentageReached = 0;

                    int percentComplete = 0;
                    int i = 0;

                    using (MySqlConnection conn = new MySqlConnection(strConn))
                    {
                        try
                        {
                            conn.Open();

                            string sql = "SELECT * FROM `estimateList` WHERE account = '" + this.name + "' AND date = '" + strDate + "'";

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

                                    sql = "SELECT * FROM `estimateItem` WHERE estimate_id = " + id + " AND name_excel = '" + item.SubItems[10].Text + "'";
                                    ds.Clear();
                                    adpt = new MySqlDataAdapter(sql, conn);
                                    adpt.Fill(ds);

                                    if (ds.Tables[0].Rows.Count == 1)
                                    {
                                        item.SubItems[2].Text = ds.Tables[0].Rows[0]["product_name"].ToString();
                                        item.SubItems[3].Text = ds.Tables[0].Rows[0]["maker"].ToString();
                                        item.SubItems[4].Text = ds.Tables[0].Rows[0]["standard"].ToString();
                                        item.SubItems[5].Text = ds.Tables[0].Rows[0]["unit"].ToString();
                                        item.SubItems[7].Text = ds.Tables[0].Rows[0]["original_price"].ToString();
                                        item.SubItems[8].Text = ds.Tables[0].Rows[0]["estimate_price"].ToString();
                                        item.SubItems[9].Text = ds.Tables[0].Rows[0]["school_price"].ToString();
                                        item.SubItems[10].Text = ds.Tables[0].Rows[0]["total_price"].ToString();
                                    }
                                }

                            }
                        }
                        catch (Exception eee)
                        {
                            MessageBox.Show("저장에 실패하였습니다.");
                            splash.Close();
                        }

                        conn.Close();
                    }
                }
                splash.Close();
            }
            calculateTotal();
            listView1.Focus();
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
                ListViewItem lvItem = items[0];
                string name = lvItem.SubItems[2].Text != "" ? lvItem.SubItems[2].Text : lvItem.SubItems[11].Text;

                selectProduct newForm = new selectProduct();

                newForm.setKeyword(name);

                if(newForm.ShowDialog() == DialogResult.OK)
                {
                    lvItem.SubItems[0].Text = newForm.id;
                    lvItem.SubItems[2].Text = newForm.name;
                    lvItem.SubItems[3].Text = newForm.maker;
                    lvItem.SubItems[4].Text = newForm.standard;
                    lvItem.SubItems[5].Text = newForm.unit;
                    lvItem.SubItems[7].Text = textTrans(newForm.str_original_price);
                    lvItem.SubItems[8].Text = textTrans(newForm.str_estimate_price);
                    lvItem.SubItems[9].Text = textTrans(newForm.str_school_price);

                    lvItem.SubItems[10].Text = textTrans((double.Parse(lvItem.SubItems[6].Text.Replace(",", "")) * Int32.Parse(lvItem.SubItems[9].Text.Replace(",", ""))).ToString());
                    calculateTotal();
                }
            }
        }

        // 저장
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

                    string sql = "SELECT * FROM `estimateList` WHERE account = '" + this.name + "' AND date = '" + this.date + "'";

                    ds.Clear();
                    MySqlDataAdapter adpt = new MySqlDataAdapter(sql, conn);
                    adpt.Fill(ds);

                    int id = 0;

                    if (ds.Tables[0].Rows.Count == 1)
                    {
                        id = (int)ds.Tables[0].Rows[0]["id"];

                        MySqlCommand insertCommand2 = new MySqlCommand();
                        insertCommand2.Connection = conn;
                        sql = "UPDATE estimateList set account = '" + this.name +
                            "', date = '" + this.date +
                            "', bid = " + (textBox3.Text == "" ? "null" : textBox3.Text.Replace(",", "")) +
                            ", base = " + (textBox4.Text == "" ? "null" : textBox4.Text.Replace(",", "")) +
                            " where id=" + id;

                        insertCommand2.CommandText = sql;
                        insertCommand2.ExecuteNonQuery();

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
                            sql = "UPDATE estimateItem set";// product_id = '" + item.SubItems[0].Text;  // +

                            if (item.SubItems[0].Text.Equals(""))
                                sql += " product_id = null";
                            else
                                sql += " product_id = " + Int32.Parse(item.SubItems[0].Text);

                            if (item.SubItems[6].Text.Equals(""))
                                sql += ", total = null";
                            else
                                sql += ", total = " + float.Parse(item.SubItems[6].Text);

                            if (item.SubItems[7].Text.Equals(""))
                                sql += ", original_price = null";
                            else
                                sql += ", original_price = " + float.Parse(item.SubItems[7].Text);

                            if (item.SubItems[8].Text.Equals(""))
                                sql += ", estimate_price = null";
                            else
                                sql += ", estimate_price = " + float.Parse(item.SubItems[8].Text);

                            if (item.SubItems[9].Text.Equals(""))
                                sql += ", school_price = null";
                            else
                                sql += ", school_price = " + float.Parse(item.SubItems[9].Text);

                            if (item.SubItems[10].Text.Equals(""))
                                sql += ", total_price = null";
                            else
                                sql += ", total_price = " + float.Parse(item.SubItems[10].Text);

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

                            sql += ", name_excel = '" + item.SubItems[11].Text +
                            "', standard_excel = '" + item.SubItems[12].Text +
                            "', unit_excel = '" + item.SubItems[13].Text;

                            if (item.SubItems[14].Text.Equals(""))
                                sql += "', total_excel = null";
                            else
                                sql += "', total_excel = " + float.Parse(item.SubItems[14].Text);

                            sql += ", text_excel = '" + item.SubItems[15].Text +
                            "' where estimate_id=" + id + " AND no = " + item.SubItems[1].Text;

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
                        insertCommand.Parameters.AddWithValue("@bid", textBox3.Text == "" ? null: textBox3.Text.Replace(",", ""));
                        insertCommand.Parameters.AddWithValue("@base", textBox4.Text == "" ? null : textBox4.Text.Replace(",", ""));

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
                            insertCommand.CommandText = "INSERT INTO estimateItem(estimate_id, no, product_id, total, original_price, estimate_price, school_price, total_price, name_excel, standard_excel, unit_excel, total_excel, text_excel) VALUES(@estimate_id, @no, @product_id, @total, @original_price, @estimate_price, @school_price, @total_price, @name_excel, @standard_excel, @unit_excel, @total_excel, @text_excel)";
                            insertCommand.Parameters.AddWithValue("@estimate_id", id);
                            insertCommand.Parameters.AddWithValue("@no", Int32.Parse(item.SubItems[1].Text));

                            if (item.SubItems[0].Text.Equals(""))
                                insertCommand.Parameters.AddWithValue("@product_id", null);
                            else
                                insertCommand.Parameters.AddWithValue("@product_id", Int32.Parse(item.SubItems[0].Text));
                            //insertCommand.Parameters.AddWithValue("@maker", item.SubItems[3].Text);
                            //insertCommand.Parameters.AddWithValue("@standard", item.SubItems[4].Text);
                            //insertCommand.Parameters.AddWithValue("@unit", item.SubItems[5].Text);
                            if (item.SubItems[6].Text.Equals(""))
                                insertCommand.Parameters.AddWithValue("@total", null);
                            else
                                insertCommand.Parameters.AddWithValue("@total", float.Parse(item.SubItems[6].Text));

                            if (item.SubItems[7].Text.Equals(""))
                                insertCommand.Parameters.AddWithValue("@original_price", null);
                            else
                                insertCommand.Parameters.AddWithValue("@original_price", float.Parse(item.SubItems[7].Text));

                            if (item.SubItems[8].Text.Equals(""))
                                insertCommand.Parameters.AddWithValue("@estimate_price", null);
                            else
                                insertCommand.Parameters.AddWithValue("@estimate_price", float.Parse(item.SubItems[8].Text));

                            if (item.SubItems[9].Text.Equals(""))
                                insertCommand.Parameters.AddWithValue("@school_price", null);
                            else
                                insertCommand.Parameters.AddWithValue("@school_price", float.Parse(item.SubItems[9].Text));
                            if (item.SubItems[10].Text.Equals(""))
                                insertCommand.Parameters.AddWithValue("@total_price", null);
                            else
                                insertCommand.Parameters.AddWithValue("@total_price", float.Parse(item.SubItems[10].Text));
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
                            insertCommand.Parameters.AddWithValue("@name_excel", item.SubItems[11].Text);
                            insertCommand.Parameters.AddWithValue("@standard_excel", item.SubItems[12].Text);
                            insertCommand.Parameters.AddWithValue("@unit_excel", item.SubItems[13].Text);
                            if (item.SubItems[14].Text.Equals(""))
                                insertCommand.Parameters.AddWithValue("@total_excel", null);
                            else
                                insertCommand.Parameters.AddWithValue("@total_excel", float.Parse(item.SubItems[14].Text));
                            insertCommand.Parameters.AddWithValue("@text_excel", item.SubItems[15].Text);

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

        // 불러오기
        private void button2_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            estimate_list newForm = new estimate_list(true);

            if(newForm.ShowDialog() == DialogResult.OK)
            {
                this.name = newForm.name;
                this.date = newForm.date;
                textBox3.Text = textTrans(newForm.bid_price);
                textBox4.Text = textTrans(newForm.base_price);
                using (MySqlConnection conn = new MySqlConnection(strConn))
                {
                    conn.Open();

                    string sql = "SELECT * FROM `estimateList` WHERE account = '" + this.name + "' AND date = '" + this.date + "'";

                    ds.Clear();
                    MySqlDataAdapter adpt = new MySqlDataAdapter(sql, conn);
                    adpt.Fill(ds);

                    int id = 0;

                    if (ds.Tables[0].Rows.Count == 1)
                    {
                        id = (int)ds.Tables[0].Rows[0]["id"];
                    }

                    sql = "SELECT * FROM `estimateItem` WHERE estimate_id = " + id;

                    ds.Clear();
                    adpt = new MySqlDataAdapter(sql, conn);
                    adpt.Fill(ds);
                    // AND product_id = (SELECT estimateItem.product_id FROM 'estimateItem' WHERE estimateItem.estimate_id = 22)
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        if (row["product_id"].ToString() != "")
                        {
                            DataSet tempDs = new DataSet();
                            MySqlDataAdapter tempAdpt = new MySqlDataAdapter("SELECT name, maker, standard, unit FROM `product` WHERE id = " + row["product_id"].ToString(), conn);
                            tempAdpt.Fill(tempDs); 
                            
                            listView1.Items.Add(new ListViewItem(new string[] {
                                row["product_id"].ToString(),
                                row["no"].ToString(), 
                                tempDs.Tables[0].Rows[0]["name"].ToString(),
                                tempDs.Tables[0].Rows[0]["maker"].ToString(),
                                tempDs.Tables[0].Rows[0]["standard"].ToString(),
                                tempDs.Tables[0].Rows[0]["unit"].ToString(),
                                textTrans(row["total"].ToString()),
                                textTrans(row["original_price"].ToString()),
                                textTrans(row["estimate_price"].ToString()),
                                textTrans(row["school_price"].ToString()),
                                textTrans(row["total_price"].ToString()),
                                row["name_excel"].ToString(),
                                row["standard_excel"].ToString(),
                                row["unit_excel"].ToString(),
                                row["total_excel"].ToString(),
                                row["text_excel"].ToString() }));
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
                                textTrans(row["total"].ToString()),
                                textTrans(row["original_price"].ToString()),
                                textTrans(row["estimate_price"].ToString()),
                                textTrans(row["school_price"].ToString()),
                                textTrans(row["total_price"].ToString()),
                                row["name_excel"].ToString(),
                                row["standard_excel"].ToString(),
                                row["unit_excel"].ToString(),
                                row["total_excel"].ToString(),
                                row["text_excel"].ToString() }));
                        }
                    }

                    conn.Close();
                }
                calculateTotal();
                listView1.Focus();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (new alarm("견적서가 삭제됩니다. 계속 하시겠습니까?", true).ShowDialog() == DialogResult.OK)
            {
                using (MySqlConnection conn = new MySqlConnection(strConn))
                {
                    conn.Open();
                    DataSet tempDs = new DataSet();
                    MySqlDataAdapter tempAdpt = new MySqlDataAdapter("SELECT morning, launch, dinner FROM `estimateList` WHERE account = '" + this.name + "' AND date = '" + this.date + "'", conn);
                    tempAdpt.Fill(tempDs);
                    //ds.Tables[0].Rows[0]["morning"].ToString() != "" , ds.Tables[0].Rows[0]["launch"].ToString() != "", ds.Tables[0].Rows[0]["dinner"].ToString() != ""
                    if ((tempDs.Tables[0].Rows[0]["morning"].ToString() != "" || tempDs.Tables[0].Rows[0]["launch"].ToString() != "") || tempDs.Tables[0].Rows[0]["dinner"].ToString() != "")
                    {
                        new alarm("견적서와 연결된 납품지시서가 존재합니다.", false).ShowDialog();
                    }
                    else
                    {
                        string sql = "DELETE FROM `estimateList` WHERE account = '" + this.name + "' AND date = '" + this.date + "'";

                        MySqlCommand insertCommand = new MySqlCommand();
                        insertCommand.Connection = conn;
                        insertCommand.CommandText = sql;

                        insertCommand.ExecuteNonQuery();

                        listView1.Items.Clear();
                        this.name = null;
                        this.date = null;
                    }
                    //listView1.Items.Remove(selectedItem);
                    conn.Close();
                }
            }
        }

        //private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    //if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
        //    //{
        //    //    e.Handled = true;
        //    //}
        //    try
        //    {
        //        if ((char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
        //        {
        //            if (textBox3.Text.Length > 2)
        //            {
        //                string s;
        //                if (e.KeyChar == Convert.ToChar(Keys.Back))
        //                {
        //                    s = ExtractComma(textBox3.Text.Substring(0,textBox3.Text.Length - 1));
        //                    textBox3.Text = PointMoney(s);
        //                    textBox3.Select(textBox3.Text.Length, 1);
        //                    e.Handled = true;
        //                }
        //                else
        //                {
        //                    s = ExtractComma(textBox3.Text);
        //                    textBox3.Text = PointMoney(s + e.KeyChar); 
        //                    textBox3.Select(textBox3.Text.Length - 1, 1);
        //                }
                        
        //            }
        //        }
        //        else { e.Handled = true; }
        //    }
        //    catch (Exception)
        //    {
        //    } 
        //}
        //public static string ExtractComma(string str) 
        //{        
        //    if ( (str.IndexOf(",") <= 0)) 
        //        return str; 
        //    str = str.Substring(0, str.IndexOf(",")) + str.Substring(str.IndexOf(",")+1); 

        //    if ( (str.IndexOf(",") <= 0)) 
        //        return str; 
        //    else 
        //        return ExtractComma(str); 
        //} 
         
        //public static string PointMoney(string s) 
        //{                        
        //    char[] c = new char[15]; 
        //    int j=15; 
        //    int readcnt=0; 
        //    string s1=""; 

        //    if (s.Length > 3) 
        //    {                                
        //        for(int i=s.Length-1; i>=0; i--) 
        //        { 
        //            j--; 
        //            c[j] = s[i]; 
        //            readcnt++; 
        //            if (readcnt == 3 && i != 0) 
        //            { 
        //                readcnt=0; 
        //                j--; 
        //                c[j] = ','; 
        //            }                                
        //        } 
        //        s1 = new String(c, j, 15-j); 
        //    } 
        //    else 
        //        s1 = s; 
        //    return s1; 
        //} 

        private void textBox3_Leave(object sender, EventArgs e)
        {
            if (textBox3.Text != "")
            {
                textBox3.Text = textTrans(textBox3.Text);
            }
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            if (textBox4.Text != "")
            {
                textBox4.Text = textTrans(textBox4.Text);
            }
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

        private void calculateTotal()
        {
            int total = 0;
            int total2 = 0;

            foreach (ListViewItem item in listView1.Items)
            {
                if (item.SubItems[8].Text != "")
                    total += (int)(Convert.ToInt32(item.SubItems[8].Text.Replace(",", "")) * float.Parse(item.SubItems[6].Text));

                if (item.SubItems[9].Text != "")
                    total2 += (int)(Convert.ToInt32(item.SubItems[9].Text.Replace(",", "")) * float.Parse(item.SubItems[6].Text));
            }
            textBox1.Text = String.Format("{0:#,###}", total);
            textBox2.Text = String.Format("{0:#,###}", total2);
        }

        private void calculateRate()
        {
            if (textBox4.Text != "" && textBox3.Text != "")
            {
                string lgsText;
                lgsText = textBox3.Text.Replace(",", "");
                int basePrice = Convert.ToInt32(lgsText);

                lgsText = textBox4.Text.Replace(",", "");
                int bidPrice = Convert.ToInt32(lgsText);

                textBox5.Text = (((float)basePrice / bidPrice) * 100).ToString("##.#") + "%";
            }
            else
            {
                textBox5.Text = "";
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            calculateRate();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            calculateRate();
        }

        private void HideTextEditor()
        {
            //ListViewHitTestInfo i = listView1.HitTest(e.X, e.Y);

            //if (i.SubItem == i.Item.SubItems[6])
            //{
            TxtEdit.Visible = false;
            if (SelectedLSI != null)
                SelectedLSI.Text = textTrans(TxtEdit.Text);
            SelectedLSI = null;
            TxtEdit.Text = "";
            calculateTotal();
            listView1.Focus();
            //}
        }

        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo i = listView1.HitTest(e.X, e.Y);
            if (i.SubItem == null) return;
            if (i.SubItem == i.Item.SubItems[9] && i.Item.SubItems[2].Text != "")
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
                    CellWidth = listView1.Columns[0].Width;

                    TxtEdit.Location = new Point(CellLeft, CellTop);
                    TxtEdit.Size = new Size(CellWidth, CellHeight);
                    TxtEdit.Visible = true;
                    TxtEdit.BringToFront();
                    TxtEdit.Text = i.SubItem.Text;
                    TxtEdit.Select();
                    TxtEdit.SelectAll();
            }
        }

        private void TxtEdit_Leave(object sender, EventArgs e)
        {
            HideTextEditor();
        }

        private void TxtEdit_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
                HideTextEditor();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            checkDelivery newForm = new checkDelivery(name, date);
            newForm.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //for (int i = 0; i < listView1.Columns.Count; i++)
            //{
            //    Console.WriteLine(listView1.Columns[i].Text);
            //}

            selectHeader newForm = new selectHeader();

            if (newForm.ShowDialog() == DialogResult.OK)
            {
                SaveFileDialog sfd = new SaveFileDialog();

                sfd.InitialDirectory = @"C:\";
                sfd.Title = "Save file";

                sfd.CreatePrompt = true;
                sfd.OverwritePrompt = true;

                sfd.DefaultExt = "*.xls";
                sfd.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm|All files|*.*";
                sfd.FileName = this.name + "_" + this.date;


                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    //try
                    //{
                    //    object missingType = Type.Missing;
                    //    Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                    //    Microsoft.Office.Interop.Excel.Workbook excelBook = excelApp.Workbooks.Add(missingType);
                    //    Microsoft.Office.Interop.Excel.Worksheet excelWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)excelBook.Worksheets.Add(missingType, missingType, missingType, missingType);
                    //    excelApp.Visible = false;

                    //    for (int i = 0; i < lstView.Items.Count; i++)
                    //    {
                    //        for (int j = 0; j < lstView.Columns.Count; j++)
                    //        {
                    //            if (i == 0)
                    //            {
                    //                excelWorksheet.Cells[1, j + 1] = this.lstView.Columns[j].Text;
                    //            }
                    //            excelWorksheet.Cells[i + 2, j + 1] = this.lstView.Items[i].SubItems[j].Text;
                    //        }
                    //    }
                    //    excelBook.SaveAs(@saveFileDialog1.FileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, missingType, missingType, missingType, missingType, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, missingType, missingType, missingType, missingType, missingType);
                    //    excelApp.Visible = true;
                    //    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    //}
                    //catch
                    //{
                    //    MessageBox.Show("Excel 파일 저장중 에러가 발생했습니다.");
                    //}
                }
            }
        }
    }
}
