using SFML.System;

namespace Match3TZ
{
    struct ClickGameField
    {
        public int x0, y0, x, y;
        public bool IsClick;
        public Vector2i Pos;

        public ClickGameField()
        {
            Pos = new Vector2i();
            x0 = 0; y0 = 0;
            x = 0; y = 0;
            IsClick = false;
        }

        public bool IsExist()
        {
            return MathF.Abs(x - x0) + MathF.Abs(y - y0) == 1;
        }

        public bool InBounds(int x, int y)
        {
            return x >= 1 && x <= 8 && y >= 1 && y <= 8;
        }
    }
}
