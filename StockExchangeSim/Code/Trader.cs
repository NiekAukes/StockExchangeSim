﻿using StockExchangeSim.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;

namespace Eco
{
    
    public enum HFTStrategy
    {
        MarketMaking, 
        ArbitrageTrading,
        PairArbitrageTrading,
        MomentumIgnition,
        LiquidityDetection
    }
    public interface IStockOwner
    {
        List<Stock> Stocks { get; set; }
        void AddStock(Stock stock);
        void UpdateHoldings();
        double money { get; set; }
    }
    public partial class Trader : IStockOwner
    {
        public static Random rn = new Random(Master.Seed);
        public double Money = 100;
        public double skill = 0.5;
        public double BaseActionTimeRequired = 20 + rn.NextDouble() * 20; //in seconds
        public double ActivityTime = 0;
        public double ActionTime; //in seconds

        Strategy strat = new PatternStrategy();

        public Trader()
        {
            Stocks = new List<Stock>();
            skill = rn.NextDouble() * 0.2 + 0.7;
        }

        public double money { get { return Money; } set { Money = value; } }

        public List<Stock> Stocks { get; set; }

        public void AddStock(Stock stock)
        {
            if (!Stocks.Contains(stock))
                Stocks.Add(stock);
            stock.Owner = this;
        }

        public Stock[] GetStocks()
        {
            return Stocks.ToArray();
        }

        public virtual void Update()
        {
            ActionTime += MainPage.master.SecondsPerTick; //accrued Time for actions
            for (int i = 0; i < Stocks.Count; i++)
            {
                Money += Stocks[i].Collect();
            }
            if (ActionTime > 0)
                strat.StrategyOutcome(this, Master.inst.exchange);


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
    }
    public class HFTrader : Trader
    {

    }
}
