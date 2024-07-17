using SFML.Graphics;

namespace Match3TZ
{
    static class Data
    {
        const string Textures = "Textures\\";
        const string Fonts = "Fonts\\";

        public static Texture Cell;
        public static Texture Red;
        public static Texture Blue;
        public static Texture Green;
        public static Texture Purple;
        public static Texture Orange;
        public static Texture LineHorizontal;
        public static Texture LineVertical;
        public static Texture Bomb;
        public static Texture Destroyer;
        public static Texture PlayButton;
        public static Texture OkButton;
        public static Font Font;

        public static void Load()
        {
            Cell = new Texture(Textures + "Cell.png");
            Red = new Texture(Textures + "Red.png");
            Blue = new Texture(Textures + "Blue.png");
            Green = new Texture(Textures + "Green.png");
            Purple = new Texture(Textures + "Purple.png");
            Orange = new Texture(Textures + "Orange.png");
            LineHorizontal = new Texture(Textures + "LineHorizontal.png");
            LineVertical = new Texture(Textures + "LineVertical.png");
            Destroyer = new Texture(Textures + "Destroyer.png");
            Bomb = new Texture(Textures + "Bomb.png");
            PlayButton = new Texture(Textures + "ButtonPlay.png");
            OkButton = new Texture(Textures + "ButtonOk.png");
            Font = new Font(Fonts + "Arial.ttf");
        }
    }
}
