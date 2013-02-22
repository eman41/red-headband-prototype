// -----------------------------------------------------------------------
// <copyright file="EngineUtility.cs" company="Me" />
// Author: Eric S. Policaro
// -----------------------------------------------------------------------
namespace TileEngine.Engine
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public class InputUtility
    {
        public static bool ButtonToggled(GamePadState last, 
                                         GamePadState current,
                                         Buttons button)
        {
            return (current.IsButtonDown(button) && last.IsButtonUp(button));
        } 
    }
}
