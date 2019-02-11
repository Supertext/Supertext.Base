namespace Supertext.Base.Core
{
    public interface ISpecification<in TSpecifiable> 
    {
        bool IsSatisfiedBy(TSpecifiable entity);
    }
}