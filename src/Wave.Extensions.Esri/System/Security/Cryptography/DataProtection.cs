using System.Text;

namespace System.Security.Cryptography
{
    /// <summary>
    ///     Provides protection using the user or machine credentials to encrypt or decrypt data.
    /// </summary>
    public static class DataProtection
    {
        #region Public Methods

        /// <summary>
        ///     Protects the specified value by applying an encryption algorthim specified using the
        ///     <see cref="DataProtectionScope.LocalMachine" /> scope.
        /// </summary>
        /// <param name="unencryptedData">The unencrypted data.</param>
        /// <returns>The encrypted data in string format.</returns>
        /// <exception cref="ArgumentNullException">The encryptedData parameter is null.</exception>
        /// <exception cref="CryptographicException">The cryptographic protection failed.</exception>
        /// <exception cref="PlatformNotSupportedException">The operating system does not support this method.</exception>
        /// <exception cref="OutOfMemoryException">The system ran out of memory while encrypting the data.</exception>
        public static string Protect(string unencryptedData)
        {
            return Protect(unencryptedData, string.Empty, DataProtectionScope.LocalMachine);
        }

        /// <summary>
        ///     Protects the specified value by applying an encryption algorthim specified using the
        ///     <see cref="DataProtectionScope.LocalMachine" /> scope.
        /// </summary>
        /// <param name="unencryptedData">The unencrypted data.</param>
        /// <param name="optionalEntropy">The optional entropy.</param>
        /// <returns>The encrypted data in string format.</returns>
        /// <exception cref="ArgumentNullException">The encryptedData parameter is null.</exception>
        /// <exception cref="CryptographicException">The cryptographic protection failed.</exception>
        /// <exception cref="PlatformNotSupportedException">The operating system does not support this method.</exception>
        /// <exception cref="OutOfMemoryException">The system ran out of memory while encrypting the data.</exception>
        public static string Protect(string unencryptedData, string optionalEntropy)
        {
            return Protect(unencryptedData, optionalEntropy, DataProtectionScope.LocalMachine);
        }

        /// <summary>
        ///     Protects the specified data by applying an encryption algorthim specified by the <see cref="DataProtectionScope" />
        ///     .
        /// </summary>
        /// <param name="unencryptedData">The unencrypted data.</param>
        /// <param name="optionalEntropy">The optional entropy.</param>
        /// <param name="scope">The scope.</param>
        /// <returns>The encrypted data in a byte array.</returns>
        /// <exception cref="ArgumentNullException">The encryptedData parameter is null.</exception>
        /// <exception cref="CryptographicException">The cryptographic protection failed.</exception>
        /// <exception cref="PlatformNotSupportedException">The operating system does not support this method.</exception>
        /// <exception cref="OutOfMemoryException">The system ran out of memory while encrypting the data.</exception>
        public static string Protect(string unencryptedData, string optionalEntropy, DataProtectionScope scope)
        {
            byte[] data = Protect(Encoding.Unicode.GetBytes(unencryptedData), optionalEntropy, scope);
            return Convert.ToBase64String(data);
        }

        /// <summary>
        ///     Protects the specified data by applying an encryption algorthim specified by the <see cref="DataProtectionScope" />
        ///     .
        /// </summary>
        /// <param name="unencryptedData">The unencrypted data.</param>
        /// <param name="optionalEntropy">The optional entropy.</param>
        /// <param name="scope">The scope.</param>
        /// <returns>The encrypted data in a byte array.</returns>
        /// <exception cref="ArgumentNullException">The encryptedData parameter is null.</exception>
        /// <exception cref="CryptographicException">The cryptographic protection failed.</exception>
        /// <exception cref="PlatformNotSupportedException">The operating system does not support this method.</exception>
        /// <exception cref="OutOfMemoryException">The system ran out of memory while encrypting the data.</exception>
        public static byte[] Protect(byte[] unencryptedData, string optionalEntropy, DataProtectionScope scope)
        {
            byte[] salt = (!string.IsNullOrEmpty(optionalEntropy)) ? Encoding.Unicode.GetBytes(optionalEntropy) : null;
            byte[] data = ProtectedData.Protect(unencryptedData, salt, scope);
            return data;
        }

