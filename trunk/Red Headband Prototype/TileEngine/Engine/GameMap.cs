// -----------------------------------------------------------------------
// <copyright file="Level.cs" company="Me" />
// Author: Eric S. Policaro
// A game map/level
// -----------------------------------------------------------------------
namespace TileEngine.Engine
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using TileEngine.Engine.Platforms;

    public class GameMap
    {
        public const int TILE_SIZE = 32;

        private string _levelName;
        private Texture2D _tileSheet;
        private TileObject[,] _tileMap;
        private List<GameObject> saveLadders;
        private Dictionary<int, Rectangle> _tileDrawRects;

        private GameMap()
        {
            PlayerStart = Vector2.Zero;
            BackgroundColor = Color.Black;
            Gravity = 8.0f;
            LadderBounds = new List<Rectangle>();
            Platforms = new List<MovingPlatform>();
            KillRects = new List<Rectangle>();
            saveLadders = new List<GameObject>();
            _tileDrawRects = new Dictionary<int, Rectangle>();
        }

        public GameMap(string levelName, string path, Texture2D tileSheet)
            : this()
        {
            if (!File.Exists(path))
            {
                throw new IOException("LEVEL FILE NOT FOUND");
            }

            _levelName = levelName;
            _tileSheet = tileSheet;
            ReadLevelFile(path);
            LadderBounds = AssembleLadders(saveLadders, TILE_SIZE);
        }
        
        private void ReadLevelFile(String path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                List<GameObject> saveLadders = new List<GameObject>();
                bool first = true;
                while (reader.Peek() >= 0)
                {
                    string[] tokens = reader.ReadLine().Split(',');
                    if (first)
                    {
                        ReadSetupLine(first, tokens);
                        first = false;
                    }
                    else
                    {
                        int x = Convert.ToInt32(tokens[0].Trim());
                        int y = Convert.ToInt32(tokens[1].Trim());
                        TileObject tile = CreateNewTile(x, y, tokens);
                        _tileMap[y, x] = tile;
                        HandleSpecialTile(tile);
                    }
                }
            }
        }

        private void ReadSetupLine(bool first, string[] tokens)
        {
            LevelWidth = Convert.ToInt32(tokens[1]) / TILE_SIZE;
            LevelHeight = Convert.ToInt32(tokens[2]) / TILE_SIZE;
            string[] rgbTokens = tokens[4].Split(';');
            BackgroundColor = new Color(
                Convert.ToInt32(rgbTokens[0]),
                Convert.ToInt32(rgbTokens[1]),
                Convert.ToInt32(rgbTokens[2]));
            _tileMap = new TileObject[LevelHeight, LevelWidth];
            CreateTileDrawRects(_tileSheet, TILE_SIZE, 34);
        }

        private void CreateTileDrawRects(Texture2D tileset, int tileSize, int tileWidth)
        {
            int count = 1;
            int offset = Convert.ToInt32((tileWidth - tileSize) * 0.5f);
            int x = tileset.Width / tileSize;
            int y = tileset.Height / tileSize;
            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    Rectangle rect = new Rectangle(
                        j * tileWidth + offset,
                        i * tileWidth + offset,
                        tileSize, tileSize);
                    _tileDrawRects.Add(count, rect);
                    count++;
                }
            }
        }

        private TileObject CreateNewTile(int posX, int posY, string[] tokens)
        {
            int TYPE_INDEX = 2;
            string tileName = GetTileName(tokens);
            float rotation = GetRotationAsRads(tokens);
            Vector2 origin = GetAdjustedOrigin(rotation);
            Rectangle draw = GetDrawRect(tileName);
            TileType type = TileObject.TypeTranslation(tokens[TYPE_INDEX].Trim());

            TileObject node = new TileObject(
                new Vector2(posX * TILE_SIZE, posY * TILE_SIZE),
                new Rectangle(posX * TILE_SIZE, posY * TILE_SIZE, TILE_SIZE, TILE_SIZE),
                draw, type);
            node.Origin = origin;
            node.Effects = SpriteEffects.None;
            node.Rotation = rotation;
            return node;
        }
        
        private String GetTileName(string[] tokens)
        {
            int NAME_INDEX = 4;
            return tokens[NAME_INDEX].Trim();
        }

        private float GetRotationAsRads(string[] tokens)
        {
            int ROTA_INDEX = 3;
            float rotation = Convert.ToInt32(tokens[ROTA_INDEX].Trim());
            return MathHelper.ToRadians(rotation);
        }

        // Move origin to rotate tile around its center
        private Vector2 GetAdjustedOrigin(float rotation)
        {
            Vector2 origin = Vector2.Zero;
            if (rotation > 0)
            {
                origin.X = TILE_SIZE * 0.5f;
                origin.Y = TILE_SIZE * 0.5f;
            }
            return origin;
        }

        private Rectangle GetDrawRect(String tileName)
        {
            if (!tileName.Contains("Tile"))
            {
                return new Rectangle();
            }
            else
            {
                int tileNumber = Convert.ToInt32(tileName.Replace("Tile ", string.Empty));
                return new Rectangle(
                    _tileDrawRects[tileNumber].X,
                    _tileDrawRects[tileNumber].Y,
                    _tileDrawRects[tileNumber].Width,
                    _tileDrawRects[tileNumber].Height);
            }
        }

        private void HandleSpecialTile(TileObject tile)
        {
            if (TileType.Ladder == tile.Type)
            {
                saveLadders.Add(tile);
            }
            else if (TileType.Spikes == tile.Type)
            {
                KillRects.Add(tile.BoundingRect);
            }
            else if (TileType.Player == tile.Type)
            {
                int posX = tile.Coords.X;
                int posY = tile.Coords.Y;
                PlayerStart = new Vector2(posX * TILE_SIZE, posY * TILE_SIZE);
            }
        }

        private List<Rectangle> AssembleLadders(List<GameObject> nodes, int tileSize)
        {
            List<Rectangle> result = new List<Rectangle>();
            while (nodes.Count > 0)
            {
                Rectangle current = new Rectangle(0, 0, tileSize, tileSize);
                GameObject root = nodes[0];
                int yTrack = root.Coords.Y;
                current.X = (int)root.Position.X;
                current.Y = (int)root.Position.Y;
                nodes.RemoveAt(0);
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (VerifyNode(nodes[i], root) && (nodes[i].Coords.Y - 1) == yTrack)
                    {
                        current.Height += tileSize;
                        yTrack++;
                        nodes.RemoveAt(i--);
                    }
                }

                result.Add(current);
            }

            return result;
        }

        private bool VerifyNode(GameObject nodeA, GameObject nodeB)
        {
            return (nodeA != null) && (nodeB != null)
                    && (nodeA.Coords.X == nodeB.Coords.X);
        }

        public Texture2D PlatformTextures { get; set; }
        public List<MovingPlatform> Platforms { get; set; }
        public List<Rectangle> LadderBounds { get; set; }
        public List<Rectangle> KillRects { get; set; }
        public Rectangle ActiveLadder { get; set; }
        public float Gravity { get; set; }
        public Color BackgroundColor { get; set; }
        public Vector2 PlayerStart { get; set; }

        /// <summary>
        /// Gets or sets the level width in tiles.
        /// </summary>
        public int LevelWidth { get; set; }

        /// <summary>
        /// Gets or sets the level height in tiles.
        /// </summary>
        public int LevelHeight { get; set; }

        /// <summary>
        /// Gets the name of the level.
        /// </summary>
        public string LevelName
        {
            private set
            {
                _levelName = value;
            }

            get
            {
                return _levelName;
            }
        }

        /// <summary>
        /// Gets the tile map.
        /// </summary>
        public TileObject[,] LevelTiles
        {
            get
            {
                return _tileMap;
            }
        }

        /// <summary>
        /// Level width in pixels.
        /// </summary>
        public float RealLevelWidth
        {
            get { return LevelWidth * TILE_SIZE; }
        }

        /// <summary>
        /// Level height in pixels.
        /// </summary>
        public float RealLevelHeight
        {
            get { return LevelHeight * TILE_SIZE; }
        }

        /// <summary>
        /// Check if the given body is colliding with any ladders.
        /// </summary>
        public bool LadderCollision(Rectangle body)
        {
            foreach (Rectangle rect in LadderBounds)
            {
                if (body.Intersects(rect))
                {
                    ActiveLadder = rect;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if the given body is colliding with any kill platforms.
        /// </summary>
        public bool KillCollision(Rectangle body)
        {
            foreach (var rect in KillRects)
            {
                if(body.Intersects(rect))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Check if the given body is completely above the currently avtive ladder.
        /// </summary>
        public bool AboveLadder(GameObject body)
        {
            return body.BoundingRect.Top < ActiveLadder.Top &&
                    body.YRectAlign(ActiveLadder);
        }

        /// <summary>
        /// Update the platforms in the level.
        /// </summary>
        /// <param name="gameTime">Game time snapshot</param>
        public void Update(GameTime gameTime)
        {
            foreach (var platform in Platforms)
            {
                platform.Update(gameTime);
            }
        }

        /// <summary>
        /// Draw the level.
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="transform"></param>
        public void Draw(SpriteBatch batch)
        {
            DrawPlatforms(batch);
            DrawTileMap(batch);
        }

        private void DrawPlatforms(SpriteBatch batch)
        {
            foreach (var platform in Platforms)
            {
                platform.Draw(batch);
            }
        }

        private void DrawTileMap(SpriteBatch batch)
        {
            for (int y = 0; y < LevelHeight; y++)
            {
                for (int x = 0; x < LevelWidth; x++)
                {
                    if (!IsDrawAble(_tileMap[y, x]))
                    {
                        continue;
                    }

                    TileObject tile = _tileMap[y, x];
                    Vector2 drawPos = tile.Position + tile.Origin;
                    batch.Draw(_tileSheet, drawPos, tile.DrawRect,
                        tile.ObjectColor, tile.Rotation, tile.Origin,
                        tile.Scale, tile.Effects, tile.ZLayer);
                    
                }
            }
        }

        private bool IsDrawAble(TileObject tile)
        {
            return (tile != null) && (tile.Type != TileType.Empty);
        }
    }
}