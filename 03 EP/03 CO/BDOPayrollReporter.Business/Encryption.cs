using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BDOPayrollReporter.Business
{
    /// <summary>
    /// Computes a basic standard encryption.
    /// </summary>
    public class Encryption
    {
        private static byte[] key = { };
        private static byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        private static string EncryptionKey = "!5623a#de";

        /// <summary>
        /// Desencripta un valor en especifico.
        /// </summary>
        /// <param name="Input">Valor a Desencriptar</param>
        /// <returns>Devuelve un valor de tipo <see cref="System.String"/> con el resultado</returns>
        public static string Decrypt(string Input)
        {
            Byte[] inputByteArray = new Byte[Input.Length];
            Encoding encoding = Encoding.UTF8;
            MemoryStream ms = new MemoryStream();

            try
            {
                key = System.Text.Encoding.UTF8.GetBytes(EncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();

                inputByteArray = HexEncoding.GetBytes(Input, 0);
                //inputByteArray = Convert.FromBase64String(Input);

                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
            }
            catch (Exception oe)
            {
                throw oe;
            }

            return encoding.GetString(ms.ToArray());
        }

        /// <summary>
        /// Encripta un valor especifico.
        /// </summary>
        /// <param name="Input">Valor a Encriptar</param>
        /// <returns>Devuelve un valor de tipo <see cref="System.String"/> con el resultado</returns>
        public static string Encrypt(string Input)
        {
            MemoryStream ms = new MemoryStream();

            try
            {
                key = System.Text.Encoding.UTF8.GetBytes(EncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                Byte[] inputByteArray = Encoding.UTF8.GetBytes(Input);
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                //return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception oe)
            {
                throw oe;
            }
            return HexEncoding.ToString(ms.ToArray());
        }
    }
}