using Proviser.Models;
using Proviser.Services;
using System;

namespace Proviser.Servise
{
    static class TextToCourtsConverter
    {
        public static Courts Transform(string _text)
        {
            Courts courts = new Courts();
            courts.Date = ExeptionDate(_text, 0);
            courts.Judge = Exeption(_text, 1);
            courts.Case = Exeption(_text, 2);
            courts.Court = Exeption(_text, 3);
            //4
            courts.Littigans = Exeption(_text, 5);
            courts.Category = Exeption(_text, 6);
            
            courts.Status = "net";

            courts.SoketHeader = "";
            courts.SoketNote = "";
            courts.SoketPrisonDate = "";
            return courts;
        }

        private static string Exeption(string _text, int _index)
        {
            try
            {
                string[] _array = _text.Split('\t');
                return _array[_index].Replace("\"", "");
            }
            catch
            {
                return "";
            }
        }

        private static DateTime ExeptionDate(string _text, int _index)
        {
            try
            {
                string[] _array = _text.Split('\t');
                return Convert.ToDateTime(_array[_index].Replace("\"", ""));
            }
            catch
            {
                return Convert.ToDateTime("01.01.2000 00:00:00");
            }
        }
    }
}
