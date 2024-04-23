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

        readonly Slider A;
        readonly Slider A_a;
        readonly Slider D;
        readonly Slider S;
        readonly Slider R;

        public ADSRSliders()
        {


            ColumnDefinition Cone = new() { Width = new GridLength(1, GridUnitType.Auto) };
            ColumnDefinition Ctwo = new() { Width = new GridLength(1, GridUnitType.Star) };


            RowDefinition Rone = new() { Height = new GridLength(1, GridUnitType.Star) };
            RowDefinition Rtwo = new() { Height = new GridLength(1, GridUnitType.Star) };
            RowDefinition Rthree = new() { Height = new GridLength(1, GridUnitType.Star) };
            RowDefinition Rfour = new() { Height = new GridLength(1, GridUnitType.Star) };
            RowDefinition Rfive = new() { Height = new GridLength(1, GridUnitType.Star) };

            ColumnDefinitions.Add(Cone);
            ColumnDefinitions.Add(Ctwo);

            RowDefinitions.Add(Rone);
            RowDefinitions.Add(Rtwo);
            RowDefinitions.Add(Rthree);
            RowDefinitions.Add(Rfour);
            RowDefinitions.Add(Rfive);



            Label Attack = new()
            {
                Content = "Attack",
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Right,
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
                HorizontalAlignment = HorizontalAlignment.Right,
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
                HorizontalAlignment = HorizontalAlignment.Right,
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
                HorizontalAlignment = HorizontalAlignment.Right,
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
                HorizontalAlignment = HorizontalAlignment.Right,
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
            };
            R.ValueChanged += Release_ValueChanged;
            SetColumn(R, 1);
            SetRow(R, 4);
            Children.Add(R);

        }

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

        Sound? sp;
        public void AddSoundPlayer(Sound sp)
        {
            this.sp = sp;
        }



    }
}
