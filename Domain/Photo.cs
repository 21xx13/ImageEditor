namespace MyPhotoshop
{
    public class Photo
    {
        
        public readonly Pixel[,] data;
        public readonly int Width;
        public readonly int Height;

        public Photo(int width, int height)
        {
            Width = width;
            Height = height;
            data = new Pixel[width, height];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    data[i, j] = new Pixel();
        }

        public Pixel this[int x, int y]
        {
            get => data[x, y];

            set
            {
                data[x, y] = value;
            }
        }
    }
}

