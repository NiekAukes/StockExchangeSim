using System;
using System.Collections.Generic;
using System.Linq;
 
namespace Eco
{
    public class BreakoutMarketWatcher : MarketWatcher<BreakoutStrategy>
    {
        public SupportResistanceTool SRTool = new SupportResistanceTool();
        public SupportResistanceData SRData = null;
        float lastInsightTime = 0;
        int lastdatapoint = 0;


        public BreakoutMarketWatcher(BreakoutStrategy strat, Company company) : base(strat)
        {
            cp = company;
        }

        public override void RedoInsights()
        {
            //Search for Support and Resistance
            SRData = SRTool.StrategyOutcome(cp);

            lastInsightTime = (float)Master.inst.Year;

            OnRedoneInsights(null);

        }
        int LastRememberUp = 0, LastRememberDown = 0;
        bool LastPotentialBreakoutUp = false, LastPotentialBreakoutDown = false;
        public override float UpdateInsights()
        {
            float ret = 0;
            //there is a price update

            //get the new prices
            List<StockPriceGraph> NewPrices;
            lock (cp.stockPrices1m)
            {
                NewPrices = new List<StockPriceGraph>(cp.stockPrices1m.Skip(cp.stockPrices1m.Count - 20));
            }
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
            if (Master.inst.Year - lastInsightTime > 0.2 / (365.25f * 24))
                SRData = null;
            if ((potentialBreakoutDown) || (potentialBreakoutUp))
                SRData = null;


            lastdatapoint = cp.stockPrices1m.Count - (cp.stockPrices1m.Count > 20 ? 20 : 0);

            if (ret == 0)
            {
                //fallback strategy
                //if stocks fall, sell
                //otherwise buy
                int n = cp.stockPrices1m.Count - 10 < 800 ? cp.stockPrices1m.Count - 10 : 800;
                if (cp.stockPrices1m[10].High > cp.stockPrices1m[n].High)
                {
                    //falling stock
                    ret = 2.5f;
                }
                else
                {
                    //rising stock
                    ret = 2.5f;
                }
            }
            return ret;
            //apply Support and Resistance to Breakouts
        }

    }
}
