using System;

namespace SmartBoleta.Domain.Abstractions.Security;

public interface IPasswordHasher
{
    byte[] GenerateSalt(int size = 16);
    byte[] Hash(string password, byte[] salt, int iterations = 100_000, int outputBytes = 32);
    bool Verify(string password, byte[] salt, byte[] expectedHash, int iterations = 100_000);
}
