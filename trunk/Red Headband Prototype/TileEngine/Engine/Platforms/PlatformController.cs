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

        private bool _oneTime = false;   // Platform will move once
        private bool _permaHold = false; // Hold until Reset()
        private bool _onHold = false;    // Platform being held at an endpoint
        
        // Hold Trackers
        private Vector2 _heldVelocity = Vector2.Zero;
        private TimeSpan _holdElapsed = TimeSpan.Zero;
        private TimeSpan _maxHoldDuration = TimeSpan.Zero;

        /// <summary>
        /// Initializes a new platform controller.
        /// </summary>
        /// <param name="start">Starting Tile Position</param>
        /// <param name="stop">End Tile Position</param>
        /// <param name="speed">Platform speed</param>
        /// <param name="holdTime">Length of time the platform spends at endpoints</param>
        /// <param name="oneTime">True: Platform only moves once (Default is false)</param>
        public PlatformController(Vector2 start, Vector2 stop, float speed, TimeSpan holdTime, bool oneTime = false)
        {
            _maxHoldDuration = holdTime;
            _oneTime = oneTime;

            _start = new Vector2(start.X * GameMap.TILE_SIZE, start.Y * GameMap.TILE_SIZE);
            _stop = new Vector2(stop.X * GameMap.TILE_SIZE, stop.Y * GameMap.TILE_SIZE);
            Position = _start;

            Vector2 moveNormal = new Vector2(start.X - stop.X, start.Y - stop.Y);
            moveNormal.Normalize();
            Velocity = new Vector2(moveNormal.X * speed, moveNormal.Y * speed);
        }

        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; private set; }

        /// <summary>
        /// Reset this entity to it's starting state
        /// </summary>
        public void Reset()
        {
            _onHold = false;
            _permaHold = false;
            Position = _start;
            _holdElapsed = TimeSpan.Zero;
        }

        /// <summary>
        /// Update the state of the platform.
        /// </summary>
        /// <param name="gameTime">Current Game time</param>
        public virtual void Update(GameTime gameTime)
        {
            if (_onHold)
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
            _onHold = false;
            _holdElapsed = TimeSpan.Zero;
            Velocity = -1 * _heldVelocity;
            Position += Velocity;
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
                _permaHold = _oneTime;
            }
        }

        private bool isCloserToStart()
        {
            return Vector2.Distance(_start, Position) < Vector2.Distance(_stop, Position);
        }

        private void holdPlatform()
        {
            _onHold = true;
            _heldVelocity = Velocity;
            Velocity = Vector2.Zero;
        }

        private void updatePosition()
        {
            if (_permaHold)
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
