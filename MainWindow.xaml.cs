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
namespace SoundOff;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window

{
    private readonly Sound sp = new();
    const int lineSegments = 300;
    public MainWindow()
    {
        InitializeComponent();


        foreach (var line in oscillator1.bArr)
            line.Click += OnButtonClick1;
        foreach (var line in oscillator2.bArr)
            line.Click += OnButtonClick2;
        foreach (var line in oscillator3.bArr)
            line.Click += OnButtonClick3;

        this.KeyDown += OnKeyDown;
        this.KeyUp += OnKeyUp;
    }

    private Key currentKey;

    int octaveShift = 0;
    int octiveSize = 12;
    int velocity = 70;
    bool shiftDown = false;

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
        if (e.Key == currentKey && !sp.OverPercentDone(.8f))
        {
            return;
        }
        currentKey = e.Key;
        switch (e.Key)
        {
            case Key.A:
                sp.PlaySound(60 + (octaveShift * octiveSize), velocity);
                break;
            case Key.W:
                sp.PlaySound(61 + (octaveShift * octiveSize), velocity);
                break;
            case Key.S:
                sp.PlaySound(62 + (octaveShift * octiveSize), velocity);
                break;
            case Key.E:
                sp.PlaySound(63 + (octaveShift * octiveSize), velocity);
                break;
            case Key.D:
                sp.PlaySound(64 + (octaveShift * octiveSize), velocity);
                break;
            case Key.F:
                sp.PlaySound(65 + (octaveShift * octiveSize), velocity);
                break;
            case Key.T:
                sp.PlaySound(66 + (octaveShift * octiveSize), velocity);
                break;
            case Key.G:
                sp.PlaySound(67 + (octaveShift * octiveSize), velocity);
                break;
            case Key.Y:
                sp.PlaySound(68 + (octaveShift * octiveSize), velocity);
                break;
            case Key.H:
                sp.PlaySound(69 + (octaveShift * octiveSize), velocity);
                break;
            case Key.U:
                sp.PlaySound(70 + (octaveShift * octiveSize), velocity);
                break;
            case Key.J:
                sp.PlaySound(71 + (octaveShift * octiveSize), velocity);
                break;
            case Key.K:
                sp.PlaySound(72 + (octaveShift * octiveSize), velocity);
                break;
            case Key.O:
                sp.PlaySound(73 + (octaveShift * octiveSize), velocity);
                break;
            case Key.L:
                sp.PlaySound(74 + (octaveShift * octiveSize), velocity);
                break;
            case Key.P:
                sp.PlaySound(75 + (octaveShift * octiveSize), velocity);
                break;
            case Key.OemSemicolon:
                sp.PlaySound(76 + (octaveShift * octiveSize), velocity);
                break;
            case Key.OemComma:
                sp.PlaySound(77 + (octaveShift * octiveSize), velocity);
                break;
        }
    }
    private void OnKeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.RightShift || e.Key == Key.LeftShift)
        {
            shiftDown = false;
        }
        if (e.Key == currentKey)
        {
            sp.StopSound();
        }
    }






    //middle C is 60 MIDI
    //KEY VELOCITY

    static int indexer = 0;
    void OnButtonClick1(object sender, RoutedEventArgs a)
    {
        sp.OS1 = (Sound.Waves)Enum.Parse(typeof(Sound.Waves), ((Button)sender).Name.ToString().ToUpper());


    }
    void OnButtonClick2(object sender, RoutedEventArgs a)
    {
        sp.OS2 = (Sound.Waves)Enum.Parse(typeof(Sound.Waves), ((Button)sender).Name.ToString().ToUpper());



    }
    void OnButtonClick3(object sender, RoutedEventArgs a)
    {
        sp.OS3 = (Sound.Waves)Enum.Parse(typeof(Sound.Waves), ((Button)sender).Name.ToString().ToUpper());

    }
    void OnButtonClick4(object sender, RoutedEventArgs a)
    {
        Console.WriteLine("ButtClick");

        sp.PlaySound(440, 10);

        //if (indexer == 0)
        //{
        //    StartBut.Background = Brushes.Plum;
        //    indexer = 1;
        //}
        //else if (indexer == 1)
        //{
        //    StartBut.Background = Brushes.Red;
        //    indexer = 0;
        //}


    }

    void MouseOver(object sender, MouseEventArgs e)
    {

    }



    //void CanvasDraw()
    //{



    //}

}