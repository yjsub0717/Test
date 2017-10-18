using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestProject
{
    public partial class Form1 : Form
    {
        private Point _imageLocation = new Point(18, 5);
        private Point _imgHitArea = new Point(15, 3);

        public Form1()
        {
            InitializeComponent();
        }

        private void 거래처관리ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage newPage = new TabPage("거래처 관리    ");
            Account newAccount = new Account();
            newAccount.Size = newPage.Size;
            newAccount.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            newPage.Controls.Add(newAccount);
            newPage.Controls[0].Show();
            tabControl1.TabPages.Add(newPage);
            tabControl1.SelectedTab = newPage;
        }

        private void tabControl1_MouseClick(object sender, EventArgs e)
        {
            try
            {
                TabControl tc = (TabControl)sender;
                MouseEventArgs ee = (MouseEventArgs)e;
                Point p = ee.Location;
                int _tabWidth = 0;
                _tabWidth = this.tabControl1.GetTabRect(tc.SelectedIndex).Width - (_imgHitArea.X);
                Rectangle r = this.tabControl1.GetTabRect(tc.SelectedIndex);
                r.Offset(_tabWidth, _imgHitArea.Y);
                r.Width = 16;
                r.Height = 16;
                if (r.Contains(p))
                {
                    TabPage TabP = (TabPage)tc.TabPages[tc.SelectedIndex];
                    tc.TabPages.Remove(TabP);
                }
            }
            catch(Exception eee)
            {

            }
        }

        private void tabcontrol1_DrawItem(object sender, DrawItemEventArgs e)
        {

            
            try
            {
                Image img;
                Font f = this.Font;
                Rectangle r = e.Bounds;
                Brush titleBrush = new SolidBrush(Color.Black);
                string title = this.tabControl1.TabPages[e.Index].Text;

                r = this.tabControl1.GetTabRect(e.Index);
                r.Offset(2, 2);

                // SelectedTab의 Background Color 는 White으로 처리
                if (this.tabControl1.SelectedIndex == e.Index)
                    e.Graphics.FillRectangle(new SolidBrush(Color.White), e.Bounds);

                // 각 Tab별로 close button 에 대한 image값 
                if (this.tabControl1.SelectedTab == this.tabControl1.TabPages[e.Index])
                    img = Properties.Resources.Close_white;
                else
                    img = Properties.Resources.Close_Gray;

                // TabPage Text
                e.Graphics.DrawString(title, f, titleBrush, new PointF(r.X, r.Y));

                // TabPage 의 닫기 버튼
                e.Graphics.DrawImage(img, new Point(r.X + this.tabControl1.GetTabRect(e.Index).Width - _imageLocation.X, _imageLocation.Y));
                img.Dispose();
                img = null;
            }

            catch (Exception)
            {
            }
        }

        private void 품목관리ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage newPage = new TabPage("품목 관리    ");
            product newProduct = new product();
            newProduct.Size = newPage.Size;
            newProduct.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            newPage.Controls.Add(newProduct);
            newPage.Controls[0].Show();
            tabControl1.TabPages.Add(newPage);
            tabControl1.SelectedTab = newPage;
        }

        private void 견적서등록ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage newPage = new TabPage("견적서 관리    ");
            estimate newEstimate = new estimate();
            newEstimate.Size = newPage.Size;
            newEstimate.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            newPage.Controls.Add(newEstimate);
            newPage.Controls[0].Show();
            tabControl1.TabPages.Add(newPage);
            tabControl1.SelectedTab = newPage;
        }

        private void 납품지시서ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage newPage = new TabPage("납품지시서 관리    ");
            delivery newDelivery = new delivery();
            newDelivery.Size = newPage.Size;
            newDelivery.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            newPage.Controls.Add(newDelivery);
            newPage.Controls[0].Show();
            tabControl1.TabPages.Add(newPage);
            tabControl1.SelectedTab = newPage;
        }

        private void 간접납품ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage newPage = new TabPage("간접납품 관리    ");
            indirect newIndirect = new indirect();
            newIndirect.Size = newPage.Size;
            newIndirect.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            newPage.Controls.Add(newIndirect);
            newPage.Controls[0].Show();
            tabControl1.TabPages.Add(newPage);
            tabControl1.SelectedTab = newPage;
        }

        private void 매출자료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage newPage = new TabPage("매출자료 관리    ");
            saleData newsaleData = new saleData();
            newsaleData.Size = newPage.Size;
            newsaleData.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            newPage.Controls.Add(newsaleData);
            newPage.Controls[0].Show();
            tabControl1.TabPages.Add(newPage);
            tabControl1.SelectedTab = newPage;
        }
    }
}
