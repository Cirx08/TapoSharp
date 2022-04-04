namespace TapoSharp.Models
{
    public class KeyPair
    {
        public KeyPair()
            : this(string.Empty, string.Empty)
        {
        }

        public KeyPair(string publicKey, string privateKey)
        {
            this.PublicKey = publicKey;
            this.PrivateKey = privateKey;
        }

        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }
    }
}