using System;

namespace Eco
{
    public abstract class MarketWatcher<S>
    {
        public Company cp;
        public S Strategy;

        public event EventHandler RedoneInsights;
        protected virtual void OnRedoneInsights(EventArgs e)
        {
            EventHandler handler = RedoneInsights;
            handler?.Invoke(this, e);
        }
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
