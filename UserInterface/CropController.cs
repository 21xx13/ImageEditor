using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyPhotoshop
{
    public static class CropController
    {
        private static int crpX, crpY, rectW, rectH;
        private static readonly Pen crpPen = new Pen(Color.White);
        private static PictureBox imageArea;

        public static void SetImage(PictureBox image) {
            imageArea = image;
        }

        public static void CropMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                imageArea.Refresh();
                rectW = e.X - crpX;
                rectH = e.Y - crpY;
                Graphics g = imageArea.CreateGraphics();
                g.DrawRectangle(crpPen, crpX, crpY, rectW, rectH);
                g.Dispose();
            }
        }

        public static void CropMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                crpPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                crpX = e.X;
                crpY = e.Y;
            }
        }

        public static Bitmap CropProcess()
        {
            Bitmap bmp2 = new Bitmap(imageArea.Width, imageArea.Height);
            imageArea.DrawToBitmap(bmp2, imageArea.ClientRectangle);
            if (rectW + crpX > imageArea.Width)
                rectW = imageArea.Width - crpX;
            if (rectH + crpY > imageArea.Height)
                rectH = imageArea.Height - crpY;
            Bitmap crpImg = new Bitmap(rectW, rectH);

            for (int i = 0; i < rectW; i++)
            {
                for (int y = 0; y < rectH; y++)
                {
                    Color pxlclr = bmp2.GetPixel(crpX + i, crpY + y);
                    crpImg.SetPixel(i, y, pxlclr);
                }
            }
            return crpImg;
        }
    }
}
