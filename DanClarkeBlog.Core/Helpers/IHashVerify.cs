using System.Security.Cryptography;

namespace DanClarkeBlog.Core.Helpers
{
    public interface IHashVerify
    {
        bool VerifySha256Hash(HashAlgorithm sha256Hash, string input, string hash);
    }
}