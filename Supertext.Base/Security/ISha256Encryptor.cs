using System;

namespace Supertext.Base.Security
{
    public interface ISha256Encryptor
    {
        Tuple<string, string> HashWithSaltAndPepper(string entry);
    }
}