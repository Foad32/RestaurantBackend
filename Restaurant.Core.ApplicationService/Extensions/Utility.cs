using System.Security.Cryptography;

namespace Restaurant.Core.ApplicationService.Extensions;

public static class Utility
{
    public static (string, string) HashPassword(string password)
    {
        // Generate a 16-byte salt
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Derive a 64-byte key using PBKDF2 (SHA512, 100,000 iterations)
        var hash = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA512);
        byte[] hashBytes = hash.GetBytes(64);

        // Combine salt + hash
        byte[] hashWithSalt = new byte[80];
        Buffer.BlockCopy(salt, 0, hashWithSalt, 0, 16);
        Buffer.BlockCopy(hashBytes, 0, hashWithSalt, 16, 64);

        // Convert both to Base64
        string hashBase64 = Convert.ToBase64String(hashWithSalt);
        string saltBase64 = Convert.ToBase64String(salt);

        return (hashBase64, saltBase64); // return both as strings
    }
}

