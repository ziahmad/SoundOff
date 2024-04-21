
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
        private Sound.Waves WaveForm = Sound.Waves.SINE;
        public Button[] bArr = new Button[4];
        public Slider vel;
        public Slider PitchShift;




        //if grid doens't extend to bottom of oscillator window, we can add another grid for two sliders
        // -- volume control
        // -- pitch control -- may cause warp clicking
        // we will not have enough time to do differnt modulation types
        //liekly no time for MIDI control either
        public Oscillator()
        {
            Background = Brushes.Brown;
            if (count >= 3)
            {
                count = 0;
            }
            count++;
            Grid grid = new();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star) });
            grid.Width = this.Width;
            grid.Height = this.Height;
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
            bArr[0] = Sine;

            Button Sqr = new()
            {
                Name = "Square",
                Margin = new System.Windows.Thickness(5, 10, 5, 10),
                Content = "Square",
                Width = 50,
                Height = 30,

            };
            bArr[1] = Sqr; ;

            Button Tri = new()
            {
                Name = "Triangle",
                Margin = new System.Windows.Thickness(5, 10, 5, 10),
                Content = "Triangle",
                Width = 50,
                Height = 30,

            };
            bArr[2] = Tri;

            Button Noise = new()
            {
                Name = "Noise",
                Margin = new System.Windows.Thickness(5, 10, 10, 10),
                Content = "Noise",
                Width = 50,
                Height = 30,

            };
            bArr[3] = Noise;

            for (int i = 0; i < bArr.Length; i++)
            {
                Grid.SetColumn(bArr[i], i);
                bArr[i].Click += WaveButtonDown;
                grid.Children.Add(bArr[i]);
                if (bArr[i].Background != Brushes.Yellow)
                    bArr[i].Background = Brushes.LightGray;
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
            Grid sliderGrid = new Grid();
            //two col two rows
            sliderGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

            sliderGrid.ColumnDefinitions.Add(new ColumnDefinition());

            sliderGrid.RowDefinitions.Add(new RowDefinition());
            sliderGrid.RowDefinitions.Add(new RowDefinition());
            sliderGrid.Margin = new Thickness(10);

            Label vL = new Label()
            {
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            vL.Content = "Vel";
            Grid.SetColumn(vL, 0);
            Grid.SetRow(vL, 0);
            sliderGrid.Children.Add(vL);

            Label vR = new Label()
            {
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            vR.Content = "Pitch Shift";
            Grid.SetColumn(vR, 0);
            Grid.SetRow(vR, 1);
            sliderGrid.Children.Add(vR);



            vel = new();
            //vel.Margin = new Thickness(20, 0, 20, 10);
            vel.Maximum = 2;
            vel.Value = 1;
            vel.Minimum = 0;
            Grid.SetColumn(vel, 1);
            Grid.SetRow(vel, 0);
            sliderGrid.Children.Add(vel);

            PitchShift = new();
            //PitchShift.Margin = new Thickness(20, 10, 20, 0);
            PitchShift.Name = "PitchShift";
            PitchShift.Maximum = 1;
            PitchShift.Value = 0;
            PitchShift.Minimum = -1;
            Grid.SetColumn(PitchShift, 1);
            Grid.SetRow(PitchShift, 1);
            sliderGrid.Children.Add(PitchShift);

            sliderGrid.Width = this.Width;
            sliderGrid.Height = this.Height;



            this.AddChild(grid);
            this.Margin = new Thickness(3);
            //this.Width = 300;
            // this.Height = 400;



            this.AddChild(sliderGrid);










        }

        private void WaveButtonDown(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            this.WaveForm = (Sound.Waves)Enum.Parse(typeof(Sound.Waves), b.Name.ToUpper());
            //MessageBox.Show(b.Name);
            foreach (var item in bArr)
            {
                if (item.Name != b.Name)
                    item.Background = Brushes.LightGray;
                else
                    item.Background = Brushes.Yellow;
            }
        }
    }
}
