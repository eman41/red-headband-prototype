// -----------------------------------------------------------------------
// <copyright file="Timer.cs" company="Me" />
// Author: Eric S. Policaro
// Tracks a countdown and resets automatically.
// -----------------------------------------------------------------------
namespace TileEngine.Engine
{
    using System;

    public class Timer : IResetable
    {
        private TimeSpan _timeLimit;
        private TimeSpan _currentTime;

        /// <summary>
        /// Initializes a new Timer object.
        /// </summary>
        /// <param name="duration">Amount of time before timer is reset.</param>
        public Timer(TimeSpan duration)
        {
            _timeLimit = duration;
            _currentTime = TimeSpan.Zero;
        }

        /// <summary>
        /// Reset the Timer back to zero.
        /// </summary>
        public void Reset()
        {
            _currentTime = TimeSpan.Zero;
        }

        /// <summary>
        /// Check if the timer has reached at least the given time.
        /// </summary>
        public bool HasReached(TimeSpan time)
        {
            return _currentTime >= time;
        }

        /// <summary>
        /// Advance the timer by the given timespan.
        /// </summary>
        /// <param name="inc">Amount of time to advance.</param>
        /// <returns>True: duration exceeded, resets to zero.</returns>
        public bool AdvanceTimerCyclic(TimeSpan inc)
        {
            _currentTime += inc;
            if (_currentTime >= _timeLimit)
            {
                _currentTime = TimeSpan.Zero;
                return true;
            }
            return false;
        }
    }
}
