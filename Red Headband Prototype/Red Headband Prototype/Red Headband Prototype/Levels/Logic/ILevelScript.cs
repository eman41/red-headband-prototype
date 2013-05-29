// -----------------------------------------------------------------------
// ILevelScript.cs: Interface providing a command for level scripting
// Author: Eric S. Policaro
// -----------------------------------------------------------------------
namespace Red_Headband_Prototype.Levels.Logic
{
    using TileEngine.Engine;
    using Red_Headband_Prototype.Core;

    /// <summary>Interface providing a command for level scripting.</summary>
    public interface ILevelScript
    {
        /// <summary>
        /// Gets the map being scripted.
        /// </summary>
        /// <returns>GameMap</returns>
        GameMap GetGameMap();

        /// <summary>
        /// Used to script a level's platforms, camera, and sensors.
        /// </summary>
        /// <param name="obj">Player object for the map</param>
        /// <param name="camera">Camera being used</param>
        void Scripting(PlayerObject obj, ref Camera2D camera);
    }
}
