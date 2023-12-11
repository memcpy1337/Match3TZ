using SFML.Graphics;
using SFML.System;

namespace Match3TZ
{
    internal class Hud : Transformable, Drawable
    {
        Text score;
        Text timer;

        public Hud()
        {
            score = new Text("Score: 0", Data.Font, 24);
            timer = new Text("Timer: 60", Data.Font, 24);
        }



        public void ChangeTimer(float newSec)
        {
            timer.DisplayedString = $"Timer: {(int)newSec}";
        }

        public void ChangeScore(int newScore)
        {
            score.DisplayedString = $"Score: {newScore}";
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(score);
            timer.Position = new Vector2f(128, 0);
            target.Draw(timer);
        }
    }
}
