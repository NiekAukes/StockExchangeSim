using System.Collections.Generic;
using System.Linq;

namespace Eco
{
    public class BreakoutMarketWatcher : MarketWatcher<BreakoutStrategy>
    {
        SupportResistanceTool SRTool = new SupportResistanceTool();
        SupportResistanceData SRData = null;
        float lastInsightTime = 0;
        int lastdatapoint = 0;

        public BreakoutMarketWatcher(Company company)
        {
            cp = company;
        }

        public override void RedoInsights()
        {
            //TODO

            //Search for Support and Resistance
            SRData = SRTool.StrategyOutcome(cp);

            lastInsightTime = Master.inst.Year;


        }
        int LastRememberUp = 0, LastRememberDown = 0;
        bool LastPotentialBreakoutUp = false, LastPotentialBreakoutDown = false;
        public override float UpdateInsights()
        {
            float ret = 0;
            //there is a price update

            //get the new prices
            IEnumerable<StockPriceGraph> nwe = cp.stockPrices1m.Skip(lastdatapoint);
            SynchronizedCollection<StockPriceGraph> NewPrices = new SynchronizedCollection<StockPriceGraph>(nwe);
            //NewPrices.AddRange(cp.stockPrices.getrange); 
            //cp.stockPrices.CopyTo(NewPrices, lastdatapoint);

            if (SRData == null)
                RedoInsights();


            bool potentialBreakoutUp = false;
            bool potentialBreakoutDown = false;

            foreach (var price in NewPrices)
            {
                if (SRData.MainResistance != null)
                {
                    if (price.Close > (SRData.MainResistance.Multiplier * price.Year + SRData.MainResistance.Adder))
                    {
                        //potential breakout
                        if (potentialBreakoutUp || LastPotentialBreakoutUp)
                        {
                            //confirmation, buy stocks
                            ret += 5;

                        }
                        else
                            potentialBreakoutUp = true; //wait for confirmation
                    }
                }
                if (SRData.MainSupport != null)
                {
                    if (price.Close < SRData.MainSupport.Multiplier * price.Year + SRData.MainSupport.Adder)
                    {
                        //potential breakout
                        if (potentialBreakoutDown || LastPotentialBreakoutDown)
                        {
                            //confirmation, sell stocks
                            ret -= 5;
                        }
                        else
                            potentialBreakoutDown = true; //wait for confirmation
                    }
                }

            }
            if (Master.inst.Year - lastInsightTime > 0.2)
                SRData = null;
            if ((potentialBreakoutDown) || (potentialBreakoutUp))
                SRData = null;


            lastdatapoint = cp.stockPrices1m.Count > 20 ? 20 : cp.stockPrices1m.Count;
            return ret;
            //apply Support and Resistance to Breakouts
        }
    }
}
