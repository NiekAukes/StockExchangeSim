using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;

using Eco;
using Syncfusion.UI.Xaml.Charts;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace Eco
{
    public class Point
    {
        public float x = 0;
        public float y = 0;
        public Point() { }
        public Point(float X, float Y)
        {
            x = X;
            y = Y;
        }
    }
    public class Line
    {
        public float Adder;
        public float Multiplier;
        public Point Begin, End;
        public Line() { }
        public Line(Point b, Point e)
        {
            Multiplier = (b.y - e.y) / (b.x - e.x);
            Adder = -Multiplier * b.x + b.y;

            Begin = b;
            End = e;
        }

        public float Length { get { return MathF.Sqrt(MathF.Pow(Begin.x - End.x, 2) + MathF.Pow(Begin.y - End.y, 2)); } }

        public LineAnnotation ConvertToContinuousChartLine()
        {
            return new LineAnnotation()
            {
                X1 = Begin.x,
                X2 = 1,
                Y1 = Begin.y,
                Y2 = 1 * Multiplier + Adder,
                Stroke = new SolidColorBrush(Colors.DarkGray)
            };
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
