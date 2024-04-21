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

            PointCollection polylineSegment = new(lineSegments);
            for (int i = 0; i < width; i += seglength)
            {
                polylineSegment.Add(new(i, centerY));
            }

            Polyline polyline = new();
            polyline.Points = polylineSegment;

            Debug.WriteLine("{0},{1}", b.Width, polyline.Width);

            polyline.Stroke = Brushes.Black;
            polyline.StrokeThickness = 2;

            b.Child = polyline;

        }



    }
}
