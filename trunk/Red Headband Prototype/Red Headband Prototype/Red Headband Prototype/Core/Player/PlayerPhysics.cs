// -----------------------------------------------------------------------
// PlayerPhysics.cs: Physics rules for player movement.
// Author: Eric S. Policaro
// -----------------------------------------------------------------------
namespace Red_Headband_Prototype.Core
{
    using System;
    using Microsoft.Xna.Framework;
    using TileEngine.Engine;
    using TileEngine.Collision;
    using Red_Headband_Prototype.Core.Interfaces;

    /// <summary>
    /// Implementation of player movement and collision rules. Standard
    /// platforming for jumping, falling and wall-sliding.
    /// </summary>
    public class PlayerPhysics : IPhysics
    {
        private Collision _collision;
        private const float SLIDE_COEFF = 0.4f;
        private const float CLIMB_COEFF = 0.5f;
        private const int SCAN_DISTANCE = 2;

        /// <summary>
        /// Creates a new PlayerPhysics class.
        /// </summary>
        /// <param name="level">Current level</param>
        public PlayerPhysics(GameMap level)
        {
            _collision = new Collision(level);
        }

        /// <summary>
        /// Update the players position based on the implemented physics rules.
        /// </summary>
        /// <param name="player">Player to update</param>
        /// <param name="level">Current level</param>
        public void Update(PlayerObject player, GameMap level)
        {
            if (_collision.MapLevel.KillCollision(player.BoundingRect))
                player.KillPlayer();

            if (!player.IsAlive)
            {
                DeathRules(player);
            }
            else if (player.OnLadder)
            {
                LadderRules(player);
            }
            else
            {
                NormalRules(player);   
            }
            
            _collision.UpdateScanBounds(player.Coords, SCAN_DISTANCE);
        }

        private void DeathRules(PlayerObject player)
        {
            player.IsSliding = false;
            player.OnLadder = false;
            player.OnFloor = CheckFooting(player);
        }

        private void NormalRules(PlayerObject player)
        {
            // Move and resolve X Axis
            player.Position.X += (float)Math.Round(player.Velocity.X, 0);
            Colliding xColliding = _collision.CheckLevelCollision(player, Axis.X_AXIS);
            CheckPlatforms(player, Axis.X_AXIS);

            // Update movement states
            if (player.IsAlive)
            {
                player.IsSliding = CheckSliding(player, xColliding);
                player.OnFloor = CheckFooting(player);
                player.OnPlatform = CheckPlatforms(player, Axis.Y_AXIS);
                player.OnLadder = CheckEnterLadder(player);
            }
        }

        // Rules for entering and exiting ladders
        private void LadderRules(PlayerObject player)
        {
            player.Position.X += (float)Math.Round(player.Velocity.X * CLIMB_COEFF, 0);
            _collision.CheckLevelCollision(player, Axis.X_AXIS);

            player.Position.Y += (float)Math.Round(player.Velocity.Y * CLIMB_COEFF);
            if (_collision.CheckLevelCollision(player, Axis.Y_AXIS) == Colliding.FromTop || !_collision.MapLevel.LadderCollision(player.BoundingRect))
            {
                player.OnLadder = false;
            }

            player.OnPlatform = CheckPlatforms(player, Axis.Y_AXIS);
        }

        // Detect if the player is standing on solid ground/platform/ladder
        private bool CheckFooting(PlayerObject player)
        {
            bool result = false;
            player.Position.Y += (float)Math.Round(player.Velocity.Y, 0);

            Colliding falling = _collision.CheckLevelCollision(player, Axis.Y_AXIS);
            if (falling == Colliding.FromTop)
            {
                player.Velocity.Y = 0f;
                result = true;
            }
            else if (falling == Colliding.FromBottom)
            {
                // Player is crushed by a platform
                CrushPlayer(player);
            }

            foreach (Rectangle rect in _collision.MapLevel.LadderBounds)
            {
                if (player.Collision(rect) && player.BoundingRect.Top < rect.Top && player.Velocity.Y > 0)
                {
                    if (_collision.DetectAndResolveCollision(player, rect, Axis.Y_AXIS) == Colliding.FromTop)
                    {
                        player.Velocity.Y = 0f;
                        result =  true;
                    }
                }
            }

            return result;
        }

