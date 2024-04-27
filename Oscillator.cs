
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
    // ~ made to have a drop in way for multiple oscilators
    public class Oscillator : ItemsControl
    {
        static int count = 0; // ~ keep track of number of oscilators for naaming purposes
        public Sound.Waves WaveForm // ~ enum defined in sound specifying the wave type for the oscilator is selected
        {
            get; private set;
        } = Sound.Waves.SINE; // ~ default is sine
        public Button[] ButtonArray // button array so i could more easily do things for each button
        {
            get;
        } = new Button[4];

        public double Vel { get; private set; } = 1; // ~ value to hold oscilator volume scale
        public double PitchShift { get; private set; } = 0; // ~ same but for pitch


        // ~ below comment block is for furture development notes

        //if grid doens't extend to bottom of oscillator window, we can add another grid for two sliders // ~ the grid is as big as it needs to be to hold its inner controls, so i was considering how to use the space
        // -- volume control
        // -- pitch control -- may cause warp clicking
        // we will not have enough time to do differnt modulation types
        //liekly no time for MIDI control either

        // ~ aboce block is old and no longer true
        public Oscillator()
        {
            Background = Brushes.Brown; // ~ don't this has a viewable affect anymore
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

            //~ i know i said the array was for more easily doing things for each, but this was here before i added the array, so i didn't change it
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

            // ~ sound be self explanitory, but adds event handler, aset colum, and sets default color of button
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
                Text = "Oscillator " + count, // ~ the soul reason for the count variable
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center
            };


            AddChild(textBlock);



            //slider
            Grid sliderGrid = new();
            sliderGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            sliderGrid.ColumnDefinitions.Add(new ColumnDefinition());

            sliderGrid.RowDefinitions.Add(new RowDefinition());
            sliderGrid.RowDefinitions.Add(new RowDefinition());
            sliderGrid.Margin = new Thickness(10);

            Label vL = new() // ~ velocity slider label
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


            Label vR = new() // ~ pitch sldier lable, the name means nothing and was chosed by auto complete which didn't know what i was doing
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
                IsTabStop = false, // ~ avoids using tab so select it because it was causing conflict with using the arrow keys to control the overall velocity
                Maximum = 2, // ~ 2 bc wanted to have the option to make  oscillator louder than normal
                Value = 1, // ~ default
                Minimum = 0, // ~ 0 = no sound from this one

            };
            Grid.SetColumn(vel, 1);
            Grid.SetRow(vel, 0);
            sliderGrid.Children.Add(vel);

            Slider PitchShift = new()
            {
                IsTabStop = false,
                Maximum = 1,
                Value = 0,
                Minimum = -1,
                //TickFrequency = 2.0 / (24*2), // ~ i wanted to have a tick for every semitone up/down or an octive
                TickPlacement = System.Windows.Controls.Primitives.TickPlacement.BottomRight,
                //IsSnapToTickEnabled = true, // ~ but i didn't want to lose fine contoll -- uncomement thse to reenable it

            };

            Grid.SetColumn(PitchShift, 1);
            Grid.SetRow(PitchShift, 1);
            sliderGrid.Children.Add(PitchShift);

            sliderGrid.Width = Width; // ~ tries to set w/h to the oscialor width/height but i don't know if it has a visibale effect
            sliderGrid.Height = Height;

            PitchShift.ValueChanged += PitchSliderMove; // ~ adding events for when slider value changes -- self explanitory
            vel.ValueChanged += VelSliderMove;




            AddChild(grid);
            Margin = new Thickness(3);



            AddChild(sliderGrid);




        }


        void OnButtonClick(object sender, RoutedEventArgs a)
        {
            WaveForm = (Sound.Waves)Enum.Parse(typeof(Sound.Waves), ((Button)sender).Name.ToString().ToUpper()); // ~ this was taken from a tutorial somehwere but what is does is b/c the button names match
                                                                                                                 // ~ the wave type, we parse the name to the wave type and set the waveform to it.
                                                                                                                 // ~ unsued

        }

        private void VelSliderMove(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Vel = e.NewValue; // ~ function header auto generated, i don't know the event types by heart.

        }

        private void PitchSliderMove(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PitchShift = e.NewValue;
        }


        private void WaveButtonDown(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            WaveForm = (Sound.Waves)Enum.Parse(typeof(Sound.Waves), b.Name.ToUpper()); // ~ same as onbuttonclick but actually used
            //MessageBox.Show(b.Name);
            foreach (var item in ButtonArray) // sets button colors to show which one is selected
            {
                if (item.Name != b.Name)
                    item.Background = Brushes.LightGray;
                else
                    item.Background = Brushes.Yellow;
            }
        }
    }
}
