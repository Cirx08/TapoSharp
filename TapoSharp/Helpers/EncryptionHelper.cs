namespace TapoSharp.Helpers
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Encodings;
    using Org.BouncyCastle.Crypto.Engines;
    using Org.BouncyCastle.Crypto.Generators;
    using Org.BouncyCastle.OpenSsl;
    using Org.BouncyCastle.Security;
    using TapoSharp.Models;

    public class EncryptionHelper
    {
        private const int KeySize = 1024;

        public static KeyPair GenerateKeyPair()
        {
            try
            {
                var r = new RsaKeyPairGenerator();
                r.Init(new KeyGenerationParameters(new SecureRandom(), KeySize));

                var keyPair = r.GenerateKeyPair();
                var publicKey = string.Empty;
                using (var stream = new MemoryStream())
                {
                    var textWriter = new StreamWriter(stream);
                    var pemWriter = new PemWriter(textWriter);
                    pemWriter.WriteObject(keyPair.Public);
                    pemWriter.Writer.Flush();
                    stream.Position = 0;
                    var sr = new StreamReader(stream);
                    publicKey = sr.ReadToEnd();
                    textWriter.Close();
                }

                var privateKey = string.Empty;
                using (var stream = new MemoryStream())
                {
                    var textWriter = new StreamWriter(stream);
                    var pemWriter = new PemWriter(textWriter);
                    pemWriter.WriteObject(keyPair.Private);
                    pemWriter.Writer.Flush();

                    stream.Position = 0;
                    var sr = new StreamReader(stream);
                    privateKey = sr.ReadToEnd();
                    textWriter.Close();
                }

                return new KeyPair(publicKey, privateKey);
            }
            catch
            {
                return new KeyPair();
            }
        }

        public static string Encode(string payload, bool hash = false)
        {
            try
            {
                if (hash)
                {
                    payload = Hash(payload);
                }

                byte[] bytes = Encoding.UTF8.GetBytes(payload);
                var data = Convert.ToBase64String(bytes);

                return data;
            }
            catch 
            {
                return string.Empty;
            }
        }

        public static string Decode(string payload)
        {
            try
            {
                var data = Convert.FromBase64String(payload);
                return Encoding.UTF8.GetString(data);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string Hash(string payload)
        {
            try
            { 
                var hash = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(payload));
                return string.Concat(hash.Select(b => b.ToString("x2")));
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string Encrypt(string payload, byte[] key, byte[] iv)
        {
            if (!string.IsNullOrEmpty(payload) && key != null && iv != null)
            {
                try
                {
                    var data = string.Empty;

                    using (var aes = new AesManaged())
                    {
                        aes.Key = key;
                        aes.IV = iv;

                        var encryptor = aes.CreateEncryptor();
                        using (var ms = new MemoryStream())
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (var sw = new StreamWriter(cs))
                            {
                                sw.Write(payload);
                            }
                            data = Convert.ToBase64String(ms.ToArray());
                        }
                    }

                    return data;
                }
                catch { }
            }
                    
            return string.Empty;
        }

        public static string Decrypt(string payload, byte[] key, byte[] iv)
        {
            if (!string.IsNullOrEmpty(payload) && key != null && iv != null)
            {
                try
                {
                    var data = string.Empty;

                    using (var aes = new AesManaged())
                    {
                        aes.Key = key;
                        aes.IV = iv;

                        var decryptor = aes.CreateDecryptor();
                        using (var ms = new MemoryStream(Convert.FromBase64String(payload)))
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (var sr = new StreamReader(cs))
                            {
                                data = sr.ReadToEnd();
                            }
                        }
                    }

                    return data;
                }
                catch { }
            }

            return string.Empty;
        }

        public static byte[] RsaDecryption(string payload, string privateKey)
        {
            try
            {
                byte[] cipherTextBytes = Convert.FromBase64String(payload);

                PemReader pr = new PemReader(new StringReader(privateKey));
                AsymmetricCipherKeyPair keys = (AsymmetricCipherKeyPair)pr.ReadObject();

                Pkcs1Encoding eng = new Pkcs1Encoding(new RsaEngine());
                eng.Init(false, keys.Private);

                int length = cipherTextBytes.Length;
                int blockSize = eng.GetInputBlockSize();

                List<byte> plainTextBytes = new List<byte>();
                for (int chunkPosition = 0; chunkPosition < length; chunkPosition += blockSize)
                {
                    int chunkSize = Math.Min(blockSize, length - chunkPosition);
                    plainTextBytes.AddRange(eng.ProcessBlock(cipherTextBytes, chunkPosition, chunkSize));
                }

                return plainTextBytes.ToArray();
            }
            catch { }

            return new byte[0];
        }
    }
}