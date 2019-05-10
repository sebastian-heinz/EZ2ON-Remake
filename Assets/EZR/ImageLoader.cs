using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using B83.Image.BMP;

namespace EZR
{
    public static class ImageLoader
    {
        public static Texture2D Load(string path)
        {
            var tex = new Texture2D(2, 2);
            if (File.Exists(path))
            {
                if (Regex.IsMatch(Path.GetExtension(path), @"\.bmp$", RegexOptions.IgnoreCase))
                {
                    // bmp
                    var bmpLoader = new BMPLoader();
                    tex = bmpLoader.LoadBMP(File.ReadAllBytes(path)).ToTexture2D();
                }
                else
                {
                    // png, jpg
                    tex.LoadImage(File.ReadAllBytes(path));
                }
            }
            else
            {
                tex.LoadImage(new byte[0]);
            }
            return tex;
        }
    }
}