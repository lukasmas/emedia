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

            //FileDecryptor.ProccessWAV("a2002011001-e02.wav");
            //GNUPlot(1);
            //GNUPlot(2);
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

            BigInteger encVal = RSA.Encrypt(123); //983415

            Console.WriteLine(123 + " : " + encVal);
            Console.WriteLine(encVal + " : " + RSA.Decrypt(encVal));

            Console.ReadKey();
        }

        private static void GNUPlot(int option)
        {
            string Pgm = @"gnuplot\bin\gnuplot.exe";
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
