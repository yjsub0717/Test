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
    public partial class checkDelivery : Form
    {
        String strConn = "Server=13.124.90.82; Port=3306; Database=rntp; Uid=root; Pwd=rntprntp;";
        ThreadedSplashFormController<nowLoading, nowLoading.ProgressChangedEventArgs> splash = null;
        DataSet ds = new DataSet();

        string name;
        string date;

        public checkDelivery(string name, string date)
        {
            InitializeComponent();

            this.name = name;
            this.date = date;

            printList();
            calculateTotal();
            check();
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

        private void printList()
        {

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
                int morning = -1;
                int launch = -1;
                int dinner = -1;

                if (ds.Tables[0].Rows.Count == 1)
                {
                    id = (int)ds.Tables[0].Rows[0]["id"];
                    morning = ds.Tables[0].Rows[0]["morning"].ToString() == "" ? -1 : (int)ds.Tables[0].Rows[0]["morning"];
                    launch = ds.Tables[0].Rows[0]["launch"].ToString() == "" ? -1 : (int)ds.Tables[0].Rows[0]["launch"];
                    dinner = ds.Tables[0].Rows[0]["dinner"].ToString() == "" ? -1 : (int)ds.Tables[0].Rows[0]["dinner"];
                }

                sql = "SELECT * FROM `estimateItem` WHERE estimate_id = " + id + " ORDER BY no";

                ds.Clear();
                adpt = new MySqlDataAdapter(sql, conn);
                adpt.Fill(ds);



                int maxNumber = ds.Tables[0].Rows.Count + ((morning == -1 ? 0 : 1) * ds.Tables[0].Rows.Count) + ((launch == -1 ? 0 : 1) * ds.Tables[0].Rows.Count) + ((dinner == -1 ? 0 : 1) * ds.Tables[0].Rows.Count);
                int highestPercentageReached = 0;

                int percentComplete = 0;
                int i = 0;

                // AND product_id = (SELECT estimateItem.product_id FROM 'estimateItem' WHERE estimateItem.estimate_id = 22)
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
                                textTrans(row["total"].ToString()),
                                "",
                                "",
                                "",
                                ""}));
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
                                "",
                                "",
                                "",
                                "" }));
                    }
                }

                if (morning != -1)
                {
                    sql = "SELECT * FROM `deliveryItem` WHERE delivery_id = " + morning + " ORDER BY no";

                    ds.Clear();
                    adpt = new MySqlDataAdapter(sql, conn);
                    adpt.Fill(ds);
                    // AND product_id = (SELECT estimateItem.product_id FROM 'estimateItem' WHERE estimateItem.estimate_id = 22)

                    int lineCount = 0;
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

                        listView1.Items[lineCount].SubItems[7].Text = (textTrans(row["total_excel"].ToString()));
                        lineCount++;
                    }
                }
                if (launch != -1)
                {
                    sql = "SELECT * FROM `deliveryItem` WHERE delivery_id = " + launch + " ORDER BY no";

                    ds.Clear();
                    adpt = new MySqlDataAdapter(sql, conn);
                    adpt.Fill(ds);
                    // AND product_id = (SELECT estimateItem.product_id FROM 'estimateItem' WHERE estimateItem.estimate_id = 22)

                    int lineCount = 0;
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

                        listView1.Items[lineCount].SubItems[8].Text = (textTrans(row["total_excel"].ToString()));
                        lineCount++;
                    }
                }
                if (dinner != -1)
                {
                    sql = "SELECT * FROM `deliveryItem` WHERE delivery_id = " + dinner + " ORDER BY no";

                    ds.Clear();
                    adpt = new MySqlDataAdapter(sql, conn);
                    adpt.Fill(ds);
                    // AND product_id = (SELECT estimateItem.product_id FROM 'estimateItem' WHERE estimateItem.estimate_id = 22)

                    int lineCount = 0;
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

                        listView1.Items[lineCount].SubItems[9].Text = (textTrans(row["total_excel"].ToString()));
                        lineCount++;
                    }
                }


                splash.Close();
                conn.Close();
            }
        }

        private void calculateTotal()
        {
            foreach (ListViewItem item in listView1.Items)
            {
                int morning = item.SubItems[7].Text == "" ? 0 : Convert.ToInt32(item.SubItems[7].Text.Replace(",", ""));
                int launch = item.SubItems[8].Text == "" ? 0 : Convert.ToInt32(item.SubItems[8].Text.Replace(",", ""));
                int dinner = item.SubItems[9].Text == "" ? 0 : Convert.ToInt32(item.SubItems[9].Text.Replace(",", ""));

                int total = morning + launch + dinner;
                item.SubItems[10].Text = textTrans(total.ToString());
            }
        }

        private void check()
        {
            foreach (ListViewItem item in listView1.Items)
            {
                if (item.SubItems[6].Text != item.SubItems[10].Text)
                {
                    item.BackColor = Color.Red;
                }
                else
                    item.BackColor = Color.White;
            }
        }
    }
}
