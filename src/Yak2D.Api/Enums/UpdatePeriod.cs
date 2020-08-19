namespace Yak2D
{
    /// <summary>
    /// Framework Update Loop time period type
    /// A fixed timestep, as small as possible, is generally advised to give more stable simulations
    /// Please note, that if the user has requested the framework processes fractional catch up update iterations
    /// before drawing, not all update timesteps will be of an equal length. It maybe preferrable to avoid
    /// fractional pre-draw updates and implement some draw update motion interpolation to achieve additional smoothness
    /// at the risk of error correction / deviation from the next update simulation step
    /// </summary>
    public enum UpdatePeriod
    {
        /// <summary>
        /// Each update is a fixed timespan as requested by the user in the start up configuration
        /// If the update (+ other framework processing) takes longer to process than the timespan itself
        /// the simulation will slow down and fall behind the clock
        /// </summary>
        Fixed,

        /// <summary>
        /// Framework automatically adjusts the fixed timestep used to process update loop iterations / 'frames'
        /// The framework measures the percentage of time sat 'idle'
        /// Should the framework be utilised above a certain percentage (currently hard coded at 95%) for a number of frames (currently 8)
        /// Then the relieve pressure on the system, the fixed timestep is doubled
        /// The opposite is true for underutilisation, however the time period over which it must be observed to 
        /// result in a timestep halfing is longer (120 frames currentyl hardcoded at <= 40% utilisation)
        /// </summary>
        Fixed_Adaptive,

        /// <summary>
        /// The framework uses a variable update period equal to the time since the last update
        /// A lower bound is provided by the user in the start update config for the smallest possible timestep
        /// that should be processed. Below this time period, the framework waits
        /// </summary>
        Variable
    }
}