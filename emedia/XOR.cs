using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace emedia
{
    class XOR
    {
        public static string keyXOR = string.Empty;
        public static void GenerateXORKey(long keyLength)
        {
            StringBuilder keyBulder = new StringBuilder();
            Random rnd = new Random();
            for (int i = 0; i < keyLength; i++)
            {
                keyBulder.Append(Convert.ToChar(rnd.Next(33, 126)));
            }
            keyXOR = keyBulder.ToString();
            using (StreamWriter outputFile = new StreamWriter(Path.Combine("XOR_key")))
            {
                outputFile.WriteLine(keyXOR);
            }
        }

        public static void EncryptData(string inPath, int headerSize, string outPath)
        {
            FileStream fileStreamIN = new FileStream(inPath, FileMode.Open);
            FileStream fileStreamOUT = new FileStream(outPath, FileMode.OpenOrCreate);
            for (int i = 0; i < headerSize; i++)
            {
                byte tmp = (byte)fileStreamIN.ReadByte();
                fileStreamOUT.WriteByte(tmp);
            }

            List<byte> toSwap = new List<byte>();
            for (int i = 0; i < keyXOR.Length; i++)
            {

                int tmp = fileStreamIN.ReadByte();
                tmp = tmp ^ keyXOR[i];
                toSwap.Add((byte)tmp);
                if (toSwap.Count == 8)
                {
                    toSwap.Reverse();
                    foreach (var item in toSwap)
                    {
                        fileStreamOUT.WriteByte(item);
                    }
                    toSwap.Clear();
                }

            }
            if (toSwap.Count != 0)
            {
                toSwap.Reverse();
                foreach (var item in toSwap)
                {
                    fileStreamOUT.WriteByte(item);
                }
                toSwap.Clear();
            }

            fileStreamIN.Close();
            fileStreamOUT.Close();

        }
        public static void DecryptData(string inPath, int headerSize, string outPath)
        {
            FileStream fileStreamIN = new FileStream(inPath, FileMode.Open);
            FileStream fileStreamOUT = new FileStream(outPath, FileMode.OpenOrCreate);
            for (int i = 0; i < headerSize; i++)
            {
                byte tmp = (byte)fileStreamIN.ReadByte();
                fileStreamOUT.WriteByte(tmp);
            }

            List<byte> toSwap = new List<byte>();
            for (int i = 0; i < keyXOR.Length; i++)
            {

                int tmp = fileStreamIN.ReadByte();
                toSwap.Add((byte)tmp);
                if (toSwap.Count == 8)
                {
                    toSwap.Reverse();
                    for (int j = toSwap.Count - 1; j >= 0; j--)
                    {
                        tmp = toSwap[toSwap.Count - 1 - j] ^ keyXOR[i - j];

                        fileStreamOUT.WriteByte((byte)tmp);
                    }
                    toSwap.Clear();
                }

            }
            if (toSwap.Count != 0)
            {
                toSwap.Reverse();
                for (int j = toSwap.Count - 1; j >= 0; j--)
                {
                    int tmp = toSwap[toSwap.Count - 1 - j] ^ keyXOR[keyXOR.Length - 1 - j];

                    fileStreamOUT.WriteByte((byte)tmp);
                }
                toSwap.Clear();
            }

            fileStreamIN.Close();
            fileStreamOUT.Close();

        }

    }
}