/* Copyright 2023 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ContainerdLibrary
{
    internal static class HashCalculator
    {
        public static string CalculateSha256HashString(byte[] bytes)
        {
            SHA256 sha256 = new SHA256Managed();
            byte[] hash = sha256.ComputeHash(bytes);
            return HexToString(hash);
        }

        public static string CalculateSha256HashString(string filePath)
        {
            byte[] hashBytes = CalculateSha256Hash(filePath);
            return HexToString(hashBytes);
        }

        public static byte[] CalculateSha256Hash(string filePath)
        {
            SHA256 sha256 = new SHA256Managed();
            FileStream fileStream = File.OpenRead(filePath);
            using (fileStream)
            {
                return sha256.ComputeHash(fileStream);
            }
        }

        private static string HexToString(byte[] array)
        {
            StringBuilder builder = new StringBuilder();
            foreach (byte b in array)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}
