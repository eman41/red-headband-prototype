using System;
using Red_Headband_Prototype.Core;

namespace Red_Headband_Prototype
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GameMaster game = new GameMaster())
            {
                game.Run();
            }
        }
    }
#endif
}

