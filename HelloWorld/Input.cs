using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HelloWorld;

public static class Input
{
    static KeyboardState currentKeyboardState;
    static KeyboardState previousKeyboardState;

    public static KeyboardState GetKeyboardState()
    {
        previousKeyboardState = currentKeyboardState;
        currentKeyboardState = Keyboard.GetState();

        return currentKeyboardState;
    }

    static MouseState currentMouseState;
    static MouseState previousMouseState;

    public static MouseState GetMouseState(GameWindow window = null)
    {
        previousMouseState = currentMouseState;
        currentMouseState = Mouse.GetState(window == null ? Main.MainWindow : window);

        return currentMouseState;
    }

    static readonly GamePadState[] currentGamepadStates = new GamePadState[4];
    static readonly GamePadState[] previousGamepadStates = new GamePadState[4];

    public static GamePadState GetGamePadState() => GetGamePadState(PlayerIndex.One);

    public static GamePadState GetGamePadState(PlayerIndex index)
    {
        for(var i = 0; i < 4; i++)
        {
            previousGamepadStates[i] = currentGamepadStates[i];
            currentGamepadStates[i] = GamePad.GetState((PlayerIndex)i);
        }

        return currentGamepadStates[(int)index];
    }

    public static JoystickState GetJoystickState() => GetJoystickState(0);

    public static JoystickState GetJoystickState(int index)
    {
        return Joystick.GetState(index);
    }

    public static bool Get(Keys key)
    {
        return currentKeyboardState.IsKeyDown(key);
    }

    public static bool GetPressed(Keys key)
    {
        return currentKeyboardState.IsKeyDown(key) && !previousKeyboardState.IsKeyDown(key);
    }

    public static bool GetReleased(Keys key)
    {
        return !currentKeyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyDown(key);
    }

    public static bool Get(Buttons button, PlayerIndex index)
    {
        return currentGamepadStates[(int)index].IsButtonDown(button);
    }

    public static bool GetPressed(Buttons button, PlayerIndex index)
    {
        return currentGamepadStates[(int)index].IsButtonDown(button) && !previousGamepadStates[(int)index].IsButtonDown(button);
    }

    public static bool GetReleased(Buttons button, PlayerIndex index)
    {
        return !currentGamepadStates[(int)index].IsButtonDown(button) && previousGamepadStates[(int)index].IsButtonDown(button);
    }

    public static bool Get(Buttons button) => Get(button, PlayerIndex.One);

    public static bool GetPressed(Buttons button) => GetPressed(button, PlayerIndex.One);

    public static bool GetReleased(Buttons button) => GetReleased(button, PlayerIndex.One);

    public static bool Get(MouseButtons button)
    {
        return GetMouseButtonState(currentMouseState, button) == ButtonState.Pressed;
    }

    public static bool GetPressed(MouseButtons button)
    {
        return GetMouseButtonState(currentMouseState, button) == ButtonState.Pressed && GetMouseButtonState(previousMouseState, button) == ButtonState.Released;
    }

    public static bool GetReleased(MouseButtons button)
    {
        return GetMouseButtonState(currentMouseState, button) == ButtonState.Released && GetMouseButtonState(previousMouseState, button) == ButtonState.Pressed;
    }

    static ButtonState GetMouseButtonState(MouseState state, MouseButtons button)
    {
        return button switch
        {
            MouseButtons.LeftButton => state.LeftButton,
            MouseButtons.RightButton => state.RightButton,
            MouseButtons.MiddleButton => state.MiddleButton,
            MouseButtons.XButton1 => state.XButton1,
            MouseButtons.XButton2 => state.XButton2,
            _ => ButtonState.Released,
        };
    }

    public static ButtonState GetMouseButtonState(MouseButtons button) => GetMouseButtonState(currentMouseState, button);

    public static int GetScrollDirection()
    {
        return System.Math.Sign(currentMouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue);
    }
}

public enum MouseButtons
{
    LeftButton,
    RightButton,
    MiddleButton,
    XButton1,
    XButton2
}
