// -----------------------------------------------------------------------
// IResetable.cs: Contains an interface for defining resetable components.
// Author: Eric S. Policaro
// -----------------------------------------------------------------------
namespace TileEngine.Engine
{
    /// <summary>
    /// Interface used to define an object that can be reset back to an initial state.
    /// </summary>
    public interface IResetable
    {
        /// <summary>
        /// Reset this object back to its starting state.
        /// </summary>
        void Reset();
    }
}
