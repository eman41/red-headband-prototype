// -----------------------------------------------------------------------
// <copyright file="Camera2D.cs" company="" />
// Author: Eric S. Policaro
// -----------------------------------------------------------------------
namespace TileEngine.Engine
{
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;

    public enum CameraLock
    {
        None, Horizontal, Vertical, Both
    }

    public class Camera2D
    {
        private Vector2 _position;
        private float _zoom;
        private float _viewPortWidth;
        private float _viewPortHeight;

        public Camera2D()
        {
        }

        public Camera2D(float viewWidth, float viewHeight)
        {
            _zoom = 1.0f;
            Rotation = 0.0f;
            _position = Vector2.Zero;
            _viewPortWidth = viewWidth;
            _viewPortHeight = viewHeight;
            Parallax = Vector2.One;
            Origin = new Vector2(_viewPortWidth / 2.0f, _viewPortHeight / 2.0f);
            HoldPositions = new Queue<int>();
        }

        public CameraLock LockStatus { get; set; }
        public float Rotation { get; set; }
        public Rectangle? Limits {get; set;}
        public Vector2 Parallax { get; set; }
        public Vector2 Origin { get; set; }
        public Queue<int> HoldPositions { get; set;}

        public bool hasHoldPositions()
        {
            return HoldPositions.Count > 0;
        }

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

        private void ValidatePosition()
        {
            if (Limits.HasValue)
            {
                Vector2 cameraWorldMin = Vector2.Transform(Vector2.Zero, Matrix.Invert(Transform));
                Vector2 cameraSize = new Vector2(_viewPortWidth, _viewPortHeight) / _zoom;
                Vector2 limitWorldMin = new Vector2(Limits.Value.Left, Limits.Value.Top);
                Vector2 limitWorldMax = new Vector2(Limits.Value.Right, Limits.Value.Bottom);
                Vector2 positionOffset = _position - cameraWorldMin;
                //positionOffset.X -= Limits.Value.Left;
                _position = Vector2.Clamp(cameraWorldMin, limitWorldMin, limitWorldMax - cameraSize) + positionOffset;
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

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, Transform);
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(Transform));
        }

        // Auxiliary function to move the camera taking into account the camera lock
        public void Move(Vector2 amount)
        {
            switch (LockStatus)
            {
                case CameraLock.None:
                    _position += amount;
                    break;
                case CameraLock.Horizontal:
                    _position.Y += amount.Y;
                    break;
                case CameraLock.Vertical:
                    _position.X += amount.X;
                    break;
                case CameraLock.Both:
                    break;
                default:
                    _position += amount;
                    break;
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
    }
}
