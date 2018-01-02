using System.IO;
using System.Text;

namespace System.Security.Cryptography
{
    /// <summary>
    ///     A symmetric key algorithm (Rijndael (pronounced rain-dahl) or Advanced Encryption Standard (AES)) to encrypt and
    ///     decrypt data. The algorithm described by AES is a symmetric-key algorithm, meaning the same key is
    ///     used for both encrypting and decrypting the data.
    /// </summary>
    public static class SymmetricKeyProtection
    {
        #region Constants

        /// <summary>
        ///     The Initialization vector (or IV). This value is required to encrypt the
        ///     first block of plaintext data. For RijndaelManaged class IV must be exactly 16 ASCII characters long.
        /// </summary>
        private const string IV = "@s8oliJu86GR6DPe";

        /// <summary>
        ///     Salt value used along with passphrase to generate password.
        /// </summary>
        private const string SALT = "xz5wAfoN";

        #endregion

        #region Public Methods

        /// <summary>
        ///     Encrypts specified plaintext using Rijndael symmetric key algorithm
        ///     and returns a base64-encoded result.
        /// </summary>
        /// <param name="unencryptedData">The unencrypted data.</param>
        /// <returns>
        ///     Encrypted value formatted as a base64-encoded string.
        /// </returns>
        public static string Protect(string unencryptedData)
        {
            return Protect(unencryptedData, string.Empty);
        }

        /// <summary>
        ///     Encrypts specified plaintext using Rijndael symmetric key algorithm
        ///     and returns a base64-encoded result.
        /// </summary>
        /// <param name="unencryptedData">The unencrypted data.</param>
        /// <param name="passphrase">The passphraSE.</param>
        /// <returns>
        ///     Encrypted value formatted as a base64-encoded string.
        /// </returns>
        public static string Protect(string unencryptedData, string passphrase)
        {
            return Protect(unencryptedData, passphrase, SALT, IV);
        }

        /// <summary>
        ///     Encrypts specified plaintext using Rijndael symmetric key algorithm
        ///     and returns a base64-encoded result.
        /// </summary>
        /// <param name="unencryptedData">Plaintext value to be encrypted.</param>
        /// <param name="passphrase">
        ///     Passphrase from which a pseudo-random password will be derived. The derived password will be used
        ///     to generate the encryption key. Passphrase can be any string. In this example we assume that this
        ///     passphrase is an ASCII string.
        /// </param>
        /// <param name="salt">Salt value used along with passphrase to generate password. Salt can be any string.</param>
        /// <param name="initializationVector">
        ///     Initialization vector (or IV). This value is required to encrypt the
        ///     first block of plaintext data. For RijndaelManaged class IV must be exactly 16 ASCII characters long.
        /// </param>
        /// <returns>
        ///     Encrypted value formatted as a base64-encoded string.
        /// </returns>
        public static string Protect(string unencryptedData, string passphrase, string salt, string initializationVector)
        {
            return Protect(unencryptedData, passphrase, salt, initializationVector, 100, 256);
        }

