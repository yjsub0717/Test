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
    public partial class newProducts : Form
    {
        Excel.Application excelApp = null;
        Excel.Workbook wb = null;
        Excel.Worksheet ws = null;
        ThreadedSplashFormController<nowLoading, nowLoading.ProgressChangedEventArgs> splash = null;


        String strConn = "Server=13.124.90.82; Port=3306; Database=rntp; Uid=root; Pwd=rntprntp;";
        DataSet ds = new DataSet();

        public newProducts()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm|All files|*.*";
            String FileName = null;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                listView1.Items.Clear();
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

                    int maxNumber = data.GetLength(0);
                    int highestPercentageReached = 0;

                    int percentComplete = 0;

                    splash = new ThreadedSplashFormController<nowLoading, nowLoading.ProgressChangedEventArgs>(x => x.ProgressChanged);
                    splash.Show();
                    nowLoading.ProgressChangedEventArgs p = new nowLoading.ProgressChangedEventArgs();

                    for (int r = 4; r <= data.GetLength(0); r++)
                    {
                        int school_price =0;
                        int price = 0;
                        string rate2 = "";
                        string rate1 = "";

                        if (Int32.TryParse(data[r, 16] == null ? "" : data[r, 16].ToString(), out school_price))
                        {
                            if (Int32.TryParse(data[r, 15] == null ? "" : data[r, 15].ToString(), out price))
                            {
                                rate2 = string.Format("{0:f2}", ((school_price * 0.9 - price) / (school_price * 0.9)) * 100);
                                rate1 = "10";
                            }
                        }

                        listView1.Items.Add(new ListViewItem(new string[] { "",
                        data[r, 5] == null ? "" : data[r, 5].ToString(), 
                        data[r, 6] == null ? "" : data[r, 6].ToString(), 
                        data[r, 11] == null ? "" : data[r, 11].ToString(), 
                        data[r, 7] == null ? "" : data[r, 7].ToString(), 
                        data[r, 8] == null ? "" : data[r, 8].ToString(), 
                        "",
                        data[r, 16] == null ? "" : data[r, 16].ToString(), 
                        data[r, 15] == null ? "" : data[r, 15].ToString(), 
                        rate1,
                        rate2,
                        "",
                        "",
                        data[r, 13] == null ? "" : data[r, 13].ToString(), 
                        data[r, 10] == null ? "" : data[r, 10].ToString() }));

                        percentComplete = (int)((float)r / (float)maxNumber * 100);
                        if (percentComplete > highestPercentageReached)
                        {
                            p.Progress = percentComplete;
                            splash.OnProgressChanged(this, p);
                            highestPercentageReached = percentComplete;
                            //bw.ReportProgress(percentComplete);
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

                    splash.Close();
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

            using (MySqlConnection conn = new MySqlConnection(strConn))
            {
                //try
                //{
                    conn.Open();

                    int maxNumber = listView1.Items.Count;
                    int highestPercentageReached = 0;

                    int percentComplete = 0;
                    int i = 0;

                    splash = new ThreadedSplashFormController<nowLoading, nowLoading.ProgressChangedEventArgs>(x => x.ProgressChanged);
                    splash.Show();
                    nowLoading.ProgressChangedEventArgs p = new nowLoading.ProgressChangedEventArgs();

                    foreach (ListViewItem item in listView1.Items)
                    {
                        percentComplete = (int)((float)i / (float)maxNumber * 100);

                        MySqlCommand insertCommand = new MySqlCommand();
                        insertCommand.Connection = conn;

                        insertCommand.CommandText = "INSERT INTO product(name, standard, maker, unit, kg, ea, school_price, estimate_price, rate_1, rate_2, original_price, rate_original, account, tax) VALUES(@name, @standard, @maker, @unit, @kg, @ea, @school_price, @estimate_price, @rate_1, @rate_2, @original_price, @rate_original, @account, @tax)";
                        insertCommand.Parameters.AddWithValue("@name", item.SubItems[1].Text);
                        insertCommand.Parameters.AddWithValue("@standard", item.SubItems[2].Text);
                        insertCommand.Parameters.AddWithValue("@maker", item.SubItems[3].Text);
                        insertCommand.Parameters.AddWithValue("@unit", item.SubItems[4].Text);
                        insertCommand.Parameters.AddWithValue("@kg", item.SubItems[5].Text == "" ? null : item.SubItems[5].Text);
                        insertCommand.Parameters.AddWithValue("@ea", item.SubItems[6].Text == "" ? null : item.SubItems[6].Text);
                        insertCommand.Parameters.AddWithValue("@school_price", item.SubItems[7].Text == "" ? null : item.SubItems[7].Text);
                        insertCommand.Parameters.AddWithValue("@estimate_price", item.SubItems[8].Text == "" ? null : item.SubItems[8].Text);
                        insertCommand.Parameters.AddWithValue("@rate_1", item.SubItems[9].Text == "" ? null : item.SubItems[9].Text);
                        insertCommand.Parameters.AddWithValue("@rate_2", item.SubItems[10].Text == "" ? null : item.SubItems[10].Text);
                        insertCommand.Parameters.AddWithValue("@original_price", item.SubItems[11].Text == "" ? null : item.SubItems[11].Text);
                        insertCommand.Parameters.AddWithValue("@rate_original", item.SubItems[12].Text == "" ? null : item.SubItems[12].Text);
                        insertCommand.Parameters.AddWithValue("@account", item.SubItems[13].Text);
                        insertCommand.Parameters.AddWithValue("@tax", item.SubItems[14].Text == "과세" ? 1:0);

                        insertCommand.ExecuteNonQuery();
                        if (percentComplete > highestPercentageReached)
                        {
                            p.Progress = percentComplete;
                            splash.OnProgressChanged(this, p);
                            highestPercentageReached = percentComplete;
                            //bw.ReportProgress(percentComplete);
                        }
                        i++;

                    }
                //}
                //catch (Exception eee)
                //{
                //    MessageBox.Show("저장에 실패하였습니다.");
                //}

                splash.Close();
                conn.Close();
                this.DialogResult = DialogResult.OK;
            }  
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem selectedItem in listView1.SelectedItems)
            {
                listView1.Items.Remove(selectedItem);
            }
        }
    }
}
