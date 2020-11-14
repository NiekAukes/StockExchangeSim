using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App1;

namespace Eco
{
    public abstract class MarketWatcher<S>
    {
        public Company cp;
        public S Strategy;
        public abstract void UpdateInsights();
        public abstract void RedoInsights();

    }
    public class BreakoutStrategy : Trader.Strategy
    {
        List<BreakoutMarketWatcher> MarketWatchers = new List<BreakoutMarketWatcher>();
        SupportResistanceTool SRTool;
        SupportResistanceData SRData;

        public override void StrategyOutcome(Trader trader, Exchange exchange)
        {
            //TODO
            foreach(var mw in MarketWatchers)
            {
                mw.RedoInsights();
            }
        }
        public override void Observe()
        {
            //Observe the market
        }
    }
    public class BreakoutMarketWatcher : MarketWatcher<BreakoutStrategy>
    {
        SupportResistanceTool SRTool;
        SupportResistanceData SRData;
        float lastInsightTime = 0;
        public override void RedoInsights()
        {
            //TODO

            //Search for Support and Resistance
            SRData = SRTool.StrategyOutcome(cp);

            lastInsightTime = MainPage.Year;

        }

        public override void UpdateInsights()
        {
            //TODO

            //there is a price update

            //get the new 

            //apply Support and Resistance to Breakouts
        }
    }
}
