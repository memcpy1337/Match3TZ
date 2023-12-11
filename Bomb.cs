using SFML.Graphics;
using SFML.System;

namespace Match3TZ
{
    internal class Bomb : Transformable
    {
        private Action<Bomb> completeTask;
        private Clock clock = new Clock();
        private float updateInterval = 0.25f;
        public int Col, Row;

        public Bomb(int Row, int Col, Action<Bomb> completeTask)
        {
            this.Col = Col;
            this.Row = Row;
            this.completeTask = completeTask;
        }

        public void Update()
        {
            if (clock.ElapsedTime.AsSeconds() < updateInterval)
            {
                return;
            }

            completeTask?.Invoke(this);
        }
    }
}
