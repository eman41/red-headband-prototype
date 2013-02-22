namespace TileEngine.Engine.Platforms
{
    using System;
    using Microsoft.Xna.Framework;
    using TileEngine.Engine;

    public class PlatformController : IResetable
    {
        private Vector2 _start = Vector2.Zero;
        private Vector2 _stop = Vector2.Zero;
        private bool _oneTime = false;
        private bool _onHold = false;
        private bool _permaHold = false;
        private TimeSpan _holdElapsed = TimeSpan.Zero;
        private TimeSpan _holdTime = TimeSpan.Zero;

        public PlatformController(Vector2 start, Vector2 stop, float speed, TimeSpan holdTime, bool oneTime = false)
        {
            Speed = speed;
            _holdTime = holdTime;
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
        public float Speed { get; set; }

        public void Reset()
        {
            _onHold = false;
            _permaHold = false;
            Position = _start;
            _holdElapsed = TimeSpan.Zero;
        }

        Vector2 _holdnormal = Vector2.Zero;
        public virtual void Update(GameTime gameTime)
        {
            if (_onHold)
            {
                if ((_holdElapsed += gameTime.ElapsedGameTime) > _holdTime)
                {
                    _onHold = false;
                    _holdElapsed = TimeSpan.Zero;
                    Velocity = -1 * _holdnormal;
                    Position += Velocity;
                }
                else
                {
                    if (isCloserToStart())
                    {
                        Position = _start;
                    }
                    else
                    {
                        Position = _stop;
                        _permaHold = _oneTime;
                    }
                    Velocity = Vector2.Zero;
                }
            }
            else if (betweenEndPoints())
            {
                _onHold = true;
                _holdnormal = Velocity;
                Velocity.Normalize();
                Velocity = Vector2.Zero;
            }

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

        private bool isCloserToStart()
        {
            return Vector2.Distance(_start, Position) < Vector2.Distance(_stop, Position);
        }

        private bool betweenEndPoints()
        {
            return !PointBetween(_start, _stop, Position + Velocity);
        }

        public static bool PointBetween(Vector2 p1, Vector2 p2, Vector2 pos)
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