        /// <summary>
        ///     Encrypts specified plaintext using Rijndael symmetric key algorithm
        ///     and returns a base64-encoded result.
        /// </summary>
        /// <param name="unencryptedData">Plaintext value to be encrypted.</param>
        /// <param name="passphrase">
        ///     Passphrase from which a pseudo-random password will be derived. The derived password will be used
        ///     to generate the encryption key. Passphrase can be any string. In this example we assume that this
        ///     passphrase is an ASCII string.
        /// </param>
        /// <param name="salt">Salt value used along with passphrase to generate password. Salt can be any string.</param>
        /// <param name="initializationVector">
        ///     Initialization vector (or IV). This value is required to encrypt the
        ///     first block of plaintext data. For RijndaelManaged class IV must be exactly 16 ASCII characters long.
        /// </param>
        /// <param name="iterations">Number of iterations used to generate password. One or two iterations should be enough.</param>
        /// <param name="keySize">
        ///     Size of encryption key in bits. Allowed values are: 128, 192, and 256. Longer keys are more
        ///     secure than shorter keys.
        /// </param>
        /// <returns>
        ///     Encrypted value formatted as a base64-encoded string.
        /// </returns>
        public static string Protect(string unencryptedData, string passphrase, string salt, string initializationVector, int iterations, int keySize)
        {
            // Convert strings into byte arrays.
            // Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8
            // encoding.
            byte[] vectorBytes = Encoding.ASCII.GetBytes(initializationVector);
            byte[] saltBytes = Encoding.ASCII.GetBytes(salt);

            // Convert our plaintext into a byte array.
            // Let us assume that plaintext contains UTF8-encoded characters.
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(unencryptedData);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = GenerateEncryptionKey(keySize, passphrase, saltBytes, iterations);

            // Generate encryptor from the existing key bytes and initialization
            // vector. Key size will be defined based on the number of the key
            // bytes.
            RijndaelManaged symmetricKey = CreateAlgorithm(CipherMode.CBC);
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, vectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            using (MemoryStream ms = new MemoryStream())
            {
                // Define cryptographic stream (always use Write mode for encryption).
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    // Start encrypting.
                    cs.Write(plainTextBytes, 0, plainTextBytes.Length);

                    // Finish encrypting.
                    cs.FlushFinalBlock();

                    // Convert our encrypted data from a memory stream into a byte array.
                    byte[] cipherTextBytes = ms.ToArray();

                    // Convert encrypted data into a base64-encoded string.
                    string cipherText = Convert.ToBase64String(cipherTextBytes);

                    // Return encrypted string.
                    return cipherText;
                }
            }
        }

        /// <summary>
        ///     Decrypts specified ciphertext using Rijndael symmetric key algorithm.
        /// </summary>
        /// <param name="encryptedData">The encrypted data.</param>
        /// <returns>
        ///     Decrypted string value.
        /// </returns>
        public static string Unprotect(string encryptedData)
        {
            return Unprotect(encryptedData, string.Empty);
        }

        /// <summary>
        ///     Decrypts specified ciphertext using Rijndael symmetric key algorithm.
        /// </summary>
        /// <param name="encryptedData">The encrypted data.</param>
        /// <param name="passphrase">The passphraSE.</param>
        /// <returns>
        ///     Decrypted string value.
        /// </returns>
        public static string Unprotect(string encryptedData, string passphrase)
        {
            return Unprotect(encryptedData, passphrase, SALT, IV);
        }

        /// <summary>
        ///     Decrypts specified ciphertext using Rijndael symmetric key algorithm.
        /// </summary>
        /// <param name="encryptedData">Base64-formatted ciphertext value.</param>
        /// <param name="passphrase">
        ///     Passphrase from which a pseudo-random password will be derived. The derived password will be used
        ///     to generate the encryption key. Passphrase can be any string.
        /// </param>
        /// <param name="salt">
        ///     Salt value used along with passphrase to generate password. Salt can
        ///     be any string.
        /// </param>
        /// <param name="initializationVector">
        ///     Initialization vector (or IV). This value is required to encrypt the
        ///     first block of plaintext data. For RijndaelManaged class IV must be exactly 16 ASCII characters long.
        /// </param>
        /// <returns>
        ///     Decrypted string value.
        /// </returns>
        /// <remarks>
        ///     Most of the logic in this function is similar to the Encrypt
        ///     logic. In order for decryption to work, all parameters of this function
        ///     - except cipherText value - must match the corresponding parameters of
        ///     the Encrypt function which was called to generate the
        ///     ciphertext.
        /// </remarks>
        public static string Unprotect(string encryptedData, string passphrase, string salt, string initializationVector)
        {
            return Unprotect(encryptedData, passphrase, salt, initializationVector, 100, 256);
        }

