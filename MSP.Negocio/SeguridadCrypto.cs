using System.Security.Cryptography;
using System.Text;

namespace MSP.Negocio
{
    public static class SeguridadCrypto
    {
        public static (byte[] Hash, byte[] Salt) CrearHash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(32);
            byte[] hash = CalcularHash(password, salt);
            return (hash, salt);
        }

        public static bool VerificarPassword(string passwordIngresada, byte[] hashGuardado, byte[] saltGuardado)
        {
            byte[] hashCalculado = CalcularHash(passwordIngresada, saltGuardado);
            return CryptographicOperations.FixedTimeEquals(hashCalculado, hashGuardado);
        }

        private static byte[] CalcularHash(string password, byte[] salt)
        {
            byte[] passBytes = Encoding.UTF8.GetBytes(password);
            byte[] combinado = new byte[passBytes.Length + salt.Length];
            Buffer.BlockCopy(passBytes, 0, combinado, 0, passBytes.Length);
            Buffer.BlockCopy(salt, 0, combinado, passBytes.Length, salt.Length);
            return SHA512.HashData(combinado);
        }
    }
}