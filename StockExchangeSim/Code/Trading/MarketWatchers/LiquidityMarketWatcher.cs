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

        //LIQUIDITY TARGET
        int LiquidityTarget = Master.fCustomLiquidityTarget ? Master.CustomLiqTarget : 189 ;

        float buyliquidity = 0;
        float sellliquidity = 0;
        public List<float> averagebuyliquidity = new List<float>();
        public List<float> averagesellliquidity = new List<float>();

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

            for (int i = 0; i < liqcount / Strategy.ActionTimeDeduction; i++ )
            {
                averagebuyliquidity.Add(0);
            }
        }

        private void Holder_StockTraded(object sender, StockTradedEventArgs e)
        {
            if (e.IsSell)
            {
                sellliquidity += (1.0f / Master.inst.Traders.Count) * (60.0f / Strategy.ActionTimeDeduction);// * (1.0f / Master.inst.SecondsPerTick);
            }
            else
            {
                buyliquidity += (1.0f / Master.inst.Traders.Count) * (60.0f / Strategy.ActionTimeDeduction);// * (1.0f / Master.inst.SecondsPerTick);
            }
        }

        public override void RedoInsights()
        {
        }

        int liqcount = 240;
        public override float UpdateInsights()
        {
            int buyorders = 0, sellorders = 0;
            double totalbuyvalue = 0, totalsellvalue = 0;
            BuyOrder highestbuyorder = null;
            SellOrder lowestsellorder = null;

            if (averagebuyliquidity.Count > liqcount/Strategy.ActionTimeDeduction)
            {
                //remove first element
                averagebuyliquidity.RemoveAt(0);
            }
            averagebuyliquidity.Add(buyliquidity);
            if (averagesellliquidity.Count > liqcount / Strategy.ActionTimeDeduction)
            {
                //remove first element
                averagesellliquidity.RemoveAt(0);
            }
            averagesellliquidity.Add(sellliquidity);
            //#warning this is a bodge too!
            #region Price Discovery
            //kijk naar vraag en aanbod
            for (int i = 0; i < cp.BuyOrders.Count; i++)
            {
                try
                {
                    buyorders += cp.BuyOrders[i].Amount;
                    totalbuyvalue += (double)cp.BuyOrders[i].LimitPrice * cp.BuyOrders[i].Amount;
                    if (i != 0)
                    {
                        if (cp.BuyOrders[i].LimitPrice > highestbuyorder.LimitPrice)
                        {
                            highestbuyorder = cp.BuyOrders[i];
                        }
                    }
                    else
                        highestbuyorder = cp.BuyOrders[i];
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            for (int i = 0; i < cp.SellOrders.Count; i++)
            {
                try
                {
                    sellorders += cp.SellOrders[i].Amount;
                    totalsellvalue += (double)cp.SellOrders[i].LimitPrice * cp.SellOrders[i].Amount;

                    if (i != 0)
                    {
                        if (cp.SellOrders[i].LimitPrice > lowestsellorder.LimitPrice)
                        {
                            lowestsellorder = cp.SellOrders[i];
                        }
                    }
                    else
                        lowestsellorder = cp.SellOrders[i];
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            #endregion
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
            //GeneralPrice = Holder.bidask.Bid - Spread;
            GeneralPrice = cp.stockprice;
            float demandsurplus = buyorders - sellorders;

            float avgliq = averagebuyliquidity.Average();

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

            //    float Liquiditysurplus = avgliq - LiquidityTarget;
            //float pricemodifier = MathF.Sqrt(MathF.Abs(Liquiditysurplus > LiquidityTarget ?
            //    Liquiditysurplus * Strategy.ActionTimeDeduction / 100.0f :
            //    LiquidityTarget * Strategy.ActionTimeDeduction / 100.0f)) * Strategy.ActionTimeDeduction / 200.0f + 1; 
                //too much stocks traded, look at demand
                if (demandsurplus < 0)
                {
                    //decrease price
                    GeneralPrice /= 0.005f * Strategy.ActionTimeDeduction + 1;
                }
                else
                {
                    //decrease price
                    GeneralPrice *= 0.005f * Strategy.ActionTimeDeduction + 1;
                }
            
            //another method 


            Spread = 0.01f * cp.Value * cp.StockPart;

            Holder.bidask.Ask = GeneralPrice - Spread;
            Holder.bidask.Bid = GeneralPrice + Spread;

            if (Master.rn.NextDouble() < 0.05)
                Debug.WriteLine(Strategy.trader.name + ", liquidity: " + buyliquidity);
            //liquidity /= (1.5f/Strategy.ActionTimeDeduction);
            ////liquidity /= 1.5f;
            //if (liquidity < 1)
                buyliquidity = 0;
            return 0;
        }

    }
}
