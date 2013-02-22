namespace Red_Headband_Prototype.Levels.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using TileEngine.Engine.Platforms;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using TileEngine.Engine.AI;
    using Red_Headband_Prototype.Core.Interfaces;
    using TileEngine.Collision;

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

        public Colliding GetKillDirection()
        {
            return _killDirection;
        }

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
