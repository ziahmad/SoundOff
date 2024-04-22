using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        const int lineSegments = 300;

        public Visualizer()
        {

            Margin = new System.Windows.Thickness(0, 10, 10, 10);
            BorderThickness = new System.Windows.Thickness(3);
            CornerRadius = new System.Windows.CornerRadius(3);
            BorderBrush = Brushes.Black;
            Background = new SolidColorBrush(Color.FromRgb(167, 142, 142));



            this.Loaded += CDr;



        }

        public void CDr(object? sender, EventArgs e)
        {

            if (sender == null) return;
            Border b = (Border)sender;

            int centerY = (int)b.ActualHeight / 2;
            //int width = (int)Visualizer.ActualWidth;
            int width = (int)b.ActualWidth;
            int seglength = (int)width / lineSegments;

            polylineSegment = new(lineSegments);
            for (int i = 0; i < width; i += seglength)
            {
                polylineSegment.Add(new(i, centerY));
            }

            polyline = new();
            polyline.Points = polylineSegment;

            Debug.WriteLine("{0},{1}", b.Width, polyline.Width);

            polyline.Stroke = Brushes.Black;
            polyline.StrokeThickness = 2;

            b.Child = polyline;

        }
        Random random = new Random();
        PointCollection polylineSegment = new(lineSegments);
        Polyline polyline = new();
        public void CDr2(object? sender, EventArgs e)
        {
            MainWindow mw = ((MainWindow)((Grid)((Grid)Parent).Parent).Parent);
            polylineSegment.Clear();
            if (sender == null) return;

            int centerY = (int)this.ActualHeight / 2;
            //int width = (int)Visualizer.ActualWidth;
            int width = (int)this.ActualWidth;
            int seglength = (int)width / lineSegments;
            if (seglength <= 0)
            {
                seglength = 1;

            }

            short h=0;
            for (int i = 0; i < width; i += seglength)
            {
               // h = mw.sp.getCurrentSampleValue();
                //Debug.Write(h.ToString());
                polylineSegment.Add(new(i, centerY+(h/short.MaxValue*(this.ActualHeight/2-10))));
            }

            polyline.Points = polylineSegment;

            //Debug.WriteLine("{0},{1}", b.Width, polyline.Width);

            polyline.Stroke = Brushes.Black;
            polyline.StrokeThickness = 2;
            

            this.Child = polyline;

        }



    }
}
