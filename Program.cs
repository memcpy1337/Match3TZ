using SFML.Graphics;
using SFML.Window;

namespace Match3TZ
{
    public class Program
    {
        static void Main(string[] args)
        {
            Data.Load();

            RenderWindow gameWindow = new RenderWindow(new VideoMode(360, 360), "Матч 3 ТЗ");
            gameWindow.SetFramerateLimit(60);
            gameWindow.Closed += Window_Closed;

            GameManager.Start(gameWindow);

            while (gameWindow.IsOpen)
            {
                gameWindow.DispatchEvents();

                GameManager.Update();

                gameWindow.Clear(Color.Black);
                GameManager.Draw(gameWindow);

                gameWindow.Display();
            }

        }

        private static void Window_Closed(object? sender, EventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }
    }
}