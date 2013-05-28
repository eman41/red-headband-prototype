// -----------------------------------------------------------------------
// Camera2D.cs
// Author: Eric S. Policaro
// Standard orthographic camera.
// Features:
//      -- Locked to viewport
//      -- Can be locked in any direction
//      -- Contains a queue that can be used to hold the camera
//         after it passes a certain point
// Code basis and inspiration from:
// http://www.david-amador.com/2009/10/xna-camera-2d-with-zoom-and-rotation/
// -----------------------------------------------------------------------
namespace TileEngine.Engine
{
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;

    public class Camera2D
    {
        private Vector2 _position;
        private float _zoom;
        private float _viewPortWidth;
        private float _viewPortHeight;

        public Camera2D()
        {
            _zoom = 1.0f;
            Rotation = 0.0f;
            _position = Vector2.Zero;
            Parallax = Vector2.One;
            HoldPositions = new Queue<int>();
        }

        public Camera2D(float viewWidth, float viewHeight) 
            : this()
        {
            _viewPortWidth = viewWidth;
            _viewPortHeight = viewHeight;
            Origin = new Vector2(_viewPortWidth / 2.0f, _viewPortHeight / 2.0f);
        }

        public bool hasHoldPositions()
        {
            return HoldPositions.Count > 0;
        }

        public CameraLock LockStatus { get; set; }
        public float Rotation { get; set; }
        public Rectangle? Limits {get; set;}
        public Vector2 Parallax { get; set; }
        public Vector2 Origin { get; set; }
        public Queue<int> HoldPositions { get; set;}

        public float Zoom
        {
            get { return _zoom; } // Negative zoom will flip image
            set 
            { 
                _zoom = value; 
                if (_zoom < 0.1f) 
                    _zoom = 0.1f;
                ValidateZoom();
            }
        }

        private void ValidateZoom()
        {
            if (Limits.HasValue)
            {
                float minZoomX = (float)_viewPortWidth / Limits.Value.Width;
                float minZoomY = (float)_viewPortHeight / Limits.Value.Height;
                _zoom = MathHelper.Max(_zoom, MathHelper.Max(minZoomX, minZoomY));
            }
        }

        public Matrix Transform
        {
            get
            {
                Vector3 translation = new Vector3(_viewPortWidth * 0.5f, _viewPortHeight * 0.5f, 0);
                Vector3 zoomVector = new Vector3(Zoom, Zoom, 1);
                return
                    Matrix.CreateTranslation(
                        new Vector3(-_position * Parallax, 0))
                        * Matrix.CreateRotationZ(Rotation)
                        * Matrix.CreateScale(zoomVector)
                        * Matrix.CreateTranslation(translation);
            }
        }  

        // Get or set the position taking into account the camera lock
        public Vector2 FocusVector
        {
            get { return _position; }
            set
            {
                switch (LockStatus)
                {
                    case CameraLock.Horizontal:
                        _position.Y = value.Y;
                        break;

                    case CameraLock.Vertical:
                        _position.X = value.X;
                        break;

                    case CameraLock.Both:
                        break;

                    case CameraLock.None:
                    default:
                        _position = value;
                        break;
                }

                ValidatePosition();
            }
        }

        private void ValidatePosition()
        {
            if (Limits.HasValue)
            {
                Vector2 cameraWorldMin = Vector2.Transform(Vector2.Zero, Matrix.Invert(Transform));
                Vector2 cameraSize = new Vector2(_viewPortWidth, _viewPortHeight) / _zoom;
                Vector2 limitWorldMin = new Vector2(Limits.Value.Left, Limits.Value.Top);
                Vector2 limitWorldMax = new Vector2(Limits.Value.Right, Limits.Value.Bottom);
                Vector2 positionOffset = _position - cameraWorldMin;
                _position = Vector2.Clamp(cameraWorldMin, limitWorldMin, limitWorldMax - cameraSize)
                    + positionOffset;
            }
        }
    }

    public enum CameraLock
    {
        None, Horizontal, Vertical, Both
    }
}
