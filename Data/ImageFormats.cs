using System.Collections.Generic;
using System.Linq;
using System.Drawing.Imaging;

namespace MyPhotoshop
{
    public static class ImageFormats
    {
        public readonly static List<ImageFormat> Formats = new List<ImageFormat>() {
            ImageFormat.Jpeg, ImageFormat.Png, ImageFormat.Gif };

        public static string SaveFilter { get => GetFilter(); }
        public static string LoadFilter { get => GetFilter() + AggregateFilter() + "|All files (*.*)|*.*"; }

        private static string GetFilter()
        {
            return string.Join("|", Formats.Select(e => string.Format("{0} Image |*.{1}",
                e.ToString().ToUpper(), e.ToString().ToLower())));
        }

        private static string AggregateFilter()
        {
            return string.Format("|Image files({0})|{1}",
                    string.Join(",", Formats.Select(e => string.Format("*.{0}", e.ToString().ToLower()))),
                    string.Join(";", Formats.Select(e => string.Format("*.{0}", e.ToString().ToLower()))));
        }
    }
}
