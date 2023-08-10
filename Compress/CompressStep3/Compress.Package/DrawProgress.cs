using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Compress.Package
{
    public class DrawProgress
    {
        public DrawProgress()
        {
            Timer timer = new Timer(1000/30);

            timer.Elapsed += PrintProgress;

            this.cursorLeft = Console.CursorLeft;
            this.cursorTop = Console.CursorTop;
        }
        public void SetArgs(FileProcessingEventArgs args)
        {
            this.args = args;
            this.changed = true;
        }

        public void PrintProgress(object source, ElapsedEventArgs e)
        {
            if (this.changed)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine($"File name: {this.args.FileName}");
                Console.WriteLine($"Operation: {this.args.Type}");
                Console.WriteLine($"File size: {this.args.Total}");
                Console.WriteLine($"Done: {this.args.TotalProcessed}");
            }
            this.changed = false;
        }

        private bool changed = false;
        private FileProcessingEventArgs args;
        private int cursorLeft;
        private int cursorTop;
    }
}
