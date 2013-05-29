// -----------------------------------------------------------------------
// MovingPlatform.cs: Standard movement/travel platform
// Author: Eric S. Policaro
// -----------------------------------------------------------------------
namespace TileEngine.Engine.Platforms
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using TileEngine.Engine;
    using TileEngine.Engine.AI;

    /// <summary>
    /// Class representing a standard platformer moving platformer.
    /// All moving platforms start sleeping.
    /// </summary>
    public class MovingPlatform : IResetable
    {
        private Rectangle _draw;
        private Texture2D _texture;
        protected PlatformController _controller;
        private bool _sleeping = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="MovingPlatform"/> class.
        /// All platforms are initialized in a sleeping state.
        /// </summary>
        /// <param name="texture">Texture sheet for platform objects.</param>
        /// <param name="control">Controller for this platform</param>
        /// <param name="draw">
        /// Draw rectangle for the texture sheet (also used in bounding box calculations).
        /// </param>
        public MovingPlatform(Texture2D texture, PlatformController control, Rectangle draw)
        {
            _texture = texture;
            _draw = draw;
            _controller = control;
        }

        /// <summary>
        /// Returns true if this platform is asleep.
        /// </summary>
        public bool IsSleeping
        {
            get { return _sleeping; }
        }
        
        /// <summary>
        /// Gets the bounding rectangle for this platform.
        /// </summary>
        public Rectangle BoundingRect
        {
            get
            {
                return new Rectangle(
                    (int)_controller.Position.X, (int)_controller.Position.Y,
                    _draw.Width, _draw.Height);
            }
        }

        /// <summary>
        /// Gets the velocity of the platform.
        /// </summary>
        public Vector2 Velocity
        {
            get
            {
                return _controller.Velocity;
            }
        }

        /// <summary>
        /// Wake the platform to allow it to move.
        /// </summary>
        public void WakeUp()
        {
            _sleeping = false;
        }

        /// <summary>
        /// Put the platform to sleep, stopping its movement.
        /// </summary>
        public void Sleep()
        {
            _sleeping = true;
        }

        /// <summary>
        /// Update the position of the platform.
        /// </summary>
        /// <param name="gameTime">Game time snapshot</param>
        public virtual void Update(GameTime gameTime)
        {
            if (!_sleeping)
            {
                _controller.Update(gameTime);
            }
        }

        /// <summary>
        /// Reset the platform to its original position.
        /// </summary>
        public void Reset()
        {
            _controller.Reset();
        }
        
        /// <summary>
        /// Draw the platform on the screen.
        /// </summary>
        /// <param name="batch">Spritebatch used to draw the platform.</param>
        public void Draw(SpriteBatch batch)
        {
            batch.Draw(_texture, _controller.Position, _draw, Color.White);
        }
    }
}
