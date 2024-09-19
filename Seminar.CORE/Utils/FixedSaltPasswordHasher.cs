using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Seminar.CORE.Utils
{
    public class FixedSaltPasswordHasher<TUser> : PasswordHasher<TUser> where TUser : class
    {
        private readonly byte[] fixedSalt;

        public FixedSaltPasswordHasher(IOptions<PasswordHasherOptions>? optionsAccessor = null) : base(optionsAccessor)
        {
            // Sử dụng một muối cố định
            fixedSalt = Convert.FromBase64String("YmxhYmxhYmxhYmxhYmxhYg=="); // Thay thế bằng muối cố định của bạn
        }

        public override string HashPassword(TUser user, string password)
        {
            ArgumentNullException.ThrowIfNull(password); // Sử dụng ArgumentNullException.ThrowIfNull

            // Sử dụng các phương thức của lớp cơ sở để truy cập _compatibilityMode và _iterCount
            var options = this.GetOptions();
            var compatibilityMode = options.CompatibilityMode;
            var iterCount = options.IterationCount;

            if (compatibilityMode == PasswordHasherCompatibilityMode.IdentityV2)
            {
                return Convert.ToBase64String(HashPasswordV2(password, fixedSalt));
            }
            else
            {
                return Convert.ToBase64String(HashPasswordV3(password, fixedSalt, iterCount));
            }
        }

        private static byte[] HashPasswordV2(string password, byte[] salt)
        {
            const KeyDerivationPrf Pbkdf2Prf = KeyDerivationPrf.HMACSHA1; // default for Rfc2898DeriveBytes
            const int Pbkdf2IterCount = 1000; // default for Rfc2898DeriveBytes
            const int Pbkdf2SubkeyLength = 256 / 8; // 256 bits

            // Produce a version 2 (see comment above) text hash.
            byte[] subkey = KeyDerivation.Pbkdf2(password, salt, Pbkdf2Prf, Pbkdf2IterCount, Pbkdf2SubkeyLength);

            var outputBytes = new byte[1 + salt.Length + Pbkdf2SubkeyLength];
            outputBytes[0] = 0x00; // format marker
            Buffer.BlockCopy(salt, 0, outputBytes, 1, salt.Length);
            Buffer.BlockCopy(subkey, 0, outputBytes, 1 + salt.Length, Pbkdf2SubkeyLength);
            return outputBytes;
        }

        private byte[] HashPasswordV3(string password, byte[] salt, int iterCount)
        {
            return HashPasswordV3(password, salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterCount: iterCount,
                saltSize: salt.Length,
                numBytesRequested: 256 / 8);
        }

        private static byte[] HashPasswordV3(string password, byte[] salt, KeyDerivationPrf prf, int iterCount, int saltSize, int numBytesRequested)
        {
            // Produce a version 3 (see comment above) text hash.
            byte[] subkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, numBytesRequested);

            var outputBytes = new byte[13 + saltSize + subkey.Length];
            outputBytes[0] = 0x01; // format marker
            WriteNetworkByteOrder(outputBytes, 1, (uint)prf);
            WriteNetworkByteOrder(outputBytes, 5, (uint)iterCount);
            WriteNetworkByteOrder(outputBytes, 9, (uint)saltSize);
            Buffer.BlockCopy(salt, 0, outputBytes, 13, saltSize);
            Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);
            return outputBytes;
        }

        private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value);
        }

        private PasswordHasherOptions GetOptions()
        {
            var options = new PasswordHasherOptions();
            if (typeof(PasswordHasher<TUser>).GetField("_compatibilityMode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) != null)
            {
                options.CompatibilityMode = (PasswordHasherCompatibilityMode)typeof(PasswordHasher<TUser>)
                    .GetField("_compatibilityMode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .GetValue(this);
            }

            if (typeof(PasswordHasher<TUser>).GetField("_iterCount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) != null)
            {
                options.IterationCount = (int)typeof(PasswordHasher<TUser>)
                    .GetField("_iterCount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .GetValue(this);
            }

            return options;
        }
    }
}