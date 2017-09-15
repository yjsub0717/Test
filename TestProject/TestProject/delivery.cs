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

        string name;
        string date;

        public delivery()
        {
            InitializeComponent();
            this.TopLevel = false;
            for (int c = 1; c <= 31; c++)
            {
                listView1.Columns.Add(c.ToString(), 50, HorizontalAlignment.Center);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            estimate_list newForm = new estimate_list();

            if (newForm.ShowDialog() == DialogResult.OK)
            {
                //listView1.BeginUpdate();
                this.name = newForm.name;
                this.date = newForm.date;
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

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        listView1.Items.Add(new ListViewItem(new string[] {
                        row["no"].ToString(), 
                        row["product_name"].ToString(),
                        row["maker"].ToString(),
                        row["standard"].ToString(),
                        row["unit"].ToString(),
                        row["total"].ToString() }));
                    }

                    conn.Close();
                }

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm|All files|*.*";
                String FileName = null;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
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
                        for (int r = 2; r <= data.GetLength(0); r++)
                        {
                            listView1.Items[r-2].SubItems.Add(data[r, data.GetLength(1)].ToString());
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
                //try
                //{
                    conn.Open();

                    string sql = "SELECT * FROM `deliveryList` WHERE account = '" + this.name + "' AND date = '" + this.date + "'";

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
                            sql = "UPDATE deliveryItem set product_name = '" + item.SubItems[1].Text +
                            "', maker = ' " + item.SubItems[2].Text +
                            "', standard = '" + item.SubItems[3].Text +
                            "', unit = '" + item.SubItems[4].Text;

                            if (item.SubItems[5].Text.Equals(""))
                                sql += "', total_estimate = null";
                            else
                                sql += "', total_estimate = " + float.Parse(item.SubItems[5].Text);

                            if (item.SubItems[6].Text.Equals(""))
                                sql += ", total_excel = null";
                            else
                                sql += ", total_excel = " + float.Parse(item.SubItems[6].Text);

                            for (int i = 0; i < listView1.Columns.Count - 7; i++)
                            {
                                sql += ", day" + (i+1) + " = " + "'" + item.SubItems[i+7].Text + "'";
                            }

                            sql += " where delivery_id=" + id + " AND no = " + item.SubItems[0].Text;

                            insertCommand.CommandText = sql;
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                    else if (ds.Tables[0].Rows.Count == 0)
                    {
                        MySqlCommand insertCommand = new MySqlCommand();
                        insertCommand.Connection = conn;
                        insertCommand.CommandText = "INSERT INTO deliveryList(account, date) VALUES(@account, @date)";
                        insertCommand.Parameters.AddWithValue("@account", this.name);
                        insertCommand.Parameters.AddWithValue("@date", this.date);

                        insertCommand.ExecuteNonQuery();


                        sql = "SELECT * FROM `deliveryList` WHERE account = '" + this.name + "' AND date = '" + this.date + "'";
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
                            sql = "INSERT INTO deliveryItem(delivery_id, no, product_name, maker, standard, unit, total_estimate, total_excel";

                            for (int i = 0; i < listView1.Columns.Count - 7; i++)
                            {
                                sql += ", day" + (i + 1);
                            }
                            sql += ") VALUES (";

                            insertCommand = new MySqlCommand();
                            insertCommand.Connection = conn;

                            sql += id + ", '" + item.SubItems[0].Text + "', '" + item.SubItems[1].Text + "', '" + item.SubItems[2].Text + "', '" + item.SubItems[3].Text + "', '" + item.SubItems[4].Text + "', ";
                            
                            if (item.SubItems[5].Text.Equals(""))
                                sql += "null, ";
                            else
                                sql += float.Parse(item.SubItems[5].Text) + ", ";
                            
                            if (item.SubItems[6].Text.Equals(""))
                                sql += "null ";
                            else
                                sql += float.Parse(item.SubItems[6].Text);

                            for (int i = 0; i < item.SubItems.Count - 7; i++)
                            {
                                sql += ", '" + item.SubItems[i+7].Text + "'";
                            }
                            sql += ")";

                            insertCommand.CommandText = sql;
                            insertCommand.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("성공적으로 저장되었습니다.");
                //}
                //catch (Exception eee)
                //{
                //    MessageBox.Show("저장에 실패하였습니다.");
                //}

                conn.Close();
            }   
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("납품지시서가 삭제됩니다.\r계속 하시겠습니까?", "남품지시서 삭제", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string sql = "DELETE FROM `deliveryList` WHERE account = '" + this.name + "' AND date = '" + this.date + "'";

                using (MySqlConnection conn = new MySqlConnection(strConn))
                {
                    conn.Open();

                    MySqlCommand insertCommand = new MySqlCommand();
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
                using (MySqlConnection conn = new MySqlConnection(strConn))
                {
                    conn.Open();

                    string sql = "SELECT * FROM `deliveryList` WHERE account = '" + this.name + "' AND date = '" + this.date + "'";

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

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        listView1.Items.Add(new ListViewItem(new string[] {
                        row["no"].ToString(), 
                        row["product_name"].ToString(),
                        row["maker"].ToString(),
                        row["standard"].ToString(),
                        row["unit"].ToString(),
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

                    conn.Close();
                }
            }
        }


    }
}
