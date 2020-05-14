using System;

namespace MyvarEdit.Rendering
{
    public class Color
    {
        public static Color Cyan { get; set; } = new Color(0f, 1f, 1f, 1f);
        public static Color Red { get; set; } = new Color(1f, 0f, 0f, 1f);
        public static Color Green { get; set; } = new Color(0f, 1f, 0f, 1f);
        public static Color Blue { get; set; } = new Color(0f, 0f, 1f, 1f);
        public static Color White { get; set; } = new Color(1f, 1f, 1f, 1f);
        public static Color Black { get; set; } = new Color(0f, 0f, 0f, 1f);

        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }

        public Color(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Color(byte r, byte g, byte b, byte a)
        {
            R = 255f / r;
            G = 255f / g;
            B = 255f / b;
            A = 255f / a;
        }

        public Color(uint c)
        {
            var bytes = BitConverter.GetBytes(c);
            R = 255f / bytes[0];
            G = 255f / bytes[1];
            B = 255f / bytes[2];
            A = 255f / bytes[3];
        }
    }
}