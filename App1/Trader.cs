using App1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        double money { get; set; }
    }
    public interface ITrader : IStockOwner
    {
        void AddStock(Stock stock);
        void SellStock(Company cp, int amount);
    }
    public partial class Trader : ITrader
    {
        public static Random rn = new Random();
        public List<List<Stock>> stocks = new List<List<Stock>>();
        public List<Company> InterestedCompanies = new List<Company>();
        public double Money = 100;
        public double BaseActionTimeRequired = 20 + rn.NextDouble() * 20; //in seconds
        public double ActivityTime = 0;
        public double ActionTime = rn.NextDouble() * 240; //in seconds
        public double skill = 1;

        List<Strategy> Strategies = new List<Strategy>();

        public Trader()
        {
            Stocks = new List<Stock>();
            InterestedCompanies.Add(MainPage.cp);
            Strategies.Add(StrategyFactory.RandomStrategy(this));
            //Strategies.Add(StrategyFactory.RandomStrategy());
            
        }

        public double money { get { return Money; } set { Money = value; } }

        public List<Stock> Stocks { get; set; }

        public void AddStock(Stock stock)
        {
            stocks[InterestedCompanies.IndexOf(stock.company)].Add(stock);
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
                    Final = Final + strat.StrategyOutcome(this, MainPage.exchange);

                foreach(var tp in Final.Results)
                {
                    if (tp.Item2 < 0)
                    {
                        //sell stocks, if any
                        List<Stock> lsstocks = stocks[InterestedCompanies.IndexOf(tp.Item1)];
                        if (stocks[InterestedCompanies.IndexOf(tp.Item1)].Count > 0)
                        {
                            for (int i = 0; i < tp.Item2 * 100 && i < lsstocks.Count; i++)
                            {
                                //sell stocks with float value or until all stocks are sold
                                MainPage.exchange.SellStock(lsstocks[i]);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < tp.Item2 * 100; i++)
                        {
                            //buy stocks here
                            MainPage.exchange.BuyStock(tp.Item1, this);
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
                MainPage.exchange.SellStock(stocks[index][0]);
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
