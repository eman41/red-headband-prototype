// -----------------------------------------------------------------------
// <copyright file="GameMaster.cs" company="Me">
// Author: Eric S. Policaro
// </copyright>
// -----------------------------------------------------------------------
namespace Red_Headband_Prototype.Core
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Red_Headband_Prototype.Core.GUI;
    using TileEngine.Engine;
    using TileEngine.Engine.Platforms;
    using Red_Headband_Prototype.Levels.Logic;

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameMaster : Microsoft.Xna.Framework.Game
    {
        /// <summary>Viewport width in tiles. Screen res width. </summary>
        public const int VIEWPORT_WIDTH_TILE = 60;

        /// <summary>Viewport height in tiles. Screen res height.</summary>
        public const int VIEWPORT_HEIGHT_TILE = 33;

        /// <summary>Viewport rotation factor (degrees)</summary>
        public const float ROTATION = 0.0f;

        /// <summary>Viewport zoom amount</summary>
        public const float ZOOM_FACTOR = 2.0f;

        /// <summary>Graphics device service</summary>
        private GraphicsDeviceManager _graphics;

        /// <summary>Sprite Drawing</summary>
        private SpriteBatch _spriteBatch;

        /// <summary>Player data reference</summary>
        private static PlayerObject _playerOne;

        /// <summary>Level data reference</summary>
        private static ILevel _level;

        /// <summary>Displays the hud gui.</summary>
        private HudComponent _hudComponent;

        private Dictionary<string, ILevel> _levelCache = new Dictionary<string,ILevel>(); 

        SpriteFont debugFont;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameMaster" /> class.
        /// </summary>
        public GameMaster()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = VIEWPORT_WIDTH_TILE * GameMap.TILE_SIZE;
            _graphics.PreferredBackBufferHeight = VIEWPORT_HEIGHT_TILE * GameMap.TILE_SIZE;
            Content.RootDirectory = "Content";
            this.IsFixedTimeStep = false;
        }

        /// <summary>Camera object</summary>
        private Camera2D _activeCamera  = new Camera2D();

        public static GameMap CurrentLevel
        {
            get
            {
                return _level.GetGameMap();
            }
        }

        /// <summary>
        /// Gets the real pixel width of the viewport
        /// </summary>
        public static int RealViewWidth
        {
            get
            {
                return VIEWPORT_WIDTH_TILE * GameMap.TILE_SIZE;
            }
        }

        /// <summary>
        /// Gets the real pixel height of the viewport
        /// </summary>
        public static int RealViewHeight
        {
            get
            {
                return VIEWPORT_HEIGHT_TILE * GameMap.TILE_SIZE;
            }
        }

        public static Point PlayerAt
        {
            get { return _playerOne.Coords; }
        }

        private string[] _knownLevels = { "LevelN" };

        /// <summary>
        /// bananas foster
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            debugFont = Content.Load<SpriteFont>("Fonts/Debug");

            GameMap levelNMap = new GameMap("LevelN", @"Levels\LevelN.csv", Content.Load<Texture2D>("TileSets/world-tiles-vert"));
            levelNMap.PlatformTextures = Content.Load<Texture2D>("Tilesets/platform-sheet");
            _levelCache.Add("LevelN", new LevelN(levelNMap));

            LoadLevel("LevelN", _playerOne);
        }

        private void LoadLevel(string levelName, PlayerObject player)
        {
            _level = _levelCache[levelName];

            if (player == null)
            {
                _playerOne = new PlayerObject(
                    Vector2.Zero,
                    Vector2.Zero,
                    PlayerIndex.One,
                    new PlayerInputComponent(),
                    new PlayerPhysics(CurrentLevel),
                    new PlayerAnimation(this, "XML/PlayerSpriteData.xml"));

                player = _playerOne;
            }
            else
            {
                _playerOne.Reset();
            }
            
            _levelCache[levelName].Scripting(player, ref _activeCamera);

            if (_hudComponent == null)
            {
                _hudComponent = new HudComponent(this, "XML/HudData.xml", _playerOne);
            }
            else
            {
                _hudComponent.Reset();
            }

            _playerOne.Position = CurrentLevel.PlayerStart;
            GC.Collect(); // Force a GC just because
        }

        private GamePadState _lastState;

        /// <summary>
        /// Controls pause state, blocks simulation.
        /// </summary>
        private bool _paused = false;
        private TimeSpan _pauseDelay = TimeSpan.Zero;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            GamePadState currentState = GamePad.GetState(PlayerIndex.One);

            // Allows the game to exit
            if (currentState.Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

            CheckPause(currentState, gameTime);

            if (!_playerOne.IsAlive && InputUtility.ButtonToggled(_lastState, currentState, Buttons.Y)) // For hit and death testing
            {
                LoadLevel(CurrentLevel.LevelName, _playerOne);
            }

            if (!_paused)
            {
                _playerOne.Update(gameTime);
                CurrentLevel.Update(gameTime);
                if (playerBelowScreen())
                {
                    _playerOne.KillPlayer();
                    _activeCamera.LockStatus = CameraLock.Both;
                }
                _hudComponent.Update();

                UpdateCamera(_activeCamera, _playerOne.Position);
                _lastState = GamePad.GetState(PlayerIndex.One);
                base.Update(gameTime);
            }
        }

        private static bool playerBelowScreen()
        {
            return _playerOne.Position.Y > CurrentLevel.RealLevelHeight;
        }

        private void CheckPause(GamePadState currentState, GameTime gameTime)
        {
            if (canChangePause())
            {
                togglePause(currentState, gameTime);
            }
            else
            {
                updatePauseDelay(currentState, gameTime);
            }
        }

        private void togglePause(GamePadState currentState, GameTime gameTime)
        {
            if (InputUtility.ButtonToggled(_lastState, currentState, Buttons.Start))
            {
                _paused = !_paused;
                _pauseDelay += gameTime.ElapsedGameTime;
            }
        }

        private void updatePauseDelay(GamePadState currentState, GameTime gameTime)
        {
            _pauseDelay += (currentState.Buttons.Start == ButtonState.Released)
                            ? gameTime.ElapsedGameTime
                            : TimeSpan.Zero;
            tryResetPause();
        }

        private void tryResetPause()
        {
            if (_pauseDelay > TimeSpan.FromMilliseconds(50))
            {
                _pauseDelay = TimeSpan.Zero;
            }
        }

        private bool canChangePause()
        {
            return _pauseDelay == TimeSpan.Zero;
        }

        private void UpdateCamera(Camera2D camera, Vector2 playerPos)
        {
            if (camera.hasHoldPositions() && (playerPos.X > nextHoldPosition(camera)))
            {
                int holdFactor = createHoldFactor(camera);
                camera.Limits = new Rectangle(
                    holdFactor, 0, 
                    (int)CurrentLevel.RealLevelWidth - holdFactor, 
                    (int)CurrentLevel.RealLevelHeight);
                
            }

            camera.FocusVector = playerPos;
        }

        private static int createHoldFactor(Camera2D camera)
        {
            return camera.HoldPositions.Dequeue() * GameMap.TILE_SIZE;
        }

        private static int nextHoldPosition(Camera2D camera)
        {
            return (camera.HoldPositions.Peek() + GameMaster.VIEWPORT_WIDTH_TILE / 4) 
                    * GameMap.TILE_SIZE;
        }
        

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.Clear(CurrentLevel.BackgroundColor);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, _activeCamera.Transform);
                CurrentLevel.Draw(_spriteBatch, _activeCamera.Transform);
                _playerOne.Draw(_spriteBatch, _activeCamera.Transform);
            _spriteBatch.End();
            
            _spriteBatch.Begin();
            _hudComponent.Draw(gameTime, _spriteBatch);
            _spriteBatch.DrawString(debugFont, playerStatsString(), new Vector2(5, 5), Color.Black);
            _spriteBatch.End();
        }

        private static string playerStatsString()
        {
            return _playerOne.Velocity.ToString() + "\r\n" + _playerOne.Position;
        }
    }
}
