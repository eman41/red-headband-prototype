// -----------------------------------------------------------------------
// Collision.cs: Provides utility for detecting and resolving collisions.
// Author: Eric S. Policaro
// -----------------------------------------------------------------------
namespace TileEngine.Collision
{
    using System;
    using TileEngine.Engine;
    using Microsoft.Xna.Framework;
    using TileEngine.Engine.Platforms;

    /// <summary>
    /// Axes for detection.
    /// </summary>
    public enum Axis
    {
        X_AXIS, Y_AXIS
    }

    /// <summary>
    /// Directions that collisions can occur.
    /// </summary>
    [Flags]
    public enum Colliding
    {
        None = 0x0, 
        FromLeft = 0x1, 
        FromRight = 0x2, 
        FromTop = 0x4, 
        FromBottom = 0x8
    }

    /// <summary>
    /// Class used to detect and resolve collision against a level and objects/platforms
    /// that exist in that level.
    /// </summary>
    public class Collision
    {
        /// <summary>
        /// Creates a new instance of Collision.
        /// </summary>
        /// <param name="level">Level to use in collision tests</param>
        public Collision(GameMap level)
        {
            MapLevel = level;
            ScanBounds = new Rectangle(0, 0, level.LevelWidth, level.LevelHeight);
            LastPlatform = null;
        }

        public GameMap MapLevel { get; set; }
        public Rectangle ScanBounds { get; set; }
        public TileObject LastCollider { get; set; }
        public MovingPlatform LastPlatform { get; set; }

        /// <summary>
        /// Detect a collision between the level and the specified game object
        /// on the given axis.
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <param name="axis">Axis to detect along</param>
        /// <returns>Direction of the collision</returns>
        public Colliding CheckLevelCollision(GameObject obj, Axis axis)
        {
            for (int y = ScanBounds.Y; y < ScanBounds.Height; y++)
            {
                for (int x = ScanBounds.X; x < ScanBounds.Width; x++)
                {
                    TileObject tile = MapLevel.LevelTiles[y, x];
                    if (TileObject.IsCollidable(tile) && obj.Collision(tile.BoundingRect))
                    {
                        LastCollider = tile;
                        return DetectAndResolveCollision(obj, tile.BoundingRect, axis);
                    }
                }
            }

            return Colliding.None;
        }

        /// <summary>
        /// Detect a collision between the level and bounding rectangle.
        /// </summary>
        /// <param name="obj">Rectangle to check</param>
        /// <param name="axis">Axis to detect along</param>
        /// <returns>Direction of the collision</returns>
        public Colliding CheckLevelCollision(Rectangle obj, Axis axis)
        {
            for (int y = ScanBounds.Y; y < ScanBounds.Height; y++)
            {
                for (int x = ScanBounds.X; x < ScanBounds.Width; x++)
                {
                    TileObject tile = MapLevel.LevelTiles[y, x];
                    if (TileObject.IsCollidable(tile) && obj.Intersects(tile.BoundingRect))
                    {
                        LastCollider = tile;
                        return ResolveRectCollision(obj, tile.BoundingRect, axis);
                    }
                }
            }

            return Colliding.None;
        }

        /// <summary>
        /// Detects a collision between a game object and a moving platform.
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <param name="axis">Axis to detect along</param>
        /// <returns>Direction of the collision</returns>
        public Colliding CheckPlatformCollision(GameObject obj, Axis axis)
        {
            foreach(MovingPlatform platform in MapLevel.Platforms)
            {
                if (obj.Collision(platform.BoundingRect))
                {
                    LastPlatform = platform;
                    return DetectCollision(obj, platform.BoundingRect, axis);
                }
            }

            return Colliding.None;
        }

        private Colliding DetectCollision(GameObject obj, Rectangle body, Axis axis)
        {
            return axis == Axis.X_AXIS ? XDirection(obj.BoundingRect, body) : YDirection(obj.BoundingRect, body);
        }

        /// <summary>
        /// Resolve a collision between a game object and bounding rectangle
        /// that occurred in the given direction.
        /// </summary>
        /// <param name="obj">Object to check</param>
        /// <param name="body">Axis to detect along</param>
        /// <param name="colliding">Direction of the collision</param>
        public void ResolveCollision(GameObject obj, Rectangle? body, Colliding colliding)
        {
            if (body.HasValue)
            {
                switch (colliding)
                {
                    case Colliding.FromTop:
                        obj.Position.Y += body.Value.Top - obj.BoundingRect.Bottom;
                        break;
                    case Colliding.FromBottom:
                        obj.Position.Y += body.Value.Bottom - obj.BoundingRect.Top;
                        break;
                    case Colliding.FromLeft:
                        obj.Position.X += body.Value.Left - obj.BoundingRect.Right;
                        break;
                    case Colliding.FromRight:
                        obj.Position.X += body.Value.Right - obj.BoundingRect.Left;
                        break;
                }
            }
        }

