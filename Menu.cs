using SFML.Graphics;
using SFML.System;

namespace Match3TZ
{
    internal class Menu : Drawable
    {
        Sprite playButton;
        Action playPress;

        private RenderWindow window;

        public Menu(Action playPress, RenderWindow window)
        {
            playButton = new Sprite(texture: Data.PlayButton);
            playButton.Origin = new Vector2f(40, 16);
            playButton.Scale = new Vector2f(4, 4);
            playButton.Position = new Vector2f(180, 180);
            this.playPress = playPress;
            this.window = window;
            ControlManager.MouseClick += MouseClick;
        }

        private void MouseClick(object? sender, MouseClickEventArgs e)
        {
            if (playButton.GetGlobalBounds().Contains(window.MapPixelToCoords(e.MousePosition).X, window.MapPixelToCoords(e.MousePosition).Y))
            {
                playPress?.Invoke();
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(playButton);
        }

    }
}
