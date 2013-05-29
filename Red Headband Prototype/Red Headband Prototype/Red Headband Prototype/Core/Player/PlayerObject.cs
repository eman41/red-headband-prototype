// -----------------------------------------------------------------------
// PlayerObject.cs: The Player
// Author: Eric S. Policaro
// -----------------------------------------------------------------------
namespace Red_Headband_Prototype.Core
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using TileEngine.Engine;

    /// <summary>
    /// Class used to represent a player.
    /// </summary>
    public sealed class PlayerObject : GameObject, IResetable
    {
        public const int PLAYER_WIDTH = 21;
        public const int PLAYER_HEIGHT = 24;
        public const float PLAYER_SPEED = 4f;
        public const float PLAYER_JUMP_SPEED = -6f;
        public const int PLAYER_MAX_HP = 300;

        private int _currentHP = PLAYER_MAX_HP;
        private IInput _input;
        private IPhysics _physics;
        private PlayerAnimation _animation;

        /// <summary>
        /// Creates a new player with the given configuration.
        /// </summary>
        public PlayerObject(Vector2 pos, Vector2 velocity, PlayerIndex player, 
            IInput input, IPhysics physics, PlayerAnimation animation)
            : base(pos, new Rectangle((int)pos.X, (int)pos.Y, PLAYER_WIDTH, PLAYER_HEIGHT), 
                   velocity, true)
        {
            _input = input;
            _physics = physics;
            _animation = animation;
            PlayerIdx = player;
            IsAlive = true;
        }

        public bool IsAlive { get; set; }
        public bool IsFacingLeft { get; set; }
        public bool OnLadder { get; set; }
        public bool OnFloor { get; set; }
        public bool OnPlatform { get; set; }
        public bool IsSliding { get; set; }
        public bool IsJumping { get; set; }
        public bool IsWallJumping { get; set; }
        public bool IsShooting { get; set; }
        public bool GotHit { get; set; }
        public PlayerState State { get; set; }
        public PlayerIndex PlayerIdx { get; set; } 

        /// <summary>
        /// Gets or sets the horizontal direction the player is moving.
        /// </summary>
        public Direction XDirection { get; set; }

        /// <summary>
        /// Gets or sets the vertical direction the player is moving.
        /// </summary>
        public Direction YDirection { get; set; }

        public int CurrentHelth 
        { 
            get { return _currentHP; } 
        }

        /// <summary>
        /// Update the player.
        /// </summary>
        /// <param name="gameTime">Game time snapshot</param>
        public void Update(GameTime gameTime)
        {
            _input.Update(this, gameTime);
            _physics.Update(this, GameMaster.CurrentLevel);
            _animation.Update(this, gameTime);
        }

        /// <summary>
        /// Adjust the player's health. 
        /// A player that is hit will be shielded for 0.75s.
        /// </summary>
        /// <param name="amount">Amount to change the player's health, must be positive.</param>
        public void AdjustHealthFixed(int amount)
        {
            // Don't allow hits during the shield
            if (amount >= 0 || NotShielded(amount))
            {
                _currentHP += amount;    
            }

            _currentHP = Math.Min(PLAYER_MAX_HP, _currentHP);
            if (_currentHP == 0)
            {
                IsAlive = false;
            }
        }

        private bool NotShielded(int amount)
        {
            return (amount < 0 && !GotHit);
        }

        public void KillPlayer()
        {
            AdjustHealthFixed(-PLAYER_MAX_HP);
        }

        /// <summary>
        /// Brings player back to life.
        /// </summary>
        public void Reset()
        {
            IsAlive = true;
            GotHit = false;
            Active = true;
            _currentHP = PLAYER_MAX_HP;
            OnFloor = true;
            _animation.ResetAnimations();
        }

        /// <summary>
        /// Draw the player.
        /// </summary>
        /// <param name="batch">Sprite batch used to draw</param>
        /// <param name="transform"></param>
        public void Draw(SpriteBatch batch)
        {
            if (Active)
            {
                Vector2 drawLoc = Position + Origin;
                batch.Draw(_animation.Sheet, drawLoc, _animation.ActiveAnimation.CurrentClip,
                    ObjectColor, Rotation, Origin, Scale, Effects, ZLayer);
            }
        }
    }

    public enum PlayerState
    {
        Stand, StandShoot, Run, RunShoot, Jump, JumpShoot, 
        WallSlideL, WallSlideR, WallSlideShoot, 
        Climb, ClimbShoot, Climbing, ClimbFinish, Hit, Death
    }
}
