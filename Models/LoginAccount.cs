
using System.Security.Cryptography;

namespace LibraryManagementSystem.Models
{
    public class LoginAccount
    {
        public Person User { get; private set; }

        private byte[] salt;
        private byte[] passwordHash;

        public LoginAccount(Person user, string password)
        {
            User = user;

            salt = RandomNumberGenerator.GetBytes(16);

            passwordHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                100000,
                HashAlgorithmName.SHA256,
                32
            );
        }

        public bool VerifyPassword(string password)
        {
            byte[] enteredHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                100000,
                HashAlgorithmName.SHA256,
                32
            );

            return CryptographicOperations.FixedTimeEquals(
                passwordHash,
                enteredHash
            );
        }
    }
}
