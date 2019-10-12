using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MesManager.UI;

namespace MesManager
{
    static class Program
    {
        private static ApplicationContext applicationContext;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MESMainForm());
            //return;
            WelcomeForm welcomeForm = new WelcomeForm();
            welcomeForm.Show();
            applicationContext = new ApplicationContext();
            applicationContext.Tag = welcomeForm;
            Application.Idle += Application_Idle;
            Application.Run(applicationContext);
        }

        private static void Application_Idle(object sender, EventArgs e)
        {
            Application.Idle -= new EventHandler(Application_Idle);
            if (applicationContext.MainForm == null)
            {
                MESMainForm mainForm = new MESMainForm();
                applicationContext.MainForm = mainForm;
                //初始化
                mainForm.InitMain();

                WelcomeForm welcomeForm = applicationContext.Tag as WelcomeForm;
                welcomeForm.Close();

                mainForm.Show();
            }
        }
    }
}
