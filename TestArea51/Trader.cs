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
        void AddStock(Stock stock);
        void UpdateHoldings();
        float money { get; set; }
    }
    public partial class Trader : IStockOwner
    {
        public static Random rn = new Random();
        public Dictionary<Stock, float> stocks = new Dictionary<Stock, float>();
        public float Money = 100;
        public float BaseActionTimeRequired = 20 + (float)rn.NextDouble() * 20; //in seconds
        public float ActivityTime = 0;
        public float ActionTime; //in seconds

        Strategy strat = new PatternStrategy();

        public Trader()
        {
            Stocks = new List<Stock>();
        }

        public float money { get { return Money; } set { Money = value; } }

        public List<Stock> Stocks { get; set; }

        public void AddStock(Stock stock)
        {
            if (!stocks.ContainsKey(stock))
                stocks.Add(stock, stock.value);
            stock.Owner = this;
        }

        public Stock[] GetStocks()
        {
            return stocks.Keys.ToArray();
        }

        public virtual void Update()
        {
            //ActionTime += MainPage.master.SecondsPerTick; //accrued Time for actions
            Stock[] stockArr = stocks.Keys.ToArray();
            for (int i = 0; i < stockArr.Length; i++)
            {
                Money += stockArr[i].Collect();
            }
            //if (ActionTime > 0)
                strat.StrategyOutcome(this, Program.exchange);


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
