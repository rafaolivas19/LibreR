using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using LibreR.Models;

namespace LibreR.Controllers {
    public class Security {
        private readonly byte[] _initVectorBytes;
        private readonly int _keysize;
        private readonly string _pass;

        public Security(string pass) : this(pass, "tu89geji34xt89u2", 256) { }

        public Security(string pass, string initVectorBytes, int KeySize) {
            if (initVectorBytes.Length != 16) throw new LibrerException("Vector bytes have to have a length of 16");

            _initVectorBytes = Encoding.ASCII.GetBytes(initVectorBytes);
            _keysize = KeySize;
            _pass = pass;
        }

        public string Encrypt(string text) {
            var plainTextBytes = Encoding.UTF8.GetBytes(text);

            using (var password = new PasswordDeriveBytes(_pass, null)) {
                var keyBytes = password.GetBytes(_keysize / 8);

                using (var symmetricKey = new RijndaelManaged()) {
                    symmetricKey.Mode = CipherMode.CBC;

                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, _initVectorBytes)) {
                        using (var memoryStream = new MemoryStream()) {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)) {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                var cipherTextBytes = memoryStream.ToArray();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public string Decrypt(string text) {
            try {
                var cipherTextBytes = Convert.FromBase64String(text);

                using (var password = new PasswordDeriveBytes(_pass, null)) {
                    var keyBytes = password.GetBytes(_keysize / 8);

                    using (var symmetricKey = new RijndaelManaged()) {
                        symmetricKey.Mode = CipherMode.CBC;

                        using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, _initVectorBytes)) {
                            using (var memoryStream = new MemoryStream(cipherTextBytes)) {
                                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read)) {
                                    var plainTextBytes = new byte[cipherTextBytes.Length];
                                    var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                    return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                                }
                            }
                        }
                    }
                }
            }
            catch {
                return null;
            }
        }

        public static string ToMd5(string s) {
            return string.Join(string.Empty, MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(s)).Select(x => x.ToString("x2")));
        }

        public static bool IsUserAdministrator() {
            WindowsIdentity user = null;

            try {
                user = WindowsIdentity.GetCurrent();

                if (user == null) return false;

                var principal = new WindowsPrincipal(user);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch {
                return false;
            }
            finally {
                user?.Dispose();
            }
        }
    }
}