// -----------------------------------------------------------------------
// <copyright file="TileObject.cs" company="Me">
// Author: Eric S. Policaro
// </copyright>
// -----------------------------------------------------------------------

namespace TileEngine.Engine
{
    using Microsoft.Xna.Framework;
    using System;

    [Flags]
    public enum TileType
    {
        Empty = 0x0000,
        Tile = 0x0001,
        Player = 0x0002,
        Flavor = 0x0003,
        Ladder = 0x0004,
        Bullet = 0x0005,
        Spikes = 0x0006
    }

    public class TileObject : GameObject
    {
        private TileType _type;
        protected Rectangle _drawRect;

        public TileObject() 
            : base (Vector2.Zero, new Rectangle(), Vector2.Zero, false)
        {
            _type = TileType.Empty;
        }

        public TileObject(Vector2 position, Rectangle bounds, Rectangle draw, bool active, TileType type)
            : base(position, bounds, Vector2.Zero, active)
        {
            _type = type;
            _drawRect = draw;
        }
        public Rectangle DrawRect
        {
            get
            {
                return _drawRect;
            }
        }

        public TileType Type
        {
            get
            {
                return _type;
            }
        }

        public static bool IsCollidable(TileObject tile)
        {
            return tile != null && (tile.Type == TileType.Tile);
        }

        public static TileType TypeTranslation(string typeName)
        {
            switch (typeName)
            {
                case "Tile":
                    return TileType.Tile;
                case "Player":
                    return TileType.Player;
                case "Flavor":
                    return TileType.Flavor;
                case "Ladder":
                    return TileType.Ladder;
                case "Bullet":
                    return TileType.Bullet;
                case "Spikes":
                    return TileType.Spikes;
                default:
                    return TileType.Empty;
            }
        }
        
        public static int GetFromBits(ushort bits, int offset)
        {
            return (bits >> (offset - 1)) & 0xF;
        }
    }
}
