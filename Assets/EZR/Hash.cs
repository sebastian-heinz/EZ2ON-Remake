using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EZR
{
    public static class Hash
    {
        public static async Task<string> Sha2Async(byte[] data)
        {
            string hex = "";
            await Task.Run(() =>
            {
                var sha2 = new SHA256Managed();
                var result = sha2.ComputeHash(data);

                for (int i = 0; i < result.Length; i++)
                {
                    hex += result[i].ToString("x2");
                }
            });
            return hex;
        }
        public static string Sha2(byte[] data)
        {
            string hex = "";
            var sha2 = new SHA256Managed();
            var result = sha2.ComputeHash(data);

            for (int i = 0; i < result.Length; i++)
            {
                hex += result[i].ToString("x2");
            }
            return hex;
        }
    }
}