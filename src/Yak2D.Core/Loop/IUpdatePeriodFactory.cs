namespace Yak2D.Core
{
    public interface IUpdatePeriodFactory
    {
        IUpdatePeriod Create(UpdatePeriod UpdatePeriodType);
    }
}