        /// <summary>
        /// Resolve a collision between a game object and a target on a specified axis.
        /// </summary>
        /// <param name="obj">Subject bounding rectangle</param>
        /// <param name="target">Target bounding rectangle</param>
        /// <param name="axis">Axis on which to resolve.</param>
        /// <returns>Colliding indicating the direction of the collision.</returns>
        public Colliding DetectAndResolveCollision(GameObject obj, Rectangle body, Axis axis)
        {
            Colliding result = DetectCollision(obj, body, axis);

            switch (result)
            {
                case Colliding.FromTop:
                    obj.Position.Y += body.Top - obj.BoundingRect.Bottom;
                    break;
                case Colliding.FromBottom:
                    obj.Position.Y += body.Bottom - obj.BoundingRect.Top;
                    break;
                case Colliding.FromLeft:
                    obj.Position.X += body.Left - obj.BoundingRect.Right;
                    break;
                case Colliding.FromRight:
                    obj.Position.X += body.Right - obj.BoundingRect.Left;
                    break;
            }

            return result;
        }

        // Resolve a collision between two bounding rectangles
        private Colliding ResolveRectCollision(Rectangle obj, Rectangle target, Axis axis)
        {
            Colliding result = (axis == Axis.X_AXIS) ? XDirection(obj, target) : YDirection(obj, target);

            switch (result)
            {
                case Colliding.FromTop:
                    obj.Y += target.Top - obj.Bottom;
                    break;
                case Colliding.FromBottom:
                    obj.Y += target.Bottom - obj.Top;
                    break;
                case Colliding.FromLeft:
                    obj.X += target.Left - obj.Right;
                    break;
                case Colliding.FromRight:
                    obj.X += target.Right - obj.Left;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Check for a collision on the X axis.
        /// </summary>
        /// <param name="obj">Subject bounding rectangle</param>
        /// <param name="target">Target bounding rectangle</param>
        /// <returns>Colliding indicating the direction of the collision.</returns>
        public Colliding YDirection(Rectangle obj, Rectangle target)
        {
            if (ObjectAboveBody(ref obj, ref target))
            {
                return Colliding.FromTop;
            }
            else if (ObjectBelowBody(ref obj, ref target))
            {
                return Colliding.FromBottom;
            }

            return Colliding.None;
        }

        private bool ObjectBelowBody(ref Rectangle obj, ref Rectangle target)
        {
            return obj.Top <= target.Bottom && obj.Bottom >= target.Bottom;
        }

        private bool ObjectAboveBody(ref Rectangle obj, ref Rectangle target)
        {
            return obj.Bottom >= target.Top && obj.Top <= target.Top;
        }

        /// <summary>
        /// Check for a collision on the X axis.
        /// </summary>
        /// <param name="obj">Subject bounding rectangle</param>
        /// <param name="target">Target bounding rectangle</param>
        /// <returns>Colliding indicated the direction of the collision.</returns>
        public Colliding XDirection(Rectangle obj, Rectangle target)
        {
            if (ObjectLeftOfBody(ref obj, ref target))
            {
                return Colliding.FromLeft;
            }
            else if (ObjectRightOfBody(ref obj, ref target))
            {
                return Colliding.FromRight;
            }

            return Colliding.None;
        }

        private bool ObjectRightOfBody(ref Rectangle obj, ref Rectangle target)
        {
            return obj.Left <= target.Right && obj.Right >= target.Right;
        }

        private bool ObjectLeftOfBody(ref Rectangle obj, ref Rectangle target)
        {
            return obj.Right >= target.Left && obj.Left <= target.Left;
        }

        /// <summary>
        /// Updates boundary around the player that will be checked for collision.
        /// </summary>
        /// <param name="coords">Player position in level coordinates</param>
        /// <param name="threshold">Distance in tiles from the player to be detected</param>
        /// <returns>Re-calculated collision boundary</returns>
        public Rectangle UpdateScanBounds(Point coords, int threshold)
        {
            Rectangle scanbounds = new Rectangle(
                Math.Max(0, coords.X - threshold),
                Math.Max(0, coords.Y - threshold),
                Math.Min(MapLevel.LevelWidth, coords.X + threshold),
                Math.Min(MapLevel.LevelHeight, coords.Y + threshold));
            return scanbounds;
        }
    }
}