using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyvarEdit.Rendering;
using MyvarEdit.Utils;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Common.Input;

namespace MyvarEdit.Editor
{
    public class TextBuffer
    {
        //for now lets just try a array of lines
        public List<string> Lines { get; set; } = new List<string>() {""};
        public int Line { get; set; }
        public int Col { get; set; }
        public int FontSize = 14;

        private string Cache = "";
        private List<string> cachedList = new List<string>();
        private int CompleteOffset = 0;

        public void DrawToPane(Pane p)
        {
            var x = p.Origin.X;
            var y = p.Origin.Y;


            for (var i = 0; i < Lines.Count; i++)
            {
                var line = Lines[i];
                DrawBuffer.DrawString(Color.White, line, FontSize, x, y);
                y += FontSize;
            }

            x = p.Origin.X;
            y = p.Origin.Y;


            for (var i = 0; i < Lines.Count; i++)
            {
                var line = Lines[i];

                y += FontSize;

                if (i == Line)
                {
                    var size = DrawBuffer.MesureString(Col != line.Length ? line.Remove(Col) : line, FontSize);
                    if (DateTime.Now.Millisecond >= 500)
                        DrawBuffer.DrawRect(Color.White, x + size.Width, y - FontSize + 2, 2, FontSize);

                    //@HACK @TMP just to test
                    var words = line[..Col].Split(' ');

                    var word = words.Last().ToLower().Trim();


                    if (Cache != word)
                    {
                        cachedList = FindClosest(word, 5);
                        Cache = word;

                        Console.Clear();

                        foreach (var z in cachedList)
                        {
                            Console.WriteLine(z);
                        }
                    }

                    if (cachedList.Count != 0
                        //&& word != cachedList[0]
                    )
                    {
                        var widest = 0f;
                        DrawBuffer.DrawRect(new Color(0, 0, 0, 0.8f), x + size.Width, y + 2, 250, 100);
                        var yoff = 0;
                        for (var index = 0; index < cachedList.Count; index++)
                        {
                            var lst = cachedList[index];
                            DrawBuffer.DrawString(index == CompleteOffset ? new Color(0, 0.5f, 0.5f, 1f) : Color.Cyan,
                                lst, 10, x + size.Width, y + 2 + yoff);
                            yoff += 10;
                            var s = DrawBuffer.MesureString(lst, 10);
                            if (widest < s.Width) widest = s.Width;
                        }

                        var def = MyvarEditEngine.WebstersEnglishDictionary[cachedList[CompleteOffset]].Split(' ');
                        var longest = def.Max(x => x.Length);
                        var spacing = longest * 10;
                        var maxWidthDef = 500;
                        var wordsPerline = maxWidthDef / spacing;

                        var sb = new StringBuilder();

                        for (int j = 0; j < def.Length; j++)
                        {
                            sb.Append(" ");
                            sb.Append(def[j]);
                            if (j % wordsPerline == 0) sb.AppendLine();
                        }

                        DrawBuffer.DrawString(new Color(0, 0.5f, 0.5f, 1f),
                            sb.ToString(), 10,
                            x + size.Width + widest + 5, y + 2);
                    }
                }
            }
        }

        private List<string> FindClosest(string s, int count)
        {
            var sorted = new List<(int, string)>();

            foreach (var (key, value) in MyvarEditEngine.WebstersEnglishDictionary)
            {
                if (key.Length != 1)
                    sorted.Add((LevenshteinDistance.Compute(s, key), key));
            }

            var re = new List<string>();

            sorted.Sort((x, y) => x.Item1.CompareTo(y.Item1));

            foreach (var tuple in sorted.ToArray()[0..count])
            {
                re.Add(tuple.Item2);
            }

            return re;
        }

        public void SendKey(KeyboardKeyEventArgs e)
        {
            var key = e.Key;
            if (key == Key.Tab && !e.Shift)
            {
                CompleteOffset++;
                if (CompleteOffset >= 5) CompleteOffset = 0;
            }

            if (key == Key.Space && e.Shift)
            {
                for (var i = 0; i < Lines.Count; i++)
                {
                    var line = Lines[i];


                    if (i == Line)
                    {
                        //@HACK @TMP just to test
                        var words = line[..Col].Split(' ');
                        var word = words.Last().ToLower().Trim();


                        if (cachedList.Count != 0
                            //&& word != cachedList[0]
                        )
                        {
                            if (line[Col - 1] != ' ')
                            {
                                Lines[i] = line.Remove(Col - word.Length, word.Length);
                                Lines[i] = Lines[i].Insert(Col - word.Length, cachedList[CompleteOffset]);
                                Col += cachedList[CompleteOffset].Length - word.Length;
                                cachedList.Clear();
                                CompleteOffset = 0;
                            }
                        }
                    }
                }
            }

            if (key == Key.Left)
            {
                if (Col == 0)
                {
                    if (Line != 0)
                    {
                        Col = Lines[Line - 1].Length;
                        Line--;
                    }
                }
                else
                {
                    Col--;
                }
            }

            if (key == Key.Right)
            {
                if (Col == Lines[Line].Length)
                {
                    if (Line != Lines.Count - 1)
                    {
                        Line++;
                        Col = 0;
                    }
                }
                else
                {
                    Col++;
                }
            }

            if (key == Key.Up)
            {
                if (Line != 0)
                {
                    if (Col >= Lines[Line - 1].Length)
                    {
                        Col = Lines[Line - 1].Length;
                    }

                    Line--;
                }
            }

            if (key == Key.Down)
            {
                if (Line >= Lines.Count - 1)
                {
                    if (Lines.Count != 0) Col = Lines[Line].Length;
                    // Line++;
                }
                else
                {
                    if (Col >= Lines[Line + 1].Length)
                    {
                        Col = Lines[Line + 1].Length;
                    }

                    Line++;
                }
            }
        }

        public void KeyUp(char c)
        {
            if (c == '\t')
            {
                return;
            }

            if (Lines.Count <= Line) Lines.Add("");

            if (c == '\n')
            {
                Line++;
                Col = 0;
                Lines.Add("");
            }
            else if (c == '\b')
            {
                if (Lines[Line].Length == 0)
                {
                    Col = Lines[Line - 1].Length;


                    Lines.RemoveAt(Line);
                    Line--;
                }
                else
                {
                    Lines[Line] = Lines[Line].Remove(Lines[Line].Length - 1);
                    Col--;
                }
            }
            else
            {
                Col++;
                Lines[Line] += c;
            }
        }
    }
}