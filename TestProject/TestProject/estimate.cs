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
        List<string> testData = new List<string>() { "Excel", "Access", "Word", "OneNote" };

        Excel.Application excelApp = null;
        Excel.Workbook wb = null;
        Excel.Worksheet ws = null;

        string name;
        string date;
        Boolean launch;
        Boolean isNew = true;

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
            newEstimate newForm = new newEstimate();

            if(newForm.ShowDialog() == DialogResult.OK)
            {
                this.name = newForm.name;
                this.date = newForm.date;
                this.launch = newForm.launch;

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm|All files|*.*";
                String FileName = null;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    isNew = true;
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

                        for (int r = 2; r <= data.GetLength(0); r++)
                        {
                            listView1.Items.Add(new ListViewItem(new string[] {
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
                }
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                ListView.SelectedListViewItemCollection items = listView1.SelectedItems;
                ListViewItem lvItem = items[0];
                string name = lvItem.SubItems[12].Text;

                selectProduct newForm = new selectProduct();

                newForm.setKeyword(name);

                if(newForm.ShowDialog() == DialogResult.OK)
                {
                    lvItem.SubItems[1].Text = newForm.name;
                    lvItem.SubItems[2].Text = newForm.maker;
                    lvItem.SubItems[3].Text = newForm.standard;
                    lvItem.SubItems[4].Text = newForm.unit;
                    lvItem.SubItems[6].Text = newForm.str_estimate_price;
                    lvItem.SubItems[7].Text = newForm.str_school_price;

                    lvItem.SubItems[8].Text = (double.Parse(lvItem.SubItems[5].Text) * Int32.Parse(lvItem.SubItems[7].Text)).ToString();
                }
            }
        }

        // 저장
        private void button3_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(strConn))
            {
                conn.Open();

                if(isNew)
                {
                    MySqlCommand insertCommand = new MySqlCommand();
                    insertCommand.Connection = conn;
                    insertCommand.CommandText = "INSERT INTO estimateList(account, date, launch) VALUES(@account, @date, @launch)";
                    insertCommand.Parameters.AddWithValue("@account", this.name);
                    insertCommand.Parameters.AddWithValue("@date", this.date);
                    if (this.launch)
                        insertCommand.Parameters.AddWithValue("@launch", 1);
                    else
                        insertCommand.Parameters.AddWithValue("@launch", 0);

                    insertCommand.ExecuteNonQuery();
                }

                // 견적서 번호 알아와서 견적 아이템 테이블에 넣기

                conn.Close();
            }   
        }

        // 불러오기
        private void button2_Click(object sender, EventArgs e)
        {
            isNew = false;   
        }


    }
}
