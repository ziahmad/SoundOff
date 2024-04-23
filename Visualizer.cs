using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SoundOff
{
    public class Visualizer : Border
    {
        readonly int lineSegments = 441;
        readonly PointCollection polylineSegment;
        readonly Polyline polyline = new();
        Sound? soundPlayer;

        public int updateMS = 10;

        public Visualizer()
        {
            polylineSegment = new(lineSegments);

            

            Margin = new System.Windows.Thickness(0, 10, 10, 10);
            BorderThickness = new System.Windows.Thickness(3);
            CornerRadius = new System.Windows.CornerRadius(3);
            BorderBrush = Brushes.Black;
            Background = new SolidColorBrush(Color.FromRgb(167, 142, 142));



            Loaded += DrawLines;



        }
        public void SetSoundPlayer(Sound player)
        {
            soundPlayer = player;
        }



        public void DrawLines(object? sender, EventArgs e)
        {

            polylineSegment.Clear();
            int centerY = (int)ActualHeight / 2;
            //int width = (int)Visualizer.ActualWidth;
            double width = ActualWidth;
            double seglength = width / lineSegments;

            double Sampeljump = (Sound.SampleQueueBufferLength * ( (double)updateMS/1000)) / lineSegments;

            Sound sp;
            if (soundPlayer != null)
            {
                sp = soundPlayer;
            }
            else return;

            short h = 0;
            for (int i = 0; i <= lineSegments; i++)
            {
                for(int j = 0; j< Sampeljump; j++)
                h = sp.CurrentSample;
                

                polylineSegment.Add(new(i*seglength, centerY + ((double)h / short.MaxValue * (ActualHeight / 2 - 10))));

            }

            polyline.Points = polylineSegment;

            polyline.Stroke = Brushes.Black;
            polyline.StrokeThickness = 2;

            
            Child = polyline;

        }



    }
}
