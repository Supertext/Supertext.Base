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
}