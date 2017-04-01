using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CS001v2
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                System.Console.WriteLine("Please enter arguments.\nusage: CS001.exe <directory> <mask1> ... <maskN>\nf.e. \"CS001.exe C:\\temp *.txt *.cs\"");
                return;
            }
            QuanString quanString = new QuanString(args.Length - 1);
            quanString.directory = args[0];
            for (int i = 1; i <= quanString.countMask; ++i)
                quanString.masks[i - 1] = args[i];
            if (!quanString.CheckMask() || !quanString.CheckDir())
            {
                Console.WriteLine("Wrong mask or directory!\n");
                return;
            }
            quanString.Count();
            Console.WriteLine("Quantity of string is: " + quanString.quantityString);
            //Console.Read();
        }
    }
}
