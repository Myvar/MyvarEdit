using System.Collections.Generic;
using System.Drawing;
using mEdit.Core.Editor;

namespace mEdit.Core
{
    public static class mEditEngine
    {
        public static Workspace Workspace { get; set; } = new Workspace();

        public static List<MEditRenderable> Renderables { get; set; } = new List<MEditRenderable>
        {
            new Window
            {
                CanRender = true
            },
            new DropDownTerminal()
        };

        static mEditEngine()
        {
        }

        public static void Render()
        {
            foreach (var mEditRenderable in Renderables)
            {
                if (mEditRenderable.CanRender)
                {
                    mEditRenderable.Render();
                }
            }
        }
    }
}