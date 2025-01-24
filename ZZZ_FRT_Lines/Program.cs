using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Login_Form

{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (DateTime.Now.TimeOfDay >= new TimeSpan(20, 0, 0)) // 20:00 or 8:00 PM
            {
                //MessageBox.Show("See you in the morning!");
                //CloseApplication();
                //return; // Close the application
            }

            Timer timer = new Timer();
            timer.Interval = 60000; // Check every minute (adjust as needed)
            timer.Tick += (sender, e) =>
            {
                TimeSpan currentTime = DateTime.Now.TimeOfDay;

                // Check if it's around 7:50 PM to display the warning message
                TimeSpan warningTimeStart = new TimeSpan(19, 49, 30);
                TimeSpan warningTimeEnd = new TimeSpan(19, 50, 30);
                if (currentTime >= warningTimeStart && currentTime <= warningTimeEnd)
                {
                    MessageBox.Show("This application will close in 10 minutes. Please save your work and log off.", "Closing warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Check if it's past 8:00 PM to close the application
                TimeSpan closingTime = new TimeSpan(20, 0, 0);
                if (currentTime >= closingTime)
                {
                    //CloseApplication();
                    timer.Stop();
                }
            };
            timer.Start();


            string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            if (username.Contains("adodge") || username.Contains("dbredehoeft") || username.Contains("DBREDEHOEFT"))
            {
                Application.Run(new FormMain9.FormMain9());
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
            }
            else
            {
                Application.Run(new LoginForm());
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
            }

        }

        static void CloseApplication()
        {
            foreach (Process proc in Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName))
            {
                if (proc.Id != Process.GetCurrentProcess().Id)
                {
                    proc.Kill();
                }
            }
            Application.Exit();
        }
    }
}
