using System;
using System.Collections.Generic;
using System.Threading;

namespace Eco
{
    /*public enum HFTStrategy
    {
        MarketMaking,
        ArbitrageTrading,
        PairArbitrageTrading,
        MomentumIgnition,
        LiquidityDetection
    }*/
    
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
        public List<BuyOrder> buyOrders = new List<BuyOrder>();
        public List<SellOrder> sellOrders = new List<SellOrder>();
        public List<Company> InterestedCompanies = null;
        double _money = 0.1f * Master.MoneyScaler;
        public float Money { get { return (float)_money; } set { _money = value; } }
        public float money { get { return Money; } set { Money = value; } }
        public float BaseActionTimeRequired = 20 + (float)rn.NextDouble() * 20; //in seconds
        public float ActivityTime = 0;
        public float ActionTime = (float)rn.NextDouble() * 240; //in seconds
        public float skill = 1;
        public string name = null;
        public bool CatchUp = false;

        public string currentThought;

        public Thread TraderThread = null;
        public MarketResults latestResults;

        List<Strategy> Strategies = new List<Strategy>();
        public List<Stock> Stocks { get; set; }


        public Trader(bool isinvestor)
        {
            //stocks & init
            money = 0.1f * Master.MoneyScaler;
            Stocks = new List<Stock>();
            
            InterestedCompanies = Master.inst.GetAllCompanies();
            for (int i = 0; i < InterestedCompanies.Count; i++)
            {
                stocks.Add(new List<Stock>());
            }

            name = PickRandomName();
            //strategies
            if (isinvestor)
                Strategies.Add(new InvestorStrategy(this));
            else
                Strategies.Add(new MarketMakingStrategy(this));
            Strategies[0].Init();
            
            //loops and threads
            TraderThread = new Thread(ThreadUpdate);
            TraderThread.Name = name;
            TraderThread.Start();
        }
        public Trader()
        {
            //stocks & initialisation
            money = 0.1f * Master.MoneyScaler;

            Stocks = new List<Stock>();
            InterestedCompanies = Master.inst.GetAllCompanies();
            while (InterestedCompanies.Count > 4)
            {
                InterestedCompanies.RemoveAt(rn.Next(InterestedCompanies.Count));
            }
            for (int i = 0; i < InterestedCompanies.Count; i++)
            {
                stocks.Add(new List<Stock>());
            }
            name = PickRandomName();

            //strategies
            StrategyFactory stFact = new StrategyFactory(this);
            bool multistrat = rn.NextDouble() > 0.3;
            do //pick strats till you have 2
            {
                Strategies.Add(stFact.RandomStrategy(multistrat));
            }
            while (Strategies.Count < 2 && multistrat);


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

        public void AddStock(Stock stock)
        {
            stocks[InterestedCompanies.IndexOf(stock.company)].Add(stock);
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
        StockPriceComparisonTool SPCT = new StockPriceComparisonTool();
        public virtual void Update()
        {
            //ActionTime += MainPage.master.SecondsPerTick; //accrued Time for actions
            //foreach (var stockArr in stocks)
            //{
            //    for (int i = 0; i < stockArr.Count; i++)
            //    {
            //        Money += stockArr[i].Collect();
            //    }
            //}
            if (ActionTime > 0)
            {
                //TraderThread.Priority = ThreadPriority.AboveNormal;
                latestResults = new MarketResults();
                
                foreach (Strategy strat in Strategies)
                    latestResults = latestResults + strat.StrategyOutcome(this, Master.inst.exchange);

                foreach (var tp in latestResults.Results)
                {
                    if (!(Strategies[0] is InvestorStrategy || Strategies[0] is MarketMakingStrategy))
                    {
                        int index = InterestedCompanies.IndexOf(tp.Item1);
                        StockPriceComparisonToolData SPCTD = SPCT.StrategyOutcome(tp.Item1);
                        if (tp.Item2 < 0)
                        {
                            //sell stocks, if any

                            List<Stock> lsstocks = null;
                            try
                            {
                                lsstocks = stocks[index];

                                if (stocks[index].Count > 0)
                                {
                                    //for (int i = 0; i < tp.Item2 * 100 && i < lsstocks.Count; i++)
                                    //{
                                    //    //sell stocks with float value or until all stocks are sold
                                    //    //Master.inst.exchange.SellStock(lsstocks[i]);

                                    //}

                                    //sell stocks
                                    Master.inst.exchange.sellOrder(stocks[index],
                                        tp.Item1, (tp.Item1.stockprice + SPCTD.ExpectedStockPrice) / 2,
                                        (int)((tp.Item2 * -100)));

                                    currentThought = Thoughts.sell;
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
                                if (buyOrders[index] != null)
                                {
                                    //remove buyorder
                                    tp.Item1.BuyOrders.list.Remove(buyOrders[index]);
                                }
                                Master.inst.exchange.buyOrder(tp.Item1, this,
                                    (tp.Item1.stockprice + SPCTD.ExpectedStockPrice) / 2,
                                    (int)((tp.Item2 * 100)));

                            }
                        }
                    }
                }
            }
            else
            {
                if (ActionTime < -10)
                    Thread.Sleep(1);
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


    public override string ToString()
    {
        return name;
    }
}
