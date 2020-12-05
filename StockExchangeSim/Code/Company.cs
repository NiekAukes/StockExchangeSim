using StockExchangeSim.Views;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Core;

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
        public ObservableCollection<StockPriceGraph> prices15m = new ObservableCollection<StockPriceGraph>();
        public ObservableCollection<StockPriceGraph> prices30m = new ObservableCollection<StockPriceGraph>();
    }
    public class ValueViewModel
    {
        public ObservableCollection<ValueGraph> values = new ObservableCollection<ValueGraph>();
    }
    public class Company : IStockOwner
    {
        public string name = null;
        public Field field = null;
        Random rn = new Random(Master.Seed);
        Stock CompanyStock = null;
        public List<Stock> Stocks { get; set; }
        public BidAsk BidAsk;
        public float Value = 50;
        public float stockprice = 0;
        public float Competitiveness = 100;

        //BidAsk bidAsk = null;

        public StockViewModel stockViewModel = new StockViewModel();
        public ValueViewModel ValueviewModel = new ValueViewModel();
        public SynchronizedCollection<StockPriceGraph> stockPrices1m = new SynchronizedCollection<StockPriceGraph>();
        public SynchronizedCollection<StockPriceGraph> stockPrices5m = new SynchronizedCollection<StockPriceGraph>();
        public SynchronizedCollection<StockPriceGraph> stockPrices10m = new SynchronizedCollection<StockPriceGraph>();
        public SynchronizedCollection<StockPriceGraph> stockPrices30m = new SynchronizedCollection<StockPriceGraph>();
        public SynchronizedCollection<ValueGraph> values = new SynchronizedCollection<ValueGraph>();

        //setup values
        public int id;
        public float CompetitivePosition = 1;
        public float Dividend = 0;
        public float StockPart = 0.01f / 50;
        public Company(Field f)
        {
            CompanyStock = CreateStock(100);
            field = f;

            open = (float)Value;
            name = initName();
        }
        public string initName()
        {
            //search a random name in list of names
            int rng = rn.Next(field.NameInfo.CompanyNames.Count);
            string ret = field.NameInfo.CompanyNames[rng];
            field.NameInfo.CompanyNames.RemoveAt(rng);

            return ret;
        }
        public Stock BecomePublic()
        {
            if (CompanyStock.Percentage == 100)
            {
                return CompanyStock.SplitStock(1);
            }
            return null;
        }
        //variable values
        #region variableValues
        public void SetValue(float val)
        {
            Value = val;
            LastDecemValue = val;
            LastCentumValue = val;
            LastMilleValue = val;
            LastDeceMilleValue = val;
            LastCentuMilleValue = val;
        }
        public float age = 0;
        public bool Bankrupt = false;

        public float LastDecemValue = 0;
        public float LastCentumValue = 0;
        public float LastMilleValue = 0;
        public float LastDeceMilleValue = 0;
        public float LastCentuMilleValue = 0;

        //Gain Calculation
        public float LastTickGain = 0.0f; //value Gained per tick
        public float LastTickSlope = 0.0f; // gain Gained per tick
        public float LastDecemGain = 0;
        public float LastCentumGain = 0;
        public float LastMilleGain = 0;
        public float LastDeceMilleGain = 0;
        public float LastCentuMilleGain = 0;
        #endregion

        long CurrentTick = Master.ticks;
        float modifier = 30000 / 2629743.8f;

        public float money { get { return Value; } set { Value = value; } }


        float open = 0;
        float high = 0;
        float low = 0;

        public float ValueGainPT = 0;
        float calcprof()
        {

            //float usableValue = Value;

            //if (Value < 100)
            //{
            //    usableValue += 500;
            //}
            CompetitivePosition = (Competitiveness / field.TotalCompetitiveness) //calculate Competitive Position
                * field.companies.Count;
            float ret = (CompetitivePosition *
                Master.Conjucture - 1) * //multiply by conjucture
                modifier;//multiply by the modifier and Economic growth
            return ret;

        }
        public void Update()
        {

            Competitiveness += -MathF.Pow(Competitiveness - 100, 3) * 0.00000001f * MainPage.master.SecondsPerTick;

            //calculate profit
            if (CurrentTick % 10 == 0)
            {
                ValueGainPT = calcprof();
                Dividend = ValueGainPT > 0 ? ValueGainPT * StockPart : 0;
            }
            CompanyStock.Update(ValueGainPT * MainPage.master.SecondsPerTick);
            Value += CompanyStock.Collect();

            CurrentTick++;

        }
        float open5m, close5m, high5m, low5m;
        float open10m, close10m, high10m, low10m;
        float open30m, close30m, high30m, low30m;
        public void Data(long tick)
        {
            if (tick % 10 == 0)
            {
                LastDecemGain = -(LastDecemValue - Value) / LastDecemValue;
                LastDecemValue = Value;

                if (tick % 100 == 0)
                {
                    LastCentumGain = -(LastCentumValue - Value) / LastCentumValue;
                    LastCentumValue = Value;

                    if (tick % 1000 == 0)
                    {
                        LastMilleGain = -(LastMilleValue - Value) / LastMilleValue;
                        LastMilleValue = Value;

                        if (tick % 10000 == 0)
                        {
                            LastDeceMilleGain = -(LastDeceMilleValue - Value) / LastDeceMilleValue;
                            LastDeceMilleValue = Value;
                        }
                        if (tick % 100000 == 0)
                        {
                            LastCentuMilleGain = -(LastCentuMilleValue - Value) / LastCentuMilleValue;
                            LastCentuMilleValue = Value;
                        }
                    }
                }
            }

            //check high and low
            if (tick % 20 == 0)
            {
                if (BidAsk != null)
                {
                    float currentprice = BidAsk.Bid;
                    if (currentprice > high)
                        high = currentprice;
                    if (currentprice < low)
                        low = currentprice;



                    //register datapoint
                    if (tick % 100 == 0)
                    {
                        StockPriceGraph sp = new StockPriceGraph(MainPage.master.Year, open, currentprice, high, low);
                        stockPrices1m.Add(sp);
                        ValueGraph vg = new ValueGraph(MainPage.master.Year, (float)Value);
                        values.Add(vg);
                        if (stockPrices1m.Count > 10000)
                        {
                            stockPrices1m.RemoveAt(0);
                        }

                        //create new highlow
                        if (currentprice > high5m)
                            high5m = currentprice;
                        if (currentprice < low5m)
                            low5m = currentprice;

                        if (tick % 500 == 0)
                        {
                            StockPriceGraph sp5m = new StockPriceGraph(MainPage.master.Year, open, currentprice, high, low);
                            stockPrices5m.Add(sp5m);
                            if (stockPrices5m.Count > 10000)
                            {
                                stockPrices5m.RemoveAt(0);
                            }
                        }

                        if (tick % 800000 == 0)
                        {
                            //StockPriceGraph sp = new StockPriceGraph(MainPage.master.Year, open, currentprice, high, low);


                            var ignore = MainPage.inst.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {

                                stockViewModel.prices1m.Add(sp);
                                ValueviewModel.values.Add(vg);

                                if (stockViewModel.prices1m.Count > 100)
                                {
                                    stockViewModel.prices1m.RemoveAt(0);
                                }
                                //MainPage.inst.SetNewYearLimit();

                            });

                            high = currentprice;
                            low = currentprice;
                            open = currentprice;
                        }
                    }

                }

            }
        }



        private Stock CreateStock(float percentage)
        {
            Stock ret = new Stock(this, percentage);
            ret.Owner = this;
            return ret;
        }


        public void UpdateHoldings()
        {
            for (int i = 0; i < Stocks.Count; i++)
            {
                if (Stocks[i].Owner != this)
                {
                    Stocks.RemoveAt(i);
                    i--;
                }
            }
        }

        public void AddStock(Stock stock)
        {
            Stocks.Add(stock);
            stock.Owner = this;
        }

    }
}
