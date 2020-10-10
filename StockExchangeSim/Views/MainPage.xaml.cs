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
using Eco;

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
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public static Master master = null;
        public static MainPage inst;
        double _year;
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

        public MainPage()
        {
            InitializeComponent();
            if (master == null)
                master = new Master(1, 1, 1);
            inst = this;
            slider.ThumbToolTipValueConverter = new TooltipConverter(f => (f * f * 0.01));
            UpdateYear();


        }
        public async Task UpdateYear()
        {
            UInt64 ticks = 0;
            while (true)
            {
                ticks++;
                year = master.Year;
                if (ticks % 20 == 0)
                {
                    display = master.Fields[0].getInfo();
                }
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
    }
}
