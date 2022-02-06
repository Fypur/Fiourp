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

        public static void UpdateState()
            => state = Keyboard.GetState();

        public static void UpdateOldState()
            => previousState = Keyboard.GetState();

        public static bool GetKeyDown(Keys key)
            => state.IsKeyDown(key) && !previousState.IsKeyDown(key);

        public static bool GetKey(Keys key)
            => state.IsKeyDown(key);

        public static bool GetKeyUp(Keys key)
            => !state.IsKeyDown(key) && previousState.IsKeyDown(key);
    }
}
