using System.Collections.Generic;
using System.Linq;

namespace Eco
{
    class TrendMarketWatcher : MarketWatcher<TrendStrategy>
    {
        TrendTool TTool = new TrendTool();
        TrendData TData = null;
        float lastInsightTime = 0;
        int lastdatapoint = 0;
        bool UptrendInvalid = false, DowntrendInvalid = false;
        public TrendMarketWatcher(Company company)
        {
            cp = company;

        }
        public override void RedoInsights()
        {
            TData = TTool.StrategyOutcome(cp);
            if (TData == null)
            {
                //something happened
                throw new System.Exception();
            }
            UptrendInvalid = false;
            DowntrendInvalid = false;
            lastInsightTime = (float)Master.inst.Year;

        }

        public override float UpdateInsights()
        {
            float ret = 0;
            //there is a price update

            //get the new prices
            IEnumerable<StockPriceGraph> NewPrices;
            lock (cp.stockPrices1m)
            {
                NewPrices = cp.stockPrices1m.Skip(lastdatapoint).ToList();
            }
            if (TData == null)
                RedoInsights();


            foreach (var price in NewPrices)
            {
                if (TData.DownTrend != null)
                {
                    if (price.Close > (TData.DownTrend.Multiplier * price.Year + TData.DownTrend.Adder))
                    {
                        //trend invalid
                        DowntrendInvalid = true;
                    }
                }
                else
                {
                    DowntrendInvalid = true;
                }

                if (TData.UpTrend != null)
                {
                    if (price.Close < (TData.UpTrend.Multiplier * price.Year + TData.UpTrend.Adder))
                    {
                        //trend invalid
                        UptrendInvalid = true;
                    }

                }
                else
                {
                    UptrendInvalid = true;
                }
                if (!DowntrendInvalid && !UptrendInvalid)
                {
                    //if there is a float trend
                    // => watch market carefully

                    //buy differences
                    float UpDiff = price.Close - (TData.UpTrend.Multiplier * (float)price.Year + TData.UpTrend.Adder);
                    float DownDiff = (TData.DownTrend.Multiplier * (float)price.Year + TData.DownTrend.Adder) - price.Close;
                    ret += UpDiff - DownDiff;
                }
                else
                {
                    if (!DowntrendInvalid)
                    {
                        //if only downtrend available => sell stocks
                        ret += 15;
                    }
                    if (!UptrendInvalid)
                    {
                        //if only uptrend available => buy stocks
                        ret -= 15;
                    }
                    if (UptrendInvalid && DowntrendInvalid)
                    {
                        //redo insights
                        TData = null;
                        return ret;
                    }
                }
            }

            return ret;
        }
    }
}
