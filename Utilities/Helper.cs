using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Configuration;
using TY.SPIMS.Entities;
using System.Data.Objects;

namespace TY.SPIMS.Utilities
{
    public class Helper
    {
        #region Encryption

        public static string EncryptString(string toEncrypt)
        {
            byte[] result;
            string passCode = "Aerich123$";
            UTF8Encoding utf8 = new UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider hashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = hashProvider.ComputeHash(utf8.GetBytes(passCode));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the encoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToEncrypt = utf8.GetBytes(toEncrypt);

            // Step 5. Attempt to encrypt the string
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                result = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                hashProvider.Clear();
            }

            // Step 6. Return the encrypted string as a base64 encoded string
            return Convert.ToBase64String(result);
        }

        public static string DecryptString(string toDecrypt)
        {
            byte[] result;
            string passCode = "Aerich123$";
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(passCode));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the decoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToDecrypt = Convert.FromBase64String(toDecrypt);

            // Step 5. Attempt to decrypt the string
            try
            {
                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                result = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the decrypted string in UTF8 format
            return UTF8.GetString(result);
        }

        #endregion

        #region App Settings

        public static string GetServerIP()
        {
            string serverIP = "127.0.0.1";

            if (ConfigurationManager.AppSettings["ServerIP"] != null)
                if (ConfigurationManager.AppSettings["ServerIP"] != string.Empty)
                    serverIP = ConfigurationManager.AppSettings["ServerIP"];

            return serverIP;
        }

        #endregion

        #region Notification

        public static void AddNotification(string message)
        {
            try
            {
                var db = ConnectionManager.Instance.Connection;

                Notification n = new Notification() { NotificationMessage = message, NotificationDate = DateTime.Now };
                db.AddToNotification(n);

                db.SaveChanges(SaveOptions.AcceptAllChangesAfterSave);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<Notification> GetAllNotifications()
        {
            try
            {
                var db = ConnectionManager.Instance.Connection;

                List<Notification> notifications = db.Notification
                    .OrderByDescending(a => a.NotificationDate)
                    .ToList();

                return notifications;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
