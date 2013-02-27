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
        private Vector2 _startPos = Vector2.Zero;
        private Vector2 _stopPos = Vector2.Zero;

        private bool _beingHeld = false;
        private bool _moveOneTime = false; 
        private bool _holdingUntilReset = false;

        private Timer _holdTimer;
        private Vector2 _heldVelocity = Vector2.Zero;

        /// <summary>
        /// Initializes a new platform controller. 
        /// Position Vectors should be given in level tile coordinates.
        /// </summary>
        public PlatformController(Vector2 start, Vector2 stop, float speed, 
            TimeSpan holdTime, bool oneTime = false)
        {
            _holdTimer = new Timer(holdTime);
            _moveOneTime = oneTime;

            _startPos = TranslateToPixelCoords(start);
            _stopPos = TranslateToPixelCoords(stop);

            Position = _startPos;
            Velocity = GetStartVelocity(start, stop, speed);
        }

        private Vector2 TranslateToPixelCoords(Vector2 levelCoords)
        {
            float x = levelCoords.X * GameMap.TILE_SIZE;
            float y = levelCoords.Y * GameMap.TILE_SIZE;
            return new Vector2(x, y);
        }

        private Vector2 GetStartVelocity(Vector2 start, Vector2 stop, float speed)
        {
            Vector2 moveNormal = new Vector2(start.X - stop.X, start.Y - stop.Y);
            moveNormal.Normalize();
            return new Vector2(moveNormal.X * speed, moveNormal.Y * speed);
        }

        /// <summary>
        /// Current X, Y pixel coordinates of this platform
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Current platform speed
        /// </summary>
        public Vector2 Velocity { get; set; }

        /// <summary>
        /// Reset this entity to it's starting state
        /// </summary>
        public void Reset()
        {
            _beingHeld = false;
            _holdingUntilReset = false;
            Position = _startPos;
            _holdTimer.Reset();
        }

        /// <summary>
        /// Update the state of the platform.
        /// </summary>
        /// <param name="gameTime">Current Game time</param>
        public virtual void Update(GameTime gameTime)
        {
            if (_beingHeld)
            {
                if (HoldDurationExceeded(gameTime.ElapsedGameTime))
                {
                    ReleaseAndReverseDirection();
                }
                else
                {
                    ClampToNearestEndpoint();
                }
            }
            else if (OutsideEndPoints())
            {
                HoldPlatform();
            }

            UpdatePosition();
        }

        private bool HoldDurationExceeded(TimeSpan elapsed)
        {
            return _holdTimer.AdvanceTimerCyclic(elapsed);

        }

        private void ReleaseAndReverseDirection()
        {
            _beingHeld = false;
            Velocity = -1 * _heldVelocity;
        }

        private void ClampToNearestEndpoint()
        {
            Velocity = Vector2.Zero;
            if (IsCloserToStart())
            {
                Position = _startPos;
            }
            else
            {
                Position = _stopPos;
                _holdingUntilReset = _moveOneTime;
            }
        }

        private bool IsCloserToStart()
        {
            return DistanceTo(_startPos) < DistanceTo(_stopPos);
        }

        private float DistanceTo(Vector2 vec)
        {
            return Vector2.Distance(vec, Position);
        }

        private bool OutsideEndPoints()
        {
            return !PointBetween(_startPos, _stopPos, Position + Velocity);
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

        private void HoldPlatform()
        {
            _beingHeld = true;
            _heldVelocity = Velocity;
            Velocity = Vector2.Zero;
        }

        private void UpdatePosition()
        {
            if (_holdingUntilReset)
            {
                Position = _stopPos;
                Velocity = Vector2.Zero;
            }
            else
            {
                Position += Velocity;
            }
        }   
    }
}
