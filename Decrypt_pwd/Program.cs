using Decrypt_pwd;

var key = "SUP3RC4L!FR4G!L!ST!C3XP!R4L!D0S0";

string? crypted = File.ReadLines(args[0]).FirstOrDefault(line => line.EndsWith("_"));
if (crypted == null)
{
    Console.Write("nothing to decrypt");
}
else
{
    crypted = crypted.TrimEnd('_');
    Console.Write(PwdMan.DecryptPwd(key, crypted));
}