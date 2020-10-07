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
    public class Trader
    {
        public List<Company.Stock> stocks = new List<Company.Stock>();
        
        public void Update()
        {
            
        }
    }
    public class HFTrader : Trader
    {

    }
}
