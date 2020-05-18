using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MyvarEdit.Editor;
using MyvarEdit.Rendering;
using Newtonsoft.Json;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Common.Input;

namespace MyvarEdit
{
    public static class MyvarEditEngine
    {
        public static Dictionary<string, string> WebstersEnglishDictionary { get; set; }
        public static List<string> Affixes { get; set; } = new List<string>();
        public static Input Input { get; set; } = new Input();

        static MyvarEditEngine()
        {
            Input.KeyPress = KeyPress;
            Input.SendKey = SendKey;
            var sw = new Stopwatch();
            sw.Start();
            WebstersEnglishDictionary =
                JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("dictionary_compact.json"));
            sw.Stop();
            Console.WriteLine($"Loading Dict took {sw.ElapsedMilliseconds}ms.");
            Console.WriteLine(WebstersEnglishDictionary.Count);
            
            Affixes.AddRange(File.ReadAllLines("afixes"));
        }

        private static void SendKey(KeyboardKeyEventArgs e)
        {
            Buffer.SendKey(e);
        }

        private static void KeyPress(char c)
        {
            Buffer.KeyUp(c);
        }

        public static Pane Window { get; set; } = new Pane();

        public static TextBuffer Buffer { get; set; } = new TextBuffer();

        public static void Draw()
        {
            Buffer.DrawToPane(Window);
        }
    }
}