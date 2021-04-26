using Library.Security;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;

namespace Services
{
    public class SecurityService
    {
        IDataProtector DataProtector { get; }

        public SecurityService(IDataProtectionProvider dataProtectionProvider)
        {
            DataProtector = dataProtectionProvider.CreateProtector("AuthorizationToken");
        }

        public AuthorizationToken CreateAuthorizationToken(CreateAuthorizationTokenDto data)
        {
            var securityDetails = new SecurityDetails
            {
                UserId = data.UserId,
                Permissions = data.Permissions,
                ExpiresOn = DateTime.Now.AddHours(1)
            };

            return new AuthorizationToken
            {
                SecurityDetails = securityDetails,
                EncryptedAuthorizationToken = EncryptSecurityDetails(securityDetails)
            };
        }

        public SecurityDetails DecryptSecurityDetails(string encrypted)
        {
            try
            {
                var securityDetailsJson = System.Text.Encoding.UTF8.GetString(DataProtector.Unprotect(Convert.FromBase64String(encrypted)));
                return JsonConvert.DeserializeObject<SecurityDetails>(securityDetailsJson);
            }
            catch (Exception e)
            {
                //TODO logging
                return null;
            }
        }

        public string EncryptSecurityDetails(SecurityDetails securityDetails)
        {
            var securityDetailsJson = JsonConvert.SerializeObject(securityDetails);
            return Convert.ToBase64String(DataProtector.Protect(System.Text.Encoding.UTF8.GetBytes(securityDetailsJson)));
        }

        public (string HashedPassword, string Salt) HashPassword(string password)
        {
            // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var hashedPassword = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            return (Convert.ToBase64String(hashedPassword), Convert.ToBase64String(salt));
        }

        public string HashPassword(string password, string salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Convert.FromBase64String(salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }
    }
}
