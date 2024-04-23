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
using System.Transactions;
using System.Windows.Documents;
using System.Windows.Navigation;
using NAudio.Wave;


namespace SoundOff
{



    public class Sound : WaveStream
    {

        private readonly DirectSoundOut? _soundOut = null;
        private readonly BlockAlignReductionStream? stream = null;

        public override WaveFormat WaveFormat
        {
            get { return new WaveFormat(SampleRate, bitsPerSample, 1); }
        }

        public override long Length
        {
            get { return long.MaxValue; }
        }

        public override long Position { get; set; }

        public override int Read(byte[] buffer, int offset, int count)
        {

            int Samples = count / 2;


            for (int i = 0; i < Samples; i++)
            {
                int io = 0;
                foreach (Oscillator os in OSInfo)
                {
                    double sine = EnvelopeRamp(time) * OscillatorVelocityShift(Amplitude, os) * WaveTypes[(int)os.WaveForm](2 * Math.PI * (Freqency + OscilatorFrequencyOffSet(Freqency, os)) * time);
                    io += (short)Math.Round(sine * (Math.Pow(2, 15) - 1));
                }
                time += 1.0 / SampleRate;
                short Trunc = (short)(io / OSInfo.Count);
                buffer[i * 2] = (byte)(Trunc & 0x00ff);
                buffer[i * 2 + 1] = (byte)((Trunc & 0xff00) >> 8);

                if (AvailableSamples.Count < SampleQueueBufferLength)
                {
                    AvailableSamples.Enqueue(Trunc);
                    AvailableSampleCount++;
                }
                else
                {
                    AvailableSamples.Dequeue();
                    AvailableSamples.Enqueue(Trunc);
                }

            }
            return count;
        }



        private double Freqency { get; set; }
        private double Amplitude;
        private double time;

        const int SampleRate = 44100;
        const short bitsPerSample = 16;


        public const int SampleQueueBufferLength = SampleRate / 20;

        private readonly Queue<short> AvailableSamples = new(SampleQueueBufferLength);
        public short AvailableSampleCount
        {
            get; private set;
        }
        public short CurrentSample
        {
            get { if (AvailableSamples.Count > 0) { AvailableSampleCount--; return AvailableSamples.Dequeue(); } else { AvailableSampleCount = 0; return (short)0; } }
        }

        public Sound(int OscillatorNumber, double f, double a)
        {

            env = new Envelope();

            OSInfo = new List<Oscillator>(OscillatorNumber);

            time = 0;
            Freqency = f;
            Amplitude = a;
            stream = new BlockAlignReductionStream(this);

            _soundOut = new();
            _soundOut.Init(stream);
            _soundOut.Play();
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
            env.triggerOn_t = time;
            env.triggerOff_t = 0;

            Freqency = freq;
            Amplitude = vel / 127;




        }
        public void StopSound()
        {

            env.triggerOff_t = time;
            AvailableSamples.Clear();
        }

        readonly List<Oscillator> OSInfo;
        public Envelope env;

        public class Envelope
        {
            public double Attack_t;
            public double Decay_t;
            public double Release_t;

            public double Sustain_A;
            public double Attack_A;

            public double triggerOn_t;
            public double triggerOff_t;

            public double CurrentEnteringA;
            public double CurrentAttackA;
            public double CurrentExitingA;

            public Envelope()
            {
                Attack_t = .01;
                Decay_t = .01;
                Release_t = .2;

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

        private double EnvelopeRamp(double t)
        {
            double RelT;
            double oooo;
            if (env.triggerOn_t > env.triggerOff_t)
            {
                RelT = t - env.triggerOn_t;
                if (RelT < 0)
                {
                    oooo = 1;
                }
                else if (RelT <= env.Attack_t && env.Attack_t != 0)
                {
                    oooo = double.Lerp(env.CurrentExitingA, env.Attack_A, RelT / env.Attack_t);
                    env.CurrentAttackA = oooo;
                }
                else if (RelT <= env.Attack_t + env.Decay_t && env.Attack_t + env.Decay_t != 0)
                {
                    oooo = double.Lerp(env.CurrentAttackA, env.Sustain_A, RelT / (env.Attack_t + env.Decay_t));
                }
                else
                {
                    oooo = env.Sustain_A;
                }
                env.CurrentEnteringA = oooo;
            }
            else
            {
                RelT = t - env.triggerOff_t;
                if (RelT < 0)
                {
                    oooo = 1;
                }
                else if (RelT <= env.Release_t)
                {
                    oooo = double.Lerp(env.CurrentEnteringA, 0, RelT / env.Release_t);

                }
                else
                {
                    oooo = 0;
                }
                env.CurrentExitingA = oooo;
            }

            return oooo;

        }


        public void AddOscillator(Oscillator oscillator)
        {
            OSInfo.Add(oscillator);
        }

        private static readonly Func<double, double>[] WaveTypes =
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

        private static double OscilatorFrequencyOffSet(double basefreq, Oscillator os)
        {

            return os.PitchShift >= 0 ? basefreq * (os.PitchShift * 2) : basefreq / (os.PitchShift * 2);

        }

        private static double OscillatorVelocityShift(double vel, Oscillator os)
        {
            return (vel * os.Vel) > 1.0 ? 1.0 : (vel * os.Vel);

        }



        private static float KeyToFreq(int K)
        {
            return 400f * (float)Math.Pow(2f, (K - 69) / 12f);

        }









        private static readonly char[] dataHeader = ['d', 'a', 't', 'a'];
        private static readonly char[] RIFFHeader = ['R', 'I', 'F', 'F'];
        private static readonly char[] FormatHeader = ['W', 'A', 'V', 'E', 'f', 'm', 't', ' '];

        static short[] MakeWave(float freq, float vel, Waves type)
        {
            short[] samples = new short[SampleRate];
            for (int i = 0; i < SampleRate; i++)
            {
                //TODO: change oscillator
                samples[i] = Convert.ToInt16((short.MaxValue * (vel / 127)) * (WaveTypes[((int)type)](((2 * Math.PI * freq) / SampleRate) * i)));

            }
            return samples;

        }

#pragma warning disable IDE0060 // Remove unused parameter
        private static MemoryStream LongMakeWav(float freq, byte[] sampleBytes)
#pragma warning restore IDE0060 // Remove unused parameter
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
