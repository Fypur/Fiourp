using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public static class Input
    {
        public static State CurrentState { get => new State(kbState, mouseState); set { kbState = value.KbState; mouseState = value.MouseState; } }
        public static State OldState { get => new State(kbPreviousState, previousMouseState); set { kbPreviousState = value.KbState; previousMouseState = value.MouseState; } }

        private static KeyboardState kbState;
        private static KeyboardState kbPreviousState;

        private static MouseState mouseState;
        private static MouseState previousMouseState;

        public static bool GamePadConnected;
        private static GamePadState gamePadState;
        private static GamePadState previousGamePadState;

        public static Vector2 ScreenMousePos { get => mouseState.Position.ToVector2(); }
        public static Vector2 MousePos { get => Engine.Cam.ScreenToWorldPosition(Engine.Cam.ScreenToRenderTargetPosition(mouseState.Position.ToVector2())); }
        public static Vector2 MousePosNoRenderTarget { get => Input.MousePos
                * Engine.Cam.RenderTargetScreenSizeCoef; }

        public enum MouseButton { Left, Right, Middle, Macro1, Macro2 }

        public class State
        {
            public KeyboardState KbState;
            public MouseState MouseState;
            public State(KeyboardState kbState, MouseState mouseState)
            {
                KbState = kbState;
                MouseState = mouseState;
            }
        }

        public static void UpdateState() 
        {
            kbState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            gamePadState = GamePad.GetState(1);
            GamePadConnected = gamePadState.IsConnected;
        }

        public static void UpdateOldState() 
        {
            kbPreviousState = Keyboard.GetState();
            previousMouseState = Mouse.GetState();
            previousGamePadState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);
        }

        public static bool GetKeyDown(Keys key)
            => kbState.IsKeyDown(key) && !kbPreviousState.IsKeyDown(key);

        public static bool GetKey(Keys key)
            => kbState.IsKeyDown(key);

        public static bool GetKeyUp(Keys key)
            => !kbState.IsKeyDown(key) && kbPreviousState.IsKeyDown(key);

        public static bool GetMouseButtonDown(MouseButton button)
            => MouseButtonToState(mouseState, button) == ButtonState.Pressed && MouseButtonToState(previousMouseState, button) == ButtonState.Released;

        public static bool GetMouseButton(MouseButton button)
            => MouseButtonToState(mouseState, button) == ButtonState.Pressed;

        public static bool GetMouseButtonUp(MouseButton button)
            => MouseButtonToState(mouseState, button) == ButtonState.Released && MouseButtonToState(previousMouseState, button) == ButtonState.Pressed;

        public static bool GetButtonDown(Buttons button)
            => gamePadState.IsButtonDown(button) && !previousGamePadState.IsButtonDown(button);

        public static bool GetButton(Buttons button)
            => gamePadState.IsButtonDown(button);

        public static bool GetButtonUp(Buttons button)
            => !gamePadState.IsButtonDown(button) && previousGamePadState.IsButtonDown(button);

        public static Vector2 GetLeftThumbstick()
            => gamePadState.ThumbSticks.Left;

        public static Vector2 GetRightThumbstick()
            => gamePadState.ThumbSticks.Left;

        private static ButtonState MouseButtonToState(MouseState mouseState, MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.Left:
                    return mouseState.LeftButton;
                case MouseButton.Right:
                    return mouseState.RightButton;
                case MouseButton.Middle:
                    return mouseState.MiddleButton;
                case MouseButton.Macro1:
                    return mouseState.XButton1;
                case MouseButton.Macro2:
                    return mouseState.XButton2;
            }

            return mouseState.LeftButton;
        }
    }
}
