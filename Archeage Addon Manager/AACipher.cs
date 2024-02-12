using System;
using System.Security.Cryptography;
using System.Text;

namespace Archeage_Addon_Manager {
    public class AACipher {
        static byte[] KEY = Encoding.ASCII.GetBytes("Archeage!(*!");

        public static string Encrypt(string text) {
            int padding = 8 - (text.Length % 8);

            byte[] textBytes = Encoding.ASCII.GetBytes(text);
            byte[] paddedBytes = new byte[textBytes.Length + padding];
            Array.Copy(textBytes, paddedBytes, textBytes.Length);
            for (int i = textBytes.Length; i < paddedBytes.Length; i++) {
                paddedBytes[i] = (byte)padding;
            }

            Array.Reverse(paddedBytes);

            using (var cipher = DES.Create()) {
                cipher.Mode = CipherMode.ECB;
                cipher.Padding = PaddingMode.None;
                cipher.Key = KEY;

                ICryptoTransform transform = cipher.CreateEncryptor();

                byte[] encryptedBytes = transform.TransformFinalBlock(paddedBytes, 0, paddedBytes.Length);

                Array.Reverse(encryptedBytes);

                return BitConverter.ToString(encryptedBytes).Replace("-", "");
            }
        }

        public static string Decrypt(string ciphertext) {
            byte[] cipherBytes = new byte[ciphertext.Length / 2];
            for (int i = 0; i < ciphertext.Length; i += 2) {
                cipherBytes[i / 2] = Convert.ToByte(ciphertext.Substring(i, 2), 16);
            }

            Array.Reverse(cipherBytes);

            using (var decipher = DES.Create()) {
                decipher.Mode = CipherMode.ECB;
                decipher.Padding = PaddingMode.None;
                decipher.Key = KEY;

                ICryptoTransform transform = decipher.CreateDecryptor();

                byte[] decryptedBytes = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

                Array.Reverse(decryptedBytes);

                int padding = decryptedBytes[decryptedBytes.Length - 1];

                if (padding < 1 || padding > 8) {
                    throw new Exception("Invalid padding");
                }

                return Encoding.ASCII.GetString(decryptedBytes, 0, decryptedBytes.Length - padding);
            }
        }
    }
}
