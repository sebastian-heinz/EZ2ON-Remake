using System.IO;
using System.Threading.Tasks;
using GameOldBoy.Data;

namespace EZR
{
    public static class DataLoader
    {
        static Package Package = null;

        public static async Task<byte[]> LoadFileAsync(string gpkPath, string entryName)
        {
            if (!File.Exists(gpkPath)) return null;
            var Package = new Package(gpkPath);
            Package.OpenStream();
            var buffer = await Package.LoadFileAsync(entryName);
            Package.CloseStream();
            return buffer;
        }
        public static byte[] LoadFile(string gpkPath, string entryName)
        {
            if (!File.Exists(gpkPath)) return null;
            var Package = new Package(gpkPath);
            Package.OpenStream();
            var buffer = Package.LoadFile(entryName);
            Package.CloseStream();
            return buffer;
        }
        public static byte[] LoadFile(string entryName)
        {
            if (Package == null) return null;
            return Package.LoadFile(entryName);
        }
        public static bool Exists(string gpkPath, string entryName)
        {
            if (!File.Exists(gpkPath)) return false;
            var Package = new Package(gpkPath);
            Package.OpenStream();
            var result = Package.Exists(entryName);
            Package.CloseStream();
            return result;
        }
        public static bool Exists(string entryName)
        {
            if (Package == null) return false;
            return Package.Exists(entryName);
        }
        public static void OpenStream(string gpkPath)
        {
            Package = new Package(gpkPath);
            Package.OpenStream();
        }
        public static void CloseStream()
        {
            if (Package != null) Package.CloseStream();
            Package = null;
        }
        public static string[] GetNames(string gpkPath, string dirName = "")
        {
            if (!File.Exists(gpkPath)) return new string[0];
            var Package = new Package(gpkPath);
            return Package.GetNames(dirName);
        }

        public static string GetPackagePath(GameType type, string songName)
        {
            if (type == GameType.EZ2ON || type == GameType.EZ2DJ)
                return Path.Combine(EZR.Master.GameResourcesFolder, "EZ2Series", type.ToString(), songName + ".gpk");
            else
                return Path.Combine(EZR.Master.GameResourcesFolder, type.ToString(), "Songs", songName + ".gpk");
        }
    }
}
