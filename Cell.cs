using SFML.Graphics;
using SFML.System;

namespace Match3TZ
{
    public class Cell : Transformable, Drawable
    {
        public int X
        {
            get;
            private set;
        }

        public int Y
        {
            get;
            private set;
        }

        public int Hit;
        public int Col, Row;
        public Byte Alpha = 255;
        public Bonus Bonus = Bonus.Empty;
        public float TimeLastMoved = 0;
        public bool IsSelected = false;
        public bool IsAnim = false;

        public CellTypes Type 
        {
            get;
            private set;
        }


        public bool HandleAnim()
        {
            int dx = 0, dy = 0;

            for (int n = 0; n < 4; n++)
            {
                dx = X - Col * GameManager.SizeCell;
                dy = Y - Row * GameManager.SizeCell;

                if (dx != 0)
                {
                    X -= dx / Math.Abs(dx);
                }

                if (dy != 0)
                {
                    Y -= dy / Math.Abs(dy);
                }
            }

            if (dx != 0 || dy != 0)
            {
                IsAnim = true;
                return IsAnim;
            }

            IsAnim = false;
            return IsAnim;
        }


        public void SetY(int y)
        {
            Y = y;
        }

        public void ChangeCoords(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void SetPosition(int row, int col)
        {
            Row = row;
            Col = col;

            X *= GameManager.SizeCell;
            Y *= GameManager.SizeCell;
        }

        public void ChangeType(CellTypes newType)
        {
            Type = newType;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            var sprite = SpritePool.GetSpriteByType(Type);

            sprite.Color = new Color(255, 255, 255, Alpha);
            sprite.Scale = IsSelected ? new Vector2f(2, 2) : new Vector2f(1, 1);
            sprite.Position = new Vector2f(X, Y);
            sprite.Position += new Vector2f(GameManager.OffsetField.X - GameManager.SizeCell, GameManager.OffsetField.Y - GameManager.SizeCell);

            target.Draw(sprite);

            var spriteBonus = SpritePool.GetBonusSpriteByType(Bonus);
            if (spriteBonus != null)
            {
                spriteBonus.Position = new Vector2f(X, Y);
                spriteBonus.Position += new Vector2f(GameManager.OffsetField.X - GameManager.SizeCell, GameManager.OffsetField.Y - GameManager.SizeCell);
                target.Draw(spriteBonus);
            }
        }

        public void SetTimeMoved(Clock clock)
        {
            TimeLastMoved = clock.ElapsedTime.AsSeconds();
        }

        public bool Equals(Cell compareObj)
        {
            if (compareObj == null)
            {
                return false;
            }
            else
            {
                return Row == compareObj.Row && Col == compareObj.Col;
            }
        }
    }


    public enum CellTypes
    {
        Red,
        Green,
        Blue,
        Purple,
        Orange
    }
    public enum Bonus
    {
        Empty,
        LineHorizontal,
        LineVertical,
        Bomb
    }
}
