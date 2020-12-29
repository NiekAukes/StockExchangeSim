using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco
{
    class LiquidityMarketWatcher : MarketWatcher<MarketMakingStrategy>
    {
        LiquidityTool LT = new LiquidityTool();
        LiquidityToolData LTD = null;
        float lastInsightTime = 0;
        int lastdatapoint = 0;

        public LiquidityMarketWatcher(MarketMakingStrategy strat) : base(strat)
        {

        }

        public override void RedoInsights()
        {
            LTD = LT.StrategyOutcome(cp);
            OnRedoneInsights(null);
            lastInsightTime = Master.inst.Year;

            UpdateTraderThoughts();

        }

        public override float UpdateInsights()
        {
            if (LTD == null)
            {
                RedoInsights();
            }
            SynchronizedCollection<Liquidity> slq =
                new SynchronizedCollection<Liquidity>(cp.BidAsk.liquidity1m.Skip(lastdatapoint));
            Liquidity liquidity = slq[0];
            for(int i = 1; i < slq.Count; i++)
            {
                liquidity += slq[i];
            }
            lastdatapoint = cp.stockPrices1m.Count > 20 ? 20 : cp.stockPrices1m.Count;
            LTD = null;
            return liquidity.Diff;
        }

        public override void UpdateTraderThoughts()
        {
            throw new NotImplementedException();
        }
    }
}
