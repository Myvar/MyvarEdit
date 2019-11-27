namespace mEdit.Core
{
    public class TextDisplayBuffer
    {
        public char[] Lfb { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public TextDisplayBuffer(int width, int height)
        {
            Width = width;
            Height = height;
            Lfb = new char[width * height];
        }

        public char this[int x, int y]
        {
            get { return Lfb[y * Width + x]; }
            set { Lfb[y * Width + x] = value; }
        }


        public void DrawString(string s)
        {
            int x = 0;
            int y = 0;

            foreach (var c in s)
            {
                this[x, y] = c;

                x++;

                if (x >= Width)
                {
                    x = 0;
                    y++;
                }
            }
        }
    }
}