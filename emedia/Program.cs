using System;
using System.IO;
using System.IO.Compression;
namespace emedia
{
    class Program
    {
        static void Main(string[] args)
        {
            //C:/ Users / Lukas / Desktop / IO.png
            FileDecryptor.ProccessWAV("a2002011001-e02.wav");
            //FileInfo fileInfo = new FileInfo("test1.png");


            System.Console.ReadKey();

        }
    }
}
