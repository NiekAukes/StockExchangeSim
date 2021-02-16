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

        int LiquidityTarget = 50;
        float liquidity = 0;
        public List<float> averageliquidity = new List<float>();

        Holder Holder { get; set; }
        StockPriceComparisonTool SPCTool = new StockPriceComparisonTool();

        public LiquidityMarketWatcher(MarketMakingStrategy strat, Company cp, Trader td) : base(strat)
        {
            this.cp = cp;
            Holder = new Holder(cp, td);
            Holder.Stocks = new SynchronizedCollection<Stock>(new bool(), cp.TradeStocks((float)Master.rn.NextDouble() * 0.1f, td, true));
            Holder.bidask.Bid = cp.stockprice;
            Holder.bidask.Ask = cp.stockprice;
            Master.inst.exchange.RegisterHolder(Holder);
            GeneralPrice = cp.stockprice;

            Holder.StockTraded += Holder_StockTraded;
        }

        private void Holder_StockTraded(object sender, EventArgs e)
        {
            liquidity += (1.0f / Master.inst.Traders.Count) * (60.0f/Strategy.ActionTimeDeduction) * (1.0f / Master.inst.SecondsPerTick);
        }

        public override void RedoInsights()
        {
        }

        public override float UpdateInsights()
        {
            int buyorders = 0, sellorders = 0;
            double totalbuyvalue = 0, totalsellvalue = 0;
            BuyOrder highestbuyorder = null;
            SellOrder lowestsellorder = null;

            if (averageliquidity.Count > 240/Strategy.ActionTimeDeduction)
            {
                //remove first element
                averageliquidity.RemoveAt(0);
            }
            averageliquidity.Add(liquidity);
//#warning this is a bodge too!
//            #region Price Discovery
//            //kijk naar vraag en aanbod
//            for (int i = 0; i < cp.BuyOrders.Count; i++)
//            {
//                try
//                {
//                    buyorders += cp.BuyOrders[i].Amount;
//                    totalbuyvalue += (double)cp.BuyOrders[i].LimitPrice * cp.BuyOrders[i].Amount;
//                    if (i != 0)
//                    {
//                        if (cp.BuyOrders[i].LimitPrice > highestbuyorder.LimitPrice)
//                        {
//                            highestbuyorder = cp.BuyOrders[i];
//                        }
//                    }
//                    else
//                        highestbuyorder = cp.BuyOrders[i];
//                }
//                catch (Exception e)
//                {
//                    Debug.WriteLine(e.Message);
//                }
//            }
//            for (int i = 0; i < cp.SellOrders.Count; i++)
//            {
//                try
//                {
//                    sellorders += cp.SellOrders[i].Amount;
//                    totalsellvalue += (double)cp.SellOrders[i].LimitPrice * cp.SellOrders[i].Amount;

//                    if (i != 0)
//                    {
//                        if (cp.SellOrders[i].LimitPrice > lowestsellorder.LimitPrice)
//                        {
//                            lowestsellorder = cp.SellOrders[i];
//                        }
//                    }
//                    else
//                        lowestsellorder = cp.SellOrders[i];
//                }
//                catch (Exception e)
//                {
//                    Debug.WriteLine(e.Message);
//                }
//            }
//            #endregion
            //kijk naar hoogste aanbod en laagste vraag


            //if (highestbuyorder == null)
            //{
            //    //if there is no bid or ask yet, construct one
            //    var data = SPCTool.StrategyOutcome(cp);
            //    GeneralPrice = data.ExpectedStockPrice;
            //}
            //else
            //{
            //    GeneralPrice = (highestbuyorder.LimitPrice);
            //}
            GeneralPrice = Holder.bidask.Bid - Spread;
            float demandsurplus = buyorders - sellorders;

            float avgliq = averageliquidity.Average();

            if (demandsurplus > (10 * Master.inst.TraderAmount) && (Holder.Stocks.Count < 50))
            {
                //there needs to be more stocks
                List<Stock> stocks = cp.TradeStocks(demandsurplus * cp.StockPart, Strategy.trader);
                lock (Holder.Stocks.SyncRoot)
                {
                    foreach (Stock st in stocks)
                    {
                        Holder.Stocks.Add(st);
                    }
                }
            }

                float Liquiditysurplus = avgliq - LiquidityTarget;
            float pricemodifier = MathF.Sqrt(MathF.Abs(Liquiditysurplus > LiquidityTarget ? Liquiditysurplus : LiquidityTarget)) / 20 + 1; 
            if (Liquiditysurplus > 0)
            {
                //too much stocks traded, look at demand
                if (demandsurplus >= 0)
                {
                    //increase price
                    GeneralPrice *= pricemodifier;
                }
                else
                {
                    //lower price
                    GeneralPrice /= pricemodifier;
                }
            }
            else
            {
                //too few stocks traded, look at demand
                if (demandsurplus >= 0)
                {
                    //lower price
                    GeneralPrice /= pricemodifier;

                }
                else
                {
                    //increase price
                    GeneralPrice *= pricemodifier;

                }
            }
            



            Spread = 0.01f * cp.Value * cp.StockPart;

            Holder.bidask.Ask = GeneralPrice - Spread;
            Holder.bidask.Bid = GeneralPrice + Spread;

            if (Master.rn.NextDouble() < 0.05)
                Debug.WriteLine(Strategy.trader.name + ", liquidity: " + liquidity);
            //liquidity /= (1.5f/Strategy.ActionTimeDeduction);
            ////liquidity /= 1.5f;
            //if (liquidity < 1)
                liquidity = 1;
            return 0;
        }

    }
}
