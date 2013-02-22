namespace TileEngine.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public class InputUtility
    {
        public static bool ButtonToggled(GamePadState lastState, GamePadState currentState, Buttons button)
        {
            return (currentState.IsButtonDown(button) && lastState.IsButtonUp(button));
        } 
    }
}
