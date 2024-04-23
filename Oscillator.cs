
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace SoundOff
{
    public class Oscillator : ItemsControl
    {
        static int count = 0;
        public Sound.Waves WaveForm
        {
            get; private set;
        } = Sound.Waves.SINE;
        public Button[] ButtonArray
        {
            get;
        } = new Button[4];

        public double Vel { get; private set; } = 1;
        public double PitchShift { get; private set; } = 0;





        //if grid doens't extend to bottom of oscillator window, we can add another grid for two sliders
        // -- volume control
        // -- pitch control -- may cause warp clicking
        // we will not have enough time to do differnt modulation types
        //liekly no time for MIDI control either
        public Oscillator()
        {
            Background = Brushes.Brown;
            count++;
            Grid grid = new();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            grid.Width = Width;
            grid.Height = Height;
            grid.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;


            Button Sine = new()
            {
                Name = "Sine",
                Margin = new System.Windows.Thickness(10, 10, 5, 10),
                Background = Brushes.Yellow,
                Content = "Sine",
                Width = 50,
                Height = 30

            };
            ButtonArray[0] = Sine;

            Button Sqr = new()
            {
                Name = "Square",
                Margin = new System.Windows.Thickness(5, 10, 5, 10),
                Content = "Square",
                Width = 50,
                Height = 30,

            };
            ButtonArray[1] = Sqr; ;

            Button Tri = new()
            {
                Name = "Triangle",
                Margin = new System.Windows.Thickness(5, 10, 5, 10),
                Content = "Triangle",
                Width = 50,
                Height = 30,

            };
            ButtonArray[2] = Tri;

            Button Noise = new()
            {
                Name = "Noise",
                Margin = new System.Windows.Thickness(5, 10, 10, 10),
                Content = "Noise",
                Width = 50,
                Height = 30,

            };
            ButtonArray[3] = Noise;

            for (int i = 0; i < ButtonArray.Length; i++)
            {
                Grid.SetColumn(ButtonArray[i], i);
                ButtonArray[i].Click += WaveButtonDown;
                grid.Children.Add(ButtonArray[i]);
                if (ButtonArray[i].Background != Brushes.Yellow)
                    ButtonArray[i].Background = Brushes.LightGray;
            }


            TextBlock textBlock = new()
            {
                Text = "Oscillator " + count,
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center
            };


            this.AddChild(textBlock);



            //slider
            Grid sliderGrid = new();
            sliderGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            sliderGrid.ColumnDefinitions.Add(new ColumnDefinition());

            sliderGrid.RowDefinitions.Add(new RowDefinition());
            sliderGrid.RowDefinitions.Add(new RowDefinition());
            sliderGrid.Margin = new Thickness(10);

            Label vL = new()
            {
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                Content = "Vel",
            };
            Grid.SetColumn(vL, 0);
            Grid.SetRow(vL, 0);
            sliderGrid.Children.Add(vL);


            Label vR = new()
            {
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                Content = "Pitch Shift",
            };
            Grid.SetColumn(vR, 0);
            Grid.SetRow(vR, 1);
            sliderGrid.Children.Add(vR);



            Slider vel = new()
            {
                Maximum = 2,
                Value = 1,
                Minimum = 0
            };
            Grid.SetColumn(vel, 1);
            Grid.SetRow(vel, 0);
            sliderGrid.Children.Add(vel);

            Slider PitchShift = new()
            {
                Maximum = 1,
                Value = 0,
                Minimum = -1
            };
            Grid.SetColumn(PitchShift, 1);
            Grid.SetRow(PitchShift, 1);
            sliderGrid.Children.Add(PitchShift);

            sliderGrid.Width = Width;
            sliderGrid.Height =Height;

            PitchShift.ValueChanged += PitchSliderMove;
            vel.ValueChanged += VelSliderMove;




            AddChild(grid);
            Margin = new Thickness(3);



            AddChild(sliderGrid);




        }


        void OnButtonClick(object sender, RoutedEventArgs a)
        {
            WaveForm = (Sound.Waves)Enum.Parse(typeof(Sound.Waves), ((Button)sender).Name.ToString().ToUpper());

        }

        private void VelSliderMove(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Vel = e.NewValue;

        }

        private void PitchSliderMove(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PitchShift = e.NewValue;
        }


        private void WaveButtonDown(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            WaveForm = (Sound.Waves)Enum.Parse(typeof(Sound.Waves), b.Name.ToUpper());
            //MessageBox.Show(b.Name);
            foreach (var item in ButtonArray)
            {
                if (item.Name != b.Name)
                    item.Background = Brushes.LightGray;
                else
                    item.Background = Brushes.Yellow;
            }
        }
    }
}
