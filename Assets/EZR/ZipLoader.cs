using System.IO;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace EZR
{
    public static class ZipLoader
    {
        public static ZipFile CurrentZip;
        public static async Task<byte[]> LoadFile(string zipPath, string entryName)
        {
            if (!File.Exists(zipPath)) return null;
            using (var archive = new ZipFile(zipPath))
            {
                var index = archive.FindEntry(entryName, true);
                if (index != -1)
                {
                    var entry = archive[index];
                    if (!entry.IsFile) return null;
                    using (var stream = archive.GetInputStream(entry))
                    {
                        var buffer = new byte[entry.Size];
                        await stream.ReadAsync(buffer, 0, (int)entry.Size);
                        return buffer;
                    }
                }
                else return null;
            }
        }
        public static byte[] LoadFileSync(string zipPath, string entryName)
        {
            if (!File.Exists(zipPath)) return null;
            using (var archive = new ZipFile(zipPath))
            {
                var index = archive.FindEntry(entryName, true);
                if (index != -1)
                {
                    var entry = archive[index];
                    if (!entry.IsFile) return null;
                    using (var stream = archive.GetInputStream(entry))
                    {
                        var buffer = new byte[entry.Size];
                        stream.Read(buffer, 0, (int)entry.Size);
                        return buffer;
                    }
                }
                else return null;
            }
        }
        public static byte[] LoadFile(string entryName)
        {
            if (CurrentZip == null) return null;
            var index = CurrentZip.FindEntry(entryName, true);
            if (index != -1)
            {
                var entry = CurrentZip[index];
                if (!entry.IsFile) return null;
                using (var reader = new BinaryReader(CurrentZip.GetInputStream(entry)))
                {
                    return reader.ReadBytes((int)entry.Size);
                }
            }
            else return null;
        }
        public static bool Exists(string zipPath, string entryName)
        {
            if (!File.Exists(zipPath)) return false;
            using (var archive = new ZipFile(zipPath))
            {
                var index = archive.FindEntry(entryName, true);
                if (index != -1)
                {
                    var entry = archive[index];
                    if (!entry.IsFile) return false;
                    return true;
                }
                else return false;
            }
        }
        public static bool Exists(string entryName)
        {
            if (CurrentZip == null) return false;
            var index = CurrentZip.FindEntry(entryName, true);
            if (index != -1)
            {
                var entry = CurrentZip[index];
                if (!entry.IsFile) return false;
                return true;
            }
            else return false;
        }
        public static void OpenZip(string zipPath)
        {
            if (!File.Exists(zipPath)) return;
            if (CurrentZip != null) CurrentZip.Close();
            CurrentZip = new ZipFile(zipPath);
        }
        public static void CloseZip()
        {
            if (CurrentZip != null)
            {
                CurrentZip.Close();
                CurrentZip = null;
            }
        }
    }
}
