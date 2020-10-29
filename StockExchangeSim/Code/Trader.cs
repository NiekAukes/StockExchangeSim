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
    public class Trader : IStockOwner
    {
        public static Random rn = new Random(Master.Seed);
        public List<Stock> stocks = new List<Stock>();
        public double Money = 100;
        public double ReactionTime = 200 + rn.NextDouble() * 200;
        public double ActivityTime = 0;

        public void AddStock(Stock stock)
        {
            
        }

        public Stock[] GetStocks()
        {
            return stocks.ToArray();
        }

        public void Update()
        {
            
        }

        public void UpdateHoldings()
        {
            throw new NotImplementedException();
        }
    }
    public class HFTrader : Trader
    {

    }
}
