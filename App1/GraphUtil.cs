using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.
using System.Threading.Tasks;

using Eco;
using Syncfusion.UI.Xaml.Charts;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace App1
{
    public class Point
    {
        public float x;
        public float y;
        public Point() { }
        public Point(float X, float Y)
        {
            x = X;
            y = Y;
        }
    }
    public class Line
    {
        public Point Begin;
        public Point End;
        public Line() { }
        public Line(Point b, Point e)
        {
            Begin = b;
            End = e;
        }

        public LineAnnotation ConvertToChartLine()
        {
            return new LineAnnotation()
            {
                X1 = Begin.x,
                X2 = End.x,
                Y1 = Begin.y,
                Y2 = End.y,
                Stroke = new SolidColorBrush(Colors.DarkGray)
            };
        }
    }
    public class Graph
    {
        public ObservableCollection<StockPriceGraph> MainGraph = new ObservableCollection<StockPriceGraph>();
        public ObservableCollection<float> HorizontalLines = new ObservableCollection<float>();
        public ObservableCollection<float> VerticalLines = new ObservableCollection<float>();
        public ObservableCollection<Line> TrendLines = new ObservableCollection<Line>();
    }
}
