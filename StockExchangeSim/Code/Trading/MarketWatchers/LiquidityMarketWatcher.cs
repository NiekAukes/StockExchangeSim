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

        int LiquidityTarget = 50;

        Holder Holder { get; set; }
        StockPriceComparisonTool SPCTool = new StockPriceComparisonTool();

        public LiquidityMarketWatcher(MarketMakingStrategy strat, Company cp, Trader td) : base(strat)
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
            
            var lowestorder = cp.SellOrders.list.Count > 0 ? cp.SellOrders.list[0] : null;
            var highestbidder = cp.BuyOrders.list.Count > 0 ? cp.BuyOrders.list[0] : null;

            if (lowestorder == null || highestbidder == null)
            {
                //if there is no bid or ask yet, construct one
                var data = SPCTool.StrategyOutcome(cp);
                GeneralPrice = data.ExpectedStockPrice;
            }
            else
            {
                GeneralPrice = (lowestorder.LimitPrice + highestbidder.LimitPrice) / 2.0f;
            }

            Spread = 0.001f * cp.Value * cp.StockPart;
            //GeneralPrice = cp.stockprice;
            //int BufferAmps = Holder.Stocks.Count - BufferTarget;
            //if (BufferAmps >= 0)
            //{
            //    //too many stocks, higher spread, while increasing price
            //    Spread *= (0.00002f * BufferAmps + 1);
            //    GeneralPrice *= (0.00002f * BufferAmps + 1);

            //}
            //else
            //{
            //    //too few stocks, lower spread, while decreasing price
            //    Spread /= (0.00002f * -BufferAmps + 1);
            //    GeneralPrice /= (0.00002f * -BufferAmps + 1);
            //}



            Holder.bidask.Ask = GeneralPrice -= Spread;
            Holder.bidask.Bid = GeneralPrice += Spread;
            return 0;
        }
    }
}
