using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Proviser.Servises
{
    public static class FileManager
    {
        public static string GeneralPath()
        {
            return @"/storage/emulated/0/Proviser/";
        }

        public static string GeneralPath(string _file)
        {
            return Path.Combine(@"/storage/emulated/0/Proviser/", _file);
        }
        public static bool FileInit()
        {
            bool refresh = false;

            if (!Directory.Exists(GeneralPath()))
            {
                Directory.CreateDirectory(GeneralPath());
                refresh = true;
            }

            if (!File.Exists(GeneralPath("log.txt")))
            {
                File.Create(GeneralPath("log.txt"));
                refresh = true;
            }

            if (!File.Exists(GeneralPath("mail.txt")))
            {
                File.Create(GeneralPath("mail.txt"));
                refresh = true;
            }

            if (!File.Exists(GeneralPath("sniffer.txt")))
            {
                File.Create(GeneralPath("sniffer.txt"));
                refresh = true;
            }

            if (refresh == true)
            {
                return true;
            }

            return false;
        }

        public static void WriteLog(string _teg, string _case, string _value)
        {
            using (StreamWriter sw = new StreamWriter(GeneralPath("log.txt"), append: true))
            {
                sw.WriteLine($"{DateTime.Now}\t{_teg}\t{_case}\t{_value}");
            }
        }

        public static bool FirstStart()
        {
            if (File.GetLastWriteTime(GeneralPath("log.txt")).DayOfYear != DateTime.Now.DayOfYear)
            {
                
                return true;
            }

            return false;
        }

        public static string GetMail()
        {
            string result = "";
            using (StreamReader sr = new StreamReader(GeneralPath("mail.txt")))
            {
                result = sr.ReadToEnd();
            }

            if (result != "")
            {                
                return result.Replace(" ", "").Replace("\n", "");
            }
            else
            {
                return "";
            }
        }

        public static List<string> GetSniffer()
        {
            List<string> result = new List<string>();
            string[] array;

            using (StreamReader sr = new StreamReader(GeneralPath("sniffer.txt")))
            {
                array = sr.ReadToEnd().Split(",");
            }
            Debug.WriteLine("array" + array.Length);
            if (array.Length > 1)
            {
                foreach (string str in array)
                {
                    result.Add(str);
                }

                return result;
            }
            else
            {
                return null;
            }
        }

        public static void SetMail(string _value)
        {
            using(StreamWriter sw = new StreamWriter(GeneralPath("mail.txt")))
            {
                sw.Write(_value);
            }   
        }

        public static void SetSniffer(string _value)
        {
            using (StreamWriter sw = new StreamWriter(GeneralPath("sniffer.txt")))
            {
                sw.Write(_value);
            }
        }
    }
}
