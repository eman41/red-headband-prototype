namespace TileEngine.Collision
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using TileEngine.Engine;
    using Microsoft.Xna.Framework;
    using TileEngine.Engine.Platforms;

    public enum Axis
    {
        X_AXIS, Y_AXIS
    }

    [Flags]
    public enum Colliding
    {
        None = 0x0, 
        FromLeft = 0x1, 
        FromRight = 0x2, 
        FromTop = 0x4, 
        FromBottom = 0x8
    }

    public sealed class Collision
    {
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
                        return ResolveCollision(obj, tile.BoundingRect, axis);
                    }
                }
            }

            return Colliding.None;
        }

        public Colliding CheckPlatformCollision(GameObject obj, Axis axis)
        {
            foreach(var platform in MapLevel.Platforms)
            {
                if (obj.Collision(platform.BoundingRect))
                {
                    LastPlatform = platform;
                    return DetectCollision(obj, platform.BoundingRect, axis);
                }
            }

            return Colliding.None;
        }

        public Colliding DetectCollision(GameObject obj, Rectangle body, Axis axis)
        {
            return axis == Axis.X_AXIS ? XDirection(obj.BoundingRect, body) : YDirection(obj.BoundingRect, body);
        }

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

        public Colliding ResolveCollision(Rectangle obj, Rectangle body, Axis axis)
        {
            Colliding result = axis == Axis.X_AXIS ? XDirection(obj, body) : YDirection(obj, body);

            switch (result)
            {
                case Colliding.FromTop:
                    obj.Y += body.Top - obj.Bottom;
                    break;
                case Colliding.FromBottom:
                    obj.Y += body.Bottom - obj.Top;
                    break;
                case Colliding.FromLeft:
                    obj.X += body.Left - obj.Right;
                    break;
                case Colliding.FromRight:
                    obj.X += body.Right - obj.Left;
                    break;
            }

            return result;
        }

        public Colliding YDirection(Rectangle obj, Rectangle body)
        {
            if (obj.Bottom >= body.Top && obj.Top <= body.Top)
            {
                return Colliding.FromTop;
            }
            else if (obj.Top <= body.Bottom && obj.Bottom >= body.Bottom)
            {
                return Colliding.FromBottom;
            }

            return Colliding.None;
        }

        public Colliding XDirection(Rectangle obj, Rectangle body)
        {
            if (obj.Right >= body.Left && obj.Left <= body.Left)
            {
                return Colliding.FromLeft;
            }
            else if (obj.Left <= body.Right && obj.Right >= body.Right)
            {
                return Colliding.FromRight;
            }

            return Colliding.None;
        }

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
