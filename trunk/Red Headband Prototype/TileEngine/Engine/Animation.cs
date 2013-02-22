namespace TileEngine.Engine
{
    using System;
    using Microsoft.Xna.Framework;

    public class Animation
    {
        private TimeSpan _elapsed;
        private TimeSpan _interval;
        private Rectangle[] _frames;
        private bool _oneShot = false;
        private bool _stopAnimation = false;

        public Animation(string name, Rectangle firstRect, int frames, int spacing, TimeSpan interval, bool oneShot = false)
        {
            _elapsed = TimeSpan.Zero;
            _frames = new Rectangle[frames];
            _interval = interval;
            Name = name;
            CurrentFrame = 0;
            TotalFrames = frames;
            _oneShot = oneShot;
            if (frames > 1)
            {
                for (int i = 0; i < frames; i++)
                {
                    _frames[i] = new Rectangle(
                        firstRect.X + (firstRect.Width + spacing) * i,
                        firstRect.Y,
                        firstRect.Width,
                        firstRect.Height);
                }
            }
            else
            {
                Frames[0] = firstRect;
            }
        }

        public int CurrentFrame { get; set; }
        public int TotalFrames { get; set; }
        public string Name { get; set; }

        public Rectangle[] Frames 
        {
            get
            {
                return _frames;
            }
        }

        public Rectangle CurrentClip
        {
            get
            {
                return _frames[CurrentFrame];
            }
        }

        public void Update(GameTime gameTime)
        {
            _elapsed += gameTime.ElapsedGameTime;
            if (AdvanceFrame())
            {
                CurrentFrame++;
                _elapsed = TimeSpan.Zero;
                if (CurrentFrame == TotalFrames)
                {
                    if (_oneShot)
                    {
                        _stopAnimation = true;
                        CurrentFrame--;
                    }
                    else
                    {
                        CurrentFrame = 0;
                    }
                }
            }
        }

        public void ResetAnimation()
        {
            _elapsed = TimeSpan.Zero;
            CurrentFrame = 0;
            _stopAnimation = false;
        }

        private bool AdvanceFrame()
        {
            return (TotalFrames > 1) && (_elapsed > _interval) && !_stopAnimation;
        }
    }
}
