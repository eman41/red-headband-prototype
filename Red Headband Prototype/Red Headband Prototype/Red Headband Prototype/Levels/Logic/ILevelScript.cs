namespace Red_Headband_Prototype.Levels.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using TileEngine.Engine;
    using Red_Headband_Prototype.Core;

    public interface ILevelScript
    {
        GameMap GetGameMap();

        void Scripting(PlayerObject obj, ref Camera2D camera);
    }
}
