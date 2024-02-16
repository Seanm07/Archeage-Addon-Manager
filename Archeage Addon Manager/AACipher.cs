using Elskom.Generic.Libs;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

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

            BlowFish b = new BlowFish(KEY);
            byte[] encryptedBytes = b.EncryptECB(paddedBytes);

            Array.Reverse(encryptedBytes);

            return BitConverter.ToString(encryptedBytes).Replace("-", "");

            /*using (var cipher = DES.Create()) {
                cipher.Mode = CipherMode.ECB;
                cipher.Padding = PaddingMode.None;
                cipher.Key = KEY;

                ICryptoTransform transform = cipher.CreateEncryptor();

                byte[] encryptedBytes = transform.TransformFinalBlock(paddedBytes, 0, paddedBytes.Length);

                Array.Reverse(encryptedBytes);

                return BitConverter.ToString(encryptedBytes).Replace("-", "");
            }*/
        }

        public static string Decrypt(string input) {
            // Convert the input string to a byte array
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);

            // Reverse the byte array for endianess
            Array.Reverse(inputBytes);

            // Using BlowFish in ebc mode, with key KEY and null initialization vector
            BlowFish b = new BlowFish(KEY);
            //b.IV = null;

            byte[] decryptedBytes = b.Decrypt(inputBytes, CipherMode.ECB);

            // Restore the byte order
            Array.Reverse(decryptedBytes);

            // Read the last byte of the decrypted data to get the padding
            int padding = decryptedBytes[decryptedBytes.Length - 1];

            if (padding < 1 || decryptedBytes.Length > 8)
                throw new Exception("Invalid padding");

            return Encoding.ASCII.GetString(decryptedBytes, 0, decryptedBytes.Length - padding);

            /*byte[] ciphertextBytes = StringToByteArray(ciphertext);

            BlowFish blowfish = new BlowFish(KEY);

            byte[] decryptedBytes = blowfish.Decrypt(ciphertextBytes, CipherMode.ECB);

            int padding = decryptedBytes[decryptedBytes.Length - 1];

            if (padding < 1 || decryptedBytes.Length > 8)
                throw new Exception("Invalid padding");

            byte[] unpaddedBytes = new byte[decryptedBytes.Length - padding];
            Array.Copy(decryptedBytes, unpaddedBytes, unpaddedBytes.Length);

            return Encoding.ASCII.GetString(unpaddedBytes);*/
        }
    }
}
