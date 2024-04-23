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


    MidiIn midiIn;

    readonly DispatcherTimer myTimer = new();

    public readonly Sound sp;
    public MainWindow()
    {
        InitializeComponent();



        sp = new(3, 1000, 0.0);

        sp.AddOscillator(oscillator1);
        sp.AddOscillator(oscillator2);
        sp.AddOscillator(oscillator3);



        KeyDown += OnKeyDown;
        KeyUp += OnKeyUp;

        Graph.SetSoundPlayer(sp);
        myTimer.Interval = new(0, 0, 0, 0, Graph.updateMS);
        myTimer.Tick += Graph.DrawLines;
        myTimer.Start();

        ASDREnvelope.AddSoundPlayer(sp);



        if (MidiIn.NumberOfDevices > 0)
        {
            midiIn = new MidiIn(0);
            midiIn.MessageReceived += MidiIn_MessageReceived;
            midiIn.ErrorReceived += (o, e) => Debug.WriteLine(e);
            midiIn.Start();
        }

        else
        {
            MessageBox.Show("No Midi Revice Detected", "You can still use the keyboard to play notes");
        }

    }


    private int CurrentMidiNote;
    private void MidiIn_MessageReceived(object? sender, MidiInMessageEventArgs e)
    {
        //Debug.WriteLine("T:{0}, M:{1},E{2}", e.Timestamp, e.RawMessage, e.MidiEvent);
        if (MidiEvent.IsNoteOn(e.MidiEvent))
        {
            if (((NoteOnEvent)e.MidiEvent).Velocity != 0)
            {
                CurrentMidiNote = ((NoteOnEvent)e.MidiEvent).NoteNumber;
                sp.PlaySound(((NoteOnEvent)e.MidiEvent).NoteNumber, ((NoteOnEvent)e.MidiEvent).Velocity);
            }
            else if (CurrentMidiNote == ((NoteOnEvent)e.MidiEvent).NoteNumber)
            {
                sp.StopSound();
            }
        }
        else if (MidiEvent.IsNoteOff(e.MidiEvent) && (CurrentMidiNote == ((NoteEvent)e.MidiEvent).NoteNumber))
        {
            sp.StopSound();
        }
    }

    private Key currentKey;

    int octaveShift = 0;
    const int octiveSize = 12;
    int velocity = 70;
    bool shiftDown = false;


    int playKey = 128;
    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Z || e.Key == Key.X || e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.LeftShift || e.Key == Key.RightShift)
        {
            switch (e.Key)
            {
                case Key.X:
                    if (77 + octaveShift * octiveSize < 127)
                        octaveShift++;
                    break;
                case Key.Z:
                    if (60 + octaveShift * octiveSize > 0)
                        octaveShift--;
                    break;
                case Key.Up:
                    if (velocity < 127 && !shiftDown)
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
        if (e.Key == currentKey)
        {
            return;
        }
        switch (e.Key)
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
        if (playKey > 127)
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
            currentKey = Key.None;
            sp.StopSound();
        }
    }






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