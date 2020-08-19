namespace Yak2D
{
    /// <summary>
    /// Defines the looping (or not) properties of an item held in a distortion collection
    /// </summary>
    public enum LifeCycle
    {
        /// <summary>
        /// One shot - evolves to final configuration and is removed from the collection
        /// </summary>
        Single,

        /// <summary>
        /// Interpolation fraction returns to 0 when it reaches 1 as it loops
        /// </summary>
        LoopLinear,

        /// <summary>
        /// Interpolation fraction first rises to 1, then falls back to 0, and repeats
        LoopReverse
    }
}