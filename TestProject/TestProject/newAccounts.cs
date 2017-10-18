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
    public partial class newAccounts : Form
    {
        Excel.Application excelApp = null;
        Excel.Workbook wb = null;
        Excel.Worksheet ws = null;
        ThreadedSplashFormController<nowLoading, nowLoading.ProgressChangedEventArgs> splash = null;

        String strConn = "Server=13.124.90.82; Port=3306; Database=rntp; Uid=root; Pwd=rntprntp;";
        DataSet ds = new DataSet();

        public newAccounts()
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


                    splash = new ThreadedSplashFormController<nowLoading, nowLoading.ProgressChangedEventArgs>(x => x.ProgressChanged);
                    splash.Show();
                    nowLoading.ProgressChangedEventArgs p = new nowLoading.ProgressChangedEventArgs();


                    int maxNumber = data.GetLength(0);
                    int highestPercentageReached = 0;

                    int percentComplete = 0;

                    for (int r = 4; r <= data.GetLength(0); r++)
                    {
                        if (data[r, 5] != null)
                        {
                            listView1.Items.Add(new ListViewItem(new string[] {
                            (r-3).ToString(),
                            data[r, 5] == null ? "" : data[r, 5].ToString(), 
                            data[r, 11] == null ? "" : data[r, 11].ToString(), 
                            data[r, 12] == null ? "" : data[r, 12].ToString(), 
                            data[r, 13] == null ? "" : data[r, 13].ToString(), 
                            data[r, 15] == null ? "" : data[r, 15].ToString(), 
                            data[r, 17] == null ? "" : data[r, 17].ToString(), 
                            data[r, 19] == null ? "" : data[r, 19].ToString() }));


                            percentComplete = (int)((float)r / (float)maxNumber * 100);
                            if (percentComplete > highestPercentageReached)
                            {
                                p.Progress = percentComplete;
                                splash.OnProgressChanged(this, p);
                                highestPercentageReached = percentComplete;
                                //bw.ReportProgress(percentComplete);
                            }
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
                    splash.Close();
                    excelApp.Quit();
                    listView1.Focus();
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

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem selectedItem in listView1.SelectedItems)
            {
                listView1.Items.Remove(selectedItem);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //if (bw.IsBusy != true)
            //{
                //bw.RunWorkerAsync();
                
            using (MySqlConnection conn = new MySqlConnection(strConn))
            {
                try
                {
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
                        insertCommand.CommandText = "INSERT INTO account(name, phone, cellphone, fax, shopname, shopid, address) VALUES(@name, @phone, @cellphone, @fax, @shopname, @shopid, @address)";

                        insertCommand.Parameters.AddWithValue("@name", item.SubItems[1].Text);
                        insertCommand.Parameters.AddWithValue("@phone", item.SubItems[2].Text);
                        insertCommand.Parameters.AddWithValue("@cellphone", item.SubItems[3].Text);
                        insertCommand.Parameters.AddWithValue("@fax", item.SubItems[4].Text);
                        insertCommand.Parameters.AddWithValue("@shopname", item.SubItems[5].Text);
                        insertCommand.Parameters.AddWithValue("@shopid", item.SubItems[6].Text);
                        insertCommand.Parameters.AddWithValue("@address", item.SubItems[7].Text);

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
                }
                catch (Exception eee)
                {
                    MessageBox.Show("저장에 실패하였습니다.");
                }

                splash.Close();
                conn.Close();
                this.DialogResult = DialogResult.OK;
            }  
               
            //}
        }
    }
}
