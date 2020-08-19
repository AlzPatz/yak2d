namespace Yak2D.Core
{
    public interface ITimer
    {
        double Seconds { get; }
        void Reset();
        void Start();
    }
}