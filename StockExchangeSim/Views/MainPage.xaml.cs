using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Syncfusion.UI.Xaml.Charts;
using Eco;
using System.Collections.ObjectModel;

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
        public double Year { get; set; }
        public double Value { get; set; }
        public CPValueData(double year, double value)
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
        LineSeries series = null;
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

        private int field = 1, trader = 1, hftrader = 1;

        public Master CreateMaster()
        {
            master = new Master(field, 1, 1);
            SetSliderValue();
            return master;
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
        public CompanyViewModel vm = new CompanyViewModel();
        public CompanyViewModel vm2 = new CompanyViewModel();
        public MainPage()
        {
            InitializeComponent();
            inst = this;
            if (master == null)
                CreateMaster();
            slider.ThumbToolTipValueConverter = new TooltipConverter(f => (f * f * 0.01 * 15));
            //DataContext = master;

            axisside.Header = "value (in $)";//master.Fields[0].companies[0].Value;
            axismain.Header = "Time (in years)";

            //series = new LineSeries()
            //{
            //    ItemsSource = vm.Data,
            //    XBindingPath = "Year",
            //    YBindingPath = "Value"
            //};
            CandleSeries candle = new CandleSeries()
            {
                ItemsSource = master.Fields[0].companies[0].stockViewModel.prices,
                XBindingPath = "Year",
                High = "High",
                Low = "Low",
                Open = "Open",
                Close = "Close"
            };
            //SplineSeries spline = new SplineSeries()
            //{
            //    ItemsSource = vm2.Data,
            //    XBindingPath = "Year",
            //    YBindingPath = "Value"
            //};
            candle.ListenPropertyChange = true;
            //series.ListenPropertyChange = true;
            chart.Series.Add(candle);
            //chart.Series.Add(series);
            DataContext = this;

            
            UpdateYear();

            
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


                vm.Data.Add(new CPValueData(year, master.Fields[0].companies[0].Value));
                //vm2.Data.Add(new CPValueData(year, Master.Conjucture));
                if (vm.Data.Count > 300)
                {
                    vm.Data.RemoveAt(0);
                }
                //if (vm2.Data.Count > 300)
                //{
                //    vm2.Data.RemoveAt(0);
                //}
                await Task.Delay(5);

            }
        }
        MessageDialog messageDialog = null;
        private async void slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (master != null)
            {
                if (messageDialog == null)
                {
                    messageDialog = new MessageDialog((slider.Value).ToString());
                    messageDialog.Commands.Add(new UICommand("Close"));
                    await messageDialog.ShowAsync();
                    //messageDialog = null;
                }
                double f = slider.Value;
                master.SetSecondsPerTick((f * f * 0.01));
            }

        }
        public void SetSliderValue()
        {
            double f = slider.Value;
            master.SetSecondsPerTick((f * f * 0.01));
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            master.active = false;
            master.alive = false;
            CreateMaster();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            master.active = false;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            master.active = true;
        }

        private async void FieldsAm_LostFocus(object sender, RoutedEventArgs e)
        {
            
            int val = 0;
            TextBox box = (TextBox)sender;
            try
            {
                val = int.Parse(box.Text);
            }
            catch (Exception ex)
            {
                MessageDialog messageDialog = new MessageDialog("Invalid seed");
                messageDialog.Commands.Add(new UICommand("Close"));
                await messageDialog.ShowAsync();
                return;
            }

            if (box.Name == "FieldsAm")
            {
                field = val;
            }

            if (box.Name == "TraderAm")
            {
                trader = val;
            }

            if (box.Name == "HFTradersAm")
            {
                hftrader = val;
            }
        }
    }
}
