using System;
using System.Collections.Generic;
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
            grid.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;


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

            this.AddChild(grid);
            this.Margin = new Thickness(3);
            this.Width = 300;
            this.Height = 400;








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
