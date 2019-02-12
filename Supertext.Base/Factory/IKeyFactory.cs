namespace Supertext.Base.Factory
{
    public interface IKeyFactory<in TKey, out T>
    {
        bool ComponentExists(TKey key);
        T CreateComponent(TKey key);
    }
}