using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco
{
    class LiquidityMarketWatcher : MarketWatcher<MarketMakingStrategy>
    {
        int BufferTarget = 1;
        float Spread = 0.01f;
        float GeneralPrice = 2;
        Holder Holder { get; set; }

        public LiquidityMarketWatcher(Company cp, Trader td)
        {
            this.cp = cp;
            Holder = new Holder(cp, td);
            Holder.bidask.Bid = cp.stockprice;
            Holder.bidask.Ask = cp.stockprice;
            Master.inst.exchange.RegisterHolder(Holder);
            GeneralPrice = cp.stockprice;
        }
        public override void RedoInsights()
        {
        }

        public override float UpdateInsights()
        {
            GeneralPrice = cp.stockprice;
            int BufferAmps = Holder.Stocks.Count - BufferTarget;
            if (BufferAmps >= 0)
            {
                //too many stocks, higher spread, while increasing price
                Spread *= (0.00002f * BufferAmps + 1);
                GeneralPrice *= (0.00002f * BufferAmps + 1);

            }
            else
            {
                //too few stocks, lower spread, while decreasing price
                Spread /= (0.00002f * -BufferAmps + 1);
                GeneralPrice /= (0.00002f * -BufferAmps + 1);
            }

            

            Holder.bidask.Ask = GeneralPrice -= Spread;
            Holder.bidask.Bid = GeneralPrice += Spread;
            return 0;
        }
    }
}
