namespace Supertext.Base.Common
{
    public interface IWildcardChecker
    {
        bool IsPassing(string filter, string value);
    }
}
