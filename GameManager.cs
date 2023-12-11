using SFML.Graphics;
using SFML.System;

namespace Match3TZ
{
    static class GameManager
    {
        public static int SizeCell = 32;
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
                    menu = new Menu(() => { ChangeGameState(GameState.Playing); }, window);
                    break;
                case GameState.Playing:
                    hud = new Hud();
                    gameField = new GameField(sizeCell: SizeCell, fieldOffset: OffsetField, gameWindow: window, scoreUpdate: (score) =>
                    {
                        hud.ChangeScore(score);
                    }, timerUpdate: (timer) => {
                        hud.ChangeTimer(gameTime - timer);
                        TimeCheck(timer);
                    });
                    break;
                case GameState.GameEnd:
                    endGame = new EndGame(okPress: () => { ChangeGameState(GameState.InMenu); }, window: window);
                    break;
            }

            gameState = newState;
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
