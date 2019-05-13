using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EZR
{
    public static class Hash
    {
        public static async Task<string> Sha1(byte[] data)
        {
            string hex = "";
            await Task.Run(() =>
            {
                var sha1 = new SHA1CryptoServiceProvider();
                var result = sha1.ComputeHash(data);

                for (int i = 0; i < result.Length; i++)
                {
                    hex += result[i].ToString("x2");
                }
            });
            return hex;
        }
        public static string Sha1Sync(byte[] data)
        {
            string hex = "";
            var sha1 = new SHA1CryptoServiceProvider();
            var result = sha1.ComputeHash(data);

            for (int i = 0; i < result.Length; i++)
            {
                hex += result[i].ToString("x2");
            }
            return hex;
        }
    }
}