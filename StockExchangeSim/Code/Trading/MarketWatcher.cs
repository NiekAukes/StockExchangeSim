using System;
using System.Collections.Generic;

namespace Eco
{
    public abstract class MarketWatcher<S>
    {
        public enum DifferentmarketWatchers
        {

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
        public MarketWatcher(S strat)
        {
            Strategy = strat;
        }


        public abstract float UpdateInsights();
        public virtual void RedoInsights()
        {
            //UpdateTraderThoughts();
        }
        protected float BackupStrat()
        {
            float ret = 0;
            //trader needs to see if stock is under or overvalued => value to price
            if (cp.Value * cp.StockPart > cp.stockprice)
            {
                //stock is generally underpriced
                ret += 1;
            }
            else
            {
                //stock is overpriced
                ret -= 1;
            }

            //trader needs to see risk and reward => input vs expected output
            //this might actually already be implemented

            //trader needs to see small trends => competitive position
            ret += MathF.Log(cp.CompetitivePosition) * 2;

            return ret;
        }
    }
    public abstract class MarketTool<ToolData>
    {
        Random rn = new Random();
        public static int MinimumPriceCount = 50;
        public abstract ToolData StrategyOutcome(Company cp);
        
    }

}

