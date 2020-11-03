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
    public class StockPrices
    {
        public double Year { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public StockPrices(double year, double open, double close, double high, double low)
        {
            Year = year;
            Open = open;
            Close = close;
            High = high;
            Low = low;
        }
    }
    public class StockViewModel
    {
        public ObservableCollection<StockPrices> prices = new ObservableCollection<StockPrices>();
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
                Master.exchange.SellStock(CompanyStock.SplitStock(1), stockprice);
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
        public void Update()
        {
            CurrentTick++;
            Competitiveness += -(Competitiveness - 100) * 0.000001 * MainPage.master.SecondsPerTick;

            double modifier = 1 / 2629743.8;
            double usableValue = Value;

            if (Value < 100)
            {
                usableValue += 500;
            }

            double totalprofit =
                (Math.Pow((Competitiveness / field.TotalCompetitiveness)
                * field.companies.Count, 2) * //calculate Competitive Position
                Master.Conjucture //multiply by conjucture and Economic growth
                - 1) * usableValue * //times value
                modifier * MainPage.master.SecondsPerTick;

            CompanyStock.Update(totalprofit);
            Value += CompanyStock.Collect();

            if (CurrentTick % 10 == 0)
            {
                LastDecemGain = -(LastDecemValue - Value) / LastDecemValue;
                LastDecemValue = Value;
            }
            if (CurrentTick % 100 == 0)
            {
                LastCentumGain = -(LastCentumValue - Value) / LastCentumValue;
                LastCentumValue = Value;
            }
            if (CurrentTick % 1000 == 0)
            {
                LastMilleGain = -(LastMilleValue - Value) / LastMilleValue;
                LastMilleValue = Value;
            }
            if (CurrentTick % 10000 == 0)
            {
                LastDeceMilleGain = -(LastDeceMilleValue - Value) / LastDeceMilleValue;
                LastDeceMilleValue = Value;
            }
            if (CurrentTick % 100000 == 0)
            {
                LastCentuMilleGain = -(LastCentuMilleValue - Value) / LastCentuMilleValue;
                LastCentuMilleValue = Value;
            }
            //check high and low
            if (CurrentTick % 100 == 0)
            {
                if (Value > high)
                    high = Value;
                if (Value < low)
                    low = Value;
            }
            //register datapoint
            if (CurrentTick % 500000 == 0)
            {
                StockPrices sp = new StockPrices(MainPage.master.Year, open, Value, high, low);
                var ignore = MainPage.inst.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    
                    stockViewModel.prices.Add(sp);

                    if (stockViewModel.prices.Count > 20)
                    {
                        stockViewModel.prices.RemoveAt(0);
                    }

                    
                });

                high = Value;
                low = Value;
                open = Value;
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
