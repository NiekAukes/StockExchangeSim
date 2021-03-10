using CsvHelper;
using StockExchangeSim.Views;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Windows.UI.Core;

namespace Eco
{
    //public static class JSONHelper
    //{
    //    public static string ToJSON(this object obj, int recursionDepth)
    //    {
    //        JavaScriptSerializer serializer = new JavaScriptSerializer();
    //        serializer.RecursionLimit = recursionDepth;
    //        return serializer.Serialize(obj);
    //    }
    //}
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
    public class Company : IStockOwner
    {
        public string name = null;
        public Field field = null;
        Random rn = new Random(Master.Seed);
        Stock CompanyStock = null;
        public float percentageSold { get { return 100 - CompanyStock.Percentage; } }
        public List<Stock> Stocks { get; set; }

        public SynchronizedCollection<BuyOrder> BuyOrders { get; set; }
        public SynchronizedCollection<SellOrder> SellOrders { get; set; }
        public BidAsk BidAsk;
        public double dValue = 50;
        public float Value { get { return (float)dValue; } set { dValue = (double)value; } }
        public float stockprice { get; set; }
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
        public float Dividend { get; set; }
        public float StockPart = 0.01f / 500;

        public Thread dataThread = null;


        public Company(Field f)
        {
            CompanyStock = CreateStock(100);
            field = f;

            BuyOrders = new SynchronizedCollection<BuyOrder>();
            SellOrders = new SynchronizedCollection<SellOrder>();

            open = Value;
            name = initName();

            //init datathreads

            //dataThread.Start();


            

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
                return CompanyStock.SplitStock(1, null);
            }
            return null;
        }
        public List<Stock> TradeStocks(float percentage, Trader buyer, bool forfree = false)
        {
            if (percentage <= 0)
                return new List<Stock>();
            int newstocksamount = 0;
            List<Stock> stocks = new List<Stock>();
            while (percentage > newstocksamount * StockPart)
            {
                newstocksamount++;
                stocks.Add(CompanyStock.SplitStock(StockPart, buyer));
            }
            if (!forfree)
            {
                buyer.money -= percentage * Value * .01f;
                Value += percentage * Value * 0.006f;
            }


            return stocks;
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
        float modifier = 300000 / 2629743.8f;

        public float money { get; set; }


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
            float ret = MathF.Log10(CompetitivePosition *
                Master.Conjucture) * //multiply by conjucture
                modifier;//multiply by the modifier and Economic growth

            return ret;

        }
        public override string ToString()
        {
            return name;
        }

        

        public void Update()
        {

            Competitiveness -= (Competitiveness - 100.0f) * 0.0000000001f * MainPage.master.SecondsPerTick;

            //calculate profit
            if (CurrentTick % 1000 == 0)
            {
                ValueGainPT = calcprof();
                Dividend = ValueGainPT > 0 ? ValueGainPT * StockPart : 0;
            }
            CompanyStock.Update(ValueGainPT * MainPage.master.SecondsPerTick);

            CurrentTick++;

        }
        float open5m, close5m, high5m, low5m;
        float open10m, close10m, high10m, low10m;
        float open30m, close30m, high30m, low30m;
        int checkModifier = 1;
        System.DateTime oldTime;

        
        public void Data(long tick, bool loop)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            
            
            do
            {
                if (Master.inst.active)
                {
                    if (true)
                    {
                        //check high and low
                        //sw.Restart();
                        if ((tick % checkModifier) == 0)
                        {
                            if (BidAsk != null)
                            {

                                float currentprice = Master.inst.exchange.GetCheapestHolderBid(this);
                                float cheapestseller = Master.inst.exchange.GetCheapestSeller(this);
                                if (cheapestseller > currentprice)
                                    stockprice = currentprice;
                                else
                                {
                                    currentprice = cheapestseller;
                                    stockprice = currentprice;
                                }



                                if (currentprice > high)
                                    high = currentprice;
                                if (currentprice < low)
                                    low = currentprice;



                                //register datapoint
                                if (tick % (5 * checkModifier) == 0)
                                {
                                    StockPriceGraph sp = new StockPriceGraph((float)MainPage.master.Year, open, currentprice, high, low);
                                    ValueGraph vg = new ValueGraph((float)MainPage.master.Year, Value);

                                    //List<Stockprice> prices = new List<StockPriceGraph>();
                                    //for(stockPrices1m.)

                                    //string jsonstring = stockPrices1m.


                                    lock (stockPrices1m)
                                    {
                                        if (stockPrices1m.Count > 10000)
                                        {
                                            stockPrices1m.RemoveAt(0);
                                        }

                                        stockPrices1m.Add(sp);
                                        lock (values)
                                        {

                                            values.Add(vg);
                                            if (values.Count > 10000)
                                            {
                                                values.RemoveAt(0);
                                            }
                                        }
                                    }
                                    //if (BidAsk.liquidity1m.Count > 1000)
                                    //{
                                    //    BidAsk.liquidity1m.RemoveAt(0);
                                    //}

                                    //BidAsk.liquidity1m.Add(new Liquidity(Master.inst.Year) { SellAmount = 0, BuyAmount = 0 });

                                    //create new highlow
                                    if (currentprice > high5m)
                                        high5m = currentprice;
                                    if (currentprice < low5m)
                                        low5m = currentprice;

                                    if (tick % (20 * checkModifier) == 0)
                                    {

                                        StockPriceGraph sp5m = new StockPriceGraph((float)MainPage.master.Year, open, currentprice, high, low);
                                        lock (stockPrices5m)
                                        {
                                            stockPrices5m.Add(sp5m);
                                            if (stockPrices5m.Count > 10000)
                                            {
                                                stockPrices5m.RemoveAt(0);
                                            }
                                        }
                                    }

                                    if (tick % (8000 * checkModifier) == 0)
                                    {
                                        //StockPriceGraph sp = new StockPriceGraph(MainPage.master.Year, open, currentprice, high, low);

                                        var ignore = MainPage.inst.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                        {

                                            stockViewModel.prices1m.Add(sp);
                                            //list thing = iets en add sp
                                            ValueviewModel.values.Add(vg);

                                            if (stockViewModel.prices1m.Count > 1000)
                                            {
                                                stockViewModel.prices1m.RemoveAt(0);
                                            }
                                            string str = MainPage.inst.Name;

                                        });



                                        high = currentprice;
                                        low = currentprice;
                                        open = currentprice;
                                    }
                                }

                            }

                        }
                        else
                        {
                            //yes
                        }
                        if (loop)
                        {
                            tick++;
                            //Thread.Sleep(1);
                        }
                    }
                }
                else
                {
                    Thread.Sleep(5);
                }
            } while (loop);
        }



        private Stock CreateStock(float percentage)
        {
            Stock ret = new Stock(this, percentage);
            ret.Owner = null;
            return ret;
        }


        public void UpdateHoldings()
        {
            for (int i = 0; i < Stocks.Count; i++)
            {
                if (Stocks[i].Owner != null)
                {
                    Stocks.RemoveAt(i);
                    i--;
                }
            }
        }

        public void AddStock(Stock stock)
        {
            Stocks.Add(stock);
            stock.Owner = null;
        }

    }
}
