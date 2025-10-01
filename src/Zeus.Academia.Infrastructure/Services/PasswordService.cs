using System.Security.Cryptography;
using System.Text;

namespace Zeus.Academia.Infrastructure.Services;

/// <summary>
/// Simple password hashing service for demonstration purposes
/// In production, use a proper password hashing library like Argon2, BCrypt, or PBKDF2
/// </summary>
public interface IPasswordService
{
    /// <summary>
    /// Hash a password
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <returns>Hashed password</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verify a password against its hash
    /// </summary>
    /// <param name="password">Plain text password</param>
    /// <param name="hashedPassword">Hashed password to verify against</param>
    /// <returns>True if password matches</returns>
    bool VerifyPassword(string password, string hashedPassword);
}

/// <summary>
/// Simple password hashing service implementation using PBKDF2
/// Note: This is a basic implementation for demonstration. 
/// In production, use a dedicated password hashing library like Argon2 or BCrypt
/// </summary>
public class PasswordService : IPasswordService
{
    private const int SaltSize = 32; // 256 bits
    private const int HashSize = 32; // 256 bits
    private const int Iterations = 10000; // PBKDF2 iterations

    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));

        // Generate a random salt
        byte[] salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Hash the password with the salt
        byte[] hash = HashPasswordWithSalt(password, salt);

        // Combine salt and hash for storage
        byte[] hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
            return false;

        try
        {
            // Decode the stored hash
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);

            if (hashBytes.Length != SaltSize + HashSize)
                return false;

            // Extract salt and hash
            byte[] salt = new byte[SaltSize];
            byte[] storedHash = new byte[HashSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);
            Array.Copy(hashBytes, SaltSize, storedHash, 0, HashSize);

            // Hash the provided password with the same salt
            byte[] computedHash = HashPasswordWithSalt(password, salt);

            // Compare the hashes
            return CompareHashes(storedHash, computedHash);
        }
        catch
        {
            return false;
        }
    }

    private static byte[] HashPasswordWithSalt(string password, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(HashSize);
    }

    private static bool CompareHashes(byte[] hash1, byte[] hash2)
    {
        if (hash1.Length != hash2.Length)
            return false;

        int result = 0;
        for (int i = 0; i < hash1.Length; i++)
        {
            result |= hash1[i] ^ hash2[i];
        }

        return result == 0;
    }
}