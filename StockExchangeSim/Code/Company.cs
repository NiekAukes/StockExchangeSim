using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StockExchangeSim;
using StockExchangeSim.Views;

namespace Eco
{
    public class Company : IStockOwner
    {
        public Field field = null;
        Random rn = new Random(Master.Seed);
        Stock CompanyStock = null;
        public List<Stock> Stocks { get; set; }
        public double Value = 50;
        public double stockprice = 0;
        public double Competitiveness = 100;

        //setup values
        public int id;
        public Company(Field f)
        {
            CompanyStock = CreateStock(100);
            field = f;

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



        public void Update()
        {
            CurrentTick++;
            //value += Math.Pow((rn.NextDouble() - 0.5) * 5, 3); 
            Competitiveness += -(Competitiveness - 100) * 0.00001 * MainPage.master.SecondsPerTick;
            CompanyStock.Update();
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
