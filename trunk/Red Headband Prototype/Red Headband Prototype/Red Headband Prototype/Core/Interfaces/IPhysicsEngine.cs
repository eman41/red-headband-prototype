// -----------------------------------------------------------------------
// <copyright file="IPhysicsEngine.cs" company="Me">
// Author: Eric S. Policaro
// </copyright>
// -----------------------------------------------------------------------

namespace Red_Headband_Prototype.Core
{
    using TileEngine.Engine;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IPhysics
    {
        void Update(PlayerObject player, GameMap level);
    }
}
