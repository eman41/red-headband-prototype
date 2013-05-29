// -----------------------------------------------------------------------
// IResetable.cs: Contains an interface for defining resetable components.
// Author: Eric S. Policaro
// -----------------------------------------------------------------------
namespace TileEngine.Engine
{
    public interface IResetable
    {
        /// <summary>
        /// Reset this object back to its starting state.
        /// </summary>
        void Reset();
    }
}
