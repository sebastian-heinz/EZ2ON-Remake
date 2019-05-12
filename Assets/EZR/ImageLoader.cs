using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using B83.Image.BMP;

namespace EZR
{
    public static class ImageLoader
    {
        public static Texture2D Load(byte[] data, string fileName)
        {
            var tex = new Texture2D(2, 2);
            if (Regex.IsMatch(Path.GetExtension(fileName), @"\.bmp$", RegexOptions.IgnoreCase))
            {
                // bmp
                var bmpLoader = new BMPLoader();
                tex = bmpLoader.LoadBMP(data).ToTexture2D();
            }
            else
            {
                // png, jpg
                tex.LoadImage(data);
            }
            return tex;
        }
    }
}