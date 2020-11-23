using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using StockExchangeSim.Core.Models;
using StockExchangeSim.Core.Services;
using Syncfusion.UI.Xaml.Charts;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace StockExchangeSim.Views
{
    public class Observable<T> : INotifyPropertyChanged
    {
        public Observable(T val)
        {
            _val = val;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        T _val;
        public T Value { get { return _val; } set {
                _val = value;
                OnPropertyChanged();
            } }
        
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
    public class Person
    {
        public string Name { get; set; }

        public double Height { get; set; }
    }

    public class ViewModel
    {
        public List<Person> Data { get; set; }

        public ViewModel()
        {
            Data = new List<Person>()
            {
                new Person { Name = "David", Height = 180 },
                new Person { Name = "Michael", Height = 170 },
                new Person { Name = "Steve", Height = 160 },
                new Person { Name = "Joel", Height = 182 }
            };
        }
    }
    public sealed partial class ChartPage : Page, INotifyPropertyChanged
    {
        public ObservableCollection<DataPoint> Source { get; } = new ObservableCollection<DataPoint>();

        // TODO WTS: Change the chart as appropriate to your app.
        // For help see http://docs.telerik.com/windows-universal/controls/radchart/getting-started
        public ChartPage()
        {

            InitializeComponent();

            ColumnSeries series = new ColumnSeries();

            series.ItemsSource = (new ViewModel()).Data;
            series.XBindingPath = "Name";
            series.YBindingPath = "Height";

            //Adding Series to the Chart Series Collection
            //chart.Series.Add(series);
        }

        //Functions for fields
        #region FieldFunctions

        public void AddFieldPage(string fieldName)
        {
            FieldPage newPage = new FieldPage();
            //newPage.
            this.fieldGrid.Children.Add(newPage);
        }

        #endregion


        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Source.Clear();

            // TODO WTS: Replace this with your actual data
            var data = await SampleDataService.GetChartDataAsync();
            foreach (var item in data)
            {
                Source.Add(item);
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
