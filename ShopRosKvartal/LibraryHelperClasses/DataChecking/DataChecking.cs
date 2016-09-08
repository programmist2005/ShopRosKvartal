using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopRosKvartal.LibraryHelperClasses.DataChecking
{
    public static class DataChecking
    {
        public static bool Phone (string[] phoneNumber, string[] countryCode)
        {
            bool correct = true;
            for (int i = 0; i < phoneNumber.Length; i++)
            {
                // проверка номера телефона
                string item = phoneNumber[i];
                long numb;
                bool isInt = Int64.TryParse(item, out numb);
                if ((item.Length != 10) || (!isInt))
                {
                    correct = false;
                    break;
                }
                // проверка телефонного кода страны
                if (countryCode[i] != null ||
                    countryCode[i].Length > 2 && countryCode[i].Length <= 3)
                {
                    string oneCode = countryCode[i];
                    // первый символ + ?
                    if (oneCode.Substring(0, 1) != "+")
                    {
                        correct = false;
                        break;
                    }
                    // остальные символы число ?
                    int code;
                    bool isCodeInt = Int32.TryParse(oneCode.Substring(1, countryCode[i].Length-1), out code);
                    if (!isCodeInt)
                    {
                        correct = false;
                        break;
                    }
                }
                else
                {
                    correct = false;
                }
            }
            return correct;
        }

        public static bool SosialNetworkLink(string[] sosialLink)
        {
            bool correct = true;
            //упрощенная проверка url
            for (int i = 0; i < sosialLink.Length; i++)
            {
                if (sosialLink[i] != null && sosialLink[i].Length > 10)
                {
                    string url1 = sosialLink[i].Substring(0, 7);
                    string url2 = sosialLink[i].Substring(0, 8);
                    if (url1 != "http://" && url2 != "https://")
                    {
                        correct = false;
                    }
                }
                else
                {
                    correct = false;
                }
            }
            return correct;
        }
    }
}