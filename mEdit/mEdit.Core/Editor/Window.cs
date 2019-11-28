using System.Drawing;

namespace mEdit.Core.Editor
{
    public class Window : MEditRenderable
    {
        public override void Render()
        {
            OpenTKHost.DrawString(Fonts.FiraCode, "Hello World\nHello World\nHello World\nHello World\nHello World\nHello World\nHello World\nHello World\nHello World\nHello World\nHello World\nHello World\nHello World\nHello World\nHello World\nHello World\nHello World\nHello World\nHello World\nHello World\nHello World\nHello World\n"
                , 0, 0, 18, Color.White);
        }
    }
}