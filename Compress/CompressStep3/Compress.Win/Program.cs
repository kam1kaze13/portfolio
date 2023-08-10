using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compress.Win
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MainForm());
            //var args = arg.Split(';');

            //args = Environment.GetCommandLineArgs();

            if (args.Length == 0)
            {
                Application.Run(new MainForm(Path.GetFullPath(Directory.GetCurrentDirectory())));
            } else if (args.Length == 1)
            {
                Application.Run(new MainForm(args[0]));
            } else
            {
                var arguments = args[1].Split(';');
                if (arguments[0] == "a")
                {
                    Application.Run(new AddForm(arguments[1], arguments.Skip(2).ToList()));
                }
            }
        }
    }
}
