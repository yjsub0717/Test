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
            this.간접납품ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.거래처관리ToolStripMenuItem,
            this.품목관리ToolStripMenuItem,
            this.견적서등록ToolStripMenuItem,
            this.납품지시서ToolStripMenuItem,
            this.간접납품ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1530, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 거래처관리ToolStripMenuItem
            // 
            this.거래처관리ToolStripMenuItem.Name = "거래처관리ToolStripMenuItem";
            this.거래처관리ToolStripMenuItem.Size = new System.Drawing.Size(83, 20);
            this.거래처관리ToolStripMenuItem.Text = "거래처 관리";
            this.거래처관리ToolStripMenuItem.Click += new System.EventHandler(this.거래처관리ToolStripMenuItem_Click);
            // 
            // 품목관리ToolStripMenuItem
            // 
            this.품목관리ToolStripMenuItem.Name = "품목관리ToolStripMenuItem";
            this.품목관리ToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.품목관리ToolStripMenuItem.Text = "품목 관리";
            this.품목관리ToolStripMenuItem.Click += new System.EventHandler(this.품목관리ToolStripMenuItem_Click);
            // 
            // 견적서등록ToolStripMenuItem
            // 
            this.견적서등록ToolStripMenuItem.Name = "견적서등록ToolStripMenuItem";
            this.견적서등록ToolStripMenuItem.Size = new System.Drawing.Size(83, 20);
            this.견적서등록ToolStripMenuItem.Text = "견적서 등록";
            this.견적서등록ToolStripMenuItem.Click += new System.EventHandler(this.견적서등록ToolStripMenuItem_Click);
            // 
            // 납품지시서ToolStripMenuItem
            // 
            this.납품지시서ToolStripMenuItem.Name = "납품지시서ToolStripMenuItem";
            this.납품지시서ToolStripMenuItem.Size = new System.Drawing.Size(79, 20);
            this.납품지시서ToolStripMenuItem.Text = "납품지시서";
            this.납품지시서ToolStripMenuItem.Click += new System.EventHandler(this.납품지시서ToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl1.Location = new System.Drawing.Point(10, 25);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1509, 602);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabcontrol1_DrawItem);
            this.tabControl1.Click += new System.EventHandler(this.tabControl1_MouseClick);
            // 
            // 간접납품ToolStripMenuItem
            // 
            this.간접납품ToolStripMenuItem.Name = "간접납품ToolStripMenuItem";
            this.간접납품ToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.간접납품ToolStripMenuItem.Text = "간접납품";
            this.간접납품ToolStripMenuItem.Click += new System.EventHandler(this.간접납품ToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1530, 637);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
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
        private System.Windows.Forms.ToolStripMenuItem 간접납품ToolStripMenuItem;
    }
}

