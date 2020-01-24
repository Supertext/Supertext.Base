namespace Supertext.Base.Factory
{
    /// <summary>
    /// Adapter for IoC-Func-Feature
    /// </summary>
    public interface IFactory
    {
        T Create<T>();
    }

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
        T Create(TParam param);
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

    /// <summary>
    /// Adapter for IoC-Func-Feature
    /// </summary>
    /// <typeparam name="TParam1"></typeparam>
    /// <typeparam name="TParam2"></typeparam>
    /// <typeparam name="TParam3"></typeparam>
    /// <typeparam name="T"></typeparam>
    public interface IFactory<in TParam1, in TParam2, in TParam3, out T>
    {
        T Create(TParam1 param1, TParam2 param2, TParam3 param3);
    }
}