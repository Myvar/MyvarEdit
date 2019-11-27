using OpenTK;

namespace mEdit.Core.OpenGL
{
    public class UserSettings
    {
        public int WindowWidth { get; set; } = 800;
        public int WindowHeight { get; set; } = 600;

        public VSyncMode VSync { get; set; } = VSyncMode.Off;

        public int MaxFrameRate { get; set; } = 400;
        public float Fov { get; set; } = 70f;

        public float MouseSensitivity { get; set; } = 20;
    }
}