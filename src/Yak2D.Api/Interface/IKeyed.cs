namespace Yak2D
{
    /// <summary>
    /// All framework object references provided to users are ultimately simple objects that hold unsigned int64 ID keys. Each reference inherrits from this base intereface. 
    /// The type system is used to help avoid keys being used incorrectly: each object type has it's own empty interface that inherrits from IKeyed (perhaps an anti-pattern). 
    /// The actual int64 keys are not hidden from the user, so key 'abuse' can occur; but allows custom storage of key values if desired. 
    /// The framework allows raw keys to be provided to it's service methods, but the user must ensure keys are used for the correct item types
    /// Note: The framework does not internally hold references to the IKeyed Objects provided to the user on resource creation, it only stores the unsigned int64
    /// Note: When resources are lost (and the framework requests resource [re-]creation), all old keys will become invalid. It is not guaranteed that keys will not be reused, but it is unlikley (multiple universe lifespan type expected periods)
    /// </summary>
    public interface IKeyed
    {
        /// <summary>
        /// The ID Key Value
        /// </summary>
        ulong Id { get; }
    }
}