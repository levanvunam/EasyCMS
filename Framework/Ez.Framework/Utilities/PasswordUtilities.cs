using Ez.Framework.Configurations;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Ez.Framework.Utilities
{
    /// <summary>
    /// Password utilities
    /// </summary>
    public static class PasswordUtilities
    {
        #region Simple Encrypt for url

        /// <summary>
        /// Encode raw text
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Decode text
        /// </summary>
        /// <param name="base64EncodedData"></param>
        /// <returns></returns>
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        #endregion

        #region Complex Encrypt for url

        /// <summary>
        /// Encrypt url using MD5 and Base64
        /// </summary>
        /// <param name="toEncrypt"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ComplexEncrypt(string toEncrypt, string key = "")
        {
            var encryptString = EncryptString(toEncrypt, key);
            return Base64Encode(encryptString);
        }

        /// <summary>
        /// Decrypt url using MD5 and Base64
        /// </summary>
        /// <param name="cipherString"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ComplexDecrypt(string cipherString, string key = "")
        {
            var base64String = Base64Decode(cipherString);
            return DecryptString(base64String, key);
        }

        /// <summary>
        /// Generate random string
        /// </summary>
        /// <param name="lenght"></param>
        /// <returns></returns>
        public static string GetRandomString(int lenght = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(
                Enumerable.Repeat(chars, lenght)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
        }

        /// <summary>
        /// Encrypt string
        /// </summary>
        /// <param name="toEncrypt"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string EncryptString(string toEncrypt, string key = "")
        {
            var toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

            var hashmd5 = new MD5CryptoServiceProvider();
            var keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
            //Always release the resources and flush data
            //of the Cryptographic service provide. Best Practice

            hashmd5.Clear();

            var tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,              //set the secret key for the tripleDES algorithm
                Mode = CipherMode.ECB,       //mode of operation. there are other 4 modes. We choose ECB(Electronic code Book)
                Padding = PaddingMode.PKCS7  //padding mode(if any extra byte added)
            };

            var cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            var resultArray = cTransform.TransformFinalBlock
                    (toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// Descrypt string
        /// </summary>
        /// <param name="cipherString"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DecryptString(string cipherString, string key = "")
        {
            var toEncryptArray = Convert.FromBase64String(cipherString);

            //if hashing was used get the hash code with regards to your key
            var hashmd5 = new MD5CryptoServiceProvider();
            var keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();

            var tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,               //set the secret key for the tripleDES algorithm
                Mode = CipherMode.ECB,        //mode of operation. there are other 4 modes. We choose ECB(Electronic code Book)
                Padding = PaddingMode.PKCS7   //padding mode(if any extra byte added)
            };

            var cTransform = tdes.CreateDecryptor();
            var resultArray = cTransform.TransformFinalBlock
                    (toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //return the Clear decrypted TEXT
            return Encoding.UTF8.GetString(resultArray);
        }

        #endregion

        #region Password Hash

        /// <summary>
        /// Generate passwword
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordSalt"></param>
        /// <param name="passwordHash"></param>
        public static void GeneratePassword(this string password, out string passwordSalt, out string passwordHash)
        {
            passwordSalt = GetPasswordSalt();
            passwordHash = CreateHashPassword(password, passwordSalt);
        }

        /// <summary>
        /// Get password salt
        /// </summary>
        /// <returns></returns>
        public static string GetPasswordSalt()
        {
            string saltKey = GetRandomString();
            return saltKey;
        }

        /// <summary>
        /// Create hash password from raw password and salt key
        /// </summary>
        /// <param name="pass"></param>
        /// <param name="saltKey"></param>
        /// <returns></returns>
        public static string CreateHashPassword(string pass, string saltKey)
        {
            var md5 = new MD5CryptoServiceProvider();
            var passHash = md5.ComputeHash(Encoding.UTF8.GetBytes(string.Format("{0}{1}", saltKey, pass)));
            return Convert.ToBase64String(passHash, 0, passHash.Length);
        }

        public static bool ValidatePass(string passHash, string pass, string saltKey)
        {
            var comparePass = CreateHashPassword(pass, saltKey);
            if (passHash.Equals(comparePass))
                return true;
            else return false;
        }

        #endregion

        /// <summary>
        /// Generate randomKey
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateUniqueKey(int length = 20)
        {
            if (length < FrameworkConstants.UniqueDateTimeFormat.Length + 8)
                length = FrameworkConstants.UniqueDateTimeFormat.Length + 8;

            var randomKey = GetRandomString(length - FrameworkConstants.UniqueDateTimeFormat.Length);

            var now = DateTime.UtcNow.ToString(FrameworkConstants.UniqueDateTimeFormat);

            return randomKey + now;
        }
    }
}