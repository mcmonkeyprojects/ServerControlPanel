using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;

namespace ServerControlPanel.Models
{
    public static class UserValidator
    {
        public static Encoding Enc = new UTF8Encoding(false);

        public static string HashV1(string input, string salt)
        {
            byte[] dat = GetPbkdf2Bytes(input, Enc.GetBytes(salt), 100 * 1000, 64);
            return "v1:" + salt + ":" + BitConverter.ToString(dat).Replace("-", "");
        }

        public static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }

        private static byte[] GetPbkdf2Bytes(string password, byte[] salt, int iterations, int outputBytes)
        {
            Rfc2898DeriveBytes pbkdf2 = new(password, salt)
            {
                IterationCount = iterations
            };
            return pbkdf2.GetBytes(outputBytes);
        }

        public static string Hash(string input)
        {
            return HashV1(input, GetRandomHex());
        }

        public static RandomNumberGenerator rng = RandomNumberGenerator.Create();

        public static string GetRandomHex(int len = 64)
        {
            byte[] data = new byte[len];
            rng.GetBytes(data);
            return BitConverter.ToString(data).Replace("-", "");
        }

        public static bool CheckValidPassword(string fullHash, string password)
        {
            string[] hashData = fullHash.Split(':');
            if (hashData[0] == "v1")
            {
                string hash = HashV1(password, hashData[1]);
                byte[] b1 = Enc.GetBytes(hash);
                byte[] b2 = Enc.GetBytes(fullHash);
                if (!SlowEquals(b1, b2))
                {
                    return false;
                }
                return true;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
