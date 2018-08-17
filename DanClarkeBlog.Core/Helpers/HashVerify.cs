using System;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;

namespace DanClarkeBlog.Core.Helpers
{
    [UsedImplicitly]
    public class HashVerify : IHashVerify
    {
        public bool VerifySha256Hash(HashAlgorithm sha256Hash, string input, string hash)
        {
            var hashOfInput = GetSha256Hash(sha256Hash, input);

            return string.Compare(hashOfInput, hash, StringComparison.OrdinalIgnoreCase) == 0;
        }

        private static string GetSha256Hash(HashAlgorithm sha256Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash
            var data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(data).Replace("-", "").ToLower();
        }
    }
}
