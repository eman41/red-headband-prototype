// -----------------------------------------------------------------------
// <copyright file="PlayerInputComponent.cs" company="Me" />
// Author: Eric S. Policaro
// Handles 
// -----------------------------------------------------------------------
namespace Red_Headband_Prototype.Core
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using TileEngine.Engine;

    public class PlayerInputComponent : IInput
    {
        private GamePadState _state;
        private GamePadState _lastState;
        private PlayerState _slideStart;
        
        private TimeSpan _wallJumpPropel = TimeSpan.FromMilliseconds(150);
        private Timer _wallJumpTimer = new Timer(TimeSpan.FromMilliseconds(350));
        private Timer _jumpTimer = new Timer(TimeSpan.FromMilliseconds(350));
        private Timer _shieldTimer = new Timer(TimeSpan.FromMilliseconds(750));

        private const float SLIDE_UP_COEFF = 0.6f;
        private const float SLIDE_DOWN_COEFF = 0.8f;

        /// <summary>Minimum amount to detect thumb-stick motion</summary>
        private const float STICK_THRESHOLD = 0.8f;

        public void Update(PlayerObject player, GameTime gameTime)
        {
            _lastState = _state;
            _state = GamePad.GetState(player.PlayerIdx, GamePadDeadZone.Circular);
            Vector2 velocity = Vector2.Zero;
            player.IsShooting = _state.IsButtonDown(Buttons.A);

            if (!player.IsAlive)
            {
                ResetJumping(player);
                velocity.Y = applyGravity(player, velocity.Y);
            }
            else if (player.GotHit) 
            {
                velocity.X = player.IsFacingLeft ? 1f : -1f;
                velocity.Y = applyGravity(player, velocity.Y);
                ResetJumping(player);
                player.GotHit = _shieldTimer.AdvanceTimerCyclic(gameTime.ElapsedGameTime);
            }
            else
            {
                SetPlayerDirection(player, _state.ThumbSticks);
                if (Math.Abs(_state.ThumbSticks.Left.X) > STICK_THRESHOLD)
                {
                    velocity.X = PlayerObject.PLAYER_SPEED * _state.ThumbSticks.Left.X;
                }

                if (Math.Abs(_state.ThumbSticks.Left.Y) > STICK_THRESHOLD)
                {
                    velocity.Y = -PlayerObject.PLAYER_SPEED * _state.ThumbSticks.Left.Y;
                }

                if (player.IsWallJumping)
                {
                    velocity = PollWallJump(player, velocity.X, gameTime);
                }
                else if (player.IsJumping)
                {
                    velocity = PollJump(player, velocity.X, gameTime);
                }

                attemptJump(player);
                velocity.Y = applyGravity(player, velocity.Y);
            }
            
            // Dead bodies always fall very fast
            player.Velocity = (player.IsAlive && !player.GotHit)
                                ? ApplySlideCoeff(velocity, player.IsSliding) 
                                : velocity;
        }

        private Vector2 ApplySlideCoeff(Vector2 velocity, bool isSliding)
        {
            if (isSliding)
            {
                velocity.Y = (velocity.Y > 0)
                                ? velocity.Y * SLIDE_DOWN_COEFF 
                                : velocity.Y * SLIDE_UP_COEFF;
            }
            
            return velocity;
        }

        private float applyGravity(PlayerObject player, float yVelocity)
        {
            if (OnTheGround(player)) // Player in a gravity-affected state
            {
                return GameMaster.CurrentLevel.Gravity;
            }

            return yVelocity;
        }

        private bool OnTheGround(PlayerObject player)
        {
            return !player.IsJumping && !player.OnLadder && !player.IsWallJumping;
        }

        private void attemptJump(PlayerObject player)
        {
            if (!player.OnLadder && _state.IsButtonDown(Buttons.B))
            {
                if (_lastState.IsButtonUp(Buttons.B)) // Prevents a double jump
                {
                    if (player.OnFloor || player.OnPlatform)
                    {
                        player.IsJumping = true;
                    }
                    else if (player.IsSliding)
                    {
                        player.IsSliding = false;
                        player.IsWallJumping = true;
                        _slideStart = player.State;
                    }
                }
            }
            else
            {
                ResetJumping(player);
            }
        }

        private Vector2 PollJump(PlayerObject player, float xStart, GameTime gameTime)
        {
            Vector2 result = new Vector2(xStart, PlayerObject.PLAYER_JUMP_SPEED);
            player.IsJumping = !_jumpTimer.AdvanceTimerCyclic(gameTime.ElapsedGameTime);
            return result;
        }

        private Vector2 PollWallJump(PlayerObject player, float xStart, GameTime gameTime)
        {
            Vector2 result = new Vector2();
            result.X = GetWallJumpX(player.State, player.IsSliding, xStart);
            result.Y = PlayerObject.PLAYER_JUMP_SPEED;

            player.IsWallJumping = AdvanceWallJumpTimer(gameTime);
            return result;
        }

        private float GetWallJumpX(PlayerState state, bool sliding, float start)
        {
            if (WallJumpPropeling())
            {
                if (state == _slideStart)
                {
                    return GetSlideSpeed(state);
                }
                else if (sliding)
                {
                    return 0f;
                }
            }
            return start;
        }

        private bool WallJumpPropeling()
        {
            return !_wallJumpTimer.HasReached(_wallJumpPropel);
        }

        private float GetSlideSpeed(PlayerState state)
        {
            return (state == PlayerState.WallSlideL)
                        ? PlayerObject.PLAYER_SPEED
                        : -PlayerObject.PLAYER_SPEED;
        }

        private bool AdvanceWallJumpTimer(GameTime gameTime)
        {
            return !_wallJumpTimer.AdvanceTimerCyclic(gameTime.ElapsedGameTime);
        }

        private void ResetJumping(PlayerObject player)
        {
            player.IsJumping = player.IsWallJumping = false;
            _jumpTimer.Reset();
            _wallJumpTimer.Reset();
        }

        private void SetPlayerDirection(PlayerObject player, GamePadThumbSticks sticks)
        {
            if (sticks.Left.X > STICK_THRESHOLD)
            {
                player.XDirection = Direction.Right;
                player.IsFacingLeft = false;
            }
            else if (sticks.Left.X < -STICK_THRESHOLD)
            {
                player.XDirection = Direction.Left;
                player.IsFacingLeft = true;
            }
            else
            {
                player.XDirection = Direction.None;
            }

            if (sticks.Left.Y > STICK_THRESHOLD) 
            {
                player.YDirection = Direction.Up;
            }
            else if (sticks.Left.Y < -STICK_THRESHOLD)
            {
                player.YDirection = Direction.Down;
            }
            else
            {
                player.YDirection = Direction.None;
            }
        }
    }

    public enum Direction
    {
        None, Left, Right, Up, Down
    }
}