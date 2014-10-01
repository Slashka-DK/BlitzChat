using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Threading;

namespace BlitzChat
{
    public static class UrlTools
    {
        private static readonly Regex UrlRegex = new Regex(@"(?#Protocol)(?:(?:ht|f)tp(?:s?)\:\/\/|~/|/)?(?#Username:Password)(?:\w+:\w+@)?(?#Subdomains)(?:(?:[-\w]+\.)+(?#TopLevel Domains)(?:com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|travel|[a-z]{2}))(?#Port)(?::[\d]{1,5})?(?#Directories)(?:(?:(?:/(?:[-\w~!$+|.,=]|%[a-f\d]{2})+)+|/)+|\?|#)?(?#Query)(?:(?:\?(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)(?:&amp;(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)*)*(?#Anchor)(?:#(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)?");

        public static bool IsHyperlink(string word)
        {
           // First check to make sure the word has at least one of the characters we need to make a hyperlink
           if (word.IndexOfAny(@".\/".ToCharArray()) != -1)
           {
               if (UrlRegex.IsMatch(word))
                   return true;
           }
           return false;
        }
        public static string DetectURLs(TextRange tr)
        {
            string paragraphText = tr.Text;
            Regex aRegex = new Regex("(<a.*href=\".*\".*(</a>|/>))");
            if (aRegex.IsMatch(paragraphText)) {
                MatchCollection mCol = aRegex.Matches(paragraphText);
                return mCol[0].Groups[1].Value;
            }
            // Split the paragraph by words
            foreach (string word in paragraphText.Split(' ').ToList())
            {
                if (IsHyperlink(word))
                {
                    
                    return word;
                }
                
            }
            return "";
        }
        public static void Hyperlink_Click(object sender, EventArgs e)
        {
            Process.Start((sender as Hyperlink).NavigateUri.AbsoluteUri);
        }

    }
}
