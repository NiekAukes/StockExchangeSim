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
        public MainPage()
        {
            inst = this;
            this.InitializeComponent();
            Log = log;
            
            cp.stockPrices = new ObservableCollection<StockPriceGraph>
            {
                new StockPriceGraph(0, 3120.3, 3119.5, 3121, 3119),
                new StockPriceGraph(0.01, 3119.5, 3116.5, 3120.5, 3115.5),
                new StockPriceGraph(0.02, 3116.5, 3113.8, 3118.0, 3112.3),
                new StockPriceGraph(0.03, 3113.8, 3114.8, 3115.3, 3113.3),
                new StockPriceGraph(0.04, 3114.8, 3111.6, 3115.0, 3109.6),
                new StockPriceGraph(0.05, 3111.6, 3112.9, 3114.6, 3111.3),
                new StockPriceGraph(0.06, 3112.9, 3113.5, 3115.1, 3111.8),
                new StockPriceGraph(0.07, 3113.5, 3113.1, 3114.1, 3111.6),
                new StockPriceGraph(0.08, 3113.1, 3115.0, 3115.1, 3113.1),
                new StockPriceGraph(0.09, 3115.0, 3118.0, 3118.1, 3113.5),
                new StockPriceGraph(0.10, 3118.0, 3116.6, 3118.1, 3116.1),
                new StockPriceGraph(0.11, 3116.6, 3115.2, 3117.7, 3114.5),
                new StockPriceGraph(0.12, 3115.2, 3113.2, 3117.0, 3112.9),
                new StockPriceGraph(0.13, 3113.2, 3113.6, 3115.1, 3112.5),
                new StockPriceGraph(0.14, 3113.6, 3118.0, 3118.0, 3113.1),
                new StockPriceGraph(0.15, 3118.0, 3116.6, 3118.1, 3116.3),
                new StockPriceGraph(0.16, 3116.6, 3118.8, 3119.0, 3115.8),
                new StockPriceGraph(0.17, 3118.8, 3119.5, 3120.2, 3118.8),
                new StockPriceGraph(0.18, 3119.5, 3121.0, 3121.8, 3118.6),
                new StockPriceGraph(0.19, 3121.0, 3119.1, 3121.1, 3117.2),
                new StockPriceGraph(0.20, 3119.1, 3120.6, 3121.9, 3119.1),
                new StockPriceGraph(0.21, 3120.6, 3120.61, 3120.7, 3119.4),
                new StockPriceGraph(0.22, 3120.6, 3120.61, 3120.7, 3119.5),
                new StockPriceGraph(0.23, 3120.6, 3122.0, 3120.6, 3119.5),
                new StockPriceGraph(0.24, 3122.0, 3122.5, 3122.1, 3120.5),
                new StockPriceGraph(0.25, 3122.5, 3121.5, 3122.5, 3119.5),
                new StockPriceGraph(0.26, 3121.5, 3120.1, 3122.0, 3119.9),
                new StockPriceGraph(0.27, 3120.1, 3122.8, 3122.8, 3120.0),
                new StockPriceGraph(0.28, 3122.8, 3123.8, 3124.8, 3122.8),
                new StockPriceGraph(0.29, 3123.8, 3122.6, 3124.0, 3122.2),
                new StockPriceGraph(0.30, 3122.6, 3121.9, 3123.1, 3121.8),
                new StockPriceGraph(0.31, 3121.9, 3120.2, 3119.4, 3122.6),
                new StockPriceGraph(0.32, 3120.2, 3122.1, 3122.4, 3119.2),
                new StockPriceGraph(0.33, 3122.1, 3124.1, 3124.1, 3122.0),
                new StockPriceGraph(0.34, 3124.1, 3123.4, 3124.5, 3120.5),
                new StockPriceGraph(0.35, 3123.4, 3122.3, 3124.1, 3121.4),
                new StockPriceGraph(0.36, 3122.3, 3121.2, 3123.7, 3120.8),
                new StockPriceGraph(0.37, 3121.2, 3119.6, 3122.8, 3117.8),
                new StockPriceGraph(0.38, 3119.6, 3121.9, 3122.5, 3119.6),
                new StockPriceGraph(0.39, 3121.9, 3122.5, 3122.5, 3120.3),
                new StockPriceGraph(0.40, 3122.5, 3124.4, 3124.8, 3122.5),
                new StockPriceGraph(0.41, 3124.4, 3126.0, 3123.4, 3126.8),
                new StockPriceGraph(0.42, 3126.0, 3125.0, 3126.1, 3123.1),
                new StockPriceGraph(0.43, 3125.0, 3122.1, 3125.2, 3122.1),
                new StockPriceGraph(0.44, 3122.1, 3126.0, 3127.0, 3121.3),
                new StockPriceGraph(0.45, 3126.0, 3126.2, 3126.8, 3124.1),
                new StockPriceGraph(0.46, 3126.2, 3126.4, 3126.8, 3124.8),
                new StockPriceGraph(0.47, 3126.4, 3127.3, 3127.8, 3125.0),
                new StockPriceGraph(0.48, 3127.3, 3126.3, 3128.0, 3126.1),
                new StockPriceGraph(0.49, 3126.3, 3127.4, 3128.3, 3125.1),
                new StockPriceGraph(0.50, 3127.4, 3127.7, 3129.0, 3125.7),
                new StockPriceGraph(0.51, 3127.7, 3128.5, 3128.7, 3126.4),
                new StockPriceGraph(0.52, 3128.5, 3128.9, 3130.0, 3127.6),
                new StockPriceGraph(0.53, 3128.9, 3132.0, 3132.2, 3128.8),
                new StockPriceGraph(0.54, 3132.0, 3133.4, 3134.1, 3130.9),
                new StockPriceGraph(0.55, 3133.4, 3133.4, 3133.9, 3131.2),
            };

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

        private void button_Click(object sender, RoutedEventArgs e)
        {
            trader.Update();
        }
    }
}
