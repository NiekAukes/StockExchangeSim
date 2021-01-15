using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco
{
    class LiquidityMarketWatcher : MarketWatcher<MarketMakingStrategy>
    {
        int BufferTarget = 25;
        float Spread = 0.0001f;
        float GeneralPrice = 2;
        Holder Holder { get; set; }

        public LiquidityMarketWatcher(Company cp, Trader td)
        {
            this.cp = cp;
            Holder = new Holder(cp, td);
            GeneralPrice = cp.stockprice;
            UpdateInsights();
        }
        public override void RedoInsights()
        {
        }

        public override float UpdateInsights()
        {
            int BufferAmps = Holder.Stocks.Count - BufferTarget;
            if (BufferAmps >= 0)
            {
                //too many stocks, higher spread
                Spread *= 1.2f;
            }
            else
            {
                //too few stocks, lower spread
                Spread /= 1.2f;
            }

            GeneralPrice = cp.stockprice;

            Holder.bidask.Ask = GeneralPrice -= Spread;
            Holder.bidask.Bid = GeneralPrice += Spread;
            return 0;
        }
    }
}
