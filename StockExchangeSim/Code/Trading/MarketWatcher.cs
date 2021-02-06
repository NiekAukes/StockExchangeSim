using System;
using System.Collections.Generic;

namespace Eco
{
    public abstract class MarketWatcher<S>
    {
        public enum DifferentmarketWatchers {

        }
        //public object strat { get; set; }
        public Company cp;
        public S Strategy;
        public float latestInsightResult;

        public event EventHandler RedoneInsights;
        protected virtual void OnRedoneInsights(EventArgs e)
        {

            EventHandler handler = RedoneInsights;
            handler?.Invoke(this, e);
        }
        public MarketWatcher(S strat){
            Strategy = strat;
        }

        
        public abstract float UpdateInsights();
        public virtual void RedoInsights()
        {
            //UpdateTraderThoughts();
        }
         
        public abstract void UpdateTraderThoughts();
            //this must activate something in the traders that have the company of this marketwatcher on their InterestedCompanies list.
            //check, for each trader, if one of the interested companies of that trader...
            //matches with the market watcher's company, and update accordingly
    }
    public abstract class MarketTool<ToolData>
    {
        Random rn = new Random();
        public static int MinimumPriceCount = 50;
        public abstract ToolData StrategyOutcome(Company cp);
    }

}
