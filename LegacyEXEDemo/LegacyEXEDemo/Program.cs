using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyEXEDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var f = System.IO.File.CreateText("monFichierLegacy.txt");
            while (true)
            {
                string msg = "";
                msg += $"[{DateTime.Now.ToString()}] {Environment.MachineName}\r\n";

                f.WriteLine(msg);
                f.Flush();
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
