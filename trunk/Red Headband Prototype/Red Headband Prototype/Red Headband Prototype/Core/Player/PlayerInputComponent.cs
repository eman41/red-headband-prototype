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
        private TimeSpan _jumpElapsed = TimeSpan.Zero;
        private TimeSpan _jumpFinish = TimeSpan.FromMilliseconds(350);
        private TimeSpan _wallJumpElapsed = TimeSpan.Zero;
        private TimeSpan _wallJumpPropel = TimeSpan.FromMilliseconds(150);
        private TimeSpan _wallJumpFinish = TimeSpan.FromMilliseconds(350);
        private TimeSpan _shieldElapsed = TimeSpan.Zero;
        private TimeSpan _afterHitShield = TimeSpan.FromMilliseconds(750);

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
                resetJumping(player);
                velocity.Y = applyGravity(player, velocity.Y);
            }
            else if (player.GotHit) 
            {
                velocity.X = player.IsFacingLeft ? 1f : -1f;
                velocity.Y = applyGravity(player, velocity.Y);
                resetJumping(player);

                if ((_shieldElapsed += gameTime.ElapsedGameTime) > _afterHitShield)
                {
                    _shieldElapsed = TimeSpan.Zero;
                    player.GotHit = false;
                }
            }
            else
            {
                setPlayerDirection(player, _state.ThumbSticks);
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
                    velocity = pollWallJump(player, velocity.X, gameTime);
                }
                else if (player.IsJumping)
                {
                    velocity = pollJump(player, velocity.X, gameTime);
                }

                attemptJump(player);
                velocity.Y = applyGravity(player, velocity.Y);
            }
            
            // Dead bodies always fall very fast
            player.Velocity = (player.IsAlive && !player.GotHit)
                                ? applySlideCoeff(velocity, player.IsSliding) 
                                : velocity;
        }

        private Vector2 applySlideCoeff(Vector2 velocity, bool isSliding)
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
            if (onTheGround(player)) // Player in a gravity affected state
            {
                return GameMaster.CurrentLevel.Gravity;
            }

            return yVelocity;
        }

        private bool onTheGround(PlayerObject player)
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
                resetJumping(player);
            }
        }

        private Vector2 pollJump(PlayerObject player, float xStart, GameTime gameTime)
        {
            Vector2 result = new Vector2(xStart, PlayerObject.PLAYER_JUMP_SPEED);
            if ((_jumpElapsed += gameTime.ElapsedGameTime) > _jumpFinish)
            {
                _jumpElapsed = TimeSpan.Zero;
                player.IsJumping = false;
            }
            
            return result;
        }

        private Vector2 pollWallJump(PlayerObject player, float xStart, GameTime gameTime)
        {
            Vector2 result = new Vector2(xStart, 0f);

            if (_wallJumpElapsed < _wallJumpPropel)
            {
                if (player.State == _slideStart)
                {
                    result.X = player.State == PlayerState.WallSlideL ? PlayerObject.PLAYER_SPEED : -PlayerObject.PLAYER_SPEED;
                }
                else
                {
                    result.X = 0f;
                }
            }
            else
            {
                if (_state.ThumbSticks.Left.X == 0 && player.State == _slideStart)
                {
                    result.X = player.State == PlayerState.WallSlideL ? PlayerObject.PLAYER_SPEED : -PlayerObject.PLAYER_SPEED;
                }
                else if (player.IsSliding)
                {
                    result.X = 0f;
                }
            }
                
            result.Y = PlayerObject.PLAYER_JUMP_SPEED;
            if ((_wallJumpElapsed += gameTime.ElapsedGameTime) > _wallJumpFinish)
            {
                _wallJumpElapsed = TimeSpan.Zero;
                player.IsWallJumping = false;
            }

            return result;
        }

        private void resetJumping(PlayerObject player)
        {
            player.IsJumping = player.IsWallJumping = false;
            _jumpElapsed = TimeSpan.Zero;
            _wallJumpElapsed = TimeSpan.Zero;
        }

        private void setPlayerDirection(PlayerObject player, GamePadThumbSticks sticks)
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