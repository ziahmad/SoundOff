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

        //NOTE TO SELF
        /**
        * ADSR can be possible If I can control the stream position of the player
        * for the first SampleRate*Attack_t samples, ramp up aplitude from 0 to vel*Attack_A
        * for the next  SampleRate*Decay_t samples, ramp donw amplitude from vel*Attack_A to vel*Sustain_A
        * * Save this point
        * for here on, we get SampleRate*1 samples for Sustain
        * everytime we reach markPoint+ SampleRate Samples, we loop back to Markpoint
        * 
        * markPoint+SampleRate is where release starts, so when release key, move stram position to release mark
        * at release mark samples, we take Release_t*SampleRate Samples to go from vel*sustain_A to 0
        * 
        * 
        * This will only work if we can freely move the stream pointer
        * 
        * We can also chose a time where we can loop back to Markpoint without Clicking
        * 
        * ADSR control pannel will be in wide open side of the synth
        */
        public class Envelope
        {
            public double Attack_t;
            public double Decay_t;
            public double Release_t;

            public double Sustain_A;
            public double Attack_A;

            public double triggerOn_t;
            public double triggerOff_t;

            public Envelope()
            {
                Attack_t = 0.01;
                Decay_t = 0.01;
                Release_t = 0.02;

                Sustain_A = 0.8;
                Attack_A = 1.0;

                triggerOn_t = 0;
                triggerOff_t = 0;

            }



        }

        public enum Waves
        {
            SINE,
            SQUARE,
            TRIANGLE,
            NOISE,
        }

        public Waves OS1 = Waves.SINE;
        public double Os1V = 1;
        public double Os1P = 0;
        public Waves OS2 = Waves.SINE;
        public double Os2V = 1;
        public double Os2P = 0;
        public Waves OS3 = Waves.SINE;
        public double Os3V = 1;
        public double Os3P = 0;


        Envelope env;


        public Sound()
        {
            env = new Envelope();
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
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 127) * Math.Sin(((2 * Math.PI * freq) / SampleRate) * i));
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
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 127) * square(((2 * Math.PI * freq) / SampleRate) * i));
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
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 127) * triangle(((2 * Math.PI * freq) / SampleRate) * i));
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
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 127) * random.NextDouble());
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
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 127) * Math.Sin(((2 * Math.PI * freq) / SampleRate) * i));
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
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 127) * square(((2 * Math.PI * freq) / SampleRate) * i));
            }

            return samples;
        }
        static short[] TriangleWaveS(float freq, float vel)
        {
            short[] samples = new short[2 * SampleRate / (int)freq];
            for (int i = 0; i < 2 * SampleRate / (int)freq; i++)
            {
                //TODO: change oscillator
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 127) * triangle(((2 * Math.PI * freq) / SampleRate) * i));
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
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 127) * random.NextDouble());
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
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 127) * Math.Sin(((2 * Math.PI * freq) / SampleRate) * i));
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
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 127) * square(((2 * Math.PI * freq) / SampleRate) * i));
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
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 127) * triangle(((2 * Math.PI * freq) / SampleRate) * i));
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
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 127) * random.NextDouble());
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
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 127) * Math.Sin(((2 * Math.PI * freq) / SampleRate) * i));
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
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 127) * square(((2 * Math.PI * freq) / SampleRate) * i));
            }

            return samples;
        }
        static short[] LongTriangleWaveS(float freq, float vel)
        {
            short[] samples = new short[SampleRate];
            for (int i = 0; i < SampleRate; i++)
            {
                //TODO: change oscillator
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 127) * triangle(((2 * Math.PI * freq) / SampleRate) * i));
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
                samples[i] = Convert.ToInt16(short.MaxValue * (vel / 127) * random.NextDouble());
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
            if (key > 127)
            {
                return;
            }

            float os1freqOff = Os1P >= 0 ? freq * ((float)Os1P * 2) : freq / ((float)Os1P * 2);
            float os2freqOff = Os2P >= 0 ? freq * ((float)Os2P * 2) : freq / ((float)Os2P * 2);
            float os3freqOff = Os3P >= 0 ? freq * ((float)Os3P * 2) : freq / ((float)Os3P * 2);
            Debug.WriteLine("{0}, {1}, {2}", os1freqOff, os2freqOff, os3freqOff);

            short[] sampleBytes1 = OS1 switch
            {
                Waves.SINE => LongSineWaveS(freq + os1freqOff, (int)((vel * Os1V) > 127 ? 127 : (vel * Os1V))),
                Waves.SQUARE => LongSquareWaveS(freq + os1freqOff, (int)((vel * Os1V) > 127 ? 127 : (vel * Os1V))),
                Waves.TRIANGLE => LongTriangleWaveS(freq + os1freqOff, (int)((vel * Os1V) > 127 ? 127 : (vel * Os1V))),
                Waves.NOISE => LongNoiseS(freq + os1freqOff, (int)((vel * Os1V) > 127 ? 127 : (vel * Os1V))),
                _ => LongSineWaveS(freq + os1freqOff, (int)((vel * Os1V) > 127 ? 127 : (vel * Os1V))),
            };
            short[] sampleBytes2 = OS2 switch
            {
                Waves.SINE => LongSineWaveS(freq + os2freqOff, (int)((vel * Os2V) > 127 ? 127 : (vel * Os2V))),
                Waves.SQUARE => LongSquareWaveS(freq + os2freqOff, (int)((vel * Os2V) > 127 ? 127 : (vel * Os2V))),
                Waves.TRIANGLE => LongTriangleWaveS(freq + os2freqOff, (int)((vel * Os2V) > 127 ? 127 : (vel * Os2V))),
                Waves.NOISE => LongNoiseS(freq + os2freqOff, (int)((vel * Os2V) > 127 ? 127 : (vel * Os2V))),
                _ => LongSineWaveS(freq + os2freqOff, (int)((vel * Os2V) > 127 ? 127 : (vel * Os2V))),
            };
            short[] sampleBytes3 = OS3 switch
            {
                Waves.SINE => LongSineWaveS(freq + os3freqOff, (int)((vel * Os3V) > 127 ? 127 : (vel * Os3V))),
                Waves.SQUARE => LongSquareWaveS(freq + os3freqOff, (int)((vel * Os3V) > 127 ? 127 : (vel * Os3V))),
                Waves.TRIANGLE => LongTriangleWaveS(freq + os3freqOff, (int)((vel * Os3V) > 127 ? 127 : (vel * Os3V))),
                Waves.NOISE => LongNoiseS(freq + os3freqOff, (int)((vel * Os3V) > 127 ? 127 : (vel * Os3V))),
                _ => LongSineWaveS(freq + os3freqOff, (int)((vel * Os3V) > 127 ? 127 : (vel * Os3V))),
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
            SPlayer.Stream = null;
        }

        public bool OverPercentDone(float pc)
        {
            if (SPlayer.Stream == null) { return true; }
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
