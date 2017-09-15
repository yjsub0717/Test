using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestProject
{
    public class ThreadedSplashFormController<T, P>
        where T : Form, new()
        where P : EventArgs
    {
        EventHandler<P> p = null;
        private System.Threading.Thread t = null;
        T splashForm = null;
        private ThreadedSplashFormController()
        {
            splashForm = new T();
        }
        public ThreadedSplashFormController(Func<T, EventHandler<P>> e)
        {
            splashForm = new T();
            p = e.Invoke(splashForm);
        }
        public void Show()
        {

            t = new System.Threading.Thread(_Show);
            t.Start();
            while (true)
            {
                System.Threading.Thread.Sleep(0);
                if (t.ThreadState == System.Threading.ThreadState.Running) break;
            }
        }
        private void _Show()
        {

            Application.Run(splashForm);
        }
        public void Close()
        {
            splashForm.Invoke(new MethodInvoker(_Close));
            if (t.Join(1000) == false)
            {
                t.Abort();
            }
        }
        private void _Close()
        {
            splashForm.Close();
        }
        public void OnProgressChanged(object sender, P e)
        {
            if (p == null) return;
            try
            {
                splashForm.Invoke(new EventHandler<P>(_OnProgressChanged), new object[] { sender, e });
            }
            catch (Exception)
            {
            }
        }

        private void _OnProgressChanged(object sender, P e)
        {
            p(sender, e);
        }
    }
}
