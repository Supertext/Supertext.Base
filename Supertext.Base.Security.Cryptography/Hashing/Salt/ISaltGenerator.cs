using System.Runtime.CompilerServices;

namespace Supertext.Base.Security.Cryptography.Hashing.Salt
{
    internal interface ISaltGenerator
    {
        string Generate();
    }
}