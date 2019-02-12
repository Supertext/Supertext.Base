namespace Supertext.Base.Core
{
    public abstract class Entity<TId>
    {
        public TId Id { get; protected set; }
    }
}