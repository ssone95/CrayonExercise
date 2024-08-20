using System.Security.Cryptography;
using System.Text;

namespace CrayonCCP.Infrastructure.Extensions;

public static class Sha256Extensions
{
    public static byte[] Hash(string input)
    {
        string lowercaseInput = input.ToLower();
        var crypt = SHA256.Create();
        byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(lowercaseInput));
        return crypto;
    }
}