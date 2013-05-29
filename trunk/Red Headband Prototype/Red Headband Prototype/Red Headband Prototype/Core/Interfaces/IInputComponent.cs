// -----------------------------------------------------------------------
// Input.cs: Interface defining player input.
// Author: Eric S. Policaro
// -----------------------------------------------------------------------
namespace Red_Headband_Prototype.Core
{
    using Microsoft.Xna.Framework;

    public interface IInput
    {
        void Update(PlayerObject player, GameTime gameTime);
    }
}
