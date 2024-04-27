using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Media;
using System.IO;
using System.Diagnostics;
using System.Windows.Automation.Provider;
using System.Windows.Threading;
using NAudio.Midi;
namespace SoundOff;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window

{


    readonly MidiIn? midiIn;

    readonly DispatcherTimer myTimer = new();

    public readonly Sound sp;
    public MainWindow()
    {
        InitializeComponent();



        sp = new(3, 1000, 0.0); // ~ set default dound amplitude to 0 so no sound until we want to

        sp.AddOscillator(oscillator1); // ~ add the oscillators from the XAML files
        sp.AddOscillator(oscillator2);
        sp.AddOscillator(oscillator3);



        KeyDown += OnKeyDown; // ~ events WOOO
        KeyUp += OnKeyUp;

        Graph.SetSoundPlayer(sp); // ~ graph is a visualizer and we want to connect is to a SOUND object to read the samples
        myTimer.Interval = new(0, 0, 0, 0, Graph.updateMS); // ~ sets graphe update time
        myTimer.Tick += Graph.DrawLines; // ~ then calls every time the timer exceeds the interval
        myTimer.Start(); // ~ timer don't start until we start it

        ASDREnvelope.AddSoundPlayer(sp); // ~ more binding the sound player


        //looks for midi devices
        if (MidiIn.NumberOfDevices > 0)
        {
            midiIn = new MidiIn(0); //~ uses the first one -- maybe not the best choice, but it worked for waht i needed it too
            midiIn.MessageReceived += MidiIn_MessageReceived;
            midiIn.ErrorReceived += (o, e) => Debug.WriteLine(e);
            midiIn.Start(); // ~ start midi capture
        }

        else
        {
            MessageBox.Show("You can still use the keyboard to play notes.\nTo use a Midi Device, plug one in and restart the process","No Midi Revice Detected"); // ~ this can get annoying, but notifies you if no midi device detected
        }

    }


    private int CurrentMidiNote;
    private void MidiIn_MessageReceived(object? sender, MidiInMessageEventArgs e)
    {
        //Debug.WriteLine("T:{0}, M:{1},E{2}", e.Timestamp, e.RawMessage, e.MidiEvent);
        if (MidiEvent.IsNoteOn(e.MidiEvent)) // ~ if we have a not on event we can treat is like one
        {
            if (((NoteOnEvent)e.MidiEvent).Velocity != 0) // ~ turn sound off if veloticy = 0
            {
                CurrentMidiNote = ((NoteOnEvent)e.MidiEvent).NoteNumber; // ~ sets most recent note because it feels better to use that was
                sp.PlaySound(((NoteOnEvent)e.MidiEvent).NoteNumber, ((NoteOnEvent)e.MidiEvent).Velocity);
            }
            else if (CurrentMidiNote == ((NoteOnEvent)e.MidiEvent).NoteNumber) // ~ only turn off if the most recent note got release -- avoids needing to time button presses perfectly to switch notes
            {
                sp.StopSound();
            }
        }
        else if (MidiEvent.IsNoteOff(e.MidiEvent) && (CurrentMidiNote == ((NoteEvent)e.MidiEvent).NoteNumber)) // ~ if we get a note off event, and it's the most recent note, then turn off
        {
            sp.StopSound();
        }
    }

    // ~ keyboard use when no midi (or even with midi keyboard is you want)
    private Key currentKey;

    int octaveShift = 0;
    const int octiveSize = 12;
    int velocity = 70;
    bool shiftDown = false;


