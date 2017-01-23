using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Iwenli.WeiXin.Robot.Utility
{
    public class SecurityUtility
    {
        /// <summary>
        /// 对字符串进行SHA1加密
        /// </summary>
        /// <param name="strIN">需要加密的字符串</param>
        /// <returns>密文</returns>
        public static string SHA1Encrypt(string Source_String)
        {
            byte[] StrRes = Encoding.Default.GetBytes(Source_String);
            HashAlgorithm iSHA = new SHA1CryptoServiceProvider();
            StrRes = iSHA.ComputeHash(StrRes);
            StringBuilder EnText = new StringBuilder();
            foreach (byte iByte in StrRes)
            {
                EnText.AppendFormat("{0:x2}", iByte);
            }
            return EnText.ToString();
        }


        /// <summary>
        /// MD5 不区分大小写的
        /// </summary>
        /// <param name="pwd"></param>
        /// <param name="type">type 类型，16位还是32位，16位就是取32位的第8到16位</param>
        /// <returns>返回小写MD5密串</returns>
        public static string Md5Encode(string pwd, string type = "32")
        {
            byte[] result = Encoding.UTF8.GetBytes(pwd);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            if (type == "16")
                return BitConverter.ToString(output).Replace("-", "").ToLower().Substring(8, 16);
            else
                return BitConverter.ToString(output).Replace("-", "").ToLower();

        }
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="toEncrypt">明文</param>
        /// <param name="key">秘钥</param>
        /// <param name="iv">byte[16]偏移向量，默认为空</param>
        /// <returns></returns>
        public static string EncryptAES(string toEncrypt, string key, string iv = "\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0")
        {  
            byte[] keyArray = MD5.Create().ComputeHash(UTF8Encoding.UTF8.GetBytes(key)); //key继续MD5哈希一次
            byte[] ivArray = UTF8Encoding.UTF8.GetBytes(iv);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            byte[] resultArray = EncryptAES(toEncryptArray, keyArray, ivArray);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="toEncrypt">明文</param>
        /// <param name="key">秘钥</param>
        /// <param name="iv">byte[16]偏移向量，默认为空</param>
        /// <returns></returns>
        public static byte[] EncryptAES(byte[] toEncrypt, byte[] key, byte[] iv)
        {
            using (AesManaged aes = new AesManaged())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(toEncrypt, 0, toEncrypt.Length);
                        cs.FlushFinalBlock();
                        return (byte[])ms.ToArray();
                    }
                }
            }

        }
        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="toDecrypt">密文</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">byte[16]偏移向量，默认为空</param>
        /// <returns></returns>
        public static string DecryptAES(string toDecrypt, string key, string iv = "\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0")
        {
            byte[] keyArray = MD5.Create().ComputeHash(UTF8Encoding.UTF8.GetBytes(key)); //key继续MD5哈希一次
            byte[] ivArray = UTF8Encoding.UTF8.GetBytes(iv);
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
            byte[] resultArray = DecryptAES(toEncryptArray, keyArray, ivArray);
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="toDecrypt">密文</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">byte[16]偏移向量，默认为空</param>
        /// <returns></returns>
        public static byte[] DecryptAES(byte[] toDecrypt, byte[] key, byte[] iv)
        {
            if (toDecrypt == null || key == null) return null;
            using (AesManaged aes = new AesManaged())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(toDecrypt, 0, toDecrypt.Length);
                        cs.FlushFinalBlock();
                        return ms.ToArray();
                    }
                }
            }
        }
        ///// <summary>
        ///// SHA256加密，不可逆转
        ///// </summary>
        ///// <param name="str">string str:被加密的字符串</param>
        ///// <returns>返回加密后的字符串</returns>
        //private string SHA256Encrypt(string str)
        //{
        //    System.Security.Cryptography.SHA256 s256 = new System.Security.Cryptography.SHA256Managed();
        //    byte[] byte1;
        //    byte1 = s256.ComputeHash(Encoding.Default.GetBytes(str));
        //    s256.Clear();
        //    return Convert.ToBase64String(byte1);
        //}

        ///// <summary>
        ///// SHA384加密，不可逆转
        ///// </summary>
        ///// <param name="str">string str:被加密的字符串</param>
        ///// <returns>返回加密后的字符串</returns>
        //private string SHA384Encrypt(string str)
        //{
        //    System.Security.Cryptography.SHA384 s384 = new System.Security.Cryptography.SHA384Managed();
        //    byte[] byte1;
        //    byte1 = s384.ComputeHash(Encoding.Default.GetBytes(str));
        //    s384.Clear();
        //    return Convert.ToBase64String(byte1);
        //}


        ///// <summary>
        ///// SHA512加密，不可逆转
        ///// </summary>
        ///// <param name="str">string str:被加密的字符串</param>
        ///// <returns>返回加密后的字符串</returns>
        //private string SHA512Encrypt(string str)
        //{
        //    System.Security.Cryptography.SHA512 s512 = new System.Security.Cryptography.SHA512Managed();
        //    byte[] byte1;
        //    byte1 = s512.ComputeHash(Encoding.Default.GetBytes(str));
        //    s512.Clear();
        //    return Convert.ToBase64String(byte1);
        //}

        ////默认密钥向量
        //private static byte[] Keys = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        ///// <summary>
        ///// DES加密字符串
        ///// </summary>
        ///// <param name="encryptString">待加密的字符串</param>
        ///// <param name="encryptKey">加密密钥,要求为8位</param>
        ///// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        //public static string EncryptDES(string encryptString, string encryptKey)
        //{
        //    try
        //    {
        //        byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
        //        byte[] rgbIV = Keys;
        //        byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
        //        DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
        //        MemoryStream mStream = new MemoryStream();
        //        CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
        //        cStream.Write(inputByteArray, 0, inputByteArray.Length);
        //        cStream.FlushFinalBlock();
        //        return Convert.ToBase64String(mStream.ToArray());
        //    }
        //    catch
        //    {
        //        return encryptString;
        //    }
        //}

        ///// <summary>
        ///// DES解密字符串
        ///// </summary>
        ///// <param name="decryptString">待解密的字符串</param>
        ///// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        ///// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        //public static string DecryptDES(string decryptString, string decryptKey)
        //{
        //    try
        //    {
        //        byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey.Substring(0, 8));
        //        byte[] rgbIV = Keys;
        //        byte[] inputByteArray = Convert.FromBase64String(decryptString);
        //        DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
        //        MemoryStream mStream = new MemoryStream();
        //        CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
        //        cStream.Write(inputByteArray, 0, inputByteArray.Length);
        //        cStream.FlushFinalBlock();
        //        return Encoding.UTF8.GetString(mStream.ToArray());
        //    }
        //    catch
        //    {
        //        return decryptString;
        //    }
        //}
    }
}
