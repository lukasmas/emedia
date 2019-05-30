using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Numerics;

namespace emedia
{
    class FileDecryptor
    {
        public struct WAV
        {
            public string RIFF;
            public int fileSize;
            public string WAVE;
            public string fmt;
            public int length;
            public int type;
            public int numberOfChannels;
            public int sampleRate;
            public int byteRate;
            public int BlockAlign;
            public int bitPerSample;
            public string data;
            public int sizeOfData;
        }

        public static WAV wav;

        public static void PrintWav()
        {
            Console.WriteLine("WAV HEADER:");
            Console.WriteLine("RIFF: " + wav.RIFF);
            Console.WriteLine("fileSize: " + wav.fileSize);
            Console.WriteLine("WAVE: " + wav.WAVE);
            Console.WriteLine("fmt: " + wav.fmt);
            Console.WriteLine("length: " + wav.length);
            Console.WriteLine("type: " + wav.type);
            Console.WriteLine("numberOfChannels: " + wav.numberOfChannels);
            Console.WriteLine("sampleRate: " + wav.sampleRate);
            Console.WriteLine("byteRate: " + wav.byteRate);
            Console.WriteLine("BlockAlign: " + wav.BlockAlign);
            Console.WriteLine("bitPerSample: " + wav.bitPerSample);
            Console.WriteLine("data: " + wav.data);
            Console.WriteLine("sizeOfData: " + wav.sizeOfData);
        }

        public static int ReadBytes_LittleEndian(ref FileStream file, int size)
        {
            var temp = 0;
            for (int i = 0; i < size; i++)
            {
                temp += Convert.ToInt32(file.ReadByte()) * (int)Math.Pow(16, i * 2);
            }
            return temp;
        }

        public static string ReadBytesToString_BigEndian(ref FileStream file, int size)
        {
            var temp = string.Empty;
            for (int i = 0; i < size; i++)
            {
                temp += Convert.ToChar(file.ReadByte());
            }
            return temp;
        }

        public static Complex[] CalculateDFT(Complex[] samples)
        {
            int N = samples.Length;
            Complex[] result = new Complex[N];
            for (int k = 0; k < N; k++)
            {
                Complex sum = 0;
                for (int n = 0; n < N; n++)
                {
                    double omega = 2 * Math.PI * n * k / N;
                    sum += samples[n] * Complex.Exp(new Complex(0, -omega));

                }
                result[k] = sum;
            }
            return result;
        }


        public static void ProccessWAV(string path)
        {
            Complex[] samples = new Complex[2000];
            int[] temp = new int[2000];
            FileStream fileStream = new FileStream(path, FileMode.Open);
            wav.RIFF = ReadBytesToString_BigEndian(ref fileStream, 4);
            wav.fileSize = ReadBytes_LittleEndian(ref fileStream, 4);
            wav.WAVE = ReadBytesToString_BigEndian(ref fileStream, 4);
            wav.fmt = ReadBytesToString_BigEndian(ref fileStream, 4);
            wav.length = ReadBytes_LittleEndian(ref fileStream, 4);
            wav.type = ReadBytes_LittleEndian(ref fileStream, 2);
            wav.numberOfChannels = ReadBytes_LittleEndian(ref fileStream, 2);
            wav.sampleRate = ReadBytes_LittleEndian(ref fileStream, 4);
            wav.byteRate = ReadBytes_LittleEndian(ref fileStream, 4);
            wav.BlockAlign = ReadBytes_LittleEndian(ref fileStream, 2);
            wav.bitPerSample = ReadBytes_LittleEndian(ref fileStream, 4);
            wav.data = ReadBytesToString_BigEndian(ref fileStream, 4);
            wav.sizeOfData = ReadBytes_LittleEndian(ref fileStream, 4);

            PrintWav();

            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = ReadBytes_LittleEndian(ref fileStream, 2);
            }
            fileStream.Close();

            using (StreamWriter outputFile = new StreamWriter(Path.Combine("samples.dat")))
            {

                for (int i = 0; i < samples.Length; i++)
                {
                    outputFile.WriteLine(samples[i].Real + ";" + samples[i].Imaginary);
                }
            }

            samples = CalculateDFT(samples);

            using (StreamWriter outputFile = new StreamWriter(Path.Combine("dft.dat")))
            {

                for (int i = 0; i < samples.Length; i++)
                {
                    outputFile.WriteLine(i + "\t" + samples[i].Real + "\t" + samples[i].Imaginary);
                }
            }


        }

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
                    width += Convert.ToInt32(fileStream.ReadByte()) * (int)Math.Pow(16, pow);
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
                fileStream.Close();
            }

        }


    }
}
