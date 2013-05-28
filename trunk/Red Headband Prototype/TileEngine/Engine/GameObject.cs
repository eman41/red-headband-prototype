// -----------------------------------------------------------------------
// GameObject.cs: A drawable entity with bounding box (full sprite options included).
// Author: Eric S. Policaro
// -----------------------------------------------------------------------
namespace TileEngine.Engine
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Class representing a standard game object.
    /// </summary>
    public class GameObject
    {
        public Vector2 Position;
        public Vector2 Velocity;
        protected Rectangle _boundingRect;

        /// <summary>
        /// Create a new GameObject.
        /// </summary>
        /// <param name="pos">Starting position</param>
        /// <param name="rect">Bounding rectangle</param>
        /// <param name="velocity">Velocity of this object (moving)</param>
        /// <param name="active">Object is active</param>
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

        /// <summary>
        /// Gets the object position in level coordinates.
        /// </summary>
        public Point Coords
        {
            get
            {
                return new Point(
                    BoundingRect.Center.X / GameMap.TILE_SIZE, 
                    BoundingRect.Center.Y / GameMap.TILE_SIZE);
            }
        }

        /// <summary>
        /// Gets the bounding rectangle for this object.
        /// </summary>
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

        /// <summary>
        /// Gets true if the object is in motion.
        /// </summary>
        public bool InMotion
        {
            get
            {
                return Math.Abs(Velocity.X) > 0f || Math.Abs(Velocity.Y) > 0f;
            }
        }

        /// <summary>
        /// Checks if this object is inside of the given object vertically.
        /// </summary>
        /// <param name="incoming">Bounding rectangle to check</param>
        /// <returns>True if object is inside (vertically)</returns>
        public bool YRectAlign(Rectangle incoming)
        {
            return BoundingRect.Left >= incoming.Left && BoundingRect.Right <= incoming.Right;
        }

        /// <summary>
        /// Checks if this object is colliding with the given rectangle.
        /// </summary>
        /// <param name="incoming">Rectangle to check</param>
        /// <returns>True if a collision has occurred</returns>
        public bool Collision(Rectangle incoming)
        {
            return BoundingRect.Intersects(incoming);
        }
    }
}
