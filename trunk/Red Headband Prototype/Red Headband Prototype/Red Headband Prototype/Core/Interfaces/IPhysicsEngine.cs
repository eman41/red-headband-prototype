// -----------------------------------------------------------------------
// IPhysicsEngine.cs: Interface defining a physics component.
// Author: Eric S. Policaro
// -----------------------------------------------------------------------
namespace Red_Headband_Prototype.Core
{
    using TileEngine.Engine;

    public interface IPhysics
    {
        void Update(PlayerObject player, GameMap level);
    }
}
