namespace TileEngine.Engine
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class GameObject
    {
        public Vector2 Position;
        public Vector2 Velocity;
        protected Rectangle _boundingRect;

        public GameObject(Vector2 pos, Rectangle rect, Vector2 velocity, bool active)
        {
            Position = pos;
            _boundingRect = rect;
            Velocity = velocity;
            Active = active;
            ZLayer = 1f;
            Scale = 1;
            ObjectColor = Color.White;
            Origin = Vector2.Zero;
            Rotation = 0f;
        }

        public bool Active { get; set; }
        public int Scale { get; set; }
        public float ZLayer { get; set; }
        public Color ObjectColor { get; set; }
        public SpriteEffects Effects { get; set; }
        public Vector2 Origin { get; set; }
        public float Rotation { get; set; }

        public Point Coords
        {
            get
            {
                return new Point(
                    BoundingRect.Center.X / GameMap.TILE_SIZE, 
                    BoundingRect.Center.Y / GameMap.TILE_SIZE);
            }
        }

        public Rectangle BoundingRect
        {
            get
            {
                if (Position.X == _boundingRect.X && Position.Y == _boundingRect.Y)
                {
                    return _boundingRect;
                }
                else
                {
                    _boundingRect = new Rectangle(
                        (int)Position.X, (int)Position.Y, 
                        _boundingRect.Width, _boundingRect.Height);
                    return _boundingRect;
                }
            }
        }

        public bool InMotion
        {
            get
            {
                return Math.Abs(Velocity.X) > 0f || Math.Abs(Velocity.Y) > 0f;
            }
        }

        public bool YRectAlign(Rectangle incoming)
        {
            return BoundingRect.Left >= incoming.Left && BoundingRect.Right <= incoming.Right;
        }

        public bool Collision(Rectangle incoming)
        {
            return BoundingRect.Intersects(incoming);
        }
    }
}
