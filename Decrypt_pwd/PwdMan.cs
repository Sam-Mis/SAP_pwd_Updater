using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Decrypt_pwd
{
    public class PwdMan
    {
        public List<string> PwdList { get; set; } 
        public string CurrentPwd { get; set; }
        public string NextPwd { get; set; }

        public PwdMan(string path)
        {
            PwdList = new List<string>();
            PwdList.AddRange(File.ReadAllLines(path));
            CurrentPwd = PwdList.First(p => p.EndsWith("_"));
            NextPwd = PwdList.SkipWhile(x => x != CurrentPwd).Skip(1).DefaultIfEmpty(PwdList[0]).FirstOrDefault();
            CurrentPwd = CurrentPwd.TrimEnd('_');
        }
        public void UpdatePwdList(List<string> pwdList, string oldpwd, string newpwd, string path)
        {
            var newinx = pwdList.FindIndex(pwd => pwd.Equals(newpwd));
            pwdList.ForEach(pwd => pwd.TrimEnd('_'));
            pwdList[newinx] = newpwd + "_";
            File.WriteAllLines(path, pwdList);
        }

        public static string EncryptPwd(string key, string pwd)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aes.CreateEncryptor();

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(pwd);

                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }
        public static string Decrypt(Stream input, string key)
        {
            byte[] iv = new byte[16];
            string result;
            Aes aes = Aes.Create();

            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;
            aes.Padding = PaddingMode.PKCS7;
            //aes.Mode = CipherMode.CBC;
            //aes.BlockSize = 128;

            ICryptoTransform aesDecryptor = aes.CreateDecryptor();

            using (CryptoStream cryptoStream = new(input, aesDecryptor, CryptoStreamMode.Read))
            {
                using (StreamReader reader = new StreamReader(cryptoStream))
                {
                     result = reader.ReadToEnd();
                    reader.Close();
                }
                cryptoStream.Close();
            }
            return result;
        }

        public static string DecryptPwd(string key, string pwd)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(pwd);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
