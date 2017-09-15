using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace TestProject
{
    //public partial class nowLoading : Form
    //{
        //public int ProgressPercentage = 0;
        //public nowLoading()
        //{
        //    InitializeComponent();
        //    Thread t1 = new Thread(new ThreadStart(setProgressBar));
        //    t1.Start();
        //}

        //public void initProgressBar() 
        //{
        //    progressBar1.Value = 0;
        //}

        //void setProgressBar()
        //{
        //    //progressBar1.Value = ProgressPercentage;
        //    while(true) Console.WriteLine(ProgressPercentage);
        //}

        public partial class nowLoading : Form
        {
            public nowLoading()
            {
                InitializeComponent();
                this.progressBar1.Value = 0;
            }
            //스플래쉬폼 프로그레스 이벤트 예
            public void ProgressChanged(object sender, ProgressChangedEventArgs e)
            {
                this.progressBar1.Value = (int)e.Progress;
            }
            public class ProgressChangedEventArgs : EventArgs
            {
                public int Progress { get; set; }
            }
            private void ThreadedSplashForm_Load(object sender, EventArgs e)
            {
            }
        }


        //public class nowLoading<T, P>
        //    where T : Form, new()
        //    where P : EventArgs
        //{
        //    EventHandler<P> p = null;
        //    private System.Threading.Thread t = null;
        //    T splashForm = null;
        //    private nowLoading()
        //    {
        //        splashForm = new T();
        //    }
        //    public nowLoading(Func<T, EventHandler<P>> e)
        //    {
        //        splashForm = new T();
        //        p = e.Invoke(splashForm);
        //    }
        //    public void Show()
        //    {

        //        t = new System.Threading.Thread(_Show);
        //        t.Start();
        //        while (true)
        //        {
        //            System.Threading.Thread.Sleep(0);
        //            if (t.ThreadState == System.Threading.ThreadState.Running) break;
        //        }
        //    }
        //    private void _Show()
        //    {

        //        Application.Run(splashForm);
        //    }
        //    public void Close()
        //    {
        //        splashForm.Invoke(new MethodInvoker(_Close));
        //        if (t.Join(1000) == false)
        //        {
        //            t.Abort();
        //        }
        //    }
        //    private void _Close()
        //    {
        //        splashForm.Close();
        //    }
        //    public void OnProgressChanged(object sender, P e)
        //    {
        //        if (p == null) return;
        //        try
        //        {
        //            splashForm.Invoke(new EventHandler<P>(_OnProgressChanged), new object[] { sender, e });
        //        }
        //        catch (Exception)
        //        {
        //        }
        //    }

        //    private void _OnProgressChanged(object sender, P e)
        //    {
        //        p(sender, e);
        //    }
        //}
    //}
}
