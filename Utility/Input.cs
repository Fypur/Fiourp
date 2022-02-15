using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public static class Input
    {
        private static KeyboardState state;
        private static KeyboardState previousState;

        private static MouseState mouseState;
        private static MouseState previousMouseState;
        public static Vector2 MousePos { get => Engine.Cam.ScreenToWorldPosition(mouseState.Position.ToVector2()); }
        public static Vector2 MousePosNoRenderTarget { get => Engine.Cam.ScreenToWorldPosition(mouseState.Position.ToVector2())
                * (Engine.ScreenSize.X / Engine.RenderTarget.Width); }

        public enum MouseButton { Left, Right, Middle, Macro1, Macro2 }

        public static void UpdateState() 
        {
            state = Keyboard.GetState();
            mouseState = Mouse.GetState();
        }

        public static void UpdateOldState() 
        {
            previousState = Keyboard.GetState();
            previousMouseState = Mouse.GetState();
        }

        public static bool GetKeyDown(Keys key)
            => state.IsKeyDown(key) && !previousState.IsKeyDown(key);

        public static bool GetKey(Keys key)
            => state.IsKeyDown(key);

        public static bool GetKeyUp(Keys key)
            => !state.IsKeyDown(key) && previousState.IsKeyDown(key);

        public static bool GetMouseButtonDown(MouseButton button)
            => MouseButtonToState(mouseState, button) == ButtonState.Pressed && MouseButtonToState(previousMouseState, button) == ButtonState.Released;

        public static bool GetMouseButton(MouseButton button)
            => MouseButtonToState(mouseState, button) == ButtonState.Pressed;

        public static bool GetMouseButtonUp(MouseButton button)
            => MouseButtonToState(mouseState, button) == ButtonState.Released && MouseButtonToState(previousMouseState, button) == ButtonState.Pressed;

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
