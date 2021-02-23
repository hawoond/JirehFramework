using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

/// <summary>
/// 암호화 클래스
/// </summary>
public class Encryption
{
    public const string KEY = "12345678901234567890123456789012";
    public Encryption()
    {
    }
    /// <summary>
    /// AES_256 암호화
    /// </summary>
    /// <param name="Input"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public String AESEncrypt256(String Input, String key)
    {
        // 암호화 세부 설정
        RijndaelManaged rmAes = new RijndaelManaged();
        rmAes.KeySize = 256;
        rmAes.BlockSize = 128;
        rmAes.Mode = CipherMode.CBC;
        rmAes.Padding = PaddingMode.PKCS7;
        rmAes.Key = Encoding.UTF8.GetBytes(key);

        //대칭키
        rmAes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        // 나는 암호화
        var encrypt = rmAes.CreateEncryptor(rmAes.Key, rmAes.IV);
        byte[] xBuff = null;
        using (var ms = new MemoryStream())
        {
            using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
            {
                byte[] xXml = Encoding.UTF8.GetBytes(Input);
                cs.Write(xXml, 0, xXml.Length);
            }

            xBuff = ms.ToArray();
        }

        String Output = Convert.ToBase64String(xBuff);
        return Output;
    }

    /// <summary>
    /// AES_256 복호화
    /// </summary>
    /// <param name="Input"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public String AESDecrypt256(String Input, String key)
    {
        // 복호화 세부 설정
        RijndaelManaged rmAes = new RijndaelManaged();
        rmAes.KeySize = 256;
        rmAes.BlockSize = 128;
        rmAes.Mode = CipherMode.CBC;
        rmAes.Padding = PaddingMode.PKCS7;
        rmAes.Key = Encoding.UTF8.GetBytes(key);

        // 대칭키
        rmAes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        // 나는 복호화
        var decrypt = rmAes.CreateDecryptor();
        byte[] xBuff = null;
        using (var ms = new MemoryStream())
        {
            using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
            {
                byte[] xXml = Convert.FromBase64String(Input);
                cs.Write(xXml, 0, xXml.Length);
            }

            xBuff = ms.ToArray();
        }

        String Output = Encoding.UTF8.GetString(xBuff);
        return Output;
    }

}