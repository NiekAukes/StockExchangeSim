using System;
using System.Collections.Generic;
using System.Threading;

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
        public static Random rn = new Random(Master.Seed);
        public static int TraderCreated = 0;
        public List<List<Stock>> stocks = new List<List<Stock>>();
        public List<Company> InterestedCompanies = null;
        public float Money = 100;
        public float BaseActionTimeRequired = 20 + (float)rn.NextDouble() * 20; //in seconds
        public float ActivityTime = 0;
        public float ActionTime = (float)rn.NextDouble() * 240; //in seconds
        public float skill = 1;
        public string name = null;
        public bool CatchUp = false;

        public Thread TraderThread = null;

        List<Strategy> Strategies = new List<Strategy>();

        public Trader()
        {
            //stocks & init
            Stocks = new List<Stock>();
            InterestedCompanies = Master.inst.GetAllCompanies();
            while (InterestedCompanies.Count > 4)
            {
                InterestedCompanies.RemoveAt(rn.Next(InterestedCompanies.Count));
            }

            //strategies
            StrategyFactory stFact = new StrategyFactory(this);
            while (Strategies.Count < 1) //pick strats till you have 2
            {
                Strategies.Add(stFact.RandomStrategy());
            }

            name = PickRandomName();

            //loops and threads
            TraderThread = new Thread(ThreadUpdate);
            TraderThread.Name = name;
            TraderThread.Start();
        }
        private string PickRandomName()
        {
            //search a random name in list of names
            string ret = "";
            if (Master.inst.MasterTraderNames.traderNames.Count > 0)
            {
                int rng = rn.Next(Master.inst.MasterTraderNames.traderNames.Count);
                ret = Master.inst.MasterTraderNames.traderNames[rng];
                Master.inst.MasterTraderNames.traderNames.RemoveAt(rng);
            }
            else
            {
                ret = "Trader " + TraderCreated;
            }
            TraderCreated++;
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

        public void ThreadUpdate()
        {
            while (Master.inst.alive)
            {
                if (Master.inst.active && !CatchUp)
                    Update();
                else
                    Thread.Sleep(10);
            }
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
                            System.Diagnostics.Debug.WriteLine("Internal Non-Fatal Error");
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
