using Isopoh.Cryptography.Argon2;
using PruebaTecnicaBack.Application.Interfaces;

namespace PruebaTecnicaBack.Infrastructure.Security;

public class Argon2PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return Argon2.Hash(password);
    }

    public bool Verify(string password, string hash)
    {
        return Argon2.Verify(hash, password);
    }
}
