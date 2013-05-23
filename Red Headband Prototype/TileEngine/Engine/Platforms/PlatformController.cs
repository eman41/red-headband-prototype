// -----------------------------------------------------------------------
// PlatformController.cs: Handles the movement, speed, and location of a platform
// Author: Eric S. Policaro
// -----------------------------------------------------------------------
namespace TileEngine.Engine.Platforms
{
    using System;
    using Microsoft.Xna.Framework;

    public class PlatformController : IResetable
    {
        private Vector2 _startPos = Vector2.Zero;
        private Vector2 _stopPos = Vector2.Zero;
        private float _speed = 0f;

        private bool _beingHeld = false;
        private bool _moveOneTime = false; 
        private bool _holdingUntilReset = false;

        private Timer _holdTimer;
        private Vector2 _heldVelocity = Vector2.Zero;

        /// <summary>
        /// Initializes a new platform controller.
        /// Start/Strop should be given in level tile coordinates.
        /// </summary>
        public PlatformController(Vector2 start, Vector2 stop, float speed, 
                                  TimeSpan holdTime, bool oneTime = false)
        {
            _holdTimer = new Timer(holdTime);
            _moveOneTime = oneTime;

            _startPos = TranslateToPixelCoords(start);
            _stopPos = TranslateToPixelCoords(stop);
            _speed = speed;

            Reset();
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

        /// <summary>Current X, Y pixel coordinates of this platform</summary>
        public Vector2 Position { get; set; }

        /// <summary>Current platform speed</summary>
        public Vector2 Velocity { get; set; }

        /// <summary>Reset this entity to it's starting state</summary>
        public void Reset()
        {
            Position = _startPos;
            Velocity = GetStartVelocity(_startPos, _stopPos, _speed);
            _beingHeld = false;
            _holdingUntilReset = false;
            _holdTimer.Reset();
        }

        /// <summary>Update the state of the platform.</summary>
        /// <param name="gameTime">Current Game time</param>
        public void Update(GameTime gameTime)
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
            else if (!BetweenEndPoints())
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
            Position = IsCloserToStart() ? _startPos : _stopPos;
            _holdingUntilReset = _moveOneTime && Position.Equals(_stopPos);
        }

        private bool IsCloserToStart()
        {
            return DistanceTo(_startPos) < DistanceTo(_stopPos);
        }

        private float DistanceTo(Vector2 vec)
        {
            return Vector2.Distance(vec, Position);
        }

        private bool BetweenEndPoints()
        {
            return MathUtilities.PointBetween(_startPos, _stopPos, Position + Velocity);
        }

        private void HoldPlatform()
        {
            _beingHeld = true;
            _heldVelocity = Velocity;
            Velocity = Vector2.Zero;
        }

        private void UpdatePosition()
        {
            if (!_holdingUntilReset)
            {
                Position += Velocity;
            }
        }
    }
}