using SFML.Graphics;
using SFML.System;

namespace Match3TZ
{
    internal class EndGame : Drawable
    {
        Sprite okButton;
        Action okPress;
        Text gameOver;

        private RenderWindow window;

        public EndGame(Action okPress, RenderWindow window)
        {
            gameOver = new Text("Game Over", Data.Font,54);
            gameOver.Position = new Vector2f(40, 0);
            okButton = new Sprite(texture: Data.OkButton);
            okButton.Origin = new Vector2f(40, 16);
            okButton.Scale = new Vector2f(2, 2);
            okButton.Position = new Vector2f(180, 180);
            this.okPress = okPress;
            this.window = window;
            ControlManager.MouseClick += MouseClick;
        }

        private void MouseClick(object? sender, MouseClickEventArgs e)
        {
            if (okButton.GetGlobalBounds().Contains(window.MapPixelToCoords(e.MousePosition).X, window.MapPixelToCoords(e.MousePosition).Y))
            {
                okPress?.Invoke();
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(gameOver);
            target.Draw(okButton);
        }
    }
}