        // Check for collisions against platforms: moving and killing
        private bool CheckPlatforms(PlayerObject player, Axis axis)
        {
            Colliding collide = _collision.CheckPlatformCollision(player, axis);

            if (collide != Colliding.None)
            {
                if (_collision.LastPlatform is IKillRect)
                {
                    IKillRect killPlat = (IKillRect)_collision.LastPlatform;
                    collide = (axis == Axis.X_AXIS)
                        ? _collision.XDirection(player.BoundingRect, _collision.LastPlatform.BoundingRect) 
                        : _collision.YDirection(player.BoundingRect, _collision.LastPlatform.BoundingRect);
                    if ((killPlat.GetKillDirection() & collide) == collide)
                    {
                        player.KillPlayer();
                    }
                }

                if (player.IsAlive)
                {
                    _collision.ResolveCollision(player, _collision.LastPlatform.BoundingRect, collide);
                    ResolvePlatformCollision(player, collide);
                }
            }

            return false;
        }

        private bool ResolvePlatformCollision(PlayerObject player, Colliding collide) 
        {
            switch (collide)
            {
                case Colliding.FromRight:
                case Colliding.FromLeft:
                    return true;

                case Colliding.FromTop:
                    _collision.LastPlatform.WakeUp();
                    AlignPlayerToPlatform(player);
                    return true;

                // Player is crushed by a platform
                case Colliding.FromBottom:
                    CrushPlayer(player);
                    return false;
            }
            return false;
        }

        private void AlignPlayerToPlatform(PlayerObject player) 
        {
            player.Velocity = _collision.LastPlatform.Velocity;
            player.Position += player.Velocity;
            player.OnFloor = false;
            player.OnLadder = false;
        }

        private void CrushPlayer(PlayerObject player)
        {
            player.IsJumping = false;
            player.IsWallJumping = false;
            player.OnLadder = false;
            if (player.OnFloor)
            {
                player.KillPlayer();
            }
        }

        private bool CheckEnterLadder(PlayerObject player)
        {
            if (IsClimbingUp(player))
            {
                return true;
            }
            else if (player.YDirection == Direction.Down)
            {
                Rectangle temp = player.BoundingRect;
                temp.Y += (int)PlayerObject.PLAYER_SPEED; // Check if we can enter a ladder here
                if (IsClimbingDown(player, ref temp))
                {
                    player.Position.Y += PlayerObject.PLAYER_SPEED;
                    return true;
                }
            }

            return false;
        }

        private bool IsClimbingDown(PlayerObject player, ref Rectangle temp)
        {
            return _collision.MapLevel.LadderCollision(temp) && _collision.MapLevel.AboveLadder(player);
        }

        private bool IsClimbingUp(PlayerObject player)
        {
            return player.YDirection == Direction.Up && _collision.MapLevel.LadderCollision(player.BoundingRect);
        }

        private bool CheckSliding(PlayerObject player, Colliding xAxis)
        {
            Rectangle leftTemp = new Rectangle(
                (int)player.Position.X - 1, 
                (int)player.Position.Y, 
                player.BoundingRect.Width, 
                player.BoundingRect.Height);
            Rectangle rightTemp = new Rectangle(
                (int)player.Position.X + 1, 
                (int)player.Position.Y, 
                player.BoundingRect.Width, 
                player.BoundingRect.Height);
            
            if (_collision.CheckLevelCollision(leftTemp, Axis.X_AXIS) == Colliding.FromRight)
            {
                if (!player.OnFloor)
                {
                    player.IsFacingLeft = true;
                }
                player.State = PlayerState.WallSlideL;
                return true;
            }
            else if (_collision.CheckLevelCollision(rightTemp, Axis.X_AXIS) == Colliding.FromLeft)
            {
                if (!player.OnFloor)
                {
                    player.IsFacingLeft = false;
                }
                player.State = PlayerState.WallSlideR;
                return true;
            }

            return false;
        }
    }
}