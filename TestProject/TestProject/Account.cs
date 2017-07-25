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
    public partial class Account : Form
    {
        public Account()
        {
            InitializeComponent();
            this.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.TopLevel = false;
            comboBox1.SelectedIndex = 0;

            listView1.Items.Add(new ListViewItem(new string[] {"1", "은하수아파트", "042-485-8384", "010-7236-8384","042-629-8012", "스마트앤스페이스", "02345855894", "대전광역시 서구 둔산2동 은하수아파트 103-1109" }));
        }

        private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            Color c = Color.FromArgb(119,199,224);
            e.Graphics.FillRectangle(new SolidBrush(c), e.Bounds);
            //e.Graphics.FillRectangle(Brushes.Aqua, e.Bounds);
            e.DrawText();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            newAccount addAccount = new newAccount();

            if (addAccount.ShowDialog() == DialogResult.OK)
            {

            }
        }
    }
}
