using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Hadvida.Shared.Managers
{
    public class AESManager : IDisposable
    {
        private Aes _aesAlg;

        private byte[] key;
        private byte[] IV;
        private int size;
        private bool disposedValue;
        private readonly X509Certificate2 certificate;

        public AESManager()
        {
            //We can move This Key into the db
            //256-bit key
            key = Encoding.UTF8.GetBytes("OAhf9Vy69X3hq3GJWbsNKySLuV7Zs3qU");
            //128-bit Initialization Vector.
            IV = Encoding.UTF8.GetBytes("nN3FFcbZ4qawGpza");

            _aesAlg = Aes.Create();
            _aesAlg.KeySize = 256;
            _aesAlg.Key = key;
            _aesAlg.IV = IV;
            _aesAlg.Mode = CipherMode.CBC;
        }

        public AESManager(byte[] key, byte[] IV)
        {
            this.key = key;
            this.IV = IV;
        }

        public AESManager(X509Certificate2 certificate)
        {
            this.certificate = certificate;
            // Derive key and IV from certificate's private key
            if (certificate.HasPrivateKey)
            {
                _aesAlg = Aes.Create();
                _aesAlg.Mode = CipherMode.CBC;
                _aesAlg.KeySize = 256;
                IV = new byte[16];
                key = new byte[32];
                RSA privateKey = certificate.GetRSAPrivateKey();
                RSAParameters privateKeyParams = privateKey.ExportParameters(true);
                Array.Copy(privateKeyParams.InverseQ, 0, IV, 0, 16);
                Array.Copy(privateKeyParams.Modulus, 0, key, 0, 32);
                _aesAlg.IV = IV;
                _aesAlg.Key = key;
            }
            else
            {
                throw new InvalidOperationException("The certificate does not have a private key.");
            }
        }

        //Encrypt raw file byte data
        public byte[] Encrypt(byte[] data)
        {
            // Create AES encryptor using key and IV provided
            ICryptoTransform encryptor = _aesAlg.CreateEncryptor(_aesAlg.Key, _aesAlg.IV);

            // Create MemoryStream and CryptoStream to encrypt data
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(data, 0, data.Length);
                    csEncrypt.FlushFinalBlock();

                    return msEncrypt.ToArray();
                }
            }
        }

        //Decrypt raw file byte data
        public byte[] Decrypt(byte[] data)
        {
            // Create AES decryptor using key and IV provided
            ICryptoTransform decryptor = _aesAlg.CreateDecryptor(_aesAlg.Key, _aesAlg.IV);
            using (MemoryStream msDecrypt = new MemoryStream(data))
            {
                // Create MemoryStream and CryptoStream to decrypt data
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    byte[] fromEncrypt = new byte[data.Length];

                    csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

                    return fromEncrypt;
                }
            }
        }

        public string BytesToString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }


        // Encrypt using Aes 
        // Takes plainText as input and encrypts it using AES
        // Returns the encrypted string
        public string EncryptString(string plainText)
        {
            byte[] array;
            // Create the encryptor    
            ICryptoTransform encryptor = _aesAlg.CreateEncryptor(_aesAlg.Key, _aesAlg.IV);

            // Create the streams used for encryption
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        // Write the plaintext to the stream
                        streamWriter.Write(plainText);
                    }

                    // Get the encrypted data from the MemoryStream
                    array = memoryStream.ToArray();
                }
            }

            // Return the encrypted string as base-64 string 
            // return System.Text.Encoding.UTF8.GetString(array);
            return Convert.ToBase64String(array);
        }

        // Decrypt using Aes 
        // Takes an encrypted string and returns the decrypted string
        public string DecryptString(string encryptedText)
        {
            try
            {
                // Convert the encrypted text to bytes
                byte[] buffer = Convert.FromBase64String(encryptedText);

                // Create the decryptor
                ICryptoTransform decryptor = _aesAlg.CreateDecryptor(_aesAlg.Key, _aesAlg.IV);

                // Create the streams used for decryption
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and return the string
                            return streamReader.ReadToEnd();
                        }

                        cryptoStream.FlushFinalBlock();
                    }
                }
            }
            catch
            {
                return "";
            }
        }

        public Aes getAesInstance()
        {
            return _aesAlg;
        }


        public string UsingStringConcat(string[] array)
        {
            return string.Concat(array);
        }

        public string ConvertToBase64(string strToEncoder)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(strToEncoder));
        }

        public string ConvertFromBase64String(string strToEncoder)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(strToEncoder));
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _aesAlg.Clear();
                    _aesAlg = null;
                }

                disposedValue = true;
            }
        }


        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}