        /// <summary>
        ///     Unprotects the specified value by applying an decryption algorthim specified using the
        ///     <see cref="DataProtectionScope.LocalMachine" /> scope.
        /// </summary>
        /// <param name="encryptedData">The encrypted data.</param>
        /// <returns>
        ///     The unencrypted data value, otherwise the original encrypted string value.
        /// </returns>
        /// <exception cref="ArgumentNullException">The encryptedData parameter is null.</exception>
        /// <exception cref="CryptographicException">The cryptographic protection failed.</exception>
        /// <exception cref="PlatformNotSupportedException">The operating system does not support this method.</exception>
        /// <exception cref="OutOfMemoryException">Out of memory.</exception>
        public static string Unprotect(string encryptedData)
        {
            return Unprotect(encryptedData, string.Empty, DataProtectionScope.LocalMachine);
        }

        /// <summary>
        ///     Unprotects the specified value by applying an decryption algorthim specified using the
        ///     <see cref="DataProtectionScope.LocalMachine" /> scope.
        /// </summary>
        /// <param name="encryptedData">The encrypted data.</param>
        /// <param name="optionalEntropy">The optional entropy.</param>
        /// <returns>
        ///     The unencrypted data value, otherwise the original encrypted string value.
        /// </returns>
        /// <exception cref="ArgumentNullException">The encryptedData parameter is null.</exception>
        /// <exception cref="CryptographicException">The cryptographic protection failed.</exception>
        /// <exception cref="PlatformNotSupportedException">The operating system does not support this method.</exception>
        /// <exception cref="OutOfMemoryException">Out of memory.</exception>
        public static string Unprotect(string encryptedData, string optionalEntropy)
        {
            return Unprotect(encryptedData, optionalEntropy, DataProtectionScope.LocalMachine);
        }

        /// <summary>
        ///     Unprotects the specified encrypted data by applying an decryption algorthim specified by the
        ///     <see cref="DataProtectionScope" />.
        /// </summary>
        /// <param name="encryptedData">The encrypted data.</param>
        /// <param name="optionalEntropy">The optional entropy.</param>
        /// <param name="scope">The scope.</param>
        /// <returns>
        ///     The unencrypted byte array, otherwise the original encrypted byte array.
        /// </returns>
        /// <exception cref="ArgumentNullException">The encryptedData parameter is null.</exception>
        /// <exception cref="CryptographicException">The cryptographic protection failed.</exception>
        /// <exception cref="PlatformNotSupportedException">The operating system does not support this method.</exception>
        /// <exception cref="OutOfMemoryException">Out of memory.</exception>
        public static string Unprotect(string encryptedData, string optionalEntropy, DataProtectionScope scope)
        {
            byte[] data = Unprotect(Convert.FromBase64String(encryptedData), optionalEntropy, scope);
            return Encoding.Unicode.GetString(data);
        }

        /// <summary>
        ///     Unprotects the specified encrypted data by applying an decryption algorthim specified by the
        ///     <see cref="DataProtectionScope" />.
        /// </summary>
        /// <param name="encryptedData">The encrypted data.</param>
        /// <param name="optionalEntropy">The optional entropy.</param>
        /// <param name="scope">The scope.</param>
        /// <returns>
        ///     The unencrypted byte array, otherwise the original encrypted byte array.
        /// </returns>
        /// <exception cref="ArgumentNullException">The encryptedData parameter is null.</exception>
        /// <exception cref="CryptographicException">The cryptographic protection failed.</exception>
        /// <exception cref="PlatformNotSupportedException">The operating system does not support this method.</exception>
        /// <exception cref="OutOfMemoryException">Out of memory.</exception>
        public static byte[] Unprotect(byte[] encryptedData, string optionalEntropy, DataProtectionScope scope)
        {
            byte[] salt = (!string.IsNullOrEmpty(optionalEntropy)) ? Encoding.Unicode.GetBytes(optionalEntropy) : null;
            byte[] data = ProtectedData.Unprotect(encryptedData, salt, scope);
            return data;
        }

        #endregion
    }
}