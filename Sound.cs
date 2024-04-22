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
using System.Windows.Documents;
using System.Windows.Navigation;
//using NAudio.Wave;


namespace SoundOff
{



    public class Sound
    {



        const int SampleRate = 44100;
        const short bitsPerSample = 16;
        private readonly System.Media.SoundPlayer SPlayer = new();

        //byte[] o = new byte[2];
        //public short getCurrentSampleValue()
        //{
        //    if (SPlayer.Stream == null)
        //        return 0;
        //    long pos = SPlayer.Stream.Position;
        //    //Debug.Write(SPlayer.Stream.CanRead.ToString());
        //    SPlayer.Stream.Position = pos;
        //    short oo = (short)(((short)o[0]) << 8 + o[1]);
        //    return 10;//oo;

        //}




        List<Oscillator> OSInfo;
        Envelope env;





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





        public Sound(int OscillatorNumber)
        {
            env = new Envelope();

            OSInfo = new List<Oscillator>(OscillatorNumber);

        }

        public void AddOscillator(Oscillator oscillator)
        {
            OSInfo.Add(oscillator);
        }

        Func<double, double>[] WaveTypes =
        [
            Math.Sin,
            Square,
            Triangle,
            Noise,
        ];


        private static double Square(double x)
        {
            return Math.Sin(x) > 0 ? 1 : -1;
        }

        private static double Triangle(double x)
        {
            return Math.Abs((x % 4) - 2) - 1;
        }

        static readonly Random random = new();
        private static double Noise(double x)
        {
            return random.NextDouble() * 2 - 1;
        }

        static float OscilatorFrequencyOffSet(float basefreq, Oscillator os)
        {

            return os.PitchShift >= 0 ? basefreq * ((float)os.PitchShift * 2) : basefreq / ((float)os.PitchShift * 2);

        }

        static int OscillatorVelocityShift(float vel, Oscillator os)
        {
            return (int)((vel * os.Vel) > 127 ? 127 : (vel * os.Vel));

        }

        short[] MakeWave(float freq, float vel, Waves type)
        {
            short[] samples = new short[SampleRate];
            for (int i = 0; i < SampleRate; i++)
            {
                //TODO: change oscillator
                samples[i] = Convert.ToInt16((short.MaxValue * (vel / 127)) * (WaveTypes[((int)type)](((2 * Math.PI * freq) / SampleRate) * i)));

            }
            return samples;

        }

        private static float KeyToFreq(int K)
        {
            return 400f * (float)Math.Pow(2f, (K - 69) / 12f);

        }



        public void PlaySound(int key, float vel)
        {

            float freq = KeyToFreq(key);
            if (vel == 0)
            {

            }
            if (key > 127)
            {
                return;
            }

            short[][] smapleStuff = new short[OSInfo.Count][];

            for (int i = 0; i < OSInfo.Count; i++)
            {
                int newVel = OscillatorVelocityShift(vel, OSInfo[i]);
                float newFreq = OscilatorFrequencyOffSet(freq, OSInfo[i]);

                smapleStuff[i] = MakeWave(freq + newFreq, newVel, OSInfo[i].WaveForm);
            }


            short[] samples = new short[smapleStuff[0].Length];
            byte[] byteArray = new byte[smapleStuff[0].Length * sizeof(short)];
            for (int i = 0; i < smapleStuff[0].Length; i++)
            {
                int combined = 0;
                foreach (var smaple in smapleStuff)
                {
                    combined += smaple[i];
                }
                samples[i] = Convert.ToInt16(combined / OSInfo.Count);
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


        private static readonly char[] dataHeader = ['d', 'a', 't', 'a'];
        private static readonly char[] RIFFHeader = ['R', 'I', 'F', 'F'];
        private static readonly char[] FormatHeader = ['W', 'A', 'V', 'E', 'f', 'm', 't', ' '];

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
