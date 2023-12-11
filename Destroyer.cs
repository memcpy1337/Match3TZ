using SFML.Graphics;
using SFML.System;

namespace Match3TZ
{
    internal class Destroyer : Transformable, Drawable
    {
        private int X, Y, Col, Row;
        private int ColTarget, RowTarget;

        private Clock clock = new Clock();
        private float updateInterval = 0.25f;

        private Action<(int x, int y)> destroyCell;
        private Action<Destroyer> completeTask;

        public Destroyer(int Col, int Row, int RowTarget, int ColTarget, Action<(int x, int y)> destroyCell, Action<Destroyer> completeTask)
        {
            this.Col = Col;
            this.Row = Row;
            this.RowTarget = RowTarget;
            this.ColTarget = ColTarget;
            this.destroyCell = destroyCell;
            this.completeTask = completeTask;

            this.X = Row * GameManager.SizeCell;
            this.Y = Col * GameManager.SizeCell;
        }


        public void Draw(RenderTarget target, RenderStates states)
        {
            var spriteDestroyer = SpritePool.GetSpriteDestroyer();
            spriteDestroyer.Position = new Vector2f(X, Y);
            spriteDestroyer.Position += new Vector2f(GameManager.OffsetField.X - GameManager.SizeCell, GameManager.OffsetField.Y - GameManager.SizeCell);
            target.Draw(spriteDestroyer);
        }

        public void Update()
        {
            if (clock.ElapsedTime.AsSeconds() < updateInterval)
            {
                return;
            }

            clock.Restart();

            if (Col != ColTarget || Row != RowTarget)
            {
                if (Col != ColTarget)
                {
                    if (Col > ColTarget)
                    {
                        Col--;
                    }
                    else if (Col < ColTarget)
                    {
                        Col++;

                    }
                    this.Y = Col * GameManager.SizeCell;

                }

                if (Row != RowTarget)
                {
                    if (Row > RowTarget)
                    {
                        Row--;
                    }
                    else if (Row < RowTarget)
                    {
                        Row++;
                    }
                    this.X = Row * GameManager.SizeCell;
                }
                destroyCell?.Invoke((Row, Col));
            }
            else
            {
                completeTask?.Invoke(this);
            }
        }
    }
}
