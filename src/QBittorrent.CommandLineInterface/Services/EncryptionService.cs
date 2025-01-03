using System;

namespace QBittorrent.CommandLineInterface.Services
{
    public abstract class EncryptionService
    {

        static EncryptionService()
        {
            Instance = OperatingSystem.IsWindows()
                ? new WindowsEncryptionService()
                : new UnixEncryptionService();
        }

        private protected EncryptionService()
        {
        }

        public static EncryptionService Instance { get; }

        public abstract string Encrypt(string input);

        public abstract string Decrypt(string input);

        public virtual void ResetKey()
        {
        }
    }
}
