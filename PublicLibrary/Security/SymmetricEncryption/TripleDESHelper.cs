using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PublicLibrary.Security.SymmetricEncryption
{
   public class TripleDESHelper
    {
      
   
        /// <summary>
        /// 给数据加密并返回加密后的密钥(索引为0)及数据（索引为1）
        /// </summary>
        /// <param name="data">需加密的数据</param>
        /// <returns>加密后的密钥(索引为0)及数据（索引为1）</returns>

        public static List<byte[]> EncryptData(byte[] data)
        {
            List<byte[]> dataAndKey = new List<byte[]>();
            MemoryStream memory = new MemoryStream();
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            CryptoStream cs = new CryptoStream(memory, tdes.CreateEncryptor(), CryptoStreamMode.Write);
            BinaryWriter bw = new BinaryWriter(cs);
            bw.Write(data);
            bw.Flush();
            bw.Close();
            cs.Close();
            byte[] dataNew = memory.ToArray();


            MemoryStream memoryKeyOut = new MemoryStream();
            BinaryWriter bwkey = new BinaryWriter(memoryKeyOut);
            bwkey.Write(tdes.Key);
            bwkey.Write(tdes.IV);
            bwkey.Flush();
            bwkey.Close();
            byte[] key = memoryKeyOut.ToArray();

            dataAndKey.Add(key);
            dataAndKey.Add(dataNew);

            memory.Close();
            memoryKeyOut.Close();
            return dataAndKey;
        }
        /// <summary>
        /// 给数据解密并返回解密后的数据
        /// </summary>
        /// <param name="key">解密密钥</param>
        /// <param name="data">待解密数据</param>
        /// <returns>解密后的数据</returns>
        public static byte[] DecryptData(byte[] key, byte[] data)
        {


            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            MemoryStream memoryKeyIn = new MemoryStream(key);
            BinaryReader br = new BinaryReader(memoryKeyIn);
            tdes.Key = br.ReadBytes(24);
            tdes.IV = br.ReadBytes(8);
            br.Close();

            MemoryStream memoryFileIn = new MemoryStream(data);
            CryptoStream cs = new CryptoStream(memoryFileIn, tdes.CreateDecryptor(), CryptoStreamMode.Read);
            BinaryReader brNewData = new BinaryReader(cs);

            // long length = cs.Length;
            byte[] dataNew = new byte[data.Length];
            //读取文件中的内容并保存到字节数组中
            brNewData.Read(dataNew, 0, dataNew.Length);
            memoryKeyIn.Close();

            return dataNew;

        }


        public static string a_strKey = "#s^un2ye31<cn%|aoXpR,+hh";

        public static byte[] Encrypt3DES(byte[] data )
        {
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = ASCIIEncoding.ASCII.GetBytes(a_strKey); 
            tdes.Mode = CipherMode.ECB; 
            ICryptoTransform DESEncrypt = tdes.CreateEncryptor();
            // byte[] Buffer = ASCIIEncoding.ASCII.GetBytes(a_strString); 
            byte[] Buffer = data;
            return DESEncrypt.TransformFinalBlock(Buffer, 0, Buffer.Length);




        }
        public static byte[] Decrypt3DES(byte[] data  ) 
        { 
            TripleDESCryptoServiceProvider DES = new TripleDESCryptoServiceProvider();
            DES.Key = ASCIIEncoding.ASCII.GetBytes(a_strKey); 
            DES.Mode = CipherMode.ECB;
            DES.Padding = System.Security.Cryptography.PaddingMode.PKCS7; 
            ICryptoTransform DESDecrypt = DES.CreateDecryptor();
         
            try 
            {
                // byte[] Buffer = Convert.FromBase64String(a_strString);
                byte[] Buffer = data;
                // result = ASCIIEncoding.ASCII.GetString(DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length));
                byte[] result = DESDecrypt.TransformFinalBlock(Buffer, 0, Buffer.Length);
                return result;
            }
            catch (Exception e)
            {
                return data;//当文件没有加密或者，已经解密成功时将返回原数据。
            }
           
        }


    }

}
