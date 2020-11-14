using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Syncfusion.UI.Xaml.Charts;
using Windows.UI.Xaml.Navigation;
using Eco;
using System.Collections.ObjectModel;
using System.Drawing;
using Windows.UI;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage inst;
        public static Company cp = new Company();
        public static Exchange exchange = new Exchange();
        public static Trader trader = new Trader();
        public static TextBlock Log;
        public ObservableCollection<StockPriceGraph> Realprices = new ObservableCollection<StockPriceGraph>
            {
                new StockPriceGraph(0, 3120.3f, 3119.5f, 3121, 3119),
                new StockPriceGraph(0.01f, 3119.5f, 3116.5f, 3120.5f, 3115.5f),
                new StockPriceGraph(0.02f, 3116.5f, 3113.8f, 3118.0f, 3112.3f),
                new StockPriceGraph(0.03f, 3113.8f, 3114.8f, 3115.3f, 3113.3f),
                new StockPriceGraph(0.04f, 3114.8f, 3111.6f, 3115.0f, 3109.6f),
                new StockPriceGraph(0.05f, 3111.6f, 3112.9f, 3114.6f, 3111.3f),
                new StockPriceGraph(0.06f, 3112.9f, 3113.5f, 3115.1f, 3111.8f),
                new StockPriceGraph(0.07f, 3113.5f, 3113.1f, 3114.1f, 3111.6f),
                new StockPriceGraph(0.08f, 3113.1f, 3115.0f, 3115.1f, 3113.1f),
                new StockPriceGraph(0.09f, 3115.0f, 3118.0f, 3118.1f, 3113.5f),
                new StockPriceGraph(0.10f, 3118.0f, 3116.6f, 3118.1f, 3116.1f),
                new StockPriceGraph(0.11f, 3116.6f, 3115.2f, 3117.7f, 3114.5f),
                new StockPriceGraph(0.12f, 3115.2f, 3113.2f, 3117.0f, 3112.9f),
                new StockPriceGraph(0.13f, 3113.2f, 3113.6f, 3115.1f, 3112.5f),
                new StockPriceGraph(0.14f, 3113.6f, 3118.0f, 3118.0f, 3113.1f),
                new StockPriceGraph(0.15f, 3118.0f, 3116.6f, 3118.1f, 3116.3f),
                new StockPriceGraph(0.16f, 3116.6f, 3118.8f, 3119.0f, 3115.8f),
                new StockPriceGraph(0.17f, 3118.8f, 3119.5f, 3120.2f, 3118.8f),
                new StockPriceGraph(0.18f, 3119.5f, 3121.0f, 3121.8f, 3118.6f),
                new StockPriceGraph(0.19f, 3121.0f, 3119.1f, 3121.1f, 3117.2f),
                new StockPriceGraph(0.20f, 3119.1f, 3120.6f, 3121.9f, 3119.1f),
                new StockPriceGraph(0.21f, 3120.6f, 3120.61f, 3120.7f, 3119.4f),
                new StockPriceGraph(0.22f, 3120.6f, 3120.61f, 3120.7f, 3119.5f),
                new StockPriceGraph(0.23f, 3120.6f, 3122.0f, 3120.6f, 3119.5f),
                new StockPriceGraph(0.24f, 3122.0f, 3122.5f, 3122.1f, 3120.5f),
                new StockPriceGraph(0.25f, 3122.5f, 3121.5f, 3122.5f, 3119.5f),
                new StockPriceGraph(0.26f, 3121.5f, 3120.1f, 3122.0f, 3119.9f),
                new StockPriceGraph(0.27f, 3120.1f, 3122.8f, 3122.8f, 3120.0f),
                new StockPriceGraph(0.28f, 3122.8f, 3123.8f, 3124.8f, 3122.8f),
                new StockPriceGraph(0.29f, 3123.8f, 3122.6f, 3124.0f, 3122.2f),
                new StockPriceGraph(0.30f, 3122.6f, 3121.9f, 3123.1f, 3121.8f),
                new StockPriceGraph(0.31f, 3121.9f, 3120.2f, 3122.6f, 3119.4f),
                new StockPriceGraph(0.32f, 3120.2f, 3122.1f, 3122.4f, 3119.2f),
                new StockPriceGraph(0.33f, 3122.1f, 3124.1f, 3124.1f, 3122.0f),
                new StockPriceGraph(0.34f, 3124.1f, 3123.4f, 3124.5f, 3120.5f),
                new StockPriceGraph(0.35f, 3123.4f, 3122.3f, 3124.1f, 3121.4f),
                new StockPriceGraph(0.36f, 3122.3f, 3121.2f, 3123.7f, 3120.8f),
                new StockPriceGraph(0.37f, 3121.2f, 3119.6f, 3122.8f, 3117.8f),
                new StockPriceGraph(0.38f, 3119.6f, 3121.9f, 3122.5f, 3119.6f),
                new StockPriceGraph(0.39f, 3121.9f, 3122.5f, 3122.5f, 3120.3f),
                new StockPriceGraph(0.40f, 3122.5f, 3124.4f, 3124.8f, 3122.5f),
                new StockPriceGraph(0.41f, 3124.4f, 3126.0f, 3126.8f, 3123.4f),
                new StockPriceGraph(0.42f, 3126.0f, 3125.0f, 3126.1f, 3123.1f),
                new StockPriceGraph(0.43f, 3125.0f, 3122.1f, 3125.2f, 3122.1f),
                new StockPriceGraph(0.44f, 3122.1f, 3126.0f, 3127.0f, 3121.3f),
                new StockPriceGraph(0.45f, 3126.0f, 3126.2f, 3126.8f, 3124.1f),
                new StockPriceGraph(0.46f, 3126.2f, 3126.4f, 3126.8f, 3124.8f),
                new StockPriceGraph(0.47f, 3126.4f, 3127.3f, 3127.8f, 3125.0f),
                new StockPriceGraph(0.48f, 3127.3f, 3126.3f, 3128.0f, 3126.1f),
                new StockPriceGraph(0.49f, 3126.3f, 3127.4f, 3128.3f, 3125.1f),
                new StockPriceGraph(0.50f, 3127.4f, 3127.7f, 3129.0f, 3125.7f),
                new StockPriceGraph(0.51f, 3127.7f, 3128.5f, 3128.7f, 3126.4f),
                new StockPriceGraph(0.52f, 3128.5f, 3128.9f, 3130.0f, 3127.6f),
                new StockPriceGraph(0.53f, 3128.9f, 3132.0f, 3132.2f, 3128.8f),
                new StockPriceGraph(0.54f, 3132.0f, 3133.4f, 3134.1f, 3130.9f),
                new StockPriceGraph(0.55f, 3133.4f, 3133.4f, 3133.9f, 3131.2f),
            };
        int counter = 0;
        public static float Year = 0;
    public MainPage()
        {
            inst = this;
            this.InitializeComponent();
            Log = log;

            cp.stockPrices = new ObservableCollection<StockPriceGraph>();

            axisside.Header = "value (in $)";
            axismain.Header = "Time (in years)";

            CandleSeries candle = new CandleSeries()
            {
                ItemsSource = cp.stockPrices,
                XBindingPath = "Year",
                High = "High",
                Low = "Low",
                Open = "Open",
                Close = "Close"
            };

            candle.BullFillColor = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 70, 220, 75));
            candle.BearFillColor = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 235, 30, 30));
            chart.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 30, 30, 35));
            //(chart.SecondaryAxis as NumericalAxis).RangePadding = NumericalPadding.Normal;
            //(chart.SecondaryAxis as NumericalAxis).Minimum = 3108;
            //(chart.SecondaryAxis as NumericalAxis).Maximum = 3136;
            (chart.SecondaryAxis as NumericalAxis).ZoomFactor = 1.4;
            candle.ComparisonMode = FinancialPrice.None;

            chart.Series.Add(candle);

            
            
        }
        TrendTool TrendTool = new TrendTool();
        SupportResistanceTool SupportResistanceTool = new SupportResistanceTool();
        private void button_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i + counter * 5 < Realprices.Count)
                    cp.stockPrices.Add(Realprices[i + counter * 5]);
            }
            if (cp.stockPrices.Count > 25)
            {
                for (int i = 0; i < 5; i++)
                {
                    cp.stockPrices.RemoveAt(0);
                }
            }

            counter++;
            Year += counter * 0.05f;
            //trader.Update();
            TrendData TData = TrendTool.StrategyOutcome(cp);
            AddContinuousline(TData.UpTrend, new SolidColorBrush(Colors.Cyan));
            AddContinuousline(TData.DownTrend, new SolidColorBrush(Colors.Magenta));

            SupportResistanceData data = SupportResistanceTool.StrategyOutcome(cp);

            AddContinuousline(data.MainSupport, new SolidColorBrush(Colors.LightGreen));
            AddContinuousline(data.MainResistance, new SolidColorBrush(Colors.Red));


            //foreach (Line ln in data.supportLevels)
            //{
            //    AddContinuousline(ln, new SolidColorBrush(Colors.LightGreen));
            //}
            //foreach (Line ln in data.resistanceLevels)
            //{
            //    AddContinuousline(ln, new SolidColorBrush(Colors.Red));
            //}


        }

        public void AddVerticalLine(float val, bool Pos = false)
        {
            chart.Annotations.Add(new VerticalLineAnnotation() { X1 = val, Y1 = 3000, Y2 = 5000, 
                Stroke = Pos ? new SolidColorBrush(Windows.UI.Color.FromArgb(255,255,0,0)) : new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 255, 0))});
        }
        public void AddHorizontalLine(float val)
        {
            chart.Annotations.Add(new HorizontalLineAnnotation() { Y1 = val, X1 = 0, X2 = 5 });
        }
        public void AddContinuousline(Line line, Brush bs)
        {
            if (line == null)
                return;
            LineAnnotation la = line.ConvertToContinuousChartLine();
            la.Stroke = bs;
            chart.Annotations.Add(la);
        }
        public void Addline(Line line, Brush bs)
        {
            LineAnnotation la = line.ConvertToChartLine();
            la.Stroke = bs;
            chart.Annotations.Add(la);
        }
    }
}
