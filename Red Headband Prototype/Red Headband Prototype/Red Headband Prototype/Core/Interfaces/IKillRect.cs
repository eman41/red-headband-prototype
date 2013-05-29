// -----------------------------------------------------------------------
// IKillRect.cs: Interface for entities that can kill players.
// Author: Eric S. Policaro
// -----------------------------------------------------------------------
namespace Red_Headband_Prototype.Core.Interfaces
{
    using TileEngine.Collision;

    interface IKillRect
    {
        Colliding GetKillDirection();
    }
}
