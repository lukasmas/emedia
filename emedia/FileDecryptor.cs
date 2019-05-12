using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace emedia
{
    class FileDecryptor
    {
        public static bool IsPNG(int[] header)
        {
            int[] first8bytes = { 137, 80, 78, 71, 13, 10, 26, 10 };

            for (int i = 0; i < first8bytes.Length; i++)
            {
                if (first8bytes[i] != header[i])
                    return false;
            }

            return true;
        }
        
        public static void ProcessPNG(string path)
        {
            bool foundIHDR = false;
            int width = 0;
            int heigth = 0;
            int bitDepth = 0;
            int colorType = 0;
            int compressionMethod = 0;
            int filterMethod = 0;
            int interlaceMethod = 0;
            FileStream fileStream = new FileStream(path, FileMode.Open);

            int[] header = new int[8];
            for (int i = 0; i < header.Length; i++)
            {
                header[i] = Convert.ToInt16(fileStream.ReadByte());
            }

            if (IsPNG(header))
            {
                while (!foundIHDR)
                {
                    StringBuilder ihdr = new StringBuilder();
                    var temp = Convert.ToChar(fileStream.ReadByte());
                    if (temp == 'I')
                    {
                        ihdr.Append(temp);
                        for (int i = 0; i < 3; i++)
                        {
                            ihdr.Append(Convert.ToChar(fileStream.ReadByte()));
                        }
                        if (ihdr.ToString().ToUpper() == "IHDR")
                        {
                            foundIHDR = true;
                        }
                    }
                }

                for (int i = 0; i < 4; i++)
                {
                    int pow = 8 - (2 * (i + 1));
                    width += Convert.ToInt32(fileStream.ReadByte()) * (int)Math.Pow(16,pow);
                }
                for (int i = 0; i < 4; i++)
                {
                    int pow = 8 - (2 * (i + 1));
                    heigth += Convert.ToInt32(fileStream.ReadByte()) * (int)Math.Pow(16, pow); ;
                }
                bitDepth = Convert.ToInt32(fileStream.ReadByte());
                colorType = Convert.ToInt32(fileStream.ReadByte());
                compressionMethod = Convert.ToInt32(fileStream.ReadByte());
                filterMethod = Convert.ToInt32(fileStream.ReadByte());
                interlaceMethod = Convert.ToInt32(fileStream.ReadByte());

                Console.WriteLine("Width: " + width.ToString());
                Console.WriteLine("Heigth: " + heigth.ToString());
                Console.WriteLine("Bit depth: " + bitDepth.ToString());
                Console.WriteLine("Color type: " + colorType.ToString());
                Console.WriteLine("Compression method: " + compressionMethod.ToString());
                Console.WriteLine("Filter method: " + filterMethod.ToString());
                Console.WriteLine("Interlace method: " + interlaceMethod.ToString());

            }
            
        }

        
    }
}
