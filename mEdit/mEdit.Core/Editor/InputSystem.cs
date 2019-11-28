using OpenTK.Input;

namespace mEdit.Core.Editor
{
    public static class InputSystem
    {
        [Command("opentk_keyup")]
        public static void HandelKeyup(KeyboardKeyEventArgs e)
        {
            mEditEngine.Workspace.InvokeAllFunction("input_keyup", e);
            if (e.Key == Key.Left)
            {
                mEditEngine.Workspace.InvokeAllFunction("input_left");
            }

            if (e.Key == Key.Right)
            {
                mEditEngine.Workspace.InvokeAllFunction("input_right");
            }

            var c = KeyToChar(e);
            if (c == '\0') return;


            mEditEngine.Workspace.InvokeAllFunction("input_keypress", c);
        }

        public static void HandelKeydown(KeyboardKeyEventArgs e)
        {
        }

        public static void HandelKeypress(char c)
        {
        }

        private static char KeyToChar(KeyboardKeyEventArgs e)
        {
            var str = e.Key.ToString();

            if (str.Length == 1)
            {
                return e.Shift ? str[0] : str.ToLower()[0];
            }
            else if (str.StartsWith("Number") || str.StartsWith("Keypad") && str.Length == 7)
            {
                return str[6];
            }

            switch (e.Key)
            {
                case Key.BackSlash:
                    return '\\';
                case Key.BracketLeft:
                    return '(';
                case Key.BracketRight:
                    return ')';
                case Key.Comma:
                    return ',';
                case Key.Space:
                    return ' ';
                case Key.BackSpace:
                    return '\b';
                case Key.Quote:
                    return e.Shift ? '"' : '\'';
            }

            return '\0';
        }
    }
}