using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace MvcBrownListAPI
{
    public class clsDEN
    {
    }

    public class RequestStatus
    {
        public int Code { get; set; }
        public string Data { get; set; }
        public string Message { get; set; }
    }

    public class UserDetail
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Alias { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Question1 { get; set; }
        public string Answer1 { get; set; }
        public string Question2 { get; set; }
        public string Answer2 { get; set; }
    }

    public class UserFile
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public byte[] FileContent { get; set; }
        public int AccountId { get; set; }
        public DateTime TimeStamp { get; set; }
    }

    public class ComplaintDetail
    {
        public int ComplaintId { get; set; }
        public int AccountId { get; set; }
        public int EntityId { get; set; }
        public int LocationTypeId { get; set; }
        public string Location { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public UserFile UserFile { get; set; }
    }

    public class PostingDetail
    {
        public int PostingId { get; set; }
        public int PostingTypeId { get; set; }
        public int ComplaintId { get; set; }
        public int ParentPostingId { get; set; }
        public int AccountId { get; set; }
        public int EntityId { get; set; }
        public string Description { get; set; }
        public UserFile UserFile { get; set; }
    }

    public class AppConstants
    {
        public const int SUCCESS_CODE = 200;
        public const int FAILED_CODE = 500;
        public const int INVALID_EMAIL_CODE = 201;
        public const int PASSWORD_MISMATCH_CODE = 202;
        public const int EMAIL_EXIST_CODE = 203;
        public const int REQUIRED_FIELDS_CODE = 205;
        public const int FATAL_ERROR_CODE = 500;

        public const string SUCCESS_MESSAGE = "Success";
        public const string FAILED_MESSAGE = "Failed";
        public const string INVALID_EMAIL_MESSAGE = "Invalid Email";
        public const string PASSWORD_MISMATCH_MESSAGE = "Password and Confirm Password do not match";
        public const string EMAIL_EXIST_MESSAGE = "Email already exists";
        public const string REQUIRED_FIELDS_MESSAGE = "Firstname or Lastname or Email or Password is empty";
        public const string FATAL_ERROR_MESSAGE = "Fatal Error";

        public const string EMAIL_MATCH_PATTERN = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
    }
}

//For JavaScriptSerializer
namespace ExtensionMethods
{
    public static class JSONHelper
    {
        public static string ToJSON(this object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = 2147483647;
            return serializer.Serialize(obj);
        }

        public static string ToJSON(this object obj, int recursionDepth)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = 2147483647;
            serializer.RecursionLimit = recursionDepth;
            return serializer.Serialize(obj);
        }
    }
}

//For Encrypt/Decrypt
namespace EncryptString
{
    public static class StringCipher
    {
        // This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        private const string initVector = "tu89geji340t89u2";

        // This constant is used to determine the keysize of the encryption algorithm.
        private const int keysize = 256;
        private const string passPhrase = "passwordKey!23";

        public static string Encrypt(string plainText)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }

        public static string Decrypt(string cipherText)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }
    }
}