using System;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Common.Input;

namespace MyvarEdit.Editor
{
    public class Input
    {
        public Action<char> KeyPress { get; set; } = c => { };
        public Action<KeyboardKeyEventArgs> SendKey { get; set; } = c => { };

        public void Handel(KeyboardKeyEventArgs e)
        {
            SendKey(e);
            switch (e.Key)
            {
                case Key.Unknown:
                    break;
                case Key.ShiftLeft:
                    break;
                case Key.ShiftRight:
                    break;
                case Key.ControlLeft:
                    break;
                case Key.ControlRight:
                    break;
                case Key.AltLeft:
                    break;
                case Key.AltRight:
                    break;
                case Key.WinLeft:
                    break;
                case Key.WinRight:
                    break;
                case Key.Menu:
                    break;
                case Key.Command:
                    break;
                case Key.F1:
                    break;
                case Key.F2:
                    break;
                case Key.F3:
                    break;
                case Key.F4:
                    break;
                case Key.F5:
                    break;
                case Key.F6:
                    break;
                case Key.F7:
                    break;
                case Key.F8:
                    break;
                case Key.F9:
                    break;
                case Key.F10:
                    break;
                case Key.F11:
                    break;
                case Key.F12:
                    break;
                case Key.F13:
                    break;
                case Key.F14:
                    break;
                case Key.F15:
                    break;
                case Key.F16:
                    break;
                case Key.F17:
                    break;
                case Key.F18:
                    break;
                case Key.F19:
                    break;
                case Key.F20:
                    break;
                case Key.F21:
                    break;
                case Key.F22:
                    break;
                case Key.F23:
                    break;
                case Key.F24:
                    break;
                case Key.F25:
                    break;
                case Key.F26:
                    break;
                case Key.F27:
                    break;
                case Key.F28:
                    break;
                case Key.F29:
                    break;
                case Key.F30:
                    break;
                case Key.F31:
                    break;
                case Key.F32:
                    break;
                case Key.F33:
                    break;
                case Key.F34:
                    break;
                case Key.F35:
                    break;
                case Key.Up:
                    break;
                case Key.Down:
                    break;
                case Key.Left:
                    break;
                case Key.Right:
                    break;
                case Key.Enter:
                    KeyPress('\n');
                    break;
                case Key.Escape:
                    break;
                case Key.Space:
                    KeyPress(' ');
                    break;
                case Key.Tab:
                    KeyPress('\t');
                    break;
                case Key.BackSpace:
                    KeyPress('\b');
                    break;
                case Key.Insert:
                    break;
                case Key.Delete:
                    break;
                case Key.PageUp:
                    break;
                case Key.PageDown:
                    break;
                case Key.Home:
                    break;
                case Key.End:
                    break;
                case Key.CapsLock:
                    break;
                case Key.ScrollLock:
                    break;
                case Key.PrintScreen:
                    break;
                case Key.Pause:
                    break;
                case Key.NumLock:
                    break;
                case Key.Clear:
                    break;
                case Key.Sleep:
                    break;
                case Key.Keypad0:
                    KeyPress('0');
                    break;
                case Key.Keypad1:
                    KeyPress('1');
                    break;
                case Key.Keypad2:
                    KeyPress('2');
                    break;
                case Key.Keypad3:
                    KeyPress('3');
                    break;
                case Key.Keypad4:
                    KeyPress('4');
                    break;
                case Key.Keypad5:
                    KeyPress('5');
                    break;
                case Key.Keypad6:
                    KeyPress('6');
                    break;
                case Key.Keypad7:
                    KeyPress('7');
                    break;
                case Key.Keypad8:
                    KeyPress('8');
                    break;
                case Key.Keypad9:
                    KeyPress('9');
                    break;
                case Key.KeypadDivide:
                    KeyPress('/');
                    break;
                case Key.KeypadMultiply:
                    KeyPress('*');
                    break;
                case Key.KeypadSubtract:
                    KeyPress('-');
                    break;
                case Key.KeypadAdd:
                    KeyPress('+');
                    break;
                case Key.KeypadEqual:
                    KeyPress('=');
                    break;
                case Key.KeypadEnter:
                    KeyPress('\n');
                    break;
                case Key.A:
                    if (e.Shift) KeyPress('A');
                    else KeyPress('a');
                    break;
                case Key.B:
                    if (e.Shift) KeyPress('B');
                    else KeyPress('b');
                    break;
                case Key.C:
                    if (e.Shift) KeyPress('C');
                    else KeyPress('c');
                    break;
                case Key.D:
                    if (e.Shift) KeyPress('D');
                    else KeyPress('d');
                    break;
                case Key.E:
                    if (e.Shift) KeyPress('E');
                    else KeyPress('e');
                    break;
                case Key.F:
                    if (e.Shift) KeyPress('F');
                    else KeyPress('f');
                    break;
                case Key.G:
                    if (e.Shift) KeyPress('G');
                    else KeyPress('g');
                    break;
                case Key.H:
                    if (e.Shift) KeyPress('H');
                    else KeyPress('h');
                    break;
                case Key.I:
                    if (e.Shift) KeyPress('I');
                    else KeyPress('i');
                    break;
                case Key.J:
                    if (e.Shift) KeyPress('J');
                    else KeyPress('j');
                    break;
                case Key.K:
                    if (e.Shift) KeyPress('K');
                    else KeyPress('k');
                    break;
                case Key.L:
                    if (e.Shift) KeyPress('L');
                    else KeyPress('l');
                    break;
                case Key.M:
                    if (e.Shift) KeyPress('M');
                    else KeyPress('m');
                    break;
                case Key.N:
                    if (e.Shift) KeyPress('N');
                    else KeyPress('n');
                    break;
                case Key.O:
                    if (e.Shift) KeyPress('O');
                    else KeyPress('o');
                    break;
                case Key.P:
                    if (e.Shift) KeyPress('P');
                    else KeyPress('p');
                    break;
                case Key.Q:
                    if (e.Shift) KeyPress('Q');
                    else KeyPress('q');
                    break;
                case Key.R:
                    if (e.Shift) KeyPress('R');
                    else KeyPress('r');
                    break;
                case Key.S:
                    if (e.Shift) KeyPress('S');
                    else KeyPress('s');
                    break;
                case Key.T:
                    if (e.Shift) KeyPress('T');
                    else KeyPress('t');
                    break;
                case Key.U:
                    if (e.Shift) KeyPress('U');
                    else KeyPress('u');
                    break;
                case Key.V:
                    if (e.Shift) KeyPress('V');
                    else KeyPress('v');
                    break;
                case Key.W:
                    if (e.Shift) KeyPress('W');
                    else KeyPress('w');
                    break;
                case Key.X:
                    if (e.Shift) KeyPress('X');
                    else KeyPress('x');
                    break;
                case Key.Y:
                    if (e.Shift) KeyPress('Y');
                    else KeyPress('y');
                    break;
                case Key.Z:
                    if (e.Shift) KeyPress('Z');
                    else KeyPress('z');
                    break;
                case Key.Number0:
                    KeyPress('0');
                    break;
                case Key.Number1:
                    KeyPress('1');
                    break;
                case Key.Number2:
                    KeyPress('2');
                    break;
                case Key.Number3:
                    KeyPress('3');
                    break;
                case Key.Number4:
                    KeyPress('4');
                    break;
                case Key.Number5:
                    KeyPress('5');
                    break;
                case Key.Number6:
                    KeyPress('6');
                    break;
                case Key.Number7:
                    KeyPress('7');
                    break;
                case Key.Number8:
                    KeyPress('8');
                    break;
                case Key.Number9:
                    KeyPress('9');
                    break;
                case Key.Tilde:
                    KeyPress('~');
                    break;
                case Key.Minus:
                    KeyPress('-');
                    break;
                case Key.Plus:
                    KeyPress('+');
                    break;
                case Key.BracketLeft:
                    KeyPress('(');
                    break;
                case Key.BracketRight:
                    KeyPress(')');
                    break;
                case Key.Semicolon:
                    KeyPress(';');
                    break;
                case Key.Quote:
                    KeyPress('"');
                    break;
                case Key.Comma:
                    KeyPress(',');
                    break;
                case Key.Period:
                    KeyPress('.');
                    break;
                case Key.Slash:
                    KeyPress('/');
                    break;
                case Key.BackSlash:
                    KeyPress('\\');
                    break;
                case Key.NonUSBackSlash:
                    break;
            }
        }
    }
}