namespace Supertext.Base.Factory
{
    /// <summary>
    /// Adapter for IoC-Func-Feature
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFactory<out T>
    {
        T Create();
    }

    /// <summary>
    /// Adapter for IoC-Func-Feature
    /// </summary>
    /// <typeparam name="TParam"></typeparam>
    /// <typeparam name="T"></typeparam>
    public interface IFactory<in TParam, out T>
    {
        T Create(TParam input);
    }

    /// <summary>
    /// Adapter for IoC-Func-Feature
    /// </summary>
    /// <typeparam name="TParam1"></typeparam>
    /// <typeparam name="TParam2"></typeparam>
    /// <typeparam name="T"></typeparam>
    public interface IFactory<in TParam1, in TParam2, out T>
    {
        T Create(TParam1 param1, TParam2 param2);
    }
}