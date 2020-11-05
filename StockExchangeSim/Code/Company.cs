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
        public double Open { get; set; }
        public double Close { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public StockPriceGraph(double year, double open, double close, double high, double low)
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
        public double Value { get; set; }
        public ValueGraph(double year, double value)
        {
            Year = year;
            Value = value;
        }
    }
    public class StockViewModel
    {
        public ObservableCollection<StockPriceGraph> prices = new ObservableCollection<StockPriceGraph>();
    }
    public class ValueViewModel
    {
        public ObservableCollection<ValueGraph> values = new ObservableCollection<ValueGraph>();
    }
    public class Company : IStockOwner
    {
        public Field field = null;
        Random rn = new Random(Master.Seed);
        Stock CompanyStock = null;
        public List<Stock> Stocks { get; set; }
        public double Value = 50;
        public double stockprice = 0;
        public double Competitiveness = 100;

        public StockViewModel stockViewModel = new StockViewModel();
        public ValueViewModel ValueviewModel = new ValueViewModel();
        public List<StockPriceGraph> stockPrices = new List<StockPriceGraph>();
        public List<ValueGraph> values = new List<ValueGraph>();

        //setup values
        public int id;
        public Company(Field f)
        {
            CompanyStock = CreateStock(100);
            field = f;

            open = Value;
        }

        public void BecomePublic()
        {
            if (CompanyStock.Percentage == 100)
            {
                for (int i = 0; i < 1000; i++)
                {
                    Master.exchange.SellStock(CompanyStock.SplitStock(1 / 1000), stockprice);
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

        public double money { get { return Value; } set { Value = value; } }


        double open = 0;
        double high = 0;
        double low = 0;
        double calcprof()
        {
            double modifier = 1000 / 2629743.8;
            double usableValue = Value;

            if (Value < 100)
            {
                usableValue += 500;
            }
            double ret = (Math.Pow((Competitiveness / field.TotalCompetitiveness)
                * field.companies.Count, 2) * //calculate Competitive Position
                Master.Conjucture - 1) * //multiply by conjucture
                modifier * MainPage.master.SecondsPerTick * (0.01 * MainPage.master.Year + 1);//multiply by the modifier and Economic growth
            if (ret > 1)
            {
                throw new Exception(); //value too high
            }
            return ret;
                
        }
        public void Update()
        {
            CurrentTick++;
            Competitiveness += -(Competitiveness - 100) * 0.0000005 * MainPage.master.SecondsPerTick;

            double totalprofit = calcprof();

            CompanyStock.Update(totalprofit);
            Value += CompanyStock.Collect();

            

        }
        public void Data(int tick)
        {
            if (tick % 10 == 0)
            {
                LastDecemGain = -(LastDecemValue - Value) / LastDecemValue;
                LastDecemValue = Value;
            }
            if (tick % 100 == 0)
            {
                LastCentumGain = -(LastCentumValue - Value) / LastCentumValue;
                LastCentumValue = Value;
            }
            if (tick % 1000 == 0)
            {
                LastMilleGain = -(LastMilleValue - Value) / LastMilleValue;
                LastMilleValue = Value;
            }
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

            //check high and low
            if (tick % 20 == 0)
            {
                Stock st = Master.exchange.GetCheapestStock(this);
                if (st != null) {
                    double currentprice = st.SellPrice;
                    if (currentprice > high)
                        high = currentprice;
                    if (currentprice < low)
                        low = currentprice;

                    //register datapoint
                    if (tick % 100 == 0)
                    {
                        StockPriceGraph sp = new StockPriceGraph(MainPage.master.Year, open, currentprice, high, low);
                        stockPrices.Add(sp);
                        ValueGraph vg = new ValueGraph(MainPage.master.Year, Value);
                        values.Add(vg);


                        if (tick % 20000 == 0)
                        {
                            //StockPriceGraph sp = new StockPriceGraph(MainPage.master.Year, open, currentprice, high, low);


                            var ignore = MainPage.inst.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                            {

                                stockViewModel.prices.Add(sp);
                                ValueviewModel.values.Add(vg);

                                if (stockViewModel.prices.Count > 300)
                                {
                                    stockViewModel.prices.RemoveAt(0);
                                }


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
