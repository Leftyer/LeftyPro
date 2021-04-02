using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Leftyer.Domains.Utilitys
{
    /*
* date:2020/08/22
* author:Leftyer  
* description:Good code doesn't need comments.
*/
    public class SecurityCore
    {
        SecurityCore() { }
        public static SecurityCore Instance => new();
        public string MD5(string input) => MD5(input, Encoding.Default);
        public string MD5(string input, Encoding encoding)
        {
            var data = new MD5CryptoServiceProvider().ComputeHash(encoding.GetBytes(input))?.ToList();
            var sb = new StringBuilder();
            data.ForEach(m => sb.Append(m.ToString("x2")));
            return sb?.ToString();
        }
    }
}
