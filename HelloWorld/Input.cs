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

    static GamePadState[] currentGamepadStates = new GamePadState[4];
    static GamePadState[] previousGamepadStates = new GamePadState[4];

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
}
