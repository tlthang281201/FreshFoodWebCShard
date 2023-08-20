using System.Text;
using System.Text.RegularExpressions;

namespace ShoeShop.Helper
{
    public static class Utinities
    {
        public static bool IsValidEmail(string email)
        {
            if(email.Trim().EndsWith("."))
            {
                return false;
            }
            try
            {
                var address = new System.Net.Mail.MailAddress(email);
                return address.Address == email;
            } catch
            {
                return false;
            }
        }
        public static void CreateIfMissing(string path)
        {
            bool folderExist = Directory.Exists(path);
            if (!folderExist)
            {
                Directory.CreateDirectory(path);
            }
        }
        public static string GetRandomKey(int length = 5)
        {
            string pattern = @"abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random rd = new Random();
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < length; i++)
            {
                sb.Append(pattern[rd.Next(0,pattern.Length)]);
            }
            return sb.ToString();
        }
        public static string SEOUrl(string url)
        {
            url = url.ToLower();
            url = Regex.Replace(url, @"[aáàạảãâấậẫầẩăắằẵẳặ]", "a");
            url = Regex.Replace(url, @"[eéèẹẻẽêếềệểễ]", "e");
            url = Regex.Replace(url, @"[oóòọỏõôốồộổỗơớờợỡở]", "o");
            url = Regex.Replace(url, @"[uúùụủũưứừựửữ]", "u");
            url = Regex.Replace(url, @"[iíìịỉĩ]", "i");
            url = Regex.Replace(url, @"[yýỳỵỷỹ]", "y");
            url = Regex.Replace(url, @"[đ]", "d");

            url = Regex.Replace(url.Trim(), @"[^0-9a-z-\s]", "").Trim();
            url = Regex.Replace(url.Trim(), @"\s+", "-");

            url = Regex.Replace(url, @"\s", "-");
            while(true)
            {
                if(url.IndexOf("--") != -1)
                {
                    url = url.Replace("--", "-");
                }
                else
                {
                    break;
                }
            }
            return url;
        }
        public static async Task<String> UploadFile(IFormFile file, string sDirectory, string newname)
        {
            try
            {
                if (newname == null) newname = file.FileName;
                string path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot","images", sDirectory);
                CreateIfMissing(path);
                string pathFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", sDirectory, newname);
                var supportedTypes = new[] { "jpg", "jpeg", "png", "gif" };
                var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                if(!supportedTypes.Contains(fileExt.ToLower())) {
                    return null;
                } else
                {
                    using (var stream = new FileStream(pathFile,FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    return newname;
                }
            } catch(Exception ex)
            {
                return null;
            }
        }
    }
}
