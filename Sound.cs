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

// ~ nuget download Naudio and system.windwos.pipelines packages to have this work
namespace SoundOff
{



    public class Sound : WaveStream // ~ parent class is from Naudio which handles most of the direct hardware hadnling; foraces you to implement some things (the ones with override)
    {

        private readonly DirectSoundOut? _soundOut = null; // ~ the type of sound out, there are more, but this one worked best for me (if you have asio drivers, use the asio version)
        private readonly BlockAlignReductionStream? stream = null; // ~ data stram to give to _soundout

        public override WaveFormat WaveFormat
        {
            get { return new WaveFormat(SampleRate, bitsPerSample, 1); }
        }

        public override long Length
        {
            get { return long.MaxValue; } // ~ use a BIG value to avoid premature ending long.Max value should be good for most purposes
        }

        public override long Position { get; set; }

        public override int Read(byte[] buffer, int offset, int count)
        {

            int Samples = count / 2; // ~ div to because we will will deal with shorts (16 bits) instead of bytes (8 bits)


            for (int i = 0; i < Samples; i++)
            {
                int io = 0; // ~ using to mix the samples 
                foreach (Oscillator os in OSInfo) // ~ per oscillator
                {
                    double sine = EnvelopeRamp(time) * OscillatorVelocityShift(Amplitude, os) * WaveTypes[(int)os.WaveForm](2 * Math.PI * (Freqency + OscilatorFrequencyOffSet(Freqency, os)) * time); 
                    // ~ ok this looks like alot but its not - envelope (ADSR) ramp scales the amplitude according to the envelope
                    // ~ oscilator velocity shift scales the amplitude according the the oscialtor value
                    // ~ waveTypes is an array of periodic functions with the same period of sine to make the math work nice, we index the function based to the oscilator enum
                    // ~ frequency offset shifts the frequency accoriding to the acilator value, and we added to the frequency -- we could probably just add is in the function aswell
                    // ~ 2*math.pi for frequency scale to work with math.sin

                    io += (short)Math.Round(sine * (Math.Pow(2, 15) - 1)); // ~ convert sine to short
                }
                time += 1.0 / SampleRate; // ~ this was all for one sample, inscrse time and doing again for the next
                short Trunc = OSInfo.Count > 0 ? (short)(io / OSInfo.Count) : (short)0; // ~ mix by averaging -- did this because easy -- if no oscialorts, will divde by 0, sooo.. don't do that (give 0 instead)
                buffer[i * 2] = (byte)(Trunc & 0x00ff); // ~ store in little endian byte format before sending it off to the stream
                buffer[i * 2 + 1] = (byte)((Trunc & 0xff00) >> 8);

                if (AvailableSamples.Count < SampleQueueBufferLength) // ~ a buffer full of the most recennt samples which we use in the visualizer
                {
                    AvailableSamples.Enqueue(Trunc);
                    AvailableSampleCount++;
                }
                else
                {
                    AvailableSamples.Dequeue(); // ~ lose the oldest -- prefer newer ones
                    AvailableSamples.Enqueue(Trunc);
                }

            }
            //Debug.WriteLine("{0},{1},{2}", (Freqency + OscilatorFrequencyOffSet(Freqency, OSInfo[0])), OscilatorFrequencyOffSet(Freqency, OSInfo[0]), OSInfo[0].PitchShift);
            return count;
        }





        private double Freqency { get; set; }
        private double Amplitude;
        private double time;

        const int SampleRate = 44100;
        const short bitsPerSample = 16;


        public const int SampleQueueBufferLength = SampleRate / 20; // ~ the size of the buffer, smaller = less delay, larger = less artifacts (to a point)

        private readonly Queue<short> AvailableSamples = new(SampleQueueBufferLength); 
        public short AvailableSampleCount
        {
            get; private set;
        }
        public short CurrentSample
        {
            get { if (AvailableSamples.Count > 0) { AvailableSampleCount--; return AvailableSamples.Dequeue(); } else { AvailableSampleCount = 0; return (short)0; } } // ~ if theres noting there, give 0, but if there is pop off the oldest and decrement count
        }

        public Sound(int OscillatorNumber, double f, double a)
        {

            env = new Envelope(); // ~ default envelope, sould proably have this defined somehere do i don't need to change it here and in ADSR sliders

            OSInfo = new List<Oscillator>(OscillatorNumber);

            time = 0;
            Freqency = f; // ~ i don't think we need this anymore, but it doesn't breaak anything to have it there
            Amplitude = a;
            stream = new BlockAlignReductionStream(this);
            _soundOut = new(50); // ~ sets latency -- 50 is the lowest i could get it before audio degredation
            _soundOut.Init(stream);
            _soundOut.Play(); // ~ start playing the stream (calls read)
        }


        public void PlaySound(int key, float vel)
        {

            float freq = KeyToFreq(key);
            if (vel == 0) // ~ does literally nothing but i may want to do things with it later I dunno
            {

            }
            if (key > 127) // ~ ignore keys that are way too high
            {
                return;
            }
            env.triggerOn_t = time; // ~ set envolpoe start time
            env.triggerOff_t = 0; // ~ removes any previouse end time stuff

            Freqency = freq; // ~ set the freqeuncy
            Amplitude = vel / 127; // set the samplitude




        }
        public void StopSound()
        {

            env.triggerOff_t = time; // ~ set end of keypress time
            AvailableSamples.Clear(); //  ~ clear samples -- i don't think you need to do this but i initially did it because i though it would remove artifacts
        }

        readonly List<Oscillator> OSInfo; // ~ list of oscialtors incase we want to change rhe amount
        public Envelope env;

