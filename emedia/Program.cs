using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
namespace emedia
{
    class Program
    {

        private static void GNUPlot(int option)
        {
            string Pgm = @"gnuplot\bin\gnuplot.exe";
            Process extPro = new Process();
            extPro.StartInfo.FileName = Pgm;
            extPro.StartInfo.UseShellExecute = false;
            extPro.StartInfo.RedirectStandardInput = true;
            extPro.Start();

            StreamWriter gnupStWr = extPro.StandardInput;
            string path =@Directory.GetCurrentDirectory();
            path = path.Replace('\\', '/');
            string dft =path + "/dft.dat";
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
        static void Main(string[] args)
        {
            //C:/ Users / Lukas / Desktop / IO.png
            FileDecryptor.ProccessWAV("a2002011001-e02.wav");
            //FileInfo fileInfo = new FileInfo("test1.png");
            GNUPlot(1);
            GNUPlot(2);


            System.Console.ReadKey();

        }
    }
}
