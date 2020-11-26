using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StockExchangeSim;
using StockExchangeSim.Views;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Eco
{
    public class StockPriceGraph
    {
        public double Year { get; set; }
        public float Open { get; set; }
        public float Close { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public StockPriceGraph(double year, float open, float close, float high, float low)
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
        public double Year { get; set; }
        public float Value { get; set; }
        public ValueGraph(double year, float value)
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
        public double Value = 50;
        public double stockprice = 0;
        public double Competitiveness = 100;

        //BidAsk bidAsk = null;

        public StockViewModel stockViewModel = new StockViewModel();
        public ValueViewModel ValueviewModel = new ValueViewModel();
        public List<StockPriceGraph> stockPrices1m = new List<StockPriceGraph>();
        public List<StockPriceGraph> stockPrices5m = new List<StockPriceGraph>();
        public List<StockPriceGraph> stockPrices10m = new List<StockPriceGraph>();
        public List<StockPriceGraph> stockPrices30m = new List<StockPriceGraph>();
        public List<ValueGraph> values = new List<ValueGraph>();

        //setup values
        public int id;
        public Company(Field f)
        {
            CompanyStock = CreateStock(100);
            field = f;

            open = (float)Value;
        }
        public void initName()
        {
            //search a random name in list of names
            name = "henk"; 
        }
        public void BecomePublic()
        {
            if (CompanyStock.Percentage == 100)
            {
                for (int i = 0; i < 50; i++)
                {
                    Master.inst.exchange.SellStock(CompanyStock.SplitStock(1.0 / 50.0), stockprice/50);
                }
            }
        }
        //variable values
        #region variableValues
        public void SetValue(double val)
        {
            Value = val;
            LastDecemValue = val;
            LastCentumValue = val;
            LastMilleValue = val;
            LastDeceMilleValue = val;
            LastCentuMilleValue = val;
        }
        public double age = 0;
        public bool Bankrupt = false;

        public double LastDecemValue = 0;
        public double LastCentumValue = 0;
        public double LastMilleValue = 0;
        public double LastDeceMilleValue = 0;
        public double LastCentuMilleValue = 0;

        //Gain Calculation
        public double LastTickGain = 0.0; //value Gained per tick
        public double LastTickSlope = 0.0; // gain Gained per tick
        public double LastDecemGain = 0;
        public double LastCentumGain = 0;
        public double LastMilleGain = 0;
        public double LastDeceMilleGain = 0;
        public double LastCentuMilleGain = 0;
        #endregion

        long CurrentTick = Master.ticks;
        double modifier = 30000 / 2629743.8;

        public double money { get { return Value; } set { Value = value; } }


        float open = 0;
        float high = 0;
        float low = 0;

        public double ValueGainPT = 0;
        double calcprof()
        {
            
            //double usableValue = Value;

            //if (Value < 100)
            //{
            //    usableValue += 500;
            //}
            double ret = ((Competitiveness / field.TotalCompetitiveness)
                * field.companies.Count * //calculate Competitive Position
                Master.Conjucture - 1) * //multiply by conjucture
                modifier;//multiply by the modifier and Economic growth
            return ret;
                
        }
        public void Update()
        {
            
            Competitiveness += -Math.Pow(Competitiveness - 100, 3) * 0.000001 * MainPage.master.SecondsPerTick;

            //calculate profit
            if (CurrentTick % 10 == 0)
            {
                ValueGainPT = calcprof();
            }
            CompanyStock.Update(ValueGainPT * MainPage.master.SecondsPerTick);
            Value += CompanyStock.Collect();

            CurrentTick++;

        }
        float open5m, close5m, high5m, low5m;
        float open10m, close10m, high10m, low10m;
        float open30m, close30m, high30m, low30m;
        public void Data(int tick)
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
                Stock st = Master.inst.exchange.GetCheapestStock(this);
                if (st != null) {
                    float currentprice = (float)st.SellPrice;
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

                        if (tick % 80000 == 0)
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



        private Stock CreateStock(double percentage)
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
