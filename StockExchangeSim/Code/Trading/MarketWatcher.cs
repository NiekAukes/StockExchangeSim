using System;

namespace Eco
{
    public abstract class MarketWatcher<S>
    {
        public Company cp;
        public S Strategy;
        public abstract float UpdateInsights();
        public abstract void RedoInsights();

    }
    public abstract class MarketTool<ToolData>
    {
        Random rn = new Random();
        public static int MinimumPriceCount = 50;
        public abstract ToolData StrategyOutcome(Company cp);
    }

}
