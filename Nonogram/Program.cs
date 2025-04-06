using System;
using System.Windows.Forms;

namespace Nonogram
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ApplicationConfiguration.Initialize();
            LoginForm loginForm = new LoginForm();
            Application.Run(loginForm);
        }
    }
}