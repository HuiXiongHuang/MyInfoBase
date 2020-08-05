using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PublicLibrary.Security.AsymmetricEncryption
{
    public class RSAHelper
    {
        /// <summary>
        /// 在当前目录下创建密钥
        /// </summary>
        public static void  CreateKey()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            using (StreamWriter writer = new StreamWriter("PrivateKey.xml", false))  //这个文件要保密...
            {
                writer.WriteLine(rsa.ToXmlString(true));
            }
            using (StreamWriter writer = new StreamWriter("PublicKey.xml", false))
            {
                writer.WriteLine(rsa.ToXmlString(false));
            }
        }
        static string PublicKey = @"<RSAKeyValue><Modulus>q5HaSn9cxTb3277jcssoF1/P0Zr8TcE8GVrMsFXXi0OYp3p7lOowlnZjmB/r70IJcRKNAo+JtzBRXpiaaUg2IZm2NTDqgwsb8Sn74Uz7dtCteLIdtyMGFuojA+c9SLGDqAkR+4TsZIMV2sYCHYpptywYROzscg2RLULn/iFzCO0=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        static string PrivateKey = @"<RSAKeyValue><Modulus>q5HaSn9cxTb3277jcssoF1/P0Zr8TcE8GVrMsFXXi0OYp3p7lOowlnZjmB/r70IJcRKNAo+JtzBRXpiaaUg2IZm2NTDqgwsb8Sn74Uz7dtCteLIdtyMGFuojA+c9SLGDqAkR+4TsZIMV2sYCHYpptywYROzscg2RLULn/iFzCO0=</Modulus><Exponent>AQAB</Exponent><P>1FTtRsOnUfs0Bs3fVe0KOAaN2FfKKzeOX7WhWjxgpS5xljRAfzJqR46g3Gifs7gFmnZsiGHM8QPtSJzvufDfuw==</P><Q>ztrX26OmawNC3OHrL+qwtumbxKgqcIJ5j9o/FALoEHet89e5sZRBEGnuLfYwyUNGdhVndqEpF4qw7Yl0MboLdw==</Q><DP>l9VjFwcxzt3jBjqRSdCHTijhpaKhXuYLWUV4bB5gvb3IW7BhrpNOjHzharsl+E3PM/UbBytocbBtLU+L3VrxoQ==</DP><DQ>DBayAr3/ncVlBO+XBQfcJ/RjDA0f3c9iN0vz38GDumKkIdn7misl2kW2i60VkM2Dsqbxkvc8JYoPaPJdfgJ3ZQ==</DQ><InverseQ>Zkc6nkVwNZYHFVSINZUs+/l+PLKK1YGByvhnkOANBtspRBwrDEQYiPigsx1QO2n1qEXY7DjDL8WgtJt91RtMjw==</InverseQ><D>K8d87zOOpZhAKOeNvFYQtd4x9mdY6DJdto6P2pMc/CFUi6/aZwZXPZ2aDlBe8N9l719vce7UklUW/k9sX18YyJEsft2FJ432QaQ8eDd6a8/PcaWOwXuqscvIze1qyu4m+lSHYMFrGtPgltmbcubxfQ07ifl7cbUJkR0NLd95X/U=</D></RSAKeyValue>";

        static RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(1024);
       
        /// <summary>
        /// 给数据加密
        /// </summary>
        /// <param name="data">需加密的数据源</param>
        /// <returns></returns>
        public static byte[] EncryptData(byte[] data)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);
            //将公钥导入到RSA对象中，准备加密；
            rsa.FromXmlString(PublicKey);
            //对数据data进行加密，并返回加密结果；
            //第二个参数用来选择Padding的格式
            byte[] buffer = rsa.Encrypt(data, false);
            return buffer;
        }
        /// <summary>
        /// 给文件解密
        /// </summary>
        /// <param name="data">需解密的数据源</param>
        /// <returns></returns>
        public static byte[] DecryptData(byte[] data)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);
            //将私钥导入RSA中，准备解密；
            rsa.FromXmlString(PrivateKey);
            //对数据进行解密，并返回解密结果；
            return rsa.Decrypt(data, false);
        }


    }
}
