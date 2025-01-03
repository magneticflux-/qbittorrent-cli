using System;

namespace QBittorrent.CommandLineInterface.Services
{
    public abstract class EncryptionService
    {
        public static EncryptionService Instance { get; }

        static EncryptionService()
        {
            Instance = OperatingSystem.IsWindows() 
                ? (EncryptionService) new WindowsEncryptionService()
                : new UnixEncryptionService();
        }

        private protected EncryptionService()
        {
        }

        public abstract string Encrypt(string input);

        public abstract string Decrypt(string input);

        public virtual void ResetKey()
        {
        }
    }
}
