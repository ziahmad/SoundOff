using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace SoundOff
{



    public class Sound
    {



        const int SampleRate = 44100;
        const short bitsPerSample = 16;
        SoundPlayer[] Notes = new SoundPlayer[128];


        float KeyToFreq(int K)
        {
            return 400f * (float)Math.Pow(2f, (K - 69) / 12f);
        }

        //public class Envelope
        //{
        //    float Attack;
        //    float Decay;
        //    float Sustain;
        //    float Release;
        //}

        public enum Waves
        {
            SINE,
            SQUARE,
            TRIANGLE,
            NOISE,
        }

        public Waves OS1 = Waves.SINE;
        public Waves OS2 = Waves.SINE;
        public Waves OS3 = Waves.SINE;


        public Sound()
        {
        }

        private static double square(double x)
        {
            return Math.Sin(x) > 0 ? 1 : -1;
        }

        private static double triangle(double x)
        {
            return Math.Abs((x % 4) - 2) - 1;
        }


        private static readonly System.Media.SoundPlayer SPlayer = new();
        private static readonly char[] dataHeader = ['d', 'a', 't', 'a'];
        private static readonly char[] RIFFHeader = ['R', 'I', 'F', 'F'];
        private static readonly char[] FormatHeader = ['W', 'A', 'V', 'E', 'f', 'm', 't', ' '];

        //trying to have samples only be one wavelength in size
        static byte[] SineWave(float freq, float vel)
        {
            short[] samples = new short[2 * SampleRate / (int)freq];
            byte[] byteArray = new byte[2 * SampleRate / (int)freq * sizeof(short)];
            for (int i = 0; i < 2 * SampleRate / (int)freq; i++)
            {
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 128) * Math.Sin(((2 * Math.PI * freq) / SampleRate) * i));
                //StartBut.Content = samples[i];
            }

            Buffer.BlockCopy(samples, 0, byteArray, 0, samples.Length * sizeof(short));
            return byteArray;
        }
        static byte[] SquareWave(float freq, float vel)
        {
            short[] samples = new short[2 * SampleRate / (int)freq];
            byte[] byteArray = new byte[2 * SampleRate / (int)freq * sizeof(short)];
            for (int i = 0; i < 2 * SampleRate / (int)freq; i++)
            {
                //TODO: change oscillator
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 128) * square(((2 * Math.PI * freq) / SampleRate) * i));
            }

            Buffer.BlockCopy(samples, 0, byteArray, 0, samples.Length * sizeof(short));
            return byteArray;
        }
        static byte[] TriangleWave(float freq, float vel)
        {
            short[] samples = new short[2 * SampleRate / (int)freq];
            byte[] byteArray = new byte[2 * SampleRate / (int)freq * sizeof(short)];
            for (int i = 0; i < 2 * SampleRate / (int)freq; i++)
            {
                //TODO: change oscillator
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 128) * triangle(((2 * Math.PI * freq) / SampleRate) * i));
            }

            Buffer.BlockCopy(samples, 0, byteArray, 0, samples.Length * sizeof(short));
            return byteArray;
        }
        static byte[] Noise(float freq, float vel)
        {
            short[] samples = new short[2 * SampleRate / (int)freq];
            byte[] byteArray = new byte[2 * SampleRate / (int)freq * sizeof(short)];
            Random random = new();
            for (int i = 0; i < 2 * SampleRate / (int)freq; i++)
            {
                //TODO: change oscillator
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 128) * random.NextDouble());
            }

            Buffer.BlockCopy(samples, 0, byteArray, 0, samples.Length * sizeof(short));
            return byteArray;
        }

        //trying to have samples only be one wavelength in size
        static short[] SineWaveS(float freq, float vel)
        {
            short[] samples = new short[2 * SampleRate / (int)freq];
            for (int i = 0; i < 2 * SampleRate / (int)freq; i++)
            {
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 128) * Math.Sin(((2 * Math.PI * freq) / SampleRate) * i));
                //StartBut.Content = samples[i];
            }

            return samples;
        }
        static short[] SquareWaveS(float freq, float vel)
        {
            short[] samples = new short[2 * SampleRate / (int)freq];
            for (int i = 0; i < 2 * SampleRate / (int)freq; i++)
            {
                //TODO: change oscillator
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 128) * square(((2 * Math.PI * freq) / SampleRate) * i));
            }

            return samples;
        }
        static short[] TriangleWaveS(float freq, float vel)
        {
            short[] samples = new short[2 * SampleRate / (int)freq];
            for (int i = 0; i < 2 * SampleRate / (int)freq; i++)
            {
                //TODO: change oscillator
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 128) * triangle(((2 * Math.PI * freq) / SampleRate) * i));
            }

            return samples;
        }
        static short[] NoiseS(float freq, float vel)
        {
            short[] samples = new short[2 * SampleRate / (int)freq];
            Random random = new();
            for (int i = 0; i < 2 * SampleRate / (int)freq; i++)
            {
                //TODO: change oscillator
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 128) * random.NextDouble());
            }

            return samples;
        }

        //not one repeating wave
        //trying to have samples only be one wavelength in size
        static byte[] LongSineWave(float freq, float vel)
        {
            short[] samples = new short[SampleRate];
            byte[] byteArray = new byte[SampleRate * sizeof(short)];
            for (int i = 0; i < SampleRate; i++)
            {
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 128) * Math.Sin(((2 * Math.PI * freq) / SampleRate) * i));
                //StartBut.Content = samples[i];
            }

            Buffer.BlockCopy(samples, 0, byteArray, 0, samples.Length * sizeof(short));
            return byteArray;
        }
        static byte[] LongSquareWave(float freq, float vel)
        {
            short[] samples = new short[SampleRate];
            byte[] byteArray = new byte[SampleRate * sizeof(short)];
            for (int i = 0; i < SampleRate; i++)
            {
                //TODO: change oscillator
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 128) * square(((2 * Math.PI * freq) / SampleRate) * i));
            }

            Buffer.BlockCopy(samples, 0, byteArray, 0, samples.Length * sizeof(short));
            return byteArray;
        }
        static byte[] LongTriangleWave(float freq, float vel)
        {
            short[] samples = new short[SampleRate];
            byte[] byteArray = new byte[SampleRate * sizeof(short)];
            for (int i = 0; i < SampleRate; i++)
            {
                //TODO: change oscillator
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 128) * triangle(((2 * Math.PI * freq) / SampleRate) * i));
            }

            Buffer.BlockCopy(samples, 0, byteArray, 0, samples.Length * sizeof(short));
            return byteArray;
        }
        static byte[] LongNoise(float freq, float vel)
        {
            short[] samples = new short[SampleRate];
            byte[] byteArray = new byte[SampleRate * sizeof(short)];
            Random random = new();
            for (int i = 0; i < SampleRate; i++)
            {
                //TODO: change oscillator
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 128) * random.NextDouble());
            }

            Buffer.BlockCopy(samples, 0, byteArray, 0, samples.Length * sizeof(short));
            return byteArray;
        }

        //trying to have samples only be one wavelength in size
        static short[] LongSineWaveS(float freq, float vel)
        {
            short[] samples = new short[SampleRate];
            for (int i = 0; i < SampleRate; i++)
            {
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 128) * Math.Sin(((2 * Math.PI * freq) / SampleRate) * i));
                //StartBut.Content = samples[i];
            }

            return samples;
        }
        static short[] LongSquareWaveS(float freq, float vel)
        {
            short[] samples = new short[SampleRate];
            for (int i = 0; i < SampleRate; i++)
            {
                //TODO: change oscillator
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 128) * square(((2 * Math.PI * freq) / SampleRate) * i));
            }

            return samples;
        }
        static short[] LongTriangleWaveS(float freq, float vel)
        {
            short[] samples = new short[SampleRate];
            for (int i = 0; i < SampleRate; i++)
            {
                //TODO: change oscillator
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 128) * triangle(((2 * Math.PI * freq) / SampleRate) * i));
            }

            return samples;
        }
        static short[] LongNoiseS(float freq, float vel)
        {
            short[] samples = new short[SampleRate];
            Random random = new();
            for (int i = 0; i < SampleRate; i++)
            {
                //TODO: change oscillator
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 128) * random.NextDouble());
            }

            return samples;
        }




        public void PlaySound(int key, float vel)
        {

            float freq = KeyToFreq(key);
            Debug.WriteLine("{0}, {1}, {2}", key, freq, vel);
            if (vel == 0)
            {
                
            }
            if(key>127)
            {
                return;
            }s
            short[] sampleBytes1 = OS1 switch
            {
                Waves.SINE => LongSineWaveS(freq, vel),
                Waves.SQUARE => LongSquareWaveS(freq, vel),
                Waves.TRIANGLE => LongTriangleWaveS(freq, vel),
                Waves.NOISE => LongNoiseS(freq, vel),
                _ => LongSineWaveS(freq, vel),
            };
            short[] sampleBytes2 = OS2 switch
            {
                Waves.SINE => LongSineWaveS(freq, vel),
                Waves.SQUARE => LongSquareWaveS(freq, vel),
                Waves.TRIANGLE => LongTriangleWaveS(freq, vel),
                Waves.NOISE => LongNoiseS(freq, vel),
                _ => LongSineWaveS(freq, vel),
            };
            short[] sampleBytes3 = OS3 switch
            {
                Waves.SINE => LongSineWaveS(freq, vel),
                Waves.SQUARE => LongSquareWaveS(freq, vel),
                Waves.TRIANGLE => LongTriangleWaveS(freq, vel),
                Waves.NOISE => LongNoiseS(freq, vel),
                _ => LongSineWaveS(freq, vel),
            };

            short[] samples = new short[sampleBytes1.Length];
            byte[] byteArray = new byte[sampleBytes1.Length * sizeof(short)];
            for (int i = 0; i < sampleBytes1.Length; i++)
            {
                samples[i] = Convert.ToInt16((sampleBytes1[i] + sampleBytes2[i] + sampleBytes3[i]) / 3);
            }


            Buffer.BlockCopy(samples, 0, byteArray, 0, samples.Length * sizeof(short));


            SPlayer.Stream = LongMakeWav(freq, byteArray);
            SPlayer.PlayLooping();



        }
        public void StopSound()
        {
            SPlayer.Stop();
            SPlayer.Dispose();
            SPlayer.Stream= null;
        }

        public bool OverPercentDone(float pc)
        {
            if(SPlayer.Stream == null) { return true; }
            float percent = SPlayer.Stream.Position / SPlayer.Stream.Length;
            if (pc < 0)
                return true;
            return pc > percent;

        }

        private static MemoryStream MakeWav(float freq, byte[] sampleBytes)
        {
            MemoryStream ms;
            BinaryWriter bw;
            ms = new();
            bw = new(ms);
            int blockAlign = bitsPerSample / 8;
            //int subChunk = SampleRate / blockAlign;
            int subChunk = 2 * SampleRate / (int)freq * blockAlign;
            bw.Write(RIFFHeader);
            bw.Write(36 + subChunk);
            bw.Write(FormatHeader);
            bw.Write(16);
            bw.Write((short)1);
            bw.Write((short)1);
            bw.Write(SampleRate);
            bw.Write(SampleRate * blockAlign);
            bw.Write((short)blockAlign);
            bw.Write(bitsPerSample);
            bw.Write(dataHeader);
            bw.Write(subChunk);
            bw.Write(sampleBytes);
            ms.Position = 0;
            return ms;
        }
        private static MemoryStream LongMakeWav(float freq, byte[] sampleBytes)
        {
            MemoryStream ms;
            BinaryWriter bw;
            ms = new();
            bw = new(ms);
            int blockAlign = bitsPerSample / 8;
            //int subChunk = SampleRate / blockAlign;
            int subChunk = SampleRate * blockAlign;
            bw.Write(RIFFHeader);
            bw.Write(36 + subChunk);
            bw.Write(FormatHeader);
            bw.Write(16);
            bw.Write((short)1);
            bw.Write((short)1);
            bw.Write(SampleRate);
            bw.Write(SampleRate * blockAlign);
            bw.Write((short)blockAlign);
            bw.Write(bitsPerSample);
            bw.Write(dataHeader);
            bw.Write(subChunk);
            bw.Write(sampleBytes);
            ms.Position = 0;
            return ms;
        }
    }
}
