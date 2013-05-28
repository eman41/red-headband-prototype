// -----------------------------------------------------------------------
// Animation.cs: Animation related objects.
// Author: Eric S. Policaro
// -----------------------------------------------------------------------
namespace TileEngine.Engine
{
    using System;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Class used to handle an animation.
    /// </summary>
    public class Animation : IResetable
    {
        private int _totalFrames;
        private int _currentFrame;
        private Rectangle[] _frames;

        private Timer _animTimer;
        private bool _isOneShot = false;
        private bool _isAnimStopped = false;

        /// <summary>
        /// Create a new Animation sequence.
        /// </summary>
        /// <param name="name">Name of the animation</param>
        /// <param name="firstRect">Rectangle of the first animation frame</param>
        /// <param name="frames">Number of frames</param>
        /// <param name="spacing">Pixel space between each frame</param>
        /// <param name="interval">Animation speed</param>
        /// <param name="oneShot">True: Animation plays once, False: plays repeatedly</param>
        public Animation(string name, Rectangle firstRect, int frames, int spacing, TimeSpan interval, bool oneShot = false)
        {
            Name = name;
            _frames = new Rectangle[frames];
            _animTimer = new Timer(interval);
            _currentFrame = 0;
            _totalFrames = frames;
            _isOneShot = oneShot;
            firstRect = InitAnimationFrames(firstRect, frames, spacing);
        }

        private Rectangle InitAnimationFrames(Rectangle firstRect, int frames, int spacing)
        {
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
            return firstRect;
        }

        /// <summary>
        /// Gets the animation name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Get the current animation frame.
        /// </summary>
        public Rectangle CurrentClip
        {
            get
            {
                return _frames[_currentFrame];
            }
        }

        /// <summary>
        /// Advance the animation if still active.
        /// </summary>
        /// <param name="gameTime">Game time snapshot</param>
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

        /// <summary>
        /// Reset the animation to its starting state.
        /// </summary>
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
