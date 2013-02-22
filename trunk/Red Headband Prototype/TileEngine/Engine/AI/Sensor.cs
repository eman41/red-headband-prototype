namespace TileEngine.Engine.AI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;

    public class Sensor
    {
        /// <summary>
        /// The game object the sensor is looking for.
        /// </summary>
        private GameObject _subject;

        /// <summary>
        /// Actual tile location of the sensor
        /// </summary>
        private Point _origin;

        /// <summary>
        /// Tiles to expand origin detection. 
        /// ie. a bias of (5,0) would detect 5 tiles left and right of the origin.
        /// </summary>
        private Point _bias;

        /// <summary>
        /// The directions this sensor is currently detecting in.
        /// </summary>
        public Facing _directionFlags;

        public Sensor(GameObject obj, Point origin, Point bias, Facing detection)
        {
            _subject = obj;
            _origin = origin;
            _bias = new Point(Math.Abs(bias.X), Math.Abs(bias.Y));
            _directionFlags = detection;
        }

        /// <summary>
        /// Gets the detection rectangle (in tile coords) for this sensor)
        /// </summary>
        private Rectangle DetectBounds
        {
            get
            {
                return new Rectangle(
                    _origin.X - _bias.X, _origin.Y - _bias.Y, 
                    _origin.X + _bias.X, _origin.Y + _bias.Y);
            }
        }

        /// <summary>
        /// Utility method to return a facing that detects in all directions.
        /// </summary>
        public static Facing RadialDetection
        {
            get
            {
                return Facing.Left | Facing.Right | Facing.Up | Facing.Down;
            }
        }

        /// <summary>
        /// Check if the sensor detect's its subject.
        /// </summary>
        public virtual bool Detect()
        {
            return (isFacing(Facing.Left) && detectedLeft())
                    || (isFacing(Facing.Right) && detectedRight())
                    || (isFacing(Facing.Up) && detectedUp())
                    || (isFacing(Facing.Down) && detectedDown());
        }

        private bool isFacing(Facing face)
        {
            return (_directionFlags & face) == face;
        }

        private bool detectedDown()
        {
            return 
                (_subject.Coords.Y >= _origin.Y && _subject.Coords.Y <= DetectBounds.Height)
                && (_subject.Coords.X >= DetectBounds.X && _subject.Coords.X <= DetectBounds.Width);
        }

        private bool detectedUp()
        {
            return 
                (_subject.Coords.Y >= DetectBounds.Y && _subject.Coords.Y <= _origin.Y)    
                && (_subject.Coords.X >= DetectBounds.X && _subject.Coords.X <= DetectBounds.Width);
        }

        private bool detectedRight()
        {
            return 
                (_subject.Coords.X >= _origin.X && _subject.Coords.X <= DetectBounds.Width)
                && (_subject.Coords.Y >= DetectBounds.Y && _subject.Coords.Y <= DetectBounds.Height);
        }

        private bool detectedLeft()
        {
            return 
                (_subject.Coords.X >= DetectBounds.X && _subject.Coords.X <= _origin.X)
                && (_subject.Coords.Y >= DetectBounds.Y && _subject.Coords.Y <= DetectBounds.Height);
        }
    }

    [Flags]
    public enum Facing
    {
        None = 0x0, Left = 0x1, Right = 0x2, Up = 0x4, Down = 0x8
    }    
}