    int playKey = 128;
    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Z || e.Key == Key.X || e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.LeftShift || e.Key == Key.RightShift) // ~ control keys z-x shift octive | up/down arrow change velocity | shift keys make velocity switching faster
        {
            switch (e.Key)
            {
                case Key.X:
                    if (77 + octaveShift * octiveSize < 127) // ~ don't go beyond bounds please (don't go too high)
                        octaveShift++;
                    break;
                case Key.Z:
                    if (60 + octaveShift * octiveSize > 0) // ~ don't go too low
                        octaveShift--;
                    break;
                case Key.Up:
                    if (velocity < 127 && !shiftDown) // ~ same but for velocity
                        velocity++;
                    else if (velocity + 10 < 127 && shiftDown)
                        velocity += 10;
                    else
                        velocity = 127;
                    break;
                case Key.Down:
                    if (velocity > 0 && !shiftDown)
                        velocity--;
                    else if (velocity - 10 > 0 && shiftDown)
                        velocity -= 10;
                    else
                        velocity = 0;
                    break;
                case Key.LeftShift:
                    shiftDown = true;
                    break;
                case Key.RightShift:
                    shiftDown = true;
                    break;
            }
            return;
        }
        if (e.Key == currentKey) // ~ don't press key again if it's being held down
        {
            return;
        }
        switch (e.Key) //~  yuck -- piano like layout -- 60 = c
        {
            case Key.A:
                playKey = 60 + (octaveShift * octiveSize);
                break;
            case Key.W:
                playKey = 61 + (octaveShift * octiveSize);
                break;
            case Key.S:
                playKey = 62 + (octaveShift * octiveSize);
                break;
            case Key.E:
                playKey = 63 + (octaveShift * octiveSize);
                break;
            case Key.D:
                playKey = 64 + (octaveShift * octiveSize);
                break;
            case Key.F:
                playKey = 65 + (octaveShift * octiveSize);
                break;
            case Key.T:
                playKey = 66 + (octaveShift * octiveSize);
                break;
            case Key.G:
                playKey = 67 + (octaveShift * octiveSize);
                break;
            case Key.Y:
                playKey = 68 + (octaveShift * octiveSize);
                break;
            case Key.H:
                playKey = 69 + (octaveShift * octiveSize);
                break;
            case Key.U:
                playKey = 70 + (octaveShift * octiveSize);
                break;
            case Key.J:
                playKey = 71 + (octaveShift * octiveSize);
                break;
            case Key.K:
                playKey = 72 + (octaveShift * octiveSize);
                break;
            case Key.O:
                playKey = 73 + (octaveShift * octiveSize);
                break;
            case Key.L:
                playKey = 74 + (octaveShift * octiveSize);
                break;
            case Key.P:
                playKey = 75 + (octaveShift * octiveSize);
                break;
            case Key.OemSemicolon:
                playKey = 76 + (octaveShift * octiveSize);
                break;
            case Key.OemQuotes:
                playKey = 77 + (octaveShift * octiveSize);
                break;
            default:
                return;
        }
        if (playKey > 127) //don't play too high notes
            return;
        sp.PlaySound(playKey, velocity);
        currentKey = e.Key;
    }
    private void OnKeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.RightShift || e.Key == Key.LeftShift)
        {
            shiftDown = false;
        }
        if (e.Key == currentKey)
        {
            currentKey = Key.None; // upon release, stop and no current key
            sp.StopSound();
        }
    }



    // ~ old stuff, very unsued, but i don't want to bury the corpse yet


    //middle C is 60 MIDI
    //KEY VELOCITY

    //void OnButtonClick1(object sender, RoutedEventArgs a)
    //{
    //    sp.OS1 = (Sound.Waves)Enum.Parse(typeof(Sound.Waves), ((Button)sender).Name.ToString().ToUpper());


    //}
    //void OnButtonClick2(object sender, RoutedEventArgs a)
    //{
    //    sp.OS2 = (Sound.Waves)Enum.Parse(typeof(Sound.Waves), ((Button)sender).Name.ToString().ToUpper());



    //}
    //void OnButtonClick3(object sender, RoutedEventArgs a)
    //{
    //    sp.OS3 = (Sound.Waves)Enum.Parse(typeof(Sound.Waves), ((Button)sender).Name.ToString().ToUpper());

    //}


}