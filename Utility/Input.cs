using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Fiourp
{
    public enum MouseButton { Left, Right, Middle, Macro1, Macro2 }
    public static class Input
    {
        public static State CurrentState { get => new State(kbState, mouseState, gamePadState); set { kbState = value.KbState; mouseState = value.MouseState; gamePadState = value.GamePadState; } }
        public static State OldState { get => new State(kbPreviousState, previousMouseState, previousGamePadState); set { kbPreviousState = value.KbState; previousMouseState = value.MouseState; previousGamePadState = value.GamePadState; } }

        private static KeyboardState kbState;
        private static KeyboardState kbPreviousState;

        private static MouseState mouseState;
        private static MouseState previousMouseState;

        private static GamePadState gamePadState;
        private static GamePadState previousGamePadState;
        public static bool GamePadConnected => gamePadState.IsConnected;

        public static Vector2 ScreenMousePos { get => mouseState.Position.ToVector2(); }
        public static Vector2 MousePos { get => Engine.Cam.ScreenToWorldPosition(Engine.Cam.ScreenToRenderTargetPosition(mouseState.Position.ToVector2())); }
        public static Vector2 MousePosNoRenderTarget { get => MousePos * Engine.Cam.RenderTargetScreenSizeCoef; }

        public static ControlList LeftControls = new ControlList(Keys.Left, Keys.A, Keys.Q, Buttons.LeftThumbstickLeft);
        public static ControlList RightControls = new ControlList(Keys.Right, Keys.D, Buttons.LeftThumbstickRight);
        public static ControlList UpControls = new ControlList(Keys.Up, Keys.Z, Buttons.LeftThumbstickUp);
        public static ControlList DownControls = new ControlList(Keys.Down, Keys.S, Buttons.LeftThumbstickDown);

        public static ControlList UIAction1 = new ControlList(Keys.Enter, Buttons.A, Keys.C, Keys.Space, Keys.I);
        public static ControlList UIActionBack = new ControlList(Keys.Escape, Buttons.B, Keys.X, Keys.O);
        public static ControlList ButtonClear = new ControlList(Keys.V, Buttons.Y);

        public struct State
        {
            public KeyboardState KbState;
            public MouseState MouseState;
            public GamePadState GamePadState;
            public State(KeyboardState kbState, MouseState mouseState, GamePadState gamePadState)
            {
                KbState = kbState;
                MouseState = mouseState;
                GamePadState = gamePadState;
            }
        }

        public static void UpdateState()
        {
            kbState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            gamePadState = GamePad.GetState(0);
        }

        public static void UpdateOldState()
        {
            kbPreviousState = kbState;
            previousMouseState = mouseState;
            previousGamePadState = gamePadState;
        }

        public static List<Control> GetPressedControls()
        {
            List<Control> controls = new();

            if(kbState != kbPreviousState)
            {
                foreach (Keys key in kbState.GetPressedKeys())
                {
                    if (GetKeyDown(key))
                        controls.Add(new Control(key));
                }
            }

            if (mouseState != previousMouseState)
            {
                foreach (MouseButton button in Enum.GetValues<MouseButton>())
                {
                    if (GetMouseButtonDown(button))
                        controls.Add(new Control(button));
                }
            }

            if(gamePadState.PacketNumber != previousGamePadState.PacketNumber)
            {
                foreach (Buttons button in Enum.GetValues<Buttons>())
                {
                    if (GetButtonDown(button))
                        controls.Add(new Control(button));
                }
            }

            return controls;
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

        public static bool GetControlDown(Control control)
            => control.IsDown();

        public static bool GetControl(Control control)
            => control.Is();

        public static bool GetControlUp(Control control)
            => control.IsUp();

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

    public struct Control
    {
        public Keys? Key { get; set; }
        public MouseButton? MouseButton { get; set; }
        public Buttons? ControllerButton { get; set; }
        public object Value 
        {
            get
            {
                if (Key != null) return Key;
                else if (MouseButton != null)
                    return MouseButton;
                else return ControllerButton;
            }
            set
            {
                if (value is Keys key)
                { Key = key; MouseButton = null; ControllerButton = null; }
                else if (value is MouseButton button)
                { MouseButton = button; ControllerButton = null; Key = null; }
                else if(value is Buttons cbutton)
                { ControllerButton = cbutton; Key = null; MouseButton = null; }
            }
        }

        public Control() { Key = null; MouseButton = null; ControllerButton = null; }
        public Control(Keys key) : this() { Key = key; }
        public Control(MouseButton mouseButton) : this() { MouseButton = mouseButton; }
        public Control(Buttons controllerButton) : this() { ControllerButton = controllerButton; }

        public bool IsDown()
        {
            if (Key is Keys k)
                return Input.GetKeyDown(k);
            if (MouseButton is MouseButton m)
                return Input.GetMouseButtonDown(m);
            if (ControllerButton is Buttons b)
                return Input.GetButtonDown(b);
            return false;
        }

        public bool Is()
        {
            if (Key is Keys k)
                return Input.GetKey(k);
            if (MouseButton is MouseButton m)
                return Input.GetMouseButton(m);
            if (ControllerButton is Buttons b)
                return Input.GetButton(b);
            return false;
        }

        public bool IsUp()
        {
            if (Key is Keys k)
                return Input.GetKeyUp(k);
            if (MouseButton is MouseButton m)
                return Input.GetMouseButtonUp(m);
            if (ControllerButton is Buttons b)
                return Input.GetButtonUp(b);
            return false;
        }

        public override string ToString()
        {
            if (Key != null)
                return Key.ToString();
            if (MouseButton != null)
                return "Mouse " + MouseButton.ToString();
            if(ControllerButton != null)
                return "Controller " + ControllerButton.ToString();
#if DEBUG
            throw new Exception("Control To String not outputing anything: Key, MouseButton and ControllerButton are null");
#endif
#if RELEASE
            return "";
#endif
        }
    }

    public class ControlList : IEnumerable<Control>
    {
        public List<Control> Controls;

        public ControlList() { Controls = new List<Control>(); }
        public ControlList(params Control[] controls) { Controls = new List<Control>(controls); }

        public ControlList(params object[] controls)
        {
            Controls = new();
            foreach (object control in controls)
            {
                if (control is Keys k)
                    Controls.Add(new Control(k));
                else if(control is MouseButton m)
                    Controls.Add(new Control(m));
                else if (control is Buttons b)
                    Controls.Add(new Control(b));
#if DEBUG
                else
                    throw new Exception("Object given as control is not a valid control");
#endif
            }
        }

        public ControlList(ControlList copyfrom)
            => Controls = new List<Control>(copyfrom.Controls);

        public int Count => Controls.Count;

        public void Clear()
            => Controls.Clear();

        public bool IsDown()
        {
            bool returned = false;
            foreach (Control c in Controls)
            {
                if (c.IsUp())
                    return false;

                if (c.Is())
                {
                    if (c.IsDown())
                    {
                        returned = true;
                        continue;
                    }
                    else
                        return false;
                }
            }

            return returned;
        }

        public bool Is()
        {
            foreach (Control c in Controls)
                if (c.Is())
                    return true;
            return false;
        }

        public bool IsUp()
        {
            bool returned = false;
            foreach (Control c in Controls)
            {
                if(c.Is())
                    return false;
                if (c.IsUp())
                    returned = true;
            }

            return returned;
        }

        public void Add(Control control)
            => Controls.Add(control);

        public void Remove(Control control)
            => Controls.Add(control);

        public string GetAllControlNames()
            => GetAllControlNames(", ");

        public string GetAllControlNames(string inBetween)
        {
            string names = "";
            foreach (Control c in Controls)
                names += c.ToString() + inBetween;
            if (names.Length > 2)
                names = names[0..(names.Length - inBetween.Length)];
            return names;
        }

        public IEnumerator GetEnumerator()
            => Controls.GetEnumerator();

        IEnumerator<Control> IEnumerable<Control>.GetEnumerator()
            => Controls.GetEnumerator();

        public ControlList Copy()
            => new ControlList(this);
    }
}
