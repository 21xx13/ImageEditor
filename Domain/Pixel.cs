using System;

namespace MyPhotoshop
{
    public struct Pixel
    {
        private double r;
        private double g;
        private double b;

        public Pixel(double r, double g, double b)
        {
            this.r = this.g = this.b = 0;
            this.r = r;
            this.b = b;
            this.g = g;
        }

        public static double Trim(double value)
        {
            if (value < 0)
                return 0;
            if (value > 255)
                return 255;
            return value;
        }
        public static Pixel Trim(Pixel pixel)
        {
            pixel.R = Trim(pixel.R);
            pixel.G = Trim(pixel.G);
            pixel.B = Trim(pixel.B);
            return pixel;
        }
        private double Check(double value)
        {
            if (value < 0 && value > 255) throw new ArgumentException("Значение не может быть меньше 0 или больше 255");
            return value;
        }
        public double R
        {
            get { return r; }
            set
            {
                r = Check(value);
            }
        }
        public double G
        {
            get { return g; }
            set
            {
                g = Check(value);
            }
        }
        public double B
        {
            get { return b; }
            set
            {
                b = Check(value);
            }
        }

        public static Pixel operator *(Pixel p, double numb)
        {
            return new Pixel(p.R * numb, p.G * numb, p.B * numb);
        }

        public static Pixel operator *(double numb, Pixel p)
        {
            return p * numb;
        }

        public static Pixel operator +(Pixel p, double numb)
        {
            return new Pixel(p.R + numb, p.G + numb, p.B + numb);
        }

        public static Pixel operator -(Pixel p, double numb)
        {
            return new Pixel(p.R - numb, p.G - numb, p.B - numb);
        }

        public static Pixel operator -(double numb, Pixel p)
        {
            return new Pixel(numb - p.R, numb - p.G, numb - p.B);
        }

        public static Pixel operator /(Pixel p, double numb)
        {
            return new Pixel(p.R / numb, p.G / numb, p.B / numb);
        }

        public static Pixel operator +(double numb, Pixel p)
        {
            return p + numb;
        }

    }
}
