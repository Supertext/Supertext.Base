namespace Supertext.Base.Abstractions
{
    public interface ILifetimeScopeAbstraction
    {
        TService Resolve<TService>();
    }
}