namespace Red_Headband_Prototype.Core
{
    using Microsoft.Xna.Framework;
    using TileEngine.Engine;

    class DemoInputComponent : IInput
    {
        public void Update(PlayerObject player, GameTime gameTime)
        {
            float xVelocity = player.Velocity.X;
            float yVelocity = player.Velocity.Y;

            if (player.Position.X < 100)
            {
                player.Position = new Vector2(101, player.Position.Y);
                xVelocity *= -1;
            }
            else if (player.Position.X > 500)
            {
                player.Position = new Vector2(499 - player.BoundingRect.Width, player.Position.Y);
                xVelocity *= -1;
            }

            player.Velocity = new Vector2(xVelocity, yVelocity);
        }

        public Direction GetXDirection()
        {
            return Direction.None;
        }

        public Direction GetYDirection()
        {
            return Direction.None;
        }
    }
}
