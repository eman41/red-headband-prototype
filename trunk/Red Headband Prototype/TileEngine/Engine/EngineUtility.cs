// -----------------------------------------------------------------------
// EngineUtility.cs: Provides generic utility methods to support XNA
// Author: Eric S. Policaro
// -----------------------------------------------------------------------
namespace TileEngine.Engine
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// Utility class containing engine support funtions for Input
    /// </summary>
    public sealed class InputUtility
    {
        /// <summary>
        /// Check if a button has been toggled between frames.
        /// </summary>
        /// <param name="last">Game pad from the last frame</param>
        /// <param name="current">Game pad of the current frame</param>
        /// <param name="button">Button to check</param>
        /// <returns>True if toggled, False otherwise</returns>
        public static bool ButtonToggled(GamePadState last, 
                                         GamePadState current,
                                         Buttons button)
        {
            return (current.IsButtonDown(button) && last.IsButtonUp(button));
        } 
    }
}
