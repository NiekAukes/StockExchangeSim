using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;

namespace Eco
{
    public class StockPriceGraph
    {
        public float Year { get; set; }
        public float Open { get; set; }
        public float Close { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public StockPriceGraph(float year, float open, float close, float high, float low)
        {
            Year = year;
            Open = open;
            Close = close;
            High = high;
            Low = low;
        }
    }

    
    public class ValueGraph
    {
        public float Year { get; set; }
        public float Value { get; set; }
        public ValueGraph(float year, float value)
        {
            Year = year;
            Value = value;
        }
    }
    public class StockViewModel
    {
        public ObservableCollection<StockPriceGraph> prices1m = new ObservableCollection<StockPriceGraph>();
        public ObservableCollection<StockPriceGraph> prices5m = new ObservableCollection<StockPriceGraph>();
        public ObservableCollection<StockPriceGraph> prices10m = new ObservableCollection<StockPriceGraph>();
        public ObservableCollection<StockPriceGraph> prices30m = new ObservableCollection<StockPriceGraph>();
    }
    public class ValueViewModel
    {
        public ObservableCollection<ValueGraph> values = new ObservableCollection<ValueGraph>();
    }

    public class Company
    {
        
    }
}
