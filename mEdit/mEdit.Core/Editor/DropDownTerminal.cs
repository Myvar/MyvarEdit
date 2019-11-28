using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using OpenTK;
using OpenTK.Graphics.ES10;
using OpenTK.Input;

namespace mEdit.Core.Editor
{
    public enum DropDownDeployState
    {
        Closed,
        Peek,
        Full
    }

    public class DropDownTerminal : MEditRenderable
    {
        public DropDownDeployState State { get; set; } = DropDownDeployState.Peek;

        //@Terminal move to terminal var state
        public int DropDownPeekDepth { get; set; } = 180;
        public int DropDownTextSize { get; set; } = 14;
        public float Transparency { get; set; } = 0.8f;
        public Color ReportColor { get; set; } = Color.Teal;
        public Color ReportBackgroundColor { get; set; } = Color.Black;
        public Color CommandColor { get; set; } = Color.White;
        public int IBeamOffset { get; set; }

        public string Command { get; set; } = "";
        public StringBuilder ReportText { get; set; } = new StringBuilder();

        [CommandInit]
        public static void Init()
        {
            Console.WriteLine("Terminal Init");
        }

        public DropDownTerminal()
        {
            mEditEngine.Workspace.RegisterFunction("input_keypress", HandelKeypress);

            mEditEngine.Workspace.RegisterFunction("input_keyup", HandelKeyup);
            mEditEngine.Workspace.RegisterFunction("input_right", (x) =>
            {
                if (IBeamOffset < Command.Length)
                    IBeamOffset++;
                return null;
            });

            mEditEngine.Workspace.RegisterFunction("input_left", (x) =>
            {
                if (IBeamOffset > 0)
                    IBeamOffset--;
                return null;
            });

            mEditEngine.Workspace.RegisterFunction("print", (x) =>
            {
                ReportText.Append(x[0].ToString());
                return null;
            });

            mEditEngine.Workspace.RegisterFunction("println", (x) =>
            {
                ReportText.Append(x[0].ToString());
                return null;
            });
        }

        private object[] HandelKeyup(object[] arg)
        {
            var e = arg[0] as KeyboardKeyEventArgs;
            if (e.Key == Key.Enter)
            {
                mEditEngine.Workspace.Eval(Command);
                Command = "";
                IBeamOffset = 0;
            }

            if (e.Key == Key.Tilde)
            {
                State = e.Shift ? DropDownDeployState.Full : DropDownDeployState.Peek;
                CanRender = !CanRender;
            }

            return null;
        }


        private object[] HandelKeypress(object[] arg)
        {
            var c = (char) arg[0];
            if (c == '\b')
            {
                if (IBeamOffset > 0)
                {
                    Command = Command.Remove(IBeamOffset - 1, 1);
                    IBeamOffset--;
                }

                return null;
            }

            IBeamOffset++;
            Command = Command.Insert(IBeamOffset - 1, c.ToString());
            return null;
        }


        public override void Render()
        {
            var depth = 0;
            if (State == DropDownDeployState.Peek)
            {
                depth = DropDownPeekDepth;
            }

            if (State == DropDownDeployState.Full)
            {
                depth = OpenTKHost.Window.Height - DropDownTextSize - 4;
            }

            OpenTKHost.FillRectangle(0, 0, OpenTKHost.Window.Width, depth,
                Color.FromArgb((int) (255 * Transparency), ReportBackgroundColor));
            OpenTKHost.FillRectangle(0, depth, OpenTKHost.Window.Width, (4) + DropDownTextSize,
                Color.FromArgb((int) (255 * Transparency), ReportBackgroundColor));
            var rows = (int) (depth / (DropDownTextSize * 1.2f));
            var rs = ReportText.ToString();
            var rl = rs.Replace("\r\n", "\n").Split('\n');

            var rt = new StringBuilder();
            if (rows >= rl.Length)
            {
                rows = rl.Length;
            }

            var lines = rl[^rows..];
            foreach (var s in lines)
            {
                rt.AppendLine(s);
            }

            OpenTKHost.DrawString(Fonts.FiraCode, rt.ToString(), 2, 2,
                DropDownTextSize,
                ReportColor);
            OpenTKHost.DrawString(Fonts.FiraCode, Command, 2, depth + (2), DropDownTextSize,
                CommandColor);

//@HardCoded Color of ibeam is hard coded
            if (!string.IsNullOrEmpty(Command))
                OpenTKHost.RenderIBeam(
                    OpenTKHost.MesureTextWidth(Fonts.FiraCode, Command.Substring(0, IBeamOffset), DropDownTextSize)
/*- (OpenTKHost.MesureTextWidth(Fonts.FiraCode, Command[IBeamOffset - 1].ToString(),
       DropDownTextSize) / 2)*/ + 2,
                    depth + (2) + DropDownTextSize, DropDownTextSize,
                    Color.Turquoise);
        }
    }
}