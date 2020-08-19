namespace Yak2D.Internal
{
    public interface IStartupPropertiesCache
    {
        StartupConfig User { get; }
        InternalStartUpProperties Internal { get; }
    }
}