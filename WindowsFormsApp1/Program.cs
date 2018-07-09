using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    static class Program
    {
        public enum Mode
        {
            view=1,
            review =2,
            quickCheck=3

            
        }

        public static string filePath;
        public static Mode workType;

        public static string ProjectName;
        public static string UnitName;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            


        }
    }
}
