using System.Runtime.CompilerServices;

namespace Supertext.Base.Security.Cryptography.Hashing.Salt
{
    public interface ISaltGenerator
    {
        string Generate();
    }
}