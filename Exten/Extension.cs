using System.Text.RegularExpressions;

namespace ShoeShop.Exten
{
    public static class Extension
    {
        public static string ToVND(this double gia) { 
            return gia.ToString("#,##0")+" đ";
        }
        public static string ToTitleCase(string str)
        {
            string result = str;
            if(!string.IsNullOrEmpty(str))
            {
                var words = str.Split(' ');
                for(int i = 0; i < words.Length; i++)
                {
                    var s = words[i];
                    if(s.Length > 0)
                    {
                        words[i] = s[0].ToString().ToUpper() + s.Substring(1);
                    }
                }
                result = string.Join(" ", words);
            }
            return result;
        }
    }
}
