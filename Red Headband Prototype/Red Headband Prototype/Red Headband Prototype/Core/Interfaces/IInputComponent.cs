namespace Red_Headband_Prototype.Core
{
    using Microsoft.Xna.Framework;

    public interface IInput
    {
        void Update(PlayerObject player, GameTime gameTime);
    }
}
