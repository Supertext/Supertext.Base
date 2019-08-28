namespace Supertext.Base.Dal
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create(string connectionString);
    }
}