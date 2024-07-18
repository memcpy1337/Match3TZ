using SFML.Graphics;
using SFML.System;

namespace Match3TZ
{
    static class GameManager
    {
        public static int SizeCell = 32;
        public static int Rows = 6;
        public static int Cols = 6;
        public static Vector2i OffsetField = new Vector2i(64, 64);
        private static GameField gameField;
        private static Hud hud;
        private static Menu menu;
        private static EndGame endGame;
        private static int gameTime = 60;
        private static GameState gameState;
        private static RenderWindow window;

        public static void Start(RenderWindow windowT)
        {
            window = windowT;
            ControlManager.Start(window);
            ChangeGameState(GameState.InMenu);
        }

        private static void ChangeGameState(GameState newState)
        {
            ControlManager.Refresh();

            switch (newState)
            {
                case GameState.InMenu:
                    ChangeWindowSize(360, 360, window);
                    menu = new Menu(() => { ChangeGameState(GameState.Playing); }, window);
                    break;
                case GameState.Playing:

                    uint width = CalculateWindowSize(Cols, SizeCell, 100);
                    uint height = CalculateWindowSize(Rows, SizeCell, 100);

                    ChangeWindowSize(width, height, window);

                    hud = new Hud();
                    gameField = new GameField(sizeCell: SizeCell, Rows: Rows, Cols: Cols, fieldOffset: OffsetField, gameWindow: window, scoreUpdate: (score) =>
                    {
                        hud.ChangeScore(score);
                    }, timerUpdate: (timer) => {
                        hud.ChangeTimer(gameTime - timer);
                        TimeCheck(timer);
                    });
                    break;
                case GameState.GameEnd:
                    ChangeWindowSize(360, 360, window);
                    endGame = new EndGame(okPress: () => { ChangeGameState(GameState.InMenu); }, window: window);
                    break;
            }

            gameState = newState;
        }

        private static uint CalculateWindowSize(int count, int sizeCell, int margin)
        {
            uint size = (uint)(count * sizeCell + margin);

            uint minSize = 240; //Чтобы худ всегда был виден
            return size < minSize ? minSize : size;
        }


        public static void ChangeWindowSize(uint width, uint height, RenderWindow gameWindow)
        {
            gameWindow.Size = new Vector2u(width, height);

            View view = new View(new FloatRect(0, 0, width, height));
            gameWindow.SetView(view);
        }

        public static void TimeCheck(float currentTime)
        {
            if (currentTime >= gameTime)
            {
                ChangeGameState(GameState.GameEnd);
            }
        }

        public static void Update() 
        {
            ControlManager.Update();

            switch (gameState)
            {
                case GameState.Playing:
                    gameField.Update();
                    break;
            }
        }

        public static void Draw(RenderTarget target)
        {
            switch (gameState)
            {
                case GameState.Playing:
                    target.Draw(gameField);
                    target.Draw(hud);
                    break;
                case GameState.InMenu:
                    target.Draw(menu);
                    break;
                case GameState.GameEnd:
                    target.Draw(endGame);
                    break;
            }
        }


    }

    public enum GameState
    {
        InMenu,
        Playing,
        GameEnd
    }
}
