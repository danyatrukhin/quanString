using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CS001v3
{
    class QuanString
    {
        public String directory;
        public String[] masks;
        public int countMask;
        List<String> files;
        public int quantityString;
        public int quantityEmptyString;
        public int quantityComments;
        public int quantityMixedComments;
        public int quantityCode;
        static Mutex mtx;
        bool isComment;
        public QuanString(int quantityMasks)
        {
            masks = new String[quantityMasks];
            countMask = quantityMasks;
            quantityString = 0;
            quantityEmptyString = 0;
            quantityComments = 0;
            quantityMixedComments = 0;
            quantityCode = 0;
            mtx = new Mutex();
            files = new List<String>();
            isComment = false;
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
                foreach (String line in File.ReadLines(files[i]))
                {
                    if (String.Compare(files[i], files[i].Length - 3, ".cs", 0, 3) == 0)
                    {
                        if (String.IsNullOrWhiteSpace(line))
                            ++quantityEmptyString;
                        else
                        {
                            int resultCom = HasComments(line);
                            if (resultCom == 0)
                                ++quantityCode;
                            if (resultCom == 1)
                                ++quantityComments;
                            if (resultCom == 2)
                                ++quantityMixedComments;
                        }
                    }
                    ++quantityString;
                }
                Thread.Sleep(1000);
            }
        }

        int HasComments(String line)
        {
            int i = 0, countQuotes = 0, x = -1, j, k, l;
            int result = 0;
            j = line.IndexOf("//", 0);
            k = line.IndexOf("/*", 0);
            l = line.IndexOf("*/", 0);
            while (i != -1)
            {
                i = line.IndexOf("\"", x + 1);
                x = i;
                if ((j > i || k > i) && i != -1)
                    ++countQuotes;
            }

            if (countQuotes % 2 == 0 && !isComment)
            {
                if (line.Contains("//"))
                {
                    if (String.IsNullOrWhiteSpace(line.Substring(0, j)))
                        return 1;
                    else
                        return 2;
                }
                if (line.Contains("/*"))
                {
                    isComment = true;

                    if (String.IsNullOrWhiteSpace(line.Substring(0, k)))
                        result = 1;
                    else
                        result = 2;
                    if (!line.Contains("*/"))
                        return result;
                }
            }
            if (line.Contains("*/"))
            {
                isComment = false;
                if (line.Length - l + 2 == 0 || String.IsNullOrWhiteSpace(line.Substring(l, line.Length - (l + 2))))
                    return 1;
                else
                    return 2;
            }
            if (isComment)
                return 1;
            return result;
        }

    }
}
