namespace Red_Headband_Prototype.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Text;
    using TileEngine.Engine;

    public class PlayerAnimation : GameComponent
    {
        private Texture2D _sheet;
        private Dictionary<PlayerState, Animation> _animations;

        public PlayerAnimation(Game game, string dataPath)
            : base(game)
        {
            _animations = new Dictionary<PlayerState, Animation>();
            LoadAssets(dataPath);
        }

        public Texture2D Sheet
        {
            get
            {
                return _sheet;
            }
        }

        public Animation ActiveAnimation { get; set; }

        public void Update(PlayerObject player, GameTime gameTime)
        {
            PlayerState state = PlayerState.Stand;
            int shoot = player.IsShooting ? 1 : 0;

            if (player.OnLadder)
            {
                if (player.BoundingRect.Center.Y < GameMaster.CurrentLevel.ActiveLadder.Top)
                {
                    state = PlayerState.ClimbFinish;
                }
                else if (player.InMotion)
                {
                    state = PlayerState.Climbing;
                }
                else
                {
                    state = PlayerState.Climb + shoot;
                }
            }
            else if (player.OnFloor || player.OnPlatform)
            {
                if (player.XDirection != Direction.None)
                {
                    state = PlayerState.Run + shoot;
                }
                else
                {
                    state = PlayerState.Stand + shoot;
                }
            }
            else if (player.IsSliding)
            {
                state = PlayerState.WallSlideR + shoot;
            }
            else if (player.IsJumping || !player.OnFloor)
            {
                state = PlayerState.Jump + shoot;
            }
            else
            {
                state = PlayerState.Stand + shoot;
            }

            if (player.GotHit)
            {
                state = PlayerState.Hit;
            }

            if (!player.IsAlive)
            {
                state = PlayerState.Death;
            }

            player.Effects = player.IsFacingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            ActiveAnimation = _animations[state];
            ActiveAnimation.Update(gameTime);
        }

        public void ResetAnimations()
        {
            foreach (var animation in _animations)
            {
                animation.Value.Reset();
            }
        }

        private void LoadAssets(string datapath)
        {
            XElement decoded = XElement.Load(datapath);

            var assetPath = from p in decoded.Elements("sheet")
                            select (string)p.Attribute("path");
            _sheet = Game.Content.Load<Texture2D>(assetPath.ElementAt<string>(0));

            var animlist = from p in decoded.Elements("animation")
                           select new Animation(
                                (string)p.Attribute("name"),
                                new Rectangle(
                                    (int)p.Element("rectangle").Attribute("x"),
                                    (int)p.Element("rectangle").Attribute("y"),
                                    (int)p.Element("rectangle").Attribute("w"),
                                    (int)p.Element("rectangle").Attribute("h")),
                                (int)p.Element("frames"),
                                (int)p.Element("spacing"),
                                TimeSpan.FromMilliseconds((int)p.Element("interval")),
                                (bool)p.Attribute("oneshot")
                              );

            foreach (var anim in animlist)
            {
                _animations.Add(TypeTranslation(anim.Name), anim);
            }
        }

        public static PlayerState TypeTranslation(string name)
        {
            switch (name)
            {
                case "stand":
                    return PlayerState.Stand;
                case "stand-shoot":
                    return PlayerState.StandShoot;
                case "run":
                    return PlayerState.Run;
                case "run-shoot":
                    return PlayerState.RunShoot;
                case "jump":
                    return PlayerState.Jump;
                case "jump-shoot":
                    return PlayerState.JumpShoot;
                case "wallslide":
                    return PlayerState.WallSlideR;
                case "wallslide-shoot":
                    return PlayerState.WallSlideShoot;
                case "climb":
                    return PlayerState.Climb;
                case "climbing":
                    return PlayerState.Climbing;
                case "climb-shoot":
                    return PlayerState.ClimbShoot;
                case "climb-finish":
                    return PlayerState.ClimbFinish;
                case "hit":
                    return PlayerState.Hit;
                case "death":
                    return PlayerState.Death;
                default:
                    return PlayerState.Stand;
            }
        }
    }
}