        public class Envelope
        {
            public double Attack_t; // ~ explained in ADSRsliders
            public double Decay_t;
            public double Release_t;

            public double Sustain_A;
            public double Attack_A;

            public double triggerOn_t;
            public double triggerOff_t;

            public double CurrentEnteringA; // ~ for if you release the key before the envelope reaches sustain, release from this amplitude
            public double CurrentAttackA; // ~ decay from this amplitude
            public double CurrentExitingA; // ~ attack from this amplitude (avoids clicks if release hsn't finished (would snap to 0 first))

            public Envelope() // ~ initial envelope values are a little arbitrary
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
    // ~ brought to you buy lerps, the one true savior of programing
        private double EnvelopeRamp(double t)
        {
            double RelT; // ~ relative time is nice to deal with
            double oooo; // ~ return value
            if (env.triggerOn_t > env.triggerOff_t) // ~ if we haven't release yet
            {
                RelT = t - env.triggerOn_t;
                if (RelT < 0) // ~ haven't pressd yet? just keey doing what your doin'
                {
                    oooo = 1;
                }
                else if (RelT <= env.Attack_t && env.Attack_t != 0) // ~ in the attack phase flip your car-- i mean lerp from release amplitude to attack amplitude
                {
                    oooo = double.Lerp(env.CurrentExitingA, env.Attack_A, RelT / env.Attack_t);
                    env.CurrentAttackA = oooo; // ~ store it for decay
                }
                else if (RelT <= env.Attack_t + env.Decay_t && env.Attack_t + env.Decay_t != 0) //~ in decay phase go from attack amp to sustain amp -- i don't think you actually need CuurentAttackA because there shoulnd't be a time where decay happens before attack finishes
                {
                    oooo = double.Lerp(env.CurrentAttackA, env.Sustain_A, RelT / (env.Attack_t + env.Decay_t));
                }
                else
                {
                    oooo = env.Sustain_A; // ~ at sustain, just keep sustaining
                }
                env.CurrentEnteringA = oooo; // ~ release ,ay happen at any time, to store current evnelope scale
            }
            else
            {
                RelT = t - env.triggerOff_t;
                if (RelT < 0) // ~ keep on trucking
                {
                    oooo = 1;
                }
                else if (RelT <= env.Release_t) // during release ramp down to 0
                {
                    oooo = double.Lerp(env.CurrentEnteringA, 0, RelT / env.Release_t);

                }
                else // ~ after release keep at 0 until next key when we don't come here anymore
                {
                    oooo = 0;
                }
                env.CurrentExitingA = oooo; // ~ store envelope amplitude to avoid snaping to 0 and causeing a click (though that seems to happen anyway)
            }

            return oooo;

        }


        public void AddOscillator(Oscillator oscillator)
        {
            OSInfo.Add(oscillator);
        }

        private static readonly Func<double, double>[] WaveTypes = // ~ collection of periodic functions that we call to avoid having a bunch of functions that all do the same thing wil a slight diffrence, also can more easy add functions later
        [
            Math.Sin,
            Square,
            Triangle,
            Noise,
        ];


        private static double Square(double x)
        {
            return Math.Sin(x) > 0 ? 1 : -1; // ~ threasshold sin
        }

        private static double Triangle(double x)
        {
            return Math.Asin(Math.Sin(x)) * 2 / Math.PI; // ~ for wierd math relationships, arcsin of sin max a trianlge graph (need to scale to -1 to 1 though)
        }

        static readonly Random random = new();
        private static double Noise(double x)
        {
            return random.NextDouble() * 2 - 1; // ~ the utter chaos of randomness is sometimes comfortimg -- also map to -1 to 1 (or 0.999999999999...)
        }

        private static double OscilatorFrequencyOffSet(double basefreq, Oscillator os)
        {

            return os.PitchShift >= 0 ? basefreq * (os.PitchShift) : basefreq / 2 * (os.PitchShift); // ~ maps from double the frequency to half (or in otherwords an octave above or below)

        }

        private static double OscillatorVelocityShift(double vel, Oscillator os)
        {
            return (vel * os.Vel) > 1.0 ? 1.0 : (vel * os.Vel); // ~ clmaps to 1 because i don't want to damage your ears

        }



        private static float KeyToFreq(int K)
        {
            return 400f * (float)Math.Pow(2f, (K - 69) / 12f); // ~ i stole this function from somewhere, but it turns midi keys (0-127) to a frequency that maps to a note in our music system

        }







        // ~ these are unused remnants of when i was using a different audio system that only worked with wav files

        private static readonly char[] dataHeader = ['d', 'a', 't', 'a'];
        private static readonly char[] RIFFHeader = ['R', 'I', 'F', 'F'];
        private static readonly char[] FormatHeader = ['W', 'A', 'V', 'E', 'f', 'm', 't', ' '];

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")] // ~ i didn't wan't visual studio to yell at me about this anymore because it was useless to me
        private static short[] MakeWave(float freq, float vel, Waves type) // ~ turn freqeuncy to sample short values
        {
            short[] samples = new short[SampleRate];
            for (int i = 0; i < SampleRate; i++)
            {
                //TODO: change oscillator
                samples[i] = Convert.ToInt16((short.MaxValue * (vel / 127)) * (WaveTypes[((int)type)](((2 * Math.PI * freq) / SampleRate) * i)));

            }
            return samples;

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")] // ~ shuts up visual studio
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private static MemoryStream LongMakeWav(float freq, byte[] sampleBytes) // ~ take a string of bytes and turn it into a wave format -- you can use this to hear bytes screaming
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
