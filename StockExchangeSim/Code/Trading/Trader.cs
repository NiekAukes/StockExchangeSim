﻿using System;
using System.Collections.Generic;

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
        float money { get; set; }
    }
    public interface ITrader : IStockOwner
    {
        void AddStock(Stock stock);
        void SellStock(Company cp, int amount);
    }
    public partial class Trader : ITrader
    {
        Thoughts thoughts = new Thoughts();
        public static Random rn = new Random(Master.Seed);
        public List<List<Stock>> stocks = new List<List<Stock>>();
        public List<Company> InterestedCompanies = null;
        public float Money = 100;
        public float BaseActionTimeRequired = 20 + (float)rn.NextDouble() * 20; //in seconds
        public float ActivityTime = 0;
        public float ActionTime = (float)rn.NextDouble() * 240; //in seconds
        public float skill = 1;
        public string name = null;
        public class TraderThought
        {
            public Company comp;
            public string thought;
            public TraderThought()
            {
                comp = null; thought = null;
            }
        }

        public List<TraderThought> traderthoughts = null;
        //IMPLEMENT TRADER.THOUGHT
        public void UpdateTraderThought(object e)
        {
            //werkt deze MarketWatcher<Strategy>? Idk maarja t mot maar weer
            MarketWatcher<Strategy> watcher = (MarketWatcher<Strategy>)e;
            //vind een mogelijke match van watcher en thought
            if (traderthoughts != null || traderthoughts.Count < 1){
                for (int i = 0; i < traderthoughts.Count; i++)
                {
                    if(traderthoughts[i].comp == watcher.cp)
                    {
                        traderthoughts[i].thought = thoughts.buy; //TIJDELIJKE ONZIN
                        //CHOOSE TRADER THOUGHT BASED ON CHANGED MARKET CONDITIONS
                        //implement function that chooses thought based on events of the market watcher.
                        //so if there is a breakout, set thought "I'm going to buy" of weet ik veel.

                    }
                }
            }

        }
        List<Strategy> Strategies = new List<Strategy>();

        public Trader()
        {
            Stocks = new List<Stock>();
            InterestedCompanies = Master.inst.GetAllCompanies();
            while (InterestedCompanies.Count > 4)
            {
                InterestedCompanies.RemoveAt(rn.Next(InterestedCompanies.Count));
            }
            StrategyFactory stFact = new StrategyFactory(this);
            while (Strategies.Count < 1) //pick strats till you have 2
            {
                Strategies.Add(stFact.RandomStrategy());
            }

            name = PickRandomName();
        }
        private string PickRandomName()
        {
            //search a random name in list of names
            int rng = rn.Next(Master.inst.MasterTraderNames.traderNames.Count);
            string ret = Master.inst.MasterTraderNames.traderNames[rng];
            Master.inst.MasterTraderNames.traderNames.RemoveAt(rng);

            return ret;
        }
        public float money { get { return Money; } set { Money = value; } }

        public List<Stock> Stocks { get; set; }

        public void AddStock(Stock stock)
        {
            stocks[InterestedCompanies.IndexOf(stock.company)].Add(stock);
        }

        public override string ToString()
        {
            return name;
        }

        public virtual void Update()
        {
            //ActionTime += MainPage.master.SecondsPerTick; //accrued Time for actions
            foreach (var stockArr in stocks)
            {
                for (int i = 0; i < stockArr.Count; i++)
                {
                    Money += stockArr[i].Collect();
                }
            }
            if (ActionTime > 0)
            {
                MarketResults Final = new MarketResults();
                foreach (Strategy strat in Strategies)
                    Final = Final + strat.StrategyOutcome(this, Master.inst.exchange);

                foreach (var tp in Final.Results)
                {
                    if (tp.Item2 < 0)
                    {
                        //sell stocks, if any
                        List<Stock> lsstocks = null;
                        int index = InterestedCompanies.IndexOf(tp.Item1);
                        try
                        {
                            lsstocks = stocks[index];


                            if (stocks[index].Count > 0)
                            {
                                for (int i = 0; i < tp.Item2 * 100 && i < lsstocks.Count; i++)
                                {
                                    //sell stocks with float value or until all stocks are sold
                                    Master.inst.exchange.SellStock(lsstocks[i]);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine("Internal Non-Fatal Error: " + e.Message);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < tp.Item2 * 100; i++)
                        {
                            //buy stocks here
                            Master.inst.exchange.BuyStock(tp.Item1, this);
                        }
                    }

                }
            }


        }

        public void SellStock(Company cp, int amount)
        {
            int index = InterestedCompanies.IndexOf(cp);
            for (int i = 0; i < amount; i++)
            {
                Master.inst.exchange.SellStock(stocks[index][0]);
                stocks[index].RemoveAt(0);
            }
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
