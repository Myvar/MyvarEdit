namespace mEdit.Core
{
    public static class Fonts
    {
        public static DynamicFont FiraCode { get; set; } 
        public static DynamicFont FiraCodeBold { get; set; }

        static Fonts()
        {
            FiraCode = new DynamicFont(Utils.GetResourceFileStream("FiraCode-Retina.ttf"));
            FiraCodeBold = new DynamicFont(Utils.GetResourceFileStream("FiraCode-Bold.ttf"));
        }
    }
}