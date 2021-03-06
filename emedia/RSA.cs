﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace emedia
{
    class RSA
    {

        private static BigInteger NWD(BigInteger e, BigInteger sth)
        {
            BigInteger temp;
            while (sth != 0)
            {
                temp = sth;
                sth = e % sth;
                e = temp;
            }
            return e;
        }

        private static BigInteger CalculateE(BigInteger sth)
        {
            BigInteger e, tmp;
            do
            {
                tmp = new Random().Next(3, int.MaxValue);
            } while (tmp % 2 == 0);

            for (e = tmp; NWD(e, sth) != 1; e -= 2) ;
            return e;
        }

        private static BigInteger CalculateInvMod(BigInteger e, BigInteger sth)
        {
            BigInteger b = sth;
            BigInteger d = 0;
            BigInteger u = 1;
            while (e != 0)
            {
                if (e < sth)
                {
                    BigInteger tmp = u;
                    u = d;
                    d = tmp;
                    tmp = e;
                    e = sth;
                    sth = tmp;
                }
                BigInteger q = e / sth;
                u = u - q * d;
                e = e - q * sth;
            }
            if (sth != 1) return -1;
            if (d < 0) d += b;
            return d;
        }

        public static BigInteger Encrypt(BigInteger value)
        {
            String key = string.Empty;
            BigInteger encryptedValue = 1;
            using (StreamReader sr = new StreamReader("RSA_pub"))
            {
                key = sr.ReadToEnd();
            }
            var tmp = key.Split(',');
            BigInteger e = BigInteger.Parse(tmp[0]);
            BigInteger n = BigInteger.Parse(tmp[1]);
            Dictionary<BigInteger, BigInteger> valuePairs = new Dictionary<BigInteger, BigInteger>();
            List<BigInteger> usefulIndex = new List<BigInteger>();
            BigInteger index = 1;

            BigInteger tmpCalc = (BigInteger)Math.Pow((double)value, 1) % n;
            valuePairs.Add(index, tmpCalc);
            index = index * 2;

            while (index < e)
            {
                tmpCalc = (BigInteger)Math.Pow((double)valuePairs[index / 2], 2) % n;
                valuePairs.Add(index, tmpCalc);
                index = index * 2;
            }

            while (e != 0)
            {
                index = index / 2;
                if (e - index >= 0)
                {
                    e = e - index;
                    usefulIndex.Add(index);
                }
            }

            foreach (var item in usefulIndex)
            {
                encryptedValue *= valuePairs[item];
            }
            encryptedValue = encryptedValue % n;
            return encryptedValue;
        }

        public static BigInteger Decrypt(BigInteger value)
        {
            String key = string.Empty;
            BigInteger decryptedValue = 1;

            using (StreamReader sr = new StreamReader("RSA_priv"))
            {
                key = sr.ReadToEnd();
            }
            var tmp = key.Split(',');
            BigInteger d = BigInteger.Parse(tmp[0]);
            BigInteger n = BigInteger.Parse(tmp[1]);
            Dictionary<BigInteger, BigInteger> valuePairs = new Dictionary<BigInteger, BigInteger>();
            List<BigInteger> usefulIndex = new List<BigInteger>();
            BigInteger index = 1;

            BigInteger tmpCalc = (BigInteger)Math.Pow((double)value, 1) % n;
            valuePairs.Add(index, tmpCalc);
            index = index * 2;

            while (index < d)
            {
                tmpCalc = (BigInteger)Math.Pow((double)valuePairs[index / 2], 2) % n;
                valuePairs.Add(index, tmpCalc);
                index = index * 2;
            }

            while (d != 0)
            {
                index = index / 2;
                if (d - index >= 0)
                {
                    d = d - index;
                    usefulIndex.Add(index);
                }
            }

            foreach (var item in usefulIndex)
            {
                decryptedValue *= valuePairs[item];
            }
            decryptedValue = decryptedValue % n;
            return decryptedValue;
        }

        static public void GenerateKey()
        {
            BigInteger p = BigInteger.Parse("123456789012345678901234567907");
            BigInteger q = 10000000000000000051;
            BigInteger n = p * q;
            BigInteger sth = (p - 1) * (q - 1);
            BigInteger e = CalculateE(sth);
            BigInteger d = CalculateInvMod(e, sth);
            Console.WriteLine("p= " + p);
            Console.WriteLine("q= " + q);

            Console.WriteLine("n= " + n);
            Console.WriteLine("e= " + e);
            Console.WriteLine("d= " + d);

            using (StreamWriter outputFile = new StreamWriter(Path.Combine("RSA_pub")))
            {
                outputFile.WriteLine(e + "," + n);
            }
            using (StreamWriter outputFile = new StreamWriter(Path.Combine("RSA_priv")))
            {
                outputFile.WriteLine(d + "," + n);
            }
        }
    }
}
