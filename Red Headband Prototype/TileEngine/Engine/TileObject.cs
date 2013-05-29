// -----------------------------------------------------------------------
// TileObject.cs: Game tile related objects.
// Author: Eric S. Policaro
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

    /// <summary>
    /// Class representing a tile on a level.
    /// </summary>
    public class TileObject : GameObject
    {
        private TileType _type;
        protected Rectangle _drawRect;

        /// <summary>
        /// Creates a new tile.
        /// </summary>
        public TileObject() 
            : base (Vector2.Zero, new Rectangle(), Vector2.Zero, false)
        {
            _type = TileType.Empty;
        }

        /// <summary>
        /// Creates a new tile with the specified parameters.
        /// </summary>
        public TileObject(Vector2 position, Rectangle bounds, Rectangle draw, TileType type)
            : base(position, bounds, Vector2.Zero, true)
        {
            _type = type;
            _drawRect = draw;
        }

        /// <summary>
        /// Gets the recentagle that will draw this tile.
        /// </summary>
        public Rectangle DrawRect
        {
            get
            {
                return _drawRect;
            }
        }

        /// <summary>
        /// Gets or Sets the tile's type.
        /// </summary>
        public TileType Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        /// <summary>
        /// Check if the given tile can be collided into.
        /// </summary>
        /// <param name="tile">Tile to check</param>
        /// <returns>True if can be collided.</returns>
        public static bool IsCollidable(TileObject tile)
        {
            return tile != null && (tile.Type == TileType.Tile);
        }

        /// <summary>
        /// Translates a string type name into the required type enum.
        /// </summary>
        /// <param name="typeName">Name to translate</param>
        /// <returns>TileType instance</returns>
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
    }
}
