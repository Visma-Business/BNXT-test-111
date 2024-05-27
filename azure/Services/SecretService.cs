using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace Visma.Business.WebhookExample.Services {
    public class SecretService {
        private SecureString SignatureHashKey { get; }

        public SecretService(SecureString signatureHashKey) {
            SignatureHashKey = signatureHashKey;
        }

        static byte[] GetUTF8Bytes(string input)
        {
            return Encoding.UTF8.GetBytes(input);
        }

        static HMACSHA256 CreateHMACSHA256Hasher(byte[] key)
        {
            return new HMACSHA256(key);
        }

        static byte[] Hash(HMACSHA256 hasher, byte[] message)
        {
            return hasher.ComputeHash(message);
        }

        static string GetBase64StringFromBytes(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        static String SecureStringToString(SecureString value)
        {
            IntPtr bstr = Marshal.SecureStringToBSTR(value);

            try
            {
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
            }
        }

        internal bool Validate (string message, StringValues signature)
        {
            var keyBytes = GetUTF8Bytes(SecureStringToString(SignatureHashKey));
            var hasher = CreateHMACSHA256Hasher(keyBytes);
            var messageHashBytes = Hash(hasher, GetUTF8Bytes(message));
            var validSignature = GetBase64StringFromBytes(messageHashBytes);
            return signature.Equals(validSignature);
        }
    }
}