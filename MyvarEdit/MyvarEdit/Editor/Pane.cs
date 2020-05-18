using System;
using MyvarEdit.Rendering;

namespace MyvarEdit.Editor
{
    public class Pane
    {
        public static PaneDock Dock { get; set; }

        public Point Origin => Dock switch
        {
            PaneDock.Fullscreen => new Point(0, 0),
            PaneDock.Left => new Point(0, 0),
            PaneDock.Right => new Point(DrawBuffer.ScreenSize.Width / 2f, 0),
        };

        public Size Size => Dock switch
        {
            PaneDock.Fullscreen => DrawBuffer.ScreenSize,
            PaneDock.Left => new Size(DrawBuffer.ScreenSize.Width / 2f, DrawBuffer.ScreenSize.Height),
            PaneDock.Right => new Size(DrawBuffer.ScreenSize.Width / 2f, DrawBuffer.ScreenSize.Height),
        };
    }
}