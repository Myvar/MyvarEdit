namespace mEdit.Core
{
    public static class mEditEngine
    {
        public static TextDisplayBuffer DisplayBuffer { get; set; } = new TextDisplayBuffer(25, 80);

        static mEditEngine()
        {
            DisplayBuffer.DrawString("Hello World!\nMy Name is bob? i think\n++ <= => := ->");
        }
    }
}