// -----------------------------------------------------------------------
// MathUtilities.cs: Utility class for math not provided by XNA
// Author: Eric S. Policaro
// -----------------------------------------------------------------------
namespace TileEngine.Engine.Platforms
{
    using System;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Utility class for specialized match functions.
    /// </summary>
    public sealed class MathUtilities
    {
        /// <summary>
        /// Check if the given position is between two points
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="pos">Position to verify</param>
        /// <returns>True if the position is between the first and second point</returns>
        public static bool PointBetween(Vector2 p1, Vector2 p2, Vector2 pos)
        {
            float xMin = Math.Min(p1.X, p2.X);
            float yMin = Math.Min(p1.Y, p2.Y);

            float xMax = Math.Max(p1.X, p2.X);
            float yMax = Math.Max(p1.Y, p2.Y);

            return (pos.X >= xMin && pos.X <= xMax)
                    && (pos.Y >= yMin && pos.Y <= yMax);
        }
    }
}
