using System;
using System.Collections.Generic;
using Windows.UI;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App1;
using Windows.UI.Xaml.Media;

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

            UptrendInvalid = false;
            DowntrendInvalid = false;
            lastInsightTime = MainPage.Year;

        }

        public override float UpdateInsights()
        {
            float ret = 0;
            //there is a price update

            //get the new prices
            List<StockPriceGraph> NewPrices = new List<StockPriceGraph>(cp.stockPrices.Skip(lastdatapoint));

            if (TData == null)
                RedoInsights();

            //bool UptrendInvalid = false, DowntrendInvalid = false;

            MainPage.inst.AddContinuousline(TData.DownTrend, new SolidColorBrush(Colors.Magenta));
            MainPage.inst.AddContinuousline(TData.UpTrend, new SolidColorBrush(Colors.Blue));

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
                    //if there is a double trend
                    // => watch market carefully
                }
                else
                {
                    if (!DowntrendInvalid)
                    {
                        //if only downtrend available => sell stocks
                    }
                    if (!UptrendInvalid)
                    {
                        //if only uptrend available => buy stocks
                    }
                }
            }

                throw new NotImplementedException();
        }
    }
}
