using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using MyvarEdit.Rendering;
using MyvarEdit.TrueType;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Desktop;
using OpenToolkit.Windowing.GraphicsLibraryFramework;

namespace MyvarEdit
{
    class Program
    {
        static void Main(string[] args)
        {
            
          
            var window = new EditorWindow(new GameWindowSettings()
            {
                RenderFrequency = 120,
                UpdateFrequency = 120
            }, new NativeWindowSettings()
            {
                Title = "MyvarEdit",
                APIVersion = new Version(4, 6),
                //Size = new Vector2i(1000,1000)
            });
            window.Run();
        }
    }
}