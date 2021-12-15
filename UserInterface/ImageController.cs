using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MyPhotoshop
{
    public static class ImageController
    {
        public static void SaveImage(Image image)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = ImageFormats.SaveFilter,
                Title = "Сохранить изображение как"
            };
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                FileStream fs = (FileStream)saveFileDialog.OpenFile();

                for (int i = 1; i <= ImageFormats.Formats.Count; i++)
                {
                    if (saveFileDialog.FilterIndex == i)
                    {
                        image.Save(fs, ImageFormats.Formats[i - 1]);
                        break;
                    }
                }
                fs.Close();
            }
        }

        public static void LoadImage(Action<Bitmap> load)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = ImageFormats.LoadFilter,
                Title = "Выберите изображение"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                load((Bitmap)Image.FromFile(openFileDialog.FileName));
        }
    }
}
