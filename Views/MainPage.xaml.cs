using Eco;
using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

namespace StockExchangeSim.Views
{
    public class TooltipConverter : IValueConverter
    {
        Func<double, double> _ranger;

        public object Convert(object value, Type type, object parameter, string language)
        {
            double d = (double)value;
            return _ranger(d);
        }

        public object ConvertBack(object value, Type type, object parameter, string language)
        {
            throw new NotSupportedException();
        }

        public TooltipConverter(Func<double, double> ranger)
        {
            _ranger = ranger;
        }
    }

    public class CPValueData
    {
        public float Year { get; set; }
        public float Value { get; set; }
        public CPValueData(float year, float value)
        {
            Year = year;
            Value = value;
        }


    }
    public class CompanyViewModel
    {
        public ObservableCollection<CPValueData> Data = new ObservableCollection<CPValueData>();

    }

    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public static Master master = null;
        public static MainPage inst;
        double _year;

        bool badInput;

        public double year
        {
            get { return _year; }
            set
            {
                double days = (value - Math.Floor(value)) * 365.25;
                double Hours = ((days - Math.Floor(days)) * 24);
                int minutes = (int)((Hours - Math.Floor(Hours)) * 60);
                years.Text = "Years: " + Math.Floor(value).ToString() + " Days: " + Math.Floor(days).ToString() + " Time: " + Math.Floor(Hours).ToString() + ":" + minutes.ToString();
                _year = value;
            }
        }

        private int field = 1, trader = 20, hftrader = 5;

        public Master CreateMaster()
        {
            if (master != null)
            {
                master.alive = false;
                //REMOVE ALL REFERENCES TO A PREVIOUS MASTER
                //master.Traders.Clear();
                //master.TraderPools.Clear();
            }
            master = new Master(field, trader, hftrader);
            Master.inst = master;
            SetSliderValue();
            RedoLines();
            //for (int i = 0; i < master.Fields[0].companies.Count; i++)
            //{
            //    LineSeries series = new LineSeries()
            //    {
            //        ItemsSource = master.Fields[0].companies[i].ValueviewModel.values,
            //        XBindingPath = "Year",
            //        YBindingPath = "Value"
            //    };
            //    series.ListenPropertyChange = true;
            //    chart.Series.Add(series);
            ////}

            //dataThread = new Thread(GatherData);
            //dataThread.Name = "dataThread";
            //dataThread.Priority = ThreadPriority.Highest;
            //dataThread.Start();

            return master;
        }
        public void RedoLines()
        {
            chart.Series.Clear();
            FastCandleBitmapSeries candleSeries = new FastCandleBitmapSeries()
            {
                ItemsSource = master.Fields[0].companies[0].stockViewModel.prices1m,
                XBindingPath = "Year",
                High = "High",
                Low = "Low",
                Open = "Open",
                Close = "Close"
            };
            candleSeries.ListenPropertyChange = true;
            candleSeries.Interior = default;

            candleSeries.Trendlines.Add(new Trendline()
            {
                Label = "Trend",
                Type = TrendlineType.Polynomial,
                PolynomialOrder = 5
            });

            chart.Series.Add(candleSeries);

            compValChart.Series.Clear();
            FastLineBitmapSeries fastLineSeries = new FastLineBitmapSeries()
            {
                ItemsSource = master.Fields[0].companies[0].ValueviewModel.values,
                XBindingPath = "Year",
                YBindingPath = "Value"
            };

            fastLineSeries.ListenPropertyChange = true;
            fastLineSeries.Interior = default;

            compValChart.Series.Add(fastLineSeries);

            compvaluelabel.Text = "Value of company: " + master.Fields[0].companies[0].name;
            stockpricelabel.Text = "Stock prices of company: " + master.Fields[0].companies[0].name;
        }
        string _display;
        public string display
        {
            get { return _display; }
            set
            {
                cpinfo.Text = value;
                _display = value;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public CompanyViewModel vm = new CompanyViewModel();
        public CompanyViewModel vm2 = new CompanyViewModel();
        public MainPage()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            InitializeComponent();
            inst = this;
            if (master == null)
                CreateMaster();
            else
                RedoLines();
            slider.ThumbToolTipValueConverter = new TooltipConverter(f => (f * f * 0.01 * 15));
            //DataContext = master;

            axisside.Header = "Value ($)";//master.Fields[0].companies[0].Value;
            axismain.Header = "Time (years)";


            DataContext = this;


            UpdateYear();
        }

        ~MainPage()
        {
            master.active = false;
        }

        public void GatherData()
        {
            for (long tick = 0; master.alive || tick <= 200; tick++)
            {

                if (master.active)
                {
                    for (int i = 0; i < master.Fields.Count; i++)
                    {
                        Field field = master.Fields[i];
                        //do Information Gathering from fields
                        for (int j = 0; j < field.companies.Count; j++)
                        {

                            field.companies[j].Data(tick * 10, false);
                        }
                    }
                }

                else
                {
                    Thread.Sleep(10);
                }
            }
            System.Diagnostics.Debug.WriteLine("ended");
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SetSliderValue();
            //master.active = true;
        }

        public async Task UpdateYear()
        {
            UInt64 ticks = 0;
            while (true)
            {
                ticks++;
                year = master.Year;
                string dis = "";
                for (int i = 0; i < master.Fields.Count; i++)
                {
                    dis += master.Fields[i].getInfo();
                }
                display = dis;



                await Task.Delay(5);

            }
        }
        //MessageDialog messageDialog = null;
        private void slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (master != null)
            {
                double f = slider.Value;
                master.SetSecondsPerTick((float)(f * f * 0.01f));
            }

        }
        public void SetSliderValue()
        {
            double f = slider.Value;
            master.SetSecondsPerTick((float)(f * f * 0.01));
        }

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        async void ShowInvalidInputMessageBox()
        {
            MessageDialog messageDialog = new MessageDialog("Invalid input in Fields, Traders or HFTraders. Could not start Simulation");
            messageDialog.Commands.Add(new UICommand("Close"));
            await messageDialog.ShowAsync();
        }
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            if (!badInput)
            {
                master.active = false;
                master.alive = false;
                CreateMaster();
            }
            else
                ShowInvalidInputMessageBox();
        }
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            if(!badInput)
                master.active = false;
            else
                ShowInvalidInputMessageBox();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if(!badInput)
                master.active = true;
            else
            {
                ShowInvalidInputMessageBox();
            }
        }

        private void HFTraderCheckBox_Click(object sender, RoutedEventArgs e) => Master.HFTEnabled = (bool)HFTraderCheckBox.IsChecked;

        private void FieldsAm_LostFocus(object sender, RoutedEventArgs e)
        {

            int val = 0;
            TextBox box = (TextBox)sender;
            try
            {
                val = int.Parse(box.Text);
                badInput = false;
            }
            catch
            {
                ShowInvalidInputMessageBox();
                badInput = true;
            }

            if (box.Name == "FieldsAm" && !badInput)
            {
                field = val;
            }

            if (box.Name == "TraderAm" && !badInput)
            {
                trader = val;
            }
            /*
            if (box.Name == "HFTradersAm" && !badInput)
            {
                hftrader = val;
            }*/
        }
    }
}
