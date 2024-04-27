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
    public class ADSRSliders : Grid
    {

        readonly Slider A; // ~ Attack time in seconds
        readonly Slider A_a; // ~ Attack peak amplitude scale(0-1)
        readonly Slider D; // ~ Decay time in seconds
        readonly Slider S; // ~ Sustain Amplitude scale (0-1); is NOT relative to attack pead
        readonly Slider R; // ~ Release time in seconds

        public ADSRSliders()
        {


            // ~ setting up the grid
            // ~ columns
            ColumnDefinition Cone = new() { Width = new GridLength(1, GridUnitType.Auto) };
            ColumnDefinition Ctwo = new() { Width = new GridLength(1, GridUnitType.Star) };
            // ~ rows
            RowDefinition Rone = new() { Height = new GridLength(1, GridUnitType.Star) };
            RowDefinition Rtwo = new() { Height = new GridLength(1, GridUnitType.Star) };
            RowDefinition Rthree = new() { Height = new GridLength(1, GridUnitType.Star) };
            RowDefinition Rfour = new() { Height = new GridLength(1, GridUnitType.Star) };
            RowDefinition Rfive = new() { Height = new GridLength(1, GridUnitType.Star) };


            // ~ adds grid definitions to this as is it extended from the grid control class
            ColumnDefinitions.Add(Cone);
            ColumnDefinitions.Add(Ctwo);

            RowDefinitions.Add(Rone);
            RowDefinitions.Add(Rtwo);
            RowDefinitions.Add(Rthree);
            RowDefinitions.Add(Rfour);
            RowDefinitions.Add(Rfive);


            // ~ this is not the best way to define the elements, but was the quickest to figure out while making this for the class
            // ~ and if it ain't broke...


            // ~ very hard to read, but essentailly sets up label font and position and sliders + events

            Label Attack = new()
            {
                Content = "Attack",
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 10, 0),

            };
            SetColumn(Attack, 0);
            SetRow(Attack, 0);
            Children.Add(Attack);
            A = new()
            {
                IsTabStop = false,
                Minimum = 0,
                Value = 0.01,
                Maximum = 2,
                VerticalAlignment = VerticalAlignment.Center,

            };
            A.ValueChanged += Attack_ValueChanged;
            SetColumn(A, 1);
            SetRow(A, 0);
            Children.Add(A);


            Label Attack_Peak = new()
            {
                Content = "Attack Peak",
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 10, 0),

            };
            SetColumn(Attack_Peak, 0);
            SetRow(Attack_Peak, 1);
            Children.Add(Attack_Peak);
            A_a = new()
            {
                IsTabStop = false,
                Minimum = 0,
                Value = 1.0,
                Maximum = 1,
                VerticalAlignment = VerticalAlignment.Center,
            };
            A_a.ValueChanged += Attack_Amplitude_ValueChanged;
            SetColumn(A_a, 1);
            SetRow(A_a, 1);
            Children.Add(A_a);


            Label Decay = new()
            {
                Content = "Decay",
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 10, 0),

            };
            SetColumn(Decay, 0);
            SetRow(Decay, 2);
            Children.Add(Decay);
            D = new()
            {
                IsTabStop = false,
                Minimum = 0,
                Value = 0.01,
                Maximum = 2,
                VerticalAlignment = VerticalAlignment.Center,
            };
            D.ValueChanged += Decay_ValueChanged;
            SetColumn(D, 1);
            SetRow(D, 2);
            Children.Add(D);


            Label Sustain = new()
            {
                Content = "Sustain",
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 10, 0),

            };
            SetColumn(Sustain, 0);
            SetRow(Sustain, 3);
            Children.Add(Sustain);
            S = new()
            {
                IsTabStop = false,
                Minimum = 0,
                Value = 0.8,
                Maximum = 1,
                VerticalAlignment = VerticalAlignment.Center,
            };
            S.ValueChanged += Sustain_ValueChanged;
            SetColumn(S, 1);
            SetRow(S, 3);
            Children.Add(S);


            Label Release = new()
            {
                Content = "Release",
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 10, 0),

            };
            SetColumn(Release, 0);
            SetRow(Release, 4);
            Children.Add(Release);
            R = new()
            {

                Minimum = 0,
                Value = 0.2,
                Maximum = 2,
                VerticalAlignment = VerticalAlignment.Center,
            };
            R.ValueChanged += Release_ValueChanged;
            SetColumn(R, 1);
            SetRow(R, 4);
            Children.Add(R);



        }


        // ~ all does the same thing but for each individual slider
        private void Attack_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Debug.Assert(sp != null);

            sp.env.Attack_t = e.NewValue;
        }

        private void Attack_Amplitude_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Debug.Assert(sp != null);

            sp.env.Attack_A = e.NewValue;
        }

        private void Decay_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Debug.Assert(sp != null);

            sp.env.Decay_t = e.NewValue;
        }

        private void Sustain_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Debug.Assert(sp != null);

            sp.env.Sustain_A = e.NewValue;
        }
        private void Release_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Debug.Assert(sp != null);

            sp.env.Release_t = e.NewValue;
        }


        // ~ a reference to my Sound class so i can modify the ADRS values directly -- again, probably not the best coding practice, but i nneeded somethign quick 
        Sound? sp;
        public void AddSoundPlayer(Sound sp)
        {
            this.sp = sp;
        }



    }
}
