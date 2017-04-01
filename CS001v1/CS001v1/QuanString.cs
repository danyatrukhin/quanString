using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CS001v1
{
    class QuanString
    {
        public String directory;
        public String[] masks;
        public int countMask;
        public QuanString(int quantityMasks)
        {
            masks = new String[quantityMasks];
            countMask = quantityMasks;
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

        public int Count()
        {
            int quantityString = 0;
            foreach (string m in masks)
                foreach (var file in Directory.EnumerateFiles(directory, m, SearchOption.AllDirectories))
                    quantityString += File.ReadAllLines(file).Length;
            return quantityString;
        }
    }
}
