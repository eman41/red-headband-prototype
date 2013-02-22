// -----------------------------------------------------------------------
// <copyright file="Level.cs" company="Me">
// Author: Eric S. Policaro
// </copyright>
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
        private Dictionary<int, Rectangle> _tileDrawRects = new Dictionary<int, Rectangle>();

        public GameMap(string levelName, string path, Texture2D tileSheet)
        {
            _levelName = levelName;
            _tileSheet = tileSheet;
            PlayerStart = Vector2.Zero;
            BackgroundColor = Color.Black;
            Gravity = 8.0f;
            LadderBounds = new List<Rectangle>();
            Platforms = new List<MovingPlatform>();
            KillRects = new List<Rectangle>();
            List<GameObject> saveLadders = new List<GameObject>();

            bool first = true;
            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    while (reader.Peek() >= 0)
                    {
                        string[] tokens = reader.ReadLine().Split(new string[] { "," }, StringSplitOptions.None);
                        if (first)
                        {

                            LevelWidth = Convert.ToInt32(tokens[1]) / TILE_SIZE;
                            LevelHeight = Convert.ToInt32(tokens[2]) / TILE_SIZE;
                            string[] rgbTokens = tokens[4].Split(new string[] { ";" }, StringSplitOptions.None);
                            BackgroundColor = new Color(
                                Convert.ToInt32(rgbTokens[0]),
                                Convert.ToInt32(rgbTokens[1]),
                                Convert.ToInt32(rgbTokens[2]));
                            _tileMap = new TileObject[LevelHeight, LevelWidth];
                            _tileDrawRects = CreateTileDrawRects(_tileSheet, TILE_SIZE, 34);
                            first = false;
                        }
                        else
                        {
                            int x = Convert.ToInt32(tokens[0].Trim());
                            int y = Convert.ToInt32(tokens[1].Trim());
                            _tileMap[y, x] = CreateTileFromTokens(x, y, tokens);
                            if (_tileMap[y, x].Type == TileType.Ladder)
                            {
                                saveLadders.Add(_tileMap[y, x]);
                            }
                            else if (_tileMap[y, x].Type == TileType.Spikes)
                            {
                                KillRects.Add(_tileMap[y, x].BoundingRect);
                            }
                        }
                    }
                }
            }

            LadderBounds = AssembleLadders(saveLadders, TILE_SIZE);
        }

        public Texture2D PlatformTextures { get; set; }
        public List<MovingPlatform> Platforms { get; set; }
        public List<Rectangle> LadderBounds { get; set; }
        public List<Rectangle> KillRects { get; set; }
        public Rectangle ActiveLadder { get; set; }
        public float Gravity { get; set; }
        public Color BackgroundColor { get; set; }
        public Vector2 PlayerStart { get; set; }
        public int LevelWidth { get; set; }
        public int LevelHeight { get; set; }

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

        public TileObject[,] LevelTiles
        {
            get
            {
                return _tileMap;
            }
        }

        public float RealLevelWidth
        {
            get { return LevelWidth * TILE_SIZE; }
        }

        public float RealLevelHeight
        {
            get { return LevelHeight * TILE_SIZE; }
        }

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

        public bool KillCollision(Rectangle body)
        {
            foreach (var rect in KillRects)
            {
                if(body.Intersects(rect))
                    return true;
            }

            return false;
        }

        public bool AboveLadder(GameObject body)
        {
            return body.BoundingRect.Top < ActiveLadder.Top &&
                    body.YRectAlign(ActiveLadder);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var platform in Platforms)
            {
                platform.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch batch, Matrix transform)
        {
            foreach (var platform in Platforms)
                platform.Draw(batch);

            for (int y = 0; y < LevelHeight; y++)
            {
                for (int x = 0; x < LevelWidth; x++)
                {
                    if (IsDrawAble(_tileMap[y,x]))
                    {
                        batch.Draw(
                            _tileSheet,
                            _tileMap[y, x].Position + _tileMap[y, x].Origin,
                            _tileMap[y,x].DrawRect,
                            _tileMap[y, x].ObjectColor,
                            _tileMap[y, x].Rotation,
                            _tileMap[y,x].Origin,
                            _tileMap[y, x].Scale,
                            _tileMap[y, x].Effects,
                            _tileMap[y, x].ZLayer);
                    }
                }
            }
        }

        private bool IsDrawAble(TileObject tile)
        {
            return tile != null && tile.Type != TileType.Empty;
        }

        private TileObject CreateTileFromTokens(int posX, int posY, string[] tokens)
        {
            string tileName = tokens[4].Trim();
            Rectangle draw = new Rectangle();
            if (tileName.Contains("Tile"))
            {
                int tileNumber = Convert.ToInt32(tileName.Replace("Tile ", string.Empty));
                draw = new Rectangle(
                    _tileDrawRects[tileNumber].X,
                    _tileDrawRects[tileNumber].Y,
                    _tileDrawRects[tileNumber].Width,
                    _tileDrawRects[tileNumber].Height);
            }

            Vector2 origin = Vector2.Zero;
            SpriteEffects effects = SpriteEffects.None;
            float rotation = Convert.ToInt32(tokens[3].Trim());
            if (rotation == 1 || rotation == 2)
            {
                if (rotation == 1)
                {
                    effects = SpriteEffects.FlipHorizontally;
                }
                else
                {
                    effects = SpriteEffects.FlipVertically;
                }

                rotation = 0.0f;
            }
            else
            {
                if (rotation > 0) // Move origin to rotate tile around it's center
                {
                    origin.X = TILE_SIZE * 0.5f;
                    origin.Y = TILE_SIZE * 0.5f;
                }

                rotation = MathHelper.ToRadians(rotation);
            }

            TileType type = TileObject.TypeTranslation(tokens[2].Trim());
            if (type == TileType.Player)
            {
                PlayerStart = new Vector2(posX * TILE_SIZE, posY * TILE_SIZE);
                return new TileObject();
            }
            else
            {
                TileObject node = new TileObject(
                    new Vector2(posX * TILE_SIZE, posY * TILE_SIZE), 
                    new Rectangle(posX * TILE_SIZE, posY * TILE_SIZE, TILE_SIZE, TILE_SIZE),
                    draw, 
                    true,
                    type);
                node.Origin = origin;
                node.Effects = effects;
                node.Rotation = rotation;
                return node;
            }
        }

        private Dictionary<int, Rectangle> CreateTileDrawRects(Texture2D tileset, int tileSize, int tileWidth)
        {
            Dictionary<int, Rectangle> result = new Dictionary<int, Rectangle>();
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
                    result.Add(count, rect);
                    count++;
                }
            }

            return result;
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
            if (nodeA == null)
                return false;
            if (nodeA.Coords.X == nodeB.Coords.X)
                return true;
            else
                return false;
        }
    }
}
