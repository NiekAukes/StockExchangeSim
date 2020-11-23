using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App1;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Eco
{
    public abstract class MarketWatcher<S>
    {
        public Company cp;
        public S Strategy;
        public abstract float UpdateInsights();
        public abstract void RedoInsights();

    }
    public class BreakoutStrategy : Trader.Strategy
    {
        List<BreakoutMarketWatcher> MarketWatchers = new List<BreakoutMarketWatcher>();
        SupportResistanceTool SRTool;
        SupportResistanceData SRData;

        public override Trader.MarketResults StrategyOutcome(Trader trader, ExchangeBroker exchange)
        {
            //TODO
            Trader.MarketResults MR = new Trader.MarketResults();
            for(int i = 0; i < MarketWatchers.Count; i++)
            {
                MR.Results.Add(new Tuple<Company, float>(MarketWatchers[i].cp, MarketWatchers[i].UpdateInsights()));
            }
            return MR;
        }
        public override void Observe()
        {
            //Observe the market
        }
    }
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

            lastInsightTime = MainPage.Year;
            

        }
        int LastRememberUp = 0, LastRememberDown = 0; 
        bool LastPotentialBreakoutUp = false, LastPotentialBreakoutDown = false;
        public override float UpdateInsights()
        {
            float ret = 0;
            //there is a price update

            //get the new prices
            List<StockPriceGraph> NewPrices = new List<StockPriceGraph>(cp.stockPrices.Skip(lastdatapoint));
            //NewPrices.AddRange(cp.stockPrices.getrange); 
            //cp.stockPrices.CopyTo(NewPrices, lastdatapoint);
            
            MainPage.inst.AddContinuousline(SRData.MainSupport, new SolidColorBrush(Colors.LightGreen));
            MainPage.inst.AddContinuousline(SRData.MainResistance, new SolidColorBrush(Colors.Red));

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
                            MainPage.inst.AddVerticalLine(price.Year);
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
                            MainPage.inst.AddVerticalLine(price.Year, true);
                            ret -= 5;
                        }
                        else
                            potentialBreakoutDown = true; //wait for confirmation
                    }
                }

            }
            if (MainPage.Year - lastInsightTime > 0.2)
                SRData = null;
            if ((potentialBreakoutDown) || (potentialBreakoutUp))
                SRData = null;

            if (SRData == null)
                RedoInsights();

            lastdatapoint = cp.stockPrices.Count > 20 ? 20 : cp.stockPrices.Count;
            return ret;
            //apply Support and Resistance to Breakouts
        }
    }
}
