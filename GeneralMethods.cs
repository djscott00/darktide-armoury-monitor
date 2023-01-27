using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Runtime.Serialization;

namespace Darktide_Armoury_Monitor
{
    public static class GeneralMethods
    {

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        private static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = MD5.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static int ParseNumber(string inputString)
        {
            int result = 0;
            StringBuilder buffer = new StringBuilder(8);

            for(int i = 0; i < inputString.Length; i++) {
                char curChar = inputString[i];

                if(char.IsDigit(curChar) ) {
                    buffer.Append(curChar);
                }
            }

            int.TryParse(buffer.ToString(), out result);

            return result;
        }




    }

}
