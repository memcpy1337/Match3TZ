using SFML.Graphics;
using SFML.System;
using SFML.Window;

public static class ControlManager
{
    public static event EventHandler<MouseClickEventArgs> MouseClick;

    private static bool wasButtonPressedLastFrame;
    private static RenderWindow gameWindow;

    public static void Start(RenderWindow renderWindow)
    {
        gameWindow = renderWindow;
    }

    public static void Refresh() => MouseClick = null;

    public static void Update()
    {
        bool isButtonPressed = Mouse.IsButtonPressed(Mouse.Button.Left);
        Vector2i mousePosition = Mouse.GetPosition(gameWindow);

        if (mousePosition.X >= 0 && mousePosition.X < gameWindow.Size.X &&
            mousePosition.Y >= 0 && mousePosition.Y < gameWindow.Size.Y)
        {
            if (isButtonPressed && !wasButtonPressedLastFrame)
            {
                MouseClick?.Invoke(null, new MouseClickEventArgs(mousePosition));
            }
        }

        wasButtonPressedLastFrame = isButtonPressed;
    }
}

public class MouseClickEventArgs : EventArgs
{
    public Vector2i MousePosition { get; }

    public MouseClickEventArgs(Vector2i mousePosition)
    {
        MousePosition = mousePosition;
    }
}