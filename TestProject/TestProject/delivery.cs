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
    public partial class delivery : Form
    {
        Excel.Application excelApp = null;
        Excel.Workbook wb = null;
        Excel.Worksheet ws = null;

        String strConn = "Server=13.124.90.82; Port=3306; Database=rntp; Uid=root; Pwd=rntprntp;";
        DataSet ds = new DataSet();

        ThreadedSplashFormController<nowLoading, nowLoading.ProgressChangedEventArgs> splash = null;

        string name;
        string date;
        int select;

        public delivery()
        {
            InitializeComponent();
            this.TopLevel = false;
            //for (int c = 1; c <= 31; c++)
            //{
            //    listView1.Columns.Add(c.ToString(), 50, HorizontalAlignment.Center);
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

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            estimate_list newForm = new estimate_list(false);

            if (newForm.ShowDialog() == DialogResult.OK)
            {
                //listView1.BeginUpdate();
                this.name = newForm.name;
                this.date = newForm.date;
                this.select = newForm.select;

                splash = new ThreadedSplashFormController<nowLoading, nowLoading.ProgressChangedEventArgs>(x => x.ProgressChanged);
                splash.Show();
                nowLoading.ProgressChangedEventArgs p = new nowLoading.ProgressChangedEventArgs();

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

                    int maxNumber = ds.Tables[0].Rows.Count;
                    int highestPercentageReached = 0;

                    int percentComplete = 0;
                    int i = 0;

                    foreach (DataRow row in ds.Tables[0].Rows)
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
                                textTrans(row["total"].ToString()) }));
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
                                textTrans(row["total"].ToString()) }));
                        }
                    }

                    conn.Close();
                }

                splash.Close();

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm|All files|*.*";
                String FileName = null;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    splash = new ThreadedSplashFormController<nowLoading, nowLoading.ProgressChangedEventArgs>(x => x.ProgressChanged);
                    splash.Show();
                    nowLoading.ProgressChangedEventArgs pp = new nowLoading.ProgressChangedEventArgs();
                    FileName = ofd.FileName;

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
                        //for (int c = 5; c < data.GetLength(1); c++)
                        //{
                        //    listView1.Columns.Add(data[1, c].ToString(), 50, HorizontalAlignment.Center);
                        //}

                        Console.WriteLine(data.GetLength(1));
                        Console.WriteLine(listView1.Items.Count);


                        int maxNumber = data.GetLength(0)-2;
                        int highestPercentageReached = 0;

                        int percentComplete = 0;
                        int i = 0;

                        for (int r = 2; r <= data.GetLength(0); r++)
                        {
                            percentComplete = (int)((float)i / (float)maxNumber * 100);
                            if (percentComplete > highestPercentageReached)
                            {
                                pp.Progress = percentComplete;
                                splash.OnProgressChanged(this, pp);
                                highestPercentageReached = percentComplete;
                                //bw.ReportProgress(percentComplete);
                            }
                            i++;

                            listView1.Items[r - 2].SubItems.Add(textTrans(data[r, data.GetLength(1)].ToString()));
                            for (int c = 5; c < data.GetLength(1); c++)
                            {
                                listView1.Items[r - 2].SubItems.Add(data[r, c].ToString());
                            }
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
                    splash.Close();
                }
                //listView1.EndUpdate();
            }
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

        private void button3_Click(object sender, EventArgs e)
        {

            using (MySqlConnection conn = new MySqlConnection(strConn))
            {
                try
                {

                    splash = new ThreadedSplashFormController<nowLoading, nowLoading.ProgressChangedEventArgs>(x => x.ProgressChanged);
                    splash.Show();
                    nowLoading.ProgressChangedEventArgs p = new nowLoading.ProgressChangedEventArgs();


                    int maxNumber = listView1.Items.Count;
                    int highestPercentageReached = 0;

                    int percentComplete = 0;
                    int progressI = 0;

                    conn.Open();

                    string sql = "SELECT * FROM `deliveryList` WHERE account = '" + this.name + "' AND date = '" + this.date + "' AND classification = " + this.select;

                    ds.Clear();
                    MySqlDataAdapter adpt = new MySqlDataAdapter(sql, conn);
                    adpt.Fill(ds);

                    int id = 0;

                    if (ds.Tables[0].Rows.Count == 1)
                    {
                        id = (int)ds.Tables[0].Rows[0]["id"];

                        foreach (ListViewItem item in listView1.Items)
                        {
                            percentComplete = (int)((float)progressI / (float)maxNumber * 100);
                            if (percentComplete > highestPercentageReached)
                            {
                                p.Progress = percentComplete;
                                splash.OnProgressChanged(this, p);
                                highestPercentageReached = percentComplete;
                                //bw.ReportProgress(percentComplete);
                            }
                            progressI++;

                            MySqlCommand insertCommand = new MySqlCommand();
                            insertCommand.Connection = conn;
                            sql = "UPDATE deliveryItem set";
                                
                            if (item.SubItems[0].Text.Equals(""))
                                sql += " product_id = null";
                            else
                                sql += " product_id = " + Int32.Parse(item.SubItems[0].Text);


                            if (item.SubItems[6].Text.Equals(""))
                                sql += ", total_estimate = null";
                            else
                                sql += ", total_estimate = " + float.Parse(item.SubItems[6].Text);

                            if (item.SubItems[7].Text.Equals(""))
                                sql += ", total_excel = null";
                            else
                                sql += ", total_excel = " + float.Parse(item.SubItems[7].Text);

                            for (int i = 0; i < listView1.Columns.Count - 8; i++)
                            {
                                sql += ", day" + (i+1) + " = " + "'" + item.SubItems[i+8].Text + "'";
                            }

                            sql += " where delivery_id=" + id + " AND no = " + item.SubItems[1].Text;

                            insertCommand.CommandText = sql;
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                    else if (ds.Tables[0].Rows.Count == 0)
                    {
                        

                        MySqlCommand insertCommand = new MySqlCommand();
                        insertCommand.Connection = conn;
                        insertCommand.CommandText = "INSERT INTO deliveryList(account, date, classification) VALUES(@account, @date, @classification)";
                        insertCommand.Parameters.AddWithValue("@account", this.name);
                        insertCommand.Parameters.AddWithValue("@date", this.date);
                        insertCommand.Parameters.AddWithValue("@classification", this.select);

                        insertCommand.ExecuteNonQuery();


                        sql = "SELECT * FROM `deliveryList` WHERE account = '" + this.name + "' AND date = '" + this.date + "' AND classification = " + this.select;
                        ds.Clear();
                        adpt = new MySqlDataAdapter(sql, conn);
                        adpt.Fill(ds);

                        id = 0;

                        if (ds.Tables[0].Rows.Count == 1)
                        {
                            id = (int)ds.Tables[0].Rows[0]["id"];
                        }

                        MySqlCommand updateCommand = new MySqlCommand();
                        updateCommand.Connection = conn;
                        switch (this.select)
                        {
                            case 0:
                                sql = "UPDATE estimateList set morning = " + id + " WHERE account = '" + this.name + "' AND date = '" + this.date + "'";
                                break;
                            case 1:
                                sql = "UPDATE estimateList set launch = " + id + " WHERE account = '" + this.name + "' AND date = '" + this.date + "'";
                                break;
                            case 2:
                                sql = "UPDATE estimateList set dinner = " + id + " WHERE account = '" + this.name + "' AND date = '" + this.date + "'";
                                break;
                            default:
                                sql = "null";
                                break;
                        }

                        updateCommand.CommandText = sql;
                        updateCommand.ExecuteNonQuery();

                        foreach (ListViewItem item in listView1.Items)
                        {
                            percentComplete = (int)((float)progressI / (float)maxNumber * 100);
                            if (percentComplete > highestPercentageReached)
                            {
                                p.Progress = percentComplete;
                                splash.OnProgressChanged(this, p);
                                highestPercentageReached = percentComplete;
                                //bw.ReportProgress(percentComplete);
                            }
                            progressI++;

                            sql = "INSERT INTO deliveryItem(delivery_id, no, product_id, total_estimate, total_excel";

                            for (int i = 0; i < listView1.Columns.Count - 8; i++)
                            {
                                sql += ", day" + (i + 1);
                            }
                            sql += ") VALUES (";

                            insertCommand = new MySqlCommand();
                            insertCommand.Connection = conn;

                            sql += id + ", " + item.SubItems[1].Text + ", ";

                            if (item.SubItems[0].Text.Equals(""))
                                sql += "null, ";
                            else
                                sql += Int32.Parse(item.SubItems[0].Text) + ", ";

                            if (item.SubItems[6].Text.Equals(""))
                                sql += "null, ";
                            else
                                sql += float.Parse(item.SubItems[6].Text) + ", ";
                            
                            if (item.SubItems[7].Text.Equals(""))
                                sql += "null ";
                            else
                                sql += float.Parse(item.SubItems[7].Text);

                            for (int i = 0; i < item.SubItems.Count - 8; i++)
                            {
                                sql += ", '" + item.SubItems[i+8].Text + "'";
                            }
                            sql += ")";

                            insertCommand.CommandText = sql;
                            insertCommand.ExecuteNonQuery();
                        }
                    }

                    splash.Close();
                    //MessageBox.Show("성공적으로 저장되었습니다.");
                }
                catch (Exception eee)
                {
                    splash.Close();
                    new alarm("저장에 실패하였습니다.", false).ShowDialog();
                }

                conn.Close();
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            string sql = null;      
            using (MySqlConnection conn = new MySqlConnection(strConn))
            {
                conn.Open();
                if (new alarm("납품지시서가 삭제됩니다. 계속 하시겠습니까?", true).ShowDialog() == DialogResult.OK)
                {
                    MySqlCommand updateCommand = new MySqlCommand();
                    updateCommand.Connection = conn;
                    switch (this.select)
                    {
                        case 0:
                            sql = "UPDATE estimateList set morning = null WHERE account = '" + this.name + "' AND date = '" + this.date + "'";
                            break;
                        case 1:
                            sql = "UPDATE estimateList set launch = null WHERE account = '" + this.name + "' AND date = '" + this.date + "'";
                            break;
                        case 2:
                            sql = "UPDATE estimateList set dinner = null WHERE account = '" + this.name + "' AND date = '" + this.date + "'";
                            break;
                        default:
                            sql = "null";
                            break;
                    }

                    updateCommand.CommandText = sql;
                    updateCommand.ExecuteNonQuery();

                    MySqlCommand insertCommand = new MySqlCommand();
                    sql = "DELETE FROM `deliveryList` WHERE account = '" + this.name + "' AND date = '" + this.date + "' AND classification = " + this.select;
                    insertCommand.Connection = conn;
                    insertCommand.CommandText = sql;

                    insertCommand.ExecuteNonQuery();

                    conn.Close();
                    listView1.Items.Clear();
                    this.name = null;
                    this.date = null;

                //listView1.Items.Remove(selectedItem);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            deliveryList newForm = new deliveryList();

            if (newForm.ShowDialog() == DialogResult.OK)
            {
                this.name = newForm.name;
                this.date = newForm.date;
                this.select = newForm.launch;

                using (MySqlConnection conn = new MySqlConnection(strConn))
                {

                    splash = new ThreadedSplashFormController<nowLoading, nowLoading.ProgressChangedEventArgs>(x => x.ProgressChanged);
                    splash.Show();
                    nowLoading.ProgressChangedEventArgs p = new nowLoading.ProgressChangedEventArgs();

                    conn.Open();

                    string sql = "SELECT * FROM `deliveryList` WHERE account = '" + this.name + "' AND date = '" + this.date + "' AND classification = " + this.select;

                    ds.Clear();
                    MySqlDataAdapter adpt = new MySqlDataAdapter(sql, conn);
                    adpt.Fill(ds);

                    int id = 0;

                    if (ds.Tables[0].Rows.Count == 1)
                    {
                        id = (int)ds.Tables[0].Rows[0]["id"];
                    }

                    sql = "SELECT * FROM `deliveryItem` WHERE delivery_id = " + id;

                    ds.Clear();
                    adpt = new MySqlDataAdapter(sql, conn);
                    adpt.Fill(ds);

                    int maxNumber = ds.Tables[0].Rows.Count;
                    int highestPercentageReached = 0;

                    int percentComplete = 0;
                    int progressI = 0;

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        percentComplete = (int)((float)progressI / (float)maxNumber * 100);
                        if (percentComplete > highestPercentageReached)
                        {
                            p.Progress = percentComplete;
                            splash.OnProgressChanged(this, p);
                            highestPercentageReached = percentComplete;
                            //bw.ReportProgress(percentComplete);
                        }
                        progressI++;

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
                                row["total_estimate"].ToString(),
                                row["total_excel"].ToString(),
                                row["day1"].ToString(),
                                row["day2"].ToString(),
                                row["day3"].ToString(),
                                row["day4"].ToString(),
                                row["day5"].ToString(),
                                row["day6"].ToString(),
                                row["day7"].ToString(),
                                row["day8"].ToString(),
                                row["day9"].ToString(),
                                row["day10"].ToString(),
                                row["day11"].ToString(),
                                row["day12"].ToString(),
                                row["day13"].ToString(),
                                row["day14"].ToString(),
                                row["day15"].ToString(),
                                row["day16"].ToString(),
                                row["day17"].ToString(),
                                row["day18"].ToString(),
                                row["day19"].ToString(),
                                row["day20"].ToString(),
                                row["day21"].ToString(),
                                row["day22"].ToString(),
                                row["day23"].ToString(),
                                row["day24"].ToString(),
                                row["day25"].ToString(),
                                row["day26"].ToString(),
                                row["day27"].ToString(),
                                row["day28"].ToString(),
                                row["day29"].ToString(),
                                row["day30"].ToString(),
                                row["day31"].ToString() }));
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
                                row["total_estimate"].ToString(),
                                row["total_excel"].ToString(),
                                row["day1"].ToString(),
                                row["day2"].ToString(),
                                row["day3"].ToString(),
                                row["day4"].ToString(),
                                row["day5"].ToString(),
                                row["day6"].ToString(),
                                row["day7"].ToString(),
                                row["day8"].ToString(),
                                row["day9"].ToString(),
                                row["day10"].ToString(),
                                row["day11"].ToString(),
                                row["day12"].ToString(),
                                row["day13"].ToString(),
                                row["day14"].ToString(),
                                row["day15"].ToString(),
                                row["day16"].ToString(),
                                row["day17"].ToString(),
                                row["day18"].ToString(),
                                row["day19"].ToString(),
                                row["day20"].ToString(),
                                row["day21"].ToString(),
                                row["day22"].ToString(),
                                row["day23"].ToString(),
                                row["day24"].ToString(),
                                row["day25"].ToString(),
                                row["day26"].ToString(),
                                row["day27"].ToString(),
                                row["day28"].ToString(),
                                row["day29"].ToString(),
                                row["day30"].ToString(),
                                row["day31"].ToString() }));
                        }
                    }

                    conn.Close();
                    splash.Close();
                }
            }
        }


    }
}
