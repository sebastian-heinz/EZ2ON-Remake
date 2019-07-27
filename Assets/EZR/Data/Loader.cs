using System.IO;
using System.Threading.Tasks;

namespace EZR
{
    public static class DataLoader
    {
        static EZRData ezrData = null;

        public static async Task<byte[]> LoadFileAsync(string ezrPath, string entryName)
        {
            if (!File.Exists(ezrPath)) return null;
            var ezrData = new EZRData(ezrPath);
            ezrData.OpenStream();
            var buffer = await ezrData.LoadFileAsync(entryName);
            ezrData.CloseStream();
            return buffer;
        }
        public static byte[] LoadFile(string ezrPath, string entryName)
        {
            if (!File.Exists(ezrPath)) return null;
            var ezrData = new EZRData(ezrPath);
            ezrData.OpenStream();
            var buffer = ezrData.LoadFile(entryName);
            ezrData.CloseStream();
            return buffer;
        }
        public static byte[] LoadFile(string entryName)
        {
            if (ezrData == null) return null;
            return ezrData.LoadFile(entryName);
        }
        public static bool Exists(string ezrPath, string entryName)
        {
            if (!File.Exists(ezrPath)) return false;
            var ezrData = new EZRData(ezrPath);
            ezrData.OpenStream();
            var result = ezrData.Exists(entryName);
            ezrData.CloseStream();
            return result;
        }
        public static bool Exists(string entryName)
        {
            if (ezrData == null) return false;
            return ezrData.Exists(entryName);
        }
        public static void OpenStream(string ezrPath)
        {
            ezrData = new EZRData(ezrPath);
            ezrData.OpenStream();
        }
        public static void CloseStream()
        {
            if (ezrData != null) ezrData.CloseStream();
            ezrData = null;
        }
        public static string[] GetNames(string ezrPath, string dirName = "")
        {
            if (!File.Exists(ezrPath)) return new string[0];
            var ezrData = new EZRData(ezrPath);
            return ezrData.GetNames(dirName);
        }

        public static string GetEZRDataPath(GameType type, string songName)
        {
            if (type == GameType.EZ2ON || type == GameType.EZ2DJ)
                return Path.Combine(EZR.Master.GameResourcesFolder, "EZ2Series", type.ToString(), songName + ".ezr");
            else
                return Path.Combine(EZR.Master.GameResourcesFolder, type.ToString(), "Songs", songName + ".ezr");
        }
    }
}