        /// <summary>
        ///     Decrypts specified ciphertext using Rijndael symmetric key algorithm.
        /// </summary>
        /// <param name="encryptedData">Base64-formatted ciphertext value.</param>
        /// <param name="passphrase">
        ///     Passphrase from which a pseudo-random password will be derived. The derived password will be used
        ///     to generate the encryption key. Passphrase can be any string.
        /// </param>
        /// <param name="salt">
        ///     Salt value used along with passphrase to generate password. Salt can
        ///     be any string.
        /// </param>
        /// <param name="initializationVector">
        ///     Initialization vector (or IV). This value is required to encrypt the
        ///     first block of plaintext data. For RijndaelManaged class IV must be exactly 16 ASCII characters long.
        /// </param>
        /// <param name="iterations">Number of iterations used to generate password. One or two iterations should be enough.</param>
        /// <param name="keySize">
        ///     Size of encryption key in bits. Allowed values are: 128, 192, and 256. Longer keys are more
        ///     secure than shorter keys.
        /// </param>
        /// <returns>
        ///     Decrypted string value.
        /// </returns>
        /// <remarks>
        ///     Most of the logic in this function is similar to the Encrypt
        ///     logic. In order for decryption to work, all parameters of this function
        ///     - except cipherText value - must match the corresponding parameters of
        ///     the Encrypt function which was called to generate the
        ///     ciphertext.
        /// </remarks>
        public static string Unprotect(string encryptedData, string passphrase, string salt, string initializationVector, int iterations, int keySize)
        {
            // Convert strings defining encryption key characteristics into byte
            // arrays. Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8
            // encoding.
            byte[] vectorBytes = Encoding.ASCII.GetBytes(initializationVector);
            byte[] saltBytes = Encoding.ASCII.GetBytes(salt);

            // Convert our ciphertext into a byte array.
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedData);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = GenerateEncryptionKey(keySize, passphrase, saltBytes, iterations);

            // Generate decryptor from the existing key bytes and initialization
            // vector. Key size will be defined based on the number of the key
            // bytes.
            RijndaelManaged symmetricKey = CreateAlgorithm(CipherMode.CBC);
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, vectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            using (MemoryStream ms = new MemoryStream(cipherTextBytes))
            {
                // Define cryptographic stream (always use Read mode for encryption).
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    // Since at this point we don't know what the size of decrypted data
                    // will be, allocate the buffer long enough to hold ciphertext,
                    // plaintext is never longer than ciphertext.
                    byte[] plainTextBytes = new byte[cipherTextBytes.Length];

                    // Start decrypting.
                    int decryptedByteCount = cs.Read(plainTextBytes, 0, plainTextBytes.Length);

                    // Convert decrypted data into a string.
                    // Let us assume that the original plaintext string was UTF8-encoded.
                    string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

                    // Return decrypted string.
                    return plainText;
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Returns the managed version of the encryption algorithm.
        /// </summary>
        /// <param name="cipherMode">The cipher mode.</param>
        /// <returns>
        ///     Returns a <see cref="RijndaelManaged" /> representing the encryption / decrypting object.
        /// </returns>
        private static RijndaelManaged CreateAlgorithm(CipherMode cipherMode)
        {
            // Create uninitialized Rijndael encryption object.
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = cipherMode;

            return symmetricKey;
        }

        /// <summary>
        ///     Returns a generate pseudo-random bytes for the encryption key.
        /// </summary>
        /// <param name="keySize">Size of the key.</param>
        /// <param name="passphrase">The passphraSE.</param>
        /// <param name="saltBytes">The salt bytes.</param>
        /// <param name="iterations">The iterations.</param>
        /// <returns>
        ///     Returns a <see cref="byte" /> array of the encryption key.
        /// </returns>
        private static byte[] GenerateEncryptionKey(int keySize, string passphrase, byte[] saltBytes, int iterations)
        {
            // First, we must create a password, from which the key will be
            // derived. This password will be generated from the specified
            // passphrase and salt value. The password will be created using
            // the specified hash algorithm. Password creation can be done in
            // several iterations.
            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(passphrase, saltBytes, iterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = deriveBytes.GetBytes(keySize/8);
            return keyBytes;
        }

        #endregion
    }
}