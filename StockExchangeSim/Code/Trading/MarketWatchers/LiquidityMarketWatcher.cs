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
            liquidity += (1 / Master.inst.Traders.Count) * (1/Master.inst.SecondsPerTick);
        }

        public override void RedoInsights()
        {
        }

        public override float UpdateInsights()
        {
            int buyorders = 0, sellorders = 0;
            double totalbuyvalue = 0, totalsellvalue = 0;
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
                    totalbuyvalue += (double)cp.BuyOrders[i].LimitPrice * cp.BuyOrders[i].Amount;
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
                    sellorders += cp.SellOrders[i].Amount;
                    totalsellvalue += (double)cp.SellOrders[i].LimitPrice * cp.SellOrders[i].Amount;

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
                GeneralPrice = (highestorder.LimitPrice);
                float demandsurplus = buyorders - sellorders;

                double DemandElasticity =((totalbuyvalue / buyorders) - highestorder.LimitPrice) /
                    (buyorders / 2.0);
                double SupplyElasticity = ((totalsellvalue / sellorders) - lowestbidder.LimitPrice) /
                    (sellorders / 2.0);

                if (demandsurplus > (30 * Master.inst.TraderAmount) && (Holder.Stocks.Count < 50))
                {
                    //there needs to be more stocks
                    List<Stock> stocks = cp.TradeStocks(demandsurplus / 100, Strategy.trader);
                    lock (Holder.Stocks.SyncRoot)
                    {
                        foreach (Stock st in stocks)
                        {
                            Holder.Stocks.Add(st);
                        }
                    }

                    //decrease price
                    GeneralPrice += (float)(1 / DemandElasticity) * 2.5f * MathF.Log(demandsurplus);
                }
                else if (demandsurplus > (10 * Master.inst.TraderAmount))
                {
                    //significantly increase prices
                    GeneralPrice += (float)(-DemandElasticity) * 1.5f * MathF.Log(demandsurplus);

                }
                else if (demandsurplus > 0)
                {
                    //price can be slightly increased
                    GeneralPrice += (float)(-DemandElasticity) * 0.5f * MathF.Log(demandsurplus);

                }
                else
                {
                    //price should be decreased
                    GeneralPrice += (float)(-SupplyElasticity) * 0.5f * MathF.Log(demandsurplus);

                }

                float Liquiditysurplus = liquidity - LiquidityTarget;
                if (Liquiditysurplus > 0)
                {
                    //too much stocks traded, look at demand
                    if (demandsurplus > 0)
                    {
                        //increase price
                        GeneralPrice *= 1.002f;
                    }
                    else
                    {
                        //lower price
                        GeneralPrice /= 1.002f;

                    }
                }
                else
                {
                    //too few stocks traded, look at demand
                    if (demandsurplus > 0)
                    {
                        //lower price
                        GeneralPrice /= 1.02f;

                    }
                    else
                    {
                        //increase price
                        GeneralPrice *= 1.02f;

                    }
                }

            }

            Spread = 0.01f * cp.Value * cp.StockPart;

            Holder.bidask.Ask = GeneralPrice -= Spread;
            Holder.bidask.Bid = GeneralPrice += Spread;

            if (Master.rn.NextDouble() < 0.05)
                Debug.WriteLine(Strategy.trader.name + ", liquidity: " + liquidity);
            liquidity = 0;
            return 0;
        }

    }
}
