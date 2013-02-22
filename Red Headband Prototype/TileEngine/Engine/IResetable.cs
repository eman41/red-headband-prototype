// -----------------------------------------------------------------------
// <copyright file="IResetable.cs" company="Me" />
// Author: Eric S. Policaro
// Defines an object that can be reset back to an initial state.
// Useful for elements that may have multiple or complex starting states.
// -----------------------------------------------------------------------
namespace TileEngine.Engine
{
    public interface IResetable
    {
        void Reset();
    }
}
