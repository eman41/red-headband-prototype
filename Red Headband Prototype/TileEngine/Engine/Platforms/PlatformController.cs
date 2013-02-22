// -----------------------------------------------------------------------
// <copyright file="PlatformController.cs" company="" />
// Author: Eric S. Policaro
// Handles the movement, speed, and location of a platform
// -----------------------------------------------------------------------
namespace TileEngine.Engine.Platforms
{
    using System;
    using Microsoft.Xna.Framework;
    using TileEngine.Engine;

    public class PlatformController : IResetable
    {
        private Vector2 _start = Vector2.Zero;
        private Vector2 _stop = Vector2.Zero;

        private bool _beingHeld = false;
        private bool _moveOneTime = false; 
        private bool _holdingUntilReset = false;

        private Vector2 _heldVelocity = Vector2.Zero;
        private TimeSpan _holdElapsed = TimeSpan.Zero;
        private TimeSpan _maxHoldDuration = TimeSpan.Zero;

        /// <summary>
        /// Initializes a new platform controller. 
        /// Position Vectors should be given in level tile coordinates.
        /// </summary>
        public PlatformController(Vector2 start, Vector2 stop, float speed, 
            TimeSpan holdTime, bool oneTime = false)
        {
            _maxHoldDuration = holdTime;
            _moveOneTime = oneTime;

            _start = translateToPixelCoords(start);
            _stop = translateToPixelCoords(stop);

            Position = _start;
            Velocity = getStartingVelocity(start, stop, speed);
        }

        private Vector2 translateToPixelCoords(Vector2 levelCoords)
        {
            float x = levelCoords.X * GameMap.TILE_SIZE;
            float y = levelCoords.Y * GameMap.TILE_SIZE;
            return new Vector2(x, y);
        }

        private Vector2 getStartingVelocity(Vector2 start, Vector2 stop, float speed)
        {
            Vector2 moveNormal = new Vector2(start.X - stop.X, start.Y - stop.Y);
            moveNormal.Normalize();
            return new Vector2(moveNormal.X * speed, moveNormal.Y * speed);
        }

        /// <summary>
        /// X, Y pixel coordinates of this platform
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Current platform speed
        /// </summary>
        public Vector2 Velocity { get; private set; }

        /// <summary>
        /// Reset this entity to it's starting state
        /// </summary>
        public void Reset()
        {
            _beingHeld = false;
            _holdingUntilReset = false;
            Position = _start;
            _holdElapsed = TimeSpan.Zero;
        }

        /// <summary>
        /// Update the state of the platform.
        /// </summary>
        /// <param name="gameTime">Current Game time</param>
        public virtual void Update(GameTime gameTime)
        {
            if (_beingHeld)
            {
                if (holdDurationExceeded(gameTime))
                {
                    releaseAndReverseDirection();
                }
                else
                {
                    clampToNearestEndpoint();
                }
            }
            else if (outsideEndPoints())
            {
                holdPlatform();
            }

            updatePosition();
        }

        private bool holdDurationExceeded(GameTime gameTime)
        {
            return (_holdElapsed += gameTime.ElapsedGameTime) > _maxHoldDuration;
        }

        private void releaseAndReverseDirection()
        {
            _beingHeld = false;
            _holdElapsed = TimeSpan.Zero;
            Velocity = -1 * _heldVelocity;
        }

        private void clampToNearestEndpoint()
        {
            Velocity = Vector2.Zero;
            if (isCloserToStart())
            {
                Position = _start;
            }
            else
            {
                Position = _stop;
                _holdingUntilReset = _moveOneTime;
            }
        }

        private bool isCloserToStart()
        {
            return Vector2.Distance(_start, Position) < Vector2.Distance(_stop, Position);
        }

        private void holdPlatform()
        {
            _beingHeld = true;
            _heldVelocity = Velocity;
            Velocity = Vector2.Zero;
        }

        private void updatePosition()
        {
            if (_holdingUntilReset)
            {
                Position = _stop;
                Velocity = Vector2.Zero;
            }
            else
            {
                Position += Velocity;
            }
        }

        private bool outsideEndPoints()
        {
            return !PointBetween(_start, _stop, Position + Velocity);
        }

        private bool PointBetween(Vector2 p1, Vector2 p2, Vector2 pos)
        {
            float xMin = Math.Min(p1.X, p2.X);
            float yMin = Math.Min(p1.Y, p2.Y);

            float xMax = Math.Max(p1.X, p2.X);
            float yMax = Math.Max(p1.Y, p2.Y);

            return (pos.X >= xMin && pos.X <= xMax)
                    && (pos.Y >= yMin && pos.Y <= yMax);
        }
    }
}
