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
        readonly int lineSegments = 441; // ~ chosen to be a factor of the sample rate
        readonly PointCollection polylineSegment;
        readonly Polyline polyline = new();
        Sound? soundPlayer;

        public int updateMS = 10; // ~ how quickly we update the graph, used in a timer to call this

        public Visualizer()
        {
            polylineSegment = new(lineSegments);

            
            //format
            Margin = new System.Windows.Thickness(0, 10, 10, 10);
            BorderThickness = new System.Windows.Thickness(3);
            CornerRadius = new System.Windows.CornerRadius(3);
            BorderBrush = Brushes.Black;
            Background = new SolidColorBrush(Color.FromRgb(167, 142, 142));


            // ~ initial call too put something on the screen
            Loaded += DrawLines;



        }
        public void SetSoundPlayer(Sound player) // ~ soundplayer to get sample data from
        {
            soundPlayer = player;
        }



        public void DrawLines(object? sender, EventArgs e)
        {

            polylineSegment.Clear(); // ~ clear lines so clean redraw
            int centerY = (int)ActualHeight / 2;
            //int width = (int)Visualizer.ActualWidth;
            double width = ActualWidth;
            double seglength = width / lineSegments;

            double Sampeljump = (Sound.SampleQueueBufferLength * ( (double)updateMS/1000)) / lineSegments; // ~ number of samples to take per segment

            Sound sp;
            if (soundPlayer != null)
            {
                sp = soundPlayer; // ~ no soundplayer, do nothing
            }
            else return;

            short h = 0;
            for (int i = 0; i <= lineSegments; i++)
            {
                for(int j = 0; j< Sampeljump; j++)
                h = sp.CurrentSample;
                

                polylineSegment.Add(new(i*seglength, centerY + ((double)h / short.MaxValue * (ActualHeight / 2 - 10)))); // ~ x set based on how many segments we are across, y - change range to -1 to 1, then scale range to fit screen (minus some buffer space)

            }

            polyline.Points = polylineSegment; // ~ put point in polyline

            polyline.Stroke = Brushes.Black; //~ set color, this may be done outside of function because polyline is already defined
            polyline.StrokeThickness = 2;

            
            Child = polyline; // ~ make sure to set this or else you won't see you're line

        }



    }
}
