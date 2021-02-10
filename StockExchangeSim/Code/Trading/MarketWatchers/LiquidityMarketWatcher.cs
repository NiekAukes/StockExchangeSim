using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        int LiquidityTarget = 500;
        float liquidity = 0;

        Holder Holder { get; set; }
        StockPriceComparisonTool SPCTool = new StockPriceComparisonTool();

        public LiquidityMarketWatcher(MarketMakingStrategy strat, Company cp, Trader td) : base(strat)
        {
            this.cp = cp;
            Holder = new Holder(cp, td);
            Holder.Stocks = new SynchronizedCollection<Stock>( new bool(), cp.TradeStocks((float)Master.rn.NextDouble() * 0.1f, td, true));
            Holder.bidask.Bid = cp.stockprice;
            Holder.bidask.Ask = cp.stockprice;
            Master.inst.exchange.RegisterHolder(Holder);
            GeneralPrice = cp.stockprice;

            Holder.StockTraded += Holder_StockTraded;
        }

        private void Holder_StockTraded(object sender, EventArgs e)
        {
            liquidity += (LiquidityTarget / Master.inst.Traders.Count) * Master.inst.SecondsPerTick;
        }

        public override void RedoInsights()
        {
        }

        public override float UpdateInsights()
        {
            int buyorders = 0, sellorders = 0;
            BuyOrder lowestbidder = null;
            SellOrder highestorder = null;
            
            #warning this is a bodge too!
            #region Price Discovery
            //kijk naar vraag en aanbod
            for (int i = 0; i < cp.BuyOrders.Count; i++)
            {
                try
                {
                    buyorders += cp.BuyOrders[i].Amount;
                    if (i != 0)
                    {
                        if (cp.BuyOrders[i].LimitPrice > lowestbidder.LimitPrice)
                        {
                            lowestbidder = cp.BuyOrders[i];
                        }
                    }
                    else
                        lowestbidder = cp.BuyOrders[i];
                }
                catch(Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            for (int i = 0; i < cp.SellOrders.Count; i++)
            {
                try
                {
                    buyorders += cp.SellOrders[i].Amount;
                    if (i != 0)
                    {
                        if (cp.SellOrders[i].LimitPrice > highestorder.LimitPrice)
                        {
                            highestorder = cp.SellOrders[i];
                        }
                    }
                    else
                        highestorder = cp.SellOrders[i];
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            #endregion
            //kijk naar hoogste aanbod en laagste vraag


            if (highestorder == null || lowestbidder == null)
            {
                //if there is no bid or ask yet, construct one
                var data = SPCTool.StrategyOutcome(cp);
                GeneralPrice = data.ExpectedStockPrice;
            }
            else
            {
                GeneralPrice = (highestorder.LimitPrice + lowestbidder.LimitPrice) / 2.0f;
            }

            Spread = 0.001f * cp.Value * cp.StockPart;

            Holder.bidask.Ask = GeneralPrice -= Spread;
            Holder.bidask.Bid = GeneralPrice += Spread;

            if (Master.rn.NextDouble() < 0.05)
                Debug.WriteLine(Strategy.trader.name + ", liquidity: " + liquidity);
            liquidity = 0;
            return 0;
        }

    }
}
