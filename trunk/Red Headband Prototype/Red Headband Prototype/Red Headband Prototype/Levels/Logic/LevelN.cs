// -----------------------------------------------------------------------
// LevelN.cs: Scripting for LevelN
// Author: Eric S. Policaro
// -----------------------------------------------------------------------
namespace Red_Headband_Prototype.Levels.Logic
{
    using System;
    using Red_Headband_Prototype.Core;
    using TileEngine.Engine;
    using TileEngine.Engine.Platforms;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using TileEngine.Engine.AI;
    using TileEngine.Collision;

    public class LevelN : ILevelScript
    {
        private GameMap _gameMap;
        
        
        public LevelN(GameMap gameMap)
        {
            _gameMap = gameMap;
        }

        public GameMap GetGameMap()
        {
            return _gameMap;
        }

        public void Scripting(PlayerObject obj, ref Camera2D camera)
        {
            // CAMERA/PLATFORM/ETC SCRIPTING FOR LEVEL N
            camera = new Camera2D(GameMaster.RealViewWidth, GameMaster.RealViewHeight);
            camera.Zoom = GameMaster.ZOOM_FACTOR;
            camera.Rotation = GameMaster.ROTATION;
            camera.LockStatus = CameraLock.None;
            camera.Limits = new Rectangle(0, 0, (int)_gameMap.RealLevelWidth, (int)_gameMap.RealLevelHeight);
            camera.HoldPositions.Enqueue(59);
            camera.HoldPositions.Enqueue(90);

            PlatformController controller = new PlatformController(new Vector2(26, 22), new Vector2(36, 22), -5f, TimeSpan.FromSeconds(1));
            PlatformController controller2 = new PlatformController(new Vector2(59, 26), new Vector2(82, 26), -5f, TimeSpan.FromSeconds(3), true);
            PlatformController controller3 = new PlatformController(new Vector2(103, 24), new Vector2(103, 8), -5f, TimeSpan.FromSeconds(3), true);

            PlatformController controller4 = new PlatformController(new Vector2(13, 6), new Vector2(27, 6), 8f, TimeSpan.FromSeconds(1), false);
            PlatformController controller5 = new PlatformController(new Vector2(27, 11), new Vector2(13, 11), 8f, TimeSpan.FromSeconds(1), false);
            Sensor sensor1 = new Sensor(obj, new Point(13, 7), new Point(20,20), Sensor.RadialDetection);
            Sensor sensor2 = new Sensor(obj, new Point(27, 12), new Point(20, 20), Sensor.RadialDetection);

            _gameMap.Platforms.Clear();
            _gameMap.Platforms.Add(new MovingPlatform(_gameMap.PlatformTextures, controller, new Rectangle(0, 0, 128, 20)));
            _gameMap.Platforms.Add(new MovingPlatform(_gameMap.PlatformTextures, controller2, new Rectangle(0, 0, 128, 20)));
            _gameMap.Platforms.Add(new MovingPlatform(_gameMap.PlatformTextures, controller3, new Rectangle(0, 0, 128, 20)));
            _gameMap.Platforms.Add(new KillPlatform(_gameMap.PlatformTextures, controller4, new Rectangle(35, 21, 32, 64), sensor1, Colliding.FromLeft | Colliding.FromRight));
            _gameMap.Platforms.Add(new KillPlatform(_gameMap.PlatformTextures, controller5, new Rectangle(35, 21, 32, 64), sensor2, Colliding.FromLeft | Colliding.FromRight));
            // END SCRIPTING
        }
    }
}
