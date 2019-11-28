namespace mEdit.Core.Editor
{
    public abstract class MEditRenderable
    {
        public bool CanRender { get; set; }
        public abstract void Render();
    }
}