using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Numerics;

namespace emedia
{
    class Program
    {


        static void Main(string[] args)
        {
            string path = "media/a2002011001-e02.wav";

            //FileDecryptor.ProcessPNG("C:/path/to/test.png");
            //Console.WriteLine();

            FileDecryptor.ProccessWAV(path);
            GNUPlot(1);
            GNUPlot(2);
            if (!File.Exists("RSA_priv") && !File.Exists("RSA_pub"))
                RSA.GenerateKey();
            else
            {
                string o = string.Empty;
                Console.WriteLine("Generate RSA keys one more time? Y/N");
                o = Console.ReadLine();
                Console.WriteLine();

                if (o[0] == 'y' || o[0] == 'Y')
                {
                    RSA.GenerateKey();
                }
            }

            if (!File.Exists("XOR_key"))
                XOR.GenerateXORKey(FileDecryptor.wav.sizeOfData);
            else
            {
                string o = string.Empty;
                Console.WriteLine("Generate XOR keys one more time? Y/N");
                o = Console.ReadLine();
                Console.WriteLine();

                if (o[0] == 'y' || o[0] == 'Y')
                {
                    XOR.GenerateXORKey(FileDecryptor.wav.sizeOfData);
                }
            }

            BigInteger encVal = RSA.Encrypt(123); //983415

            Console.WriteLine(123 + " : " + encVal);
            Console.WriteLine(encVal + " : " + RSA.Decrypt(encVal));

            Console.WriteLine();
            XOR.EncryptData(path, 44, "media/Encrypted.wav");
            Console.WriteLine("File " + path + " encrypted into \"media/Encrypted.wav\"");

            XOR.DecryptData("media/Encrypted.wav", 44, "media/Decrypted.wav");
            Console.WriteLine("File \"media/Encrypted.wav\" decrypted into \"media/Decrypted.wav\"");

            Console.ReadKey();
        }

        private static void GNUPlot(int option)
        {
            string Pgm = @"gnuplot\bin\gnuplot.exe";
            if (!File.Exists(Pgm))
            {
                Console.WriteLine("Brak zainstalowanego gnuplota\nZainstaluj gnuplot i wklej go do katalogu z programem!");
                return;
            }
            Process extPro = new Process();
            extPro.StartInfo.FileName = Pgm;
            extPro.StartInfo.UseShellExecute = false;
            extPro.StartInfo.RedirectStandardInput = true;
            extPro.Start();

            StreamWriter gnupStWr = extPro.StandardInput;
            string path = @Directory.GetCurrentDirectory();
            path = path.Replace('\\', '/');
            string dft = path + "/dft.dat";
            switch (option)
            {
                case 1:
                    {
                        gnupStWr.WriteLine("plot \"" + dft + "\" using 1:2 title 'REAL' with lines");
                        break;
                    }
                case 2:
                    {
                        gnupStWr.WriteLine("plot \"" + dft + "\" using 1:3 title 'IMAG' with lines");
                        break;
                    }
                default:
                    {
                        gnupStWr.WriteLine("plot sin(x) ");
                        break;
                    }
            }


            gnupStWr.Flush();

        }
    }
}
