// -----------------------------------------------------------------------
// KillPlatform.cs: Platform used to kill the player.
// Author: Eric S. Policaro
// -----------------------------------------------------------------------
namespace Red_Headband_Prototype.Levels.Logic
{
    using TileEngine.Engine.Platforms;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using TileEngine.Engine.AI;
    using Red_Headband_Prototype.Core.Interfaces;
    using TileEngine.Collision;

    /// <summary>
    /// Class used for a moving platform that can kill the playe.
    /// </summary>
    class KillPlatform : MovingPlatform, IKillRect
    {
        private Sensor _sensor;
        private Colliding _killDirection;

        public KillPlatform(Texture2D sheet, PlatformController controller, Rectangle draw, Sensor sensor, Colliding killDirection)
            : base(sheet, controller, draw)
        {
            _sensor = sensor;
            _killDirection = killDirection;
        }

        /// <summary>
        /// Get the direction the player must collide to be killed.
        /// </summary>
        /// <returns></returns>
        public Colliding GetKillDirection()
        {
            return _killDirection;
        }

        /// <summary>
        /// Update this platform, wakes it if its sensor fires.
        /// </summary>
        /// <param name="gameTime">Game time snapshot</param>
        public override void Update(GameTime gameTime)
        {
            if (!IsSleeping || _sensor.Detect())
            {
                this.WakeUp();
                base.Update(gameTime);
            }
        }
    }
}
