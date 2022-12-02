using EncryptionApp;


var key = "SUP3RC4L!FR4G!L!ST!C3XP!R4L!D0S0";

List<string> PwdList = new List<string>();
List<string> CryptedPwdList = new List<string>();

PwdList.AddRange(File.ReadLines(args[0]));

PwdList.ForEach(pwd => CryptedPwdList.Add(PwdMan.EncryptPwd(key,pwd)));

CryptedPwdList[0] = CryptedPwdList[0] +"_";

File.WriteAllLines(args[0], CryptedPwdList);