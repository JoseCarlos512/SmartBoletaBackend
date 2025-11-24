using System;
using System.Security.Cryptography;
using SmartBoleta.Domain.Abstractions.Security;

namespace SmartBoleta.Infrastructure.Security;

public class Pbkdf2PasswordHasher : IPasswordHasher
{
    public byte[] GenerateSalt(int size = 16)
    {
        byte[] salt = new byte[size];
        RandomNumberGenerator.Fill(salt);
        return salt;
    }

    public byte[] Hash(string password, byte[] salt, int iterations = 100000, int outputBytes = 32)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256
        );

        return pbkdf2.GetBytes(outputBytes);
    }

    public bool Verify(string password, byte[] salt, byte[] expectedHash, int iterations = 100000)
    {
        var computed = Hash(password, salt, iterations, expectedHash.Length);

        // Comparación segura (tiempo constante)
        return CryptographicOperations.FixedTimeEquals(computed, expectedHash);
    }
}
