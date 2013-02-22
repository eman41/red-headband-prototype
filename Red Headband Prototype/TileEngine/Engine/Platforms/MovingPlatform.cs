// -----------------------------------------------------------------------
// <copyright file="MovingPlatform.cs" company="" />
// Author: Eric S. Policaro
// -----------------------------------------------------------------------
namespace TileEngine.Engine.Platforms
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using TileEngine.Engine;
    using TileEngine.Engine.AI;

    public class MovingPlatform : IResetable
    {
        private Rectangle _draw;
        private Texture2D _elevatorSheet;
        protected PlatformController _controller;
        private bool _sleeping = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="MovingPlatform"/> class.
        /// All platforms are initialized in a sleeping state.
        /// </summary>
        /// <param name="sheet">Texture sheet for platform objects.</param>
        /// <param name="controller">Controller for this platform</param>
        /// <param name="draw">Draw rectangle for the texture sheet (also used in bounding box calculations).</param>
        public MovingPlatform(Texture2D sheet, PlatformController controller, Rectangle draw)
        {
            _elevatorSheet = sheet;
            _draw = draw;
            _controller = controller;
        }

        public bool IsSleeping
        {
            get { return _sleeping; }
        }

        public Rectangle BoundingRect
        {
            get
            {
                return new Rectangle(
                    (int)_controller.Position.X, (int)_controller.Position.Y,
                    _draw.Width, _draw.Height);
            }
        }

        public Vector2 Velocity
        {
            get
            {
                return _controller.Velocity;
            }
        }

        public void WakeUp()
        {
            _sleeping = false;
        }

        public void Sleep()
        {
            _sleeping = true;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!_sleeping)
            {
                _controller.Update(gameTime);
            }
        }

        public void Reset()
        {
            _controller.Reset();
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(_elevatorSheet, _controller.Position, _draw, Color.White);
        }
    }
}
