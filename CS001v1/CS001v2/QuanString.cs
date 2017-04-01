using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CS001v2
{
    class QuanString
    {
        public String directory;
        public String[] masks;
        public int countMask;
        public List<String> files;
        public int quantityString;
        public static Mutex mtx;
        public QuanString(int quantityMasks)
        {
            masks = new String[quantityMasks];
            countMask = quantityMasks;
            quantityString = 0;
            mtx = new Mutex();
            files = new List<String>();
        }
        public bool CheckMask()
        {
            Regex rgx = new Regex(@"[*][.][a-zA-Z0-9]+");
            foreach (string mask in masks)
                if (!rgx.IsMatch(mask))
                    return false;
            return true;
        }

        public bool CheckDir()
        {
            Regex rgx = new Regex(@"[A-Z][:]([\][a-zA-Z0-9а-яА-Я ]*)*");
            if (!rgx.IsMatch(directory) || !Directory.Exists(directory))
                return false;
            return true;
        }

        public void Count()
        {
            IEnumerable<String> str;
            Thread thread = new Thread(ThreadFunc);
            thread.Start();
            foreach (string m in masks)
            {
                str = Directory.EnumerateFiles(directory, m, SearchOption.AllDirectories);
                foreach (string s in str)
                {
                    mtx.WaitOne();
                    files.Add(s);
                    mtx.ReleaseMutex();
                }
            }
            thread.Join();
        }
        public void ThreadFunc()
        {
            for (int i = 0; i < files.Count; ++i)
            {
                quantityString += File.ReadAllLines(files[i]).Length;
                Thread.Sleep(500);
            }
        }
    }
}
