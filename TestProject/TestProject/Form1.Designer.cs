namespace TestProject
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.거래처관리ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.품목관리ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.견적서등록ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.납품지시서ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.거래처관리ToolStripMenuItem,
            this.품목관리ToolStripMenuItem,
            this.견적서등록ToolStripMenuItem,
            this.납품지시서ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1749, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 거래처관리ToolStripMenuItem
            // 
            this.거래처관리ToolStripMenuItem.Name = "거래처관리ToolStripMenuItem";
            this.거래처관리ToolStripMenuItem.Size = new System.Drawing.Size(101, 24);
            this.거래처관리ToolStripMenuItem.Text = "거래처 관리";
            this.거래처관리ToolStripMenuItem.Click += new System.EventHandler(this.거래처관리ToolStripMenuItem_Click);
            // 
            // 품목관리ToolStripMenuItem
            // 
            this.품목관리ToolStripMenuItem.Name = "품목관리ToolStripMenuItem";
            this.품목관리ToolStripMenuItem.Size = new System.Drawing.Size(86, 24);
            this.품목관리ToolStripMenuItem.Text = "품목 관리";
            // 
            // 견적서등록ToolStripMenuItem
            // 
            this.견적서등록ToolStripMenuItem.Name = "견적서등록ToolStripMenuItem";
            this.견적서등록ToolStripMenuItem.Size = new System.Drawing.Size(101, 24);
            this.견적서등록ToolStripMenuItem.Text = "견적서 등록";
            // 
            // 납품지시서ToolStripMenuItem
            // 
            this.납품지시서ToolStripMenuItem.Name = "납품지시서ToolStripMenuItem";
            this.납품지시서ToolStripMenuItem.Size = new System.Drawing.Size(96, 24);
            this.납품지시서ToolStripMenuItem.Text = "납품지시서";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl1.Location = new System.Drawing.Point(12, 31);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1725, 753);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabcontrol1_DrawItem);
            this.tabControl1.Click += new System.EventHandler(this.tabControl1_MouseClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1749, 796);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Click += new System.EventHandler(this.tabControl1_MouseClick);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 거래처관리ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 품목관리ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 견적서등록ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 납품지시서ToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
    }
}

