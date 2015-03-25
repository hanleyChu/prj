using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MVCSharp.Core.Configuration;
using MVCSharp.Core.Tasks;
using MVCSharp.Winforms;
using Hoe.Basic.AppLogic;
using Hoe.Basic.View;
using Hoe.Basic.Model;
using Hoe.Basic.DB;
using Hoe.App.UI.Admin;
using System.Diagnostics;

namespace Hoe.App
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // login
            new LoginForm().ShowDialog();

            if (LoginForm.loginType=="software") 
            {
                if (!DatabaseHelper.IsDatabaseAlive())
                {
                    MessageBox.Show("亲，数据库服务没有启动~,请检查MongoDB服务是否已经正常启动了");

                    Application.Exit();
                    return;
                }

                TasksManager tasksManager = new TasksManager(WinformsViewsManager.
                                                         GetDefaultConfig());
                tasksManager.StartTask(typeof(SaleTask));

                Application.Run(Application.OpenForms[0]);
            }
            else if(LoginForm.loginType=="spider")
            {
                startSpiderGame();

            }
        }

        private static void startSpiderGame()
        {
            

            ProcessStartInfo p=null;
            Process Proc;    
              
            p = new ProcessStartInfo(@"spider\spider.exe");
            //p.WorkingDirectory = exepath;//设置此外部程序所在windows目录
            p.WindowStyle = ProcessWindowStyle.Hidden;//在调用外部exe程序的时候，控制台窗口不弹出

            Proc  =  Process.Start(p);//调用外部程序

        }

    }
}