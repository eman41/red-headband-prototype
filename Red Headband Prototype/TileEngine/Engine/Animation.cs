namespace TileEngine.Engine
{
    using System;
    using Microsoft.Xna.Framework;

    public class Animation : IResetable
    {
        private int _totalFrames;
        private int _currentFrame;
        private Rectangle[] _frames;

        private Timer _animTimer;
        private bool _isOneShot = false;
        private bool _isAnimStopped = false;

        public Animation(string name, Rectangle firstRect, int frames, int spacing, TimeSpan interval, bool oneShot = false)
        {
            _frames = new Rectangle[frames];
            _animTimer = new Timer(interval);
            Name = name;
            _currentFrame = 0;
            _totalFrames = frames;
            _isOneShot = oneShot;
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
                _frames[0] = firstRect;
            }
        }

        public string Name { get; private set; }

        public Rectangle CurrentClip
        {
            get
            {
                return _frames[_currentFrame];
            }
        }

        public void Update(GameTime gameTime)
        {
            bool isTimeUp = _animTimer.AdvanceTimerCyclic(gameTime.ElapsedGameTime);
            if (AdvanceFrame(isTimeUp))
            {
                _currentFrame++;
                _animTimer.Reset();
                if (_currentFrame == _totalFrames)
                {
                    if (_isOneShot)
                    {
                        _isAnimStopped = true;
                        _currentFrame--;
                    }
                    else
                    {
                        _currentFrame = 0;
                    }
                }
            }
        }

        public void Reset()
        {
            _animTimer.Reset();
            _currentFrame = 0;
            _isAnimStopped = false;
        }

        private bool AdvanceFrame(bool isTimeUp)
        {
            return (_totalFrames > 1) && !_isAnimStopped && isTimeUp;
        }
    }
}
